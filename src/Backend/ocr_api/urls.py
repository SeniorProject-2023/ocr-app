from django.urls import path
from . import views

# URLConf
urlpatterns = [
    path('arabic-ocr/', views.arabic_ocr)
]