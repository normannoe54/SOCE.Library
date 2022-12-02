using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;


namespace SOCE.Library.Excel
{
    public class Excel : IDisposable
    {
        public SpreadsheetDocument document { get; set; }
        public WorksheetPart activeworksheet { get; set; }
        public Excel(string fileName, AppEnum appenum)
        {

            //Check if Excel is open
            try
            {
                switch (appenum)
                {
                    case AppEnum.Existing:
                        {
                            document = SpreadsheetDocument.Open(fileName, true);
                            activeworksheet = document.WorkbookPart.WorksheetParts.First();
                            break;
                        }
                    case AppEnum.New:
                        {
                            document = SpreadsheetDocument.Create(fileName, SpreadsheetDocumentType.Workbook);
                            WorkbookPart workbookpart = document.AddWorkbookPart();
                            workbookpart.Workbook = new Workbook();

                            // Add a WorksheetPart to the WorkbookPart.
                            activeworksheet = workbookpart.AddNewPart<WorksheetPart>();
                            activeworksheet.Worksheet = new Worksheet(new SheetData());

                            // Add Sheets to the Workbook.
                            Sheets sheets = document.WorkbookPart.Workbook.
                                AppendChild<Sheets>(new Sheets());

                            // Append a new worksheet and associate it with the workbook.
                            Sheet sheet = new Sheet()
                            {
                                Id = document.WorkbookPart.GetIdOfPart(activeworksheet),
                                SheetId = 1,
                                Name = "Sheet1"
                            };
                            sheets.Append(sheet);
                            workbookpart.Workbook.Save();
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            catch
            {
                throw new InvalidOperationException("Excel file is currently in use on the machine.");
            }

        }

        /// <summary>
        /// Write to a specified row
        /// </summary>
        /// <param name="row"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool WriteRow(int row, List<string> values)
        {
            uint rowIdx = (uint)row;

            for (int i = 0; i < values.Count; ++i)
            {
                Cell cell = InsertCell(rowIdx, i + 1, activeworksheet.Worksheet);
                cell.CellValue = new CellValue(values[i].ToString());
                cell.DataType = CellValues.String;
                activeworksheet.Worksheet.Save();
            }

            return true;
        }

        /// <summary>
        /// Insert Row
        /// </summary>
        /// <param name="row"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool InsertRow(int row, List<string> values)
        {
            uint rowIdx = (uint)row;
            SheetData sheetData = activeworksheet.Worksheet.GetFirstChild<SheetData>();
            Row lastRow = sheetData.Elements<Row>().LastOrDefault();

            //Row rowofinterest = (Row)sheetData.Elements<Row>().Where(x => x.RowIndex == rowIdx).FirstOrDefault();

            sheetData.InsertBefore(new Row() { RowIndex = rowIdx }, lastRow);
            activeworksheet.Worksheet.Save();

            //for (int i = 0; i < values.Count; ++i)
            //{
            //    Cell cell = InsertCell(rowIdx -1, i + 1, activeworksheet.Worksheet);
            //    cell.CellValue = new CellValue(values[i].ToString());
            //    cell.DataType = CellValues.String;
            //    activeworksheet.Worksheet.Save();
            //}

            return true;
        }

        /// <summary>
        /// Read from a specified row
        /// </summary>
        /// <param name="row"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public List<string> ReadRow(int row)
        {
            SheetData sheetData = activeworksheet.Worksheet.GetFirstChild<SheetData>();

            List<string> output = new List<string>();

            //get the row at the index specified
            Row r = sheetData.Elements<Row>().Where(x => x.RowIndex == row).First();

            //cycle through all the cells in the row, ignores blanks
            foreach (Cell c in r.Elements<Cell>())
            {
                output.Add(c.CellValue.Text);
            }

            return output;
        }

        /// <summary>
        /// Read from a specified row
        /// </summary>
        /// <param name="row"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public List<List<string>> ReadSheetByRow()
        {
            SheetData sheetData = activeworksheet.Worksheet.GetFirstChild<SheetData>();

            List<List<string>> output = new List<List<string>>();

            List<Row> rows = sheetData.Elements<Row>().ToList();

            foreach (Row r in rows)
            {
                List<string> rowlist = new List<string>();
                //cycle through all the cells in the row, ignores blanks
                foreach (Cell c in r.Elements<Cell>())
                {
                    int res = 0;
                    Int32.TryParse(c.CellValue.InnerText, out res);
                    string val = GetSharedStringItemById(res);
                    rowlist.Add(val);
                }
                output.Add(rowlist);
            }

            return output;
        }


        private Cell InsertCell(uint rowIndex, int columnIndex, Worksheet worksheet)
        {
            Row row = null;
            var sheetData = worksheet.GetFirstChild<SheetData>();

            // Check if the worksheet contains a row with the specified row index.
            row = sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex == rowIndex);

            if (row == null)
            {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            // Convert column index to column name for cell reference.
            var columnName = GetExcelColumnName(columnIndex);
            var cellReference = columnName + rowIndex;      // e.g. A1

            // Check if the row contains a cell with the specified column name.
            var cell = row.Elements<Cell>().FirstOrDefault(c => c.CellReference.Value == cellReference);

            if (cell == null)
            {
                cell = new Cell() { CellReference = cellReference };
                if (row.ChildElements.Count < columnIndex)
                    row.AppendChild(cell);
                else
                    row.InsertAt(cell, (int)columnIndex);
            }

            return cell;
        }

        /// <summary>
        /// Close the document
        /// </summary>
        public void Dispose()
        {
            document.WorkbookPart.Workbook.Save();
            document.Close();
        }

        /// <summary>
        /// return shared string item
        /// </summary>
        /// <param name="workbookPart"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private string GetSharedStringItemById(int id)
        {
            string text = document.WorkbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(id).InnerText;
            return text;
        }

        /// <summary>
        /// Convert column integer to excel name
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }
    }
}
