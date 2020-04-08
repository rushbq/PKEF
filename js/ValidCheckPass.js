//驗証目前的控制項是否通過 (validationGroup = 驗証群組名稱)
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