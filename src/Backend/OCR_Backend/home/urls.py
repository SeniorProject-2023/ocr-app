from django.urls import path
from . import views

# URLConf
urlpatterns = [
    path('', views.home),
    path('health/', views.check_health)
]
