using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using SOCE.Library.Db;

namespace SOCE.Library.UI
{
    public class HourSummaryIndModel : PropertyChangedBase
    {
        private EmployeeLowResModel _employee;
        public EmployeeLowResModel Employee
        {
            get
            {
                return _employee;
            }
            set
            {
                _employee = value;
                RaisePropertyChanged(nameof(Employee));
            }
        }

        private ObservableCollection<HourEntryModel> _entries = new ObservableCollection<HourEntryModel>();
        public ObservableCollection<HourEntryModel> Entries
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
