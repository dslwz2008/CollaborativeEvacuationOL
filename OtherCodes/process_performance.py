# coding:utf-8

import io
import psutil
import time
from datetime import datetime
import shutil

# get from taskmgr
target_pid = 12896

# Example Data:
# {'以太网': snetio(bytes_sent=2693730598, bytes_recv=17211723862, packets_sent=4454361, packets_recv=0, errin=0, errout=0, dropin=0, dropout=0),
# '本地连接* 4': snetio(bytes_sent=0, bytes_recv=0, packets_sent=0, packets_recv=0, errin=0, errout=0, dropin=0, dropout=0),
# 'VMware Network Adapter VMnet1': snetio(bytes_sent=4393, bytes_recv=22, packets_sent=19, packets_recv=22, errin=0, errout=0, dropin=0, dropout=0),
# 'VMware Network Adapter VMnet8': snetio(bytes_sent=6511, bytes_recv=23, packets_sent=19, packets_recv=23, errin=0, errout=0, dropin=0, dropout=0),
# 'WLAN': snetio(bytes_sent=137610357, bytes_recv=1970745088, packets_sent=886502, packets_recv=1426192, errin=0, errout=0, dropin=0, dropout=0),
# 'Loopback Pseudo-Interface 1': snetio(bytes_sent=0, bytes_recv=0, packets_sent=0, packets_recv=0, errin=0, errout=0, dropin=0, dropout=0),
# '本地连接* 14': snetio(bytes_sent=0, bytes_recv=0, packets_sent=0, packets_recv=0, errin=0, errout=0, dropin=0, dropout=0)}
target_net = 'WLAN'

def main():
    strbuf = io.StringIO()
    strbuf.write('Datetime,CPU Percent,Memory Size,Network Sent,Network Received\n')

    # get process
    for proc in psutil.process_iter():
        if proc.pid == target_pid:
            target_proc = proc
            filename = target_proc.name() + '.csv'

    # init net valus
    netio = psutil.net_io_counters(pernic=True)[target_net]
    last_sent_bytes = netio.bytes_sent
    last_recv_bytes = netio.bytes_recv

    try:
        while target_proc is not None:
            cpu_pcnt = target_proc.cpu_percent() / psutil.cpu_count()
            mem_usage = target_proc.memory_info().rss/2**20
            netio = psutil.net_io_counters(pernic=True)[target_net]
            cur_sent_bytes = netio.bytes_sent
            cur_recv_bytes = netio.bytes_recv
            sent_bytes = cur_sent_bytes - last_sent_bytes
            recv_bytes = cur_recv_bytes - last_recv_bytes
            full_record = '{0},{1},{2},{3},{4}\n'.format(datetime.now().isoformat(),cpu_pcnt,mem_usage,sent_bytes,recv_bytes)
            strbuf.write(full_record)
            last_sent_bytes = cur_sent_bytes
            last_recv_bytes = cur_recv_bytes
            time.sleep(0.5)
    except psutil.NoSuchProcess:
        with open(filename, 'w') as fp:
            strbuf.seek(0)
            shutil.copyfileobj(strbuf, fp)
        print('Finished!')

if __name__ == '__main__':
    main()

