﻿using MaterialDesignThemes.Wpf;
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
    public class ProjectListVM : BaseVM
    {
        public ICommand PrintCommand { get; set; }
        public ICommand ReloadCommand { get; set; }
        public ICommand ClearSearchParamters { get; set; }
        public ICommand SearchCommand { get; set; }

        private ObservableCollection<ClientModel> _clients = new ObservableCollection<ClientModel>();
        public ObservableCollection<ClientModel> Clients
        {
            get { return _clients; }
            set
            {
                _clients = value;
                RaisePropertyChanged(nameof(Clients));
            }
        }

        private ObservableCollection<ProjectModel> _projectList = new ObservableCollection<ProjectModel>();
        public ObservableCollection<ProjectModel> ProjectList
        {
            get { return _projectList; }
            set
            {
                _projectList = value;
                RaisePropertyChanged(nameof(ProjectList));
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

        private EmployeeModel _selectedPM;
        public EmployeeModel SelectedPM
        {
            get { return _selectedPM; }
            set
            {
                _selectedPM = value;
                RaisePropertyChanged(nameof(SelectedPM));
            }
        }

        private ClientModel _selectedClient;
        public ClientModel SelectedClient
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
            this.ReloadCommand = new RelayCommand(this.Reload);
            this.PrintCommand = new RelayCommand(this.Print);
            this.ClearSearchParamters = new RelayCommand(this.ClearInputsandReload);
            this.SearchCommand = new RelayCommand(this.RunSearch);
            Constructor();

        }

        private void Constructor()
        {
            ProjectManagers.Clear();
            ProjectList.Clear();
            LoadProjectManagers();
            LoadClients();
            LoadSchedulingProjects();
        }

        private List<ProjectModel> AllProjects = new List<ProjectModel>();

        private void RunSearch()
        {
            List<ProjectModel> pmnew = AllProjects;

            if (SelectedPM != null)
            {
                pmnew = pmnew.Where(x => x.ProjectManager?.Id == SelectedPM.Id).ToList();
            }

            if (SelectedClient != null)
            {
                pmnew = pmnew.Where(x => x.Client.Id == SelectedClient.Id).ToList();
            }

            if (!String.IsNullOrEmpty(SearchableText))
            {
                pmnew = pmnew.Where(x => x.ProjectName.ToUpper().Contains(_searchableText.ToUpper()) || x.ProjectNumber.ToString().Contains(_searchableText)).ToList();
            }

            ProjectList = new ObservableCollection<ProjectModel>(pmnew);
        }

        private void ClearInputsandReload()
        {
            Reload();
        }

        private void Print()
        {
            //do stuff
            //save down to downloads
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
                    exinst.WriteCell(1, 2, ProjectNumber.ToString());

                    foreach (ProjectModel pm in ProjectList)
                    {
                        List<object> values = new List<object>();
                        string duedatevar = "";

                        if (pm.DueDate != null)
                        {
                            duedatevar = pm.DueDate?.ToString("MM/dd/yyyy");
                        }

                        values.Add(pm.ProjectName);
                        values.Add(pm.ProjectNumber);
                        values.Add(pm.Client.ClientNumber);
                        values.Add(pm.SchedulingValue.ToString());
                        values.Add(duedatevar);
                        values.Add(pm.PercentComplete*0.01);
                        values.Add(pm.ProjectManager != null ? pm.ProjectManager.FullName : "");
                        values.Add(pm.Remarks ?? "");

                        if (rowid == 3)
                        {
                            exinst.WriteRow<object>(rowid,1, values);
                        }
                        else
                        {
                            exinst.InsertRowBelow(rowid -1, values);
                            
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
        }

        private void Reload()
        {
            SearchableText = "";
            SelectedClient = null;
            SelectedPM = null;
            Constructor();
        }

        public void LoadClients()
        {
            List<ClientDbModel> dbclients = SQLAccess.LoadClients();

            List<ClientModel> clients = new List<ClientModel>();
            foreach (ClientDbModel cdbm in dbclients)
            {
                clients.Add(new ClientModel(cdbm));
            }

            List<ClientModel> newclients = clients.OrderBy(x => x.ClientNumber).ToList();

            Clients = new ObservableCollection<ClientModel>(newclients);
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

        private void LoadSchedulingProjects()
        {         
            List<SubProjectDbModel> subdbs = SQLAccess.LoadSubProjectsByActiveScheduling();
            List<ProjectDbModel> projdbs = SQLAccess.LoadProjectsByOnHoldScheduling();
            List<ClientDbModel> dbclients = SQLAccess.LoadClients();
            List<ClientModel> clients = new List<ClientModel>();

            List<MarketDbModel> dbmarkets = SQLAccess.LoadMarkets();

            ObservableCollection<MarketModel> members = new ObservableCollection<MarketModel>();

            foreach (MarketDbModel mdbm in dbmarkets)
            {
                members.Add(new MarketModel(mdbm));
            }

            foreach (ClientDbModel cdbm in dbclients)
            {
                clients.Add(new ClientModel(cdbm));
            }

            List<ProjectDbModel> projectsdb = SQLAccess.LoadActiveProjects(true);
            List<ProjectModel> projects = new List<ProjectModel>();

            foreach (SubProjectDbModel sub in subdbs)
            {
                ProjectDbModel pdb = projectsdb.Where(x => x.Id == sub.ProjectId).FirstOrDefault();

                if (pdb != null)
                {
                    ProjectModel pm = new ProjectModel(pdb, true);
                    EmployeeModel em = ProjectManagers.Where(x => x.Id == pdb.ManagerId).FirstOrDefault();
                    ClientModel cm = clients.Where(x => x.Id == pdb.ClientId).FirstOrDefault();
                    MarketModel mm = members.Where(x => x.Id == pdb.MarketId).FirstOrDefault();

                    pm.ProjectManager = em;
                    pm.Client = cm;
                    pm.Market = mm;

                    if (sub.PointNumber == "CA")
                    {
                        pm.SchedulingValue = SchedulingEnum.CA;
                    }
                    else
                    {
                        pm.SchedulingValue = SchedulingEnum.D;
                    }
                    pm.LoadSubProjects();
                    projects.Add(pm);
                }
            }

            foreach (ProjectDbModel proj in projdbs)
            {
                ProjectModel pm = new ProjectModel(proj, true);
                
                EmployeeModel em = ProjectManagers.Where(x => x.Id == proj.ManagerId).FirstOrDefault();
                ClientModel cm = clients.Where(x => x.Id == proj.ClientId).FirstOrDefault();
                MarketModel mm = members.Where(x => x.Id == proj.MarketId).FirstOrDefault();

                pm.ProjectManager = em;
                pm.Client = cm;
                pm.Market = mm;

                pm.SchedulingValue = SchedulingEnum.H;
                pm.LoadSubProjects();
                projects.Add(pm);

            }

            AllProjects = (projects.OrderBy(x => x.SchedulingValue).ThenBy(y => y.Client.ClientNumber).ThenBy(z => z.ProjectNumber)).ToList();
            ProjectList = new ObservableCollection<ProjectModel>(AllProjects);

            ProjectNumber = ProjectList.Count();
        }
    }
}