using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using SORD.Library.Models.Accounts;

namespace SORD.Library.UI.ViewModels
{
    public class SignupVM :BaseVM
    {
        public RegisterRequest Register { get; set; } = new RegisterRequest();
        public ICommand GoToNewViewCommand { get; set; }

        public SignupVM()
        {
            Register.Password = "pass123";
            Register.ConfirmPassword = "pass123";
            Register.AcceptTerms = true;
            this.GoToNewViewCommand = new RelayCommand<ApplicationPage>(GoToViewCommand.GoToPageWrapper);
        }
    }
}
