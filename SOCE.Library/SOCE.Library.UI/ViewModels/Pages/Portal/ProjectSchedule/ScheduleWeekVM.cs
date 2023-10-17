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

namespace SOCE.Library.UI.ViewModels
{
    public class ScheduleWeekVM : BaseVM
    {
        public ICommand ClearSelectedProjectCommand { get; set; }

        public ICommand SelectedItemChangedCommand { get; set; }
        public ICommand OpenEmployeeSummary { get; set; }
        public ICommand SaveSchedlingHoursCommand { get; set; }
        public ICommand AddRoleCommand { get; set; }
        public ICommand DeleteRole { get; set; }
        public ICommand PreviousCommand { get; set; }
        public ICommand NextCommand { get; set; }
        public ICommand CurrentCommand { get; set; }
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

        private ObservableCollection<EmployeeModel> _employeeSummary = new ObservableCollection<EmployeeModel>();
        public ObservableCollection<EmployeeModel> EmployeeSummary
        {
            get { return _employeeSummary; }
            set
            {
                _employeeSummary = value;
                RaisePropertyChanged(nameof(EmployeeSummary));
            }
        }

        private ObservableCollection<ProjectModel> _projectList;
        public ObservableCollection<ProjectModel> ProjectList
        {
            get { return _projectList; }
            set
            {
                _projectList = value;
                RaisePropertyChanged(nameof(ProjectList));
            }
        }

        public ObservableCollection<ProjectModel> BaseProjectList { get; set; }

        private ProjectModel _selectedProject;
        public ProjectModel SelectedProject
        {
            get { return _selectedProject; }
            set
            {
                _selectedProject = value;

                //CollectSubProjects();
                if (SchedulingItems.Count > 0 && IsEditableItems)
                {
                    SaveData();
                }

                if (_selectedProject != null)
                {
                    _selectedProject.FormatData(true);

                    List<SubProjectModel> SubProjectstemp = _selectedProject.SubProjects.Renumber(true).ToList();
                    SubProjectstemp.RemoveAt(SubProjectstemp.Count - 1);
                    List<SubProjectModel> SubProjectsNew = SubProjectstemp.Where(x => x.IsActive).ToList();
                    int idofscheduleactive = 0;
                    bool stuffhappened = false;

                    foreach (SubProjectModel sub in SubProjectsNew)
                    {
                        if (sub.IsScheduleActive)
                        {
                            stuffhappened = true;
                            idofscheduleactive = SubProjectsNew.IndexOf(sub);
                            //members = new ObservableCollection<SubProjectModel>(newlist);
                            break;
                        }
                    }


                    if (stuffhappened)
                    {
                        List<SubProjectModel> newsubs = SubProjectsNew.ToList();
                        newsubs.MoveItemAtIndexToFront(idofscheduleactive);
                        SubProjects = new ObservableCollection<SubProjectModel>(newsubs);
                    }
                    else
                    {
                        SubProjects = new ObservableCollection<SubProjectModel>(SubProjectsNew);
                    }

                    //SubProjects = new ObservableCollection<SubProjectModel>(SubProjects.Where(x => x.IsActive).ToList());


                    if (SubProjects.Count > 0)
                    {
                        SelectedSubproject = SubProjects[0];
                    }
                    IsThisEditable = false;
                    ProjectList = BaseProjectList;
                }
                else
                {
                    IsThisEditable = true;
                    SelectedSubproject = null;
                    SchedulingItems.Clear();
                    CopiedSchedulingItems.Clear();
                }
                RaisePropertyChanged(nameof(SelectedProject));
            }
        }

        private ObservableCollection<SubProjectModel> _subprojects = new ObservableCollection<SubProjectModel>();
        public ObservableCollection<SubProjectModel> SubProjects
        {
            get { return _subprojects; }
            set
            {
                _subprojects = value;
                RaisePropertyChanged(nameof(SubProjects));
            }
        }

        private ObservableCollection<EmployeeModel> _projectManagers = new ObservableCollection<EmployeeModel>();
        public ObservableCollection<EmployeeModel> ProjectManagers
        {
            get { return _projectManagers; }
            set
            {
                _projectManagers = value;
                RaisePropertyChanged(nameof(ProjectManagers));
            }
        }

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

