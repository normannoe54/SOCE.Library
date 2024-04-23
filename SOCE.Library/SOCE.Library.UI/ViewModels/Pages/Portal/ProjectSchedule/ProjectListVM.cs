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
    public class ProjectListVM : BaseVM
    {
        public ICommand PrintCommand { get; set; }
        public ICommand ReloadCommand { get; set; }
        public ICommand ClearSearchParamters { get; set; }
        public ICommand SearchCommand { get; set; }

        public ICommand ArchiveProject { get; set; }

        private ObservableCollection<ClientLowResModel> _clients = new ObservableCollection<ClientLowResModel>();
        public ObservableCollection<ClientLowResModel> Clients
        {
            get { return _clients; }
            set
            {
                _clients = value;
                RaisePropertyChanged(nameof(Clients));
            }
        }

        private ObservableCollection<ProjectListModel> _projectList = new ObservableCollection<ProjectListModel>();
        public ObservableCollection<ProjectListModel> ProjectList
        {
            get { return _projectList; }
            set
            {
                _projectList = value;
                RaisePropertyChanged(nameof(ProjectList));
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

        private double _projectNumber = 0;
        public double ProjectNumber
        {
            get { return _projectNumber; }
            set
            {
                _projectNumber = value;
                RaisePropertyChanged(nameof(ProjectNumber));
            }
        }

        private EmployeeLowResModel _selectedPM;
        public EmployeeLowResModel SelectedPM
        {
            get { return _selectedPM; }
            set
            {
                _selectedPM = value;
                RaisePropertyChanged(nameof(SelectedPM));
            }
        }

        private ClientLowResModel _selectedClient;
        public ClientLowResModel SelectedClient
        {
            get { return _selectedClient; }
            set
            {
                _selectedClient = value;
                RaisePropertyChanged(nameof(SelectedClient));
            }
        }

        private string _searchableText { get; set; }
        public string SearchableText
        {
            get { return _searchableText; }
            set
            {
                _searchableText = value;
                RaisePropertyChanged(nameof(SearchableText));
            }
        }

        public ProjectListVM()
        {
            this.ArchiveProject = new RelayCommand<ProjectListModel>(this.AskArchiveProject);
            this.ReloadCommand = new RelayCommand(this.Reload);
            this.PrintCommand = new RelayCommand(this.Print);
            this.ClearSearchParamters = new RelayCommand(this.ClearInputsandReload);
            this.SearchCommand = new RelayCommand(this.RunSearch);
            Constructor();

        }

        private async void AskArchiveProject(ProjectListModel plm)
        {
            YesNoView ynv = new YesNoView();
            YesNoVM ynvm = new YesNoVM(plm);
            ynv.DataContext = ynvm;

            var result2 = await DialogHost.Show(ynv, "RootDialog");
            ynvm = ynv.DataContext as YesNoVM;

            if (ynvm.Result)
            {
                SQLAccess.UpdateActiveProject(plm.ProjectId, 0);

                if ((SearchableText == "") && (SelectedClient == null) && (SelectedPM == null))
                {
                    Reload();
                }
                else
                {

                    ClientLowResModel clrsm = SelectedClient;
                    EmployeeLowResModel elrm = SelectedPM;
                    Constructor();

                    if (clrsm != null)
                    {
                        ClientLowResModel foundclrm = Clients.Where(x => x.Id == clrsm.Id).FirstOrDefault();
                        SelectedClient = foundclrm;
                    }

                    if (elrm != null)
                    {
                        EmployeeLowResModel foundelrm = ProjectManagers.Where(x => x.Id == elrm.Id).FirstOrDefault();
                        SelectedPM = foundelrm;
                    }

                    RunSearch();
                }
            }
        }

        private void Constructor()
        {
            ProjectManagers.Clear();
            ProjectList.Clear();
            LoadProjectManagers();
            LoadClients();
            LoadSchedulingProjects();
        }

        private List<ProjectListModel> AllProjects = new List<ProjectListModel>();

        private async void RunSearch()
        {
            //async here
            CoreAI CurrentPage = IoCCore.Application as CoreAI;
            CurrentPage.MakeBlurry();
            await Task.Run(() => Task.Delay(600));
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                List<ProjectListModel> pmnew = AllProjects;

                if (SelectedPM != null)
                {
                    pmnew = pmnew.Where(x => x.ProjectManager?.Id == SelectedPM.Id).ToList();
                }

                if (SelectedClient != null)
                {
                    pmnew = pmnew.Where(x => x.ClientNumber == SelectedClient.ClientNumber).ToList();
                }

                if (!String.IsNullOrEmpty(SearchableText))
                {
                    pmnew = pmnew.Where(x => x.ProjectName.ToUpper().Contains(_searchableText.ToUpper()) || x.ProjectNumber.ToString().Contains(_searchableText)).ToList();
                }

                ProjectList = new ObservableCollection<ProjectListModel>(pmnew);
            }));
            await Task.Run(() => Task.Delay(600));
            CurrentPage.MakeClear();
        }

        private async void ClearInputsandReload()
        {
            //async here
            CoreAI CurrentPage = IoCCore.Application as CoreAI;
            CurrentPage.MakeBlurry();
            await Task.Run(() => Task.Delay(600));
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Reload()));
            await Task.Run(() => Task.Delay(600));
            CurrentPage.MakeClear();

        }

        private async void Print()
        {
            //async here
            CoreAI CurrentPage = IoCCore.Application as CoreAI;
            CurrentPage.MakeBlurry();
            await Task.Run(() => Task.Delay(600));
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {

                try
                {
                    string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    string pathDownload = Path.Combine(pathUser, "Downloads\\ProjectList.xlsx");
                    File.WriteAllBytes(pathDownload, Properties.Resources.ProjectList);
                    Excel.Excel exinst = new Excel.Excel(pathDownload);

                    Thread.Sleep(200);
                    int rowid = 3;
                    if (ProjectList.Count > 0)
                    {
                        var monday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);
                        exinst.WriteCell(1, 2, monday.ToString("MM/dd/yyyy"));

                        foreach (ProjectListModel pm in ProjectList)
                        {
                            List<object> values = new List<object>();
                            string duedatevar = "";

                            if (pm.DueDate != null)
                            {
                                duedatevar = pm.DueDate?.ToString("MM/dd/yyyy");
                            }

                            values.Add(pm.ProjectName);
                            values.Add(pm.ProjectNumber);
                            values.Add(pm.ClientNumber);
                            values.Add(pm.SchedulingValue.ToString());
                            values.Add(duedatevar);
                            values.Add(pm.PercentComplete * 0.01);
                            values.Add(pm.ProjectManager != null ? pm.ProjectManager.FullName : "");
                            values.Add(pm.Remarks ?? "");

                            if (rowid == 3)
                            {
                                exinst.WriteRow<object>(rowid, 1, values);
                            }
                            else
                            {
                                exinst.InsertRowBelow(rowid - 1, values);

                            }
                            rowid++;

                        }

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

        private async void Reload()
        {
            CoreAI CurrentPage = IoCCore.Application as CoreAI;
            CurrentPage.MakeBlurry();
            await Task.Run(() => Task.Delay(600));
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                SearchableText = "";
                SelectedClient = null;
                SelectedPM = null;
                Constructor();
            }));
            await Task.Run(() => Task.Delay(600));
            CurrentPage.MakeClear();
        }

        public void LoadClients()
        {
            List<ClientDbModel> dbclients = SQLAccess.LoadClients();

            List<ClientLowResModel> clients = new List<ClientLowResModel>();
            foreach (ClientDbModel cdbm in dbclients)
            {
                clients.Add(new ClientLowResModel(cdbm));
            }

            List<ClientLowResModel> newclients = clients.OrderBy(x => x.ClientNumber).ToList();

            Clients = new ObservableCollection<ClientLowResModel>(newclients);
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

        private void LoadSchedulingProjects()
        {
            List<SubProjectDbModel> subdbs = SQLAccess.LoadSubProjectsByActiveScheduling();
            List<ProjectDbModel> projdbs = SQLAccess.LoadProjectsByOnHoldScheduling();
            List<ProjectDbModel> projectsdb = SQLAccess.LoadActiveProjects(true);

            List<ProjectListModel> projects = new List<ProjectListModel>();

            foreach (SubProjectDbModel sub in subdbs)
            {
                ProjectDbModel pdb = projectsdb.Where(x => x.Id == sub.ProjectId).FirstOrDefault();

                if (pdb != null)
                {
                    ProjectListModel pm = new ProjectListModel(pdb);
                    EmployeeLowResModel em = ProjectManagers.Where(x => x.Id == pdb.ManagerId).FirstOrDefault();
                    ClientLowResModel cm = Clients.Where(x => x.Id == pdb.ClientId).FirstOrDefault();

                    pm.ProjectManager = em;
                    pm.ClientNumber = cm.ClientNumber;

                    if (sub.PointNumber == "CA")
                    {
                        pm.SchedulingValue = SchedulingEnum.CA;
                    }
                    else
                    {
                        pm.SchedulingValue = SchedulingEnum.D;
                    }
                    //pm.LoadSubProjects();
                    projects.Add(pm);
                }
            }

            foreach (ProjectDbModel proj in projdbs)
            {

                if (proj.Id == 989)
                {
                    bool test = false;
                }

                ProjectListModel pm = new ProjectListModel(proj);

                EmployeeLowResModel em = ProjectManagers.Where(x => x.Id == proj.ManagerId).FirstOrDefault();
                ClientLowResModel cm = Clients.Where(x => x.Id == proj.ClientId).FirstOrDefault();

                pm.ProjectManager = em;
                pm.ClientNumber = cm.ClientNumber;

                pm.SchedulingValue = SchedulingEnum.H;
                //pm.LoadSubProjects();
                projects.Add(pm);

            }

            AllProjects = (projects.OrderBy(x => x.SchedulingValue).ThenBy(y => y.ClientNumber).ThenBy(z => z.ProjectNumber)).ToList();
            ProjectList = new ObservableCollection<ProjectListModel>(AllProjects);
            ProjectNumber = ProjectList.Count();
        }
    }
}
