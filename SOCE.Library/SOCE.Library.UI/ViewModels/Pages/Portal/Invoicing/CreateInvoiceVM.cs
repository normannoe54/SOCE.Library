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

        private InvoicingSummaryVM baseViewModel;

        public List<HourEntryModel> timesheetidstoinvoice;
        public ICommand CreateInvoiceCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public List<InvoicingModelDb> previousinvoices { get; set; } = new List<InvoicingModelDb>();
        public CreateInvoiceVM(ProjectInvoicingModel pm, InvoicingSummaryVM psvm, double hours, double budget, DateTime dateinitial, List<HourEntryModel> hourentry)
        {
            Istouchable = true;
            BaseProject = pm;
            baseViewModel = psvm;
            //TotalFeeInp = pm.Fee;
            timesheetidstoinvoice = hourentry;
            //PreviousSpentInp = pm.TotalFeeInvoiced;
            //this.CreateInvoiceCommand = new RelayCommand(this.CreateInvoice);
            this.CloseCommand = new RelayCommand(this.CloseWindow);
            this.CreateInvoiceCommand = new RelayCommand(this.CreateInvoice);
            //TotalHoursInvoicedInp = hours;
            //TotalBudgetInvoiced = budget;
            DateofServicesAreComplete = dateinitial;
            InvoiceDateInp = DateTime.Today;
            ProjectNameInp = BaseProject.ProjectName;
            ProjectNumberInp = BaseProject.ProjectNumber;
            LoadProjectManagers();
            LoadInfo();

        }

        public CreateInvoiceVM(ProjectInvoicingModel pm, InvoicingModel invoice)
        {
            Istouchable = false;
            this.CloseCommand = new RelayCommand(this.CloseWindow);
            BaseProject = pm;
            ClientAddressInp = invoice.ClientAddress;
            ClientNameInp = invoice.ClientName;
            ClientCompanyNameInp = invoice.ClientCompany;
            ClientCityInp = invoice.ClientCity;
            InvoiceDateInp = invoice.Date;
            ProjectNameInp = BaseProject.ProjectName;
            ProjectNumberInp = BaseProject.ProjectNumber;
            InvoiceNumberInp = invoice.InvoiceId;
            EmployeeDbModel signer = SQLAccess.LoadEmployeeById(invoice.EmployeeSignedId);

            if (signer != null)
            {
                EmployeeLowResModel elrsm = new EmployeeLowResModel(signer);
                ProjectManagers = new ObservableCollection<EmployeeLowResModel>(new List<EmployeeLowResModel>() { elrsm });
                SelectedPM = elrsm;
            }

            List<InvoicingRowsDb> rows = SQLAccess.LoadInvoiceRows(invoice.Id);

            List<TimesheetRowDbModel> timesheettime = new List<TimesheetRowDbModel>();

            foreach(int timeid in invoice.TimesheetIds)
            {
                TimesheetRowDbModel trdbm = SQLAccess.LoadTimeSheetDatabyId(timeid);
                timesheettime.Add(trdbm);
            }

            foreach (InvoicingRowsDb rowoi in rows)
            {
                InvoicingRows irn = new InvoicingRows(rowoi);

                SubProjectDbModel sub = SQLAccess.LoadSubProjectsById(rowoi.SubId);

                List<TimesheetRowDbModel> trdbmforsub = timesheettime.Where(x => x.SubProjectId == sub.Id).ToList();
                double budgetspent = trdbmforsub.Sum(x => x.BudgetSpent);

                irn.BudgetSpent = budgetspent;
                irn.ContractFee = sub.Fee;
                if (sub.IsAdservice == 1)
                {
                    if (rowoi.PercentComplete == 100 && rowoi.ThisPeriodInvoiced != 0)
                    {
                        //irn.DateOfInvoice = DateTime.ParseExact(rowoi.Date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MMMM dd, yyyy");
                        AdServicePhasesInvoiced.Add(irn);
                    }
                    else
                    {
                        previousinvoices = SQLAccess.LoadInvoices(BaseProject.Id);
                        int numinvoices = previousinvoices.Count() + 1;
                        List<InvoicingModelDb> sortedprevious = previousinvoices.OrderBy(x => x.Date).ToList();
                        InvoicingModelDb first = sortedprevious[0];
                        irn.DateOfInvoice = first.Date.ToString("MMMM dd, yyyy");
                        AdServicePhasesUnInvoiced.Add(irn);
                    }
                }
                else
                {
                    NonAdServicePhases.Add(irn);
                }
            }

            IsPreviousAdServiceVis = AdServicePhasesInvoiced.Count > 0 ? true : false;
            DoBasicServicesExist = NonAdServicePhases.Count > 0 ? true : false;
            DoAdservicesExist = AdServicePhasesUnInvoiced.Count > 0 ? true : false;

            SumValues();
        }

        public void LoadInfo()
        {
            AdServicePhasesInvoiced.Clear();
            AdServicePhasesUnInvoiced.Clear();
            NonAdServicePhases.Clear();

            previousinvoices = SQLAccess.LoadInvoices(BaseProject.Id);
            int numinvoices = previousinvoices.Count() + 1;
            List<InvoicingModelDb> sortedprevious = previousinvoices.OrderBy(x => x.Date).ToList();
            if (previousinvoices.Count > 0)
            {
                InvoicingModelDb first = previousinvoices[0];
                ClientAddressInp = first.ClientAddress;
                ClientNameInp = first.ClientName;
                ClientCompanyNameInp = first.ClientCompany;
                ClientCityInp = first.ClientCity;

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
                    if (previousinvoices.Count > 0)
                    {
                        InvoicingModelDb first = sortedprevious[0];
                        InvoicingRowsDb founditem = SQLAccess.LoadInvoiceRowsByInvoiceAndSubId(sub.Id, first.Id);
                        previousinvoiced = founditem.ThisPeriodInvoiced + founditem.PreviousInvoiced;
                        scopename = founditem.ScopeName;

                        if (founditem.PercentComplete == 100 && founditem.ThisPeriodInvoiced != 0)
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

                    List<HourEntryModel> hours = timesheetidstoinvoice.Where(x => x.SubId == sub.Id).ToList();

                    double budgetspent = 0;
                    if (hours.Count > 0)
                    {
                        budgetspent = hours.Sum(x => x.BudgetSpent);
                    }

                    if (iseditable)
                    {
                        if (sub.Fee != 0)
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
                            percentcomplete = 0;
                        }

                    }

                    InvoicingRows row = new InvoicingRows()
                    {
                        SubId = sub.Id,
                        viewmodel = this,
                        PreviousInvoiced = previousinvoiced,
                        ScopeName = scopename,
                        ContractFee = sub.Fee,
                        PercentComplete = percentcomplete,
                        BudgetSpent = budgetspent,
                    };
                    row.ThisPeriodInvoiced = ThisPeriodInvoiced;

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

            InvoiceNumberInp = (BaseProject.ProjectNumber * 1000) + numinvoices;

            IsPreviousAdServiceVis = AdServicePhasesInvoiced.Count > 0 ? true : false;
            DoBasicServicesExist = NonAdServicePhases.Count > 0 ? true : false;
            DoAdservicesExist = AdServicePhasesUnInvoiced.Count > 0 ? true : false;

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
            foreach (InvoicingRows row in NonAdServicePhases)
            {
                TotalContractFee += row.ContractFee;

                InvoicedToDateTotal += row.InvoicedtoDate;
                PreviousTotal += row.PreviousInvoiced;
                ThisPeriodTotal += row.ThisPeriodInvoiced;
                ThisPeriodTotalToDate += row.ThisPeriodInvoiced;
                TotalAmountDue += row.ThisPeriodInvoiced;
            }

            foreach (InvoicingRows row in AdServicePhasesInvoiced)
            {
                InvoicedToDateTotalToDate += row.InvoicedtoDate;
                PreviousTotalToDate += row.PreviousInvoiced;
                TotalAmountDue += row.ThisPeriodInvoiced;
            }

            foreach (InvoicingRows row in AdServicePhasesUnInvoiced)
            {
                TotalContractAdFee += row.ContractFee;
                InvoicedToDateTotalAd += row.InvoicedtoDate;
                PreviousTotalAd += row.PreviousInvoiced;
                ThisPeriodTotalAd += row.ThisPeriodInvoiced;
                ThisPeriodTotalToDate += row.ThisPeriodInvoiced;
                TotalAmountDue += row.ThisPeriodInvoiced;
            }
            IsTotalContractAdFee = TotalContractAdFee == 0 ? false : true;
            IsTotalContractFee = TotalContractFee == 0 ? false : true;
        }

        public void CreateInvoice()
        {
            int duedatevar = 0;

            if (InvoiceDateInp != null)
            {
                duedatevar = (int)long.Parse(InvoiceDateInp?.ToString("yyyyMMdd"));
            }

            //var bytes = timesheetidstoinvoice.Select(i => BitConverter.GetBytes(i.TimeId)).ToArray();
            int[] timeids = timesheetidstoinvoice.Select(x => x.TimeId).ToArray();
            string result = string.Join(",", timeids);

            InvoicingModelDb invoice = new InvoicingModelDb()
            {

                ProjectId = BaseProject.Id,
                InvoiceNumber = InvoiceNumberInp,
                Date = duedatevar,
                ClientName = ClientNameInp,
                ClientCompany = ClientCompanyNameInp,
                ClientAddress = ClientAddressInp,
                ClientCity = ClientCityInp,
                PreviousSpent = BaseProject.TotalFeeInvoiced,
                AmountDue = ThisPeriodTotalToDate,
                EmployeeSignedId = SelectedPM.Id,
                TimesheetIds = result,
            };

            int id = SQLAccess.AddInvoice(invoice);

            if (id > -1)
            {
                //AdServicePhasesInvoiced.Add(row);

                //AdServicePhasesUnInvoiced.Add(row);

                //        NonAdServicePhases.Add(row);

                foreach (InvoicingRows nonadrow in NonAdServicePhases)
                {
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
                }

                foreach (InvoicingRows adrow in AdServicePhasesUnInvoiced)
                {
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
                }

                foreach (HourEntryModel idval in timesheetidstoinvoice)
                {
                    SQLAccess.UpdateInvoiceStatusTime(idval.TimeId, 1);
                }
            }

            //if (id > 0)
            //{
            //    foreach (int idval in timesheetidstoinvoice)
            //    {
            //        SQLAccess.UpdateInvoiceStatusTime(idval, 1);
            //    }
            //}

            CloseWindow();
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
