using LimLauncher.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace LimLauncher
{
    /// <summary>
    /// FileListView.xaml 的交互逻辑
    /// </summary>
    public partial class FileListView : UserControl
    {
        public ObservableCollection<ShortcutInfo> Files { get; set; }
        public FileListView()
        {
            InitializeComponent();
            SetFileList(CreateTestData());
        }

        public ObservableCollection<ShortcutInfo> CreateTestData()
        {
            ObservableCollection<ShortcutInfo> tmpFiles = new ObservableCollection<ShortcutInfo>();
            foreach (string strFile in System.IO.Directory.GetFiles(@"C:\Windows"))
            {
                tmpFiles.Add(new ShortcutInfo() { FileFullPath = strFile });
            }
            return tmpFiles;
        }

        public void SetFileList(ObservableCollection<ShortcutInfo> Files)
        {
            this.Files = Files;
            this.DataContext = this;
        }
    }
}
