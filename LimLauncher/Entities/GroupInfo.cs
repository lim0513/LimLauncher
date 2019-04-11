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
        public string GroupName { get; set; }

        public ObservableCollection<ShortcutInfo> ListShortcutInfo { get; set; }
    }
}
