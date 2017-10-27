//$('#input1,#input2').decimalInput('9999.99');
//$("#input").myDecimalInput({ format: "9999.99" });
(function ($) {
    $.fn.myDecimalInput = function (options) {
        var defaultFormat = '99999999.99';
        if (!options || !options.format) {
            this.decimalInput(defaultFormat);
        }
        else {
            this.decimalInput(options.format);
        }
    }
})(jQuery);

(function ($) {

    $.fn.decimalInput = function (mask, isNegative) {

        if (!mask || !mask.match) {
            throw 'decimalnput occurs an error';
        }

        var 
      v,
        //      is = (function () { v = mask.match(/[0-9]{1,}/); var r = v !== null ? v[0].length : 0; console.log(r); return r; })(),
        //      ds = (function () { v = mask.match(/[0-9]{1,}$/); var r = v !== null ? v[0].length : 0; console.log(r); return r; })(),
      is = (function () { v = mask.match(/[0-9]{1,}/); var r = v !== null ? v[0].length : 0; return r; })(),
      ds = (function () { v = mask.match(/[0-9]{1,}$/); var r = v !== null ? v[0].length : 0; return r; })(),
      sep = (function () { v = mask.match(/,|\./); var r = v !== null ? v[0] : null; return r; })(),
      matcher = null,
      tester = null,
      events = /.*MSIE 8.*|.*MSIE 7.*|.*MSIE 6.*|.*MSIE 5.*/.test(navigator.userAgent) ? 'keyup propertychange paste' : 'input paste';

        if (sep === null) {
            tester = new RegExp('^[0-9]{0,' + is + '}$');
            matcher = new RegExp('[0-9]{0,' + is + '}', 'g');
        } else {
            if (!isNegative) {
                tester = new RegExp('^[0-9]{0,' + is + '}' + (sep === '.' ? '\\.' : ',') + '[0-9]{0,' + ds + '}$|^[0-9]{0,' + is + '}' + (sep === '.' ? '\\.' : ',') + '$|^[0-9]{0,' + is + '}$');
                matcher = new RegExp('[0-9]{0,' + is + '}' + (sep === '.' ? '\\.' : ',') + '[0-9]{0,' + ds + '}|[0-9]{0,' + is + '}' + (sep === '.' ? '\\.' : ',') + '|[0-9]{0,' + is + '}', 'g');
            } else {
                tester = new RegExp('^-$|^-?[0-9]{0,' + is + '}$|^-?[0-9]{0,' + is + '}' + (sep === '.' ? '\\.' : ',') + '[0-9]{0,' + ds + '}$|^[0-9]{0,' + is + '}' + (sep === '.' ? '\\.' : ',') + '$|^[0-9]{0,' + is + '}$');
                matcher = new RegExp('-|^-?[0-9]{0,' + is + '}|^-?[0-9]{0,' + is + '}' + (sep === '.' ? '\\.' : ',') + '[0-9]{0,' + ds + '}|[0-9]{0,' + is + '}' + (sep === '.' ? '\\.' : ',') + '|[0-9]{0,' + is + '}', 'g');
            }

        }

        function handler(e) {
            
            var self = $(e.currentTarget);
            var domSelf = e.currentTarget;
            if (self.val() !== e.data.ov) {
                if (!tester.test(self.val())) {
                    var r = self.val().match(matcher);
                    //alert(r);
                    //self.val(r === null ? '' : r[0]);
                    //self.val(self[0].data);
                     self.val( $.data(domSelf , "value"));
                } else {
                   $.data(domSelf , "value", self.val());
                    //self[0].data = self.val();
                }

                //console.log(e.data.ov);
                ov = e.data.ov = self.val();
            }
        }

        //this
        //  .attr('maxlength', (is + ds + (sep === null ? 0 : 1)))
        //  .val(this.val() ? this.val().replace('.',sep) : this.val())
        //  .on(events,{ov:this.val()},handler);

        this
     .attr('maxlength', (is + ds + (sep === null ? 0 : 1) + (!isNegative ? 0 : 1)))
     .val(this.val() ? this.val().replace('.', sep) : this.val())
     .bind(events, { ov: this.val() }, handler)
     .bind("blur", { sep: sep, ov: this.val() }, function (e) {
         var self = $(e.currentTarget);
         var sep = e.data.sep;
         if (self.val() === "-") self.val("0.0");
         self.val(self.val().replace("-.", "-0."));
         if (sep && sep != null && self.val().indexOf(sep) != -1) {
             if (self.val().indexOf(e.data.sep) == 0) {
                 self.val("0" + self.val());
             }
             var lastIndexSep = self.val().lastIndexOf(sep);
             if (lastIndexSep == self.val().length - 1) {
                 for (var i = 0; i < ds; i++) {
                     self.val(self.val() + "0");
                 }
             }
         }
     })
    }
})(jQuery);
