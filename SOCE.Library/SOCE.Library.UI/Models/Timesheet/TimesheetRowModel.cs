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

namespace SOCE.Library.UI
{
    public class TimesheetRowModel : PropertyChangedBase, ICloneable
    {

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
                //set selected item

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

            //1 = active subprojects - doesnt work cuz of saved or submitted previous phases
            List<SubProjectDbModel> subdbprojects = SQLAccess.LoadSubProjectsByProject(id);

            ObservableCollection<SubProjectModel> members = new ObservableCollection<SubProjectModel>();

            foreach (SubProjectDbModel sdb in subdbprojects)
            {
                members.Add(new SubProjectModel(sdb));
            }

            SubProjects = members;
        }


        public object Clone()
        {
            ObservableCollection<SubProjectModel> spms = new ObservableCollection<SubProjectModel>();
            foreach(SubProjectModel spm in SubProjects)
            {
                spms.Add((SubProjectModel)spm.Clone());
            }

            ObservableCollection<TREntryModel> trs = new ObservableCollection<TREntryModel>();
            foreach (TREntryModel tr in Entries)
            {
                trs.Add((TREntryModel)tr.Clone());
            }


            return new TimesheetRowModel() { Project = (ProjectModel)this.Project.Clone(),
                SelectedSubproject = (SubProjectModel)this.SelectedSubproject.Clone(),
                SubProjects = spms,
                Entries = trs };
        }

    }
}
