using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dashboard.Core.Utility.Microsoft.ApplicationBlocks.Data;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace Dashboard.Core.Utility.SqlHelper
{
    public static class SpExecution
    {
        public static DataSet ExecuteDataset(string spName, params SqlParameter[] commandParameters)
        {
            string connectString = ConfigurationManager.ConnectionStrings["DashboardEntities"].ConnectionString;
            //return Dashboard.Core.Utility.Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteDataset(connectString, spName, parameterValues);
            //public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
            DataSet ds = Dashboard.Core.Utility.Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteDataset(connectString, CommandType.StoredProcedure, spName, commandParameters);
            return ds;


        }
        public static DataTable ExecuteDataTable(string strsql)
        {
            string connectString = ConfigurationManager.ConnectionStrings["DashboardEntities"].ConnectionString;
            DataTable dt = Dashboard.Core.Utility.Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteDataset(connectString, CommandType.Text, strsql).Tables[0];
            return dt;
        }

    }
}
