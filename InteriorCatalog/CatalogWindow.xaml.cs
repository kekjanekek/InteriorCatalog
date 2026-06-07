using Model.Core;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace InteriorCatalog
{
    /// <summary>
    /// Логика взаимодействия для CatalogWindow.xaml
    /// </summary>
    public partial class CatalogWindow : Window
    {
        private FurnitureCatalog _catalog;
        private Furniture[] _currentView;
        private string _currentFilter = "All";
        private Action<string> _logger;
        private Furniture _lastRemoved;
        public CatalogWindow(FurnitureCatalog catalog)
        {
            InitializeComponent();
            _catalog = catalog;
            _currentView = _catalog.Items;
            FurnitureGrid.ItemsSource = _currentView;
          
            TypeFilterComboBox.SelectedIndex = 0;
            UpdateTotalPrice();
            _logger = message =>
            {
                MessageBox.Show(message);
            };
        }
        public CatalogWindow()
        {
            InitializeComponent();
        }
        private void UpdateTotalPrice()
        {
            decimal total = 0;
            foreach (var item in _currentView)
            {
                if (item is Furniture f)
                {
                    total += f; //из за оператора
                }
            }
            TotalPriceText.Text = $"Итого: {total} ₽";
        }
        // ---кнопка фильтра---
        private T[] Filter<T>(T[] source, Func<T, bool> predicate)
        {
            T[] temp = new T[source.Length];
            int count = 0;
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] != null && predicate(source[i]))
                {
                    temp[count] = source[i];
                    count++;
                }
            }
            T[] result = new T[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = temp[i];
            }
            return result;
        }
        private void TypeFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selecteditem = null;
            foreach (var obj in TypeFilterComboBox.Items)
            {
                ComboBoxItem item = obj as ComboBoxItem;
                if (item != null && item.IsSelected)
                {
                    selecteditem = item;
                    break;
                }
            }
            if (selecteditem == null)
            {
                return;
            }
            string selectedType = selecteditem.Content.ToString();
            _currentFilter = selectedType;
            RefreshView();
        }
        private void RefreshView()
        {
            Furniture[] data = _catalog.Items;
            if (_currentFilter != "All")
            {
                data = Filter(data, x=>  x.GetType().Name == _currentFilter);
            }
            _currentView = data;
            FurnitureGrid.ItemsSource = null;
            FurnitureGrid.ItemsSource = _currentView;
            UpdateTotalPrice();
        }
        // ----
        private void SortArticleAsc_Click(object sender, RoutedEventArgs e) //по артикулу (возр)
        {
            _catalog.Sort(true);

            _currentView = _catalog.Items;
            RefreshView();
        }
        private void SortArticleDesc_Click(object sender, RoutedEventArgs e) //по артикулу (убыв)
        {
            _catalog.Sort(false);

            _currentView = _catalog.Items;
            RefreshView();
        }
        private void SortNameAsc_Click(object sender, RoutedEventArgs e)
        {
            _catalog.SortByName(true);

            _currentView = _catalog.Items;
            RefreshView();
        }
        private void SortNameDesc_Click(object sender, RoutedEventArgs e)
        {
            _catalog.SortByName(false);

            _currentView = _catalog.Items;
            RefreshView();
        }
        private void SortPriceAsc_Click(object sender, RoutedEventArgs e)
        {
            _catalog.SortByPrice(true);

            _currentView = _catalog.Items;
            RefreshView();
            _logger("Сортировка по возрастанию цены выполнена");
        }
        private void SortPriceDesc_Click(object sender, RoutedEventArgs e)
        {
            _catalog.SortByPrice(false);

            _currentView = _catalog.Items;
            RefreshView();
            _logger("Сортировка по убыванию цены выполнена");
        }
        private void PrioritySort_Click(object sender, RoutedEventArgs e)
        {
            _catalog.PrioritySort();

            _currentView = _catalog.Items;
            RefreshView();
        }
        // ----
        private void OpenImage_Click(object sender, RoutedEventArgs e)
        {
            Furniture selectedFurniture = FurnitureGrid.SelectedItem as Furniture;
            if (selectedFurniture == null)
            {
                MessageBox.Show("Выберите предмет мебели");
                return;
            }
            FurnitureImageWindow window = new FurnitureImageWindow(selectedFurniture);
            window.Show();
        }
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Furniture selected = FurnitureGrid.SelectedItem as Furniture;
            if (selected == null)
            {
                MessageBox.Show("Выберите предмет для удаления");
                return;
            }
            _lastRemoved = selected;
            _catalog.Remove(selected.Article);

            RefreshView();
            UndoButton.IsEnabled = true;
        }
        private void UndoRemove_Click(object sender, RoutedEventArgs e)
        {
            if (_lastRemoved == null)
            {
                MessageBox.Show("Нечего возвращать");
                return;
            }

            _catalog.Add(_lastRemoved);
            _lastRemoved = null;

            RefreshView();

            UndoButton.IsEnabled = false;
        }
    }
}
