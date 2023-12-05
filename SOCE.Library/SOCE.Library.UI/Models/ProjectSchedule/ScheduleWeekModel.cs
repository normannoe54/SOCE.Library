using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using SOCE.Library.Db;

namespace SOCE.Library.UI
{
    public class ScheduleWeekModel : PropertyChangedBase, ICloneable
    {

        public int Id { get; set; }


        private string _projectName { get; set; }
        public string ProjectName
        {
            get
            {
                return _projectName;
            }
            set
            {
                _projectName = value;
                RaisePropertyChanged(nameof(ProjectName));
            }
        }

        private string _phaseName { get; set; }
        public string PhaseName
        {
            get
            {
                return _phaseName;
            }
            set
            {
                _phaseName = value;
                RaisePropertyChanged(nameof(PhaseName));
            }
        }

        private double _budgetedHours = 0;
        public double BudgetedHours
        {
            get { return _budgetedHours; }
            set
            {
                _budgetedHours = value;
                RaisePropertyChanged(nameof(BudgetedHours));
            }
        }

        private double _spentHours = 0;
        public double SpentHours
        {
            get { return _spentHours; }
            set
            {
                _spentHours = value;
                RaisePropertyChanged(nameof(SpentHours));
            }
        }

        private ObservableCollection<EmployeeScheduleModel> _employeeList { get; set; } = new ObservableCollection<EmployeeScheduleModel>();
        public ObservableCollection<EmployeeScheduleModel> EmployeeList
        {
            get
            {
                return _employeeList;
            }
            set
            {
                _employeeList = value;
                RaisePropertyChanged(nameof(EmployeeList));
            }
        }

        private EmployeeScheduleModel _selectedEmployee { get; set; } = new EmployeeScheduleModel();
        public EmployeeScheduleModel SelectedEmployee
        {
            get
            {
                return _selectedEmployee;
            }
            set
            {
                _selectedEmployee = value;

                //load time used
                List<RolePerSubProjectDbModel> roles = SQLAccess.LoadRolesPerSubProject(PhaseId);
                RolePerSubProjectDbModel role = roles.Where(x => x.EmployeeId == _selectedEmployee.Id).FirstOrDefault();
                if (role != null)
                {
                    BudgetedHours = role.BudgetHours;
                }
                List<TimesheetRowDbModel> time = SQLAccess.LoadTimeSheetDataByIds(_selectedEmployee.Id, PhaseId);
                
                if (time != null)
                {
                    SpentHours = time.Sum(x => x.TimeEntry);
                }

                RaisePropertyChanged(nameof(SelectedEmployee));
            }
        }

        public int PhaseId { get; set; }
        public int ManagerId { get; set; }
        public int ProjectNumber { get; set; }

        public DateTime Date { get; set; }


        private double _total { get; set; }
        public double Total
        {
            get
            {
                return _total;
            }
            set
            {
                _total = value;
                RaisePropertyChanged(nameof(Total));
            }
        }

        private ObservableCollection<SDEntryModel> _entries = new ObservableCollection<SDEntryModel>();
        public ObservableCollection<SDEntryModel> Entries
        {
            get
            {
                return _entries;
            }
            set
            {
                _entries = value;
                RaisePropertyChanged(nameof(Entries));
            }
        }

        public object Clone()
        {

            ObservableCollection<SDEntryModel> trs = new ObservableCollection<SDEntryModel>();
            foreach (SDEntryModel tr in Entries)
            {
                trs.Add((SDEntryModel)tr?.Clone());
            }

            ScheduleWeekModel trm = new ScheduleWeekModel()
            {
                Id = this.Id,
                PhaseName = this.PhaseName,
                PhaseId = this.PhaseId,
                ProjectName = this.ProjectName,
                SelectedEmployee = (EmployeeScheduleModel)this.SelectedEmployee?.Clone(),
                Entries = trs
            };

            return trm;
        }
    }
}
