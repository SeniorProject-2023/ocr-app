from django.db import models
from django.contrib.auth import get_user_model

class HistoryItem(models.Model):
    id = models.AutoField(primary_key=True)
    user = models.ForeignKey(get_user_model(), on_delete=models.CASCADE)
    date_and_time = models.DateTimeField()

class HistoryItemElement(models.Model):
    id = models.AutoField(primary_key=True)
    history_item = models.ForeignKey(HistoryItem, on_delete=models.CASCADE, related_name='elements')
    text = models.TextField()

    def __str__(self):
        return self.text
