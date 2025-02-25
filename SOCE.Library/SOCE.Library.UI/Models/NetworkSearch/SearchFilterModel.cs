using SOCE.Library.Db;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCE.Library.UI
{
    public class SearchFilterModel : PropertyChangedBase
    {
        private ObservableCollection<FolderModel> _folders = new ObservableCollection<FolderModel>();
        public ObservableCollection<FolderModel> Folders
        {
            get { return _folders; }
            set
            {
                _folders = value;
                RaisePropertyChanged(nameof(Folders));
            }
        }

        private string _header;
        public string Header
        {
            get { return _header; }
            set
            {
                _header = value;
                RaisePropertyChanged(nameof(Header));
            }
        }

        private string _baseFolderPath;
        public string BaseFolderPath
        {
            get { return _baseFolderPath; }
            set
            {
                _baseFolderPath = value;
                RaisePropertyChanged(nameof(BaseFolderPath));
            }
        }

        private int _numberOrder;
        public int NumberOrder
        {
            get { return _numberOrder; }
            set
            {
                _numberOrder = value;
                RaisePropertyChanged(nameof(NumberOrder));
            }
        }

        private bool _searchType;
        public bool SearchType
        {
            get { return _searchType; }
            set
            {
                _searchType = value;
                RaisePropertyChanged(nameof(SearchType));
            }
        }

        private int _subLayer;
        public int SubLayer
        {
            get { return _subLayer; }
            set
            {
                _subLayer = value;
                RaisePropertyChanged(nameof(SubLayer));
            }
        }

        public SearchFilterModel(SearchFilterDbModel filter)
        {
            NumberOrder = filter.NumberOrder;
            Header = filter.Header;
            BaseFolderPath = filter.FolderPath;
            SearchType = Convert.ToBoolean(filter.SearchFileType);
            SubLayer = filter.SubLayer;
        }
    }
}
