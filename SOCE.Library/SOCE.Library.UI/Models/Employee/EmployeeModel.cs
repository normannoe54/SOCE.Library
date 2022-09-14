using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using SOCE.Library.Db;

namespace SOCE.Library.UI
{
    public class EmployeeModel : PropertyChangedBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public AuthEnum Title { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Extension { get; set; }

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
            Email = emdb.Email;
            PhoneNumber = emdb.PhoneNumber;
            Extension = emdb.Extension;
        }

    }
}
