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
    public class ScheduleWeekControlVM : BaseVM
    {
        public ICommand DeletePhaseCommand { get; set; }
        public ICommand AddRoleCommand { get; set; }
        public ICommand DeleteRole { get; set; }

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

        private SubProjectLowResModel _selectedSubproject;
        public SubProjectLowResModel SelectedSubproject
        {
            get
            {
                return _selectedSubproject;
            }
            set
            {
                _selectedSubproject = value;

                if (_selectedSubproject != null && firstopen)
                {
                    firstopen = false;
                    BaseVM.UpdatePhaseLists();
                }

                foreach (ScheduleWeekModel roles in SchedulingItems)
                {
                    roles.PhaseId = _selectedSubproject.Id;
                }

                firstopen = true;
                RaisePropertyChanged(nameof(SelectedSubproject));
            }
        }

        bool firstopen = false;

        //private ObservableCollection<RolePerSubProjectModel> CopiedSchedulingItems = new ObservableCollection<RolePerSubProjectModel>();

        private ObservableCollection<ScheduleWeekModel> _schedulingItems = new ObservableCollection<ScheduleWeekModel>();
        public ObservableCollection<ScheduleWeekModel> SchedulingItems
        {
            get { return _schedulingItems; }
            set
            {
                _schedulingItems = value;
                RaisePropertyChanged(nameof(SchedulingItems));
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

        public ScheduleWeekVM BaseVM;

        public ScheduleWeekControlVM(List<ScheduleWeekModel> InitialRoleLoadIn, ObservableCollection<DateWrapper> dates, SubProjectLowResModel sub, ObservableCollection<SubProjectLowResModel> subs, ScheduleWeekVM scheduleweek)
        {
            Constructor();
            SubProjects = subs;
            SelectedSubproject = sub;
            HoursLeft = SelectedSubproject.HoursLeft;
            HoursSpent = SelectedSubproject.HoursUsed;
            //List<ScheduleWeekModel> swm = new List<ScheduleWeekModel>();
            //foreach (ScheduleWeekModel role in InitialRoleLoadIn)
            //{
            //    role.EmployeeList = Employees;
            //    swm.Add(role);
            //}
            SchedulingItems = new ObservableCollection<ScheduleWeekModel>(InitialRoleLoadIn);

            foreach (ScheduleWeekModel swm in SchedulingItems)
            {
                foreach (SDEntryModel entry in swm.Entries)
                {
                    entry.basevm = this;
                }
            }
            //foreach (RolePerSubProjectModel role in InitialRoleLoadIn)
            //{
            //    CopiedSchedulingItems.Add((RolePerSubProjectModel)role.Clone());
            //}
            //UpdateLists();
            DateSummary = dates;
            BaseVM = scheduleweek;
            firstopen = true;
            //UpdateDateSummary(DateTime.Today);
            //CollectEmployeeSummary();
        }

        public ScheduleWeekControlVM(ObservableCollection<DateWrapper> dates, SubProjectLowResModel sub, ObservableCollection<SubProjectLowResModel> subs, ScheduleWeekVM scheduleweek)
        {
            Constructor();
            SubProjects = subs;
            SelectedSubproject = sub;
            HoursLeft = SelectedSubproject.HoursLeft;
            HoursSpent = SelectedSubproject.HoursUsed;
            DateSummary = dates;
            BaseVM = scheduleweek;
            //UpdateDateSummary(DateTime.Today);
            //CollectEmployeeSummary();
            firstopen = true;
        }

        public void Constructor()
        {
            this.AddRoleCommand = new RelayCommand(this.AddRole);
            this.DeletePhaseCommand = new RelayCommand(this.DeletePhase);
            this.DeleteRole = new RelayCommand<ScheduleWeekModel>(this.DeleteRoleIfPossible);
            //SchedulingItems.CollectionChanged += RowDataChanged;
            List<EmployeeDbModel> employeesDb = SQLAccess.LoadEmployees();
            List<EmployeeScheduleModel> totalemployees = new List<EmployeeScheduleModel>();
            foreach (EmployeeDbModel employeenew in employeesDb)
            {
                totalemployees.Add(new EmployeeScheduleModel(employeenew));
            }

            ObservableCollection<EmployeeScheduleModel> ordered = new ObservableCollection<EmployeeScheduleModel>(totalemployees.OrderBy(x => x.FullName).ToList());
            //OverallFee = overallfee;
            Employees = ordered;

        }

        public void UpdateTotals()
        {
            foreach (ScheduleWeekModel swm in SchedulingItems)
            {
                double total = 0;
                for (int i = 0; i< swm.Entries.Count(); i++)
                {
                    total += swm.Entries[i].TimeEntry;
                }
                swm.Total = total;
            }
        }

        private async void DeletePhase()
        {
            int id = -1;

            if (SchedulingItems.Count > 0)
            {
                YesNoView view = new YesNoView();
                YesNoVM aysvm = new YesNoVM();

                aysvm.Message = $"Are you sure you want to delete?";
                view.DataContext = aysvm;

                //show the dialog
                var Result = await DialogHost.Show(view, "RootDialog");

                YesNoVM vm = view.DataContext as YesNoVM;
                bool resultvm = vm.Result;
                if (!resultvm)
                {
                    return;
                }
            }

            for (int i = 0; i < BaseVM.SchedulingViews.Count; i++)
            {
                ScheduleWeekControlVM swcvm = (ScheduleWeekControlVM)BaseVM.SchedulingViews[i].DataContext;
                if (swcvm.SelectedSubproject.Id == SelectedSubproject.Id)
                {
                    id = i;
                }
            }

            if (id != -1)
            {
                BaseVM.SchedulingViews.RemoveAt(id);
            }

            BaseVM.UpdatePhaseLists();
        }

        //public void UpdateLists()
        //{
        //    List<EmployeeModel> employeesused = SchedulingItems.Select(x => x?.Employee).ToList();

        //    List<EmployeeModel> result = Employees.Where(p => !employeesused.Any(l => p?.Id == l?.Id)).ToList();

        //    foreach (RolePerSubProjectModel role in SchedulingItems)
        //    {
        //        List<EmployeeModel> employeestoselect = new List<EmployeeModel>();
        //        if (role.Employee != null && role.Employee.Id != 0)
        //        {
        //            employeestoselect.Add(role.Employee);
        //        }
        //        employeestoselect.AddRange(result);
        //        role.EmployeeList = new ObservableCollection<EmployeeModel>(employeestoselect);

        //    }
        //}

        //private void RowDataChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    if (e.NewItems != null)
        //    {
        //        foreach (INotifyPropertyChanged added in e?.NewItems)
        //        {
        //            RolePerSubProjectModel trm = (RolePerSubProjectModel)added;
        //            //trm.PropertyChanged += ListPropertyChanged;
        //            foreach (SDEntryModel trentry in trm.Entries)
        //            {
        //                trentry.PropertyChanged += ItemModificationOnPropertyChanged;
        //            }
        //        }
        //    }

        //    if (e.OldItems != null)
        //    {
        //        foreach (INotifyPropertyChanged added in e?.OldItems)
        //        {
        //            RolePerSubProjectModel trm = (RolePerSubProjectModel)added;
        //            foreach (SDEntryModel trentry in trm.Entries)
        //            {
        //                trentry.PropertyChanged -= ItemModificationOnPropertyChanged;
        //            }
        //        }
        //    }
        //}

        //private void ItemModificationOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        //{
        //    foreach (ScheduleWeekModel trm in SchedulingItems)
        //    {
        //        trm.Total = trm.Entries.Sum(x => x.TimeEntry);
        //    }
        //}

        private void AddRole()
        {
            if (SelectedSubproject != null)
            {
                ScheduleWeekModel rpspm = new ScheduleWeekModel();
                rpspm.PhaseId = SelectedSubproject.Id;
                rpspm.ProjectName = BaseVM.SelectedProject.ProjectName;
                rpspm.PhaseName = SelectedSubproject.PointNumber;
                rpspm.ProjectNumber = BaseVM.SelectedProject.ProjectNumber;
                rpspm.ManagerId = BaseVM.SelectedProject.ManagerId;
                rpspm.EmployeeList = Employees;
               
                foreach (DateWrapper date in DateSummary)
                {
                    if (!rpspm.Entries.Any(x => x.Date == date.Value))
                    {
                        Brush cellcolor = Brushes.Transparent;
                        
                        if (BaseVM.DueDate != null)
                        {
                            double diff = ((DateTime)BaseVM.DueDate - date.Value).TotalDays;

                            if (diff >= 0 && diff < 7)
                            {
                                cellcolor = (SolidColorBrush)new BrushConverter().ConvertFrom("#f9aeae");
                            }
                        }

                        rpspm.Entries.Add(new SDEntryModel() { Date = date.Value, TimeEntry = 0, CellColor = cellcolor, basevm = this });
                    }
                }

                SchedulingItems.Add(rpspm);
                //UpdateLists();
            }
        }

        private void DeleteRoleIfPossible(ScheduleWeekModel rpsm)
        {
            SchedulingItems.Remove(rpsm);
            //UpdateLists();
        }

        //public void SaveData()
        //{
        //    //deleting
        //    foreach (RolePerSubProjectModel ctrm in CopiedSchedulingItems)
        //    {
        //        if (ctrm.Employee != null && ctrm.Employee.Id != 0)
        //        {


        //            int index = SchedulingItems.ToList().FindIndex(x => x?.Employee.Id == ctrm.Employee.Id);

        //            //it exists
        //            if (index < 0)
        //            {

        //                SchedulingDataDbModel trdbm = SQLAccess.LoadSingleSchedulingData(ctrm.Employee.Id, SelectedSubproject.Id, DateSummary[0].Value);

        //                if (trdbm != null)
        //                {
        //                    SQLAccess.DeleteSchedulingData(trdbm.Id);

        //                    //delete role if 0 hours?
        //                }

        //            }
        //        }
        //    }

        //    foreach (RolePerSubProjectModel trm in SchedulingItems)
        //    {
        //        double hour1 = trm.Entries[0].TimeEntry;
        //        double hour2 = trm.Entries[1].TimeEntry;
        //        double hour3 = trm.Entries[2].TimeEntry;
        //        double hour4 = trm.Entries[3].TimeEntry;
        //        double hour5 = trm.Entries[4].TimeEntry;
        //        double hour6 = trm.Entries[5].TimeEntry;
        //        double hour7 = trm.Entries[6].TimeEntry;
        //        double hour8 = trm.Entries[7].TimeEntry;


        //        if (trm.Employee != null && trm.Employee.Id != 0 || (hour1 != 0 && hour2 != 0 && hour3 != 0 && hour4 != 0 && hour5 != 0 && hour6 != 0 && hour7 != 0 && hour8 != 0))
        //        {
        //            if (trm.Id == 0)
        //            {
        //                RolePerSubProjectDbModel rpp = new RolePerSubProjectDbModel()
        //                {
        //                    EmployeeId = trm.Employee.Id,
        //                    SubProjectId = trm.Subproject.Id,
        //                    Rate = trm.Rate,
        //                    Role = Convert.ToInt32(trm.Role),
        //                    BudgetHours = 0
        //                };
        //                //if (trm.Id == 0)
        //                //{
        //                int val = SQLAccess.AddRolesPerSubProject(rpp);
        //                trm.Id = val;
        //                //}
        //            }

        //            SchedulingDataDbModel dbmodel = new SchedulingDataDbModel()
        //            {
        //                EmployeeId = trm.Employee.Id,
        //                PhaseId = SelectedSubproject.Id,
        //                RoleId = trm.Id,
        //                Date = (int)long.Parse(DateSummary[0].Value.ToString("yyyyMMdd")),
        //                Hours1 = hour1,
        //                Hours2 = hour2,
        //                Hours3 = hour3,
        //                Hours4 = hour4,
        //                Hours5 = hour5,
        //                Hours6 = hour6,
        //                Hours7 = hour7,
        //                Hours8 = hour8,
        //            };

        //            SQLAccess.AddSchedulingData(dbmodel);
        //        }
        //    }
        //}

        //private void CollectEmployeeSummary()
        //{
        //    foreach (EmployeeModel em in Employees)
        //    {
        //        em.Entries.Clear();
        //        List<SchedulingDataDbModel> data = SQLAccess.LoadSchedulingDataByEmployee(DateSummary[0].Value, em.Id);
        //        double hours1 = data.Sum(x => x.Hours1);
        //        double hours2 = data.Sum(x => x.Hours2);

        //        Brush solidgreen = Brushes.Blue;
        //        Brush solidred = Brushes.Red;

        //        double brushhours1 = Math.Min(hours1, 40);
        //        double brushhours2 = Math.Min(hours2, 40);

        //        Brush brush1 = solidred.Blend(solidgreen, 0.9 * (brushhours1 / 40));
        //        Brush brush2 = solidred.Blend(solidgreen, 0.9 * (brushhours2 / 40));

        //        SDEntryModel hours1entry = new SDEntryModel() { Date = DateSummary[0].Value, TimeEntry = hours1, CellColor = brush1 };
        //        SDEntryModel hours2entry = new SDEntryModel() { Date = DateSummary[1].Value, TimeEntry = hours2, CellColor = brush2 };

        //        em.Entries.Add(hours1entry);
        //        em.Entries.Add(hours2entry);
        //    }
        //}

        //private void UpdateDateSummary(DateTime currdate)
        //{
        //    DateSummary.Clear();
        //    // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
        //    int daysUntilMonday = ((int)DayOfWeek.Monday - (int)currdate.DayOfWeek + 7) % 7;
        //    DateTime nextMonday = currdate.AddDays(daysUntilMonday);
        //    DateSummary.Add(new DateWrapper(nextMonday));
        //    DateSummary.Add(new DateWrapper(nextMonday.AddDays(7)));
        //    DateSummary.Add(new DateWrapper(nextMonday.AddDays(14)));
        //    DateSummary.Add(new DateWrapper(nextMonday.AddDays(21)));
        //    DateSummary.Add(new DateWrapper(nextMonday.AddDays(28)));
        //    DateSummary.Add(new DateWrapper(nextMonday.AddDays(35)));
        //    DateSummary.Add(new DateWrapper(nextMonday.AddDays(42)));
        //    DateSummary.Add(new DateWrapper(nextMonday.AddDays(49)));
        //}

        //private void LoadSchedulingData()
        //{
        //    CanDeleteSchedule = true;

        //    foreach (RolePerSubProjectModel itemold in SchedulingItems)
        //    {
        //        itemold.Entries.Clear();
        //    }

        //    SchedulingItems.Clear();
        //    CopiedSchedulingItems.Clear();

        //    DateTime datestart = DateSummary.First().Value;
        //    DateTime dateend = DateSummary.Last().Value;
        //    //update employee Id
        //    List<SchedulingDataDbModel> schedulingdata = SQLAccess.LoadSchedulingData(DateSummary.First().Value, SelectedSubproject.Id);
        //    int i = 0;
        //    foreach (SchedulingDataDbModel item in schedulingdata)
        //    {
        //        RolePerSubProjectModel role = SelectedSubproject.RolesPerSub.Where(x => x.Id == item?.RoleId).FirstOrDefault();
        //        EmployeeModel em = null;

        //        if (role == null || item.EmployeeId != role.Employee.Id)
        //        {
        //            role = new RolePerSubProjectModel();
        //            role.Subproject = SelectedSubproject;
        //            EmployeeDbModel emdb = SQLAccess.LoadEmployeeById(item.EmployeeId);
        //            em = new EmployeeModel(emdb);
        //        }
        //        else
        //        {
        //            em = role.Employee;
        //        }

        //        role.Subproject = SelectedSubproject;
        //        role.EmployeeWrapper = em;

        //        role.Entries.Add(new SDEntryModel() { Date = DateSummary[0].Value, TimeEntry = item.Hours1 });
        //        role.Entries.Add(new SDEntryModel() { Date = DateSummary[1].Value, TimeEntry = item.Hours2 });
        //        role.Entries.Add(new SDEntryModel() { Date = DateSummary[2].Value, TimeEntry = item.Hours3 });
        //        role.Entries.Add(new SDEntryModel() { Date = DateSummary[3].Value, TimeEntry = item.Hours4 });
        //        role.Entries.Add(new SDEntryModel() { Date = DateSummary[4].Value, TimeEntry = item.Hours5 });
        //        role.Entries.Add(new SDEntryModel() { Date = DateSummary[5].Value, TimeEntry = item.Hours6 });
        //        role.Entries.Add(new SDEntryModel() { Date = DateSummary[6].Value, TimeEntry = item.Hours7 });
        //        role.Entries.Add(new SDEntryModel() { Date = DateSummary[7].Value, TimeEntry = item.Hours8 });

        //        role.Total = item.Hours1 + item.Hours2 + item.Hours3 + item.Hours4 + item.Hours5 + item.Hours6 + item.Hours7 + item.Hours8;
        //        SchedulingItems.Add(role);

        //        CopiedSchedulingItems.Add((RolePerSubProjectModel)role.Clone());
        //        i++;
        //    }

        //    //CollectEmployeeSummary();
        //    UpdateLists();

        //    DateTime lastSaturday = DateTime.Now.AddDays(-1);
        //    while (lastSaturday.DayOfWeek != DayOfWeek.Saturday)
        //        lastSaturday = lastSaturday.AddDays(-1);

        //    DateTime Curr = DateTime.Now.AddDays(-1);

        //    if (DateSummary[0].Value > Curr)
        //    {
        //        IsEditableItems = true;
        //    }
        //    else
        //    {
        //        IsEditableItems = false;
        //    }
        //    CollectEmployeeSummary();
        //}
    }
}
