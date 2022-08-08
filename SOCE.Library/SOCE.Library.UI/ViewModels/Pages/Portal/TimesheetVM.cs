using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using SOCE.Library.Models.Accounts;
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

        private TimesheetRowModel _totalHeader = new TimesheetRowModel();
        public TimesheetRowModel TotalHeader
        {
            get { return _totalHeader; }
            set
            {
                _totalHeader = value;
                RaisePropertyChanged(nameof(TotalHeader));
            }
        }

        private ObservableCollection<TREntryModel> BlankEntry = new ObservableCollection<TREntryModel>();

        public TimesheetVM()
        {
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
            Rowdata.CollectionChanged += ContentCollectionChanged;
        }

        public void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Rowdata.CollectionChanged -= ContentCollectionChanged;
            SumTable();
            Rowdata.CollectionChanged += ContentCollectionChanged;
        }

        private void AddRowToCollection()
        {

            Rowdata.Add(new TimesheetRowModel { Project = new ProjectModel { ProjectName = "" } });
        }

        private void RemoveRow(TimesheetRowModel trm)
        {
            Rowdata.Remove(trm);
        }

        private void SubmitTimesheet()
        {

        }


        private void ExportWorkReport()
        {

        }

        private void SumTable()
        {
            double m = 0;
            double tu = 0;
            double w = 0;
            double th = 0;
            double f = 0;
            double sat = 0;
            double sun = 0;

            for (int i = 0; i < Rowdata.Count; i++)
            {
                m += Rowdata[i].MondayTime;
                tu += +Rowdata[i].TuesdayTime;
                w += Rowdata[i].WednesdayTime;
                th += Rowdata[i].ThursdayTime;
                f += Rowdata[i].FridayTime;
                sat += Rowdata[i].SaturdayTime;
                sun += Rowdata[i].SundayTime;
            }

            TotalHeader.MondayTime = m;
            TotalHeader.TuesdayTime = tu;
            TotalHeader.WednesdayTime = w;
            TotalHeader.ThursdayTime = th;
            TotalHeader.FridayTime = f;
            TotalHeader.SaturdayTime = sat;
            TotalHeader.SundayTime = sun;
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
