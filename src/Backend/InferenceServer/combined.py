import json
import requests
import numpy as np
import cv2
from more_itertools import split_when
from ultralytics import YOLO
import urllib3

urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)
global_session = requests.Session()

def infer_words(word_model: YOLO, img: np.ndarray):
    # returns List[(word_image, bb, class)]
    results = word_model.predict(img, verbose=False)
    return [box.xyxy[0].tolist() for r in results for box in r.boxes]


def infer_letters(img: np.ndarray):
    global global_session
    while True:
        # TODO: Remove this hard-coded key.
        api_key = 'IpLcbz1k0SREcuoNEGah9aRPuu7zXaLS'
        if not api_key:
            raise Exception("A key should be provided to invoke the endpoint")


        headers = { 'Authorization':('Bearer '+ api_key) }
        _, encoded = cv2.imencode('.jpg', img)
        files = { 'image': ('image', encoded) }

        try:
            r = global_session.post('https://ocrmlwokspace-jwzyl.eastus2.inference.ml.azure.com/score', headers=headers, files=files, verify=False)
            if r.status_code == 429:
                continue
            elif r.status_code == 200:
                return json.loads(r.text)["predict"]
            else:
                return r.text
        except requests.exceptions.ConnectionError:
            global_session = requests.Session()


def get_y_center(b):
    x1, y1, x2, y2 = b[1].xyxy[0].tolist()
    return (y1 + y2) / 2


def groupbyrow(boxes):
    def get_y_center(b):
        x1, y1, x2, y2 = b
        return (y1 + y2) / 2

    def not_vertically_overlapping(b1, b2):
        _, up1, _, down1 = b1
        _, up2, _, down2 = b2
        return down1 < up2 or (down1 - up2) < (up2 - up1)

    sorted_boxes = sorted(boxes, key=get_y_center)
    return list(split_when(sorted_boxes, not_vertically_overlapping))


def merge_boxes(boxes, iou_thresh=0.3):
    boxes = sorted(boxes, key=lambda b: b[0])

    if len(boxes) == 0:
        return []

    merged_boxes = [boxes[0]]

    for k in range(1, len(boxes)):
        prev_box = merged_boxes[-1]

        x1 = boxes[k][0]
        y1 = boxes[k][1]
        x2 = boxes[k][2]
        y2 = boxes[k][3]
        x1_other = prev_box[0]
        y1_other = prev_box[1]
        x2_other = prev_box[2]
        y2_other = prev_box[3]

        intersection_area = 0 if x2_other < x1 else (
            x2_other - x1) * (max(y2, y2_other) - max(y1, y1_other))
        area = (x2 - x1) * (y2 - y1)
        area_other = (x2_other - x1_other) * (y2_other - y1_other)
        union_area = area + area_other - intersection_area
        if intersection_area / union_area > iou_thresh or intersection_area / area > 0.7 or intersection_area / area_other > 0.7:
            x1_new = np.minimum(x1, x1_other)
            y1_new = np.minimum(y1, y1_other)
            x2_new = np.maximum(x2, x2_other)
            y2_new = np.maximum(y2, y2_other)

            merged_boxes[-1] = ([x1_new, y1_new, x2_new, y2_new])
        else:
            merged_boxes.append([x1, y1, x2, y2])

    return merged_boxes


def map2d(func, grid):
    return [[func(value) for value in row] for row in grid]
