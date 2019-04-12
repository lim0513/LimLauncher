using LimLauncher.Entities;
using LimLauncher.Modules;
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
        private void LbFiles_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if (GroupInfo != null)
                {
                    foreach (string fileName in ((System.Array)e.Data.GetData(DataFormats.FileDrop)))
                    {
                        Files.Add(new ShortcutInfo() { FileFullPath = fileName });
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
                if (e.Key == Key.Delete && ((ListBox)sender).SelectedItem != null)
                {
                    Files.Remove(((ListBox)sender).SelectedValue as ShortcutInfo);
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
                Files.Remove((sender as MenuItem).DataContext as ShortcutInfo);
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
            string strFileName = ((sender as MenuItem).DataContext as ShortcutInfo).FileFullPath;
            StartFile(strFileName);
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
        #endregion
    }
}
