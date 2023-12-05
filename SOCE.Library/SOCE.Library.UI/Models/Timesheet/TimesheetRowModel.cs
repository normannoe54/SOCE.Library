using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using SOCE.Library.Db;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SOCE.Library.UI.ViewModels;
using System.Windows.Input;

namespace SOCE.Library.UI
{
    public class TimesheetRowModel : BaseVM, ICloneable
    {
        public ICommand ClearSelectedProjectCommand { get; set; }
        public ICommand SelectedItemChangedCommand { get; set; }

        public bool allowprojectchanges = true;

        private ProjectLowResModel _project;
        public ProjectLowResModel Project
        {
            get
            {
                return _project;
            }
            set
            {
                _project = value;

                if (_project != null)
                {
                    if (allowprojectchanges)
                    {
                        CollectSubProjects();
                    }
                    IsThisEditable = false;
                    ProjectList = BaseProjectList;
                }
                else
                {
                    IsThisEditable = true;
                    SelectedSubproject = null;

                    foreach (TREntryModel ent in Entries)
                    {
                        ent.ReadOnly = true;
                        ent.TimeEntry = 0;
                    }
                }
               
                RaisePropertyChanged(nameof(Project));
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
                if (_selectedSubproject != null)
                {
                    UpdateStatus();
                    ProjectSelected = true;
                }
                RaisePropertyChanged(nameof(SelectedSubproject));
            }
        }

        private ObservableCollection<TREntryModel> _entries = new ObservableCollection<TREntryModel>();
        public ObservableCollection<TREntryModel> Entries
        {
            get
            {
                return _entries;
            }
            set
            {
                _entries = value;
                SetTotalNew();
                RaisePropertyChanged(nameof(Entries));
            }
        }


        private double _total;
        public double Total
        {
            get
            {
                return _total;
            }
            set
            {
                _total = value;
                RaisePropertyChanged(nameof(Total));
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


        private bool _projectSelected = false;
        public bool ProjectSelected
        {
            get
            {
                return _projectSelected;
            }
            set
            {
                _projectSelected = value;
                
                RaisePropertyChanged(nameof(ProjectSelected));
            }
        }

        private TimesheetRowAlertStatus _alertStatus;
        public TimesheetRowAlertStatus AlertStatus
        {
            get
            {
                return _alertStatus;
            }
            set
            {
                _alertStatus = value;
                RaisePropertyChanged(nameof(AlertStatus));
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

        public ObservableCollection<ProjectLowResModel> BaseProjectList { get; set; } = new ObservableCollection<ProjectLowResModel>();

        private void SetTotalNew()
        {
            Total = Entries.Sum(i => i.TimeEntry);
        }

        public TimesheetRowModel()
        {
            Constructor();
        }

        public TimesheetRowModel(List<ProjectLowResModel> projs)
        {
            Constructor();
            BaseProjectList = new ObservableCollection<ProjectLowResModel>(projs);
            ProjectList = BaseProjectList;
        }

        public void Constructor()
        {
            this.SelectedItemChangedCommand = new RelayCommand<string>(this.SelectionCombo);
            this.ClearSelectedProjectCommand = new RelayCommand(this.ClearSelected);
        }

        public void ClearSelected()
        {
            Project = null;
        }

        private void SelectionCombo(string project)
        {
            if (Project == null)
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

        public void CollectSubProjects()
        {
            if (Project == null)
            {
                return;
            }

            int id = Project.Id;

            //1 = active subprojects - doesnt work cuz of saved or submitted previous phases
            List<SubProjectDbModel> subdbprojects = SQLAccess.LoadSubProjectsByProject(id);

            List<SubProjectLowResModel> submodels = FormatSubProjects(subdbprojects, Project.IsActive);
            SubProjects = new ObservableCollection<SubProjectLowResModel>(submodels);

            try
            {
                SelectedSubproject = submodels[0];
            }
            catch
            {
            }

            foreach (TREntryModel ent in Entries)
            {
                ent.ReadOnly = false;
            }
        }

        public static List<SubProjectLowResModel> FormatSubProjects(List<SubProjectDbModel> subs, bool isprojectactive)
        {
            //if (Project == null)
            //{
            //    return;
            //}

            //int id = Project.Id;

            ////1 = active subprojects - doesnt work cuz of saved or submitted previous phases
            //List<SubProjectDbModel> subdbprojects = SQLAccess.LoadSubProjectsByProject(id);
            List<SubProjectLowResModel> members = new List<SubProjectLowResModel>();
            List<SubProjectLowResModel> output = new List<SubProjectLowResModel>();

            //bool projisactive = Project.IsActive;
            foreach (SubProjectDbModel sdb in subs)
            {
                bool subisactive = Convert.ToBoolean(sdb.IsActive);

                if (subisactive || (!isprojectactive))
                {
                    members.Add(new SubProjectLowResModel(sdb));
                }
            }

            //members.Renumber(true);
            int idofscheduleactive = 0;
            bool stuffhappened = false;

            foreach (SubProjectLowResModel sub in members)
            {
                if (sub.IsScheduleActive)
                {
                    stuffhappened = true;
                    idofscheduleactive = members.IndexOf(sub);
                    //members = new ObservableCollection<SubProjectModel>(newlist);
                    break;
                }
            }

            
            if (stuffhappened)
            {
                List<SubProjectLowResModel> newsubs = members.ToList();
                newsubs.MoveItemAtIndexToFront(idofscheduleactive);
                output = new List<SubProjectLowResModel>(newsubs);
            }
            else
            {
                output = members;
            }

            return output;
            
        }

        private void UpdateStatus()
        {
            bool status = SelectedSubproject.IsActive;

            AlertStatus = status ? TimesheetRowAlertStatus.Active : TimesheetRowAlertStatus.Inactive;

        }


        public object Clone()
        {
            ObservableCollection<SubProjectLowResModel> spms = new ObservableCollection<SubProjectLowResModel>();

            foreach (SubProjectLowResModel spm in SubProjects)
            {
                spms.Add((SubProjectLowResModel)spm?.Clone());
            }

            ObservableCollection<TREntryModel> trs = new ObservableCollection<TREntryModel>();
            foreach (TREntryModel tr in Entries)
            {
                trs.Add((TREntryModel)tr?.Clone());
            }

            TimesheetRowModel trm = new TimesheetRowModel(ProjectList.ToList())
            {
                Project = (ProjectLowResModel)this.Project?.Clone(),
                SelectedSubproject = (SubProjectLowResModel)this.SelectedSubproject?.Clone(),
                //SubProjects = spms,
                Entries = trs
            };

            return trm;
        }

    }
}
