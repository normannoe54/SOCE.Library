using System;
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

namespace SOCE.Library.UI.ViewModels
{
    public class TimesheetVM : BaseVM
    {
        public List<RegisteredTimesheetDataModel> TimesheetData;
        public ICommand AddRowCommand { get; set; }
        public ICommand WorkReportCommand { get; set; }
        public ICommand SubmitTimeSheetCommand { get; set; }
        public ICommand RemoveRowCommand { get; set; }

        private ObservableCollection<TimesheetRowModel> _rowdata = new ObservableCollection<TimesheetRowModel>();
        public ObservableCollection<TimesheetRowModel> Rowdata
        {
            get { return _rowdata; }
            set
            {
                _rowdata = value;
                SumTable();
                //CollectDates();
                RaisePropertyChanged(nameof(Rowdata));
            }
        }

        private ObservableCollection<ProjectModel> _projectList;
        public ObservableCollection<ProjectModel> ProjectList
        {
            get { return _projectList; }
            set
            {
                _projectList = value;
                RaisePropertyChanged(nameof(ProjectList));
            }
        }

        private TimesheetRowModel _selectedRow = new TimesheetRowModel();
        public TimesheetRowModel SelectedRow
        {
            get { return _selectedRow; }
            set
            {
                _selectedRow = value;
                RaisePropertyChanged(nameof(SelectedRow));
            }
        }

        private ObservableCollection<DoubleWrapper> _totalHeader = new ObservableCollection<DoubleWrapper>();
        public ObservableCollection<DoubleWrapper> TotalHeader
        {
            get { return _totalHeader; }
            set
            {
                _totalHeader = value;
                RaisePropertyChanged(nameof(TotalHeader));
            }
        }

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

        private ObservableCollection<TREntryModel> BlankEntry = new ObservableCollection<TREntryModel>();

        public TimesheetVM()
        {
            //get timesheet data from database
            List<RegisteredTimesheetDataModel> rtdm = new List<RegisteredTimesheetDataModel>();

            //get current date
            DateTime current = DateTime.Now.Date;
            DateTime final = current.AddDays(16);

            int diff = (final - current).Days;

            List<TREntryModel> trentrymodels = new List<TREntryModel>();

            for(int i = 0; i < diff;i++)
            {
                BlankEntry.Add(new TREntryModel{ Date = current.AddDays(i)});
            }

            SetDates();
            LoadProjects();
            
            this.AddRowCommand = new RelayCommand(AddRowToCollection);
            this.SubmitTimeSheetCommand = new RelayCommand(SubmitTimesheet);
            this.SubmitTimeSheetCommand = new RelayCommand(ExportWorkReport);
            this.RemoveRowCommand = new RelayCommand<TimesheetRowModel>(RemoveRow);
            SumTable();
        }

        private void AddRowToCollection()
        {   
            Rowdata.Add(new TimesheetRowModel {Entries = AddNewBlankRow()});
            //CollectDates();
        }

        private void RemoveRow(TimesheetRowModel trm)
        {
            Rowdata.Remove(trm);
        }

        private void SubmitTimesheet()
        {
            //create new blanktimesheet
        }


        private void ExportWorkReport()
        {

        }

        private void SetDates()
        {
            List<DateWrapper> dates = new List<DateWrapper>();
            foreach (TREntryModel dt in BlankEntry)
            {
                dates.Add(new DateWrapper(dt.Date));
            }

            DateSummary = new ObservableCollection<DateWrapper>(dates);
            DateTime startdate = dates[0].Value;
            MonthYearString = $"{startdate.ToString("MMMM")} {startdate.Year}";
            DateString = $"{startdate.Day}";
        }

        private void SumTable()
        {
            if (Rowdata.Count > 1)
            {
                TotalHeader.Clear();
                int numofentries = Rowdata[0].Entries.Count();

                for (int i = 0; i < numofentries;i++)
                {
                    double total = 0;

                    foreach (TimesheetRowModel trm in Rowdata)
                    {
                        total = trm.Entries[i].TimeEntry;
                    }

                    TotalHeader.Add(new DoubleWrapper(total));
                }
            } 
        }

        private ObservableCollection<TREntryModel> AddNewBlankRow()
        {
            ObservableCollection<TREntryModel> newblank = new ObservableCollection<TREntryModel>();

            foreach(TREntryModel tr in BlankEntry)
            {
                newblank.Add(new TREntryModel() { Date = tr.Date, TimeEntry = tr.TimeEntry });
            }
            return newblank;
        }

        private void LoadProjects()
        {
            List<ProjectDbModel> dbprojects = SQLAccess.LoadProjects();

            ObservableCollection<ProjectModel> members = new ObservableCollection<ProjectModel>();

            foreach (ProjectDbModel pdb in dbprojects)
            {
                members.Add(new ProjectModel(pdb));
            }

            ProjectList = members;
        }

        
    }
}
