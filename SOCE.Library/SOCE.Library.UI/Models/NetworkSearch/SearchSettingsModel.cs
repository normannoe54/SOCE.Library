using SOCE.Library.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCE.Library.UI
{
    public class SearchSettingsModel : PropertyChangedBase
    {
        public int Id;


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

        private bool _active = true;
        public bool Active
        {
            get { return _active; }
            set
            {
                _active = value;
                RaisePropertyChanged(nameof(Active));
            }
        }

        private string _searchFileName = "Folder";
        public string SearchFileName
        {
            get { return _searchFileName; }
            set
            {
                _searchFileName = value;
                RaisePropertyChanged(nameof(SearchFileName));
            }
        }

        private bool _searchFileType = true;
        public bool SearchFileType
        {
            get { return _searchFileType; }
            set
            {
                _searchFileType = value;

                SearchFileName = _searchFileType ? "Folder" : "File";

                if (!onstart)
                {
                    if (SearchFileType)
                    {
                        SubLayer = 1;
                    }
                    else
                    {
                        SubLayer = 0;
                    }
                }

                RaisePropertyChanged(nameof(SearchFileType));
            }
        }

        private int _subLayer =1;
        public int SubLayer
        {
            get { return _subLayer; }
            set
            {
                _subLayer = value;
                RaisePropertyChanged(nameof(SubLayer));
            }
        }

        public bool onstart = true;
        public SearchSettingsModel()
        {}

        public SearchSettingsModel(SearchFilterDbModel dbmod)
        {
            Id = dbmod.Id;
            FolderPath = dbmod.FolderPath;
            Header = dbmod.Header;
            NumberOrder = dbmod.NumberOrder;
            Active = Convert.ToBoolean(dbmod.Active);
            SearchFileType = Convert.ToBoolean(dbmod.SearchFileType);
            SubLayer = dbmod.SubLayer;
            onstart = false;
        }

        //public SearchSettingsModel(string header, string folderpath, int numorder, bool active)
        //{
        //    FolderPath = folderpath;
        //    Header = header;
        //    NumberOrder = numorder;
        //    Active = active;

        //}

    }
}
