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
using System.Windows.Threading;

namespace SOCE.Library.UI.ViewModels
{
    public class WeeklyScheduleVM : BaseVM
    {
        public ICommand PrintCommand { get; set; }
        public ICommand PreviousCommand { get; set; }
        public ICommand NextCommand { get; set; }
        public ICommand CurrentCommand { get; set; }
        public ICommand GoToSchedulingCommand { get; set; }

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

        private EmployeeLowResModel _selectedEmployee;
        public EmployeeLowResModel SelectedEmployee
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

        //private bool _canSelect = false;
        //public bool CanSelect
        //{
        //    get { return _canSelect; }
        //    set
        //    {
        //        _canSelect = value;
        //        RaisePropertyChanged(nameof(CanSelect));
        //    }
        //}

        private bool _schedulePerson = false;
        public bool SchedulePerson
        {
            get { return _schedulePerson; }
            set
            {
                _schedulePerson = value;
                RaisePropertyChanged(nameof(SchedulePerson));
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
            this.GoToSchedulingCommand = new RelayCommand<WeeklyScheduleModel>(OpenScheduling);
            List<EmployeeDbModel> employeesDb = SQLAccess.LoadEmployees();

            List<EmployeeLowResModel> totalemployees = new List<EmployeeLowResModel>();

            if (employee.Status == AuthEnum.Principal || employee.Status == AuthEnum.Admin || employee.Status == AuthEnum.PM)
            {
                SchedulePerson = true;
            }

            //if (employee.Status == AuthEnum.Admin || employee.Status == AuthEnum.Principal)
            //{
            //    CanSelect = true;
            //}

            foreach (EmployeeDbModel employeenew in employeesDb)
            {
                totalemployees.Add(new EmployeeLowResModel(employeenew));
            }

            UpdateDateSummary(DateTime.Today);

            ObservableCollection<EmployeeLowResModel> ordered = new ObservableCollection<EmployeeLowResModel>(totalemployees.OrderBy(x => x.LastName).ToList());
            Employees = ordered;
            SelectedEmployee = ordered.Where(x => x.Id == employee.Id).FirstOrDefault();
            //LoadScheduling();
        }

        public async void OpenScheduling(WeeklyScheduleModel wsm)
        {
            CoreAI CurrentPage = IoCCore.Application as CoreAI;
            CurrentPage.MakeBlurry();
            await Task.Run(() => Task.Delay(600));
           
            BaseAI originalpage = IoCPortal.Application as BaseAI;
            PortalAI portAI = (PortalAI)originalpage;
            ProjectScheduleVM psvm = (ProjectScheduleVM)portAI.CurrentPage;

            await psvm.NavigateToMenuAsync(ProjectScheduleViewEnum.UpcomingSchedule);

            //await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            //{
            //    psvm.SelectedViewEnum = ProjectScheduleViewEnum.UpcomingSchedule;
            //}));

            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                if (psvm.SelectedVM is ScheduleWeekVM)
                {
                    ScheduleWeekVM weekvm = (ScheduleWeekVM)psvm.SelectedVM;
                    ProjectLowResModel plrm = weekvm.ProjectList.Where(x => x.ProjectNumber == wsm.ProjectNumber).FirstOrDefault();

                    if (plrm != null)
                    {
                        weekvm.SelectedProject = plrm;
                    }
                }
            }));

