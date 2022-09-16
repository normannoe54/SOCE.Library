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
        //private ObservableCollection<TimesheetSubmission> _timesheetSubmitted
        //public ObservableCollection<TimesheetSubmission> TimesheetSubmitted
        //{
        //    get { return _selectedCurr; }
        //    set
        //    {
        //        _selectedCurr = value;
        //        RaisePropertyChanged(nameof(SelectedCurr));
        //    }
        //}

        public int Id { get; set; }
        public string Name { get; set; }
        public AuthEnum Title { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Extension { get; set; }
        public double Rate { get; set; }

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

        public EmployeeModel()
        { }

        public EmployeeModel(EmployeeDbModel emdb)
        {
            Id = emdb.Id;
            Name = emdb.FullName;
            Title = ((AuthEnum)emdb.AuthId);

            int index = emdb.Email.IndexOf("@");
            Email = emdb.Email.Substring(0, index);


            PhoneNumber = emdb.PhoneNumber;
            Extension = emdb.Extension;
            Rate = emdb.Rate;
        }

    }
}
