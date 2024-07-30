using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SOCE.Library.Db;
using System.Collections.ObjectModel;
using SOCE.Library.UI.Views;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Office.Interop.Excel;

namespace SOCE.Library.UI.ViewModels
{
    public class AddServiceLogVM : BaseVM
    {

        public bool result = false;

        private ObservableCollection<SubProjectAddServiceModel> _subProjects = new ObservableCollection<SubProjectAddServiceModel>();
        public ObservableCollection<SubProjectAddServiceModel> SubProjects
        {
            get { return _subProjects; }
            set
            {
                _subProjects = value;
                RaisePropertyChanged(nameof(SubProjects));
            }
        }

        private string _errorMessage = "";
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                RaisePropertyChanged("ErrorMessage");
            }
        }

        private ProjectViewResModel _baseProject;
        public ProjectViewResModel BaseProject
        {
            get { return _baseProject; }
            set
            {
                _baseProject = value;
                RaisePropertyChanged("BaseProject");
            }
        }

        public ICommand AcceptCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        private AddServiceVM baseViewModel;
        //public ICommand AdserviceCommand { get; set; }
        //public ICommand SynchAdserviceFileCommand { get; set; }
        //public ICommand RemoveAdserviceCommand { get; set; }

        public AddServiceLogVM(ProjectViewResModel pm, AddServiceVM psvm, List<SubProjectAddServiceModel> addservices)
        {
            baseViewModel = psvm;
            BaseProject = pm;
            SubProjects = new ObservableCollection<SubProjectAddServiceModel>(addservices);
            this.AcceptCommand = new RelayCommand(this.Imortalize);
            this.CloseCommand = new RelayCommand(this.CloseWindow);

        }

        private void CloseWindow()
        {

            baseViewModel.LoadAdservice();
            baseViewModel.LeftDrawerOpen = false;

        }

        private void Imortalize()
        {
            List<SubProjectAddServiceModel> subslogged = SubProjects.Where(x => x.SelectedLogAddService).ToList();

            //bool checkfornonbillablepublish = subslogged.Any(x => !x.IsBillable && x.PrintLogAddService);

            if (String.IsNullOrEmpty(BaseProject.Projectfolder))
            {
                //are you sure you want to blah?
                ErrorMessage = "Specify project folder before exporting.";
            }
            else if (SubProjects.Count == 0)
            {
                //are you sure you want to blah?
                ErrorMessage = "Select something to log or cancel.";
            }
            //else if (checkfornonbillablepublish)
            //{
            //    ErrorMessage = "You cannot publish a proposal for a non billable add-service.  Please revise inputs.";
            //}
            else
            {
                string generalpath = BaseProject.Projectfolder;
                string adservicefolderpath = generalpath + "\\Add Services";
                string newfilename = $"{adservicefolderpath}\\{BaseProject.ProjectNumber}_Add_Service_Tracking_Log_{DateTime.Now.ToString("yyyyMMdd")}.xlsx";

                if (Directory.Exists(adservicefolderpath))
                {
                    //does fileexist
                    string[] files = Directory.GetFiles(adservicefolderpath, "*.xlsx");

                    if (files.Length == 0)
                    {
                        File.WriteAllBytes(newfilename, Properties.Resources.AddServiceTrackingLog);
                        CreateLog(adservicefolderpath, newfilename, true);
                    }
                    else
                    {
                        string archivepath = adservicefolderpath + "\\Archive";

                        if (!Directory.Exists(archivepath))
                        {
                            DirectoryInfo di = Directory.CreateDirectory(archivepath);
                        }

                        string archivepathdir = archivepath + "\\" + $"{DateTime.Now.ToString("yyyyMMdd")}";

                        if (!Directory.Exists(archivepathdir))
                        {
                            DirectoryInfo di = Directory.CreateDirectory(archivepathdir);
                        }
                        else
                        {
                            List<string> directorieslist = Directory.GetDirectories(archivepath).ToList();
                            bool stillexists = true;
                            int i = 0;
                            do
                            {
                                i++;
                                string newname = archivepathdir + $"({i})";
                                stillexists = directorieslist.Any(x => x.Equals(newname));

                            } while (stillexists);

                            archivepathdir = archivepathdir + $"({i})";
                            DirectoryInfo di = Directory.CreateDirectory(archivepathdir);

                        }

                        foreach (string file in files)
                        {
                            string oldname = file.Remove(0, adservicefolderpath.Length + 1);
                            string newname = archivepathdir + "\\" + oldname;

                            try
                            {
                                File.Move(file, newname);
                                File.Copy(newname, newfilename);
                            }
                            catch
                            {
                                ErrorMessage = "Error has occured, double check existing add-service files are not open.";
                            }
                        }

                        //List<SubProjectAddServiceModel> subthatarebeingpublished = subslogged.Where(x => x.IsBillable).ToList();

                        //only move directories that are being changed.
                        string[] directories = Directory.GetDirectories(adservicefolderpath);
                        foreach (string dir in directories)
                        {
                            string oldname = dir.Remove(0, adservicefolderpath.Length + 1);
                            string newname = archivepathdir + "\\" + oldname;
                            bool isfound = false;
                            foreach (SubProjectAddServiceModel suubadd in subslogged)
                            {
                                string nametocompare = $"{BaseProject.ProjectNumber}{suubadd.PointNumber.Substring(1)}";
                                if (nametocompare == oldname)
                                {
                                    isfound = true;
                                    break;
                                }
                            }
                            //bool isfound = subthatarebeingpublished.Any(x => oldname.Equals($"{BaseProject.ProjectNumber}{x.PointNumber.Substring(1)}"));

                            if (dir != archivepath && isfound)
                            {
                                try
                                {
                                    Directory.Move(dir, newname);
                                }
                                catch
                                {
                                    ErrorMessage = "Error has occured, double check existing add-service files are not open.";
                                }
                            }
                        }
                        CreateLog(adservicefolderpath, newfilename, false);
                    }
                }
                else
                {
                    DirectoryInfo di = Directory.CreateDirectory(adservicefolderpath);
                    File.WriteAllBytes(newfilename, Properties.Resources.AddServiceTrackingLog);
                    CreateLog(adservicefolderpath, newfilename, true);
                }


                Process.Start(adservicefolderpath);
                baseViewModel.LoadAdservice();
                baseViewModel.LeftDrawerOpen = false;

            }
        }

        private void WriteSummary(Excel.Excel exinst, SubProjectAddServiceModel submodel, int row)
        {
            exinst.WriteCell(row, 3, $"{BaseProject.ProjectNumber}{submodel.PointNumber.Substring(1)}");
            exinst.WriteCell(row, 4, submodel.Description);
            exinst.WriteCell(row, 5, submodel.DateInitiated?.ToString("MM/dd/yyyy"));
            exinst.WriteCell(row, 6, submodel.IsBillable ? "YES" : "NO");
            exinst.WriteCell(row, 7, submodel.DateInvoiced?.ToString("MM/dd/yyyy"));

            if (submodel.IsHourly && submodel.Fee <= 0)
            {
                exinst.WriteCell(row, 8, "Hourly");
            }
            else if (submodel.IsHourly && submodel.Fee > 0)
            {
                exinst.WriteCell(row, 8, $"Hourly Not to Exceed (${submodel.Fee:n0})");
            }
            else
            {
                exinst.WriteCell(row, 8, $" ${submodel.Fee:n0}");
            }
        }

        private void CreateLog(string basepath, string finalpath, bool isnew)
        {
            List<SubProjectAddServiceModel> SubsOrdered = SubProjects.ToList().OrderBy(t => Convert.ToDouble(t.PointNumber)).ToList();
            List<foundpage> foundpages = new List<foundpage>();
            try
            {
                //string finalpath = $"{path}\\{BaseProject.ProjectNumber}_Add_Service_Tracking_Log_{DateTime.Now.ToString("yyyyMMdd")}.xlsx";

                //File.WriteAllBytes(finalpath, Properties.Resources.AddServiceTrackingLog);
                Excel.Excel exinst = new Excel.Excel(finalpath);
                Thread.Sleep(200);

                if (isnew)
                {
                    exinst.WriteCell(8, 4, $"{BaseProject.ProjectName}");
                    exinst.WriteCell(9, 4, $"{BaseProject.ProjectNumber}");
                    exinst.WriteCell(10, 4, $"{BaseProject.Client.ClientName}");
                }

                foreach (SubProjectAddServiceModel submodel in SubsOrdered)
                {
                    //if (submodel.SelectedLogAddService)
                    //{
                        string firstcell = "";
                        int row = 13;
                        bool found = false;
                        do
                        {
                            firstcell = exinst.GetCellValue(row, 3);
                            if (firstcell == $"{BaseProject.ProjectNumber}{submodel.PointNumber.Substring(1)}")
                            {
                                WriteSummary(exinst, submodel, row);
                                found = true;
                                break;
                            }
                            row++;
                        }
                        while (!String.IsNullOrEmpty(firstcell) && row < 100);

                        if (!found)
                        {
                            WriteSummary(exinst, submodel, row - 1);
                        }

                        submodel.IsChangedLog = false;
                        submodel.UpdateSubProject(true);
                    //}
                }

                exinst.SaveDocument();

                foreach (SubProjectAddServiceModel submodel in SubsOrdered)
                {
                    if (submodel.SelectedLogAddService)
                    {
                        //first delete worksheet if it exists?
                        string worksheetname = $"Proposal #{submodel.PointNumber.Substring(2)}";
                        exinst.DeleteWorksheet(worksheetname);
                        exinst.CopyFirstWorksheet(worksheetname, "Default");

                        exinst.WriteCell(7, 4, submodel.PersonAddressed);
                        exinst.WriteCell(9, 4, submodel.ClientAddress);
                        exinst.WriteCell(11, 4, submodel.ClientCity);
                        //exinst.WriteCell(11, 4, submodel.DateInitiated?.ToString("MM/dd/yyyy"));
                        exinst.WriteCell(13, 4, submodel.NameOfClient);
                        string date = submodel.DateSent?.ToString("MMMM dd, yyyy");
                        //exinst.WriteCell(7, 10, submodel.DateInitiated?.ToString("MM/dd/yyyy"));
                        exinst.WriteCell(7, 10, date);
                        exinst.WriteCell(9, 10, BaseProject.ProjectName);
                        exinst.WriteCell(11, 10, $"{BaseProject.ProjectNumber}{submodel.PointNumber.Substring(1)}");
                        exinst.WriteCell(13, 10, submodel.ClientCompanyName);
                        exinst.WriteCell(16, 5, submodel.ExpandedDescription);

                        if (submodel.IsHourly && submodel.Fee <= 0)
                        {
                            exinst.WriteCell(23, 5, "Hourly");
                        }
                        else if (submodel.IsHourly && submodel.Fee > 0)
                        {
                            exinst.WriteCell(23, 5, $"Hourly Not to Exceed (${submodel.Fee:n0})");
                        }
                        else
                        {
                            exinst.WriteCell(23, 5, $" ${submodel.Fee:n0}");
                        }

                        exinst.WriteCell(32, 5, submodel.SelectedEmployee?.FullName);

                        if (submodel.SelectedEmployee?.SignatureOfPM != null)
                        {

                            exinst.AddPicture(31, 4, submodel.SelectedEmployee.SignatureOfPM);
                        }
                    }
                    exinst.SaveDocument();

                    int number = exinst.GetActiveWorksheetNumber();
                    foundpage page = new foundpage()
                    {
                        pagenumber = number,
                        foldername = $"{BaseProject.ProjectNumber}{submodel.PointNumber.Substring(1)}",
                        addname = $"{BaseProject.ProjectNumber}{submodel.PointNumber.Substring(1)}_Add_Service_Proposal_#{submodel.PointNumber.Substring(2)}",
                        publish = submodel.SelectedLogAddService,
                        billable = submodel.IsBillable
                    };

                    foundpages.Add(page);
                }
                exinst.SaveDocument();
                exinst.Close();

                if (foundpages.Count > 0)
                {
                    foundpages.ForEach(x => x.pagenumber -= 1);
                }
            }
            catch
            {
                ErrorMessage = "Error has occured, double check existing add-service files are not open.";
            }
            //Thread.Sleep(2000);

            ExportWorkbookToPdf(finalpath, basepath, foundpages);
        }


        public bool ExportWorkbookToPdf(string workbookPath, string basepath, List<foundpage> foundpages)
        {
            // If either required string is null or empty, stop and bail out
            if (string.IsNullOrEmpty(workbookPath) || string.IsNullOrEmpty(basepath))
            {
                return false;
            }

            // Create COM Objects
            Microsoft.Office.Interop.Excel.Application excelApplication;
            Microsoft.Office.Interop.Excel.Workbook excelWorkbook;

            // Create new instance of Excel
            excelApplication = new Microsoft.Office.Interop.Excel.Application();

            // Make the process invisible to the user
            excelApplication.ScreenUpdating = false;

            // Make the process silent
            excelApplication.DisplayAlerts = false;

            // Open the workbook that you wish to export to PDF
            excelWorkbook = excelApplication.Workbooks.Open(workbookPath);

            // If the workbook failed to open, stop, clean up, and bail out
            if (excelWorkbook == null)
            {
                excelApplication.Quit();

                excelApplication = null;
                excelWorkbook = null;

                return false;
            }

            var exportSuccessful = true;
            try
            {
                //foreach (Worksheet work in excelWorkbook.Worksheets)
                //{
                //    work.PageSetup.PrintArea = "Print_Area";
                //}
                foreach (foundpage page in foundpages)
                {
                    //if (page.billable)
                    //{
                        //create directory
                        string foldername = basepath + "\\" + page.foldername;
                        DirectoryInfo di = Directory.CreateDirectory(foldername);
                        if (page.publish)
                        {
                            string filename = foldername + "\\" + page.addname + ".pdf";
                            int newpage = page.pagenumber;

                            excelWorkbook.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, filename, XlFixedFormatQuality.xlQualityStandard, true, false, newpage, newpage, false, System.Reflection.Missing.Value);
                        }
                    //}
                }
            }
            catch (System.Exception ex)
            {
                // Mark the export as failed for the return value...
                exportSuccessful = false;

                // Do something with any exceptions here, if you wish...
                // MessageBox.Show...        
            }
            finally
            {
                // Close the workbook, quit the Excel, and clean up regardless of the results...
                excelWorkbook.Close();
                excelApplication.Quit();

                excelApplication = null;
                excelWorkbook = null;
            }

            // You can use the following method to automatically open the PDF after export if you wish
            // Make sure that the file actually exists first...
            //if (System.IO.File.Exists(outputPath))
            //{
            //    System.Diagnostics.Process.Start(outputPath);
            //}

            return exportSuccessful;
        }
    }
    public class foundpage
    {
        public int pagenumber { get; set; }
        public string addname { get; set; }
        public string foldername { get; set; }
        public bool publish { get; set; }
        public bool billable { get; set; }
    }
}
