using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SOCE.Library.UI.Views
{
    /// <summary>
    /// Interaction logic for Signup.xaml
    /// </summary>
    public partial class Signup : UserControl
    {
        public Signup()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            { ((dynamic)this.DataContext).Register.Password = ((PasswordBox)sender).Password; }
        }

        //private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        //{
        //    if (this.DataContext != null)
        //    { ((dynamic)this.DataContext).Register.Password = ((PasswordBox)sender).Password; }
        //}
    }
}
