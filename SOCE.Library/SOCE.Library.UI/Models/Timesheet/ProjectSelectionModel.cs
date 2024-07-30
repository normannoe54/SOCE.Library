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
    public class ProjectSelectionModel : PropertyChangedBase, ICloneable
    {
        private int _subId { get; set; }
        public int SubId
        {
            get
            {
                return _subId;
            }
            set
            {
                _subId = value;
                RaisePropertyChanged(nameof(SubId));
            }
        }

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

        private string _pointNumber { get; set; }
        public string PointNumber
        {
            get
            {
                return _pointNumber;
            }
            set
            {
                _pointNumber = value;
                RaisePropertyChanged(nameof(PointNumber));
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


        public string CombinedName { get; set; }

        public ProjectSelectionModel()
        {

        }

        public ProjectSelectionModel(ProjectLowResModel pm, SubProjectLowResModel sub)
        {
            SubId = sub.Id;
            ProjectName = pm.ProjectName;
            ProjectNumber = pm.ProjectNumber;
            PointNumber = sub.PointNumber;
            IsActive = Convert.ToBoolean(pm.IsActive);
            CombinedName = $"[{ProjectNumber.ToString()}] {ProjectName}";
        }

        public object Clone()
        {
            return new ProjectSelectionModel()
            {
                SubId = this.SubId,
                ProjectName = this.ProjectName,
                ProjectNumber = this.ProjectNumber,
                IsActive = this.IsActive,
            };
        }
    }
}
