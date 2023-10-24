using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SOCE.Library.Db;
using System.Reflection;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.IO;

namespace SOCE.Library.UI.ViewModels
{
    public class IndividualSingleVM : BaseVM
    {
        public ResultEnum Result { get; set; } = ResultEnum.Cancel;
        public ICommand PrintFullReportCommand { get; set; }

        public ICommand PrintIndividualCommand { get; set; }

        public ICommand CloseCommand { get; set; }


        public IndividualSingleVM()
        {
            this.PrintFullReportCommand = new RelayCommand(this.PrintFullReport);
            this.PrintIndividualCommand = new RelayCommand(this.PrintIndividualReport);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
        }

        private void PrintFullReport()
        {
            Result = ResultEnum.ResultOne;
            CancelCommand();
        }

        private void PrintIndividualReport()
        {
            Result = ResultEnum.ResultTwo;
            CancelCommand();
        }


        private void CancelCommand()
        {
            DialogHost.Close("RootDialog");
        }
    }
}
