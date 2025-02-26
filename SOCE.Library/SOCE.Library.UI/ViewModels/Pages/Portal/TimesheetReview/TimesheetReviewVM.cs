﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using SOCE.Library.Db;
using System.Globalization;
using MaterialDesignThemes.Wpf;
using SOCE.Library.UI.Views;
using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using System.Windows;

namespace SOCE.Library.UI.ViewModels
{
    public class TimesheetReviewVM : BaseVM
    {

        private BaseVM _reviewVM;
        public BaseVM ReviewVM
        {
            get
            {
                return _reviewVM;
            }
            set
            {
                _reviewVM = value;
                RaisePropertyChanged(nameof(ReviewVM));
            }
        }

        private EmployeeModel _currentEmployee;
        public EmployeeModel CurrentEmployee
        {
            get
            {
                return _currentEmployee;
            }
            set
            {
                _currentEmployee = value;
                RaisePropertyChanged(nameof(CurrentEmployee));
            }
        }

        public ICommand PreviousCommand { get; set; }

        public ICommand NextCommand { get; set; }

        public ICommand CurrentCommand { get; set; }

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

        private string _monthYearString = "";
        public string MonthYearString
        {
            get { return _monthYearString; }
            set
            {
                _monthYearString = value;
                RaisePropertyChanged(nameof(MonthYearString));
            }
        }

        private string _dateString = "";
        public string DateString
        {
            get { return _dateString; }
            set
            {
                _dateString = value;
                RaisePropertyChanged(nameof(DateString));
            }
        }

        private double _baseHours = 0;
        public double BaseHours
        {
            get { return _baseHours; }
            set
            {
                _baseHours = value;
                RaisePropertyChanged(nameof(BaseHours));
            }
       }

        public DateTime firstdate;

        public DateTime lastdate;

        private DateTime _dateSelected;
        public DateTime DateSelected
        {
            get { return _dateSelected; }
            set
            {
                _dateSelected = value;
                if (allowdatechange)
                {
                    SelectedTimesheet();
                }
                RaisePropertyChanged(nameof(DateSelected));
            }
        }

        private bool allowdatechange = true;

        public TimesheetReviewVM(EmployeeModel loggedinEmployee)
        { 
            CurrentEmployee = loggedinEmployee;
            UpdateDates(DateTime.Now);
            
            CurrentTimesheet();

            this.PreviousCommand = new RelayCommand(PreviousTimesheet);
            this.NextCommand = new RelayCommand(NextTimesheet);
            this.CurrentCommand = new RelayCommand(CurrentTimesheet);

            ReviewVM = new EmployeeSummaryVM(this);

        }

        public void UpdateDates(DateTime currdate)
        {
            allowdatechange = false;
            if (currdate.Day > 16)
            {
                //second tier
                firstdate = new DateTime(currdate.Year, currdate.Month, 17);
                lastdate = new DateTime(currdate.Year, currdate.Month, DateTime.DaysInMonth(currdate.Year, currdate.Month));
            }
            else
            {
                //first tier
                firstdate = new DateTime(currdate.Year, currdate.Month, 1);
                lastdate = new DateTime(currdate.Year, currdate.Month, 16);
            }

            int diff = (lastdate - firstdate).Days;
            List<DateWrapper> dates = new List<DateWrapper>();
            //double basehours = 0;
            for (int i = 0; i <= diff; i++)
            {
                DateTime dt = firstdate.AddDays(i);
                dates.Add(new DateWrapper(dt.Date));
            }

            double basehours = 0;

            if (ReviewVM != null)
            {
                TimesheetViewerVM vm = ReviewVM as TimesheetViewerVM;

                if (vm != null)
                {
                    for (int i = 0; i <= diff; i++)
                    {
                        DateTime dt = firstdate.AddDays(i);

                        if (dt.DayOfWeek == DayOfWeek.Friday)
                        {
                            basehours += vm.SelectedEmployee.FridayHours;
                        }
                        else if (dt.DayOfWeek == DayOfWeek.Thursday)
                        {
                            basehours += vm.SelectedEmployee.ThursdayHours;
                        }
                        else if (dt.DayOfWeek == DayOfWeek.Wednesday)
                        {
                            basehours += vm.SelectedEmployee.WednesdayHours;
                        }
                        else if (dt.DayOfWeek == DayOfWeek.Tuesday)
                        {
                            basehours += vm.SelectedEmployee.TuesdayHours;
                        }
                        else if (dt.DayOfWeek == DayOfWeek.Monday)
                        {
                            basehours += vm.SelectedEmployee.MondayHours;
                        }
                    }
                }
            }

            DateSummary = new ObservableCollection<DateWrapper>(dates);
            BaseHours = basehours;
            DateSummary = new ObservableCollection<DateWrapper>(dates);
            MonthYearString = $"{firstdate.ToString("MMMM")} {firstdate.Year}";
            DateString = $"[{firstdate.Day} - {lastdate.Day}]";
            //DateTimesheet = (int)long.Parse(firstdate.Date.ToString("yyyyMMdd"));
            DateTime enddate = DateSummary.Last().Value;
            int difference = (int)Math.Ceiling(Math.Max((enddate - DateTime.Now).TotalDays, 0));
            DateSelected = firstdate;
            allowdatechange = true;
        }

        private async void SelectedTimesheet()
        {
            CoreAI CurrentPage = IoCCore.Application as CoreAI;
            CurrentPage.MakeBlurry();
            await Task.Run(() => Task.Delay(600));
            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                LoadCurrentTimesheet(DateSelected);
            }));
            await Task.Run(() => Task.Delay(600));
            CurrentPage.MakeClear();
        }

        /// <summary>
        /// Button Press
        /// </summary>
        private async void PreviousTimesheet()
        {
            CoreAI CurrentPage = IoCCore.Application as CoreAI;
            CurrentPage.MakeBlurry();
            await Task.Run(() => Task.Delay(600));
            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                LoadCurrentTimesheet(DateSummary.First().Value.AddDays(-1));
            }));
            await Task.Run(() => Task.Delay(600));
            CurrentPage.MakeClear();
        }

        /// <summary>
        /// Button Press
        /// </summary>
        private async void NextTimesheet()
        {
            CoreAI CurrentPage = IoCCore.Application as CoreAI;
            CurrentPage.MakeBlurry();
            await Task.Run(() => Task.Delay(600));
            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                LoadCurrentTimesheet(DateSummary.Last().Value.AddDays(1));
            }));
            await Task.Run(() => Task.Delay(600));
            CurrentPage.MakeClear();
        }

        /// <summary>
        /// Button Press
        /// </summary>
        private async void CurrentTimesheet()
        {
            CoreAI CurrentPage = IoCCore.Application as CoreAI;
            CurrentPage.MakeBlurry();
            await Task.Run(() => Task.Delay(600));
            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                LoadCurrentTimesheet(DateTime.Now);
            }));
            await Task.Run(() => Task.Delay(600));
            CurrentPage.MakeClear();
            
        }

        private void LoadCurrentTimesheet(DateTime currdate)
        {
            UpdateDates(currdate);
            
            //determine which viewmodel is active and do stuff.

            if (ReviewVM is EmployeeSummaryVM)
            {
                EmployeeSummaryVM emvsm = (EmployeeSummaryVM)ReviewVM;
                emvsm.LoadTimesSheetSubmissionData(firstdate);
            }
            else if (ReviewVM is TimesheetViewerVM)
            {
                TimesheetViewerVM tvvm = (TimesheetViewerVM)ReviewVM;
                tvvm.ReloadTimesheet(firstdate);
            }
        }

    }
}
