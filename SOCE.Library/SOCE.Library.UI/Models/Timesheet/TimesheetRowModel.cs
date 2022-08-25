using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using SOCE.Library.Db;

namespace SOCE.Library.UI
{
    public class TimesheetRowModel : PropertyChangedBase
    {
        private int _selectedItemIndex = -1;
        public int SelectedItemIndex
        {
            get { return _selectedItemIndex; }
            set
            {
                _selectedItemIndex = value;
                RaisePropertyChanged(nameof(SelectedItemIndex));
            }
        }

        private int _selectedsubItemIndex = -1;
        public int SelectedSubItemIndex
        {
            get { return _selectedsubItemIndex; }
            set
            {
                _selectedsubItemIndex = value;
                RaisePropertyChanged(nameof(SelectedSubItemIndex));
            }
        }

        private ProjectModel _project = new ProjectModel { ProjectName = "" };
        public ProjectModel Project
        {
            get
            {      
                return _project; 
            }
            set
            {
                _project = value;
                CollectSubProjects();
                RaisePropertyChanged(nameof(Project));
            }
        }

        private ObservableCollection<SubProjectModel> _subprojects;
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

        private void SetTotalNew()
        {
            Total = Entries.Sum(i => i.TimeEntry);
        }


        private void CollectSubProjects()
        {
            if (Project == null)
            {
                return;
            }

            int id = Project.Id;

            List<SubProjectDbModel> subdbprojects = SQLAccess.LoadSubProjects(id);

            ObservableCollection<SubProjectModel> members = new ObservableCollection<SubProjectModel>();

            foreach (SubProjectDbModel sdb in subdbprojects)
            {
                members.Add(new SubProjectModel() { Id = sdb.Id, ProjectNumber = Project.ProjectNumber, PointNumber = sdb.PointNumber, Description = sdb.Description, Fee = sdb.Fee });
            }

            SubProjects = members;
        }

    }
}
