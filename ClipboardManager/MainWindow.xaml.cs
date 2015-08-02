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
        /// <summary>
        /// Backing viewmodel
        /// </summary>
        public viewmodel.Main ViewModel = new viewmodel.Main();
        /// <summary>
        /// Clipboard assistant object, contains clipboard events
        /// </summary>
        ClipboardAssist.ClipboardMonitor Monitor = new ClipboardAssist.ClipboardMonitor();
        /// <summary>
        /// Icon object for taskbar
        /// </summary>
        NotifyIcon icon = new NotifyIcon();
        /// <summary>
        /// Copy menu items for recent on contextmenu for NotifyIcon
        /// </summary>
        System.Windows.Forms.MenuItem copyMenuItems = new System.Windows.Forms.MenuItem("Recent");
        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = ViewModel;
            //create basic context menu for taskbar including the open method for app
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
            //add additional context menu items for NotifyIcon
            icon.ContextMenu.MenuItems.Add("-");
            icon.ContextMenu.MenuItems.Add(copyMenuItems);
            icon.ContextMenu.MenuItems.Add("-");
            icon.ContextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("Exit", new EventHandler(delegate(object o, EventArgs e)
            {
                System.Environment.Exit(0);
                icon.Visible = false;
                icon = null;
            })));
            icon.Icon = ClipboardManager.Properties.Resources.clipboard;
            icon.Visible = true;
            //handler for when the clipboard change, here we keep track of new items tossed in the clipboard.
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
            //instead of closing the app on close move it to the taskbar and just hide
            this.Closing += new System.ComponentModel.CancelEventHandler(delegate(object o, System.ComponentModel.CancelEventArgs e)
            {
                this.Hide();
                icon.ShowBalloonTip(2000, "Clipboard Manager Minimized.", "Clipboard Manager has been minimized to your system tray.", ToolTipIcon.Info);
                e.Cancel = true;
            });
        }
        /// <summary>
        /// when the settings button is clicked open up the flyout
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void FlyoutOpen_Click(object sender, RoutedEventArgs e)
        {
            this.Flyout.IsOpen = true;
        }
        /// <summary>
        /// when clear history button is clicked go ahead and....do that
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void ClearHistoryBtn_Click(object sender, RoutedEventArgs e)
        {
            //reinstantiate the clipboard history in viewmodel to clear it
            this.ViewModel.ClipboardHistory.Clear();
        }
        /// <summary>
        /// save settings on click
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
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
