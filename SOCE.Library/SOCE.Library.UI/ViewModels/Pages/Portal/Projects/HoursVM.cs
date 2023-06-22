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
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;
using System.Threading.Tasks;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Painting.Effects;


namespace SOCE.Library.UI.ViewModels
{
    public class HoursVM : BaseVM
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

        //private List<SeriesDataModel> SeriesData = new List<SeriesDataModel>();
        private ObservableCollection<ISeries> _overallData = new ObservableCollection<ISeries>();
        public ObservableCollection<ISeries> OverallData
        {
            get { return _overallData; }
            set
            {
                _overallData = value;
                RaisePropertyChanged(nameof(OverallData));
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

        private SubProjectModel _selectedProjectPhase;
        public SubProjectModel SelectedProjectPhase
        {
            get
            {
                return _selectedProjectPhase;
            }
            set
            {
                _selectedProjectPhase = value;
                if (_selectedProjectPhase != null)
                {
                    LoadGraphData();
                }
                RaisePropertyChanged(nameof(SelectedProjectPhase));
            }
        }

        private ProjectModel _baseProject;
        public ProjectModel BaseProject
        {
            get { return _baseProject; }
            set
            {
                _baseProject = value;
                RaisePropertyChanged("BaseProject");
            }
        }

        private static readonly SKColor background = new SKColor(195, 195, 195);
        private static readonly SKColor s_gray1 = new SKColor(160, 160, 160);
        private static readonly SKColor seperators = new SKColor(90, 90, 90);
        private static readonly SKColor outline = new SKColor(60, 60, 60);
        private static readonly SKColor s_crosshair = new SKColor(255, 171, 145);

        public Axis[] XAxes { get; set; } =
        {
        new Axis
        {
            Labeler = value => new DateTime((long) value).ToString("MM/dd/yy"),
            MinStep = TimeSpan.FromDays(1).Ticks,
            Name = "Date Saved",
            NamePaint = new SolidColorPaint(SKColors.Black),
            TextSize = 12,
            UnitWidth = 0.09,
            Padding = new Padding(5, 15, 5, 5),
            LabelsPaint = new SolidColorPaint(SKColors.Black),
            SeparatorsPaint = new SolidColorPaint
            {
                    Color = seperators,
                    StrokeThickness = 1,
                    ZIndex = 1
            },
            }
        };

        public Axis[] YAxes { get; set; } =
        {
        new Axis
        {
            Name = "Total Hours (hr.)",
            NamePaint = new SolidColorPaint(SKColors.Black),
            TextSize = 12,
            UnitWidth = 0.09,
            Padding = new Padding(5, 15, 5, 5),
            LabelsPaint = new SolidColorPaint(SKColors.Black),
            SeparatorsPaint = new SolidColorPaint
            {
                    Color = seperators,
                    StrokeThickness = 1,
                    ZIndex = 1,
                    PathEffect = new DashEffect(new float[] { 3, 3 })
            },
            }
        };

        public DrawMarginFrame Frame { get; set; } =
        new DrawMarginFrame()
        {
            Fill = new SolidColorPaint(background),
            Stroke = new SolidColorPaint
            {
                Color = outline,
                StrokeThickness = 1,
                ZIndex = 2
            }
        };

        public HoursVM(ProjectModel pm, EmployeeModel employee)
        {
            RelevantEmployees.CollectionChanged += this.EmployeesChanged;
            CurrentEmployee = employee;
            BaseProject = pm;
            pm.FormatData(true);
            SubProjects = pm.SubProjects;

            SelectedProjectPhase = SubProjects[SubProjects.Count - 1];
        }

        private void EmployeesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged added in e?.NewItems)
                {
                    added.PropertyChanged += ItemModificationOnPropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged added in e?.OldItems)
                {
                    added.PropertyChanged -= ItemModificationOnPropertyChanged;
                }
            }
        }

        private void ItemModificationOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            EmployeeVisualModel evm = sender as EmployeeVisualModel;
            foreach (LineSeries<DateTimePoint> ls in OverallData)
            {
                if (ls.Name == evm.Name)
                {
                    if (evm.SelectedCurr)
                    {
                        ls.IsVisible = true;
                    }
                    else
                    {
                        ls.IsVisible = false;
                    }
                    break;
                }
            }
        }

        private void LoadGraphData()
        {
            RelevantEmployees.Clear();
            OverallData.Clear();

            List<TimesheetRowDbModel> total = new List<TimesheetRowDbModel>();

            if (SelectedProjectPhase.Id == 0)
            {
                //total
                //get all subprojectIds associated with projectId
                List<SubProjectDbModel> subdbmodels = SQLAccess.LoadSubProjectsByProject(SelectedProjectPhase.baseproject.Id);

                foreach (SubProjectDbModel spdm in subdbmodels)
                {
                    List<TimesheetRowDbModel> tmdata = SQLAccess.LoadTimeSheetDatabySubId(spdm.Id);
                    total.AddRange(tmdata);
                }
            }
            else
            {
                //load by subprojectId
                List<TimesheetRowDbModel> tmdata = SQLAccess.LoadTimeSheetDatabySubId(SelectedProjectPhase.Id);
                total.AddRange(tmdata);
            }

            if (total.Count > 0)
            {
                //filter by employee Id
                var grouped = total.OrderBy(x => x.EmployeeId).GroupBy(x => x.EmployeeId);
                //order by date
                int j = 0;
                foreach (var item in grouped)
                {
                    //lookup employee
                    EmployeeDbModel employee = SQLAccess.LoadEmployeeById(item.Key);

                    if (employee != null)
                    {
                        //order by date
                        List<TimesheetRowDbModel> employeetimesheetdata = item.OrderBy(x => x.Date).ToList();

                        EmployeeVisualModel evm = new EmployeeVisualModel(employee);
                        evm.SumHours = employeetimesheetdata.Sum(x => x.TimeEntry);

                        SolidColorBrush b;
                        if (j < colorcodes.Count)
                        {
                            b = (SolidColorBrush)new BrushConverter().ConvertFrom(colorcodes[j]);
                        }
                        else
                        {
                            b = RandomColorGenerator();
                        }

                        evm.VisualColor = b;

                        OverallData.Add(CreateLineSeries(evm, employeetimesheetdata));
                        //SeriesData.Add(new SeriesDataModel(evm, employeetimesheetdata));
                        RelevantEmployees.Add(evm);
                    }

                    j++;
                }
            }
        }

        public LineSeries<DateTimePoint> CreateLineSeries(EmployeeVisualModel EmployeeVis, List<TimesheetRowDbModel> TimesheetData)
        {
            List<DateTimePoint> values = new List<DateTimePoint>();

            //need to 
            List<TimesheetRowDbModel> mergedList = TimesheetData.GroupBy(x => x.Date).Select(g => new TimesheetRowDbModel
            {
                Date = g.Key,
                TimeEntry = g.Sum(r => r.TimeEntry)
            }).ToList();

            double sum = 0;

            //incremental
            foreach (TimesheetRowDbModel data in mergedList)
            {
                DateTime dt = DateTime.ParseExact(data.Date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                DateTimePoint dtp = new DateTimePoint() { DateTime = dt };

                //if (SelectedDataType == DataTypeEnum.IncBudget)
                //{
                //    dtp.Value = data.TimeEntry * EmployeeVis.Rate;
                //}
                //else if (SelectedDataType == DataTypeEnum.IncHours)
                //{
                //    dtp.Value = data.TimeEntry;
                //}
                //else if (SelectedDataType == DataTypeEnum.TotalBudget)
                //{
                //    sum += data.TimeEntry;
                //    dtp.Value = sum * EmployeeVis.Rate;
                //}
                //else
                //{
                sum += data.TimeEntry;
                dtp.Value = sum;
                //}

                values.Add(dtp);
            }

            LineSeries<DateTimePoint> ls = new LineSeries<DateTimePoint>
            {
                TooltipLabelFormatter = (chartPoint) => $"{EmployeeVis.Name} [{chartPoint.PrimaryValue}hr.]",
                Name = EmployeeVis.Name,
                Values = values,
                GeometrySize = 10,
                LineSmoothness = 0,
                Stroke = new SolidColorPaint(new SKColor(EmployeeVis.VisualColor.Color.R, EmployeeVis.VisualColor.Color.G, EmployeeVis.VisualColor.Color.B)) { StrokeThickness = 2 },
                GeometryStroke = new SolidColorPaint(new SKColor(EmployeeVis.VisualColor.Color.R, EmployeeVis.VisualColor.Color.G, EmployeeVis.VisualColor.Color.B)),
                GeometryFill = new SolidColorPaint(SKColors.White),
                Fill = null
            };

            return ls;
        }


        private Random r = new Random();
        /// <summary>
        /// Create Color
        /// </summary>
        /// <returns></returns>
        private SolidColorBrush RandomColorGenerator()
        {
            Byte[] b = new Byte[3];
            r.NextBytes(b);
            Color color = Color.FromRgb(b[0], b[1], b[2]);
            SolidColorBrush brush = new SolidColorBrush(color);
            return brush;
        }

        private List<string> colorcodes = new List<string>()
        {
"#000000",
"#00FF00",
"#0000FF",
"#FF0000",
"#01FFFE",
"#FFA6FE",
"#FFDB66",
"#006401",
"#010067",
"#95003A",
"#007DB5",
"#FF00F6",
"#FFEEE8",
"#774D00",
"#90FB92",
"#0076FF",
"#D5FF00",
"#FF937E",
"#6A826C",
"#FF029D",
"#FE8900",
"#7A4782",
"#7E2DD2",
"#85A900",
"#FF0056",
"#A42400",
"#00AE7E",
"#683D3B",
"#BDC6FF",
"#263400",
"#BDD393",
"#00B917",
"#9E008E",
"#001544",
"#C28C9F",
"#FF74A3",
"#01D0FF",
"#004754",
"#E56FFE",
"#788231",
"#0E4CA1",
"#91D0CB",
"#BE9970",
"#968AE8",
"#BB8800",
"#43002C",
"#DEFF74",
"#00FFC6",
"#FFE502",
"#620E00",
"#008F9C",
"#98FF52",
"#7544B1",
"#B500FF",
"#00FF78",
"#FF6E41",
"#005F39",
"#6B6882",
"#5FAD4E",
"#A75740",
"#A5FFD2",
"#FFB167",
"#009BFF",
"#E85EBE",
        };
    }
}
