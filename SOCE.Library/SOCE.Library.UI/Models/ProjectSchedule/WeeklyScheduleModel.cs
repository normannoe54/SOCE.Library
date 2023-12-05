using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using SOCE.Library.Db;

namespace SOCE.Library.UI
{
    public class WeeklyScheduleModel : PropertyChangedBase
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

        private double _clientNumber { get; set; }
        public double ClientNumber
        {
            get
            {
                return _clientNumber;
            }
            set
            {
                _clientNumber = value;
                RaisePropertyChanged(nameof(ClientNumber));
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

        private string _dueDate { get; set; }
        public string DueDate
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

        private string _pM { get; set; }
        public string PM
        {
            get
            {
                return _pM;
            }
            set
            {
                _pM = value;
                RaisePropertyChanged(nameof(PM));
            }
        }

        private ObservableCollection<SDEntryModel> _entries = new ObservableCollection<SDEntryModel>();
        public ObservableCollection<SDEntryModel> Entries
        {
            get
            {
                return _entries;
            }
            set
            {
                _entries = value;
                RaisePropertyChanged(nameof(Entries));
            }
        }

        private double _total { get; set; }
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


    }
}
