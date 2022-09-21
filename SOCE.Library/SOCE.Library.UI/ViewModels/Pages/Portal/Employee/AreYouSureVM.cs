using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SOCE.Library.Db;

namespace SOCE.Library.UI.ViewModels
{
    public class AreYouSureVM : BaseVM
    {
        public bool Result { get; set; }
        public string EmployeeFullName { get; set; }
        public ICommand YesCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public AreYouSureVM(EmployeeModel em)
        {
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
            EmployeeFullName = em.FirstName + " " + em.LastName;
        }

        public void YesDoTheAction()
        {
            Result = true;
            //do stuff
            CloseWindow();
        }

        private void CancelCommand()
        {
            Result = false;
            //do stuff
            CloseWindow();
        }

        private void CloseWindow()
        {
            DialogHost.Close("RootDialog");
        }
    }
}
