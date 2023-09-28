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
    public class EmployeeScheduleSummaryVM : BaseVM
    {
        public ICommand GoBackCommand { get; set; }

        private ObservableCollection<DateWrapper> _datesummary = new ObservableCollection<DateWrapper>();
        public ObservableCollection<DateWrapper> DateSummary
        {
            get { return _datesummary; }
            set
            {
                _datesummary = value;
                RaisePropertyChanged(nameof(DateSummary));
            }
        }

        private ObservableCollection<EmployeeModel> _employeeSummary = new ObservableCollection<EmployeeModel>();
        public ObservableCollection<EmployeeModel> EmployeeSummary
        {
            get { return _employeeSummary; }
            set
            {
                _employeeSummary = value;
                RaisePropertyChanged(nameof(EmployeeSummary));
            }
        }

        public EmployeeScheduleSummaryVM(ObservableCollection<EmployeeModel> Employees, ObservableCollection<DateWrapper> Dates)
        {
            this.GoBackCommand = new RelayCommand(this.GoBack);
            EmployeeSummary = Employees;
            DateSummary = Dates;
            CollectEmployeeSummary();
        }

        private void GoBack()
        {
            BaseAI CurrentPage = IoCPortal.Application as BaseAI;
            PortalAI portAI = (PortalAI)CurrentPage;

            portAI.RightDrawerOpen = false;
        }

        private void CollectEmployeeSummary()
        {
            foreach (EmployeeModel em in EmployeeSummary)
            {
                em.Entries.Clear();
                List<SchedulingDataDbModel> data = SQLAccess.LoadSchedulingDataByEmployee(DateSummary[0].Value, em.Id);
                double hours1 = data.Sum(x => x.Hours1);
                double hours2 = data.Sum(x => x.Hours2);
                double hours3 = data.Sum(x => x.Hours3);
                double hours4 = data.Sum(x => x.Hours4);
                double hours5 = data.Sum(x => x.Hours5);
                double hours6 = data.Sum(x => x.Hours6);
                double hours7 = data.Sum(x => x.Hours7);
                double hours8 = data.Sum(x => x.Hours8);

                Brush brush1 = hours1 > em.ScheduledTotalHours ? (SolidColorBrush)new BrushConverter().ConvertFrom("#c92127") : Brushes.Black;
                Brush brush2 = hours2 > em.ScheduledTotalHours ? (SolidColorBrush)new BrushConverter().ConvertFrom("#c92127") : Brushes.Black;
                Brush brush3 = hours3 > em.ScheduledTotalHours ? (SolidColorBrush)new BrushConverter().ConvertFrom("#c92127") : Brushes.Black;
                Brush brush4 = hours4 > em.ScheduledTotalHours ? (SolidColorBrush)new BrushConverter().ConvertFrom("#c92127") : Brushes.Black;
                Brush brush5 = hours5 > em.ScheduledTotalHours ? (SolidColorBrush)new BrushConverter().ConvertFrom("#c92127") : Brushes.Black;
                Brush brush6 = hours6 > em.ScheduledTotalHours ? (SolidColorBrush)new BrushConverter().ConvertFrom("#c92127") : Brushes.Black;
                Brush brush7 = hours7 > em.ScheduledTotalHours ? (SolidColorBrush)new BrushConverter().ConvertFrom("#c92127") : Brushes.Black;
                Brush brush8 = hours8 > em.ScheduledTotalHours ? (SolidColorBrush)new BrushConverter().ConvertFrom("#c92127") : Brushes.Black;


                SDEntryModel hours1entry = new SDEntryModel() { Date = DateSummary[0].Value, TimeEntry = hours1, CellColor = brush1 };
                SDEntryModel hours2entry = new SDEntryModel() { Date = DateSummary[1].Value, TimeEntry = hours2, CellColor = brush2 };
                SDEntryModel hours3entry = new SDEntryModel() { Date = DateSummary[2].Value, TimeEntry = hours3, CellColor = brush3 };
                SDEntryModel hours4entry = new SDEntryModel() { Date = DateSummary[3].Value, TimeEntry = hours4, CellColor = brush4 };
                SDEntryModel hours5entry = new SDEntryModel() { Date = DateSummary[4].Value, TimeEntry = hours5, CellColor = brush5 };
                SDEntryModel hours6entry = new SDEntryModel() { Date = DateSummary[5].Value, TimeEntry = hours6, CellColor = brush6 };
                SDEntryModel hours7entry = new SDEntryModel() { Date = DateSummary[6].Value, TimeEntry = hours7, CellColor = brush7 };
                SDEntryModel hours8entry = new SDEntryModel() { Date = DateSummary[7].Value, TimeEntry = hours8, CellColor = brush8 };


                em.Entries.Add(hours1entry);
                em.Entries.Add(hours2entry);
                em.Entries.Add(hours3entry);
                em.Entries.Add(hours4entry);
                em.Entries.Add(hours5entry);
                em.Entries.Add(hours6entry);
                em.Entries.Add(hours7entry);
                em.Entries.Add(hours8entry);
            }
        }
       
    }
}
