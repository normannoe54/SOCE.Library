using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;

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

        public void FitColumns()
        {
            activeworksheet.Columns().AdjustToContents(1,100,4.5,45);
            document.Save();
        }

        public void SaveDocument()
        {
            document.Save();
        }

        public void InsertBlankColumns(int column)
        {
            IXLColumn colnew = activeworksheet.Column(column);
            colnew.InsertColumnsAfter(1);
            //document.Save();
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
            IXLRange rangeWithStrings = activeworksheet.Cell(row, startingcell).InsertData(values, true);

            //document.Save();
        }

        public void WriteCell(int row, int column, string cellvalue)
        {
            activeworksheet.Cell(row, column).Value = cellvalue;
            //activeworksheet.Cell(row, column).Comment.Style.Alignment.SetHorizontal(XLDrawingHorizontalAlignment.Left);

            //document.Save();
        }

        public void CenterCell(int row, int column)
        {
            activeworksheet.Cell(row, column).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
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

        public void Save()
        {
            document.Save();
        }
    }
}
