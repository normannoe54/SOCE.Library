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
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Threading;

namespace SOCE.Library.UI.ViewModels
{
    public class ScheduleWeekVM : BaseVM
    {
        public ICommand ClearSelectedProjectCommand { get; set; }
        public ICommand AddNewPhasecommand { get; set; }
        public ICommand SelectedItemChangedCommand { get; set; }
        public ICommand OpenEmployeeSummary { get; set; }
        public ICommand SaveSchedlingHoursCommand { get; set; }
        public ICommand PreviousCommand { get; set; }
        public ICommand NextCommand { get; set; }
        public ICommand CurrentCommand { get; set; }
        public ICommand OpenManagerStatus { get; set; }
        //public ICommand KeyDownCommand { get; set; }

        private ObservableCollection<DateWrapper> _datesummary = new ObservableCollection<DateWrapper>();
        public ObservableCollection<DateWrapper> DateSummary
        {
            get { return _datesummary; }
            set
            {
                _datesummary = value;
                RaisePropertyChanged(nameof(DateSummary));
            }
        }

        private DateTime _firstDate;
        public DateTime FirstDate
        {
            get { return _firstDate; }
            set
            {
                _firstDate = value;
                RaisePropertyChanged(nameof(FirstDate));
            }
        }

        private ObservableCollection<DateWrapper> _employeeDatesummary = new ObservableCollection<DateWrapper>();
        public ObservableCollection<DateWrapper> EmployeeDateSummary
        {
            get { return _employeeDatesummary; }
            set
            {
                _employeeDatesummary = value;
                RaisePropertyChanged(nameof(EmployeeDateSummary));
            }
        }

        


        private Brush _colorStatusPM;
        public Brush ColorStatusPM
        {
            get { return _colorStatusPM; }
            set
            {
                _colorStatusPM = value;
                RaisePropertyChanged(nameof(ColorStatusPM));
            }
        }

        private string _weekDate = "";
        public string WeekDate
        {
            get { return _weekDate; }
            set
            {
                _weekDate = value;
                RaisePropertyChanged(nameof(WeekDate));
            }
        }

        //private ObservableCollection<EmployeeScheduleModel> _employeeSummary = new ObservableCollection<EmployeeScheduleModel>();
        //public ObservableCollection<EmployeeScheduleModel> EmployeeSummary
        //{
        //    get { return _employeeSummary; }
        //    set
        //    {
        //        _employeeSummary = value;
        //        RaisePropertyChanged(nameof(EmployeeSummary));
        //    }
        //}

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

        private ObservableCollection<ProjectLowResModel> _projectList;
        public ObservableCollection<ProjectLowResModel> ProjectList
        {
            get { return _projectList; }
            set
            {
                _projectList = value;
                RaisePropertyChanged(nameof(ProjectList));
            }
        }

        public ObservableCollection<ProjectLowResModel> BaseProjectList { get; set; }

        private ProjectLowResModel _selectedProject;
        public ProjectLowResModel SelectedProject
        {
            get { return _selectedProject; }
            set
            {
                _selectedProject = value;

                //CollectSubProjects();
                if (SchedulingViews.Count > 0 && IsEditableItems)
                {
                    SaveData();
                }

                if (_selectedProject != null)
                {
                    EmployeeDbModel em = SQLAccess.LoadEmployeeById(_selectedProject.ManagerId);

                    if (em != null)
                    {
                        ProjectManager = em.FullName;
                    }
                    PercentComplete = _selectedProject.PercentComplete;
                    DueDate = _selectedProject.DueDate;
                    SubProjects = new ObservableCollection<SubProjectLowResModel>();
                    List<SubProjectDbModel> subs = SQLAccess.LoadSubProjectsByProject(_selectedProject.Id);
                    double feespent = 0;
                    foreach (SubProjectDbModel sub in subs)
                    {
                        List<TimesheetRowDbModel> time = SQLAccess.LoadTimeSheetDatabySubId(sub.Id);
                        List<RolePerSubProjectDbModel> roles = SQLAccess.LoadRolesPerSubProject(sub.Id);
                        SubProjectLowResModel lowres = new SubProjectLowResModel(sub);
                        if (time != null)
                        {
                            lowres.HoursUsed = time.Sum(x => x.TimeEntry);
                            feespent += time.Sum(x => x.BudgetSpent);
                        }

                        if (roles != null)
                        {
                            lowres.HoursLeft = roles.Sum(x => x.BudgetHours) - lowres.HoursUsed;
                        }

                        SubProjects.Add(lowres);
                    }
                    PercentTotalFeeSpent = feespent / _selectedProject.Fee * 100;
                    int idofscheduleactive = 0;
                    bool stuffhappened = false;

                    foreach (SubProjectLowResModel sub in SubProjects)
                    {
                        if (sub.IsScheduleActive)
                        {
                            stuffhappened = true;
                            idofscheduleactive = SubProjects.IndexOf(sub);
                            //members = new ObservableCollection<SubProjectModel>(newlist);
                            break;
                        }
                    }

                    if (stuffhappened)
                    {
                        List<SubProjectLowResModel> newsubs = SubProjects.ToList();
                        newsubs.MoveItemAtIndexToFront(idofscheduleactive);
                        SubProjects = new ObservableCollection<SubProjectLowResModel>(newsubs);
                    }

                    if (SubProjects.Count > 0)
                    {
                        LoadSchedulingData();
                    }
                    IsThisEditable = false;

                }
                else
                {
                    IsThisEditable = true;
                    SchedulingViews.Clear();
                    CopiedSchedulingItems.Clear();
                    PercentTotalFeeSpent = 0;
                    HoursSpent = 0;
                    HoursLeft = 0;
                    ProjectManager = null;
                    DueDate = null;


                }
                //CollectEmployeeSummary();
                RaisePropertyChanged(nameof(SelectedProject));
            }
        }

        private ObservableCollection<SubProjectLowResModel> _subprojects = new ObservableCollection<SubProjectLowResModel>();
        public ObservableCollection<SubProjectLowResModel> SubProjects
        {
            get { return _subprojects; }
            set
            {
                _subprojects = value;
                RaisePropertyChanged(nameof(SubProjects));
            }
        }

