﻿using System;
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

        private double _sickCarryover;
        public double SickCarryover
        {
            get { return _sickCarryover; }
            set
            {
                _sickCarryover = value;
                RaisePropertyChanged(nameof(SickCarryover));
            }
        }

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
                if (value && !Formatted)
                {
                    CollectTimesheetSubmission();
                }
                if (IsEditable)
                {
                    _canExpand = value;
                    RaisePropertyChanged(nameof(CanExpand));
                }
            }
        }

        public bool Formatted = false;

        public EmployeeModel()
        { }

        public EmployeeModel(EmployeeDbModel emdb)
        {
            Id = emdb.Id;
            FirstName = emdb.FirstName;
            LastName = emdb.LastName;
            FullName = FirstName + " " + LastName;
            DefaultRole = ((DefaultRoleEnum)emdb.DefaultRoleId);
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

            PTOCarryover = emdb.PTOCarryover;
            PTORate = emdb.PTORate;

            SickCarryover = emdb.SickCarryover;
            SickRate = emdb.SickRate;

            HolidayHours = emdb.HolidayHours;
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
            foreach (TimesheetSubmissionDbModel tsmdb in dbdata)
            {
                if (Convert.ToBoolean(tsmdb.Approved))
                {
                    count++;
                    tsm.Add(new TimesheetSubmissionModel(tsmdb, this));
                    ptospent += tsmdb.PTOHours;
                    otspent += tsmdb.OTHours;
                    sickspent += tsmdb.SickHours;
                    holidayspent += tsmdb.HolidayHours;
                }
               
            }

            TimesheetSubmissions = new ObservableCollection<TimesheetSubmissionModel>(tsm);
            PTOUsed = ptospent;
            TotalOT = otspent;
            SickUsed = sickspent;
            PTOEarned = (PTORate * count) / 2;
            SickEarned = (SickRate * count) / 2;
            HolidayLeft = HolidayHours - HolidayUsed;
            PTOHours = PTOCarryover + PTOEarned - PTOUsed;
            SickHours = SickCarryover + SickEarned - SickUsed;
        }

        public void SetEmployeeModelfromUser(EmployeeModel currentuser)
        {
            switch (currentuser.Status)
            {
                case AuthEnum.Admin:
                    RateVisible = true;
                    IsEditable = true;
                    CanEditorDelete = true;
                    CanEditPTO = true;
                    break;
                case AuthEnum.Principal:
                    RateVisible = true;
                    IsEditable = true;
                    CanEditorDelete = false;
                    CanEditPTO = false;
                    break;
                case AuthEnum.PM:
                    RateVisible = true;
                    IsEditable = false;
                    CanEditorDelete = false;
                    CanEditPTO = false;

                    break;
                case AuthEnum.Standard:
                    RateVisible = false;
                    IsEditable = false;
                    CanEditorDelete = false;
                    CanEditPTO = false;

                    break;
                default:
                    RateVisible = false;
                    IsEditable = false;
                    CanEditorDelete = false;
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
                PTORate = PTORate,
                //PTOHours = PTOHours,
                PTOCarryover = PTOCarryover,
                HolidayHours = HolidayHours,
                //SickHours = SickHours,
                SickRate = SickRate,
                SickCarryover = SickCarryover
            };

            SQLAccess.UpdateEmployee(employee);
            CollectTimesheetSubmission();
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
