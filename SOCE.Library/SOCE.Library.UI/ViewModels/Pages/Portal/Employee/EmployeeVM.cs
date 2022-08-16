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
        public ICommand GoToAddEmployee { get; set; }

        private ObservableCollection<EmployeeModel> _employees;
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
            this.GoToAddEmployee = new RelayCommand<object>(this.ExecuteRunDialog);


            LoadEmployees();
        }

        private async void ExecuteRunDialog(object o)
        {
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            var view = new AddEmployeeView();

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);

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

            foreach (EmployeeDbModel emdb in dbemployees)
            {
                members.Add(new EmployeeModel(emdb));
            }

            Employees = members;
        }
    }
}
