using LiveCharts;
using LiveCharts.Wpf;
using SOCE.Library.Db;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using LiveCharts.Defaults;
using System.Globalization;

namespace SOCE.Library.UI.ViewModels
{
    public class ProjectDataVM : BaseVM
    {
        private SeriesCollection _overallData = new SeriesCollection();
        public SeriesCollection OverallData
        {
            get { return _overallData; }
            set
            {
                _overallData = value;
                RaisePropertyChanged(nameof(OverallData));
            }
        }

        public Func<double, string> XFormatter { get; set; }

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
                    CollectSubProjects();
                    LoadGraphData();
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
                    if (SubProjects.Count>0)
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

        private double _totalHourSpent = 50;
        public double TotalHourSpent
        {
            get
            {
                return _totalHourSpent;
            }
            set
            {
                _totalHourSpent = value;
                RaisePropertyChanged(nameof(TotalHourSpent));
            }
        }

        private double _totalBudgetSpent = 50;
        public double TotalBudgetSpent
        {
            get
            {
                return _totalBudgetSpent;
            }
            set
            {
                _totalBudgetSpent = value;
                RaisePropertyChanged(nameof(TotalBudgetSpent));
            }
        }

        private double _budgetLeft = 20;
        public double BudgetLeft
        {
            get
            {
                return _budgetLeft;
            }
            set
            {
                _budgetLeft = value;
                RaisePropertyChanged(nameof(BudgetLeft));
            }
        }

        private double _totalbudget = 100;
        public double TotalBudget
        {
            get
            {
                return _totalbudget;
            }
            set
            {
                _totalbudget = value;
                RaisePropertyChanged(nameof(TotalBudget));
            }
        }

        private double _estimatedhoursleft = 15;
        public double EstimatedHoursLeft
        {
            get
            {
                return _estimatedhoursleft;
            }
            set
            {
                _estimatedhoursleft = value;
                RaisePropertyChanged(nameof(EstimatedHoursLeft));
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
                if (_selectedSubproject != null)
                {
                    LoadGraphData();
                }
                RaisePropertyChanged(nameof(SelectedSubproject));
            }
        }

        public ProjectDataVM()
        {
            LoadProjects();

            //List<EmployeeVisualModel> evm = new List<EmployeeVisualModel>();

            //evm.Add(new EmployeeVisualModel() { Name = "Norm Noe", Rate = 150, SumHours = 100, VisualColor = Brushes.Red });
            //evm.Add(new EmployeeVisualModel() { Name = "Guy Mazzotta", Rate = 250, SumHours = 50, VisualColor = Brushes.Blue });
            //evm.Add(new EmployeeVisualModel() { Name = "Ben Schieber", Rate = 120, SumHours = 150, VisualColor = Brushes.Purple });
            //RelevantEmployees = new ObservableCollection<EmployeeVisualModel>(evm);

            //OverallData = new SeriesCollection
            //{
            //    new LineSeries
            //    {
            //        Title = "Series 1",
            //        Values = new ChartValues<double> { 4, 6, 5, 2 ,4 }
            //    },
            //    new LineSeries
            //    {
            //        Title = "Series 2",
            //        Values = new ChartValues<double> { 6, 7, 3, 4 ,6 },
            //    },
            //    new LineSeries
            //    {
            //        Title = "Series 3",
            //        Values = new ChartValues<double> { 4,2,7,2,7 },
            //    }
            //};
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

        private void LoadGraphData() //get approved only data
        {
            RelevantEmployees.Clear();
            OverallData.Clear();
            List<TimesheetRowDbModel> total = new List<TimesheetRowDbModel>();
            if (ProjectofInterest != null)
            {
                if (SelectedSubproject == null)
                {
                    //total
                    //get all subprojectIds associated with projectId
                    List<SubProjectDbModel> subdbmodels = SQLAccess.LoadSubProjectsByProject(ProjectofInterest.Id);

                    foreach(SubProjectDbModel spdm in subdbmodels)
                    {
                        List<TimesheetRowDbModel> tmdata = SQLAccess.LoadTimeSheetDatabySubId(spdm.Id);
                        total.AddRange(tmdata);
                    }
                    
                }
                else
                {
                    //load by subprojectId
                    List<TimesheetRowDbModel> tmdata = SQLAccess.LoadTimeSheetDatabySubId(SelectedSubproject.Id);
                    total.AddRange(tmdata);
                }
            }

            if (total.Count>0)
            {
                //filter by employee Id
                var grouped = total.OrderBy(x => x.EmployeeId)
                   .GroupBy(x => x.EmployeeId);
                //order by date

                foreach(var item in grouped)
                {
                    //lookup employee
                    EmployeeDbModel employee = SQLAccess.LoadEmployeeById(item.Key);

                    if (employee !=null)
                    {
                        //List<TimesheetRowDbModel> employeetimesheetdata = new List<TimesheetRowDbModel>();
                        ChartValues<DateTimePoint> values = new ChartValues<DateTimePoint>();

                        //order by date
                        List<TimesheetRowDbModel> employeetimesheetdata = item.OrderBy(x => x.Date).ToList();

                        

                        double sum = 0;
                        foreach (TimesheetRowDbModel data in employeetimesheetdata)
                        {
                            DateTime dt = DateTime.ParseExact(data.Date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                            values.Add(new DateTimePoint(dt, data.TimeEntry));
                            sum = +data.TimeEntry;
                        }

                        //get employee by Id
                        EmployeeVisualModel evm = new EmployeeVisualModel(employee);
                        evm.Rate = 250;
                        evm.SumHours = sum;

                        Brush b = RandomColorGenerator();
                        evm.VisualColor = b;

                        RelevantEmployees.Add(evm);

                        LineSeries ls = new LineSeries
                        {
                            Title = evm.Name,
                            Values = values,
                            PointGeometrySize = 1,
                            
                        };

                        XFormatter = value => new System.DateTime((long)(value * TimeSpan.FromDays(1).Ticks)).ToString("t");
                        OverallData.Add(ls);
                    }



                }

                //do stuff
            }

        }

        private Brush RandomColorGenerator()
        {
            Random rnd = new Random();
            Byte[] b = new Byte[3];
            rnd.NextBytes(b);
            Color color = Color.FromRgb(b[0], b[1], b[2]);
            SolidColorBrush brush = new SolidColorBrush(color);
            return brush;
        }
    }
}
