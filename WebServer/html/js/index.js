/**
 * Item Name :
 * Creater : peijiqiu
 * Email : peijiqiu@gmail.com
 * Created Date : 17/8/19.
 */
var app = new Vue({
  el: '#app',
  data: {
    title: '在线虚拟协同疏散实验报名及调查问卷',
    loginname: '',
    questions: [
      {
        title: '请选择方便参与实验的时间(可多选)：',
        type: 'checkbox',
        val:[],
        name: 'ques0',
        options: [
          {
            id: 'ques0-1',
            text: '10:00',
            val: '10'
          },
          {
            id: 'ques0-2',
            text: '11:00',
            val: '11'
          },
          {
            id: 'ques0-3',
            text: '12:00',
            val: '12'
          },
          {
            id: 'ques0-4',
            text: '13:00',
            val: '13'
          },
          {
            id: 'ques0-5',
            text: '14:00',
            val: '14'
          },
          {
            id: 'ques0-6',
            text: '15:00',
            val: '15'
          },
          {
            id: 'ques0-7',
            text: '16:00',
            val: '16'
          },
          {
            id: 'ques0-8',
            text: '17:00',
            val: '17'
          },
          {
            id: 'ques0-9',
            text: '18:00',
            val: '18'
          },
          {
            id: 'ques0-10',
            text: '19:00',
            val: '19'
          },
          {
            id: 'ques0-11',
            text: '20:00',
            val: '20'
          }

        ]
      },
      {
        title: '用户名(英文与数字,长度6-20)：',
        type: 'text',
        val:'',
        checkuser: true
      },
      {
        title: '您的年龄：',
        type: 'select',
        val:'',
        options: [
          {
            text: '<=15',
            val: '1'
          },
          {
            text: '16-20',
            val: '2'
          },
          {
            text: '21-25',
            val: '3'
          },
          {
            text: '26-30',
            val: '4'
          },
          {
            text: '31-35',
            val: '5'
          },
          {
            text: '36-40',
            val: '6'
          },
          {
            text: '41-45',
            val: '7'
          },
          {
            text: '46-50',
            val: '8'
          },
          {
            text: '>50',
            val: '9'
          }
        ]
      },
      {
        title: '您的性别：',
        type: 'radio',
        name: 'gender',
        val: '',
        options: [
          {
            id: 'male',
            text: '男',
            val: '1'
          },
          {
            id: 'female',
            text: '女',
            val: '2'
          }
        ]
      },
      {
        title: '在日常工作和生活中，你是否经常与4-6人结伴行走？',
        type: 'radio',
        name: 'ques1',
        val: '',
        options: [
          {
            id: 'ques1-1',
            text: '经常',
            val: '1'
          },
          {
            id: 'ques1-2',
            text: '偶尔',
            val: '2'
          },
          {
            id: 'ques1-3',
            text: '极少',
            val: '3'
          },
          {
            id: 'ques1-4',
            text: '从不',
            val: '4'
          }
        ]
      },
      {
        title: '在日常工作和生活中，你是否经常与7人及以上结伴行走？',
        type: 'radio',
        name: 'ques2',
        val: '',
        options: [
          {
            id: 'ques2-1',
            text: '经常',
            val: '1'
          },
          {
            id: 'ques2-2',
            text: '偶尔',
            val: '2'
          },
          {
            id: 'ques2-3',
            text: '极少',
            val: '3'
          },
          {
            id: 'ques2-4',
            text: '从不',
            val: '4'
          }
        ]
      },
      {
        title: '你认为，有多个成员（如5人-7人）的同伴群在行走过程中，需要考虑的因素有哪些？（多选）',
        type: 'checkbox',
        name: 'ques3',
        val: [],
        options: [
          {
            id: 'ques3-1',
            text: '每个成员都有可交流的邻近同伴，不被孤立',
            val: '1'
          },
          {
            id: 'ques3-2',
            text: '保持团队紧凑性',
            val: '2'
          },
          {
            id: 'ques3-3',
            text: '形成易于同伴群前进的结构',
            val: '3'
          },
          {
            id: 'ques3-4',
            text: '保持一致的速度',
            val: '4'
          }
        ]
      },
      {
        title: '你认为，在结伴行走过程中，如何保持同伴群紧凑性？',
        type: 'radio',
        name: 'ques4',
        val: '',
        options: [
          {
            id: 'ques4-1',
            text: '所有成员向同伴群中心靠拢',
            val: '1'
          },
          {
            id: 'ques4-2',
            text: '所有成员向某个成员靠拢',
            val: '2'
          },
          {
            id: 'ques4-3',
            text: '保持同排成员紧凑，前后排紧凑性由后排成员控制',
            val: '3'
          },
          {
            id: 'ques4-4',
            text: '只保持同排成员紧凑，无需保持前后排成员紧凑',
            val: '4'
          }
        ]
      },
      {
        title: '日常行走过程中，你与同伴交换位置的现象是否普遍？',
        type: 'radio',
        name: 'ques5',
        val: '',
        options: [
          {
            id: 'ques5-1',
            text: '是',
            val: '1'
          },
          {
            id: 'ques5-1',
            text: '否',
            val: '2'
          }
        ]
      },
      {
        title: '你与同伴交换位置的原因是什么？（多选）',
        type: 'checkbox',
        name: 'ques6',
        val: [],
        options: [
          {
            id: 'ques6-1',
            text: '为了同伴群内的舒适度',
            val: '1'
          },
          {
            id: 'ques6-2',
            text: '由于行走前方有障碍物',
            val: '2'
          },
          {
            id: 'ques6-3',
            text: '重新出发（如光顾商店后）改变了相对位置',
            val: '3'
          },{
            id: 'ques6-4',
            text: '其他',
            val: '4'
          }
        ]
      },
      {
        title: '在行走过程中，前进方向上遇到一个多人同伴群（如旅游团），你会怎么做？',
        type: 'radio',
        name: 'ques7',
        val: '',
        options: [
          {
            id: 'ques7-1',
            text: '与这个同伴群内离自己最近的人保持安全距离即可',
            val: '1'
          },
          {
            id: 'ques7-2',
            text: '尽可能与这个同伴群保持最大距离',
            val: '2'
          },
          {
            id: 'ques7-3',
            text: '不受影响，保持原来的路线行走',
            val: '3'
          },
          {
            id: 'ques7-4',
            text: '停下等待该同伴群离开，再继续行走',
            val: '4'
          }
        ]
      },
      {
        title: '遇到的同伴群人数越大，您是否越想远离？',
        type: 'radio',
        name: 'ques8',
        val: '',
        options: [
          {
            id: 'ques8-1',
            text: '是',
            val: '1'
          },
          {
            id: 'ques8-2',
            text: '否',
            val: '2'
          }
        ]
      },
      {
        title: '你认为在现实场景中，哪些地物特征容易被感知到？（多选）',
        type: 'checkbox',
        name: 'ques9',
        val: [],
        options: [
          {
            id: 'ques9-1',
            text: '形状',
            val: '1'
          },
          {
            id: 'ques9-2',
            text: '大小',
            val: '2'
          },
          {
            id: 'ques9-3',
            text: '纹理材质',
            val: '3'
          },
          {
            id: 'ques9-4',
            text: '颜色',
            val: '4'
          },
          {
            id: 'ques9-5',
            text: '结构',
            val: '5'
          },
          {
            id: 'ques9-6',
            text: '距离',
            val: '6'
          },
          {
            id: 'ques9-7',
            text: '文字',
            val: '7'
          },
          {
            id: 'ques9-8',
            text: '光线阴影',
            val: '8'
          },
          {
            id: 'ques9-9',
            text: '其他',
            val: '9'
          }
        ]
      },
      {
        title: '在结伴行走过程中，如果你的同伴行走较慢，你会：',
        type: 'radio',
        name: 'ques10',
        val: '',
        options: [
          {
            id: 'ques10-1',
            text: '停下等待',
            val: '1'
          },
          {
            id: 'ques10-2',
            text: '放慢速度等待',
            val: '2'
          },
          {
            id: 'ques10-3',
            text: '保持原有速度，希望同伴赶上来',
            val: '3'
          }
        ]
      },
      {
        title: '在结伴行走过程中，如果你的同伴行走较快，你会：',
        type: 'radio',
        name: 'ques11',
        val: '',
        options: [
          {
            id: 'ques11-1',
            text: '全速前进赶上同伴',
            val: '1'
          },
          {
            id: 'ques11-2',
            text: '稍微加快步伐追赶同伴',
            val: '2'
          },
          {
            id: 'ques11-3',
            text: '保持原有速度，希望同伴等待',
            val: '3'
          }
        ]
      },
      {
        title: '结伴同行时，你一般与同伴保持多远的距离？',
        type: 'radio',
        name: 'ques12',
        val: '',
        options: [
          {
            id: 'ques12-1',
            text: '<=0.5m',
            val: '1'
          },
          {
            id: 'ques12-2',
            text: '0.5-1.0m',
            val: '2'
          },
          {
            id: 'ques12-3',
            text: '1-1.5m',
            val: '3'
          },
          {
            id: 'ques12-4',
            text: '>1.5m',
            val: '4'
          }
        ]
      },
      {
        title: '陌生人与你有多远距离时，你会感觉不舒服？',
        type: 'radio',
        name: 'ques13',
        val: '',
        options: [
          {
            id: 'ques13-1',
            text: '<=0.5m',
            val: '1'
          },
          {
            id: 'ques13-2',
            text: '0.5-1.0m',
            val: '2'
          },
          {
            id: 'ques13-3',
            text: '1-1.5m',
            val: '3'
          },
          {
            id: 'ques13-4',
            text: '>1.5m',
            val: '4'
          }
        ]
      },
      {
        title: '如果你是一个小组的队长，你习惯走在队中哪个位置？',
        type: 'radio',
        name: 'ques14',
        val: '',
        options: [
          {
            id: 'ques14-1',
            text: '队伍最前面',
            val: '1'
          },
          {
            id: 'ques14-2',
            text: '队伍中间',
            val: '2'
          },
          {
            id: 'ques14-3',
            text: '队伍末尾',
            val: '3'
          }
        ]
      },
      {
        title: '如果你是一个小组的队员，你习惯走在队中哪个位置？',
        type: 'radio',
        name: 'ques15',
        val: '',
        options: [
          {
            id: 'ques15-1',
            text: '队伍最前面',
            val: '1'
          },
          {
            id: 'ques15-2',
            text: '队伍中间',
            val: '2'
          },
          {
            id: 'ques15-3',
            text: '队伍末尾',
            val: '3'
          },
          {
            id: 'ques15-4',
            text: '紧跟队长',
            val: '4'
          }
        ]
      },
      {
        title: '在火灾、地震等紧急情况下，如果你是一个小组的队长，但是对周围环境并不熟悉，你会：',
        type: 'radio',
        name: 'ques16',
        val: '',
        options: [
          {
            id: 'ques16-1',
            text: '尝试带领小组找出路',
            val: '1'
          },
          {
            id: 'ques16-2',
            text: '随大流逃生',
            val: '2'
          }
        ]
      },
      {
        title: '在火灾、地震等紧急情况下，如果你所在小组的逃生方向与大多数人相反，你会：',
        type: 'radio',
        name: 'ques17',
        val: '',
        options: [
          {
            id: 'ques17-1',
            text: '坚持小组的前进方向',
            val: '1'
          },
          {
            id: 'ques17-2',
            text: '脱离小组，跟随大多数人',
            val: '2'
          },
          {
            id: 'ques17-3',
            text: '说服小组成员一起跟随大多数人',
            val: '3'
          },
          {
            id: 'ques17-4',
            text: '犹豫不决，原地观察',
            val: '4'
          }
        ]
      },
      {
        title: '在火灾、地震等紧急情况下，如果你是一个小组的队长，当你与其他队员意见不一致时，你会：',
        type: 'radio',
        name: 'ques18',
        val: '',
        options: [
          {
            id: 'ques18-1',
            text: '坚持自己的意见',
            val: '1'
          },
          {
            id: 'ques18-2',
            text: '相信同伴',
            val: '2'
          },
          {
            id: 'ques18-3',
            text: '犹豫不决',
            val: '3'
          }
        ]
      },
      {
        title: '在火灾、地震等紧急情况下，如果你是一个小组的队员，当你与其他队员意见不一致时，你会：',
        type: 'radio',
        name: 'ques19',
        val: '',
        options: [
          {
            id: 'ques19-1',
            text: '坚持自己的意见',
            val: '1'
          },
          {
            id: 'ques19-2',
            text: '相信同伴',
            val: '2'
          },
          {
            id: 'ques19-3',
            text: '相信队长',
            val: '3'
          },
          {
            id: 'ques19-4',
            text: '犹豫不决',
            val: '4'
          }
        ]
      },
      {
        title: '在火灾、地震等紧急情况下，如果你行走在同伴的前面，你是否会等待同伴？',
        type: 'radio',
        name: 'ques20',
        val: '',
        options: [
          {
            id: 'ques20-1',
            text: '是',
            val: '1'
          },
          {
            id: 'ques20-2',
            text: '否',
            val: '2'
          }
        ]
      },
      {
        title: '在火灾、地震等紧急情况下，如果你行走在同伴的后面，你是否会希望同伴等待？',
        type: 'radio',
        name: 'ques21',
        val: '',
        options: [
          {
            id: 'ques21-1',
            text: '是',
            val: '1'
          },
          {
            id: 'ques21-2',
            text: '否',
            val: '2'
          }
        ]
      }
    ],
    validuser: false,
    checkUserUrl: 'http://119.23.128.14:9931/checkUsername',
    uploadQuesUrl: 'http://119.23.128.14:9931/uploadQuestionaire'
  },
  methods: {
    checkUserName: function(name, e) {
      var root = this;
      var reg = /^[\da-zA-Z]{6,20}/;
      if(!reg.test(name)) {
        alert('用户名采用英文与数字,长度6-20');
        e.target.style.borderColor = 'red';
        root.validuser = false;
        return
      }
      this.$http.get(this.checkUserUrl, {username:name}).then(function(res){
        if(!res.body.code == 0) {
          alert('该用户名已存在,请更换');
          e.target.style.borderColor = 'red';
          root.validuser = false;
        } else {
          e.target.style.borderColor = '#dbdbdb';
          root.loginname = name;
          root.validuser = true;
        }
      },function(res){
        alert('error');
      });
    },

    submitQuestion: function() {
      if(!this.validuser) {
        alert('用户名已存在,请重新填写');
        return;
      }

      var flag = true;
      var dataArr = [];
      var data = '';
      for(var i=0; i < this.questions.length; i++) {

        if(this.questions[i].type === 'radio' || this.questions[i].type === 'text' || this.questions[i].type === 'select') {
          var answer = this.questions[i].val.replace(/(^\s*)|(\s*$)/g, "");
          if(!answer) {
            flag = false;
            break;
          } else {
            dataArr.push(answer);
          }
        } else if(this.questions[i].type === 'checkbox') {
          if(!this.questions[i].val.length) {
            flag = false;
            break;
          } else {
            dataArr.push(this.questions[i].val.join(','));
          }
        }
      }


      if(!flag) {
        alert('您还有未填写的试题,请填写完整');
      } else {
        this.$http.post(this.uploadQuesUrl, {"data":dataArr.join(';')},
          {emulateJSON:true,headers: {'Content-Type': 'application/x-www-form-urlencoded'}}).then(function(res){
          if(res.body.code == 0) {
            location.href = 'result.html?name='+this.loginname+'&pwd='+ res.body.password;
          } else {
            alert('error')
          }
        },function(res){
          alert('error');
        });
      }
    }
  }
});