import cv2
import numpy as np


def highPassFilter(img, kSize=51):
    if not kSize % 2:
        kSize += 1
    kernel = np.ones((kSize, kSize), np.float32) / (kSize * kSize)
    filtered = cv2.filter2D(img, -1, kernel)
    filtered = img.astype('float32') - filtered.astype('float32')
    filtered = filtered + 127 * np.ones(img.shape, np.uint8)
    filtered = filtered.astype('uint8')

    return filtered


def blackPointSelect(img, blackPoint=66):
    def map(x, in_min, in_max, out_min, out_max):
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min
    img = img.astype('int32')
    img = map(img, blackPoint, 255, 0, 255)
    _, img = cv2.threshold(img, 0, 255, cv2.THRESH_TOZERO)
    return img.astype('uint8')


def whitePointSelect(img, whitePoint=127):
    def map(x, in_min, in_max, out_min, out_max):
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min
    _, img = cv2.threshold(img, whitePoint, 255, cv2.THRESH_TRUNC)
    img = img.astype('int32')
    img = map(img, 0, whitePoint, 0, 255)
    img = img.astype('uint8')

    return img
