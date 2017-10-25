using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.Util;
using System.Reflection;
using Dashboard.Domain.ValidationAttributes;

namespace Dashboard.Common.Util
{
    public class NpoiHelper
    {
        private IWorkbook _workbook;
        private bool AutoColumnHeder;

        #region MonthStr
        private enum MonthStr
        {
            Jan = 1,
            Feb = 2,
            Mar = 3,
            Apr = 4,
            Friday = 5,
            Jun = 6,
            Jul = 7,
            Aug = 8,
            Sept = 9,
            Oct = 10,
            Nov = 11,
            Dec = 12
        }
        #endregion

        #region initHSSFWorkbook
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="_excelFielPath"></param>
        private void initHSSFWorkbook(string _excelFielPath)
        {
            if (string.IsNullOrEmpty(_excelFielPath))
            {
                throw new Exception("Excel文件路径不能为空！");
            }
            if (!File.Exists(_excelFielPath))
            {
                throw new Exception("指定路径的Excel文件不存在！");
            }
            string str = Path.GetExtension(_excelFielPath).ToLower();
            using (FileStream stream = new FileStream(_excelFielPath, FileMode.Open, FileAccess.Read))
            {
                this._workbook = WorkbookFactory.Create(stream);//使用接口，自动识别excel2003/2007格式
            }
        }
        #endregion

        #region GetExcelTablesName
        /// <summary>
        /// 得到Excel所有的Sheet Name
        /// </summary>
        /// <returns></returns>
        private List<string> GetExcelTablesName()
        {
            List<string> list = new List<string>();
            foreach (ISheet sheet in this._workbook)
            {
                list.Add(sheet.SheetName);
            }
            return list;
        }
        #endregion

        #region IsExistExcelTableName
        /// <summary>
        /// Excel是否存在Sheet
        /// </summary>
        /// <param name="tName">sheet name</param>
        /// <returns></returns>
        private bool IsExistExcelTableName(string tName)
        {
            return this.GetExcelTablesName().Contains(tName);
        }
        #endregion

        #region Constructor
        public NpoiHelper(string _excelFielPath)
        {
            this.AutoColumnHeder = false;
            this.initHSSFWorkbook(_excelFielPath);
        }

        public NpoiHelper(string _excelFielPath, bool _autoColumnHeder)
        {
            this.initHSSFWorkbook(_excelFielPath);
            this.AutoColumnHeder = _autoColumnHeder;
        }

        public NpoiHelper(string _excelFielPath, string sheetName)
        {
            this.AutoColumnHeder = false;
            this.initHSSFWorkbook(_excelFielPath);
            if (!this.IsExistExcelTableName(sheetName))
            {
                throw new Exception("指定的模板格式不正确，请选择正确的EXCEL模板");
            }
        }
        #endregion

        #region Excel Import

        /// <summary>
        /// Excel 转换为Dataset
        /// </summary>
        /// <returns></returns>
        public DataSet ExcelToDataSet()
        {
            DataSet ds = new DataSet();
            List<string> excelTablesName = this.GetExcelTablesName();

            foreach (string str in excelTablesName)
            {
                ds.Tables.Add(this.ExcelToDataTable(str));
            }
            return ds;
        }

        /// <summary>
        /// Excel 转换为DataTable
        /// </summary>
        /// <param name="index">Sheet 标号</param>
        /// <returns></returns>
        public DataTable ExcelToDataTable(int index)
        {
            ISheet sheetAt = this._workbook.GetSheetAt(index);
            return this.ExcelToDataTable(sheetAt, 0);
        }

        /// <summary>
        /// Excel 转换为DataTable
        /// </summary>
        /// <param name="tName">Sheet name</param>
        /// <returns></returns>
        public DataTable ExcelToDataTable(string tName)
        {
            ISheet sheet = this._workbook.GetSheet(tName);
            return this.ExcelToDataTable(sheet, 0);

        }

        /// <summary>
        /// Excel 转换为DataTable
        /// </summary>
        /// <param name="tName">Sheet name</param>
        /// <param name="headerIndex">表头行</param>
        /// <returns></returns>
        public DataTable ExcelToDataTable(string tName, int headerIndex)
        {
            ISheet sheet = this._workbook.GetSheet(tName);
            return this.ExcelToDataTable(sheet, headerIndex);
        }

