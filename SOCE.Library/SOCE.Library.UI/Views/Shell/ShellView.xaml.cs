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
using System.Threading;

namespace SOCE.Library.UI.Views
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        //CancellationTokenSource cancelSource = new CancellationTokenSource();
        //CancellationToken token = cancelSource.Token;

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
            
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                dragAction = true;
                //this.MouseMove += StrangeMouseMove;
                //this.MouseMove += Window_MouseMove;
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

                    await Task.Delay(1000);

                    if (dragAction)
                    {
                        core.MaximizeWindowCom();
                        Top = Math.Max(point.Y - 5, 0);

                        if (point.X > 1800)
                        {
                            Left = 1920;
                        }
                        else
                        {
                            Left = 0;
                        }

                        var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
                        //
                        //Left = point.X - Left
                        //if (dragAction == true)
                        //{
                            this.DragMove();
                            //Left = Math.Max(ActualWidth - point.X, 0);
                            //
                        //}
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
            //}
            //catch
            //{
            //    this.MouseMove -= Window_MouseMove;
            //}


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
            //if (dragAction)
            //{
            //    cancelSource.Cancel();
            //}

             dragAction = false;
            
            //this.MouseMove -= Window_MouseMove;
        }
    }
}
