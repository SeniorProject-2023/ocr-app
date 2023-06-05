import jwt
from rest_framework.permissions import BasePermission
import importlib.util

spec = importlib.util.spec_from_file_location("settings", "settings.py")
settings = importlib.util.module_from_spec(spec)
spec.loader.exec_module(settings)
secret_key = settings.SECRET_KEY
hashing_alg = settings.HASHING_ALG

class JobJWTPermission(BasePermission):
    def has_permission(self, request, view):
        # Get the JWT from the Authorization header
        job_header = request.headers.get('Job')
        if not job_header:
            return False
        job_token = job_header.split(' ')[1]

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