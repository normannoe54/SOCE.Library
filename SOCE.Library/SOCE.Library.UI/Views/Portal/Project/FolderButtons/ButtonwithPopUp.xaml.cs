using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SOCE.Library.UI.ViewModels;
using MaterialDesignThemes.Wpf;

namespace SOCE.Library.UI.Views
{
    /// <summary>
    /// Interaction logic for PopUpInput.xaml
    /// </summary>
    public partial class ButtonwithPopUp : UserControl
    {
        public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register("Icon", typeof(PackIconKind), typeof(ButtonwithPopUp));

        //public static readonly DependencyProperty FilenameProperty =
        //    DependencyProperty.Register("Filename", typeof(string), typeof(ButtonwithPopUp),new PropertyMetadata("", OnMyDependencyPropertyChanged));

        public PackIconKind Icon
        {
            get
            {
                return (PackIconKind)this.GetValue(IconProperty);
            }
            set
            {
                this.SetValue(IconProperty, value);
            }

        }

        //public string Filename
        //{
        //    get
        //    {
        //        return (string)this.GetValue(FilenameProperty);
        //    }
        //    set
        //    {
        //        this.SetValue(FilenameProperty, value);
        //    }

        //}

        //private static void OnMyDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var target = (ButtonwithPopUp)d;
        //    PackIconKind oldMyDependencyProperty = (PackIconKind)e.OldValue;
        //    PackIconKind newMyDependencyProperty = target.Icon;
        //    vm.ButtonIcon = newMyDependencyProperty;
            //target.OnMyDependencyPropertyChanged(oldMyDependencyProperty, newMyDependencyProperty);
        //}

        //protected virtual void OnMyDependencyPropertyChanged(PackIconKind oldMyDependencyProperty, PackIconKind newMyDependencyProperty)
        //{
        //    vm = (ButtonwithPopUpVM)DataContext;
        //    vm.ButtonIcon = Icon;
        //}

        public ButtonwithPopUp()
        {
            InitializeComponent();

            //var nameOfPropertyInVm = "Icon";
            //var binding = new Binding(nameOfPropertyInVm) { Mode = BindingMode.TwoWay };
            //this.SetBinding(SearchStringProperty, binding);
            DataContext = new ButtonwithPopUpVM();
        }
    }
}
