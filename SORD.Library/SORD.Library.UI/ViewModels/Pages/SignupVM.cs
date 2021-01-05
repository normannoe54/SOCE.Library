using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace SORD.Library.UI.ViewModels
{
    public class SignupVM :BaseVM
    {
        public ICommand GoToNewViewCommand { get; set; }

        public SignupVM()
        {
            this.GoToNewViewCommand = new RelayCommand<ApplicationPage>(GoToViewCommand.GoToPageWrapper);
        }
    }
}
