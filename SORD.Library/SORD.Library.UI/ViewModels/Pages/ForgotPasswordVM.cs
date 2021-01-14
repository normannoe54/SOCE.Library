using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using SORD.Library.Models.Accounts;


namespace SORD.Library.UI.ViewModels
{
    public class ForgotPasswordVM : BaseVM
    {
        public ForgotPasswordRequest ForgetPassswordRequest { get; set; }

        public ICommand GoToNewViewCommand { get; set; }

        public ICommand SendEmailCommand { get; set; }

        public ForgotPasswordVM()
        {
            this.GoToNewViewCommand = new RelayCommand<ApplicationPage>(GoToViewCommand.GoToPageWrapper);
            this.SendEmailCommand = new RelayCommand<ForgotPasswordRequest>(AccountCommand.SendForgotEmail);
        }
    }
}
