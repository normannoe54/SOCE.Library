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
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Media;
using System.Threading;
using System.Windows.Threading;

namespace SOCE.Library.UI.ViewModels
{
    public class InvoicingSummaryVM : BaseVM
    {
        public ICommand ViewInvoiceCommand { get; set; }
        public ICommand DeleteInvoiceCommand { get; set; }
        public ICommand PrintCommand { get; set; }
        public ICommand PreviousCommand { get; set; }
        public ICommand NextCommand { get; set; }
        public ICommand CurrentCommand { get; set; }

        public ICommand HighlightStuffCommand { get; set; }

        public ICommand HighlightStuffAsDateCommand { get; set; }
        public ICommand CreateInvoiceCommand { get; set; }
        public ICommand ReviseInvoiceCommand { get; set; }
        public ICommand GoToDateCommand { get; set; }
        public ICommand SelectAllCommand { get; set; }
        public ICommand OpenLink { get; set; }


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

        private ObservableCollection<EmployeeLowResModel> _employees = new ObservableCollection<EmployeeLowResModel>();
        public ObservableCollection<EmployeeLowResModel> Employees
        {
            get { return _employees; }
            set
            {
                _employees = value;
                RaisePropertyChanged(nameof(Employees));
            }
        }

        private ObservableCollection<EmployeeLowResModel> _projectManagers = new ObservableCollection<EmployeeLowResModel>();
        public ObservableCollection<EmployeeLowResModel> ProjectManagers
        {
            get { return _projectManagers; }
            set
            {
                _projectManagers = value;
                RaisePropertyChanged(nameof(ProjectManagers));
            }
        }

        private ObservableCollection<InvoicingModel> _invoices = new ObservableCollection<InvoicingModel>();
        public ObservableCollection<InvoicingModel> Invoices
        {
            get { return _invoices; }
            set
            {
                _invoices = value;
                RaisePropertyChanged(nameof(Invoices));
            }
        }

        private ObservableCollection<ExpenseInvoiceModel> _expenses = new ObservableCollection<ExpenseInvoiceModel>();
        public ObservableCollection<ExpenseInvoiceModel> Expenses
        {
            get { return _expenses; }
            set
            {
                _expenses = value;
                RaisePropertyChanged(nameof(Expenses));
            }
        }

        private ObservableCollection<HourSummaryModel> _hourSummary = new ObservableCollection<HourSummaryModel>();
        public ObservableCollection<HourSummaryModel> HourSummary
        {
            get { return _hourSummary; }
            set
            {
                _hourSummary = value;
                RaisePropertyChanged(nameof(HourSummary));
            }
        }

        private double _hoursTotalSelection;
        public double HoursTotalSelection
        {
            get { return _hoursTotalSelection; }
            set
            {
                _hoursTotalSelection = value;
                RaisePropertyChanged(nameof(HoursTotalSelection));
            }
        }

        private double _budgetTotalSelection;
        public double BudgetTotalSelection
        {
            get { return _budgetTotalSelection; }
            set
            {
                _budgetTotalSelection = value;
                RaisePropertyChanged(nameof(BudgetTotalSelection));
            }
        }

        private DateTime? _dateOfSelection;
        public DateTime? DateOfSelection
        {
            get { return _dateOfSelection; }
            set
            {
                _dateOfSelection = value;
                RaisePropertyChanged(nameof(DateOfSelection));
            }
        }

        private bool _leftDrawerOpen = false;
        public bool LeftDrawerOpen
        {
            get { return _leftDrawerOpen; }
            set
            {
                _leftDrawerOpen = value;
                RaisePropertyChanged("LeftDrawerOpen");
            }
        }

        private bool _selectTrigger = false;
        public bool SelectTrigger
        {
            get { return _selectTrigger; }
            set
            {
                _selectTrigger = value;
                RaisePropertyChanged("SelectTrigger");
            }
        }

        private UserControl _leftViewToShow = new UserControl();
        public UserControl LeftViewToShow
        {
            get { return _leftViewToShow; }
            set
            {
                _leftViewToShow = value;
                RaisePropertyChanged(nameof(LeftViewToShow));
            }
        }

