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

namespace SOCE.Library.UI.ViewModels
{
    public class TimesheetVM : BaseVM
    {
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
            }
        }

        public List<RegisteredTimesheetDataModel> TimesheetData;
        public ICommand AddRowCommand { get; set; }
        public ICommand WorkReportCommand { get; set; }
        public ICommand SubmitTimeSheetCommand { get; set; }
        public ICommand SaveTimesheetCommand { get; set; }
        public ICommand RemoveRowCommand { get; set; }

        public ICommand PreviousCommand { get; set; }

        public ICommand NextCommand { get; set; }
        public ICommand CurrentCommand { get; set; }

        public ICommand CopyPreviousCommand { get; set; }
        public List<TimesheetRowModel> CopiedTimesheetData { get; set; } = new List<TimesheetRowModel>();

        private ObservableCollection<TimesheetRowModel> _rowdata = new ObservableCollection<TimesheetRowModel>();
        public ObservableCollection<TimesheetRowModel> Rowdata
        {
            get { return _rowdata; }
            set
            {
                _rowdata = value;
                SumTable();
                //CollectDates();
                RaisePropertyChanged(nameof(Rowdata));
            }
        }

        private ObservableCollection<ProjectModel> _projectList;
        public ObservableCollection<ProjectModel> ProjectList
        {
            get { return _projectList; }
            set
            {
                _projectList = value;
                RaisePropertyChanged(nameof(ProjectList));
            }
        }

        private bool _isSubEditable = false;
        public bool IsSubEditable
        {
            get { return _isSubEditable; }
            set
            {
                _isSubEditable = value;
                RaisePropertyChanged(nameof(IsSubEditable));
            }
        }

        private TimesheetRowModel _selectedRow = new TimesheetRowModel();
        public TimesheetRowModel SelectedRow
        {
            get { return _selectedRow; }
            set
            {
                _selectedRow = value;
                RaisePropertyChanged(nameof(SelectedRow));
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

        private PackIconKind _icon;
        public PackIconKind Icon
        {
            get { return _icon; }
            set
            {
                _icon = value;
                RaisePropertyChanged(nameof(Icon));
            }
        }


        private Brush _iconcolor;
        public Brush Iconcolor
        {
            get { return _iconcolor; }
            set
            {
                _iconcolor = value;
                RaisePropertyChanged(nameof(Iconcolor));
            }
        }

        private ObservableCollection<TREntryModel> BlankEntry = new ObservableCollection<TREntryModel>();

        public TimesheetVM(EmployeeModel loggedinEmployee)
        {
            CurrentEmployee = loggedinEmployee;
            //get timesheet data from database
            List<RegisteredTimesheetDataModel> rtdm = new List<RegisteredTimesheetDataModel>();
            LoadProjects();
            LoadCurrentTimesheet(DateTime.Now);
            //determine if it has been submitted

            //LoadTimesheetData();
            this.AddRowCommand = new RelayCommand(AddRowToCollection);
            this.SubmitTimeSheetCommand = new RelayCommand(SubmitTimesheet);
            this.RemoveRowCommand = new RelayCommand<TimesheetRowModel>(RemoveRow);
            this.SaveTimesheetCommand = new RelayCommand<int>(SaveCommand);

            this.PreviousCommand = new RelayCommand(PreviousTimesheet);
            this.NextCommand = new RelayCommand(NextTimesheet);
            this.CurrentCommand = new RelayCommand(CurrentTimesheet);
            this.CopyPreviousCommand = new RelayCommand(CopyPrevious);
            SumTable();
        }

        private void AddRowToCollection()
        {
            Rowdata.Add(new TimesheetRowModel { Entries = AddNewBlankRow() });
            //CollectDates();
        }

        private void RemoveRow(TimesheetRowModel trm)
        {
            Rowdata.Remove(trm);
        }

        private void ExportWorkReport()
        {

        }

        /// <summary>
        /// Sum Table
        /// </summary>
        private void SumTable()
        {
            if (Rowdata.Count > 1)
            {
                TotalHeader.Clear();
                int numofentries = Rowdata[0].Entries.Count();

                for (int i = 0; i < numofentries; i++)
                {
                    double total = 0;

                    foreach (TimesheetRowModel trm in Rowdata)
                    {
                        total = trm.Entries[i].TimeEntry;
                    }

                    TotalHeader.Add(new DoubleWrapper(total));
                }
            }
        }

        /// <summary>
        /// Add blank row
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<TREntryModel> AddNewBlankRow()
        {
            ObservableCollection<TREntryModel> newblank = new ObservableCollection<TREntryModel>();

            foreach (TREntryModel tr in BlankEntry)
            {
                newblank.Add(new TREntryModel() { Date = tr.Date, TimeEntry = tr.TimeEntry });
            }
            return newblank;
        }

        /// <summary>
        /// Load Projects from DB
        /// </summary>
        private void LoadProjects()
        {
            List<ProjectDbModel> dbprojects = SQLAccess.LoadProjects();

            ObservableCollection<ProjectModel> members = new ObservableCollection<ProjectModel>();

            foreach (ProjectDbModel pdb in dbprojects)
            {
                ProjectModel pm = new ProjectModel(pdb);
                if (pm.SubProjects.Count > 0)
                {
                    members.Add(pm);
                }
            }

            ProjectList = members;
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

        private void LoadTimesheetSubmissionData()
        {
            TimesheetSubmissionDbModel tsdbm = SQLAccess.LoadTimeSheetSubmissionData(DateTimesheet, CurrentEmployee.Id);


            IsSubEditable = (tsdbm == null) ? true : false;

            Icon = (tsdbm == null) ? MaterialDesignThemes.Wpf.PackIconKind.DotsHorizontalCircleOutline : MaterialDesignThemes.Wpf.PackIconKind.CheckCircleOutline;
            Iconcolor = (tsdbm == null) ? Brushes.SlateBlue : Brushes.Green;
        }

        /// <summary>
        /// Load DB
        /// </summary>
        private void LoadTimesheetData()
        {
            CopiedTimesheetData.Clear();
            DateTime datestart = DateSummary.First().Value;
            DateTime dateend = DateSummary.Last().Value;

            //update employee Id
            List<TimesheetRowDbModel> dbtimesheetdata = SQLAccess.LoadTimeSheet(datestart, dateend, CurrentEmployee.Id);

            ObservableCollection<TimesheetRowModel> members = new ObservableCollection<TimesheetRowModel>();

            var groupedlist = dbtimesheetdata.OrderBy(x => x.SubProjectId).GroupBy(x => x.SubProjectId).ToList();

            foreach (var item in groupedlist)
            {
                TimesheetRowDbModel subitem = item.First();
                SubProjectDbModel spdb = SQLAccess.LoadSubProjectsBySubProject(subitem.SubProjectId);
                ProjectDbModel pdb = SQLAccess.LoadProjectsById(spdb.ProjectId);

                ProjectModel pm = new ProjectModel(pdb);
                SubProjectModel spm = new SubProjectModel(spdb);

                ProjectModel pmnew = ProjectList.Where(x => x.Id == pm.Id)?.First();

                TimesheetRowModel trm = new TimesheetRowModel()
                {
                    Project = pmnew
                };

                SubProjectModel subpmnew = trm.SubProjects.Where(x => x.Id == spm.Id)?.First();

                trm.SelectedSubproject = subpmnew;

                foreach (TimesheetRowDbModel trdm in item)
                {
                    DateTime dt = DateTime.ParseExact(trdm.Date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                    trm.Entries.Add(new TREntryModel() { Date = dt, TimeEntry = trdm.TimeEntry });
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
                members.Add(trm);
            }

            Rowdata = members;

            foreach (TimesheetRowModel trm in Rowdata)
            {
                CopiedTimesheetData.Add((TimesheetRowModel)trm.Clone());
            }

        }

        private void SubmitTimesheet()
        {
            //create new blanktimesheet
            SaveCommand(1);
            double pto = 0;
            double ot = 0;
            double sick = 0;
            double holiday = 0;
            double sum = 0;
            foreach (TimesheetRowModel trm in Rowdata)
            {
                //adding or modifying an existing submission
                foreach (TREntryModel trentry in trm.Entries)
                {
                    if (trentry.TimeEntry > 0)
                    {
                        sum += trentry.TimeEntry;

                        if (trm.Project.ProjectName == "PTO")
                        {
                            pto += trentry.TimeEntry;
                        }
                        else if (trm.Project.ProjectName == "SICK")
                        {
                            sick += trentry.TimeEntry;
                        }
                        else if (trm.Project.ProjectName == "HOLIDAY")
                        {
                            holiday += trentry.TimeEntry;
                        }
                    }
                }
            }
            ot = Math.Max(sum - BaseHours,0);

            TimesheetSubmissionDbModel timesheetsubdbmodel = new TimesheetSubmissionDbModel()
            {
                EmployeeId = CurrentEmployee.Id,
                Date = DateTimesheet,
                TotalHours = sum,
                PTOHours = pto,
                OTHours = ot,
                SickHours = sick,
                HolidayHours = holiday,
                Approved = 0, //not approved yet
            };

            SQLAccess.AddTimesheetSubmissionData(timesheetsubdbmodel);
            LoadTimesheetSubmissionData();

        }

        /// <summary>
        /// Save to DB
        /// </summary>
        private void SaveCommand(int submit)
        {
            //need to include employee Id in here

            //adding and modifying
            foreach (TimesheetRowModel trm in Rowdata)
            {
                //adding or modifying an existing submission
                foreach (TREntryModel trentry in trm.Entries)
                {
                    if (trentry.TimeEntry > 0)
                    {
                        TimesheetRowDbModel dbmodel = new TimesheetRowDbModel()
                        {
                            EmployeeId = CurrentEmployee.Id,
                            SubProjectId = trm.SelectedSubproject.Id,
                            Date = (int)long.Parse(trentry.Date.ToString("yyyyMMdd")),
                            Submitted = submit,
                            Approved = 0,
                            TimeEntry = trentry.TimeEntry
                        };

                        SQLAccess.AddTimesheetData(dbmodel);
                        //get data that needs to be removed
                    }
                }
            }

            //deleting
            foreach (TimesheetRowModel ctrm in CopiedTimesheetData)
            {
                int index = Rowdata.ToList().FindIndex(x => x.SelectedSubproject.Id == ctrm.SelectedSubproject.Id);

                //it exists
                if (index != -1)
                {
                    TimesheetRowModel trmfound = Rowdata[index];

                    foreach (TREntryModel trentry in ctrm.Entries)
                    {
                        //think about this expression
                        if (!trmfound.Entries.Any(x => x.Date == trentry.Date && x.TimeEntry == trentry.TimeEntry))
                        {
                            if (trentry.TimeEntry > 0)
                            {
                                //delete
                                TimesheetRowDbModel trdbm = SQLAccess.LoadTimeSheetData(1, ctrm.SelectedSubproject.Id, trentry.Date);
                                SQLAccess.DeleteTimesheetData(trdbm.Id);
                            }
                        }
                    }

                }
                else
                {
                    //delete all
                    foreach (TREntryModel trentry in ctrm.Entries)
                    {
                        if (trentry.TimeEntry > 0)
                        {
                            TimesheetRowDbModel trdbm = SQLAccess.LoadTimeSheetData(1, ctrm.SelectedSubproject.Id, trentry.Date);
                            SQLAccess.DeleteTimesheetData(trdbm.Id);
                        }

                    }
                }
            }

            CopiedTimesheetData = Rowdata.ToList();
        }


        /// <summary>
        /// Load Date of Timesheet
        /// </summary>
        /// <param name="currdate"></param>
        private void LoadCurrentTimesheet(DateTime currdate)
        {
            UpdateDateSummary(currdate);
            LoadTimesheetSubmissionData();
            LoadTimesheetData();
            
        }

        /// <summary>
        /// Load Date of Timesheet
        /// </summary>
        /// <param name="currdate"></param>
        private void CopyPrevious()
        {
            DateTime currdate = DateSummary.First().Value.AddDays(-1);
            //UpdateDateSummary(currdate);
            LoadTimesheetSubmissionData();
            LoadTimesheetDataforCopyPrevious();
        }

        private void LoadTimesheetDataforCopyPrevious()
        {
            CopiedTimesheetData.Clear();
            Rowdata.Clear();
            DateTime currdate = DateSummary.First().Value.AddDays(-1);
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

            //update employee Id
            List<TimesheetRowDbModel> dbtimesheetdata = SQLAccess.LoadTimeSheet(firstdate, lastdate, CurrentEmployee.Id);

            ObservableCollection<TimesheetRowModel> members = new ObservableCollection<TimesheetRowModel>();

            var groupedlist = dbtimesheetdata.OrderBy(x => x.SubProjectId).GroupBy(x => x.SubProjectId).ToList();

            foreach (var item in groupedlist)
            {
                TimesheetRowDbModel subitem = item.First();
                SubProjectDbModel spdb = SQLAccess.LoadSubProjectsBySubProject(subitem.SubProjectId);
                ProjectDbModel pdb = SQLAccess.LoadProjectsById(spdb.ProjectId);

                ProjectModel pm = new ProjectModel(pdb);
                SubProjectModel spm = new SubProjectModel(spdb);

                ProjectModel pmnew = ProjectList.Where(x => x.Id == pm.Id)?.First();

                TimesheetRowModel trm = new TimesheetRowModel()
                {
                    Project = pmnew
                };

                SubProjectModel subpmnew = trm.SubProjects.Where(x => x.Id == spm.Id)?.First();

                trm.SelectedSubproject = subpmnew;

                trm.Entries = BlankEntry;
                //foreach (TimesheetRowDbModel trdm in item)
                //{
                //    DateTime dt = DateTime.ParseExact(trdm.Date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                //    trm.Entries.Add(new TREntryModel() { Date = dt, TimeEntry = 0 });
                //}

                //DateTime dateinc = datestart;

                //while (dateinc <= dateend)
                //{
                //    if (!trm.Entries.Any(x => x.Date == dateinc))
                //    {
                //        //add
                //        trm.Entries.Add(new TREntryModel() { Date = dateinc, TimeEntry = 0 });
                //    }
                //    dateinc = dateinc.AddDays(1);
                //}
                //trm.Entries = new ObservableCollection<TREntryModel>(trm.Entries.OrderBy(x => x.Date).ToList());
                members.Add(trm);
            }

            Rowdata = members;

            foreach (TimesheetRowModel trm in Rowdata)
            {
                CopiedTimesheetData.Add((TimesheetRowModel)trm.Clone());
            }

        }

        private void UpdateDateSummary(DateTime currdate)
        {
            BlankEntry.Clear();
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
                BlankEntry.Add(new TREntryModel { Date = dt });
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
        }
    }
}
