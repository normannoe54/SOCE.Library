using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SOCE.Library.Db;

namespace SOCE.Library.UI
{
    public class EmployeeLowResModel : PropertyChangedBase, ICloneable
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

        public EmployeeLowResModel()
        { }

        public EmployeeDbModel baseemployee;
        public EmployeeLowResModel(EmployeeDbModel emdb)
        {
            Id = emdb.Id;
            FirstName = emdb.FirstName;
            LastName = emdb.LastName;
            FullName = FirstName + " " + LastName;
            DefaultRole = ((DefaultRoleEnum)emdb.DefaultRoleId);
            Status = ((AuthEnum)emdb.AuthId);
            Rate = emdb.Rate;
            ScheduledTotalHours = emdb.MondayHours + emdb.TuesdayHours + emdb.WednesdayHours + emdb.ThursdayHours + emdb.FridayHours;
            baseemployee = emdb;
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


        public object Clone()
        {
            return new EmployeeLowResModel()
            {
                Id = this.Id
            };
        }

    }
}
