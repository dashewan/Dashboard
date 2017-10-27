/*
* Async Treeview 0.1 - Lazy-loading extension for Treeview
* 
* http://bassistance.de/jquery-plugins/jquery-plugin-treeview/
*
* Copyright (c) 2007 Jörn Zaefferer
*
* Dual licensed under the MIT and GPL licenses:
*   http://www.opensource.org/licenses/mit-license.php
*   http://www.gnu.org/licenses/gpl.html
*
* Revision: $Id$
*
*/

; (function($) {
    function load(settings, root, child, container) {
        //是否建立菜单项
        if(settings.hasLevel1Menu || settings.hasLevel2Menu || settings.hasLevel3Menu ){
           
           
           
           if(document.getElementById("dMenu")== null){
                var detailMenu = document.createElement("ul");
                detailMenu.id = "dMenu";
                detailMenu.className = "contextMenu";
                document.body.appendChild(detailMenu);
                $("#dMenu").append('<li class="add"><a href="#add">新增</a></li>');
                $("#dMenu").append('<li class="delete"><a href="#delete">删除</a></li>');
                $("#dMenu").append('<li class="audit"><a href="#audit">提交</a></li>');
                $("#dMenu").append('<li class="quit separator"><a href="#quit">关闭菜单</a></li>');
            }
        }
    
        $.getJSON(settings.url, { root: root }, function(response) {
            function createNode(parent) {
                $.ajaxSettings.async = false; 
                var str = "",spanstr="";
                // str = " onclick=onTreeNodeClick('" + this.id + "')";               
                if (typeof (settings.depth) == 'undefined') {
                    if (root.substr(0, 1) == 'o') {str = " onclick=onTreeNodeClick('" + this.id + "')";}
                } else { 
                    if(settings.depth==2)   //只显示订单
                    { if (root.substr(0, 1) == 'c') str = " onclick=onTreeNodeClick('" + this.id + "')"; }
                    else if(settings.depth==0)
                    { str="";spanstr=" onclick=onTreeNodeClick('" + this.id + "')";
                    }
                }
                
                var current = $("<li style='cursor: pointer;'" + str + "/>").attr("id", this.id || "").html("<span "+spanstr+">" + this.text + "</span>").appendTo(parent);
                if (this.classes) {
                    current.children("span").addClass(this.classes);
                }
                if (this.expanded) {
                    current.addClass("open");
                }
                if (this.hasChildren || this.children && this.children.length) {
                    var branch = $("<ul/>").appendTo(current);
                    if (this.hasChildren) {
                        current.addClass("hasChildren");
                        createNode.call({
                            text: "placeholder",
                            id: "placeholder",
                            children: []
                        }, branch);
                    }
                    if (this.children && this.children.length) {
                        $.each(this.children, createNode, [branch])
                    }
                }
            }
            $.each(response, createNode, [child]);
            $(container).treeview({ add: child });
        });
         
     
      
    }

    var proxied = $.fn.treeview;
    $.fn.treeview = function(settings) {
        if (!settings.url) {
            return proxied.apply(this, arguments);
        }
        var container = this;
        load(settings, "source", this, container);
        var userClick = settings.click;
        var userToggle = settings.toggle;
        return proxied.call(this, $.extend({}, settings, {
            collapsed: true,
            toggle: function() {
                var $this = $(this);
                if ($this.hasClass("hasChildren")) {
                    var childList = $this.removeClass("hasChildren").find("ul");
                    childList.empty();
                    load(settings, this.id, childList, container);
                }
                if (userToggle) {
                    userToggle.apply(this, arguments);
                }
                if (userClick) {
                    userClick.apply(this, arguments);
                }
            }
        }));
    };

})(jQuery);