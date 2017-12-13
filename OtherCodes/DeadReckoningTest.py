# coding:utf-8
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D
from datetime import time
import time
import random

csvfilename = 'data/0901/exp1/xiaoxiong.csv'
df = pd.read_csv(csvfilename, header=None,
                 names=['abstime','posx','posy','posz','roty','rotx','anim'])
                 # skiprows=1, skipfooter=1)
df.head()

Tdelta = 0.5
Xr=df['posx'].values
Yr=df['posy'].values
Zr=df['posz'].values
m=len(Xr)
print(m)

Qtx = []
Qty = []
Qtz = []
Tf = 0.0 # since last frame
Tt = 0.0 # since last update
deltaTime = 0.0
totalTime = 0.0
Pm1 = None
Vm1 = None
P0 = None
# V0 = None
# V0Prime = None
# Pt = None

mean = [i*0.01 for i in range(1,21)]
print(mean)
for i in mean:
    random.seed(1)
    randomFactor = [random.random() * 0.01 + (i - 0.005) for _ in range(m)]
    # print(randomFactor)

    # update
    for idx,step in enumerate(range(m)):
        frameBegin = time.time()
        time.sleep(randomFactor[idx])
        computeBegin = time.time()
        if idx == 0:
            Pm1 = np.array([Xr[idx],Yr[idx],Zr[idx]])
            Qtx.append(Pm1[0])
            Qty.append(Pm1[1])
            Qtz.append(Pm1[2])
        elif idx == 1:
            P0 = np.array([Xr[idx],Yr[idx],Zr[idx]])
            V0 = (P0 - Pm1 ) / deltaTime
            # 只有一个位置点，使用simple extrapolate预测
            combined = P0 + V0 * deltaTime
            Qtx.append(combined[0])
            Qty.append(combined[1])
            Qtz.append(combined[2])
            Pm1 = P0
            Vm1 = V0
            P0 = combined
        else:
            Tt += deltaTime
            Tnorm = 0.3#max(0.0, min(Tt / Tdelta, 1.0))
            #1.从server收到新位置数据，作为last known，求出新的速度、加速度
            V0 = (P0 - Pm1) / deltaTime
            P0Prime = np.array([Xr[idx],Yr[idx],Zr[idx]])
            V0Prime = P0Prime - Pm1
            A0Prime = (V0Prime-Vm1)/deltaTime
            #2.velocity blending
            Vb = V0 + (V0Prime - V0) * Tnorm
            #3.从当前位置，根据混合速度和新加速度，计算预测位置Pt
            Pt = P0 + Vb * deltaTime + 0.5 * A0Prime * deltaTime * deltaTime
            #4.从last known位置，根据新速度和加速度，计算预测位置Pt'
            PtPrime = P0Prime + V0Prime * deltaTime + 0.5 * A0Prime * deltaTime * deltaTime
            #5.结合两个预测位置，计算目标位置
            combined = Pt + (PtPrime - Pt) * Tnorm
            # print(idx,Tnorm,combined)
            Qtx.append(combined[0])
            Qty.append(combined[1])
            Qtz.append(combined[2])
            #6.移动到目标位置
            Tt = 0.0

            # 准备下一次迭代
            Pm1 = P0Prime
            Vm1 = Vb
            P0 = combined
        frameEnd = time.time()
        deltaTime = frameEnd - frameBegin
        totalTime += (frameEnd*1000000 - computeBegin*1000000)
        print(totalTime)

    # 画图
    fig = plt.figure(figsize=(16, 9))
    ax = fig.add_subplot(111, projection='3d')
    ax.plot(Qtx, Qtz, Qty, label='DR Estimate')
    ax.plot(Xr, Zr, Yr, label='Real')
    ax.set_xlabel('X')
    ax.set_ylabel('Z')
    ax.set_zlabel('Y')
    ax.legend()
    plt.title('Trajectory estimated with Dead Reckoning')

    # Axis equal
    # max_range = np.array([Xm.max()-Xm.min(), Ym.max()-Ym.min(), Zm.max()-Zm.min()]).max() / 3.0
    # mean_x = Xm.mean()
    # mean_y = Ym.mean()
    # mean_z = Zm.mean()
    # print(mean_x,mean_y,mean_z)
    # ax.set_xlim(mean_x - max_range, mean_x + max_range)
    # ax.set_ylim(mean_z - max_range, mean_z + max_range)
    # ax.set_zlim(mean_y, mean_z + 15)
    # plt.show()

    # distance calculate
    dist = np.sqrt(((Xr - Qtx)**2 + (Yr - Qty)**2 + (Zr - Qtz)**2).mean())
    # print(dist)
    print('%.3f,%.8f,%.3f' % (i, totalTime, dist))
    Qtx=[]
    Qty=[]
    Qtz=[]
    totalTime = 0.0
