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

        private bool _isFolderorFile;
        public bool IsFolderorFile
        {
            get { return _isFolderorFile; }
            set
            {
                _isFolderorFile = value;
                RaisePropertyChanged(nameof(IsFolderorFile));
            }
        }

        private AsyncObservableCollection<FolderModel> _architecturalFolders = new AsyncObservableCollection<FolderModel>();
        public AsyncObservableCollection<FolderModel> ArchitecturalFolders
        {
            get { return _architecturalFolders; }
            set
            {
                _architecturalFolders = value;
                RaisePropertyChanged(nameof(ArchitecturalFolders));
            }
        }

        private AsyncObservableCollection<FolderModel> _plotFolders = new AsyncObservableCollection<FolderModel>();
        public AsyncObservableCollection<FolderModel> PlotFolders
        {
            get { return _plotFolders; }
            set
            {
                _plotFolders = value;
                RaisePropertyChanged(nameof(PlotFolders));
            }
        }

        private AsyncObservableCollection<FolderModel> _drawingFolders = new AsyncObservableCollection<FolderModel>();
        public AsyncObservableCollection<FolderModel> DrawingFolders
        {
            get { return _drawingFolders; }
            set
            {
                _drawingFolders = value;
                RaisePropertyChanged(nameof(DrawingFolders));
            }
        }

        private AsyncObservableCollection<FolderModel> _projectFolders = new AsyncObservableCollection<FolderModel>();
        public AsyncObservableCollection<FolderModel> ProjectFolders
        {
            get { return _projectFolders; }
            set
            {
                _projectFolders = value;
                RaisePropertyChanged(nameof(ProjectFolders));
            }
        }

        private AsyncObservableCollection<FolderModel> _archiveFolders = new AsyncObservableCollection<FolderModel>();
        public AsyncObservableCollection<FolderModel> ArchiveFolders
        {
            get { return _archiveFolders; }
            set
            {
                _archiveFolders = value;
                RaisePropertyChanged(nameof(ArchiveFolders));
            }
        }


        private string drawing = $"N:\\dwg";
        private string plot = $"N:\\Plot";
        private string architectural = $"N:\\ARCHITECTURALS";
        private string project = $"P:\\";
        private string archive = $"R:\\";
        public NetworkSearchVM(EmployeeModel loggedinEmployee)
        {
            CurrentEmployee = loggedinEmployee;
            this.OpenProjectCommand = new RelayCommand<object>(this.OpenProject);
            this.SearchCommand = new RelayCommand(this.RunSearch);
            this.OpenSearchFiltersCommand = new RelayCommand(this.OpenSearchFilters);
        }

        public async void OpenSearchFilters()
        {
            var view = new SearchFiltersView();
            SearchFiltersVM vm = new SearchFiltersVM(CurrentEmployee);
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
                    DrawingFolders.Clear();
                    List<string> drawingsubs = Directory.GetDirectories(drawing).ToList();
                    foreach (string indd in drawingsubs)
                    {
                        string final = indd.Remove(0, drawing.Length + 1);

                        if (!IsCaseSensitive)
                        {
                            if (final.ToUpper().Contains(TextSearch.ToUpper()))
                            {
                                DrawingFolders.Add(new FolderModel(indd, final));
                            }
                        }
                        else
                        {
                            if (final.Contains(TextSearch))
                            {
                                DrawingFolders.Add(new FolderModel(indd, final));
                            }
                        }
                    }

                    PlotFolders.Clear();
                    List<string> plotsubs = Directory.GetDirectories(plot).ToList();
                    foreach (string indd in plotsubs)
                    {
                        string final = indd.Remove(0, plot.Length + 1);

                        if (!IsCaseSensitive)
                        {
                            if (final.ToUpper().Contains(TextSearch.ToUpper()))
                            {
                                PlotFolders.Add(new FolderModel(indd, final));
                            }
                        }
                        else
                        {
                            if (final.Contains(TextSearch))
                            {
                                PlotFolders.Add(new FolderModel(indd, final));
                            }
                        }
                    }

                    ArchitecturalFolders.Clear();
                    List<string> archsubs = Directory.GetDirectories(architectural).ToList();
                    foreach (string indd in archsubs)
                    {
                        string final = indd.Remove(0, architectural.Length + 1);

                        if (!IsCaseSensitive)
                        {
                            if (final.ToUpper().Contains(TextSearch.ToUpper()))
                            {
                                ArchitecturalFolders.Add(new FolderModel(indd, final));
                            }
                        }
                        else
                        {
                            if (final.Contains(TextSearch))
                            {
                                ArchitecturalFolders.Add(new FolderModel(indd, final));
                            }
                        }
                    }

                    ProjectFolders.Clear();
                    List<string> projsubs0 = Directory.GetDirectories(project).ToList();

                    foreach (string subdirectory1 in projsubs0)
                    {
                        List<string> projsubs1 = Directory.GetDirectories(subdirectory1).ToList();

                        foreach (string subdirectory2 in projsubs1)
                        {
                            List<string> projsubs2 = Directory.GetDirectories(subdirectory2).ToList();

                            foreach (string indd in projsubs2)
                            {
                                string final = indd.Remove(0, subdirectory2.Length + 1);

                                if (!IsCaseSensitive)
                                {
                                    if (final.ToUpper().Contains(TextSearch.ToUpper()))
                                    {
                                        ProjectFolders.Add(new FolderModel(indd, final));
                                    }
                                }
                                else
                                {
                                    if (final.Contains(TextSearch))
                                    {
                                        ProjectFolders.Add(new FolderModel(indd, final));
                                    }
                                }
                            }
                        }
                    }

                    ArchiveFolders.Clear();
                    List<string> archsubs0 = Directory.GetDirectories(archive).ToList();

                    foreach (string subdirectory1 in archsubs0)
                    {
                        List<string> archsubs1 = Directory.GetDirectories(subdirectory1).ToList();

                        foreach (string indd in archsubs1)
                        {
                            string final = indd.Remove(0, subdirectory1.Length + 1);

                            if (!IsCaseSensitive)
                            {
                                if (final.ToUpper().Contains(TextSearch.ToUpper()))
                                {
                                    ArchiveFolders.Add(new FolderModel(indd, final));
                                }
                            }
                            else
                            {
                                if (final.Contains(TextSearch))
                                {
                                    ArchiveFolders.Add(new FolderModel(indd, final));
                                }
                            }
                        }
                    }
                }
                else
                {
                    ArchiveFolders.Clear();
                    ArchitecturalFolders.Clear();
                    ProjectFolders.Clear();
                    DrawingFolders.Clear();
                    PlotFolders.Clear();
                }
            }));
            //List<string> founddrawings = drawingsubs.Where(s => s.Contains(TextSearch)).ToList();
            await Task.Run(() => Task.Delay(600));
            CurrentPage.MakeClear();
            //DrawingFolders = new ObservableCollection<string>(founddrawings);
        }

        public void OpenProject(object o)
        {
            FolderModel folder = (FolderModel)o;
            if (folder != null)
            {
                Process.Start(folder.FullFolderPath);
            }
        }

    }
}
