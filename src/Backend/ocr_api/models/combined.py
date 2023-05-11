import os
import re
from pathlib import Path

import cv2
import numpy as np
import torch
from PIL import Image
from more_itertools import split_when
from datasets import load_metric

current_dir = os.path.dirname(os.path.abspath(__file__))

# Load Model
# TODO: The work done here is only loading onnx.
# TODO: Continue remaining adjustments to properly run inference
# TODO: See https://alimustoofaa.medium.com/how-to-load-model-yolov8-onnx-cv2-dnn-3e176cde16e6
word_model = cv2.dnn.readNet(f'{current_dir}/word_model.onnx')
letter_model = cv2.dnn.readNet(f'{current_dir}/letter_model.onnx')


name_to_unicode = {
    'baa': 1576,
    'hamza-sater': 1569,
    'ya': 1609,
    'sad': 1589,
    'kaf': 1603,
    'ha-last': 1607,
    'waw': 1608,
    'sen': 1587,
    'ta': 1578,
    'lam': 1604,
    'alef-hamza-under': 1573,
    'dal': 1583,
    'non': 1606,
    'zen': 1586,
    'ya-hamza': 1574,
    '3en': 1593,
    '8en': 1594,
    '5a': 1582,
    'fa-3dot': 1700,
    'mem': 1605,
    'fa': 1601,
    'alef-hamza-up': 1571,
    'ya-dot': 1610,
    'thal': 1584,
    'waw-hamza': 1572,
    'tha': 1579,
    'taa-': 1591,
    'ha': 1726,
    '7a': 1581,
    'baa-3dot': 1662,
    'ra': 1585,
    'dad': 1590,
    'sheen': 1588,
    'gem': 1580,
    'qaf': 1602,
    'zaa': 1592,
    'gem-three-dot': 1670,
    'alef-mad': 1570,
    'taa-marpota': 1577,
    'alef-': 1575,
    'lam-alef': 65276,
    'lam-alef-la': 65275,
    'space': ord(" "),
}


def load_image(img_path: str):
    frame = cv2.cvtColor(cv2.imread(img_path), cv2.COLOR_BGR2RGB)
    return frame


def get_device():
    return 0 if torch.cuda.is_available() else "cpu"

def infer_words(img: np.ndarray):
    global word_model
    # returns List[(word_image, bb, class)]
    results = word_model.predict(img, device=get_device(),verbose=False)
    returnable = []
    while len(results[0]) != 0:
        for r in results:
            boxes = r.boxes
            for box in boxes:
                x1, y1, x2, y2 = box.xyxy[0].cpu(
                ).data.numpy().astype(int).tolist()
                returnable.append(
                    (img[y1:y2, x1:x2].copy(), box, word_model.names[int(box.cls)]))
                img = cv2.rectangle(img, (x1, y1), (x2, y2),
                                    (255, 255, 255), -1)
        results = word_model.predict(img)
    return returnable


def preprocess_box(img):
    img = cv2.cvtColor(img, cv2.COLOR_RGB2GRAY)
    img = cv2.threshold(img, 0, 255, cv2.THRESH_BINARY + cv2.THRESH_OTSU)[1]
    img = cv2.cvtColor(img, cv2.COLOR_GRAY2RGB)
    img = np.pad(img, ((10, 10), (10, 10), (0, 0)), constant_values=255)
    return img


def infer_letters(img: np.ndarray, conf=0.5):
    results = letter_model.predict(img, device=get_device(), conf=conf, verbose=False)[0]
    boxes = [box.xyxy[0].tolist() for box in results.boxes]  # x1, y1, x2, y2
    xs = [box[2] for box in boxes]
    sorted_indices = np.argsort(xs)[::-1]  # sort by max x2

    letters = [int(results.boxes.cls[i]) for i in sorted_indices]
    letters = [letter_model.names[class_no] for class_no in letters]
    letters = [name_to_unicode[name] for name in letters]
    letters = [chr(unc) for unc in letters]
    word = "".join(letters)
    return word


def save_to_file():
    pass


def get_y_center(b):
    x1, y1, x2, y2 = b[1].xyxy[0].tolist()
    return (y1 + y2) / 2


def not_vertically_overlapping(b1, b2):
    _, up1, _, down1 = b1[1].xyxy[0].tolist()
    _, up2, _, down2 = b2[1].xyxy[0].tolist()
    return down1 < up2 or (down1 - up2) < (up2 - up1)


def groupbyrow(boxes):
    sorted_boxes = sorted(boxes, key=get_y_center)
    return list(split_when(sorted_boxes, not_vertically_overlapping))


def evaluate_yolo_model(test_dir, model):
    test_dir = Path(test_dir)
    assert test_dir.exists()
    assert "images" in os.listdir(test_dir)
    assert "labels" in os.listdir(test_dir)

    predicted_texts = []
    ground_truths = []
    for filename in os.listdir(Path(test_dir) / "images"):
        match = re.search("(.+)-\d+.png", filename)
        if not match:
            continue
        else:
            ground_truths.append(match.group(1))
            img = cv2.cvtColor(cv2.imread(str(Path(test_dir) / "images/" / filename)), cv2.COLOR_BGR2RGB)
            predicted_texts.append(infer_letters(img))

    cer_metric = load_metric("cer")
    return cer_metric.compute(predictions=predicted_texts, references=ground_truths)
