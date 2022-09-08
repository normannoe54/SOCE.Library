using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using SOCE.Library.Db;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SOCE.Library.UI
{
    public class SeriesDataModel
    {
        public EmployeeVisualModel EmployeeVis { get; set; }

        public List<TimesheetRowDbModel> TimesheetData { get; set; } = new List<TimesheetRowDbModel>();

        public SeriesDataModel(EmployeeVisualModel employee, List<TimesheetRowDbModel> data)
        {
            EmployeeVis = employee;
            TimesheetData = data;
        }

        public LineSeries CreateLineSeries(TimeOptionEnum toe, DataTypeEnum dtype)
        {
            ChartValues<DateTimePoint> values = new ChartValues<DateTimePoint>();

            //need to 
            List<TimesheetRowDbModel> mergedList = TimesheetData.GroupBy(x => x.Date).Select(g => new TimesheetRowDbModel
            { Date = g.Key,
              TimeEntry = g.Sum(r => r.TimeEntry)
            }).ToList();

            double sum = 0;

            //incremental
            foreach (TimesheetRowDbModel data in mergedList)
            {
                DateTime dt = DateTime.ParseExact(data.Date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                DateTimePoint dtp = new DateTimePoint() { DateTime = dt};
                
                if (dtype == DataTypeEnum.IncBudget)
                {
                    dtp.Value = data.TimeEntry * EmployeeVis.Rate;
                }
                else if (dtype == DataTypeEnum.IncHours)
                {
                    dtp.Value = data.TimeEntry;
                }
                else if (dtype == DataTypeEnum.TotalBudget)
                {
                    sum += data.TimeEntry;
                    dtp.Value = sum * EmployeeVis.Rate;
                }
                else
                {
                    sum += data.TimeEntry;
                    dtp.Value = sum;
                }


                values.Add(dtp);
            }

            LineSeries ls = new LineSeries
            {
                Title = EmployeeVis.Name,
                Values = values,
                PointGeometrySize = 10,
                Stroke = EmployeeVis.VisualColor,
                Fill = new SolidColorBrush() { Opacity = 0, Color = Brushes.White.Color }
            };

            return ls;
        }


        //create seriesdata

    }
}
