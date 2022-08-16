using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;


namespace SOCE.Library.UI.ViewModels
{
    public class ForgotPasswordVM : BaseVM
    {
        public ForgotPassRequestModel _forgetPassswordRequest { get; set; }
        public ForgotPassRequestModel ForgetPassswordRequest
        {
            get
            {
                return _forgetPassswordRequest;
            }
            set
            {
                _forgetPassswordRequest = value;
                RaisePropertyChanged(nameof(ForgetPassswordRequest));
            }
        }

        public ICommand GoToNewViewCommand { get; set; }

        public ICommand SendEmailCommand { get; set; }

        public ForgotPasswordVM()
        {
            this.GoToNewViewCommand = new RelayCommand<LoginPage>(GoToViewCommand.GoToPageWrapper);
            this.SendEmailCommand = new RelayCommand<ForgotPassRequestModel>(AccountCommand.SendForgotEmail);
        }
    }
}
