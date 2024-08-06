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

        private string _errorMessage = "";
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                RaisePropertyChanged("ErrorMessage");
            }
        }

        public ICommand AddMarketCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public AddMarketVM()
        {
            ButtonInAction = true;
            this.AddMarketCommand = new RelayCommand(this.AddMarket);
            this.CloseCommand = new RelayCommand(this.CloseWindow);
        }

        public void AddMarket()
        {
            if (!ButtonInAction)
            {
                return;
            }
            ButtonInAction = false;

            MarketDbModel market = new MarketDbModel()
            { MarketName = MarketNameInp, IsActive = 1 };

            int id = SQLAccess.AddMarket(market);

            if (id == 0)
            {
                ErrorMessage = $"Market name exists already";
                return;
            }

            //do stuff
            CloseWindow();
        }

        private void CloseWindow()
        {
            DialogHost.Close("RootDialog");
        }
    }
}
