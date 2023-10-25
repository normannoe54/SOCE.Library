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

        private TimesheetRowModel _selectedRow;
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

        private string _message = "";
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                RaisePropertyChanged(nameof(Message));
            }
        }

        private string _inactiveMessage = "";
        public string InactiveMessage
        {
            get { return _inactiveMessage; }
            set
            {
                _inactiveMessage = value;
                RaisePropertyChanged(nameof(InactiveMessage));
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

        private bool _messageVisible = false;
        public bool MessageVisible
        {
            get { return _messageVisible; }
            set
            {
                _messageVisible = value;
                RaisePropertyChanged(nameof(MessageVisible));
            }
        }

        private ObservableCollection<TREntryModel> BlankEntry = new ObservableCollection<TREntryModel>();

        public TimesheetVM(EmployeeModel loggedinEmployee)
        {
            Constructor(loggedinEmployee);
            LoadCurrentTimesheet(DateTime.Now);
            SumTable();
            SearchFilter = false;

        }

        public TimesheetVM(EmployeeModel loggedinEmployee, DateTime date)
        {
            Constructor(loggedinEmployee);
            LoadCurrentTimesheet(date);
            SumTable();
            SearchFilter = false;

        }

        private void Constructor(EmployeeModel loggedinEmployee)
        {
            CurrentEmployee = loggedinEmployee;
            Rowdata.CollectionChanged += RowDataChanged;
            this.AddRowCommand = new RelayCommand(AddRowToCollection);
            this.SubmitTimeSheetCommand = new RelayCommand(SubmitTimesheet);
            this.RemoveRowCommand = new RelayCommand<TimesheetRowModel>(RemoveRow);
            this.SaveTimesheetCommand = new AsyncRelayCommand<int>(SaveCommand);

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
            Rowdata.Add(new TimesheetRowModel(ProjectList.ToList()) { Entries = AddNewBlankRow() });
            //CollectDates();
        }

        private async void RemoveRow(TimesheetRowModel trm)
        {
            if (trm.Entries.Any(x => x.TimeEntry > 0))
            {
                YesNoView view = new YesNoView();
                YesNoVM aysvm = new YesNoVM();
                aysvm.Message = "Are you sure you want to delete";
                aysvm.SubMessage = trm.Project.ProjectName + " [" + trm.Project.ProjectNumber.ToString() + "]";
                view.DataContext = aysvm;
                var result = await DialogHost.Show(view, "RootDialog");
                aysvm = view.DataContext as YesNoVM;

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

                    string cell = $"{MonthYearString} {DateString}";
                    exinst.WriteCell(1, 4, cell);

                    string name = $"{CurrentEmployee.FullName}";
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
            for (int i = 0; i < dbprojects.Count; i++)
            {
                ProjectDbModel pdb = dbprojects[i];
                ProjectModel pm = new ProjectModel(pdb);


                bool activetest = submitted ? true : pm.IsActive;
                if (activetest)
                {
                    ProjectArray[i] = pm;
                }

                //pm.LoadSubProjects();

                //if (pm.SubProjects.Count > 0 && activetest)
                //{
            }

            //Parallel.For(0, dbprojects.Count, i =>
            //{
            //    ProjectDbModel pdb = dbprojects[i];
            //    ProjectModel pm = new ProjectModel(pdb);
            //    bool activetest = submitted ? true : pm.IsActive;

            //    //pm.LoadSubProjects();

            //    //if (pm.SubProjects.Count > 0 && activetest)
            //    //{

            //    if (activetest)
            //    {
            //        ProjectArray[i] = pm;
            //    }
            //    //members.Add(pm);
            //    //}
            //}
            //);

            ProjectArray = ProjectArray.Where(x => x != null).OrderByDescending(x => x.ProjectNumber).ToArray();
            ProjectList = new ObservableCollection<ProjectModel>(ProjectArray.ToList());
            SearchFilter = SearchFilter;
            //ProjectList = new ObservableCollection<ProjectModel>(ProjectArray.ToList().OrderByDescending(x=>x.ProjectNumber));



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
            CopiedTimesheetData = null;
            Rowdata = null;
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
                TimesheetRowModel trm = new TimesheetRowModel(ProjectList.ToList());
                ProjectDbModel pdb = null;
                ProjectModel pm = null;
                SubProjectModel spm = null;
                ProjectModel pmnew = null;

                //doesnt exist in database
                if (spdb == null)
                {
                    trm.AlertStatus = TimesheetRowAlertStatus.Deleted;
                }
                else
                {
                    pmnew = ProjectList.Where(x => x.Id == spdb.ProjectId).FirstOrDefault();

                    if (pmnew == null)
                    {
                        pdb = SQLAccess.LoadProjectsById(spdb.ProjectId);
                        pm = new ProjectModel(pdb);
                        trm.AlertStatus = TimesheetRowAlertStatus.Inactive;
                        List<ProjectModel> pmlist = ProjectList.ToList();
                        pmlist.Add(pm);
                        //trm.BaseProjectList = new ObservableCollection<ProjectModel>(pmlist);
                        trm.ProjectList = new ObservableCollection<ProjectModel>(pmlist);
                        //ProjectList.Add(pm);
                        pmnew = pm;
                    }
                    //else
                    //{

                    //}

                    spm = new SubProjectModel(spdb);
                    //pmnew = ProjectList.Where(x => x.Id == pmnew.Id)?.FirstOrDefault();


                    //if (pmnew == null)
                    //{
                    //    //are you dumb?
                    //    foreach (TimesheetRowDbModel trdm in item)
                    //    {
                    //        SQLAccess.DeleteTimesheetData(trdm.Id);
                    //    }

                    //    continue;
                    //}

                    trm.Project = pmnew;

                    SubProjectModel subpmnew;

                    try
                    {
                        subpmnew = trm.SubProjects.Where(x => x.Id == spm.Id)?.First();
                        trm.AlertStatus = TimesheetRowAlertStatus.Active;
                    }
                    catch
                    {
                        trm.AlertStatus = TimesheetRowAlertStatus.Inactive;
                        trm.SubProjects.Add(spm);
                        subpmnew = spm;
                        //foreach (TimesheetRowDbModel trdm in item)
                        //{
                        //    SQLAccess.DeleteTimesheetData(trdm.Id);
                        //}

                        //continue;
                    }

                    trm.SelectedSubproject = subpmnew;

                    item.ToList().RemoveAll(x => x == null);

                    List<TimesheetRowDbModel> distincts = item.ToList().GroupBy(x => x.Date).Select(y => y.First()).ToList();
                    var duplicates = item.Except(distincts).ToList();

                    foreach (TimesheetRowDbModel trdm in duplicates)
                    {
                        SQLAccess.DeleteTimesheetData(trdm.Id);
                    }

                    foreach (TimesheetRowDbModel trdm in distincts)
                    {
                        DateTime dt = DateTime.ParseExact(trdm.Date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                        trm.Entries.Add(new TREntryModel() { Date = dt, TimeEntry = trdm.TimeEntry, ReadOnly = false });
                    }

                    DateTime dateinc = datestart;

                    while (dateinc <= dateend)
                    {
                        if (!trm.Entries.Any(x => x.Date == dateinc))
                        {
                            //add
                            trm.Entries.Add(new TREntryModel() { Date = dateinc, TimeEntry = 0, ReadOnly = false });
                        }
                        dateinc = dateinc.AddDays(1);
                    }
                    trm.Entries = new ObservableCollection<TREntryModel>(trm.Entries.OrderBy(x => x.Date).ToList());
                    trms.Add(trm);
                    //}
                }
            }

            List<TimesheetRowModel> trmadjusted = trms?.OrderByDescending(x => x.Project.ProjectNumber).ToList();

            List<TimesheetRowModel> rowlist = new List<TimesheetRowModel>();
            List<TimesheetRowModel> rowlistcopied = new List<TimesheetRowModel>();

            foreach (TimesheetRowModel trm in trmadjusted)
            {
                rowlist.Add(trm);
                rowlistcopied.Add((TimesheetRowModel)trm.Clone());
                //CopiedTimesheetData.Add((TimesheetRowModel)trm.Clone());
            }

            Rowdata = new ObservableCollection<TimesheetRowModel>(rowlist);
            CopiedTimesheetData = new List<TimesheetRowModel>(rowlistcopied);

            SumTable();
        }

        private async void SubmitTimesheet()
        {
            YesNoView view = new YesNoView();
            YesNoVM aysvm = new YesNoVM();
            aysvm.Message = "Submit Time Sheet?";
            view.DataContext = aysvm;
            var result = await DialogHost.Show(view, "RootDialog");
            aysvm = view.DataContext as YesNoVM;

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

                            if (trm.Project.ProjectName.ToUpper() == "VACATION")
                            {
                                pto += trentry.TimeEntry;
                            }
                            else if (trm.Project.ProjectName.ToUpper() == "SICK")
                            {
                                sick += trentry.TimeEntry;
                            }
                            else if (trm.Project.ProjectName.ToUpper() == "HOLIDAY")
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
            DateTime starttime = DateTime.Now;
            //Message = "Timesheet Saved";

            MessageVisible = true;
            //adding and modifying

            //bool isallactive = !Rowdata.Any(x => x.AlertStatus != TimesheetRowAlertStatus.Active);

            //bool result = true;

            //if (!isallactive)
            //{
            //    YesNoView view = new YesNoView();
            //    YesNoVM aysvm = new YesNoVM();
            //    aysvm.Message = "One or more projects highlighted in red are not active.";
            //    aysvm.SubMessage = "Do you still want to save?";
            //    view.DataContext = aysvm;
            //    var dialogres = await DialogHost.Show(view, "RootDialog");
            //    aysvm = view.DataContext as YesNoVM;
            //    result = aysvm.Result;
            //}

            //if (result)
            //{
            bool contains;

            try
            {
                //deleting
                foreach (TimesheetRowModel ctrm in CopiedTimesheetData)
                {
                    if (ctrm.SelectedSubproject != null)
                    {
                        int index = Rowdata.ToList().FindIndex(x => x.SelectedSubproject?.Id == ctrm.SelectedSubproject.Id);

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

                                        if (trdbm != null)
                                        {
                                            SQLAccess.DeleteTimesheetData(trdbm.Id);
                                        }
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

                                    if (trdbm != null)
                                    {
                                        SQLAccess.DeleteTimesheetData(trdbm.Id);
                                    }
                                }

                            }
                        }
                    }

                }



                foreach (TimesheetRowModel trm in Rowdata)
                {

                    double timepersub = 0;
                    //adding or modifying an existing submission
                    foreach (TREntryModel trentry in trm.Entries)
                    {
                        if (trentry.TimeEntry > 0 && trm.SelectedSubproject != null)
                        {
                            timepersub += trentry.TimeEntry;
                            TimesheetRowDbModel dbmodel = new TimesheetRowDbModel()
                            {
                                EmployeeId = CurrentEmployee.Id,
                                SubProjectId = trm.SelectedSubproject.Id,
                                Date = (int)long.Parse(trentry.Date.ToString("yyyyMMdd")),
                                Submitted = submit,
                                Approved = 0,
                                TimeEntry = trentry.TimeEntry,
                                BudgetSpent = CurrentEmployee.Rate * trentry.TimeEntry
                            };

                            SQLAccess.AddTimesheetData(dbmodel);
                            //get data that needs to be removed
                        }
                    }



                    //try
                    //{
                    //    RolePerSubProjectDbModel rpp = new RolePerSubProjectDbModel()
                    //    {
                    //        SubProjectId = trm.SelectedSubproject.Id,
                    //        EmployeeId = CurrentEmployee.Id,
                    //        Role = (int)CurrentEmployee.DefaultRole,
                    //        Rate = CurrentEmployee.Rate,
                    //        BudgetHours = 0
                    //    };
                    //    SQLAccess.AddRolesPerSubProject(rpp);

                    //}
                    //catch
                    //{
                    //}

                    //ids++;
                }



                CopiedTimesheetData.Clear();
                foreach (TimesheetRowModel trm in Rowdata)
                {
                    CopiedTimesheetData.Add((TimesheetRowModel)trm.Clone());
                }

                double diffInSeconds = 0;
                do
                {
                    diffInSeconds = (DateTime.Now - starttime).TotalSeconds;

                } while (diffInSeconds < 2);


                Message = "Timesheet Saved";
                MessageVisible = false;
            }
            catch
            {

                Message = "Something went wrong!";
                MessageVisible = false;
            }
            //}
            //Message = "";
        }


        /// <summary>
        /// Load Date of Timesheet
        /// </summary>
        /// <param name="currdate"></param>
        public void LoadCurrentTimesheet(DateTime currdate)
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
            if (Rowdata.Count == 0)
            {
                YesNoView view = new YesNoView();
                YesNoVM aysvm = new YesNoVM();
                aysvm.Message = "Copy the previous timesheet?";
                view.DataContext = aysvm;
                var result = await DialogHost.Show(view, "RootDialog");
                aysvm = view.DataContext as YesNoVM;

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
                    TimesheetRowModel trm = new TimesheetRowModel(ProjectList.ToList())
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
            double basehours = 0;
            for (int i = 0; i <= diff; i++)
            {
                DateTime dt = firstdate.AddDays(i);
                BlankEntry.Add(new TREntryModel { Date = dt });
                dates.Add(new DateWrapper(dt.Date));

                if (dt.DayOfWeek == DayOfWeek.Friday)
                {
                    basehours += CurrentEmployee.FridayHours;
                }
                else if (dt.DayOfWeek == DayOfWeek.Thursday)
                {
                    basehours += CurrentEmployee.ThursdayHours;
                }
                else if (dt.DayOfWeek == DayOfWeek.Wednesday)
                {
                    basehours += CurrentEmployee.WednesdayHours;
                }
                else if (dt.DayOfWeek == DayOfWeek.Tuesday)
                {
                    basehours += CurrentEmployee.TuesdayHours;
                }
                else if (dt.DayOfWeek == DayOfWeek.Monday)
                {
                    basehours += CurrentEmployee.MondayHours;
                }
            }

            DateSummary = new ObservableCollection<DateWrapper>(dates);
            MonthYearString = $"{firstdate.ToString("MMMM")} {firstdate.Year}";
            DateString = $"[{firstdate.Day} - {lastdate.Day}]";

            BaseHours = basehours;

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
                await Task.Delay(30000);

                await Task.Run(() =>
                {
                    SaveCommand(0);

                });

                currentlyqued = false;
            }
        }
    }
}
