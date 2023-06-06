import jwt
from rest_framework.permissions import BasePermission
import importlib.util
import json

spec = importlib.util.spec_from_file_location("../arabic_ocr_backend", "settings.py")
settings = importlib.util.module_from_spec(spec)
spec.loader.exec_module(settings)
secret_key = settings.SECRET_KEY
hashing_alg = settings.HASHING_ALG

class JobJWTPermission(BasePermission):
    def has_permission(self, request, view):
        body_dic = json.loads(request.body)
        if 'job_token' not in body_dic:
            return False

        job_token = body_dic['job_token']
        try:
            # Decode the JWT
            payload = jwt.decode(job_token, secret_key, algorithms=[hashing_alg])
        except jwt.InvalidTokenError:
            return False

        # Check if the payload has the required fields
        if 'user_id' not in payload or 'uuid' not in payload or payload['user_id'] != request.user.id:
            return False
        
        # Set the req.job.payload attribute to the decoded payload
        request.job = type('job', (object,), {'payload': payload})
        
        return True