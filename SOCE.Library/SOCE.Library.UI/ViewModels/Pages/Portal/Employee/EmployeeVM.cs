using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

                RaisePropertyChanged(nameof(CurrentEmployee));
            }
        }
        public ICommand GoToAddEmployee { get; set; }

        public ICommand DeleteEmployee { get; set; }

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

        private ObservableCollection<EmployeeModel> _employees = new ObservableCollection<EmployeeModel>();
        public ObservableCollection<EmployeeModel> Employees
        {
            get { return _employees; }
            set
            {
                _employees = value;
                TextEmployees = Employees.Count + " Total";
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

        

        public EmployeeVM()
        {
            this.GoToAddEmployee = new RelayCommand<object>(this.ExecuteRunAddDialog);
            this.DeleteEmployee = new RelayCommand<object>(this.ExecuteRunDeleteDialog);
            LoadEmployees();
        }

        private async void ExecuteRunAddDialog(object o)
        {
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            var view = new AddEmployeeView();

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);

        }

        private async void ExecuteRunDeleteDialog(object o)
        {
            EmployeeModel em = o as EmployeeModel;
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            AreYouSureView view = new AreYouSureView();
            AreYouSureVM aysvm = new AreYouSureVM(em);

            view.DataContext = aysvm;

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);

            aysvm = view.DataContext as AreYouSureVM;

            if (aysvm.Result)
            {
                SQLAccess.DeleteEmployee(em.Id);
            }
        }

        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            //load list here
            LoadEmployees();
        }

        private void LoadEmployees()
        {

            List<EmployeeDbModel> dbemployees = SQLAccess.LoadEmployees();

            ObservableCollection<EmployeeModel> members = new ObservableCollection<EmployeeModel>();

            CurrentEmployee = new EmployeeModel(dbemployees[2]);

            foreach (EmployeeDbModel emdb in dbemployees)
            {
                EmployeeModel em = new EmployeeModel(emdb);

                //be able to see your own stuff
                em.SetEmployeeModelfromUser(CurrentEmployee);

                members.Add(em);
            }

            Employees = members;
        }

    }
}
