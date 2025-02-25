using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using System.Windows.Media;
using System.IO;
using System.Windows.Media.Imaging;
using ClosedXML.Excel.Drawings;

namespace SOCE.Library.Excel
{
    public class Excel
    {
        public XLWorkbook document { get; set; }
        public IXLWorksheet activeworksheet { get; set; }
        public Excel(string fileName)
        {
            try
            {
                document = new XLWorkbook(fileName);
                activeworksheet = document.Worksheet(1);
            }
            catch
            {
            }
        }

        public Excel()
        {
            document = new XLWorkbook();
            activeworksheet = document.Worksheets.Add("Employees");
        }

        public void Close()
        {
            document.Dispose(); 
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        public void FitColumns()
        {
            activeworksheet.Columns().AdjustToContents(1,100,4.5,45);
            document.Save();
        }

        public void SaveDocument()
        {
            document.Save();
        }

        public void RenameWorksheet(string name)
        {
            activeworksheet.Name = name;
        }

        public void DeleteRow(int row)
        {
            IXLRow rownew = activeworksheet.Row(row);
            rownew.Delete();
        }

        public void CopyFirstWorksheet(string name, string nametocopy)
        {
            IXLWorksheet value = GetSheet(nametocopy)?.CopyTo(name);
            value.Unhide();
            SetActiveSheet(value);
        }

        public int GetActiveWorksheetNumber()
        {
            int numb = activeworksheet.Position;
            return numb;
        }

        public void DeleteWorksheet(string name)
        {
            IXLWorksheet sheet = GetSheet(name);
            if (sheet != null)
            {
                sheet.Delete();
            }
        }

        public IXLWorksheet GetSheet(string name)
        {
            IXLWorksheet sheetfound = null;

            foreach (IXLWorksheet sheet in document.Worksheets)
            {
                if (sheet.Name == name)
                {
                    sheetfound = sheet;
                    break;
                }
            }
            return sheetfound;
        }

        public bool SheetExist(string name)
        {
            bool issheetfound = false;

            foreach (IXLWorksheet sheet in document.Worksheets)
            {
                if (sheet.Name == name)
                {
                    issheetfound = true;
                    break;
                }
            }
            return issheetfound;
        }

        public void SetActiveSheetByName(string name)
        {
            IXLWorksheet sheet = GetSheet(name);

            if (sheet != null)
            {
                SetActiveSheet(sheet);
            }
        }

        public void HideActiveSheet()
        {
            activeworksheet.Hide();
        }

        public void SetActiveSheet(IXLWorksheet sheetToSetActive)
        {
            foreach (IXLWorksheet sheet in document.Worksheets)
            {
                var isCurrentSheet = sheet.Equals(sheetToSetActive);

                if (isCurrentSheet)
                {
                    sheet.SetTabActive(isCurrentSheet);
                    sheet.SetTabSelected(isCurrentSheet);
                    activeworksheet = sheet;
                    break;
                }
            }
        }


        public void InsertBlankColumns(int column)
        {
            IXLColumn colnew = activeworksheet.Column(column);
            colnew.InsertColumnsAfter(1);
            //document.Save();
        }

        public void InsertBlankRowBelow(int row)
        {
            IXLRow rownew = activeworksheet.Row(row);
            rownew.InsertRowsBelow(1);
        }

        /// <summary>
        /// Insert Row
        /// </summary>
        /// <param name="row"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public void InsertRowBelow(int row, List<object> values)
        {
            IXLRow rownew = activeworksheet.Row(row);
            rownew.InsertRowsBelow(1);

            int rownewval = row + 1;

            IXLRange rangeWithStrings = activeworksheet.Cell(rownewval, 1).InsertData(values,true);

            //document.Save();
        }

        public void WriteRow<T>(int row, int startingcell, List<T> values)
        {

            IXLCell cell= activeworksheet.Cell(row, startingcell);
            IXLRange rangeWithStrings = cell.InsertData(values, true);
            //IXLRange rangeWithStrings = activeworksheet.Cell(row, startingcell).InsertData(values, true);

            //document.Save();
        }

        public void MergeCellsInRow(int row, int firstcol, int secondcol)
        {
            activeworksheet.Range(row, firstcol, row, secondcol).Merge();
            return;
        }

        public string GetCellValue(int row, int column)
        {

            IXLCell cell = activeworksheet.Cell(row, column);
            string retvalue = cell.Value.ToString();
            return retvalue;
            //IXLRange rangeWithStrings = activeworksheet.Cell(row, startingcell).InsertData(values, true);

            //document.Save();
        }

        public void MakeRowBold(int row, int startingcell, int lastcell)
        {
            for (int i = 0; i < lastcell; i++ )
            {
                IXLCell cell = activeworksheet.Cell(row, startingcell + i);
                cell.Style.Font.Bold = true;
            }

        }

        public void MakeRowCustomBorder(int row, int startingcell, int lastcell)
        {
            for (int i = 0; i < lastcell; i++)
            {
                IXLCell cell = activeworksheet.Cell(row, startingcell + i);
                cell.Style.Border.BottomBorder = XLBorderStyleValues.Medium;
                cell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            }
        }

        public void MakeRowDoubleBorderedTopandBot(int row, int startingcell, int lastcell)
        {
            for (int i = 0; i < lastcell; i++)
            {
                IXLCell cell = activeworksheet.Cell(row, startingcell + i);
                cell.Style.Border.BottomBorder = XLBorderStyleValues.Double;
            }
        }

        public void MakeRowLightBorderTop(int row, int startingcell, int lastcell)
        {
            for (int i = 0; i < lastcell; i++)
            {
                IXLCell cell = activeworksheet.Cell(row, startingcell + i);
                cell.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            }
        }

        public void MakeRowGray(int row, int startingcell, int lastcell)
        {
            for (int i = 0; i < lastcell; i++)
            {
                IXLCell cell = activeworksheet.Cell(row, startingcell + i);
                cell.Style.Fill.BackgroundColor = XLColor.DarkGray;
            }
        }

        public void MakeRowItalic(int row, int startingcell, int lastcell)
        {
            for (int i = 0; i < lastcell; i++)
            {
                IXLCell cell = activeworksheet.Cell(row, startingcell + i);
                cell.Style.Font.Italic = true;
            }
        }

        public void WriteCell(int row, int column, string cellvalue)
        {
            activeworksheet.Cell(row, column).Value = cellvalue;

        }

        public void SetCellAsAccounting(int row, int column)
        {
            var cell = activeworksheet.Cell(row, column);
            cell.DataType = XLDataType.Number;
            cell.Style.NumberFormat.SetNumberFormatId(43);
            cell.Style.NumberFormat.Format = @"_(""$""* #,##0.00_);_(""$""* \(#,##0.00\);_(""$""* ""-""??_);_(@_)";
            //cell.Style.NumberFormat.Format = "_ $* # ##0.00_ ;_ $* -# ##0.00_ ;_ $* \"-\"??_ ;_ @_ ";
            //
        }

        public void SetCellAsPercentage(int row, int column)
        {
            var cell = activeworksheet.Cell(row, column);
            cell.DataType = XLDataType.Number;
            cell.Style.NumberFormat.Format = "0%";
        }

        public void AddPicture(int row, int column, ImageSource image, double minwidth = 400)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            var bitmapSource = image as BitmapSource;
            if (bitmapSource != null)
            {
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                //MemoryStream stream = null;
                using (var stream = new MemoryStream())
                {
                    try
                    {
                        encoder.Save(stream);
                        IXLPicture picture = activeworksheet.AddPicture(stream);
                        picture.MoveTo(activeworksheet.Cell(row, column));
                        double scalefactor = 50 / Convert.ToDouble(picture.Height);
                        scalefactor = Math.Min(minwidth / Convert.ToDouble(picture.Width), scalefactor);
                        picture.ScaleHeight(scalefactor);
                        picture.ScaleWidth(scalefactor);

                    }
                    catch
                    {

                    }
                }

                //if (stream != null)
                //{

                //    //

                //}

            }

            //activeworksheet.Cell(row, column).Value = cellvalue;

        }

        public void CenterCell(int row, int column)
        {
            activeworksheet.Cell(row, column).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        }

        public void LeftCell(int row, int column)
        {
            activeworksheet.Cell(row, column).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
        }

        public void RotateTextVertical(int row, int column)
        {
            activeworksheet.Cell(row, column).Style.Alignment.SetTextRotation(90);
        }

        //public void WriteCellLeft(int row, int column, string cellvalue)
        //{
        //    activeworksheet.Cell(row, column).Value = cellvalue;
        //    activeworksheet.Cell(row, column).Comment.Style.Alignment.SetHorizontal(XLDrawingHorizontalAlignment.Left);

        //    //document.Save();
        //}

        public void WriteFormula(int row, int column, string cellvalue)
        {
            activeworksheet.Cell(row, column).FormulaA1 = cellvalue;

            //document.Save();
        }

        public void SaveAs(string filename)
        {
            document.SaveAs(filename);
        }

        public void Save()
        {
            document.Save();
        }
    }
}