        private SubProjectModel _selectedSubproject;
        public SubProjectModel SelectedSubproject
        {
            get
            {
                return _selectedSubproject;
            }
            set
            {
                _selectedSubproject = value;

                if (_selectedSubproject != null)
                {
                    LoadSchedulingData();
                }

                RaisePropertyChanged(nameof(SelectedSubproject));
            }
        }

        private ObservableCollection<RolePerSubProjectModel> CopiedSchedulingItems = new ObservableCollection<RolePerSubProjectModel>();

        private ObservableCollection<RolePerSubProjectModel> _schedulingItems = new ObservableCollection<RolePerSubProjectModel>();
        public ObservableCollection<RolePerSubProjectModel> SchedulingItems
        {
            get { return _schedulingItems; }
            set
            {
                _schedulingItems = value;
                RaisePropertyChanged(nameof(SchedulingItems));
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

        private bool _searchFilter = false;
        public bool SearchFilter
        {
            get { return _searchFilter; }
            set
            {
                _searchFilter = value;

                //if (_searchFilter)
                //{
                foreach (ProjectModel pm in ProjectList)
                {
                    pm.SearchText = _searchFilter ? pm.ProjectName : pm.ProjectNumber.ToString();
                }
                //}
                RaisePropertyChanged(nameof(SearchFilter));
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
            this.AddRoleCommand = new RelayCommand(this.AddRole);
            this.DeleteRole = new RelayCommand<RolePerSubProjectModel>(this.DeleteRoleIfPossible);
            this.SaveSchedlingHoursCommand = new RelayCommand(this.SaveData);
            this.PreviousCommand = new RelayCommand(PreviousTimesheet);
            this.NextCommand = new RelayCommand(NextTimesheet);
            this.CurrentCommand = new RelayCommand(CurrentTimesheet);
            this.OpenEmployeeSummary = new RelayCommand(OpenRightDrawer);
            SchedulingItems.CollectionChanged += RowDataChanged;
            List<EmployeeDbModel> employeesDb = SQLAccess.LoadEmployees();

            List<EmployeeModel> totalemployees = new List<EmployeeModel>();

            foreach (EmployeeDbModel employeenew in employeesDb)
            {
                totalemployees.Add(new EmployeeModel(employeenew));
            }

            ObservableCollection<EmployeeModel> ordered = new ObservableCollection<EmployeeModel>(totalemployees.OrderBy(x => x.LastName).ToList());
            //OverallFee = overallfee;
            Employees = ordered;
            UpdateDateSummary(DateTime.Today);
            LoadProjectManagers();
            LoadProjects();
            ProjectList = BaseProjectList;
            CollectEmployeeSummary();
            //CollectEmployeeSummary();
            //UpdateLists();
            //LoadSchedulingData();
        }

        public void ClearSelected()
        {
            SelectedProject = null;
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


        //        SDEntryModel hours1entry = new SDEntryModel() { Date = DateSummary[0].Value, TimeEntry = hours1, CellColor = brush1 };
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

        private void UpdateLists()
        {
            List<EmployeeModel> employeesused = SchedulingItems.Select(x => x?.Employee).ToList();

            List<EmployeeModel> result = Employees.Where(p => !employeesused.Any(l => p?.Id == l?.Id)).ToList();

            foreach (RolePerSubProjectModel role in SchedulingItems)
            {
                List<EmployeeModel> employeestoselect = new List<EmployeeModel>();
                if (role.Employee != null && role.Employee.Id != 0)
                {
                    employeestoselect.Add(role.Employee);
                }
                employeestoselect.AddRange(result);
                role.EmployeeList = new ObservableCollection<EmployeeModel>(employeestoselect);

            }
        }

        private void SelectionCombo(string project)
        {
            if (SelectedProject == null)
            {
                if (!String.IsNullOrEmpty(project))
                {
                    ComboOpen = true;

                    ProjectList = new ObservableCollection<ProjectModel>(BaseProjectList.Where(x => (x.ProjectName.ToUpper() + x.ProjectNumber.ToString()).Contains(project.ToUpper())));
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
            DateTime starttime = DateTime.Now;
            MessageVisible = true;
            try
            {


                //deleting
                foreach (RolePerSubProjectModel ctrm in CopiedSchedulingItems)
                {
                    if (ctrm.Employee != null && ctrm.Employee.Id != 0)
                    {
                        int index = SchedulingItems.ToList().FindIndex(x => x?.Employee.Id == ctrm.Employee.Id);

                        //it exists
                        if (index < 0)
                        {

                            SchedulingDataDbModel trdbm = SQLAccess.LoadSingleSchedulingData(ctrm.Employee.Id, SelectedSubproject.Id, DateSummary[0].Value);

                            if (trdbm != null)
                            {
                                SQLAccess.DeleteSchedulingData(trdbm.Id);

                                //delete role if 0 hours?
                            }

                        }
                    }
                }

                //}


                foreach (RolePerSubProjectModel trm in SchedulingItems)
                {
                    double hour1 = trm.Entries[0].TimeEntry;
                    double hour2 = trm.Entries[1].TimeEntry;
                    double hour3 = trm.Entries[2].TimeEntry;
                    double hour4 = trm.Entries[3].TimeEntry;
                    double hour5 = trm.Entries[4].TimeEntry;
                    double hour6 = trm.Entries[5].TimeEntry;
                    double hour7 = trm.Entries[6].TimeEntry;
                    double hour8 = trm.Entries[7].TimeEntry;


                    if (trm.Employee != null && trm.Employee.Id != 0 || (hour1 != 0 && hour2 != 0 && hour3 != 0 && hour4 != 0 && hour5 != 0 && hour6 != 0 && hour7 != 0 && hour8 != 0))
                    {
                        if (trm.Id == 0)
                        {
                            RolePerSubProjectDbModel rpp = new RolePerSubProjectDbModel()
                            {
                                EmployeeId = trm.Employee.Id,
                                SubProjectId = trm.Subproject.Id,
                                Rate = trm.Rate,
                                Role = Convert.ToInt32(trm.Role),
                                BudgetHours = 0
                            };
                            //if (trm.Id == 0)
                            //{
                            int val = SQLAccess.AddRolesPerSubProject(rpp);
                            trm.Id = val;
                            //}
                        }

                        SchedulingDataDbModel dbmodel = new SchedulingDataDbModel()
                        {
                            EmployeeId = trm.Employee.Id,
                            PhaseId = SelectedSubproject.Id,
                            RoleId = trm.Id,
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

        private void PreviousTimesheet()
        {
            UpdateDateSummary(DateSummary.First().Value.AddDays(-7));
            if (SelectedSubproject != null)
            {
                LoadSchedulingData();
            }
        }

        /// <summary>
        /// Button Press
        /// </summary>
        private void NextTimesheet()
        {
            UpdateDateSummary(DateSummary.First().Value.AddDays(7));
            if (SelectedSubproject != null)
            {
                LoadSchedulingData();
            }
        }

        /// <summary>
        /// Button Press
        /// </summary>
        private void CurrentTimesheet()
        {
            UpdateDateSummary(DateTime.Now);
            if (SelectedSubproject != null)
            {
                LoadSchedulingData();
            }
        }

        private void OpenRightDrawer()
        {

            BaseAI CurrentPage = IoCPortal.Application as BaseAI;
            PortalAI portAI = (PortalAI)CurrentPage;

            EmployeeSummaryScheduleView cv = new EmployeeSummaryScheduleView();
            portAI.RightViewToShow = cv;

            ObservableCollection<EmployeeModel> employeesforref = new ObservableCollection<EmployeeModel>();

            foreach(EmployeeModel em in Employees)
            {
                employeesforref.Add(new EmployeeModel() { Id = em.Id, FullName = em.FullName, ScheduledTotalHours = em.ScheduledTotalHours });
            }

            EmployeeScheduleSummaryVM cvm = new EmployeeScheduleSummaryVM(employeesforref, DateSummary);
            portAI.RightViewToShow.DataContext = cvm;
            portAI.RightDrawerOpen = true;
        }

        private void RowDataChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged added in e?.NewItems)
                {
                    RolePerSubProjectModel trm = (RolePerSubProjectModel)added;
                    //trm.PropertyChanged += ListPropertyChanged;
                    foreach (SDEntryModel trentry in trm.Entries)
                    {
                        trentry.PropertyChanged += ItemModificationOnPropertyChanged;
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged added in e?.OldItems)
                {
                    RolePerSubProjectModel trm = (RolePerSubProjectModel)added;
                    //trm.PropertyChanged -= ListPropertyChanged;
                    foreach (SDEntryModel trentry in trm.Entries)
                    {
                        trentry.PropertyChanged -= ItemModificationOnPropertyChanged;
                    }
                }
            }


        }

        private void ItemModificationOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            foreach (RolePerSubProjectModel trm in SchedulingItems)
            {
                trm.Total = trm.Entries.Sum(x => x.TimeEntry);
            }
            //UpdateLists();
            //CollectEmployeeSummary();
        }

        //private void ListPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        //{
        //    RolePerSubProjectModel trm = (RolePerSubProjectModel)sender;
        //    trm.PropertyChanged -= ListPropertyChanged;
        //    UpdateLists();
        //    trm.PropertyChanged += ListPropertyChanged;
        //}

        private void AddRole()
        {
            if (SelectedSubproject != null)
            {
                RolePerSubProjectModel rpspm = new RolePerSubProjectModel(SelectedSubproject, SelectedSubproject.Fee);
                rpspm.EditRoleFieldState = false;
                rpspm.SpentHours = 0;
                rpspm.Subproject = SelectedSubproject;

                //SelectedSubproject.RolesPerSub.Add(rpspm);

                foreach (DateWrapper date in DateSummary)
                {
                    if (!rpspm.Entries.Any(x => x.Date == date.Value))
                    {
                        //add
                        rpspm.Entries.Add(new SDEntryModel() { Date = date.Value, TimeEntry = 0 });
                    }
                }

                SchedulingItems.Add(rpspm);
                UpdateLists();
            }
        }

        private void DeleteRoleIfPossible(RolePerSubProjectModel rpsm)
        {
            //if (rpsm.Id != 0)
            //{
            //    LeftViewToShow = new YesNoView();
            //    YesNoVM aysvm = new YesNoVM(rpsm, this);
            //    LeftViewToShow.DataContext = aysvm;
            //    ItemToDelete = rpsm;
            //    LeftDrawerOpen = true;
            //}
            //else
            //{
            SchedulingItems.Remove(rpsm);
            UpdateLists();
            //}

            //update roles
        }

        private void CollectEmployeeSummary()
        {
            foreach (EmployeeModel em in Employees)
            {
                em.Entries.Clear();
                List<SchedulingDataDbModel> data = SQLAccess.LoadSchedulingDataByEmployee(DateSummary[0].Value, em.Id);
                double hours1 = data.Sum(x => x.Hours1);
                double hours2 = data.Sum(x => x.Hours2);

                Brush solidgreen = Brushes.Blue;
                Brush solidred = Brushes.Red;

                Brush brush1 = solidred.Blend(solidgreen, 0.9 * (hours1 / 40));
                Brush brush2 = solidred.Blend(solidgreen, 0.9 * (hours2 / 40));

                SDEntryModel hours1entry = new SDEntryModel() { Date = DateSummary[0].Value, TimeEntry = hours1, CellColor = brush1 };
                SDEntryModel hours2entry = new SDEntryModel() { Date = DateSummary[1].Value, TimeEntry = hours2, CellColor = brush2 };

                em.Entries.Add(hours1entry);
                em.Entries.Add(hours2entry);
            }
        }

        private void UpdateDateSummary(DateTime currdate)
        {
            EmployeeDateSummary.Clear();
            DateSummary.Clear();
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysUntilMonday = ((int)DayOfWeek.Monday - (int)currdate.DayOfWeek + 7) % 7;
            DateTime nextMonday = currdate.AddDays(daysUntilMonday);
            DateSummary.Add(new DateWrapper(nextMonday));
            EmployeeDateSummary.Add(new DateWrapper(nextMonday));
            DateSummary.Add(new DateWrapper(nextMonday.AddDays(7)));
            EmployeeDateSummary.Add(new DateWrapper(nextMonday.AddDays(7)));
            DateSummary.Add(new DateWrapper(nextMonday.AddDays(14)));
            DateSummary.Add(new DateWrapper(nextMonday.AddDays(21)));
            DateSummary.Add(new DateWrapper(nextMonday.AddDays(28)));
            DateSummary.Add(new DateWrapper(nextMonday.AddDays(35)));
            DateSummary.Add(new DateWrapper(nextMonday.AddDays(42)));
            DateSummary.Add(new DateWrapper(nextMonday.AddDays(49)));
        }

        private void LoadSchedulingData()
        {
            CanDeleteSchedule = true;

            foreach (RolePerSubProjectModel itemold in SchedulingItems)
            {
                itemold.Entries.Clear();
            }

            SchedulingItems.Clear();
            CopiedSchedulingItems.Clear();

            DateTime datestart = DateSummary.First().Value;
            DateTime dateend = DateSummary.Last().Value;
            //update employee Id
            List<SchedulingDataDbModel> schedulingdata = SQLAccess.LoadSchedulingData(DateSummary.First().Value, SelectedSubproject.Id);
            int i = 0;
            foreach (SchedulingDataDbModel item in schedulingdata)
            {
                RolePerSubProjectModel role = SelectedSubproject.RolesPerSub.Where(x => x.Id == item?.RoleId).FirstOrDefault();
                EmployeeModel em = null;

                if (role == null || item.EmployeeId != role.Employee.Id)
                {
                    role = new RolePerSubProjectModel();
                    role.Subproject = SelectedSubproject;
                    EmployeeDbModel emdb = SQLAccess.LoadEmployeeById(item.EmployeeId);
                    em = new EmployeeModel(emdb);
                }
                else
                {
                    em = role.Employee;
                }

                role.Subproject = SelectedSubproject;
                role.EmployeeWrapper = em;

                role.Entries.Add(new SDEntryModel() { Date = DateSummary[0].Value, TimeEntry = item.Hours1 });
                role.Entries.Add(new SDEntryModel() { Date = DateSummary[1].Value, TimeEntry = item.Hours2 });
                role.Entries.Add(new SDEntryModel() { Date = DateSummary[2].Value, TimeEntry = item.Hours3 });
                role.Entries.Add(new SDEntryModel() { Date = DateSummary[3].Value, TimeEntry = item.Hours4 });
                role.Entries.Add(new SDEntryModel() { Date = DateSummary[4].Value, TimeEntry = item.Hours5 });
                role.Entries.Add(new SDEntryModel() { Date = DateSummary[5].Value, TimeEntry = item.Hours6 });
                role.Entries.Add(new SDEntryModel() { Date = DateSummary[6].Value, TimeEntry = item.Hours7 });
                role.Entries.Add(new SDEntryModel() { Date = DateSummary[7].Value, TimeEntry = item.Hours8 });

                role.Total = item.Hours1 + item.Hours2 + item.Hours3 + item.Hours4 + item.Hours5 + item.Hours6 + item.Hours7 + item.Hours8;
                SchedulingItems.Add(role);

                CopiedSchedulingItems.Add((RolePerSubProjectModel)role.Clone());
                i++;
            }

            //CollectEmployeeSummary();
            UpdateLists();

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
            CollectEmployeeSummary();
        }


        private void LoadProjectManagers()
        {
            List<EmployeeDbModel> PMs = SQLAccess.LoadProjectManagers();

            ObservableCollection<EmployeeModel> members = new ObservableCollection<EmployeeModel>();

            foreach (EmployeeDbModel edbm in PMs)
            {
                members.Add(new EmployeeModel(edbm));
            }

            ProjectManagers = members;
        }

        private void LoadProjects()
        {
            List<ProjectDbModel> dbprojects = SQLAccess.LoadActiveNonHoldProjects();

            ObservableCollection<ProjectModel> members = new ObservableCollection<ProjectModel>();

            ProjectModel[] ProjectArray = new ProjectModel[dbprojects.Count];

            //Do not include the last layer
            Parallel.For(0, dbprojects.Count, i =>
            {
                ProjectDbModel pdb = dbprojects[i];

                //if (pdb.ProjectName != "VACATION" && pdb.ProjectName != "OFFICE" && pdb.ProjectName != "HOLIDAY" && pdb.ProjectName != "SICK")
                //{
                ProjectModel pm = new ProjectModel(pdb);
                EmployeeModel em = ProjectManagers.Where(x => x.Id == pdb.ManagerId).FirstOrDefault();

                pm.ProjectManager = em;


                ProjectArray[i] = pm;
                //}
            }
            );

            ProjectArray = ProjectArray.Where(c => c != null).ToArray();
            List<ProjectModel> orderedlist = ProjectArray.Where(x => x != null).OrderByDescending(x => x.ProjectNumber).ToList();

            BaseProjectList = new ObservableCollection<ProjectModel>(orderedlist.ToList());
            //SelectedProject = null;
            //SelectedProject = ProjectList.First();
            //SelectedSubproject = SelectedProject.SubProjects[0];
        }
    }
}