        //private ObservableCollection<EmployeeScheduleModel> _projectManagers = new ObservableCollection<EmployeeScheduleModel>();
        //public ObservableCollection<EmployeeScheduleModel> ProjectManagers
        //{
        //    get { return _projectManagers; }
        //    set
        //    {
        //        _projectManagers = value;
        //        RaisePropertyChanged(nameof(ProjectManagers));
        //    }
        //}

        private ObservableCollection<EmployeeScheduleModel> _employees = new ObservableCollection<EmployeeScheduleModel>();
        public ObservableCollection<EmployeeScheduleModel> Employees
        {
            get { return _employees; }
            set
            {
                _employees = value;
                RaisePropertyChanged(nameof(Employees));
            }
        }

        //private SubProjectModel _selectedSubproject;
        //public SubProjectModel SelectedSubproject
        //{
        //    get
        //    {
        //        return _selectedSubproject;
        //    }
        //    set
        //    {
        //        _selectedSubproject = value;

        //        if (_selectedSubproject != null)
        //        {
        //            LoadSchedulingData();
        //        }

        //        RaisePropertyChanged(nameof(SelectedSubproject));
        //    }
        //}

        //private ObservableCollection<RolePerSubProjectModel> CopiedSchedulingItems = new ObservableCollection<RolePerSubProjectModel>();

        //private ObservableCollection<RolePerSubProjectModel> _schedulingItems = new ObservableCollection<RolePerSubProjectModel>();
        //public ObservableCollection<RolePerSubProjectModel> SchedulingItems
        //{
        //    get { return _schedulingItems; }
        //    set
        //    {
        //        _schedulingItems = value;
        //        RaisePropertyChanged(nameof(SchedulingItems));
        //    }
        //}

        private ObservableCollection<ScheduleWeekModel> CopiedSchedulingItems = new ObservableCollection<ScheduleWeekModel>();

        private ObservableCollection<SchedulingControlView> _schedulingViews = new ObservableCollection<SchedulingControlView>();
        public ObservableCollection<SchedulingControlView> SchedulingViews
        {
            get { return _schedulingViews; }
            set
            {
                _schedulingViews = value;
                RaisePropertyChanged(nameof(SchedulingViews));
            }
        }


        private string _clientNumber;
        public string ClientNumber
        {
            get
            {
                return _clientNumber;
            }
            set
            {
                _clientNumber = value;
                RaisePropertyChanged(nameof(ClientNumber));
            }
        }

