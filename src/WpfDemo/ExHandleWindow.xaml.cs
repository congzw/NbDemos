using System;
using System.Windows;
using System.Threading;

namespace WpfDemo
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ExHandleWindow : Window
    {
        internal bool ThrowThreadException { get; set; }
        internal Thread CurrentThread { get; set; }
        public ExHandleWindow()
        {
            InitializeComponent();
        }

        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.chkIsThread.IsChecked.HasValue && this.chkIsThread.IsChecked.Value)
                {
                    this.ThrowThreadException = true;
                    return;
                }
                throw new ApplicationException("Shit Happens!");
            }
            catch(Exception ex)
            {
                if (rbtnCatch.IsChecked.HasValue && rbtnCatch.IsChecked.Value)
                    MessageBox.Show(ex.Message, "MainWindow Handle", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    App app = App.Current as App;
                    app.DoHandle = rbtnApplication.IsChecked.HasValue && rbtnApplication.IsChecked.Value;
                    throw ex;
                }
            }
        }

        private void chkIsThread_Checked(object sender, RoutedEventArgs e)
        {
            this.CurrentThread = this.CurrentThread ?? new Thread(new ThreadStart(this.RunMe));
            if(this.CurrentThread.ThreadState == ThreadState.Unstarted)
                this.CurrentThread.Start();
        }

        private void RunMe()
        {
            do
            {
                lock (this)
                {
                    if (this.ThrowThreadException)
                    {
                        this.ThrowThreadException = false;
                        throw new ApplicationException(string.Format("Shit Happens!(Thread:{0})", Thread.CurrentThread.ManagedThreadId));
                    }
                }
            } while (true);
            
        }
    }
}
