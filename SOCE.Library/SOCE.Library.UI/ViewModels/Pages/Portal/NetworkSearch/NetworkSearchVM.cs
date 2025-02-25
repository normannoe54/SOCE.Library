using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;
using SOCE.Library.Db;
using SOCE.Library.UI.Views;

namespace SOCE.Library.UI.ViewModels
{
    public class NetworkSearchVM : BaseVM
    {
        private EmployeeModel _currentEmployee;
        public EmployeeModel CurrentEmployee
        {
            get
            {
                return _currentEmployee;
            }
            set
            {
                _currentEmployee = value;
                RaisePropertyChanged(nameof(CurrentEmployee));
            }
        }
        public ICommand OpenProjectCommand { get; set; }
        public ICommand SearchCommand { get; set; }

        public ICommand OpenSearchFiltersCommand { get; set; }

        private string _textSearch;
        public string TextSearch
        {
            get { return _textSearch; }
            set
            {
                _textSearch = value;
                RaisePropertyChanged(nameof(TextSearch));
            }
        }

        private bool _isCaseSensitive;
        public bool IsCaseSensitive
        {
            get { return _isCaseSensitive; }
            set
            {
                _isCaseSensitive = value;
                RaisePropertyChanged(nameof(IsCaseSensitive));
            }
        }

        //private bool _isFolderorFile;
        //public bool IsFolderorFile
        //{
        //    get { return _isFolderorFile; }
        //    set
        //    {
        //        _isFolderorFile = value;
        //        RaisePropertyChanged(nameof(IsFolderorFile));
        //    }
        //}

        //private AsyncObservableCollection<FolderModel> _architecturalFolders = new AsyncObservableCollection<FolderModel>();
        //public AsyncObservableCollection<FolderModel> ArchitecturalFolders
        //{
        //    get { return _architecturalFolders; }
        //    set
        //    {
        //        _architecturalFolders = value;
        //        RaisePropertyChanged(nameof(ArchitecturalFolders));
        //    }
        //}

        //private AsyncObservableCollection<FolderModel> _plotFolders = new AsyncObservableCollection<FolderModel>();
        //public AsyncObservableCollection<FolderModel> PlotFolders
        //{
        //    get { return _plotFolders; }
        //    set
        //    {
        //        _plotFolders = value;
        //        RaisePropertyChanged(nameof(PlotFolders));
        //    }
        //}

        //private AsyncObservableCollection<FolderModel> _drawingFolders = new AsyncObservableCollection<FolderModel>();
        //public AsyncObservableCollection<FolderModel> DrawingFolders
        //{
        //    get { return _drawingFolders; }
        //    set
        //    {
        //        _drawingFolders = value;
        //        RaisePropertyChanged(nameof(DrawingFolders));
        //    }
        //}

        //private AsyncObservableCollection<FolderModel> _projectFolders = new AsyncObservableCollection<FolderModel>();
        //public AsyncObservableCollection<FolderModel> ProjectFolders
        //{
        //    get { return _projectFolders; }
        //    set
        //    {
        //        _projectFolders = value;
        //        RaisePropertyChanged(nameof(ProjectFolders));
        //    }
        //}

        private ObservableCollection<SearchFilterModel> _searchFilters = new ObservableCollection<SearchFilterModel>();
        public ObservableCollection<SearchFilterModel> SearchFilters
        {
            get { return _searchFilters; }
            set
            {
                _searchFilters = value;
                RaisePropertyChanged(nameof(SearchFilters));
            }
        }

        //private AsyncObservableCollection<FolderModel> _archiveFolders = new AsyncObservableCollection<FolderModel>();
        //public AsyncObservableCollection<FolderModel> ArchiveFolders
        //{
        //    get { return _archiveFolders; }
        //    set
        //    {
        //        _archiveFolders = value;
        //        RaisePropertyChanged(nameof(ArchiveFolders));
        //    }
        //}


        //private string drawing = $"N:\\dwg";
        //private string plot = $"N:\\Plot";
        //private string architectural = $"N:\\ARCHITECTURALS";
        //private string project = $"P:\\";
        //private string archive = $"R:\\";
        public NetworkSearchVM(EmployeeModel loggedinEmployee)
        {
            CurrentEmployee = loggedinEmployee;
            this.OpenProjectCommand = new RelayCommand<object>(this.OpenProject);
            this.SearchCommand = new RelayCommand(this.RunSearch);
            this.OpenSearchFiltersCommand = new RelayCommand(this.OpenSearchFilters);

            LoadFilters();
        }

        public void LoadFilters()
        {
            List<SearchFilterDbModel> searchfilters = SQLAccess.LoadSearchFilterByEmployeeId(CurrentEmployee.Id);
            List<SearchFilterModel> filters = new List<SearchFilterModel>();

            if (searchfilters.Count == 0)
            {
                //do this later
                CreateSearchFilters();
                searchfilters = SQLAccess.LoadSearchFilterByEmployeeId(CurrentEmployee.Id);
            }

            foreach (SearchFilterDbModel search in searchfilters)
            {
                if (Convert.ToBoolean(search.Active))
                {
                    SearchFilterModel sfm = new SearchFilterModel(search);
                    filters.Add(sfm);
                }
            }

            List<SearchFilterModel> orderedfilters = filters.OrderBy(x => x.NumberOrder).ToList();

            SearchFilters = new ObservableCollection<SearchFilterModel>(orderedfilters);
        }

