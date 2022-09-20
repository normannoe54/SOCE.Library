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
        public ICommand YesCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public AreYouSureVM()
        {
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CloseWindow);
        }

        public void YesDoTheAction()
        {
   
            //do stuff
            CloseWindow();
        }

        private void CloseWindow()
        {
            DialogHost.Close("RootDialog");
        }
    }
}
