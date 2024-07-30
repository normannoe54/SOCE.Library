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
using MaterialDesignThemes.Wpf;
using SOCE.Library.UI.Views;
using System.Threading.Tasks;
using SOCE.Library.Excel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;

namespace SOCE.Library.UI.ViewModels
{
    public class ExpenseProjectVM : BaseVM
    {
        private bool _leftDrawerOpen = false;
        public bool LeftDrawerOpen
        {
            get { return _leftDrawerOpen; }
            set
            {
                _leftDrawerOpen = value;
                RaisePropertyChanged("LeftDrawerOpen");
            }
        }

        private UserControl _leftViewToShow = new UserControl();
        public UserControl LeftViewToShow
        {
            get { return _leftViewToShow; }
            set
            {
                _leftViewToShow = value;
                RaisePropertyChanged(nameof(LeftViewToShow));
            }
        }

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

        //public List<RegisteredTimesheetDataModel> TimesheetData;
        public ICommand AddExpenseCommandCommand { get; set; }
        public ICommand DeleteExpenseCommand { get; set; }

        private ObservableCollection<ExpenseProjectModel> _rowdata = new ObservableCollection<ExpenseProjectModel>();
        public ObservableCollection<ExpenseProjectModel> Rowdata
        {
            get { return _rowdata; }
            set
            {
                _rowdata = value;
                RaisePropertyChanged(nameof(Rowdata));
            }
        }

        private ProjectViewResModel _baseProject;
        public ProjectViewResModel BaseProject
        {
            get { return _baseProject; }
            set
            {
                _baseProject = value;
                RaisePropertyChanged("BaseProject");
            }
        }

        private double _total;
        public double Total
        {
            get { return _total; }
            set
            {
                _total = value;
                RaisePropertyChanged(nameof(Total));
            }
        }

        private double _pentotalTotal;
        public double PenTotalTotal
        {
            get { return _pentotalTotal; }
            set
            {
                _pentotalTotal = value;
                RaisePropertyChanged(nameof(PenTotalTotal));
            }
        }

        private double _totalTotal;
        public double TotalTotal
        {
            get { return _totalTotal; }
            set
            {
                _totalTotal = value;
                RaisePropertyChanged(nameof(TotalTotal));
            }
        }

        private double _invTotalTotal;
        public double InvTotalTotal
        {
            get { return _invTotalTotal; }
            set
            {
                _invTotalTotal = value;
                RaisePropertyChanged(nameof(InvTotalTotal));
            }
        }

        public ExpenseProjectVM(ProjectViewResModel baseproj, EmployeeModel currentemployee)
        {
            CurrentEmployee = currentemployee;
            DeleteExpenseCommand = new RelayCommand<ExpenseProjectModel>(DeleteExpense);
            AddExpenseCommandCommand = new RelayCommand(AddExpense);

            BaseProject = baseproj;
            LoadExpensesForViewing();
            SumTable();
        }

        public void SumTable()
        {
            List<ExpenseProjectModel> invoiced = Rowdata.Where(x => x.IsInvoiced).ToList();

            //InvHotelTotal = invoiced.Sum(x => x.HotelExp);
            //InvTransportTotal = invoiced.Sum(x => x.TransportExp);
            //InvParkingTotal = invoiced.Sum(x => x.ParkingExp);
            //InvMealsTotal = invoiced.Sum(x => x.MealsExp);
            //InvMiscTotal = invoiced.Sum(x => x.MiscExp);
            //InvMileageTotal = invoiced.Sum(x => x.MileageExp);
            //InvMileageCostTotal = invoiced.Sum(x => x.MileageTotalCostExp);
            InvTotalTotal = invoiced.Sum(x => x.TotalCostExp);


            //HotelTotal = Rowdata.Sum(x => x.HotelExp);
            //TransportTotal = Rowdata.Sum(x => x.TransportExp);
            //ParkingTotal = Rowdata.Sum(x => x.ParkingExp);
            //MealsTotal = Rowdata.Sum(x => x.MealsExp);
            //MiscTotal = Rowdata.Sum(x => x.MiscExp);
            //MileageTotal = Rowdata.Sum(x => x.MileageExp);
            //MileageCostTotal = Rowdata.Sum(x => x.MileageTotalCostExp);
            TotalTotal = Rowdata.Sum(x => x.TotalCostExp);

            //PenHotelTotal = HotelTotal - InvHotelTotal;
            //PenTransportTotal = TransportTotal - InvTransportTotal;
            //PenParkingTotal = ParkingTotal - InvParkingTotal;
            //PenMealsTotal = MealsTotal - InvMealsTotal;
            //PenMiscTotal = MiscTotal - InvMiscTotal;
            //PenMileageTotal = MileageTotal - InvMileageTotal;
            //PenMileageCostTotal = MileageCostTotal - InvMileageCostTotal;
            PenTotalTotal = TotalTotal - InvTotalTotal;
        }

        /// <summary>
        /// Load DB
        /// </summary>
        public void LoadExpensesForViewing()
        {
            List<ExpenseReportDbModel> expenses = SQLAccess.LoadExpensesByProjectId(BaseProject.Id);
            List<ExpenseProjectModel> exlist = new List<ExpenseProjectModel>();

            foreach (ExpenseReportDbModel item in expenses)
            {
                ExpenseProjectModel erm = new ExpenseProjectModel(item);

                exlist.Add(erm);
            }

            List<ExpenseProjectModel> trmadjusted = exlist?.OrderByDescending(x => x.DateExp).ToList();

            Rowdata = new ObservableCollection<ExpenseProjectModel>(trmadjusted);
            SumTable();
        }

        private void DeleteExpense(ExpenseProjectModel epm)
        {
            LeftViewToShow = new YesNoView();
            YesNoVM aysvm = new YesNoVM(epm, this);
            LeftViewToShow.DataContext = aysvm;
            LeftDrawerOpen = true;
        }

        private void AddExpense()
        {
            LeftViewToShow = new AddExpenseView();
            AddExpenseVM addsubvm = new AddExpenseVM(BaseProject, this, CurrentEmployee);
            LeftViewToShow.DataContext = addsubvm;
            LeftDrawerOpen = true;
        }
    }
}
