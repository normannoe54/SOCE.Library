using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using SOCE.Library.Db;
using SOCE.Library.UI.Views;

namespace SOCE.Library.UI.ViewModels
{
    public class EmployeeVM : BaseVM
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

                if (_currentEmployee.Status == AuthEnum.Admin)
                {
                    CanAddEmployee = true;
                }
            }
        }

        public ICommand GoToAddEmployee { get; set; }

        public ICommand DeleteEmployee { get; set; }
        public ICommand GoToEmployeeInfoCommand { get; set; }

        private bool _canAddEmployee = false;
        public bool CanAddEmployee
        {
            get
            {
                return _canAddEmployee;
            }
            set
            {
                _canAddEmployee = value;
                RaisePropertyChanged(nameof(CanAddEmployee));
            }
        }

        private bool _showRates = false;
        public bool ShowRates
        {
            get
            {
                return _showRates;
            }
            set
            {
                _showRates = value;

                foreach (EmployeeModel em in Employees)
                {
                    em.RateVisible = _showRates;
                }

                RaisePropertyChanged(nameof(ShowRates));
            }
        }

        private ObservableCollection<EmployeeModel> _employees = new ObservableCollection<EmployeeModel>();
        public ObservableCollection<EmployeeModel> Employees
        {
            get { return _employees; }
            set
            {
                _employees = value;
                TextEmployees = _employees.Count + " Total";
                RaisePropertyChanged(nameof(Employees));
            }
        }

        private string _textEmployees;
        public string TextEmployees
        {
            get { return _textEmployees; }
            set
            {
                _textEmployees = value;
                RaisePropertyChanged(nameof(TextEmployees));
            }
        }

        private bool _rateTitleVisible;
        public bool RateTitleVisible
        {
            get { return _rateTitleVisible; }
            set
            {
                _rateTitleVisible = value;
                RaisePropertyChanged(nameof(RateTitleVisible));
            }
        }

        public EmployeeVM(EmployeeModel loggedinEmployee)
        {
            CurrentEmployee = loggedinEmployee;

            RateTitleVisible = loggedinEmployee.Status == AuthEnum.Standard ? false : true;

            this.GoToAddEmployee = new RelayCommand<object>(this.ExecuteRunAddDialog);
            this.DeleteEmployee = new RelayCommand<object>(this.ExecuteRunDeleteDialog);
            this.GoToEmployeeInfoCommand = new RelayCommand<object>(GoToEmployeeSummary);
            LoadEmployees();
        }

        public async void GoToEmployeeSummary(object o)
        {

            EmployeeModel em = (EmployeeModel)o;
            em.CollectTimesheetSubmission();
            em.IsEditable = true;
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            var view = new EmployeeInfoView();
            var vm = new EmployeeInfoVM(em);
            view.DataContext = vm;
            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog");

            em.EditFieldState = true;
            em.IsEditable = false;
        }

        private async void ExecuteRunAddDialog(object o)
        {
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            var view = new AddEmployeeView();

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog");

            AddEmployeeVM vm = view.DataContext as AddEmployeeVM;
            bool resultvm = vm.result;

            if (resultvm)
            {
                LoadEmployees();
            }
        }

        private async void ExecuteRunDeleteDialog(object o)
        {
            EmployeeModel em = o as EmployeeModel;
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            YesNoView view = new YesNoView();
            YesNoVM aysvm = new YesNoVM(em);

            view.DataContext = aysvm;

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog");

            aysvm = view.DataContext as YesNoVM;

            if (aysvm.Result)
            {
                SQLAccess.ArchiveEmployee(em.Id);
                LoadEmployees();
            }
        }

        private void LoadEmployees()
        {
            Employees.Clear();
            List<EmployeeDbModel> dbemployees = SQLAccess.LoadEmployees();

            foreach (EmployeeDbModel emdb in dbemployees)
            {
                EmployeeModel em = new EmployeeModel(emdb);

                //be able to see your own stuff
                em.SetEmployeeModelfromUser(CurrentEmployee);

                //members.Add(em);
                Employees.Add(em);
            }

            var index = Employees.ToList().FindIndex(x => x.Id == CurrentEmployee.Id);

            if (index != -1)
            {
                var item = Employees[index];
                Employees[index] = Employees[0];
                Employees[0] = item;
            }
        }

    }
}
