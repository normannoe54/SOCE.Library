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

namespace SOCE.Library.UI.ViewModels
{
    public class TimesheetReviewVM : BaseVM
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
            }
        }

        private ObservableCollection<TimesheetSubmissionModel> _timesheetSubmissions = new ObservableCollection<TimesheetSubmissionModel>();
        public ObservableCollection<TimesheetSubmissionModel> TimesheetSubmissions
        {
            get { return _timesheetSubmissions; }
            set
            {
                _timesheetSubmissions = value;
                RaisePropertyChanged(nameof(TimesheetSubmissions));
            }
        }

        private EmployeeModel _currentEmployee;
        public EmployeeModel CurrentEmployee
        {
            get
            {
                return _currentEmployee;
            }
            set
            {
                _currentEmployee = value;
                RaisePropertyChanged(nameof(CurrentEmployee));
            }
        }

        public ICommand ApproveTimesheetCommand { get; set; }
        public ICommand DenyTimesheetCommand { get; set; }

        public ICommand PreviousCommand { get; set; }

        public ICommand NextCommand { get; set; }

        public ICommand CurrentCommand { get; set; }

        public ICommand OpenTimesheetSubmission { get; set; }

        public ICommand EmailReminder { get; set; }

        private ObservableCollection<TimesheetRowModel> _rowdata = new ObservableCollection<TimesheetRowModel>();
        public ObservableCollection<TimesheetRowModel> Rowdata
        {
            get { return _rowdata; }
            set
            {
                _rowdata = value;
                //SumTable();
                //CollectDates();
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

        private string _monthYearString = "";
        public string MonthYearString
        {
            get { return _monthYearString; }
            set
            {
                _monthYearString = value;
                RaisePropertyChanged(nameof(MonthYearString));
            }
        }

        private string _dateString = "";
        public string DateString
        {
            get { return _dateString; }
            set
            {
                _dateString = value;
                RaisePropertyChanged(nameof(DateString));
            }
        }

        private int DateTimesheet;

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


        private int _numSubmittedEmployees = 0;
        public int NumSubmittedEmployees
        {
            get { return _numSubmittedEmployees; }
            set
            {
                _numSubmittedEmployees = value;
                RaisePropertyChanged(nameof(NumSubmittedEmployees));
            }
        }

        private double _numApprovedEmployees = 0;
        public double NumApprovedEmployees
        {
            get { return _numApprovedEmployees; }
            set
            {
                _numApprovedEmployees = value;
                RaisePropertyChanged(nameof(NumApprovedEmployees));
            }
        }

        private double _numTotalEmployees = 0;
        public double NumTotalEmployees
        {
            get { return _numTotalEmployees; }
            set
            {
                _numTotalEmployees = value;
                RaisePropertyChanged(nameof(NumTotalEmployees));
            }
        }

        private bool _isButtonEditable = false;
        public bool IsButtonEditable
        {
            get { return _isButtonEditable; }
            set
            {
                _isButtonEditable = value;
                RaisePropertyChanged(nameof(IsButtonEditable));
            }
        }

        //private double _percentComplete = 0;
        //public double PercentComplete
        //{
        //    get { return _percentComplete; }
        //    set
        //    {
        //        _percentComplete = value;
        //        RaisePropertyChanged(nameof(PercentComplete));
        //    }
        //}

        //private double _expectedProgress = 0;
        //public double ExpectedProgress
        //{
        //    get { return _expectedProgress; }
        //    set
        //    {
        //        _expectedProgress = value;
        //        RaisePropertyChanged(nameof(ExpectedProgress));
        //    }
        //}

        //private ObservableCollection<TREntryModel> BlankEntry = new ObservableCollection<TREntryModel>();

        public TimesheetReviewVM(EmployeeModel loggedinEmployee)
        {
            

            IsButtonEditable = false;
            CurrentEmployee = loggedinEmployee;

            CurrentTimesheet();

            //List<RegisteredTimesheetDataModel> rtdm = new List<RegisteredTimesheetDataModel>();

            this.ApproveTimesheetCommand = new RelayCommand<bool>(ReportTimesheet);
            this.DenyTimesheetCommand = new RelayCommand<bool>(ReportTimesheet);
            this.PreviousCommand = new RelayCommand(PreviousTimesheet);
            this.NextCommand = new RelayCommand(NextTimesheet);
            this.CurrentCommand = new RelayCommand(CurrentTimesheet);
            this.OpenTimesheetSubmission = new RelayCommand<TimesheetSubmissionModel>(OpenTimeSheet);
            this.EmailReminder = new RelayCommand<TimesheetSubmissionModel>(SendEmail);
            //SumTable();
        }

        private void SendEmail(TimesheetSubmissionModel tsm)
        {

            TextPart txt = new TextPart("plain")
            {
                Text = $"Hello {tsm.Employee.FirstName}, {Environment.NewLine}" +
                $"Please submit your timesheet for the pay period: {Environment.NewLine}" +
                $"{Environment.NewLine} {MonthYearString} {DateString} {Environment.NewLine} {Environment.NewLine}" +
                $"Thanks,{Environment.NewLine}" +
                $"SOCE Portal Dev Team."

            };

            SendEmailMessage(txt, tsm.Employee, "SOCE Portal Timesheet Reminder");

        }

        /// <summary>
        /// Button Press
        /// </summary>
        private void PreviousTimesheet()
        {
            LoadCurrentTimesheet(DateSummary.First().Value.AddDays(-1));
        }

        /// <summary>
        /// Button Press
        /// </summary>
        private void NextTimesheet()
        {
            LoadCurrentTimesheet(DateSummary.Last().Value.AddDays(1));
        }

        /// <summary>
        /// Button Press
        /// </summary>
        private void CurrentTimesheet()
        {
            LoadCurrentTimesheet(DateTime.Now);
        }

        /// <summary>
        /// Load Date of Timesheet
        /// </summary>
        /// <param name="currdate"></param>
        private void LoadCurrentTimesheet(DateTime currdate)
        {
            UpdateDates(currdate);
            LoadTimesSheetSubmissionData(currdate);
            //LoadProjects(issubmitted);
            //LoadTimesheetData();
        }

        private void OpenTimeSheet(TimesheetSubmissionModel timesheetsubmission)
        {
            if (!timesheetsubmission.Missing)
            {
                DisplayName = timesheetsubmission.Employee.FullName;
                SubmittedTimesheet = timesheetsubmission;
                LoadTimesheetData(timesheetsubmission.Employee);
            }
        }

        /// <summary>
        /// Load DB
        /// </summary>
        private void LoadTimesheetData(EmployeeModel em)
        {
            IsButtonEditable = true;
            Rowdata.Clear();
            DateTime datestart = DateSummary.First().Value;
            DateTime dateend = DateSummary.Last().Value;

            //update employee Id
            List<TimesheetRowDbModel> dbtimesheetdata = SQLAccess.LoadTimeSheet(datestart, dateend, em.Id);

            var groupedlist = dbtimesheetdata.OrderBy(x => x.SubProjectId).GroupBy(x => x.SubProjectId).ToList();

            foreach (var item in groupedlist)
            {
                TimesheetRowDbModel subitem = item.First();
                SubProjectDbModel spdb = SQLAccess.LoadSubProjectsBySubProject(subitem.SubProjectId);
                ProjectDbModel pdb = SQLAccess.LoadProjectsById(spdb.ProjectId);

                ProjectModel pm = new ProjectModel(pdb);
                SubProjectModel spm = new SubProjectModel(spdb);

                //ProjectModel pmnew = ProjectList.Where(x => x.Id == pm.Id)?.FirstOrDefault();

                //if (pmnew == null)
                //{
                //    //are you dumb?
                //    foreach (TimesheetRowDbModel trdm in item)
                //    {
                //        SQLAccess.DeleteTimesheetData(trdm.Id);
                //    }

                //    continue;
                //}

                TimesheetRowModel trm = new TimesheetRowModel()
                {
                    Project = pm
                };

                SubProjectModel subpmnew = trm.SubProjects.Where(x => x.Id == spm.Id)?.First();

                trm.SelectedSubproject = subpmnew;

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
                Rowdata.Add(trm);
            }

            SumTable();
        }

        private void UpdateDates(DateTime currdate)
        {
            DisplayName = "";
            IsButtonEditable = false;
            Rowdata.Clear();
            DateTime firstdate;
            DateTime lastdate;
            if (currdate.Day > 16)
            {
                //second tier
                firstdate = new DateTime(currdate.Year, currdate.Month, 17);
                lastdate = new DateTime(currdate.Year, currdate.Month, DateTime.DaysInMonth(currdate.Year, currdate.Month));
            }
            else
            {
                //first tier
                firstdate = new DateTime(currdate.Year, currdate.Month, 1);
                lastdate = new DateTime(currdate.Year, currdate.Month, 16);
            }

            int diff = (lastdate - firstdate).Days;
            List<DateWrapper> dates = new List<DateWrapper>();
            int workdays = 0;

            for (int i = 0; i <= diff; i++)
            {
                DateTime dt = firstdate.AddDays(i);
                dates.Add(new DateWrapper(dt.Date));

                if (!(dt.DayOfWeek == DayOfWeek.Saturday) && !(dt.DayOfWeek == DayOfWeek.Sunday))
                {
                    workdays++;
                }
            }

            DateSummary = new ObservableCollection<DateWrapper>(dates);
            MonthYearString = $"{firstdate.ToString("MMMM")} {firstdate.Year}";
            DateString = $"[{firstdate.Day} - {lastdate.Day}]";
            BaseHours = workdays * 9;
            DateTimesheet = (int)long.Parse(firstdate.Date.ToString("yyyyMMdd"));
            DateTime enddate = DateSummary.Last().Value;
            int difference = (int)Math.Ceiling(Math.Max((enddate - DateTime.Now).TotalDays, 0));
        }

        private void LoadTimesSheetSubmissionData(DateTime currdate)
        {
            List<EmployeeModel> allemployees = LoadActiveEmployees();
            TotalHeader.Clear();
            TimesheetSubmissions.Clear();

            foreach (EmployeeModel em in allemployees)
            {
                TimesheetSubmissionDbModel tsdbm = SQLAccess.LoadTimeSheetSubmissionData(DateTimesheet, em.Id);

                if (tsdbm == null)
                {
                    //not submitted yet
                    if (em.IsActive)
                    {
                        TimesheetSubmissions.Add(TimesheetSubmissionModel.Didnotsubmityet(em));
                    }
                }
                else
                {
                    TimesheetSubmissions.Add(new TimesheetSubmissionModel(tsdbm, em));
                }

            }
            NumSubmittedEmployees = TimesheetSubmissions.Where(x => x.Missing == false).Count();
            NumApprovedEmployees = TimesheetSubmissions.Where(x => x.Approved).Count();
            NumTotalEmployees = TimesheetSubmissions.Count();
        }


        private List<EmployeeModel> LoadActiveEmployees()
        {
            List<EmployeeDbModel> dbemployees = SQLAccess.LoadEmployees();

            List<EmployeeModel> members = new List<EmployeeModel>();

            foreach (EmployeeDbModel emdb in dbemployees)
            {
                //add is active here
                EmployeeModel em = new EmployeeModel(emdb);
                members.Add(em);
            }

            return members;
        }

        //private void AddRowToCollection()
        //{
        //    Rowdata.Add(new TimesheetRowModel { Entries = AddNewBlankRow() });
        //    //CollectDates();
        //}

        //private async void RemoveRow(TimesheetRowModel trm)
        //{
        //    if (trm.Entries.Any(x => x.TimeEntry > 0))
        //    {
        //        AreYouSureView view = new AreYouSureView();
        //        AreYouSureVM aysvm = new AreYouSureVM();
        //        aysvm.TexttoDisplay = trm.Project.ProjectName + " [" + trm.Project.ProjectNumber.ToString() + "]";
        //        view.DataContext = aysvm;
        //        var result = await DialogHost.Show(view, "RootDialog");
        //        aysvm = view.DataContext as AreYouSureVM;

        //        if (!aysvm.Result)
        //        {
        //            return;
        //        }
        //    }

        //    Rowdata.Remove(trm);
        //}

        //private void ExportWorkReport()
        //{

        //}

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

                    //Last one
                    //if (i == numofentries-1)
                    //{
                    //    PercentComplete = (total / BaseHours) * 100;
                    //}
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

        /// <summary>
        /// Add blank row
        /// </summary>
        /// <returns></returns>
        //private ObservableCollection<TREntryModel> AddNewBlankRow()
        //{
        //    ObservableCollection<TREntryModel> newblank = new ObservableCollection<TREntryModel>();

        //    foreach (TREntryModel tr in BlankEntry)
        //    {
        //        newblank.Add(new TREntryModel() { Date = tr.Date, TimeEntry = tr.TimeEntry });
        //    }
        //    return newblank;
        //}

        /// <summary>
        /// Load Projects from DB
        /// </summary>
        //private void LoadProjects(bool submitted)
        //{
        //    List<ProjectDbModel> dbprojects = SQLAccess.LoadProjects();

        //    ObservableCollection<ProjectModel> members = new ObservableCollection<ProjectModel>();

        //    foreach (ProjectDbModel pdb in dbprojects)
        //    {
        //        ProjectModel pm = new ProjectModel(pdb);
        //        bool activetest = submitted ? true : pm.IsActive;
        //        if (pm.SubProjects.Count > 0 && activetest)
        //        {
        //            members.Add(pm);
        //        }
        //    }

        //    ProjectList = members;
        //}

        /// <summary>
        /// Button Press
        /// </summary>
        //private void PreviousTimesheet()
        //{
        //    LoadCurrentTimesheet(DateSummary.First().Value.AddDays(-1));
        //}

        /// <summary>
        /// Button Press
        /// </summary>
        //private void NextTimesheet()
        //{
        //    LoadCurrentTimesheet(DateSummary.Last().Value.AddDays(1));
        //}

        /// <summary>
        /// Button Press
        /// </summary>
        //private void CurrentTimesheet()
        //{
        //    LoadCurrentTimesheet(DateTime.Now);
        //}


        private void ReportTimesheet(bool approve)
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
                            EmployeeId = CurrentEmployee.Id,
                            SubProjectId = trm.SelectedSubproject.Id,
                            Date = (int)long.Parse(trentry.Date.ToString("yyyyMMdd")),
                            Submitted = 1,
                            Approved = Convert.ToInt32(approve),
                            TimeEntry = trentry.TimeEntry
                    };

                        SQLAccess.UpdateTimesheetData(dbmodel);
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
                    $"{Environment.NewLine} {MonthYearString} {DateString} {Environment.NewLine} {Environment.NewLine}" +
                    $"Thanks,{Environment.NewLine}" +
                    $"SOCE Portal Dev Team."

                };

                //SendEmailMessage(txt, SubmittedTimesheet.Employee, "SOCE Portal Timesheet Denied");
                
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

            LoadCurrentTimesheet(DateSummary.First().Value);

        }

        private async void SendEmailMessage(TextPart textbody, EmployeeModel employeetosendto, string subject)
        {
            SmtpClient client = new SmtpClient();

            client.Connect("smtp-mail.outlook.com", 587, false);
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            client.Authenticate("normnoe@shirkodonovan.com", "Barry553");
            MimeMessage mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress("Norm", "normnoe@shirkodonovan.com"));
            mailMessage.To.Add(new MailboxAddress(employeetosendto.FirstName, employeetosendto.Email));
            mailMessage.Subject = subject;
            mailMessage.Body = textbody;
            

            await client.SendAsync(mailMessage);
            client.Disconnect(true);
        }
    }
}
