using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SOCE.Library.Db;

namespace SOCE.Library.UI.ViewModels
{
    public class AddEmployeeVM : BaseVM
    {
        private string _firstNameInp;
        public string FirstNameInp
        {
            get { return _firstNameInp; }
            set
            {
                _firstNameInp = value;
                RaisePropertyChanged("FirstNameInp");
            }
        }

        private string _lastNameInp;
        public string LastNameInp
        {
            get { return _lastNameInp; }
            set
            {
                _lastNameInp = value;
                RaisePropertyChanged("LastNameInp");
            }
        }

        private string _emailInp;
        public string EmailInp
        {
            get { return _emailInp; }
            set
            {
                _emailInp = value;
                RaisePropertyChanged("EmailInp");
            }
        }

        private string _extensionInp;
        public string ExtensionInp
        {
            get { return _extensionInp; }
            set
            {
                _extensionInp = value;
                RaisePropertyChanged("ExtensionInp");
            }
        }

        private string _phoneNumberInp;
        public string PhoneNumberInp
        {
            get { return _phoneNumberInp; }
            set
            {
                _phoneNumberInp = value;
                RaisePropertyChanged("PhoneNumberInp");
            }
        }

        private string _titleInp;
        public string TitleInp
        {
            get { return _titleInp; }
            set
            {
                _titleInp = value;
                RaisePropertyChanged("TitleInp");
            }
        }

        private AuthEnum _selectedAuthorization;
        public AuthEnum SelectedAuthorization
        {
            get { return _selectedAuthorization; }
            set
            {
                _selectedAuthorization = value;
                RaisePropertyChanged("SelectedAuthorization");
            }
        }

        public ICommand AddEmployeeCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public AddEmployeeVM()
        {
            this.AddEmployeeCommand = new RelayCommand(this.AddEmployee);
            this.CloseCommand = new RelayCommand(this.CloseWindow);
        }

        public void AddEmployee()
        {
            EmployeeDbModel employee = new EmployeeDbModel()
            { FirstName = FirstNameInp, LastName = LastNameInp, AuthId = (int)SelectedAuthorization, Email = EmailInp, PhoneNumber = PhoneNumberInp, Extension = ExtensionInp };

            SQLAccess.AddEmployee(employee);

            //do stuff
            CloseWindow();
        }

        private void CloseWindow()
        {
            DialogHost.Close("RootDialog");
        }
    }
}
