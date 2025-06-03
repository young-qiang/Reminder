using HandyControl.Controls;
using HandyControl.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Windows.Threading;
using System.Diagnostics;

namespace Reminder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private DispatcherTimer timer;
        public static DateTime nextReminder { get; set; }
        public static  bool isRunning { get; set; } = false;
        private const string RegistryKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string ApplicationName = "Reminder";

        public MainWindow()
        {
            InitializeComponent();
            CheckAutoStartStatus();
            if (!isRunning)
            {
                Button_Click(null, null);
            }
        }

        private void CheckAutoStartStatus()
        {
            if (AutoStartHelper.IsExistKey(Process.GetCurrentProcess().MainModule.ModuleName))
            { 
                AutoStartCheckBox.IsChecked = true;
            }
            //using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath))
            //{
            //    if (key != null)
            //    {
            //        string value = (string)key.GetValue(ApplicationName);
            //        bool isAutoStartEnabled = !string.IsNullOrEmpty(value);
            //        AutoStartCheckBox.IsChecked = isAutoStartEnabled;
            //    }
            //}
        }

        private void AutoStartCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SetAutoStart(true);
        }

        private void AutoStartCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SetAutoStart(false);
        }
        private void SetAutoStart(bool enable)
        {
            if (AutoStartHelper.SetMeStart(enable))
            {
                System.Windows.MessageBox.Show("设置开机自启动成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                System.Windows.MessageBox.Show("设置开机自启动失败", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                AutoStartCheckBox.IsChecked = !enable;
            }

            //try
            //{
            //    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, true))
            //    {
            //        if (key != null)
            //        {
            //            if (enable)
            //            {
            //                string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            //                key.SetValue(ApplicationName, exePath);
            //            }
            //            else
            //            {
            //                key.DeleteValue(ApplicationName, false);
            //            }

            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    System.Windows.MessageBox.Show($"设置开机自启动失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            //    AutoStartCheckBox.IsChecked = !enable;
            //}
        }


        private void NotifyIcon_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
            this.Activate();
            this.Topmost = true;
            this.Topmost = false;
            this.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StartReminder();
            if (ShowCountShutdown.IsChecked.Value) 
                clock.Show();

            isRunning = !isRunning;
            if (isRunning)
            {
                btn.Content = "停止";
            }
            else
            {
                btn.Content = "开始";
            }
        }

        private void StartReminder()
        {
            nextReminder = DateTime.Now.AddMinutes(xxjg.Value);

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private bool isNotifySaveProgress = false;
        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeSpan remainingTime = nextReminder - DateTime.Now;

            if (remainingTime <= TimeSpan.Zero)
            {
                ShowReminderMessageBox();
                nextReminder = DateTime.Now.AddMinutes(xxsc.Value + xxjg.Value);
            }
            else if (remainingTime <= TimeSpan.FromSeconds(xxqtx.Value) && isNotifySaveProgress == false)
            {
                ShowSaveProgressMessageBox();
            }
        }

        private void ShowReminderMessageBox()
        {

            Notification.Show(new AppNotification("休息一下"), ShowAnimation.Fade);
            isNotifySaveProgress = false;

        }

        private void ShowSaveProgressMessageBox()
        {

            isNotifySaveProgress = true;
            Notification.Show(new AppNotification($@"{xxqtx.Value}S"), ShowAnimation.Fade);
        }

        private bool isClose = false;
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isClose == false)
            {
                e.Cancel = true;
                this.Hide();
            }
            else
            {
                timer.Stop();
                timer = null;
                Application.Current.Shutdown();
            }
        }


        Clock clock = new Clock();
        private void ShowCountShutdown_Checked(object sender, RoutedEventArgs e)
        { 

            clock.Show();
        }

        private void ShowCountShutdown_Unchecked(object sender, RoutedEventArgs e)
        {
            clock.Hide();
        }
    }
}