using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SOCE.Library.Db;
using System.Reflection;
using System.ComponentModel;

namespace SOCE.Library.UI.ViewModels
{
    public class MessageBoxVM : BaseVM
    {
        public string Message;
        public ICommand CloseCommand { get; set; }
        public MessageBoxVM(string message)
        {
            Message = message;
            this.CloseCommand = new RelayCommand(this.CancelCommand);
        }
        private void CancelCommand()
        {
            DialogHost.Close("RootDialog");
        }
    }
}
