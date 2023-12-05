using MaterialDesignThemes.Wpf;
using Microsoft.WindowsAPICodePack.Dialogs;
using SOCE.Library.Db;
using SOCE.Library.UI.ViewModels;
using SOCE.Library.UI.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SOCE.Library.UI
{
    public class ProjectLowResModel : PropertyChangedBase, ICloneable
    {
        private int _id { get; set; }
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                RaisePropertyChanged(nameof(Id));
            }
        }

        private int _managerId { get; set; }
        public int ManagerId
        {
            get
            {
                return _managerId;
            }
            set
            {
                _managerId = value;
                RaisePropertyChanged(nameof(ManagerId));
            }
        }

        private double _percentComplete { get; set; }
        public double PercentComplete
        {
            get
            {
                return _percentComplete;
            }
            set
            {
                _percentComplete = value;
                RaisePropertyChanged(nameof(PercentComplete));
            }
        }

        private DateTime? _dueDate { get; set; }
        public DateTime? DueDate
        {
            get
            {
                return _dueDate;
            }
            set
            {
                _dueDate = value;
                RaisePropertyChanged(nameof(DueDate));
            }
        }
        //private ObservableCollection<SubProjectLowResModel> _subprojects { get; set; } = new ObservableCollection<SubProjectLowResModel>();
        //public ObservableCollection<SubProjectLowResModel> SubProjects
        //{
        //    get
        //    {
        //        return _subprojects;
        //    }
        //    set
        //    {
        //        _subprojects = value;
        //        RaisePropertyChanged(nameof(SubProjects));
        //    }
        //}

        private string _projectName { get; set; }
        public string ProjectName
        {
            get
            {
                return _projectName;
            }
            set
            {
                _projectName = value;
                RaisePropertyChanged(nameof(ProjectName));
            }
        }

        private int _projectNumber { get; set; }
        public int ProjectNumber
        {
            get
            {
                return _projectNumber;
            }
            set
            {
                _projectNumber = value;
                RaisePropertyChanged(nameof(ProjectNumber));
            }
        }

        private bool _isActive = false;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                RaisePropertyChanged(nameof(IsActive));
            }
        }

        public double Fee;

        public ProjectLowResModel()
        {

        }

        public ProjectLowResModel(ProjectDbModel pm)
        {
            Id = pm.Id;
            ProjectName = pm.ProjectName;
            ProjectNumber = pm.ProjectNumber;
            IsActive = Convert.ToBoolean(pm.IsActive);
            Fee = pm.Fee;
        }

        //public void CollectSubProjects()
        //{
        //    SubProjects.Clear();

        //    int id = Id;

        //    //1 = active subprojects - doesnt work cuz of saved or submitted previous phases
        //    List<SubProjectDbModel> subdbprojects = SQLAccess.LoadSubProjectsByProject(id);

        //    ObservableCollection<SubProjectLowResModel> members = new ObservableCollection<SubProjectLowResModel>();

        //    bool projisactive = IsActive;
        //    foreach (SubProjectDbModel sdb in subdbprojects)
        //    {
        //        bool subisactive = Convert.ToBoolean(sdb.IsActive);

        //        if (subisactive || (!projisactive))
        //        {
        //            members.Add(new SubProjectLowResModel(sdb));
        //        }
        //    }

        //    members.Renumber(true);
        //    int idofscheduleactive = 0;
        //    bool stuffhappened = false;

        //    foreach (SubProjectLowResModel sub in members)
        //    {
        //        if (sub.IsScheduleActive)
        //        {
        //            stuffhappened = true;
        //            idofscheduleactive = members.IndexOf(sub);
        //            //members = new ObservableCollection<SubProjectModel>(newlist);
        //            break;
        //        }
        //    }


        //    if (stuffhappened)
        //    {
        //        List<SubProjectLowResModel> newsubs = members.ToList();
        //        newsubs.MoveItemAtIndexToFront(idofscheduleactive);
        //        SubProjects = new ObservableCollection<SubProjectLowResModel>(newsubs);
        //    }
        //    else
        //    {
        //        SubProjects = members;
        //    }

        //}

        public object Clone()
        {
            return new ProjectLowResModel()
            {
                Id = this.Id,
                ProjectName = this.ProjectName,
                ProjectNumber = this.ProjectNumber,
                IsActive = this.IsActive,
            };
        }
    }
}
