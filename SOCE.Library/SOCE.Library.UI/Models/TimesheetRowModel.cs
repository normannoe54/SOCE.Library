﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace SOCE.Library.UI
{
    public class TimesheetRowModel : PropertyChangedBase
    {
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

        private ProjectModel _projectModel;
        public ProjectModel ProjectModel
        {
            get { return _projectModel; }
            set
            {
                _projectModel = value;
                RaisePropertyChanged(nameof(ProjectModel));
            }
        }

        //private string _projectName;
        //public string ProjectName
        //{
        //    get { return _projectName; }
        //    set
        //    {
        //        _projectName = value;
        //        RaisePropertyChanged(nameof(ProjectName));
        //    }
        //}

        //private double _jobNum;
        //public double JobNum
        //{
        //    get { return _jobNum; }
        //    set
        //    {
        //        _jobNum = value;
        //        RaisePropertyChanged(nameof(JobNum));
        //    }
        //}

        private double _mondayTime;
        public double MondayTime
        {
            get
            {
                return _mondayTime;
            }
            set
            {
                _mondayTime = value;
                SetTotal();
                RaisePropertyChanged(nameof(MondayTime));
            }
        }

        private double _tuesdayTime;
        public double TuesdayTime
        {
            get
            {
                return _tuesdayTime;
            }
            set
            {
                _tuesdayTime = value;
                SetTotal();
                RaisePropertyChanged(nameof(TuesdayTime));
            }
        }

        private double _wednesdayTime;
        public double WednesdayTime
        {
            get
            {
                return _wednesdayTime;
            }
            set
            {
                _wednesdayTime = value;
                SetTotal();
                RaisePropertyChanged(nameof(WednesdayTime));
            }
        }

        private double _thursdayTime;
        public double ThursdayTime
        {
            get
            {
                return _thursdayTime;
            }
            set
            {
                _thursdayTime = value;
                SetTotal();
                RaisePropertyChanged(nameof(ThursdayTime));
            }
        }

        private double _fridayTime;
        public double FridayTime
        {
            get
            {
                return _fridayTime;
            }
            set
            {
                _fridayTime = value;
                SetTotal();
                RaisePropertyChanged(nameof(FridayTime));
            }
        }

        private double _saturdayTime;
        public double SaturdayTime
        {
            get
            {
                return _saturdayTime;
            }
            set
            {
                _saturdayTime = value;
                SetTotal();
                RaisePropertyChanged(nameof(SaturdayTime));
            }
        }

        private double _sundayTime;
        public double SundayTime
        {
            get
            {
                return _sundayTime;
            }
            set
            {
                _sundayTime = value;
                SetTotal();
                RaisePropertyChanged(nameof(SundayTime));
            }
        }

        private double _total;
        public double Total
        {
            get
            {
                return _total;
            }
            private set
            {
                _total = value;
                RaisePropertyChanged(nameof(Total));
            }
        }

        private void SetTotal()
        {
            Total = MondayTime + TuesdayTime + WednesdayTime + ThursdayTime + FridayTime + SaturdayTime + SundayTime;
        }
    }
}