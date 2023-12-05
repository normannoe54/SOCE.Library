using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using SOCE.Library.Db;
using System.Globalization;
using MaterialDesignThemes.Wpf;
using SOCE.Library.UI.Views;
using MimeKit;
using MailKit.Net.Smtp;
using System.Diagnostics;
using System.Windows;
using System.Threading.Tasks;

namespace SOCE.Library.UI.ViewModels
{
    public class TimesheetViewerVM : BaseVM
    {

        private TimesheetSubmissionModel _submittedTimesheet;
        public TimesheetSubmissionModel SubmittedTimesheet
        {
            get
            {
                return _submittedTimesheet;
            }
            set
            {
                _submittedTimesheet = value;

                IsButtonEnabled = _submittedTimesheet.Missing ? false : true;

                RaisePropertyChanged(nameof(SubmittedTimesheet));
            }
        }



        private EmployeeModel _selectedEmployee;
        public EmployeeModel SelectedEmployee
        {
            get
            {
                return _selectedEmployee;
            }
            set
            {
                _selectedEmployee = value;
                RaisePropertyChanged(nameof(SelectedEmployee));
            }
        }

        public ICommand ApproveTimesheetCommand { get; set; }
        public ICommand DenyTimesheetCommand { get; set; }

        public ICommand PreviousCommand { get; set; }
        public ICommand BackToSummaryCommand { get; set; }
        
        public ICommand NextCommand { get; set; }

        public ICommand CurrentCommand { get; set; }

        public ICommand ExportToExcel { get; set; }

        private ObservableCollection<TimesheetRowModel> _rowdata = new ObservableCollection<TimesheetRowModel>();
        public ObservableCollection<TimesheetRowModel> Rowdata
        {
            get { return _rowdata; }
            set
            {
                _rowdata = value;
                RaisePropertyChanged(nameof(Rowdata));
            }
        }

