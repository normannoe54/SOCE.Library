using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Media;
using SOCE.Library.Db;

namespace SOCE.Library.UI
{
    public class MarketModel : PropertyChangedBase
    {
        public int Id { get; set; }

        private string _marketName { get; set; }
        public string MarketName
        {
            get
            {
                return _marketName;
            }
            set
            {
                _marketName = value;
                RaisePropertyChanged(nameof(MarketName));
            }
        }

        private bool _editFieldMarketState = true;
        public bool EditFieldMarketState
        {
            get { return _editFieldMarketState; }
            set
            {
                if (!_editFieldMarketState && value)
                {
                    UpdateMarket();
                }
                _editFieldMarketState = value;

                RaisePropertyChanged(nameof(EditFieldMarketState));
            }
        }

        public MarketModel()
        { }

        public MarketModel(MarketDbModel mdbm)
        {
            Id = mdbm.Id;
            MarketName = mdbm.MarketName;
        }

        public void UpdateMarket()
        {
            MarketDbModel market = new MarketDbModel()
            {
                Id = Id,
                MarketName = MarketName,
            };

            SQLAccess.UpdateMarket(market);
        }

    }
}