        private string _message = "";
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                RaisePropertyChanged(nameof(Message));
            }
        }

        private bool _messageVisible = false;
        public bool MessageVisible
        {
            get { return _messageVisible; }
            set
            {
                _messageVisible = value;
                RaisePropertyChanged(nameof(MessageVisible));
            }
        }

        private bool _isEditableItems = true;
        public bool IsEditableItems
        {
            get
            {
                return _isEditableItems;
            }
            set
            {
                _isEditableItems = value;
                RaisePropertyChanged(nameof(IsEditableItems));
            }
        }


        private bool _canDeleteSchedule = false;
        public bool CanDeleteSchedule
        {
            get
            {
                return _canDeleteSchedule;
            }
            set
            {
                _canDeleteSchedule = value;
                RaisePropertyChanged(nameof(CanDeleteSchedule));
            }
        }

        private bool _comboOpen = false;
        public bool ComboOpen
        {
            get { return _comboOpen; }
            set
            {
                _comboOpen = value;

                RaisePropertyChanged(nameof(ComboOpen));
            }
        }

        private bool _isThisEditable = true;
        public bool IsThisEditable
        {
            get { return _isThisEditable; }
            set
            {
                _isThisEditable = value;

                RaisePropertyChanged(nameof(IsThisEditable));
            }
        }

        private double _hoursLeft = 0;
        public double HoursLeft
        {
            get { return _hoursLeft; }
            set
            {
                _hoursLeft = value;
                RaisePropertyChanged(nameof(HoursLeft));
            }
        }

        private double _percentComplete = 0;
        public double PercentComplete
        {
            get { return _percentComplete; }
            set
            {
                _percentComplete = value;
                RaisePropertyChanged(nameof(PercentComplete));
            }
        }

        private string _projectManager = "";
        public string ProjectManager
        {
            get { return _projectManager; }
            set
            {
                _projectManager = value;
                RaisePropertyChanged(nameof(ProjectManager));
            }
        }

        private double _hoursSpent = 0;
        public double HoursSpent
        {
            get { return _hoursSpent; }
            set
            {
                _hoursSpent = value;
                RaisePropertyChanged(nameof(HoursSpent));
            }
        }

        private DateTime? _dueDate;
        public DateTime? DueDate
        {
            get { return _dueDate; }
            set
            {
                _dueDate = value;
                RaisePropertyChanged(nameof(DueDate));
            }
        }

        //private void CollectData()
        //{
        //    List<RolePerSubProjectDbModel> rolesdbmodel = SQLAccess.LoadRolesPerSubProject(SelectedSubproject.Id);
        //    List<TimesheetRowDbModel> tmdata = SQLAccess.LoadTimeSheetDatabySubId(SelectedSubproject.Id);

        //    double hoursspent = 0;
        //    double hoursbudgeted = 0;


        //    foreach (TimesheetRowDbModel time in tmdata)
        //    {
        //        hoursspent += time.TimeEntry;
        //    }

        //    foreach (RolePerSubProjectDbModel role in rolesdbmodel)
        //    {
        //        hoursbudgeted += role.BudgetHours;
        //    }
        //    SelectedSubproject.HoursUsed = hoursspent;
        //    SelectedSubproject.HoursLeft = hoursbudgeted - hoursspent;
        //}

        //private void CollectSubProjects()
        //{

        //    if (SelectedProject == null)
        //    {
        //        return;
        //    }

        //    int id = SelectedProject.Id;

        //    //1 = active subprojects - doesnt work cuz of saved or submitted previous phases
        //    List<SubProjectDbModel> subdbprojects = SQLAccess.LoadSubProjectsByProject(id);

        //    ObservableCollection<SubProjectModel> members = new ObservableCollection<SubProjectModel>();

        //    foreach (SubProjectDbModel sdb in subdbprojects)
        //    {
        //        if (Convert.ToBoolean(sdb.IsActive))
        //        {
        //            members.Add(new SubProjectModel(sdb));
        //        }
        //    }

        //    SubProjects = members;
        //    try
        //    {
        //        SelectedSubproject = SubProjects[0];
        //    }
        //    catch
        //    {
        //    }

        //    ClientNumber = SelectedProject.ProjectNumber.ToString().Substring(2, 2);
        //}

        public ScheduleWeekVM()
        {
            this.ClearSelectedProjectCommand = new RelayCommand(this.ClearSelected);
            this.SelectedItemChangedCommand = new RelayCommand<string>(this.SelectionCombo);
            this.SaveSchedlingHoursCommand = new RelayCommand(this.SaveData);
            this.PreviousCommand = new RelayCommand(PreviousTimesheet);
            this.AddNewPhasecommand = new RelayCommand(AddNewPhase);
            this.NextCommand = new RelayCommand(NextTimesheet);
            this.CurrentCommand = new RelayCommand(CurrentTimesheet);
            this.OpenEmployeeSummary = new RelayCommand(OpenRightDrawer);
            this.OpenManagerStatus = new RelayCommand(OpenRightManager);
            List<EmployeeDbModel> employeesDb = SQLAccess.LoadEmployees();

            List<EmployeeScheduleModel> totalemployees = new List<EmployeeScheduleModel>();

            UpdateDateSummary(DateTime.Today);

            foreach (EmployeeDbModel employeenew in employeesDb)
            {
                EmployeeScheduleModel em = new EmployeeScheduleModel(employeenew);
                totalemployees.Add(em);
            }

            ObservableCollection<EmployeeScheduleModel> ordered = new ObservableCollection<EmployeeScheduleModel>(totalemployees.OrderBy(x => x.FullName).ToList());
            //OverallFee = overallfee;
            Employees = ordered;
            //LoadProjectManagers();
            LoadProjects();
            ProjectList = BaseProjectList;
            CollectEmployeeSummary();
            CheckStatusColor();
            //CollectEmployeeSummary();
            //UpdateLists();
            //LoadSchedulingData();
        }

        private void CheckStatusColor()
        {
            bool iswrong = true;
            foreach (EmployeeScheduleModel employee in Employees)
            {
                if ((employee.Status == AuthEnum.Principal || employee.Status == AuthEnum.PM) && !employee.ScheduleWeekCheck)
                {
                    iswrong = false;
                    break;
                }
            }

            ColorStatusPM = !iswrong ? (SolidColorBrush)new BrushConverter().ConvertFrom("#c92127") : (SolidColorBrush)new BrushConverter().ConvertFrom("#1d9719");
        }

        public void ReloadEmployees()
        {
            List<EmployeeDbModel> employeesDb = SQLAccess.LoadEmployees();

            List<EmployeeScheduleModel> totalemployees = new List<EmployeeScheduleModel>();

            foreach (EmployeeDbModel employeenew in employeesDb)
            {
                EmployeeScheduleModel em = new EmployeeScheduleModel(employeenew);
                totalemployees.Add(em);
            }
            ObservableCollection<EmployeeScheduleModel> ordered = new ObservableCollection<EmployeeScheduleModel>(totalemployees.OrderBy(x => x.LastName).ToList());
            Employees = ordered;
            CollectEmployeeSummary();
            CheckStatusColor();
        }

        public void ClearSelected()
        {
            SelectedProject = null;
        }

        public void AddNewPhase()
        {
            List<SubProjectLowResModel> subsremaining = SubProjects.ToList();
            foreach (SchedulingControlView scv in SchedulingViews)
            {
                ScheduleWeekControlVM scvm = (ScheduleWeekControlVM)scv.DataContext;
                SubProjectLowResModel subm = scvm.SelectedSubproject;
                SubProjectLowResModel sub = subsremaining.Where(x => x.Id == subm.Id).FirstOrDefault();
                subsremaining.Remove(sub);

            }
            if (subsremaining.Count > 0)
            {
                SchedulingControlView schedview = new SchedulingControlView();
                ObservableCollection<SubProjectLowResModel> remaining = new ObservableCollection<SubProjectLowResModel>(subsremaining);
                ScheduleWeekControlVM schedvm = new ScheduleWeekControlVM(DateSummary, remaining[0], remaining, this);
                schedview.DataContext = schedvm;
                SchedulingViews.Add(schedview);
            }

            UpdatePhaseLists();

        }

        public void UpdatePhaseLists()
        {
            List<SubProjectLowResModel> subsremaining = SubProjects.ToList();

            foreach (SchedulingControlView scv in SchedulingViews)
            {
                ScheduleWeekControlVM scvm = (ScheduleWeekControlVM)scv.DataContext;
                SubProjectLowResModel subm = scvm.SelectedSubproject;
                SubProjectLowResModel sub = subsremaining.Where(x => x.Id == subm.Id).FirstOrDefault();

                subsremaining.Remove(sub);
            }

            foreach (SchedulingControlView scv in SchedulingViews)
            {
                ScheduleWeekControlVM scvm = (ScheduleWeekControlVM)scv.DataContext;
                //SubProjectModel subm = scvm.SelectedSubproject;
                ObservableCollection<SubProjectLowResModel> sublist = new ObservableCollection<SubProjectLowResModel>(subsremaining);
                sublist.Insert(0, scvm.SelectedSubproject);
                scvm.SubProjects = sublist;

            }
        }
        //private void CollectEmployeeSummary()
        //{
        //    foreach(EmployeeModel em in EmployeeSummary)
        //    {
        //        em.Entries.Clear();
        //        List<SchedulingDataDbModel> data = SQLAccess.LoadSchedulingDataByEmployee(DateSummary[0].Value, em.Id);
        //        double hours1 = data.Sum(x => x.Hours1);
        //        double hours2 = data.Sum(x => x.Hours2);
        //        double hours3 = data.Sum(x => x.Hours3);
        //        double hours4 = data.Sum(x => x.Hours4);
        //        double hours5 = data.Sum(x => x.Hours5);
        //        double hours6 = data.Sum(x => x.Hours6);
        //        double hours7 = data.Sum(x => x.Hours7);
        //        double hours8 = data.Sum(x => x.Hours8);

        //        Brush brush1 = hours1 > em.ScheduledTotalHours ? (SolidColorBrush)new BrushConverter().ConvertFrom("#c92127") : Brushes.Black;
        //        Brush brush2 = hours2 > em.ScheduledTotalHours ? (SolidColorBrush)new BrushConverter().ConvertFrom("#c92127") : Brushes.Black;
        //        Brush brush3 = hours3 > em.ScheduledTotalHours ? (SolidColorBrush)new BrushConverter().ConvertFrom("#c92127") : Brushes.Black;
        //        Brush brush4 = hours4 > em.ScheduledTotalHours ? (SolidColorBrush)new BrushConverter().ConvertFrom("#c92127") : Brushes.Black;
        //        Brush brush5 = hours5 > em.ScheduledTotalHours ? (SolidColorBrush)new BrushConverter().ConvertFrom("#c92127") : Brushes.Black;
        //        Brush brush6 = hours6 > em.ScheduledTotalHours ? (SolidColorBrush)new BrushConverter().ConvertFrom("#c92127") : Brushes.Black;
        //        Brush brush7 = hours7 > em.ScheduledTotalHours ? (SolidColorBrush)new BrushConverter().ConvertFrom("#c92127") : Brushes.Black;
        //        Brush brush8 = hours8 > em.ScheduledTotalHours ? (SolidColorBrush)new BrushConverter().ConvertFrom("#c92127") : Brushes.Black;


                //SDEntryModel hours1entry = new SDEntryModel() { Date = DateSummary[0].Value, TimeEntry = hours1, CellColor = brush1 };
        //        SDEntryModel hours2entry = new SDEntryModel() { Date = DateSummary[1].Value, TimeEntry = hours2, CellColor = brush2 };
        //        SDEntryModel hours3entry = new SDEntryModel() { Date = DateSummary[2].Value, TimeEntry = hours3, CellColor = brush3 };
        //        SDEntryModel hours4entry = new SDEntryModel() { Date = DateSummary[3].Value, TimeEntry = hours4, CellColor = brush4 };
        //        SDEntryModel hours5entry = new SDEntryModel() { Date = DateSummary[4].Value, TimeEntry = hours5, CellColor = brush5 };
        //        SDEntryModel hours6entry = new SDEntryModel() { Date = DateSummary[5].Value, TimeEntry = hours6, CellColor = brush6 };
        //        SDEntryModel hours7entry = new SDEntryModel() { Date = DateSummary[6].Value, TimeEntry = hours7, CellColor = brush7 };
        //        SDEntryModel hours8entry = new SDEntryModel() { Date = DateSummary[7].Value, TimeEntry = hours8, CellColor = brush8 };


        //        em.Entries.Add(hours1entry);
        //        em.Entries.Add(hours2entry);
        //        em.Entries.Add(hours3entry);
        //        em.Entries.Add(hours4entry);
        //        em.Entries.Add(hours5entry);
        //        em.Entries.Add(hours6entry);
        //        em.Entries.Add(hours7entry);
        //        em.Entries.Add(hours8entry);
        //    }
        //}

        private void SelectionCombo(string project)
        {
            if (SelectedProject == null)
            {
                if (!String.IsNullOrEmpty(project))
                {
                    ComboOpen = true;

                    ProjectList = new ObservableCollection<ProjectLowResModel>(BaseProjectList.Where(x => (x.ProjectName.ToUpper() + x.ProjectNumber.ToString()).Contains(project.ToUpper())));
                }
                else
                {
                    ProjectList = BaseProjectList;
                }

                //SelectedProject = null;
            }
        }


        private void SaveData()
        {
            MessageVisible = true;
            try
            {
                List<ScheduleWeekModel> totalroles = new List<ScheduleWeekModel>();
                foreach (SchedulingControlView sched in SchedulingViews)
                {
                    ScheduleWeekControlVM vm = (ScheduleWeekControlVM)sched.DataContext;

                    //foreach (ScheduleWeekModel role in vm.SchedulingItems)
                    //{
                        //ScheduleWeekModel rolefirst = totalroles.Where(x => x.SelectedEmployee.Id == role.SelectedEmployee.Id && x.PhaseId == role.PhaseId).FirstOrDefault();

                        //if (rolefirst == null)
                        //{
                        //    totalroles.Add(role);
                        //}
                        //else
                        //{
                        //    rolefirst.Entries[0].TimeEntry += role.Entries[0].TimeEntry;
                        //    rolefirst.Entries[1].TimeEntry += role.Entries[1].TimeEntry;
                        //    rolefirst.Entries[2].TimeEntry += role.Entries[2].TimeEntry;
                        //    rolefirst.Entries[3].TimeEntry += role.Entries[3].TimeEntry;
                        //    rolefirst.Entries[4].TimeEntry += role.Entries[4].TimeEntry;
                        //    rolefirst.Entries[5].TimeEntry += role.Entries[5].TimeEntry;
                        //    rolefirst.Entries[6].TimeEntry += role.Entries[6].TimeEntry;
                        //    rolefirst.Entries[7].TimeEntry += role.Entries[7].TimeEntry;

                        //}
                    //}

                    totalroles.AddRange(vm.SchedulingItems);
                }

                foreach (ScheduleWeekModel ctrm in CopiedSchedulingItems)
                {
                    if (ctrm.SelectedEmployee != null && ctrm.SelectedEmployee.Id != 0)
                    {
                        int index = totalroles.FindIndex(x => x?.SelectedEmployee.Id == ctrm.SelectedEmployee.Id && x?.PhaseId == ctrm.PhaseId);

                        //it exists
                        if (index < 0)
                        {
                            SchedulingDataDbModel trdbm = SQLAccess.LoadSingleSchedulingData(ctrm.SelectedEmployee.Id, ctrm.PhaseId, DateSummary[0].Value);

                            if (trdbm != null)
                            {
                                SQLAccess.DeleteSchedulingData(trdbm.Id);

                                //delete role if 0 hours?
                            }

                        }
                    }
                }

                foreach (ScheduleWeekModel trm in totalroles)
                {
                    double hour1 = trm.Entries[0].TimeEntry;
                    double hour2 = trm.Entries[1].TimeEntry;
                    double hour3 = trm.Entries[2].TimeEntry;
                    double hour4 = trm.Entries[3].TimeEntry;
                    double hour5 = trm.Entries[4].TimeEntry;
                    double hour6 = trm.Entries[5].TimeEntry;
                    double hour7 = trm.Entries[6].TimeEntry;
                    double hour8 = trm.Entries[7].TimeEntry;


                    if (trm.SelectedEmployee != null && trm.SelectedEmployee.Id != 0 || (hour1 != 0 && hour2 != 0 && hour3 != 0 && hour4 != 0 && hour5 != 0 && hour6 != 0 && hour7 != 0 && hour8 != 0))
                    {
                        //if (trm.Id == 0)
                        //{
                        //    SchedulingData rpp = new RolePerSubProjectDbModel()
                        //    {
                        //        EmployeeId = trm.SelectedEmployee.Id,
                        //        SubProjectId = trm.Subproject.Id,
                        //        Rate = trm.Rate,
                        //        Role = Convert.ToInt32(trm.Role),
                        //        BudgetHours = 0
                        //    };
                        //    //if (trm.Id == 0)
                        //    //{
                        //    int val = SQLAccess.AddRolesPerSubProject(rpp);
                        //    trm.Id = val;
                        //    //}
                        //}

                        SchedulingDataDbModel dbmodel = new SchedulingDataDbModel()
                        {
                            EmployeeId = trm.SelectedEmployee.Id,
                            EmployeeName = trm.SelectedEmployee.FullName,
                            PhaseId = trm.PhaseId,
                            PhaseName = trm.PhaseName,
                            ProjectName = trm.ProjectName,
                            ProjectNumber = trm.ProjectNumber,
                            ManagerId = trm.ManagerId,
                            Date = (int)long.Parse(DateSummary[0].Value.ToString("yyyyMMdd")),
                            Hours1 = hour1,
                            Hours2 = hour2,
                            Hours3 = hour3,
                            Hours4 = hour4,
                            Hours5 = hour5,
                            Hours6 = hour6,
                            Hours7 = hour7,
                            Hours8 = hour8,
                        };

                        SQLAccess.AddSchedulingData(dbmodel);
                    }
                }

                //int id = SelectedSubproject.Id;

                //SelectedProject.FormatData(true);
                //SubProjects.RemoveAt(SubProjects.Count - 1);
                //SelectedSubproject = SelectedProject.SubProjects.Where(x => x.Id == id).FirstOrDefault();
                //CollectEmployeeSummary();
                //double diffInSeconds = 0;
                //do
                //{
                //    diffInSeconds = (DateTime.Now - starttime).TotalSeconds;

                //} while (diffInSeconds < 2);
                CollectEmployeeSummary();
                Message = "Schedule Saved";
                MessageVisible = false;
            }
            catch
            {
                Message = "Something went Wrong";
                MessageVisible = false;
            }


        }

        private async void PreviousTimesheet()
        {
            CoreAI CurrentPage = IoCCore.Application as CoreAI;
            CurrentPage.MakeBlurry();
            await Task.Run(() => Task.Delay(600));
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => UpdateDateSummary(DateSummary.First().Value.AddDays(-7))));
            if (SelectedProject != null)
            {
                await Application.Current.Dispatcher.BeginInvoke( DispatcherPriority.Background, new Action(() => LoadSchedulingData()));

            }
            await Task.Run(() => Task.Delay(600));
            CurrentPage.MakeClear();
        }

        /// <summary>
        /// Button Press
        /// </summary>
        private async void NextTimesheet()
        {
            CoreAI CurrentPage = IoCCore.Application as CoreAI;
            CurrentPage.MakeBlurry();
            await Task.Run(() => Task.Delay(600));
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => UpdateDateSummary(DateSummary.First().Value.AddDays(7))));
            if (SelectedProject != null)
            {
                await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => LoadSchedulingData()));
                //await Task.Run(() => LoadSchedulingData());

            }
            await Task.Run(() => Task.Delay(600));
            CurrentPage.MakeClear();
        }

        /// <summary>
        /// Button Press
        /// </summary>
        private async void CurrentTimesheet()
        {
            CoreAI CurrentPage = IoCCore.Application as CoreAI;
            CurrentPage.MakeBlurry();
            await Task.Run(() => Task.Delay(600));
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => UpdateDateSummary(DateTime.Now)));
            if (SelectedProject != null)
            {
                await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => LoadSchedulingData()));
                //await Task.Run(() => LoadSchedulingData());

            }
            await Task.Run(() => Task.Delay(600));
            CurrentPage.MakeClear();
        }

        private async void OpenRightDrawer()
        {
            CoreAI CurrentPage = IoCCore.Application as CoreAI;
            CurrentPage.MakeBlurry();
            await Task.Run(() => Task.Delay(600));
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                BaseAI CurrentPage2 = IoCPortal.Application as BaseAI;
                PortalAI portAI = (PortalAI)CurrentPage2;

                EmployeeSummaryScheduleView cv = new EmployeeSummaryScheduleView();
                portAI.RightViewToShow = cv;

                ObservableCollection<EmployeeScheduleModel> employeesforref = new ObservableCollection<EmployeeScheduleModel>();

                foreach (EmployeeScheduleModel em in Employees)
                {
                    employeesforref.Add(new EmployeeScheduleModel() { Id = em.Id, FullName = em.FullName, ScheduledTotalHours = em.ScheduledTotalHours });
                }

                EmployeeScheduleSummaryVM cvm = new EmployeeScheduleSummaryVM(employeesforref, DateSummary);
                portAI.RightViewToShow.DataContext = cvm;
                portAI.RightDrawerOpen = true;
            }));
            await Task.Run(() => Task.Delay(600));
            CurrentPage.MakeClear();
        }

        private async void OpenRightManager()
        {
            CoreAI CurrentPage = IoCCore.Application as CoreAI;
            CurrentPage.MakeBlurry();
            await Task.Run(() => Task.Delay(600));
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                BaseAI CurrentPage2 = IoCPortal.Application as BaseAI;
                PortalAI portAI = (PortalAI)CurrentPage2;

                ManagerStatusView cv = new ManagerStatusView();
                portAI.RightViewToShow = cv;

                ObservableCollection<EmployeeScheduleModel> employeesforref = new ObservableCollection<EmployeeScheduleModel>();

                foreach (EmployeeScheduleModel em in Employees)
                {
                    if (em.Status == AuthEnum.PM || em.Status == AuthEnum.Principal)
                    {
                        employeesforref.Add(new EmployeeScheduleModel() { Id = em.Id, FullName = em.FullName, ScheduleWeekCheck = em.ScheduleWeekCheck });
                    }
                }

                ManagerStatusVM cvm = new ManagerStatusVM(employeesforref, this);
                portAI.RightViewToShow.DataContext = cvm;
                portAI.RightDrawerOpen = true;
            }));
            await Task.Run(() => Task.Delay(600));
            CurrentPage.MakeClear();
        }

        //private void ListPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        //{
        //    RolePerSubProjectModel trm = (RolePerSubProjectModel)sender;
        //    trm.PropertyChanged -= ListPropertyChanged;
        //    UpdateLists();
        //    trm.PropertyChanged += ListPropertyChanged;
        //}

        public void CollectEmployeeSummary()
        {
            foreach (EmployeeScheduleModel em in Employees)
            {
                List<SchedulingDataDbModel> data = SQLAccess.LoadSchedulingDataByEmployee(DateSummary[0].Value, em.Id);
                double hours1 = data.Sum(x => x.Hours1);
                double hours2 = data.Sum(x => x.Hours2);

                Brush solidgreen = Brushes.Blue;
                Brush solidred = Brushes.Red;

                double brushhours1 = Math.Min(hours1, 40);
                double brushhours2 = Math.Min(hours2, 40);

                Brush brush1 = solidred.Blend(solidgreen, 0.9 * (brushhours1 / 40));
                Brush brush2 = solidred.Blend(solidgreen, 0.9 * (brushhours2 / 40));

                SDEntryModel hours1entry = new SDEntryModel() { Date = DateSummary[0].Value, TimeEntry = hours1, CellColor = brush1 };
                SDEntryModel hours2entry = new SDEntryModel() { Date = DateSummary[1].Value, TimeEntry = hours2, CellColor = brush2 };
                em.Hours1 = hours1entry;
                em.Hours2 = hours2entry;
            }
        }

        private void UpdateDateSummary(DateTime currdate)
        {
            EmployeeDateSummary = null ;
            DateSummary = null;
            List<DateWrapper> dates = new List<DateWrapper>();
            List<DateWrapper> employeeDates = new List<DateWrapper>();
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysUntilMonday = ((int)DayOfWeek.Monday - (int)currdate.DayOfWeek + 7) % 7;
            DateTime nextMonday = currdate.AddDays(daysUntilMonday);
            WeekDate = nextMonday.ToString("MM/dd/yyyy");
            dates.Add(new DateWrapper(nextMonday));
            employeeDates.Add(new DateWrapper(nextMonday));
            dates.Add(new DateWrapper(nextMonday.AddDays(7)));
            employeeDates.Add(new DateWrapper(nextMonday.AddDays(7)));
            dates.Add(new DateWrapper(nextMonday.AddDays(14)));
            dates.Add(new DateWrapper(nextMonday.AddDays(21)));
            dates.Add(new DateWrapper(nextMonday.AddDays(28)));
            dates.Add(new DateWrapper(nextMonday.AddDays(35)));
            dates.Add(new DateWrapper(nextMonday.AddDays(42)));
            dates.Add(new DateWrapper(nextMonday.AddDays(49)));

            EmployeeDateSummary = new ObservableCollection<DateWrapper>(employeeDates);
            DateSummary = new ObservableCollection<DateWrapper>(dates);

            FirstDate = DateSummary[0].Value;
        }


        //private void LoadSummary()
        //{
        //    foreach(SubProjectModel sub in SubProjects)
        //    {
        //        List<SchedulingDataDbModel> schedulingdata = SQLAccess.LoadSchedulingData(DateSummary.First().Value, sub.Id);
        //        sub.TotalScheduledHours = schedulingdata.Sum(x => x.Hours1);

        //    }
        //}


        private void LoadSchedulingData()
        {
            CanDeleteSchedule = true;

            //SchedulingViews.Clear();
            SchedulingViews = null;
            CopiedSchedulingItems = null;
            List<ScheduleWeekModel> copiedscheduled = new List<ScheduleWeekModel>();
            List<SchedulingControlView> scheduleitems = new List<SchedulingControlView>();

            //CopiedSchedulingItems.Clear();

            DateTime datestart = DateSummary.First().Value;
            DateTime dateend = DateSummary.Last().Value;
            //List<SchedulingControlView> controlview = new List<SchedulingControlView>();
            foreach (SubProjectLowResModel sub in SubProjects)
            {
                List<SchedulingDataDbModel> schedulingdata = SQLAccess.LoadSchedulingData(DateSummary.First().Value, sub.Id);

                if (schedulingdata.Count > 0)
                {
                    int i = 0;
                    List<ScheduleWeekModel> rolespersub = new List<ScheduleWeekModel>();
                    foreach (SchedulingDataDbModel item in schedulingdata)
                    {
                        ScheduleWeekModel weekmodel = rolespersub.Where(x => x.SelectedEmployee.Id == item.EmployeeId && x.PhaseId == item.PhaseId).FirstOrDefault();
                        
                        if (weekmodel != null)
                        {
                            weekmodel.Entries[0].TimeEntry += item.Hours1;
                            weekmodel.Entries[1].TimeEntry += item.Hours2;
                            weekmodel.Entries[2].TimeEntry += item.Hours3;
                            weekmodel.Entries[3].TimeEntry += item.Hours4;
                            weekmodel.Entries[4].TimeEntry += item.Hours5;
                            weekmodel.Entries[5].TimeEntry += item.Hours6;
                            weekmodel.Entries[6].TimeEntry += item.Hours7;
                            weekmodel.Entries[7].TimeEntry += item.Hours8;

                            SchedulingDataDbModel dbmodel = new SchedulingDataDbModel()
                            {
                                EmployeeId = weekmodel.SelectedEmployee.Id,
                                EmployeeName = weekmodel.SelectedEmployee.FullName,
                                PhaseId = weekmodel.PhaseId,
                                PhaseName = weekmodel.PhaseName,
                                ProjectNumber = weekmodel.ProjectNumber,
                                ManagerId = weekmodel.ManagerId,
                                ProjectName = weekmodel.ProjectName,
                                Date = (int)long.Parse(weekmodel.Date.ToString("yyyyMMdd")),
                                Hours1 = weekmodel.Entries[0].TimeEntry,
                                Hours2 = weekmodel.Entries[1].TimeEntry,
                                Hours3 = weekmodel.Entries[2].TimeEntry,
                                Hours4 = weekmodel.Entries[3].TimeEntry,
                                Hours5 = weekmodel.Entries[4].TimeEntry,
                                Hours6 = weekmodel.Entries[5].TimeEntry,
                                Hours7 = weekmodel.Entries[6].TimeEntry,
                                Hours8 = weekmodel.Entries[7].TimeEntry,
                            };

                            SQLAccess.AddSchedulingData(dbmodel);
                            SQLAccess.DeleteSchedulingData(item.Id);

                            //replace

                            copiedscheduled[i] =((ScheduleWeekModel)weekmodel.Clone());
                        }
                        else
                        {
                            ScheduleWeekModel swm = new ScheduleWeekModel()
                            {
                                EmployeeList = Employees,
                                Id = item.Id,
                                ProjectNumber = item.ProjectNumber,
                                ManagerId = item.ManagerId,
                                PhaseId = item.PhaseId,
                                ProjectName = item.ProjectName,
                                Date = DateTime.ParseExact(item.Date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None),
                                PhaseName = item.PhaseName

                            };
                            EmployeeScheduleModel em = swm.EmployeeList.Where(x => x.Id == item.EmployeeId).FirstOrDefault();

                            if (em != null)
                            {
                                swm.SelectedEmployee = em;
                            }
                            else
                            {
                                EmployeeScheduleModel newem = new EmployeeScheduleModel();
                                newem.Id = item.EmployeeId;
                                newem.FullName = item.EmployeeName;
                                swm.EmployeeList.Add(newem);
                                swm.SelectedEmployee = newem;
                            }

                            //int dateofintetest = (int)long.Parse(DateSummary[0].Value.AddDays(-7).ToString("yyyyMMdd"));
                            
                            Brush brush1 = Brushes.Transparent;
                            Brush brush2 = Brushes.Transparent;
                            Brush brush3 = Brushes.Transparent;
                            Brush brush4 = Brushes.Transparent;
                            Brush brush5 = Brushes.Transparent;
                            Brush brush6 = Brushes.Transparent;
                            Brush brush7 = Brushes.Transparent;
                            Brush brush8 = Brushes.Transparent;

                            if (SelectedProject != null && SelectedProject.DueDate != null)
                            {
                                double diff = ((DateTime)SelectedProject.DueDate - DateSummary[0].Value).TotalDays;
                                double roundup = Math.Ceiling((diff+0.01)/7);
                                Brush redbrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#f9aeae");
                                if (roundup == 1) { brush1 = Brushes.Red; }
                                else if (roundup == 2) { brush2 = redbrush; }
                                else if (roundup == 3) { brush3= redbrush; }
                                else if (roundup == 4) { brush4 = redbrush; }
                                else if (roundup == 5) { brush5 = redbrush; }
                                else if (roundup == 6) { brush6 = redbrush; }
                                else if (roundup == 7) { brush7 = redbrush; }
                                else if (roundup == 8) { brush8= redbrush; }

                            }

                            swm.Entries.Add(new SDEntryModel() { Date = DateSummary[0].Value, TimeEntry = item.Hours1, CellColor = brush1 });
                            swm.Entries.Add(new SDEntryModel() { Date = DateSummary[1].Value, TimeEntry = item.Hours2, CellColor = brush2 });
                            swm.Entries.Add(new SDEntryModel() { Date = DateSummary[2].Value, TimeEntry = item.Hours3, CellColor = brush3 });
                            swm.Entries.Add(new SDEntryModel() { Date = DateSummary[3].Value, TimeEntry = item.Hours4, CellColor = brush4 });
                            swm.Entries.Add(new SDEntryModel() { Date = DateSummary[4].Value, TimeEntry = item.Hours5, CellColor = brush5 });
                            swm.Entries.Add(new SDEntryModel() { Date = DateSummary[5].Value, TimeEntry = item.Hours6, CellColor = brush6 });
                            swm.Entries.Add(new SDEntryModel() { Date = DateSummary[6].Value, TimeEntry = item.Hours7, CellColor = brush7 });
                            swm.Entries.Add(new SDEntryModel() { Date = DateSummary[7].Value, TimeEntry = item.Hours8, CellColor = brush8 });

                            swm.Total = item.Hours1 + item.Hours2 + item.Hours3 + item.Hours4 + item.Hours5 + item.Hours6 + item.Hours7 + item.Hours8;

                            copiedscheduled.Add((ScheduleWeekModel)swm.Clone());

                            rolespersub.Add(swm);

                            i++;
                        }
                    }
                    SchedulingControlView schedview = new SchedulingControlView();
                    ScheduleWeekControlVM schedvm = new ScheduleWeekControlVM(rolespersub, DateSummary, sub, SubProjects, this);
                    schedview.DataContext = schedvm;
                    scheduleitems.Add(schedview);
                }
            }

            SchedulingViews = new ObservableCollection<SchedulingControlView>(scheduleitems);
            CopiedSchedulingItems = new ObservableCollection<ScheduleWeekModel>(copiedscheduled);
            //update employee Id
            //List<SchedulingDataDbModel> schedulingdata = SQLAccess.LoadSchedulingData(DateSummary.First().Value, SelectedSubproject.Id);        

            //CollectEmployeeSummary();
            //UpdateLists();

            DateTime lastSaturday = DateTime.Now.AddDays(-1);
            while (lastSaturday.DayOfWeek != DayOfWeek.Saturday)
                lastSaturday = lastSaturday.AddDays(-1);

            DateTime Curr = DateTime.Now.AddDays(-1);

            if (DateSummary[0].Value > Curr)
            {
                IsEditableItems = true;
            }
            else
            {
                IsEditableItems = false;
            }

            //CollectEmployeeSummary();

            HoursSpent = SubProjects.Sum(x => x.HoursUsed);
            HoursLeft = SubProjects.Sum(x => x.HoursLeft);
            //CopiedSchedulingItems = SchedulingViews;
            //CanDeleteSchedule = true;

            //foreach (RolePerSubProjectModel itemold in SchedulingItems)
            //{
            //    itemold.Entries.Clear();
            //}

            //SchedulingItems.Clear();
            //CopiedSchedulingItems.Clear();

            //DateTime datestart = DateSummary.First().Value;
            //DateTime dateend = DateSummary.Last().Value;
            ////update employee Id
            //List<SchedulingDataDbModel> schedulingdata = SQLAccess.LoadSchedulingData(DateSummary.First().Value, SelectedSubproject.Id);
            //int i = 0;
            //foreach (SchedulingDataDbModel item in schedulingdata)
            //{
            //    RolePerSubProjectModel role = SelectedSubproject.RolesPerSub.Where(x => x.Id == item?.RoleId).FirstOrDefault();
            //    EmployeeModel em = null;

            //    if (role == null || item.EmployeeId != role.Employee.Id)
            //    {
            //        role = new RolePerSubProjectModel();
            //        role.Subproject = SelectedSubproject;
            //        EmployeeDbModel emdb = SQLAccess.LoadEmployeeById(item.EmployeeId);
            //        em = new EmployeeModel(emdb);
            //    }
            //    else
            //    {
            //        em = role.Employee;
            //    }

            //    role.Subproject = SelectedSubproject;
            //    role.EmployeeWrapper = em;

            //    role.Entries.Add(new SDEntryModel() { Date = DateSummary[0].Value, TimeEntry = item.Hours1 });
            //    role.Entries.Add(new SDEntryModel() { Date = DateSummary[1].Value, TimeEntry = item.Hours2 });
            //    role.Entries.Add(new SDEntryModel() { Date = DateSummary[2].Value, TimeEntry = item.Hours3 });
            //    role.Entries.Add(new SDEntryModel() { Date = DateSummary[3].Value, TimeEntry = item.Hours4 });
            //    role.Entries.Add(new SDEntryModel() { Date = DateSummary[4].Value, TimeEntry = item.Hours5 });
            //    role.Entries.Add(new SDEntryModel() { Date = DateSummary[5].Value, TimeEntry = item.Hours6 });
            //    role.Entries.Add(new SDEntryModel() { Date = DateSummary[6].Value, TimeEntry = item.Hours7 });
            //    role.Entries.Add(new SDEntryModel() { Date = DateSummary[7].Value, TimeEntry = item.Hours8 });

            //    role.Total = item.Hours1 + item.Hours2 + item.Hours3 + item.Hours4 + item.Hours5 + item.Hours6 + item.Hours7 + item.Hours8;
            //    SchedulingItems.Add(role);

            //    CopiedSchedulingItems.Add((RolePerSubProjectModel)role.Clone());
            //    i++;
            //}

            ////CollectEmployeeSummary();
            //UpdateLists();

            //DateTime lastSaturday = DateTime.Now.AddDays(-1);
            //while (lastSaturday.DayOfWeek != DayOfWeek.Saturday)
            //    lastSaturday = lastSaturday.AddDays(-1);

            //DateTime Curr = DateTime.Now.AddDays(-1);

            //if (DateSummary[0].Value > Curr)
            //{
            //    IsEditableItems = true;
            //}
            //else
            //{
            //    IsEditableItems = false;
            //}
            //CollectEmployeeSummary();
        }


        //private void LoadProjectManagers()
        //{
        //    List<EmployeeDbModel> PMs = SQLAccess.LoadProjectManagers();

        //    ObservableCollection<EmployeeModel> members = new ObservableCollection<EmployeeModel>();

        //    foreach (EmployeeDbModel edbm in PMs)
        //    {
        //        members.Add(new EmployeeModel(edbm));
        //    }

        //    ProjectManagers = members;
        //}

        private void LoadProjects()
        {
            List<ProjectDbModel> dbprojects = SQLAccess.LoadActiveNonHoldProjects();

            ObservableCollection<ProjectLowResModel> members = new ObservableCollection<ProjectLowResModel>();

            ProjectLowResModel[] ProjectArray = new ProjectLowResModel[dbprojects.Count];

            //Do not include the last layer
            Parallel.For(0, dbprojects.Count, i =>
            {
                ProjectDbModel pdb = dbprojects[i];

                //if (pdb.ProjectName != "VACATION" && pdb.ProjectName != "OFFICE" && pdb.ProjectName != "HOLIDAY" && pdb.ProjectName != "SICK")
                //{
                ProjectLowResModel pm = new ProjectLowResModel(pdb);
                pm.PercentComplete = pdb.PercentComplete;
                pm.ManagerId = pdb.ManagerId;

                if (pdb.DueDate != null)
                {
                    pm.DueDate = DateTime.ParseExact(pdb.DueDate.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                }

                //EmployeeModel em = ProjectManagers.Where(x => x.Id == pdb.ManagerId).FirstOrDefault();

                //pm.ProjectManager = em;


                ProjectArray[i] = pm;
                //}
            }
            );

            ProjectArray = ProjectArray.Where(c => c != null).ToArray();
            List<ProjectLowResModel> orderedlist = ProjectArray.Where(x => x != null).OrderByDescending(x => x.ProjectNumber).ToList();

            BaseProjectList = new ObservableCollection<ProjectLowResModel>(orderedlist.ToList());
            //SelectedProject = null;
            //SelectedProject = ProjectList.First();
            //SelectedSubproject = SelectedProject.SubProjects[0];
        }
    }
}
