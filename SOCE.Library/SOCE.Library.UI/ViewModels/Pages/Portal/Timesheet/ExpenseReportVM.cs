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

namespace SOCE.Library.UI.ViewModels
{
    public class ExpenseReportVM : BaseVM
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

        //public List<RegisteredTimesheetDataModel> TimesheetData;
        public ICommand AddRowCommand { get; set; }
        public ICommand SaveExpenseReport { get; set; }
        public ICommand RemoveRowCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        public List<TimesheetRowModel> CopiedTimesheetData { get; set; } = new List<TimesheetRowModel>();

        private ObservableCollection<ExpenseReportModel> _rowdata = new ObservableCollection<ExpenseReportModel>();
        public ObservableCollection<ExpenseReportModel> Rowdata
        {
            get { return _rowdata; }
            set
            {
                _rowdata = value;
                RaisePropertyChanged(nameof(Rowdata));
            }
        }

        private bool _isSubEditable = true;
        public bool IsSubEditable
        {
            get { return _isSubEditable; }
            set
            {
                _isSubEditable = value;
                RaisePropertyChanged(nameof(IsSubEditable));
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

        //private double _mileagerate;
        //public double Mileagerate
        //{
        //    get { return _mileagerate; }
        //    set
        //    {
        //        _mileagerate = value;
        //        RaisePropertyChanged(nameof(Mileagerate));
        //    }
        //}

        private DateTime _payperiodbeginning;
        public DateTime Payperiodbeginning
        {
            get { return _payperiodbeginning; }
            set
            {
                _payperiodbeginning = value;
                RaisePropertyChanged(nameof(Payperiodbeginning));
            }
        }

        private DateTime _payperiodend;
        public DateTime Payperiodend
        {
            get { return _payperiodend; }
            set
            {
                _payperiodend = value;
                RaisePropertyChanged(nameof(Payperiodend));
            }
        }

        private ExpenseReportModel _selectedRow;
        public ExpenseReportModel SelectedRow
        {
            get { return _selectedRow; }
            set
            {
                _selectedRow = value;
                RaisePropertyChanged(nameof(SelectedRow));
            }
        }

        private string _message = "";
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                RaisePropertyChanged(nameof(Message));
            }
        }

