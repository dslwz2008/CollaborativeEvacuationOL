
# coding: utf-8

import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D
import time
import random


csvfilename = 'data/0901/exp1/xiaoxiong.csv'
df = pd.read_csv(csvfilename, header=None,
                 names=['abstime','posx','posy','posz','roty','rotx','anim'])
                 # skiprows=1, skipfooter=1)
df.head()

Xr=df['posx'].values
Yr=df['posy'].values
Zr=df['posz'].values
m=len(Xr)
print(m)

deltaTime = 0.0
totalTime = 0.0

P = 1.0*np.eye(9)
H = np.matrix([[1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0],
               [0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0],
               [0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0]])
rp = 0.01  # Noise of Position Measurement
R = np.matrix([[rp, 0.0, 0.0],
               [0.0, rp, 0.0],
               [0.0, 0.0, rp]])
sa = 0.05
u = 0.0
B = np.matrix([[0.0],
               [0.0],
               [0.0],
               [0.0],
               [0.0],
               [0.0],
               [0.0],
               [0.0],
               [0.0]])
I = np.eye(9)
sp= 0.01 # Sigma for position noise
Xm = Xr + sp * (np.random.randn(m))
Ym = Yr + sp * (np.random.randn(m))
Zm = Zr + sp * (np.random.randn(m))
measurements = np.vstack((Xm,Ym,Zm))
x = np.matrix([measurements[0][0], measurements[1][0],measurements[2][0], 0.0, 0.0, 0.0, 0.0, 0.0, 0.0]).T
# Preallocation for Plotting
xt = []
yt = []
zt = []

mean = [i*0.01 for i in range(1,21)]
print(mean)
for i in mean:
    random.seed(1)
    randomFactor = [random.random() * 0.01 + (i - 0.005) for _ in range(m)]
    for idx,step in enumerate(range(m)):
        frameBegin = time.time()
        time.sleep(randomFactor[idx])
        computeBegin = time.time()
        # 更新随时间变化的矩阵
        dt = i if idx == 0 else deltaTime  # Time Step between Filter Steps
        A = np.matrix([[1.0, 0.0, 0.0, dt, 0.0, 0.0, 1 / 2.0 * dt ** 2, 0.0, 0.0],
                       [0.0, 1.0, 0.0, 0.0, dt, 0.0, 0.0, 1 / 2.0 * dt ** 2, 0.0],
                       [0.0, 0.0, 1.0, 0.0, 0.0, dt, 0.0, 0.0, 1 / 2.0 * dt ** 2],
                       [0.0, 0.0, 0.0, 1.0, 0.0, 0.0, dt, 0.0, 0.0],
                       [0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, dt, 0.0],
                       [0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, dt],
                       [0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0],
                       [0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0],
                       [0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0]])

        G = np.matrix([[1 / 2.0 * dt ** 2],
                       [1 / 2.0 * dt ** 2],
                       [1 / 2.0 * dt ** 2],
                       [dt],
                       [dt],
                       [dt],
                       [1.0],
                       [1.0],
                       [1.0]])
        Q = G * G.T * sa ** 2

        # Time Update (Prediction)
        # ========================
        # Project the state ahead
        x = A*x + B*u

        # Project the error covariance ahead
        P = A*P*A.T + Q

        # Measurement Update (Correction)
        # ===============================
        # Compute the Kalman Gain
        S = H*P*H.T + R
        K = (P*H.T) * np.linalg.pinv(S)

        # Update the estimate via z
        Z = measurements[:,step].reshape(H.shape[0],1)
        y = Z - (H*x) # Innovation or Residual
        x = x + (K*y)

        # Update the error covariance
        P = (I - (K*H))*P

        # Save states for Plotting
        xt.append(float(x[0]))
        yt.append(float(x[1]))
        zt.append(float(x[2]))

        frameEnd = time.time()
        deltaTime = frameEnd - frameBegin
        totalTime += (frameEnd - computeBegin)

    # distance calculate
    dist = np.sqrt(((Xr-xt)**2 + (Yr-yt)**2 + (Zr-zt)**2).mean())
    print('%.3f,%.8f,%.3f' % (i, totalTime, dist))
    # 还原初始设置
    totalTime = 0.0

    P = 1.0 * np.eye(9)
    H = np.matrix([[1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0],
                   [0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0],
                   [0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0]])
    rp = 0.01  # Noise of Position Measurement
    R = np.matrix([[rp, 0.0, 0.0],
                   [0.0, rp, 0.0],
                   [0.0, 0.0, rp]])
    sa = 0.05
    u = 0.0
    B = np.matrix([[0.0],
                   [0.0],
                   [0.0],
                   [0.0],
                   [0.0],
                   [0.0],
                   [0.0],
                   [0.0],
                   [0.0]])
    I = np.eye(9)
    sp = 0.01  # Sigma for position noise
    Xm = Xr + sp * (np.random.randn(m))
    Ym = Yr + sp * (np.random.randn(m))
    Zm = Zr + sp * (np.random.randn(m))
    measurements = np.vstack((Xm, Ym, Zm))
    x = np.matrix([measurements[0][0], measurements[1][0], measurements[2][0], 0.0, 0.0, 0.0, 0.0, 0.0, 0.0]).T
    # Preallocation for Plotting
    xt = []
    yt = []
    zt = []

