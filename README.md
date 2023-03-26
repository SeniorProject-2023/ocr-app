# Arabic OCR App

## Publishing on Android

To publish the app on Android, run the following command in the `OCRApp.Mobile` directory:

```
dotnet publish -f:net7.0-android -c:Release -p:RunAOTCompilation=true
```

The resulting APK will be in `OCRApp.Mobile\bin\Release\net7.0-android\android-arm64\publish`
