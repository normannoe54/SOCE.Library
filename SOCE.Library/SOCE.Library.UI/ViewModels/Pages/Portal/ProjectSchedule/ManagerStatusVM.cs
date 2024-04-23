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
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Media;

namespace SOCE.Library.UI.ViewModels
{
    public class ManagerStatusVM : BaseVM
    {
        public ICommand GoBackCommand { get; set; }

        private ObservableCollection<EmployeeScheduleModel> _managerStatusSummary = new ObservableCollection<EmployeeScheduleModel>();
        public ObservableCollection<EmployeeScheduleModel> ManagerStatusSummary
        {
            get { return _managerStatusSummary; }
            set
            {
                _managerStatusSummary = value;
                RaisePropertyChanged(nameof(ManagerStatusSummary));
            }
        }

        private ScheduleWeekVM vm;

        public ManagerStatusVM(ObservableCollection<EmployeeScheduleModel> Managers, ScheduleWeekVM svm)
        {
            vm = svm;
            this.GoBackCommand = new RelayCommand(this.GoBack);
            ManagerStatusSummary = Managers;
        }

        private void GoBack()
        {
            BaseAI CurrentPage = IoCPortal.Application as BaseAI;
            PortalAI portAI = (PortalAI)CurrentPage;
            vm.ReloadEmployees();
            portAI.RightDrawerOpen = false;
        }

    }
}
