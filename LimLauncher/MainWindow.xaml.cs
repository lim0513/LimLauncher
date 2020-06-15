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
using System.Windows.Controls.Primitives;
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
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        private string JsonFile => System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "filelist.json");
        public ObservableCollection<GroupInfo> Groups { get; set; }
        private string OldCellValue;

        public MainWindow()
        {
            InitializeComponent();
            if (System.IO.File.Exists(JsonFile))
            {
                Groups = Newtonsoft.Json.JsonConvert.DeserializeObject<ObservableCollection<GroupInfo>>(System.IO.File.ReadAllText(JsonFile));
            }
            else
            {
                Groups = new ObservableCollection<GroupInfo>();
                AddNewGroup("默认分组");
            }
            this.Height = Properties.Settings.Default.LastHeight;
            this.Width = Properties.Settings.Default.LastWidth;
            this.Top = Properties.Settings.Default.LastTop;
            this.Left = Properties.Settings.Default.LastLeft;
            tog_Topmost.IsChecked = Properties.Settings.Default.TopMost;

            this.DataContext = this;
        }

        #region 事件
        /// <summary>
        /// 窗体加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Groups.Count > 0)
            {
                dg_Groups.SelectedIndex = 0;
            }
            MessageBoxHelper.GlobalParentWindow = this;
        }

        /// <summary>
        /// 添加分组
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAddGroup_Click(object sender, RoutedEventArgs e)
        {
            AddNewGroup();
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        /// <summary>
        /// 滚轮穿透
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        /// <summary>
        /// 分组名编辑完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (string.IsNullOrEmpty(((TextBox)e.EditingElement).Text))
            {
                ((TextBox)e.EditingElement).Text = OldCellValue;
            }
        }

        /// <summary>
        /// 分组名删除快捷键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Delete && ((DataGrid)sender).SelectedItem != null && !((DataGrid)sender).IsEditing())
            {
                if (MessageBoxHelper.ShowYesNoMessage("确定要永久性的删除此分组吗？", "删除确认") != ModernMessageBoxLib.ModernMessageboxResult.Button1)
                {
                    e.Handled = true;
                }
                else
                {
                    if (Groups.Count == 1) FileView.SetFileList(AddNewGroup("默认分组"));
                }

            }
        }

        /// <summary>
        /// 更改选中分组
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                FileView.SetFileList((GroupInfo)e.AddedItems[0]);
            }
        }

        /// <summary>
        /// 分组名预存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            OldCellValue = ((TextBox)e.EditingElement).Text;
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MyNotifyIcon.Dispose();
            SaveSettings();
            Save();
        }



        private void MIDel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxHelper.ShowYesNoMessage("确定要永久性的删除此分组吗？", "删除确认") == ModernMessageBoxLib.ModernMessageboxResult.Button1)
            {
                Groups.Remove(((MenuItem)sender).DataContext as GroupInfo);
                if (Groups.Count == 0) FileView.SetFileList(AddNewGroup("默认分组"));
            }

        }
        #endregion

        #region 方法

        /// <summary>
        /// 添加分组
        /// </summary>
        /// <param name="defualtGroupName"></param>
        /// <returns></returns>
        private GroupInfo AddNewGroup(string defualtGroupName = "新分组")
        {
            GroupInfo GroupNew = new GroupInfo() { GroupName = defualtGroupName };
            Groups.Add(GroupNew);
            dg_Groups.SelectedIndex = dg_Groups.Items.Count - 1;
            scv.ScrollToEnd();
            return GroupNew;
        }

        /// <summary>
        /// 保存分组
        /// </summary>
        private void Save()
        {
            string strJson = Newtonsoft.Json.JsonConvert.SerializeObject(Groups);
            System.IO.File.WriteAllText(JsonFile, strJson);
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        private void SaveSettings()
        {
            Properties.Settings.Default.LastHeight = this.Height;
            Properties.Settings.Default.LastWidth = this.Width;
            Properties.Settings.Default.TopMost = this.Topmost;
            Properties.Settings.Default.LastTop = this.Top;
            Properties.Settings.Default.LastLeft = this.Left;
            Properties.Settings.Default.Save();
        }

        #endregion

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            (((FrameworkElement)sender).DataContext as ShortcutInfo).StartFile();
        }

        private void MyNotifyIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.Activate();
            }
        }

        private void MetroWindow_StateChanged(object sender, EventArgs e)
        {
            this.ShowInTaskbar = this.WindowState != WindowState.Minimized;
        }
    }
}