        public void CreateSearchFilters()
        {
            //only happens if they have no search filters
            SearchFilterDbModel projectfolder = new SearchFilterDbModel()
            {
                EmployeeId = CurrentEmployee.Id,
                Header = "Project Folder",
                FolderPath = @"P:\",
                NumberOrder = 0,
                Active = 1,
                SearchFileType = 1,
                SubLayer = 3
            };
            int pid = SQLAccess.AddSearchFilter(projectfolder);

            SearchFilterDbModel drawingfolder = new SearchFilterDbModel()
            {
                EmployeeId = CurrentEmployee.Id,
                Header = "Drawing Folder",
                FolderPath = @"N:\dwg",
                NumberOrder = 1,
                Active = 1,
                SearchFileType = 1,
                SubLayer = 1
            };
            int did = SQLAccess.AddSearchFilter(drawingfolder);

            SearchFilterDbModel archfolder = new SearchFilterDbModel()
            {
                EmployeeId = CurrentEmployee.Id,
                Header = "Architecturals Folder",
                FolderPath = @"N:\ARCHITECTURALS",
                NumberOrder = 2,
                Active = 1,
                SearchFileType = 1,
                SubLayer = 1
            };
            int archid = SQLAccess.AddSearchFilter(archfolder);

            SearchFilterDbModel plotfolder = new SearchFilterDbModel()
            {
                EmployeeId = CurrentEmployee.Id,
                Header = "Plot Folder",
                FolderPath = @"N:\Plot",
                NumberOrder = 3,
                Active = 1,
                SearchFileType = 1,
                SubLayer = 1
            };
            int plotid = SQLAccess.AddSearchFilter(plotfolder);

            SearchFilterDbModel archivefolder = new SearchFilterDbModel()
            {
                EmployeeId = CurrentEmployee.Id,
                Header = "Archive Folder",
                FolderPath = @"R:\",
                NumberOrder = 3,
                Active = 1,
                SearchFileType = 1,
                SubLayer = 2
            };
            int archiveid = SQLAccess.AddSearchFilter(archivefolder);
        }

        public async void OpenSearchFilters()
        {
            var view = new SearchFiltersView();
            SearchFiltersVM vm = new SearchFiltersVM(CurrentEmployee, this);
            view.DataContext = vm;
            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog");
        }

        public async void RunSearch()
        {
            CoreAI CurrentPage = IoCCore.Application as CoreAI;
            CurrentPage.MakeBlurry();
            await Task.Run(() => Task.Delay(600));
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                if (!String.IsNullOrEmpty(TextSearch))
                {
                    foreach(SearchFilterModel sfm in SearchFilters)
                    {
                        sfm.Folders.Clear();

                        if (sfm.SearchType)
                        {
                            List<string> list1 = Directory.GetDirectories(sfm.BaseFolderPath).ToList();

                            List<string> list2 = new List<string>();

                            int i = 1;

                            while (i < sfm.SubLayer)
                            {
                                foreach(string indd in list1)
                                {
                                    List<string> founditems = new List<string>();
                                    founditems = Directory.GetDirectories(indd).ToList();
                                    list2.AddRange(founditems);
                                }

                                list1 = new List<string>(list2);
                                list2.Clear();
                                i++;
                            }

                            foreach (string indd in list1)
                            {
                                int pos = indd.LastIndexOf("\\") + 1;
                                string final = indd.Substring(pos, indd.Length - pos);
                                if (!IsCaseSensitive)
                                {
                                    if (final.ToUpper().Contains(TextSearch.ToUpper()))
                                    {
                                        sfm.Folders.Add(new FolderModel(indd, final));
                                    }
                                }
                                else
                                {
                                    if (final.Contains(TextSearch))
                                    {
                                        sfm.Folders.Add(new FolderModel(indd, final));
                                    }
                                }
                            }

                        }
                        else
                        {
                            List<string> list1 = Directory.GetFiles(sfm.BaseFolderPath, "*", SearchOption.AllDirectories).ToList();

                            foreach (string indd in list1)
                            {
                                //string final = indd.Remove(0, sfm.BaseFolderPath.Length + 1);

                                int pos = indd.LastIndexOf("\\") + 1;
                                string final = indd.Substring(pos, indd.Length - pos);
                                if (!IsCaseSensitive)
                                {
                                    if (final.ToUpper().Contains(TextSearch.ToUpper()))
                                    {
                                        sfm.Folders.Add(new FolderModel(indd, final));
                                    }
                                }
                                else
                                {
                                    if (final.Contains(TextSearch))
                                    {
                                        sfm.Folders.Add(new FolderModel(indd, final));
                                    }
                                }
                            }
                        }

                        
                    }
                }
                else
                {
                    foreach (SearchFilterModel sfm in SearchFilters)
                    {
                        sfm.Folders.Clear();
                    }
                }
            }));
            await Task.Run(() => Task.Delay(600));
            CurrentPage.MakeClear();
        }

        public void OpenProject(object o)
        {
            FolderModel folder = (FolderModel)o;

            if (folder != null)
            {
                try
                {
                    string path = folder.FullFolderPath;
                    FileAttributes attr = File.GetAttributes(folder.FullFolderPath);

                    //detect whether its a directory or file
                    if ((attr & FileAttributes.Directory) != FileAttributes.Directory)
                    { 
                        path = Path.GetDirectoryName(folder.FullFolderPath); 
                    }
                    
                    Process.Start(path);
                }
                catch
                {
                }
            }
        }
    }
}