            await Task.Run(() => Task.Delay(800));
            CurrentPage.MakeClear();
        }

        private async void Print()
        {
            IndividualSingleView view = new IndividualSingleView();
            IndividualSingleVM aysvm = new IndividualSingleVM();
            view.DataContext = aysvm;

            //show the dialog
            await DialogHost.Show(view, "RootDialog");

            IndividualSingleVM vm = view.DataContext as IndividualSingleVM;
            ResultEnum res = vm.Result;

            CoreAI CurrentPage = IoCCore.Application as CoreAI;

            if (res == ResultEnum.ResultTwo)
            {
                CurrentPage.MakeBlurry();
                await Task.Run(() => Task.Delay(600));
                await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => 
                {
                try
                {
                    string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    string pathDownload = Path.Combine(pathUser, "Downloads\\WeeklySchedule.xlsx");
                    File.WriteAllBytes(pathDownload, Properties.Resources.WeeklySchedule);
                    Excel.Excel exinst = new Excel.Excel(pathDownload);
                    Thread.Sleep(200);

                    if (SchedulingItems.Count > 0)
                    {
                        RunIndividualExport(ref exinst, SelectedEmployee);
                    }

                    Process.Start(pathDownload);
                }
                catch
                {
                }
                }));
                await Task.Run(() => Task.Delay(600));
                CurrentPage.MakeClear();
            }
            else if (res == ResultEnum.ResultOne)
            {
                CurrentPage.MakeBlurry();
                await Task.Run(() => Task.Delay(600));
                await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    try
                    {
                        EmployeeLowResModel selectedpm = SelectedEmployee;
                        string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                        string pathDownload = Path.Combine(pathUser, "Downloads\\WeeklySchedule.xlsx");
                        File.WriteAllBytes(pathDownload, Properties.Resources.WeeklySchedule);
                        Excel.Excel exinst = new Excel.Excel(pathDownload);

                        Thread.Sleep(200);

                        foreach (EmployeeLowResModel em in Employees)
                        {
                            exinst.CopyFirstWorksheet(em.FullName, "Sheet1");

                            RunIndividualExport(ref exinst, em);
                        }

                        exinst.DeleteWorksheet("Sheet1");
                        exinst.SaveDocument();
                        Process.Start(pathDownload);
                        SelectedEmployee = selectedpm;
                    }
                    catch
                    {
                    }
                }));
                await Task.Run(() => Task.Delay(600));
                CurrentPage.MakeClear();
            }
        }


        private void RunIndividualExport(ref Excel.Excel exinst, EmployeeLowResModel pmofinterest)
        {
            int rowid = 1;
            exinst.WriteCell(rowid, 1, "Report for: " + pmofinterest.FullName);
            List<string> dates = new List<string>();

            foreach (DateWrapper date in DateSummary)
            {
                dates.Add(date.Value.ToString("MM/dd/yyyy"));
            }

            exinst.WriteRow<string>(rowid + 2, 8, dates);

            List<WeeklyScheduleModel> newitems = LoadEmployeeSchedulingData(pmofinterest);

            List<WeeklyScheduleModel> items = newitems.OrderBy(x => x.ClientNumber).ToList();

            foreach (WeeklyScheduleModel wsm in items)
            {
                List<object> values = new List<object>();

                values.Add(wsm.ProjectName);
                values.Add(wsm.ProjectNumber);
                values.Add(wsm.ClientNumber);
                values.Add(wsm.PhaseName);
                values.Add(wsm.DueDate ?? "");
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

                if (rowid   == 1)
                {
                    exinst.WriteRow<object>(rowid + 3, 1, values);
                }
                else
                {
                    exinst.InsertRowBelow(rowid +2, values);

                }


                    string formula = $"SUM(H{rowid+3}: O{rowid+3})";
                    exinst.WriteFormula(rowid+3, 16, formula);
                


                rowid++;
            }

            exinst.SaveDocument();

        }

        private async void PreviousTimesheet()
        {
            CoreAI CurrentPage = IoCCore.Application as CoreAI;
            CurrentPage.MakeBlurry();
            await Task.Run(() => Task.Delay(600));
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => UpdateDateSummary(DateSummary.First().Value.AddDays(-7))));
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => LoadScheduling()));
            await Task.Run(() => Task.Delay(600));
            CurrentPage.MakeClear();
        }

        /// <summary>LoadScheduling
        /// Button Press
        /// </summary>
        private async void NextTimesheet()
        {
            CoreAI CurrentPage = IoCCore.Application as CoreAI;
            CurrentPage.MakeBlurry();
            await Task.Run(() => Task.Delay(600));
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => UpdateDateSummary(DateSummary.First().Value.AddDays(7))));
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => LoadScheduling()));
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
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => LoadScheduling()));
            await Task.Run(() => Task.Delay(600));
            CurrentPage.MakeClear();
        }

        private void UpdateDateSummary(DateTime currdate)
        {
            DateSummary = null;
            List<DateWrapper> dates = new List<DateWrapper>();

            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysUntilMonday = ((int)DayOfWeek.Monday - (int)currdate.DayOfWeek + 7) % 7;
            DateTime nextMonday = currdate.AddDays(daysUntilMonday);
            WeekDate = nextMonday.ToString("MM/dd/yyyy");
            dates.Add(new DateWrapper(nextMonday));
            dates.Add(new DateWrapper(nextMonday.AddDays(7)));
            dates.Add(new DateWrapper(nextMonday.AddDays(14)));
            dates.Add(new DateWrapper(nextMonday.AddDays(21)));
            dates.Add(new DateWrapper(nextMonday.AddDays(28)));
            dates.Add(new DateWrapper(nextMonday.AddDays(35)));
            dates.Add(new DateWrapper(nextMonday.AddDays(42)));
            dates.Add(new DateWrapper(nextMonday.AddDays(49)));
            DateSummary = new ObservableCollection<DateWrapper>(dates);
        }

        private void LoadScheduling()
        {
            List<SDEntryModel> totals = new List<SDEntryModel>();
            SchedulingItems = null;

            List<WeeklyScheduleModel> weeklystuff = LoadEmployeeSchedulingData(SelectedEmployee);
            SchedulingItems = new ObservableCollection<WeeklyScheduleModel>(weeklystuff);
            double total1 = 0;
            double total2 = 0;
            double total3 = 0;
            double total4 = 0;
            double total5 = 0;
            double total6 = 0;
            double total7 = 0;
            double total8 = 0;
            foreach (WeeklyScheduleModel week in weeklystuff)
            {
                total1 += week.Entries[0].TimeEntry;
                total2 += week.Entries[1].TimeEntry;
                total3 += week.Entries[2].TimeEntry;
                total4 += week.Entries[3].TimeEntry;
                total5 += week.Entries[4].TimeEntry;
                total6 += week.Entries[5].TimeEntry;
                total7 += week.Entries[6].TimeEntry;
                total8 += week.Entries[7].TimeEntry;
            }

            totals.Add(new SDEntryModel() { Date = DateSummary[0].Value, TimeEntry = total1 });
            totals.Add(new SDEntryModel() { Date = DateSummary[1].Value, TimeEntry = total2 });
            totals.Add(new SDEntryModel() { Date = DateSummary[2].Value, TimeEntry = total3 });
            totals.Add(new SDEntryModel() { Date = DateSummary[3].Value, TimeEntry = total4 });
            totals.Add(new SDEntryModel() { Date = DateSummary[4].Value, TimeEntry = total5 });
            totals.Add(new SDEntryModel() { Date = DateSummary[5].Value, TimeEntry = total6 });
            totals.Add(new SDEntryModel() { Date = DateSummary[6].Value, TimeEntry = total7 });
            totals.Add(new SDEntryModel() { Date = DateSummary[7].Value, TimeEntry = total8 });

            Totals = new ObservableCollection<SDEntryModel>(totals);

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

        private List<WeeklyScheduleModel> LoadEmployeeSchedulingData(EmployeeLowResModel employeeofinterest)
        {
            List<WeeklyScheduleModel> weekly = new List<WeeklyScheduleModel>();
            List<SchedulingDataDbModel> schedulingdata = SQLAccess.LoadSchedulingDataByEmployee(DateSummary.First().Value, employeeofinterest.Id);

            foreach (SchedulingDataDbModel dbmodel in schedulingdata)
            {
                SubProjectDbModel sub = SQLAccess.LoadSubProjectsBySubProject(dbmodel.PhaseId);

                string subprojectname = "";
                string projectname = "";
                string projectmanager = "";
                double projectnumber=0;
                double percentcomplete = 0;
                DateTime? duedate = null;
                double clientnumber=0;

                if (sub != null)
                {
                    subprojectname = sub.PointNumber;
                    percentcomplete = sub.PercentComplete;
                    ProjectDbModel project = SQLAccess.LoadProjectsById(sub.ProjectId);

                    if (project != null)
                    {
                        projectname = project.ProjectName;
                        projectnumber = project.ProjectNumber;
                        EmployeeDbModel emtemp = SQLAccess.LoadEmployeeById(project.ManagerId);
                        projectmanager = emtemp.FullName;
                        if (project?.DueDate != null && project?.DueDate != 0)
                        {
                            duedate = DateTime.ParseExact(project.DueDate.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                        }
                    }

                    ClientDbModel client = SQLAccess.LoadClientById(project.ClientId);
                    clientnumber = client.ClientNumber;
                }
                else
                {
                    subprojectname = dbmodel.PhaseName;
                    projectname = dbmodel.ProjectName;

                }

                string finisheddate = duedate?.ToString("MM/dd/yyyy");

                WeeklyScheduleModel weeklymodel = new WeeklyScheduleModel();
                weeklymodel.ProjectName = projectname;
                weeklymodel.ProjectNumber = projectnumber;
                weeklymodel.ClientNumber = clientnumber;
                weeklymodel.PhaseName = subprojectname;
                weeklymodel.DueDate = finisheddate;
                weeklymodel.PercentComplete = percentcomplete;
                weeklymodel.PM = projectmanager;

                weeklymodel.Entries.Add(new SDEntryModel() { Date = DateSummary[0].Value, TimeEntry = dbmodel.Hours1 });
                weeklymodel.Entries.Add(new SDEntryModel() { Date = DateSummary[1].Value, TimeEntry = dbmodel.Hours2 });
                weeklymodel.Entries.Add(new SDEntryModel() { Date = DateSummary[2].Value, TimeEntry = dbmodel.Hours3 });
                weeklymodel.Entries.Add(new SDEntryModel() { Date = DateSummary[3].Value, TimeEntry = dbmodel.Hours4 });
                weeklymodel.Entries.Add(new SDEntryModel() { Date = DateSummary[4].Value, TimeEntry = dbmodel.Hours5 });
                weeklymodel.Entries.Add(new SDEntryModel() { Date = DateSummary[5].Value, TimeEntry = dbmodel.Hours6 });
                weeklymodel.Entries.Add(new SDEntryModel() { Date = DateSummary[6].Value, TimeEntry = dbmodel.Hours7 });
                weeklymodel.Entries.Add(new SDEntryModel() { Date = DateSummary[7].Value, TimeEntry = dbmodel.Hours8 });

                weeklymodel.Total = dbmodel.Hours1 + dbmodel.Hours2 + dbmodel.Hours3 + dbmodel.Hours4 + dbmodel.Hours5 + dbmodel.Hours6 + dbmodel.Hours7 + dbmodel.Hours8;

                weekly.Add(weeklymodel);
            }

            return weekly;
        }
    }
}
