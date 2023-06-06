import rpyc
from rpyc.utils.server import ThreadedServer
import numpy as np
from typing import Dict
from threading import Thread
import queue
import cv2
from .filters import highPassFilter, whitePointSelect, blackPointSelect
from .combined import infer_letters, map2d, merge_boxes, infer_words, groupbyrow

import cv2
import numpy as np
from PIL import Image
from deskew import determine_skew
from jdeskew.utility import rotate
from ultralytics.yolo.utils.plotting import Annotator
from functools import reduce
from io import BytesIO
import pickle

server = None

def infer_image(img_array):
    img = cv2.imdecode(img_array, cv2.IMREAD_COLOR)
    img = cv2.cvtColor(img, cv2.COLOR_RGB2BGR)
    img = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
    img = highPassFilter(img)
    img = whitePointSelect(img)
    img = blackPointSelect(img)
    _, img = cv2.threshold(img, 0, 255, cv2.THRESH_BINARY + cv2.THRESH_OTSU)
    img = rotate(img, determine_skew(img),
                 border_mode=cv2.BORDER_CONSTANT, border_value=255)
    img = cv2.cvtColor(img, cv2.COLOR_GRAY2RGB)
    pil_img_before_inference = Image.fromarray(img)
    word_boxes = infer_words(img)
    box_bounds = [box[1].xyxy[0].cpu().data.numpy() for box in word_boxes]
    box_bounds = [
        b + np.multiply(np.array([b[0] - b[2], b[1] - b[3], b[2] - b[0], b[3] - b[1]]), [0.05, 0.2, 0.05, 0.05]) for
        b in box_bounds]  # pad the boxes
    box_bounds = [np.array([max(0, b[0]), max(0, b[1]), min(img.shape[1] - 1, b[2]), min(img.shape[0] - 1, b[3])])
                  for b in box_bounds]  # make sure padding doesn't spill out of image
    box_bounds = reduce(lambda x, y: np.vstack((x, y)), box_bounds)
    rows_of_boxes = groupbyrow(box_bounds)
    rows_of_boxes = [merge_boxes(row) for row in rows_of_boxes]

    rows_of_word_imgs = map2d(lambda box: np.array(
        pil_img_before_inference.crop(box)), rows_of_boxes)

    rows_of_word_texts = map2d(lambda x: infer_letters(
        x, debug=False, conf=0.3, iou=0.5, agnostic_nms=True), rows_of_word_imgs)
    final_rows = [" ".join(reversed(row)) for row in rows_of_word_texts]
    return "\n".join(final_rows)


def infer(images, callback):
    response = {}
    for img, id in zip(images, range(len(images))):
        response[id] = infer_image(img)
    print(response)
    buffer = BytesIO()
    pickle.dump(response, buffer)
    return callback(buffer.getvalue())


@rpyc.service
class WordInference(rpyc.Service):
    active = False
    thread = None
    workq = queue.Queue()
    # TODO: we will need to limit the number of concurrent model inferences

    @rpyc.exposed
    def register_task(self, data: list, callback):
        if not self.active:
            self.active = True
            self.thread = Thread(target=self.__inference)
            self.thread.start()
        deseralized = pickle.loads(data)
        self.workq.put((deseralized, rpyc.async_(callback)))
        return True

    def __inference(self):
        while self.active:
            data, callback = self.workq.get()
            infer(data, callback)

def StartServer():
    global server
    print('[INFO] Starting Inference Server')
    server = ThreadedServer(WordInference, port=18811)
    server.start()
