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
using System.Windows.Threading;
using System.Threading;

namespace SOCE.Library.UI.ViewModels
{
    public class EmployeeScheduleSummaryVM : BaseVM
    {
        public ICommand GoBackCommand { get; set; }
        public ICommand PrintCommand { get; set; }

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

        private ObservableCollection<EmployeeScheduleModel> _employeeSummary = new ObservableCollection<EmployeeScheduleModel>();
        public ObservableCollection<EmployeeScheduleModel> EmployeeSummary
        {
            get { return _employeeSummary; }
            set
            {
                _employeeSummary = value;
                RaisePropertyChanged(nameof(EmployeeSummary));
            }
        }

        public EmployeeScheduleSummaryVM(ObservableCollection<EmployeeScheduleModel> Employees, ObservableCollection<DateWrapper> Dates)
        {
            this.GoBackCommand = new RelayCommand(this.GoBack);
            this.PrintCommand = new RelayCommand(Print);
            EmployeeSummary = Employees;
            DateSummary = Dates;
            CollectEmployeeSummary();
        }

        private async void Print()
        {
            CoreAI CurrentPage = IoCCore.Application as CoreAI;
            CurrentPage.MakeBlurry();
            await Task.Run(() => Task.Delay(600));
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                try
                {
                    string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    string pathDownload = Path.Combine(pathUser, "Downloads\\EmployeeSummary.xlsx");
                    File.WriteAllBytes(pathDownload, Properties.Resources.EmployeeSummary);
                    Excel.Excel exinst = new Excel.Excel(pathDownload);

                    Thread.Sleep(200);

                    int rowid = 1;

                    List<object> values = new List<object>();
                    values.Add("Employee");

                    foreach (DateWrapper date in DateSummary)
                    {
                        values.Add(date.Value.ToString("MM/dd/yyyy"));
                    }

                    exinst.WriteRow<object>(rowid, 1, values);
                    exinst.MakeRowBold(rowid, 1, values.Count());
                    rowid++;
                    values = new List<object>();

                    foreach (EmployeeScheduleModel esm in EmployeeSummary)
                    {
                        values.Add(esm.FullName);
                        foreach (SDEntryModel entry in esm.Entries)
                        {
                            values.Add(entry.TimeEntry);
                        }

                        values.Add(esm.Entries.Sum(x => x.TimeEntry));
                        exinst.WriteRow<object>(rowid, 1, values);
                        exinst.MakeRowCustomBorder(rowid, 1, values.Count());
                        rowid++;
                        values = new List<object>();
                    }

                    exinst.SaveDocument();
                    Process.Start(pathDownload);
                }
                catch
                {
                }
            }));
            await Task.Run(() => Task.Delay(600));
            CurrentPage.MakeClear();

        }

        private void GoBack()
        {
            BaseAI CurrentPage = IoCPortal.Application as BaseAI;
            PortalAI portAI = (PortalAI)CurrentPage;

            portAI.RightDrawerOpen = false;
        }

        private void CollectEmployeeSummary()
        {
            foreach (EmployeeScheduleModel em in EmployeeSummary)
            {
                em.Entries = null;
                List<SchedulingDataDbModel> data = SQLAccess.LoadSchedulingDataByEmployee(DateSummary[0].Value, em.Id);
                double hours1 = data.Sum(x => x.Hours1);
                double hours2 = data.Sum(x => x.Hours2);
                double hours3 = data.Sum(x => x.Hours3);
                double hours4 = data.Sum(x => x.Hours4);
                double hours5 = data.Sum(x => x.Hours5);
                double hours6 = data.Sum(x => x.Hours6);
                double hours7 = data.Sum(x => x.Hours7);
                double hours8 = data.Sum(x => x.Hours8);

                Brush solidgreen = Brushes.Blue;
                Brush solidred = Brushes.Red;

                double brushhours1 = Math.Min(hours1, 40);
                double brushhours2 = Math.Min(hours2, 40);
                double brushhours3 = Math.Min(hours2, 40);
                double brushhours4 = Math.Min(hours2, 40);
                double brushhours5 = Math.Min(hours2, 40);
                double brushhours6 = Math.Min(hours2, 40);
                double brushhours7 = Math.Min(hours2, 40);
                double brushhours8 = Math.Min(hours2, 40);

                Brush brush1 = solidred.Blend(solidgreen, 0.9 * (brushhours1 / 40));
                Brush brush2 = solidred.Blend(solidgreen, 0.9 * (brushhours2 / 40));
                Brush brush3 = solidred.Blend(solidgreen, 0.9 * (brushhours3 / 40));
                Brush brush4 = solidred.Blend(solidgreen, 0.9 * (brushhours4 / 40));
                Brush brush5 = solidred.Blend(solidgreen, 0.9 * (brushhours5 / 40));
                Brush brush6 = solidred.Blend(solidgreen, 0.9 * (brushhours6 / 40));
                Brush brush7 = solidred.Blend(solidgreen, 0.9 * (brushhours7 / 40));
                Brush brush8 = solidred.Blend(solidgreen, 0.9 * (brushhours8 / 40));

                SDEntryModel hours1entry = new SDEntryModel() { Date = DateSummary[0].Value, TimeEntry = hours1, CellColor = brush1 };
                SDEntryModel hours2entry = new SDEntryModel() { Date = DateSummary[1].Value, TimeEntry = hours2, CellColor = brush2 };
                SDEntryModel hours3entry = new SDEntryModel() { Date = DateSummary[2].Value, TimeEntry = hours3, CellColor = brush3 };
                SDEntryModel hours4entry = new SDEntryModel() { Date = DateSummary[3].Value, TimeEntry = hours4, CellColor = brush4 };
                SDEntryModel hours5entry = new SDEntryModel() { Date = DateSummary[4].Value, TimeEntry = hours5, CellColor = brush5 };
                SDEntryModel hours6entry = new SDEntryModel() { Date = DateSummary[5].Value, TimeEntry = hours6, CellColor = brush6 };
                SDEntryModel hours7entry = new SDEntryModel() { Date = DateSummary[6].Value, TimeEntry = hours7, CellColor = brush7 };
                SDEntryModel hours8entry = new SDEntryModel() { Date = DateSummary[7].Value, TimeEntry = hours8, CellColor = brush8 };

                List<SDEntryModel> entries = new List<SDEntryModel>();
                entries.Add(hours1entry);
                entries.Add(hours2entry);
                entries.Add(hours3entry);
                entries.Add(hours4entry);
                entries.Add(hours5entry);
                entries.Add(hours6entry);
                entries.Add(hours7entry);
                entries.Add(hours8entry);
                em.Entries = new ObservableCollection<SDEntryModel>(entries);
            }
        }
    }
}
