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
                CollectDates();
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
            //DateTime current = DateTime.Now.Date;

            //collect timesheet closest 


            //set Row Data


            //collect Project list from database





            //For demonstration purposes
            List<ProjectModel> pm = new List<ProjectModel>();
            pm.Add(new ProjectModel { ProjectName = "DSD1 Delivery Station", JobNum = 223501, Description = "", IsAdservice = false });
            pm.Add(new ProjectModel { ProjectName = "East 55th St.", JobNum = 228103, Description = "", IsAdservice = false });
            pm.Add(new ProjectModel { ProjectName = "Byers Subaru", JobNum = 220103, Description = "", IsAdservice = false });
            pm.Add(new ProjectModel { ProjectName = "CMH086", JobNum = 211116, Description = "", IsAdservice = false });
            pm.Add(new ProjectModel { ProjectName = "John Hinderer", JobNum = 210109.2, Description = "", IsAdservice = true });
            pm.Add(new ProjectModel { ProjectName = "Germain Ford", JobNum = 210118, Description = "", IsAdservice = false });
            pm.Add(new ProjectModel { ProjectName = "Germain Ford", JobNum = 210118.3, Description = "", IsAdservice = true });
            pm.Add(new ProjectModel { ProjectName = "ABQ TNS", JobNum = 211125.6, Description = "", IsAdservice = true });
            ProjectList = new ObservableCollection<ProjectModel>(pm);

            //get current date
            DateTime current = DateTime.Now.Date;
            DateTime final = current.AddDays(16);

            int diff = (final - current).Days;

            List<TREntryModel> trentrymodels = new List<TREntryModel>();

            for(int i = 0; i < diff;i++)
            {
                BlankEntry.Add(new TREntryModel{ Date = current.AddDays(i)});
            }

            TrulyObservableCollection<TimesheetRowModel> members = new TrulyObservableCollection<TimesheetRowModel>();
            members.Add(new TimesheetRowModel { Project = pm[0], Entries = AddNewBlankRow()});
            members.Add(new TimesheetRowModel { Project = pm[1], Entries = AddNewBlankRow()});
            Rowdata = members;

            this.AddRowCommand = new RelayCommand(AddRowToCollection);
            this.SubmitTimeSheetCommand = new RelayCommand(SubmitTimesheet);
            this.SubmitTimeSheetCommand = new RelayCommand(ExportWorkReport);
            this.RemoveRowCommand = new RelayCommand<TimesheetRowModel>(RemoveRow);
            SumTable();
        }

        private void AddRowToCollection()
        {
            Rowdata.Add(new TimesheetRowModel {Entries = AddNewBlankRow()});
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

        private void CollectDates()
        {
            if (Rowdata.Count > 1)
            {
                TimesheetRowModel trmfirst = Rowdata[0];

                List<DateWrapper> dates = new List<DateWrapper>();

                foreach(TREntryModel dt in trmfirst.Entries)
                {
                    dates.Add(new DateWrapper(dt.Date));
                }

                DateSummary = new ObservableCollection<DateWrapper>(dates);

                DateTime startdate = dates[0].Value;
                MonthYearString = $"{startdate.ToString("MMMM")} {startdate.Year}";
                DateString = $"{startdate.Day}";

            }
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

    }
}
