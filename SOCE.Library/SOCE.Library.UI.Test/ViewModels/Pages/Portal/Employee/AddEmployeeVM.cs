using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace SOCE.Library.UI.ViewModels
{
    public class AddEmployeeVM : BaseVM
    {
        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                RaisePropertyChanged("FirstName");
            }
        }

        private string _lastName;
        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                RaisePropertyChanged("LastName");
            }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                RaisePropertyChanged("Email");
            }
        }

        private string _extension;
        public string Extension
        {
            get { return _extension; }
            set
            {
                _extension = value;
                RaisePropertyChanged("Extension");
            }
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set
            {
                _phoneNumber = value;
                RaisePropertyChanged("PhoneNumber");
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged("Title");
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
            //do stuff
            CloseWindow();
        }

        private void CloseWindow()
        {
            DialogHost.Close("RootDialog");
        }
    }
}
