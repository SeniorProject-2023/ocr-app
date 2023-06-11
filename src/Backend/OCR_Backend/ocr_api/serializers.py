from rest_framework import serializers

class ImageSerializer(serializers.Serializer):
    images = serializers.ListField(
        child = serializers.ImageField(),
        max_length = 10, # maximum number of images that can be handled in one request 
        allow_empty = False
    )