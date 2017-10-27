/*
Grid内联想控件
*/
(function ($) {
    var _selInput = null;
    $.fn.extend({
        autoComplete: function (options) {
            var gridId = $(this).attr("id");
            return this.each(function () {
                $(this).undelegate("input[AutoCode=" + options.AutoCode + "]", "focus");
                $(this).delegate("input[AutoCode=" + options.AutoCode + "]", "focus", function (p) {
                    var obj = p.target;
                    if ($(p.target).attr("id").length > 0) return;
                    var index = $(p.target).parent().parent().index();
                    var controlId = gridId + options.AutoCode + "___" + parseInt(Math.random() * 1000 + 1);
                    $(p.target).attr("id", controlId);

                    controlJson.initInput({
                        displayFields: options.displayFields,
                        displayLabels: options.displayLabels,
                        width: 450,
                        queryFieldName: options.queryFieldName,
                        isRefalshFromDb: "0",
                        url: options.url,
                        isDefinedRecordsPerPage: "0",
                        recordsPerPage: "5",
                        imagePath: "../Content/Images/AutoImage",
                        controlId: controlId,
                        fillColumns: options.fillColumns
                    });
                });
            });
        },
        getCurrentInput: function () {
            return _selInput;
        }
    })
})(jQuery);

(function ($) {
    var _selInput = null;
    $.fn.extend({
        delRow: function (options) {
            var gridObj = $(this);
            return this.each(function () {
                gridObj.delRowData(gridObj.getGridParam('selrow'));
                $.each((gridObj.find("tr")), function (i, v) {
                    $(this).attr("id", i);
                });
            });
        },
        delArrRow: function (arrIds) {
            var gridObj = $(this);
            return this.each(function () {
                $.each(arrIds, function (i, v) {
                    gridObj.delRowData(v);
                });
                $.each((gridObj.find("tr")), function (i, v) {
                    $(this).attr("id", i);
                });
            });
        },
        getCurrentInput: function () {
            return _selInput;
        },
        disableDragable:function(){
           var gridObj = $(this);
           /*
           var sibings = gridObj.parent().parent().siblings();
             sibings.eq(1).find("table").find("th").each(function(i,v){
              //$(v).draggable();
              $(v).draggable('disable');
              //draggable( "option", "disabled", false);
             });*/
             var sibings = gridObj.parent().parent().siblings();
             sibings.eq(1).find("table>thead>tr").unbind();
        }
    })
})(jQuery);

var Util = {
    Lang: "English"
};

var initdatePicker = function (fmt) {
    WdatePicker({ lang: Util.Lang,dateFmt: fmt });
}
