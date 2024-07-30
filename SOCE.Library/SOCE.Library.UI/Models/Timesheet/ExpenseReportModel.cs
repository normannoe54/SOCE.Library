using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using SOCE.Library.Db;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SOCE.Library.UI.ViewModels;
using System.Windows.Input;
using System.Globalization;

namespace SOCE.Library.UI
{
    public class ExpenseReportModel : PropertyChangedBase, ICloneable
    {
        public ICommand ClearSelectedProjectCommand { get; set; }
        public ICommand SelectedItemChangedCommand { get; set; }
        public bool allowprojectchanges = true;

        private ProjectLowResModel _project;
        public ProjectLowResModel Project
        {
            get
            {
                return _project;
            }
            set
            {
                _project = value;

                if (_project != null)
                {
                    IsThisEditable = false;
                    ProjectList = BaseProjectList;
                }
                else
                {
                    IsThisEditable = true;
                }

                RaisePropertyChanged(nameof(Project));
            }
        }

        private bool _isThisEditable = true;
        public bool IsThisEditable
        {
            get { return _isThisEditable; }
            set
            {
                _isThisEditable = value;

                RaisePropertyChanged(nameof(IsThisEditable));
            }
        }

        private int _id = -1;
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                RaisePropertyChanged(nameof(Id));
            }
        }

        //private double _total;
        //public double Total
        //{
        //    get
        //    {
        //        return _total;
        //    }
        //    set
        //    {
        //        _total = value;
        //        RaisePropertyChanged(nameof(Total));
        //    }
        //}

        private bool _comboOpen = false;
        public bool ComboOpen
        {
            get { return _comboOpen; }
            set
            {
                _comboOpen = value;
                RaisePropertyChanged(nameof(ComboOpen));
            }
        }

        private bool _projectSelected = false;
        public bool ProjectSelected
        {
            get
            {
                return _projectSelected;
            }
            set
            {
                _projectSelected = value;
                RaisePropertyChanged(nameof(ProjectSelected));
            }
        }

        private TimesheetRowAlertStatus _alertStatus;
        public TimesheetRowAlertStatus AlertStatus
        {
            get
            {
                return _alertStatus;
            }
            set
            {
                _alertStatus = value;
                RaisePropertyChanged(nameof(AlertStatus));
            }
        }

        private ExpenseEnum _typeExpense;
        public ExpenseEnum TypeExpense
        {
            get
            {
                return _typeExpense;
            }
            set
            {
                _typeExpense = value;

                MileageVis = _typeExpense == ExpenseEnum.Mileage;

                if (MileageVis && onstartup)
                {
                    TotalCostExp = 0;
                    MileageExp = 0;
                }

                RaisePropertyChanged(nameof(TypeExpense));
            }
        }

        private DateTime _dateExp;
        public DateTime DateExp
        {
            get
            {
                return _dateExp;
            }
            set
            {
                _dateExp = value;
                RaisePropertyChanged(nameof(DateExp));
            }
        }

        private string _descriptionExp;
        public string DescriptionExp
        {
            get
            {
                return _descriptionExp;
            }
            set
            {
                _descriptionExp = value;
                RaisePropertyChanged(nameof(DescriptionExp));
            }
        }

        private double _rate;
        public double Rate
        {
            get
            {
                return _rate;
            }
            set
            {
                _rate = value;
                RaisePropertyChanged(nameof(Rate));
            }
        }

        private double _mileageExp;
        public double MileageExp
        {
            get
            {
                return _mileageExp;
            }
            set
            {
                _mileageExp = value;

                if (onstartup)
                {
                    TotalCostExp = Rate * _mileageExp;
                }

                RaisePropertyChanged(nameof(MileageExp));
            }
        }

        private double _totalCostExp;
        public double TotalCostExp
        {
            get
            {
                return _totalCostExp;
            }
            set
            {
                _totalCostExp = value;

                if (onstartup)
                {
                    viewmodel.SumTable();
                }

                RaisePropertyChanged(nameof(TotalCostExp));
            }
        }

        private bool _mileageVis;
        public bool MileageVis
        {
            get
            {
                return _mileageVis;
            }
            set
            {
                _mileageVis = value;
                RaisePropertyChanged(nameof(MileageVis));
            }
        }

        private bool _isClientBillable;
        public bool IsClientBillable
        {
            get
            {
                return _isClientBillable;
            }
            set
            {
                _isClientBillable = value;
                RaisePropertyChanged(nameof(IsClientBillable));
            }
        }

        private bool _isReimbursable;
        public bool IsReimbursable
        {
            get
            {
                return _isReimbursable;
            }
            set
            {
                _isReimbursable = value;
                RaisePropertyChanged(nameof(IsReimbursable));
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

        private bool onstartup = false;

        private ExpenseReportVM viewmodel;
        public ObservableCollection<ProjectLowResModel> BaseProjectList { get; set; } = new ObservableCollection<ProjectLowResModel>();

        public ExpenseReportModel()
        {
            Constructor();
        }

        public ExpenseReportModel(List<ProjectLowResModel> projs, ExpenseReportDbModel dbmodel, ExpenseReportVM vm)
        {   
            Constructor();
            viewmodel = vm;
            Id = dbmodel.Id;
            BaseProjectList = new ObservableCollection<ProjectLowResModel>(projs);
            ProjectList = BaseProjectList;
            TypeExpense = (ExpenseEnum)dbmodel.TypeExpense;
            DateExp = DateTime.ParseExact(dbmodel.Date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            DescriptionExp = dbmodel.Description;
            MileageExp = dbmodel.Mileage;
            TotalCostExp = dbmodel.TotalCost;
            Rate = dbmodel.MileageRate;
            IsClientBillable = Convert.ToBoolean(dbmodel.IsClientBillable);
            IsReimbursable = Convert.ToBoolean(dbmodel.Reimbursable);
            onstartup = true;
        }

        public ExpenseReportModel(ExpenseReportDbModel dbmodel, ExpenseReportVM vm)
        {
            Constructor();
            viewmodel = vm;
            Id = dbmodel.Id;
            TypeExpense = (ExpenseEnum)dbmodel.TypeExpense;
            DateExp = DateTime.ParseExact(dbmodel.Date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            DescriptionExp = dbmodel.Description;
            MileageExp = dbmodel.Mileage;
            TotalCostExp = dbmodel.TotalCost;
            Rate = dbmodel.MileageRate;
            IsClientBillable = Convert.ToBoolean(dbmodel.IsClientBillable);
            IsReimbursable = Convert.ToBoolean(dbmodel.Reimbursable);
            onstartup = true;
        }

        public ExpenseReportModel(List<ProjectLowResModel> projs, DateTime date, double rateinv, ExpenseReportVM vm)
        {
            Constructor();
            viewmodel = vm;
            BaseProjectList = new ObservableCollection<ProjectLowResModel>(projs);
            ProjectList = BaseProjectList;
            DateExp = date;
            Rate = rateinv;
            onstartup = true;
        }

        public void CollectSubProjects()
        {
            if (Project == null)
            {
                return;
            }

            int id = Project.Id;

            //1 = active subprojects - doesnt work cuz of saved or submitted previous phases
            List<SubProjectDbModel> subdbprojects = SQLAccess.LoadSubProjectsByProject(id);

            List<SubProjectLowResModel> submodels = TimesheetRowModel.FormatSubProjects(subdbprojects, Project.IsActive);
        }

        public void Constructor()
        {
            this.SelectedItemChangedCommand = new RelayCommand<string>(this.SelectionCombo);
            this.ClearSelectedProjectCommand = new RelayCommand(this.ClearSelected);
        }

        public void ClearSelected()
        {
            Project = null;
        }

        private void SelectionCombo(string project)
        {
            if (Project == null)
            {
                if (!String.IsNullOrEmpty(project))
                {
                    ComboOpen = true;

                    ProjectList = new ObservableCollection<ProjectLowResModel>(BaseProjectList.Where(x => (x.ProjectName.ToUpper() + x.ProjectNumber.ToString()).Contains(project.ToUpper())));
                }
                else
                {
                    ProjectList = BaseProjectList;
                }
            }
        }
        
        private void UpdateStatus()
        {
            bool status = _project.IsActive;

            AlertStatus = status ? TimesheetRowAlertStatus.Active : TimesheetRowAlertStatus.Inactive;

        }

        public object Clone()
        {
            ObservableCollection<SubProjectLowResModel> spms = new ObservableCollection<SubProjectLowResModel>();


            ExpenseReportModel trm = new ExpenseReportModel()
            {
                Project = (ProjectLowResModel)this.Project?.Clone(),
                Id = this.Id,
            };

            return trm;
        }
    }
}
