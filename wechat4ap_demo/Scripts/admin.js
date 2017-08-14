
wx.config({
    debug: false,
    appId: 'wx75ee8ceec612733b',
    timestamp: TIMESTAMP,
    nonceStr: NONCESTR,
    signature: SIGNATURE,
    jsApiList: [
      'checkJsApi',
      'onMenuShareTimeline',
      'onMenuShareAppMessage',
      'onMenuShareQQ',
      'onMenuShareWeibo',
      'hideMenuItems',
      'showMenuItems',
      'hideAllNonBaseMenuItem',
      'showAllNonBaseMenuItem',
      'translateVoice',
      'startRecord',
      'stopRecord',
      'onRecordEnd',
      'playVoice',
      'pauseVoice',
      'stopVoice',
      'uploadVoice',
      'downloadVoice',
      'chooseImage',
      'previewImage',
      'uploadImage',
      'downloadImage',
      'getNetworkType',
      'openLocation',
      'getLocation',
      'hideOptionMenu',
      'showOptionMenu',
      'closeWindow',
      'scanQRCode',
      'chooseWXPay',
      'openProductSpecificView',
      'addCard',
      'chooseCard',
        'openCard',
       'getLocalImgData'
    ]
});


var images = {
    localId: [],//微信返回的本地id列表
    serverId: [],//微信返回的服务器id列表


};
ioslocId = [];//用于兼容ios的本地id列表 图片是base64格式的



function wxChooseImage() {
    wx.chooseImage({
        count: 1, // 默认9
        sizeType: ['original', 'compressed'], // 可以指定是原图还是压缩图，默认二者都有
        sourceType: ['album', 'camera'], // 可以指定来源是相册还是相机，默认二者都有
        success: function (res) {
            var localIds = res.localIds; // 返回选定照片的本地ID列表，localId可以作为img标签的src属性显示图片
            ioslocId = localIds;
            
            for (var i = 0; i < localIds.length; i++) {
                var localId = localIds[i];
                addImg(localId);
            }  
        }
    });

    return '';
}


function wxUploadImage() {
    for (var i = 0; i < ioslocId.length; i++) {
        var lId = ioslocId[i];
        wx.getLocalImgData({
            localId: lId, // 图片的localID
            success: function (res) {
                var localData = res.localData; // localData是图片的base64数据，可以用img标签显示
                
                //$("#base64Label").text("base64Data:" + localData);

                if (localData.indexOf('base64') > 0)
                    $("#uploadedImg").attr('src', localData);
                else
                    $("#uploadedImg").attr('src', 'data:image/jpeg;base64,' + localData);
            }
        });
    } 


    
    return '';
}


function addImg(src) {
    var col = $('#imgsdiv');
    col.addClass('col-xs-6 col-md-3');
    var a = $('<a />');
    //a.addClass('thumbnail');
    var img = $('<img />');
    img.attr('src', src);
    img.attr('style', 'width:100px;height:70px;');
    a.append(img).append(src);
    col.append(a);
    $('.row').append(col);
}  
