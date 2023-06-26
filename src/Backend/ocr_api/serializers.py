from rest_framework import serializers

from ocr_api.models import HistoryItem, HistoryItemElement

class ImageSerializer(serializers.Serializer):
    images = serializers.ListField(
        child = serializers.ImageField(),
        max_length = 10, # maximum number of images that can be handled in one request 
        allow_empty = False
    )

class HistoryItemElementSerializer(serializers.ModelSerializer):
    class Meta:
        model = HistoryItemElement
        fields = ('id', 'history_item', 'text')


class HistoryItemSerializer(serializers.ModelSerializer):
    elements = serializers.StringRelatedField(many=True)

    class Meta:
        model = HistoryItem
        fields = ('id', 'user', 'date_and_time', 'elements')
