using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SOCE.Library.Db;

namespace SOCE.Library.UI
{
    public class EmployeeModel : PropertyChangedBase, ICloneable
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

        private string _firstName ="";
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

        private double _pTORate;
        public double PTORate
        {
            get { return _pTORate; }
            set
            {
                _pTORate = value;
                RaisePropertyChanged(nameof(PTORate));
            }
        }

        private double _pTOUsed;
        public double PTOUsed
        {
            get { return _pTOUsed; }
            set
            {
                _pTOUsed = value;
                RaisePropertyChanged(nameof(PTOUsed));
            }
        }

        private double _pTOEarned;
        public double PTOEarned
        {
            get { return _pTOEarned; }
            set
            {
                _pTOEarned = value;
                RaisePropertyChanged(nameof(PTOEarned));
            }
        }

        private double _pTOCarryover;
        public double PTOCarryover
        {
            get { return _pTOCarryover; }
            set
            {
                _pTOCarryover = value;
                RaisePropertyChanged(nameof(PTOCarryover));
            }
        }

        private double _pTOHours;
        public double PTOHours
        {
            get { return _pTOHours; }
            set
            {
                _pTOHours = value;
                RaisePropertyChanged(nameof(PTOHours));
            }
        }

        private double _sickRate;
        public double SickRate
        {
            get { return _sickRate; }
            set
            {
                _sickRate = value;
                RaisePropertyChanged(nameof(SickRate));
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

        //private double _sickCarryover;
        //public double SickCarryover
        //{
        //    get { return _sickCarryover; }
        //    set
        //    {
        //        _sickCarryover = value;
        //        RaisePropertyChanged(nameof(SickCarryover));
        //    }
        //}

        private double _sickUsed;
        public double SickUsed
        {
            get { return _sickUsed; }
            set
            {
                _sickUsed = value;
                RaisePropertyChanged(nameof(SickUsed));
            }
        }

        private double _sickEarned;
        public double SickEarned
        {
            get { return _sickEarned; }
            set
            {
                _sickEarned = value;
                RaisePropertyChanged(nameof(SickEarned));
            }
        }

        private double _sickHours;
        public double SickHours
        {
            get { return _sickHours; }
            set
            {
                _sickHours = value;
                RaisePropertyChanged(nameof(SickHours));
            }
        }

        private double _holidayHours;
        public double HolidayHours
        {
            get { return _holidayHours; }
            set
            {
                _holidayHours = value;
                RaisePropertyChanged(nameof(HolidayHours));
            }
        }

        private double _holidayUsed;
        public double HolidayUsed
        {
            get { return _holidayUsed; }
            set
            {
                _holidayUsed = value;
                RaisePropertyChanged(nameof(HolidayUsed));
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

        //private bool _canEditRate = true;
        //public bool CanEditRate
        //{
        //    get { return _canEditRate; }
        //    set
        //    {
        //        _canEditRate = value;
        //        RaisePropertyChanged(nameof(CanEditRate));
        //    }
        //}

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


        private bool _rateVisible = false;
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

        //private bool _editRateState = true;
        //public bool EditRateState
        //{
        //    get { return _editRateState; }
        //    set
        //    {
        //        if (!_editRateState && value)
        //        {
        //            UpdateEmployee();
        //        }
        //        _editRateState = value;
        //        RaisePropertyChanged(nameof(EditRateState));
        //    }
        //}


        private bool _canReviewTimesheet;
        public bool CanReviewTimesheet
        {
            get { return _canReviewTimesheet; }
            set
            {
                _canReviewTimesheet = value;
                RaisePropertyChanged(nameof(CanReviewTimesheet));
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

        private double _mondayHours;
        public double MondayHours
        {
            get { return _mondayHours; }
            set
            {
                _mondayHours = value;
                if (IsEditable)
                {
                    TotalHours = MondayHours + TuesdayHours + WednesdayHours + ThursdayHours + FridayHours;
                }
                RaisePropertyChanged(nameof(MondayHours));
            }
        }

        private double _tuesdayHours;
        public double TuesdayHours
        {
            get { return _tuesdayHours; }
            set
            {
                _tuesdayHours = value;
                if (IsEditable)
                {
                    TotalHours = MondayHours + TuesdayHours + WednesdayHours + ThursdayHours + FridayHours;
                }
                RaisePropertyChanged(nameof(TuesdayHours));
            }
        }

        private double _wednesdayHours;
        public double WednesdayHours
        {
            get { return _wednesdayHours; }
            set
            {
                _wednesdayHours = value;
                if (IsEditable)
                {
                    TotalHours = MondayHours + TuesdayHours + WednesdayHours + ThursdayHours + FridayHours;
                }
                RaisePropertyChanged(nameof(WednesdayHours));
            }
        }

        private double _thursdayHours;
        public double ThursdayHours
        {
            get { return _thursdayHours; }
            set
            {
                _thursdayHours = value;
                if (IsEditable)
                {
                    TotalHours = MondayHours + TuesdayHours + WednesdayHours + ThursdayHours + FridayHours;
                }
                RaisePropertyChanged(nameof(ThursdayHours));
            }
        }

        private double _fridayHours;
        public double FridayHours
        {
            get { return _fridayHours; }
            set
            {
                _fridayHours = value;

                if (IsEditable)
                {
                    TotalHours = MondayHours + TuesdayHours + WednesdayHours + ThursdayHours + FridayHours;
                }
                RaisePropertyChanged(nameof(FridayHours));
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

        private bool _isEditable;
        public bool IsEditable
        {
            get { return _isEditable; }
            set
            {
                _isEditable = value;
                if (_isEditable)
                {
                    TotalHours = MondayHours + TuesdayHours + WednesdayHours + ThursdayHours + FridayHours;
                }
            }
        }
        //private bool _canExpand = false;
        //public bool CanExpand
        //{
        //    get
        //    {
        //        return _canExpand;
        //    }
        //    set
        //    {

        //        if (IsEditable)
        //        {
        //            _canExpand = value;
        //            RaisePropertyChanged(nameof(CanExpand));

        //            if (value && !Formatted)
        //            {
        //                CollectTimesheetSubmission();
        //            }
        //        }
        //    }
        //}

        private System.Windows.Media.Brush _borderColor = System.Windows.Media.Brushes.Transparent;
        public System.Windows.Media.Brush BorderColor
        {
            get { return _borderColor; }
            set
            {
                _borderColor = value;
                RaisePropertyChanged(nameof(BorderColor));
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

        public bool Formatted = false;

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                RaisePropertyChanged(nameof(IsActive));
            }
        }

        private System.Windows.Media.ImageSource _signatureOfPM;
        public System.Windows.Media.ImageSource SignatureOfPM
        {
            get { return _signatureOfPM; }
            set
            {
                _signatureOfPM = value;
                RaisePropertyChanged(nameof(SignatureOfPM));
            }
        }
        public EmployeeDbModel baseemployee;
        public EmployeeModel()
        { }

        public EmployeeModel(EmployeeDbModel emdb)
        {
            baseemployee = emdb;
            Id = emdb.Id;
            FirstName = emdb.FirstName;
            LastName = emdb.LastName;
            FullName = FirstName + " " + LastName;
            DefaultRole = ((DefaultRoleEnum)emdb.DefaultRoleId);
            Status = ((AuthEnum)emdb.AuthId);
            Title = emdb.Title;
            Email = emdb.Email;
            IsActive = Convert.ToBoolean(emdb.IsActive);
            int index = emdb.Email.IndexOf("@");
            if (index != -1)
            {
                EmailForward = emdb.Email.Substring(0, index);
            }

            PhoneNumber = emdb.PhoneNumber;
            Extension = emdb.Extension;
            Rate = emdb.Rate;

            PTOCarryover = emdb.PTOCarryover;
            PTORate = emdb.PTORate;

            MondayHours = emdb.MondayHours;
            TuesdayHours = emdb.TuesdayHours;
            WednesdayHours = emdb.WednesdayHours;
            ThursdayHours = emdb.ThursdayHours;
            FridayHours = emdb.FridayHours;
            ScheduledTotalHours = MondayHours + TuesdayHours + WednesdayHours + ThursdayHours + FridayHours;
            SickRate = emdb.SickRate;
            HolidayHours = emdb.HolidayHours;
        }

        public void LoadSignature()
        {
            if (baseemployee != null)
            {
                if (baseemployee.PMSignature != null)
                {
                    byte[] imageData = (byte[])baseemployee.PMSignature;
                    BitmapImage biImg = new BitmapImage();
                    MemoryStream ms = new MemoryStream(imageData);
                    biImg.BeginInit();
                    biImg.StreamSource = ms;
                    biImg.EndInit();
                    ImageSource imgSrc = biImg as ImageSource;
                    SignatureOfPM = (ImageSource)imgSrc;
                }
            }
        }

        public void CollectTimesheetSubmission()
        {
            double ptospent = 0;
            double otspent = 0;
            double sickspent = 0;
            double holidayspent = 0;
            //load timesheet submissions
            List<TimesheetSubmissionModel> tsm = new List<TimesheetSubmissionModel>();

            //years
            int count = 0;
            List<TimesheetSubmissionDbModel> dbdata = SQLAccess.LoadTimesheetSubmissionByEmployee(Id);

            int year = DateTime.Now.Year;
            DateTime firstDay = new DateTime(year, 1, 1);

            foreach (TimesheetSubmissionDbModel tsmdb in dbdata)
            {
                TimesheetSubmissionModel tsmnew = new TimesheetSubmissionModel(tsmdb, this);
                tsm.Add(tsmnew);

                if (tsmnew.Date >= firstDay)
                {
                    count++;
                    ptospent += tsmdb.PTOHours;
                    otspent += tsmdb.OTHours;
                    sickspent += tsmdb.SickHours;
                    holidayspent += tsmdb.HolidayHours;
                }
            }
            List<TimesheetSubmissionModel> tsmfinal = tsm.OrderByDescending(x => x.Date).ToList(); 
            TimesheetSubmissions = new ObservableCollection<TimesheetSubmissionModel>(tsmfinal);
            PTOUsed = ptospent;
            TotalOT = otspent;
            SickUsed = sickspent;
            PTOEarned = (PTORate * count) / 2;
            SickEarned = (SickRate * count) / 2;
            HolidayUsed = holidayspent;
            HolidayLeft = HolidayHours - HolidayUsed;
            PTOHours = PTOCarryover + PTOEarned - PTOUsed;
            SickHours = SickEarned - SickUsed;
        }

        public void SetEmployeeModelfromUser(EmployeeModel currentuser)
        {
            switch (currentuser.Status)
            {
                case AuthEnum.Admin:
                    IsEditable = true;
                    CanEditorDelete = true;
                    CanEditPTO = true;
                    break;
                case AuthEnum.Principal:
                    IsEditable = true;
                    CanEditorDelete = true;
                    CanEditPTO = true;
                    break;
                case AuthEnum.PM:
                    IsEditable = false;
                    CanEditorDelete = false;
                    CanEditPTO = false;

                    break;
                case AuthEnum.Standard:
                    IsEditable = false;
                    CanEditorDelete = false;
                    CanEditPTO = false;

                    break;
                default:
                    IsEditable = false;
                    CanEditorDelete = false;
                    CanEditPTO = false;
                    break;
            }

            if (Id == currentuser.Id)
            {
                IsEditable = true;
                CanReviewTimesheet = true;
                BorderColor = System.Windows.Media.Brushes.LightGray;
            }
            else
            {
                CanReviewTimesheet = false;
            }
        }

        public void UpdateEmployee()
        {
            byte[] imageArray = null;

            if (SignatureOfPM != null)
            {
                imageArray = ImageSourceToBytes(SignatureOfPM);
            }
            else if (baseemployee.PMSignature != null)
            {
                imageArray = (byte[])baseemployee.PMSignature;
            }

            try
            {
                EmployeeDbModel employee = new EmployeeDbModel()
                {
                    Id = Id,
                    FirstName = FirstName,
                    LastName = LastName,
                    AuthId = (int)Status,
                    Title = Title,
                    Email = Email,
                    PhoneNumber = PhoneNumber,
                    Extension = Extension,
                    Rate = Rate,
                    PTORate = PTORate,
                    //PTOHours = PTOHours
                    DefaultRoleId = (int)DefaultRole,
                    PTOCarryover = PTOCarryover,
                    HolidayHours = HolidayHours,
                    //SickHours = SickHours,
                    SickRate = SickRate,
                    MondayHours = MondayHours,
                    TuesdayHours = TuesdayHours,
                    WednesdayHours = WednesdayHours,
                    ThursdayHours = ThursdayHours,
                    FridayHours = FridayHours,
                    PMSignature = imageArray,
                    IsActive = 1
                };

                SQLAccess.UpdateEmployee(employee);
                baseemployee.PMSignature = imageArray;
            }
            catch
            {

            }

            CollectTimesheetSubmission();
        }

        public byte[] ImageSourceToBytes(ImageSource imageSource)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            byte[] bytes = null;
            var bitmapSource = imageSource as BitmapSource;

            if (bitmapSource != null)
            {
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                using (var stream = new MemoryStream())
                {
                    encoder.Save(stream);
                    bytes = stream.ToArray();
                }
            }

            return bytes;
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

        public object Clone()
        {
            return new EmployeeModel()
            {
                Id = this.Id
                //FirstName = this.FirstName,
                //LastName = this.LastName,
                //Status = this.Status,
                //Title = this.Title,
                //Email = this.Email,
                //PhoneNumber = this.PhoneNumber,
                //Extension = this.Extension,
                //Rate = this.Rate,
                //PTORate = this.PTORate,
                ////PTOHours = PTOHours
                //DefaultRole = this.DefaultRole,
                //PTOCarryover = this.PTOCarryover,
                //HolidayHours = this.HolidayHours,
                ////SickHours = SickHours,
                //SickRate = this.SickRate,
                //MondayHours = this.MondayHours,
                //TuesdayHours = this.TuesdayHours,
                //WednesdayHours = this.WednesdayHours,
                //ThursdayHours = this.ThursdayHours,
                //FridayHours = this.FridayHours,
                //IsActive = this.IsActive
            };
        }

    }
}
