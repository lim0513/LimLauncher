using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LimLauncher.Modules
{
    public static class Common
    {
        public static TContainer GetContainerFromIndex<TContainer>(this ItemsControl itemsControl, int index) where TContainer : DependencyObject
        {
            return (TContainer)itemsControl.ItemContainerGenerator.ContainerFromIndex(index);
        }

        public static bool IsEditing(this DataGrid dataGrid)
        {
            return dataGrid.GetEditingRow() != null;
        }

        public static DataGridRow GetEditingRow(this DataGrid dataGrid)
        {
            var sIndex = dataGrid.SelectedIndex;
            if (sIndex >= 0)
            {
                var selected = dataGrid.GetContainerFromIndex<DataGridRow>(sIndex);
                if (selected.IsEditing) return selected;
            }

            for (int i = 0; i < dataGrid.Items.Count; i++)
            {
                if (i == sIndex) continue;
                var item = dataGrid.GetContainerFromIndex<DataGridRow>(i);
                if (item.IsEditing) return item;
            }

            return null;
        }
    }
}
