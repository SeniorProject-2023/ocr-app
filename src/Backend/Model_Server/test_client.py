import pickle
import rpyc
import numpy as np
from io import BytesIO, StringIO
from pickle import Pickler, Unpickler
import urllib
import urllib.request


def mycallback(response):
    print(pickle.loads(response))


url_response = urllib.request.urlopen("https://i.imgur.com/tkUfiHg.png")
img_array = np.array(bytearray(url_response.read()), dtype=np.uint8)
images = [img_array]
buffer = BytesIO()
pickle.dump(images, buffer)

connection = rpyc.connect('localhost', 18811)
print(connection.root.register_task(buffer.getvalue(), mycallback))

bgsrv = rpyc.BgServingThread(connection)
bgsrv._thread.join()