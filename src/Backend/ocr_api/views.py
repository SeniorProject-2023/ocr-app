from rest_framework.decorators import api_view, permission_classes
from rest_framework.permissions import IsAuthenticated
from rest_framework.response import Response
from rest_framework import status
from .serializers import ImageSerializer
import cv2
import numpy as np
import uuid
import jwt
import importlib.util
import rpyc
from io import StringIO
from pickle import Pickler, Unpickler
from .job_jwt_permission import JobJWTPermission

spec = importlib.util.spec_from_file_location("settings", "settings.py")
settings = importlib.util.module_from_spec(spec)
spec.loader.exec_module(settings)
secret_key = settings.SECRET_KEY
hashing_alg = settings.HASHING_ALG
model_backend = settings.MODEL_BACKEND

jobs_dict = dict()

# Connect to the model backend rpyc server
model_conn = rpyc.connect(model_backend.HOST, model_backend.PORT)

def generate_model_callback(uuid):
    def handle_model_callback(result):
        job_uuid = uuid
        decoded_result =  Unpickler(StringIO(result)).load()
        jobs_dict[job_uuid] = decoded_result

    return handle_model_callback

# Create your views here.
@api_view(['POST'])
@permission_classes([IsAuthenticated])
def arabic_ocr(req):
    serializer = ImageSerializer(data=req.data)
    if serializer.is_valid():
        job_uuid = str(uuid.uuid4())
        
        images = serializer.validated_data['images']
        images_list = []
        for image in images:
            np_image = cv2.imdecode(np.frombuffer(image.read(), np.uint8), cv2.IMREAD_UNCHANGED)
            images_list.append(np_image)

        buffer = StringIO()
        Pickler(buffer).dump(images_list)
        status = model_backend.root.run_model(buffer.getvalue(), generate_model_callback(job_uuid))

        if status:
            encoded_job_id = jwt.encode({"user_id": req.user.id, "uuid": job_uuid},
                                    secret_key, algorithm=hashing_alg, options={"verify_exp": False})
            
            return Response({'job_id': encoded_job_id})
        
    return Response(serializer.errors, status=status.HTTP_400_BAD_REQUEST)


@api_view(['GET'])
@permission_classes([IsAuthenticated, JobJWTPermission])
def check_for_job(req):
    # Access the 'uuid' element in the job jwt payload
    job_uuid = req.job.payload['uuid']

    Response({'results': jobs_dict[job_uuid] if job_uuid in jobs_dict else None})
    