/*
* Utility 1.1
*
* AUTHORS.txt Ryanding
*
* Copyright (c) 2010 - 2012 Ryanding
*/

//alert验证信息
function CheckFormMessage(formId) {
    var str = "";
    if (typeof formId == "undefined") {
        $.each($("label.error"), function (i, v) {
            if ($(v).css("display") != "none")
                str += $(v).text() + " \n";
        });
    } else {
        $.each($("#" + formId).find("label.error"), function (i, v) {
            if ($(v).css("display") != "none")
                str += $(v).text() + " \n";
        });
    }

    alert(str);
}

//根据索引删除数组元素
Array.prototype.remove = function (dx) {
    if (isNaN(dx) || dx > this.length) { return false; }
    this.splice(dx, 1);
}

//字符串处理
String.format = function () {
    if (arguments.length == 0)
        return "";
    var str = arguments[0];
    for (var i = 1; i < arguments.length; i++) {
        var re = new RegExp('\\{' + (i - 1) + '\\}', 'gm');
        str = str.replace(re, arguments[i] == null ? "" : arguments[i]);
    }
    return str;
}
//string builder
function StringBuilder(value) {
    this.strings = new Array("");
    this.append(value);
}

StringBuilder.prototype.append = function (value) {
    if (value) {
        this.strings.push(value);
    }
}

StringBuilder.prototype.clear = function () {
    this.strings.length = 1;
}

StringBuilder.prototype.toString = function () {
    return this.strings.join("");
}
//数组最大值获取
Array.max = function (array) {
    return Math.max.apply(Math, array);
};

//数组最小值获取
Array.min = function (array) {
    return Math.min.apply(Math, array);
};
//截取字符串
function substring(values, length) {
    if (values == "" || values == "") {
        return "";
    }
    var reg = /<.*?>/g; //过滤html正则表达式
    values = values.replace(reg, "");
    if (values.length <= length) {
        return values;
    }
    return values.substring(0, length) + "...";
}

function getDate() {
    var d = new Date()
    var vYear = d.getFullYear()
    var vMon = d.getMonth() + 1
    var vDay = d.getDate()
    var h = d.getHours();
    var m = d.getMinutes();
    var se = d.getSeconds();
    s = vYear + "-" + (vMon < 10 ? "0" + vMon : vMon) + "-" + (vDay < 10 ? "0" + vDay : vDay) + " " + (h < 10 ? "0" + h : h) + ":" + (m < 10 ? "0" + m : m) + ":" + (se < 10 ? "0" + se : se);
    return s;
}

var jqGridCrossColor = function (gridName) {
    var n = $('#' + gridName).getGridParam('records');
    for (i = 0; i < n; i++) {
        //jqgrid的行数是从1开始算起
        var p = i + 1;
        if (p % 2 == 0) {
//            $('#' + gridName + ' tr').eq(p - 1).css("background-color", "#FFDDDB");
            $('#' + gridName + ' tr').eq(p - 1).addClass("jqodd");
        }
    }
}

        //去掉字串左边的空格          
        function lTrim(str) {
            if (str.charAt(0) == " ") {
                //如果字串左边第一个字符为空格                     
                str = str.slice(1);
                //将空格从字串中去掉                  
                //str = str.substring(1, str.length);                  
                str = lTrim(str);
                //递归调用              
            }
            return str;
        }

        //去掉字串右边的空格          
        function rTrim(str) {
            var iLength;
            iLength = str.length;
            if (str.charAt(iLength - 1) == " ") {
                //如果字串右边第一个字符为空格                  
                str = str.slice(0, iLength - 1);
                //将空格从字串中去掉                  
                //这一句也可改成 str = str.substring(0, iLength - 1);                  
                str = rTrim(str);
                //递归调用              
            }
            return str;
        }
        //去掉字串两边的空格
        function trim(str) { return lTrim(rTrim(str)) };

        //获取浏览器高度
        function getBrowserWidthHeight() {
            var intH = 0;
            var intW = 0;
            if (typeof window.innerWidth == 'number') {
                intH = window.innerHeight;
                intW = window.innerWidth;
            }
            else if (document.documentElement &&
                  (document.documentElement.clientWidth ||
                  document.documentElement.clientHeight)) {
                intH = document.documentElement.clientHeight;
                intW = document.documentElement.clientWidth;
            }
            else if (document.body &&
                (document.body.clientWidth || document.body.clientHeight)) {
                if (document.body.scrollHeight > document.body.clientHeight) {
                    intH = document.body.scrollHeight;
                    intW = document.body.scrollWidth;
                }
                else {
                    intH = document.body.clientHeight;
                    intW = document.body.clientWidth;
                }
            }

            return { width: parseInt(intW), height: parseInt(intH) };
        }