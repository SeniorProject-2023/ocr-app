from inference import WordInference, ThreadedServer, server

server = ThreadedServer(WordInference, port=18811, hostname='0.0.0.0')
print('[INFO] Starting Inference Server')
server.start()