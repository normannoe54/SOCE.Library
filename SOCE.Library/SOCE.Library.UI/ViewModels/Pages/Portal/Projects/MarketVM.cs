using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SOCE.Library.Db;
using System.Collections.ObjectModel;
using System.Linq;
using SOCE.Library.UI.Views;

namespace SOCE.Library.UI.ViewModels
{
    public class MarketVM : BaseVM
    {

        private ObservableCollection<MarketModel> _markets = new ObservableCollection<MarketModel>();
        public ObservableCollection<MarketModel> Markets
        {
            get { return _markets; }
            set
            {
                _markets = value;
                RaisePropertyChanged(nameof(Markets));
            }
        }


        private bool _canAddProject = false;
        public bool CanAddProject
        {
            get
            {
                return _canAddProject;
            }
            set
            {
                _canAddProject = value;
                RaisePropertyChanged(nameof(CanAddProject));
            }
        }


        public ICommand GoToAddMarket { get; set; }

        public ICommand GoBackCommand { get; set; }
        public ICommand DeleteMarket { get; set; }

        private ProjectVM baseproject;
        public MarketVM(ProjectVM projectvm, bool canadd)
        {
            baseproject = projectvm;
            this.GoToAddMarket = new RelayCommand<object>(this.ExecuteRunAddMarketDialog);
            this.GoBackCommand = new RelayCommand(this.GoBack);
            this.DeleteMarket = new RelayCommand<object>(this.ExecuteRunDeleteDialog);
            CanAddProject = canadd;
            LoadMarkets();
        }

        private void GoBack()
        {
            BaseAI CurrentPage = IoCPortal.Application as BaseAI;
            PortalAI portAI = (PortalAI)CurrentPage;

            portAI.RightDrawerOpen = false;
        }

        private async void ExecuteRunAddMarketDialog(object o)
        {
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            var view = new AddMarketView();

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog");
            //if (result != null)
            //{
            LoadMarkets();
            //}
        }

        private void LoadMarkets()
        {
            List<MarketDbModel> dbmarkets = SQLAccess.LoadMarkets();

            ObservableCollection<MarketModel> members = new ObservableCollection<MarketModel>();

            foreach (MarketDbModel mdbm in dbmarkets)
            {
                members.Add(new MarketModel(mdbm));
            }

            Markets = members;
            baseproject.LoadMarkets();
        }

        private async void ExecuteRunDeleteDialog(object o)
        {
            AreYouSureView view = new AreYouSureView();
            AreYouSureVM aysvm = new AreYouSureVM();
            MarketModel mm = new MarketModel();
            bool donothing = false;


            List<ProjectDbModel> project2 = SQLAccess.LoadProjects();
            List<ProjectDbModel> projmarket = project2.Where(x => x.MarketId == mm.Id).ToList();

            if (projmarket.Count > 0)
            {
                aysvm.TopLine = "Cannot delete market,";
                aysvm.BottomLine = "market assigned to existing project";
                donothing = true;

            }

            aysvm = new AreYouSureVM(mm);

            view.DataContext = aysvm;
            var result = await DialogHost.Show(view, "RootDialog");

            //show the dialog

            aysvm = view.DataContext as AreYouSureVM;

            if (aysvm.Result && !donothing)
            {

                SQLAccess.DeleteMarket(mm.Id);
                LoadMarkets();

            }
        }
    }
}
