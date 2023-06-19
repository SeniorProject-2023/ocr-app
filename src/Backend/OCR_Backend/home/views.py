from django.shortcuts import redirect
from rest_framework.decorators import api_view
from django.http import HttpResponse

def home(req):
    return redirect('https://seniorproject-2023.github.io/ocr-app/')


@api_view(['GET'])
def check_health(req):
    return HttpResponse('')
