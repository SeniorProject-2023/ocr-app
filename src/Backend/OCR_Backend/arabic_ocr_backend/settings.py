"""
Django settings for arabic_ocr_backend project.

Generated by 'django-admin startproject' using Django 4.1.7.

For more information on this file, see
https://docs.djangoproject.com/en/4.1/topics/settings/

For the full list of settings and their values, see
https://docs.djangoproject.com/en/4.1/ref/settings/
"""

from pathlib import Path
import os
from datetime import timedelta
import configparser

# Build paths inside the project like this: BASE_DIR / 'subdir'.
BASE_DIR = Path(__file__).resolve().parent.parent


# Quick-start development settings - unsuitable for production
# See https://docs.djangoproject.com/en/4.1/howto/deployment/checklist/

# Load environment variables from config.ini file
config = configparser.ConfigParser()
config.read(os.path.join('arabic_ocr_backend', 'config.ini'))

# Set environment variables based on current environment
ENVIRONMENT = os.environ.get('ENVIRONMENT')
if ENVIRONMENT == 'development' or ENVIRONMENT == 'docker_development':
    DEBUG_CONFIG = config.getboolean(ENVIRONMENT, 'DEBUG')
    MODEL_BACKEND_HOST = config.get(ENVIRONMENT, 'MODEL_BACKEND_HOST')
    MODEL_BACKEND_PORT = config.getint(ENVIRONMENT, 'MODEL_BACKEND_PORT')
    DB_HOST = config.get(ENVIRONMENT, 'DB_HOST')
    DB_USER = config.get(ENVIRONMENT, 'DB_USER')
    DB_PASSWORD = config.get(ENVIRONMENT, 'DB_PASSWORD')
    SECRET = config.get(ENVIRONMENT, 'SECRET_KEY')
elif ENVIRONMENT == 'production':
    DEBUG_CONFIG = config.getboolean('production', 'DEBUG')
    MODEL_BACKEND_HOST = os.environ.get('MODEL_BACKEND_HOST')
    MODEL_BACKEND_PORT = int(os.environ.get('MODEL_BACKEND_PORT'))
    DB_HOST = os.environ.get('DB_HOST')
    DB_USER = os.environ.get('DB_USER')
    DB_PASSWORD = os.environ.get('DB_PASSWORD')
    SECRET = os.environ.get('SECRET_KEY')
    
# SECURITY WARNING: keep the secret key used in production secret!
SECRET_KEY = SECRET
HASHING_ALG = 'HS256'
MODEL_BACKEND = {
    'HOST': MODEL_BACKEND_HOST,
    'PORT': MODEL_BACKEND_PORT
}

AWS = {
    'AWS_ACCESS_KEY_ID': os.environ.get('AWS_ACCESS_KEY_ID'),
    'AWS_SECRET_ACCESS_KEY': os.environ.get('AWS_SECRET_ACCESS_KEY'),
    'AWS_STORAGE_BUCKET_NAME': os.environ.get('AWS_STORAGE_BUCKET_NAME'),
    'AWS_S3_REGION_NAME': os.environ.get('AWS_S3_REGION_NAME') # e.g. 'us-east-1'
}

DEBUG = DEBUG_CONFIG

ALLOWED_HOSTS = ['localhost', os.environ.get('ALLOWED_HOSTS', 'ocr2023.azurewebsites.net')]

# Application definition

INSTALLED_APPS = [
    "django.contrib.admin",
    "django.contrib.auth",
    "django.contrib.contenttypes",
    "django.contrib.messages",
    "django.contrib.staticfiles",
    "rest_framework",
    "rest_framework_simplejwt",
    "ocr_api",
    "corsheaders",
    "users", 
    "users.validators"
]

MIDDLEWARE = [
    "django.middleware.security.SecurityMiddleware",
    "django.contrib.sessions.middleware.SessionMiddleware",
    'corsheaders.middleware.CorsMiddleware',
    "django.middleware.common.CommonMiddleware",
    "django.middleware.csrf.CsrfViewMiddleware",
    "django.contrib.auth.middleware.AuthenticationMiddleware",
    "django.contrib.messages.middleware.MessageMiddleware",
    "django.middleware.clickjacking.XFrameOptionsMiddleware",
]

ROOT_URLCONF = "arabic_ocr_backend.urls"

TEMPLATES = [
    {
        "BACKEND": "django.template.backends.django.DjangoTemplates",
        "DIRS": [],
        "APP_DIRS": True,
        "OPTIONS": {
            "context_processors": [
                "django.template.context_processors.debug",
                "django.template.context_processors.request",
                "django.contrib.auth.context_processors.auth",
                "django.contrib.messages.context_processors.messages",
            ],
        },
    },
]

WSGI_APPLICATION = "arabic_ocr_backend.wsgi.application"


# Database
# https://docs.djangoproject.com/en/4.1/ref/settings/#databases

DATABASES = {
    "default": {
        'ENGINE': 'django.db.backends.mysql',
        'NAME': 'arabic_ocr_database',
        "HOST": DB_HOST,
        "USER": DB_USER,
        "PASSWORD": DB_PASSWORD
    }
}

REST_FRAMEWORK = {
    "DEFAULT_AUTHENTICATION_CLASSES": [
        "rest_framework_simplejwt.authentication.JWTAuthentication",
    ],
}

SIMPLE_JWT = {
    "AUTH_HEADER_TYPES": ("Bearer",),
    "ACCESS_TOKEN_LIFETIME": timedelta(minutes=10),
    "REFRESH_TOKEN_LIFETIME": timedelta(days=3),
}

# Password validation
# https://docs.djangoproject.com/en/4.1/ref/settings/#auth-password-validators

AUTH_PASSWORD_VALIDATORS = [
    {
        'NAME': 'django.contrib.auth.password_validation.MinimumLengthValidator',
        'OPTIONS': {
            'min_length': 8,
        }
    },
    {
        'NAME': 'django.contrib.auth.password_validation.UserAttributeSimilarityValidator',
        'OPTIONS': {
            'user_attributes': ('username'), #Checks whether the password is too similar to the user\'s attributes, such as their username or email
        }
    },
    {
        'NAME': 'django.contrib.auth.password_validation.CommonPasswordValidator',
    },
    {
        'NAME': 'django.contrib.auth.password_validation.NumericPasswordValidator',
    },
    {
        'NAME': 'users.validators.pass_validators.UppercaseValidator',
        'OPTIONS': {
            'min_upper': 1,
        }
    },
    {
        'NAME': 'users.validators.pass_validators.LowercaseValidator',
        'OPTIONS': {
            'min_lower': 3,
        }
    },
    {
        'NAME': 'users.validators.pass_validators.NumericValidator',
        'OPTIONS': {
            'min_numeric': 3,
        }
    },
    {
        'NAME': 'users.validators.pass_validators.SpecialCharValidator',
        'OPTIONS': {
            'min_special': 1,
        }
    },
]


# Internationalization
# https://docs.djangoproject.com/en/4.1/topics/i18n/

LANGUAGE_CODE = "en-us"

TIME_ZONE = "UTC"

USE_I18N = True

USE_TZ = True


# Static files (CSS, JavaScript, Images)
# https://docs.djangoproject.com/en/4.1/howto/static-files/

STATIC_URL = 'static/'
STATIC_ROOT = 'static/'

# Default primary key field type
# https://docs.djangoproject.com/en/4.1/ref/settings/#default-auto-field

DEFAULT_AUTO_FIELD = "django.db.models.BigAutoField"

CORS_ALLOWED_ORIGINS = [
    'https://seniorproject-2023.github.io',
]
