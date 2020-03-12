using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using XlRowCol = Microsoft.Office.Interop.Excel.XlRowCol;

namespace ZoDream.Helper.Local
{
    public class ExcelHelper
    {
        public string MFilename;
        public Application App;
        public Workbooks Wbs;
        public Workbook Wb;
        public Worksheets Wss;
        public Worksheet Ws;

        /// <summary>
        /// 创建一个Microsoft.Office.Interop.Excel对象
        /// </summary>
        public void Create()
        {
            App = new Application();
            Wbs = App.Workbooks;
            Wb = Wbs.Add(true);
        }
        /// <summary>
        /// 打开一个Microsoft.Office.Interop.Excel文件
        /// </summary>
        /// <param name="fileName"></param>
        public void Open(string fileName)
        {
            App = new Application();
            Wbs = App.Workbooks;
            Wb = Wbs.Add(fileName);
            //wb = wbs.Open(FileName, 0, true, 5,"", "", true, XlPlatform.xlWindows, "t", false, false, 0, true,Type.Missing,Type.Missing);
            //wb = wbs.Open(FileName,Type.Missing,Type.Missing,Type.Missing,Type.Missing,Type.Missing,Type.Missing,XlPlatform.xlWindows,Type.Missing,Type.Missing,Type.Missing,Type.Missing,Type.Missing,Type.Missing,Type.Missing);
            MFilename = fileName;
        }
        /// <summary>
        /// 获取一个工作表
        /// </summary>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public Worksheet GetSheet(string sheetName)
        {
            var s = (Worksheet)Wb.Worksheets[sheetName];
            return s;
        }

