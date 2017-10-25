using System;
using System.Collections.Generic;
using System.Text;

namespace Dashboard.AutoInput.AutoInput
{
    public class AutoControlTag
    {
        /// <summary>
        /// selectId
        /// </summary>
        public const string AUTOCONTROL_ID_TAG = "selectId";

        /// <summary>
        /// Mapping Name
        /// </summary>
        public const string MAPPINGNAME_TAG = "mappingName";

        /// <summary>
        /// default Condition
        /// </summary>
        public const string DEFAULTCONDITIOPN_TAG = "defaultCondition";

        /// <summary>
        /// display Fields
        /// </summary>
        public const string DISPLAYFIELDS_TAG = "displayFields";

        /// <summary>
        /// display Labels
        /// </summary>
        public const string DISPLAYLABELS_TAG = "displayLabels";

        /// <summary>
        /// field Name for query
        /// </summary>
        public const string QUERYFIELDNAME_TAG = "queryFieldName";

        /// <summary>
        /// field for query
        /// </summary>
        public const string QUERYFIELDVALUE_TAG = "queryFieldvalue";

        /// <summary>
        /// sort Fields
        /// </summary>
        public const string SORTFIELDS_TAG = "sortFields";

        /// <summary>
        /// assembly for load object
        /// </summary>
        public const string ASSEMBLYNAME_TAG = "assemblyName";

        /// <summary>
        /// auto Height
        /// </summary>
        public const string AUTOHEIGHT_TAG = "autoHeight";

        /// <summary>
        /// auto Width
        /// </summary>
        public const string AUTOWIDTH_TAG = "autoWidth";

        /// <summary>
        /// character Num
        /// </summary>
        public const string CHARACTERNUM_TAG = "characterNum";

        /// <summary>
        /// current Page
        /// </summary>
        public const string CURRPENT_PAGE_TAG = "currentPage";

        /// <summary>
        /// Total records 
        /// </summary>
        public const string TOTAL_RECORDS_TAG = "totalRecords";

        /// <summary>
        /// records Per Page
        /// </summary>
        public const string RECORDSPERPAGE_TAG = "recordsPerPage";

        /// <summary>
        /// char for split query field
        /// </summary>
        public const string SPLIT_FIELDS_CHAR_TAG = ";";

        /// <summary>
        /// char for split field and value
        /// </summary>
        public const string SPLIT_FIELD_VALUE_CHAR_TAG = ":";

        /// <summary>
        /// {
        /// </summary>
        public const string BEGIN_LIST_VALUE_CHAR_TAG = "{";

        /// <summary>
        /// }
        /// </summary>
        public const string END_LIST_VALUE_CHAR_TAG = "}";

        /// <summary>
        /// ,
        /// </summary>
        public const string SPLIT_LIST_VALUE_CHAR_TAG = ",";

        /// <summary>
        /// isRefalshFromDb
        /// </summary>
        public const string IS_REFLASH_FROMDB = "isRefalshFromDb";

        /// <summary>
        /// PreFix
        /// </summary>
        public const string PREFIX_TAG = "Domain.";

        /// <summary>
        /// gnoreIUCase
        /// </summary>
        public const string GNORE_IUCASE = "gnoreIUCase";

        /// <summary>
        /// cachePk
        /// </summary>
        public const string CACHE_PK = "cachePk";

        /// <summary>
        /// Sort field Name
        /// </summary>
        public const string SORT_FIELDNAME_TAG = "SortFields";

        /// <summary>
        /// Image path
        /// </summary>
        public const string IMAGE_PATH_TAG = "imagePath";

        /// <summary>
        /// optionRecordsPerPage
        /// </summary>
        public const string OPTIONRECORDS_PAGE_TAG = "optionRecordsPerPage";


        /// <summary>
        /// isDefinedRecordsPerPage
        /// </summary>
        public const string IS_DEFINEDRECORDS_PERPAGE_TAG = "isDefinedRecordsPerPage";

        /// <summary>
        /// Mutiple Supported 
        /// </summary>
        public const string EXTRAQUERYFIELDNAME_TAG = "ExtraQueryFieldNames";
    }


    public class AutoMemCachedTag
    {
        public const string TOTAL_RECORDS_TAG = "TotalRecords";

        public const string CACHE_PREFIX_TAG = "Domain.";

    }
}
