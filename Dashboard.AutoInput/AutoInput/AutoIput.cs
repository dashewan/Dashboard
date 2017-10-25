using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Web.Script.Serialization;

namespace Dashboard.AutoInput.AutoInput
{
    public class AutoIput
    {
        #region Private Methods

        private NameValueCollection _valueCollection = new NameValueCollection();

        private IList GetResponseData(NameValueCollection valueCollection)
        {
            return null;
        }

        private string GetHtml(NameValueCollection valueCollection, IList lstEntity)
        {
            string imagePath = valueCollection[AutoControlTag.IMAGE_PATH_TAG];
            string autoControlId = valueCollection[AutoControlTag.AUTOCONTROL_ID_TAG];
            string[] displayFieldNames = valueCollection[AutoControlTag.DISPLAYFIELDS_TAG].Split(AutoControlTag.SPLIT_FIELDS_CHAR_TAG.ToCharArray());
            string[] displayLabels = null;
            //Type eEntityType = geteEntityType(valueCollection[AutoControlTag.ASSEMBLYNAME_TAG]);
            System.Text.StringBuilder innerHTML = new System.Text.StringBuilder();

            if (valueCollection[AutoControlTag.DISPLAYLABELS_TAG] != null &&
                valueCollection[AutoControlTag.DISPLAYLABELS_TAG].Split(AutoControlTag.SPLIT_FIELDS_CHAR_TAG.ToCharArray()).Length ==
                displayFieldNames.Length)
            {
                displayLabels = valueCollection[AutoControlTag.DISPLAYLABELS_TAG].Split(AutoControlTag.SPLIT_FIELDS_CHAR_TAG.ToCharArray());
            }

            if (lstEntity.Count > 0)
            {
                string divHeight = valueCollection[AutoControlTag.AUTOHEIGHT_TAG];

                if (divHeight != null && divHeight.Trim() != "")
                {
                    divHeight = " style='height:" + divHeight + ";overflow:auto;'";
                }
                innerHTML.Append("<div class='autoDiv' " + divHeight + ">");
                innerHTML.Append(" <table class='autoTable' border='0' cellpadding='0' cellspacing='0'  id='" + autoControlId + "AutoSelectTableId'>");
                innerHTML.Append("  <tr class='autoTableHeadClass'>");
                Type typeTmp = lstEntity[0].GetType();
                for (int i = 0; i < displayLabels.Length; i++)
                {
                    if (typeTmp.GetProperty(displayFieldNames[i]) != null)
                    {
                        innerHTML.Append("   <td><div class='headTdText'>" + displayLabels[i] + "</div><span class='sepLineStyle'></span></td>");
                    }
                }
                innerHTML.Append("  </tr>");
                for (int index = 0; index < lstEntity.Count; index++)
                {
                    object eEntity = lstEntity[index];
                    innerHTML.Append("  <tr class='trSourceClass' id='" + autoControlId + "_" + index.ToString() + "' index='" + index.ToString() + "' DbSource='" + getJsonString(eEntity) + "'>");
                    foreach (string displayName in displayFieldNames)
                    {
                        Type type = eEntity.GetType();
                        if (type.GetProperty(displayName) != null)
                        {
                            innerHTML.Append("   <td nowrap>" + type.GetProperty(displayName).GetValue(eEntity, null) + "</td>");
                        }
                    }
                    innerHTML.Append("  </tr>");
                }
                innerHTML.Append(" </table>");
                innerHTML.Append(" </div>");
            }

            #region page info
            string currentPage = valueCollection[AutoControlTag.CURRPENT_PAGE_TAG].Trim();
            int FromRecord = (int.Parse(currentPage) - 1) * int.Parse(valueCollection[AutoControlTag.RECORDSPERPAGE_TAG].Trim()) + 1;
            int ToRecord = (int.Parse(currentPage)) * int.Parse(valueCollection[AutoControlTag.RECORDSPERPAGE_TAG].Trim());
            float total = int.Parse(valueCollection[AutoControlTag.TOTAL_RECORDS_TAG].Trim());

            if (ToRecord > total)
            {
                ToRecord = (int)total;
            }
            int totalPage = (int)System.Math.Floor(total / int.Parse(valueCollection[AutoControlTag.RECORDSPERPAGE_TAG].Trim()));
            float totalPageTmp = total / int.Parse(valueCollection[AutoControlTag.RECORDSPERPAGE_TAG].Trim());
            if (totalPageTmp > totalPage && totalPage < totalPageTmp + 1)
            {
                totalPage = totalPage + 1;
            }
            string[] recordsPerPage = valueCollection[AutoControlTag.OPTIONRECORDS_PAGE_TAG].Trim().Split(AutoControlTag.SPLIT_FIELDS_CHAR_TAG.ToCharArray());

            innerHTML.Append("<div class='autoPageDiv'>");
            innerHTML.Append(" <table border='0' class='autoPageTable'>");
            innerHTML.Append("  <tr>");
            innerHTML.Append("   <td align='left'><a href='#'><img id='" + autoControlId + "firstPage' src='" + imagePath + "/first.gif' width=10 height=18 hspace=5 border=0 align=absmiddle alt='first page' title='first page' class='pageIndex'/></a></td>");
            innerHTML.Append("   <td><a href='#'><img id='" + autoControlId + "prevPage' src='" + imagePath + "/previous.gif' width=10 height=18 hspace=5 border=0 align=absmiddle alt='previous page' title='previous page' class='pageIndex'/></a></td>");
            innerHTML.Append("   <td><a href='#'><img id='" + autoControlId + "nextPage' src='" + imagePath + "/next.gif' width=10 height=18 hspace=5 border=0 align=absmiddle alt='next page' title='next page' class='pageIndex'/></a></td>");
            innerHTML.Append("   <td><a href='#'><img id='" + autoControlId + "lastPage' src='" + imagePath + "/last.gif' width=10 height=18 hspace=5 border=0 align=absmiddle alt='last page' title='last page' class='pageIndex'/></a></td>");
            innerHTML.Append("   <td style='padding-top:4px;display:none;'>");
            innerHTML.Append("   <a id ='aReflesh' href='#'><img id=\"" + autoControlId + "reflashPage\" src=\"" + imagePath + "/Refresh.png\" class='auto_GoToBtn'  alt=\"Reflash\" title=\"Reflash\" height=\"16\" width=\"16\"></a></td>");
            if (valueCollection[AutoControlTag.IS_DEFINEDRECORDS_PERPAGE_TAG] != null && valueCollection[AutoControlTag.IS_DEFINEDRECORDS_PERPAGE_TAG].Trim() == "1")
            {
                //innerHTML.Append(" <select style='width:50px;' id='" + autoControlId + "_selectPageRow'>");
                //foreach (string recordPerPage in recordsPerPage)
                //{
                //    if (valueCollection[AutoControlTag.RECORDSPERPAGE_TAG].Trim() == recordPerPage.Trim())
                //    {
                //        innerHTML.Append("<option value='" + recordPerPage.Trim() + "' selected='selected'>" + recordPerPage.Trim() + "</option>");
                //    }
                //    else
                //    {
                //        innerHTML.Append("<option value='" + recordPerPage.Trim() + "'>" + recordPerPage.Trim() + "</option>");
                //    }
                //}
                //innerHTML.Append("</select>");
            }
            innerHTML.Append("   </td>");

            innerHTML.Append("   <td align='right'>Pages:" + currentPage.ToString() + "/" + totalPage.ToString() + "  Total Records: " + total.ToString() + "</td>");

            innerHTML.Append("  </tr>");
            innerHTML.Append(" </table>");
            innerHTML.Append("</div>");
            innerHTML.Append("<div style='z-index: 15; background-color:Black;'>");
            innerHTML.Append("<input id=\"" + autoControlId + "currPageId\" type=\"hidden\" value=\"" + currentPage + "\" />");
            innerHTML.Append("<input id=\"" + autoControlId + "totalPageId\" type=\"hidden\" value=\"" + totalPage + "\" />");
            innerHTML.Append("<input id=\"" + autoControlId + "totalNumId\" type=\"hidden\" value=\"" + total + "\" />");
            innerHTML.Append("</div>");
            #endregion

            return innerHTML.ToString();
        }

        private string getJsonString(object eEntity)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            new JavaScriptSerializer().Serialize(eEntity, sb);
            return sb.ToString();
        }
        #endregion

        #region Public Methods

        //public NameValueCollection GetCondtions()
        //{
        //    System.Collections.Specialized.NameValueCollection nameValueCollection = new System.Collections.Specialized.NameValueCollection();
        //    foreach (string key in Request.QueryString)
        //    {
        //        nameValueCollection.Add(key, Request.QueryString[key]);
        //    }
        //    foreach (string key in Request.Form)
        //    {
        //        nameValueCollection.Add(key, Request.Form[key]);
        //    }
        //}


        public AutoIput(NameValueCollection valueCollection, int totalCount)
        {
            foreach (string key in valueCollection)
            {
                _valueCollection.Add(key, valueCollection[key]);
            }

            _valueCollection.Add("totalRecords", totalCount.ToString());
        }


        public string GetResponseHtml(IList lstEntity)
        {
            //IList lstEntity = this.GetResponseData(valueCollection);
            return GetHtml(_valueCollection, lstEntity);
        }
        #endregion
    }
}