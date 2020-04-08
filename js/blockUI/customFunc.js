/*
 *** 這是自訂BlockUI的Css ***
 * 一般轉頁或Postback 會自動解除BlockUI
 * 若要解除就使用 $.unblockUI()
*/

/*
    [一號機] A型
    使用時機:當有表單驗證時, 自訂顯示文字
    欄位:
      validGroup = ValidationGroup名稱
      inputMessage = 你想說的話
*/
function blockBox1(validGroup, inputMessage) {
    if (Page_ClientValidate_AllPass(validGroup)) {
        $.blockUI({
            message: inputMessage,
            css: {
                border: 'none',
                padding: '15px',
                backgroundColor: '#000',
                '-webkit-border-radius': '10px',
                '-moz-border-radius': '10px',
                opacity: .8,
                color: '#fff'
            }
        });

    }
}

/*
    [一號機] B型
    使用時機:當有表單驗證時, 用內建的提示文字(Please wait)
    欄位:
      validGroup = ValidationGroup名稱
*/
function blockBox1_NoMsg(validGroup) {
    if (Page_ClientValidate_AllPass(validGroup)) {
        $.blockUI({
            css: {
                border: 'none',
                padding: '15px',
                backgroundColor: '#000',
                '-webkit-border-radius': '10px',
                '-moz-border-radius': '10px',
                opacity: .8,
                color: '#fff'
            }
        });

    }
}

/*
    [二號機] A型
    使用時機:直接使用BlockUI, 自訂顯示文字
    欄位:
      inputMessage = 你想說的話
*/
function blockBox2(inputMessage) {
    $.blockUI({
        message: inputMessage,
        css: {
            border: 'none',
            padding: '15px',
            backgroundColor: '#000',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            opacity: .8,
            color: '#fff'
        }
    });
}

/*
    [二號機] B型
    使用時機:直接使用BlockUI, 用內建的提示文字(Please wait)
*/
function blockBox2_NoMsg() {
    $.blockUI({
        css: {
            border: 'none',
            padding: '15px',
            backgroundColor: '#000',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            opacity: .8,
            color: '#fff'
        }
    });
}



/*
    驗証目前的控制項是否通過 (validationGroup = 驗証群組名稱)
*/
function Page_ClientValidate_AllPass(validationGroup) {
    if (typeof (Page_Validators) == "undefined") { return true; }
    var i;
    for (i = 0; i < Page_Validators.length; i++) {
        var val = Page_Validators[i];
        val.isvalid = true;
        if ((typeof (val.enabled) == "undefined" || val.enabled != false) && IsValidationGroupMatch(val, validationGroup)) {
            if (typeof (val.evaluationfunction) == "function") {
                val.isvalid = val.evaluationfunction(val);
            }
        }
    }
    ValidatorUpdateIsValid();
    return Page_IsValid;
}