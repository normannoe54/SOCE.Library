using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCE.Library.UI
{
    public class FolderModel : PropertyChangedBase
    {
        private string _fullFolderPath;
        public string FullFolderPath
        {
            get { return _fullFolderPath; }
            set
            {
                _fullFolderPath = value;
                RaisePropertyChanged(nameof(FullFolderPath));
            }
        }

        private string _partialFolderPath;
        public string PartialFolderPath
        {
            get { return _partialFolderPath; }
            set
            {
                _partialFolderPath = value;
                RaisePropertyChanged(nameof(PartialFolderPath));
            }
        }

        public FolderModel(string fullpath, string partialpath)
        {
            FullFolderPath = fullpath;
            PartialFolderPath = partialpath;
        }

        public FolderModel()
        {

        }
    }
}
