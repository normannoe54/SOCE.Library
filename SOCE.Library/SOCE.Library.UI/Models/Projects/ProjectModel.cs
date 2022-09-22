using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Media;
using SOCE.Library.Db;

namespace SOCE.Library.UI
{
    public class ProjectModel: PropertyChangedBase
    {
        private ObservableCollection<SubProjectModel> _subProjects;
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

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
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

                RaisePropertyChanged(nameof(PercentComplete));
            }
        }

        private string _projectfolder;
        public string Projectfolder
        {
            get { return _projectfolder; }
            set
            {
                _projectfolder = value;
                RaisePropertyChanged(nameof(Projectfolder));
            }
        }

        private string _drawingsfolder;
        public string Drawingsfolder
        {
            get { return _drawingsfolder; }
            set
            {
                _drawingsfolder = value;
                RaisePropertyChanged(nameof(Drawingsfolder));
            }
        }

        private string _architectfolder;
        public string Architectfolder
        {
            get { return _architectfolder; }
            set
            {
                _architectfolder = value;
                RaisePropertyChanged(nameof(Architectfolder));
            }
        }

        private string _plotfolder;
        public string Plotfolder
        {
            get { return _plotfolder; }
            set
            {
                _plotfolder = value;
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

        public ProjectModel()
        { }
    
        public ProjectModel(ProjectDbModel pm)
        {
            Id = pm.Id;
            ProjectName = pm.ProjectName;
            ProjectNumber = pm.ProjectNumber;
            Fee = pm.Fee;
            IsActive = Convert.ToBoolean(pm.IsActive);
            PercentComplete = pm.PercentComplete;
            Projectfolder = pm.Projectfolder;
            Drawingsfolder = pm.Drawingsfolder;
            Architectfolder = pm.Architectfolder;
            Plotfolder = pm.Plotfolder;

            EmployeeDbModel emdbm = SQLAccess.LoadEmployeeById(pm.ManagerId);
            EmployeeModel em = new EmployeeModel(emdbm);
            ProjectManager = em;

            ClientDbModel cdbm = SQLAccess.LoadClientById(pm.ClientId);
            ClientModel cm = new ClientModel(cdbm);
            Client = cm;

            MarketDbModel mdbm = SQLAccess.LoadMarketeById(pm.MarketId);
            MarketModel mm = new MarketModel(mdbm);
            Market = mm;

        }

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
                IsActive = IsActive?1:0,
                PercentComplete = PercentComplete,
                Projectfolder = Projectfolder,
                Drawingsfolder = Drawingsfolder,
                Architectfolder = Architectfolder,
                Plotfolder = Plotfolder,
        };

            SQLAccess.UpdateProjects(project);
        }

        public object Clone()
        {
            return new ProjectModel() 
            { Id = this.Id,
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
