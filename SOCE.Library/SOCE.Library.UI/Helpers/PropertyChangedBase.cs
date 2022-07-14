using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace SOCE.Library.UI
{
    public class PropertyChangedBase : INotifyPropertyChanged
    {
        /// <summary>
        /// The event that fires when any child property changes its value
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
