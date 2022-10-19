using MaterialDesignThemes.Wpf;
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

        private bool _cDPhase = false;
        public bool CDPhase
        {
            get { return _cDPhase; }
            set
            {

                _cDPhase = value;
                RaisePropertyChanged("CDPhase");

            }
        }

        private bool _cAPhase = false;
        public bool CAPhase
        {
            get { return _cAPhase; }
            set
            {

                _cAPhase = value;
                RaisePropertyChanged("CAPhase");

            }
        }

        private bool _pPhase = false;
        public bool PPhase
        {
            get { return _pPhase; }
            set
            {

                _pPhase = value;
                RaisePropertyChanged("PPhase");

            }
        }

        private bool _miscPhase = false;
        public bool MISCPhase
        {
            get { return _miscPhase; }
            set
            {

                _miscPhase = value;
                RaisePropertyChanged("MISCPhase");

            }
        }

        private bool _cDEnabled = true;
        public bool CDEnabled
        {
            get { return _cDEnabled; }
            set
            {

                _cDEnabled = value;
                RaisePropertyChanged("CDEnabled");

            }
        }

        private bool _cAEnabled = true;
        public bool CAEnabled
        {
            get { return _cAEnabled; }
            set
            {

                _cAEnabled = value;
                RaisePropertyChanged("CAEnabled");

            }
        }

        private bool _pEnabled = true;
        public bool PEnabled
        {
            get { return _pEnabled; }
            set
            {

                _pEnabled = value;
                RaisePropertyChanged("PEnabled");

            }
        }

        private bool _miscEnabled = true;
        public bool MISCEnabled
        {
            get { return _miscEnabled; }
            set
            {

                _miscEnabled = value;
                RaisePropertyChanged("MISCEnabled");

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

        public ICommand AddSubProjectCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public AddSubProjectVM(ProjectModel pm)
        {

            this.AddSubProjectCommand = new RelayCommand(this.AddSubProject);
            this.CloseCommand = new RelayCommand(this.CloseWindow);

            List<SubProjectDbModel> Markets = SQLAccess.LoadSubProjectsByProject(1);
            ObservableCollection<SubProjectDbModel> TotalSubProjects = new ObservableCollection<SubProjectDbModel>();

            BaseProject = pm;

            //get latest adservice
            List<SubProjectModel> spms = BaseProject.SubProjects.ToList();

            bool CDphaseexists = !spms.Any(x => x.PointNumber == "CD");
            bool Prephaseexists = !spms.Any(x => x.PointNumber == "Pre");
            bool CAphaseexists = !spms.Any(x => x.PointNumber == "CA");
            bool MISCphaseexists = !spms.Any(x => x.PointNumber == "MISC");

            CDEnabled = CDphaseexists;
            CAEnabled = CAphaseexists;
            PEnabled = Prephaseexists;
            MISCEnabled = MISCphaseexists;

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

                if (CDPhase && CDEnabled)
                {
                    
                    subproject.PointNumber = "CA";
                    subproject.Description = "Construction Document Phase";
                    SQLAccess.AddSubProject(subproject);
                }

                if (CAPhase && CAPhase)
                {
                    subproject.PointNumber = "CD";
                    subproject.Description = "Construction Administration Phase";
                    SQLAccess.AddSubProject(subproject);
                }

                if (PPhase && PPhase)
                {
                    subproject.PointNumber = "Pre";
                    subproject.Description = "Preposal Phase";
                    SQLAccess.AddSubProject(subproject);
                }

                if (MISCPhase && MISCEnabled)
                {
                    subproject.PointNumber = "MISC";
                    subproject.Description = "Miscellaneous";
                    SQLAccess.AddSubProject(subproject);
                }

            }
            else if (AdSelected)
            {
                subproject.PointNumber = LatestAdServiceNumber.ToString();
                subproject.Fee = AdditionalServicesFee;
                subproject.PercentBudget = Math.Round(AdditionalServicesFee / (BaseProject.Fee+ AdditionalServicesFee) *100,2);
                SQLAccess.AddSubProject(subproject);


                //update project overall fee
                BaseProject.Fee += AdditionalServicesFee;
                SQLAccess.UpdateFee(BaseProject.Id, BaseProject.Fee);
            }
            
            //do stuff
            CloseWindow();
        }

        private void CloseWindow()
        {
            DialogHost.Close("RootDialog");
        }
    }
}
