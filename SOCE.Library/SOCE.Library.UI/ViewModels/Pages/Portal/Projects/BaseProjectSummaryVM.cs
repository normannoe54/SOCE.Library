using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SOCE.Library.Db;
using System.Collections.ObjectModel;
using SOCE.Library.UI.Views;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Diagnostics;
using System.Linq;

namespace SOCE.Library.UI.ViewModels
{
    public class BaseProjectSummaryVM : BaseVM
    {
        private ViewEnum _selectedViewEnum;
        public ViewEnum SelectedViewEnum
        {
            get { return _selectedViewEnum; }
            set
            {
                _selectedViewEnum = value;

                switch (_selectedViewEnum)
                {
                    case ViewEnum.ProjectSummary:
                        ProjectSummaryVM Psummaryvm = new ProjectSummaryVM(BaseProject, CurrentEmployee);
                        SelectedVM = Psummaryvm;
                        break;
                    case ViewEnum.Timeline:
                        HoursVM Hoursvm = new HoursVM(BaseProject, CurrentEmployee);
                        SelectedVM = Hoursvm;
                        break;
                    case ViewEnum.AddService:
                        SelectedVM = new AddServiceVM(BaseProject, CurrentEmployee);
                        break;
                    case ViewEnum.Expenses:
                        SelectedVM = new ExpenseProjectVM(BaseProject, CurrentEmployee);
                        break;
                    case ViewEnum.Invoicing:
                        ProjectSummaryVM Psummaryvm2 = new ProjectSummaryVM(BaseProject, CurrentEmployee);
                        SelectedVM = Psummaryvm2;
                        SelectedVM = new InvoicingSummaryVM(BaseProject);
                        break;
                    default:
                        break;
                }
                RaisePropertyChanged(nameof(SelectedViewEnum));
            }
        }

        private BaseVM _selectedVM = new BaseVM();
        public BaseVM SelectedVM
        {
            get { return _selectedVM; }
            set
            {
                _selectedVM = value;
                RaisePropertyChanged(nameof(SelectedVM));
            }
        }

        //private ProjectSummaryVM _psummaryvm;
        //public ProjectSummaryVM Psummaryvm
        //{
        //    get { return _psummaryvm; }
        //    set
        //    {
        //        _psummaryvm = value;
        //        RaisePropertyChanged(nameof(Psummaryvm));
        //    }
        //}

        private HoursVM _hoursvm;
        public HoursVM Hoursvm
        {
            get { return _hoursvm; }
            set
            {
                _hoursvm = value;
                RaisePropertyChanged(nameof(Hoursvm));
            }
        }

        public ICommand CloseCommand { get; set; }
        public EmployeeModel CurrentEmployee { get; set; }
        public ProjectViewResModel BaseProject { get; set; }

        public InvoicingVM invm { get; set; }
        public BaseProjectSummaryVM(EmployeeModel employee, ProjectViewResModel baseproject, ViewEnum viewselection, InvoicingVM invoicing = null)
        {
            ButtonInAction = true;
            this.CloseCommand = new RelayCommand(this.CloseWindow);
            CurrentEmployee = employee;
            BaseProject = baseproject;
            if (invoicing != null)
            {
                invm = invoicing;
            }
            //Psummaryvm = new ProjectSummaryVM(baseproject, employee);
            SelectedViewEnum = viewselection;
        }

        private void CloseWindow()
        {
            if (!ButtonInAction)
            {
                return;
            }
            ButtonInAction = false;

            if (invm != null)
            {
                invm.LoadProjects();
            }
            DialogHost.Close("RootDialog");
        }
    }
}
