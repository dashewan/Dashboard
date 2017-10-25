using System;
using System.Data;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Dashboard.Common.Util
{
    public class ExcelExportHelper
    {
        private IWorkbook _workbook;
        private string fileName;

        public ExcelExportHelper(string _excelFielPath)
        {
            this.fileName = _excelFielPath;
        }

        #region Excel Export

        public int DataTableToExcel(DataTable data, string sheetName, bool isColumnWritten)
        {
            int i = 0;
            int j = 0;
            int count = 0;
            ISheet sheet = null;

            using (FileStream fs = new FileStream(this.fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                if (this.fileName.IndexOf(".xlsx") > 0) // 2007版本
                    _workbook = new XSSFWorkbook();
                else if (this.fileName.IndexOf(".xls") > 0) // 2003版本
                    _workbook = new HSSFWorkbook();

                try
                {
                    if (_workbook != null)
                    {
                        sheet = _workbook.CreateSheet(sheetName);
                    }
                    else
                    {
                        return -1;
                    }

                    if (isColumnWritten == true) //写入DataTable的列名
                    {
                        IRow row = sheet.CreateRow(0);
                        for (j = 0; j < data.Columns.Count; ++j)
                        {
                            row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                        }
                        count = 1;
                    }
                    else
                    {
                        count = 0;
                    }

                    for (i = 0; i < data.Rows.Count; ++i)
                    {
                        IRow row = sheet.CreateRow(count);
                        for (j = 0; j < data.Columns.Count; ++j)
                        {
                            row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
                        }
                        ++count;
                    }
                    _workbook.Write(fs); //写入到excel
                    return count;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.Message);
                    return -1;
                }
            }
        }
        #endregion
    }
}
