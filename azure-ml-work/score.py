from azureml.contrib.services.aml_request import rawhttp
import re
import os
import numpy as np
from ultralytics import YOLO
from PIL import Image
import cv2

# The init() method is called once, when the web service starts up.
def init():
    print("Loading model.")
    global model
    # The AZUREML_MODEL_DIR environment variable indicates
    # a directory containing the model file you registered.
    model_file_name = "letter_model.pt"
    model_path = os.path.join(os.environ.get("AZUREML_MODEL_DIR"), model_file_name)
    model = YOLO(model_path)
    print("Model is loaded.")


# The run() method is called each time a request is made to the scoring API.
@rawhttp
def run(request):
    print("Got a request.")
    if request.method == 'POST':
        try:
            file_bytes = request.files["image"]
            image = Image.open(file_bytes).convert('RGB')
            result = infer_letters(image, conf=0.3, iou=0.5, agnostic_nms=True)
            return { "predict": result }
        except Exception as ex:
            return { "message": str(ex) }
    


def normalize_arabic(text):
   text = re.sub("[إأآا]", "ا", text)
   text = re.sub("ى", "ي", text)
   text = re.sub("ؤ", "ء", text)
   text = re.sub("ئ", "ء", text)
   text = re.sub("ة", "ه", text)
   text = re.sub("گ", "ك", text)

   return text


def infer_letters(img, **kwargs):
   results = model.predict(img, verbose=False, **kwargs)[0]
   boxes = [box.tolist() for box in results.boxes.data]
   xs = [np.max(box[::2]) for box in boxes]
   sorted_indices = np.argsort(xs)[::-1]  # sort by max x in each box
   letters = [model.names[int(results.boxes.cls[i])] for i in sorted_indices]
   word = "".join(letters)
   return_result = normalize_arabic(word)

   return return_result
