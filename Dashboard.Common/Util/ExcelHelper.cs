using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.IO;
using System.Data;
using System.Globalization;
using System.ComponentModel;

namespace Dashboard.Common.Util
{
    public class ExcelHelper : IDisposable
    {
        private string fileName = null; //文件名
        private IWorkbook workbook = null;
        private FileStream fs = null;
        private bool disposed;

        public ExcelHelper(string fileName)
        {
            this.fileName = fileName;
            disposed = false;
        }

        /// <summary>
        /// 将DataTable数据导入到excel中
        /// </summary>
        /// <param name="data">要导入的数据</param>
        /// <param name="isColumnWritten">DataTable的列名是否要导入</param>
        /// <param name="sheetName">要导入的excel的sheet的名称</param>
        /// <returns>导入数据行数(包含列名那一行)</returns>
        public int DataTableToExcel(DataTable data, string sheetName, bool isColumnWritten)
        {
            int i = 0;
            int j = 0;
            int count = 0;
            ISheet sheet = null;

            fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                workbook = new XSSFWorkbook();
            else if (fileName.IndexOf(".xls") > 0) // 2003版本
                workbook = new HSSFWorkbook();

            try
            {
                if (workbook != null)
                {
                    sheet = workbook.CreateSheet(sheetName);
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
                workbook.Write(fs); //写入到excel
                return count;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// 将excel中的数据导入到DataTable中
        /// </summary>
        /// <param name="sheetName">excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <returns>返回的DataTable</returns>
        public DataTable ExcelToDataTable(string sheetName, bool isFirstRowColumn)
        {
            ISheet sheet = null;
            DataTable data = new DataTable();
            int startRow = 0;
            NpoiDataType[] ColumnDataType = null;
            try
            {
                using (FileStream stream = new FileStream(@fileName, FileMode.Open, FileAccess.Read))
                {
                    workbook = WorkbookFactory.Create(stream);//使用接口，自动识别excel2003/2007格式
                    if (sheetName != null)
                    {
                        sheet = workbook.GetSheet(sheetName);
                    }
                    else
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                    if (sheet != null)
                    {
                        IRow firstRow = sheet.GetRow(0);
                        int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数

                        if (isFirstRowColumn)
                        {
                            for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                            {
                                DataColumn column = new DataColumn(firstRow.GetCell(i).StringCellValue);
                                data.Columns.Add(column);
                            }
                            startRow = sheet.FirstRowNum + 1;
                        }
                        else
                        {
                            startRow = sheet.FirstRowNum;
                        }

                        //最后一列的标号
                        int rowCount = sheet.LastRowNum;
                        for (int i = startRow; i <= rowCount; ++i)
                        {
                            IRow row = sheet.GetRow(i);
                            if (row == null) continue; //没有数据的行默认是null　　　　　　　
                            if (ColumnDataType == null)
                            {
                                ColumnDataType = new NpoiDataType[cellCount];
                                for (int j = 0; j < cellCount; j++)
                                {
                                    ICell hs = row.GetCell(j);
                                    if (hs != null)
                                        ColumnDataType[j] = GetCellDataType(hs);
                                }
                            }

                            DataRow dataRow = data.NewRow();
                            for (int j = row.FirstCellNum; j < cellCount; ++j)
                            {
                                ICell hs = row.GetCell(j);
                                if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null
                                {
                                    if (hs.CellType == CellType.Numeric && HSSFDateUtil.IsCellDateFormatted(hs))
                                    {
                                        dataRow[j] = HSSFDateUtil.GetJavaDate(hs.NumericCellValue).ToString("HH:mm:ss");
                                    }
                                    else
                                    {
                                        dataRow[j] = hs;
                                    }
                                    //dataRow[j] = row.GetCell(j).ToString();
                                }
                                //dataRow[j] = GetCellData(ColumnDataType[j], row, j);
                            }
                            data.Rows.Add(dataRow);
                        }
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return null;
            }
        }

        #region 读Excel-根据NpoiDataType创建的DataTable列的数据类型
        /// <summary>
        /// 读Excel-根据NpoiDataType创建的DataTable列的数据类型
        /// </summary>
        /// <param name="datatype"></param>
        /// <returns></returns>
        private Type GetDataTableType(NpoiDataType datatype)
        {
            Type tp = typeof(string);
            switch (datatype)
            {
                case NpoiDataType.Bool:
                    tp = typeof(bool);
                    break;
                case NpoiDataType.Datetime:
                    tp = typeof(DateTime);
                    break;
                case NpoiDataType.Numeric:
                    tp = typeof(double);
                    break;
                case NpoiDataType.Error:
                    tp = typeof(string);
                    break;
                case NpoiDataType.Blank:
                    tp = typeof(string);
                    break;
            }
            return tp;
        }
        #endregion

        #region 读Excel-得到不同数据类型单元格的数据
        /// <summary>
        /// 读Excel-得到不同数据类型单元格的数据
        /// </summary>
        /// <param name="datatype">数据类型</param>
        /// <param name="row">数据中的一行</param>
        /// <param name="column">哪列</param>
        /// <returns></returns>
        private object GetCellData(NpoiDataType datatype, IRow row, int column)
        {

            switch (datatype)
            {
                case NpoiDataType.String:
                    try
                    {
                        return row.GetCell(column).DateCellValue;
                    }
                    catch
                    {
                        try
                        {
                            return row.GetCell(column).StringCellValue;
                        }
                        catch
                        {
                            return row.GetCell(column).NumericCellValue;
                        }
                    }
                case NpoiDataType.Bool:
                    try { return row.GetCell(column).BooleanCellValue; }
                    catch { return row.GetCell(column).StringCellValue; }
                case NpoiDataType.Datetime:
                    try { return row.GetCell(column).DateCellValue; }
                    catch { return row.GetCell(column).StringCellValue; }
                case NpoiDataType.Numeric:
                    try { return row.GetCell(column).NumericCellValue; }
                    catch { return row.GetCell(column).StringCellValue; }
                case NpoiDataType.Richtext:
                    try { return row.GetCell(column).RichStringCellValue; }
                    catch { return row.GetCell(column).StringCellValue; }
                case NpoiDataType.Error:
                    try { return row.GetCell(column).ErrorCellValue; }
                    catch { return row.GetCell(column).StringCellValue; }
                case NpoiDataType.Blank:
                    try { return row.GetCell(column).StringCellValue; }
                    catch { return ""; }
                default: return "";
            }
        }
        #endregion

        #region 获取单元格数据类型
        /// <summary>
        /// 获取单元格数据类型
        /// </summary>
        /// <param name="hs"></param>
        /// <returns></returns>
        private NpoiDataType GetCellDataType(ICell hs)
        {
            NpoiDataType dtype;
            DateTime t1;
            string cellvalue = "";
            switch (hs.CellType)
            {
                case CellType.Blank:
                    dtype = NpoiDataType.String;
                    cellvalue = hs.StringCellValue;
                    break;
                case CellType.Boolean:
                    dtype = NpoiDataType.Bool;
                    break;
                case CellType.Numeric:
                    dtype = NpoiDataType.Numeric;
                    cellvalue = hs.NumericCellValue.ToString();
                    break;
                case CellType.String:
                    dtype = NpoiDataType.String;
                    cellvalue = hs.StringCellValue;
                    break;
                case CellType.Error:
                    dtype = NpoiDataType.Error;
                    break;
                case CellType.Formula:
                default:
                    dtype = NpoiDataType.Datetime;
                    break;
            }
            if (cellvalue != "" && DateTime.TryParse(cellvalue, out t1)) dtype = NpoiDataType.Datetime;
            return dtype;
        }
        #endregion

        /// <summary> 
        /// NOPI要转文件的转化 
        /// </summary> 
        /// <param name="cell"></param> 
        /// <param name="conversionType"></param> 
        /// <returns></returns> 
        private object ChangeTypeTo(ICell cell, Type conversionType)
        {
            if (conversionType == null)
                throw new ArgumentNullException("conversionType");
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (cell == null)
                    return null;
                NullableConverter nullableConverter = new NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }
            if (conversionType == typeof(string) && (cell == null || string.IsNullOrEmpty(cell.ToString())))
                return null;
            if (conversionType == typeof(Guid))
            {
                return new Guid(cell.ToString());
            }
            else if (conversionType == typeof(DateTime))
            {
                if (cell.CellType == CellType.Numeric)
                    return cell.DateCellValue;
                else
                {
                        return DateTime.Parse(cell.StringCellValue, new CultureInfo("fr-FR"));
                }
            }
            else if (cell.CellType == CellType.Numeric)
            {
                return Convert.ChangeType(cell.NumericCellValue, conversionType);
            }
            else
            {
                return Convert.ChangeType(cell.StringCellValue, conversionType);
            }
        }

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (fs != null)
                        fs.Close();
                }

                fs = null;
                disposed = true;
            }
        }
        #endregion
    }

    #region 枚举(Excel单元格数据类型)
    /// <summary>
    /// 枚举(Excel单元格数据类型)
    /// </summary>
    public enum NpoiDataType
    {
        /// <summary>
        /// 字符串类型-值为1
        /// </summary>
        String,
        /// <summary>
        /// 布尔类型-值为2
        /// </summary>
        Bool,
        /// <summary>
        /// 时间类型-值为3
        /// </summary>
        Datetime,
        /// <summary>
        /// 数字类型-值为4
        /// </summary>
        Numeric,
        /// <summary>
        /// 复杂文本类型-值为5
        /// </summary>
        Richtext,
        /// <summary>
        /// 空白
        /// </summary>
        Blank,
        /// <summary>
        /// 错误
        /// </summary>
        Error
    }
    #endregion
}
