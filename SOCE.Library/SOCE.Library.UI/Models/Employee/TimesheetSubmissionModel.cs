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

                Icon = _approved ? MaterialDesignThemes.Wpf.PackIconKind.CheckCircleOutline: MaterialDesignThemes.Wpf.PackIconKind.AlertOctagonOutline;
                Iconcolor = _approved ? Brushes.Green : Brushes.Orange;
                StatusTooltip = _approved ? "Approved" : "In Progress";
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
                SubmittedIcon = _missing ? MaterialDesignThemes.Wpf.PackIconKind.QuestionMark : MaterialDesignThemes.Wpf.PackIconKind.CheckCircleOutline;
                SubmittedIconcolor = _missing ? Brushes.Purple : Brushes.Green;
                SubmittedStatusTooltip = _missing ? "Not Submitted" : "Submitted";
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

        private PackIconKind _icon;
        public PackIconKind Icon
        {
            get { return _icon; }
            set
            {
                _icon = value;
                RaisePropertyChanged(nameof(Icon));
            }
        }

        private Brush _iconcolor;
        public Brush Iconcolor
        {
            get { return _iconcolor; }
            set
            {
                _iconcolor = value;
                RaisePropertyChanged(nameof(Iconcolor));
            }
        }

        private Brush _submittedIconcolor;
        public Brush SubmittedIconcolor
        {
            get { return _submittedIconcolor; }
            set
            {
                _submittedIconcolor = value;
                RaisePropertyChanged(nameof(SubmittedIconcolor));
            }
        }

        private PackIconKind _submittedIcon;
        public PackIconKind SubmittedIcon
        {
            get { return _submittedIcon; }
            set
            {
                _submittedIcon = value;
                RaisePropertyChanged(nameof(SubmittedIcon));
            }
        }


        private string _statusTooltip;
        public string StatusTooltip
        {
            get { return _statusTooltip; }
            set
            {
                _statusTooltip = value;
                RaisePropertyChanged(nameof(StatusTooltip));
            }
        }

        private string _submittedStatusTooltip;
        public string SubmittedStatusTooltip
        {
            get { return _submittedStatusTooltip; }
            set
            {
                _submittedStatusTooltip = value;
                RaisePropertyChanged(nameof(StatusTooltip));
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
            Approved = Convert.ToBoolean(emdb.Approved);
            Missing = false;
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
            return tsm;
        }
    }
}
