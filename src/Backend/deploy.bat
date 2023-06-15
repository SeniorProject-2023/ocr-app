kubectl apply -f env-secret.yaml
kubectl apply -f env-configmap.yaml
kubectl apply -f ./OCR_Backend/deployment.yaml
kubectl apply -f ./Model_Server/deployment.yaml
REM kubectl create namespace backend
kubectl apply -f ./OCR_Backend/service.yaml
kubectl apply -f ./Model_Server/service.yaml
REM kubectl expose deployment ocr-backend --type=LoadBalancer --name=public-ocr-backend --port=8000