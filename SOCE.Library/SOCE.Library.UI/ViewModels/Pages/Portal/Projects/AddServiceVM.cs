using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SOCE.Library.Db;
using System.Collections.ObjectModel;
using SOCE.Library.UI.Views;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace SOCE.Library.UI.ViewModels
{
    public class AddServiceVM : BaseVM
    {
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

        private UserControl _leftViewToShow = new UserControl();
        public UserControl LeftViewToShow
        {
            get { return _leftViewToShow; }
            set
            {
                _leftViewToShow = value;
                RaisePropertyChanged(nameof(LeftViewToShow));
            }
        }

        private object _itemToDelete;
        public object ItemToDelete
        {
            get { return _itemToDelete; }
            set
            {
                _itemToDelete = value;
                RaisePropertyChanged(nameof(ItemToDelete));
            }
        }

        public bool result = false;

        private ObservableCollection<SubProjectAddServiceModel> _subProjects = new ObservableCollection<SubProjectAddServiceModel>();
        public ObservableCollection<SubProjectAddServiceModel> SubProjects
        {
            get { return _subProjects; }
            set
            {
                _subProjects = value;
                RaisePropertyChanged(nameof(SubProjects));
            }
        }

        private SubProjectAddServiceModel _selectedAddService;
        public SubProjectAddServiceModel SelectedAddService
        {
            get { return _selectedAddService; }
            set
            {
                //if ((_selectedAddService != null || !string.IsNullOrEmpty(_selectedAddService?.PointNumber)))
                //{
                //    if (!_selectedAddService.EditSubFieldState)
                //    {
                //        _selectedAddService.EditSubFieldState = true;
                //    }
                //}

                _selectedAddService = value;
                //Employees = BaseEmployees;
                ////set stuff
                //foreach (RolePerSubProjectModel role in _selectedProjectPhase.RolesPerSub)
                //{
                //    Employees.Remove(role.Employee);
                //}
                RaisePropertyChanged("SelectedAddService");
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

        private bool _leftDrawerOpen = false;
        public bool LeftDrawerOpen
        {
            get { return _leftDrawerOpen; }
            set
            {
                _leftDrawerOpen = value;
                RaisePropertyChanged("LeftDrawerOpen");
            }
        }

        private bool _canAddPhase;
        public bool CanAddPhase
        {
            get { return _canAddPhase; }
            set
            {
                _canAddPhase = value;
                RaisePropertyChanged("CanAddPhase");
            }
        }

        private bool _canEditPhase;
        public bool CanEditPhase
        {
            get { return _canEditPhase; }
            set
            {
                _canEditPhase = value;
                RaisePropertyChanged("CanEditPhase");
            }
        }

        private ProjectViewResModel _baseProject;
        public ProjectViewResModel BaseProject
        {
            get { return _baseProject; }
            set
            {
                _baseProject = value;
                RaisePropertyChanged("BaseProject");
            }
        }

        private bool _activeProject;
        public bool ActiveProject
        {
            get { return _activeProject; }
            set
            {
                _activeProject = value;

                if (!_activeProject)
                {
                    CanAddPhase = false;
                }
                RaisePropertyChanged("ActiveProject");
            }
        }

        private AddServiceLogStatusEnum _addServiceLogStatus;
        public AddServiceLogStatusEnum AddServiceLogStatus
        {
            get { return _addServiceLogStatus; }
            set
            {
                _addServiceLogStatus = value;
                RaisePropertyChanged("AddServiceLogStatus");
            }
        } 

        public ICommand AddSubCommand { get; set; }
        //public ICommand CloseCommand { get; set; }
        public ICommand DeleteSubProject { get; set; }
        public ICommand ImortalizeCommand { get; set; }
        public ICommand OpenAdCommand { get; set; }
        public ICommand MoveUpCommand { get; set; }
        public ICommand MoveDownCommand { get; set; }

        //public ICommand AdserviceCommand { get; set; }
        //public ICommand SynchAdserviceFileCommand { get; set; }
        //public ICommand RemoveAdserviceCommand { get; set; }

        public AddServiceVM(ProjectViewResModel pm, EmployeeModel employee)
        {
            CanAddPhase = employee.Status != AuthEnum.Standard ? true : false;
            CanEditPhase = employee.Status != AuthEnum.Standard ? true : false;
            BaseProject = pm;
            //Roles.CollectionChanged += CollectionChanged;
            this.AddSubCommand = new RelayCommand(this.AddSubProject);
            //this.CloseCommand = new RelayCommand(this.CloseWindow);
            this.DeleteSubProject = new RelayCommand<SubProjectAddServiceModel>(this.DeleteSub);
            this.ImortalizeCommand = new RelayCommand(this.RunLogCommand);

            //this.AdserviceCommand = new RelayCommand(this.RunAdserviceCommand);

            ActiveProject = pm.IsActive;

            //pm.FormatData(true);
            LoadProjectManagers();
            LoadAdservice();
        }

        private void LoadProjectManagers()
        {
            List<EmployeeDbModel> PMs = SQLAccess.LoadProjectManagers();

            ObservableCollection<EmployeeLowResModel> members = new ObservableCollection<EmployeeLowResModel>();

            foreach (EmployeeDbModel edbm in PMs)
            {
                members.Add(new EmployeeLowResModel(edbm));
            }
            List<EmployeeLowResModel> newpms = members.OrderBy(x => x.FullName).ToList();
            ProjectManagers = new ObservableCollection<EmployeeLowResModel>(newpms);
        }

        public void LoadAdservice()
        {
            List<SubProjectDbModel> subsdb = SQLAccess.LoadSubProjectsByProject(BaseProject.Id);

            List<SubProjectAddServiceModel> subs = new List<SubProjectAddServiceModel>();

            List<SubProjectDbModel> subdbmodels = SQLAccess.LoadSubProjectsByProject(BaseProject.Id);

            double totalfee = 0;
            double totalbudget = BaseProject.Fee;
            double percentComplete = BaseProject.PercentComplete;

            foreach (SubProjectDbModel spdm in subdbmodels)
            {
                if (Convert.ToBoolean(spdm.IsBillable) && !Convert.ToBoolean(spdm.IsHourly))
                {
                    totalfee += spdm.Fee;
                }
            }

            //update overall fee
            if (BaseProject.Fee != totalfee)
            {
                SQLAccess.UpdateFee(BaseProject.Id, totalfee);
                BaseProject.Fee = totalfee;
            }

            foreach (SubProjectDbModel sub in subsdb)
            {
                if (sub.IsAdservice == 1)
                {
                    List<TimesheetRowDbModel> time = SQLAccess.LoadTimeSheetDatabySubId(sub.Id);

                    double sum = time.Sum(x => x.TimeEntry);

                    EmployeeLowResModel elrsm = null;
                    if (sub.EmployeeIdSigned > 0)
                    {
                        elrsm = ProjectManagers.Where(x => x.Id == sub.EmployeeIdSigned).FirstOrDefault();
                        elrsm.LoadSignature();
                    }

                    SubProjectAddServiceModel spsm = new SubProjectAddServiceModel(sub, BaseProject, elrsm, this);

                    if (sum == 0)
                    {
                        spsm.CanDelete = true;
                    }
                    subs.Add(spsm);
                }
            }

            subs.OrderBy(x => x.NumberOrder);
            SubProjects = new ObservableCollection<SubProjectAddServiceModel>(subs);

            if (SubProjects.Count > 0)
            {
                SelectedAddService = SubProjects.Last();
            }
            CheckLogs();
        }

        private void CheckLogs()
        {
            AddServiceLogStatusEnum finalstatus = AddServiceLogStatusEnum.Incomplete;
            if (String.IsNullOrEmpty(BaseProject.Projectfolder))
            {
                finalstatus = AddServiceLogStatusEnum.Missing;
                foreach (SubProjectAddServiceModel sub in SubProjects)
                {
                    sub.LogStatus = AddServiceLogStatusEnum.Missing;
                }
            }
            else
            {
                string generalpath = BaseProject.Projectfolder;
                string adservicefolderpath = generalpath + "\\Add Services";
                if (!Directory.Exists(adservicefolderpath))
                {
                    foreach (SubProjectAddServiceModel sub in SubProjects)
                    {
                        sub.LogStatus = AddServiceLogStatusEnum.Missing;
                    }
                }
                else
                {
                    foreach (SubProjectAddServiceModel sub in SubProjects)
                    {
                        if (sub.IsBillable)
                        {
                            string indadservicepath = adservicefolderpath + "\\" + $"{BaseProject.ProjectNumber}{sub.PointNumber.Substring(1)}";
                            if (Directory.Exists(indadservicepath))
                            {
                                int fCount = Directory.GetFiles(indadservicepath, "*", SearchOption.AllDirectories).Length;
                                if (fCount > 0 && !sub.IsChangedLog)
                                {
                                    sub.LogStatus = AddServiceLogStatusEnum.Complete;
                                }
                                else
                                {
                                    sub.LogStatus = AddServiceLogStatusEnum.Incomplete;
                                }
                            }
                            else
                            {
                                sub.LogStatus = AddServiceLogStatusEnum.Missing;
                            }
                        }
                        else
                        {
                            sub.LogStatus = sub.IsChangedLog ? AddServiceLogStatusEnum.Incomplete : AddServiceLogStatusEnum.Complete;
                        }
                    }
                }

                bool breaktest = false;
                foreach (SubProjectAddServiceModel sub in SubProjects)
                {
                    if (sub.LogStatus != AddServiceLogStatusEnum.Complete)
                    {
                        breaktest = true;
                        break;
                    }
                }
                if (!breaktest)
                {
                    finalstatus = AddServiceLogStatusEnum.Complete;
                }
            }

            AddServiceLogStatus = finalstatus;
        }

        private void DeleteSub(SubProjectAddServiceModel spm)
        {
            if (SubProjects.Count > 0)
            {
                LeftViewToShow = new YesNoView();
                YesNoVM aysvm = new YesNoVM(spm, this);
                LeftViewToShow.DataContext = aysvm;
                ItemToDelete = spm;
                LeftDrawerOpen = true;
            }
        }

        private void AddSubProject()
        {
            SubProjectAddServiceModel basemodel = null;
            if (SubProjects.Count > 0)
            {
                basemodel = SubProjects.Last();
            }

            LeftViewToShow = new AddAddServiceView();
            AddAddServiceVM addsubvm = new AddAddServiceVM(BaseProject, this, basemodel);
            LeftViewToShow.DataContext = addsubvm;
            LeftDrawerOpen = true;
        }

        public void RunLogCommand()
        {
            LeftViewToShow = new AddServiceRunLogView();
            AddServiceLogVM addsubvm = new AddServiceLogVM(BaseProject, this, SubProjects.ToList());
            LeftViewToShow.DataContext = addsubvm;
            LeftDrawerOpen = true;
        }

        //private void CloseWindow()
        //{
        //    DialogHost.Close("RootDialog");
        //}
    }
}
