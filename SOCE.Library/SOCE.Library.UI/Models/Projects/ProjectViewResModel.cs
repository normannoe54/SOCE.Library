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
    public class ProjectViewResModel : PropertyChangedBase
    {
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

        private EmployeeLowResModel _projectManager { get; set; }
        public EmployeeLowResModel ProjectManager
        {
            get
            {
                return _projectManager;
            }
            set
            {
                _projectManager = value;
                RaisePropertyChanged(nameof(ProjectManager));
            }
        }

        private ClientModel _client { get; set; }
        public ClientModel Client
        {
            get
            {
                return _client;
            }
            set
            {
                _client = value;
                RaisePropertyChanged(nameof(Client));
            }
        }

        private MarketModel _market { get; set; }
        public MarketModel Market
        {
            get
            {
                return _market;
            }
            set
            {
                _market = value;
                RaisePropertyChanged(nameof(Market));
            }
        }

        private double _percentComplete { get; set; }
        public double PercentComplete
        {
            get
            {
                return _percentComplete;
            }
            set
            {
                _percentComplete = value;
                RaisePropertyChanged(nameof(PercentComplete));
            }
        }

        private DateTime? _dueDate { get; set; }
        public DateTime? DueDate
        {
            get
            {
                return _dueDate;
            }
            set
            {
                _dueDate = value;
                RaisePropertyChanged(nameof(DueDate));
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

        private bool _isActive = false;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive && !value)
                {
                    ProjectEnd = (int)long.Parse(DateTime.Now.ToString("yyyyMMdd"));
                    //FinalSpent = CollectTotalBudgetSpent();
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
                    UpdateProjectMainModel();
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
                    UpdateProjectMainModel();
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
                    UpdateProjectMainModel();
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
                    UpdateProjectMainModel();
                }
                RaisePropertyChanged(nameof(Plotfolder));
            }
        }

        private double _fee = 0;
        public double Fee
        {
            get { return _fee; }
            set
            {
                _fee = value;
                RaisePropertyChanged(nameof(Fee));
            }
        }

        private bool _isOnHold;
        public bool IsOnHold
        {
            get { return _isOnHold; }
            set
            {
                _isOnHold = value;
                RaisePropertyChanged(nameof(IsOnHold));
            }
        }

        public int ProjectStart;
        public int ProjectEnd;
        public double FinalSpent;

        private bool _editFieldState = true;
        public bool EditFieldState
        {
            get { return _editFieldState; }
            set
            {
                if (!_editFieldState && value)
                {
                    UpdateProjectMainModel();
                }
                _editFieldState = value;
                //ComboFieldState = !_editFieldState;

                RaisePropertyChanged(nameof(EditFieldState));
            }
        }

        public ProjectViewResModel()
        {

        }

        public ProjectViewResModel(ProjectDbModel pm)
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

            Id = pm.Id;
            ProjectName = pm.ProjectName;
            ProjectNumber = pm.ProjectNumber;
            IsActive = Convert.ToBoolean(pm.IsActive);
            PercentComplete = pm.PercentComplete;
            ProjectStart = pm.ProjectStart;
            ProjectEnd = pm.ProjectEnd;
            FinalSpent = pm.FinalSpent;
            Fee = pm.Fee;
            IsOnHold = Convert.ToBoolean(pm.IsOnHold);
            if (pm?.DueDate != null && pm?.DueDate != 0)
            {
                DueDate = DateTime.ParseExact(pm.DueDate.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            }

            Projectfolder = pm.Projectfolder;
            Drawingsfolder = pm.Drawingsfolder;
            Architectfolder = pm.Architectfolder;
            Plotfolder = pm.Plotfolder;
            onstartup = true;
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

        private ProjectDbModel UpdateProjectMainModel()
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
            };
            SQLAccess.UpdateProjectMain(project);

            List<SubProjectDbModel> subs = SQLAccess.LoadSubProjectsByProject(Id);
            if (IsActive)
            {
                bool anyactive = subs.Any(x => x.IsActive == 1);
                if (!anyactive && subs.Count > 0)
                {
                    SubProjectDbModel firstsub = subs[0];
                    SQLAccess.UpdateActive(firstsub.Id, 1);
                }

                if (!IsOnHold)
                {
                    bool anyschedactive = subs.Any(x => x.IsScheduleActive == 1);
                    if (!anyschedactive && subs.Count > 0)
                    {
                        SubProjectDbModel firstsub = subs[0];
                        SQLAccess.UpdateScheduleActive(firstsub.Id, 1);
                    }
                }
            }
            else
            {
                foreach (SubProjectDbModel sub in subs)
                {
                    if (sub.IsActive == 1)
                    {
                        SQLAccess.UpdateActive(sub.Id, 0);
                    }

                    if (!IsOnHold)
                    {
                        if (sub.IsScheduleActive == 1)
                        {
                            SQLAccess.UpdateScheduleActive(sub.Id, 0);
                        }
                    }
                }

            }

            return project;
        }

        //public object Clone()
        //{
        //    return new ProjectLowResModel()
        //    {
        //        Id = this.Id,
        //        ProjectName = this.ProjectName,
        //        ProjectNumber = this.ProjectNumber,
        //        IsActive = this.IsActive,
        //    };
        //}
    }
}
