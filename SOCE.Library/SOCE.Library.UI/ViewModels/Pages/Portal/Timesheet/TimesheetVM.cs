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
using System.Threading.Tasks;
using SOCE.Library.Excel;
using System.Diagnostics;

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

        //public List<RegisteredTimesheetDataModel> TimesheetData;
        public ICommand AddRowCommand { get; set; }
        public ICommand WorkReportCommand { get; set; }
        public ICommand SubmitTimeSheetCommand { get; set; }
        public ICommand SaveTimesheetCommand { get; set; }
        public ICommand RemoveRowCommand { get; set; }

        public ICommand PreviousCommand { get; set; }

        public ICommand NextCommand { get; set; }
        public ICommand CurrentCommand { get; set; }

        public ICommand CopyPreviousCommand { get; set; }

        public ICommand ExportToExcel { get; set; }
        public List<TimesheetRowModel> CopiedTimesheetData { get; set; } = new List<TimesheetRowModel>();

        private ObservableCollection<TimesheetRowModel> _rowdata = new ObservableCollection<TimesheetRowModel>();
        public ObservableCollection<TimesheetRowModel> Rowdata
        {
            get { return _rowdata; }
            set
            {
                _rowdata = value;
                //SumTable();
                //CollectDates();

                if (Rowdata.Count == 0)
                {

                }
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

        private bool _canPressButton = false;
        public bool CanPressButton
        {
            get { return _canPressButton; }
            set
            {
                _canPressButton = value;
                RaisePropertyChanged(nameof(CanPressButton));
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

        private bool _searchFilter = false;
        public bool SearchFilter
        {
            get { return _searchFilter; }
            set
            {
                _searchFilter = value;

                //if (_searchFilter)
                //{
                foreach (ProjectModel pm in ProjectList)
                {
                    pm.SearchText = _searchFilter ? pm.ProjectName : pm.ProjectNumber.ToString();
                }
                //}
                RaisePropertyChanged(nameof(SearchFilter));
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

        private int _dueDateValue = 0;
        public int DueDateValue
        {
            get { return _dueDateValue; }
            set
            {
                _dueDateValue = value;
                RaisePropertyChanged(nameof(DueDateValue));
            }
        }

        private double _percentComplete = 0;
        public double PercentComplete
        {
            get { return _percentComplete; }
            set
            {
                _percentComplete = value;
                RaisePropertyChanged(nameof(PercentComplete));
            }
        }

        private double _expectedProgress = 0;
        public double ExpectedProgress
        {
            get { return _expectedProgress; }
            set
            {
                _expectedProgress = value;
                RaisePropertyChanged(nameof(ExpectedProgress));
            }
        }

        private ObservableCollection<TREntryModel> BlankEntry = new ObservableCollection<TREntryModel>();

        public TimesheetVM(EmployeeModel loggedinEmployee)
        {
            Constructor(loggedinEmployee);
            LoadCurrentTimesheet(DateTime.Now);
            SumTable();
        }

        public TimesheetVM(EmployeeModel loggedinEmployee, DateTime date)
        {
            Constructor(loggedinEmployee);
            LoadCurrentTimesheet(date);
            SumTable();
        }

        private void Constructor(EmployeeModel loggedinEmployee)
        {
            CurrentEmployee = loggedinEmployee;
            Rowdata.CollectionChanged += RowDataChanged;
            this.AddRowCommand = new RelayCommand(AddRowToCollection);
            this.SubmitTimeSheetCommand = new RelayCommand(SubmitTimesheet);
            this.RemoveRowCommand = new RelayCommand<TimesheetRowModel>(RemoveRow);
            this.SaveTimesheetCommand = new RelayCommand<int>(SaveCommand);

            this.PreviousCommand = new RelayCommand(PreviousTimesheet);
            this.NextCommand = new RelayCommand(NextTimesheet);
            this.CurrentCommand = new RelayCommand(CurrentTimesheet);
            this.CopyPreviousCommand = new RelayCommand(CopyPrevious);
            this.ExportToExcel = new RelayCommand(ExportCurrentTimesheetToExcel);
            
        }

        private void Rowdata_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void AddRowToCollection()
        {
            Rowdata.Add(new TimesheetRowModel { Entries = AddNewBlankRow() });
            //CollectDates();
        }

        private async void RemoveRow(TimesheetRowModel trm)
        {
            if (trm.Entries.Any(x => x.TimeEntry > 0))
            {
                AreYouSureView view = new AreYouSureView();
                AreYouSureVM aysvm = new AreYouSureVM();
                aysvm.TexttoDisplay = trm.Project.ProjectName + " [" + trm.Project.ProjectNumber.ToString() + "]";
                view.DataContext = aysvm;
                var result = await DialogHost.Show(view, "RootDialog");
                aysvm = view.DataContext as AreYouSureVM;

                if (!aysvm.Result)
                {
                    return;
                }
            }

            Rowdata.Remove(trm);
        }


        private void ExportCurrentTimesheetToExcel()
        {
            //do stuff
            //save down to downloads

            string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string pathDownload = Path.Combine(pathUser, "Downloads\\TimeSheet.xlsx");
            File.WriteAllBytes(pathDownload, Properties.Resources.TimesheetBase);
            Excel.Excel exinst = new Excel.Excel(pathDownload, AppEnum.Existing);

            foreach(TimesheetRowModel trm in Rowdata)
            {

            }
            //exinst.InsertRow(6, new List<string>() { "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1" });
            //exinst.InsertRow(7, new List<string>() { "2", "2", "2", "2", "2", "2", "2", "2", "2", "2", "2", "2", "2", "2", "2", "2" });
            exinst.Dispose();
            Process.Start(pathDownload);

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

                    //Last one
                    //if (i == numofentries-1)
                    //{
                    //    PercentComplete = (total / BaseHours) * 100;
                    //}
                }
                Total = TotalHeader.Sum(x => x.Value);
                PercentComplete = Math.Min(Math.Round(Total / BaseHours * 100, 2), 100);
            }
            else
            {
                //make 0s
                TotalHeader.Clear();
                foreach (DateWrapper date in DateSummary)
                {
                    TotalHeader.Add(new DoubleWrapper(0));
                }

                PercentComplete = 0;
                Total = 0;
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
        private void LoadProjects(bool submitted)
        {
            List<ProjectDbModel> dbprojects = SQLAccess.LoadProjects();

            ObservableCollection<ProjectModel> members = new ObservableCollection<ProjectModel>();

            ProjectModel[] ProjectArray = new ProjectModel[dbprojects.Count];

            //Do not include the last layer
            Parallel.For(0, dbprojects.Count, i =>
            {
                ProjectDbModel pdb = dbprojects[i];
                ProjectModel pm = new ProjectModel(pdb);
                bool activetest = submitted ? true : pm.IsActive;

                //pm.LoadSubProjects();

                //if (pm.SubProjects.Count > 0 && activetest)
                //{

                if (activetest)
                {
                    ProjectArray[i] = pm;
                }
                //members.Add(pm);
                //}
            }
            );

            ProjectArray = ProjectArray.Where(x => x != null).OrderBy(x=>x.ProjectNumber).ToArray();

            ProjectList = new ObservableCollection<ProjectModel>(ProjectArray.ToList());



            //foreach (ProjectDbModel pdb in dbprojects)
            //{
            //    ProjectModel pm = new ProjectModel(pdb);
            //pm.LoadSubProjects();
            //bool activetest = submitted ? true : pm.IsActive;
            //if (pm.SubProjects.Count > 0 && activetest)
            //{
            //    members.Add(pm);
            //}
            //}

            //ProjectList = members;
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

        private bool LoadTimesheetSubmissionData()
        {
            TimesheetSubmissionDbModel tsdbm = SQLAccess.LoadTimeSheetSubmissionData(DateTimesheet, CurrentEmployee.Id);

            IsSubEditable = (tsdbm == null) ? true : false;

            Icon = (tsdbm == null) ? MaterialDesignThemes.Wpf.PackIconKind.DotsHorizontalCircleOutline : MaterialDesignThemes.Wpf.PackIconKind.CheckCircleOutline;
            Iconcolor = (tsdbm == null) ? Brushes.SlateBlue : Brushes.Green;
            return !IsSubEditable;
        }

        /// <summary>
        /// Load DB
        /// </summary>
        private void LoadTimesheetData()
        {
            CopiedTimesheetData.Clear();
            Rowdata.Clear();
            DateTime datestart = DateSummary.First().Value;
            DateTime dateend = DateSummary.Last().Value;

            //update employee Id
            List<TimesheetRowDbModel> dbtimesheetdata = SQLAccess.LoadTimeSheet(datestart, dateend, CurrentEmployee.Id);

            var groupedlist = dbtimesheetdata.OrderBy(x => x.SubProjectId).GroupBy(x => x.SubProjectId).ToList();

            List<TimesheetRowModel> trms = new List<TimesheetRowModel>();

            foreach (var item in groupedlist)
            {
                TimesheetRowDbModel subitem = item.First();
                SubProjectDbModel spdb = SQLAccess.LoadSubProjectsBySubProject(subitem.SubProjectId);
                ProjectDbModel pdb = SQLAccess.LoadProjectsById(spdb.ProjectId);

                ProjectModel pm = new ProjectModel(pdb);
                SubProjectModel spm = new SubProjectModel(spdb);

                ProjectModel pmnew = ProjectList.Where(x => x.Id == pm.Id)?.FirstOrDefault();

                if (pmnew == null)
                {
                    //are you dumb?
                    foreach (TimesheetRowDbModel trdm in item)
                    {
                        SQLAccess.DeleteTimesheetData(trdm.Id);
                    }

                    continue;
                }

                TimesheetRowModel trm = new TimesheetRowModel()
                {
                    Project = pmnew
                };

                SubProjectModel subpmnew;

                try
                {
                    subpmnew = trm.SubProjects.Where(x => x.Id == spm.Id)?.First();
                }
                catch
                {
                    foreach (TimesheetRowDbModel trdm in item)
                    {
                        SQLAccess.DeleteTimesheetData(trdm.Id);
                    }

                    continue;
                }

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
                trms.Add(trm);
            }

            List<TimesheetRowModel> trmadjusted = trms?.OrderBy(x => x.Project.ProjectNumber).ToList();

            foreach (TimesheetRowModel trm in trmadjusted)
            {
                Rowdata.Add(trm);
                CopiedTimesheetData.Add((TimesheetRowModel)trm.Clone());
            }
            SumTable();
        }

        private async void SubmitTimesheet()
        {
            AreYouSureView view = new AreYouSureView();
            AreYouSureVM aysvm = new AreYouSureVM();
            aysvm.TexttoDisplay = "submit timesheet?";
            aysvm.WordNeeded = "";
            view.DataContext = aysvm;
            var result = await DialogHost.Show(view, "RootDialog");
            aysvm = view.DataContext as AreYouSureVM;

            if (aysvm.Result)
            {
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
                ot = Math.Max(sum - BaseHours, 0);

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
        }

        /// <summary>
        /// Save to DB
        /// </summary>
        private void SaveCommand(int submit)
        {

            //adding and modifying
            foreach (TimesheetRowModel trm in Rowdata)
            {
                //adding or modifying an existing submission
                foreach (TREntryModel trentry in trm.Entries)
                {
                    if (trentry.TimeEntry > 0 && trm.SelectedSubproject != null)
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

                //RatesPerProjectDbModel rpp = new RatesPerProjectDbModel()
                //{
                //    ProjectId = trm.Project.Id,
                //    EmployeeId = CurrentEmployee.Id,
                //    Rate = CurrentEmployee.Rate
                //};

                //SQLAccess.AddRatesPerProject(rpp);
            }

            //deleting
            foreach (TimesheetRowModel ctrm in CopiedTimesheetData)
            {
                if (ctrm.SelectedSubproject != null)
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
                                    TimesheetRowDbModel trdbm = SQLAccess.LoadTimeSheetData(CurrentEmployee.Id, ctrm.SelectedSubproject.Id, trentry.Date);
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
                                TimesheetRowDbModel trdbm = SQLAccess.LoadTimeSheetData(CurrentEmployee.Id, ctrm.SelectedSubproject.Id, trentry.Date);
                                SQLAccess.DeleteTimesheetData(trdbm.Id);
                            }

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
            bool issubmitted = LoadTimesheetSubmissionData();
            LoadProjects(issubmitted);
            LoadTimesheetData();

        }

        /// <summary>
        /// Load Date of Timesheet
        /// </summary>
        /// <param name="currdate"></param>
        private async void CopyPrevious()
        {
            if (Rowdata.Count==0)
            {
                AreYouSureView view = new AreYouSureView();
                AreYouSureVM aysvm = new AreYouSureVM();
                aysvm.TexttoDisplay = "copy the previous timesheet?";
                aysvm.WordNeeded = "";
                view.DataContext = aysvm;
                var result = await DialogHost.Show(view, "RootDialog");
                aysvm = view.DataContext as AreYouSureVM;

                if (aysvm.Result)
                {

                    DateTime currdate = DateSummary.First().Value.AddDays(-1);
                    //UpdateDateSummary(currdate);
                    bool issubmitted = LoadTimesheetSubmissionData();
                    LoadProjects(issubmitted);
                    LoadTimesheetDataforCopyPrevious();
                }
            }
            else
            {
                //Message sorry fam
            }
            
        }

        private void LoadTimesheetDataforCopyPrevious()
        {
            //LoadCurrentTimesheet();

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

            //ObservableCollection<TimesheetRowModel> members = new ObservableCollection<TimesheetRowModel>();

            var groupedlist = dbtimesheetdata.OrderBy(x => x.SubProjectId).GroupBy(x => x.SubProjectId).ToList();

            foreach (var item in groupedlist)
            {
                TimesheetRowDbModel subitem = item.First();
                SubProjectDbModel spdb = SQLAccess.LoadSubProjectsBySubProject(subitem.SubProjectId);
                ProjectDbModel pdb = SQLAccess.LoadProjectsById(spdb.ProjectId);

                try
                {
                    ProjectModel pm = new ProjectModel(pdb);
                    SubProjectModel spm = new SubProjectModel(spdb);

                    ProjectModel pmnew = ProjectList.Where(x => x.Id == pm.Id)?.First();
                    TimesheetRowModel trm = new TimesheetRowModel()
                    {
                        Project = pmnew
                    };

                    SubProjectModel subpmnew = trm.SubProjects.Where(x => x.Id == spm.Id)?.First();

                    trm.SelectedSubproject = subpmnew;

                    ObservableCollection<TREntryModel> blanks = new ObservableCollection<TREntryModel>();

                    foreach (TREntryModel trentry in BlankEntry)
                    {
                        blanks.Add((TREntryModel)trentry.Clone());
                    }

                    trm.Entries = blanks;

                    Rowdata.Add(trm);
                }
                catch
                {
                    continue;
                }    
            }

            //Rowdata = members;

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
            DateTime enddate = DateSummary.Last().Value;
            int difference = (int)Math.Ceiling(Math.Max((enddate - DateTime.Now).TotalDays, 0));
            DueDateValue = difference;
            ExpectedProgress = 100 - Math.Min(Math.Round((Convert.ToDouble(DueDateValue) / Convert.ToDouble(diff)) * 100, 2), 100);

        }

        private void RowDataChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged added in e?.NewItems)
                {
                    TimesheetRowModel trm = (TimesheetRowModel)added;

                    foreach (TREntryModel trentry in trm.Entries)
                    {
                        trentry.PropertyChanged += ItemModificationOnPropertyChanged;
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged added in e?.OldItems)
                {
                    TimesheetRowModel trm = (TimesheetRowModel)added;

                    foreach (TREntryModel trentry in trm.Entries)
                    {
                        trentry.PropertyChanged -= ItemModificationOnPropertyChanged;
                    }
                }
            }

            CanPressButton = Rowdata.Count == 0 ? true : false;
            
        }

        private void ItemModificationOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            SumTable();

            foreach (TimesheetRowModel trm in Rowdata)
            {
                trm.Total = trm.Entries.Sum(x => x.TimeEntry);
            }
            AutoSave();
        }

        bool currentlyqued = false;

        private async Task AutoSave()
        {
            if (!currentlyqued)
            {
                currentlyqued = true;
                await Task.Delay(60000);
                SaveCommand(0);
                currentlyqued = false;
            }
        }
    }
}
