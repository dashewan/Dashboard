/*
* Utility 1.1
*
* AUTHORS.txt Ryanding
*
* Copyright (c) 2013 Ryanding
* jqGrid addRowData extend
*/

$.jgrid.extend({
    addRowData: function (rowid, rdata, pos, src) {
        //debugger;
        if (!pos) { pos = "last"; }
        var success = false, nm, row, gi, si, ni, sind, i, v, prp = "", aradd, cnm, cn;
        if (rdata) {
            if ($.isArray(rdata)) {
                aradd = true;
                pos = "last";
                cnm = rowid;
            } else {
                rdata = [rdata];
                aradd = false;
            }
            this.each(function () {
                var t = this, rowslen = t.rows.length || 0, datalen = rdata.length;
                ni = t.p.rownumbers === true ? 1 : 0;
                gi = t.p.multiselect === true ? 1 : 0;
                si = t.p.subGrid === true ? 1 : 0;
                if (!aradd) {
                    if (typeof (rowid) != 'undefined') rowid = rowid + "";
                    else {
                        rowid = (t.p.records + 1) + "";
                        if (t.p.keyIndex !== false) {
                            cmn = t.p.colModel[t.p.keyIndex + gi + si + ni].name;
                            if (typeof rdata[0][cmn] != "undefined") rowid = rdata[0][cmn];
                        }
                    }
                }
                cn = t.p.altclass;
                var k = 0;
                var air = $.isFunction(t.p.afterInsertRow) ? true : false;
                while (k < datalen) {
                    data = rdata[k];
                    row = "";
                    if (aradd) {
                        try { rowid = data[cnm]; }
                        catch (e) { rowid = t.p.records + 1; }
                        var cna = t.p.altRows === true ? (t.rows.length - 1) % 2 == 0 ? cn : "" : "";
                    }
                    if (ni) {
                        prp = t.formatCol(ni, 1);
                        row += "<td role=\"gridcell\" class=\"ui-state-default jqgrid-rownum\" " + prp + ">0</td>";
                    }
                    if (gi) {
                        v = "<input type=\"checkbox\"" + " id=\"jqg_" + t.id + "_" + rowid + "\" name=\"jqg_" + t.id + "_" + rowid + "\" class=\"cbox\"/>";
                        prp = t.formatCol(ni, 1);
                        row += "<td role=\"gridcell\" " + prp + ">" + v + "</td>";
                    }
                    if (si) {
                        row += $(t).jqGrid("addSubGridCell", gi + ni, 1);
                    }

                    //begin modify
                    row = "<tr id=\"" + rowid + "\" role=\"row\" class=\"ui-widget-content jqgrow ui-row-" + t.p.direction + " " + cna + "\">" + row + "</tr>";
                    if (t.rows.length === 0) {
                        $("table:first", t.grid.bDiv).append(row);
                    }
                    else { //insert to first
                        if (pos == "first")
                            $(t.rows[0]).before(row);
                        else
                            $(t.rows[t.rows.length - 1]).after(row);
                    }

                    for (i = gi + si + ni; i < this.p.colModel.length; i++) {
                        nm = this.p.colModel[i].name;
                        v = t.formatter(rowid, data[nm], i, data, 'add');
                        prp = t.formatCol(i, 1);
                        if (data[nm] == null) data[nm] = "";
                        if (data[nm] == "") v = "";
                        if (typeof data[nm] === "object" && typeof data[nm] != null) {
                            if (pos == "first") {
                                $(t.rows[0]).append("<td role=\"gridcell\" " + prp + " title=\"" + $.jgrid.stripHtml(v) + "\">" + "</td>");
                                $(t.rows[0]).find("td:last").append(data[nm]);
                            } else {
                                $(t.rows[t.rows.length - 1]).append("<td role=\"gridcell\" " + prp + " title=\"" + $.jgrid.stripHtml(v) + "\">" + "</td>");
                                $(t.rows[t.rows.length - 1]).find("td:last").append(data[nm]);
                            }
                        } else
                            if (pos == "first") {
                                $(t.rows[0]).append($("<td role=\"gridcell\" " + prp + " title=\"" + $.jgrid.stripHtml(v) + "\"></td>").text(v));
                            } else {
                                $(t.rows[t.rows.length - 1]).append($("<td role=\"gridcell\" " + prp + " title=\"" + $.jgrid.stripHtml(v) + "\"></td>").text(v));
                            }


                    }
                    /*
                    row = "<tr id=\"" + rowid + "\" role=\"row\" class=\"ui-widget-content jqgrow ui-row-" + t.p.direction + " " + cna + "\">" + row + "</tr>";
                    if (t.p.subGrid === true) {
                    row = $(row)[0];
                    $(t).jqGrid("addSubGrid", row, gi + ni);
                    }
                    if (t.rows.length === 0) {
                    $("table:first", t.grid.bDiv).append(row);
                    } else {
                    switch (pos) {
                    case 'last':
                    $(t.rows[t.rows.length - 1]).after(row);
                    break;
                    case 'first':
                    $(t.rows[0]).before(row);
                    break;
                    case 'after':
                    sind = t.rows.namedItem(src);
                    if (sind) { $(t.rows[sind.rowIndex + 1]).hasClass("ui-subgrid") ? $(t.rows[sind.rowIndex + 1]).after(row) : $(sind).after(row); }
                    break;
                    case 'before':
                    sind = t.rows.namedItem(src);
                    if (sind) { $(sind).before(row); sind = sind.rowIndex; }
                    break;
                    }
                    }
                    */
                    t.p.records++;
                    t.p.reccount++;
                    if (!t.grid.cols || !t.grid.cols.length) t.grid.cols = t.rows[0].cells;
                    if (pos === 'first' || (pos === 'before' && sind <= 1) || t.rows.length === 1) {
                        t.updateColumns();
                    }
                    if (air) t.p.afterInsertRow(t, rowid, data);
                    k++;
                }
                if (t.p.altRows === true && !aradd) {
                    if (pos == "last") {
                        if ((t.rows.length - 1) % 2 == 1) { $(t.rows[t.rows.length - 1]).addClass(cn); }
                    } else {
                        $(t.rows).each(function (i) {
                            if (i % 2 == 1) $(this).addClass(cn);
                            else $(this).removeClass(cn);
                        });
                    }
                }
                t.updatepager(true, true);
                success = true;
            });
        }
        return success;
    }
});