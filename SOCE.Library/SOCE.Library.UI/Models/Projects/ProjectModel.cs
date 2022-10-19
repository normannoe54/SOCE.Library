using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using Microsoft.WindowsAPICodePack.Dialogs;
using SOCE.Library.Db;
using SOCE.Library.UI.ViewModels;

namespace SOCE.Library.UI
{
    public class ProjectModel : BaseVM
    {

        private ObservableCollection<RatePerProjectModel> _ratePerProject = new ObservableCollection<RatePerProjectModel>();
        public ObservableCollection<RatePerProjectModel> RatePerProject
        {
            get { return _ratePerProject; }
            set
            {
                _ratePerProject = value;
                RaisePropertyChanged(nameof(RatePerProject));
            }
        }

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
                }

                if (!_isActive && value)
                {
                    ProjectEnd = 0;
                    FinalSpent = 0;
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
                //if (onstartup)
                //{
                //    UpdateProject();
                //}
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
                //if (onstartup)
                //{
                //    UpdateProject();
                //}
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
                //if (onstartup)
                //{
                //    UpdateProject();
                //}
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
                //if (onstartup)
                //{
                //    UpdateProject();
                //}
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
                ComboFieldState = !_editFieldState;

                RaisePropertyChanged(nameof(EditFieldState));
            }
        }

        private bool _comboFieldState;
        public bool ComboFieldState
        {
            get { return _comboFieldState; }
            set
            {
                _comboFieldState = value;
                RaisePropertyChanged(nameof(ComboFieldState));
            }
        }


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

        private bool _canExpand = false;
        public bool CanExpand
        {
            get
            {
                return _canExpand;
            }
            set
            {
                if (IsEditable)
                {
                    _canExpand = value;
                    RaisePropertyChanged(nameof(CanExpand));
                }
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
            onstartup = true;
        }

        public ProjectModel(ProjectDbModel pm, bool iseditable = true)
        {
            IsEditable = iseditable;
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

            Id = pm.Id;
            ProjectName = pm.ProjectName;
            ProjectNumber = pm.ProjectNumber;
            SearchText = String.Format("{0} {1}", ProjectName, ProjectNumber.ToString());
            Fee = pm.Fee;
            IsActive = Convert.ToBoolean(pm.IsActive);
            PercentComplete = pm.PercentComplete;
            ProjectStart = pm.ProjectStart;
            ProjectEnd = pm.ProjectEnd;
            FinalSpent = pm.FinalSpent;

            EmployeeDbModel emdbm = SQLAccess.LoadEmployeeById(pm.ManagerId);
            EmployeeModel em = new EmployeeModel(emdbm);
            ProjectManager = em;

            ClientDbModel cdbm = SQLAccess.LoadClientById(pm.ClientId);
            ClientModel cm = new ClientModel(cdbm);
            Client = cm;

            MarketDbModel mdbm = SQLAccess.LoadMarketeById(pm.MarketId);
            MarketModel mm = new MarketModel(mdbm);
            Market = mm;

            Projectfolder = pm.Projectfolder;
            Drawingsfolder = pm.Drawingsfolder;
            Architectfolder = pm.Architectfolder;
            Plotfolder = pm.Plotfolder;
            FormatData();
            onstartup = true;

        }
        #endregion

        public void FormatData()
        {
            TotalBudget = Fee;

            List<TimesheetRowDbModel> total = new List<TimesheetRowDbModel>();

            //total
            //get all subprojectIds associated with projectId
            List<SubProjectDbModel> subdbmodels = SQLAccess.LoadSubProjectsByProject(Id);

            foreach (SubProjectDbModel spdm in subdbmodels)
            {
                SubProjectModel spm = new SubProjectModel(spdm, Fee);
                SubProjects.Add(spm);
                List<TimesheetRowDbModel> tmdata = SQLAccess.LoadTimeSheetDatabySubId(spdm.Id);
                total.AddRange(tmdata);
            }

            double averagerate = 250;
            double hourstotal = 0;
            double budgetspent = 0;
            int count = 1;

            if (total.Count != 0)
            {
                var grouped = total.OrderBy(x => x.EmployeeId).GroupBy(x => x.EmployeeId);

                count = 0;
                foreach (var item in grouped)
                {
                    EmployeeDbModel employee = SQLAccess.LoadEmployeeById(item.Key);

                    if (employee != null)
                    {
                        //order by date
                        List<TimesheetRowDbModel> employeetimesheetdata = item.OrderBy(x => x.Date).ToList();

                        double hours = employeetimesheetdata.Sum(x => x.TimeEntry);
                        hourstotal += hours;

                        double budgetperemployee = hours * employee.Rate;
                        budgetspent += budgetperemployee;
                        averagerate += employee.Rate;
                        count++;
                    }
                }
            }     

            HoursSpent = hourstotal;
            BudgetSpent = budgetspent;
            BudgetLeft = TotalBudget - BudgetSpent;
            PercentBudgetSpent = Math.Min(Math.Ceiling((BudgetSpent / TotalBudget) * 100), 100);
            HoursLeft = Math.Round(Math.Max(0, BudgetLeft / (averagerate / count)),2);

            //IconforBudgetSummary = PercentBudgetSpent > PercentComplete ? PackIconKind.AlertCircleOutline : PackIconKind.CheckboxMarkedOutline;
            //PercentCompleteColor = PercentBudgetSpent > PercentComplete ? Brushes.Red : Brushes.Green;

            List<RatesPerProjectDbModel> ratesdbmodel = SQLAccess.LoadRatesPerProject(Id);

            foreach (RatesPerProjectDbModel rpp in ratesdbmodel)
            {
                RatePerProjectModel rpm = new RatePerProjectModel(rpp, this, Fee);

                //catch for strange shenanigans
                if (rpm.TotalHours == 0)
                {
                    //remove
                    SQLAccess.DeleteRatesPerProject(rpm.Id);
                }

                RatePerProject.Add(rpm);
            }
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
            Clipboard.SetText(Projectfolder);
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
            Clipboard.SetText(Architectfolder);
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
            Clipboard.SetText(Drawingsfolder);
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
            Clipboard.SetText(Plotfolder);
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
