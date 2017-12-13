
# coding: utf-8

import os
import os.path
import pandas as pd
import json


def build_group_info(group_file):
    result = dict()
    gids = []
    with open(group_file) as fp:
        for line in fp.readlines():
            items = line.split()
            if len(items) == 0:
                continue
            pid = int(items[0])
            if pid < 0: 
                continue
            group_size = int(items[1])
            other_pid = []
            for i in range(group_size-1):
                other_pid.append(int(items[i+2]))
            other_pid.append(pid)
            gid = sum(other_pid)
            result[pid] = gid
    return result

# print(build_group_info('E:/Resources/PedestrianData/ATC-5/groups_ATC-5.dat'))


root_folder = 'E:/Resources/PedestrianData/'
#一个文件拆成一个文件夹中的多个文件
subfolders = ['ATC-1','ATC-2','ATC-3','ATC-4','ATC-5','ATC-6']
for sf in subfolders:
#     先处理组文件
    group_file = 'groups_' + sf + '.dat'
    ped_to_group = build_group_info(root_folder + sf + '/' + group_file)
    # 存成JSON
    group_json_file = 'groups_' + sf + '.json'
    print('wrting ' + group_json_file)
    with open(group_json_file, 'w') as fp:
        fp.write(json.dumps(ped_to_group))

    for file in os.listdir(root_folder+sf):
        # 处理总的数据文件
        if file.split('.')[-1] == 'csv':
            output_dir_name = file.split('.')[0]
            if not os.path.exists(output_dir_name):
                os.mkdir(output_dir_name)
            filename = root_folder+sf+'/'+file
            print('processing '+filename)

            df = pd.read_csv(filename, header=None, names=['Time','PedID','X','Y','Z','Vel','VelDir','FacDir'])
#             df['GroupID'] = df['PedID'].apply(lambda x: ped_to_group[x] if x in ped_to_group else 0)
            # 取不同人的id进行导出
            for pid in df['PedID'].unique():
                tmp_df = df[df['PedID'] == pid]
                output_filename = output_dir_name + '/' + str(pid) + '.csv'
                tmp_df.to_csv(output_filename, index=False) # 要列名
#                 tmp_df.to_csv(output_filename, header=False, index=False) # 不要列名

