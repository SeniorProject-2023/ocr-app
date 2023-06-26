from django.urls import path
from . import views

# URLConf
urlpatterns = [
    path('arabic-ocr/', views.arabic_ocr),
    path('check-for-job/', views.check_for_job),
    path('history/', views.get_history)
]
