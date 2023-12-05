using MaterialDesignThemes.Wpf;
using Microsoft.WindowsAPICodePack.Dialogs;
using SOCE.Library.Db;
using SOCE.Library.UI.ViewModels;
using SOCE.Library.UI.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SOCE.Library.UI
{
    public class ProjectListModel : PropertyChangedBase
    {
        private int _projectIdid { get; set; }
        public int ProjectId
        {
            get
            {
                return _projectIdid;
            }
            set
            {
                _projectIdid = value;
                RaisePropertyChanged(nameof(ProjectId));
            }
        }

        private EmployeeLowResModel _projectManager;
        public EmployeeLowResModel ProjectManager
        {
            get { return _projectManager; }
            set
            {
                _projectManager = value;
                RaisePropertyChanged(nameof(ProjectManager));
            }
        }

        public bool isfirsttimesetting = true;
        public SchedulingEnum SchedulingValueInit { get; set; }
        private SchedulingEnum _schedulingValue { get; set; }
        public SchedulingEnum SchedulingValue
        {
            get
            {
                return _schedulingValue;
            }
            set
            {
                _schedulingValue = value;

                if (isfirsttimesetting)
                {
                    SchedulingValueInit = _schedulingValue;
                    isfirsttimesetting = false;
                }
                RaisePropertyChanged(nameof(SchedulingValue));

            }
        }

        private double _percentComplete { get; set; }
        public double PercentComplete
        {
            get
            {
                return _percentComplete;
            }
            set
            {
                _percentComplete = value;
                RaisePropertyChanged(nameof(PercentComplete));
            }
        }

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

        private int _clientNumber { get; set; }
        public int ClientNumber
        {
            get
            {
                return _clientNumber;
            }
            set
            {
                _clientNumber = value;
                RaisePropertyChanged(nameof(ClientNumber));
            }
        }

        private DateTime? _dueDate;
        public DateTime? DueDate
        {
            get { return _dueDate; }
            set
            {
                _dueDate = value;
                RaisePropertyChanged(nameof(DueDate));
            }
        }

        private string _remarks { get; set; }
        public string Remarks
        {
            get
            {
                return _remarks;
            }
            set
            {
                _remarks = value;
                RaisePropertyChanged(nameof(Remarks));
            }
        }

        private int _projectNumber { get; set; }
        public int ProjectNumber
        {
            get
            {
                return _projectNumber;
            }
            set
            {
                _projectNumber = value;
                RaisePropertyChanged(nameof(ProjectNumber));
            }
        }

        private Brush _colorForListButton = Brushes.Orange;
        public Brush ColorForListButton
        {
            get { return _colorForListButton; }
            set
            {
                _colorForListButton = value;
                RaisePropertyChanged(nameof(ColorForListButton));
            }
        }

        private bool _editList = true;
        public bool EditList
        {
            get { return _editList; }
            set
            {
                _editList = value;

                RaisePropertyChanged(nameof(EditList));
            }
        }

        private string _tooltipforList = "Locked";
        public string TooltipforList
        {
            get { return _tooltipforList; }
            set
            {
                _tooltipforList = value;
                RaisePropertyChanged(nameof(TooltipforList));
            }
        }

        private PackIconKind _iconForListButton = PackIconKind.Lock;
        public PackIconKind IconForListButton
        {
            get
            {
                return _iconForListButton;
            }
            set
            {
                _iconForListButton = value;
                RaisePropertyChanged(nameof(IconForListButton));
            }
        }

        private bool _isOnHold = false;
        public bool IsOnHold
        {
            get { return _isOnHold; }
            set
            {
                _isOnHold = value;
                RaisePropertyChanged(nameof(IsOnHold));
            }
        }

        public ICommand ButtonPress { get; set; }

        public ProjectListModel(ProjectDbModel pm)
        {
            this.ButtonPress = new RelayCommand(this.RunButtonPress);
            ProjectId = pm.Id;
            ProjectName = pm.ProjectName;
            ProjectNumber = pm.ProjectNumber;
            PercentComplete = pm.PercentComplete;
            Remarks = pm.Remarks;

            if (pm?.DueDate != null && pm?.DueDate != 0)
            {
                DueDate = DateTime.ParseExact(pm.DueDate.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            }

            Remarks = pm.Remarks;
        }



        private async void RunButtonPress()
        {
            bool makechange = true;
            if (EditList)
            {
                ColorForListButton = Brushes.Green;
                IconForListButton = PackIconKind.ContentSave;
                TooltipforList = "Save Changes";
            }
            else
            {
                //load subprojects
                List<SubProjectLowResModel> SubProjects = new List<SubProjectLowResModel>();
                List<SubProjectDbModel> subdbmodels = SQLAccess.LoadSubProjectsByProject(ProjectId);

                foreach (SubProjectDbModel spdm in subdbmodels)
                {
                    SubProjectLowResModel spm = new SubProjectLowResModel(spdm);
                    SubProjects.Add(spm);
                }

                if (SchedulingValue == SchedulingEnum.H)
                {
                    IsOnHold = true;
                    foreach (SubProjectLowResModel sub in SubProjects)
                    {
                        sub.IsScheduleActive = false;
                        SQLAccess.UpdateScheduleActive(sub.Id, sub.IsScheduleActive ?  1 : 0);

                    }

                }
                else if (SchedulingValue == SchedulingEnum.CA)
                {
                    SubProjectLowResModel subcontainingCA = SubProjects.Where(x => x.PointNumber == "CA").FirstOrDefault();

                    bool containsCA = SubProjects.Where(x => x.PointNumber == "CA").Count() > 0;

                    if (containsCA)
                    {
                        foreach (SubProjectLowResModel sub in SubProjects)
                        {
                            sub.IsScheduleActive = sub.PointNumber == "CA" ? true : false;
                            SQLAccess.UpdateScheduleActive(sub.Id, sub.IsScheduleActive ? 1 : 0);
                            if (sub.PointNumber == "CA")
                            {
                                sub.IsActive = true;
                                SQLAccess.UpdateActive(sub.Id, sub.IsActive ? 1 : 0);
                            }
                        }
                    }
                    else
                    {
                        YesNoView view = new YesNoView();
                        YesNoVM aysvm = new YesNoVM();

                        aysvm.Message = $"CA Phase does not exist";
                        aysvm.SubMessage = $"Would you like to add one?";
                        view.DataContext = aysvm;

                        //show the dialog
                        var Result = await DialogHost.Show(view, "RootDialog");

                        YesNoVM vm = view.DataContext as YesNoVM;
                        bool resultvm = vm.Result;
                        makechange = resultvm;
                        if (resultvm)
                        {
                            IsOnHold = false;
                            double numorder = SubProjects.Select(x => x.NumberOrder).Max();
                            SubProjectDbModel subproject = new SubProjectDbModel
                            {
                                ProjectId = ProjectId,
                                PointNumber = "CA",
                                Description = "Construction Administration",
                                IsActive = 1,
                                IsInvoiced = 0,
                                PercentComplete = 0,
                                PercentBudget = 0,
                                IsScheduleActive = 1,
                                NumberOrder = Convert.ToInt32(numorder) + 1,
                                Fee = 0
                            };
                            SQLAccess.AddSubProject(subproject);

                            foreach (SubProjectLowResModel sub in SubProjects)
                            {
                                sub.IsScheduleActive = false;
                                SQLAccess.UpdateScheduleActive(sub.Id, sub.IsScheduleActive ? 1 : 0);
                            }
                        } //show ui and ask user for stuff
                    }
                }
                else if (SchedulingValue == SchedulingEnum.D)
                {
                    if (SchedulingValueInit != SchedulingValue)
                    {
                        double count = SubProjects.Where(x => x.PointNumber != "CA").Count();

                        if (count > 1)
                        {
                            SelectionBoxView view = new SelectionBoxView();
                            SelectionBoxVM aysvm = new SelectionBoxVM();

                            aysvm.Message = $"More than 1 possible active phase";
                            aysvm.SubMessage = $"Select the active phase";
                            aysvm.SubProjects = new ObservableCollection<SubProjectLowResModel>(SubProjects.Where(x => x.PointNumber != "CA").ToList());
                            view.DataContext = aysvm;

                            //show the dialog
                            var Result = await DialogHost.Show(view, "RootDialog");

                            SelectionBoxVM vm = view.DataContext as SelectionBoxVM;
                            bool resultvm = vm.Result;
                            makechange = resultvm;
                            if (resultvm && vm.SelectedSubproject != null)
                            {
                                IsOnHold = false;
                                foreach (SubProjectLowResModel sub in SubProjects)
                                {
                                    if (sub.Id != vm.SelectedSubproject.Id)
                                    {
                                        sub.IsScheduleActive = false;
                                        SQLAccess.UpdateScheduleActive(sub.Id, sub.IsScheduleActive ? 1 : 0);
                                    }
                                    else
                                    {
                                        vm.SelectedSubproject.IsScheduleActive = true;
                                        vm.SelectedSubproject.IsActive = true;
                                        SQLAccess.UpdateScheduleActive(vm.SelectedSubproject.Id, vm.SelectedSubproject.IsScheduleActive ? 1 : 0);
                                        SQLAccess.UpdateActive(vm.SelectedSubproject.Id, vm.SelectedSubproject.IsActive ? 1 : 0);
                                    }

                                }

                                
                            } //show ui and ask user for stuff
                              //ask which phase is active
                        }
                        else if (count == 1)
                        {
                            makechange = true;
                            IsOnHold = false;
                            foreach (SubProjectLowResModel sub in SubProjects)
                            {
                                if (sub.PointNumber != "CA")
                                {
                                    sub.IsScheduleActive = true;
                                    sub.IsActive = true;
                                    SQLAccess.UpdateScheduleActive(sub.Id, sub.IsScheduleActive ? 1 : 0);
                                    SQLAccess.UpdateActive(sub.Id, sub.IsActive ? 1 : 0);
                                }
                                else
                                {
                                    sub.IsScheduleActive = false;
                                    SQLAccess.UpdateScheduleActive(sub.Id, sub.IsScheduleActive ? 1 : 0);
                                }
                            }
                        }
                    }
                    
                }

                if (makechange)
                {
                    UpdateProjectModel();
                    //UpdateSubProjects();
                    //LoadSubProjects();

                    ColorForListButton = Brushes.Orange;
                    IconForListButton = PackIconKind.Lock;
                    TooltipforList = "Locked";
                }
            }

            if (makechange)
            {
                EditList = !EditList;
            }

        }

        private void UpdateProjectModel()
        {
            int? duedatevar = null;

            if (DueDate != null)
            {
                duedatevar = (int)long.Parse(DueDate?.ToString("yyyyMMdd"));
            }
            int pmid = 0;
            if (ProjectManager != null)
            {
                pmid = ProjectManager.Id;
            }

            SQLAccess.UpdateProjectList(ProjectId, pmid, duedatevar, PercentComplete, IsOnHold ? 1:0, Remarks);
        }
    }
}
