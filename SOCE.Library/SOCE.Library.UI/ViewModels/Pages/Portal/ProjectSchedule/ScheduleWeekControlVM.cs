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

                if (_selectedSubproject != null && firstopen)
                {
                    firstopen = false;
                    BaseVM.UpdatePhaseLists();
                }

                foreach (RolePerSubProjectModel roles in SchedulingItems)
                {
                    roles.Subproject = _selectedSubproject;
                }

                firstopen = true;
                RaisePropertyChanged(nameof(SelectedSubproject));
            }
        }

        bool firstopen = false;

        //private ObservableCollection<RolePerSubProjectModel> CopiedSchedulingItems = new ObservableCollection<RolePerSubProjectModel>();

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

        public ScheduleWeekControlVM(List<RolePerSubProjectModel> InitialRoleLoadIn, ObservableCollection<DateWrapper> dates, SubProjectModel sub, ObservableCollection<SubProjectModel> subs, ScheduleWeekVM scheduleweek)
        {
            Constructor();
            SubProjects = subs;
            SelectedSubproject = sub;
            HoursLeft = SelectedSubproject.HoursLeft;
            HoursSpent = SelectedSubproject.HoursUsed;
            foreach (RolePerSubProjectModel role in InitialRoleLoadIn)
            {
                role.EmployeeList = Employees;
                SchedulingItems.Add(role);
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

        public ScheduleWeekControlVM(ObservableCollection<DateWrapper> dates, SubProjectModel sub, ObservableCollection<SubProjectModel> subs, ScheduleWeekVM scheduleweek)
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
            this.DeleteRole = new RelayCommand<RolePerSubProjectModel>(this.DeleteRoleIfPossible);
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

        }

        private void DeletePhase()
        {
            int id = -1;
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
        }

        private void AddRole()
        {
            if (SelectedSubproject != null)
            {
                RolePerSubProjectModel rpspm = new RolePerSubProjectModel(SelectedSubproject, SelectedSubproject.Fee);
                rpspm.EditRoleFieldState = false;
                rpspm.SpentHours = 0;
                rpspm.Subproject = SelectedSubproject;
                rpspm.EmployeeList = Employees;
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
                //UpdateLists();
            }
        }

        private void DeleteRoleIfPossible(RolePerSubProjectModel rpsm)
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
