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

        private ProjectModel _project;
        public ProjectModel Project
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
                    CollectSubProjects();
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

        private void SetTotalNew()
        {
            Total = Entries.Sum(i => i.TimeEntry);
        }

        public TimesheetRowModel()
        {
        }

        public TimesheetRowModel(List<ProjectModel> projs)
        {
            this.SelectedItemChangedCommand = new RelayCommand<string>(this.SelectionCombo);
            this.ClearSelectedProjectCommand = new RelayCommand(this.ClearSelected);
            BaseProjectList = new ObservableCollection<ProjectModel> (projs);
            ProjectList = BaseProjectList;
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

                    ProjectList = new ObservableCollection<ProjectModel>(BaseProjectList.Where(x => (x.ProjectName.ToUpper() + x.ProjectNumber.ToString()).Contains(project.ToUpper())));
                }
                else
                {
                    ProjectList = BaseProjectList;
                }

                //SelectedProject = null;
            }
        }

        private void CollectSubProjects()
        {
            if (Project == null)
            {
                return;
            }

            int id = Project.Id;

            //1 = active subprojects - doesnt work cuz of saved or submitted previous phases
            List<SubProjectDbModel> subdbprojects = SQLAccess.LoadSubProjectsByProject(id);

            ObservableCollection<SubProjectModel> members = new ObservableCollection<SubProjectModel>();

            bool projisactive = Project.IsActive;
            foreach (SubProjectDbModel sdb in subdbprojects)
            {
                bool subisactive = Convert.ToBoolean(sdb.IsActive);

                if (subisactive || (!projisactive))
                {
                    members.Add(new SubProjectModel(sdb));
                }
            }

            members.Renumber(true);
            int idofscheduleactive = 0;
            bool stuffhappened = false;

            foreach (SubProjectModel sub in members)
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
                List<SubProjectModel> newsubs = members.ToList();
                newsubs.MoveItemAtIndexToFront(idofscheduleactive);
                SubProjects = new ObservableCollection<SubProjectModel>(newsubs);
            }
            else
            {
                SubProjects = members;
            }

            try
            {
                SelectedSubproject = SubProjects[0];
            }
            catch
            {
            }

            ProjectSelected = true;
            foreach (TREntryModel ent in Entries)
            {
                ent.ReadOnly = false;
            }
        }



        public object Clone()
        {
            ObservableCollection<SubProjectModel> spms = new ObservableCollection<SubProjectModel>();

            foreach (SubProjectModel spm in SubProjects)
            {
                spms.Add((SubProjectModel)spm?.Clone());
            }

            ObservableCollection<TREntryModel> trs = new ObservableCollection<TREntryModel>();
            foreach (TREntryModel tr in Entries)
            {
                trs.Add((TREntryModel)tr?.Clone());
            }

            TimesheetRowModel trm = new TimesheetRowModel(ProjectList.ToList())
            {
                Project = (ProjectModel)this.Project?.Clone(),
                SelectedSubproject = (SubProjectModel)this.SelectedSubproject?.Clone(),
                SubProjects = spms,
                Entries = trs
            };

            return trm;
        }

    }
}
