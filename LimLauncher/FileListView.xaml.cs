using LimLauncher.Entities;
using LimLauncher.Modules;
using ModernMessageBoxLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public partial class FileListView : UserControl
    {
        public ObservableCollection<ShortcutInfo> Files { get; set; }

        public GroupInfo GroupInfo { get; set; }

        public FileListView()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        #region 事件
        /// <summary>
        /// 双击打开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string strFileName = (((ListBoxItem)sender).Content as ShortcutInfo).FileFullPath;
            StartFile(strFileName);
        }

        /// <summary>
        /// 切换大图标模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MIBigIcon_Click(object sender, RoutedEventArgs e)
        {
            lbFiles.Style = (Style)this.FindResource("ListBoxBigIcon");
        }

        /// <summary>
        /// 切换列表模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MIStretching_Click(object sender, RoutedEventArgs e)
        {
            lbFiles.Style = (Style)this.FindResource("ListBoxStretching");
        }

        /// <summary>
        /// 文件拖拽
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [Obsolete]
        private void LbFiles_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if (GroupInfo != null)
                {
                    foreach (string fileNameBase in ((System.Array)e.Data.GetData(DataFormats.FileDrop)))
                    {
                        string filePath = fileNameBase;
                        string fileReName = null;
                        if (System.IO.Path.GetExtension(filePath) == ".lnk")
                        {
                            IShellLink vShellLink = (IShellLink)new ShellLink();
                            UCOMIPersistFile vPersistFile = vShellLink as UCOMIPersistFile;
                            vPersistFile.Load(filePath, 0);
                            StringBuilder vStringBuilder = new StringBuilder(260);
                            WIN32_FIND_DATA vWIN32_FIND_DATA;
                            vShellLink.GetPath(vStringBuilder, vStringBuilder.Capacity, out vWIN32_FIND_DATA, SLGP_FLAGS.SLGP_RAWPATH);
                            filePath = vStringBuilder.ToString();
                            fileReName = System.IO.Path.GetFileNameWithoutExtension(fileNameBase);
                        }
                        if (Files.Where(t => t.FileFullPath == filePath).Count() == 0)
                            Files.Add(new ShortcutInfo()
                            {
                                FileFullPath = filePath,
                                FileRename = fileReName,
                            });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowErrorMessage(ex);
            }

        }

        /// <summary>
        /// 拖拽完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LbFiles_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                e.Effects = DragDropEffects.None;
                //e.Effects = DragDropEffects.None;
                if (e.Data.GetDataPresent(DataFormats.FileDrop) && GroupInfo != null)
                    e.Effects = DragDropEffects.Link;
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowErrorMessage(ex);
            }
            e.Handled = true;
        }

        /// <summary>
        /// 快捷键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (((ListBox)sender).SelectedItem != null)
                {
                    if (e.Key == Key.Delete)
                    {
                        Files.Remove(((ListBox)sender).SelectedValue as ShortcutInfo);
                    }
                    else if (e.Key == Key.F2)
                    {
                        Rename(((ListBox)sender).SelectedValue as ShortcutInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MIDelFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<ShortcutInfo> toBeDelete = new List<ShortcutInfo>();
                foreach (ShortcutInfo File in lbFiles.SelectedItems)
                {
                    toBeDelete.Add(File);
                }

                toBeDelete.ForEach(File => Files.Remove(File));

            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// 打开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MIOpenFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (ShortcutInfo File in lbFiles.SelectedItems)
                {
                    StartFile(File.FileFullPath);
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowErrorMessage(ex);
            }
        }
        private void MIRename_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Rename((sender as MenuItem).DataContext as ShortcutInfo);
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowErrorMessage(ex);
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 重置文件列表
        /// </summary>
        /// <param name="group"></param>
        public void SetFileList(GroupInfo group)
        {
            this.GroupInfo = group;
            this.Files = group?.ListShortcutInfo;
            this.DataContext = this;
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="FileName"></param>
        private void StartFile(string FileName)
        {
            new System.Threading.Thread(() =>
            {
                try
                {
                    System.Diagnostics.Process.Start(FileName);
                }
                catch { }
            }).Start();
        }

        private void Rename(ShortcutInfo shortcutInfo)
        {
            var msg = new ModernMessageBox("请设置一个用于显示的别名", "设置显示名", ModernMessageboxIcons.Question, "确定", "取消")
            {
                TextBoxText = shortcutInfo.FileRenameDisp,
                TextBoxVisibility = Visibility.Visible,
            };
            msg.Owner = MessageBoxHelper.GlobalParentWindow;
            msg.ShowDialog();
            if (msg.Result == ModernMessageboxResult.Button1)
                shortcutInfo.FileRename = msg.TextBoxText;
        }
        #endregion

        private void MIOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (ShortcutInfo File in lbFiles.SelectedItems)
                {
                    Process p = new Process();
                    p.StartInfo.FileName = "Explorer.exe";
                    p.StartInfo.Arguments = "/select," + File.FileFullPath;
                    p.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowErrorMessage(ex);
            }
        }

        private void MIAdminRun_Click(object sender, RoutedEventArgs e)
        {
            foreach (ShortcutInfo File in lbFiles.SelectedItems)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.Verb = "runas";
                startInfo.UseShellExecute = true;
                //设置启动动作,确保以管理员身份运行
                startInfo.FileName = File.FileFullPath;
                try
                {
                    System.Diagnostics.Process.Start(startInfo);
                }
                catch
                {
                    return;
                }
            }

        }

        private void ContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (lbFiles.SelectedItem != null)
            {
                (((FrameworkElement)sender).ContextMenu.Items.GetItemAt(5) as MenuItem).Visibility =
                    (lbFiles.SelectedItem as ShortcutInfo).FileFullPath.ToLower().EndsWith(".exe") ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
