﻿using System;
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
using System.Windows;
using System.Windows.Threading;

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
        public ICommand OpenExpenseReport { get; set; }
        public List<TimesheetRowModel> CopiedTimesheetData { get; set; } = new List<TimesheetRowModel>();

        private AsyncObservableCollection<TimesheetRowModel> _rowdata = new AsyncObservableCollection<TimesheetRowModel>();
        public AsyncObservableCollection<TimesheetRowModel> Rowdata
        {
            get { return _rowdata; }
            set
            {
                _rowdata = value;
                RaisePropertyChanged(nameof(Rowdata));
            }
        }

        private AsyncObservableCollection<ProjectLowResModel> _projectList;
        public AsyncObservableCollection<ProjectLowResModel> ProjectList
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

        private AsyncObservableCollection<DoubleWrapper> _totalHeader = new AsyncObservableCollection<DoubleWrapper>();
        public AsyncObservableCollection<DoubleWrapper> TotalHeader
        {
            get { return _totalHeader; }
            set
            {
                _totalHeader = value;
                RaisePropertyChanged(nameof(TotalHeader));
            }
        }

        private AsyncObservableCollection<DateWrapper> _datesummary = new AsyncObservableCollection<DateWrapper>();
        public AsyncObservableCollection<DateWrapper> DateSummary
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



        private Brush _expensePresent;
        public Brush ExpensePresent
        {
            get { return _expensePresent; }
            set
            {
                _expensePresent = value;
                RaisePropertyChanged(nameof(ExpensePresent));
            }
        }

        private DateTime _dateSelected;
        public DateTime DateSelected
        {
            get { return _dateSelected; }
            set
            {
                _dateSelected = value;
                if (allowdatechange)
                {
                    SelectedTimesheet();
                }
                RaisePropertyChanged(nameof(DateSelected));
            }
        }

        private bool allowdatechange = true;

        private ObservableCollection<TREntryModel> BlankEntry = new ObservableCollection<TREntryModel>();

        public TimesheetVM(EmployeeModel loggedinEmployee)
        {
            Constructor(loggedinEmployee);
            LoadCurrentTimesheet(DateTime.Now);
            SumTable();
            //SearchFilter = false;

        }

        public TimesheetVM(EmployeeModel loggedinEmployee, DateTime date)
        {
            Constructor(loggedinEmployee);
            LoadCurrentTimesheet(date);
            SumTable();
            //SearchFilter = false;
        }

        private void Constructor(EmployeeModel loggedinEmployee)
        {
            CurrentEmployee = loggedinEmployee;
            //Rowdata.CollectionChanged += RowDataChanged;
            this.AddRowCommand = new RelayCommand(AddRowToCollection);
            this.SubmitTimeSheetCommand = new RelayCommand(SubmitTimesheet);
            this.RemoveRowCommand = new RelayCommand<TimesheetRowModel>(RemoveRow);
            this.SaveTimesheetCommand = new AsyncRelayCommand<int>(SaveCommand);

            this.PreviousCommand = new RelayCommand(PreviousTimesheet);
            this.NextCommand = new RelayCommand(NextTimesheet);
            this.CurrentCommand = new RelayCommand(CurrentTimesheet);
            this.ExportToExcel = new RelayCommand(ExportCurrentTimesheetToExcel);
            this.OpenExpenseReport = new RelayCommand(ExpenseReport);
        }


        private async void ExpenseReport()
        {
            if (!ButtonInAction)
            {
                return;
            }
            ButtonInAction = false;

            ExpenseReportView ynv = new ExpenseReportView();
            ExpenseReportVM ynvm = new ExpenseReportVM(CurrentEmployee, DateSummary.FirstOrDefault().Value, DateSummary.LastOrDefault().Value, IsSubEditable, ProjectList.ToList());
            ynv.DataContext = ynvm;

            var result2 = await DialogHost.Show(ynv, "RootDialog");

            LoadExpenseData();

            ButtonInAction = true;
        }

        private void Rowdata_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void AddRowToCollection()
        {
            if (!ButtonInAction)
            {
                return;
            }
            ButtonInAction = false;

            Rowdata.Add(new TimesheetRowModel(ProjectList.ToList()) { Entries = AddNewBlankRow() });

            ButtonInAction = true;
            //CollectDates();
        }

        private async void RemoveRow(TimesheetRowModel trm)
        {
            if (!ButtonInAction)
            {
                return;
            }
            ButtonInAction = false;

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
                    ButtonInAction = true;
                    return;
                }
            }
            Rowdata.Remove(trm);
            SumTable();
            ButtonInAction = true;

        }

        private async void ExportCurrentTimesheetToExcel()
        {
            if (!ButtonInAction)
            {
                return;
            }
            ButtonInAction = false;

            CoreAI CurrentPage = IoCCore.Application as CoreAI;
            CurrentPage.MakeBlurry();
            await Task.Run(() => Task.Delay(600));
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
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

                        List<TimesheetRowModel> exportedtime = Rowdata.ToList();

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

                    DateWrapper startdate = DateSummary.FirstOrDefault();
                    DateWrapper enddate = DateSummary.LastOrDefault();

                    if (startdate != null && enddate != null)
                    {
                        List<ExpenseReportDbModel> edb = SQLAccess.LoadExpenses(startdate.Value, enddate.Value, CurrentEmployee.Id);

                        if (edb.Count > 0)
                        {
                            exinst.SetActiveSheetByName("ExpenseReport");
                            string name = $"{CurrentEmployee.FullName}";
                            exinst.WriteCell(10, 3, name);
                            exinst.WriteCell(11, 6, startdate.Value.ToString("MM/dd/yyyy"));
                            exinst.WriteCell(12, 6, enddate.Value.ToString("MM/dd/yyyy"));

                            int row = 16;
                            bool mileagefound = true;

                            foreach (ExpenseReportDbModel rep in edb)
                            {
                                DateTime date = DateTime.ParseExact(rep.Date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                                string description = rep.Description ?? "";

                                ProjectDbModel project = SQLAccess.LoadProjectsById(rep.ProjectId);
                                List<object> values = new List<object>();
                                values.Add(date.ToString("MM/dd/yyyy"));
                                values.Add(project.ProjectNumber);
                                values.Add(description);

                                ExpenseEnum typeex = (ExpenseEnum)rep.TypeExpense;
                                values.Add(typeex.ToString());

                                if (rep.Mileage == 0)
                                {
                                    values.Add(" ");
                                    values.Add(rep.TotalCost);

                                }
                                else
                                {
                                    if (mileagefound)
                                    {
                                        exinst.WriteCell(10, 6, rep.MileageRate.ToString());
                                        mileagefound = false;
                                    }
                                    values.Add(rep.Mileage);
                                    values.Add(rep.MileageRate * rep.Mileage);
                                }


                                if (row == 16)
                                {
                                    exinst.WriteRow<object>(row, 1, values);
                                }
                                else
                                {
                                    exinst.InsertRowBelow(row - 1, values);
                                    //string formula1 = $"SUM(D{row} + E{row} + F{row} + G{row} + H{row} + J{row})";
                                    //exinst.WriteFormula(row, 11, formula1);
                                    //string formula2 = $"SUM(I{row}*K9)";
                                    //exinst.WriteFormula(row, 10, formula2);
                                }
                                row++;
                            }
                            string formulamileage = $"SUM(E16:E{row - 1})";
                            string formulatotal = $"SUM(F16:F{row - 1})";
                            exinst.WriteFormula(row, 5, formulamileage);
                            exinst.WriteFormula(row, 6, formulatotal);

                            exinst.SaveDocument();
                        }
                        else
                        {
                            exinst.DeleteWorksheet("ExpenseReport");
                            exinst.SaveDocument();
                        }
                    }
                    else
                    {
                        ExpensePresent = Brushes.Black;
                    }

                    Process.Start(pathDownload);
                }
                catch
                {
                }
            }));
            await Task.Run(() => Task.Delay(600));
            CurrentPage.MakeClear();
            ButtonInAction = true;
        }

        /// <summary>
        /// Sum Table
        /// </summary>
        public void SumTable()
        {
            if (Rowdata.Count > 0)
            {
                List<DoubleWrapper> dblwrapper = new List<DoubleWrapper>();
                //TotalHeader.Clear();
                int numofentries = Rowdata[0].Entries.Count();

                for (int i = 0; i < numofentries; i++)
                {
                    double total = 0;

                    foreach (TimesheetRowModel trm in Rowdata)
                    {
                        total += trm.Entries[i].TimeEntry;
                    }

                    dblwrapper.Add(new DoubleWrapper(total));
                }
                TotalHeader = new AsyncObservableCollection<DoubleWrapper>(dblwrapper);
                Total = TotalHeader.Sum(x => x.Value);
                PercentComplete = Math.Min(Math.Round(Total / BaseHours * 100, 2), 100);
            }
            else
            {
                //make 0s
                List<DoubleWrapper> dblwrapper = new List<DoubleWrapper>();

                //TotalHeader.Clear();
                foreach (DateWrapper date in DateSummary)
                {
                    dblwrapper.Add(new DoubleWrapper(0));
                    //TotalHeader.Add(new DoubleWrapper(0));
                }
                TotalHeader = new AsyncObservableCollection<DoubleWrapper>(dblwrapper);

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
                TREntryModel trnew = new TREntryModel() { Date = tr.Date, TimeEntry = tr.TimeEntry };
                trnew.timesheetvm = this;
                newblank.Add(trnew);
            }
            return newblank;
        }

        /// <summary>
        /// Load Projects from DB
        /// </summary>
        private void LoadProjects(bool submitted)
        {
            List<ProjectDbModel> dbprojects = SQLAccess.LoadProjects();

            ObservableCollection<ProjectLowResModel> members = new ObservableCollection<ProjectLowResModel>();

            ProjectLowResModel[] ProjectArray = new ProjectLowResModel[dbprojects.Count];

            //Do not include the last layer
            for (int i = 0; i < dbprojects.Count; i++)
            {
                ProjectDbModel pdb = dbprojects[i];
                ProjectLowResModel pm = new ProjectLowResModel(pdb);


                bool activetest = submitted ? true : pm.IsActive;
                if (activetest)
                {
                    ProjectArray[i] = pm;
                }
            }

            ProjectArray = ProjectArray.Where(x => x != null).OrderByDescending(x => x.ProjectNumber).ToArray();
            ProjectList = new AsyncObservableCollection<ProjectLowResModel>(ProjectArray.ToList());
        }


        /// <summary>
        /// Button Press
        /// </summary>
        private async void PreviousTimesheet()
        {
            if (!ButtonInAction)
            {
                return;
            }
            ButtonInAction = false;

            CoreAI CurrentPage = IoCCore.Application as CoreAI;
            CurrentPage.MakeBlurry();
            await Task.Run(() => Task.Delay(600));
            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                LoadCurrentTimesheet(DateSummary.First().Value.AddDays(-1));
            }));
            await Task.Run(() => Task.Delay(600));
            CurrentPage.MakeClear();
            ButtonInAction = true;
        }

        private async void SelectedTimesheet()
        {
            if (!ButtonInAction)
            {
                return;
            }
            ButtonInAction = false;

            CoreAI CurrentPage = IoCCore.Application as CoreAI;
            CurrentPage.MakeBlurry();
            await Task.Run(() => Task.Delay(600));
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => LoadCurrentTimesheet(DateSelected)));
            await Task.Run(() => Task.Delay(600));
            CurrentPage.MakeClear();
            ButtonInAction = true;
        }

        /// <summary>
        /// Button Press
        /// </summary>
        private async void NextTimesheet()
        {
            if (!ButtonInAction)
            {
                return;
            }
            ButtonInAction = false;

            CoreAI CurrentPage = IoCCore.Application as CoreAI;
            CurrentPage.MakeBlurry();
            await Task.Run(() => Task.Delay(600));
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => LoadCurrentTimesheet(DateSummary.Last().Value.AddDays(7))));
            await Task.Run(() => Task.Delay(600));
            CurrentPage.MakeClear();
            ButtonInAction = true;

        }

        /// <summary>
        /// Button Press
        /// </summary>
        private async void CurrentTimesheet()
        {
            if (!ButtonInAction)
            {
                return;
            }
            ButtonInAction = false;

            CoreAI CurrentPage = IoCCore.Application as CoreAI;
            CurrentPage.MakeBlurry();
            await Task.Run(() => Task.Delay(600));
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => LoadCurrentTimesheet(DateTime.Now)));
            await Task.Run(() => Task.Delay(600));
            CurrentPage.MakeClear();
            ButtonInAction = true;
        }

        private bool LoadTimesheetSubmissionData()
        {
            TimesheetSubmissionDbModel tsdbm = SQLAccess.LoadTimeSheetSubmissionData(DateTimesheet, CurrentEmployee.Id);

            IsSubEditable = (tsdbm == null) ? true : false;

            Icon = (tsdbm == null) ? MaterialDesignThemes.Wpf.PackIconKind.DotsHorizontalCircleOutline : MaterialDesignThemes.Wpf.PackIconKind.CheckCircleOutline;
            Iconcolor = (tsdbm == null) ? Brushes.SlateBlue : Brushes.Green;
            return !IsSubEditable;
        }

        public void LoadExpenseData()
        {
            DateWrapper startdate = DateSummary.FirstOrDefault();
            DateWrapper enddate = DateSummary.LastOrDefault();

            if (startdate != null && enddate != null)
            {
                List<ExpenseReportDbModel> edb = SQLAccess.LoadExpenses(startdate.Value, enddate.Value, CurrentEmployee.Id);
                ExpensePresent = (edb.Count == 0) ? Brushes.Black : Brushes.Green;
            }
            else
            {
                ExpensePresent = Brushes.Black;
            }
        }

        /// <summary>
        /// Load DB
        /// </summary>
        private void LoadTimesheetData()
        {
            //CopiedTimesheetData = new List<TimesheetRowModel>();
            //Rowdata = new ObservableCollection<TimesheetRowModel>();
            SQLAccess sqldb = new SQLAccess();
            sqldb.Open();
            DateTime datestart = DateSummary.First().Value;
            DateTime dateend = DateSummary.Last().Value;

            //update employee Id
            List<TimesheetRowDbModel> dbtimesheetdata = SQLAccess.LoadTimeSheet(datestart, dateend, CurrentEmployee.Id);

            var groupedlist = dbtimesheetdata.OrderBy(x => x.SubProjectId).GroupBy(x => x.SubProjectId).ToList();

            List<int> subids = new List<int>();
            foreach (var item in groupedlist)
            {
                TimesheetRowDbModel subitem = item.First();
                subids.Add(subitem.SubProjectId);
            }


            List<ProjectFormatResultDb> formatted = sqldb.LoadProjectSummaryByPhaseIds(subids);

            List<TimesheetRowModel> trms = new List<TimesheetRowModel>();

            foreach (var item in groupedlist)
            {
                TimesheetRowDbModel subitem = item.First();
                ProjectFormatResultDb founditem = formatted.Where(x => x.SelectedSubProject.Id == subitem.SubProjectId).FirstOrDefault();
                SubProjectDbModel spdb = founditem?.SelectedSubProject;

                //SubProjectDbModel spdb = SQLAccess.LoadSubProjectsBySubProject(subitem.SubProjectId);
                TimesheetRowModel trm = new TimesheetRowModel(ProjectList.ToList());
                ProjectDbModel pdb = null;
                ProjectLowResModel pm = null;
                SubProjectLowResModel spm = null;
                ProjectLowResModel pmnew = null;

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
                        //pdb = SQLAccess.LoadProjectsById(spdb.ProjectId);
                        pm = new ProjectLowResModel(founditem.Project);
                        trm.AlertStatus = TimesheetRowAlertStatus.Inactive;
                        //List<ProjectLowResModel> pmlist = ProjectList.ToList();
                        //pmlist.Add(pm);
                        //trm.BaseProjectList = new ObservableCollection<ProjectModel>(pmlist);
                        //trm.ProjectList = new ObservableCollection<ProjectLowResModel>(pmlist);
                        //pmnew = ProjectList.Where(x => x.Id == spdb.ProjectId).FirstOrDefault();
                        trm.ProjectList.Add(pm);

                        //ProjectList.Add(pm);
                        pmnew = pm;
                    }

                    spm = new SubProjectLowResModel(spdb);

                    trm.allowprojectchanges = false;
                    trm.Project = pmnew;
                    trm.allowprojectchanges = true;
                    trm.SubProjects = new ObservableCollection<SubProjectLowResModel>(TimesheetRowModel.FormatSubProjects(founditem.SubProjects, pmnew.IsActive));
                    SubProjectLowResModel subpmnew = trm.SubProjects.Where(x => x.Id == spm.Id)?.FirstOrDefault();

                    if (subpmnew == null)
                    {
                        trm.AlertStatus = TimesheetRowAlertStatus.Inactive;
                        trm.SubProjects.Add(spm);
                        subpmnew = spm;
                    }
                    else
                    {
                        trm.AlertStatus = TimesheetRowAlertStatus.Active;
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
                        TREntryModel entry = new TREntryModel() { Date = dt, TimeEntry = trdm.TimeEntry, ReadOnly = false };
                        entry.timesheetvm = this;
                        trm.Entries.Add(entry);
                    }

                    DateTime dateinc = datestart;

                    while (dateinc <= dateend)
                    {
                        if (!trm.Entries.Any(x => x.Date == dateinc))
                        {
                            //add
                            TREntryModel entry = new TREntryModel() { Date = dateinc, TimeEntry = 0, ReadOnly = false };
                            entry.timesheetvm = this;
                            trm.Entries.Add(entry);
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

            Rowdata = new AsyncObservableCollection<TimesheetRowModel>(rowlist);
            CopiedTimesheetData = new List<TimesheetRowModel>(rowlistcopied);
            sqldb.Close();
            SumTable();
        }

        private async void SubmitTimesheet()
        {
            if (!ButtonInAction)
            {
                return;
            }
            ButtonInAction = false;

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
                bool cancelpto = false;
                double holiday = 0;
                double sum = 0;

                List<TimesheetSubmissionDbModel> subsmissions = SQLAccess.LoadTimesheetSubmissionByEmployee(CurrentEmployee.Id);
                int numberofworkdays = 0;
                DateTime firstday = DateSummary.First().Value;

                if ((subsmissions.Count == 0 && firstday.Day == 17))
                {
                    cancelpto = true;
                }
                else if ((subsmissions.Count == 0 && firstday.Day == 1) || (subsmissions.Count == 1 && firstday.Day == 17))
                {
                    if (CurrentEmployee.MondayHours > 0)
                    {
                        numberofworkdays += DateSummary.Where(x => x.Value.DayOfWeek == DayOfWeek.Monday).Count();
                    }
                    if (CurrentEmployee.TuesdayHours > 0)
                    {
                        numberofworkdays += DateSummary.Where(x => x.Value.DayOfWeek == DayOfWeek.Tuesday).Count();
                    }
                    if (CurrentEmployee.WednesdayHours > 0)
                    {
                        numberofworkdays += DateSummary.Where(x => x.Value.DayOfWeek == DayOfWeek.Wednesday).Count();
                    }
                    if (CurrentEmployee.ThursdayHours > 0)
                    {
                        numberofworkdays += DateSummary.Where(x => x.Value.DayOfWeek == DayOfWeek.Thursday).Count();
                    }
                    if (CurrentEmployee.FridayHours > 0)
                    {
                        numberofworkdays += DateSummary.Where(x => x.Value.DayOfWeek == DayOfWeek.Friday).Count();
                    }

                    int countoftotal = TotalHeader.Where(x => x.Value != 0).Count();

                    cancelpto = numberofworkdays > countoftotal;
                }

                foreach (TimesheetRowModel trm in Rowdata)
                {
                    //adding or modifying an existing submission
                    foreach (TREntryModel trentry in trm.Entries)
                    {
                        if (trentry.TimeEntry > 0)
                        {
                            sum += trentry.TimeEntry;
                            if (trm.Project.ProjectName.ToUpper() == "MATERNITY PATERNITY LEAVE")
                            {
                                cancelpto = true;
                            }
                            if (trm.Project.ProjectName.ToUpper() == "PAID TIME OFF (PTO)")
                            {
                                pto += trentry.TimeEntry;
                            }
                            else if (trm.Project.ProjectName.ToUpper() == "HOLIDAY")
                            {
                                holiday += trentry.TimeEntry;
                            }
                        }
                    }
                }
                ot = Math.Max(sum - BaseHours, 0);

                DateTime datestart = DateSummary.First().Value;
                DateTime dateend = DateSummary.Last().Value;

                List<ExpenseReportDbModel> dbs = SQLAccess.LoadExpenses(datestart, dateend, CurrentEmployee.Id);
                double total = 0;
                if (dbs.Count > 0)
                {
                    List<ExpenseReportDbModel> dbsexpense = dbs.Where(x => x.Reimbursable == 1).ToList();

                    if (dbsexpense.Count > 0)
                    {
                        total = dbsexpense.Sum(x => x.TotalCost);
                    }
                }

                double ptoaccrued = cancelpto ? 0 : CurrentEmployee.PTORate * 0.5;

                TimesheetSubmissionDbModel timesheetsubdbmodel = new TimesheetSubmissionDbModel()
                {
                    EmployeeId = CurrentEmployee.Id,
                    Date = DateTimesheet,
                    TotalHours = sum,
                    PTOHours = pto,
                    OTHours = ot,
                    PTOAdded = ptoaccrued,
                    HolidayHours = holiday,
                    Approved = 0,
                    ExpensesCost = total //not approved yet
                };

                SQLAccess.AddTimesheetSubmissionData(timesheetsubdbmodel);
                LoadTimesheetSubmissionData();
            }
            ButtonInAction = true;
        }

        /// <summary>
        /// Save to DB
        /// </summary>
        private void SaveCommand(int submit)
        {
            if (!ButtonInAction)
            {
                return;
            }
            ButtonInAction = false;

            DateTime starttime = DateTime.Now;
            MessageVisible = true;

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
                            double budgetspent = CurrentEmployee.Rate * trentry.TimeEntry;

                            if (budgetspent == 0)
                            {

                            }
                            TimesheetRowDbModel dbmodel = new TimesheetRowDbModel()
                            {
                                EmployeeId = CurrentEmployee.Id,
                                SubProjectId = trm.SelectedSubproject.Id,
                                Date = (int)long.Parse(trentry.Date.ToString("yyyyMMdd")),
                                Submitted = submit,
                                Approved = 0,
                                TimeEntry = trentry.TimeEntry,
                                BudgetSpent = CurrentEmployee.Rate * trentry.TimeEntry,
                                ProjIdRef = trm.Project.Id,
                                Invoiced = 0
                            };

                            SQLAccess.AddTimesheetData(dbmodel);
                            //get data that needs to be removed
                        }
                    }



                    try
                    {
                        RolePerSubProjectDbModel rpp = new RolePerSubProjectDbModel()
                        {
                            SubProjectId = trm.SelectedSubproject.Id,
                            EmployeeId = CurrentEmployee.Id,
                            Role = (int)CurrentEmployee.DefaultRole,
                            Rate = CurrentEmployee.Rate,
                            BudgetHours = 0
                        };
                        SQLAccess.AddRolesPerSubProject(rpp);

                    }
                    catch
                    {
                    }

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

            ButtonInAction = true;
        }


        /// <summary>
        /// Load Date of Timesheet
        /// </summary>
        /// <param name="currdate"></param>
        public Task LoadCurrentTimesheet(DateTime currdate)
        {
            UpdateDateSummary(currdate);
            bool issubmitted = LoadTimesheetSubmissionData();
            LoadProjects(issubmitted);
            LoadTimesheetData();
            LoadExpenseData();
            return Task.CompletedTask;
            //CurrentPage.MakeClear();

        }

        private void UpdateDateSummary(DateTime currdate)
        {
            allowdatechange = false;
            BlankEntry = null;
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
            List<TREntryModel> blankentry = new List<TREntryModel>();
            double basehours = 0;
            for (int i = 0; i <= diff; i++)
            {
                DateTime dt = firstdate.AddDays(i);
                blankentry.Add(new TREntryModel { Date = dt });
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
            BlankEntry = new ObservableCollection<TREntryModel>(blankentry);
            DateSummary = new AsyncObservableCollection<DateWrapper>(dates);
            MonthYearString = $"{firstdate.ToString("MMMM")} {firstdate.Year}";
            DateString = $"[{firstdate.Day} - {lastdate.Day}]";

            BaseHours = basehours;

            DateTimesheet = (int)long.Parse(firstdate.Date.ToString("yyyyMMdd"));
            DateTime enddate = DateSummary.Last().Value;
            int difference = (int)Math.Ceiling(Math.Max((enddate - DateTime.Now).TotalDays, 0));
            DueDateValue = difference;
            ExpectedProgress = 100 - Math.Min(Math.Round((Convert.ToDouble(DueDateValue) / Convert.ToDouble(diff)) * 100, 2), 100);
            DateSelected = firstdate;
            allowdatechange = true;
        }

        //private void RowDataChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    if (e.NewItems != null)
        //    {
        //        foreach (INotifyPropertyChanged added in e?.NewItems)
        //        {
        //            TimesheetRowModel trm = (TimesheetRowModel)added;

        //            foreach (TREntryModel trentry in trm.Entries)
        //            {
        //                trentry.PropertyChanged += ItemModificationOnPropertyChanged;
        //            }
        //        }
        //    }

        //    if (e.OldItems != null)
        //    {
        //        foreach (INotifyPropertyChanged added in e?.OldItems)
        //        {
        //            TimesheetRowModel trm = (TimesheetRowModel)added;

        //            foreach (TREntryModel trentry in trm.Entries)
        //            {
        //                trentry.PropertyChanged -= ItemModificationOnPropertyChanged;
        //            }
        //        }
        //    }

        //    CanPressButton = Rowdata.Count == 0 ? true : false;

        //}

        //private void ItemModificationOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        //{
        //    SumTable();

        //    foreach (TimesheetRowModel trm in Rowdata)
        //    {
        //        trm.Total = trm.Entries.Sum(x => x.TimeEntry);
        //    }
        //    AutoSave();
        //}

        bool currentlyqued = false;

        public async void AutoSave()
        {
            if (!currentlyqued)
            {
                currentlyqued = true;
                await Task.Delay(30000);

                await Task.Run(() =>
                {
                    if (IsSubEditable)
                    {
                        SaveCommand(0);
                    }
                });

                currentlyqued = false;
            }
        }
    }
}
