
# coding: utf-8


from ztp import ztpoisson
from collections import Counter
import numpy as np


def assign_group(mu):
    # 各种size的群组所占百分比，服从ZTP分布
    groupPercents = [ztpoisson.rvs(mu) for _ in range(100)]
    # print(groupPercents)
    c = Counter(groupPercents)
    results = [tup[1] for tup in c.most_common()]
    # 大于5截断
    if len(results) >= 5:
        results = results[:5]
    # 小于5补齐
    else:
        results+=[0]*(5-len(results))
#     print(results) # 各种size的群组所占百分比，这里没考虑人数
    return np.array(results)

# 车站
# target = np.array([44,36,10,6,4])
# # 购物中心
# target = np.array([33,48,14,3,2])
# # 大学校园
# target = np.array([52,28,15,3,2])
# # 某大街
target = np.array([37,48,10,4,1])

mu_start = 100
mu_end = 300
sim_times = 100
assignments = []

min_error = 1000
min_mu = 0
min_mean = None
for mu in range(mu_start, mu_end, 1):
    mu = mu/100.0
    print(mu)
    for _ in range(sim_times):
        assignments.append(assign_group(mu))
    results = np.array(assignments)
    # compute mean
    mean_val = np.mean(results, axis=0)
    error = np.sum(np.abs(target-mean_val))
    if error < min_error:
        min_error = error
        min_mu = mu
        min_mean = mean_val
print(min_mu)
print(min_mean)
print(min_error)

# 车站
# 2.05
# [ 44.04915094  30.70292453  16.02066038   6.40330189   2.15188679]
# 13.6183018868

# 购物中心
# 1.97
# [ 44.81132653  30.88091837  15.64397959   6.09836735   1.9922449 ]
# 33.6805102041

# 大学校园
# 1.41
# [ 51.925       30.8002381   12.41119048   3.7947619    0.93357143]
# 7.32523809524

# 某大街
# 1.77
# [ 46.96948718  31.27038462  14.60089744   5.21602564   1.5724359 ]
# 33.0884615385