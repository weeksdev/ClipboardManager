using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

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
        public void AddToClipboardHistory(object value)
        {
            if (value != null)
            {
                var content = (string)value;
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
                ClipboardHistory.Add(new ClipboardItem() { Content = content });
            }
        }
    }
}
