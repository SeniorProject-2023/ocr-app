from django.urls import path
from . import views

# URLConf
urlpatterns = [
    path('signup/', views.signup),
    path('login/', views.login),
    path('refresh-token/', views.refresh_token)
]