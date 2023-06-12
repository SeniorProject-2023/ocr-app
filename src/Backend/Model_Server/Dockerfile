# For more information, please refer to https://aka.ms/vscode-docker-python
FROM python:3.10

EXPOSE 18811

# Keeps Python from generating .pyc files in the container
ENV PYTHONDONTWRITEBYTECODE=1

# Turns off buffering for easier container logging
ENV PYTHONUNBUFFERED=1

COPY ./requirements.txt .
# Install pip requirements
RUN apt-get update && apt-get install -y libgl1 && python -m pip install -r requirements.txt --no-cache-di && python -m pip install gdown

WORKDIR /model
COPY . /model

RUN gdown 1pfFmbmzbiPPryt2LEltOvml7HH9SrMVQ -O ./letter_model.pt

CMD ["python", "__init__.py"]
