using SOCE.Library.UI.ViewModels;
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
using System.Windows.Shapes;
using System.Windows.Forms;

namespace SOCE.Library.UI.Views
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        public bool dragAction = false;
        public ShellView()
        {
            //microsoft get your shit togethor
            //https://stackoverflow.com/questions/36811199/trouble-binding-a-floating-point-input-field-in-wpf
            FrameworkCompatibilityPreferences.KeepTextBoxDisplaySynchronizedWithTextProperty = false;

            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.DataContext = IoCCore.Application;
            this.MouseDown += Window_MouseDown;
            this.MouseUp += Window_MouseUp;
            //this.MouseMove += Window_MouseMove;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var point = e.GetPosition(this);

                dragAction = true;
                //await Task.Delay(1000);
                Window_MouseMove(this, e);
            }
        }

        private async void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            CoreAI core = (CoreAI)this.DataContext;
            if (core.WindowType == WindowState.Maximized)
            {
                //var point = e.GetPosition(this);
                System.Drawing.Point point = System.Windows.Forms.Cursor.Position;
                if (point.Y < 50)
                {
                    await Task.Delay(120);
                    core.MaximizeWindowCom();
                    var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
                    Top = Math.Max(point.Y - 5,0);
                    Left = point.X - ActualWidth*0.5;
                    if (dragAction == true)
                    {
                        this.DragMove();
                    }
                }          
            }
            else
            {
                if (dragAction == true)
                {
                    this.DragMove();
                }
            }

            

            //if (e.LeftButton == MouseButtonState.Pressed)
            //{
            //    CoreAI core = (CoreAI)this.DataContext;
            //    if (core.WindowType == WindowState.Maximized)
            //    {
            //        core.MaximizeWindowCom();

            //        this.DragMove();
            //    }
            //}
        }

        private void Window_MouseUp(object sender, System.Windows.Input.MouseEventArgs e)
        {
            dragAction = false;
        }
    }
}
