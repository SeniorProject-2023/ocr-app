from django.apps import AppConfig
import sys
from os import path
sys.path.append(path.join(path.join(path.dirname(__file__), '..'),'..'))
from InferenceServer.inference import StartServer


class OcrApiConfig(AppConfig):
    default_auto_field = "django.db.models.BigAutoField"
    name = "ocr_api"
    def ready(self) -> None:
        StartServer()
        return super().ready()
