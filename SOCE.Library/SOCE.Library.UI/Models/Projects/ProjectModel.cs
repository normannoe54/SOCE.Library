using MaterialDesignThemes.Wpf;
using Microsoft.WindowsAPICodePack.Dialogs;
using SOCE.Library.Db;
using SOCE.Library.UI.ViewModels;
using SOCE.Library.UI.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SOCE.Library.UI
{
    public class ProjectModel : BaseVM
    {

        //private ObservableCollection<RatePerProjectModel> _ratePerProject = new ObservableCollection<RatePerProjectModel>();
        //public ObservableCollection<RatePerProjectModel> RatePerProject
        //{
        //    get { return _ratePerProject; }
        //    set
        //    {
        //        _ratePerProject = value;
        //        RaisePropertyChanged(nameof(RatePerProject));
        //    }
        //}



        private ObservableCollection<SubProjectModel> _subProjects = new ObservableCollection<SubProjectModel>();
        public ObservableCollection<SubProjectModel> SubProjects
        {
            get { return _subProjects; }
            set
            {
                _subProjects = value;
                RaisePropertyChanged(nameof(SubProjects));
            }
        }

        private int _id { get; set; }
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                RaisePropertyChanged(nameof(Id));
            }
        }

        private string _projectName { get; set; }
        public string ProjectName
        {
            get
            {
                return _projectName;
            }
            set
            {
                _projectName = value;
                RaisePropertyChanged(nameof(ProjectName));
            }
        }

        private int _projectNumber { get; set; }
        public int ProjectNumber
        {
            get
            {
                return _projectNumber;
            }
            set
            {
                _projectNumber = value;
                RaisePropertyChanged(nameof(ProjectNumber));
            }
        }

        private string _searchText { get; set; }
        public string SearchText
        {
            get
            {
                return _searchText;
            }
            set
            {
                _searchText = value;
                RaisePropertyChanged(nameof(SearchText));
            }
        }

        private string _remarks { get; set; }
        public string Remarks
        {
            get
            {
                return _remarks;
            }
            set
            {
                _remarks = value;
                RaisePropertyChanged(nameof(Remarks));
            }
        }

        private double _fee { get; set; }
        public double Fee
        {
            get
            {
                return _fee;
            }
            set
            {
                _fee = value;
                RaisePropertyChanged(nameof(Fee));
            }
        }

        private SchedulingEnum _schedulingValue { get; set; }
        public SchedulingEnum SchedulingValue
        {
            get
            {
                return _schedulingValue;
            }
            set
            {
                //bool returnvalue = true;
                //if (!allowedtomakechanges)
                //{
                //    if (value == SchedulingEnum.H)
                //    {
                //        IsOnHold = true;
                //        foreach (SubProjectModel sub in SubProjects)
                //        {
                //            sub.IsScheduleActive = false;
                //        }

                //    }
                //    else if (value == SchedulingEnum.CA)
                //    {
                //        SubProjectModel subcontainingCA = SubProjects.Where(x => x.PointNumber == "CA").FirstOrDefault();

                //        bool containsCA = SubProjects.Where(x => x.PointNumber == "CA").Count() > 0;

                //        if (containsCA)
                //        {
                //            foreach (SubProjectModel sub in SubProjects)
                //            {
                //                sub.IsScheduleActive = sub.PointNumber == "CA" ? true : false;
                //            }
                //        }
                //        else
                //        {
                //            YesNoView view = new YesNoView();
                //            YesNoVM aysvm = new YesNoVM();

                //            aysvm.Message = $"CA Phase does not exist";
                //            aysvm.SubMessage = $"Would you like to add one?";
                //            view.DataContext = aysvm;

                //            //show the dialog
                //            var Result = DialogHost.Show(view, "RootDialog");

                //            YesNoVM vm = view.DataContext as YesNoVM;
                //            bool resultvm = vm.Result;

                //            returnvalue = resultvm;
                //            if (resultvm)
                //            {
                //                double numorder = SubProjects.Max(x => x.NumberOrder);
                //                SubProjectDbModel subproject = new SubProjectDbModel
                //                {
                //                    ProjectId = Id,
                //                    Description = "Construction Administration",
                //                    IsActive = 1,
                //                    IsInvoiced = 0,
                //                    PercentComplete = 0,
                //                    PercentBudget = 0,
                //                    NumberOrder = Convert.ToInt32(numorder) + 1,
                //                    Fee = 0
                //                };
                //                SQLAccess.AddSubProject(subproject);
                //            } //show ui and ask user for stuff
                //        }
                //    }
                //    else if (value == SchedulingEnum.A)
                //    {
                //        //popup box asking which subproject is the active one.
                //    }

                //    UpdateProject();
                //    UpdateSubProjects();
                //}

                //if (returnvalue)
                //{
                _schedulingValue = value;
                //}

                RaisePropertyChanged(nameof(SchedulingValue));

            }
        }

        private string _adserviceFile { get; set; }
        public string AdserviceFile
        {
            get
            {
                return _adserviceFile;
            }
            set
            {
                _adserviceFile = value;
                RaisePropertyChanged(nameof(AdserviceFile));
            }
        }

        private ClientModel _client;
        public ClientModel Client
        {
            get { return _client; }
            set
            {
                _client = value;
                RaisePropertyChanged(nameof(Client));
            }
        }

        private MarketModel _market;
        public MarketModel Market
        {
            get { return _market; }
            set
            {
                _market = value;
                RaisePropertyChanged(nameof(Market));
            }
        }

        private EmployeeModel _projectManager;
        public EmployeeModel ProjectManager
        {
            get { return _projectManager; }
            set
            {
                _projectManager = value;
                RaisePropertyChanged(nameof(ProjectManager));
            }
        }

        private Brush _percentCompleteColor;
        public Brush PercentCompleteColor
        {
            get { return _percentCompleteColor; }
            set
            {
                _percentCompleteColor = value;
                RaisePropertyChanged(nameof(PercentCompleteColor));
            }
        }

        private bool _isActive = false;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive && !value)
                {
                    ProjectEnd = (int)long.Parse(DateTime.Now.ToString("yyyyMMdd"));
                    FinalSpent = BudgetSpent;
                    //foreach (SubProjectModel sub in SubProjects)
                    //{
                    //    sub.IsActive = false;
                    //}
                }

                if (!_isActive && value)
                {
                    ProjectEnd = 0;
                    FinalSpent = 0;
                    //UpdateSubProjects(SubProjects[0]);
                }

                _isActive = value;
                RaisePropertyChanged(nameof(IsActive));
            }
        }

        private DateTime? _dueDate;
        public DateTime? DueDate
        {
            get { return _dueDate; }
            set
            {
                _dueDate = value;
                RaisePropertyChanged(nameof(DueDate));
            }
        }

        private double _percentComplete;
        public double PercentComplete
        {
            get { return _percentComplete; }
            set
            {
                _percentComplete = value;
                IconforBudgetSummary = PercentBudgetSpent > PercentComplete ? PackIconKind.AlertCircleOutline : PackIconKind.CheckboxMarkedOutline;
                PercentCompleteColor = PercentBudgetSpent > PercentComplete ? Brushes.Red : Brushes.Green;
                RaisePropertyChanged(nameof(PercentComplete));
            }
        }

        private bool onstartup = false;

        private string _projectfolder = "";
        public string Projectfolder
        {
            get { return _projectfolder; }
            set
            {
                _projectfolder = value;
                if (onstartup)
                {
                    UpdateProject();
                }
                RaisePropertyChanged(nameof(Projectfolder));
            }
        }


        private string _drawingsfolder = "";
        public string Drawingsfolder
        {
            get { return _drawingsfolder; }
            set
            {
                _drawingsfolder = value;
                if (onstartup)
                {
                    UpdateProject();
                }
                RaisePropertyChanged(nameof(Drawingsfolder));
            }
        }

        private string _architectfolder = "";
        public string Architectfolder
        {
            get { return _architectfolder; }
            set
            {
                _architectfolder = value;
                if (onstartup)
                {
                    UpdateProject();
                }
                RaisePropertyChanged(nameof(Architectfolder));
            }
        }

        private string _plotfolder = "";
        public string Plotfolder
        {
            get { return _plotfolder; }
            set
            {
                _plotfolder = value;
                if (onstartup)
                {
                    UpdateProject();
                }
                RaisePropertyChanged(nameof(Plotfolder));
            }
        }

        private bool _editFieldState = true;
        public bool EditFieldState
        {
            get { return _editFieldState; }
            set
            {
                if (!_editFieldState && value)
                {
                    UpdateProject();
                }
                _editFieldState = value;
                //ComboFieldState = !_editFieldState;

                RaisePropertyChanged(nameof(EditFieldState));
            }
        }

        private bool _isOnHold = false;
        public bool IsOnHold
        {
            get { return _isOnHold; }
            set
            {
                _isOnHold = value;
                RaisePropertyChanged(nameof(IsOnHold));
            }
        }

        //private bool _comboFieldState;
        //public bool ComboFieldState
        //{
        //    get { return _comboFieldState; }
        //    set
        //    {
        //        _comboFieldState = value;
        //        RaisePropertyChanged(nameof(ComboFieldState));
        //    }
        //}

        private double _totalBudget = 0;
        public double TotalBudget
        {
            get { return _totalBudget; }
            set
            {
                _totalBudget = value;
                RaisePropertyChanged(nameof(TotalBudget));
            }
        }

        private double _totalRegulatedBudget = 0;
        public double TotalRegulatedBudget
        {
            get { return _totalRegulatedBudget; }
            set
            {
                _totalRegulatedBudget = value;
                RaisePropertyChanged(nameof(TotalRegulatedBudget));
            }
        }

        private double _percentofInvoicedFee = 0;
        public double PercentofInvoicedFee
        {
            get { return _percentofInvoicedFee; }
            set
            {
                _percentofInvoicedFee = value;
                RaisePropertyChanged(nameof(PercentofInvoicedFee));
            }
        }

        private double _percentTotalFeeSpent = 0;
        public double PercentTotalFeeSpent
        {
            get { return _percentTotalFeeSpent; }
            set
            {
                _percentTotalFeeSpent = value;
                RaisePropertyChanged(nameof(PercentTotalFeeSpent));
            }
        }

        private double _budgetSpent = 0;
        public double BudgetSpent
        {
            get { return _budgetSpent; }
            set
            {
                _budgetSpent = value;
                RaisePropertyChanged(nameof(BudgetSpent));
            }
        }

        private double _budgetLeft = 0;
        public double BudgetLeft
        {
            get { return _budgetLeft; }
            set
            {
                _budgetLeft = value;
                RaisePropertyChanged(nameof(BudgetLeft));
            }
        }

        private double _hoursSpent = 0;
        public double HoursSpent
        {
            get { return _hoursSpent; }
            set
            {
                _hoursSpent = value;
                RaisePropertyChanged(nameof(HoursSpent));
            }
        }

        private double _hoursLeft = 0;
        public double HoursLeft
        {
            get { return _hoursLeft; }
            set
            {
                _hoursLeft = value;
                RaisePropertyChanged(nameof(HoursLeft));
            }
        }

        private double _percentBudgetSpent = 0;
        public double PercentBudgetSpent
        {
            get { return _percentBudgetSpent; }
            set
            {
                _percentBudgetSpent = value;
                IconforBudgetSummary = PercentBudgetSpent > PercentComplete ? PackIconKind.AlertCircleOutline : PackIconKind.CheckboxMarkedOutline;
                PercentCompleteColor = PercentBudgetSpent > PercentComplete ? Brushes.Red : Brushes.Green;
                RaisePropertyChanged(nameof(PercentBudgetSpent));
            }
        }

        private double AvgRate = 0;

        private PackIconKind _iconforBudgetSummary = PackIconKind.CheckboxMarkedOutline;
        public PackIconKind IconforBudgetSummary
        {
            get
            {
                return _iconforBudgetSummary;
            }
            set
            {
                _iconforBudgetSummary = value;
                RaisePropertyChanged(nameof(IconforBudgetSummary));
            }
        }

        private PackIconKind _iconForListButton = PackIconKind.Lock;
        public PackIconKind IconForListButton
        {
            get
            {
                return _iconForListButton;
            }
            set
            {
                _iconForListButton = value;
                RaisePropertyChanged(nameof(IconForListButton));
            }
        }

        private Brush _colorForListButton = Brushes.Orange;
        public Brush ColorForListButton
        {
            get { return _colorForListButton; }
            set
            {
                _colorForListButton = value;
                RaisePropertyChanged(nameof(ColorForListButton));
            }
        }

        private string _tooltipforList = "Locked";
        public string TooltipforList
        {
            get { return _tooltipforList; }
            set
            {
                _tooltipforList = value;
                RaisePropertyChanged(nameof(TooltipforList));
            }
        }

        private bool IsEditable;

        private bool _canDelete { get; set; } = false;
        public bool CanDelete
        {
            get
            {
                return _canDelete;
            }
            set
            {
                _canDelete = value;
                RaisePropertyChanged(nameof(CanDelete));

            }
        }

        private bool _editList = true;
        public bool EditList
        {
            get { return _editList; }
            set
            {
                _editList = value;

                RaisePropertyChanged(nameof(EditList));
            }
        }
        public string CombinedName { get; set; }

        public int ProjectStart;

        public int ProjectEnd;

        public double FinalSpent;
        public bool sourcevalue = true;
        #region project
        public ICommand CopyProjectFolderCommand { get; set; }
        public ICommand SelectProjectFolderCommand { get; set; }
        public ICommand OpenProjectFolderCommand { get; set; }

        public ICommand ButtonPress { get; set; }
        #endregion

        #region arch
        public ICommand CopyArchitectFolderCommand { get; set; }
        public ICommand SelectArchitectFolderCommand { get; set; }
        public ICommand OpenArchitectFolderCommand { get; set; }
        #endregion

        #region drawings
        public ICommand CopyDrawingsFolderCommand { get; set; }
        public ICommand SelectDrawingsFolderCommand { get; set; }
        public ICommand OpenDrawingsFolderCommand { get; set; }
        #endregion

        #region plot
        public ICommand CopyPlotFolderCommand { get; set; }
        public ICommand SelectPlotFolderCommand { get; set; }
        public ICommand OpenPlotFolderCommand { get; set; }
        #endregion

        #region constructors
        public ProjectModel()
        {

        }

        public ProjectModel(ProjectDbModel pm, bool iseditable = true, bool filter = false)
        {
            //SubProjects.CollectionChanged += this.SubProjectChanged;
            //RatePerProject.CollectionChanged += this.RatesChanged;
            IsEditable = iseditable;
            Constructor();

            Id = pm.Id;
            ProjectName = pm.ProjectName;
            ProjectNumber = pm.ProjectNumber;
            Fee = pm.Fee;
            IsActive = Convert.ToBoolean(pm.IsActive);
            PercentComplete = pm.PercentComplete;
            ProjectStart = pm.ProjectStart;
            ProjectEnd = pm.ProjectEnd;
            FinalSpent = pm.FinalSpent;
            Remarks = pm.Remarks;

            if (pm?.DueDate != null && pm?.DueDate != 0)
            {
                DueDate = DateTime.ParseExact(pm.DueDate.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            }

            //EmployeeDbModel emdbm = SQLAccess.LoadEmployeeById(pm.ManagerId);
            //EmployeeModel em = new EmployeeModel(emdbm);
            //ProjectManager = em;

            //ClientDbModel cdbm = SQLAccess.LoadClientById(pm.ClientId);
            //ClientModel cm = new ClientModel(cdbm);
            //Client = cm;

            //MarketDbModel mdbm = SQLAccess.LoadMarketeById(pm.MarketId);
            //MarketModel mm = new MarketModel(mdbm);
            //Market = mm;

            Remarks = pm.Remarks;
            Projectfolder = pm.Projectfolder;
            Drawingsfolder = pm.Drawingsfolder;
            Architectfolder = pm.Architectfolder;
            Plotfolder = pm.Plotfolder;
            IsOnHold = Convert.ToBoolean(pm.IsOnHold);
            TotalBudget = Fee;
            //FormatData();
            onstartup = true;
        }

        public ProjectModel(ProjectDbModel pm)
        {
            //SubProjects.CollectionChanged += this.SubProjectChanged;
            //RatePerProject.CollectionChanged += this.RatesChanged;

            //IsEditable = iseditable;
            //Constructor();

            Id = pm.Id;
            ProjectName = pm.ProjectName;
            ProjectNumber = pm.ProjectNumber;
            IsOnHold = Convert.ToBoolean(pm.IsOnHold);
            //SearchText = String.Format("{0} {1}", ProjectNumber.ToString(), ProjectName);
            Fee = pm.Fee;

            if (pm?.DueDate != null && pm?.DueDate != 0)
            {
                DueDate = DateTime.ParseExact(pm.DueDate.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            }

            IsActive = Convert.ToBoolean(pm.IsActive);
            SearchText = ProjectNumber.ToString();
            CombinedName = $"[{ProjectNumber.ToString()}] {ProjectName}";
            //PercentComplete = pm.PercentComplete;
            //ProjectStart = pm.ProjectStart;
            //ProjectEnd = pm.ProjectEnd;
            //FinalSpent = pm.FinalSpent;

            //FormatData();
            //onstartup = true;
        }

        private void Constructor()
        {
            this.ButtonPress = new RelayCommand(this.RunButtonPress);
            this.CopyProjectFolderCommand = new RelayCommand(this.CopyProjectFolder);
            this.SelectProjectFolderCommand = new RelayCommand(this.SelectProjectFolder);
            this.OpenProjectFolderCommand = new RelayCommand(this.OpenProjectFolder);

            this.CopyArchitectFolderCommand = new RelayCommand(this.CopyArchFolder);
            this.SelectArchitectFolderCommand = new RelayCommand(this.SelectArchFolder);
            this.OpenArchitectFolderCommand = new RelayCommand(this.OpenArchFolder);

            this.CopyDrawingsFolderCommand = new RelayCommand(this.CopyDrawingsFolder);
            this.SelectDrawingsFolderCommand = new RelayCommand(this.SelectDrawingsFolder);
            this.OpenDrawingsFolderCommand = new RelayCommand(this.OpenDrawingsFolder);

            this.CopyPlotFolderCommand = new RelayCommand(this.CopyPlotFolder);
            this.SelectPlotFolderCommand = new RelayCommand(this.SelectPlotFolder);
            this.OpenPlotFolderCommand = new RelayCommand(this.OpenPlotFolder);

            onstartup = false;
        }
        #endregion

        private async void RunButtonPress()
        {
            bool makechange = true;
            if (EditList)
            {
                ColorForListButton = Brushes.Green;
                IconForListButton = PackIconKind.ContentSave;
                TooltipforList = "Save Changes";
            }
            else
            {
                if (SchedulingValue == SchedulingEnum.H)
                {
                    IsOnHold = true;
                    foreach (SubProjectModel sub in SubProjects)
                    {
                        sub.IsScheduleActive = false;
                    }

                }
                else if (SchedulingValue == SchedulingEnum.CA)
                {
                    SubProjectModel subcontainingCA = SubProjects.Where(x => x.PointNumber == "CA").FirstOrDefault();

                    bool containsCA = SubProjects.Where(x => x.PointNumber == "CA").Count() > 0;

                    if (containsCA)
                    {
                        foreach (SubProjectModel sub in SubProjects)
                        {
                            sub.IsScheduleActive = sub.PointNumber == "CA" ? true : false;
                            if (sub.PointNumber == "CA")
                            {
                                sub.IsActive = true;
                            }
                        }
                    }
                    else
                    {
                        YesNoView view = new YesNoView();
                        YesNoVM aysvm = new YesNoVM();

                        aysvm.Message = $"CA Phase does not exist";
                        aysvm.SubMessage = $"Would you like to add one?";
                        view.DataContext = aysvm;

                        //show the dialog
                        var Result = await DialogHost.Show(view, "RootDialog");

                        YesNoVM vm = view.DataContext as YesNoVM;
                        bool resultvm = vm.Result;
                        makechange = resultvm;
                        if (resultvm)
                        {
                            IsOnHold = false;
                            double numorder = SubProjects.Select(x => x.NumberOrder).Max();
                            SubProjectDbModel subproject = new SubProjectDbModel
                            {
                                ProjectId = Id,
                                PointNumber = "CA",
                                Description = "Construction Administration",
                                IsActive = 1,
                                IsInvoiced = 0,
                                PercentComplete = 0,
                                PercentBudget = 0,
                                IsScheduleActive = 1,
                                NumberOrder = Convert.ToInt32(numorder) + 1,
                                Fee = 0
                            };
                            SQLAccess.AddSubProject(subproject);

                            foreach (SubProjectModel sub in SubProjects)
                            {
                                sub.IsScheduleActive = false;
                            }
                        } //show ui and ask user for stuff
                    }
                }
                else if (SchedulingValue == SchedulingEnum.D)
                {
                    double count = SubProjects.Where(x => x.PointNumber != "CA").Count();

                    if (count > 1)
                    {
                        SelectionBoxView view = new SelectionBoxView();
                        SelectionBoxVM aysvm = new SelectionBoxVM();

                        aysvm.Message = $"More than 1 possible active phase";
                        aysvm.SubMessage = $"Select the active phase";
                        //aysvm.SubProjects = new ObservableCollection<SubProjectModel>(SubProjects.Where(x=>x.PointNumber != "CA").ToList());
                        view.DataContext = aysvm;

                        //show the dialog
                        var Result = await DialogHost.Show(view, "RootDialog");

                        SelectionBoxVM vm = view.DataContext as SelectionBoxVM;
                        bool resultvm = vm.Result;
                        makechange = resultvm;
                        if (resultvm && vm.SelectedSubproject != null)
                        {
                            IsOnHold = false;
                            foreach (SubProjectModel sub in SubProjects)
                            {
                                sub.IsScheduleActive = false;
                            }

                            vm.SelectedSubproject.IsScheduleActive = true;
                            vm.SelectedSubproject.IsActive = true;
                        } //show ui and ask user for stuff
                        //ask which phase is active
                    }
                    else if (count == 1)
                    {
                        makechange = true;
                        IsOnHold = false;
                        foreach (SubProjectModel sub in SubProjects)
                        {
                            if (sub.PointNumber != "CA")
                            {
                                sub.IsScheduleActive = true;
                                sub.IsActive = true;
                            }
                            else
                            {
                                sub.IsScheduleActive = false;
                            }
                        }
                    }
                }

                if (makechange)
                {
                    UpdateProjectModel();
                    UpdateSubProjects();
                    LoadSubProjects();

                    ColorForListButton = Brushes.Orange;
                    IconForListButton = PackIconKind.Lock;
                    TooltipforList = "Locked";
                }
            }

            if (makechange)
            {
                EditList = !EditList;
            }

        }


        //private void RatesChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    if (e.NewItems != null)
        //    {
        //        foreach (INotifyPropertyChanged added in e?.NewItems)
        //        {
        //            RatePerProjectModel rpm = (RatePerProjectModel)added;

        //            rpm.PropertyChanged += RateItemModificationOnPropertyChanged;
        //        }
        //    }

        //    if (e.OldItems != null)
        //    {
        //        foreach (INotifyPropertyChanged added in e?.OldItems)
        //        {
        //            RatePerProjectModel rpm = (RatePerProjectModel)added;

        //            rpm.PropertyChanged -= RateItemModificationOnPropertyChanged;
        //        }
        //    }
        //}

        //private void RateItemModificationOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        //{
        //    FormatData(false);
        //}


        //private void SubProjectChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    if (e.NewItems != null)
        //    {
        //        foreach (INotifyPropertyChanged added in e?.NewItems)
        //        {
        //            SubProjectModel spm = (SubProjectModel)added;

        //            spm.PropertyChanged += SubItemModificationOnPropertyChanged;
        //        }
        //    }

        //    if (e.OldItems != null)
        //    {
        //        foreach (INotifyPropertyChanged added in e?.OldItems)
        //        {
        //            SubProjectModel spm = (SubProjectModel)added;

        //            spm.PropertyChanged -= SubItemModificationOnPropertyChanged;
        //        }
        //    }
        //}

        //private void SubItemModificationOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        //{
        //    UpdateSubProjects();
        //}

        //public void UpdateTotalBudget()
        //{
        //    TotalBudget = 0;

        //    foreach (SubProjectModel spm in SubProjects)
        //    {
        //        TotalBudget += spm.Fee;
        //    }

        //}

        //private SchedulingEnum RunCAAsyncMethod(SchedulingEnum value)
        //{

        //    if (!allowedtomakechanges)
        //    {
        //        if (value == SchedulingEnum.H)
        //        {
        //            IsOnHold = true;
        //            foreach (SubProjectModel sub in SubProjects)
        //            {
        //                sub.IsScheduleActive = false;
        //            }

        //        }
        //        else if (value == SchedulingEnum.CA)
        //        {
        //            SubProjectModel subcontainingCA = SubProjects.Where(x => x.PointNumber == "CA").FirstOrDefault();

        //            bool containsCA = SubProjects.Where(x => x.PointNumber == "CA").Count() > 0;

        //            if (containsCA)
        //            {
        //                foreach (SubProjectModel sub in SubProjects)
        //                {
        //                    sub.IsScheduleActive = sub.PointNumber == "CA" ? true : false;
        //                }
        //            }
        //            else
        //            {
        //                YesNoView view = new YesNoView();
        //                YesNoVM aysvm = new YesNoVM();

        //                aysvm.Message = $"CA Phase does not exist";
        //                aysvm.SubMessage = $"Would you like to add one?";
        //                view.DataContext = aysvm;

        //                //show the dialog
        //                var Result = DialogHost.Show(view, "RootDialog");

        //                YesNoVM vm = view.DataContext as YesNoVM;
        //                bool resultvm = vm.Result;

        //                if (resultvm)
        //                {
        //                    double numorder = SubProjects.Max(x => x.NumberOrder);
        //                    SubProjectDbModel subproject = new SubProjectDbModel
        //                    {
        //                        ProjectId = Id,
        //                        Description = "Construction Administration",
        //                        IsActive = 1,
        //                        IsInvoiced = 0,
        //                        PercentComplete = 0,
        //                        PercentBudget = 0,
        //                        NumberOrder = Convert.ToInt32(numorder) + 1,
        //                        Fee = 0
        //                    };
        //                    SQLAccess.AddSubProject(subproject);
        //                } //show ui and ask user for stuff
        //            }
        //        }
        //        else if (value == SchedulingEnum.A)
        //        {
        //            //popup box asking which subproject is the active one.
        //        }

        //        UpdateProject();
        //        UpdateSubProjects();
        //    }
        //}

        public bool UpdateSubProjects()
        {
            TotalBudget = 0;

            foreach (SubProjectModel spm in SubProjects)
            {
                TotalBudget += spm.Fee;
            }

            SQLAccess.UpdateFee(Id, TotalBudget);

            Fee = TotalBudget;
            //update total budget
            bool onehastobeactive = false;

            //bool onehastobeinprogress = false;
            //turn off item mo

            //if (!IsOnHold && (SubProjects.Where(x => x.IsScheduleActive == true).Count() == 0) && (SubProjects.Count > 0))
            //{
            //    SubProjects[0].IsScheduleActive = true;
            //}
            bool isonescheduleactive = false;

            int minnum = SubProjects.Select(x => x.NumberOrder).Min();

            foreach (SubProjectModel spm in SubProjects)
            {
                //spm.PropertyChanged -= SubItemModificationOnPropertyChanged;
                spm.TotalFee = TotalBudget;
                spm.UpdatePercents();
                //spm.UpdatePercentBudget();
                int? datefinal = null;
                //if (spm.DueDate != null)
                //{
                //    datefinal = (int)long.Parse(spm.DueDate?.ToString("yyyyMMdd"));
                //}

                if (IsOnHold && spm.IsScheduleActive)
                {
                    spm.IsScheduleActive = false;
                }
                else if (!IsOnHold && SubProjects.Where(x=>x.IsScheduleActive == true).Count() == 0 && spm.NumberOrder == minnum)
                {
                    spm.IsScheduleActive = true;
                }

                SubProjectDbModel subproject = new SubProjectDbModel()
                {
                    Id = spm.Id,
                    ProjectId = spm.ProjectNumber,
                    PointNumber = spm.PointNumber,
                    Description = spm.Description,
                    Fee = spm.Fee,
                    IsActive = spm.IsActive ? 1 : 0,
                    PercentComplete = spm.PercentComplete,
                    PercentBudget = spm.PercentBudget,
                    IsInvoiced = spm.IsInvoiced ? 1 : 0,
                    SubStart = spm.DateInitiated != null ? (int)long.Parse(spm.DateInitiated?.ToString("yyyyMMdd")) : (int?)null,
                    SubEnd = spm.DateInvoiced != null ? (int)long.Parse(spm.DateInvoiced?.ToString("yyyyMMdd")) : (int?)null,
                    NameOfClient = spm.NameOfClient,
                    IsBillable = spm.IsBillable == null ? (int?)null : Convert.ToInt32(spm.IsBillable),
                    ExpandedDescription = spm.ExpandedDescription,
                    IsAdservice = spm.IsAddService ? 1 : 0,
                    NumberOrder = spm.NumberOrder,
                    ClientCompanyName = spm.ClientCompanyName,
                    ClientAddress = spm.ClientAddress,
                    EmployeeIdSigned = spm.EmployeeIdSigned,
                    IsScheduleActive = spm.IsScheduleActive ? 1 : 0
                };

                SQLAccess.UpdateSubProject(subproject);

                onehastobeactive = onehastobeactive || (spm.IsActive || !spm.EditSubFieldState);
                isonescheduleactive = onehastobeactive || spm.IsScheduleActive;
                //if (!IsOnHold)
                //{
                //    onehastobeinprogress = onehastobeinprogress || (spm.IsScheduleActive || !spm.EditSubFieldState);
                //}
            }


            //catch for no actives
            //if (subinput != null)
            //{
            //    //if (!onehastobeactive)
            //    //{
            //        SubProjectDbModel subproject = new SubProjectDbModel()
            //        {
            //            Id = subinput.Id,
            //            ProjectId = subinput.ProjectNumber,
            //            PointNumber = subinput.PointNumber,
            //            Description = subinput.Description,
            //            Fee = subinput.Fee,
            //            IsActive = subinput.IsActive ? 1 : 0,
            //            PercentComplete = subinput.PercentComplete,
            //            PercentBudget = subinput.PercentBudget,
            //            IsInvoiced = subinput.IsInvoiced ? 1 : 0,
            //            ExpandedDescription = subinput.ExpandedDescription,
            //            IsAdservice = subinput.IsAddService ? 1 : 0,
            //            NumberOrder = subinput.NumberOrder
            //        };
            //        SQLAccess.UpdateSubProject(subproject);
            //    //}
            //}
            UpdateData();

            //if (SubProjects.Where(x => x.IsScheduleActive == true).Count() > 0 && IsOnHold)
            //{
            //    IsOnHold = false;
            //    UpdateProjectModel();
            //}

            if (!onehastobeactive)
            {
                IsActive = false;
                UpdateProjectModel();
            }


            //if (!onehastobeinprogress)
            //{
            //     = false;
            //    UpdateProjectModel();
            //}
            return IsOnHold;
        }

        public void LoadSubProjects()
        {
            SubProjects.Clear();
            List<SubProjectDbModel> subdbmodels = SQLAccess.LoadSubProjectsByProject(Id);

            foreach (SubProjectDbModel spdm in subdbmodels)
            {
                SubProjectModel spm = new SubProjectModel(spdm, Fee, this);
                SubProjects.Add(spm);
            }
        }

        public bool Formatted = false;

        public void FormatData(bool addistrue)
        {
            if (addistrue)
            {
                SubProjects.Clear();
            }
            else
            {
                SubProjectModel allsub = SubProjects.Where(x => x.Description == "All Phases").FirstOrDefault();
                if (allsub != null)
                {
                    allsub.RolesPerSub.Clear();
                    //SubProjects.Remove(allsub);
                }
            }

            Formatted = true;

            //List<TimesheetRowDbModel> total = new List<TimesheetRowDbModel>();
            List<RolePerSubProjectModel> allrolesummary = new List<RolePerSubProjectModel>();
            //total
            //get all subprojectIds associated with projectId
            List<SubProjectDbModel> subdbmodels = SQLAccess.LoadAllSubProjectsByProject(Id);
            //foreach (SubProjectDbModel sub in subdbmodels)
            //{
            //    totalfee += sub.Fee;
            //}                

            //Fee = totalfee;
            TotalBudget = Fee;

            double hourstotal = 0;
            double hourstotalleft = 0;
            double budgetspent = 0;
            double totalregulatedbudget = 0;
            int count = 1;

            foreach (SubProjectDbModel spdm in subdbmodels)
            {
                double hoursspentpersub = 0;
                double hoursleftpersub = 0;
                double budgetspentpersub = 0;
                double regulatedbudgetpersub = 0;
                double totalbudgethours = 0;

                SubProjectModel spm = null;

                //if (spdm.IsCurrActive == 1)
                //{
                if (addistrue)
                {
                    spm = new SubProjectModel(spdm, Fee, this);
                }
                else
                {
                    spm = SubProjects.Where(x => x.Id == spdm.Id).FirstOrDefault();
                    spm.Fee = spdm.Fee;
                    spm.Constructor(spdm);

                    //spm.RolesPerSub.Clear();
                    if (spm == null)
                    {
                        //delete item from database? should never happen
                        continue;
                    }
                }

                spm.NumberOrder = spdm.NumberOrder;
                //}
                //else
                //{
                //    continue;
                //}

                List<RolePerSubProjectDbModel> rolesdbmodel = SQLAccess.LoadRolesPerSubProject(spm.Id);
                List<TimesheetRowDbModel> tmdata = SQLAccess.LoadTimeSheetDatabySubId(spdm.Id);

                if (tmdata.Count != 0)
                {
                    var grouped = tmdata.OrderBy(x => x.EmployeeId).GroupBy(x => x.EmployeeId);

                    foreach (var item in grouped)
                    {
                        EmployeeDbModel employee = SQLAccess.LoadEmployeeById(item.Key);

                        if (employee != null)
                        {
                            //order by date
                            List<TimesheetRowDbModel> employeetimesheetdata = item.OrderBy(x => x.Date).ToList();
                            double hours = employeetimesheetdata.Sum(x => x.TimeEntry);
                            double spentbudget = employeetimesheetdata.Sum(x => x.BudgetSpent);
                            RolePerSubProjectModel rspm = null;

                            if (addistrue)
                            {
                                RolePerSubProjectDbModel rpdm = rolesdbmodel.Where(x => x.EmployeeId == employee.Id).FirstOrDefault();

                                if (rpdm != null)
                                {
                                    rspm = new RolePerSubProjectModel(rpdm.Id, rpdm.Rate, (DefaultRoleEnum)rpdm.Role, rpdm.EmployeeId, spm, rpdm.BudgetHours, spm.Fee);
                                    spm.RolesPerSub.Add(rspm);
                                }
                                else
                                {

                                    RolePerSubProjectDbModel rpp = new RolePerSubProjectDbModel()
                                    {
                                        SubProjectId = spm.Id,
                                        EmployeeId = employee.Id,
                                        Role = employee.DefaultRoleId,
                                        Rate = employee.Rate,
                                        BudgetHours = 0
                                    };
                                    int id = SQLAccess.AddRolesPerSubProject(rpp);

                                    if (id != 0)
                                    {
                                        rspm = new RolePerSubProjectModel(id, employee.Rate, (DefaultRoleEnum)employee.DefaultRoleId, employee.Id, spm, 0, spm.Fee);
                                        spm.RolesPerSub.Add(rspm);
                                    }

                                    //for debugging purposes only
                                }
                            }
                            else
                            {
                                rspm = spm.RolesPerSub.Where(x => x.Employee.Id == employee.Id).FirstOrDefault();

                                if (rspm == null)
                                {
                                    //for debugging purposes only
                                }
                            }

                            rspm.SpentHours = hours;
                            rspm.SpentBudget = spentbudget;
                            double hoursleft = rspm.BudgetedHours - hours;
                            hoursspentpersub += hours;
                            //hoursleftpersub += hoursleft;
                            budgetspentpersub += spentbudget;
                            //regulatedbudgetpersub += rspm.BudgetedHours * rspm.Rate;
                            //totalbudgethours += rspm.BudgetedHours;

                            RolePerSubProjectModel foundrspm = allrolesummary.Where(x => x.Employee.Id == rspm.Employee.Id).FirstOrDefault();

                            if (foundrspm != null)
                            {
                                foundrspm.SpentHours += hours;
                                foundrspm.SpentBudget += spentbudget;
                                //foundrspm.BudgetedHours += rspm.BudgetedHours;
                            }
                            else
                            {

                                RolePerSubProjectModel cloned = new RolePerSubProjectModel()
                                {
                                    Id = rspm.Id,
                                    Rate = rspm.Rate,
                                    Role = rspm.Role,
                                    Employee = rspm.Employee,
                                    BudgetedHours = 0,
                                    OverallFee = rspm.OverallFee,
                                    SpentHours = rspm.SpentHours,
                                    SpentBudget = rspm.SpentBudget,
                                    CanDelete = false,
                                };
                                allrolesummary.Add(cloned);
                            }
                        }
                    }
                }

                if (!IsActive)
                {
                    spm.CanEdit = false;
                }

                foreach (RolePerSubProjectDbModel rspdb in rolesdbmodel)
                {
                    RolePerSubProjectModel rpdm = spm.RolesPerSub.Where(x => x.Employee.Id == rspdb.EmployeeId).FirstOrDefault();
                    hoursleftpersub += rspdb.BudgetHours;
                    regulatedbudgetpersub += rspdb.BudgetHours * rspdb.Rate;
                    totalbudgethours += rspdb.BudgetHours;

                    if (rpdm == null)
                    {
                        RolePerSubProjectModel rspm = new RolePerSubProjectModel(rspdb.Id, rspdb.Rate, (DefaultRoleEnum)rspdb.Role, rspdb.EmployeeId, spm, rspdb.BudgetHours, spm.Fee);

                        rspm.SpentHours = 0;
                        spm.RolesPerSub.Add(rspm);
                    }

                    RolePerSubProjectModel rpdmall = allrolesummary.Where(x => x.Employee.Id == rspdb.EmployeeId).FirstOrDefault();

                    if (rpdmall == null)
                    {
                        RolePerSubProjectModel rspmnew = new RolePerSubProjectModel(rspdb.Id, rspdb.Rate, (DefaultRoleEnum)rspdb.Role, rspdb.EmployeeId, spm, rspdb.BudgetHours, spm.Fee);
                        rspmnew.CanDelete = false;
                        allrolesummary.Add(rspmnew);
                    }
                    else
                    {
                        rpdmall.BudgetedHours += rspdb.BudgetHours;
                    }
                }

                //List<RolePerSubProjectDbModel> unaccountedFor = new List<RolePerSubProjectDbModel>();

                //foreach (RolePerSubProjectDbModel rspdb in rolesdbmodel)
                //{
                //    if (!allrolesummary.Any(x => x.Employee.Id == rspdb.EmployeeId))
                //    {
                //        unaccountedFor.Add(rspdb);
                //    }
                //}

                //foreach (RolePerSubProjectDbModel rspdb in rolesdbmodel)
                //{
                //    RolePerSubProjectModel rpdm = spm.RolesPerSub.Where(x => x.Employee.Id == rspdb.EmployeeId).FirstOrDefault();

                //    RolePerSubProjectModel rpdmall = allrolesummary.Where(x => x.Employee.Id == rspdb.EmployeeId).FirstOrDefault();

                //if (rpdmall == null)
                //{
                //    RolePerSubProjectModel rspm = new RolePerSubProjectModel(rspdb.Id, rspdb.Rate, (DefaultRoleEnum)rspdb.Role, rspdb.EmployeeId, spm, rspdb.BudgetHours, spm.Fee);
                //    allrolesummary.Add(rspm);
                //}
                //else
                //{
                //    rpdmall.BudgetedHours += rspdb.BudgetHours;
                //}

                spm.HoursUsed = hoursspentpersub;
                spm.HoursLeft = hoursleftpersub - hoursspentpersub;
                spm.FeeUsed = budgetspentpersub;
                spm.FeeLeft = regulatedbudgetpersub - spm.FeeUsed;
                spm.RegulatedBudget = regulatedbudgetpersub;
                spm.TotalHours = totalbudgethours;
                spm.UpdatePercents();

                //if (spdm.IsCurrActive == 1)
                //{
                if (addistrue)
                {
                    spm.baseproject = this;
                    SubProjects.Add(spm);
                }
                //}

                totalregulatedbudget += regulatedbudgetpersub;
                hourstotal += hoursspentpersub;
                hourstotalleft += hoursleftpersub;
                budgetspent += budgetspentpersub;
                //averagerate += rate * hours;
            }

            //all phases subphase
            if (addistrue)
            {
                if (subdbmodels.Count > 0)
                {
                    SubProjectModel allphases = new SubProjectModel();
                    allphases.Description = "All Phases";
                    allphases.TotalHours = hourstotal + hourstotalleft;
                    allphases.HoursUsed = hourstotal;
                    allphases.HoursLeft = hourstotalleft;
                    allphases.RolesPerSub = new ObservableCollection<RolePerSubProjectModel>(allrolesummary);
                    allphases.RegulatedBudget = totalregulatedbudget;
                    allphases.IsVisible = false;
                    allphases.CanDelete = false;
                    allphases.CanEdit = false;
                    allphases.baseproject = this;
                    allphases.CanAdd = false;
                    SubProjects.Add(allphases);
                }
            }
            else
            {
                SubProjectModel allsub = SubProjects.Where(x => x.Description == "All Phases").FirstOrDefault();
                if (allsub != null)
                {
                    allsub.RolesPerSub = new ObservableCollection<RolePerSubProjectModel>(allrolesummary);
                }
            }

            //update project fee?

            TotalRegulatedBudget = totalregulatedbudget;
            HoursSpent = hourstotal;
            HoursLeft = hourstotalleft;
            BudgetSpent = budgetspent;
            PercentofInvoicedFee = Math.Round((TotalBudget / TotalRegulatedBudget) * 100, 2);
            UpdateData();
        }

        public void UpdateData()
        {
            BudgetLeft = TotalRegulatedBudget - BudgetSpent;
            PercentBudgetSpent = Math.Round((BudgetSpent / TotalRegulatedBudget) * 100, 2);
            PercentTotalFeeSpent = Math.Round((BudgetSpent / Fee) * 100, 2);
            //HoursLeft = Math.Round(Math.Max(0, BudgetLeft / AvgRate), 2);
        }

        #region project folder
        public void SelectProjectFolder()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            //dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;

            // Process open file dialog box results
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                //save file
                Projectfolder = dialog.FileName;
            }
        }

        public void CopyProjectFolder()
        {
            if (Projectfolder != null)
            {
                Clipboard.SetText(Projectfolder);
            }
        }

        public void OpenProjectFolder()
        {
            try
            {
                Process.Start(Projectfolder);
            }
            catch { }
        }
        #endregion

        #region arch folder
        public void SelectArchFolder()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            //dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;

            // Process open file dialog box results
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                //save file
                Architectfolder = dialog.FileName;
            }
        }

        public void CopyArchFolder()
        {
            if (Architectfolder != null)
            {
                Clipboard.SetText(Architectfolder);
            }
        }

        public void OpenArchFolder()
        {
            try
            {
                Process.Start(Architectfolder);
            }
            catch { }
        }
        #endregion

        #region drawing folder
        public void SelectDrawingsFolder()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            //dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;

            // Process open file dialog box results
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                //save file
                Drawingsfolder = dialog.FileName;
            }
        }

        public void CopyDrawingsFolder()
        {
            if (Drawingsfolder != null)
            {
                Clipboard.SetText(Drawingsfolder);
            }
        }

        public void OpenDrawingsFolder()
        {
            try
            {
                Process.Start(Drawingsfolder);
            }
            catch { }
        }
        #endregion

        #region plot folder
        public void SelectPlotFolder()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            //dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;

            // Process open file dialog box results
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                //save file
                Plotfolder = dialog.FileName;
            }
        }

        public void CopyPlotFolder()
        {
            if (Plotfolder != null)
            {
                Clipboard.SetText(Plotfolder);
            }
        }

        public void OpenPlotFolder()
        {
            try
            {
                Process.Start(Plotfolder);
            }
            catch { }
        }
        #endregion

        public void UpdateProject()
        {
            ProjectDbModel existingproject = SQLAccess.LoadProjectsById(Id);
            ProjectDbModel project = UpdateProjectModel();

            if (existingproject.IsActive != project.IsActive)
            {
                FormatData(true);

                if (!IsActive)
                {
                    foreach (SubProjectModel sub in SubProjects)
                    {
                        if (sub.Id != 0)
                        {
                            sub.IsActive = false;
                            SubProjectDbModel subproject = new SubProjectDbModel()
                            {
                                Id = sub.Id,
                                ProjectId = sub.ProjectNumber,
                                PointNumber = sub.PointNumber,
                                Description = sub.Description,
                                Fee = sub.Fee,
                                IsActive = 0,
                                SubStart = sub.DateInitiated != null ? (int)long.Parse(sub.DateInitiated?.ToString("yyyyMMdd")) : (int?)null,
                                SubEnd = sub.DateInvoiced != null ? (int)long.Parse(sub.DateInvoiced?.ToString("yyyyMMdd")) : (int?)null,
                                NameOfClient = sub.NameOfClient,
                                IsBillable = sub.IsBillable  == null ? (int?) null : Convert.ToInt32(sub.IsBillable),
                                PercentComplete = sub.PercentComplete,
                                PercentBudget = sub.PercentBudget,
                                IsInvoiced = sub.IsInvoiced ? 1 : 0,
                                ExpandedDescription = sub.ExpandedDescription,
                                IsAdservice = sub.IsAddService ? 1 : 0,
                                NumberOrder = sub.NumberOrder,
                                ClientCompanyName = sub.ClientCompanyName,
                                ClientAddress = sub.ClientAddress,
                                EmployeeIdSigned = sub.EmployeeIdSigned,
                                IsScheduleActive = 0,
                            };
                            SQLAccess.UpdateSubProject(subproject);
                        }
                    }
                }
                else
                {
                    bool foundoneschedule = false;
                    bool foundone = false;
                    //check if one is active
                    foreach (SubProjectModel sub in SubProjects)
                    {
                        if (sub.IsActive)
                        {
                            foundone = true;
                            break;
                        }

                        if (sub.IsScheduleActive)
                        {
                            foundone = true;
                            break;
                        }
                    }
                    if (!foundone)
                    {
                        if (SubProjects.Count > 1)
                        {
                            SubProjectModel subnew = SubProjects[SubProjects.Count - 2];
                            subnew.IsActive = true;
                            SubProjectDbModel subproject = new SubProjectDbModel()
                            {
                                Id = subnew.Id,
                                ProjectId = subnew.ProjectNumber,
                                PointNumber = subnew.PointNumber,
                                Description = subnew.Description,
                                Fee = subnew.Fee,
                                IsActive = 1,
                                SubStart = subnew.DateInitiated != null ? (int)long.Parse(subnew.DateInitiated?.ToString("yyyyMMdd")) : (int?)null,
                                SubEnd = subnew.DateInvoiced != null ? (int)long.Parse(subnew.DateInvoiced?.ToString("yyyyMMdd")) : (int?)null,
                                NameOfClient = subnew.NameOfClient,
                                IsBillable = subnew.IsBillable == null ? (int?)null : Convert.ToInt32(subnew.IsBillable),
                                PercentComplete = subnew.PercentComplete,
                                PercentBudget = subnew.PercentBudget,
                                IsInvoiced = subnew.IsInvoiced ? 1 : 0,
                                ExpandedDescription = subnew.ExpandedDescription,
                                IsAdservice = subnew.IsAddService ? 1 : 0,
                                NumberOrder = subnew.NumberOrder,
                                ClientCompanyName = subnew.ClientCompanyName,
                                ClientAddress = subnew.ClientAddress,
                                EmployeeIdSigned = subnew.EmployeeIdSigned,
                                IsScheduleActive = subnew.IsScheduleActive ? 1 : 0
                            };
                            SQLAccess.UpdateSubProject(subproject);
                        }
                    }

                    if (!foundoneschedule)
                    {
                        if (SubProjects.Count > 1)
                        {
                            SubProjectModel subnew = SubProjects[SubProjects.Count - 2];
                            subnew.IsScheduleActive = true;
                            SubProjectDbModel subproject = new SubProjectDbModel()
                            {
                                Id = subnew.Id,
                                ProjectId = subnew.ProjectNumber,
                                PointNumber = subnew.PointNumber,
                                Description = subnew.Description,
                                Fee = subnew.Fee,
                                IsActive = 1,
                                SubStart = subnew.DateInitiated != null ? (int)long.Parse(subnew.DateInitiated?.ToString("yyyyMMdd")) : (int?)null,
                                SubEnd = subnew.DateInvoiced != null ? (int)long.Parse(subnew.DateInvoiced?.ToString("yyyyMMdd")) : (int?)null,
                                NameOfClient = subnew.NameOfClient,
                                IsBillable = subnew.IsBillable == null ? (int?)null : Convert.ToInt32(subnew.IsBillable),
                                PercentComplete = subnew.PercentComplete,
                                PercentBudget = subnew.PercentBudget,
                                IsInvoiced = subnew.IsInvoiced ? 1 : 0,
                                ExpandedDescription = subnew.ExpandedDescription,
                                IsAdservice = subnew.IsAddService ? 1 : 0,
                                NumberOrder = subnew.NumberOrder,
                                ClientCompanyName = subnew.ClientCompanyName,
                                ClientAddress = subnew.ClientAddress,
                                EmployeeIdSigned = subnew.EmployeeIdSigned,
                                IsScheduleActive = subnew.IsScheduleActive ? 1 : 0
                                
                            };
                            SQLAccess.UpdateSubProject(subproject);
                        }
                    }
                }
            }
        }

        private ProjectDbModel UpdateProjectModel()
        {
            int? duedatevar = null;

            if (DueDate != null)
            {
                duedatevar = (int)long.Parse(DueDate?.ToString("yyyyMMdd"));
            }
            int pmid = 0;
            if (ProjectManager != null)
            {
                pmid = ProjectManager.Id;
            }

            ProjectDbModel project = new ProjectDbModel()
            {
                Id = Id,
                ProjectName = ProjectName,
                ProjectNumber = ProjectNumber,
                DueDate = duedatevar,
                ClientId = Client.Id,
                MarketId = Market.Id,
                ManagerId = pmid,
                Fee = Fee,
                IsActive = IsActive ? 1 : 0,
                PercentComplete = PercentComplete,
                Projectfolder = Projectfolder,
                Drawingsfolder = Drawingsfolder,
                Architectfolder = Architectfolder,
                Plotfolder = Plotfolder,
                ProjectStart = ProjectStart,
                ProjectEnd = ProjectEnd,
                FinalSpent = FinalSpent,
                Remarks = Remarks,
                IsOnHold = Convert.ToInt32(IsOnHold)
            };
            SQLAccess.UpdateProjects(project);
            return project;
        }

        public object Clone()
        {
            return new ProjectModel()
            {
                Id = this.Id,
                ProjectName = this.ProjectName,
                ProjectNumber = this.ProjectNumber,
                DueDate = this.DueDate,
                Client = this.Client,
                Fee = this.Fee,
                Market = this.Market,
                ProjectManager = this.ProjectManager,
                IsActive = this.IsActive,
                PercentComplete = this.PercentComplete,
                Projectfolder = this.Projectfolder,
                Drawingsfolder = this.Drawingsfolder,
                Architectfolder = this.Architectfolder,
                Plotfolder = this.Plotfolder,
                Remarks = this.Remarks,
                IsOnHold = this.IsOnHold
            };
        }

    }


}
