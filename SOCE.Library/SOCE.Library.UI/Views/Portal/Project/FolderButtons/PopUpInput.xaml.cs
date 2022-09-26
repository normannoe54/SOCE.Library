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

namespace SOCE.Library.UI.Views
{
    /// <summary>
    /// Interaction logic for PopUpInput.xaml
    /// </summary>
    public partial class PopUpInput : UserControl
    {
        public PopUpInput()
        {
            InitializeComponent();
            DataContext = new PopUpInputVM();
        }
    }
}
