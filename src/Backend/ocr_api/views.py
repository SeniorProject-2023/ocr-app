from rest_framework.decorators import api_view, permission_classes
from rest_framework.permissions import IsAuthenticated
from rest_framework.response import Response
from rest_framework import status
from .serializers import ImageSerializer

# Create your views here.
@api_view(['POST'])
@permission_classes([IsAuthenticated])
def arabic_ocr(req):
    print(req.data)
    serializer = ImageSerializer(data=req.data)
    if serializer.is_valid():
        images = serializer.validated_data['images']
        images_text = None
        # TODO: Call the ocr model
        return Response({'success': images_text})
    else:
        return Response(serializer.errors, status=status.HTTP_400_BAD_REQUEST)
