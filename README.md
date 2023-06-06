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

## Run the backend locally with the application

To be able to make the application consume local backend, add `local-test.cmd` file in `src\Backend` with the following content:

```
SET SECRET_KEY=Random_Value_Local_Insecure
SET ARABIC_OCR_DATABASE_HOST=PUT_HOST_HERE
SET ARABIC_OCR_DATABASE_USER=PUT_USER_HERE
SET ARABIC_OCR_DATABASE_PASS=PUT_PASS_HERE
SET ALLOWED_HOSTS=*

python manage.py runserver 0.0.0.0:8000
```

This will run the server and will be accessible via LAN. To know the IP to access the server, type in `ipconfig` in cmd and check the local IPv4 Address (usually 192.168.x.x).

Then, go to OCRService.cs in the application code, and update the `BaseUri` constant defined for DEBUG configuration.

Note that `local-test.cmd` is added to `.gitignore`, this is for security reasons if we test locally against the deployed database.
It also allows everyone to freely set different environment variables in that script without one developer pushing the change and overwriting the other.
