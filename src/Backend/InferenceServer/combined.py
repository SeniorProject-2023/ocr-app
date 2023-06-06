import os
import re
from pathlib import Path
import string


import cv2
import numpy as np
import torch
from PIL import Image
from more_itertools import split_when
from ultralytics import YOLO
from ultralytics.yolo.utils.plotting import Annotator
from datasets import load_metric

current_dir = os.path.dirname(os.path.abspath(__file__))

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

arabic_punctuations = '''`÷×؛<>_()*&^%][ـ،/:"؟.,'{}~¦+|!”…“–ـ'''
english_punctuations = string.punctuation
punctuations_list = arabic_punctuations + english_punctuations
arabic_diacritics = re.compile("""
                                 ّ    | # Tashdid
                                 َ    | # Fatha
                                 ً    | # Tanwin Fath
                                 ُ    | # Damma
                                 ٌ    | # Tanwin Damm
                                 ِ    | # Kasra
                                 ٍ    | # Tanwin Kasr
                                 ْ    | # Sukun
                                 ـ     # Tatwil/Kashida
                             """, re.VERBOSE)


def normalize_arabic(text):
    text = re.sub("[إأآا]", "ا", text)
    text = re.sub("ى", "ي", text)
    text = re.sub("ؤ", "ء", text)
    text = re.sub("ئ", "ء", text)
    text = re.sub("ة", "ه", text)
    text = re.sub("گ", "ك", text)

    return text


def remove_diacritics(text):
    text = re.sub(arabic_diacritics, '', text)
    return text


def remove_punctuations(text):
    translator = str.maketrans('', '', punctuations_list)
    return text.translate(translator)


def load_image(img_path: str):
    frame = cv2.cvtColor(cv2.imread(img_path), cv2.COLOR_BGR2RGB)
    return frame


def get_device():
    return 0 if torch.cuda.is_available() else "cpu"


def infer_words(img: np.ndarray):
    global word_model
    # returns List[(word_image, bb, class)]
    results = word_model.predict(img, device=get_device(), verbose=False)
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


def infer_words(img: np.ndarray):
    word_model = YOLO(f'{current_dir}/word_model.pt')
    # returns List[(word_image, bb, class)]
    results = word_model.predict(img, verbose=False)
    returnable = []
    for r in results:
        boxes = r.boxes
        for box in boxes:
            x1, y1, x2, y2 = box.xyxy[0].cpu(
            ).data.numpy().astype(int).tolist()
            returnable.append(
                (img[y1:y2, x1:x2].copy(), box, word_model.names[int(box.cls)]))
            img = cv2.rectangle(img, (x1, y1), (x2, y2),
                                (255, 255, 255), -1)
    return returnable


def infer_letters(img: np.ndarray, out_full_path=None, debug=False, convert_names=False, **kwargs):
    model = YOLO(f'{current_dir}/letter_model.pt')
    results = model.predict(img, verbose=False, **kwargs)[0]
    # x1, y1, x2, y2, x3, y3, etc
    boxes = [box.tolist() for box in results.boxes.data]
    xs = [np.max(box[::2]) for box in boxes]
    sorted_indices = np.argsort(xs)[::-1]  # sort by max x in each box

    letters = [int(results.boxes.cls[i]) for i in sorted_indices]
    letters = [model.names[class_no] for class_no in letters]

    if convert_names:
        letters = [name_to_unicode[name] for name in letters]
        letters = [chr(unc) for unc in letters]

    word = "".join(letters)

    if debug:
        annotator = Annotator(img, line_width=1)
        for i, b in enumerate(np.array(boxes)[sorted_indices]):
            annotator.box_label(b, str(i))
        frame = annotator.result()
        if out_full_path is not None:
            cv2.imwrite(out_full_path, img)
        # cv2_imshow(img)

    return normalize_arabic(remove_diacritics(remove_punctuations(word)))


def save_to_file():
    pass


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


def rectangles_intersect(rect1, rect2):
    x1, y1, w1, h1 = rect1
    x2, y2, w2, h2 = rect2
    if (x1 + w1 < x2 or x2 + w2 < x1 or y1 + h1 < y2 or y2 + h2 < y1):
        return False
    else:
        return True


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
            img = cv2.cvtColor(cv2.imread(
                str(Path(test_dir) / "images/" / filename)), cv2.COLOR_BGR2RGB)
            predicted_texts.append(infer_letters(img))

    cer_metric = load_metric("cer")
    return cer_metric.compute(predictions=predicted_texts, references=ground_truths)
