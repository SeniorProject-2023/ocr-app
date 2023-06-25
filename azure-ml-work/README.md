# Azure ML

We used Azure ML for model deployment, using a real-time managed endpoint.

First, we create a temporary compute on Azure ML to move the model from Google Drive to Azure without having to upload it with our slow internet speed.

To do that, create a new notebook, and use the following code:

```
%pip install gdown

import gdown

gdown.download(url="https://drive.google.com/file/d/1pfFmbmzbiPPryt2LEltOvml7HH9SrMVQ/view", output="letter_model.pt", quiet=False, fuzzy=True)
from azure.ai.ml import MLClient, Input
from azure.ai.ml.entities import Model
from azure.ai.ml.constants import AssetTypes
from azure.identity import DefaultAzureCredential

subscription_id = "f4f75b61-8d75-4263-8b2a-bcfac715f231"
resource_group = "ocrapp"
workspace = "ocrworkspace"

ml_client = MLClient(DefaultAzureCredential(), subscription_id, resource_group, workspace)

from azure.ai.ml.entities import Model
from azure.ai.ml.constants import AssetTypes

file_model = Model(
    path="letter_model.pt",
    type=AssetTypes.CUSTOM_MODEL,
    name="letter_model",
    description="Arabic OCR Letter Model",
)
ml_client.models.create_or_update(file_model)
```

After this is run, your model is now deployed. Make sure to delete the created compute to avoid cost.

Then, we create an endpoint for the uploaded model using a custom environment with the following Dockerfile:

```Dockerfile
FROM mcr.microsoft.com/azureml/openmpi4.1.0-ubuntu20.04
RUN apt-get update && apt-get install -y libgl1 && pip install azureml-inference-server-http azureml-mlflow datasets more_itertools numpy deskew jdeskew ultralytics --no-cache-dir
```

The scoring script is `score.py` in this directory.