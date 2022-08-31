using LiveCharts;
using LiveCharts.Wpf;
using SOCE.Library.Db;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;


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

                CollectSubProjects();
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

        private ObservableCollection<EmployeeVisualModel> _employees;
        public ObservableCollection<EmployeeVisualModel> Employees
        {
            get { return _employees; }
            set
            {
                _employees = value;
                RaisePropertyChanged(nameof(Employees));
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

                RaisePropertyChanged(nameof(SelectedSubproject));
            }
        }

        public ProjectDataVM()
        {
            LoadProjects();

            List<EmployeeVisualModel> evm = new List<EmployeeVisualModel>();
            EmployeeVisualModel evm1 = new EmployeeVisualModel() { Name = "Norm Noe", Rate = 250, SumHours = 100, VisualColor = new Color(). }


            OverallData = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Series 1",
                    Values = new ChartValues<double> { 4, 6, 5, 2 ,4 }
                },
                new LineSeries
                {
                    Title = "Series 2",
                    Values = new ChartValues<double> { 6, 7, 3, 4 ,6 },
                    PointGeometry = null
                },
                new LineSeries
                {
                    Title = "Series 3",
                    Values = new ChartValues<double> { 4,2,7,2,7 },
                    PointGeometry = DefaultGeometries.Square,
                    PointGeometrySize = 15
                }
            };
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
    }
}
