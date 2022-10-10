using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Media;
using SOCE.Library.Db;

namespace SOCE.Library.UI
{
    public class EmployeeModel : PropertyChangedBase
    {
        private ObservableCollection<TimesheetSubmissionModel> _timesheetSubmissions;
        public ObservableCollection<TimesheetSubmissionModel> TimesheetSubmissions
        {
            get { return _timesheetSubmissions; }
            set
            {
                _timesheetSubmissions = value;
                RaisePropertyChanged(nameof(TimesheetSubmissions));
            }
        }


        private int _id;
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                RaisePropertyChanged(nameof(Id));
            }
        }

        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                RaisePropertyChanged(nameof(FirstName));
            }
        }

        private string _lastName;
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

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged(nameof(Title));
            }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                RaisePropertyChanged(nameof(Email));
            }
        }

        private string _emailForward;
        public string EmailForward
        {
            get { return _emailForward; }
            set
            {
                _emailForward = value;
                Email = EmailForward + "@shirkodonovan.com";
                RaisePropertyChanged(nameof(EmailForward));
            }
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set
            {
                _phoneNumber = value;
                RaisePropertyChanged(nameof(PhoneNumber));
            }
        }

        private string _extension;
        public string Extension
        {
            get { return _extension; }
            set
            {
                _extension = value;
                RaisePropertyChanged(nameof(Extension));
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

        private double _ptoBalance;
        public double PTOBalance
        {
            get { return _ptoBalance; }
            set
            {
                _ptoBalance = value;
                RaisePropertyChanged(nameof(PTOBalance));
            }
        }

        private double _totalPTO;
        public double TotalPTO
        {
            get { return _totalPTO; }
            set
            {
                _totalPTO = value;
                RaisePropertyChanged(nameof(TotalPTO));
            }
        }

        private double _ptoLeft;
        public double PTOLeft
        {
            get { return _ptoLeft; }
            set
            {
                _ptoLeft = value;
                RaisePropertyChanged(nameof(PTOLeft));
            }
        }

        private double _sickBalance;
        public double SickBalance
        {
            get { return _sickBalance; }
            set
            {
                _sickBalance = value;
                RaisePropertyChanged(nameof(SickBalance));
            }
        }

        private double _totalSick;
        public double TotalSick
        {
            get { return _totalSick; }
            set
            {
                _totalSick = value;
                RaisePropertyChanged(nameof(TotalSick));
            }
        }

        private double _sickLeft;
        public double SickLeft
        {
            get { return _sickLeft; }
            set
            {
                _sickLeft = value;
                RaisePropertyChanged(nameof(SickLeft));
            }
        }

        private double _holidayBalance;
        public double HolidayBalance
        {
            get { return _holidayBalance; }
            set
            {
                _holidayBalance = value;
                RaisePropertyChanged(nameof(HolidayBalance));
            }
        }

        private double _totalHoliday;
        public double TotalHoliday
        {
            get { return _totalHoliday; }
            set
            {
                _totalHoliday = value;
                RaisePropertyChanged(nameof(TotalHoliday));
            }
        }

        private double _holidayLeft;
        public double HolidayLeft
        {
            get { return _holidayLeft; }
            set
            {
                _holidayLeft = value;
                RaisePropertyChanged(nameof(HolidayLeft));
            }
        }

        private double _totalOT;
        public double TotalOT
        {
            get { return _totalOT; }
            set
            {
                _totalOT = value;
                RaisePropertyChanged(nameof(TotalOT));
            }
        }
        private bool _canEditorDelete = true;
        public bool CanEditorDelete
        {
            get { return _canEditorDelete; }
            set
            {
                _canEditorDelete = value;
                RaisePropertyChanged(nameof(CanEditorDelete));
            }
        }

        private bool _canEditRate = true;
        public bool CanEditRate
        {
            get { return _canEditRate; }
            set
            {
                _canEditRate = value;
                RaisePropertyChanged(nameof(CanEditRate));
            }
        }

        private bool _canEditPTO = true;
        public bool CanEditPTO
        {
            get { return _canEditPTO; }
            set
            {
                _canEditPTO = value;
                RaisePropertyChanged(nameof(CanEditPTO));
            }
        }


        private bool _rateVisible = true;
        public bool RateVisible
        {
            get { return _rateVisible; }
            set
            {
                _rateVisible = value;
                RaisePropertyChanged(nameof(RateVisible));
            }
        }

        private bool _editFieldState = true;
        public bool EditFieldState
        {
            get { return _editFieldState; }
            set
            {
                if (!_editFieldState && value)
                {
                    UpdateEmployee();
                }
                _editFieldState = value;
                ComboFieldState = !_editFieldState;

                if (!_editFieldState && CanEditPTO)
                {
                    EditPTOState = false;
                }
                else
                {
                    EditPTOState = true;
                }

                RaisePropertyChanged(nameof(EditFieldState));
            }
        }

        private bool _editPTOState = true;
        public bool EditPTOState
        {
            get { return _editPTOState; }
            set
            {
                if (!_editPTOState && value)
                {
                    UpdateEmployee();
                }
                _editPTOState = value;
                RaisePropertyChanged(nameof(EditPTOState));
            }
        }

        private bool _editRateState = true;
        public bool EditRateState
        {
            get { return _editRateState; }
            set
            {
                if (!_editRateState && value)
                {
                    UpdateEmployee();
                }
                _editRateState = value;
                RaisePropertyChanged(nameof(EditRateState));
            }
        }

        private bool _enabledforView;
        public bool EnabledforView
        {
            get { return _enabledforView; }
            set
            {
                _enabledforView = value;
                RaisePropertyChanged(nameof(TotalOT));
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

        private bool _comboFieldState;
        public bool ComboFieldState
        {
            get { return _comboFieldState; }
            set
            {
                _comboFieldState = value;
                RaisePropertyChanged(nameof(ComboFieldState));
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

        private bool IsEditable;

        private bool _canExpand = false;
        public bool CanExpand
        {
            get
            {
                return _canExpand;
            }
            set
            {
                if (IsEditable)
                {
                    _canExpand = value;
                    RaisePropertyChanged(nameof(CanExpand));
                }
            }
        }

        public EmployeeModel()
        { }

        public EmployeeModel(EmployeeDbModel emdb)
        {
            Id = emdb.Id;
            FirstName = emdb.FirstName;
            LastName = emdb.LastName;
            FullName = FirstName + " " + LastName;
            
            Status = ((AuthEnum)emdb.AuthId);
            Title = emdb.Title;
            Email = emdb.Email;

            int index = emdb.Email.IndexOf("@");
            if (index != -1)
            {
                EmailForward = emdb.Email.Substring(0, index);
            }

            PhoneNumber = emdb.PhoneNumber;
            Extension = emdb.Extension;
            Rate = emdb.Rate;
            PTOBalance = emdb.PTOHours;
            HolidayBalance = emdb.HolidayHours;
            SickBalance = emdb.SickHours;

            double ptospent = 0;
            double otspent = 0;
            double sickspent = 0;
            double holidayspent = 0;
            //load timesheet submissions
            List<TimesheetSubmissionModel> tsm = new List<TimesheetSubmissionModel>();

            List<TimesheetSubmissionDbModel> dbdata = SQLAccess.LoadTimesheetSubmissionByEmployee(Id);
            foreach(TimesheetSubmissionDbModel tsmdb in dbdata)
            {
                tsm.Add(new TimesheetSubmissionModel(tsmdb));

                ptospent += tsmdb.PTOHours;
                otspent += tsmdb.OTHours;
                sickspent += tsmdb.SickHours;
                holidayspent += tsmdb.HolidayHours;

            }
            TimesheetSubmissions = new ObservableCollection<TimesheetSubmissionModel>(tsm);
            TotalPTO = ptospent;
            TotalOT = otspent;
            TotalSick = sickspent;

            PTOLeft = PTOBalance - TotalPTO;
            SickLeft = SickBalance - TotalSick;
            HolidayLeft = HolidayBalance - TotalHoliday;
        }


        public void SetEmployeeModelfromUser(EmployeeModel currentuser)
        {
            switch (currentuser.Status)
            {
                case AuthEnum.Admin:
                    RateVisible = true;
                    IsEditable = true;
                    CanEditorDelete = true;
                    CanEditRate = true;
                    CanEditPTO = true;
                    break;
                case AuthEnum.Principal:
                    RateVisible = true;
                    IsEditable = true;
                    CanEditorDelete = false;
                    CanEditRate = true;
                    CanEditPTO = false;
                    break;
                case AuthEnum.PM:
                    RateVisible = true;
                    IsEditable = false;
                    CanEditorDelete = false;
                    CanEditRate = false;
                    CanEditPTO = false;

                    break;
                case AuthEnum.Standard:
                    RateVisible = false;
                    IsEditable = false;
                    CanEditorDelete = false;
                    CanEditRate = false;
                    CanEditPTO = false;

                    break;
                default:
                    RateVisible = false;
                    IsEditable = false;
                    CanEditorDelete = false;
                    CanEditRate = false;
                    CanEditPTO = false;

                    break;
            }

            if (Id == currentuser.Id)
            {
                IsEditable = true;
            }
        }

        public void UpdateEmployee()
        {
            EmployeeDbModel employee = new EmployeeDbModel()
            { Id = Id,
                FirstName = FirstName,
                LastName = LastName,
                AuthId = (int)Status,
                Title = Title,
                Email = Email,
                PhoneNumber = PhoneNumber, 
                Extension = Extension,
                Rate= Rate,
                PTOHours = PTOBalance,
                HolidayHours = HolidayBalance,
                SickHours = SickBalance};

            SQLAccess.UpdateEmployee(employee);
        }

        public override bool Equals(object obj)
        {
            EmployeeModel em = (EmployeeModel)obj;

            if (em == null)
            {
                return false;
            }

            return em != null && Id == em.Id && FirstName == em.FirstName && LastName == em.LastName;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                hash = hash * 23 + Id.GetHashCode();
                hash = hash * 23 + FirstName.GetHashCode();
                hash = hash * 23 + LastName.GetHashCode();
                return hash;
            }
        }

    }
}
