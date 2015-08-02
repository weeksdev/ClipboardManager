using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Configuration;
using ClipboardManager;

namespace ClipboardManager.viewmodel
{
    public class Main: Base
    {
        public class ClipboardItem: Base
        {
            private string _Content;

            public string Content
            {
                get { 
                    return _Content; 
                }
                set { 
                    _Content = value;
                    OnPropertyChange();
                }
            }
            
        }
        private ObservableCollection<ClipboardItem> _ClipboardHistory = new ObservableCollection<ClipboardItem>();

        public ObservableCollection<ClipboardItem> ClipboardHistory
        {
            get { return _ClipboardHistory; }
            set { 
                _ClipboardHistory = value;
                OnPropertyChange();
            }
        }
        public ClipboardItem AddToClipboardHistory(object value)
        {
            if (value != null)
            {
                var content = (string)value;
                if (content != "")
                {
                    //iterate through history seeing if we already have this value if so remove it
                    var removeIndexes = new List<int>();
                    for (var i = 0; i < ClipboardHistory.Count; i++)
                    {
                        if (ClipboardHistory[i].Content == content)
                        {
                            removeIndexes.Add(i);
                        }
                    }
                    removeIndexes.ForEach(a => ClipboardHistory.RemoveAt(a));
                    //add the new content
                    var newClipboardItem = new ClipboardItem() { Content = content };
                    ClipboardHistory.Add(newClipboardItem);
                    UpdateHistoryLength();
                    return newClipboardItem;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        private int _MaxHistory = int.Parse(ConfigurationManager.AppSettings["maxHistory"]);
        public int MaxHistory
        {
            get {
                return _MaxHistory;
            }
            set {
                _MaxHistory = value;
                OnPropertyChange();
            }
        }
        public void SaveSettings()
        {
            ConfigurationManager.AppSettings.Set("maxHistory", MaxHistory.ToString());
            UpdateSetting("maxHistory", MaxHistory.ToString());   
        }
        public void UpdateHistoryLength()
        {
            //if we're over the max history trim us down
            var count = ClipboardHistory.Count;
            if (count > MaxHistory)
            {
                //http://stackoverflow.com/questions/3453274/using-linq-to-get-the-last-n-elements-of-a-collection
                //http://stackoverflow.com/questions/3559821/how-to-convert-ienumerable-to-observablecollection
                ClipboardHistory = new ObservableCollection<ClipboardItem>(ClipboardHistory.Skip(Math.Max(0, ClipboardHistory.Count() - MaxHistory)));
            }
        }
        private void UpdateSetting(string key, string value)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings[key].Value = value;
            configuration.Save();
            ConfigurationManager.RefreshSection("appSettings");
            UpdateHistoryLength();
        }
    }
}
