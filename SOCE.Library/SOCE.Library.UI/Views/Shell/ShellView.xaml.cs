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
using System.Windows.Interop;

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

        private async void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    dragAction = true;
                    CoreAI basevm = (CoreAI)DataContext;
                    if (basevm.WindowType == WindowState.Maximized)
                    {
                        //double widthref = System.Windows.SystemParameters.PrimaryScreenWidth;
                        System.Drawing.Point point = System.Windows.Forms.Cursor.Position;
                        if (point.Y < 50)
                        {
                            
                            await Task.Delay(300);

                            if (dragAction)
                            {
                                WindowInteropHelper windowInteropHelper = new WindowInteropHelper(System.Windows.Application.Current.MainWindow);
                                Screen screen = System.Windows.Forms.Screen.FromHandle(windowInteropHelper.Handle);
                                basevm.MaximizeWindowCom();
                                Top = 0;

                                double val = Convert.ToDouble(screen.Bounds.X) + (Convert.ToDouble(point.X - screen.Bounds.X) - (((Convert.ToDouble(point.X) - Convert.ToDouble(screen.Bounds.X)) / Convert.ToDouble(screen.Bounds.Width)) * Convert.ToDouble(basevm.WidthRef)));
                                Left = Math.Max(val, 0);
                                //if (point.X > 1800)
                                //{
                                //    Left = 1920 + 1920/2;
                                //}
                                //else
                                //{
                                //    Left = 1920 / 2;
                                //}

                                var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
                                this.MouseMove += Window_MouseMove;
                            }
                        }
                    }
                    else
                    {
                        this.MouseMove += Window_MouseMove;
                    }

                    ////this.MouseMove += StrangeMouseMove;
                    ////await Task.Delay(1000);
                    //if (dragAction)
                    //{
                    //    this.MouseMove += Window_MouseMove;
                    //    Window_MouseMove(this, e);
                    //    this.MouseMove -= Window_MouseMove;
                    //}

                }
            }
            catch
            {
                this.MouseMove -= Window_MouseMove;
            }

        }

        private void Window_MouseUp(object sender, System.Windows.Input.MouseEventArgs e)
        {
            dragAction = false;
            this.MouseMove -= Window_MouseMove;
        }

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //CoreAI core = (CoreAI)this.DataContext;
            //if (core.WindowType == WindowState.Maximized)
            //{
            //    System.Drawing.Point point = System.Windows.Forms.Cursor.Position;
            //    if (point.Y < 50)
            //    {


            //        if (dragAction)
            //        {
            //            core.MaximizeWindowCom();
            //            Top = Math.Max(point.Y - 5, 0);

            //            if (point.X > 1800)
            //            {
            //                Left = 1920;
            //            }
            //            else
            //            {
            //                Left = 0;
            //            }

            //            var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;

            //            this.DragMove();

            //        }



            //    }
            //}
            //else
            //{
            if (dragAction == true)
            {
                try
                {
                    this.DragMove();
                }
                catch { }
            }
            //}
            this.MouseMove -= Window_MouseMove;

        }


    }
}
