using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.IO;
using Microsoft.Office.Interop.Excel;
using System.Diagnostics;

namespace Dashboard.Core.Utility
{
    public static class ExcelUtil
    {
        private static OleDbConnection connection;

        private static string[] sheetNames;

        private static string[] SheetNames
        {
            get
            {
                try
                {
                    connection.Open();
                    System.Data.DataTable schema = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                    sheetNames = new string[schema.Rows.Count];
                    for (int i = 0; i < schema.Rows.Count; i++)
                    {
                        sheetNames[i] = schema.Rows[i]["TABLE_NAME"].ToString();
                    }
                }
                finally
                {
                    connection.Close();
                }
                return sheetNames;
            }
        }

        public static string[] GetSheetList(string path)
        {
            if (!System.IO.File.Exists(path))
                throw new FileNotFoundException();

            string ext = Path.GetExtension(path).ToLower();

            if (ext == ".xls")
                connection = new OleDbConnection(String.Format(
                    //"Provider=Microsoft.Jet.OLEDB.4.0; Extended properties=Excel 8.0; Data Source={0}",
                    //path
                       "Provider=Microsoft.Jet.OLEDB.4.0;Data Source = " + path + ";Extended Properties ='Excel 8.0;HDR=YES;IMEX=1'"
                       ));
            else // xlsx.
                connection = new OleDbConnection(String.Format(
                    "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES\"",
                    path));
            return SheetNames;

        }

        public static System.Data.DataTable FillDataFromXlsFile(string sheetName, System.Data.DataTable dataTable)
        {
            try
            {
                connection.Open();
                OleDbDataAdapter adapter = new OleDbDataAdapter(String.Format("select * from [{0}]", sheetName), connection);
                adapter.Fill(dataTable);
                return dataTable;
            }
            finally
            {
                connection.Close();
            }
        }

        #region ChangeSheetName
        /// <summary>
        /// 修改Sheet名字
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="str">Sheet名称</param>
        public static void ChangeSheetName(string path, string[] str)
        {
            Application objExcel = new Application();
            _Workbook objBook = objExcel.Workbooks.Open(path, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            for (int i = 0; i < str.Length; i++)
            {
                _Worksheet objSheet = (Worksheet)objBook.Worksheets.get_Item(i + 1);
                objSheet.Name = str[i];
            }
            objExcel.DisplayAlerts = false;
            objBook.SaveAs(path, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            objExcel.DisplayAlerts = true;
            objBook.Close(false, Type.Missing, Type.Missing);
            objExcel.Quit();
            GC.Collect();
            //Process.GetProcesses().Where(p => p.ProcessName == "EXCEL").FirstOrDefault().Kill();
        } 
        #endregion
    }
}
