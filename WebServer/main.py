# coding:utf-8
from flask import Flask, session, redirect, url_for, escape, request, jsonify
import db
import os
import os.path
import sys
import json

PROJECT_ROOT = os.path.dirname(os.path.realpath(__file__))
UPLOAD_FOLDER = os.path.join(PROJECT_ROOT, 'logs')
app = Flask(__name__)
app.config['UPLOAD_FOLDER'] = UPLOAD_FOLDER

@app.route('/')
def hello_world():
    return jsonify({'hello':'world'})

@app.route('/login', methods=['POST'])
def login_handler():
    if request.method == 'POST':
        result = dict()
        username = request.form['username']
        password = request.form['password']
        ret = db.check_user(username, password)
        # error
        if ret[0] != 0:
            result['code'] = 1
            result['msg'] = 'error'
        else:
            result['code'] = 0
            result['msg'] = 'passed'
            result['groupID'] = ret[1][2]
            result['groupSize'] = ret[1][3]
            result['leader'] = ret[1][4]
            result['gender'] = ret[1][5]
            result['speed'] = ret[1][6]
            result['knowTruth'] = ret[1][7]
        return jsonify(result)

@app.route('/updateResults',methods=['POST'])
def upload_results():
    if request.method == 'POST':
        result = dict()
        groupID = int(request.form['groupID'])
        username = request.form['username']
        userEvacTime = float(request.form['userEvacTime'])
        groupEvacTime = float(request.form['groupEvacTime'])
        ret = db.write_evac_time(groupID, username, userEvacTime, groupEvacTime)
        if ret:#success
            result['code'] = 0
        else:
            result['code'] = 1
        return jsonify(result)

@app.route('/uploadLogfile',methods=['POST'])
def upload_logfile():
    if request.method == 'POST':
        result = dict()
        try:
            filename = request.form['fileName']
            fileData = request.form['fileData']
            filePath = os.path.join(app.config['UPLOAD_FOLDER'], filename)
            with open(filePath, 'w') as fp:
                fp.writelines(fileData)
            result['code'] = 0
        except Exception,err:
            sys.stderr.write('ERROR: %s\n' % str(err))
            result['code'] = 1
        return jsonify(result)

@app.route('/uploadQuestionaire',methods=['POST'])
def upload_questionair():
    if request.method == 'POST':
        result = dict()
        try:
            dataStr = request.form['data']
            answers = dataStr.split(';')
            # 取用户名生成密码
            username = answers[1]
            password = abs(hash(username))%(10**4)
            # 把密码加到数组最后一个
            answers.append(password)
            # 插入数据库
            if db.insert_user_answers(answers):
                result['code'] = 0
                result['password'] = password
            else:
                result['code'] = 1
        except Exception,err:
            sys.stderr.write('ERROR: %s\n' % str(err))
            result['code'] = 1

        return jsonify(result)

@app.route('/checkUsername',methods=['GET'])
def check_username():
    if request.method == 'GET':
        result = dict()
        try:
            username = request.args.get('username')
            if db.check_username(username):# valid
                result['code'] = 0
            else:
                result['code'] = 1
        except Exception,err:
            sys.stderr.write('ERROR: %s\n' % str(err))
            result['code'] = 1
        return jsonify(result)

@app.route('/knowTruthProb',methods=['GET'])
def update_know_truth_prob():
    if request.method == 'GET':
        result = dict()
        probs = [0.0,0.333,0.667,1.0]
        try:
            prob = int(request.args.get('prob'))
            if db.update_know_truth(probs[prob]):
                result['code'] = 0
                result['message'] = 'success'
            else:
                result['code'] = 1
        except Exception,err:
            sys.stderr.write('ERROR: %s\n' % str(err))
            result['code'] = 1
        return jsonify(result)

if __name__ == '__main__':
    app.run(host='127.0.0.1', debug=True, port=9031)
