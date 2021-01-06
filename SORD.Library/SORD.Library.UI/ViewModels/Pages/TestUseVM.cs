using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace SORD.Library.UI.ViewModels.Pages
{
    public class TestUseVM
    {
        public ICommand GoToNewViewCommand { get; set; }

        public TestUseVM()
        {
            this.GoToNewViewCommand = new RelayCommand<ApplicationPage>(GoToViewCommand.GoToPageWrapper);
        }
    }
}
