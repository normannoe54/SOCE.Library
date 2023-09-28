using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using SOCE.Library.Db;

namespace SOCE.Library.UI
{
    public class PMScheduleIndModel : PropertyChangedBase
    {
        private string _employeeName = "";
        public string EmployeeName
        {
            get
            {
                return _employeeName;
            }
            set
            {
                _employeeName = value;
                RaisePropertyChanged(nameof(EmployeeName));
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
    }
}
