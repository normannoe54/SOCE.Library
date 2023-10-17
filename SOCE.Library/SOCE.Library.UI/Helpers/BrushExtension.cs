using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SOCE.Library.Db;
using System.Collections.ObjectModel;
using SOCE.Library.UI.Views;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Media;

namespace SOCE.Library.UI
{
    public static class BrushExtension
    {
        public static Brush Blend(this Brush color, Brush backColor, double amount)
        {
            Color frontcolor = ((SolidColorBrush)color).Color;
            Color backcolor = ((SolidColorBrush)backColor).Color;
            byte r = (byte)(frontcolor.R * amount + backcolor.R * (1 - amount));
            byte g = (byte)(frontcolor.G * amount + backcolor.G * (1 - amount));
            byte b = (byte)(frontcolor.B * amount + backcolor.B * (1 - amount));
            Brush brush = new SolidColorBrush(Color.FromArgb(255, (byte)r, (byte)g, (byte)b));
            return brush;
        }
    }
}