        private string _displayName = "";
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                _displayName = value;
                RaisePropertyChanged(nameof(DisplayName));
            }
        }


        private double _total;
        public double Total
        {
            get { return _total; }
            set
            {
                _total = value;
                RaisePropertyChanged(nameof(Total));
            }
        }

        private ObservableCollection<DoubleWrapper> _totalHeader = new ObservableCollection<DoubleWrapper>();
        public ObservableCollection<DoubleWrapper> TotalHeader
        {
            get { return _totalHeader; }
            set
            {
                _totalHeader = value;
                RaisePropertyChanged(nameof(TotalHeader));
            }
        }

        private ObservableCollection<DateWrapper> _datesummary = new ObservableCollection<DateWrapper>();
        public ObservableCollection<DateWrapper> DateSummary
        {
            get { return _datesummary; }
            set
            {
                _datesummary = value;
                RaisePropertyChanged(nameof(DateSummary));
            }
        }

        //private int DateTimesheet;

        private double _baseHours = 0;
        public double BaseHours
        {
            get { return _baseHours; }
            set
            {
                _baseHours = value;
                RaisePropertyChanged(nameof(BaseHours));
            }
        }

        private bool _isButtonEnabled = true;
        public bool IsButtonEnabled
        {
            get { return _isButtonEnabled; }
            set
            {
                _isButtonEnabled = value;
                RaisePropertyChanged(nameof(IsButtonEnabled));
            }
        }

        public TimesheetReviewVM basevm { get; set; }
        public TimesheetViewerVM(TimesheetReviewVM vmbase, TimesheetSubmissionModel tsm)
        {
            basevm = vmbase;
            SubmittedTimesheet = tsm;
            BaseHours = basevm.BaseHours;
            this.ApproveTimesheetCommand = new RelayCommand<bool>(ReportTimesheet);
            this.DenyTimesheetCommand = new RelayCommand<bool>(ReportTimesheet);
            this.BackToSummaryCommand = new RelayCommand(BackToSummary);
            this.ExportToExcel = new RelayCommand(ExportCurrentTimesheetToExcel);
            SelectedEmployee = tsm.Employee;
            basevm.UpdateDates(basevm.firstdate);
            LoadTimesheetData(tsm.Employee);
        }

        /// <summary>
        /// Load Date of Timesheet
        /// </summary>
        /// <param name="currdate"></param>
        //private void LoadCurrentTimesheet(DateTime currdate)
        //{
        //    UpdateDates(currdate);
        //    //LoadTimesSheetSubmissionData(currdate);
        //    //LoadProjects(issubmitted);
        //    //LoadTimesheetData();
        //}

        //private void OpenTimeSheet(TimesheetSubmissionModel timesheetsubmission)
        //{
        //    if (!timesheetsubmission.Missing)
        //    {
        //        DisplayName = timesheetsubmission.Employee.FullName;
        //        SubmittedTimesheet = timesheetsubmission;
        //        LoadTimesheetData(timesheetsubmission.Employee);
        //    }
        //}

        /// <summary>
        /// Load DB
        /// </summary>
        private void LoadTimesheetData(EmployeeModel em)
        {
            Rowdata.Clear();
            DateTime datestart = basevm.DateSummary.First().Value;
            DateTime dateend = basevm.DateSummary.Last().Value;

            //update employee Id
            List<TimesheetRowDbModel> dbtimesheetdata = SQLAccess.LoadTimeSheet(datestart, dateend, em.Id);

            var groupedlist = dbtimesheetdata.OrderBy(x => x.SubProjectId).GroupBy(x => x.SubProjectId).ToList();
            List<TimesheetRowModel> trms = new List<TimesheetRowModel>();
            foreach (var item in groupedlist)
            {
                TimesheetRowDbModel subitem = item.First();
                SubProjectDbModel spdb = SQLAccess.LoadSubProjectsBySubProject(subitem.SubProjectId);
                ProjectDbModel pdb = SQLAccess.LoadProjectsById(spdb.ProjectId);

                ProjectLowResModel pm = new ProjectLowResModel(pdb);
                SubProjectLowResModel spm = new SubProjectLowResModel(spdb);

                TimesheetRowModel trm = new TimesheetRowModel()
                {
                    Project = pm
                };

                //SubProjectModel subpmnew = trm.SubProjects.Where(x => x.Id == spm.Id)?.First();

                trm.SelectedSubproject = spm;

                foreach (TimesheetRowDbModel trdm in item)
                {
                    DateTime dt = DateTime.ParseExact(trdm.Date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                    trm.Entries.Add(new TREntryModel() { Date = dt, TimeEntry = trdm.TimeEntry, Id = trdm.Id });
                }

                DateTime dateinc = datestart;

                while (dateinc <= dateend)
                {
                    if (!trm.Entries.Any(x => x.Date == dateinc))
                    {
                        //add
                        trm.Entries.Add(new TREntryModel() { Date = dateinc, TimeEntry = 0 });
                    }
                    dateinc = dateinc.AddDays(1);
                }
                trm.Entries = new ObservableCollection<TREntryModel>(trm.Entries.OrderBy(x => x.Date).ToList());
                trms.Add(trm);
            }

            List<TimesheetRowModel> trmadjusted = trms?.OrderByDescending(x => x.Project.ProjectNumber).ToList();

            foreach (TimesheetRowModel trm in trmadjusted)
            {
                Rowdata.Add(trm);
            }

            SumTable();
        }

        /// <summary>
        /// Sum Table
        /// </summary>
        private void SumTable()
        {
            if (Rowdata.Count > 0)
            {
                TotalHeader.Clear();
                int numofentries = Rowdata[0].Entries.Count();

                for (int i = 0; i < numofentries; i++)
                {
                    double total = 0;

                    foreach (TimesheetRowModel trm in Rowdata)
                    {
                        total += trm.Entries[i].TimeEntry;
                    }

                    TotalHeader.Add(new DoubleWrapper(total));
                }
                Total = TotalHeader.Sum(x => x.Value);
            }
            else
            {
                //make 0s
                TotalHeader.Clear();
                foreach (DateWrapper date in DateSummary)
                {
                    TotalHeader.Add(new DoubleWrapper(0));
                }
                Total = 0;
            }
        }

        private void ExportCurrentTimesheetToExcel()
        {
            //do stuff
            //save down to downloads
            try
            {
                string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string pathDownload = Path.Combine(pathUser, "Downloads\\TimeSheet.xlsx");
                File.WriteAllBytes(pathDownload, Properties.Resources.TimesheetBase);
                Excel.Excel exinst = new Excel.Excel(pathDownload);

                if (Rowdata.Count > 0)
                {
                    TimesheetRowModel trmfirst = Rowdata[0];
                    int count = trmfirst.Entries.Count;

                    for (int i = 0; i < count - 2; i++)
                    {
                        exinst.InsertBlankColumns(i + 4);
                    }

                    List<string> dates = new List<string>();
                    List<double> number = new List<double>();

                    //write column formula
                    char cval = 'D';
                    char finalval = 'C';

                    foreach (TREntryModel ent1 in Rowdata[0].Entries)
                    {
                        dates.Add(ent1.Date.DayOfWeek.ToString().Substring(0, 1));
                        number.Add(ent1.Date.Day);
                        finalval++;
                    }

                    exinst.WriteRow(5, 4, dates);
                    exinst.WriteRow(6, 4, number);

                    string cell = $"{basevm.MonthYearString} {basevm.DateString}";
                    exinst.WriteCell(1, 4, cell);

                    string name = $"{SelectedEmployee.FullName}";
                    exinst.WriteCell(3, 4, name);

                    int basenum = 6;

                    List<TimesheetRowModel> exportedtime = Rowdata.ToList().OrderBy(x => x.Project.ProjectNumber.ToString().Substring(2)).ThenBy(x => x.Project.ProjectNumber).ToList();

                    foreach (TimesheetRowModel trm in exportedtime)
                    {
                        List<object> rowinputs = new List<object>();
                        //string projectname = $"[{trm.SelectedSubproject.PointNumber}]  {trm.Project.ProjectName}";
                        rowinputs.Add(trm.Project.ProjectNumber);
                        rowinputs.Add(trm.SelectedSubproject.PointNumber);
                        rowinputs.Add(trm.Project.ProjectName);

                        foreach (TREntryModel ent in trm.Entries)
                        {
                            rowinputs.Add(ent.TimeEntry);
                        }

                        exinst.InsertRowBelow(basenum, rowinputs);

                        basenum++;

                        string formula = $"SUM(D{basenum}: {finalval}{basenum})";
                        exinst.WriteFormula(basenum, count + 4, formula);
                    }

                    for (int i = 0; i < count + 1; i++)
                    {
                        string formula = $"SUM({cval}7:{cval}{basenum})";
                        exinst.WriteFormula(basenum + 1, i + 4, formula);
                        cval++;
                    }
                    exinst.RotateTextVertical(5, 2);
                    exinst.CenterCell(5, 3);
                    exinst.SaveDocument();
                }

                Process.Start(pathDownload);
            }
            catch
            {
            }
        }

        private async void ReportTimesheet(bool approve)
        {
            YesNoView view = new YesNoView();
            YesNoVM aysvm = new YesNoVM();

            if (approve)
            {
                aysvm.Message = "Approve Time Sheet?";
            }
            else
            {
                aysvm.Message = "Deny Time Sheet?";
            }

            view.DataContext = aysvm;
            var result = await DialogHost.Show(view, "RootDialog");
            aysvm = view.DataContext as YesNoVM;

            if (aysvm.Result)
            {

                if (!SubmittedTimesheet.Missing)
                {
                    foreach (TimesheetRowModel trm in Rowdata)
                    {
                        //adding or modifying an existing submission
                        foreach (TREntryModel trentry in trm.Entries)
                        {
                            if (trentry.TimeEntry > 0 && trm.SelectedSubproject != null && trentry.Id != 0)
                            {
                                TimesheetRowDbModel dbmodel = new TimesheetRowDbModel()
                                {
                                    Id = trentry.Id,
                                    Approved = Convert.ToInt32(approve),
                                };

                                SQLAccess.UpdateTimesheetDataApproved(dbmodel);
                                //get data that needs to be removed
                            }
                        }
                    }

                    //load timesheets


                    if (!approve)
                    {
                        //if (SubmittedTimesheet.Approved)
                        //{
                        //    EmployeeModel em = SubmittedTimesheet.Employee;

                        //    EmployeeDbModel employee = new EmployeeDbModel()
                        //    {
                        //        Id = em.Id,
                        //        FirstName = em.FirstName,
                        //        LastName = em.LastName,
                        //        AuthId = (int)em.Status,
                        //        Title = em.Title,
                        //        Email = em.Email,
                        //        PhoneNumber = em.PhoneNumber,
                        //        Extension = em.Extension,
                        //        Rate = em.Rate,
                        //        PTORate = em.PTORate,
                        //        PTOHours = em.PTOHours,
                        //        PTOCarryover = em.PTOCarryover,
                        //        HolidayHours = em.HolidayHours,
                        //        SickHours = em.SickHours,
                        //        SickRate = em.SickRate,
                        //        SickCarryover = em.SickCarryover
                        //    };
                        //    //delete the time
                        //    SQLAccess.UpdateEmployee()
                        //}

                        SQLAccess.DeleteTimesheetSubmission(SubmittedTimesheet.Id);

                        TextPart txt = new TextPart("plain")
                        {
                            Text = $"Hello {SubmittedTimesheet.Employee.FirstName}, {Environment.NewLine}" +
                            $"Your timesheet has been denied for the following pay period: {Environment.NewLine}" +
                            $"{Environment.NewLine} {basevm.MonthYearString} {basevm.DateString} {Environment.NewLine} {Environment.NewLine}" +
                            $"Thanks,{Environment.NewLine}" +
                            $"SOCE Portal Dev Team."

                        };

                        SendEmailMessage(txt, SubmittedTimesheet.Employee, "SOCE Portal Timesheet Denied");

                    }
                    else
                    {
                        TimesheetSubmissionDbModel dbmodel2 = new TimesheetSubmissionDbModel()
                        {
                            Id = SubmittedTimesheet.Id,
                            EmployeeId = SubmittedTimesheet.Employee.Id,
                            Date = (int)long.Parse(SubmittedTimesheet.Date.ToString("yyyyMMdd")),
                            TotalHours = SubmittedTimesheet.TotalHours,
                            PTOHours = SubmittedTimesheet.PTOHours,
                            OTHours = SubmittedTimesheet.OTHours,
                            SickHours = SubmittedTimesheet.SickHours,
                            HolidayHours = SubmittedTimesheet.HolidayHours,
                            Approved = Convert.ToInt32(approve)
                        };

                        SQLAccess.UpdateTimesheetSubmission(dbmodel2);

                        //TextPart txt = new TextPart("plain")
                        //{
                        //    Text = $"Hello {SubmittedTimesheet.Employee.FirstName}, {Environment.NewLine}" +
                        //    $"Your timesheet has been approved for the following pay period: {Environment.NewLine}" +
                        //    $"{Environment.NewLine} {MonthYearString} {DateString} {Environment.NewLine} {Environment.NewLine}" +
                        //    $"Thanks,{Environment.NewLine}" +
                        //    $"SOCE Portal Dev Team."

                        //};

                        //SendEmailMessage(txt, SubmittedTimesheet.Employee, "SOCE Portal Timesheet Approved");
                    }

                    ReloadTimesheet(basevm.DateSummary.First().Value);
                }
            }
        }

        private async void BackToSummary()
        {
            CoreAI CurrentPage = IoCCore.Application as CoreAI;
            CurrentPage.MakeBlurry();
            await Task.Run(() => Task.Delay(600));
            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                basevm.ReviewVM = new EmployeeSummaryVM(basevm);
            }));
            await Task.Run(() => Task.Delay(600));
            CurrentPage.MakeClear();

        }

        public void ReloadTimesheet(DateTime date)
        {
            TotalHeader.Clear();

            TimesheetSubmissionDbModel tsdbm = SQLAccess.LoadTimeSheetSubmissionData((int)long.Parse(date.Date.ToString("yyyyMMdd")), SelectedEmployee.Id);

            if (tsdbm == null)
            {
                //not submitted yet
                if (SelectedEmployee.IsActive)
                {
                    SubmittedTimesheet = TimesheetSubmissionModel.Didnotsubmityet(SelectedEmployee);
                    LoadTimesheetData(SelectedEmployee);
                }
            }
            else
            {
                SubmittedTimesheet =  new TimesheetSubmissionModel(tsdbm, SelectedEmployee);
                LoadTimesheetData(SelectedEmployee);
            }

        }

        private async void SendEmailMessage(TextPart textbody, EmployeeModel employeetosendto, string subject)
        {
            SmtpClient client = new SmtpClient();
            client.Connect("smtp-mail.outlook.com", 587, false);
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            client.Authenticate("normnoe@shirkodonovan.com", "Jules0714!");
            MimeMessage mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress("Portal Help Desk", "portalhelpdesk@shirkodonovan.com"));
            mailMessage.To.Add(new MailboxAddress(employeetosendto.FirstName, employeetosendto.Email));
            mailMessage.Subject = subject;
            mailMessage.Body = textbody;
            await client.SendAsync(mailMessage);
            client.Disconnect(true);
        }
    }
}
