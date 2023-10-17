using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using SOCE.Library.Db;

namespace SOCE.Library.UI
{
    public class PMScheduleModel : PropertyChangedBase
    {
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

        private double _projectNumber { get; set; }
        public double ProjectNumber
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

        private string _phaseName { get; set; }
        public string PhaseName
        {
            get
            {
                return _phaseName;
            }
            set
            {
                _phaseName = value;
                RaisePropertyChanged(nameof(PhaseName));
            }
        }

        public int PhaseId { get; set; }

        private ObservableCollection<PMScheduleIndModel> _employeeSummary = new ObservableCollection<PMScheduleIndModel>();
        public ObservableCollection<PMScheduleIndModel> EmployeeSummary
        {
            get
            {
                return _employeeSummary;
            }
            set
            {
                _employeeSummary = value;
                RaisePropertyChanged(nameof(EmployeeSummary));
            }
        }

    }
}
