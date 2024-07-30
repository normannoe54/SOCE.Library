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

        
        private ObservableCollection<RoleSummaryModel> _rolesPerSub = new ObservableCollection<RoleSummaryModel>();
        public ObservableCollection<RoleSummaryModel> RolesPerSub
        {
            get { return _rolesPerSub; }
            set
            {
                _rolesPerSub = value;
                RaisePropertyChanged(nameof(RolesPerSub));
            }
        }

        private ObservableCollection<SubProjectSummaryModel> _subProjects = new ObservableCollection<SubProjectSummaryModel>();
        public ObservableCollection<SubProjectSummaryModel> SubProjects
        {
            get { return _subProjects; }
            set
            {
                _subProjects = value;
                RaisePropertyChanged(nameof(SubProjects));
            }
        }

        private ObservableCollection<SubProjectSummaryModel> _subProjectsForSelection = new ObservableCollection<SubProjectSummaryModel>();
        public ObservableCollection<SubProjectSummaryModel> SubProjectsForSelection
        {
            get { return _subProjectsForSelection; }
            set
            {
                _subProjectsForSelection = value;
                RaisePropertyChanged(nameof(SubProjectsForSelection));
            }
        }
        bool firstloadv2 = true;
        private SubProjectSummaryModel _selectedActiveProjectPhase;
        public SubProjectSummaryModel SelectedActiveProjectPhase
        {
            get { return _selectedActiveProjectPhase; }
            set
            {
                if (value != null)
                {
                    if (!firstloadv2 && value != _selectedActiveProjectPhase)
                    {
                        if (_selectedActiveProjectPhase != null)
                        {
                            SQLAccess.UpdateScheduleActive(_selectedActiveProjectPhase.Id, 0);
                            _selectedActiveProjectPhase.IsScheduleActive = false;
                        }

                        SQLAccess.UpdateScheduleActive(value.Id, 1);
                        
                    }
                }

                _selectedActiveProjectPhase = value;

                if (_selectedActiveProjectPhase != null)
                {
                    _selectedActiveProjectPhase.IsScheduleActive = true;
                }

                if (value != null)
                {
                    if (!_selectedActiveProjectPhase.IsActive)
                    {
                        _selectedActiveProjectPhase.IsActive = true;
                        SQLAccess.UpdateActive(_selectedActiveProjectPhase.Id, 1);
                    }
                }

                RaisePropertyChanged(nameof(SelectedActiveProjectPhase));
            }
        }


        public ObservableCollection<EmployeeLowResModel> BaseEmployees = new ObservableCollection<EmployeeLowResModel>();

        private ObservableCollection<EmployeeLowResModel> _employees = new ObservableCollection<EmployeeLowResModel>();
        public ObservableCollection<EmployeeLowResModel> Employees
        {
            get { return _employees; }
            set
            {
                _employees = value;
                RaisePropertyChanged(nameof(Employees));
            }
        }

        private SubProjectSummaryModel _selectedProjectPhase;
        public SubProjectSummaryModel SelectedProjectPhase
        {
            get { return _selectedProjectPhase; }
            set
            {
                _selectedProjectPhase = value;

                if ((_selectedProjectPhase != null || !string.IsNullOrEmpty(_selectedProjectPhase?.PointNumber)))
                {
                    if (_selectedProjectPhase.PointNumber == "All Phases")
                    {
                        List<RoleSummaryModel> newroles = new List<RoleSummaryModel>();
                        foreach (SubProjectSummaryModel spsm in SubProjects)
                        {
                            List<RolePerSubProjectDbModel> roles = SQLAccess.LoadRolesPerSubProject(spsm.Id);
                            foreach (RolePerSubProjectDbModel rpspm in roles)
                            {
                                RoleSummaryModel rolefound = newroles.Where(x => x.Employee.Id == rpspm.EmployeeId && x.Role == (DefaultRoleEnum)rpspm.Role).FirstOrDefault();

                                if (rolefound == null)
                                {
                                    RoleSummaryModel role = new RoleSummaryModel(rpspm, spsm, this, Employees);
                                    role.CanDelete = false;
                                    newroles.Add(role);
                                }
                                else
                                {
                                    rolefound.BudgetedHours += rpspm.BudgetHours;
                                    List<TimesheetRowDbModel> time = SQLAccess.LoadTimeSheetDataByIds(rolefound.Employee.Id, spsm.Id);
                                    rolefound.SpentHours += time.Sum(x => x.TimeEntry);
                                    rolefound.SpentBudget += time.Sum(x => x.BudgetSpent);
                                    rolefound.PlannedBudget += rpspm.BudgetHours * rpspm.Rate;
                                    rolefound.PercentofRegulatedBudget = (rolefound.SpentBudget / rolefound.PlannedBudget) * 100;
                                }
                                
                            }
                        }
                        RolesPerSub = new ObservableCollection<RoleSummaryModel>(newroles);

                    }
                    else
                    {
                        List<RolePerSubProjectDbModel> roles = SQLAccess.LoadRolesPerSubProject(_selectedProjectPhase.Id);
                        List<RoleSummaryModel> newroles = new List<RoleSummaryModel>();
                        foreach (RolePerSubProjectDbModel rpspm in roles)
                        {
                            RoleSummaryModel role = new RoleSummaryModel(rpspm, _selectedProjectPhase, this, Employees);
                            //EmployeeLowResModel emlowres = Employees.Where(x => x.Id == rpspm.EmployeeId).FirstOrDefault();

                            //if (emlowres == null)
                            //{
                            //    EmployeeDbModel emdb = SQLAccess.LoadEmployeeById(rpspm.EmployeeId);
                            //    EmployeeLowResModel emnew = new EmployeeLowResModel(emdb);
                            //    Employees.Add(emnew);
                            //    role.Employee = emnew;
                            //}
                            //else
                            //{
                            //    role.Employee = emlowres;
                            //}

                            newroles.Add(role);
                        }

                        RolesPerSub = new ObservableCollection<RoleSummaryModel>(newroles);
                    }

                     
                    //if (!_selectedProjectPhase.EditSubFieldState)
                    //{
                    //    _selectedProjectPhase.EditSubFieldState = true;
                    //}
                }


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

        private string _forecastChipName = "";
        public string ForecastChipName
        {
            get { return _forecastChipName; }
            set
            {
                _forecastChipName = value;
                RaisePropertyChanged("ForecastChipName");
            }
        }

        private bool _isForecastHoursChecked;
        public bool IsForecastHoursChecked
        {
            get { return _isForecastHoursChecked; }
            set
            {

                if (value)
                {
                    ForecastChipName = "FORECAST HOURS";
                    //IsSelectionVisible = false;
                }
                else
                {
                    ForecastChipName = "STANDARD HOURS";
                    //IsSelectionVisible = true;
                }

                if (value != _isForecastHoursChecked && !firstload)
                {
                    if (BaseProject.ForecastHours != value)
                    {
                        SQLAccess.UpdateForecastStatus(BaseProject.Id, Convert.ToInt32(value));
                        BaseProject.ForecastHours = value;
                    }

                    CollectSubProjectsInfo();
                    //    if (value)
                    //    {
                    //        foreach (SubProjectSummaryModel sub in SubProjects)
                    //        {
                    //            if (sub.IsScheduleActive)
                    //            {
                    //                sub.IsScheduleActive = false;
                    //                SQLAccess.UpdateScheduleActive(sub.Id, 0);
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        SQLAccess.UpdateScheduleActive(SubProjects[0].Id, 1);
                    //        SubProjects[0].IsScheduleActive = true;
                    //        SelectedActiveProjectPhase = SubProjects[0];

                    //    }

                }

                _isForecastHoursChecked = value;
                RaisePropertyChanged("IsForecastHoursChecked");
            }
        }

        bool firstload = true;
        private bool _isOnHoldChecked;
        public bool IsOnHoldChecked
        {
            get { return _isOnHoldChecked; }
            set
            {

                if (value)
                {
                    ChipName = "ON HOLD";
                    IsSelectionVisible = false;
                }
                else
                {
                    ChipName = "IN PROGRESS";
                    IsSelectionVisible = true;
                }

                if (value != _isOnHoldChecked && !firstload)
                {
                    if (BaseProject.IsOnHold != value)
                    {
                        SQLAccess.UpdateOnHoldStatus(BaseProject.Id, Convert.ToInt32(value));
                        BaseProject.IsOnHold = value;
                    }

                    if (value)
                    {
                        foreach (SubProjectSummaryModel sub in SubProjects)
                        {
                            if (sub.IsScheduleActive)
                            {
                                sub.IsScheduleActive = false;
                                SQLAccess.UpdateScheduleActive(sub.Id, 0);
                            }
                        }
                    }
                    else
                    {
                        SQLAccess.UpdateScheduleActive(SubProjects[0].Id, 1);
                        SubProjects[0].IsScheduleActive = true;
                        SelectedActiveProjectPhase = SubProjects[0];

                    }
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

        public ProjectViewResModel BaseProject { get; set; }

        private bool _isSelectionVisible;
        public bool IsSelectionVisible
        {
            get { return _isSelectionVisible; }
            set
            {
                _isSelectionVisible = value;
                RaisePropertyChanged(nameof(IsSelectionVisible));
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

        private double _totalBudget;
        public double TotalBudget
        {
            get { return _totalBudget; }
            set
            {
                _totalBudget = value;
                RaisePropertyChanged(nameof(TotalBudget));
            }
        }

        private double _percentTotalFeeSpent;
        public double PercentTotalFeeSpent
        {
            get { return _percentTotalFeeSpent; }
            set
            {
                _percentTotalFeeSpent = value;
                RaisePropertyChanged(nameof(PercentTotalFeeSpent));
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

        private double _percentBudgetSpent;
        public double PercentBudgetSpent
        {
            get { return _percentBudgetSpent; }
            set
            {
                _percentBudgetSpent = value;
                RaisePropertyChanged(nameof(PercentBudgetSpent));
            }
        }

        private double _totalRegulatedBudget;
        public double TotalRegulatedBudget
        {
            get { return _totalRegulatedBudget; }
            set
            {
                _totalRegulatedBudget = value;
                RaisePropertyChanged(nameof(TotalRegulatedBudget));
            }
        }

        private double _percentofInvoicedFee;
        public double PercentofInvoicedFee
        {
            get { return _percentofInvoicedFee; }
            set
            {
                _percentofInvoicedFee = value;
                RaisePropertyChanged(nameof(PercentofInvoicedFee));
            }
        }

        private double _budgetSpent;
        public double BudgetSpent
        {
            get { return _budgetSpent; }
            set
            {
                _budgetSpent = value;
                RaisePropertyChanged(nameof(BudgetSpent));
            }
        }

        private double _budgetLeft;
        public double BudgetLeft
        {
            get { return _budgetLeft; }
            set
            {
                _budgetLeft = value;
                RaisePropertyChanged(nameof(BudgetLeft));
            }
        }

        private double _hoursLeft;
        public double HoursLeft
        {
            get { return _hoursLeft; }
            set
            {
                _hoursLeft = value;
                RaisePropertyChanged(nameof(HoursLeft));
            }
        }

        private double _hoursSpent;
        public double HoursSpent
        {
            get { return _hoursSpent; }
            set
            {
                _hoursSpent = value;
                RaisePropertyChanged(nameof(HoursSpent));
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

        public ProjectSummaryVM(ProjectViewResModel pm, EmployeeModel employee)
        {
            CanAddPhase = employee.Status != AuthEnum.Standard ? true : false;
            CanEditPhase = employee.Status != AuthEnum.Standard ? true : false;
            BaseProject = pm;


            this.AddSubCommand = new RelayCommand(this.AddSubProject);
            this.AddRoleCommand = new RelayCommand(this.AddRole);

            this.CloseCommand = new RelayCommand(this.CloseWindow);
            this.DeleteSubProject = new RelayCommand<SubProjectSummaryModel>(this.DeleteSub);
            this.DeleteRole = new RelayCommand<RoleSummaryModel>(this.DeleteRoleIfPossible);
            this.ExportDataCommand = new RelayCommand(this.RunExport);
            this.OpenAdCommand = new RelayCommand(this.OpenAd);
            this.MoveUpCommand = new RelayCommand<SubProjectSummaryModel>(this.MoveUpSub);
            this.MoveDownCommand = new RelayCommand<SubProjectSummaryModel>(this.MoveDownSub);

            
            List<EmployeeDbModel> employeesDb = SQLAccess.LoadEmployees();

            ObservableCollection<EmployeeLowResModel> totalemployees = new ObservableCollection<EmployeeLowResModel>();

            foreach (EmployeeDbModel employeenew in employeesDb)
            {
                totalemployees.Add(new EmployeeLowResModel(employeenew));
            }

            //OverallFee = overallfee;
            List<EmployeeLowResModel> newems = totalemployees.OrderBy(x => x.FullName).ToList();
            Employees = new ObservableCollection<EmployeeLowResModel>(newems);

            CollectSubProjectsInfo();

            if (SubProjectsForSelection.Count > 0)
            {
                SelectedProjectPhase = SubProjectsForSelection[SubProjectsForSelection.Count - 1];
            }

            //pm.FormatData(true);
            //SubProjects = pm.SubProjects;
            IsOnHoldChecked = pm.IsOnHold;
            IsForecastHoursChecked = pm.ForecastHours;

            if (!BaseProject.IsActive || IsOnHoldChecked)
            {
                IsSelectionVisible = false;
            }
            else
            {
                IsSelectionVisible = true;
            }

            firstload = false;
            firstloadv2 = false;
        }

        public void CollectSubProjectsInfo()
        {
            List<SubProjectSummaryModel> subs = new List<SubProjectSummaryModel>();
            List<SubProjectDbModel> subdbmodels = SQLAccess.LoadSubProjectsByProject(BaseProject.Id);

            double totalfee = 0;
            TotalBudget = BaseProject.Fee;
            PercentComplete = BaseProject.PercentComplete;

            foreach (SubProjectDbModel spdm in subdbmodels)
            {
                //if (Convert.ToBoolean(spdm.IsBillable) && !Convert.ToBoolean(spdm.IsHourly))
                //{
                //    totalfee += spdm.Fee;
                //}
                if (Convert.ToBoolean(spdm.IsBillable))
                {
                    totalfee += spdm.Fee;
                }
            }

            //update overall fee
            if (BaseProject.Fee != totalfee)
            {
                SQLAccess.UpdateFee(BaseProject.Id, totalfee);
                BaseProject.Fee = totalfee;
                TotalBudget = totalfee;
            }

            double totalregbudget = 0;
            double totalbudgetspent = 0;
            double hoursleft = 0;
            double hoursspent = 0;     

            foreach (SubProjectDbModel spdm in subdbmodels)
            {
                SubProjectSummaryModel spm = new SubProjectSummaryModel(spdm, BaseProject, this) ;
                
                List<RolePerSubProjectDbModel> roles = SQLAccess.LoadRolesPerSubProject(spdm.Id);

                //Add roles if needed
                List<TimesheetRowDbModel> timecheck = SQLAccess.LoadTimeSheetDatabySubId(spdm.Id);

                var groupedlist = timecheck.GroupBy(x => x.EmployeeId).ToList();

                foreach (var item in groupedlist)
                {
                    TimesheetRowDbModel subitem = item.First();
                    if (!roles.Any(x=>x.EmployeeId == subitem.EmployeeId))
                    {

                        try
                        {
                            EmployeeDbModel em = SQLAccess.LoadEmployeeById(subitem.EmployeeId);
                            if (em!= null)
                            {
                                RolePerSubProjectDbModel rpp = new RolePerSubProjectDbModel()
                                {
                                    SubProjectId = subitem.SubProjectId,
                                    EmployeeId = em.Id,
                                    Role = em.DefaultRoleId,
                                    Rate = em.Rate,
                                    BudgetHours = 0
                                };
                                SQLAccess.AddRolesPerSubProject(rpp);
                                roles.Add(rpp);
                            }
                        }
                        catch
                        {
                        }
                    }
                }

                double plannedbudget = 0;
                double budgethours = 0;
                double spenthours = 0;
                double budgetspent = 0;
                foreach (RolePerSubProjectDbModel rpspm in roles)
                {
                    EmployeeDbModel emdb = SQLAccess.LoadEmployeeById(rpspm.EmployeeId);
                    List<TimesheetRowDbModel> time = SQLAccess.LoadTimeSheetDataByIds(emdb.Id, spdm.Id);
                    spenthours += time.Sum(x => x.TimeEntry);
                    budgethours += rpspm.BudgetHours;
                    plannedbudget += rpspm.BudgetHours * rpspm.Rate;
                    budgetspent += time.Sum(x => x.BudgetSpent);
                }
                spm.HoursUsed = spenthours;
                spm.HoursLeft = budgethours - spenthours;
                spm.RegulatedBudget = plannedbudget;
                spm.PercentofInvoicedFee = (plannedbudget / spm.Fee) * 100;
                spm.PercentBudget = (plannedbudget / TotalBudget) * 100;
                spm.TotalHours = budgethours;
                spm.FeeUsed = budgetspent;

                spm.FeeLeft = BaseProject.ForecastHours ? (plannedbudget - budgetspent) : (spm.Fee - budgetspent);

                
                spm.PercentSpent = BaseProject.ForecastHours ? (budgetspent / plannedbudget) * 100 : (budgetspent / spm.Fee) * 100;
                subs.Add(spm);

                hoursspent += spenthours;
                hoursleft += budgethours - spenthours;
                totalregbudget += plannedbudget;
                totalbudgetspent += budgetspent;
                totalfee += spm.Fee;
            }

            TotalRegulatedBudget = totalregbudget;
            PercentofInvoicedFee = (TotalBudget / totalregbudget)*100;
            BudgetSpent = totalbudgetspent;
            BudgetLeft = BaseProject.ForecastHours ? (totalregbudget - totalbudgetspent) : (BaseProject.Fee - totalbudgetspent);
            HoursLeft = hoursleft;
            HoursSpent = hoursspent;
            PercentTotalFeeSpent = (BudgetSpent / TotalBudget)*100;
            PercentBudgetSpent = (BudgetSpent / totalregbudget)*100;

            //List <SubProjectSummaryModel> subsum = new List<SubProjectSummaryModel>(subs.OrderBy(x => x.NumberOrder).ToList());
            SubProjects = new ObservableCollection<SubProjectSummaryModel>(subs);
            SubProjects = SubProjects.Renumber(true);

            SubProjectSummaryModel allphases = new SubProjectSummaryModel() { PointNumber = "All Phases", Description = "All Phases", HoursUsed = hoursspent, HoursLeft = hoursleft,
                RegulatedBudget = totalregbudget, TotalHours = hoursspent + hoursleft,
                CanEdit = false};

            subs = SubProjects.ToList();
            subs.Add(allphases);
            int id = 0;

            foreach (SubProjectSummaryModel subcheck in SubProjects)
            {
                if (subcheck.IsScheduleActive)
                {
                    SelectedActiveProjectPhase = subcheck;
                    break;
                }
            }

            if (SelectedProjectPhase != null)
            {
                int idforselection = SelectedProjectPhase.Id;

                SubProjectsForSelection = new ObservableCollection<SubProjectSummaryModel>(subs);

                if (idforselection >= 0)
                {
                    SubProjectSummaryModel subm = SubProjectsForSelection.Where(x => x.Id == idforselection).FirstOrDefault();
                    if (subm != null)
                    {
                        SelectedProjectPhase = subm;
                    }
                    else
                    {
                        SelectedProjectPhase = SubProjectsForSelection.LastOrDefault();
                    }
                }
            }
            else
            {
                SubProjectsForSelection = new ObservableCollection<SubProjectSummaryModel>(subs);
                SelectedProjectPhase = SubProjectsForSelection.LastOrDefault();
            }
            
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

            //if (resultvm)
            //{
            //    ExportConfirmView ecv = new ExportConfirmView();
            //    ExportConfirmVM ecvm = new ExportConfirmVM(new List<ProjectModel> { BaseProject });
            //    //show progress bar and do stuff
            //    ecv.DataContext = ecvm;
            //    var newres = await DialogHost.Show(ecv, "RootDialog");
            //}
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
        //public void Renumber()
        //{
        //    SubProjects.OrderBy(x => x.NumberOrder);
        //    SubProjectsForSelection.OrderBy(x => x.NumberOrder);
        //}


        private void MoveUpSub(SubProjectSummaryModel sub)
        {
            int ind = SubProjects.IndexOf(sub);

            if (ind > 0)
            {
                SubProjects.Move(ind, ind - 1);
                sub.NumberOrder = ind - 1;
                SubProjects =  SubProjects.Renumber(false);
                
            }

        }

        private void MoveDownSub(SubProjectSummaryModel sub)
        {
            int ind = SubProjects.IndexOf(sub);

            if (ind < SubProjects.Count - 1)
            {
                SubProjects.Move(ind, ind + 1);
                sub.NumberOrder = ind + 1;
                SubProjects =  SubProjects.Renumber(false);
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

        private void DeleteRoleIfPossible(RoleSummaryModel rpsm)
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
                RolesPerSub.Remove(rpsm);
            }

            //update roles
        }

        private void DeleteSub(SubProjectSummaryModel spm)
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
            if (SelectedProjectPhase != null)
            {
                RoleSummaryModel rpspm = new RoleSummaryModel(SelectedProjectPhase, SelectedProjectPhase.Fee, this, Employees);
                rpspm.EditRoleFieldState = false;
                rpspm.SpentHours = 0;
                RolesPerSub.Add(rpspm);
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
