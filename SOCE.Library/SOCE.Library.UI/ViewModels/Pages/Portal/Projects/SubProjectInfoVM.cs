﻿using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SOCE.Library.Db;
using System.Collections.ObjectModel;
using System.Linq;

namespace SOCE.Library.UI.ViewModels
{
    public class SubProjectInfoVM : BaseVM
    {
        private SubProjectSummaryModel _subProject = new SubProjectSummaryModel();
        public SubProjectSummaryModel SubProject
        {
            get { return _subProject; }
            set
            {
                _subProject = value;
                RaisePropertyChanged(nameof(SubProject));
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                RaisePropertyChanged("Description");
            }
        }

        private string _expandedDescription;
        public string ExpandedDescription
        {
            get { return _expandedDescription; }
            set
            {
                _expandedDescription = value;
                RaisePropertyChanged("ExpandedDescription");
            }
        }

        private double _additionalServicesFee;
        public double AdditionalServicesFee
        {
            get { return _additionalServicesFee; }
            set
            {
                _additionalServicesFee = value;
                RaisePropertyChanged("AdditionalServicesFee");
            }
        }

        private double _adServiceNumber = 0.1;
        public double AdServiceNumber
        {
            get { return _adServiceNumber; }
            set
            {

                _adServiceNumber = value;
                RaisePropertyChanged("AdServiceNumber");

            }
        }

        public ICommand SaveSubProjectCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        private ProjectSummaryVM baseViewModel;

        public SubProjectInfoVM(SubProjectSummaryModel sub, ProjectSummaryVM proj)
        {
            ButtonInAction = true;
            this.SaveSubProjectCommand = new RelayCommand(this.SaveSubProject);
            this.CloseCommand = new RelayCommand(this.CloseWindow);

            baseViewModel = proj;
            SubProject = sub;
            Description = sub.Description;
            ExpandedDescription = sub.ExpandedDescription;
            AdditionalServicesFee = sub.Fee;
            AdServiceNumber = Convert.ToDouble(sub.PointNumber);

            //do stuff
            //CloseWindow();
        }
        private bool result = false;
        private void SaveSubProject()
        {
            if (!ButtonInAction)
            {
                return;
            }
            ButtonInAction = false;

            SubProjectDbModel subupdate = new SubProjectDbModel()
            {
                Id = SubProject.Id,
                ProjectId = SubProject.ProjectNumber,
                PointNumber = AdServiceNumber.ToString(),
                Description = Description,
                Fee = AdditionalServicesFee,
                IsActive = Convert.ToInt32(SubProject.IsActive),
                PercentComplete = SubProject.PercentComplete,
                PercentBudget = SubProject.PercentBudget,
                IsInvoiced = Convert.ToInt32(SubProject.IsInvoiced),
                ExpandedDescription = ExpandedDescription,
                IsAdservice = Convert.ToInt32(SubProject.IsAddService),
                NumberOrder = SubProject.NumberOrder,
                IsScheduleActive = Convert.ToInt32(SubProject.IsScheduleActive)
            };

            SQLAccess.UpdateSubProjectSummary(subupdate);

            result = true;
            ButtonInAction = true;
            CloseWindow();
        }

        private void CloseWindow()
        {
            if (!ButtonInAction)
            {
                return;
            }
            ButtonInAction = false;


            baseViewModel.CollectSubProjectsInfo();
            baseViewModel.LeftDrawerOpen = false;
        }
    }
}
