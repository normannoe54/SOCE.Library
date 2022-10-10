using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;
using SOCE.Library.Db;

namespace SOCE.Library.UI.ViewModels
{
    public class ResetPasswordVM : BaseVM
    {
        private string _passwordConfirm = "";
        public string PasswordConfirm
        {
            get
            {
                return _passwordConfirm;
            }
            set
            {
                _passwordConfirm = value;
                RaisePropertyChanged(nameof(PasswordConfirm));
            }
        }

        private string _password = "";
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                RaisePropertyChanged(nameof(Password));
            }
        }

        public ICommand GoToNewViewCommand { get; set; }

        public ICommand ConfirmPasswordChangeCommand { get; set; }

        private string _message { get; set; }
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                RaisePropertyChanged(nameof(Message));
            }
        }

        public EmployeeDbModel LoggedInEmployee;

        public ResetPasswordVM(EmployeeDbModel employee)
        {
            LoggedInEmployee = employee;
            this.GoToNewViewCommand = new RelayCommand(IoCLogin.Application.ForgotPassword);
            this.ConfirmPasswordChangeCommand = new RelayCommand(ConfirmPasswordChange);
        }

        /// <summary>
        /// Login command
        /// </summary>
        /// <param name="loginrequest"></param>
        public void ConfirmPasswordChange()
        {

            if (Password == PasswordConfirm && LoggedInEmployee !=null)
            {
                LoggedInEmployee.Password = Password;
                SQLAccess.UpdatePassword(LoggedInEmployee);
                IoCLogin.Application.GoToLogin();
            }
            else
            {
                Message = $"Passwords do not match, try again.";
                return;
            }
        }
    }
}