        private bool _messageVisible = false;
        public bool MessageVisible
        {
            get { return _messageVisible; }
            set
            {
                _messageVisible = value;
                RaisePropertyChanged(nameof(MessageVisible));
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

        private ObservableCollection<ProjectLowResModel> _projectList;
        public ObservableCollection<ProjectLowResModel> ProjectList
        {
            get { return _projectList; }
            set
            {
                _projectList = value;
                RaisePropertyChanged(nameof(ProjectList));
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


        public List<ExpenseReportModel> CopiedData { get; set; } = new List<ExpenseReportModel>();

        public ExpenseReportVM(EmployeeModel loggedinEmployee, DateTime startdate, DateTime enddate, bool isactive, List<ProjectLowResModel> projects)
        {
            //CanIEdit = true;
            this.AddRowCommand = new RelayCommand(AddRowToCollection);
            this.RemoveRowCommand = new RelayCommand<ExpenseReportModel>(RemoveRow);
            this.SaveExpenseReport = new RelayCommand(SaveCommand);
            this.CancelCommand = new RelayCommand(Cancel);
            CurrentEmployee = loggedinEmployee;
            Payperiodend = enddate;
            Payperiodbeginning = startdate;
            IsSubEditable = isactive;
            ProjectList = new ObservableCollection<ProjectLowResModel>(projects);
            LoadExpenses();
        }

        public ExpenseReportVM(EmployeeModel loggedinEmployee, DateTime startdate, DateTime enddate)
        {
            //CanIEdit = true;
            this.AddRowCommand = new RelayCommand(AddRowToCollection);
            this.RemoveRowCommand = new RelayCommand<ExpenseReportModel>(RemoveRow);
            this.SaveExpenseReport = new RelayCommand(SaveCommand);
            this.CancelCommand = new RelayCommand(Cancel);
            CurrentEmployee = loggedinEmployee;
            Payperiodend = enddate;
            Payperiodbeginning = startdate;
            LoadExpensesForViewing();
        }

        private void AddRowToCollection()
        {
            Rowdata.Add(new ExpenseReportModel(ProjectList.ToList(), Payperiodbeginning, 0.67, this));
        }

        private void RemoveRow(ExpenseReportModel trm)
        {
            Rowdata.Remove(trm);
            SumTable();
        }

        public void SumTable()
        {
            TotalTotal = Rowdata.Sum(x => x.TotalCostExp);
        }

        /// <summary>
        /// Load DB
        /// </summary>
        private void LoadExpensesForViewing()
        {
            List<ExpenseReportDbModel> expenses = SQLAccess.LoadExpenses(Payperiodbeginning, Payperiodend, CurrentEmployee.Id);

            //get rate
            if (expenses.Count > 0)
            {
                ExpenseReportDbModel first = expenses.FirstOrDefault();
                //Mileagerate = first.MileageRate;
            }
            else
            {
                //Mileagerate = 0.67;
            }

            List<ExpenseReportModel> exlist = new List<ExpenseReportModel>();


            foreach (ExpenseReportDbModel item in expenses)
            {
                ExpenseReportModel erm = new ExpenseReportModel(item, this);
                //ProjectLowResModel pmnew = null;

                ProjectDbModel dbmod = SQLAccess.LoadProjectsById(item.ProjectId);
                ProjectLowResModel pm = new ProjectLowResModel(dbmod);

                erm.allowprojectchanges = false;
                erm.Project = pm;
                erm.allowprojectchanges = true;

                erm.AlertStatus = TimesheetRowAlertStatus.Active;

                exlist.Add(erm);
            }


            List<ExpenseReportModel> trmadjusted = exlist?.OrderByDescending(x => x.Project.ProjectNumber).ToList();
            List<ExpenseReportModel> rowlistcopied = new List<ExpenseReportModel>();

            foreach (ExpenseReportModel trm in trmadjusted)
            {
                rowlistcopied.Add((ExpenseReportModel)trm.Clone());
            }

            Rowdata = new ObservableCollection<ExpenseReportModel>(exlist);
            CopiedData = new List<ExpenseReportModel>(rowlistcopied);
            SumTable();
        }


        /// <summary>
        /// Load DB
        /// </summary>
        private void LoadExpenses()
        {
            List<ExpenseReportDbModel> expenses = SQLAccess.LoadExpenses(Payperiodbeginning, Payperiodend, CurrentEmployee.Id);

            //get rate
            if (expenses.Count > 0)
            {
                ExpenseReportDbModel first = expenses.FirstOrDefault();
                //Mileagerate = first.MileageRate;
            }
            else
            {
                //Mileagerate = 0.67;
            }

            List<ExpenseReportModel> exlist = new List<ExpenseReportModel>();

            foreach (ExpenseReportDbModel item in expenses)
            {
                ExpenseReportModel erm = new ExpenseReportModel(ProjectList.ToList(), item, this);
                ProjectLowResModel pm = null;
                ProjectLowResModel pmnew = null;

                pmnew = ProjectList.Where(x => x.Id == item.ProjectId).FirstOrDefault();

                if (pmnew == null)
                {
                    ProjectDbModel dbmod = SQLAccess.LoadProjectsById(item.ProjectId);
                    pm = new ProjectLowResModel(dbmod);
                    erm.AlertStatus = TimesheetRowAlertStatus.Inactive;
                    erm.ProjectList.Add(pm);
                    pmnew = pm;
                }
                else
                {
                    erm.AlertStatus = TimesheetRowAlertStatus.Active;
                }


                erm.allowprojectchanges = false;
                erm.Project = pmnew;
                erm.allowprojectchanges = true;

                exlist.Add(erm);
                //}
                //}
            }

            List<ExpenseReportModel> trmadjusted = exlist?.OrderByDescending(x => x.Project.ProjectNumber).ToList();
            List<ExpenseReportModel> rowlistcopied = new List<ExpenseReportModel>();

            foreach (ExpenseReportModel trm in trmadjusted)
            {
                rowlistcopied.Add((ExpenseReportModel)trm.Clone());
            }

            Rowdata = new ObservableCollection<ExpenseReportModel>(exlist);
            CopiedData = new List<ExpenseReportModel>(rowlistcopied);
            SumTable();
        }

        /// <summary>
        /// Save to DB
        /// </summary>
        private void SaveCommand()
        {
            MessageVisible = true;

            try
            {
                //deleting
                foreach (ExpenseReportModel ctrm in CopiedData)
                {
                    if (ctrm.Project != null)
                    {
                        int index = Rowdata.ToList().FindIndex(x => x?.Id == ctrm.Id);

                        if (index == -1)
                        {
                            if (ctrm.Id != -1)
                            {
                                SQLAccess.DeleteExpenseReport(ctrm.Id);
                            }
                        }
                    }
                }

                foreach (ExpenseReportModel ex in Rowdata)
                {
                    if (ex.Project != null)
                    {
                        ExpenseReportDbModel exdb = new ExpenseReportDbModel()
                        {
                            Id = ex.Id,
                            ProjectId = ex.Project.Id,
                            EmployeeId = CurrentEmployee.Id,
                            Description = ex.DescriptionExp,
                            Date = (int)long.Parse(ex.DateExp.ToString("yyyyMMdd")),
                            TypeExpense = (int)ex.TypeExpense,
                            Mileage = ex.MileageExp,
                            TotalCost = ex.TotalCostExp,
                            MileageRate = ex.Rate,
                            Invoiced = 0,
                            IsClientBillable = Convert.ToInt32(ex.IsClientBillable),
                            Reimbursable = Convert.ToInt32(ex.IsReimbursable),
                            IsCustom = 0
                        };

                        int id = SQLAccess.AddExpenseReport(exdb);
                        ex.Id = id;
                    }

                }

                Message = "Report Saved";
                MessageVisible = false;
            }
            catch
            {

                Message = "Something went wrong!";
                MessageVisible = false;
            }

        }

        /// <summary>
        /// Save to DB
        /// </summary>
        private void Cancel()
        {
            DialogHost.Close("RootDialog");
        }
    }
}
