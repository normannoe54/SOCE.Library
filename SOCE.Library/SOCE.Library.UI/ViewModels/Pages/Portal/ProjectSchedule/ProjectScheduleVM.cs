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
    public class ProjectScheduleVM : BaseVM
    {
        private ProjectScheduleViewEnum _selectedViewEnum;
        public ProjectScheduleViewEnum SelectedViewEnum
        {
            get { return _selectedViewEnum; }
            set
            {
                _selectedViewEnum = value;

                switch (_selectedViewEnum)
                {
                    case ProjectScheduleViewEnum.WeeklySchedule:
                        WeeklyScheduleVM wsvm = new WeeklyScheduleVM(CurrentEmployee);
                        SelectedVM = wsvm;
                        break;
                    case ProjectScheduleViewEnum.PMReport:
                        PMScheduleVM pmschedvm = new PMScheduleVM(CurrentEmployee);
                        SelectedVM = pmschedvm;
                        break;
                    case ProjectScheduleViewEnum.UpcomingSchedule:
                        ScheduleWeekVM swvm = new ScheduleWeekVM();
                        SelectedVM = swvm;
                        break;
                    case ProjectScheduleViewEnum.ProjectList:
                        ProjectListVM plvm = new ProjectListVM();
                        SelectedVM = plvm;
                        break;
                    default:
                        break;
                }
                RaisePropertyChanged(nameof(SelectedViewEnum));
            }
        }

        private BaseVM _selectedVM;
        public BaseVM SelectedVM
        {
            get { return _selectedVM; }
            set
            {
                _selectedVM = value;
                RaisePropertyChanged(nameof(SelectedVM));
            }
        }

        private EmployeeModel _currentEmployee;
        public EmployeeModel CurrentEmployee
        {
            get
            {
                return _currentEmployee;
            }
            set
            {
                _currentEmployee = value;
                RaisePropertyChanged(nameof(CurrentEmployee));
            }
        }

        private string _dateEndString = "";
        public string DateEndString
        {
            get { return _dateEndString; }
            set
            {
                _dateEndString = value;
                RaisePropertyChanged(nameof(DateEndString));
            }
        }

        private string _dateStartString = "";
        public string DateStartString
        {
            get { return _dateStartString; }
            set
            {
                _dateStartString = value;
                RaisePropertyChanged(nameof(DateStartString));
            }
        }

        private bool _managerSettings = false;
        public bool ManagerSettings
        {
            get { return _managerSettings; }
            set
            {
                _managerSettings = value;
                RaisePropertyChanged(nameof(ManagerSettings));
            }
        }

        public DateTime firstdate;

        public DateTime lastdate;


        public ProjectScheduleVM(EmployeeModel loggedinEmployee)
        {
            //UpdateDates(DateTime.Now);
            UpdateDates(DateTime.Now);
            //IsButtonEditable = false;
            CurrentEmployee = loggedinEmployee;
            if (CurrentEmployee.Status == AuthEnum.Principal || CurrentEmployee.Status == AuthEnum.Admin || CurrentEmployee.Status == AuthEnum.PM)
            {
                ManagerSettings = true;
            }

            //check if any data exists after monday of the week
            //if not copy all from last week

            DateTime nextweekdate = DateTime.Today.AddDays(7);
            DateTime thisMonday = nextweekdate.StartOfWeek(DayOfWeek.Monday);
            //int daysUntilMonday = ((int)DayOfWeek.Monday - (int)DateTime.Today.DayOfWeek) % 7;
            //DateTime nextMonday = DateTime.Today.AddDays(daysUntilMonday);
            List<SchedulingDataDbModel> schedulingdata = SQLAccess.LoadSchedulingDataByAboveDate(thisMonday);

            if (schedulingdata.Count == 0)
            {
                //copy all
                List<SchedulingDataDbModel> previousweekschedulingdata = SQLAccess.LoadSchedulingDataByDate(thisMonday.AddDays(-7));

                foreach(SchedulingDataDbModel sched in previousweekschedulingdata)
                {
                    if (sched.Hours2 != 0 || sched.Hours3 != 0 || sched.Hours4 != 0 || sched.Hours5 != 0 || sched.Hours6 != 0 || sched.Hours7 != 0 || sched.Hours8 != 0)
                    {
                        sched.Date = (int)long.Parse(thisMonday.ToString("yyyyMMdd"));
                        sched.Hours1 = sched.Hours2;
                        sched.Hours2 = sched.Hours3;
                        sched.Hours3 = sched.Hours4;
                        sched.Hours4 = sched.Hours5;
                        sched.Hours5 = sched.Hours6;
                        sched.Hours6 = sched.Hours7;
                        sched.Hours7 = sched.Hours8;
                        sched.Hours8 = 0;
                        SQLAccess.AddSchedulingData(sched);
                    }
                }

            }

            SelectedViewEnum = ProjectScheduleViewEnum.WeeklySchedule;
            //ReviewVM = new EmployeeSummaryVM(this);
        }

        public void UpdateDates(DateTime currdate)
        {
            DateTime firstdate = currdate.StartOfWeek(DayOfWeek.Monday).AddDays(7);
            DateTime lastdate = firstdate.StartOfWeek(DayOfWeek.Friday).AddDays(7);

            //firstdate = new DateTime(currdate.Year, currdate.Month + 1, 1);

            //if (currdate.Day > 16)
            //{
            //    firstdate = new DateTime(currdate.Year, currdate.Month + 1, 1);
            //    lastdate = new DateTime(currdate.Year, currdate.Month + 1, 16);
            //    //second tier

            //}
            //else
            //{
            //    //first tier
            //    firstdate = new DateTime(currdate.Year, currdate.Month, 17);
            //    lastdate = new DateTime(currdate.Year, currdate.Month, DateTime.DaysInMonth(currdate.Year, currdate.Month));
            //}

            int diff = (lastdate - firstdate).Days;
            List<DateWrapper> dates = new List<DateWrapper>();
            int workdays = 0;

            for (int i = 0; i <= diff; i++)
            {
                DateTime dt = firstdate.AddDays(i);
                dates.Add(new DateWrapper(dt.Date));

                if (!(dt.DayOfWeek == DayOfWeek.Saturday) && !(dt.DayOfWeek == DayOfWeek.Sunday))
                {
                    workdays++;
                }
            }

            DateStartString = $"{firstdate.ToString("MMM")}. {firstdate.Day}";
            DateEndString = $" {lastdate.ToString("MMM")}. {lastdate.Day}";

        }
    }
}
