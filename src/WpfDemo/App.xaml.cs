using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace WpfDemo
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public bool DoHandle { get; set; }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //var wpfExceptionHelper = new WpfExceptionHelper();
            //wpfExceptionHelper.SetupExceptionHandling(true);
            SetupExceptionHandling();
        }

        private void SetupExceptionHandling()
        {
            #region desc

            //You can trap unhandled exceptions at different levels:

            //#1 AppDomain.CurrentDomain.UnhandledException From all threads in the AppDomain.
            //#2 Dispatcher.UnhandledException From a single specific UI dispatcher thread.
            //#3 Application.Current.DispatcherUnhandledException From the main UI dispatcher thread in your WPF application.
            //#4 TaskScheduler.UnobservedTaskException from within each AppDomain that uses a task scheduler for asynchronous operations.

            //You should consider what level you need to trap unhandled exceptions at.
            //Deciding between #2 and #3 depends upon whether you're using more than one WPF thread. This is quite an exotic situation and 

            #endregion
            //if you're unsure whether you are or not, then it's most likely that you're not.

            //UI Ex
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            //Custom Thread Ex
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            //task scheduler
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                MessageBox.Show(ex.Message, "AppDomain.CurrentDomain.UnhandledException", MessageBoxButton.OK, MessageBoxImage.Error);
                if (e.IsTerminating)
                {
                    MessageBox.Show("即将终止");
                }
                LogUnhandledException(ex, "AppDomain.CurrentDomain.UnhandledException");
            }
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (this.DoHandle)
            {
                //Handling the exception within the UnhandledExcpeiton handler.
                MessageBox.Show(e.Exception.Message, "AppDomain.CurrentDomain.UnhandledException", MessageBoxButton.OK, MessageBoxImage.Error);
                e.Handled = true;
                LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
            }
            else
            {
                //If you do not set e.Handled to true, the application will close due to crash.
                MessageBox.Show("Application is going to close! ", "AppDomain.CurrentDomain.UnhandledException");
                e.Handled = false;
                LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
            }
        }

        void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            if (e.Exception != null)
            {
                //MessageBox.Show(ex.Message, "TaskScheduler.UnobservedTaskException", MessageBoxButton.OK, MessageBoxImage.Error);
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
