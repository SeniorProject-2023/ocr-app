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

def get_angle(
        gray_img: np.ndarray, angle_max: float = 15, steps_per_degree: int = 15
):
    width = height = cv2.getOptimalDFTSize(max(gray_img.shape))
    gray_img = cv2.copyMakeBorder(
        src=gray_img,
        top=0,
        bottom=height - gray_img.shape[0],
        left=0,
        right=width - gray_img.shape[1],
        borderType=cv2.BORDER_CONSTANT,
        value=255,
    )

    dft = np.fft.fft2(gray_img)
    shifted_dft = np.fft.fftshift(dft)
    m = np.abs(shifted_dft)

    r = c = m.shape[0] // 2

    tr = np.linspace(-1 * angle_max, angle_max, int(angle_max * steps_per_degree * 2)) / 180 * np.pi
    profile_arr = tr.copy()

    def f(t):
        _f = np.vectorize(
            lambda x: m[c + int(x * np.cos(t)), c + int(-1 * x * np.sin(t))]
        )
        _l = _f(range(0, r))
        val_init = np.sum(_l)
        return val_init

    vf = np.vectorize(f)
    li = vf(profile_arr)

    a = tr[np.argmax(li)] / np.pi * 180

    if a == -1 * angle_max:
        return 0
    return a
