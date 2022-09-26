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

        public override bool Equals(object obj)
        {
            MarketModel em = (MarketModel)obj;

            if (em == null)
            {
                return false;
            }

            return em != null && Id == em.Id && MarketName == em.MarketName;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                hash = hash * 23 + Id.GetHashCode();
                hash = hash * 23 + MarketName.GetHashCode();
                return hash;
            }
        }

    }
}
