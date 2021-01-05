using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace SORD.Library.UI.ViewModels
{ 
    public class LoginVM : BaseVM
    {
        public ICommand GoToNewViewCommand { get; set; }

        public LoginVM()
        {
            this.GoToNewViewCommand = new RelayCommand<ApplicationPage>(GoToViewCommand.GoToPageWrapper);
        }   
    }
}
