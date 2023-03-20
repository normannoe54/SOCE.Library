using System;
using System.Collections.Generic;
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

namespace SOCE.Library.UI.Views
{
    /// <summary>
    /// Interaction logic for TimesheetView.xaml
    /// </summary>
    public partial class TimesheetView : UserControl
    {
        bool isactive = true;

        TimeSpan timer;
        public TimesheetView()
        {
            InitializeComponent();
            timer= DateTime.Now.TimeOfDay;
            //this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);
            //this.RequestBringIntoView += new RequestBringIntoViewEventHandler(ComboBox_RequestBringIntoView);
        }

        private void ComboBox_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            if (isactive)
            {
                e.Handled = true;
            }
        }

        private async void ComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            timer = DateTime.Now.TimeOfDay;

            isactive = false;

            await Task.Delay(200);

            TimeSpan difftime = DateTime.Now.TimeOfDay - timer;
            TimeSpan limit = TimeSpan.FromSeconds(0.15);

            if (!isactive && difftime >= limit)
            {
                isactive = true;
            }
        }
    }
}
