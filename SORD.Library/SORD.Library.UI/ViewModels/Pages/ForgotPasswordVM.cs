using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;


namespace SORD.Library.UI.ViewModels
{
    public class ForgotPasswordVM : BaseVM
    {
        

        public ICommand GoToNewViewCommand { get; set; }

        public ForgotPasswordVM()
        {
            this.GoToNewViewCommand = new RelayCommand<ApplicationPage>(GoToViewCommand.GoToPageWrapper);
        }
    }
}
