﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SOCE.Library.UI.ViewModels
{
    //[AddINotifyPropertyChangedInterface]
    public class BaseVM : PropertyChangedBase
    {
        private bool _buttonInAction = true;
        public bool ButtonInAction
        {
            get { return _buttonInAction; }
            set
            {
                _buttonInAction = value;
                RaisePropertyChanged(nameof(ButtonInAction));
            }
        }
        /// <summary>
        /// The event that fires when any child property changes its value
        /// </summary>
        //public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        protected async Task RunCommand(Expression<Func<bool>> updatingFlag, Func<Task> action)
        {
            if (updatingFlag.GetPropertyValue())
                return;

            // Set the property flag to true to indicate we are running
            updatingFlag.SetPropertyValue(true);

            try
            {
                // Run the passed action
                await action();
            }
            finally
            {
                // Set the property flag back to false now that it's finished
                updatingFlag.SetPropertyValue(false);
            }
        }

        //public void RaisePropertyChanged(string propertyName)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}
    }
}
