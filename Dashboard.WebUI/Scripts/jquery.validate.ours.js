


//判断是否为电话号码：号码可为7~9位数字，或区号-7~9位数字，或86-区号-7~9位数字，或手机号11位或12位数字
jQuery.validator.addMethod("isPhone", function (value, element) {
    var tel = /(-|\s|\d|\*)$/g;
    return this.optional(element) || (tel.test(value));
}, "wewewe");

//邮编判断
jQuery.validator.addMethod("isZip", function (value, element) {
    var zip = /^[0-9]\d{5}(?!\d)/;
    return this.optional(element) || (zip.test(value));
}, "邮编格式错误");

//折扣判断
jQuery.validator.addMethod("isDiscount", function (value, element) {
    var discount = /(^\d(\.\d+)?$|\d{2,})/g;
    return this.optional(element) || (discount.test(value));
}, "折扣格式错误");

//
jQuery.validator.addMethod("isDecimal", function (value, element) {
    var decimal = /^\d+(\.\d{1,2}?)?$/g;
    return this.optional(element) || (decimal.test(value));
}, "价格错误");

//非零
jQuery.validator.addMethod("notZero", function (value, element) {
    var zero = /[^0(\.0+)?]/g;
    return this.optional(element) || (zero.test(value));
}, "非零");
//整数
jQuery.validator.addMethod("isInteger", function (value, element) {
    var isInt = /^-?\d+$/g;
    return this.optional(element) || (isInt.test(value));
}, "非整数");
//正数
jQuery.validator.addMethod("isPlus", function (value, element) {
    var isPlus = /^\d+$/g;
    return this.optional(element) || (isPlus.test(value));
}, "非正数");

//两位以内小数或正数
jQuery.validator.addMethod("isFloat", function (value, element) {
    var isFloat = /^\d+(\.\d{1,3})?$/g;
    return this.optional(element) || (isFloat.test(value));
}, "");

//12位以内的电话号码
jQuery.validator.addMethod("isCellPhone", function (value, element) {
    var isCellPhone = /^\d{11,12}$/g;
    return this.optional(element) || (isCellPhone.test(value));
}, "");

//电话区号
jQuery.validator.addMethod("isDistrictNum", function (value, element) {
    var isDistrictNum = /^\d{3,4}$/;
    return this.optional(element) || (isDistrictNum.test(value));
}, "区号错误");