using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SOCE.Library.Db;

namespace SOCE.Library.UI.ViewModels
{
    public class AddMarketVM : BaseVM
    {
        private string _marketNameInp;
        public string MarketNameInp
        {
            get { return _marketNameInp; }
            set
            {
                _marketNameInp = value;
                RaisePropertyChanged("MarketNameInp");
            }
        }

        public ICommand AddMarketCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public AddMarketVM()
        {
            this.AddMarketCommand = new RelayCommand(this.AddMarket);
            this.CloseCommand = new RelayCommand(this.CloseWindow);
        }

        public void AddMarket()
        {
            MarketDbModel market = new MarketDbModel()
            { MarketName = MarketNameInp, IsActive = 1 };

            SQLAccess.AddMarket(market);

            //do stuff
            CloseWindow();
        }

        private void CloseWindow()
        {
            DialogHost.Close("RootDialog");
        }
    }
}
