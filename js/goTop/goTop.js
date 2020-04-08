$(window).load(function () {
    var $win = $(window),
			$ad = $('#abgne_float_Top').css('opacity', 0).show(), // 讓廣告區塊變透明且顯示出來
			_width = $ad.width(),
			_height = $ad.height(),
			_diffY = 20, _diffX = 20, // 距離右及下方邊距
			_moveSpeed = 800; // 移動的速度

    // 先把 #abgne_float_Top 移動到定點
    $ad.css({
        top: $(document).height(), // 取得網頁高度
        left: $win.width() - _width - _diffX,
        opacity: 1
    });

    // 幫網頁加上 scroll 及 resize 事件
    $win.bind('scroll resize', function () {
        var $this = $(this);

        // 控制 #abgne_float_Top 的移動
        $ad.stop().animate({
            top: $this.scrollTop() + $this.height() - _height - _diffY, //置於下方
            left: $this.scrollLeft() + $this.width() - _width - _diffX
        }, _moveSpeed);
    }).scroll(); // 觸發一次 scroll()

    // 關閉廣告
    $('#abgne_float_Top .abgne_close_Top').click(function () {
        $ad.hide();
    });

    //GoTop按鈕
    $("a.gotoTop").click(function () {
        var $body = (window.opera) ? (document.compatMode == "CSS1Compat" ? $("html") : $("body")) : $("html,body");
        $body.animate({
            scrollTop: 0
        }, 1000);

        return false;
    });
    $("a.gotoBottom").click(function () {
        var $body = (window.opera) ? (document.compatMode == "CSS1Compat" ? $("html") : $("body")) : $("html,body");
        $body.animate({
            scrollTop: $(document).height()
        }, 1000);
        
        return false;
    });
});
