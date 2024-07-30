using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SOCE.Library.Db;
using System.Reflection;
using System.ComponentModel;

namespace SOCE.Library.UI.ViewModels
{
    public class YesNoVM : BaseVM
    {
        public bool Result { get; set; }
        public ExpenseProjectModel expenseobject {get;set;}
        private ProjectSummaryVM ProjectSummary { get; set; }
        private ExpenseProjectVM ExpenseProject { get; set; }

        private AddServiceVM AddServiceSummary { get; set; }

        private InvoicingSummaryVM invoicingsummaryvm { get; set; }

        private InvoicingModel baseinvoice { get; set; }
        private int baseindex { get; set; }
        public string Message { get; set; } = "";

        private bool _isSubVisible = false;
        public bool IsSubVisible
        {
            get
            {
                return _isSubVisible;
            }
            set
            {
                _isSubVisible = value;
                RaisePropertyChanged(nameof(IsSubVisible));
            }
        }

        private string _subMessage = "";
        public string SubMessage
        {
            get
            {
                return _subMessage;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    IsSubVisible = true;
                }
                _subMessage = value;
                RaisePropertyChanged(nameof(SubMessage));
            }
        }

        public ICommand YesCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public YesNoVM()
        {
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
        }

        public YesNoVM(EmployeeModel em)
        {
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
            Message = "Are you sure you want to delete:";
            SubMessage = em.FirstName + " " + em.LastName;
        }

        public YesNoVM(ProjectListModel plm)
        {
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
            Message = "Do you want to make this project inactive:";
            SubMessage = $"{plm.ProjectName} [{plm.ProjectNumber}]";
        }

        public YesNoVM(ClientModel cm)
        {
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
            Message = "Are you sure you want to delete:";
            SubMessage = cm.ClientName;
        }

        public YesNoVM(MarketModel mm)
        {
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
            Message = "Are you sure you want to delete:";
            SubMessage = mm.MarketName;
        }

        public YesNoVM(int index, InvoicingSummaryVM basevm)
        {
            baseindex = index;
            invoicingsummaryvm = basevm;
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
        }

        public YesNoVM(InvoicingModel invoice, InvoicingSummaryVM basevm)
        {
            baseinvoice = invoice;
            invoicingsummaryvm = basevm;
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
        }

        public YesNoVM(SubProjectSummaryModel spm, ProjectSummaryVM psm)
        {
            ProjectSummary = psm;
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
            Message = "Are you sure you want to delete:";
            SubMessage = $"Phase: {spm.PointNumber}";
        }

        public YesNoVM(ExpenseProjectModel epm, ExpenseProjectVM epvm)
        {
            ExpenseProject = epvm;
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
            expenseobject = epm;
            Message = "Are you sure you want to delete:";
            SubMessage = $"Expense by {epm.EmployeeExp} ({epm.DateExp.ToString("MM/dd/yyyy")})";
        }

        public YesNoVM(SubProjectAddServiceModel spm, AddServiceVM psm)
        {
            AddServiceSummary = psm;
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
            Message = "Are you sure you want to delete:";
            SubMessage = $"Add-Service: {spm.PointNumber}";
        }

        public YesNoVM(RoleSummaryModel rpspm, ProjectSummaryVM psm)
        {
            ProjectSummary = psm;
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
            string description = GetEnumDescription((DefaultRoleEnum)rpspm.Role);
            Message = "Are you sure you want to delete:";
            SubMessage = $"Role: {description} {Environment.NewLine}Employee: {rpspm.Employee.FullName}";
        }

        public YesNoVM(ProjectViewResModel pm)
        {
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
            Message = "Are you sure you want to delete:";
            SubMessage = pm.ProjectName;
        }

        public YesNoVM(ProposalViewResModel pm)
        {
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
            Message = "Are you sure you want to delete:";
            SubMessage = pm.ProposalName;
        }

        private string GetEnumDescription(Enum value)
        {
            // Get the Description attribute value for the enum value
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public void YesDoTheAction()
        {
            Result = true;
            //do stuff

            CloseWindow();
        }

        private void CancelCommand()
        {
            Result = false;
            //do stuff
            CloseWindow();
        }

        private void CloseWindow()
        {
            if (ProjectSummary == null && AddServiceSummary == null && invoicingsummaryvm == null && baseinvoice == null && ExpenseProject == null)
            {
                bool val = DialogHost.IsDialogOpen("RootDialog");
                DialogHost.Close("RootDialog");
            }
            else if(invoicingsummaryvm != null && baseinvoice == null)
            {
                if (Result)
                {
                    invoicingsummaryvm.DeletefromUI(baseindex);
                }

                invoicingsummaryvm.LeftDrawerOpen = false;
            }
            else if (invoicingsummaryvm != null && baseinvoice != null)
            {
                if (Result)
                {
                    invoicingsummaryvm.UpdateInvoicewithOverhead(baseinvoice);
                }

                invoicingsummaryvm.LeftDrawerOpen = false;
            }
            else if (ExpenseProject != null)
            {
                if (Result)
                {
                    if (expenseobject != null)
                    {
                        SQLAccess.DeleteExpenseReport(expenseobject.Id);
                        ExpenseProject.LoadExpensesForViewing();
                    }
                }
                ExpenseProject.LeftDrawerOpen = false;
            }
            else if (AddServiceSummary != null)
            {
                SubProjectAddServiceModel itemtodelete = (SubProjectAddServiceModel)AddServiceSummary.ItemToDelete;

                if (Result)
                {
                    List<RolePerSubProjectDbModel> roles = SQLAccess.LoadRolesPerSubProject(itemtodelete.Id);

                    foreach (RolePerSubProjectDbModel rpspm in roles)
                    {
                        SQLAccess.DeleteRolesPerSubProject(rpspm.Id);
                    }

                    SQLAccess.DeleteSubProject(itemtodelete.Id);
                    AddServiceSummary.SubProjects.Remove(itemtodelete);
                }
                AddServiceSummary.ItemToDelete = null;
                AddServiceSummary.LeftDrawerOpen = false;
            }
            else
            {
                object itemtodelete = ProjectSummary.ItemToDelete;

                if (Result)
                {
                    switch (itemtodelete)
                    {
                        case SubProjectSummaryModel sub:
                            {
                                List<RolePerSubProjectDbModel> roles = SQLAccess.LoadRolesPerSubProject(sub.Id);
                                foreach (RolePerSubProjectDbModel rpspm in roles)
                                {
                                    SQLAccess.DeleteRolesPerSubProject(rpspm.Id);
                                }
                                SQLAccess.DeleteSubProject(sub.Id);
                                ProjectSummary.CollectSubProjectsInfo();
                                //ProjectSummary.SubProjects.Remove(sub);
                                //ProjectSummary.BaseProject.UpdateSubProjects();
                                //ProjectSummary.SubProjects.Renumber(true);
                                break;
                            }
                        case RoleSummaryModel role:
                            {
                                if (role.SpentHours == 0)
                                {
                                    SQLAccess.DeleteRolesPerSubProject(role.Id);
                                }
                                ProjectSummary.CollectSubProjectsInfo();
                                //ProjectSummary.SelectedProjectPhase.RolesPerSub.Remove(role);
                                //role.Subproject.baseproject.FormatData(false);

                                break;
                            }
                        default:
                            break;
                    }
                }
                ProjectSummary.ItemToDelete = null;
                ProjectSummary.LeftDrawerOpen = false;

            }
        }
    }
}
