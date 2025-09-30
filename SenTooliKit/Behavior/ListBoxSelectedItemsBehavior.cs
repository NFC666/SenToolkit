using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace SenTooliKit.Behavior
{
    public static class ListBoxSelectedItemsBehavior
    {
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.RegisterAttached(
                "SelectedItems",
                typeof(IList),
                typeof(ListBoxSelectedItemsBehavior),
                new PropertyMetadata(null, OnSelectedItemsChanged));

        public static void SetSelectedItems(DependencyObject element, IList value)
            => element.SetValue(SelectedItemsProperty, value);

        public static IList GetSelectedItems(DependencyObject element)
            => (IList)element.GetValue(SelectedItemsProperty);

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListBox listBox)
            {
                listBox.SelectionChanged -= ListBox_SelectionChanged;
                listBox.SelectionChanged += ListBox_SelectionChanged;

                // 初始化已选项
                if (e.NewValue is IList list)
                {
                    listBox.SelectedItems.Clear();
                    foreach (var item in list)
                        listBox.SelectedItems.Add(item);
                }
            }
        }

        private static void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox)
            {
                var selectedItems = GetSelectedItems(listBox);
                if (selectedItems == null) return;

                selectedItems.Clear();
                foreach (var item in listBox.SelectedItems)
                    selectedItems.Add(item);
            }
        }
    }
}