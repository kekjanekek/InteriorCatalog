using Model;
using Model.Core;
using Model.Core.Interface;
using Model.Data;
using System.IO;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace InteriorCatalog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FurnitureCatalog[] _catalogs;
        private AbstractSerializer _serializer;
        private string _extension = "json"; //по умолчанию
        private bool _isFormatSelected = false;
        public MainWindow()
        {
            InitializeComponent();
            _serializer = null;
            _extension = null;
            LoadCatalogs();
            //по умолчанию
            CatalogComboBox.ItemsSource = _catalogs;
            CatalogComboBox.DisplayMemberPath = "Name";
        }
        private void DemonstrateInterfaces() //приведение к интерфейсу
        {
            IFurnitureCatalog catalog = _catalogs[0];
            catalog.Add(new Chair());
            ISortable sortable = _catalogs[0];
            sortable.Sort(true);

            IFurnitureCatalog catalog2 = _catalogs[1];
            catalog2.Add(new Sofa());
            ISortable sortable2 = _catalogs[1];
            sortable2.Sort(true);

            IFurnitureCatalog catalog3 = _catalogs[2];
            catalog3.Add(new Bed());
            ISortable sortable3 = _catalogs[2];
            sortable3.Sort(true);
        }

        //----КНОПКИ----
        //кнопка Сохранить/Перезагрузить
        private void UpdateSaveBtnState() //состояние кнопки 
        {
            SaveBtn.IsEnabled = _catalogs != null && _serializer != null && _isFormatSelected;
        }
        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!_isFormatSelected || _serializer == null)
            {
                MessageBox.Show("Выберите формат сохранения");
                return;
            }
            SaveCatalogs();
        }
        private void ReloadBtn_Click(Object sender, RoutedEventArgs e)
        {
            LoadCatalogs();

            CatalogComboBox.ItemsSource = null;
            CatalogComboBox.ItemsSource = _catalogs;
        }
        // ----
        //кнопка Формат
        private void FormatComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FormatComboBox.SelectedItem == null)
            {
                return;
            }
            string format = ((ComboBoxItem)FormatComboBox.SelectedItem).Content.ToString();
            SetFormat(format);
        }
        private void SetFormat(string format)
        {
            if (format == "XML")
            {
                _serializer = new XmlSerialize();
                _extension = "xml";
            }
            else
            {
                _serializer = new JsonSerialize();
                _extension = "json";
            }
            _isFormatSelected = true;
            LoadCatalogs();
            UpdateSaveBtnState();
        }
        // ----
        //работа с каталогами
        private void LoadCatalogs()
        {
            _catalogs = new FurnitureCatalog[4];
            for (int i = 0; i < _catalogs.Length; i++)
            {
                _catalogs[i] = new FurnitureCatalog
                {
                    Name = "Каталог" + (i + 1)
                };
            }
            for (int i = 0; i < _catalogs.Length; i++)
            {
                string path = System.IO.Path.Combine("Catalogs",$"catalog{i}.{_extension}");
                if (File.Exists(path))
                {
                    _catalogs[i].Items = _serializer.Load(path);
                }
                else
                {
                    CreateCatalogs();
                }
            }
            UpdateSaveBtnState();
        }
        private void SaveCatalogs()
        {
            string folder = "Catalogs";
            Directory.CreateDirectory(folder);
            for (int i = 0; i < _catalogs.Length; i++)
            {
                string path = System.IO.Path.Combine(folder, $"catalog{i}.{_extension}");
                Furniture[] items = _catalogs[i].Items;
                _serializer.Save(path, items);
            }
        }
        private void CatalogComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) //состояние кнопки Открыть каталог
        {
            if (CatalogComboBox.SelectedItem != null)
            {
                OpenCatalogBtn.IsEnabled = true;
            }
            else 
            { 
                OpenCatalogBtn.IsEnabled = false; 
            }
        }
        private void OpenCatalogBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedCatalog = CatalogComboBox.SelectedItem as FurnitureCatalog;
            if (selectedCatalog == null)
            {
                MessageBox.Show("Выбери каталог");
                return;
            }
            var window = new CatalogWindow(selectedCatalog);
            window.Show();
        }
        private void CreateCatalogs()
        {
            _catalogs = new FurnitureCatalog[4];

            _catalogs[0] = new FurnitureCatalog { Name = "Офис" };
            _catalogs[1] = new FurnitureCatalog { Name = "Кухня" };
            _catalogs[2] = new FurnitureCatalog { Name = "Гостиная" };
            _catalogs[3] = new FurnitureCatalog { Name = "Спальня" };
            //МЕБЕЛЬ
            Chair chair1 = new Chair
            {
                Id = 1,
                Article = "A1",
                Brand = "IKEA",
                Model = "Markus",
                Name = "Office Chair",
                _basePrice = 10000,
                ImagePath = "Images/chair1.jpg",
                HasArmrests = true,
                LegsCount = 4
            };

            Chair chair2 = new Chair
            {
                Id = 2,
                Article = "A2",
                Brand = "Herman Miller",
                Model = "Aeron",
                Name = "Premium Chair",
                _basePrice = 50000,
                ImagePath = "Images/chair2.jpg",
                HasArmrests = true,
                LegsCount = 5
            };
            Chair chair3 = new Chair
            {
                Id = 2,
                Article = "A7",
                Brand = "Obby",
                Model = "Quadro",
                Name = "Kitchen Chair",
                _basePrice = 5590,
                ImagePath = "Images/chair3.jpg",
                HasArmrests = false,
                LegsCount = 4
            };

            Stool stool1 = new Stool
            {
                Id = 3,
                Article = "A3",
                Brand = "IKEA",
                Model = "FROSTA",
                Name = "Wood Stool",
                _basePrice = 2000,
                ImagePath = "Images/stool1.jpg",
                HasArmrests = false,
                LegsCount = 4,
                HasWheels = false
            };

            Armchair armchair1 = new Armchair
            {
                Id = 4,
                Article = "A4",
                Brand = "BoConcept",
                Model = "Royal",
                Name = "Luxury Armchair",
                _basePrice = 70000,
                ImagePath = "Images/armchair1.jpg",
                HasArmrests = true,
                LegsCount = 4,
                HasGenuineLeather = true
            };
            Armchair armchair2 = new Armchair
            {
                Id = 14,
                Article = "A5",
                Brand = "LoveSit",
                Model = "Frank",
                Name = "Office armchair",
                _basePrice = 55000,
                ImagePath = "Images/armchair2.jpg",
                HasArmrests = true,
                LegsCount = 4,
                HasGenuineLeather = false
            };
            Armchair armchair3 = new Armchair
            {
                Id = 15,
                Article = "A6",
                Brand = "Hoff",
                Model = "Baron",
                Name = "Comfy armchair",
                _basePrice = 20990,
                ImagePath = "Images/armchair3.jpg",
                HasArmrests = true,
                LegsCount = 4,
                HasGenuineLeather = false
            };
            Sofa sofa1 = new Sofa
            {
                Id = 5,
                Article = "B1",
                Brand = "Lazurit",
                Model = "SoftLine",
                Name = "Corner Sofa",
                _basePrice = 65000,
                ImagePath = "Images/sofa1.jpg",
                IsCorner = true,
                SeatsCounts = 3
            };

            Sofa sofa2 = new Sofa
            {
                Id = 6,
                Article = "B2",
                Brand = "Hoff",
                Model = "Comfort",
                Name = "Classic Sofa",
                _basePrice = 35000,
                ImagePath = "Images/sofa2.jpg",
                IsCorner = false,
                SeatsCounts = 2
            };
            Sofa sofa3 = new Sofa
            {
                Id = 11,
                Article = "B3",
                Brand = "Askona",
                Model = "Grand De Luxe",
                Name = "Sofa Domo Pro",
                _basePrice = 100000,
                ImagePath = "Images/sofa3.jpg",
                IsCorner = true,
                SeatsCounts = 6
            };

            Model.Core.Table table1 = new Model.Core.Table
            {
                Id = 7,
                Article = "C1",
                Brand = "IKEA",
                Model = "Lack",
                Name = "Dining Table",
                _basePrice = 8000,
                ImagePath = "Images/table1.jpg",
                HasDrawers = false,
                SeatsCounts = 4
            };

            Model.Core.Table table2 = new Model.Core.Table
            {
                Id = 8,
                Article = "C2",
                Brand = "IKEA",
                Model = "Bekant",
                Name = "Office Table",
                _basePrice = 12000,
                ImagePath = "Images/table2.jpg",
                HasDrawers = true,
                SeatsCounts = 1
            };
            Model.Core.Table table3 = new Model.Core.Table
            {
                Id = 16,
                Article = "C3",
                Brand = "Hosta",
                Model = "Loft",
                Name = "Coffee Table",
                _basePrice = 9706,
                ImagePath = "Images/table3.jpg",
                HasDrawers = false,
                SeatsCounts = 0
            };
            Model.Core.Table table4 = new Model.Core.Table
            {
                Id = 17,
                Article = "C4",
                Brand = "Hoff",
                Model = "Domino",
                Name = "Computer Table",
                _basePrice = 40999,
                ImagePath = "Images/table4.jpg",
                HasDrawers = true,
                SeatsCounts = 1
            };

            Bed bed1 = new Bed
            {
                Id = 9,
                Article = "D1",
                Brand = "Hoff",
                Model = "Sleepy",
                Name = "Single Bed",
                _basePrice = 20000,
                ImagePath = "Images/bed1.jpg",
                HasStorageBox = true,
                Size = "Single"
            };

            Bed bed2 = new Bed
            {
                Id = 10,
                Article = "D2",
                Brand = "Lazurit",
                Model = "Dream",
                Name = "Double Bed",
                _basePrice = 35000,
                ImagePath = "Images/bed2.jpg",
                HasStorageBox = false,
                Size = "Double"
            };
            Bed bed3 = new Bed
            {
                Id = 12,
                Article = "D3",
                Brand = "IKEA",
                Model = "Horizon",
                Name = "OneAndHalf Bed",
                _basePrice = 40000,
                ImagePath = "Images/bed3.jpg",
                HasStorageBox = true,
                Size = "OneAndHalf"
            };
            Bed bed4 = new Bed
            {
                Id = 13,
                Article = "D4",
                Brand = "The Era",
                Model = "Florence",
                Name = "White gloss King Bed",
                _basePrice = 40000,
                ImagePath = "Images/bed4.jpg",
                HasStorageBox = false,
                Size = "King"
            };
            //приведение к базовому классу 
            Furniture f1 = chair1;
            Furniture f2 = chair2;
            Furniture f3 = stool1;
            Furniture f4 = armchair1;
            Furniture f5 = sofa1;
            Furniture f6 = sofa2;
            Furniture f7 = sofa3;
            Furniture f8 = table1;
            Furniture f9 = table2;
            Furniture f10 = bed1;
            Furniture f11 = bed2;
            Furniture f12 = bed3;
            Furniture f13 = bed4;
            Furniture f14 = armchair2;
            Furniture f15 = armchair3;
            Furniture f16 = table3;
            Furniture f17 = chair3;
            Furniture f18 = table4;

            // Каталог 1 - офисный
            _catalogs[0].Add(f1);
            _catalogs[0].Add(f5);
            _catalogs[0].Add(f9);
            _catalogs[0].Add(f4);
            _catalogs[0].Add(f14);

            // Каталог 2 - кухня
            _catalogs[1].Add(f8);
            _catalogs[1].Add(f16);
            _catalogs[1].Add(f6);
            _catalogs[1].Add(f17);
            _catalogs[1].Add(f14);
            _catalogs[1].Add(f3);

            // Каталог 3 - гостиная
            _catalogs[2].Add(f7);
            _catalogs[2].Add(f16);
            _catalogs[2].Add(f15);
            _catalogs[2].Add(f14);
            _catalogs[2].Add(f6);
            _catalogs[2].Add(f5);

            //Каталог 4 - спальня
            _catalogs[3].Add(f10);
            _catalogs[3].Add(f11);
            _catalogs[3].Add(f12);
            _catalogs[3].Add(f13);
            _catalogs[3].Add(f18);
            _catalogs[3].Add(f2);
            _catalogs[3].Add(f14);
            _catalogs[3].Add(f6);
        }
    }
}