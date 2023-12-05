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
    public class PMScheduleVM : BaseVM
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

        private EmployeeLowResModel _selectedPM;
        public EmployeeLowResModel SelectedPM
        {
            get { return _selectedPM; }
            set
            {
                _selectedPM = value;
                PMReportItems.Clear();
                PMReportItems = new ObservableCollection<PMScheduleModel>(LoadPMSchedule(value, DateSummary.First()));
                RaisePropertyChanged(nameof(SelectedPM));
            }
        }

        private ObservableCollection<PMScheduleModel> _pMReportItems = new ObservableCollection<PMScheduleModel>();
        public ObservableCollection<PMScheduleModel> PMReportItems
        {
            get { return _pMReportItems; }
            set
            {
                _pMReportItems = value;
                RaisePropertyChanged(nameof(PMReportItems));
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

        public PMScheduleVM(EmployeeModel employee)
        {
            this.PreviousCommand = new RelayCommand(PreviousTimesheet);
            this.NextCommand = new RelayCommand(NextTimesheet);
            this.CurrentCommand = new RelayCommand(CurrentTimesheet);
            this.PrintCommand = new RelayCommand(Print);
            List<EmployeeDbModel> employeesDb = SQLAccess.LoadEmployees();

            List<EmployeeLowResModel> totalemployees = new List<EmployeeLowResModel>();

            foreach (EmployeeDbModel employeenew in employeesDb)
            {
                totalemployees.Add(new EmployeeLowResModel(employeenew));
            }

            UpdateDateSummary(DateTime.Today);

            ObservableCollection<EmployeeLowResModel> ordered = new ObservableCollection<EmployeeLowResModel>(totalemployees.OrderBy(x => x.LastName).ToList());
            Employees = ordered;

            EmployeeLowResModel foundem = Employees.Where(x => x.Id == employee.Id).FirstOrDefault();
            LoadProjectManagers(foundem);
            //SelectedEmployee = ordered.Where(x => x.Id == employee.Id).FirstOrDefault();
            //LoadScheduling();
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

            if (res == ResultEnum.ResultTwo)
            {
                CoreAI CurrentPage = IoCCore.Application as CoreAI;
                CurrentPage.MakeBlurry();
                await Task.Run(() => Task.Delay(600));
                await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    try
                    {
                        string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                        string pathDownload = Path.Combine(pathUser, "Downloads\\PMSchedule.xlsx");
                        File.WriteAllBytes(pathDownload, Properties.Resources.PMSchedule);
                        Excel.Excel exinst = new Excel.Excel(pathDownload);

                        Thread.Sleep(200);

                        int rowid = 1;
                        if (PMReportItems.Count > 0)
                        {
                            RunIndividualExport(ref exinst, ref rowid, SelectedPM);

                            exinst.SaveDocument();
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
                CoreAI CurrentPage = IoCCore.Application as CoreAI;
                CurrentPage.MakeBlurry();
                await Task.Run(() => Task.Delay(600));
                await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    try
                {
                    EmployeeLowResModel selectedpm = SelectedPM;
                    string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    string pathDownload = Path.Combine(pathUser, "Downloads\\PMSchedule.xlsx");
                    File.WriteAllBytes(pathDownload, Properties.Resources.PMSchedule);
                    Excel.Excel exinst = new Excel.Excel(pathDownload);

                    Thread.Sleep(200);

                    int rowid = 1;

                    foreach (EmployeeLowResModel em in ProjectManagers)
                    {
                        RunIndividualExport(ref exinst, ref rowid, em);
                    }

                    Process.Start(pathDownload);
                    SelectedPM = selectedpm;
                }
                catch
                {
                }
                }));
                await Task.Run(() => Task.Delay(600));
                CurrentPage.MakeClear();
            }
        }

        private void RunIndividualExport(ref Excel.Excel exinst, ref int rowid, EmployeeLowResModel pmofinterest)
        {
            List<PMScheduleModel> pmstuff = LoadPMSchedule(pmofinterest, DateSummary.First());

            if (pmstuff.Count > 0)
            {
                List<object> values = new List<object>();

                string header = "Project Manager Report for: " + pmofinterest.FullName;

                values.Add(header);

                foreach (DateWrapper date in DateSummary)
                {
                    values.Add(date.Value.ToString("MM/dd/yyyy"));
                }

                exinst.WriteRow<object>(rowid, 1, values);
                exinst.LeftCell(rowid, 1);
                exinst.MakeRowBold(rowid, 1, values.Count());
                exinst.MakeRowItalic(rowid, 1, values.Count());
                exinst.MakeRowGray(rowid, 1, values.Count());
                exinst.MakeRowDoubleBorderedTopandBot(rowid, 1, values.Count());
                rowid++;

                foreach (PMScheduleModel pm in pmstuff)
                {
                    exinst.WriteCell(rowid, 1, pm.ProjectName + " [" + pm.ProjectNumber + "]");
                    exinst.LeftCell(rowid, 1);
                    exinst.MakeRowBold(rowid, 1, values.Count());

                    rowid++;

                    int count = pm.EmployeeSummary.Count();
                    int j = 0;
                    foreach (PMScheduleIndModel pmind in pm.EmployeeSummary)
                    {
                        List<object> projectvalues = new List<object>();
                        projectvalues.Add(pmind.EmployeeName);

                        foreach (SDEntryModel entry in pmind.Entries)
                        {
                            if (entry.TimeEntry == 0)
                            {
                                projectvalues.Add("");
                            }
                            else
                            {
                                projectvalues.Add(entry.TimeEntry);
                            }
                        }

                        exinst.WriteRow<object>(rowid, 1, projectvalues);
                        j++;

                        if (j == count)
                        {
                            exinst.MakeRowDoubleBorderedTopandBot(rowid, 1, values.Count());
                        }

                        rowid++;

                    }
                }

                exinst.SaveDocument();
            }
        }

        private async void PreviousTimesheet()
        {
            CoreAI CurrentPage = IoCCore.Application as CoreAI;
            CurrentPage.MakeBlurry();
            await Task.Run(() => Task.Delay(600));
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => UpdateDateSummary(DateSummary.First().Value.AddDays(-7))));
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                PMReportItems = new ObservableCollection<PMScheduleModel>(LoadPMSchedule(SelectedPM, DateSummary.First()));
            }));
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
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                PMReportItems = new ObservableCollection<PMScheduleModel>(LoadPMSchedule(SelectedPM, DateSummary.First()));
            }));
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
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                PMReportItems = new ObservableCollection<PMScheduleModel>(LoadPMSchedule(SelectedPM, DateSummary.First()));
            }));
            await Task.Run(() => Task.Delay(600));
            CurrentPage.MakeClear();
        }

        private void UpdateDateSummary(DateTime currdate)
        {
            DateSummary.Clear();
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysUntilMonday = ((int)DayOfWeek.Monday - (int)currdate.DayOfWeek + 7) % 7;
            DateTime nextMonday = currdate.AddDays(daysUntilMonday);
            WeekDate = nextMonday.ToString("MM/dd/yyyy");
            DateSummary.Add(new DateWrapper(nextMonday));
            DateSummary.Add(new DateWrapper(nextMonday.AddDays(7)));
            DateSummary.Add(new DateWrapper(nextMonday.AddDays(14)));
            DateSummary.Add(new DateWrapper(nextMonday.AddDays(21)));
            DateSummary.Add(new DateWrapper(nextMonday.AddDays(28)));
            DateSummary.Add(new DateWrapper(nextMonday.AddDays(35)));
            DateSummary.Add(new DateWrapper(nextMonday.AddDays(42)));
            DateSummary.Add(new DateWrapper(nextMonday.AddDays(49)));


        }

        private void LoadProjectManagers(EmployeeLowResModel curremployee)
        {
            List<EmployeeDbModel> PMs = SQLAccess.LoadProjectManagers();

            ObservableCollection<EmployeeLowResModel> members = new ObservableCollection<EmployeeLowResModel>();

            foreach (EmployeeDbModel edbm in PMs)
            {
                members.Add(new EmployeeLowResModel(edbm));
            }

            ProjectManagers = members;
            EmployeeLowResModel em = ProjectManagers.Where(x => x.Id == curremployee.Id).FirstOrDefault();
            if (em == null)
            {
                SelectedPM = ProjectManagers[0];
            }
            else
            {
                SelectedPM = em;
            }
        }

        private List<PMScheduleModel> LoadPMSchedule(EmployeeLowResModel selectedpm, DateWrapper firstdate)
        {
            List<PMScheduleModel> pms = new List<PMScheduleModel>();

            List<SchedulingDataDbModel> schedulingdata = SQLAccess.LoadSchedulingDataByDateandPM(firstdate.Value, selectedpm.Id);

            foreach (SchedulingDataDbModel dbmodel in schedulingdata)
            {
                //SubProjectDbModel sub = SQLAccess.LoadSubProjectsBySubProject(dbmodel.PhaseId);
                //ProjectDbModel project = SQLAccess.LoadProjectsById(sub.ProjectId);
                //EmployeeDbModel employee = SQLAccess.LoadEmployeeById(dbmodel.EmployeeId);

                //if (project.ManagerId == selectedpm.Id)
                //{
                PMScheduleIndModel pmnew = new PMScheduleIndModel() { EmployeeName = dbmodel.EmployeeName };
                pmnew.Entries.Add(new SDEntryModel() { Date = DateSummary[0].Value, TimeEntry = dbmodel.Hours1 });
                pmnew.Entries.Add(new SDEntryModel() { Date = DateSummary[1].Value, TimeEntry = dbmodel.Hours2 });
                pmnew.Entries.Add(new SDEntryModel() { Date = DateSummary[2].Value, TimeEntry = dbmodel.Hours3 });
                pmnew.Entries.Add(new SDEntryModel() { Date = DateSummary[3].Value, TimeEntry = dbmodel.Hours4 });
                pmnew.Entries.Add(new SDEntryModel() { Date = DateSummary[4].Value, TimeEntry = dbmodel.Hours5 });
                pmnew.Entries.Add(new SDEntryModel() { Date = DateSummary[5].Value, TimeEntry = dbmodel.Hours6 });
                pmnew.Entries.Add(new SDEntryModel() { Date = DateSummary[6].Value, TimeEntry = dbmodel.Hours7 });
                pmnew.Entries.Add(new SDEntryModel() { Date = DateSummary[7].Value, TimeEntry = dbmodel.Hours8 });

                if (!pms.Any(x => x.PhaseId == dbmodel.PhaseId))
                {
                    PMScheduleModel pmschedmod = new PMScheduleModel()
                    { ProjectNumber = dbmodel.ProjectNumber, PhaseName = dbmodel.PhaseName, PhaseId = dbmodel.PhaseId, ProjectName = dbmodel.ProjectName };

                    pmschedmod.EmployeeSummary.Add(pmnew);
                    pms.Add(pmschedmod);
                }
                else
                {
                    PMScheduleModel pmschedmod = pms.Where(x => x.PhaseId == dbmodel.PhaseId).FirstOrDefault();
                    pmschedmod.EmployeeSummary.Add(pmnew);
                }
                //}
            }

            DateTime Curr = DateTime.Now.AddDays(-1);

            if (DateSummary[0].Value > Curr)
            {
                CanSeeNext = false;
            }
            else
            {
                CanSeeNext = true;
            }

            return pms;
        }

        //private void LoadProjects()
        //{
        //    List<ProjectDbModel> dbprojects = SQLAccess.LoadActiveProjects(true);

        //    ObservableCollection<ProjectModel> members = new ObservableCollection<ProjectModel>();

        //    ProjectModel[] ProjectArray = new ProjectModel[dbprojects.Count];

        //    //Do not include the last layer
        //    Parallel.For(0, dbprojects.Count, i =>
        //    {
        //        ProjectDbModel pdb = dbprojects[i];

        //        //if (pdb.ProjectName != "VACATION" && pdb.ProjectName != "OFFICE" && pdb.ProjectName != "HOLIDAY" && pdb.ProjectName != "SICK")
        //        //{
        //        ProjectModel pm = new ProjectModel(pdb);
        //        EmployeeModel em = Employees.Where(x => x.Id == pdb.ManagerId).FirstOrDefault();

        //        pm.ProjectManager = em;


        //        ProjectArray[i] = pm;
        //        //}
        //    }
        //    );

        //    ProjectArray = ProjectArray.Where(c => c != null).ToArray();
        //    List<ProjectModel> orderedlist = ProjectArray.Where(x => x != null).OrderByDescending(x => x.ProjectNumber).ToList();

        //    ProjectList = new List<ProjectModel>(orderedlist.ToList());
        //    //SelectedSubproject = SelectedProject.SubProjects[0];
        //}
    }
}
