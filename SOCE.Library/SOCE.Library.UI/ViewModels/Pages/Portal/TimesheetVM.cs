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

namespace SOCE.Library.UI.ViewModels
{
    public class TimesheetVM : BaseVM
    {
        private TrulyObservableCollection<TimesheetRowModel> _rowdata = new TrulyObservableCollection<TimesheetRowModel>();
        public TrulyObservableCollection<TimesheetRowModel> Rowdata
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


        //public TimesheetRowModel SelectedRowItem
        //{
        //    set
        //    {
        //        SumTotal();
        //    }
        //}


        public TimesheetVM()
        {          
            TrulyObservableCollection<TimesheetRowModel> members = new TrulyObservableCollection<TimesheetRowModel>();
            members.Add(new TimesheetRowModel {ProjectModel = new ProjectModel { ProjectName = "Total", JobNum = null }, MondayTime = 0, TuesdayTime = 0, WednesdayTime = 0, ThursdayTime = 0, FridayTime = 0, SaturdayTime = 0, SundayTime = 0 });
            members.Add(new TimesheetRowModel { ProjectModel = new ProjectModel { ProjectName = "AMAZON TNS", JobNum = 34123512 }, MondayTime = 1.5, TuesdayTime = 2, WednesdayTime = 0, ThursdayTime = 1, FridayTime = 0, SaturdayTime = 0, SundayTime = 0 });
            members.Add(new TimesheetRowModel { ProjectModel = new ProjectModel { ProjectName = "AMAZON TNS", JobNum = 34123512 }, MondayTime = 1.5, TuesdayTime = 2, WednesdayTime = 0, ThursdayTime = 1, FridayTime = 0, SaturdayTime = 0, SundayTime = 0 });
            members.Add(new TimesheetRowModel { ProjectModel = new ProjectModel { ProjectName = "AMAZON TNS", JobNum = 34123512 }, MondayTime = 1.5, TuesdayTime = 2, WednesdayTime = 0, ThursdayTime = 1, FridayTime = 0, SaturdayTime = 0, SundayTime = 0 });
            members.Add(new TimesheetRowModel { ProjectModel = new ProjectModel { ProjectName = "AMAZON TNS", JobNum = 34123512 }, MondayTime = 1.5, TuesdayTime = 2, WednesdayTime = 0, ThursdayTime = 1, FridayTime = 0, SaturdayTime = 0, SundayTime = 0 });
            members.Add(new TimesheetRowModel { ProjectModel = new ProjectModel { ProjectName = "AMAZON TNS", JobNum = 34123512 }, MondayTime = 1.5, TuesdayTime = 2, WednesdayTime = 0, ThursdayTime = 1, FridayTime = 0, SaturdayTime = 0, SundayTime = 0 }); 
            Rowdata = members;
            CheckProcess();
            Rowdata.CollectionChanged += ContentCollectionChanged;
        }

        public void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Rowdata.CollectionChanged -= ContentCollectionChanged;
            CheckProcess();
            Rowdata.CollectionChanged += ContentCollectionChanged;
        }

        private void CheckProcess()
        {
            SumTable();

            var lastitem = Rowdata.Last();

            if (!String.IsNullOrEmpty(lastitem.ProjectModel?.ProjectName))
            {
                Rowdata.Add(new TimesheetRowModel());
            }

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

            for (int i = 1; i < Rowdata.Count; i++)
            {
                m += Rowdata[i].MondayTime;
                tu += +Rowdata[i].TuesdayTime;
                w += Rowdata[i].WednesdayTime;
                th += Rowdata[i].ThursdayTime;
                f += Rowdata[i].FridayTime;
                sat += Rowdata[i].SaturdayTime;
                sun += Rowdata[i].SundayTime;
            }

            Rowdata[0].MondayTime = m;
            Rowdata[0].TuesdayTime = tu;
            Rowdata[0].WednesdayTime = w;
            Rowdata[0].ThursdayTime = th;
            Rowdata[0].FridayTime = f;
            Rowdata[0].SaturdayTime = sat;
            Rowdata[0].SundayTime = sun;
        }

        //    List<TimesheetRowModel> curr = Rowdata;
        //    curr.Re
        //    Rowdata[0] = new TimesheetRowModel { ProjectModel = new ProjectModel { ProjectName = "Total", JobNum = null }, MondayTime = montime, TuesdayTime = 2, WednesdayTime = 0, ThursdayTime = 1, FridayTime = 0, SaturdayTime = 0, SundayTime = 0 };
        //}
    }
}
