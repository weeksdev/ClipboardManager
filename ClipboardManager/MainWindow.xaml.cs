using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using System.Windows.Forms;

namespace ClipboardManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public viewmodel.Main ViewModel = new viewmodel.Main();
        ClipboardAssist.ClipboardMonitor Monitor = new ClipboardAssist.ClipboardMonitor();
        NotifyIcon icon = new NotifyIcon();
        System.Windows.Forms.MenuItem copyMenuItems = new System.Windows.Forms.MenuItem("Recent");
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = ViewModel;
            
            icon.ContextMenu = new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[]{
                new System.Windows.Forms.MenuItem("Open", new EventHandler(delegate(object o, EventArgs e){
                    this.Show();
                    this.WindowState = System.Windows.WindowState.Normal;
                    this.Activate();
                    this.Topmost = true;  // important
                    this.Topmost = false; // important
                    this.Focus();         // important
                }))
            });
            icon.ContextMenu.MenuItems.Add("-");
            icon.ContextMenu.MenuItems.Add(copyMenuItems);
            icon.Icon = new System.Drawing.Icon(@"C:\Users\WeeksDev\Downloads\Note29.ico");
            icon.Visible = true;
            Monitor.ClipboardChanged += new EventHandler<ClipboardAssist.ClipboardChangedEventArgs>(delegate(object sender, ClipboardAssist.ClipboardChangedEventArgs e)
            {
                var item = ViewModel.AddToClipboardHistory(e.DataObject.GetData(typeof(string)));
                if (item != null)
                {
                    this.ClipboardGrid.ScrollIntoView(item);
                }
                copyMenuItems.MenuItems.Clear();
                var l = Extensions.TakeLast(this.ViewModel.ClipboardHistory, 5);
                l.ToList().ForEach(a =>
                {
                    var newMenuItem = new System.Windows.Forms.MenuItem();
                    newMenuItem.Click += new EventHandler(delegate(object o, EventArgs args)
                    {
                        System.Windows.Clipboard.SetText(a.Content);
                    });
                    if (a.Content.Length > 50)
                    {
                        newMenuItem.Text = a.Content.Substring(0, 50) + "...";
                    }
                    else
                    {
                        newMenuItem.Text = a.Content;
                    }
                    copyMenuItems.MenuItems.Add(newMenuItem);
                });
            });
            this.StateChanged += new EventHandler(delegate(object o, EventArgs e)
            {
                if (this.WindowState == System.Windows.WindowState.Minimized)
                {
                    this.Hide();
                    icon.ShowBalloonTip(2000, "Clipboard Manager Minimized.", "Clipboard Manager has been minimized to your system tray.", ToolTipIcon.Info);
                }
                else
                {
                    this.Show();
                    this.Activate();
                }
            });
            this.Closing += new System.ComponentModel.CancelEventHandler(delegate(object o, System.ComponentModel.CancelEventArgs e)
            {
                icon.Visible = false;
                icon = null;
            });
        }


        private void FlyoutOpen_Click(object sender, RoutedEventArgs e)
        {
            this.Flyout.IsOpen = true;
        }

        private void ClearHistoryBtn_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.ClipboardHistory = new System.Collections.ObjectModel.ObservableCollection<viewmodel.Main.ClipboardItem>();
        }

        private void SaveSettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ViewModel.SaveSettings();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Sorry, an error occured saving settings");
            }
        }
    }
}
