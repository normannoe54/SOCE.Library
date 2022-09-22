﻿using MaterialDesignThemes.Wpf;
using SOCE.Library.Db;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SOCE.Library.UI
{
    public class TimesheetSubmissionModel : PropertyChangedBase
    {
        public int Id { get; set; }

        private DateTime _date;
        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
                RaisePropertyChanged(nameof(Date));
            }
        }

        private bool _approved = false;
        public bool Approved
        {
            get
            {
                return _approved;
            }
            set
            {
                _approved = value;

                Icon = _approved ? MaterialDesignThemes.Wpf.PackIconKind.CheckCircleOutline: MaterialDesignThemes.Wpf.PackIconKind.AlertOctagonOutline;
                Iconcolor = _approved ? Brushes.Green : Brushes.Orange;
                StatusTooltip = _approved ? "Approved" : "In Progress";
                RaisePropertyChanged(nameof(Approved));
            }
        }

        public double OTHours { get; set; }

        public double PTOHours { get; set; }

        public double HolidayHours { get; set; }

        public double SickHours { get; set; }

        private double _totalHours { get; set; }
        public double TotalHours
        {
            get
            {
                return _totalHours;
            }
            set
            {
                _totalHours = value;
                RaisePropertyChanged(nameof(TotalHours));
            }
        }

        private PackIconKind _icon;
        public PackIconKind Icon
        {
            get { return _icon; }
            set
            {
                _icon = value;
                RaisePropertyChanged(nameof(Icon));
            }
        }


        private Brush _iconcolor;
        public Brush Iconcolor
        {
            get { return _iconcolor; }
            set
            {
                _iconcolor = value;
                RaisePropertyChanged(nameof(Iconcolor));
            }
        }

        private string _statusTooltip;
        public string StatusTooltip
        {
            get { return _statusTooltip; }
            set
            {
                _statusTooltip = value;
                RaisePropertyChanged(nameof(StatusTooltip));
            }
        }

        private bool _selectedCurr;
        public bool SelectedCurr
        {
            get { return _selectedCurr; }
            set
            {
                _selectedCurr = value;
                RaisePropertyChanged(nameof(SelectedCurr));
            }
        }

        public TimesheetSubmissionModel()
        { }

        public TimesheetSubmissionModel(TimesheetSubmissionDbModel emdb)
        {
            Id = emdb.Id;
            Date = DateTime.ParseExact(emdb.Date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            TotalHours = emdb.TotalHours;
            PTOHours = emdb.PTOHours;
            SickHours = emdb.SickHours;
            HolidayHours = emdb.HolidayHours;
            Approved = Convert.ToBoolean(emdb.Approved);
        }
    }
}