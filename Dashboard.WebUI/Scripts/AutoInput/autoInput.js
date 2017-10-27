var objControlJson = null;
var objControlTypeJson = null;
var timeId;

var controlJson = {
    initGlobalObjs: function () {
        if (objControlJson == null) {
            objControlJson = new Array();
            objControlTypeJson = new Array();
        }
    },
    initInput: function (config, buttonid) {
        this.initGlobalObjs();
        var inputConfig = new InputN(config);
        objControlJson[config.controlId] = (inputConfig);
        objControlTypeJson[config.controlId] = ("input");
        return objControlJson;
    },
    initMultiInput: function (config) {
        this.initGlobalObjs();
        var inputConfig = new MultiInputN(config);
    },
    createFrame: function (config) {
        this.initGlobalObjs();
        var n = controlJson.popupName;
        if (!objControlTypeJson[n]) {
            objControlJson[n] = new FrameN(config);
            objControlTypeJson[n] = "iFrame";
        } else {
            objControlJson[n].setCurrent(config);
        }
        objControlJson[n].Display();
    }
};
var Utilframe = {
    popupName: "_auto_popup",
    popupTextId: "_auto_popup_text",
    popupUpdateDivId: "_auto_popup_div",
    popupSelectePageRow: "_auto_popup_selectPageRow",
    popupImgDivId: "_auto_popup_div_image_associate"
};
var MultiInputN = function (config) {
    var inputN = null;
    var init = function (config) {
        //获取联动控件 ID组
        var arr = config.controlId.split(";");
        //获取联动控件 赋值联动列名
        var columns = config.fillControls.split(";");
        //获取联动控件 queryFieldNames
        var fieldNames = config.queryFieldName.split(";");
        //循环创建联想控件
        for (var index = 0; index < arr.length; index++) {
            //当前控件ID
            var control = arr[index];
            //设置配置
            config.controlId = control; //输入框ID
            //设置不自动填写 需增加 blur处理函数
            config.stay = false;

            config.queryFieldName = fieldNames[index];

            //联想控件过滤条件函数
            config.whereConditionFunc = function () {
                var obj = "";

                var eleIndex = $.inArray(document.activeElement.id.replace("autoSelectDiv", ""), arr);
                for (var cd = 0; cd < eleIndex; cd++) {
                    var column = fieldNames[cd]; //columns[cd].split(":")[1];
                    obj += column + ":" + $("#" + arr[cd]).val() + ';';
                }
                return obj;
            };
            //执行创建
            controlJson.initInput(config, control);
        }
    };
    init(config);
};
var InputN = function (config) {
    var _s = this;
    var current = new Object();
    var pGrid = null;
    var pInput = null;

    _s.ForceEnter = function () {
        //&& $("#" + pInput.id).val()
        if ($("#" + pGrid.id + ":visible").length > 0 && pGrid.itemContainers.length > 0) {
            //event.keyCode = 13;
            $("#" + pGrid.id).trigger("keyup");
            pGrid.handleDb();
        }
        else {
            UtilglControl.setInputValue(null, current);
            UtilglControl.closeDiv(current, current.controlId);
        }
        UtilglControl.closeLoading(current);
    }

    var init = function (config) {

        var inputId = config.controlId;
        var gridId = "autoSelectDiv" + inputId;

        $("#" + inputId).bind("cut", function (e) {
            alert("Can not cut from these text, try copy if you want.");
            e.preventDefault();
        });


        $("#" + inputId).bind("keydown", function (e) {
            if (e.ctrlKey && e.keyCode == 67) return;
            // alert("you changed");
            if (e.keyCode == 9
              || e.keyCode == 16 || e.keyCode == 37
              || e.keyCode == 38 || e.keyCode == 39
              || e.keyCode == 17)
                return;

            if (config.fillControls && config.fillControls != null) {
                var arr = config.fillControls.split(";");
                for (var index = 0; index < arr.length; index++) {
                    if (arr != "") {
                        var nameValue = arr[index].split(":");
                        if (nameValue.length > 1) {
                            $("#" + nameValue[0]).val("");
                        }
                    }
                }
            }

            if (config.fillColumns && config.fillColumns != null) {
                var arr = config.fillColumns.split(";");
                var gridId = config.controlId;
                var obj = $("#" + config.controlId);
                //var rowId = obj.parent().parent().index();
                var rowIndex = obj.parent().parent().index();
                var rowId = obj.parent().parent().attr("id");
                gridId = gridId.replace(/___\d+/, "");
                gridId = $.trim(gridId.replace(obj.attr("AutoCode"), ""));
                var gridObj = $("#" + gridId);
                var arrModel = gridObj.jqGrid('getGridParam', 'colModel');
                var arrName = [];
                $.each(arrModel, function (i, v) {
                    arrName.push(v.name);
                });

                for (var index = 0; index < arr.length; index++) {
                    if (arr[index] != "") {
                        var nameValue = arr[index].split(":");
                        if (nameValue.length > 1) {
                            var colIndex = $.inArray(nameValue[0], arrName);
                            var innerObj = gridObj.find("tr").eq(rowIndex).find("td").eq(colIndex).children();
                            if (innerObj.length > 0) {
                                innerObj.val("");
                            } else
                                gridObj.setCell(rowId, nameValue[0], "  ", null, null);
                        }
                    }
                }
            }

            //gridObj.setCell(rowId, nameValue[0], "  ", null, null);
        });

        var _input = new easyInput({
            id: inputId
        },
		handleKeyUp);
        var _grid = new easyGrid({
            id: gridId
        },
		handleDb);

        //_input.dblclick = handleKeyUp;
        _input.keydown = _grid.InputKeyDown;
        _grid.callback = _grid.handleDb;

        var button = $("#" + config.buttonId);
        if (button) {
            button.click(function (e) {
                UtilglControl.closeDiv(config);
                controlJson.createFrame(config);
            });
        }

        UtilglControl.cloneConfig(config, current);
        current.ajaxDivId = gridId;
        current.currentDom = $("#" + inputId)[0];
        current.imageDivId = inputId + "_image_associate";

        //multipleInput
        config.currentDom = $("#" + inputId)[0];

        //多选功能判断
        if (current.multiple) {
            _grid.keydown = handleKeyDown;
            _input.click = handleClick;
        }
        else {
            _grid.keydown = _input.keyup = _grid.keyup = keydownPrevent;
        }

        _input.blur = _grid.blur = pBlur;

        pGrid = _grid;
        pInput = _input;
    };

    //多选 相关功能函数 开始
    var handleClick = function (e) {
        var input = document.getElementById(pInput.id);
        var t = input.value, s = getSelectionStart(input);
        var afterCaret = t.substring(s);
        var beforeCaret = t.substring(0, s);
        var start = beforeCaret.lastIndexOf(";") + 1;
        var end = afterCaret.indexOf(";");
        if (end > -1 && end + s > start) {
            range = input.createTextRange(); ;
            range.collapse(true);
            range.moveStart('character', start);
            range.moveEnd('character', end + s - start + 1);
            range.select();
        }
    }
    function getSelectionStart(o) {
        if (o.createTextRange) {
            var r = document.selection.createRange().duplicate();
            r.moveEnd('character', o.value.length);
            if (r.text == '') return o.value.length;
            return o.value.lastIndexOf(r.text);
        } else return o.selectionStart;
    }
    function getSelectionEnd(o) {
        if (o.createTextRange) {
            var r = document.selection.createRange().duplicate()
            r.moveStart('character', -o.value.length)
            return r.text.length
        } else return o.selectionEnd
    }

    _s.deleteFlag = false;
    var handleKeyDown = function (e) {
        var kc = UtilglControl.getKeyCode(e);
        if (kc == 27) {
            UtilglControl.closeLoading(current);
            UtilglControl.closeDiv(current, current.controlId);
            e.preventDefault();
        }
        else if (kc == 186) {
            e.preventDefault();
            if ($("#" + pGrid.id + ":visible").length > 0) {
                pGrid.handleDb();
            }
        }
        else if (kc == 46 || kc == 8) {
            _s.deleteFlag = false;
            var input = document.getElementById(pInput.id);
            var t = input.value, s = getSelectionStart(input), n = getSelectionEnd(input);
            var selectedText = t.substring(s, n);
            if (selectedText == "") {
                if (kc == 8 && t.charAt(s - 1) == ";" || kc == 46 && t.charAt(s + 1) == ";") {
                    e.preventDefault();
                    _s.deleteFlag = true;
                    UtilglControl.removeInputValue(s - 1, current);
                }
            }
            else if (selectedText.indexOf(";") > 0) {
                e.preventDefault();
                _s.deleteFlag = true;
                var comma = selectedText.indexOf(";");
                UtilglControl.removeInputValue(s + comma, current);
            }
        }
        else if (kc == 37 || kc == 39) {
            e.preventDefault();
            var input = document.getElementById(pInput.id);
            var s = (kc == 37 ? (getSelectionStart(input) - 1) : (getSelectionEnd(input) + 1));
            range = input.createTextRange(); ;
            range.collapse(true);
            range.moveStart('character', (s > 0 ? s : 0));
            range.select();
            handleClick();
        }
        else if (kc != 16 && kc != 17 && kc != 9 && kc != 13 && kc != 38 && kc != 40) {
            var input = document.getElementById(pInput.id);
            var t = input.value, s = getSelectionStart(input), n = getSelectionEnd(input);
            var selectedText = t.substring(s, n);
            if (selectedText != "" && selectedText.indexOf(";") > 0) {
                e.preventDefault();
                alert("Can not alter this value.");
            }
        }
    };
    //多选 相关功能函数 结束于上

    var keydownPrevent = function (e) {
        var kc = UtilglControl.getKeyCode(e);
        if (kc == 13) return false;
    }

    var handleKeyUp = function (e) {
        var kc = UtilglControl.getKeyCode(e);
        if (kc == 27) { UtilglControl.closeDiv(current, current.controlId); }
        else if (kc == 186) { }
        //pass Enter to grid 32
        else if ((kc == 13)
        //&& $("#" + pGrid.id + ":visible").length > 0) {
        && $("#" + pGrid.id + ":visible").length > 0 && $("#" + pGrid.id).find("tr[class=selected]").length != 0) {
            $("#" + pGrid.id).trigger("keyup");
        } else {
            if (kc == 8 || kc == 46) {
                //多选功能判断
                if (!current.multiple) {
                    if ($("#" + pInput.id).val().length == 0) {
                        UtilglControl.setInputValue(null, current);
                        UtilglControl.closeDiv(current, current.controlId);
                        return;
                    }
                }
                else if (_s.deleteFlag) {
                    return;
                }
            }
            //if (kc == 32) // 只有空格才能联想
            if ($("#" + pGrid.id + ":visible").length != 0 && (kc == 38 || kc == 40)) {
                return;
            }

            clearTimeout(timeId);
            timeId = setTimeout(function () {
                UtilglControl.getSelectHtml(current, openDiv);
            }, current.delay);
        }
    };

    var pBlur = function () {
        var focusId = document.activeElement.id;
        if (focusId == pInput.id)
            return;
        else if (focusId == pGrid.id
		|| $(document.activeElement).parents("#" + pGrid.id).length == 1)
            $("#" + pGrid.id).focus();
        //表单失去焦点处理
        else if ($("#" + pGrid.id + ":visible").length > 0 && $("#" + pGrid.id).find("tr[class=selected]").length != 0) {
            //联动功能判断
            if (!current.stay) {
                if (pGrid.itemContainers.length > 0 && $("#" + pInput.id).val()) {
                    event.keyCode = 13;
                    // $("#" + pGrid.id).trigger("keyup");
                    UtilglControl.closeDiv(current, current.controlId);
                }
                else {
                    if (!current.isValidValue) {
                        UtilglControl.setInputValue(null, current);
                    }
                    UtilglControl.closeDiv(current, current.controlId);
                }
            }
            else if (current.fillControls.indexOf(focusId) > -1) {
                UtilglControl.closeDiv(current, current.controlId);
                UtilglControl.getSelectHtml(objControlJson[focusId].Config(), openDiv);
            }
        }
        UtilglControl.closeLoading(current);

        //表单失去焦点处理：
        if (focusId != "aReflesh") {
            if (($("#" + pGrid.id).find("tr[class=selected]").length == 0 && focusId != "autoSelectDiv" + current.controlId))
                UtilglControl.closeDiv(current, current.controlId);
        }
    };
    _s.Config = function () {
        return current;
    }

    function openDiv(config, msg) {
        pGrid.SetHtml(msg);

        var obj = config.currentDom;
        var gridObj = $("#" + pGrid.id);

        var width = config.width;
        var offset = $(obj).offset();
        var w = $(document.body).attr("offsetWidth") - offset.left;
        var left = w < width ? offset.left + $(obj).attr("offsetWidth") - width : offset.left;

        var bottomDistance = $(document.body).attr("clientHeight") - offset.top;
        var topDistance = offset.top - $(obj).attr("offsetHeight");

        var top = offset.top + $(obj).height() + 5;
        //if (gridObj.attr("offsetHeight") + 5 > bottomDistance && topDistance > gridObj.attr("offsetHeight"))
        //    top = offset.top - gridObj.attr("offsetHeight") - 3;

        var gridHeight = gridObj.height();
        if (gridHeight + 5 > bottomDistance && topDistance > gridHeight)
            top = offset.top - gridHeight - 3;

        if (!window.ActiveXObject)
            left = left + 4;

        gridObj.css("left", left + "px");
        gridObj.css("top", top + "px");
        gridObj.css("width", width + "px");
        gridObj.css("zIndex", '9003');
        gridObj.css("position", 'absolute');
        gridObj.css("cursor", 'default');
        gridObj.show();
        // $("#" + pInput.Id).focus();

        //$(document).scrollTop($(document).height());
        //alert();
    };

    var handleDb = function (db) {
        UtilglControl.setInputValue(db, current, current.controlId);
        UtilglControl.closeDiv(current, current.controlId);
        if (config.handleDb) {
            config.handleDb.call(this, db);
        }
    };
    init(config);
};
var easyGridItem = function (currentId, trClassName) {
    var _s = this;
    var oldClass = trClassName;
    var id = currentId;
    _s.index = null;
    _s.db = null;
    _s.callBack = null;
    _s.resetGrid = null;

    var init = function () {
        var trObj = $("#" + id);
        _s.db = eval("(" + trObj.attr("DbSource") + ")");
        trObj.removeClass();
        trObj.addClass(oldClass);

        trObj.click(function (evt) {
            trclick(evt);
        });
        trObj.mouseover(function (e) {
            trmouseover(e);
        });
        trObj.mouseout(function (e) {
            trmouseout(e);
        });
    };
    var trclick = function (evt) {
        if (_s.callBack && _s.callBack != null) {
            _s.callBack.call(this, _s.index);
        }
    };
    var trmouseover = function (e) {
        if (_s.resetGrid && _s.resetGrid != null) {
            _s.resetGrid.call(this, _s.index);
        }
        return false;
    };
    var trmouseout = function (e) {
        return false;
    };
    _s.setCurrentClass = function (newClass) {
        var trObj = $("#" + id);
        trObj.removeClass();
        trObj.addClass(newClass);
    };
    _s.getOldClass = function () {
        return oldClass;
    };
    _s.getCurrent = function () {
        return _s.db;
    };
    init();
};
var easyGrid = function (gconfig, fun) {
    var _s = this;
    _s.selectItem = null;
    _s.itemContainers = null;
    _s.id = gconfig.id;
    _s.parentId = gconfig.parent;
    _s.fn = fun;
    _s.click = null;
    _s.dblclick = null;
    _s.keydown = null;
    _s.keyup = null;
    _s.blur = null;
    _s.callback = null;

    _s.SetHtml = function (_html) {
        var dom = $("#" + _s.id);
        dom.removeClass();
        dom.addClass(UtilglControl.FrameDivClass);
        dom.html(_html);
        var trCollection = $(dom).find("TR.trSourceClass");
        _s.itemContainers.length = 0;
        for (var index = 0; index < trCollection.length; index++) {
            var trClassName = UtilglControl.simpleTrClass;
            if (index % 2 == 0) {
                trClassName = UtilglControl.doubleTrClass;
            }
            var item = new easyGridItem(trCollection[index].id, trClassName);
            item.index = index;
            item.callBack = itemCallBack;
            item.resetGrid = resetGrid;
            if (index == 0) {
                item.setCurrentClass(UtilglControl.selectedClass);
                selectItem = item;
            }
            _s.itemContainers.push(item);
        }
    };
    var itemCallBack = function (index) {
        resetGrid(index);
        if (_s.callback) {
            _s.callback.call(this);
        }
    };
    var resetGrid = function (index) {
        if (selectItem == null) {
            //return;
        } else {
            selectItem.setCurrentClass(selectItem.getOldClass())
        }
        if (_s.itemContainers.length > index && index > -1) {
            selectItem = _s.itemContainers[index];
            selectItem.setCurrentClass(UtilglControl.selectedClass);
        }
    };
    var init = function () {
        var dom = $("#" + _s.id);
        var parent = _s.parentId ? $("#" + _s.parentId) : $(document.body);

        _s.itemContainers = new Array();
        if (dom.length == 0) {
            dom = document.createElement("div");
            dom.setAttribute("id", _s.id);
            dom.setAttribute("tabindex", -1);
            parent.append(dom);
            dom = $(dom);
            dom.hide();
        }
        dom.click(function (e) {
            InputClick(e);
        });
        dom.dblclick(function (e) {
            _s.InputdblClick(e);
        });
        dom.keydown(function (e) {
            _s.InputKeyDown(e);
        });
        dom.keyup(function (e) {
            InputKeyUp(e);
        });
        dom.blur(function (e) {
            InputBlur(e);
        });
    };
    var InputClick = function (e) {
        if (_s.click) {
            _s.click.call(this, e)
        }
    };
    _s.InputdblClick = function (e) {
        if (_s.dbclick) {
            _s.dblclick.call(this, e)
        }
    };
    _s.handleDb = function () {
        var db = null;
        if (selectItem != null) {
            var index = selectItem.index;
            if (index > -1 && index < _s.itemContainers.length) {
                db = selectItem.getCurrent();
            }
        }
        if (_s.fn) {
            _s.fn.call(this, db); //selectItem = null in fn;
        }
    };
    _s.InputKeyDown = function (e) {
        if (_s.keydown) {
            _s.keydown.call(this, e)
        }
        var kc = UtilglControl.getKeyCode(e);
        if (kc == 38 || kc == 40) {
            VerticalDirectionKeydown(e);
        }
    };
    var InputKeyUp = function (e) {
        if (_s.keyup) {
            _s.keyup.call(this, e)
        }
        var kc = UtilglControl.getKeyCode(e);
        if (kc == "13") {
            _s.handleDb();
            event.keyCode = 0;
        }
        return false;
    };
    var InputBlur = function (e) {
        if (_s.blur) {
            _s.blur.call(this, e);
        }
    };
    var HoriztalDirectionKeydown = function (e) {

    }
    var VerticalDirectionKeydown = function (e) {
        if (_s.itemContainers.length < 1)
            return;
        var index = 0;
        if (selectItem != null && $("#" + _s.id + ":visible").length > 0) {
            index = selectItem.index;
        }
        var kc = UtilglControl.getKeyCode(e);
        var des = 0;
        if (index == -1)
            return;
        if (kc == 38 && index == 0)
            return;
        if (kc == 40 && index == _s.itemContainers.length - 1) {
            _s.itemContainers[0].setCurrentClass(UtilglControl.selectedClass);
            selectItem = _s.itemContainers[0];
            if (index != 0) _s.itemContainers[index].setCurrentClass(_s.itemContainers[index].getOldClass());
            return;
        }
        if (kc == 38) {
            des = index - 1;
        }
        if (kc == 40) {
            des = index + 1;
        }
        var firstClass = $("#" + _s.id).find("tr:eq(1)").attr("class"); //selected
        if (firstClass == "selected" || index > 0) {
            _s.itemContainers[index].setCurrentClass(_s.itemContainers[index].getOldClass());
            _s.itemContainers[des].setCurrentClass(UtilglControl.selectedClass);
            selectItem = _s.itemContainers[des];
        } else {
            _s.itemContainers[0].setCurrentClass(UtilglControl.selectedClass);
            selectItem = _s.itemContainers[0];
            //_s.itemContainers[des].setCurrentClass(UtilglControl.selectedClass);
        }
    };
    init();
};
var easyInput = function (iconfig, fun) {
    var _s = this;
    _s.id = iconfig.id;
    _s.fn = fun;
    _s.parentId = iconfig.parent;
    _s.click = null;
    _s.dblclick = null;
    _s.keydown = null;
    _s.keyup = null;
    _s.blur = null;
    var init = function () {
        var input = $("#" + _s.id);
        var parent = _s.parentId ? $("#" + _s.parentId) : $(document.body);

        if (input.length == 0) {
            input = document.createElement("input");
            input.setAttribute('id', _s.id);
            input.type = "text";
            parent.append(input);
            input = $(input);
        }
        //input.css("background-color", "#E0E0E0");
        input.addClass("glControlText");
        input.attr("autocomplete", "off");

        input.click(function (e) {
            InputClick(e);
        });
        input.dblclick(function (e) {
            UtilglControl.CurrentControl = _s;
            InputdblClick(e);
        });
        input.keydown(function (e) {
            UtilglControl.CurrentControl = _s;
            InputKeyDown(e);
        });
        input.keyup(function (e) {
            var code = UtilglControl.getKeyCode(e);
            if (code == 32) {
                //$(this).val($(this).val().replace(/ /g, ""));
                //$(this).val($.trim($(this).val()));
            }
            InputKeyUp(e);
        });
        input.blur(function (e) {
            InputBlur(e);
        });
    };
    var InputClick = function (e) {
        if (_s.click) {
            _s.click.call(this, e);
        }
    };
    var InputdblClick = function (e) {
        if (_s.dblclick) {
            _s.dblclick.call(this, e);
        }
    };
    var InputKeyDown = function (e) {

        if (UtilglControl.getKeyCode(e) == 9) {
            if (typeof selectItem != "undefined" && selectItem != null)
                selectItem = null;
        }

        if (_s.keydown) {
            _s.keydown.call(this, e);
        }


        if (UtilglControl.getKeyCode(e) == 27)
            $(".autoFrameDivClass").hide();
    };
    var InputKeyUp = function (e) {
        if (_s.keyup) {
            _s.keyup.call(this, e);
        }
        var kc = UtilglControl.getKeyCode(e);
        //if (kc != 9 && kc != 37 && kc != 38 && kc != 39 && kc != 40) {
        //if (kc == 13) {
        //空格联想
        //if (kc == 32 || kc == 13 || kc == 27) {
        if (_s.fn) {
            _s.fn.call(this, e);
        }
        //}
    };
    var InputBlur = function (e) {
        if (_s.blur) {
            _s.blur.call(this, e);
        }
    };
    init();
};
var UtilglControl = {
    selectedClass: "selected",
    simpleTrClass: "simpleTr",
    doubleTrClass: "doubleTr",
    FrameDivClass: "autoFrameDivClass",
    DisplayDivClass: "autoDisplayDivClass",
    ImagePath: "Images/AutoImage",
    hidenTrClass: "hiden",
    showSpanClass: "glControlShowSpan",
    displayClass: "glControlDisplaySpan",

    CurrentControl: null,
    setValue: function (input, value) {
        input.value = value;
        range = input.createTextRange(); ;
        range.collapse(true);
        range.moveStart('character', value.length);
        range.select();
    },
    getValue: function (source) {
        var arr = source.value.toString().split(";");
        return arr[arr.length - 1];
    },
    getSelectHtml: function (config, fn) {
        UtilglControl.startLoading(config);
        config = UtilglControl.ajaxConfig(config);
        $.ajax({
            type: "POST",
            url: config.url,
            timeout: 200000,
            error: function (e) {
                if (e.status == 404)
                    alert("Element not found.");
                else if (e.status == 403) {
                    reDirectToLoginPage();
                }
                else {
                    var response = e.responseText.toString();
                    alert(response.substring(response.indexOf("<title>") + "<index>".length, response.indexOf("</title>")));
                }
            },
            data: config.ajaxPara,
            success: function (msg) {
                //alert(msg);
                fn.call(this, config, msg.msg);
                config.ajaxPara.isRefalshFromDb = "0";
                UtilglControl.bindEvent(config, UtilglControl.getSelectHtml, fn);
                UtilglControl.setImgDisabled(config);
                var focusID = document.activeElement.id;
                if (config.controlId != focusID && focusID != "autoSelectDiv" + config.controlId) {
                    //表单失去焦点处理
                    UtilglControl.closeLoading(config);
                    UtilglControl.closeDiv(config, config.controlId);
                }


            },
            complete: function (e) {
                UtilglControl.closeLoading(config);
            }
        });
    },
    cloneConfig: function (b, a) {
        a.controlId = b.controlId,
        a.fillControls = b.fillControls,
        //多选功能
        a.multiple = b.multiple,
        //联动功能
        a.stay = b.stay,
        a.delay = b.delay || 450,
		a.url = b.url || "",
		a.urlOverride = b.urlOverride || 0,
		a.width = b.width || 350,
		a.imagePath = b.imagePath || "",
		a.filter = b.filter || "",
		a.characterNum = b.characterNum || 1,
		a.condition = b.condition || "likeeic",
		a.searchKeyCode = b.searchKeyCode || 13,
		a.defaultCondition = b.defaultCondition || "",
		a.index = b.index || -1,
		a.selectedRowId = b.selectedRowId || null,
		a.IsClick = b.IsClikc || true,
		a.handleDb = b.handleDb || "",
		a.fillColumns = b.fillColumns || "",
		a.isValidValue = b.isValidValue || "",
		a.whereConditionFunc = b.whereConditionFunc || null,
		a.ajaxPara = b.ajaxPara || {
		    assemblyName: b.assemblyName || "",
		    sortFields: b.sortFields || "",
		    displayFields: b.displayFields || "",
		    displayLabels: b.displayLabels || "",
		    queryFieldName: b.queryFieldName || "",
		    recordsPerPage: b.recordsPerPage || 5,
		    filter: b.filter || "",
		    parentFieldName: b.parentFieldName || "",
		    mappingName: b.mappingName || "",
		    autoHeight: b.autoHeight,
		    defaultCondition: b.defaultCondition || "",
		    isRefalshFromDb: b.isRefalshFromDb || "0",
		    gnoreIUCase: b.gnoreIUCase || "1",
		    cachePk: b.cachePk,
		    optionRecordsPerPage: "25;50;100",
		    isDefinedRecordsPerPage: b.isDefinedRecordsPerPage || "1",
		    imagePath: b.imagePath || UtilglControl.ImagePath
		};
        a.query = b.query & true;
        a.isAjaxDivHide = b.isAjaxDivHide & true;
        a.isFromSelect = b.isFromSelect | false;
        if (a.ajaxPara && a.ajaxPara.isDefinedRecordsPerPage == "1") {
            if (a.ajaxPara.recordsPerPage != "10" &&
			a.ajaxPara.recordsPerPage != "25" &&
			a.ajaxPara.recordsPerPage != "50") {
                a.ajaxPara.recordsPerPage = 10;
            }
        }
        if (a.urlOverride != 1) {
            //a.url = a.url + "SelectCommand.aspx";
            a.urlOverride = 1;
        }
    },
    ajaxConfig: function (config) {
        if (config.ajaxPara.queryFieldvalue !== config.currentDom.value) {
            config.ajaxPara.currentPage = 1;
        }
        if ($("#" + config.currentDom.id + "_selectPageRow") &&
		$("#" + config.currentDom.id + "_selectPageRow").val() &&
		$("#" + config.currentDom.id + "_selectPageRow").val() != null) {
            config.ajaxPara.recordsPerPage = $("#" + config.currentDom.id + "_selectPageRow").val();
        }
        if ($("#" + config.parentId).length > 0 &&
		$("#" + config.parentId).val() != '' &&
		$("#" + config.parentHiddenId).length > 0) {
            config.parentHiddenValue = $("#" + config.parentHiddenId).val();
            config.ajaxPara.parentFieldValue = config.parentHiddenValue;
        } else {
            config.ajaxPara.parentFieldValue = '';
        }
        config.ajaxPara.selectId = config.currentDom.id;
        config.ajaxPara.queryFieldvalue = config.currentDom.value.toString();
        config.ajaxPara.condition = config.condition;
        if (config.whereConditionFunc) {
            config.ajaxPara.defaultCondition = config.whereConditionFunc; //eval(config.whereConditionFunc + "()");
        } else {
            config.ajaxPara.defaultCondition = config.defaultCondition;
        }
        return config;
    },
    InputClick: function (evt, fn, config) {
        fn.call(evt, config);
    },
    InputKeyDown: function (evt, fn, config) {
        fn.call(evt, config);
    },
    InputKeyUp: function (evt, fn, config) {
        fn.call(evt, config);
    },
    func: function (config) {
    },
    startLoading: function (config) {
        var obj = config.currentDom;
        var offset = $(obj).offset();
        var top = offset.top + $(obj).height() + 5;
        var left = offset.left;

        var loadingDiv = $("#" + obj.id + 'LoadingDiv');
        if (loadingDiv.length == 0) {
            loadingDiv = $("<div>");
            loadingDiv.attr("id", obj.id + 'LoadingDiv');
            $(document.body).append(loadingDiv);
        }
        loadingDiv.html("<img src='" + config.ajaxPara.imagePath + "/loading.gif'alt='Loading...'></img>");
        loadingDiv.css("left", left + "px");
        loadingDiv.css("top", top + "px");
        loadingDiv.css("zIndex", "9999");
        loadingDiv.css("position", "absolute");
    },
    closeLoading: function (config) {
        var loadingDiv = $("#" + config.currentDom.id + 'LoadingDiv');
        if (loadingDiv) {
            loadingDiv.css("left", "-9999px");
            loadingDiv.css("top", "-9999px");
        }
    },
    bindEvent: function (config, fun, fn) {
        var obj = config.currentDom;
        var currPage = $("#" + obj.id + "currPageId");
        var totalPage = $("#" + obj.id + "totalPageId");
        var totalNum = $("#" + obj.id + "totalNumId");
        if (currPage[0])
            config.currentPage = parseInt(currPage[0].value);
        if (totalPage[0])
            config.totalPage = parseInt(totalPage[0].value);
        if (totalNum[0])
            config.totalNum = parseInt(totalNum[0].value);
        if (config.currentPage != 1) {
            $("#" + obj.id + "firstPage").click(function (e) {
                config.ajaxPara.currentPage = 1;
                fun.call(this, config, fn);
            });
            $("#" + obj.id + "prevPage").click(function (e) {
                if (config.ajaxPara.currentPage > 1) {
                    config.ajaxPara.currentPage = config.currentPage - 1;
                    fun.call(this, config, fn);
                }
            });
        }
        if (config.currentPage != config.totalPage) {
            $("#" + obj.id + "nextPage").click(function (e) {
                if (config.currentPage < config.totalPage) {
                    config.ajaxPara.currentPage = config.currentPage + 1;
                    fun.call(this, config, fn);
                }
            });
            $("#" + obj.id + "lastPage").click(function (e) {
                config.ajaxPara.currentPage = config.totalPage;
                fun.call(this, config, fn);
            });
        }
        var tfun = null;
        $("#" + obj.id + "gotoPageInput").bind("focus", function (event) {
            tfun = document.onclick;
            document.onclick = null;
        });
        $("#" + obj.id + "gotoPageInput").bind("blur", function (event) {
            document.onclick = tfun;
        });
        $("#" + obj.id + "gotoPageInput").bind("keyup", function (event) {
            var e = event || window.event;
            if (e.keyCode == 13) {
                $("#" + obj.id + "gotoPage").trigger("click");
            }
        });
        $("#" + obj.id + "gotoPage").click(function (e) {
            var value = $("#" + obj.id + "gotoPageInput")[0].value;
            if (value) {
                if (parseInt(value) != config.currentPage &&
				parseInt(value) > 0 &&
				parseInt(value) <= config.totalPage) {
                    config.ajaxPara.currentPage = value;
                    fun.call(this, config, fn);
                }
            }
        });
        $("#" + obj.id + "reflashPage").click(function (e) {
            //var value = $("#" + obj.id + "gotoPageInput")[0].value;
            var value = $("#" + obj.id + "reflashPage")[0].value;
            config.ajaxPara.isRefalshFromDb = "1";
            fun.call(this, config, fn);
        });
        $("#" + obj.id + "_selectPageRow").change(function (e) {
            config.ajaxPara.recordsPerPage = $("#" + obj.id + "_selectPageRow").val();
            fun.call(this, config, fn);
        });
    },
    setImgDisabled: function (config) {
        if (config.currentPage == 1) {
            UtilglControl.setPageInfo(config.currentDom.id + "firstPage", config.ajaxPara.imagePath + "/" + "first_.gif");
            UtilglControl.setPageInfo(config.currentDom.id + "prevPage", config.ajaxPara.imagePath + "/" + "previous_.gif");
        }
        if (config.currentPage == config.totalPage) {
            UtilglControl.setPageInfo(config.currentDom.id + "nextPage", config.ajaxPara.imagePath + "/" + "next_.gif");
            UtilglControl.setPageInfo(config.currentDom.id + "lastPage", config.ajaxPara.imagePath + "/" + "last_.gif");
        }
    },
    setPageInfo: function (id, image) {
        $("#" + id).attr("src", image);
        $("#" + id).attr("title", "");
        $("#" + id).attr("alt", "");
    },
    getKeyCode: function (e) {
        var msie = (document.all) ? true : false;
        var keyCode;
        if (!msie)
            keyCode = e.which;
        else
            keyCode = event.keyCode;
        return keyCode;
    },
    //多选功能函数
    removeInputValue: function (position, config) {
        var original = config.currentDom.value;
        if (original.charAt(position) != ";")
            alert("position invalid");
        else {
            var arr = original.substring(0, position).split(";");
            var removePosition = arr.length - 1;

            UtilglControl.setValue(config.currentDom, UtilglControl.removeValue(original, removePosition));
            // config.currentDom.value = UtilglControl.removeValue(original, removePosition);

            if (config.fillControls && config.fillControls != null) {
                arr = config.fillControls.split(";");
                for (var index = 0; index < arr.length; index++) {
                    if (arr[index] != "") {
                        var nameValue = arr[index].split(":");
                        if (nameValue.length > 1) {

                            var value = UtilglControl.removeValue($("#" + nameValue[0]).val(), removePosition);
                            $("#" + nameValue[0]).val(value);
                        }
                    }
                }
            }
        }
        config.currentDom.focus();
    },
    removeValue: function (original, removePosition) {
        var value = "";
        var arr = original.split(";");
        for (var i = 0; i < arr.length - 1; i++) {
            if (i != removePosition)
                value += arr[i] + ";";
        }
        return value + arr[arr.length - 1];
    },
    updateValue: function (original, appendValue) {
        var value = "";
        var arr = original.split(";");
        var existed = false;
        for (var i = 0; i < arr.length - 1; i++) {
            if (arr[i] == appendValue) {
                existed = true;
                break;
            }
        }
        for (var i = 0; i < arr.length - 1; i++) {
            value += arr[i] + ";";
        }
        if (!existed)
            value += appendValue + ";";
        return value;
    },
    //多选功能结束于上
    setInputValue: function (db, config) {
        //多选功能判断
        if (!config.multiple) {
            if (db != null) {
                $("#" + config.controlId).focus();
                //config.currentDom.value = db[config.ajaxPara.queryFieldName];
                $("#" + config.controlId).val(db[config.ajaxPara.queryFieldName]);
            } else {
                config.currentDom.value = "";
            }
            if (config.fillControls && config.fillControls != null) {
                var arr = config.fillControls.split(";");
                for (var index = 0; index < arr.length; index++) {
                    if (arr != "") {
                        var nameValue = arr[index].split(":");
                        if (nameValue.length > 1) {
                            if (db != null) {
                                $("#" + nameValue[0]).val(db[nameValue[1]]);
                            } else {
                                $("#" + nameValue[0]).val("");
                            }
                        }
                    }
                }
            }
            if (arguments.length > 2) {
                if (config.isValidValue) {
                    $("#" + arguments[2]).attr("ValidValue", "1");
                }
            }
        }
        else if (db != null) {
            var domValue = UtilglControl.updateValue(config.currentDom.value, db[config.ajaxPara.queryFieldName]);
            //config.currentDom.value = domValue;
            UtilglControl.setValue(config.currentDom, domValue);

            if (config.fillControls && config.fillControls != null) {
                var arr = config.fillControls.split(";");
                for (var index = 0; index < arr.length; index++) {
                    if (arr[index] != "") {
                        var nameValue = arr[index].split(":");
                        if (nameValue.length > 1) {
                            var value = UtilglControl.updateValue($("#" + nameValue[0]).val(), db[nameValue[1]]);
                            $("#" + nameValue[0]).val(value);
                        }
                    }
                }
            }

            if (arguments.length > 2) {
                if (config.isValidValue) {
                    $("#" + arguments[2]).attr("ValidValue", "1");
                }
            }
        }

        if (config.fillColumns && config.fillColumns != null && db != null) {
            var arr = config.fillColumns.split(";");
            var gridId = config.controlId;
            var obj = $("#" + config.controlId);
            //var rowId = obj.parent().parent().index();
            var rowIndex = obj.parent().parent().index();
            var rowId = obj.parent().parent().attr("id");
            gridId = gridId.replace(/___\d+/, "");
            gridId = $.trim(gridId.replace(obj.attr("AutoCode"), ""));
            var gridObj = $("#" + gridId);
            //判断是否含有子控件
            var arrModel = gridObj.jqGrid('getGridParam', 'colModel');
            var arrName = [];
            $.each(arrModel, function (i, v) {
                arrName.push(v.name);
            });
            for (var index = 0; index < arr.length; index++) {
                if (arr[index] != "") {
                    var nameValue = arr[index].split(":");
                    if (nameValue.length > 1) {
                        var colIndex = $.inArray(nameValue[0], arrName);
                        var innerObj = gridObj.find("tr").eq(rowIndex).find("td").eq(colIndex).children();
                        if (innerObj.length > 0) {
                            innerObj.val(db[nameValue[1]]);
                        } else
                            gridObj.setCell(rowId, nameValue[0], db[nameValue[1]], null, null);
                    }
                }
            }
        }
    },
    closeDiv: function (config) {
        var autoSelectDiv = $("#" + config.ajaxDivId);
        autoSelectDiv.hide();
    },
    eventTool: function (evt) {
        var e = (evt) ? evt : window.event;
        if (window.event) {
            e.cancelBubble = true;
        } else {
            e.stopPropagation();
        }
    },
    getUtilInput: function (id) {
        return id + "";
    },
    getUtilButton: function (id) {
        return id + "_Btn";
    },
    UtilSpanDisplay: function (id) {
        if (arguments.length > 1) {
            $("#" + id).removeClass();
            if (arguments[1] == "show") {
                $("#" + id).addClass(UtilglControl.showSpanClass);
            } else {
                $("#" + id).addClass(UtilglControl.displayClass);
            }
            return;
        }
        var oldClassName = $("#" + id).attr("className");
        $("#" + id).removeClass();
        if (oldClassName != UtilglControl.showSpanClass) {
            $("#" + id).addClass(UtilglControl.showSpanClass);
        } else {
            $("#" + id).addClass(UtilglControl.displayClass);
        }
    },
    UtilElementPostion: function (id) {
        var postion = {
            x: 0,
            y: 0
        };
        var target = $("#" + id)[0];
        postion.x = target.offsetLeft;
        postion.y = target.offsetTop + target.offsetHeight;
        target = target.offsetParent;
        while (target) {
            postion.x = postion.x + target.offsetLeft;
            postion.y = postion.y + target.offsetTop;
            target = target.offsetParent;
        }
        if (window.ActiveXObject) {
            postion.x += 3;
            postion.y += 3;
        } else {
            postion.x += 6;
        }
        return postion;
    }
};


var AutoInput = function (options) {
    controlJson.initInput({
        displayFields: options.displayFields,
        displayLabels: options.displayLabels,
        width: options.width,
        queryFieldName: options.queryFieldName,
        isRefalshFromDb: "0",
        url: options.url,
        isDefinedRecordsPerPage: "0",
        recordsPerPage: options.recordsPerPage,
        imagePath: options.imagePath || "../Content/Images/AutoImage",
        fillControls: options.fillControls,
        controlId: options.controlId
    })
}