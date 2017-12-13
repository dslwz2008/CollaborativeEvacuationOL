/**
 * Item Name :
 * Creater : peijiqiu
 * Email : peijiqiu@gmail.com
 * Created Date : 17/8/20.
 */
/**
 * Item Name :
 * Creater : peijiqiu
 * Email : peijiqiu@gmail.com
 * Created Date : 17/8/19.
 */
var app = new Vue({
  el: '#app',
  data: {

  },
  methods: {
    getUrlKey:function(name){
      return decodeURIComponent((new RegExp('[?|&]'+name+'='+'([^&;]+?)(&|#|;|$)').exec(location.href)||[,""])[1].replace(/\+/g,'%20'))||null;
    }
  },
  computed: {
    // 仅读取，值只须为函数
    username: function () {
      return this.getUrlKey('name')
    },
    userpwd: function () {
      return this.getUrlKey('pwd')
    }
  }
});