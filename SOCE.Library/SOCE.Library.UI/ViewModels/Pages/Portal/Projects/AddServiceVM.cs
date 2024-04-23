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
            this.ImortalizeCommand = new RelayCommand(this.Imortalize);

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

            ProjectManagers = members;
        }

        public void LoadAdservice()
        {
            List<SubProjectDbModel> subsdb = SQLAccess.LoadAllSubProjectsByProject(BaseProject.Id);

            List<SubProjectAddServiceModel> subs = new List<SubProjectAddServiceModel>();

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

                    SubProjectAddServiceModel spsm = new SubProjectAddServiceModel(sub, BaseProject, elrsm);

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
        }

        private void Imortalize()
        {
            if (String.IsNullOrEmpty(BaseProject.Projectfolder))
            {
                //are you sure you want to blah?
            }
            else
            {
                if (SubProjects.Count > 0)
                {
                    string generalpath = BaseProject.Projectfolder;
                    string adservicefolderpath = generalpath + "\\Add Services";

                    if (Directory.Exists(adservicefolderpath))
                    {
                        //does fileexist
                        string[] files = Directory.GetFiles(adservicefolderpath);

                        string archivepath = adservicefolderpath + "\\Archive";

                        if (!Directory.Exists(archivepath))
                        {
                            DirectoryInfo di = Directory.CreateDirectory(archivepath);
                        }

                        foreach (string file in files)
                        {
                            string oldname = file.Remove(0, adservicefolderpath.Length + 1);
                            string newname = archivepath + "\\" + oldname;

                            if (File.Exists(newname))
                            {
                                File.Delete(newname);
                            }

                            try
                            {
                                File.Move(file, newname);
                            }
                            catch
                            {

                            }
                        }

                        //create log
                        CreateLog(adservicefolderpath);
                    }
                    else
                    {
                        DirectoryInfo di = Directory.CreateDirectory(adservicefolderpath);
                        //create log
                        CreateLog(adservicefolderpath);
                    }
                    Process.Start(adservicefolderpath);
                }
            }

        }


        private void CreateLog(string path)
        {
            try
            {
                string finalpath = $"{path}\\{BaseProject.ProjectNumber}_Add_Service_Tracking_Log_{DateTime.Now.ToString("yyyyMMdd")}.xlsx";

                File.WriteAllBytes(finalpath, Properties.Resources.AddServiceTrackingLog);
                Excel.Excel exinst = new Excel.Excel(finalpath);
                Thread.Sleep(200);

                exinst.WriteCell(8, 4, $"{BaseProject.ProjectName}");
                exinst.WriteCell(9, 4, $"{BaseProject.ProjectNumber}");
                exinst.WriteCell(10, 4, $"{BaseProject.Client.ClientName}");

                int row = 13;
                foreach (SubProjectAddServiceModel submodel in SubProjects)
                {
                    exinst.WriteCell(row, 3, $"{BaseProject.ProjectNumber}{submodel.PointNumber.Substring(1)}");
                    exinst.WriteCell(row, 4, submodel.Description);
                    exinst.WriteCell(row, 5, submodel.DateInitiated?.ToString("MM/dd/yyyy"));
                    exinst.WriteCell(row, 6, submodel.IsBillable ? "YES" : "NO");
                    exinst.WriteCell(row, 7, submodel.DateInvoiced?.ToString("MM/dd/yyyy"));

                    if (submodel.IsHourly && submodel.Fee <= 0)
                    {
                        exinst.WriteCell(row, 8, "Hourly");
                    }
                    else if (submodel.IsHourly && submodel.Fee > 0)
                    {
                        exinst.WriteCell(row, 8, $"Hourly Not to Exceed (${submodel.Fee:n0})");
                    }
                    else
                    {
                        exinst.WriteCell(row, 8, $" ${submodel.Fee:n0}");
                    }

                    row++;
                }

                exinst.SaveDocument();

                foreach (SubProjectAddServiceModel submodel in SubProjects)
                {
                    if (submodel.IsBillable)
                    {
                        exinst.CopyFirstWorksheet($"Proposal #{submodel.PointNumber.Substring(2)}", "Default");
                        exinst.WriteCell(7, 4, submodel.PersonAddressed);
                        exinst.WriteCell(9, 4, submodel.ClientAddress);
                        exinst.WriteCell(11, 4, submodel.ClientCity);
                        //exinst.WriteCell(11, 4, submodel.DateInitiated?.ToString("MM/dd/yyyy"));
                        exinst.WriteCell(13, 4, submodel.NameOfClient);
                        string date = submodel.DateSent?.ToString("MMMM dd, yyyy");
                        //exinst.WriteCell(7, 10, submodel.DateInitiated?.ToString("MM/dd/yyyy"));
                        exinst.WriteCell(7, 10, date);
                        exinst.WriteCell(9, 10, BaseProject.ProjectName);
                        exinst.WriteCell(11, 10, $"{BaseProject.ProjectNumber}{submodel.PointNumber.Substring(1)}");
                        exinst.WriteCell(13, 10, submodel.ClientCompanyName);
                        exinst.WriteCell(16, 5, submodel.Description);

                        if (submodel.IsHourly && submodel.Fee <= 0)
                        {
                            exinst.WriteCell(23, 5, "Hourly");
                        }
                        else if (submodel.IsHourly && submodel.Fee > 0)
                        {
                            exinst.WriteCell(23, 5, $"Hourly Not to Exceed (${submodel.Fee:n0})");
                        }
                        else
                        {
                            exinst.WriteCell(23, 5, $" ${submodel.Fee:n0}");
                        }

                        exinst.WriteCell(32, 5, submodel.SelectedEmployee?.FullName);

                        if (submodel.SelectedEmployee?.SignatureOfPM != null)
                        {

                            exinst.AddPicture(31, 4, submodel.SelectedEmployee.SignatureOfPM);
                        }

                        exinst.SaveDocument();
                    }
                }
            }
            catch
            {

            }

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

        //private void CloseWindow()
        //{
        //    DialogHost.Close("RootDialog");
        //}
    }
}
