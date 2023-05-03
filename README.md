# Arabic OCR App

## Publishing on Android

To publish the app on Android, run the following command in the `OCRApp.Mobile` directory:

```
dotnet publish -f:net7.0-android -c:Release -p:RunAOTCompilation=true
```

The resulting APK will be in `OCRApp.Mobile\bin\Release\net7.0-android\android-arm64\publish`

## Backend

1. Open a terminal in `src/Backend` and run `pip install -r requirements.txt`
1. Download MySQL Community from <https://dev.mysql.com/downloads/installer/>
1. While installing, remember the username you configured (defaults to `root`) and the password you created.
1. In `src/Backend/arabic_ocr_backend/settings.py`, adjust `DATABASES` as necessary.

### APIs

#### Login API

Method: POST
URL: `/users/login`
Body: `{"username": "USERNAME", "password": "PASSWORD"}`

#### Signup API

Method: POST
URL: `/users/signup`
Body: `{"username": "USERNAME", "password": "PASSWORD"}`
