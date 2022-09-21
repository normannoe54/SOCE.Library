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
        public string MarketName { get; set; }

        private bool _editFieldState = true;
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

                RaisePropertyChanged(nameof(EditFieldState));
            }
        }

        public MarketModel()
        { }

        public MarketModel(MarketDbModel mdbm)
        {
            Id = pm.Id;
            ProjectName = pm.ProjectName;
            ProjectNumber = pm.ProjectNumber;
            ClientName = "";
            Fee = pm.Fee;
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
