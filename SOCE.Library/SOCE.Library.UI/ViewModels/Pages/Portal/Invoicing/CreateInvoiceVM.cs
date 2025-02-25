using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SOCE.Library.Db;
using System.Collections.ObjectModel;
using System.Linq;
using System.Globalization;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace SOCE.Library.UI.ViewModels
{
    public class CreateInvoiceVM : BaseVM
    {
        public bool result = false;

        private bool _istouchable = true;
        public bool Istouchable
        {
            get { return _istouchable; }
            set
            {
                _istouchable = value;
                RaisePropertyChanged(nameof(Istouchable));
            }
        }

        private string _errorMessage = "";
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                RaisePropertyChanged(nameof(ErrorMessage));
            }
        }

        private ProjectInvoicingModel baseProject = new ProjectInvoicingModel();
        public ProjectInvoicingModel BaseProject
        {
            get { return baseProject; }
            set
            {
                baseProject = value;
                RaisePropertyChanged(nameof(BaseProject));
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

        private EmployeeLowResModel _selectedPM;
        public EmployeeLowResModel SelectedPM
        {
            get { return _selectedPM; }
            set
            {
                _selectedPM = value;

                _selectedPM.LoadSignature();

                RaisePropertyChanged(nameof(SelectedPM));
            }
        }

        private int _invoiceNumberInp;
        public int InvoiceNumberInp
        {
            get { return _invoiceNumberInp; }
            set
            {
                _invoiceNumberInp = value;
                RaisePropertyChanged("InvoiceNumberInp");
            }
        }

        private DateTime? _invoiceDateInp;
        public DateTime? InvoiceDateInp
        {
            get { return _invoiceDateInp; }
            set
            {
                _invoiceDateInp = value;
                RaisePropertyChanged("InvoiceDateInp");
            }
        }

        private DateTime? _invoiceDateRevisedInp;
        public DateTime? InvoiceDateRevisedInp
        {
            get { return _invoiceDateRevisedInp; }
            set
            {
                _invoiceDateRevisedInp = value;
                RaisePropertyChanged("InvoiceDateRevisedInp");
            }
        }


        private string _clientNameInp;
        public string ClientNameInp
        {
            get { return _clientNameInp; }
            set
            {
                _clientNameInp = value;
                RaisePropertyChanged("ClientNameInp");
            }
        }

        private string _clientCompanyNameInp;
        public string ClientCompanyNameInp
        {
            get { return _clientCompanyNameInp; }
            set
            {
                _clientCompanyNameInp = value;
                RaisePropertyChanged("ClientCompanyNameInp");
            }
        }

        private string _clientAddressInp;
        public string ClientAddressInp
        {
            get { return _clientAddressInp; }
            set
            {
                _clientAddressInp = value;
                RaisePropertyChanged("ClientAddressInp");
            }
        }

        private string _clientCityInp;
        public string ClientCityInp
        {
            get { return _clientCityInp; }
            set
            {
                _clientCityInp = value;
                RaisePropertyChanged("ClientCityInp");
            }
        }

        private string _projectNameInp;
        public string ProjectNameInp
        {
            get { return _projectNameInp; }
            set
            {
                _projectNameInp = value;
                RaisePropertyChanged("ProjectNameInp");
            }
        }

        private int _projectNumberInp;
        public int ProjectNumberInp
        {
            get { return _projectNumberInp; }
            set
            {
                _projectNumberInp = value;
                RaisePropertyChanged("ProjectNumberInp");
            }
        }

        private string _linkLocation;
        public string LinkLocation
        {
            get { return _linkLocation; }
            set
            {
                _linkLocation = value;
                RaisePropertyChanged("LinkLocation");
            }
        }



        private ObservableCollection<InvoicingRows> _nonAdServicePhases = new ObservableCollection<InvoicingRows>();
        public ObservableCollection<InvoicingRows> NonAdServicePhases
        {
            get { return _nonAdServicePhases; }
            set
            {
                _nonAdServicePhases = value;
                RaisePropertyChanged(nameof(NonAdServicePhases));
            }
        }

        private ObservableCollection<InvoicingRows> _adServicePhasesUnInvoiced = new ObservableCollection<InvoicingRows>();
        public ObservableCollection<InvoicingRows> AdServicePhasesUnInvoiced
        {
            get { return _adServicePhasesUnInvoiced; }
            set
            {
                _adServicePhasesUnInvoiced = value;
                RaisePropertyChanged(nameof(AdServicePhasesUnInvoiced));
            }
        }

        private ObservableCollection<InvoicingRows> _adServicePhasesInvoiced = new ObservableCollection<InvoicingRows>();
        public ObservableCollection<InvoicingRows> AdServicePhasesInvoiced
        {
            get { return _adServicePhasesInvoiced; }
            set
            {
                _adServicePhasesInvoiced = value;
                RaisePropertyChanged(nameof(AdServicePhasesInvoiced));
            }
        }

        private ObservableCollection<InvoicingRows> _expenses = new ObservableCollection<InvoicingRows>();
        public ObservableCollection<InvoicingRows> Expenses
        {
            get { return _expenses; }
            set
            {
                _expenses = value;
                RaisePropertyChanged(nameof(Expenses));
            }
        }

        private DateTime? _dateofServicesAreComplete;
        public DateTime? DateofServicesAreComplete
        {
            get { return _dateofServicesAreComplete; }
            set
            {
                _dateofServicesAreComplete = value;
                RaisePropertyChanged("DateofServicesAreComplete");
            }
        }

        private bool _doBasicServicesExist;
        public bool DoBasicServicesExist
        {
            get { return _doBasicServicesExist; }
            set
            {
                _doBasicServicesExist = value;
                RaisePropertyChanged("DoBasicServicesExist");
            }
        }

        private bool _isRevised = false;
        public bool IsRevised
        {
            get { return _isRevised; }
            set
            {
                _isRevised = value;
                RaisePropertyChanged("IsRevised");
            }
        }

        private bool _doAdservicesExist;
        public bool DoAdservicesExist
        {
            get { return _doAdservicesExist; }
            set
            {
                _doAdservicesExist = value;
                RaisePropertyChanged("DoAdservicesExist");
            }
        }

        private bool _isPreviousAdServiceVis;
        public bool IsPreviousAdServiceVis
        {
            get { return _isPreviousAdServiceVis; }
            set
            {
                _isPreviousAdServiceVis = value;
                RaisePropertyChanged("IsPreviousAdServiceVis");
            }
        }

        private double _totalContractFee;
        public double TotalContractFee
        {
            get { return _totalContractFee; }
            set
            {
                _totalContractFee = value;
                RaisePropertyChanged("TotalContractFee");
            }
        }

        private double _totalContractAdFee;
        public double TotalContractAdFee
        {
            get { return _totalContractAdFee; }
            set
            {
                _totalContractAdFee = value;
                RaisePropertyChanged("TotalContractAdFee");
            }
        }

        private bool _isTotalContractAdFee;
        public bool IsTotalContractAdFee
        {
            get { return _isTotalContractAdFee; }
            set
            {
                _isTotalContractAdFee = value;
                RaisePropertyChanged("IsTotalContractAdFee");
            }
        }


        private double _invoicedToDateTotal;
        public double InvoicedToDateTotal
        {
            get { return _invoicedToDateTotal; }
            set
            {
                _invoicedToDateTotal = value;
                RaisePropertyChanged("InvoicedToDateTotal");
            }
        }

        private double _previousTotal;
        public double PreviousTotal
        {
            get { return _previousTotal; }
            set
            {
                _previousTotal = value;
                RaisePropertyChanged("PreviousTotal");
            }
        }

        private double _thisPeriodTotal;
        public double ThisPeriodTotal
        {
            get { return _thisPeriodTotal; }
            set
            {
                _thisPeriodTotal = value;
                RaisePropertyChanged("ThisPeriodTotal");
            }
        }

        private double _invoicedToDateTotalAd;
        public double InvoicedToDateTotalAd
        {
            get { return _invoicedToDateTotalAd; }
            set
            {
                _invoicedToDateTotalAd = value;
                RaisePropertyChanged("InvoicedToDateTotalAd");
            }
        }

        private double _previousTotalAd;
        public double PreviousTotalAd
        {
            get { return _previousTotalAd; }
            set
            {
                _previousTotalAd = value;
                RaisePropertyChanged("PreviousTotalAd");
            }
        }

        private double _thisPeriodTotalAd;
        public double ThisPeriodTotalAd
        {
            get { return _thisPeriodTotalAd; }
            set
            {
                _thisPeriodTotalAd = value;
                RaisePropertyChanged("ThisPeriodTotalAd");
            }
        }

        private double _invoicedToDateTotalToDate;
        public double InvoicedToDateTotalToDate
        {
            get { return _invoicedToDateTotalToDate; }
            set
            {
                _invoicedToDateTotalToDate = value;
                RaisePropertyChanged("InvoicedToDateTotalToDate");
            }
        }

        private double _previousTotalToDate;
        public double PreviousTotalToDate
        {
            get { return _previousTotalToDate; }
            set
            {
                _previousTotalToDate = value;
                RaisePropertyChanged("PreviousTotalToDate");
            }
        }

        private double _thisPeriodTotalToDate;
        public double ThisPeriodTotalToDate
        {
            get { return _thisPeriodTotalToDate; }
            set
            {
                _thisPeriodTotalToDate = value;
                RaisePropertyChanged("ThisPeriodTotalToDate");
            }
        }

        private double _totalAmountDue;
        public double TotalAmountDue
        {
            get { return _totalAmountDue; }
            set
            {
                _totalAmountDue = value;
                RaisePropertyChanged("TotalAmountDue");
            }
        }

        private bool _isTotalContractFee;
        public bool IsTotalContractFee
        {
            get { return _isTotalContractFee; }
            set
            {
                _isTotalContractFee = value;
                RaisePropertyChanged("IsTotalContractFee");
            }
        }

        private double _thisPeriodSpent;
        public double ThisPeriodSpent
        {
            get { return _thisPeriodSpent; }
            set
            {
                _thisPeriodSpent = value;
                RaisePropertyChanged("ThisPeriodSpent");
            }
        }

        private double _thisAdPeriodSpent;
        public double ThisAdPeriodSpent
        {
            get { return _thisAdPeriodSpent; }
            set
            {
                _thisAdPeriodSpent = value;
                RaisePropertyChanged("ThisAdPeriodSpent");
            }
        }

        private bool _isReimbursablesVis;
        public bool IsReimbursablesVis
        {
            get { return _isReimbursablesVis; }
            set
            {
                _isReimbursablesVis = value;
                RaisePropertyChanged("IsReimbursablesVis");
            }
        }

        private double _reimbursableToDate;
        public double ReimbursableToDate
        {
            get { return _reimbursableToDate; }
            set
            {
                _reimbursableToDate = value;
                RaisePropertyChanged("ReimbursableToDate");
            }
        }

        private double _reimbursablePrevious;
        public double ReimbursablePrevious
        {
            get { return _reimbursablePrevious; }
            set
            {
                _reimbursablePrevious = value;
                RaisePropertyChanged("ReimbursablePrevious");
            }
        }

        private double _reimbursableThisPeriod;
        public double ReimbursableThisPeriod
        {
            get { return _reimbursableThisPeriod; }
            set
            {
                _reimbursableThisPeriod = value;
                RaisePropertyChanged("ReimbursableThisPeriod");
            }
        }

        private double _previousExpensesInvoicedToDate;
        public double PreviousExpensesInvoicedToDate
        {
            get { return _previousExpensesInvoicedToDate; }
            set
            {
                _previousExpensesInvoicedToDate = value;
                RaisePropertyChanged("PreviousExpensesInvoicedToDate");
            }
        }

        private DateTime _datePreviousExpenses;
        public DateTime DatePreviousExpenses
        {
            get { return _datePreviousExpenses; }
            set
            {
                _datePreviousExpenses = value;
                RaisePropertyChanged("DatePreviousExpenses");
            }
        }

        private bool _isPreviousExpenseVis = false;
        public bool IsPreviousExpenseVis
        {
            get { return _isPreviousExpenseVis; }
            set
            {
                _isPreviousExpenseVis = value;
                RaisePropertyChanged("IsPreviousExpenseVis");
            }
        }


        private InvoicingSummaryVM baseViewModel;

        public List<HourEntryModel> timesheetidstoinvoice = new List<HourEntryModel>();
        public ICommand CreateInvoiceCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand SelectInvoiceLocation { get; set; }
        public List<InvoicingModelDb> previousinvoices { get; set; } = new List<InvoicingModelDb>();

        public List<ExpenseInvoiceModel> selectedexpenses { get; set; } = new List<ExpenseInvoiceModel>();

        public InvoicingModel revisedinvoice;
        public CreateInvoiceVM(ProjectInvoicingModel pm, InvoicingSummaryVM psvm, double hours, double budget, DateTime dateinitial, List<HourEntryModel> hourentry, List<ExpenseInvoiceModel> expenses)
        {
            selectedexpenses = expenses;
            Istouchable = true;
            BaseProject = pm;
            baseViewModel = psvm;
            //TotalFeeInp = pm.Fee;
            timesheetidstoinvoice = hourentry;
            //PreviousSpentInp = pm.TotalFeeInvoiced;
            //this.CreateInvoiceCommand = new RelayCommand(this.CreateInvoice);
            this.CloseCommand = new RelayCommand(this.CloseWindow);
            this.CreateInvoiceCommand = new RelayCommand(this.CreateInvoice);
            this.SelectInvoiceLocation = new RelayCommand(this.ClickInvoice);
            //TotalHoursInvoicedInp = hours;
            //TotalBudgetInvoiced = budget;
            DateofServicesAreComplete = dateinitial;
            InvoiceDateInp = DateTime.Today;
            ProjectNameInp = BaseProject.ProjectName;
            ProjectNumberInp = BaseProject.ProjectNumber;
            LoadProjectManagers();
            LoadInfo();
        }

        public CreateInvoiceVM(ProjectInvoicingModel pm, InvoicingSummaryVM psvm, InvoicingModel invoice, List<ExpenseInvoiceModel> expenses)
        {
            IsRevised = true;
            selectedexpenses = expenses;
            Istouchable = true;
            BaseProject = pm;
            baseViewModel = psvm;
            this.CloseCommand = new RelayCommand(this.CloseWindow);
            this.CreateInvoiceCommand = new RelayCommand(this.CreateInvoice);
            this.SelectInvoiceLocation = new RelayCommand(this.ClickInvoice);
            BaseProject = pm;
            ProjectNameInp = BaseProject.ProjectName;
            ProjectNumberInp = BaseProject.ProjectNumber;
            revisedinvoice = invoice;
            LoadProjectManagers();
            ShowInvoice(invoice);
        }

        public CreateInvoiceVM(ProjectInvoicingModel pm, InvoicingModel invoice)
        {
            Istouchable = false;
            this.CloseCommand = new RelayCommand(this.CloseWindow);
            BaseProject = pm;
            IsRevised = invoice.Revised;
            ProjectNameInp = BaseProject.ProjectName;
            ProjectNumberInp = BaseProject.ProjectNumber;
            LinkLocation = invoice.LocationofLink;

            ShowInvoice(invoice);
        }

        private void ShowInvoice(InvoicingModel invoice)
        {
            ClientAddressInp = invoice.ClientAddress;
            ClientNameInp = invoice.ClientName;
            ClientCompanyNameInp = invoice.ClientCompany;
            ClientCityInp = invoice.ClientCity;
            InvoiceDateInp = DateTime.Today;
            InvoiceNumberInp = invoice.InvoiceId;
            InvoiceDateRevisedInp = invoice.Date;

            if (invoice.AddServicesDate != null)
            {
                //DateTime datefoundinvoice = DateTime.ParseExact(invoice.AddServicesDate.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                DateofServicesAreComplete = invoice.AddServicesDate;
            }

            if (invoice.ExpensePrevious > 0)
            {
                IsPreviousExpenseVis = true;
                //PreviousExpensesPrevious = first.ExpensesPrevious;
                PreviousExpensesInvoicedToDate = invoice.ExpensePrevious;
                DatePreviousExpenses = invoice.DatePrevExpenses;
            }

            EmployeeDbModel signer = SQLAccess.LoadEmployeeById(invoice.EmployeeSignedId);

            if (signer != null)
            {
                EmployeeLowResModel elrsm = new EmployeeLowResModel(signer);
                ProjectManagers = new ObservableCollection<EmployeeLowResModel>(new List<EmployeeLowResModel>() { elrsm });
                SelectedPM = elrsm;
            }

            List<InvoicingRowsDb> rows = SQLAccess.LoadInvoiceRows(invoice.Id);

            List<TimesheetRowDbModel> timesheettime = new List<TimesheetRowDbModel>();

            foreach (int timeid in invoice.TimesheetIds)
            {
                TimesheetRowDbModel trdbm = SQLAccess.LoadTimeSheetDatabyId(timeid);
                timesheettime.Add(trdbm);
            }

            foreach (InvoicingRowsDb rowoi in rows)
            {
                InvoicingRows irn = new InvoicingRows(rowoi);
                irn.viewmodel = this;

                if (!Convert.ToBoolean(rowoi.IsExpense))
                {
                    SubProjectDbModel sub = SQLAccess.LoadSubProjectsById(rowoi.SubId);

                    List<TimesheetRowDbModel> trdbmforsub = timesheettime.Where(x => x.SubProjectId == sub.Id).ToList();
                    double budgetspent = trdbmforsub.Sum(x => x.BudgetSpent);

                    irn.IsBillable = Convert.ToBoolean(sub.IsBillable);
                    irn.BudgetSpent = budgetspent;
                    irn.ContractFee = sub.Fee;
                    irn.IsHourly = Convert.ToBoolean(sub.IsHourly);
                    irn.PercentComplete = rowoi.PercentComplete;


                    if (sub.IsAdservice == 1)
                    {
                        if (rowoi.PercentComplete == 100 && rowoi.ThisPeriodInvoiced == 0)
                        {
                            previousinvoices = SQLAccess.LoadInvoices(BaseProject.Id);
                            int numinvoices = previousinvoices.Count() + 1;
                            List<InvoicingModelDb> sortedprevious = previousinvoices.OrderBy(x => x.Date).ToList();
                            InvoicingModelDb first = sortedprevious[0];
                            irn.DateOfInvoice = DateTime.ParseExact(first.Date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MMMM dd, yyyy");
                            AdServicePhasesInvoiced.Add(irn);
                        }
                        else
                        {
                            AdServicePhasesUnInvoiced.Add(irn);
                        }
                    }
                    else
                    {
                        irn.IsHourly = Convert.ToBoolean(sub.IsHourly);
                        NonAdServicePhases.Add(irn);
                    }
                }
                else
                {
                    irn.InvoicedtoDate = irn.ThisPeriodInvoiced + irn.PreviousInvoiced;
                    irn.IsHourly = true;
                    //irn.BudgetSpent = irn.BudgetSpent;
                    Expenses.Add(irn);
                    //ExpenseEnum enumtofind = (ExpenseEnum)rowoi.ExpenseEnum;    
                }

            }
            SumValues();
        }

        public void ClickInvoice()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            //dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;

            // Process open file dialog box results
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                //save file
                LinkLocation = dialog.FileName;
            }
        }

        public void LoadInfo()
        {
            AdServicePhasesInvoiced.Clear();
            AdServicePhasesUnInvoiced.Clear();
            NonAdServicePhases.Clear();

            previousinvoices = SQLAccess.LoadInvoices(BaseProject.Id);
            int numinvoices = previousinvoices.Count() + 1;
            int numinvoice = 0;
            List<InvoicingModelDb> sortedprevious = previousinvoices.OrderBy(x => x.InvoiceNumber).ToList();
            if (previousinvoices.Count > 0)
            {
                InvoicingModelDb first = sortedprevious.Last();
                ClientAddressInp = first.ClientAddress;
                ClientNameInp = first.ClientName;
                ClientCompanyNameInp = first.ClientCompany;
                ClientCityInp = first.ClientCity;
                LinkLocation = first.Link;
                numinvoice = first.InvoiceNumber - (BaseProject.ProjectNumber * 1000);

                if ((first.ExpensesDue) > 0)
                {
                    IsPreviousExpenseVis = true;
                    //PreviousExpensesPrevious = first.ExpensesPrevious;
                    PreviousExpensesInvoicedToDate = first.ExpensesDue;
                    DatePreviousExpenses = DateTime.ParseExact(first.Date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                }
            }
            else
            {
                List<SubProjectDbModel> subs = SQLAccess.LoadSubProjectsByProject(BaseProject.Id);
                bool found = false;
                foreach (SubProjectDbModel sub in subs)
                {
                    if (!String.IsNullOrEmpty(sub.NameOfClient) || !String.IsNullOrEmpty(sub.ClientCompanyName) || !String.IsNullOrEmpty(sub.ClientAddress) || !String.IsNullOrEmpty(sub.ClientCity))
                    {
                        ClientAddressInp = sub.ClientAddress;
                        ClientNameInp = sub.NameOfClient;
                        ClientCompanyNameInp = sub.ClientCompanyName;
                        ClientCityInp = sub.ClientCity;

                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    ProjectDbModel proj = SQLAccess.LoadProjectsById(BaseProject.Id);
                    ClientDbModel client = SQLAccess.LoadClientById(proj.ClientId);
                    if (client != null)
                    {
                        //ClientDbModel client = SQLAccess.LoadClientById(BaseProject.Client.Id);

                        if (client != null)
                        {
                            ClientAddressInp = client.ClientAddress;
                            ClientNameInp = client.NameOfClient;
                            ClientCompanyNameInp = client.ClientName;
                            ClientCityInp = client.ClientCity;
                        }
                    }

                }
            }

            List<SubProjectDbModel> subforinvoice = SQLAccess.LoadSubProjectsByProject(BaseProject.Id);

            foreach (SubProjectDbModel sub in subforinvoice)
            {
                if (sub.IsBillable == 1)
                {
                    double percentcomplete = 0;
                    double previousinvoiced = 0;
                    double ThisPeriodInvoiced = 0;
                    string scopename = "";
                    bool iseditable = true;
                    bool foundprevious = false;
                    string datefoundinvoice = "";
                    double basepercentcomplete = 0;

                    if (previousinvoices.Count > 0)
                    {
                        InvoicingModelDb first = null;
                        InvoicingRowsDb founditem = null;

                        foreach (InvoicingModelDb invsingle in sortedprevious)
                        {
                            InvoicingRowsDb rowfound = SQLAccess.LoadInvoiceRowsByInvoiceAndSubId(sub.Id, invsingle.Id);

                            if (first == null || founditem == null || rowfound.PercentComplete > founditem.PercentComplete || sub.IsHourly == 1)
                            {
                                first = invsingle;
                                founditem = rowfound;
                            }
                        }

                        if (founditem != null)
                        {
                            previousinvoiced = founditem.ThisPeriodInvoiced + founditem.PreviousInvoiced;
                            scopename = founditem.ScopeName;
                            basepercentcomplete = Math.Max(founditem.PercentComplete, sub.PercentComplete);

                            if (founditem.PercentComplete == 100)
                            {
                                foundprevious = true;
                                datefoundinvoice = DateTime.ParseExact(first.Date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MMMM dd, yyyy");
                                iseditable = false;
                                percentcomplete = 100;
                            }
                        }
                        else
                        {
                            previousinvoiced = 0;
                            scopename = sub.Description;
                        }
                    }
                    else
                    {
                        previousinvoiced = 0;
                        scopename = sub.Description;
                    }

                    double budgetspent = 0;

                    if (timesheetidstoinvoice != null && timesheetidstoinvoice.Count > 0)
                    {
                        List<HourEntryModel> hours = timesheetidstoinvoice.Where(x => x.SubId == sub.Id).ToList();


                        if (hours.Count > 0)
                        {
                            budgetspent = hours.Sum(x => x.BudgetSpent);
                        }

                        if (iseditable)
                        {
                            if (sub.Fee != 0 && !Convert.ToBoolean(sub.IsHourly))
                            {
                                double estimatedpercent = ((previousinvoiced + budgetspent) / sub.Fee) * 100;
                                double roundedestimatepercent = Math.Round(estimatedpercent / 5.0) * 5;
                                double estimatedperiodinvoiced = (roundedestimatepercent / 100 * sub.Fee) - previousinvoiced;
                                ThisPeriodInvoiced = Math.Min(sub.Fee - previousinvoiced, estimatedperiodinvoiced);
                                percentcomplete = Math.Min(roundedestimatepercent, 100);
                            }
                            else
                            {
                                ThisPeriodInvoiced = budgetspent;
                                percentcomplete = basepercentcomplete;
                            }

                        }
                    }

                    InvoicingRows row = new InvoicingRows()
                    {
                        SubId = sub.Id,
                        viewmodel = this,
                        PreviousInvoiced = previousinvoiced,
                        ScopeName = scopename,
                        ContractFee = sub.Fee,
                        BudgetSpent = budgetspent,
                        IsBillable = Convert.ToBoolean(sub.IsBillable),
                        IsHourly = Convert.ToBoolean(sub.IsHourly),
                        BasePercentComplete = basepercentcomplete
                    };

                    row.ThisPeriodInvoiced = ThisPeriodInvoiced;
                    row.PercentComplete = percentcomplete;

                    if (sub.IsAdservice == 1)
                    {
                        if (foundprevious)
                        {
                            row.DateOfInvoice = datefoundinvoice;
                            AdServicePhasesInvoiced.Add(row);
                        }
                        else
                        {
                            AdServicePhasesUnInvoiced.Add(row);
                        }
                    }
                    else
                    {
                        NonAdServicePhases.Add(row);
                    }
                }
            }

            //if (sortedprevious.Count > 0)
            //{
            //foreach (InvoicingModelDb invmod in sortedprevious)
            //{
            //    //InvoicingModelDb latestinvoice = sortedprevious.Last();
            //    List<InvoicingRowsDb> expenses = SQLAccess.LoadInvoiceRows(invmod.Id);

            //    foreach (InvoicingRowsDb inv in expenses)
            //    {
            //        if (Convert.ToBoolean(inv.IsExpense))
            //        {
            //            InvoicingRows eim = new InvoicingRows()
            //            {
            //                viewmodel = this,
            //                PreviousInvoiced = inv.PreviousInvoiced + inv.ThisPeriodInvoiced,
            //                ThisPeriodInvoiced = 0,
            //                InvoicedtoDate = inv.PreviousInvoiced + inv.ThisPeriodInvoiced,
            //                ScopeName = inv.ScopeName,
            //                IsHourly = true,
            //            };

            //            //List<InvoicingRows> exfound = Expenses.Where(x => x.ScopeName == inv.ScopeName).ToList();

            //            //if (exfound.Count > 0)
            //            //{
            //            //    InvoicingRows exitem = exfound.OrderByDescending(x => x.InvoicedtoDate).FirstOrDefault();
            //            //    int index = Expenses.ToList().FindIndex(s => s == exitem);

            //            //    if (index != -1)
            //            //    {
            //            //        Expenses[index] = exitem;
            //            //    }
            //            //}
            //            //else
            //            //{
            //                Expenses.Add(eim);

            //            //}


            //        }
            //    }
            //}

            //}

            foreach (ExpenseInvoiceModel eim in selectedexpenses)
            {
                //InvoicingRows found = Expenses.Where(x => x.ScopeName == eim.DescriptionExp).FirstOrDefault();

                //if (found != null)
                //{
                //    found.PreviousInvoiced = found.InvoicedtoDate;
                //    found.ThisPeriodInvoiced = eim.TotalCost;
                //    found.InvoicedtoDate = found.PreviousInvoiced + found.ThisPeriodInvoiced;
                //}
                //else
                //{
                    InvoicingRows eimnew = new InvoicingRows()
                    {
                        viewmodel = this,
                        PreviousInvoiced = 0,
                        BudgetSpent = eim.TotalCost,
                        ThisPeriodInvoiced = eim.TotalCost,
                        InvoicedtoDate = eim.TotalCost,
                        ScopeName = eim.DescriptionExp,
                        IsHourly = true,
                    };
                    Expenses.Add(eimnew);
                //}

            }

            InvoiceNumberInp = (BaseProject.ProjectNumber * 1000) + numinvoice + 1;

            SumValues();
        }

        public void SumValues()
        {
            TotalContractFee = 0;
            InvoicedToDateTotal = 0;
            PreviousTotal = 0;
            ThisPeriodTotal = 0;
            ThisPeriodTotalToDate = 0;
            TotalAmountDue = 0;
            InvoicedToDateTotalToDate = 0;
            PreviousTotalToDate = 0;
            InvoicedToDateTotalAd = 0;
            PreviousTotalAd = 0;
            ThisPeriodTotalAd = 0;
            TotalContractAdFee = 0;
            ThisPeriodSpent = 0;
            ReimbursablePrevious = 0;
            ReimbursableThisPeriod = 0;
            ReimbursableToDate = 0;

            foreach (InvoicingRows row in NonAdServicePhases)
            {
                //if (row.IsBillable)
                //{
                if (!row.IsHourly)
                {
                    TotalContractFee += row.ContractFee;
                }
                InvoicedToDateTotal += row.InvoicedtoDate;
                InvoicedToDateTotalToDate += row.InvoicedtoDate;
                PreviousTotal += row.PreviousInvoiced;
                PreviousTotalToDate += row.PreviousInvoiced;
                ThisPeriodTotal += row.ThisPeriodInvoiced;
                ThisPeriodTotalToDate += row.ThisPeriodInvoiced;
                TotalAmountDue += row.ThisPeriodInvoiced;
                ThisPeriodSpent += row.BudgetSpent;
                //}
            }

            foreach (InvoicingRows row in AdServicePhasesInvoiced)
            {
                //if (row.IsBillable)
                //{
                InvoicedToDateTotalToDate += row.InvoicedtoDate;
                PreviousTotalToDate += row.PreviousInvoiced;
                TotalAmountDue += row.ThisPeriodInvoiced;
                ThisAdPeriodSpent += row.BudgetSpent;
                //}
            }

            foreach (InvoicingRows row in AdServicePhasesUnInvoiced)
            {
                //if (row.IsBillable)
                //{
                if (!row.IsHourly)
                {
                    TotalContractAdFee += row.ContractFee;
                }
                InvoicedToDateTotalAd += row.InvoicedtoDate;
                InvoicedToDateTotalToDate += row.InvoicedtoDate;
                PreviousTotalAd += row.PreviousInvoiced;
                PreviousTotalToDate += row.PreviousInvoiced;
                ThisPeriodTotalAd += row.ThisPeriodInvoiced;
                ThisPeriodTotalToDate += row.ThisPeriodInvoiced;
                TotalAmountDue += row.ThisPeriodInvoiced;
                //}
            }

            foreach (InvoicingRows row in Expenses)
            {
                ReimbursableToDate += row.InvoicedtoDate;
                ReimbursablePrevious += row.PreviousInvoiced;
                ReimbursableThisPeriod += row.ThisPeriodInvoiced;
                TotalAmountDue += row.ThisPeriodInvoiced;
            }

            IsPreviousAdServiceVis = AdServicePhasesInvoiced.Count > 0 ? true : false;
            DoBasicServicesExist = NonAdServicePhases.Count > 0 ? true : false;
            DoAdservicesExist = AdServicePhasesUnInvoiced.Count > 0 ? true : false;

            IsTotalContractAdFee = TotalContractAdFee == 0 ? false : true;
            IsTotalContractFee = TotalContractFee == 0 ? false : true;

            IsReimbursablesVis = Expenses.Count() > 0;
        }

        public void CreateInvoice()
        {
            if (TotalAmountDue <= 0)
            {
                ErrorMessage = $"Total amount to invoice needs{Environment.NewLine} to be greater than 0";
                return;
            }

            if (String.IsNullOrEmpty(LinkLocation))
            {
                ErrorMessage = $"Specify an invoice location{Environment.NewLine} before creating the invoice.";
                return;
            }

            if (SelectedPM == null)
            {
                ErrorMessage = $"Specify a project manager{Environment.NewLine} before creating the invoice.";
                return;
            }

            int duedatevar = 0;
            int addates = 0;
            int dateexpprev = 0;

            if (InvoiceDateInp != null)
            {
                duedatevar = (int)long.Parse(InvoiceDateInp?.ToString("yyyyMMdd"));
            }

            if (DateofServicesAreComplete != null)
            {
                addates = (int)long.Parse(DateofServicesAreComplete?.ToString("yyyyMMdd"));
            }

            if (DatePreviousExpenses != null)
            {
                dateexpprev = (int)long.Parse(DatePreviousExpenses.ToString("yyyyMMdd"));
            }

            //var bytes = timesheetidstoinvoice.Select(i => BitConverter.GetBytes(i.TimeId)).ToArray();
            int[] timeids = timesheetidstoinvoice.Select(x => x.TimeId).ToArray();
            string result = string.Join(",", timeids);

            int[] expenseids = selectedexpenses.Select(x => x.Id).ToArray();
            string result2 = string.Join(",", expenseids);

            double previousspent = BaseProject.TotalFeeInvoiced;

            if (IsRevised)
            {
                previousspent = revisedinvoice.PreviousSpent;
            }

            double expdue = Expenses.Sum(x => x.ThisPeriodInvoiced);

            InvoicingModelDb invoice = new InvoicingModelDb()
            {
                ProjectId = BaseProject.Id,
                InvoiceNumber = InvoiceNumberInp,
                Date = duedatevar,
                ClientName = ClientNameInp,
                ClientCompany = ClientCompanyNameInp,
                ClientAddress = ClientAddressInp,
                ClientCity = ClientCityInp,
                PreviousSpent = previousspent,
                AmountDue = TotalAmountDue,
                EmployeeSignedId = SelectedPM.Id,
                TimesheetIds = result,
                AddServicesDate = addates,
                ExpenseReportIds = result2,
                Link = LinkLocation,
                ExpensesDue = expdue + PreviousExpensesInvoicedToDate,
                ExpensesPrevious = PreviousExpensesInvoicedToDate,
                IsRevised = Convert.ToInt32(IsRevised),
                ExpensePreviousDate = dateexpprev,
            };

            if (Directory.Exists(LinkLocation))
            {
                string projname = InvoiceNumberInp.ToString();
                if (IsRevised)
                {
                    projname = projname + "R";
                }

                string generalpath = LinkLocation + "\\" + projname + " - Invoice - " + ProjectNameInp + ".xlsx";

                File.WriteAllBytes(generalpath, Properties.Resources.InvoiceTemplate);
                CreateLog(generalpath);
            }

            int id = SQLAccess.AddInvoice(invoice);

            if (id > -1)
            {
                //AdServicePhasesInvoiced.Add(row);

                //AdServicePhasesUnInvoiced.Add(row);

                //        NonAdServicePhases.Add(row);

                foreach (InvoicingRows nonadrow in NonAdServicePhases)
                {
                    //if (nonadrow.IsBillable)
                    //{
                    InvoicingRowsDb invoicerow = new InvoicingRowsDb()
                    {
                        SubId = nonadrow.SubId,
                        InvoiceId = id,
                        PercentComplete = nonadrow.PercentComplete,
                        PreviousInvoiced = nonadrow.PreviousInvoiced,
                        ThisPeriodInvoiced = nonadrow.ThisPeriodInvoiced,
                        ScopeName = nonadrow.ScopeName,
                    };
                    SQLAccess.AddInvoiceRow(invoicerow);

                    SubProjectDbModel sub = SQLAccess.LoadSubProjectsById(nonadrow.SubId);

                    if (sub.Fee != nonadrow.ContractFee && !nonadrow.IsHourly)
                    {
                        SQLAccess.UpdateSubFee(nonadrow.SubId, nonadrow.ContractFee);
                    }

                    if (sub.PercentComplete < nonadrow.PercentComplete)
                    {
                        SQLAccess.UpdatePercentComplete(nonadrow.SubId, nonadrow.PercentComplete);
                    }

                    if (nonadrow.PercentComplete == 100 || (sub.IsHourly == 1 && nonadrow.ThisPeriodInvoiced > 0))
                    {
                        SQLAccess.UpdateInvoiced(nonadrow.SubId, 1, duedatevar, 100);
                    }

                    if (nonadrow.IsHourly)
                    {
                        SQLAccess.UpdateFee(BaseProject.Id, BaseProject.Fee + nonadrow.ThisPeriodInvoiced);
                        SQLAccess.UpdateSubFee(nonadrow.SubId, nonadrow.InvoicedtoDate);
                    }
                    //}
                }

                foreach (InvoicingRows adrow in AdServicePhasesUnInvoiced)
                {
                    //if (adrow.IsBillable)
                    //{
                    InvoicingRowsDb invoicerow = new InvoicingRowsDb()
                    {
                        SubId = adrow.SubId,
                        InvoiceId = id,
                        PercentComplete = adrow.PercentComplete,
                        PreviousInvoiced = adrow.PreviousInvoiced,
                        ThisPeriodInvoiced = adrow.ThisPeriodInvoiced,
                        ScopeName = adrow.ScopeName,
                    };
                    SQLAccess.AddInvoiceRow(invoicerow);

                    SubProjectDbModel sub = SQLAccess.LoadSubProjectsById(adrow.SubId);

                    if (sub.Fee != adrow.ContractFee && !adrow.IsHourly)
                    {
                        SQLAccess.UpdateSubFee(adrow.SubId, adrow.ContractFee);
                    }

                    if (sub.PercentComplete < adrow.PercentComplete)
                    {
                        SQLAccess.UpdatePercentComplete(adrow.SubId, adrow.PercentComplete);
                    }

                    if (adrow.PercentComplete == 100 || (sub.IsHourly == 1 && adrow.ThisPeriodInvoiced > 0))
                    {
                        SQLAccess.UpdateInvoiced(adrow.SubId, 1, duedatevar, 100);
                    }

                    if (adrow.IsHourly)
                    {
                        SQLAccess.UpdateFee(BaseProject.Id, BaseProject.Fee + adrow.ThisPeriodInvoiced);
                        SQLAccess.UpdateSubFee(adrow.SubId, adrow.InvoicedtoDate);
                    }

                }

                foreach (InvoicingRows adrow in AdServicePhasesInvoiced)
                {
                    //if (adrow.IsBillable)
                    //{
                    InvoicingRowsDb invoicerow = new InvoicingRowsDb()
                    {
                        SubId = adrow.SubId,
                        InvoiceId = id,
                        PercentComplete = adrow.PercentComplete,
                        PreviousInvoiced = adrow.PreviousInvoiced,
                        ThisPeriodInvoiced = adrow.ThisPeriodInvoiced,
                        ScopeName = adrow.ScopeName,
                        IsExpense = 0,
                        //ExpenseEnum = 0
                    };
                    SQLAccess.AddInvoiceRow(invoicerow);
                    //SQLAccess.UpdatePercentComplete(adrow.SubId, adrow.PercentComplete);
                    //}
                }

                foreach (InvoicingRows adrow in Expenses)
                {
                    //if (adrow.IsBillable)
                    //{
                    InvoicingRowsDb invoicerow = new InvoicingRowsDb()
                    {
                        SubId = 0,
                        InvoiceId = id,
                        PercentComplete = 0,
                        PreviousInvoiced = adrow.PreviousInvoiced,
                        ThisPeriodInvoiced = adrow.ThisPeriodInvoiced,
                        ScopeName = adrow.ScopeName,
                        IsExpense = 1,
                        //ExpenseEnum = 0
                    };
                    SQLAccess.AddInvoiceRow(invoicerow);
                    //SQLAccess.UpdatePercentComplete(adrow.SubId, adrow.PercentComplete);
                    //}
                }

                foreach (HourEntryModel idval in timesheetidstoinvoice)
                {
                    SQLAccess.UpdateInvoiceStatusTime(idval.TimeId, 1);
                }

                foreach (ExpenseInvoiceModel eim in selectedexpenses)
                {
                    SQLAccess.UpdateInvoiced(eim.Id, 1);
                }

                List<SubProjectSummaryModel> subs = new List<SubProjectSummaryModel>();
                List<SubProjectDbModel> subdbmodels = SQLAccess.LoadSubProjectsByProject(BaseProject.Id);

                double totalfee = 0;

                foreach (SubProjectDbModel spdm in subdbmodels)
                {
                    if (Convert.ToBoolean(spdm.IsBillable))
                    {
                        totalfee += spdm.Fee;
                    }
                }
                //update overall fee
                if (BaseProject.Fee != totalfee)
                {
                    SQLAccess.UpdateFee(BaseProject.Id, totalfee);
                }

                Process.Start(LinkLocation);
                //Process.Start(adservicefolderpath);
            }
            CloseWindow();
        }

        private void CreateLog(string path)
        {
            //List<SubProjectAddServiceModel> SubsOrdered = SubProjects.ToList().OrderBy(t => Convert.ToDouble(t.PointNumber)).ToList();
            //List<foundpage> foundpages = new List<foundpage>();
            try
            {
                Excel.Excel exinst = new Excel.Excel(path);
                Thread.Sleep(200);
                string invnum = InvoiceNumberInp.ToString();
                if (IsRevised)
                {
                    invnum = invnum + "R";
                }
                exinst.WriteCell(4, 7, invnum);

                if (IsRevised)
                {
                    exinst.WriteCell(6, 1, InvoiceDateInp?.ToString("MMMM dd, yyyy") + " (Revised)");
                    exinst.WriteCell(7, 1, InvoiceDateInp?.ToString("MMMM dd, yyyy"));
                }
                else
                {
                    exinst.WriteCell(7, 1, InvoiceDateInp?.ToString("MMMM dd, yyyy"));
                }

                exinst.WriteCell(10, 1, ClientNameInp);
                exinst.WriteCell(11, 1, ClientCompanyNameInp);
                exinst.WriteCell(12, 1, ClientAddressInp);
                exinst.WriteCell(13, 1, ClientCityInp);
                exinst.WriteCell(17, 2, ProjectNameInp);
                //exinst.WriteCell(17, 7, ProjectNumberInp.ToString());
                int row = 22;

                foreach (InvoicingRows nonadrow in NonAdServicePhases)
                {
                    exinst.MergeCellsInRow(row, 1, 2);
                    exinst.WriteCell(row, 1, nonadrow.ScopeName);

                    if (nonadrow.IsHourly)
                    {
                        exinst.WriteCell(row, 3, "HOURLY");
                    }
                    else
                    {
                        exinst.SetCellAsAccounting(row, 3);
                        exinst.WriteCell(row, 3, nonadrow.ContractFee.ToString());
                        double percent = nonadrow.PercentComplete / 100;
                        exinst.SetCellAsPercentage(row, 4);
                        exinst.WriteCell(row, 4, percent.ToString());
                    }
                    exinst.SetCellAsAccounting(row, 5);
                    exinst.WriteCell(row, 5, nonadrow.InvoicedtoDate.ToString());
                    exinst.SetCellAsAccounting(row, 6);
                    exinst.WriteCell(row, 6, nonadrow.PreviousInvoiced.ToString());
                    exinst.SetCellAsAccounting(row, 7);
                    exinst.WriteCell(row, 7, nonadrow.ThisPeriodInvoiced.ToString());
                    exinst.InsertBlankRowBelow(row);

                    //exinst.SaveDocument();
                    row++;
                }

                if (NonAdServicePhases.Count > 0)
                {
                    exinst.MergeCellsInRow(row, 1, 2);
                    exinst.WriteCell(row, 1, "Total Basic Services");
                    exinst.SetCellAsAccounting(row, 3);
                    exinst.WriteFormula(row, 3, $"SUM(C22:C{row - 1})");
                    exinst.SetCellAsAccounting(row, 5);
                    exinst.WriteFormula(row, 5, $"SUM(E22:E{row - 1})");
                    exinst.SetCellAsAccounting(row, 6);
                    exinst.WriteFormula(row, 6, $"SUM(F22:F{row - 1})");
                    exinst.SetCellAsAccounting(row, 7);
                    exinst.WriteFormula(row, 7, $"SUM(G22:G{row - 1})");
                    exinst.InsertBlankRowBelow(row);
                    row++;
                    //exinst.InsertBlankRowBelow(row);
                    exinst.MakeRowLightBorderTop(row - 1, 1, 7);
                    exinst.InsertBlankRowBelow(row);
                    row++;
                }

                if (AdServicePhasesUnInvoiced.Count > 0)
                {
                    //exinst.InsertBlankRowBelow(row);
                    //row++;
                    exinst.MergeCellsInRow(row, 1, 2);
                    exinst.WriteCell(row, 1, "Additional Services");
                    exinst.InsertBlankRowBelow(row);
                    row++;
                    exinst.MergeCellsInRow(row, 1, 2);
                    exinst.WriteCell(row, 1, $"Services Through {DateofServicesAreComplete?.ToString("MMMM dd, yyyy")}");
                    exinst.InsertBlankRowBelow(row);
                    row++;
                }

                foreach (InvoicingRows adrowun in AdServicePhasesUnInvoiced)
                {
                    exinst.MergeCellsInRow(row, 1, 2);
                    exinst.WriteCell(row, 1, adrowun.ScopeName);

                    if (adrowun.IsHourly)
                    {
                        exinst.WriteCell(row, 3, "HOURLY");
                    }
                    else
                    {
                        exinst.SetCellAsAccounting(row, 3);
                        exinst.WriteCell(row, 3, adrowun.ContractFee.ToString());
                    }
                    double percent = adrowun.PercentComplete / 100;
                    exinst.SetCellAsPercentage(row, 4);
                    exinst.WriteCell(row, 4, percent.ToString());
                    exinst.SetCellAsAccounting(row, 5);
                    exinst.WriteCell(row, 5, adrowun.InvoicedtoDate.ToString());
                    exinst.SetCellAsAccounting(row, 6);
                    exinst.WriteCell(row, 6, adrowun.PreviousInvoiced.ToString());
                    exinst.SetCellAsAccounting(row, 7);
                    exinst.WriteCell(row, 7, adrowun.ThisPeriodInvoiced.ToString());
                    exinst.InsertBlankRowBelow(row);
                    //exinst.SaveDocument();
                    row++;
                }

                if (AdServicePhasesUnInvoiced.Count > 0)
                {
                    exinst.MergeCellsInRow(row, 1, 2);
                    exinst.WriteCell(row, 1, "Total Additional Services This Invoice");
                    exinst.SetCellAsAccounting(row, 3);
                    exinst.WriteFormula(row, 3, $"SUM(C{row - AdServicePhasesUnInvoiced.Count}:C{row - 1})");
                    exinst.SetCellAsAccounting(row, 5);
                    exinst.WriteFormula(row, 5, $"SUM(E{row - AdServicePhasesUnInvoiced.Count}:E{row - 1})");
                    exinst.SetCellAsAccounting(row, 6);
                    exinst.WriteFormula(row, 6, $"SUM(F{row - AdServicePhasesUnInvoiced.Count}:F{row - 1})");
                    exinst.SetCellAsAccounting(row, 7);
                    exinst.WriteFormula(row, 7, $"SUM(G{row - AdServicePhasesUnInvoiced.Count}:G{row - 1})");
                    exinst.InsertBlankRowBelow(row);
                    row++;
                    //exinst.InsertBlankRowBelow(row);
                    exinst.MakeRowLightBorderTop(row - 1, 1, 7);
                    exinst.InsertBlankRowBelow(row);
                    row++;
                }

                if (AdServicePhasesInvoiced.Count > 0)
                {
                    exinst.InsertBlankRowBelow(row);
                    row++;
                    exinst.MergeCellsInRow(row, 1, 2);
                    exinst.WriteCell(row, 1, "Previous Additional Services");
                    exinst.InsertBlankRowBelow(row);
                    row++;
                }

                foreach (InvoicingRows adrownonun in AdServicePhasesInvoiced)
                {
                    exinst.MergeCellsInRow(row, 1, 2);
                    exinst.WriteCell(row, 1, $"Services Through {adrownonun.DateOfInvoice}");

                    exinst.SetCellAsAccounting(row, 5);
                    exinst.WriteCell(row, 5, adrownonun.InvoicedtoDate.ToString());
                    exinst.SetCellAsAccounting(row, 6);
                    exinst.WriteCell(row, 6, adrownonun.PreviousInvoiced.ToString());
                    exinst.SetCellAsAccounting(row, 7);
                    exinst.WriteCell(row, 7, adrownonun.ThisPeriodInvoiced.ToString());
                    exinst.InsertBlankRowBelow(row);
                    //exinst.SaveDocument();
                    row++;
                }

                if (AdServicePhasesInvoiced.Count > 0)
                {
                    exinst.MergeCellsInRow(row, 1, 2);
                    exinst.WriteCell(row, 1, "Total Services To Date");
                    exinst.SetCellAsAccounting(row, 3);
                    exinst.WriteFormula(row, 3, $"SUM(C{row - AdServicePhasesInvoiced.Count}:C{row - 1})");
                    exinst.SetCellAsAccounting(row, 5);
                    exinst.WriteFormula(row, 5, $"SUM(E{row - AdServicePhasesInvoiced.Count}:E{row - 1})");
                    exinst.SetCellAsAccounting(row, 6);
                    exinst.WriteFormula(row, 6, $"SUM(F{row - AdServicePhasesInvoiced.Count}:F{row - 1})");
                    exinst.SetCellAsAccounting(row, 7);
                    exinst.WriteFormula(row, 7, $"SUM(G{row - AdServicePhasesInvoiced.Count}:G{row - 1})");
                    exinst.InsertBlankRowBelow(row);
                    row++;
                    //exinst.InsertBlankRowBelow(row);
                    exinst.MakeRowLightBorderTop(row - 1, 1, 7);
                    exinst.InsertBlankRowBelow(row);
                    row++;
                }

                if (Expenses.Count > 0)
                {
                    //exinst.InsertBlankRowBelow(row);
                    //row++;
                    exinst.MergeCellsInRow(row, 1, 2);
                    exinst.WriteCell(row, 1, "Reimbursable Expenses");
                    exinst.InsertBlankRowBelow(row);
                    row++;
                }

                foreach (InvoicingRows adrownonun in Expenses)
                {
                    exinst.MergeCellsInRow(row, 1, 2);
                    exinst.WriteCell(row, 1, adrownonun.ScopeName);
                    exinst.SetCellAsAccounting(row, 5);
                    exinst.WriteCell(row, 5, adrownonun.InvoicedtoDate.ToString());
                    exinst.SetCellAsAccounting(row, 6);
                    exinst.WriteCell(row, 6, adrownonun.PreviousInvoiced.ToString());
                    exinst.SetCellAsAccounting(row, 7);
                    exinst.WriteCell(row, 7, adrownonun.ThisPeriodInvoiced.ToString());
                    exinst.InsertBlankRowBelow(row);
                    //exinst.SaveDocument();
                    row++;
                }

                if (Expenses.Count > 0)
                {
                    exinst.MergeCellsInRow(row, 1, 2);
                    exinst.WriteCell(row, 1, "Total Reimbursable Expenses");
                    exinst.SetCellAsAccounting(row, 5);
                    exinst.WriteFormula(row, 5, $"SUM(E{row - Expenses.Count}:E{row - 1})");
                    exinst.SetCellAsAccounting(row, 6);
                    exinst.WriteFormula(row, 6, $"SUM(F{row - Expenses.Count}:F{row - 1})");
                    exinst.SetCellAsAccounting(row, 7);
                    exinst.WriteFormula(row, 7, $"SUM(G{row - Expenses.Count}:G{row - 1})");
                    exinst.InsertBlankRowBelow(row);
                    row++;
                    //exinst.InsertBlankRowBelow(row);
                    exinst.MakeRowLightBorderTop(row - 1, 1, 7);
                    exinst.InsertBlankRowBelow(row);
                    row++;
                }

                if (IsPreviousExpenseVis)
                {
                    exinst.MergeCellsInRow(row, 1, 2);
                    exinst.WriteCell(row, 1, "Previous Reimbursable Expenses");
                    exinst.InsertBlankRowBelow(row);
                    row++;
                    exinst.MergeCellsInRow(row, 1, 2);
                    exinst.WriteCell(row, 1, $"Through {DatePreviousExpenses.ToString("MMMM dd, yyyy")}");
                    exinst.SetCellAsAccounting(row, 5);
                    exinst.WriteCell(row, 5, PreviousExpensesInvoicedToDate.ToString());
                    exinst.SetCellAsAccounting(row, 6);
                    exinst.WriteCell(row, 6, PreviousExpensesInvoicedToDate.ToString());
                    exinst.InsertBlankRowBelow(row);
                    row++;

                    exinst.MergeCellsInRow(row, 1, 3);
                    exinst.WriteCell(row, 1, "Previous Reimbursable Expenses To Date");
                    exinst.SetCellAsAccounting(row, 5);
                    exinst.WriteCell(row, 5, PreviousExpensesInvoicedToDate.ToString());
                    exinst.SetCellAsAccounting(row, 6);
                    exinst.WriteCell(row, 6, PreviousExpensesInvoicedToDate.ToString());                   
                    exinst.InsertBlankRowBelow(row);
                    row++;
                    exinst.MakeRowLightBorderTop(row - 1, 1, 7);
                    //exinst.InsertBlankRowBelow(row);
                    //exinst.MakeRowLightBorderTop(row - 1, 1, 7);
                    //exinst.InsertBlankRowBelow(row);

                }
                row++;
                exinst.SetCellAsAccounting(row, 7);
                exinst.WriteCell(row, 7, TotalAmountDue.ToString());
                exinst.WriteCell(row + 5, 1, SelectedPM.FullName + ", P.E.");
                exinst.SaveDocument();

                if (SelectedPM.SignatureOfPM != null)
                {
                    exinst.AddPicture(row+3, 1, SelectedPM.SignatureOfPM, 100);
                    exinst.SaveDocument();
                }
                exinst.Close();
            }
            catch
            {
                ErrorMessage = "Error has occured, double check existing add-service files are not open.";
            }
        }

        private void LoadProjectManagers()
        {
            List<EmployeeDbModel> PMs = SQLAccess.LoadProjectManagers();

            ObservableCollection<EmployeeLowResModel> members = new ObservableCollection<EmployeeLowResModel>();

            foreach (EmployeeDbModel edbm in PMs)
            {
                members.Add(new EmployeeLowResModel(edbm));
            }

            ProjectManagers = members;

            if (BaseProject.ProjectManager != null)
            {
                EmployeeLowResModel foundpm = ProjectManagers.Where(x => x.Id == BaseProject.ProjectManager.Id).FirstOrDefault();

                if (foundpm != null)
                {
                    SelectedPM = foundpm;
                }
                else
                {
                    SelectedPM = ProjectManagers.FirstOrDefault();
                }
            }
        }

        private void CloseWindow()
        {
            baseViewModel.Reload(BaseProject.Id, BaseProject.ProjectManager);
            //baseViewModel.LoadAdservice();
            baseViewModel.LeftDrawerOpen = false;
        }
    }
}
