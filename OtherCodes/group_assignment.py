
# coding: utf-8

from ztp import ztpoisson
import sqlite3
import math
from collections import Counter
import numpy as np

mu = 1.3
dbpath = '../WebServer/participants.db'
# 获取总的参与人数
sqlGetCount =  'select count(*) from participants'
sqlUpdateGroupID = ''
con = sqlite3.connect(dbpath)

cur = con.cursor()
cur.execute(sqlGetCount)
record = cur.fetchone()
con.close()

totalNumber = record[0]
print(totalNumber)


# 各种size的群组所占百分比，服从ZTP分布
groupPercents = [ztpoisson.rvs(mu) for _ in range(100)]
# print(groupPercents)
c = Counter(groupPercents)
results = [tup[1] for tup in c.most_common()][:5]
print(results) # 各种size的群组所占百分比，这里没考虑人数

# 计算人数
peopleActually = [results[i]*(i+1) for i in range(len(results))]
peopleActually = np.array(peopleActually)
sumPeople = peopleActually.sum() # 需要的总人数
peoplePercents = peopleActually/sumPeople # 各组按照人数计算，所占的百分比
print(peoplePercents)

# 实际人数
peopleNumber = np.round(totalNumber*peoplePercents) # 计算各组人数
print(peopleNumber)
# 计算结果表示，groupsize为index的组有value个人


# reassignment fro each group
# 如果一个组分不完的话，或者一个组不够分的话，归还到pool中，分配到其他组；
pool = 0
peopleNumber = peopleNumber.astype(int).tolist()
peopleNumber.reverse()
print(peopleNumber)
result = []

for idx,num in enumerate(peopleNumber):
    print(num,idx)
    # get from pool
    tempNum = peopleNumber[idx] + pool

    groupIdx = idx + 1
    result.append(num // groupIdx)
    remains = num % groupIdx
    # return to pool
    pool = remains
    
result.reverse()
print(result)

# 数据库手动修改一下groupid和groupsize




