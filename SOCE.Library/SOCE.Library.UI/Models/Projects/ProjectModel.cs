using MaterialDesignThemes.Wpf;
using Microsoft.WindowsAPICodePack.Dialogs;
using SOCE.Library.Db;
using SOCE.Library.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

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

        public int ProjectStart;

        public int ProjectEnd;

        public double FinalSpent;

        #region project
        public ICommand CopyProjectFolderCommand { get; set; }
        public ICommand SelectProjectFolderCommand { get; set; }
        public ICommand OpenProjectFolderCommand { get; set; }
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

            //EmployeeDbModel emdbm = SQLAccess.LoadEmployeeById(pm.ManagerId);
            //EmployeeModel em = new EmployeeModel(emdbm);
            //ProjectManager = em;

            //ClientDbModel cdbm = SQLAccess.LoadClientById(pm.ClientId);
            //ClientModel cm = new ClientModel(cdbm);
            //Client = cm;

            //MarketDbModel mdbm = SQLAccess.LoadMarketeById(pm.MarketId);
            //MarketModel mm = new MarketModel(mdbm);
            //Market = mm;

            Projectfolder = pm.Projectfolder;
            Drawingsfolder = pm.Drawingsfolder;
            Architectfolder = pm.Architectfolder;
            Plotfolder = pm.Plotfolder;
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
            //SearchText = String.Format("{0} {1}", ProjectNumber.ToString(), ProjectName);
            Fee = pm.Fee;
            IsActive = Convert.ToBoolean(pm.IsActive);
            //PercentComplete = pm.PercentComplete;
            //ProjectStart = pm.ProjectStart;
            //ProjectEnd = pm.ProjectEnd;
            //FinalSpent = pm.FinalSpent;

            //FormatData();
            //onstartup = true;
        }

        private void Constructor()
        {
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

        public void UpdateSubProjects(SubProjectModel subinput = null)
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
            //turn off item mo
            foreach (SubProjectModel spm in SubProjects)
            {
                //spm.PropertyChanged -= SubItemModificationOnPropertyChanged;
                spm.TotalFee = TotalBudget;
                spm.UpdatePercents();
                //spm.UpdatePercentBudget();

                SubProjectDbModel subproject = new SubProjectDbModel()
                {
                    Id = spm.Id,
                    ProjectId = spm.ProjectNumber,
                    PointNumber = spm.PointNumber,
                    Description = spm.Description,
                    Fee = spm.Fee,
                    IsCurrActive = 1,
                    IsActive = spm.IsActive ? 1 : 0,
                    IsInvoiced = spm.IsInvoiced ? 1 : 0,
                    PercentComplete = spm.PercentComplete,
                    PercentBudget = spm.PercentBudget,
                };

                SQLAccess.UpdateSubProject(subproject);

                onehastobeactive = onehastobeactive || spm.IsActive;

            }

            //catch for no actives
            if (subinput !=null)
            {
                if (!onehastobeactive)
                {
                    subinput.IsActive = true;
                    SubProjectDbModel subproject = new SubProjectDbModel()
                    {
                        Id = subinput.Id,
                        ProjectId = subinput.ProjectNumber,
                        PointNumber = subinput.PointNumber,
                        Description = subinput.Description,
                        Fee = subinput.Fee,
                        IsCurrActive = 1,
                        IsActive = 1,
                        IsInvoiced = subinput.IsInvoiced ? 1 : 0,
                        PercentComplete = subinput.PercentComplete,
                        PercentBudget = subinput.PercentBudget,
                    };
                    SQLAccess.UpdateSubProject(subproject);
                }
            }
            
            UpdateData();
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

            Formatted = true;
            TotalBudget = Fee;

            //List<TimesheetRowDbModel> total = new List<TimesheetRowDbModel>();

            //total
            //get all subprojectIds associated with projectId
            List<SubProjectDbModel> subdbmodels = SQLAccess.LoadAllSubProjectsByProject(Id);

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

                if (spdm.IsCurrActive == 1)
                {
                    if (addistrue)
                    {
                        spm = new SubProjectModel(spdm, Fee, this);
                    }
                    else
                    {
                
                        spm = SubProjects.Where(x => x.Id == spdm.Id).FirstOrDefault();
                        spm.RolesPerSub.Clear();
                        if (spm == null)
                        {
                            //delete item from database? should never happen
                            continue;
                        }
                    }
                }
                else
                {
                    continue;
                }

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
                            RolePerSubProjectDbModel rpdm = rolesdbmodel.Where(x => x.EmployeeId == employee.Id).FirstOrDefault();
                            double rate = rpdm.Rate;
                     
                            double hoursleft = rpdm.BudgetHours - hours;
                            hoursspentpersub += hours;
                            hoursleftpersub += hoursleft;
                            budgetspentpersub += spentbudget;
                            regulatedbudgetpersub += rpdm.BudgetHours * rate;
                            totalbudgethours += rpdm.BudgetHours;

                            //get rate
                            if (rpdm != null)
                            {
                                RolePerSubProjectModel rspm = new RolePerSubProjectModel(rpdm.Id, rpdm.Rate, (DefaultRoleEnum)rpdm.Role, rpdm.EmployeeId, spm, rpdm.BudgetHours, spm.Fee);
                                rspm.SpentHours = hours;
                                rspm.SpentBudget = spentbudget;
                                //rspm.PercentofRegulatedBudget = (rpdm.BudgetHours /regulatedbudgetpersub)*100;
                                rate = rpdm.Rate;
                                spm.RolesPerSub.Add(rspm);
                                
                            }

                        }
                    }     
                }

                foreach (RolePerSubProjectDbModel rspdb in rolesdbmodel)
                {
                    RolePerSubProjectModel rpdm = spm.RolesPerSub.Where(x => x.Employee.Id == rspdb.EmployeeId).FirstOrDefault();

                    if (rpdm == null)
                    {
                        hoursleftpersub += rspdb.BudgetHours;
                        regulatedbudgetpersub += rspdb.BudgetHours * rspdb.Rate;
                        totalbudgethours += rspdb.BudgetHours;
                        //add
                        RolePerSubProjectModel rspm = new RolePerSubProjectModel(rspdb.Id, rspdb.Rate, (DefaultRoleEnum)rspdb.Role, rspdb.EmployeeId, spm, rspdb.BudgetHours, spm.Fee);
                        rspm.SpentHours = 0;
                        //rspm.PercentofRegulatedBudget = (rspdb.BudgetHours / regulatedbudgetpersub) * 100;
                        spm.RolesPerSub.Add(rspm);

                    }
                }

                spm.HoursUsed = hoursspentpersub;
                spm.HoursLeft = hoursleftpersub;
                spm.FeeUsed = budgetspentpersub;
                spm.FeeLeft = regulatedbudgetpersub - spm.FeeUsed;
                spm.RegulatedBudget = regulatedbudgetpersub;
                spm.TotalHours = totalbudgethours;
                spm.UpdatePercents();

                if (spdm.IsCurrActive == 1)
                {
                    if (addistrue)
                    {
                        SubProjects.Add(spm);
                    }
                }

                totalregulatedbudget += regulatedbudgetpersub;
                hourstotal += hoursspentpersub;
                hourstotalleft += hoursleftpersub;
                budgetspent += budgetspentpersub;

                //averagerate += rate * hours;
            }

            TotalRegulatedBudget = totalregulatedbudget;
            HoursSpent = hourstotal;
            HoursLeft = hourstotalleft;
            BudgetSpent = budgetspent;
            PercentofInvoicedFee = Math.Round((TotalRegulatedBudget / TotalBudget) * 100, 2);
            UpdateData();
        }

        public void UpdateData()
        {
            BudgetLeft = TotalRegulatedBudget - BudgetSpent;
            PercentBudgetSpent = Math.Min(Math.Round((BudgetSpent / TotalRegulatedBudget) * 100, 2), 100);
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
            if (Projectfolder!=null)
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
            ProjectDbModel project = new ProjectDbModel()
            {
                Id = Id,
                ProjectName = ProjectName,
                ProjectNumber = ProjectNumber,
                ClientId = Client.Id,
                MarketId = Market.Id,
                ManagerId = ProjectManager.Id,
                Fee = Fee,
                IsActive = IsActive ? 1 : 0,
                IsCurrActive = 1,
                PercentComplete = PercentComplete,
                Projectfolder = Projectfolder,
                Drawingsfolder = Drawingsfolder,
                Architectfolder = Architectfolder,
                Plotfolder = Plotfolder,
                ProjectStart = ProjectStart,
                ProjectEnd = ProjectEnd,
                FinalSpent = FinalSpent
            };

            SQLAccess.UpdateProjects(project);
        }

        public object Clone()
        {
            return new ProjectModel()
            {
                Id = this.Id,
                ProjectName = this.ProjectName,
                ProjectNumber = this.ProjectNumber,
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
            };
        }

    }


}
