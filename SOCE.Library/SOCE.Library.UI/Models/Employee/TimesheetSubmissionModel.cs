using MaterialDesignThemes.Wpf;
using SOCE.Library.Db;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SOCE.Library.UI
{
    public class TimesheetSubmissionModel : PropertyChangedBase
    {
        public int Id { get; set; }

        private EmployeeModel _employee;
        public EmployeeModel Employee
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

        private DateTime _date;
        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
                RaisePropertyChanged(nameof(Date));
            }
        }

        private StatusEnum _status;
        public StatusEnum Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                RaisePropertyChanged(nameof(Status));
            }
        }


        private bool _approved = false;
        public bool Approved
        {
            get
            {
                return _approved;
            }
            set
            {
                _approved = value;
                RaisePropertyChanged(nameof(Approved));
            }
        }

        private bool _missing = false;
        public bool Missing
        {
            get
            {
                return _missing;
            }
            set
            {
                _missing = value;
                RaisePropertyChanged(nameof(Missing));
            }
        }

        public double OTHours { get; set; }

        public double PTOHours { get; set; }

        public double HolidayHours { get; set; }

        public double SickHours { get; set; }

        private double _totalHours { get; set; }
        public double TotalHours
        {
            get
            {
                return _totalHours;
            }
            set
            {
                _totalHours = value;
                RaisePropertyChanged(nameof(TotalHours));
            }
        }


        private bool _selectedCurr;
        public bool SelectedCurr
        {
            get { return _selectedCurr; }
            set
            {
                _selectedCurr = value;
                RaisePropertyChanged(nameof(SelectedCurr));
            }
        }

        private bool _emailRemind;
        public bool EmailRemind
        {
            get { return _emailRemind; }
            set
            {
                _emailRemind = value;
                RaisePropertyChanged(nameof(EmailRemind));
            }
        }

        private bool _submittedEnabled;
        public bool SubmittedEnabled
        {
            get { return _submittedEnabled; }
            set
            {
                _submittedEnabled = value;
                RaisePropertyChanged(nameof(SubmittedEnabled));
            }
        }


        public TimesheetSubmissionModel()
        { }

        public TimesheetSubmissionModel(TimesheetSubmissionDbModel emdb, EmployeeModel employee)
        {
            Id = emdb.Id;
            Employee = employee;
            Date = DateTime.ParseExact(emdb.Date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            TotalHours = emdb.TotalHours;
            PTOHours = emdb.PTOHours;
            SickHours = emdb.SickHours;
            HolidayHours = emdb.HolidayHours;
            OTHours = emdb.OTHours;
            Approved = Convert.ToBoolean(emdb.Approved);
            Missing = false;

            if (Approved)
            {
                Status = StatusEnum.Approved;
            }
            else if (!Approved && !Missing)
            {
                Status = StatusEnum.Submitted;
            }
            else
            {
                Status = StatusEnum.Incomplete;
            }

            EmailRemind = false;
            SubmittedEnabled = true;
        }

        public static TimesheetSubmissionModel Didnotsubmityet(EmployeeModel em)
        {
            TimesheetSubmissionModel tsm = new TimesheetSubmissionModel();
            tsm.TotalHours = 0;
            tsm.PTOHours = 0;
            tsm.SickHours = 0;
            tsm.Employee = em;
            tsm.HolidayHours = 0;
            tsm.Approved = false;
            tsm.Missing = true;
            tsm.EmailRemind = true;
            tsm.SubmittedEnabled = false;
            tsm.Status = StatusEnum.Incomplete;
            return tsm;
        }
    }
}
