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

namespace SOCE.Library.UI.Views
{
    /// <summary>
    /// Interaction logic for WeeklyScheduleView.xaml
    /// </summary>
    public partial class ScheduleWeekView : UserControl
    {
        bool isactive = true;

        TimeSpan timer;
        public ScheduleWeekView()
        {
            //Focusable = true;
            //Loaded += (s, e) => Keyboard.Focus(this);
            timer = DateTime.Now.TimeOfDay;
            InitializeComponent();
        }

        //private void ItemsControl_KeyDown(object sender, KeyEventArgs e)
        //{
        //    var list = sender as ListView;
        //    switch (e.Key)
        //    {
        //        case Key.Right:
        //            if (!list.Items.MoveCurrentToNext()) list.Items.MoveCurrentToLast();
        //            break;

        //        case Key.Left:
        //            if (!list.Items.MoveCurrentToPrevious()) list.Items.MoveCurrentToFirst();
        //            break;
        //    }

        //    e.Handled = true;

        //    if (list.SelectedItem != null)
        //    {
        //        (Keyboard.FocusedElement as UIElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        //    }
        //}

        //private void ComboBox_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        //{
        //    if (isactive)
        //    {
        //        e.Handled = true;
        //    }
        //}

        //private async void ComboBox_KeyDown(object sender, KeyEventArgs e)
        //{
        //    timer = DateTime.Now.TimeOfDay;

        //    isactive = false;

        //    await Task.Delay(200);

        //    TimeSpan difftime = DateTime.Now.TimeOfDay - timer;
        //    TimeSpan limit = TimeSpan.FromSeconds(0.15);

        //    if (!isactive && difftime >= limit)
        //    {
        //        isactive = true;
        //    }
        //}

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
