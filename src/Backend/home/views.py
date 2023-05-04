from django.shortcuts import redirect


def home(req):
    return redirect('/static/index.html')
