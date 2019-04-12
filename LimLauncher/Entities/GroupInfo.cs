using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LimLauncher.Entities
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class GroupInfo
    {
        public long ID { get; set; }
        public string GroupName { get; set; }

        public GroupInfo()
        {
            ID = DateTime.Now.Ticks;
        }

        public ObservableCollection<ShortcutInfo> ListShortcutInfo { get; set; } = new ObservableCollection<ShortcutInfo>();
    }
}
