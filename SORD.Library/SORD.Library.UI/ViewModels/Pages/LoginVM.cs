using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using SORD.Library.Models.Accounts;

namespace SORD.Library.UI.ViewModels
{ 
    public class LoginVM : BaseVM
    {
        public AuthenticateRequest LoginRequest { get; set; } = new AuthenticateRequest();
        public ICommand GoToNewViewCommand { get; set; }

        public ICommand LoginAttemptCommand { get; set; }

        public LoginVM()
        {
            LoginRequest.Password = "pass123";
            this.GoToNewViewCommand = new RelayCommand<ApplicationPage>(GoToViewCommand.GoToPageWrapper);
            //this.LoginAttemptCommand = new RelayCommand<AuthenticateRequest>(LoginCommand.Login);
        }   
    }
}
