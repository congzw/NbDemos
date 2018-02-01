using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace WpfDemo
{
    public class WpfExceptionHelper
    {
        public bool DoHandle { get; set; }

        public void SetupExceptionHandling(bool handleEx)
        {
            #region desc

            //https://stackoverflow.com/questions/1472498/wpf-global-exception-handler/1472562#1472562

            //You can trap unhandled exceptions at different levels:

            //#1 AppDomain.CurrentDomain.UnhandledException From all threads in the AppDomain.
            //#2 Dispatcher.UnhandledException From a single specific UI dispatcher thread.
            //#3 Application.Current.DispatcherUnhandledException From the main UI dispatcher thread in your WPF application.
            //#4 TaskScheduler.UnobservedTaskException from within each AppDomain that uses a task scheduler for asynchronous operations.

            //You should consider what level you need to trap unhandled exceptions at.
            //Deciding between #2 and #3 depends upon whether you're using more than one WPF thread. This is quite an exotic situation and 
            //if you're unsure whether you are or not, then it's most likely that you're not.

            #endregion
            
            Application.Current.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            DoHandle = handleEx;
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                MessageBox.Show(ex.Message, "Uncaught Thread Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                LogUnhandledException(ex, "AppDomain.CurrentDomain.UnhandledException");
            }
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (this.DoHandle)
            {
                //Handling the exception within the UnhandledExcpeiton handler.
                MessageBox.Show(e.Exception.Message, "Exception Caught", MessageBoxButton.OK, MessageBoxImage.Error);
                e.Handled = true;
                LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
            }
            else
            {
                //If you do not set e.Handled to true, the application will close due to crash.
                MessageBox.Show("Application is going to close! ", "Uncaught Exception");
                e.Handled = false;
                LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
            }
        }

        void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            if (e.Exception != null)
            {
                //MessageBox.Show(ex.Message, "Uncaught Thread Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
            }
        }

        private void LogUnhandledException(Exception exception, string source)
        {
            //todo log ex
            //var myLogHelper = MyLogHelper.Resolve();
            //myLogHelper.LogEx(exception, source);
        }
    }
}
