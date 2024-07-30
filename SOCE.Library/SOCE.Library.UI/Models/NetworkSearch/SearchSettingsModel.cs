using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCE.Library.UI
{
    public class SearchSettingsModel : PropertyChangedBase
    {
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

        private string _folderPath;
        public string FolderPath
        {
            get { return _folderPath; }
            set
            {
                _folderPath = value;
                RaisePropertyChanged(nameof(FolderPath));
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

        private bool _active;
        public bool Active
        {
            get { return _active; }
            set
            {
                _active = value;
                RaisePropertyChanged(nameof(Active));
            }
        }

        public SearchSettingsModel(string header, string folderpath, int numorder, bool active)
        {
            FolderPath = folderpath;
            Header = header;
            NumberOrder = numorder;
            Active = active;

        }

    }
}
