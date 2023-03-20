using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SOCE.Library.UI
{
    public class AsyncRelayCommand : ICommand
    {
        /// <summary>
        /// The action to run
        /// </summary>
        private Action mAction;

        /// <summary>
        /// The event fired when <see cref="CanExecute(object)"/> value is changed
        /// </summary>
        public event EventHandler CanExecuteChanged = (sender, e) => { };

        public AsyncRelayCommand(Action action)
        {
            mAction = action;
        }

        public bool CanExecute(object parameter)
        {
            // A relay command can always execute
            return true;
        }

        public void Execute(object parameter)
        {
            Task.Run(() =>
            {
                mAction();
            });
        }
    }

    public class AsyncRelayCommand<T> : ICommand
    {
        /// <summary>
        /// The action to run
        /// </summary>
        private Action<T> mAction;

        /// <summary>
        /// The event fired when <see cref="CanExecute(object)"/> value is changed
        /// </summary>
        public event EventHandler CanExecuteChanged = (sender, e) => { };

        public AsyncRelayCommand(Action<T> action)
        {
            mAction = action;
        }

        public bool CanExecute(object parameter)
        {
            // A relay command can always execute
            return true;
        }

        public void Execute(object parameter)
        {
            Task.Run(() =>
            {
                mAction((T)parameter);

            });
        }
    }

    //public class RelayCommandWithReturn<T> : ICommand
    //{
    //    /// <summary>
    //    /// The action to run
    //    /// </summary>
    //    private Action<T> mAction;

    //    /// <summary>
    //    /// The event fired when <see cref="CanExecute(object)"/> value is changed
    //    /// </summary>
    //    public event EventHandler CanExecuteChanged = (sender, e) => { };

    //    public RelayCommandWithReturn(Func<object> action)
    //    {
    //        mAction = action;
    //    }

    //    public bool CanExecute(object parameter)
    //    {
    //        // A relay command can always execute
    //        return true;
    //    }

    //    public object Execute(object parameter)
    //    {
    //        mAction((T)parameter);
    //    }
    //}
}
