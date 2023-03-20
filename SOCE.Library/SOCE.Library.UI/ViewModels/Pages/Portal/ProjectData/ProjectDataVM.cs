using SOCE.Library.Db;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using System.Globalization;
using System.Collections.Specialized;
using System.ComponentModel;
//using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;
using System.Threading.Tasks;

namespace SOCE.Library.UI.ViewModels
{
    public class ProjectDataVM : BaseVM
    {
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
            }
        }

        private List<SeriesDataModel> SeriesData = new List<SeriesDataModel>();

        //private ISeries[] _overallData = new ISeries[];
        //public SeriesCollection OverallData
        //{
        //    get { return _overallData; }
        //    set
        //    {
        //        _overallData = value;
        //        RaisePropertyChanged(nameof(OverallData));
        //    }
        //}

        private bool _isChartVisible = false;
        public bool IsChartVisible
        {
            get { return _isChartVisible; }
            set
            {
                _isChartVisible = value;
                RaisePropertyChanged(nameof(IsChartVisible));
            }
        }

        public Func<double, string> Formatter { get; set; }

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

        private ProjectModel _projectofInterest = new ProjectModel { ProjectName = "" };
        public ProjectModel ProjectofInterest
        {
            get
            {
                return _projectofInterest;
            }
            set
            {
                _projectofInterest = value;
                //set selected item
                UnLockedSubs = false;

                if (_projectofInterest != null)
                {
                    _projectofInterest.FormatData(true);
                    //LoadGraphData();
                }

                RaisePropertyChanged(nameof(ProjectofInterest));
            }
        }

        private ObservableCollection<SubProjectModel> _subprojects;
        public ObservableCollection<SubProjectModel> SubProjects
        {
            get { return _subprojects; }
            set
            {
                _subprojects = value;
                RaisePropertyChanged(nameof(SubProjects));
            }
        }

        private bool _showActiveProjects = true;
        public bool ShowActiveProjects
        {
            get { return _unLockedSubs; }
            set
            {
                _showActiveProjects = value;
                LoadProjects();

                RaisePropertyChanged(nameof(ShowActiveProjects));
            }
        }

        private bool _unLockedSubs = false;
        public bool UnLockedSubs
        {
            get { return _unLockedSubs; }
            set
            {
                _unLockedSubs = value;

                if (!_unLockedSubs)
                {
                    SelectedSubproject = null;
                }
                else
                {
                    if (SubProjects.Count > 0)
                    {
                        SelectedSubproject = SubProjects[0];
                    }
                }

                RaisePropertyChanged(nameof(UnLockedSubs));
            }
        }


        private ObservableCollection<EmployeeVisualModel> _relevantEmployees = new ObservableCollection<EmployeeVisualModel>();
        public ObservableCollection<EmployeeVisualModel> RelevantEmployees
        {
            get { return _relevantEmployees; }
            set
            {
                _relevantEmployees = value;
                RaisePropertyChanged(nameof(RelevantEmployees));
            }
        }

        private TimeOptionEnum _selectedTimeSpan;
        public TimeOptionEnum SelectedTimeSpan
        {
            get
            {
                return _selectedTimeSpan;
            }
            set
            {
                _selectedTimeSpan = value;
                //FormatData();
                RaisePropertyChanged(nameof(SelectedTimeSpan));
            }
        }

        private DataTypeEnum _selectedDataType;
        public DataTypeEnum SelectedDataType
        {
            get
            {
                return _selectedDataType;
            }
            set
            {
                _selectedDataType = value;
                //FormatData();
                RaisePropertyChanged(nameof(SelectedDataType));
            }
        }

        private SubProjectModel _selectedSubproject;
        public SubProjectModel SelectedSubproject
        {
            get
            {
                return _selectedSubproject;
            }
            set
            {
                _selectedSubproject = value;
                //if (_selectedSubproject != null)
                //{
                //    LoadGraphData();
                //}
                RaisePropertyChanged(nameof(SelectedSubproject));
            }
        }

        public ProjectDataVM(EmployeeModel loggedinEmployee)
        {
            CurrentEmployee = loggedinEmployee;

            Formatter = value => new DateTime((long)value).ToString("yyyy-MM-dd");
            LoadProjects();

            //RelevantEmployees.CollectionChanged += this.EmployeesChanged;

        }

        //private void EmployeesChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    if (e.NewItems != null)
        //    {
        //        foreach (INotifyPropertyChanged added in e?.NewItems)
        //        {
        //            added.PropertyChanged += ItemModificationOnPropertyChanged;
        //        }
        //    }

        //    if (e.OldItems != null)
        //    {
        //        foreach (INotifyPropertyChanged added in e?.OldItems)
        //        {
        //            added.PropertyChanged -= ItemModificationOnPropertyChanged;
        //        }
        //    }
        //}

        //private void ItemModificationOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        //{
        //    EmployeeVisualModel evm = sender as EmployeeVisualModel;
        //    foreach (LineSeries<double> ls in OverallData)
        //    {
        //        if (ls.Title == evm.Name)
        //        {
        //            if (evm.SelectedCurr)
        //            {
        //                ls.Visibility = System.Windows.Visibility.Visible;
        //            }
        //            else
        //            {
        //                ls.Visibility = System.Windows.Visibility.Hidden;
        //            }
        //            break;
        //        }
        //    }
        //}

        private void LoadProjects()
        {
            ProjectList.Clear();

            List<ProjectDbModel> dbprojects = SQLAccess.LoadActiveProjects(ShowActiveProjects);

            ObservableCollection<ProjectModel> members = new ObservableCollection<ProjectModel>();

            foreach (ProjectDbModel pdb in dbprojects)
            {
                members.Add(new ProjectModel(pdb));
            }

            ProjectList = members;
        }

        private void CollectSubProjects()
        {

            if (ProjectofInterest == null)
            {
                return;
            }

            int id = ProjectofInterest.Id;

            List<SubProjectDbModel> subdbprojects = SQLAccess.LoadSubProjectsByProject(id);

            ObservableCollection<SubProjectModel> members = new ObservableCollection<SubProjectModel>();

            foreach (SubProjectDbModel sdb in subdbprojects)
            {
                members.Add(new SubProjectModel(sdb));
            }

            SubProjects = members;
        }

        //private void LoadGraphData()
        //{
        //    RelevantEmployees.Clear();
        //    SeriesData.Clear();

        //    List<TimesheetRowDbModel> total = new List<TimesheetRowDbModel>();
        //    if (ProjectofInterest != null)
        //    {
        //        if (SelectedSubproject == null)
        //        {
        //            //total
        //            //get all subprojectIds associated with projectId
        //            List<SubProjectDbModel> subdbmodels = SQLAccess.LoadSubProjectsByProject(ProjectofInterest.Id);

        //            foreach (SubProjectDbModel spdm in subdbmodels)
        //            {
        //                List<TimesheetRowDbModel> tmdata = SQLAccess.LoadTimeSheetDatabySubId(spdm.Id);
        //                total.AddRange(tmdata);
        //            }
        //        }
        //        else
        //        {
        //            //load by subprojectId
        //            List<TimesheetRowDbModel> tmdata = SQLAccess.LoadTimeSheetDatabySubId(SelectedSubproject.Id);
        //            total.AddRange(tmdata);
        //        }
        //    }

        //    if (total.Count > 0)
        //    {
        //        //filter by employee Id
        //        var grouped = total.OrderBy(x => x.EmployeeId)
        //           .GroupBy(x => x.EmployeeId);
        //        //order by date

        //        foreach (var item in grouped)
        //        {
        //            //lookup employee
        //            EmployeeDbModel employee = SQLAccess.LoadEmployeeById(item.Key);

        //            if (employee != null)
        //            {
        //                //List<TimesheetRowDbModel> employeetimesheetdata = new List<TimesheetRowDbModel>();
        //                ChartValues<DateTimePoint> values = new ChartValues<DateTimePoint>();

        //                //order by date
        //                List<TimesheetRowDbModel> employeetimesheetdata = item.OrderBy(x => x.Date).ToList();

        //                EmployeeVisualModel evm = new EmployeeVisualModel(employee);
        //                evm.SumHours = employeetimesheetdata.Sum(x => x.TimeEntry);
        //                Brush b = RandomColorGenerator();
        //                evm.VisualColor = b;

        //                SeriesData.Add(new SeriesDataModel(evm, employeetimesheetdata));
        //                RelevantEmployees.Add(evm);
        //            }
        //        }



        //    }
        //    FormatData();
        //}


        //private void FormatData()
        //{
        //    TotalHourSpent = 0;
        //    TotalBudgetSpent = 0;
        //    TotalBudget = 0;
        //    BudgetLeft = 0;
        //    EstimatedHoursLeft = 0;
        //    OverallData.Clear();

        //    double averagerate = 0;

        //    foreach (SeriesDataModel sdm in SeriesData)
        //    {
        //        LineSeries ls = sdm.CreateLineSeries(SelectedTimeSpan, SelectedDataType);
        //        OverallData.Add(ls);
        //        TotalHourSpent += sdm.EmployeeVis.SumHours;
        //        TotalBudgetSpent += sdm.EmployeeVis.SumHours * sdm.EmployeeVis.Rate;
        //        averagerate += sdm.EmployeeVis.Rate;
        //    }

        //    //CreateTotalLine();

        //    if (ProjectofInterest != null)
        //    {
        //        if (SelectedSubproject == null)
        //        {
        //            TotalBudget = ProjectofInterest.Fee;
        //            BudgetLeft = ProjectofInterest.Fee - TotalBudgetSpent;
        //        }
        //        else
        //        {
        //            TotalBudget = SelectedSubproject.Fee;
        //            BudgetLeft = SelectedSubproject.Fee - TotalBudgetSpent;
        //        }
        //    }

        //    EstimatedHoursLeft = Math.Max(0, BudgetLeft / (averagerate / SeriesData.Count));

        //    if (OverallData.Count > 0)
        //    {
        //        IsChartVisible = true;
        //    }
        //}

        //private void CreateTotalLine()
        //{
        //    List<DateTimePoint> values = new List<DateTimePoint>();

        //    foreach (LineSeries ls in OverallData)
        //    {
        //        foreach (DateTimePoint dtp in ls.Values)
        //        {
        //            values.Add(dtp);
        //        }
        //    }

        //    List<DateTimePoint> mergedList = values.GroupBy(x => x.DateTime).Select(g => new DateTimePoint
        //    {
        //        DateTime = g.Key,
        //        Value = g.Sum(r => r.Value)
        //    }).OrderBy(x => x.DateTime).ToList();

        //    //List<TimesheetRowDbModel> employeetimesheetdata = values.OrderBy(x => x.Date).ToList();

        //    //Add total
        //    EmployeeVisualModel evm = new EmployeeVisualModel();
        //    evm.Name = "Total";
        //    evm.Rate = 0;
        //    evm.VisualColor = Brushes.Black;
        //    evm.SelectedCurr = false;

        //    if (RelevantEmployees.Any(x => x.Name == "Total"))
        //    {
        //        RelevantEmployees[0] = evm;
        //    }
        //    else
        //    {
        //        RelevantEmployees.Insert(0, evm);
        //    }

        //    ChartValues<DateTimePoint> dtpts = new ChartValues<DateTimePoint>();

        //    double sumvalue = 0;
        //    foreach (DateTimePoint dtp in mergedList)
        //    {
        //        //if ()
        //        sumvalue += dtp.Value;
        //        DateTimePoint dtpnew = new DateTimePoint() { DateTime = dtp.DateTime };
        //        dtpnew.Value = sumvalue;
        //        dtpts.Add(dtpnew);
        //    }

        //    LineSeries lstot = new LineSeries
        //    {
        //        Title = evm.Name,
        //        Values = dtpts,
        //        PointGeometrySize = 10,
        //        Stroke = Brushes.Black,
        //        Fill = new SolidColorBrush() { Opacity = 0, Color = Brushes.White.Color },
        //        StrokeDashArray = new DoubleCollection { 2 },
        //    };

        //    OverallData.Add(lstot);

        //    ItemModificationOnPropertyChanged(evm, null);

        //}

        //private Random r = new Random();
        ///// <summary>
        ///// Create Color
        ///// </summary>
        ///// <returns></returns>
        //private Brush RandomColorGenerator()
        //{
        //    Byte[] b = new Byte[3];
        //    r.NextBytes(b);
        //    Color color = Color.FromRgb(b[0], b[1], b[2]);
        //    SolidColorBrush brush = new SolidColorBrush(color);
        //    return brush;
        //}
    }
}
