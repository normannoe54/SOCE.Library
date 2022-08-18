using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;

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

        private ProjectUIModel _project = new ProjectUIModel { ProjectName = "" };
        public ProjectUIModel Project
        {
            get
            {      
                return _project; 
            }
            set
            {
                _project = value;
                RaisePropertyChanged(nameof(Project));
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

    }
}