        public List<HourEntryModel> SelectedTimesheetInfo { get; set; } = new List<HourEntryModel>();

        private ProjectInvoicingModel _baseProject;
        public ProjectInvoicingModel BaseProject
        {
            get { return _baseProject; }
            set
            {
                _baseProject = value;
                RaisePropertyChanged(nameof(BaseProject));
            }
        }


        public InvoicingSummaryVM(ProjectViewResModel pm)
        {
            this.CreateInvoiceCommand = new RelayCommand(CreateInvoice);
            this.PreviousCommand = new RelayCommand(Previous);
            this.NextCommand = new RelayCommand(Next);
            this.CurrentCommand = new RelayCommand(Current);
            this.HighlightStuffCommand = new RelayCommand<HourEntryModel>(HighlightSubProject);
            this.HighlightStuffAsDateCommand = new RelayCommand<DateWrapper>(HighlightFromDate);
            this.GoToDateCommand = new RelayCommand<DateTime>(GoToDate);
            this.DeleteInvoiceCommand = new RelayCommand<InvoicingModel>(DeleteInvoice);
            this.ViewInvoiceCommand = new RelayCommand<InvoicingModel>(ViewInvoice);
            this.SelectAllCommand = new RelayCommand(SelectAll);
            this.OpenLink = new RelayCommand<InvoicingModel>(OpenLinkLocation);
            this.ReviseInvoiceCommand = new RelayCommand<InvoicingModel>(ReviseInvoice);

            List<EmployeeDbModel> employeesDb = SQLAccess.LoadEmployees();

            List<EmployeeLowResModel> totalemployees = new List<EmployeeLowResModel>();

            foreach (EmployeeDbModel employeenew in employeesDb)
            {
                totalemployees.Add(new EmployeeLowResModel(employeenew));
            }

            ObservableCollection<EmployeeLowResModel> ordered = new ObservableCollection<EmployeeLowResModel>(totalemployees.OrderBy(x => x.LastName).ToList());
            Employees = ordered;

            Reload(pm.Id, pm.ProjectManager);
        }

        private void OpenLinkLocation(InvoicingModel invoice)
        {
            if (invoice.LocationofLink != null)
            {
                try
                {
                    Process.Start(invoice.LocationofLink);
                }
                catch
                {

                }
            }
        }

        private void SelectAll()
        {
            SelectedTimesheetInfo = new List<HourEntryModel>();

            if (!SelectTrigger)
            {
                List<TimesheetRowDbModel> time = SQLAccess.LoadTimeSheetDataByUninvoiced(BaseProject.Id);

                foreach (TimesheetRowDbModel dbmod in time)
                {
                    DateTime dt = DateTime.ParseExact(dbmod.Date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);

                    HourEntryModel hem = new HourEntryModel()
                    {
                        TimeId = dbmod.Id,
                        SubId = dbmod.SubProjectId,
                        Invoiced = false,
                        Date = dt,
                        TimeEntry = dbmod.TimeEntry,
                        BudgetSpent = dbmod.BudgetSpent,
                        IsSelectedForInvoicing = true
                    };
                    SelectedTimesheetInfo.Add(hem);
                }
            }

            GoToDate(DateSummary.First().Value);
            SelectTrigger = !SelectTrigger;
        }

