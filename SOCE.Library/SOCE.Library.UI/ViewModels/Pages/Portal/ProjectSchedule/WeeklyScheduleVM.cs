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
using System.Threading;

namespace SOCE.Library.UI.ViewModels
{
    public class WeeklyScheduleVM : BaseVM
    {
        public ICommand PrintCommand { get; set; }
        public ICommand PreviousCommand { get; set; }
        public ICommand NextCommand { get; set; }
        public ICommand CurrentCommand { get; set; }

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

        //private ObservableCollection<ProjectModel> _projectList;
        //public ObservableCollection<ProjectModel> ProjectList
        //{
        //    get { return _projectList; }
        //    set
        //    {
        //        _projectList = value;
        //        RaisePropertyChanged(nameof(ProjectList));
        //    }
        //}

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

        private EmployeeModel _selectedEmployee;
        public EmployeeModel SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
                LoadScheduling();
                RaisePropertyChanged(nameof(SelectedEmployee));
            }
        }

        private ObservableCollection<WeeklyScheduleModel> _schedulingItems = new ObservableCollection<WeeklyScheduleModel>();
        public ObservableCollection<WeeklyScheduleModel> SchedulingItems
        {
            get { return _schedulingItems; }
            set
            {
                _schedulingItems = value;
                RaisePropertyChanged(nameof(SchedulingItems));
            }
        }

        private bool _canSelect = false;
        public bool CanSelect
        {
            get { return _canSelect; }
            set
            {
                _canSelect = value;
                RaisePropertyChanged(nameof(CanSelect));
            }
        }

        private bool _canSeeNext = false;
        public bool CanSeeNext
        {
            get { return _canSeeNext; }
            set
            {
                _canSeeNext = value;
                RaisePropertyChanged(nameof(CanSeeNext));
            }
        }

        private ObservableCollection<SDEntryModel> _totals = new ObservableCollection<SDEntryModel>();
        public ObservableCollection<SDEntryModel> Totals
        {
            get { return _totals; }
            set
            {
                _totals = value;
                RaisePropertyChanged(nameof(Totals));
            }
        }


        public WeeklyScheduleVM(EmployeeModel employee)
        {
            //this.PrintCommand = new RelayCommand(this.Print);
            this.PreviousCommand = new RelayCommand(PreviousTimesheet);
            this.NextCommand = new RelayCommand(NextTimesheet);
            this.CurrentCommand = new RelayCommand(CurrentTimesheet);
            this.PrintCommand = new RelayCommand(Print);
            List<EmployeeDbModel> employeesDb = SQLAccess.LoadEmployees();

            List<EmployeeModel> totalemployees = new List<EmployeeModel>();

            if (employee.Status == AuthEnum.Admin || employee.Status == AuthEnum.Principal)
            {
                CanSelect = true;
            }



            foreach (EmployeeDbModel employeenew in employeesDb)
            {
                totalemployees.Add(new EmployeeModel(employeenew));
            }

            UpdateDateSummary(DateTime.Today);

            ObservableCollection<EmployeeModel> ordered = new ObservableCollection<EmployeeModel>(totalemployees.OrderBy(x => x.LastName).ToList());
            Employees = ordered;
            SelectedEmployee = ordered.Where(x => x.Id == employee.Id).FirstOrDefault();
            //LoadScheduling();
        }

        private void Print()
        {
            //do stuff
            //save down to downloads
            try
            {
                string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string pathDownload = Path.Combine(pathUser, "Downloads\\WeeklySchedule.xlsx");
                File.WriteAllBytes(pathDownload, Properties.Resources.WeeklySchedule);
                Excel.Excel exinst = new Excel.Excel(pathDownload);

                Thread.Sleep(200);

                int rowid = 4;

                if (SchedulingItems.Count > 0)
                {
                    exinst.WriteCell(1, 1, "Report for: " + SelectedEmployee.FullName);
                    List<string> dates = new List<string>();

                    foreach (DateWrapper date in DateSummary)
                    {
                        dates.Add(date.Value.ToString("MM/dd/yyyy"));
                    }

                    exinst.WriteRow<string>(rowid - 1, 8, dates);

                    List< WeeklyScheduleModel> items = SchedulingItems.OrderBy(x => x.ClientNumber).ToList();

                    foreach (WeeklyScheduleModel wsm in items)
                    {
                        List<object> values = new List<object>();

                        values.Add(wsm.ProjectName);
                        values.Add(wsm.ProjectNumber);
                        values.Add(wsm.ClientNumber);
                        values.Add(wsm.PhaseName);
                        values.Add(wsm.DueDate ?? "" );
                        values.Add(wsm.PercentComplete);
                        values.Add(wsm.PM);

                        foreach (SDEntryModel sd in wsm.Entries)
                        {
                            if (sd.TimeEntry == 0)
                            {
                                values.Add("");
                            }
                            else
                            {
                                values.Add(sd.TimeEntry);
                            }
                        }

                        if (rowid == 4)
                        {
                            exinst.WriteRow<object>(rowid, 1, values);
                        }
                        else
                        {
                            exinst.InsertRowBelow(rowid - 1, values);

                        }

                        string formula = $"SUM(H{rowid}: O{rowid})";
                        exinst.WriteFormula(rowid, 16, formula);
                        rowid++;

                    }

                    exinst.SaveDocument();
                }

                Process.Start(pathDownload);
            }
            catch
            {
            }
        }


        private void PreviousTimesheet()
        {
            UpdateDateSummary(DateSummary.First().Value.AddDays(-7));
            LoadScheduling();
        }

        /// <summary>LoadScheduling
        /// Button Press
        /// </summary>
        private void NextTimesheet()
        {
            UpdateDateSummary(DateSummary.First().Value.AddDays(7));
            LoadScheduling();
        }

        /// <summary>
        /// Button Press
        /// </summary>
        private void CurrentTimesheet()
        {
            UpdateDateSummary(DateTime.Now);
            LoadScheduling();
        }

        private void UpdateDateSummary(DateTime currdate)
        {
            DateSummary.Clear();
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysUntilMonday = ((int)DayOfWeek.Monday - (int)currdate.DayOfWeek + 7) % 7;
            DateTime nextMonday = currdate.AddDays(daysUntilMonday);
            DateSummary.Add(new DateWrapper(nextMonday));
            DateSummary.Add(new DateWrapper(nextMonday.AddDays(7)));
            DateSummary.Add(new DateWrapper(nextMonday.AddDays(14)));
            DateSummary.Add(new DateWrapper(nextMonday.AddDays(21)));
            DateSummary.Add(new DateWrapper(nextMonday.AddDays(28)));
            DateSummary.Add(new DateWrapper(nextMonday.AddDays(35)));
            DateSummary.Add(new DateWrapper(nextMonday.AddDays(42)));
            DateSummary.Add(new DateWrapper(nextMonday.AddDays(49)));

        }

        private void LoadScheduling()
        {
            Totals.Clear();
            SchedulingItems.Clear();
            List<SchedulingDataDbModel> schedulingdata = SQLAccess.LoadSchedulingDataByEmployee(DateSummary.First().Value, SelectedEmployee.Id);
            double total1 = 0;
            double total2 = 0;
            double total3 = 0;
            double total4 = 0;
            double total5 = 0;
            double total6 = 0;
            double total7 = 0;
            double total8 = 0;

            foreach (SchedulingDataDbModel dbmodel in schedulingdata)
            {
                SubProjectDbModel sub = SQLAccess.LoadSubProjectsBySubProject(dbmodel.PhaseId);
                ProjectDbModel project = SQLAccess.LoadProjectsById(sub.ProjectId);
                ClientDbModel client = SQLAccess.LoadClientById(project.ClientId);

                DateTime? newdate = null;

                if (project?.DueDate != null && project?.DueDate != 0)
                {
                    newdate = DateTime.ParseExact(project.DueDate.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                }

                string finisheddate = newdate?.ToString("MM/dd/yyyy");

                WeeklyScheduleModel weeklymodel = new WeeklyScheduleModel();
                weeklymodel.ProjectName = project.ProjectName;
                weeklymodel.ProjectNumber = project.ProjectNumber;
                weeklymodel.ClientNumber = client.ClientNumber;
                weeklymodel.PhaseName = sub.PointNumber;
                weeklymodel.DueDate = finisheddate;
                weeklymodel.PercentComplete = sub.PercentComplete;
                EmployeeDbModel employee = SQLAccess.LoadEmployeeById(project.ManagerId);
                weeklymodel.PM = employee.FullName;

                weeklymodel.Entries.Add(new SDEntryModel() { Date = DateSummary[0].Value, TimeEntry = dbmodel.Hours1 });
                weeklymodel.Entries.Add(new SDEntryModel() { Date = DateSummary[1].Value, TimeEntry = dbmodel.Hours2 });
                weeklymodel.Entries.Add(new SDEntryModel() { Date = DateSummary[2].Value, TimeEntry = dbmodel.Hours3 });
                weeklymodel.Entries.Add(new SDEntryModel() { Date = DateSummary[3].Value, TimeEntry = dbmodel.Hours4 });
                weeklymodel.Entries.Add(new SDEntryModel() { Date = DateSummary[4].Value, TimeEntry = dbmodel.Hours5 });
                weeklymodel.Entries.Add(new SDEntryModel() { Date = DateSummary[5].Value, TimeEntry = dbmodel.Hours6 });
                weeklymodel.Entries.Add(new SDEntryModel() { Date = DateSummary[6].Value, TimeEntry = dbmodel.Hours7 });
                weeklymodel.Entries.Add(new SDEntryModel() { Date = DateSummary[7].Value, TimeEntry = dbmodel.Hours8 });


                weeklymodel.Total = dbmodel.Hours1 + dbmodel.Hours2 + dbmodel.Hours3;

                total1 += dbmodel.Hours1;
                total2 += dbmodel.Hours2;
                total3 += dbmodel.Hours3;
                total4 += dbmodel.Hours4;
                total5 += dbmodel.Hours5;
                total6 += dbmodel.Hours6;
                total7 += dbmodel.Hours7;
                total8 += dbmodel.Hours8;

                SchedulingItems.Add(weeklymodel);
            }
            Totals.Add(new SDEntryModel() { Date = DateSummary[0].Value, TimeEntry = total1 } );
            Totals.Add(new SDEntryModel() { Date = DateSummary[1].Value, TimeEntry = total2 } );
            Totals.Add(new SDEntryModel() { Date = DateSummary[2].Value, TimeEntry = total3 } );
            Totals.Add(new SDEntryModel() { Date = DateSummary[3].Value, TimeEntry = total4 });
            Totals.Add(new SDEntryModel() { Date = DateSummary[4].Value, TimeEntry = total5 });
            Totals.Add(new SDEntryModel() { Date = DateSummary[5].Value, TimeEntry = total6 });
            Totals.Add(new SDEntryModel() { Date = DateSummary[6].Value, TimeEntry = total7 });
            Totals.Add(new SDEntryModel() { Date = DateSummary[7].Value, TimeEntry = total8 });

            DateTime Curr = DateTime.Now.AddDays(-1);

            if (DateSummary[0].Value > Curr)
            {
                CanSeeNext = false;
            }
            else
            {
                CanSeeNext = true;
            }
        }
    }
}
