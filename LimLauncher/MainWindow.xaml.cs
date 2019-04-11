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
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public ObservableCollection<GroupInfo> Groups { get; set; } = new ObservableCollection<GroupInfo>() { new GroupInfo() { GroupName = "Apps" }, new GroupInfo() { GroupName = "Documents" }, new GroupInfo() { GroupName = "New Group" }, new GroupInfo() { GroupName = "New Group" }, };

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            MessageBoxHelper.GlobalParentWindow = this;
        }

        private void BtnAddGroup_Click(object sender, RoutedEventArgs e)
        {
            Groups.Add(new GroupInfo() { GroupName = "新分组" });
            scv.ScrollToEnd();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

        }

        private void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {

        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Delete && ((DataGrid)sender).SelectedItem != null && !((DataGrid)sender).IsEditing())
            {
                if (MessageBoxHelper.ShowYesNoMessage("确定要永久性的删除此分组吗？", "删除确认") != ModernMessageBoxLib.ModernMessageboxResult.Button1)
                {
                    e.Handled = true;
                }
            }
        }

        private void Test()
        { }
    }
}
