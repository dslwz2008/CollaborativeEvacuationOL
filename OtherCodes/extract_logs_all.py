# coding:utf-8
import os
import os.path
import sys
import csv
from datetime import datetime,timedelta

def get_users(filename):
    result = {}
    with open(filename,'r') as fp:
        for idx,line in enumerate(fp.readlines()):
            key = 'exp'+str(idx+1)
            users = line.strip().split(',')
            result[key] = users
    return result

# get_users('data/0826/exp.txt')

data_path = 'data/0826/'
exp_file = 'exp.txt'
# 包含实验数据的文件
files = ['exp1', 'exp2', 'exp3']
file_ext = '.txt'
interval = timedelta(milliseconds=300)  # ms
dt_format_short = '%Y-%m-%dT%H:%M:%S'
dt_format_long = '%Y-%m-%dT%H:%M:%S.%f'

users_in_exp = get_users(data_path + exp_file)
for file in files:
    # 开始解析文件
    filename = data_path + file + file_ext
    print('start processing ' + filename)
    result = []
    temp_dict = {}
    start_time = None
    end_time = None
    base_time = None
    frame = 0

    # 读文件，提取用户数据
    with open(filename, 'r') as fp:
        for idx, line in enumerate(fp.readlines()):
            items = line.strip().split('|')[-1].strip().split(',')
            # print(items)
            if 'UserVariablesUpdate' not in items[0]:  # 10项一共
                continue
            # 每个用户在每个间隔内的数据，取最后出现的一个
            str_format = dt_format_short if len(items[1]) == 19 else dt_format_long
            if start_time is None:
                start_time = datetime.strptime(items[1], str_format)
                end_time = start_time + interval
                base_time = start_time
                temp_dict = {}
            cur_time = datetime.strptime(items[1], str_format)
            if cur_time < end_time:
                temp_dict[items[3]] = ','.join(
                    [end_time.strftime(dt_format_long), str(frame), items[3], items[4], items[5], items[6],
                     items[7], items[8], items[9]]) + '\n'
            else:  # 清理
                for k in temp_dict:
                    result.append(temp_dict[k])
                temp_dict = {}
                frame += 1
                start_time = end_time
                end_time = start_time + interval
                temp_dict[items[3]] = ','.join(
                    [end_time.strftime(dt_format_long), str(frame), items[3], items[4], items[5], items[6],
                     items[7], items[8], items[9]]) + '\n'

            if idx % 10000 == 0:
                print(idx)

    # 写入文件
    # print(result)
    filename = '{0}/{1}.csv'.format(data_path, file)
    with open(filename, 'w') as fp:
        fp.writelines(result)

    print('finish!')

