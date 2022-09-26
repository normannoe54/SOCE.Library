using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SOCE.Library.Db;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace SOCE.Library.UI.ViewModels
{
    public class PopUpInputVM : BaseVM
    {
        private string _filename;
        public string Filename
        {
            get { return _filename; }
            set
            {
                _filename = value;
                RaisePropertyChanged("Filename");
            }
        }

        public ICommand SelectFolderCommand { get; set; }
        public ICommand CopyFileCommand { get; set; }
        public PopUpInputVM()
        {
            this.SelectFolderCommand = new RelayCommand(this.SelectFolder);
            this.CopyFileCommand = new RelayCommand(this.CopyFolder);
        }

        public void SelectFolder()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            //dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;

            // Process open file dialog box results
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                //save file
                Filename = dialog.FileName;
            }
        }

        public void CopyFolder()
        {
            Clipboard.SetText(Filename);
        }


    }
}
