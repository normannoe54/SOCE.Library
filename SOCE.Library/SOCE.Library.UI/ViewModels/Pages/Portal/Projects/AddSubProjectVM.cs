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
    public class AddSubProjectVM : BaseVM
    {
        public bool result = false;

        private ProjectModel baseProject = new ProjectModel();
        public ProjectModel BaseProject
        {
            get { return baseProject; }
            set
            {
                baseProject = value;
                RaisePropertyChanged(nameof(BaseProject));
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
        
        private bool _phaseSelected = true;
        public bool PhaseSelected
        {
            get { return _phaseSelected; }
            set
            {

                _phaseSelected = value;
                RaisePropertyChanged("PhaseSelected");

            }
        }

        private bool _adSelected = false;
        public bool AdSelected
        {
            get { return _adSelected; }
            set
            {

                _adSelected = value;
                RaisePropertyChanged("AdSelected");

            }
        }

        private bool _cLDevPhase;
        public bool CLDevPhase
        {
            get { return _cLDevPhase; }
            set
            {
                _cLDevPhase = value;
                RaisePropertyChanged("CLDevPhase");
            }
        }


        private bool _cDPhase;
        public bool CDPhase
        {
            get { return _cDPhase; }
            set
            {
                _cDPhase = value;
                RaisePropertyChanged("CDPhase");
            }
        }

        private bool _sDPhase;
        public bool SDPhase
        {
            get { return _sDPhase; }
            set
            {
                _sDPhase = value;
                RaisePropertyChanged("SDPhase");
            }
        }

        private bool _dDPhase;
        public bool DDPhase
        {
            get { return _dDPhase; }
            set
            {
                _dDPhase = value;
                RaisePropertyChanged("DDPhase"); 
            }
        }

        private bool _cAPhase;
        public bool CAPhase
        {
            get { return _cAPhase; }
            set
            {
                _cAPhase = value;
                RaisePropertyChanged("CAPhase");
            }
        }

        private bool _pPhase;
        public bool PPhase
        {
            get { return _pPhase; }
            set
            {
                _pPhase = value;
                RaisePropertyChanged("PPhase");
            }
        }

        private bool _invPhase;
        public bool InvPhase
        {
            get { return _invPhase; }
            set
            {
                _invPhase = value;
                RaisePropertyChanged("InvPhase");
            }
        }

        private bool _cOPhase;
        public bool COPhase
        {
            get { return _cOPhase; }
            set
            {
                _cOPhase = value;
                RaisePropertyChanged("COPhase");
            }
        }

        private bool _cLDevEnabled;
        public bool CLDevEnabled
        {
            get { return _cLDevEnabled; }
            set
            {

                _cLDevEnabled = value;
                RaisePropertyChanged("CLDevEnabled");

            }
        }

        private bool _pEnabled;
        public bool PEnabled
        {
            get { return _pEnabled; }
            set
            {

                _pEnabled = value;
                RaisePropertyChanged("PEnabled");

            }
        }

        private bool _sDEnabled;
        public bool SDEnabled
        {
            get { return _sDEnabled; }
            set
            {

                _sDEnabled = value;
                RaisePropertyChanged("SDEnabled");

            }
        }

        private bool _dDEnabled;
        public bool DDEnabled
        {
            get { return _dDEnabled; }
            set
            {

                _dDEnabled = value;
                RaisePropertyChanged("DDEnabled");

            }
        }

        private bool _cDEnabled;
        public bool CDEnabled
        {
            get { return _cDEnabled; }
            set
            {

                _cDEnabled = value;
                RaisePropertyChanged("CDEnabled");

            }
        }

        private bool _cAEnabled;
        public bool CAEnabled
        {
            get { return _cAEnabled; }
            set
            {

                _cAEnabled = value;
                RaisePropertyChanged("CAEnabled");

            }
        }

        private bool _cOEnabled;
        public bool COEnabled
        {
            get { return _cOEnabled; }
            set
            {

                _cOEnabled = value;
                RaisePropertyChanged("COEnabled");

            }
        }

        private bool _invEnabled;
        public bool InvEnabled
        {
            get { return _invEnabled; }
            set
            {

                _invEnabled = value;
                RaisePropertyChanged("InvEnabled");

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

        public ICommand AddSubProjectCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        private ProjectSummaryVM baseViewModel;

        public AddSubProjectVM(ProjectModel pm, ProjectSummaryVM psvm)
        {
            baseViewModel = psvm;
            this.AddSubProjectCommand = new RelayCommand(this.AddSubProject);
            this.CloseCommand = new RelayCommand(this.CloseWindow);

            List<SubProjectDbModel> Markets = SQLAccess.LoadSubProjectsByProject(1);
            ObservableCollection<SubProjectDbModel> TotalSubProjects = new ObservableCollection<SubProjectDbModel>();

            BaseProject = pm;

            //get latest adservice
            List<SubProjectModel> spms = BaseProject.SubProjects.ToList();

            CLDevEnabled = !spms.Any(x => x.PointNumber == "CLD");
            PEnabled = !spms.Any(x => x.PointNumber == "P");
            InvEnabled = !spms.Any(x => x.PointNumber == "INV");
            COEnabled = !spms.Any(x => x.PointNumber == "CO");
            SDEnabled = !spms.Any(x => x.PointNumber == "SD");
            DDEnabled = !spms.Any(x => x.PointNumber == "DD");
            CDEnabled = !spms.Any(x => x.PointNumber == "CD");
            CAEnabled = !spms.Any(x => x.PointNumber == "CA");

            //bool Prephaseexists = !spms.Any(x => x.PointNumber == "Pre");
            //bool CAphaseexists = !spms.Any(x => x.PointNumber == "CA");
            //bool MISCphaseexists = !spms.Any(x => x.PointNumber == "MISC");

            //see if CA, CD, P are available
            double pointmax = 0;

            foreach(SubProjectModel spm in spms)
            {
                double num = 0;
                bool succ = Double.TryParse(spm.PointNumber, out num);

                if (succ)
                {
                    pointmax = Math.Max(num, pointmax);
                }
            }

            LatestAdServiceNumber = pointmax + 0.1;
        }

        public void AddSubProject()
        {
            SubProjectDbModel subproject = new SubProjectDbModel
            {
                ProjectId = BaseProject.Id,
                Description = Description,
                IsActive = 1,
                IsCurrActive = 1,
                IsInvoiced = 0,
                PercentComplete = 0,
                PercentBudget = 0,
                Fee = 0
            };

            if (PhaseSelected)
            {
                if (InvEnabled && InvPhase)
                {
                    subproject.PointNumber = "INV";
                    subproject.Description = "Investigation";
                    SQLAccess.AddSubProject(subproject);
                }

                if (CLDevPhase && CLDevPhase)
                {
                    subproject.PointNumber = "CLD";
                    subproject.Description = "Client Developement";
                    SQLAccess.AddSubProject(subproject);
                }

                if (PPhase && PEnabled)
                {
                    subproject.PointNumber = "P";
                    subproject.Description = "Proposal";
                    SQLAccess.AddSubProject(subproject);
                }

                if (SDPhase && SDEnabled)
                {
                    subproject.PointNumber = "SD";
                    subproject.Description = "Schematic Design";
                    SQLAccess.AddSubProject(subproject);
                }

                if (DDPhase && DDEnabled)
                {
                    subproject.PointNumber = "DD";
                    subproject.Description = "Design Developement";
                    SQLAccess.AddSubProject(subproject);
                }

                if (CDPhase && CDEnabled)
                {
                    subproject.PointNumber = "CD";
                    subproject.Description = "Construction Document";
                    SQLAccess.AddSubProject(subproject);
                }

                if (CAPhase && CAPhase)
                {
                    subproject.PointNumber = "CA";
                    subproject.Description = "Construction Administration";
                    SQLAccess.AddSubProject(subproject);
                }

                if (COPhase && COEnabled)
                {
                    subproject.PointNumber = "CO";
                    subproject.Description = "Construction Observation";
                    SQLAccess.AddSubProject(subproject);
                }
            }
            else if (AdSelected)
            {
                if (String.IsNullOrEmpty(Description) || LatestAdServiceNumber == 0)
                {
                    ErrorMessage = $"Double check that all inputs have been {Environment.NewLine}filled out correctly and try again.";
                    return;
                }

                subproject.PointNumber = LatestAdServiceNumber.ToString();
                subproject.Fee = AdditionalServicesFee;
                subproject.PercentBudget = Math.Round(AdditionalServicesFee / (BaseProject.Fee+ AdditionalServicesFee) *100,2);
                SQLAccess.AddSubProject(subproject);

                //update project overall fee
                BaseProject.Fee += AdditionalServicesFee;
                SQLAccess.UpdateFee(BaseProject.Id, BaseProject.Fee);
            }
            result = true;
            //do stuff
            CloseWindow();
        }

        private void CloseWindow()
        {
            baseViewModel.BaseProject.FormatData(result);
            baseViewModel.LeftDrawerOpen = false;
            //DialogHost.Close("RootDialog");
        }
    }
}
