using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SOCE.Library.Db;
using System.Collections.ObjectModel;
using SOCE.Library.UI.Views;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Diagnostics;
using System.Linq;

namespace SOCE.Library.UI.ViewModels
{
    public class ProposalSummaryVM : BaseVM
    {
        public ICommand CloseCommand { get; set; }
        public EmployeeModel CurrentEmployee { get; set; }
        public ProposalViewResModel BaseProposal { get; set; }
        public ProposalSummaryVM(EmployeeModel employee, ProposalViewResModel baseproposal)
        {
            this.CloseCommand = new RelayCommand(this.CloseWindow);
            CurrentEmployee = employee;
            BaseProposal = baseproposal;
        }

        private void CloseWindow()
        {
            DialogHost.Close("RootDialog");
        }
    }
}
