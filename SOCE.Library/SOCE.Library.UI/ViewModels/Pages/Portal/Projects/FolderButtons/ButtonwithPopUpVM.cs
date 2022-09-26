using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SOCE.Library.Db;

namespace SOCE.Library.UI.ViewModels
{
    public class ButtonwithPopUpVM : BaseVM
    {
        //private PackIconKind _buttonIcon;
        //public PackIconKind ButtonIcon
        //{
        //    get { return _buttonIcon; }
        //    set
        //    {
        //        _buttonIcon = value;
        //        RaisePropertyChanged("ButtonIcon");
        //    }
        //}



        public ICommand OpenFolderCommand{ get; set; }
        public ButtonwithPopUpVM()
        {
            this.OpenFolderCommand = new RelayCommand<PopUpInputVM>(this.OpenFolder);
        }

        public void OpenFolder(PopUpInputVM input)
        {
            
        }

    }
}