        /// <summary>
        /// 添加一个工作表
        /// </summary>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public Worksheet AddSheet(string sheetName)
        {
            var s = (Worksheet)Wb.Worksheets.Add(Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            s.Name = sheetName;
            return s;
        }

        /// <summary>
        /// 删除一个工作表
        /// </summary>
        /// <param name="sheetName"></param>
        public void DelSheet(string sheetName)
        {
            ((Worksheet)Wb.Worksheets[sheetName]).Delete();
        }

        /// <summary>
        /// 重命名一个工作表一
        /// </summary>
        /// <param name="oldSheetName"></param>
        /// <param name="newSheetName"></param>
        /// <returns></returns>
        public Worksheet ReNameSheet(string oldSheetName, string newSheetName)
        {
            var s = (Worksheet)Wb.Worksheets[oldSheetName];
            s.Name = newSheetName;
            return s;
        }

        /// <summary>
        /// 重命名一个工作表二
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="newSheetName"></param>
        /// <returns></returns>
        public Worksheet ReNameSheet(Worksheet sheet, string newSheetName)
        {

            sheet.Name = newSheetName;

            return sheet;
        }
        /// <summary>
        /// ws：要设值的工作表     X行Y列     value   值
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="value"></param>
        public void SetCellValue(Worksheet ws, int x, int y, object value)
        {
            ws.Cells[x, y] = value;
        }

        public void SetRow(Worksheet ws, IList<object> columns, int row = 0, int column = 0)
        {
            for (var i = 0; i < columns.Count; i++)
            {
                ws.Cells[column + i, row] = columns[i];
            }
        }

        /// <summary>
        /// ws：要设值的工作表的名称 X行Y列 value 值
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="value"></param>
        public void SetCellValue(string ws, int x, int y, object value)
        {

            GetSheet(ws).Cells[x, y] = value;
        }

        /// <summary>
        /// 设置一个单元格的属性   字体，   大小，颜色   ，对齐方式
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="startx"></param>
        /// <param name="starty"></param>
        /// <param name="endx"></param>
        /// <param name="endy"></param>
        /// <param name="size"></param>
        /// <param name="name"></param>
        /// <param name="color"></param>
        /// <param name="horizontalAlignment"></param>
        public void SetCellProperty(Worksheet ws, int startx, int starty, int endx, int endy, int size, string name, Constants color, Constants horizontalAlignment)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));
            if (!Enum.IsDefined(typeof(Constants), color))
                throw new InvalidEnumArgumentException(nameof(color), (int) color, typeof(Constants));
            if (!Enum.IsDefined(typeof(Constants), horizontalAlignment))
                throw new InvalidEnumArgumentException(nameof(horizontalAlignment), (int) horizontalAlignment,
                    typeof(Constants));
            name = "宋体";
            size = 12;
            color = Constants.xlAutomatic;
            horizontalAlignment = Constants.xlRight;
            ws.Range[ws.Cells[startx, starty], ws.Cells[endx, endy]].Font.Name = name;
            ws.Range[ws.Cells[startx, starty], ws.Cells[endx, endy]].Font.Size = size;
            ws.Range[ws.Cells[startx, starty], ws.Cells[endx, endy]].Font.Color = color;
            ws.Range[ws.Cells[startx, starty], ws.Cells[endx, endy]].HorizontalAlignment = horizontalAlignment;
        }

        public void SetCellProperty(string wsn, int startx, int starty, int endx, int endy, int size, string name, Constants color, Constants horizontalAlignment)
        {
            //name = "宋体";
            //size = 12;
            //color = Constants.xlAutomatic;
            //HorizontalAlignment = Constants.xlRight;

            var ws = GetSheet(wsn);
            ws.Range[ws.Cells[startx, starty], ws.Cells[endx, endy]].Font.Name = name;
            ws.Range[ws.Cells[startx, starty], ws.Cells[endx, endy]].Font.Size = size;
            ws.Range[ws.Cells[startx, starty], ws.Cells[endx, endy]].Font.Color = color;

            ws.Range[ws.Cells[startx, starty], ws.Cells[endx, endy]].HorizontalAlignment = horizontalAlignment;
        }

        /// <summary>
        /// 合并单元格
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public void UniteCells(Worksheet ws, int x1, int y1, int x2, int y2)
        {
            ws.Range[ws.Cells[x1, y1], ws.Cells[x2, y2]].Merge(Type.Missing);
        }
        /// <summary>
        /// 合并单元格
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public void UniteCells(string ws, int x1, int y1, int x2, int y2)
        {
            GetSheet(ws).Range[GetSheet(ws).Cells[x1, y1], GetSheet(ws).Cells[x2, y2]].Merge(Type.Missing);

        }

        /// <summary>
        /// 将内存中数据表格插入到Microsoft.Office.Interop.Excel指定工作表的指定位置 为在使用模板时控制格式时使用一
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="ws"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        public void InsertTable(System.Data.DataTable dt, string ws, int startX, int startY)
        {

            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                for (int j = 0; j <= dt.Columns.Count - 1; j++)
                {
                    GetSheet(ws).Cells[startX + i, j + startY] = dt.Rows[i][j].ToString();

                }

            }

        }

        /// <summary>
        /// 将内存中数据表格插入到Microsoft.Office.Interop.Excel指定工作表的指定位置二
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="ws"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        public void InsertTable(System.Data.DataTable dt, Worksheet ws, int startX, int startY)
        {

            for (var i = 0; i <= dt.Rows.Count - 1; i++)
            {
                for (var j = 0; j <= dt.Columns.Count - 1; j++)
                {

                    ws.Cells[startX + i, j + startY] = dt.Rows[i][j];

                }

            }

        }

        /// <summary>
        /// 将内存中数据表格添加到Microsoft.Office.Interop.Excel指定工作表的指定位置一
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="ws"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        public void AddTable(System.Data.DataTable dt, string ws, int startX, int startY)
        {

            for (var i = 0; i <= dt.Rows.Count - 1; i++)
            {
                for (var j = 0; j <= dt.Columns.Count - 1; j++)
                {

                    GetSheet(ws).Cells[i + startX, j + startY] = dt.Rows[i][j];

                }

            }

        }

        /// <summary>
        /// 将内存中数据表格添加到Microsoft.Office.Interop.Excel指定工作表的指定位置二
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="ws"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        public void AddTable(System.Data.DataTable dt, Worksheet ws, int startX, int startY)
        {


            for (var i = 0; i <= dt.Rows.Count - 1; i++)
            {
                for (var j = 0; j <= dt.Columns.Count - 1; j++)
                {

                    ws.Cells[i + startX, j + startY] = dt.Rows[i][j];

                }
            }

        }

        /// <summary>
        /// 插入图片操作一
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="ws"></param>
        public void InsertPictures(string filename, string ws)
        {
            GetSheet(ws).Shapes.AddPicture(filename, MsoTriState.msoFalse, MsoTriState.msoTrue, 10, 10, 150, 150);
            //后面的数字表示位置
        }
        /// <summary>
        /// 插入图片操作二
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="ws"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        public void InsertPictures(string filename, string ws, int height, int width)
        {
            GetSheet(ws).Shapes.AddPicture(filename, MsoTriState.msoFalse, MsoTriState.msoTrue, 10, 10, 150, 150);
            GetSheet(ws).Shapes.Range[Type.Missing].Height = height;
            GetSheet(ws).Shapes.Range[Type.Missing].Width = width;
        }

        /// <summary>
        /// 插入图片操作三
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="ws"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        public void InsertPictures(string filename, string ws, int left, int top, int height, int width)
        {

            GetSheet(ws).Shapes.AddPicture(filename, MsoTriState.msoFalse, MsoTriState.msoTrue, 10, 10, 150, 150);
            GetSheet(ws).Shapes.Range[Type.Missing].IncrementLeft(left);
            GetSheet(ws).Shapes.Range[Type.Missing].IncrementTop(top);
            GetSheet(ws).Shapes.Range[Type.Missing].Height = height;
            GetSheet(ws).Shapes.Range[Type.Missing].Width = width;
        }

        /// <summary>
        /// 插入图表操作
        /// </summary>
        /// <param name="chartType"></param>
        /// <param name="ws"></param>
        /// <param name="dataSourcesX1"></param>
        /// <param name="dataSourcesY1"></param>
        /// <param name="dataSourcesX2"></param>
        /// <param name="dataSourcesY2"></param>
        /// <param name="chartDataType"></param>
        public void InsertActiveChart(Microsoft.Office.Interop.Excel.XlChartType chartType, string ws, int dataSourcesX1, int dataSourcesY1, int dataSourcesX2, int dataSourcesY2, XlRowCol chartDataType)
        {
            if (!Enum.IsDefined(typeof(XlRowCol), chartDataType))
                throw new InvalidEnumArgumentException(nameof(chartDataType), (int) chartDataType, typeof(XlRowCol));
            chartDataType = XlRowCol.xlColumns;
            Wb.Charts.Add(Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            {
                Wb.ActiveChart.ChartType = chartType;
                Wb.ActiveChart.SetSourceData(GetSheet(ws).Range[GetSheet(ws).Cells[dataSourcesX1, dataSourcesY1], GetSheet(ws).Cells[dataSourcesX2, dataSourcesY2]], chartDataType);
                Wb.ActiveChart.Location(XlChartLocation.xlLocationAsObject, ws);
            }
        }
        /// <summary>
        /// 保存文档
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            if (MFilename == "")
            {
                return false;
            }
            try
            {
                Wb.Save();
                return true;
            }

            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 文档另存为
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool SaveAs(object fileName)
        {
            try
            {
                Wb.SaveAs(fileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                return true;

            }

            catch (Exception)
            {
                return false;

            }
        }
        /// <summary>
        /// 关闭一个Microsoft.Office.Interop.Excel对象，销毁对象
        /// </summary>
        public void Close()
        {
            //wb.Save();
            Wb.Close(Type.Missing, Type.Missing, Type.Missing);
            Wbs.Close();
            App.Quit();
            Wb = null;
            Wbs = null;
            App = null;
            GC.Collect();
        }
    }
}
