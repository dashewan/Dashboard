
(function ($) {
    var _selInput = null;
    $.fn.extend({
        select: function (index) {
            var tab = $(this);
            return this.each(function () {
                tab.find("li").eq(index).find("a").click();
            });
        }
    })
})(jQuery);