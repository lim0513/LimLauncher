using LimLauncher.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LimLauncher.Entities
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class ShortcutInfo
    {
        /// <summary>
        /// 文件完整路径
        /// </summary>
        public string FileFullPath { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get { return System.IO.Path.GetFileName(FileFullPath); } }

        /// <summary>
        /// 文件图标
        /// </summary>
        public ImageSource FileIcon { get { return IconManager.FindIconForFilename(FileFullPath, true); } }
    }
}
