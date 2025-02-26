﻿using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SOCE.Library.Db;
using System.Collections.ObjectModel;
using System.Linq;

namespace SOCE.Library.UI.ViewModels
{
    public class AddAddServiceVM : BaseVM
    {
        public bool result = false;

        private ProjectViewResModel baseProject = new ProjectViewResModel();
        public ProjectViewResModel BaseProject
        {
            get { return baseProject; }
            set
            {
                baseProject = value;
                RaisePropertyChanged(nameof(BaseProject));
            }
        }

        private string _personAddressedInp;
        public string PersonAddressedInp
        {
            get { return _personAddressedInp; }
            set
            {
                _personAddressedInp = value;
                RaisePropertyChanged("PersonAddressedInp");
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

        private string _nameOfClientInp;
        public string NameOfClientInp
        {
            get { return _nameOfClientInp; }
            set
            {
                _nameOfClientInp = value;
                RaisePropertyChanged("NameOfClientInp");
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

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                RaisePropertyChanged("Description");
            }
        }

        private string _expandedDescription;
        public string ExpandedDescription
        {
            get { return _expandedDescription; }
            set
            {
                _expandedDescription = value;
                RaisePropertyChanged("ExpandedDescription");
            }
        }

        private double _additionalServicesFee;
        public double AdditionalServicesFee
        {
            get { return _additionalServicesFee; }
            set
            {
                _additionalServicesFee = value;
                RaisePropertyChanged("AdditionalServicesFee");
            }
        }

        private double _latestAdserviceNumber = 0.1;
        public double LatestAdServiceNumber
        {
            get { return _latestAdserviceNumber; }
            set
            {

                _latestAdserviceNumber = value;
                RaisePropertyChanged("LatestAdServiceNumber");

            }
        }

        private bool _isBillable = true;
        public bool IsBillable
        {
            get { return _isBillable; }
            set
            {

                _isBillable = value;
                RaisePropertyChanged("IsBillable");

            }
        }

        private bool _isHourly = false;
        public bool IsHourly
        {
            get { return _isHourly; }
            set
            {

                _isHourly = value;
                RaisePropertyChanged("IsHourly");

            }
        }

        private string _errorMessage = "";
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                RaisePropertyChanged("ErrorMessage");
            }
        }
        private AddServiceVM baseViewModel;
        private int BigNumOrder;

        public ICommand AddSubProjectCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        //private ProjectSummaryVM baseViewModel;

        public AddAddServiceVM(ProjectViewResModel pm, AddServiceVM psvm, SubProjectAddServiceModel sub)
        {
            ButtonInAction = true;
            baseViewModel = psvm;
            this.AddSubProjectCommand = new RelayCommand(this.AddSubProject);
            this.CloseCommand = new RelayCommand(this.CloseWindow);

            List<SubProjectDbModel> Markets = SQLAccess.LoadSubProjectsByProject(1);
            ObservableCollection<SubProjectDbModel> TotalSubProjects = new ObservableCollection<SubProjectDbModel>();

            BaseProject = pm;

            //get latest adservice
            List<SubProjectAddServiceModel> spms = psvm.SubProjects.ToList();

            //bool Prephaseexists = !spms.Any(x => x.PointNumber == "Pre");
            //bool CAphaseexists = !spms.Any(x => x.PointNumber == "CA");
            //bool MISCphaseexists = !spms.Any(x => x.PointNumber == "MISC");

            //see if CA, CD, P are available
            double pointmax = 0;
            int maxnumberorder = 0;
            foreach (SubProjectAddServiceModel spm in spms)
            {
                double num = 0;
                bool succ = Double.TryParse(spm.PointNumber, out num);

                if (succ)
                {
                    pointmax = Math.Max(num, pointmax);
                }
                maxnumberorder = Math.Max(spm.NumberOrder, maxnumberorder);
            }

            if (sub != null)
            {
                ClientCompanyNameInp = sub.ClientCompanyName;
                ClientAddressInp = sub.ClientAddress;
                ClientCityInp = sub.ClientCity;
                NameOfClientInp = sub.NameOfClient;
                PersonAddressedInp = sub.PersonAddressed;
            }
            else
            {
                ClientCompanyNameInp = pm.Client.ClientName;
                ClientAddressInp = pm.Client.ClientAddress;
                ClientCityInp = pm.Client.ClientCity;
                NameOfClientInp = pm.Client.NameOfClient;
                PersonAddressedInp = pm.Client.ClientName;
            }

            LatestAdServiceNumber = pointmax + 0.1;
            BigNumOrder = maxnumberorder;
        }

        public void AddSubProject()
        {
            if (!ButtonInAction)
            {
                return;
            }
            ButtonInAction = false;

            SubProjectDbModel subproject = new SubProjectDbModel
            {
                ProjectId = BaseProject.Id,
                Description = Description,
                IsActive = 1,
                IsInvoiced = 0,
                SubStart = (int)long.Parse(DateTime.Now.ToString("yyyyMMdd")),
                ClientAddress = ClientAddressInp,
                ClientCity = ClientCityInp,
                NameOfClient = NameOfClientInp,
                IsBillable = IsBillable ? 1 : 0,
                IsHourly = IsHourly ? 1 : 0,
                ClientCompanyName = ClientCompanyNameInp,
                PercentComplete = 0,
                PercentBudget = 0,
                NumberOrder = Convert.ToInt32(BigNumOrder) + 1,
                PersonToAddress = PersonAddressedInp,
                IsChangedLog = 1,
                Fee = 0
            };

            subproject.IsAdservice = 1;
            if (BaseProject.ProjectManager.Id > 0)
            {
                subproject.EmployeeIdSigned = BaseProject.ProjectManager.Id;
            }

            if (String.IsNullOrEmpty(Description) || LatestAdServiceNumber == 0)
            {
                ErrorMessage = $"Double check that all inputs have been {Environment.NewLine}filled out correctly and try again.";
                ButtonInAction = true;
                return;
            }

            subproject.PointNumber = LatestAdServiceNumber.ToString();
            subproject.ExpandedDescription = ExpandedDescription;

            if (IsHourly)
            {
                AdditionalServicesFee = 0;
            }

            subproject.Fee = AdditionalServicesFee;
            subproject.PercentBudget = Math.Round(AdditionalServicesFee / (BaseProject.Fee + AdditionalServicesFee) * 100, 2);
            SQLAccess.AddSubProject(subproject);

            //update project overall fee
            BaseProject.Fee += AdditionalServicesFee;
            SQLAccess.UpdateFee(BaseProject.Id, BaseProject.Fee);

            result = true;

            ButtonInAction = true;
            //do stuff
            CloseWindow();
        }

        private void CloseWindow()
        {
            if (!ButtonInAction)
            {
                return;
            }
            ButtonInAction = false;

            baseViewModel.LoadAdservice();
            baseViewModel.LeftDrawerOpen = false;
        }
    }
}