        public void Reload(int pmid, EmployeeLowResModel projman)
        {
            SelectedTimesheetInfo = new List<HourEntryModel>();

            ProjectDbModel pdb = SQLAccess.LoadProjectsById(pmid);
            ProjectInvoicingModel basemod = new ProjectInvoicingModel(pdb);
            basemod.ProjectManager = projman;

            BaseProject = basemod;

            List<InvoicingModelDb> invoices = SQLAccess.LoadInvoices(pmid);
            List<InvoicingModel> invmodels = new List<InvoicingModel>();

            foreach (InvoicingModelDb invold in invoices)
            {
                InvoicingModel invnew = new InvoicingModel(invold);
                invmodels.Add(invnew);
            }


            foreach (InvoicingModel invprev in invmodels)
            {
                List<InvoicingModel> founditems = invmodels.Where(x => x.InvoiceId == invprev.InvoiceId).ToList();

                if (founditems != null && founditems.Count() > 1)
                {
                    invprev.RevisedButton = false;
                }
            }

            List<InvoicingModel> orderedinvoices = invmodels.OrderBy(x => x.InvoiceId).ToList();

            if (invmodels.Count > 0)
            {
                InvoicingModel maxitem = orderedinvoices.OrderByDescending(x => x.Id).FirstOrDefault();

                if (maxitem != null)
                {
                    maxitem.CanDelete = true;
                }
            }

            Invoices = new ObservableCollection<InvoicingModel>(orderedinvoices);

            //expenses
            List<ExpenseReportDbModel> expenses = SQLAccess.LoadExpensesByProjectId(pmid);
            List<ExpenseInvoiceModel> expensemodels = new List<ExpenseInvoiceModel>();

            foreach (ExpenseReportDbModel expense in expenses)
            {
                if (Convert.ToBoolean(expense.IsClientBillable))
                {
                    ExpenseInvoiceModel invnew = new ExpenseInvoiceModel(expense);
                    expensemodels.Add(invnew);
                }

            }

            Expenses = new ObservableCollection<ExpenseInvoiceModel>(expensemodels);

            Current();
        }

        private void GoToDate(DateTime date)
        {
            UpdateDateSummary(date);

            HourSummary = new ObservableCollection<HourSummaryModel>(LoadHourBreakdown(DateSummary.First(), DateSummary.Last()));
            CalculateSelected();
        }


