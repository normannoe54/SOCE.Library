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

namespace SOCE.Library.UI.ViewModels
{
    public class ProjectSummaryVM : BaseVM
    {

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

        public ObservableCollection<EmployeeModel> BaseEmployees = new ObservableCollection<EmployeeModel>();

        private ObservableCollection<EmployeeModel> _employees = new ObservableCollection<EmployeeModel>();
        public ObservableCollection<EmployeeModel> Employees
        {
            get { return _employees; }
            set
            {
                _employees = value;
                RaisePropertyChanged(nameof(Employees));
            }
        }

        private SubProjectModel _selectedProjectPhase;
        public SubProjectModel SelectedProjectPhase
        {
            get { return _selectedProjectPhase; }
            set
            {
                if ((_selectedProjectPhase != null || !string.IsNullOrEmpty(_selectedProjectPhase?.PointNumber)) && GlobalEditMode)
                {

                    foreach (RolePerSubProjectModel rpspm in _selectedProjectPhase.RolesPerSub)
                    {
                        if (!rpspm.EditRoleFieldState)
                        {
                            rpspm.EditRoleFieldState = true;
                        }
                    }
                    if (!_selectedProjectPhase.EditSubFieldState)
                    {
                        _selectedProjectPhase.EditSubFieldState = true;
                    }
                }

                _selectedProjectPhase = value;
                //Employees = BaseEmployees;
                ////set stuff
                //foreach (RolePerSubProjectModel role in _selectedProjectPhase.RolesPerSub)
                //{
                //    Employees.Remove(role.Employee);
                //}
                RaisePropertyChanged("SelectedProjectPhase");
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

        private string _chipName = "";
        public string ChipName
        {
            get { return _chipName; }
            set
            {
                _chipName = value;
                RaisePropertyChanged("ChipName");
            }
        }

        private bool _isOnHoldChecked;
        public bool IsOnHoldChecked
        {
            get { return _isOnHoldChecked; }
            set
            {

                if (value)
                {
                    ChipName = "ON HOLD";
                }
                else
                {
                    ChipName = "IN PROGRESS";
                }

                if (value != _isOnHoldChecked)
                {
                    BaseProject.IsOnHold = value;
                    BaseProject.UpdateProject();

                    if (value)
                    {
                        foreach(SubProjectModel sub in SubProjects)
                        {
                            if (sub.IsScheduleActive)
                            {
                                sub.IsScheduleActive = false;
                            }
                        }
                    }
                    else
                    {
                        SubProjects[0].IsScheduleActive = true;
                    }
                    BaseProject.SubProjects = SubProjects;
                    BaseProject.UpdateSubProjects();
                }

                _isOnHoldChecked = value;
                RaisePropertyChanged("IsOnHoldChecked");
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

        private bool _canAddRole = true;
        public bool CanAddRole
        {
            get { return _canAddRole; }
            set
            {
                _canAddRole = value;
                RaisePropertyChanged("CDPhase");
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

        private bool _globalEditMode = true;
        public bool GlobalEditMode
        {
            get { return _globalEditMode; }
            set
            {
                int id = SubProjects.IndexOf(SelectedProjectPhase);

                //if (value)
                //{
                for (int i = 0; i < SubProjects.Count; i++)
                {
                    SubProjectModel sub = SubProjects[i];
                    if (sub.CanEdit)
                    {
                        //List<RolePerSubProjectModel> rolesinp = sub.RolesPerSub.OrderBy(x => x.Id).ToList();
                        for (int j = 0; j < sub.RolesPerSub.Count; j++)
                        {
                            RolePerSubProjectModel role = sub.RolesPerSub[j];

                            if (value)
                            {
                                if (role.Employee != null)
                                {
                                    role.EditRoleFieldState = value;
                                    role.globaleditmode = false;
                                    role.UpdateRolePerSub();
                                }
                            }
                            else
                            {
                                role.globaleditmode = true;
                                role.EditRoleFieldState = value;
                            }

                        }

                        if (value)
                        {
                            sub.EditSubFieldState = value;
                            sub.globaleditmode = false;
                        }
                        else
                        {
                            sub.globaleditmode = true;
                            sub.EditSubFieldState = value;
                        }

                    }
                }

                if (value)
                {
                    BaseProject.UpdateSubProjects();
                    BaseProject.FormatData(false);
                    SubProjects = BaseProject.SubProjects.Renumber(true);
                    //Renumber(true);

                    if (SelectedProjectPhase == null)
                    {
                        SelectedProjectPhase = SubProjects.Where(x => x.Description.ToUpper() == "ALL PHASES").FirstOrDefault();
                    }
                }


                _globalEditMode = value;
                RaisePropertyChanged("GlobalEditMode");
            }
        }

        private ProjectModel _baseProject;
        public ProjectModel BaseProject
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
                    CanAddRole = false;
                }
                RaisePropertyChanged("ActiveProject");
            }
        }

        private bool _isSnychVisible;
        public bool IsSnychVisible
        {
            get { return _isSnychVisible; }
            set
            {
                _isSnychVisible = value;

                RaisePropertyChanged("IsSnychVisible");
            }
        }

        public ICommand AddSubCommand { get; set; }
        public ICommand AddRoleCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand DeleteSubProject { get; set; }
        public ICommand DeleteRole { get; set; }
        public ICommand ExportDataCommand { get; set; }

        public ICommand OpenAdCommand { get; set; }
        public ICommand MoveUpCommand { get; set; }
        public ICommand MoveDownCommand { get; set; }

        //public ICommand AdserviceCommand { get; set; }
        //public ICommand SynchAdserviceFileCommand { get; set; }
        //public ICommand RemoveAdserviceCommand { get; set; }

        public ProjectSummaryVM(ProjectModel pm, EmployeeModel employee)
        {
            CanAddPhase = employee.Status != AuthEnum.Standard ? true : false;
            CanEditPhase = employee.Status != AuthEnum.Standard ? true : false;
            BaseProject = pm;

            //Roles.CollectionChanged += CollectionChanged;
            this.AddSubCommand = new RelayCommand(this.AddSubProject);
            this.AddRoleCommand = new RelayCommand(this.AddRole);

            this.CloseCommand = new RelayCommand(this.CloseWindow);
            this.DeleteSubProject = new RelayCommand<SubProjectModel>(this.DeleteSub);
            this.DeleteRole = new RelayCommand<RolePerSubProjectModel>(this.DeleteRoleIfPossible);
            this.ExportDataCommand = new RelayCommand(this.RunExport);
            this.OpenAdCommand = new RelayCommand(this.OpenAd);
            this.MoveUpCommand = new RelayCommand<SubProjectModel>(this.MoveUpSub);
            this.MoveDownCommand = new RelayCommand<SubProjectModel>(this.MoveDownSub);

            //this.AdserviceCommand = new RelayCommand(this.RunAdserviceCommand);

            ActiveProject = pm.IsActive;
            List<EmployeeDbModel> employeesDb = SQLAccess.LoadEmployees();

            ObservableCollection<EmployeeModel> totalemployees = new ObservableCollection<EmployeeModel>();

            foreach (EmployeeDbModel employeenew in employeesDb)
            {
                totalemployees.Add(new EmployeeModel(employeenew));
            }

            //OverallFee = overallfee;
            Employees = totalemployees;

            pm.FormatData(true);
            SubProjects = pm.SubProjects;

            if (SubProjects.Count > 0)
            {
                SelectedProjectPhase = SubProjects[0];
                SubProjects = SubProjects.Renumber(true);
            }

            IsOnHoldChecked = pm.IsOnHold;
        }

        private async void RunExport()
        {
            YesNoView view = new YesNoView();
            YesNoVM aysvm = new YesNoVM();

            aysvm.Message = $"Are you sure you want to export";
            aysvm.SubMessage = $"[{BaseProject.ProjectNumber}] {BaseProject.ProjectName}";
            view.DataContext = aysvm;

            //show the dialog
            var Result = await DialogHost.Show(view, "RootDialog");

            YesNoVM vm = view.DataContext as YesNoVM;
            bool resultvm = vm.Result;

            if (resultvm)
            {
                ExportConfirmView ecv = new ExportConfirmView();
                ExportConfirmVM ecvm = new ExportConfirmVM(new List<ProjectModel> { BaseProject });
                //show progress bar and do stuff
                ecv.DataContext = ecvm;
                var newres = await DialogHost.Show(ecv, "RootDialog");
            }
        }

        //private void RunAdserviceCommand()
        //{
        //    if (string.IsNullOrEmpty(BaseProject.AdserviceFile))
        //    {
        //        CommonOpenFileDialog dialog = new CommonOpenFileDialog();
        //        dialog.InitialDirectory = BaseProject.Projectfolder;
        //        dialog.IsFolderPicker = true;

        //        // Process open file dialog box results
        //        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
        //        {
        //            string foldername = dialog.FileName + "\\Add services";

        //            //check if it already exists
        //            if (Directory.Exists(foldername))
        //            {
        //                //string filename = $"{BaseProject.ProjectNumber} Add Service Tracking Log.xlsx";

        //                //if (File.Exists(Path.Combine(foldername, filename)))
        //                //{
        //                //    //popup that says file already exists
        //                //}
        //                //else
        //                //{
        //                //    //CreateFile();
        //                //}
        //            }
        //            else
        //            {
        //                //Directory.CreateDirectory(foldername);
        //                //CreateFile();
        //            }
        //        }
        //    }
        //    else
        //    {
        //        try
        //        {
        //            Process.Start(BaseProject.AdserviceFile);
        //        }
        //        catch { }
        //    }
        //}

        private void MoveUpSub(SubProjectModel sub)
        {
            int ind = SubProjects.IndexOf(sub);

            if (ind > 0)
            {
                SubProjects.Move(ind, ind - 1);
                sub.NumberOrder = ind - 1;
                SubProjects =  SubProjects.Renumber(false);

            }

        }

        private void MoveDownSub(SubProjectModel sub)
        {
            int ind = SubProjects.IndexOf(sub);

            if (ind < SubProjects.Count - 1)
            {
                SubProjects.Move(ind, ind + 1);
                sub.NumberOrder = ind + 1;
                SubProjects = SubProjects.Renumber(false);
            }

        }

        //public void Renumber(bool firstload)
        //{
        //    bool toggle0 = false;
        //    SubProjectModel Lastitem = null;
        //    List<SubProjectModel> subs = new List<SubProjectModel>();
        //    for (int i = 0; i < SubProjects.Count; i++)
        //    {
        //        SubProjectModel sub = SubProjects[i];

        //        if (sub.Id != 0)
        //        {
        //            int num = 0;
        //            if (firstload)
        //            {
        //                if (sub.NumberOrder == 0 && !toggle0)
        //                {
        //                    num = 0;
        //                    toggle0 = true;
        //                }
        //                else
        //                {
        //                    num = sub.NumberOrder == 0 ? i : sub.NumberOrder;
        //                }
        //            }
        //            else
        //            {
        //                num = i;
        //            }

        //            SQLAccess.UpdateNumberOrder(sub.Id, num);
        //            sub.NumberOrder = num;
        //            subs.Add(sub);
        //        }
        //        else
        //        {
        //            Lastitem = sub;
        //        }
        //    }

        //    //SubProjects.ToList().Sort((x1, x2) =>  x1.NumberOrder - x2.NumberOrder );
        //    SubProjects = new ObservableCollection<SubProjectModel>(subs.OrderBy(x => x.NumberOrder).ToList());

        //    if (Lastitem != null)
        //    {
        //        SubProjects.Add(Lastitem);
        //    }
        //}

        private void DeleteRoleIfPossible(RolePerSubProjectModel rpsm)
        {
            if (rpsm.Id != 0)
            {
                LeftViewToShow = new YesNoView();
                YesNoVM aysvm = new YesNoVM(rpsm, this);
                LeftViewToShow.DataContext = aysvm;
                ItemToDelete = rpsm;
                LeftDrawerOpen = true;
            }
            else
            {
                SelectedProjectPhase.RolesPerSub.Remove(rpsm);
            }

            //update roles
        }

        private void DeleteSub(SubProjectModel spm)
        {
            if (SubProjects.Count > 1)
            {
                LeftViewToShow = new YesNoView();
                YesNoVM aysvm = new YesNoVM(spm, this);
                LeftViewToShow.DataContext = aysvm;
                ItemToDelete = spm;
                LeftDrawerOpen = true;
            }

        }
        private void AddRole()
        {
            GlobalEditMode = true;
            if (SelectedProjectPhase != null)
            {
                RolePerSubProjectModel rpspm = new RolePerSubProjectModel(SelectedProjectPhase, SelectedProjectPhase.Fee);
                rpspm.EditRoleFieldState = false;
                rpspm.SpentHours = 0;
                rpspm.globaleditmode = !GlobalEditMode;
                SelectedProjectPhase.RolesPerSub.Add(rpspm);
            }
        }

        private void OpenAd()
        {
            //GlobalEditMode = true;
            LeftViewToShow = new SubProjectInfoView();
            SubProjectInfoVM subvminfo = new SubProjectInfoVM(SelectedProjectPhase, this);
            LeftViewToShow.DataContext = subvminfo;
            LeftDrawerOpen = true;
        }

        private void AddSubProject()
        {
            GlobalEditMode = true;
            LeftViewToShow = new AddSubProjectView();
            AddSubProjectVM addsubvm = new AddSubProjectVM(BaseProject, this);
            LeftViewToShow.DataContext = addsubvm;
            LeftDrawerOpen = true;
        }

        private void CloseWindow()
        {
            DialogHost.Close("RootDialog");
        }
    }
}
