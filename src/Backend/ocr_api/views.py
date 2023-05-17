from rest_framework.decorators import api_view, permission_classes
from rest_framework.permissions import IsAuthenticated
from rest_framework.response import Response
from rest_framework import status
from .serializers import ImageSerializer
from .models.combined import groupbyrow, infer_letters, infer_words, preprocess_box
import cv2
import numpy as np

# Create your views here.
@api_view(['POST'])
@permission_classes([IsAuthenticated])
def arabic_ocr(req):
    print(req.data)
    serializer = ImageSerializer(data=req.data)
    if serializer.is_valid():
        images = serializer.validated_data['images']
        images_text = []
        for image in images:
            np_image = cv2.imdecode(np.frombuffer(image.read(), np.uint8), cv2.IMREAD_UNCHANGED)
            word_boxes = infer_words(np_image)

            word_boxes = groupbyrow(word_boxes)
            word_boxes = [i for sublist in word_boxes for i in
                          sorted(sublist, key=lambda b: b[1].xyxy[0].tolist()[2], reverse=True)]

            word_imgs = [x[0] for x in word_boxes]
            word_imgs = [preprocess_box(img) for img in word_imgs]
            word_texts = [infer_letters(x) for x in word_imgs]
            images_text.append(" ".join(word_texts))

        return Response({'success': images_text})
    else:
        return Response(serializer.errors, status=status.HTTP_400_BAD_REQUEST)