        private void HighlightSubProject(HourEntryModel hemval)
        {
            bool curstatus = hemval.IsSelectedForInvoicing;
            //bool anyselectedforinvoicing = false;
            foreach (HourSummaryModel hsm in HourSummary)
            {
                if (hemval.SubId == hsm.PhaseId)
                {
                    foreach (HourSummaryIndModel hsim in hsm.EmployeeSummary)
                    {
                        foreach (HourEntryModel hem in hsim.Entries)
                        {
                            if (hem.Date <= hemval.Date)
                            {
                                if (hem.TimeEntry > 0 && !hem.Invoiced)
                                {
                                    hem.IsSelectedForInvoicing = !curstatus;

                                    if (hem.IsSelectedForInvoicing)
                                    {
                                        HourEntryModel hemfound = SelectedTimesheetInfo.Where(x => x.TimeId == hem.TimeId).FirstOrDefault();

                                        if (hemfound == null)
                                        {
                                            SelectedTimesheetInfo.Add(hem);
                                        }
                                    }
                                    else
                                    {
                                        HourEntryModel hemfound = SelectedTimesheetInfo.Where(x => x.TimeId == hem.TimeId).FirstOrDefault();

                                        if (hemfound != null)
                                        {
                                            SelectedTimesheetInfo.Remove(hemfound);
                                        }
                                    }
                                    //anyselectedforinvoicing = anyselectedforinvoicing || hem.IsSelectedForInvoicing;
                                }
                            }
                            else
                            {
                                if (hem.TimeEntry > 0 && !hem.Invoiced)
                                {
                                    hem.IsSelectedForInvoicing = false;

                                    HourEntryModel hemfound = SelectedTimesheetInfo.Where(x => x.TimeId == hem.TimeId).FirstOrDefault();

                                    if (hemfound != null)
                                    {
                                        SelectedTimesheetInfo.Remove(hemfound);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            CalculateSelected();

        }

        private void HighlightFromDate(DateWrapper hemval)
        {
            //bool anyselectedforinvoicing = true;

            List<HourEntryModel> hems = new List<HourEntryModel>();
            if (HourSummary.Count > 0)
            {
                foreach (HourSummaryModel hsm in HourSummary)
                {
                    foreach (HourSummaryIndModel hsim in hsm.EmployeeSummary)
                    {
                        foreach (HourEntryModel hem in hsim.Entries)
                        {
                            if (hem.TimeEntry > 0 && hem.Date <= hemval.Value && !hem.Invoiced)
                            {
                                hems.Add(hem);
                            }
                        }
                    }
                }
            }


            if (hems.Count > 0)
            {
                HourEntryModel hemofinterest = hems.OrderBy(x => x.Date).Last();
                bool curraction = !hemofinterest.IsSelectedForInvoicing;
                //anyselectedforinvoicing = false;

                foreach (HourSummaryModel hsm in HourSummary)
                {
                    //double sum = 0;
                    foreach (HourSummaryIndModel hsim in hsm.EmployeeSummary)
                    {
                        foreach (HourEntryModel hem in hsim.Entries)
                        {
                            if (hem.Date <= hemval.Value)
                            {
                                if (hem.TimeEntry > 0 && !hem.Invoiced)
                                {
                                    hem.IsSelectedForInvoicing = curraction;

                                    if (hem.IsSelectedForInvoicing)
                                    {
                                        HourEntryModel hemfound = SelectedTimesheetInfo.Where(x => x.TimeId == hem.TimeId).FirstOrDefault();

                                        if (hemfound == null)
                                        {
                                            SelectedTimesheetInfo.Add(hem);
                                        }
                                    }
                                    else
                                    {
                                        HourEntryModel hemfound = SelectedTimesheetInfo.Where(x => x.TimeId == hem.TimeId).FirstOrDefault();

                                        if (hemfound != null)
                                        {
                                            SelectedTimesheetInfo.Remove(hemfound);
                                        }
                                    }
                                    //anyselectedforinvoicing = anyselectedforinvoicing || hem.IsSelectedForInvoicing;
                                }
                            }
                            else
                            {
                                if (hem.TimeEntry > 0 && !hem.Invoiced)
                                {
                                    hem.IsSelectedForInvoicing = false;

                                    HourEntryModel hemfound = SelectedTimesheetInfo.Where(x => x.TimeId == hem.TimeId).FirstOrDefault();

                                    if (hemfound != null)
                                    {
                                        SelectedTimesheetInfo.Remove(hemfound);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            CalculateSelected();
        }

        private void CalculateSelected()
        {
            double hourstotal = 0;
            double budgettotal = 0;
            DateTime? dateoflast = null;

            foreach (HourSummaryModel hsm in HourSummary)
            {
                if (hsm.HoursSelectableVis)
                {
                    double sum = 0;
                    foreach (HourEntryModel hem in SelectedTimesheetInfo)
                    {
                        if (hem.SubId == hsm.PhaseId)
                        {
                            if (dateoflast == null || hem.Date > dateoflast)
                            {
                                dateoflast = hem.Date;
                            }
                            budgettotal += hem.BudgetSpent;
                            sum += hem.TimeEntry;
                        }
                    }
                    hsm.SelectedHours = sum;
                    hourstotal += sum;
                }
            }

            HoursTotalSelection = hourstotal;
            BudgetTotalSelection = budgettotal;
            DateOfSelection = dateoflast;
        }

        private void Previous()
        {
            //CoreAI CurrentPage = IoCCore.Application as CoreAI;
            //CurrentPage.MakeBlurry();
            //await Task.Run(() => Task.Delay(600));
            UpdateDateSummary(DateSummary.First().Value.AddDays(-1));
            //await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            //{
            HourSummary = new ObservableCollection<HourSummaryModel>(LoadHourBreakdown(DateSummary.First(), DateSummary.Last()));
            CalculateSelected();
            //BlankSelection();
            //}));
            //await Task.Run(() => Task.Delay(600));
            //CurrentPage.MakeClear();
        }

        /// <summary>LoadScheduling
        /// Button Press
        /// </summary>
        private void Next()
        {
            //CoreAI CurrentPage = IoCCore.Application as CoreAI;
            //CurrentPage.MakeBlurry();
            //await Task.Run(() => Task.Delay(600));
            UpdateDateSummary(DateSummary.Last().Value.AddDays(7));
            //await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            //{
            HourSummary = new ObservableCollection<HourSummaryModel>(LoadHourBreakdown(DateSummary.First(), DateSummary.Last()));
            CalculateSelected();
            //BlankSelection();
            //}));
            //await Task.Run(() => Task.Delay(600));
            //CurrentPage.MakeClear();

        }

        /// <summary>
        /// Button Press
        /// </summary>
        private void Current()
        {
            //CoreAI CurrentPage = IoCCore.Application as CoreAI;
            //CurrentPage.MakeBlurry();
            //await Task.Run(() => Task.Delay(600));
            //await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => UpdateDateSummary(DateTime.Now)));
            //await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            //{
            UpdateDateSummary(DateTime.Now);
            HourSummary = new ObservableCollection<HourSummaryModel>(LoadHourBreakdown(DateSummary.First(), DateSummary.Last()));
            CalculateSelected();
            //BlankSelection();
            //}));
            //await Task.Run(() => Task.Delay(600));
            //CurrentPage.MakeClear();
        }

        private void UpdateDateSummary(DateTime currdate)
        {
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
            for (int i = 0; i <= diff; i++)
            {
                DateTime dt = firstdate.AddDays(i);
                blankentry.Add(new TREntryModel { Date = dt });
                dates.Add(new DateWrapper(dt.Date));
            }
            DateSummary = new AsyncObservableCollection<DateWrapper>(dates);

        }

        private List<HourSummaryModel> LoadHourBreakdown(DateWrapper firstdate, DateWrapper lastdate)
        {
            HourSummary.Clear();
            List<HourSummaryModel> hsm = new List<HourSummaryModel>();

            List<SubProjectDbModel> subs = SQLAccess.LoadSubProjectsByProject(BaseProject.Id);

            SolidColorBrush billablecolor = Brushes.Green;
            SolidColorBrush nonbillablecolor = Brushes.Red;

            foreach (SubProjectDbModel sub in subs)
            {
                bool billable = Convert.ToBoolean(sub.IsBillable);

                HourSummaryModel hsma = new HourSummaryModel()
                {
                    PhaseId = sub.Id,
                    PhaseName = sub.PointNumber,
                    Invoiced = Convert.ToBoolean(sub.IsInvoiced),
                    IsBillableKind = billable ? PackIconKind.CurrencyUsd : PackIconKind.CurrencyUsdOff,
                    IsBillableKindBackground = billable ? billablecolor : nonbillablecolor
                };

                List<RolePerSubProjectDbModel> roles = SQLAccess.LoadRolesPerSubProject(sub.Id);

                List<HourSummaryIndModel> employeesummary = new List<HourSummaryIndModel>();

                foreach (RolePerSubProjectDbModel role in roles)
                {
                    EmployeeLowResModel em = Employees.Where(x => x.Id == role.EmployeeId).FirstOrDefault();

                    if (em == null)
                    {
                        EmployeeDbModel emfind = SQLAccess.LoadEmployeeById(role.EmployeeId);
                        em = new EmployeeLowResModel(emfind);
                    }

                    if (em != null)
                    {
                        List<HourEntryModel> entries = new List<HourEntryModel>();

                        HourSummaryIndModel hsim = new HourSummaryIndModel()
                        {
                            Employee = em
                        };

                        List<TimesheetRowDbModel> timeperrole = SQLAccess.LoadTimeSheetBySub(firstdate.Value, lastdate.Value, em.Id, sub.Id);

                        foreach (TimesheetRowDbModel timeind in timeperrole)
                        {
                            HourEntryModel hemfound = SelectedTimesheetInfo.Where(x => x.TimeId == timeind.Id).FirstOrDefault();
                            bool found = hemfound == null ? false : true;

                            DateTime dt = DateTime.ParseExact(timeind.Date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);

                            bool invoiced = Convert.ToBoolean(timeind.Invoiced);
                            HourEntryModel hem = new HourEntryModel()
                            {
                                TimeId = timeind.Id,
                                SubId = sub.Id,
                                Invoiced = invoiced,
                                Date = dt,
                                TimeEntry = timeind.TimeEntry,
                                BudgetSpent = timeind.BudgetSpent,
                                IsSelectedForInvoicing = found
                            };
                            entries.Add(hem);
                        }

                        DateTime dateinc = firstdate.Value;

                        while (dateinc <= lastdate.Value)
                        {
                            if (!entries.Any(x => x.Date == dateinc))
                            {
                                //add

                                HourEntryModel entry = new HourEntryModel() { Date = dateinc, TimeEntry = 0 };
                                entries.Add(entry);
                            }
                            dateinc = dateinc.AddDays(1);
                        }
                        List<HourEntryModel> hoursordered = entries.OrderBy(x => x.Date).ToList();
                        hsim.Entries = new ObservableCollection<HourEntryModel>(hoursordered);
                        employeesummary.Add(hsim);
                    }
                }
                hsma.EmployeeSummary = new ObservableCollection<HourSummaryIndModel>(employeesummary);

                if (hsma.EmployeeSummary.Count > 0)
                {
                    List<TimesheetRowDbModel> time = SQLAccess.LoadUninvoicedTimeSheetDatabySub(sub.Id);
                    if (time.Count > 0)
                    {
                        hsma.OutstandingHours = time.Sum(x => x.TimeEntry);
                        int dateofintetst = time.Min(x => x.Date);
                        DateTime dt = DateTime.ParseExact(dateofintetst.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                        hsma.DateOfPrevious = dt;
                        hsma.SelectedHours = 0;
                    }

                    hsma.HoursSelectableVis = hsma.OutstandingHours > 0 ? true : false;
                    hsm.Add(hsma);
                }
                else if (billable)
                {
                    hsm.Add(hsma);
                }
            }

            return hsm;
        }

        public void DeletefromUI(int index)
        {
            if (Invoices.Count > 0)
            {
                InvoicingModel invoi = Invoices[index];

                List<InvoicingRowsDb> rows = SQLAccess.LoadInvoiceRows(invoi.Id);

                foreach (InvoicingRowsDb row in rows)
                {
                    double percentcomplete = 0;
                    double previoussubhourlyfee = 0;
                    if (row.SubId != 0)
                    {
                        SubProjectDbModel sub = SQLAccess.LoadSubProjectsById(row.SubId);

                        if (index > 0)
                        {
                            InvoicingModel invprev = Invoices[index - 1];
                            InvoicingRowsDb dbprev = SQLAccess.LoadInvoiceRowsByInvoiceAndSubId(row.SubId, invprev.Id);

                            if (dbprev != null)
                            {
                                percentcomplete = dbprev.PercentComplete;

                                if (Convert.ToBoolean(sub.IsHourly))
                                {
                                    previoussubhourlyfee = dbprev.PreviousInvoiced + dbprev.ThisPeriodInvoiced;
                                }
                            }
                        }

                        if (sub.Id != 0)
                        {
                            if (row.ThisPeriodInvoiced > 0)
                            {
                                SQLAccess.UpdateInvoiced(row.SubId, 0, 0, percentcomplete);

                                if (sub?.IsHourly != null)
                                {
                                    if (Convert.ToBoolean(sub?.IsHourly))
                                    {
                                        SQLAccess.UpdateSubFee(row.SubId, previoussubhourlyfee);
                                        double diff = BaseProject.Fee - sub.Fee - previoussubhourlyfee;
                                        SQLAccess.UpdateFee(BaseProject.Id, diff);
                                    }
                                }
                            }
                        }
                    }

                    SQLAccess.DeleteInvoiceRows(row.Id);

                    //int invoiced = percentcomplete >= 100 ? 1 : 0;

                }

                SQLAccess.DeleteInvoice(invoi.Id);


                foreach (int time in invoi.TimesheetIds)
                {
                    SQLAccess.UpdateInvoiceStatusTime(time, 0);

                }

                if (invoi.ExpenseReportIds != null)
                {
                    foreach (int exid in invoi.ExpenseReportIds)
                    {
                        SQLAccess.UpdateInvoiced(exid, 0);
                    }
                }
            }

            Reload(BaseProject.Id, BaseProject.ProjectManager);
        }

        private void DeleteInvoice(InvoicingModel invoice)
        {
            int index = Invoices.IndexOf(invoice);
            LeftViewToShow = new YesNoView();
            YesNoVM addsubvm = new YesNoVM(index, this);
            addsubvm.Message = "Are you sure you want to delete";

            addsubvm.SubMessage = $"(Invoice: {invoice.InvoiceId}) ";
            LeftViewToShow.DataContext = addsubvm;
            LeftDrawerOpen = true;
        }

        public void UpdateInvoicewithOverhead(InvoicingModel invoice)
        {
            List<int> timeids = invoice.TimesheetIds;
            foreach (HourEntryModel hem in SelectedTimesheetInfo)
            {
                timeids.Add(hem.TimeId);
                SQLAccess.UpdateInvoiceStatusTime(hem.TimeId, 1);
            }

            int[] timeidsnew = timeids.ToArray();
            string result = string.Join(",", timeidsnew);

            SQLAccess.UpdateInvoiceTime(invoice.Id, result);
            Reload(BaseProject.Id, BaseProject.ProjectManager);
        }

        private void ViewInvoice(InvoicingModel invoice)
        {
            LeftViewToShow = new CreateInvoiceView();
            CreateInvoiceVM addsubvm = new CreateInvoiceVM(BaseProject, invoice);
            LeftViewToShow.DataContext = addsubvm;
            LeftDrawerOpen = true;


            //List<InvoicingRowsDb> rows = SQLAccess.LoadInvoiceRows(invoice.Id);

            //foreach (InvoicingRowsDb row in rows)
            //{
            //    SQLAccess.DeleteInvoiceRows(row.Id);
            //}

            //SQLAccess.DeleteInvoice(invoice.Id);


            //foreach (int time in invoice.TimesheetIds)
            //{
            //    SQLAccess.UpdateInvoiceStatusTime(time, 0);
            //}
            //Reload(BaseProject.Id, BaseProject.ProjectManager);
        }
        private void ReviseInvoice(InvoicingModel invoice)
        {
            List<ExpenseInvoiceModel> selectedexpenses = Expenses.Where(x => x.IsSelected && !x.IsInvoiced).ToList();

            LeftViewToShow = new CreateInvoiceView();
            CreateInvoiceVM addsubvm = new CreateInvoiceVM(BaseProject, this, invoice, selectedexpenses);
            LeftViewToShow.DataContext = addsubvm;
            LeftDrawerOpen = true;

        }

        private void CreateInvoice()
        {
            List<ExpenseInvoiceModel> selectedexpenses = Expenses.Where(x => x.IsSelected && !x.IsInvoiced).ToList();

            if (DateOfSelection != null || selectedexpenses.Count > 0)
            {
                bool subcheck = false;
                List<SubProjectDbModel> subs = SQLAccess.LoadSubProjectsByProject(BaseProject.Id);

                foreach (SubProjectDbModel sub in subs)
                {
                    if (Convert.ToBoolean(sub.IsBillable) && sub.PercentComplete < 100)
                    {
                        subcheck = true;
                        break;
                    }
                }

                if (!subcheck)
                {
                    InvoicingModel invlatest = Invoices.LastOrDefault();
                    YesNoVM addsubvm = new YesNoVM(invlatest, this);
                    addsubvm.Message = $"Project is fully invoiced {Environment.NewLine} Add overhead time to";
                    addsubvm.SubMessage = $"Invoice Number {Environment.NewLine} {invlatest.InvoiceId}";

                    LeftViewToShow = new YesNoView();
                    LeftViewToShow.DataContext = addsubvm;
                    LeftDrawerOpen = true;
                }
                else
                {
                    LeftViewToShow = new CreateInvoiceView();
                    CreateInvoiceVM addsubvm = new CreateInvoiceVM(BaseProject, this, HoursTotalSelection, BudgetTotalSelection, (DateTime)DateOfSelection, SelectedTimesheetInfo, selectedexpenses);
                    LeftViewToShow.DataContext = addsubvm;
                    LeftDrawerOpen = true;
                }
            }
        }
    }
}

