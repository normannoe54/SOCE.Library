using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using SOCE.Library.Db;

namespace SOCE.Library.UI
{
    public class EmployeeScheduleModel : PropertyChangedBase, ICloneable
    {
        private int _id = 0;
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                RaisePropertyChanged(nameof(Id));
            }
        }

        private ObservableCollection<SDEntryModel> _entries = new ObservableCollection<SDEntryModel>();
        public ObservableCollection<SDEntryModel> Entries
        {
            get { return _entries; }
            set
            {
                _entries = value;
                RaisePropertyChanged(nameof(Entries));
            }
        }

        private SDEntryModel _hours1;
        public SDEntryModel Hours1
        {
            get { return _hours1; }
            set
            {
                _hours1 = value;
                RaisePropertyChanged(nameof(Hours1));
            }
        }

        private SDEntryModel _hours2;
        public SDEntryModel Hours2
        {
            get { return _hours2; }
            set
            {
                _hours2 = value;
                RaisePropertyChanged(nameof(Hours2));
            }
        }

        private string _firstName = "";
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                RaisePropertyChanged(nameof(FirstName));
            }
        }

        private string _lastName = "";
        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                RaisePropertyChanged(nameof(LastName));
            }
        }

        private AuthEnum _status;
        public AuthEnum Status
        {
            get { return _status; }
            set
            {
                _status = value;
                RaisePropertyChanged(nameof(Status));
            }
        }

        private DefaultRoleEnum _defaultRole;
        public DefaultRoleEnum DefaultRole
        {
            get { return _defaultRole; }
            set
            {
                _defaultRole = value;
                RaisePropertyChanged(nameof(DefaultRole));
            }
        }

        private double _rate;
        public double Rate
        {
            get { return _rate; }
            set
            {
                _rate = value;
                RaisePropertyChanged(nameof(Rate));
            }
        }

        private double _hoursPerWeek;
        public double HoursPerWeek
        {
            get { return _hoursPerWeek; }
            set
            {
                _hoursPerWeek = value;
                RaisePropertyChanged(nameof(HoursPerWeek));
            }
        }


        private string _fullName;
        public string FullName
        {
            get { return _fullName; }
            set
            {
                _fullName = value;
                RaisePropertyChanged(nameof(FullName));
            }
        }

        private double _totalHours;
        public double TotalHours
        {
            get { return _totalHours; }
            set
            {
                _totalHours = value;
                RaisePropertyChanged(nameof(TotalHours));
            }
        }

        private double _scheduledTotalHours = 0;
        public double ScheduledTotalHours
        {
            get { return _scheduledTotalHours; }
            set
            {
                _scheduledTotalHours = value;
                RaisePropertyChanged(nameof(ScheduledTotalHours));
            }
        }

        public EmployeeScheduleModel()
        { }

        public EmployeeScheduleModel(EmployeeDbModel emdb)
        {
            Id = emdb.Id;
            FirstName = emdb.FirstName;
            LastName = emdb.LastName;
            FullName = FirstName + " " + LastName;
            DefaultRole = ((DefaultRoleEnum)emdb.DefaultRoleId);
            Status = ((AuthEnum)emdb.AuthId);
            Rate = emdb.Rate;
            ScheduledTotalHours = emdb.MondayHours + emdb.TuesdayHours + emdb.WednesdayHours + emdb.ThursdayHours + emdb.FridayHours;
        }

        //public EmployeeScheduleModel(EmployeeDbModel emdb, SDEntryModel hours1, SDEntryModel hours2)
        //{
        //    Id = emdb.Id;
        //    FirstName = emdb.FirstName;
        //    LastName = emdb.LastName;
        //    FullName = FirstName + " " + LastName;
        //    DefaultRole = ((DefaultRoleEnum)emdb.DefaultRoleId);
        //    Status = ((AuthEnum)emdb.AuthId);
        //    Rate = emdb.Rate;
        //    ScheduledTotalHours = emdb.MondayHours + emdb.TuesdayHours + emdb.WednesdayHours + emdb.ThursdayHours + emdb.FridayHours;
        //    Hours1 = hours1;
        //    Hours2 = hours2;
        //}

        public object Clone()
        {
            return new EmployeeScheduleModel()
            {
                Id = this.Id
            };
        }

    }
}
