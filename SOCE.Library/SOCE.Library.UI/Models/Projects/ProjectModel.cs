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

        public int Id { get; set; }
        public string ProjectName { get; set; }
        public int ProjectNumber { get; set; }


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

        public double Fee { get; set; }

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

        public string JobNumStr
        {
            get
            {
                string jobnumstr = $"[{ProjectNumber.ToString()}]";
                return jobnumstr;
            }
        }

        public ProjectModel()
        { }

        public ProjectModel(ProjectDbModel pm)
        {
            Id = pm.Id;
            ProjectName = pm.ProjectName;
            ProjectNumber = pm.ProjectNumber;
            ClientName = "";
            Fee = pm.Fee;
        }

        public void UpdateProject()
        {
            ProjectDbModel project = new ProjectDbModel()
            {
                Id = Id,
                ProjectName = FirstName,
                ProjectNumber = LastName,
                ClientId = (int)Status,
                MarketId = Title,
                ManagerId = Email,
                Fee = Fee,
                IsActive = PhoneNumber,
                PercentComplete = Extension
            };

            SQLAccess.UpdateEmployee(employee);
        }

        public object Clone()
        {
            return new ProjectModel() { Id = this.Id, ProjectName = this.ProjectName, ProjectNumber = this.ProjectNumber, ClientName = this.ClientName, Fee = this.Fee };
        }

    }
}
