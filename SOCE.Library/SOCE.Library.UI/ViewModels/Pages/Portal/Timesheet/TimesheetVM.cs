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
using System.Globalization;

namespace SOCE.Library.UI.ViewModels
{
    public class TimesheetVM : BaseVM
    {
        public List<RegisteredTimesheetDataModel> TimesheetData;
        public ICommand AddRowCommand { get; set; }
        public ICommand WorkReportCommand { get; set; }
        public ICommand SubmitTimeSheetCommand { get; set; }
        public ICommand SaveTimesheetCommand { get; set; }
        public ICommand RemoveRowCommand { get; set; }

        public List<TimesheetRowModel> CopiedTimesheetData { get; set; } = new List<TimesheetRowModel>();

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
            //DateTime current = DateTime.Now.Date;
            //DateTime final = current.AddDays(16);

            //int diff = (final - current).Days;

            //List<TREntryModel> trentrymodels = new List<TREntryModel>();

            //for (int i = 0; i < diff; i++)
            //{
            //    BlankEntry.Add(new TREntryModel { Date = current.AddDays(i) });
            //}
            LoadCurrentTimesheet(DateTime.Now);
            SetDates();
            LoadProjects();
            LoadTimesheetData();
            this.AddRowCommand = new RelayCommand(AddRowToCollection);
            this.SubmitTimeSheetCommand = new RelayCommand(SubmitTimesheet);
            this.SubmitTimeSheetCommand = new RelayCommand(ExportWorkReport);
            this.RemoveRowCommand = new RelayCommand<TimesheetRowModel>(RemoveRow);
            this.SaveTimesheetCommand = new RelayCommand(SaveCommand);
            SumTable();
        }

        private void AddRowToCollection()
        {
            Rowdata.Add(new TimesheetRowModel { Entries = AddNewBlankRow() });
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
            DateTime lastdate = dates.Last().Value;
            MonthYearString = $"{startdate.ToString("MMMM")} {startdate.Year}";
            DateString = $"[{startdate.Day} - {lastdate.Day}]";
        }

        private void SumTable()
        {
            if (Rowdata.Count > 1)
            {
                TotalHeader.Clear();
                int numofentries = Rowdata[0].Entries.Count();

                for (int i = 0; i < numofentries; i++)
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

            foreach (TREntryModel tr in BlankEntry)
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

        private void LoadTimesheetData()
        {
            DateTime datestart = DateSummary.First().Value;
            DateTime dateend = DateSummary.Last().Value;

            //update employee Id
            List<TimesheetRowDbModel> dbtimesheetdata = SQLAccess.LoadTimeSheet(datestart, dateend, 0);

            ObservableCollection<TimesheetRowModel> members = new ObservableCollection<TimesheetRowModel>();

            var groupedlist = dbtimesheetdata.OrderBy(x => x.SubProjectId).GroupBy(x => x.SubProjectId).ToList();

            foreach (var item in groupedlist)
            {
                TimesheetRowDbModel subitem = item.First();
                SubProjectDbModel spdb = SQLAccess.LoadSubProjectsBySubProject(subitem.SubProjectId);
                ProjectDbModel pdb = SQLAccess.LoadProjectsById(spdb.ProjectId);

                ProjectModel pm = new ProjectModel(pdb);
                SubProjectModel spm = new SubProjectModel(spdb);

                ProjectModel pmnew = ProjectList.Where(x => x.Id == pm.Id)?.First();

                TimesheetRowModel trm = new TimesheetRowModel()
                {
                    Project = pmnew
                };

                SubProjectModel subpmnew = trm.SubProjects.Where(x => x.Id == spm.Id)?.First();

                trm.SelectedSubproject = subpmnew;

                foreach (TimesheetRowDbModel trdm in item)
                {
                    DateTime dt = DateTime.ParseExact(trdm.Date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                    trm.Entries.Add(new TREntryModel() { Date = dt, TimeEntry = trdm.TimeEntry });
                }

                DateTime dateinc = datestart;

                while (dateinc <= dateend)
                {
                    if (!trm.Entries.Any(x => x.Date == dateinc))
                    {
                        //add
                        trm.Entries.Add(new TREntryModel() { Date = dateinc, TimeEntry = 0 });
                    }
                    dateinc = dateinc.AddDays(1);
                }
                trm.Entries = new ObservableCollection<TREntryModel>(trm.Entries.OrderBy(x => x.Date).ToList());
                members.Add(trm);
            }

            Rowdata = members;

            foreach(TimesheetRowModel trm in Rowdata)
            {
                CopiedTimesheetData.Add((TimesheetRowModel)trm.Clone());
            }

        }

        private void SaveCommand()
        {
            //need to include employee Id in here

            //adding and modifying
            foreach (TimesheetRowModel trm in Rowdata)
            {
                //adding or modifying an existing submission
                foreach (TREntryModel trentry in trm.Entries)
                {
                    if (trentry.TimeEntry > 0)
                    {
                        TimesheetRowDbModel dbmodel = new TimesheetRowDbModel()
                        {
                            EmployeeId = 0,
                            SubProjectId = trm.SelectedSubproject.Id,
                            Date = (int)long.Parse(trentry.Date.ToString("yyyyMMdd")),
                            Submitted = 0,
                            Approved = 0,
                            TimeEntry = trentry.TimeEntry
                        };

                        SQLAccess.AddTimesheetData(dbmodel);
                        //get data that needs to be removed
                    }
                }
            }

            //deleting
            foreach (TimesheetRowModel ctrm in CopiedTimesheetData)
            {
                int index = Rowdata.ToList().FindIndex(x => x.SelectedSubproject.Id == ctrm.SelectedSubproject.Id);

                //it exists
                if (index != -1)
                {
                    TimesheetRowModel trmfound = Rowdata[index];

                    foreach (TREntryModel trentry in ctrm.Entries)
                    {
                        //think about this expression
                        if (!trmfound.Entries.Any(x=>x.Date == trentry.Date && x.TimeEntry == trentry.TimeEntry))
                        {
                            if (trentry.TimeEntry > 0)
                            {
                                //delete
                                TimesheetRowDbModel trdbm = SQLAccess.LoadTimeSheetData(0, ctrm.SelectedSubproject.Id, trentry.Date);
                                SQLAccess.DeleteTimesheetData(trdbm.Id);
                            }  
                        }
                    }

                }
                else
                {
                    //delete all
                    foreach(TREntryModel trentry in ctrm.Entries)
                    {
                        if (trentry.TimeEntry > 0)
                        {
                            TimesheetRowDbModel trdbm = SQLAccess.LoadTimeSheetData(0, ctrm.SelectedSubproject.Id, trentry.Date);
                            SQLAccess.DeleteTimesheetData(trdbm.Id);
                        }
                            
                    }
                }        
            }

            CopiedTimesheetData = Rowdata.ToList();
        }


        private void LoadCurrentTimesheet(DateTime currdate)
        {
            DateTime firstdate;
            DateTime lastdate;
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

            List<TREntryModel> trentrymodels = new List<TREntryModel>();

            for (int i = 0; i < diff; i++)
            {
                BlankEntry.Add(new TREntryModel { Date = firstdate.AddDays(i) });
            }
        }
    }
}