        /// <summary>
        /// Excel 转换为DataTable
        /// </summary>
        /// <param name="sheet">ISheet</param>
        /// <param name="headerIndex">表头行</param>
        /// <returns></returns>
        private DataTable ExcelToDataTable(ISheet sheet, int headerIndex)
        {
            if (sheet == null)
            {
                throw new Exception("Excel模板格式不对，读取sheet错误");
            }
            if (sheet.LastRowNum < headerIndex)
            {
                throw new Exception("Excel模板格式不对，读取下标值错误");
            }

            int startRow = headerIndex;
            DataTable table = new DataTable();
            IRow row = sheet.GetRow(headerIndex);

            int lastCellNum = row.LastCellNum;

            for (int i = 0; i < lastCellNum; i++)
            {
                string columnName = string.Empty;
                if (this.AutoColumnHeder)
                {
                    columnName = Convert.ToChar(0x41) + i.ToString();
                }
                else
                {
                    columnName = row.GetCell(i).ToString();
                    startRow = headerIndex + 1;
                }
                table.Columns.Add(columnName);
            }

            int rowCount = sheet.LastRowNum; //最后一行的标号
            for (int i = startRow; i <= rowCount; ++i)
            {
                IRow current = sheet.GetRow(i);
                if (current == null)
                    continue; //没有数据的行默认是null

                DataRow dataRow = table.NewRow();
                for (int j = current.FirstCellNum; j < lastCellNum; ++j)
                {
                    ICell cell = current.GetCell(j);
                    if (cell != null) //没有数据的单元格都默认是null
                    {
                        //如果单元格格式是时间格式的转换为HH:mm:ss
                        if (cell.CellType == CellType.Numeric && HSSFDateUtil.IsCellDateFormatted(cell))
                        {
                            dataRow[j] = HSSFDateUtil.GetJavaDate(cell.NumericCellValue).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        else
                        {
                            dataRow[j] = cell;
                        }
                    }
                }
                table.Rows.Add(dataRow);
            }
            return table;
        }
        #endregion

        public static int NpoiColorTest(string strFileName)
        {
            IWorkbook workbook = null;
            if (strFileName.IndexOf(".xlsx") > 0) // 2007版本
                workbook = new XSSFWorkbook();
            else if (strFileName.IndexOf(".xls") > 0) // 2003版本
                workbook = new HSSFWorkbook();
            string sheetname = "ColorTest";
            try
            {
                ISheet sheet = workbook.CreateSheet(sheetname);
                IRow row = sheet.CreateRow(0);
                row.CreateCell(0).SetCellValue("顏色");
                row.CreateCell(1).SetCellValue("測試");
                row.CreateCell(2).SetCellValue("Class名稱");
                row.CreateCell(3).SetCellValue("short");

                Type colorType = typeof(NPOI.HSSF.Util.HSSFColor);
                //找出此Type下公用的class
                var colorInstance = colorType.GetNestedTypes(BindingFlags.Public);
                int count = 2;

                foreach (var item in colorInstance)
                {

                    //找出class下的靜態名為index的欄位  

                    var shortColor = item.GetField("Index").GetValue(null);

                    IRow r = sheet.CreateRow(count);

                    //第一種style，設字型的顏色  

                    ICellStyle cellStyleFontColor = workbook.CreateCellStyle();

                    IFont fontS = workbook.CreateFont();

                    fontS.Color = (short)shortColor;

                    cellStyleFontColor.SetFont(fontS);

                    //第二種style，設儲存格的前景色  

                    ICellStyle cellStyleBg = workbook.CreateCellStyle();

                    cellStyleBg.FillForegroundColor = (short)shortColor;

                    cellStyleBg.FillPattern = FillPattern.SolidForeground;

                    r.CreateCell(0).CellStyle = cellStyleBg;

                    ICell cell1 = r.CreateCell(1);

                    cell1.CellStyle = cellStyleFontColor;

                    cell1.SetCellValue("Test顏色");

                    r.CreateCell(2).SetCellValue(item.Name);

                    r.CreateCell(3).SetCellValue(shortColor.ToString());

                    count++;

                }
                int lastCellNum = row.LastCellNum;

                for (int i = 0; i < lastCellNum; i++)
                {
                    sheet.AutoSizeColumn(i);
                }
                using (FileStream fs = new FileStream(strFileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    workbook.Write(fs); //写入到excel
                }
                return count;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return -1;
            }
        }

        public static int Export(DataTable dtSource, string strFileName)
        {
            IWorkbook workbook = null;
            if (strFileName.IndexOf(".xlsx") > 0) // 2007版本
                workbook = new XSSFWorkbook();
            else if (strFileName.IndexOf(".xls") > 0) // 2003版本
                workbook = new HSSFWorkbook();

            string sheetname = "Customer Performance Report";
            try
            {
                IFont font1 = workbook.CreateFont();
                font1.FontName = "CorpoS";
                //font1.Boldweight = (short)FontBoldWeight.Bold;//字体加粗样式
                //font1.Color = HSSFColor.Blue.Index;
                font1.FontHeight = 10;


                ICellStyle style1 = workbook.CreateCellStyle();
                style1.FillForegroundColor = HSSFColor.LemonChiffon.Index;
                style1.FillPattern = FillPattern.SolidForeground;
                style1.Alignment = HorizontalAlignment.Center;
                style1.VerticalAlignment = VerticalAlignment.Center;
                style1.SetFont(font1);
                style1.BorderBottom = BorderStyle.Thin;
                style1.BorderLeft = BorderStyle.Thin;
                style1.BorderRight = BorderStyle.Thin;
                style1.BorderTop = BorderStyle.Thin;

                ISheet sheet = workbook.CreateSheet(sheetname);
                IRow row = sheet.CreateRow(1);
                foreach (DataColumn column in dtSource.Columns)
                {
                    ICell newCell = row.CreateCell(column.Ordinal + 1);
                    newCell.SetCellValue(column.ColumnName);
                    newCell.CellStyle = style1;
                }
                int rownum = 2;

                foreach (DataRow row2 in dtSource.Rows)
                {

                    IRow row3 = sheet.CreateRow(rownum);
                    foreach (DataColumn column in dtSource.Columns)
                    {
                        ICell newCell = row3.CreateCell(column.Ordinal + 1);
                        newCell.SetCellValue(row2[column].ToString());
                    }
                    rownum++;
                }

                sheet.AddMergedRegion(new CellRangeAddress(2, 11, 1, 1));
                sheet.AddMergedRegion(new CellRangeAddress(2, 11, 2, 2));

                int lastCellNum = row.LastCellNum;

                for (int i = 0; i < lastCellNum; i++)
                {
                    sheet.AutoSizeColumn(i);
                }
                using (FileStream fs = new FileStream(strFileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    workbook.Write(fs); //写入到excel
                }
                return rownum;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return -1;
            }
        }


        #region GenerateHeadStyle
        /// <summary>
        /// 生成表头样式 浅黄,居中,边框 CorpoS 11
        /// </summary>
        /// <param name="workbook"></param>
        /// <returns></returns>
        private static ICellStyle GenerateHeadStyle(IWorkbook workbook)
        {
            ICellStyle styleHeader = workbook.CreateCellStyle();
            styleHeader.FillForegroundColor = HSSFColor.LemonChiffon.Index;
            styleHeader.FillPattern = FillPattern.SolidForeground;
            styleHeader.Alignment = HorizontalAlignment.Center;
            styleHeader.VerticalAlignment = VerticalAlignment.Center;
            styleHeader.BorderBottom = BorderStyle.Thin;
            styleHeader.BorderLeft = BorderStyle.Thin;
            styleHeader.BorderRight = BorderStyle.Thin;
            styleHeader.BorderTop = BorderStyle.Thin;

            IFont fontHeader = workbook.CreateFont();
            fontHeader.FontName = "CorpoS";
            fontHeader.FontHeight = 11;
            styleHeader.SetFont(fontHeader);

            return styleHeader;
        }
        #endregion

        #region GenerateContentStyle
        /// <summary>
        /// 生成内容样式 边框 CorpoS 10
        /// </summary>
        /// <param name="workbook"></param>
        /// <returns></returns>
        private static ICellStyle GenerateContentStyle(IWorkbook workbook)
        {
            ICellStyle styleContent = workbook.CreateCellStyle();
            styleContent.BorderBottom = BorderStyle.Thin;
            styleContent.BorderLeft = BorderStyle.Thin;
            styleContent.BorderRight = BorderStyle.Thin;
            styleContent.BorderTop = BorderStyle.Thin;

            IFont fontContent = workbook.CreateFont();
            fontContent.FontName = "CorpoS";
            fontContent.FontHeight = 10;

            styleContent.SetFont(fontContent);
            return styleContent;
        }
        #endregion

        #region AllDataFormExportX
        public static void AllDataFormExportX(DataTable dtSource, string strFileName, string sheetname)
        {
            IWorkbook workbook = null;
            if (strFileName.IndexOf(".xlsx") > 0) // 2007版本
                workbook = new XSSFWorkbook();
            else if (strFileName.IndexOf(".xls") > 0) // 2003版本
                workbook = new HSSFWorkbook();

            if (sheetname == "")
                sheetname = "Report";


            if (File.Exists(strFileName))
                File.Delete(strFileName);

            ISheet sheet = workbook.CreateSheet(sheetname);
            using (FileStream fs = File.Create(strFileName))
            {
                workbook.Write(fs); //写入到excel
            }
            try
            {
                #region Header
                IWorkbook workbook2 = null;
                using (FileStream fs = File.Open(strFileName, FileMode.Open, FileAccess.Read))
                {
                    workbook2 = WorkbookFactory.Create(fs);
                    fs.Close();
                    fs.Dispose();
                }
                ISheet sheet2 = workbook2.GetSheet(sheetname);
                ICellStyle styleHeader = GenerateHeadStyle(workbook2);
                IRow row = sheet2.CreateRow(0);
                for (int i = 0; i < dtSource.Columns.Count; i++)
                {
                    DataColumn column = dtSource.Columns[i];
                    ICell newCell = row.CreateCell(i);
                    newCell.SetCellValue(column.ColumnName);
                    newCell.CellStyle = styleHeader;
                }
                for (int i = 0; i < row.LastCellNum; i++)
                {
                    sheet2.AutoSizeColumn(i);
                }
                #endregion
                using (FileStream fs = new FileStream(strFileName, FileMode.Open, FileAccess.Write))
                {
                    workbook2.Write(fs); //写入到excel
                }
                int rownum = 1;
                int pageSize = 10000;
                int totalRecordCount = dtSource.Rows.Count;
                int totalPageCount = totalRecordCount % pageSize == 0 ? totalRecordCount / pageSize : totalRecordCount / pageSize + 1;
                for (int i = 1; i <= totalPageCount; i++)
                {
                    IWorkbook workbook1 = null;
                    using (FileStream fs = File.Open(strFileName, FileMode.Open, FileAccess.Read))
                    {
                        workbook1 = WorkbookFactory.Create(fs);
                    }
                    ISheet sheet1 = workbook1.GetSheetAt(0);

                    ICellStyle styleContent = GenerateContentStyle(workbook1);
                    for (int j = (i - 1) * pageSize; j < (i - 1) * pageSize + pageSize; j++)
                    {
                        if (j >= totalRecordCount) break;
                        DataRow row2 = dtSource.Rows[j];
                        IRow row3 = sheet1.CreateRow(rownum);
                        for (int k = 0; k < dtSource.Columns.Count; k++)
                        {
                            DataColumn column = dtSource.Columns[k];
                            ICell newCell = row3.CreateCell(k);
                            newCell.SetCellValue(row2[column].ToString());
                            newCell.CellStyle = styleContent;
                        }
                        rownum++;
                    }
                    using (FileStream fs = new FileStream(strFileName, FileMode.Open, FileAccess.Write))
                    {
                        workbook1.Write(fs); //写入到excel
                        fs.Close();
                        fs.Dispose();
                    }
                    sheet1 = null;
                    workbook1 = null;
                    GC.Collect();
                }
                dtSource = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region AllDataFormExport
        public static void AllDataFormExport(DataTable dtSource, string strFileName, string sheetname)
        {
            IWorkbook workbook = null;
            ISheet sheet = null;
            if (strFileName.IndexOf(".xlsx") > 0) // 2007版本
                workbook = new XSSFWorkbook();
            else if (strFileName.IndexOf(".xls") > 0) // 2003版本
                workbook = new HSSFWorkbook();
            if (sheetname == "")
                sheetname = "Report";
            try
            {
                #region Header
                ICellStyle styleHeader = GenerateHeadStyle(workbook);
                sheet = workbook.CreateSheet(sheetname);
                IRow row = sheet.CreateRow(0);
                for (int i = 0; i < dtSource.Columns.Count; i++)
                {
                    DataColumn column = dtSource.Columns[i];
                    ICell newCell = row.CreateCell(i);
                    newCell.SetCellValue(column.ColumnName);
                    newCell.CellStyle = styleHeader;
                }
                for (int i = 0; i < row.LastCellNum; i++)
                {
                    sheet.AutoSizeColumn(i);
                }
                #endregion

                ICellStyle styleContent = GenerateContentStyle(workbook);
                //用于区分转换为日期格式的列
                ICellStyle styleContentForDefault= GenerateContentStyle(workbook);
                int rownum = 1;
                foreach (DataRow row2 in dtSource.Rows)
                {
                    IRow row3 = sheet.CreateRow(rownum);
                    for (int i = 0; i < dtSource.Columns.Count; i++)
                    {
                        DataColumn column = dtSource.Columns[i];
                        ICell newCell = row3.CreateCell(i);
                        DateTime reldt;
                        double relde;
                        string cluName = column.ColumnName;
                        switch (cluName)
                        {
                            case "Operator Creation Time":
                            case "Supervisor Confirm Time":
                            case "TN Print Time":
                            case "Contract Arriving Time by Cargo Type (Earliest)":
                            case "Contract Arriving Time by Trans Mode(Earliest)":
                            case "Contract Arriving Time by Cargo Type (latest)":
                            case "Contract Arriving Time by Trans Mode (latest)":
                            case "Receiving Time":
                                if (DateTime.TryParse(row2[column].ToString(), out reldt))
                                {
                                    IDataFormat dataformat = workbook.CreateDataFormat();
                                    styleContent.DataFormat = dataformat.GetFormat("yyyy-MM-dd hh:mm:ss");
                                    newCell.SetCellValue(reldt);//如果是日期类型则做转换,以使导出的Excel为日期类型的
                                    newCell.CellStyle = styleContent;
                                }
                                else
                                {
                                    newCell.SetCellValue(row2[column].ToString());
                                    newCell.CellStyle = styleContentForDefault;
                                }
                                break;
                            case "Load Rate":
                            case "Case Qty":
                            case "Volume":
                            case "Gross Weight":
                            case "Total Chargable Weight":
                            case "Facing Distance":
                            case "Drop Off Fee":
                            case "Special Fee":
                            case "Discount Fee":
                            case "Total Charge":
                            case "Total Charge by Volume":
                            case "Total Charge by Chargable Weight":
                            case "Unit Price(RMB)":
                            case "Transportation Fee":
                                if (double.TryParse(row2[column].ToString(), out relde))
                                {
                                    newCell.SetCellValue(relde);//如果是数字类型则做转换,以使导出的Excel为数字类型的
                                    newCell.CellStyle = styleContentForDefault;
                                }
                                else
                                {
                                    newCell.SetCellValue(row2[column].ToString());
                                    newCell.CellStyle = styleContentForDefault;
                                }
                                break;
                            default:
                                newCell.SetCellValue(row2[column].ToString());
                                newCell.CellStyle = styleContentForDefault;
                                break;
                        }
                    }
                    rownum++;
                }

                //数据大的时候影响性能
                //int lastCellNum = row.LastCellNum;
                //for (int i = 0; i < lastCellNum; i++)
                //{
                //    sheet.AutoSizeColumn(i);
                //}
                if (File.Exists(strFileName))
                {
                    File.Delete(strFileName);
                }
                using (FileStream fs = new FileStream(strFileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    workbook.Write(fs); //写入到excel
                    fs.Close();
                    fs.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dtSource = null;
                sheet = null;
                workbook = null;
                GC.Collect();
            }
        }
        #endregion

        #region QueryCondition
        private static void QueryCondition(IWorkbook workbook, DataTable dtSource, string sheetname)
        {
            try
            {
                ISheet sheet = workbook.CreateSheet(sheetname);

                ICellStyle style3 = workbook.CreateCellStyle();
                //style3.BorderBottom = BorderStyle.Thin;
                //style3.BorderLeft = BorderStyle.Thin;
                //style3.BorderRight = BorderStyle.Thin;
                //style3.BorderTop = BorderStyle.Thin;
                //style3.FillForegroundColor = HSSFColor.LemonChiffon.Index;
                //style3.FillPattern = FillPattern.SolidForeground;
                IFont font3 = workbook.CreateFont();
                font3.FontName = "CorpoS";
                font3.FontHeight = 10;
                style3.SetFont(font3);

                int rownum = 0;
                IRow row = null;
                foreach (DataRow row2 in dtSource.Rows)
                {
                    IRow row3 = sheet.CreateRow(rownum);
                    row = row3;
                    for (int i = 0; i < dtSource.Columns.Count; i++)
                    {
                        DataColumn column = dtSource.Columns[i];
                        if (column.ColumnName == "SERIAL_NO")
                            continue;
                        ICell newCell = row3.CreateCell(i);
                        newCell.SetCellValue(row2[column].ToString());
                        newCell.CellStyle = style3;
                    }
                    rownum++;
                }
                if (row != null)
                {
                    int lastCellNum = row.LastCellNum;

                    for (int i = 0; i < lastCellNum; i++)
                    {
                        sheet.AutoSizeColumn(i);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dtSource = null;
                GC.Collect();
            }
        }
        #endregion

        #region AllDataFormExport
        private static void AllDataFormExport(IWorkbook workbook, DataTable dtSource, string sheetname)
        {
            try
            {
                ISheet sheet = workbook.CreateSheet(sheetname);
                #region Header
                ICellStyle styleHeader = GenerateHeadStyle(workbook);

                IRow row = sheet.CreateRow(0);
                for (int i = 0; i < dtSource.Columns.Count; i++)
                {
                    DataColumn column = dtSource.Columns[i];
                    ICell newCell = row.CreateCell(i);
                    newCell.SetCellValue(column.ColumnName);
                    newCell.CellStyle = styleHeader;
                }
                for (int i = 0; i < row.LastCellNum; i++)
                {
                    sheet.AutoSizeColumn(i);
                }
                #endregion
                ICellStyle styleContent = GenerateContentStyle(workbook);
                //用于区分转换为日期格式的列
                ICellStyle styleContentForDefault = GenerateContentStyle(workbook);
                int rownum = 1;
                foreach (DataRow row2 in dtSource.Rows)
                {
                    IRow row3 = sheet.CreateRow(rownum);
                    for (int i = 0; i < dtSource.Columns.Count; i++)
                    {
                        DataColumn column = dtSource.Columns[i];
                        ICell newCell = row3.CreateCell(i);
                        DateTime reldt;
                        double relde;
                        string cluName = column.ColumnName;
                        switch (cluName)
                        {
                            case "Operator Creation Time":
                            case "Supervisor Confirm Time":
                            case "TN Print Time":
                            case "Contract Arriving Time by Cargo Type (Earliest)":
                            case "Contract Arriving Time by Trans Mode(Earliest)":
                            case "Contract Arriving Time by Cargo Type (latest)":
                            case "Contract Arriving Time by Trans Mode (latest)":
                            case "Receiving Time":
                                if (DateTime.TryParse(row2[column].ToString(), out reldt))
                                {

                                    IDataFormat dataformat = workbook.CreateDataFormat();
                                    styleContent.DataFormat = dataformat.GetFormat("yyyy-MM-dd hh:mm:ss");
                                    newCell.SetCellValue(reldt);//如果是日期类型则做转换,以使导出的Excel为日期类型的
                                    newCell.CellStyle = styleContent;
                                }
                                else
                                {
                                    newCell.SetCellValue(row2[column].ToString());
                                    newCell.CellStyle = styleContentForDefault;
                                }
                                break;
                            case "Load Rate":
                            case "Case Qty":
                            case "Volume":
                            case "Gross Weight":
                            case "Total Chargable Weight":
                            case "Facing Distance":
                            case "Drop Off Fee":
                            case "Special Fee":
                            case "Discount Fee":
                            case "Total Charge":
                            case "Total Charge by Volume":
                            case "Total Charge by Chargable Weight":
                            case "Unit Price(RMB)":
                            case "Transportation Fee":
                                if (double.TryParse(row2[column].ToString(), out relde))
                                {
                                    newCell.SetCellValue(relde);//如果是数字类型则做转换,以使导出的Excel为数字类型的
                                    newCell.CellStyle = styleContentForDefault;
                                }
                                else
                                {
                                    newCell.SetCellValue(row2[column].ToString());
                                    newCell.CellStyle = styleContentForDefault;
                                }
                                break;
                            default:
                                newCell.SetCellValue(row2[column].ToString());
                                newCell.CellStyle = styleContentForDefault;
                                break;
                        }
                    }
                    rownum++;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dtSource = null;
                GC.Collect();
            }
        }
        #endregion

        #region Customer Performance Report
        #region CustomerPerformanceExport
        public static void CustomerPerformanceExport(DataSet dsSource, string strFileName)
        {
            IWorkbook workbook = null;
            if (strFileName.IndexOf(".xlsx") > 0) // 2007版本
                workbook = new XSSFWorkbook();
            else if (strFileName.IndexOf(".xls") > 0) // 2003版本
                workbook = new HSSFWorkbook();
            try
            {
                QueryCondition(workbook, dsSource.Tables[0], "Query condition");
                CustomerPerformance(workbook, dsSource.Tables[1], "Customer Performance Report");
                MonthlyShipmentCt(workbook, dsSource.Tables[2], "Monthly shipment Stat by CT");
                AllDataFormExport(workbook, dsSource.Tables[3], "All data form");
                AllDataFormExport(workbook, dsSource.Tables[4], "All data form(No calculation)");
                if (File.Exists(strFileName))
                {
                    File.Delete(strFileName);
                }
                using (FileStream fs = new FileStream(strFileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    workbook.Write(fs); //写入到excel
                    fs.Close();
                    fs.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dsSource = null;
                workbook = null;
                GC.Collect();
            }
        }
        #endregion

        #region CustomerPerformance
        private static void CustomerPerformance(IWorkbook workbook, DataTable dtSource, string sheetname)
        {
            try
            {
                ISheet sheet = workbook.CreateSheet(sheetname);
                sheet.DefaultRowHeightInPoints = 13.5F;
                int first = 3;
                #region Header
                ICellStyle styleHeader = workbook.CreateCellStyle();
                styleHeader.FillForegroundColor = HSSFColor.LemonChiffon.Index;
                styleHeader.FillPattern = FillPattern.SolidForeground;
                styleHeader.BorderBottom = BorderStyle.Thin;
                styleHeader.BorderLeft = BorderStyle.Thin;
                styleHeader.BorderRight = BorderStyle.Thin;
                styleHeader.BorderTop = BorderStyle.Thin;

                IFont fontHeader = workbook.CreateFont();
                fontHeader.FontName = "CorpoS";
                fontHeader.FontHeight = 10;
                styleHeader.SetFont(fontHeader);


                IRow row = sheet.CreateRow(first);
                int indexColumn = 9;
                for (int i = 0; i < dtSource.Columns.Count; i++)
                {
                    DataColumn column = dtSource.Columns[i];
                    if (column.ColumnName.Contains("-"))
                    {
                        string[] s = column.ColumnName.Split('-');
                        ICell newCell = row.CreateCell(indexColumn);
                        newCell.SetCellValue(Enum.GetName(typeof(MonthStr), Convert.ToInt32(s[1])));
                        newCell.CellStyle = styleHeader;
                        indexColumn++;
                    }
                }
                #endregion
                ICellStyle style1 = workbook.CreateCellStyle();
                style1.BorderBottom = BorderStyle.Thin;
                style1.BorderLeft = BorderStyle.Thin;
                style1.BorderRight = BorderStyle.Thin;
                style1.BorderTop = BorderStyle.Thin;
                style1.FillForegroundColor = HSSFColor.LemonChiffon.Index;
                style1.FillPattern = FillPattern.SolidForeground;
                style1.Alignment = HorizontalAlignment.Center;
                style1.VerticalAlignment = VerticalAlignment.Center;
                style1.WrapText = true;
                IFont font1 = workbook.CreateFont();
                font1.FontName = "CorpoS";
                font1.FontHeight = 12;
                font1.Boldweight = (short)FontBoldWeight.Bold;
                style1.SetFont(font1);

                ICellStyle style2 = workbook.CreateCellStyle();
                style2.BorderBottom = BorderStyle.Thin;
                style2.BorderLeft = BorderStyle.Thin;
                style2.BorderRight = BorderStyle.Thin;
                style2.BorderTop = BorderStyle.Thin;
                style2.FillForegroundColor = HSSFColor.LemonChiffon.Index;
                style2.FillPattern = FillPattern.SolidForeground;
                style2.Alignment = HorizontalAlignment.Center;
                style2.VerticalAlignment = VerticalAlignment.Center;
                style2.WrapText = true;
                IFont font2 = workbook.CreateFont();
                font2.FontName = "CorpoS";
                font2.FontHeight = 10;
                font2.Boldweight = (short)FontBoldWeight.Bold;
                style2.SetFont(font2);

                ICellStyle style3 = workbook.CreateCellStyle();
                style3.BorderBottom = BorderStyle.Thin;
                style3.BorderLeft = BorderStyle.Thin;
                style3.BorderRight = BorderStyle.Thin;
                style3.BorderTop = BorderStyle.Thin;
                style3.FillForegroundColor = HSSFColor.LemonChiffon.Index;
                style3.FillPattern = FillPattern.SolidForeground;
                IFont font3 = workbook.CreateFont();
                font3.FontName = "CorpoS";
                font3.FontHeight = 10;
                style3.SetFont(font3);


                ICellStyle style4 = workbook.CreateCellStyle();
                style4.BorderBottom = BorderStyle.Thin;
                style4.BorderLeft = BorderStyle.Thin;
                style4.BorderRight = BorderStyle.Thin;
                style4.BorderTop = BorderStyle.Thin;
                style4.FillForegroundColor = HSSFColor.LemonChiffon.Index;
                style4.FillPattern = FillPattern.SolidForeground;
                style4.Alignment = HorizontalAlignment.Center;
                style4.VerticalAlignment = VerticalAlignment.Center;
                style4.WrapText = true;
                IFont font4 = workbook.CreateFont();
                font4.FontName = "CorpoS";
                font4.FontHeight = 11;
                style4.SetFont(font4);

                ICellStyle styleRate = workbook.CreateCellStyle();
                styleRate.BorderBottom = BorderStyle.Thin;
                styleRate.BorderLeft = BorderStyle.Thin;
                styleRate.BorderRight = BorderStyle.Thin;
                styleRate.BorderTop = BorderStyle.Thin;
                styleRate.FillForegroundColor = HSSFColor.LemonChiffon.Index;
                styleRate.FillPattern = FillPattern.SolidForeground;
                styleRate.Alignment = HorizontalAlignment.Right;
                styleRate.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00%");
                IFont fontRate = workbook.CreateFont();
                fontRate.FontName = "CorpoS";
                fontRate.FontHeight = 10;
                styleRate.SetFont(fontRate);

                int rownum = first + 1;
                int mergedNum1 = first + 1;
                int mergedNum2 = first + 1;
                int count = dtSource.Rows.Count;
                string serialNo = "1";
                string sequenceNo = "1";
                for (int i = 0; i < count; i++)
                {
                    DataRow row2 = dtSource.Rows[i];

                    if (sequenceNo != row2["SEQUENCE_NUMBER"].ToString())
                    {
                        sequenceNo = row2["SEQUENCE_NUMBER"].ToString();
                        sheet.AddMergedRegion(new CellRangeAddress(mergedNum2, rownum - 1, 5, 5));
                        mergedNum2 = rownum;
                    }

                    if (serialNo != row2["SERIAL_NO"].ToString())
                    {
                        serialNo = row2["SERIAL_NO"].ToString();
                        for (int j = 1; j <= 4; j++)
                        {
                            sheet.AddMergedRegion(new CellRangeAddress(mergedNum1, rownum - 1, j, j));
                        }
                        rownum++;
                        mergedNum1 = rownum;
                        mergedNum2 = rownum;
                    }
                    IRow row3 = sheet.CreateRow(rownum);
                    ICell serialNoCell = row3.CreateCell(1);
                    serialNoCell.SetCellValue(Convert.ToDouble(row2["SERIAL_NO"]));
                    serialNoCell.CellStyle = style4;

                    ICell title = row3.CreateCell(2);
                    title.SetCellValue(row2["TITLE"].ToString());
                    title.CellStyle = style1;

                    ICell ict = row3.CreateCell(3);
                    ict.SetCellValue(row2["ICT_STR"].ToString());
                    ict.CellStyle = style2;

                    ICell orderType = row3.CreateCell(4);
                    orderType.SetCellValue(row2["ORDER_TYPE_STR"].ToString());
                    orderType.CellStyle = style2;

                    ICell kpiResult = row3.CreateCell(5);
                    kpiResult.SetCellValue(row2["KPI_RESULT_STR"].ToString());
                    kpiResult.CellStyle = style2;

                    ICell cityCode = row3.CreateCell(6);
                    cityCode.SetCellValue(row2["CITY_CODE"].ToString());
                    cityCode.CellStyle = style3;

                    ICell pdc = row3.CreateCell(7);
                    pdc.SetCellValue(row2["PDC"].ToString());
                    pdc.CellStyle = style3;

                    #region Rate
                    int indexColumnRate = 9;
                    int colCount = dtSource.Columns.Count;
                    for (int k = 0; k < dtSource.Columns.Count; k++)
                    {
                        DataColumn column = dtSource.Columns[k];
                        if (column.ColumnName.Contains("-"))
                        {
                            string[] s = column.ColumnName.Split('-');
                            ICell newCell = row3.CreateCell(indexColumnRate);
                            newCell.CellStyle = styleRate;
                            newCell.SetCellValue(Convert.ToDouble(row2[column.ColumnName]));
                            indexColumnRate++;
                        }
                    }
                    #endregion
                    if (i == count - 1)
                    {
                        for (int j = 1; j <= 4; j++)
                        {
                            sheet.AddMergedRegion(new CellRangeAddress(mergedNum1, rownum, j, j));
                        }
                        sheet.AddMergedRegion(new CellRangeAddress(mergedNum2, rownum, 5, 5));
                    }
                    rownum++;
                }
                sheet.SetColumnWidth(0, 3 * 256);
                sheet.SetColumnWidth(1, 3 * 256);
                sheet.SetColumnWidth(2, 13 * 256);
                sheet.SetColumnWidth(3, 10 * 256);
                sheet.SetColumnWidth(5, 11 * 256);
                sheet.SetColumnWidth(6, 11 * 256);
                sheet.SetColumnWidth(7, 11 * 256);
                sheet.SetColumnWidth(8, 3 * 256);

                int lastCellNum = row.LastCellNum;
                for (int i = 0; i < lastCellNum; i++)
                {
                    if (i >= 9)
                    {
                        sheet.SetColumnWidth(i, 10 * 256);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dtSource = null;
                GC.Collect();
            }
        }
        #endregion

        #region MonthlyShipmentCt
        private static void MonthlyShipmentCt(IWorkbook workbook, DataTable dtSource, string sheetname)
        {
            try
            {
                ISheet sheet = workbook.CreateSheet(sheetname);
                sheet.DefaultRowHeightInPoints = 13.5F;

                #region Header
                ICellStyle styleHeader = workbook.CreateCellStyle();
                styleHeader.FillForegroundColor = HSSFColor.LemonChiffon.Index;
                styleHeader.FillPattern = FillPattern.SolidForeground;
                styleHeader.Alignment = HorizontalAlignment.Center;
                styleHeader.VerticalAlignment = VerticalAlignment.Center;
                styleHeader.WrapText = true;//自动换行
                styleHeader.BorderBottom = BorderStyle.Thin;
                styleHeader.BorderLeft = BorderStyle.Thin;
                styleHeader.BorderRight = BorderStyle.Thin;
                styleHeader.BorderTop = BorderStyle.Thin;

                IFont fontHeader = workbook.CreateFont();
                fontHeader.FontName = "CorpoS";
                fontHeader.FontHeight = 11;
                styleHeader.SetFont(fontHeader);

                int first = 3;

                IRow row2 = sheet.CreateRow(first - 1);
                ICell cell_2_6 = row2.CreateCell(6);
                cell_2_6.SetCellValue("Calculation method by Normal / DG");
                cell_2_6.CellStyle = styleHeader;

                ICell cell_2_7 = row2.CreateCell(7);
                cell_2_7.SetCellValue("Calculation method by Normal / DG");
                cell_2_7.CellStyle = styleHeader;

                ICell cell_2_8 = row2.CreateCell(8);
                cell_2_8.SetCellValue("Calculation method by Normal / DG");
                cell_2_8.CellStyle = styleHeader;

                ICell cell_2_9 = row2.CreateCell(9);
                cell_2_9.SetCellValue("Calculation method by Normal / DG");
                cell_2_9.CellStyle = styleHeader;


                IRow row3 = sheet.CreateRow(first);
                row3.HeightInPoints = 54F;
                ICell cell_3_5 = row3.CreateCell(5);
                cell_3_5.SetCellValue("PDC");
                cell_3_5.CellStyle = styleHeader;

                ICell cell_3_6 = row3.CreateCell(6);
                cell_3_6.SetCellValue("Shipments as criteria ");
                cell_3_6.CellStyle = styleHeader;

                ICell cell_3_7 = row3.CreateCell(7);
                cell_3_7.SetCellValue("All shipments");
                cell_3_7.CellStyle = styleHeader;

                ICell cell_3_8 = row3.CreateCell(8);
                cell_3_8.SetCellValue("not-on time shipments");
                cell_3_8.CellStyle = styleHeader;

                ICell cell_3_9 = row3.CreateCell(9);
                cell_3_9.SetCellValue("Percentage");
                cell_3_9.CellStyle = styleHeader;
                #endregion
                sheet.AddMergedRegion(new CellRangeAddress(2, 2, 6, 9));

                ICellStyle style2 = workbook.CreateCellStyle();
                style2.BorderBottom = BorderStyle.Thin;
                style2.BorderLeft = BorderStyle.Thin;
                style2.BorderRight = BorderStyle.Thin;
                style2.BorderTop = BorderStyle.Thin;
                style2.FillForegroundColor = HSSFColor.LemonChiffon.Index;
                style2.FillPattern = FillPattern.SolidForeground;
                style2.Alignment = HorizontalAlignment.Center;
                style2.VerticalAlignment = VerticalAlignment.Center;
                style2.WrapText = true;
                IFont font2 = workbook.CreateFont();
                font2.FontName = "CorpoS";
                font2.FontHeight = 10;
                font2.Boldweight = (short)FontBoldWeight.Bold;
                style2.SetFont(font2);

                ICellStyle style3 = workbook.CreateCellStyle();
                style3.BorderBottom = BorderStyle.Thin;
                style3.BorderLeft = BorderStyle.Thin;
                style3.BorderRight = BorderStyle.Thin;
                style3.BorderTop = BorderStyle.Thin;
                style3.FillForegroundColor = HSSFColor.LemonChiffon.Index;
                style3.FillPattern = FillPattern.SolidForeground;
                IFont font3 = workbook.CreateFont();
                font3.FontName = "CorpoS";
                font3.FontHeight = 10;
                style3.SetFont(font3);


                ICellStyle style4 = workbook.CreateCellStyle();
                style4.BorderBottom = BorderStyle.Thin;
                style4.BorderLeft = BorderStyle.Thin;
                style4.BorderRight = BorderStyle.Thin;
                style4.BorderTop = BorderStyle.Thin;
                style4.FillForegroundColor = HSSFColor.LemonChiffon.Index;
                style4.FillPattern = FillPattern.SolidForeground;
                style4.Alignment = HorizontalAlignment.Center;
                style4.VerticalAlignment = VerticalAlignment.Center;
                style4.WrapText = true;
                IFont font4 = workbook.CreateFont();
                font4.FontName = "CorpoS";
                font4.FontHeight = 11;
                style4.SetFont(font4);

                ICellStyle styleRate = workbook.CreateCellStyle();
                styleRate.BorderBottom = BorderStyle.Thin;
                styleRate.BorderLeft = BorderStyle.Thin;
                styleRate.BorderRight = BorderStyle.Thin;
                styleRate.BorderTop = BorderStyle.Thin;
                styleRate.FillForegroundColor = HSSFColor.LemonChiffon.Index;
                styleRate.FillPattern = FillPattern.SolidForeground;
                styleRate.Alignment = HorizontalAlignment.Right;
                styleRate.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00%");
                IFont fontRate = workbook.CreateFont();
                fontRate.FontName = "CorpoS";
                fontRate.FontHeight = 10;
                styleRate.SetFont(fontRate);

                int rownum = first + 1;
                int mergedNum1 = first + 1;
                int mergedNum2 = first + 1;
                int count = dtSource.Rows.Count;
                string serialNo = "1";
                string sequenceNo = "1";
                for (int i = 0; i < count; i++)
                {
                    DataRow row = dtSource.Rows[i];

                    if (sequenceNo != row["SEQUENCE_NUMBER"].ToString())
                    {
                        sequenceNo = row["SEQUENCE_NUMBER"].ToString();
                        sheet.AddMergedRegion(new CellRangeAddress(mergedNum2, rownum - 1, 4, 4));
                        mergedNum2 = rownum;
                    }

                    if (serialNo != row["SERIAL_NO"].ToString())
                    {
                        serialNo = row["SERIAL_NO"].ToString();
                        for (int j = 1; j <= 3; j++)
                        {
                            sheet.AddMergedRegion(new CellRangeAddress(mergedNum1, rownum - 1, j, j));
                        }
                        rownum++;
                        mergedNum1 = rownum;
                        mergedNum2 = rownum;
                    }

                    IRow rowDet = sheet.CreateRow(rownum);
                    ICell serialNoCell = rowDet.CreateCell(1);
                    serialNoCell.SetCellValue(Convert.ToDouble(row["SERIAL_NO"]));
                    serialNoCell.CellStyle = style4;

                    ICell ict = rowDet.CreateCell(2);
                    ict.SetCellValue(row["ICT_STR"].ToString());
                    ict.CellStyle = style2;

                    ICell orderType = rowDet.CreateCell(3);
                    orderType.SetCellValue(row["ORDER_TYPE_STR"].ToString());
                    orderType.CellStyle = style2;

                    ICell kpiResult = rowDet.CreateCell(4);
                    kpiResult.SetCellValue(row["KPI_RESULT_STR"].ToString());
                    kpiResult.CellStyle = style2;

                    ICell pdc = rowDet.CreateCell(5);
                    pdc.SetCellValue(row["PDC"].ToString());
                    pdc.CellStyle = style3;

                    ICell criteriaShipments = rowDet.CreateCell(6);
                    criteriaShipments.SetCellValue(Convert.ToDouble(row["CRITERIA_SHIPMENTS"]));
                    criteriaShipments.CellStyle = style3;

                    ICell allShipments = rowDet.CreateCell(7);
                    allShipments.SetCellValue(Convert.ToDouble(row["ALL_SHIPMENTS"]));
                    allShipments.CellStyle = style3;

                    ICell notOntimeShipments = rowDet.CreateCell(8);
                    notOntimeShipments.SetCellValue(Convert.ToDouble(row["NOT_ONTIME_SHIPMENTS"]));
                    notOntimeShipments.CellStyle = style3;

                    ICell percentage = rowDet.CreateCell(9);
                    percentage.SetCellValue(Convert.ToDouble(row["PERCENTAGE"]));
                    percentage.CellStyle = styleRate;

                    if (i == count - 1)
                    {
                        for (int j = 1; j <= 3; j++)
                        {
                            sheet.AddMergedRegion(new CellRangeAddress(mergedNum1, rownum, j, j));
                        }
                        sheet.AddMergedRegion(new CellRangeAddress(mergedNum2, rownum, 4, 4));
                    }
                    rownum++;
                }
                sheet.SetColumnWidth(0, 3 * 256);
                sheet.SetColumnWidth(1, 3 * 256);
                sheet.SetColumnWidth(2, 13 * 256);
                sheet.SetColumnWidth(3, 10 * 256);
                sheet.SetColumnWidth(4, 11 * 256);
                sheet.SetColumnWidth(5, 11 * 256);
                sheet.SetColumnWidth(6, 11 * 256);
                sheet.SetColumnWidth(7, 11 * 256);
                sheet.SetColumnWidth(8, 11 * 256);
                sheet.SetColumnWidth(9, 11 * 256);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dtSource = null;
                GC.Collect();
            }
        }
        #endregion
        #endregion

        #region Forwarder Performance Report
        #region ForwarderPerformanceExport
        public static void ForwarderPerformanceExport(DataSet dsSource, string strFileName)
        {
            IWorkbook workbook = null;
            if (strFileName.IndexOf(".xlsx") > 0) // 2007版本
                workbook = new XSSFWorkbook();
            else if (strFileName.IndexOf(".xls") > 0) // 2003版本
                workbook = new HSSFWorkbook();
            try
            {
                QueryCondition(workbook, dsSource.Tables[0], "Query condition");
                ForwarderPerformance(workbook, dsSource.Tables[1], "Forwarder Performance Report");
                MonthlyShipmentTm(workbook, dsSource.Tables[2], "Monthly shipment Stat by TM");
                AllDataFormExport(workbook, dsSource.Tables[3], "All data form");
                AllDataFormExport(workbook, dsSource.Tables[4], "All data form(No calculation)");
                if (File.Exists(strFileName))
                {
                    File.Delete(strFileName);
                }
                using (FileStream fs = new FileStream(strFileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    workbook.Write(fs); //写入到excel
                    fs.Close();
                    fs.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dsSource = null;
                workbook = null;
                GC.Collect();
            }
        }
        #endregion

        #region ForwarderPerformance
        private static void ForwarderPerformance(IWorkbook workbook, DataTable dtSource, string sheetname)
        {
            try
            {
                ISheet sheet = workbook.CreateSheet(sheetname);
                sheet.DefaultRowHeightInPoints = 13.5F;
                #region Header
                ICellStyle styleHeader = workbook.CreateCellStyle();
                styleHeader.FillForegroundColor = HSSFColor.LemonChiffon.Index;
                styleHeader.FillPattern = FillPattern.SolidForeground;
                styleHeader.BorderBottom = BorderStyle.Thin;
                styleHeader.BorderLeft = BorderStyle.Thin;
                styleHeader.BorderRight = BorderStyle.Thin;
                styleHeader.BorderTop = BorderStyle.Thin;

                IFont fontHeader = workbook.CreateFont();
                fontHeader.FontName = "CorpoS";
                fontHeader.FontHeight = 10;
                styleHeader.SetFont(fontHeader);

                int first = 0;
                IRow row = sheet.CreateRow(first);
                ICell pdcCell = row.CreateCell(5);
                pdcCell.SetCellValue("PDC");
                pdcCell.CellStyle = styleHeader;

                int indexColumn = 6;
                for (int i = 0; i < dtSource.Columns.Count; i++)
                {
                    DataColumn column = dtSource.Columns[i];
                    if (column.ColumnName.Contains("-"))
                    {
                        string[] s = column.ColumnName.Split('-');
                        ICell newCell = row.CreateCell(indexColumn);
                        newCell.SetCellValue(Enum.GetName(typeof(MonthStr), Convert.ToInt32(s[1])));
                        newCell.CellStyle = styleHeader;
                        indexColumn++;
                    }
                }
                #endregion

                ICellStyle style1 = workbook.CreateCellStyle();
                style1.BorderBottom = BorderStyle.Thin;
                style1.BorderLeft = BorderStyle.Thin;
                style1.BorderRight = BorderStyle.Thin;
                style1.BorderTop = BorderStyle.Thin;
                style1.FillForegroundColor = HSSFColor.LemonChiffon.Index;
                style1.FillPattern = FillPattern.SolidForeground;
                style1.Alignment = HorizontalAlignment.Center;
                style1.VerticalAlignment = VerticalAlignment.Center;
                style1.WrapText = true;
                IFont font1 = workbook.CreateFont();
                font1.FontName = "CorpoS";
                font1.FontHeight = 12;
                font1.Boldweight = (short)FontBoldWeight.Bold;
                style1.SetFont(font1);

                ICellStyle style2 = workbook.CreateCellStyle();
                style2.BorderBottom = BorderStyle.Thin;
                style2.BorderLeft = BorderStyle.Thin;
                style2.BorderRight = BorderStyle.Thin;
                style2.BorderTop = BorderStyle.Thin;
                style2.FillForegroundColor = HSSFColor.LemonChiffon.Index;
                style2.FillPattern = FillPattern.SolidForeground;
                style2.Alignment = HorizontalAlignment.Center;
                style2.VerticalAlignment = VerticalAlignment.Center;
                style2.WrapText = true;
                IFont font2 = workbook.CreateFont();
                font2.FontName = "CorpoS";
                font2.FontHeight = 10;
                font2.Boldweight = (short)FontBoldWeight.Bold;
                style2.SetFont(font2);

                ICellStyle style3 = workbook.CreateCellStyle();
                style3.BorderBottom = BorderStyle.Thin;
                style3.BorderLeft = BorderStyle.Thin;
                style3.BorderRight = BorderStyle.Thin;
                style3.BorderTop = BorderStyle.Thin;
                style3.FillForegroundColor = HSSFColor.LemonChiffon.Index;
                style3.FillPattern = FillPattern.SolidForeground;
                IFont font3 = workbook.CreateFont();
                font3.FontName = "CorpoS";
                font3.FontHeight = 10;
                style3.SetFont(font3);


                ICellStyle style4 = workbook.CreateCellStyle();
                style4.BorderBottom = BorderStyle.Thin;
                style4.BorderLeft = BorderStyle.Thin;
                style4.BorderRight = BorderStyle.Thin;
                style4.BorderTop = BorderStyle.Thin;
                style4.FillForegroundColor = HSSFColor.LemonChiffon.Index;
                style4.FillPattern = FillPattern.SolidForeground;
                style4.Alignment = HorizontalAlignment.Center;
                style4.VerticalAlignment = VerticalAlignment.Center;
                style4.WrapText = true;
                IFont font4 = workbook.CreateFont();
                font4.FontName = "CorpoS";
                font4.FontHeight = 11;
                style4.SetFont(font4);

                ICellStyle styleRate = workbook.CreateCellStyle();
                styleRate.BorderBottom = BorderStyle.Thin;
                styleRate.BorderLeft = BorderStyle.Thin;
                styleRate.BorderRight = BorderStyle.Thin;
                styleRate.BorderTop = BorderStyle.Thin;
                styleRate.FillForegroundColor = HSSFColor.LemonChiffon.Index;
                styleRate.FillPattern = FillPattern.SolidForeground;
                styleRate.Alignment = HorizontalAlignment.Right;
                styleRate.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00%");
                IFont fontRate = workbook.CreateFont();
                fontRate.FontName = "CorpoS";
                fontRate.FontHeight = 10;
                styleRate.SetFont(fontRate);

                int rownum = first + 1;
                int mergedNum1 = first + 1;
                int mergedNum2 = first + 1;
                int count = dtSource.Rows.Count;
                string serialNo = "1";
                string sequenceNo = "1";
                for (int i = 0; i < count; i++)
                {
                    DataRow row2 = dtSource.Rows[i];

                    if (sequenceNo != row2["SEQUENCE_NUMBER"].ToString())
                    {
                        sequenceNo = row2["SEQUENCE_NUMBER"].ToString();
                        sheet.AddMergedRegion(new CellRangeAddress(mergedNum2, rownum - 1, 4, 4));
                        mergedNum2 = rownum;
                    }

                    if (serialNo != row2["SERIAL_NO"].ToString())
                    {
                        serialNo = row2["SERIAL_NO"].ToString();
                        for (int j = 1; j <= 3; j++)
                        {
                            sheet.AddMergedRegion(new CellRangeAddress(mergedNum1, rownum - 1, j, j));
                        }
                        rownum++;
                        mergedNum1 = rownum;
                        mergedNum2 = rownum;
                    }
                    IRow row3 = sheet.CreateRow(rownum);
                    ICell serialNoCell = row3.CreateCell(1);
                    serialNoCell.SetCellValue(Convert.ToDouble(row2["SERIAL_NO"]));
                    serialNoCell.CellStyle = style4;

                    ICell ict = row3.CreateCell(2);
                    if (row2["TRANS_MODE"].ToString() != "")
                    {
                        ict.SetCellValue(row2["TRANS_MODE"].ToString());
                    }
                    else
                    {
                        ict.SetCellValue(row2["ICT_STR"].ToString());
                    }
                    ict.CellStyle = style2;

                    ICell orderType = row3.CreateCell(3);
                    orderType.SetCellValue(row2["ORDER_TYPE_STR"].ToString());
                    orderType.CellStyle = style2;

                    ICell kpiResult = row3.CreateCell(4);
                    kpiResult.SetCellValue(row2["KPI_RESULT_STR"].ToString());
                    kpiResult.CellStyle = style2;

                    ICell cityCode = row3.CreateCell(5);
                    cityCode.SetCellValue(row2["PDC"].ToString());
                    cityCode.CellStyle = style3;

                    #region Rate
                    int indexColumnRate = 6;
                    int colCount = dtSource.Columns.Count;
                    for (int k = 0; k < colCount; k++)
                    {
                        DataColumn column = dtSource.Columns[k];
                        if (column.ColumnName.Contains("-"))
                        {
                            string[] s = column.ColumnName.Split('-');
                            ICell newCell = row3.CreateCell(indexColumnRate);
                            newCell.CellStyle = styleRate;
                            newCell.SetCellValue(Convert.ToDouble(row2[column.ColumnName]));
                            indexColumnRate++;
                        }
                    }
                    #endregion
                    if (i == count - 1)
                    {
                        for (int j = 1; j <= 3; j++)
                        {
                            sheet.AddMergedRegion(new CellRangeAddress(mergedNum1, rownum, j, j));
                        }
                        sheet.AddMergedRegion(new CellRangeAddress(mergedNum2, rownum, 4, 4));
                    }
                    rownum++;
                }
                sheet.SetColumnWidth(0, 3 * 256);
                sheet.SetColumnWidth(1, 3 * 256);
                sheet.SetColumnWidth(2, 13 * 256);
                sheet.SetColumnWidth(3, 10 * 256);
                sheet.SetColumnWidth(4, 11 * 256);
                sheet.SetColumnWidth(5, 11 * 256);
                int lastCellNum = row.LastCellNum;
                for (int i = 0; i < lastCellNum; i++)
                {
                    if (i >= 6)
                    {
                        sheet.SetColumnWidth(i, 10 * 256);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dtSource = null;
                GC.Collect();
            }
        }
        #endregion

        #region MonthlyShipmentTm
        private static void MonthlyShipmentTm(IWorkbook workbook, DataTable dtSource, string sheetname)
        {
            try
            {
                ISheet sheet = workbook.CreateSheet(sheetname);
                sheet.DefaultRowHeightInPoints = 13.5F;

                #region Header
                ICellStyle styleHeader = workbook.CreateCellStyle();
                styleHeader.FillForegroundColor = HSSFColor.LemonChiffon.Index;
                styleHeader.FillPattern = FillPattern.SolidForeground;
                styleHeader.Alignment = HorizontalAlignment.Center;
                styleHeader.VerticalAlignment = VerticalAlignment.Center;
                styleHeader.WrapText = true;//自动换行
                styleHeader.BorderBottom = BorderStyle.Thin;
                styleHeader.BorderLeft = BorderStyle.Thin;
                styleHeader.BorderRight = BorderStyle.Thin;
                styleHeader.BorderTop = BorderStyle.Thin;

                IFont fontHeader = workbook.CreateFont();
                fontHeader.FontName = "CorpoS";
                fontHeader.FontHeight = 11;
                styleHeader.SetFont(fontHeader);

                int first = 3;
                IRow row2 = sheet.CreateRow(first - 1);
                ICell cell_2_6 = row2.CreateCell(6);
                cell_2_6.SetCellValue("Calculation method by Transportation Mode");
                cell_2_6.CellStyle = styleHeader;

                ICell cell_2_7 = row2.CreateCell(7);
                cell_2_7.SetCellValue("Calculation method by Transportation Mode");
                cell_2_7.CellStyle = styleHeader;

                ICell cell_2_8 = row2.CreateCell(8);
                cell_2_8.SetCellValue("Calculation method by Transportation Mode");
                cell_2_8.CellStyle = styleHeader;

                ICell cell_2_9 = row2.CreateCell(9);
                cell_2_9.SetCellValue("Calculation method by Transportation Mode");
                cell_2_9.CellStyle = styleHeader;


                IRow row3 = sheet.CreateRow(first);
                row3.HeightInPoints = 54F;
                ICell cell_3_5 = row3.CreateCell(5);
                cell_3_5.SetCellValue("PDC");
                cell_3_5.CellStyle = styleHeader;

                ICell cell_3_6 = row3.CreateCell(6);
                cell_3_6.SetCellValue("Shipments as criteria ");
                cell_3_6.CellStyle = styleHeader;

                ICell cell_3_7 = row3.CreateCell(7);
                cell_3_7.SetCellValue("All shipments");
                cell_3_7.CellStyle = styleHeader;

                ICell cell_3_8 = row3.CreateCell(8);
                cell_3_8.SetCellValue("not-on time shipments");
                cell_3_8.CellStyle = styleHeader;

                ICell cell_3_9 = row3.CreateCell(9);
                cell_3_9.SetCellValue("Percentage");
                cell_3_9.CellStyle = styleHeader;
                #endregion
                sheet.AddMergedRegion(new CellRangeAddress(2, 2, 6, 9));

                ICellStyle style2 = workbook.CreateCellStyle();
                style2.BorderBottom = BorderStyle.Thin;
                style2.BorderLeft = BorderStyle.Thin;
                style2.BorderRight = BorderStyle.Thin;
                style2.BorderTop = BorderStyle.Thin;
                style2.FillForegroundColor = HSSFColor.LemonChiffon.Index;
                style2.FillPattern = FillPattern.SolidForeground;
                style2.Alignment = HorizontalAlignment.Center;
                style2.VerticalAlignment = VerticalAlignment.Center;
                style2.WrapText = true;
                IFont font2 = workbook.CreateFont();
                font2.FontName = "CorpoS";
                font2.FontHeight = 10;
                font2.Boldweight = (short)FontBoldWeight.Bold;
                style2.SetFont(font2);

                ICellStyle style3 = workbook.CreateCellStyle();
                style3.BorderBottom = BorderStyle.Thin;
                style3.BorderLeft = BorderStyle.Thin;
                style3.BorderRight = BorderStyle.Thin;
                style3.BorderTop = BorderStyle.Thin;
                style3.FillForegroundColor = HSSFColor.LemonChiffon.Index;
                style3.FillPattern = FillPattern.SolidForeground;
                IFont font3 = workbook.CreateFont();
                font3.FontName = "CorpoS";
                font3.FontHeight = 10;
                style3.SetFont(font3);

                ICellStyle style4 = workbook.CreateCellStyle();
                style4.BorderBottom = BorderStyle.Thin;
                style4.BorderLeft = BorderStyle.Thin;
                style4.BorderRight = BorderStyle.Thin;
                style4.BorderTop = BorderStyle.Thin;
                style4.FillForegroundColor = HSSFColor.LemonChiffon.Index;
                style4.FillPattern = FillPattern.SolidForeground;
                style4.Alignment = HorizontalAlignment.Center;
                style4.VerticalAlignment = VerticalAlignment.Center;
                style4.WrapText = true;
                IFont font4 = workbook.CreateFont();
                font4.FontName = "CorpoS";
                font4.FontHeight = 11;
                style4.SetFont(font4);

                ICellStyle styleRate = workbook.CreateCellStyle();
                styleRate.BorderBottom = BorderStyle.Thin;
                styleRate.BorderLeft = BorderStyle.Thin;
                styleRate.BorderRight = BorderStyle.Thin;
                styleRate.BorderTop = BorderStyle.Thin;
                styleRate.FillForegroundColor = HSSFColor.LemonChiffon.Index;
                styleRate.FillPattern = FillPattern.SolidForeground;
                styleRate.Alignment = HorizontalAlignment.Right;
                styleRate.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00%");
                IFont fontRate = workbook.CreateFont();
                fontRate.FontName = "CorpoS";
                fontRate.FontHeight = 10;
                styleRate.SetFont(fontRate);

                int rownum = first + 1;
                int mergedNum1 = first + 1;
                int mergedNum2 = first + 1;
                int count = dtSource.Rows.Count;
                string serialNo = "1";
                string sequenceNo = "1";
                for (int i = 0; i < count; i++)
                {
                    DataRow row = dtSource.Rows[i];

                    if (sequenceNo != row["SEQUENCE_NUMBER"].ToString())
                    {
                        sequenceNo = row["SEQUENCE_NUMBER"].ToString();
                        sheet.AddMergedRegion(new CellRangeAddress(mergedNum2, rownum - 1, 4, 4));
                        mergedNum2 = rownum;
                    }

                    if (serialNo != row["SERIAL_NO"].ToString())
                    {
                        serialNo = row["SERIAL_NO"].ToString();
                        for (int j = 1; j <= 3; j++)
                        {
                            sheet.AddMergedRegion(new CellRangeAddress(mergedNum1, rownum - 1, j, j));
                        }
                        rownum++;
                        mergedNum1 = rownum;
                        mergedNum2 = rownum;
                    }
                    IRow rowDet = sheet.CreateRow(rownum);
                    ICell serialNoCell = rowDet.CreateCell(1);
                    serialNoCell.SetCellValue(Convert.ToDouble(row["SERIAL_NO"]));
                    serialNoCell.CellStyle = style4;

                    ICell ict = rowDet.CreateCell(2);
                    if (row["TRANS_MODE"].ToString() != "")
                    {
                        ict.SetCellValue(row["TRANS_MODE"].ToString());
                    }
                    else
                    {
                        ict.SetCellValue(row["ICT_STR"].ToString());
                    }
                    ict.CellStyle = style2;

                    ICell orderType = rowDet.CreateCell(3);
                    orderType.SetCellValue(row["ORDER_TYPE_STR"].ToString());
                    orderType.CellStyle = style2;

                    ICell kpiResult = rowDet.CreateCell(4);
                    kpiResult.SetCellValue(row["KPI_RESULT_STR"].ToString());
                    kpiResult.CellStyle = style2;

                    ICell pdc = rowDet.CreateCell(5);
                    pdc.SetCellValue(row["PDC"].ToString());
                    pdc.CellStyle = style3;

                    ICell criteriaShipments = rowDet.CreateCell(6);
                    criteriaShipments.SetCellValue(Convert.ToDouble(row["CRITERIA_SHIPMENTS"]));
                    criteriaShipments.CellStyle = style3;

                    ICell allShipments = rowDet.CreateCell(7);
                    allShipments.SetCellValue(Convert.ToDouble(row["ALL_SHIPMENTS"]));
                    allShipments.CellStyle = style3;

                    ICell notOntimeShipments = rowDet.CreateCell(8);
                    notOntimeShipments.SetCellValue(Convert.ToDouble(row["NOT_ONTIME_SHIPMENTS"]));
                    notOntimeShipments.CellStyle = style3;

                    ICell percentage = rowDet.CreateCell(9);
                    percentage.SetCellValue(Convert.ToDouble(row["PERCENTAGE"]));
                    percentage.CellStyle = styleRate;
                    if (i == count - 1)
                    {
                        for (int j = 1; j <= 3; j++)
                        {
                            sheet.AddMergedRegion(new CellRangeAddress(mergedNum1, rownum, j, j));
                        }
                        sheet.AddMergedRegion(new CellRangeAddress(mergedNum2, rownum, 4, 4));
                    }
                    rownum++;
                }
                sheet.SetColumnWidth(0, 3 * 256);
                sheet.SetColumnWidth(1, 3 * 256);
                sheet.SetColumnWidth(2, 13 * 256);
                sheet.SetColumnWidth(3, 10 * 256);
                sheet.SetColumnWidth(4, 11 * 256);
                sheet.SetColumnWidth(5, 11 * 256);
                sheet.SetColumnWidth(6, 11 * 256);
                sheet.SetColumnWidth(7, 11 * 256);
                sheet.SetColumnWidth(8, 11 * 256);
                sheet.SetColumnWidth(9, 11 * 256);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dtSource = null;
                GC.Collect();
            }
        }
        #endregion

        #endregion

        #region Montly KPI by Dealer
        #region MonthlyKpiByDealerExport
        public static void MonthlyKpiByDealerExport(DataSet dsSource, string strFileName, string type)
        {
            IWorkbook workbook = null;
            if (strFileName.IndexOf(".xlsx") > 0) // 2007版本
                workbook = new XSSFWorkbook();
            else if (strFileName.IndexOf(".xls") > 0) // 2003版本
                workbook = new HSSFWorkbook();
            try
            {
                QueryCondition(workbook, dsSource.Tables[0], "Query condition");
                MonthlyKpiByDealer(workbook, dsSource.Tables[1], type);
                if (File.Exists(strFileName))
                {
                    File.Delete(strFileName);
                }
                using (FileStream fs = new FileStream(strFileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    workbook.Write(fs); //写入到excel
                    fs.Close();
                    fs.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dsSource = null;
                workbook = null;
                GC.Collect();
            }
        }
        #endregion

        #region MonthlyKpiByDealer
        private static void MonthlyKpiByDealer(IWorkbook workbook, DataTable dtSource, string type)
        {
            try
            {
                string sheetname = "";
                string titleStr = "";
                if (type == "TM")
                {
                    sheetname = "Montly KPI by Dealer by TM";
                    titleStr = "Calculation method by Transportation Mode";
                }
                else
                {
                    sheetname = "Montly KPI by Dealer by CT";
                    titleStr = "Calculation method by Normal / DG";
                }
                ISheet sheet = workbook.CreateSheet(sheetname);
                sheet.DefaultRowHeightInPoints = 13.5F;

                #region Header
                ICellStyle styleHeader = workbook.CreateCellStyle();
                styleHeader.FillForegroundColor = HSSFColor.LemonChiffon.Index;
                styleHeader.FillPattern = FillPattern.SolidForeground;
                styleHeader.Alignment = HorizontalAlignment.Center;
                styleHeader.VerticalAlignment = VerticalAlignment.Center;
                styleHeader.WrapText = true;//自动换行
                styleHeader.BorderBottom = styleHeader.BorderTop = styleHeader.BorderRight = styleHeader.BorderLeft = styleHeader.BorderBottom = BorderStyle.Thin;

                IFont fontHeader = workbook.CreateFont();
                fontHeader.FontName = "CorpoS";
                fontHeader.FontHeight = 11;
                styleHeader.SetFont(fontHeader);

                int first = 3;
                IRow row2 = sheet.CreateRow(first - 1);
                ICell cell_2_6 = row2.CreateCell(6);
                cell_2_6.SetCellValue(titleStr);
                cell_2_6.CellStyle = styleHeader;

                ICell cell_2_7 = row2.CreateCell(7);
                cell_2_7.SetCellValue(titleStr);
                cell_2_7.CellStyle = styleHeader;

                ICell cell_2_8 = row2.CreateCell(8);
                cell_2_8.SetCellValue(titleStr);
                cell_2_8.CellStyle = styleHeader;

                ICell cell_2_9 = row2.CreateCell(9);
                cell_2_9.SetCellValue(titleStr);
                cell_2_9.CellStyle = styleHeader;


                IRow row3 = sheet.CreateRow(first);
                row3.HeightInPoints = 54F;
                ICell cell_3_5 = row3.CreateCell(5);
                cell_3_5.SetCellValue("Dealer");
                cell_3_5.CellStyle = styleHeader;

                ICell cell_3_6 = row3.CreateCell(6);
                cell_3_6.SetCellValue("Shipments as criteria ");
                cell_3_6.CellStyle = styleHeader;

                ICell cell_3_7 = row3.CreateCell(7);
                cell_3_7.SetCellValue("All shipments");
                cell_3_7.CellStyle = styleHeader;

                ICell cell_3_8 = row3.CreateCell(8);
                cell_3_8.SetCellValue("not-on time shipments");
                cell_3_8.CellStyle = styleHeader;

                ICell cell_3_9 = row3.CreateCell(9);
                cell_3_9.SetCellValue("Percentage");
                cell_3_9.CellStyle = styleHeader;
                #endregion
                sheet.AddMergedRegion(new CellRangeAddress(2, 2, 6, 9));

                ICellStyle style2 = workbook.CreateCellStyle();
                style2.BorderBottom = BorderStyle.Thin;
                style2.BorderLeft = BorderStyle.Thin;
                style2.BorderRight = BorderStyle.Thin;
                style2.BorderTop = BorderStyle.Thin;
                style2.FillForegroundColor = HSSFColor.LemonChiffon.Index;
                style2.FillPattern = FillPattern.SolidForeground;
                style2.Alignment = HorizontalAlignment.Center;
                style2.VerticalAlignment = VerticalAlignment.Center;
                style2.WrapText = true;
                IFont font2 = workbook.CreateFont();
                font2.FontName = "CorpoS";
                font2.FontHeight = 10;
                font2.Boldweight = (short)FontBoldWeight.Bold;
                style2.SetFont(font2);

                ICellStyle style3 = workbook.CreateCellStyle();
                style3.BorderBottom = BorderStyle.Thin;
                style3.BorderLeft = BorderStyle.Thin;
                style3.BorderRight = BorderStyle.Thin;
                style3.BorderTop = BorderStyle.Thin;
                style3.FillForegroundColor = HSSFColor.LemonChiffon.Index;
                style3.FillPattern = FillPattern.SolidForeground;
                IFont font3 = workbook.CreateFont();
                font3.FontName = "CorpoS";
                font3.FontHeight = 10;
                style3.SetFont(font3);


                ICellStyle style4 = workbook.CreateCellStyle();
                style4.BorderBottom = BorderStyle.Thin;
                style4.BorderLeft = BorderStyle.Thin;
                style4.BorderRight = BorderStyle.Thin;
                style4.BorderTop = BorderStyle.Thin;
                style4.FillForegroundColor = HSSFColor.LemonChiffon.Index;
                style4.FillPattern = FillPattern.SolidForeground;
                style4.Alignment = HorizontalAlignment.Center;
                style4.VerticalAlignment = VerticalAlignment.Center;
                style4.WrapText = true;
                IFont font4 = workbook.CreateFont();
                font4.FontName = "CorpoS";
                font4.FontHeight = 11;
                style4.SetFont(font4);

                ICellStyle styleRate = workbook.CreateCellStyle();
                styleRate.BorderBottom = BorderStyle.Thin;
                styleRate.BorderLeft = BorderStyle.Thin;
                styleRate.BorderRight = BorderStyle.Thin;
                styleRate.BorderTop = BorderStyle.Thin;
                styleRate.FillForegroundColor = HSSFColor.LemonChiffon.Index;
                styleRate.FillPattern = FillPattern.SolidForeground;
                styleRate.Alignment = HorizontalAlignment.Right;
                styleRate.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00%");
                IFont fontRate = workbook.CreateFont();
                fontRate.FontName = "CorpoS";
                fontRate.FontHeight = 10;
                styleRate.SetFont(fontRate);

                int rownum = first + 1;
                int mergedNum1 = first + 1;
                int mergedNum2 = first + 1;
                int count = dtSource.Rows.Count;
                string pdcSequance = "1";
                string sequenceNo = "1";
                for (int i = 0; i < count; i++)
                {
                    DataRow row = dtSource.Rows[i];

                    if (sequenceNo != row["SEQUENCE_NUMBER"].ToString() || pdcSequance != row["PDC_SEQUANCE"].ToString())
                    {
                        sequenceNo = row["SEQUENCE_NUMBER"].ToString();
                        sheet.AddMergedRegion(new CellRangeAddress(mergedNum2, rownum - 1, 4, 4));
                        mergedNum2 = rownum;
                    }

                    if (pdcSequance != row["PDC_SEQUANCE"].ToString())
                    {
                        pdcSequance = row["PDC_SEQUANCE"].ToString();
                        for (int j = 1; j <= 3; j++)
                        {
                            sheet.AddMergedRegion(new CellRangeAddress(mergedNum1, rownum - 1, j, j));
                        }
                        rownum++;
                        mergedNum1 = rownum;
                        mergedNum2 = rownum;
                    }
                    IRow rowDet = sheet.CreateRow(rownum);
                    ICell pdcSequanceCell = rowDet.CreateCell(1);
                    pdcSequanceCell.SetCellValue(Convert.ToDouble(row["PDC_SEQUANCE"]));
                    pdcSequanceCell.CellStyle = style4;

                    ICell title = rowDet.CreateCell(2);
                    title.SetCellValue(row["TITLE"].ToString());
                    title.CellStyle = style2;

                    ICell orderType = rowDet.CreateCell(3);
                    orderType.SetCellValue(row["ORDER_TYPE_STR"].ToString());
                    orderType.CellStyle = style2;

                    ICell kpiResult = rowDet.CreateCell(4);
                    kpiResult.SetCellValue(row["KPI_RESULT_STR"].ToString());
                    kpiResult.CellStyle = style2;

                    ICell dealerCode = rowDet.CreateCell(5);
                    dealerCode.SetCellValue(row["DEALER_CODE"].ToString());
                    dealerCode.CellStyle = style3;

                    ICell criteriaShipments = rowDet.CreateCell(6);
                    criteriaShipments.SetCellValue(Convert.ToDouble(row["CRITERIA_SHIPMENTS"]));
                    criteriaShipments.CellStyle = style3;

                    ICell allShipments = rowDet.CreateCell(7);
                    allShipments.SetCellValue(Convert.ToDouble(row["ALL_SHIPMENTS"]));
                    allShipments.CellStyle = style3;

                    ICell notOntimeShipments = rowDet.CreateCell(8);
                    notOntimeShipments.SetCellValue(Convert.ToDouble(row["NOT_ONTIME_SHIPMENTS"]));
                    notOntimeShipments.CellStyle = style3;

                    ICell percentage = rowDet.CreateCell(9);
                    percentage.SetCellValue(Convert.ToDouble(row["PERCENTAGE"]));
                    percentage.CellStyle = styleRate;
                    if (i == count - 1)
                    {
                        for (int j = 1; j <= 3; j++)
                        {
                            sheet.AddMergedRegion(new CellRangeAddress(mergedNum1, rownum, j, j));
                        }
                        sheet.AddMergedRegion(new CellRangeAddress(mergedNum2, rownum, 4, 4));
                    }
                    rownum++;
                }
                sheet.SetColumnWidth(0, 3 * 256);
                sheet.SetColumnWidth(1, 3 * 256);
                sheet.SetColumnWidth(2, 13 * 256);
                sheet.SetColumnWidth(3, 10 * 256);
                sheet.SetColumnWidth(4, 11 * 256);
                sheet.SetColumnWidth(5, 11 * 256);
                sheet.SetColumnWidth(6, 11 * 256);
                sheet.SetColumnWidth(7, 11 * 256);
                sheet.SetColumnWidth(8, 11 * 256);
                sheet.SetColumnWidth(9, 11 * 256);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dtSource = null;
                GC.Collect();
            }
        }
        #endregion

        #endregion

        #region DateDiff
        public enum DateInterval
        {
            Second, Minute, Hour, Day, Week, Month, Quarter, Year
        }

        public static long DateDiff(DateInterval Interval, System.DateTime StartDate, System.DateTime EndDate)
        {
            long lngDateDiffValue = 0;
            System.TimeSpan TS = new System.TimeSpan(EndDate.Ticks - StartDate.Ticks);
            switch (Interval)
            {
                case DateInterval.Second:
                    lngDateDiffValue = (long)TS.TotalSeconds;
                    break;
                case DateInterval.Minute:
                    lngDateDiffValue = (long)TS.TotalMinutes;
                    break;
                case DateInterval.Hour:
                    lngDateDiffValue = (long)TS.TotalHours;
                    break;
                case DateInterval.Day:
                    lngDateDiffValue = (long)TS.Days;
                    break;
                case DateInterval.Week:
                    lngDateDiffValue = (long)(TS.Days / 7);
                    break;
                case DateInterval.Month:
                    lngDateDiffValue = (long)(TS.Days / 30);
                    break;
                case DateInterval.Quarter:
                    lngDateDiffValue = (long)((TS.Days / 30) / 3);
                    break;
                case DateInterval.Year:
                    lngDateDiffValue = (long)(TS.Days / 365);
                    break;
            }
            return (lngDateDiffValue);
        }
        #endregion

        #region XLine
        /// <summary>
        /// 循环父节点
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<object> AddTopCategory(DataTable dt) 
        {
            var nodes = new List<object>();

            foreach (DataRow dr in dt.Rows)
            {
                //如果为空就是父节点
                if (string.IsNullOrEmpty(dr["xline"].ToString()))
                {
                    TreeDescriptor tree = new TreeDescriptor();
                    tree.Id = dr["treeID"].ToString();
                    tree.Text = dr["treeID"].ToString();
                    //如果不为空就是子节点
                    tree.Children = AddChildrenCategory(dr["treeID"].ToString(), dt);
                    nodes.Add(new { id = tree.Id, text = tree.Text, children = tree.Children });
                }
            }
            return nodes;
        }

        /// <summary>
        /// 循环子节点
        /// </summary>
        /// <param name="treeid"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static List<object> AddChildrenCategory(string treeid, DataTable dt)
        {
            var nodes = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                if (string.IsNullOrEmpty(dr["xline"].ToString()) || dr["treeID"].ToString() != treeid)
                    continue;
                TreeDescriptor tree = new TreeDescriptor();
                tree.Id = dr["treeID"].ToString() + "-" + dr["xline"].ToString();
                tree.Text = dr["xline"].ToString();
                nodes.Add(new { id = tree.Id, text = tree.Text });

            }
            return nodes;
        }
        #endregion
    }
}
