using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;

namespace lab10
{
    public partial class MainWindow : Window
    {
        private CustomBindingList<Car> myCars;

        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        public MainWindow()
        {

            AllocConsole();
            Console.WriteLine("Hello world!!!");

            InitializeComponent();

            var myCarsList = new List<Car>
            {
                new Car("Mercedes E250", new Engine(1.8, 204, "CGI"), 2009),
                new Car("Mercedes E350", new Engine(3.5, 292, "CGI"), 2009),
                new Car("Audi A6", new Engine(2.5, 187, "FSI"), 2012),
                new Car("Audi A6", new Engine(2.8, 220, "FSI"), 2012),
                new Car("Audi A6", new Engine(3.0, 295, "TFSI"), 2012),
                new Car("Audi A6", new Engine(2.0, 175, "TDI"), 2011),
                new Car("Audi A6", new Engine(3.0, 309, "TDI"), 2011),
                new Car("Audi S6", new Engine(4.0, 414, "TFSI"), 2012),
                new Car("Audi S8", new Engine(4.0, 513, "TFSI"), 2012)
            };

            Console.WriteLine("query expression syntax");
            var query1 = from car in myCarsList
                         where car.Model == "Audi A6"
                         let engineType = car.Motor.Model == "TDI" ? "diesel" : "petrol"
                         group car by engineType into carGroup
                         select new
                         {
                             engineType = carGroup.Key,
                             avgHPPL = carGroup.Average(car => car.Motor.HorsePower / car.Motor.Displacement)
                         } into result
                         orderby result.avgHPPL descending
                         select result;

            foreach (var e in query1)
            {
                Console.WriteLine(e.engineType + ": " + e.avgHPPL);
            }


            Console.WriteLine("method-based syntax");

            var query2 = myCarsList
                .Where(car => car.Model == "Audi A6")
                .GroupBy(car => car.Motor.Model == "TDI" ? "diesel" : "petrol")
                .Select(carGroup => new
                {
                    engineType = carGroup.Key,
                    avgHPPL = carGroup.Average(car => car.Motor.HorsePower / car.Motor.Displacement)
                })
                .OrderByDescending(result => result.avgHPPL);

            foreach (var e in query2)
            {
                Console.WriteLine(e.engineType + ": " + e.avgHPPL);
            }



            myCars = new CustomBindingList<Car>(myCarsList);

            // arg1 - Func for sorting by descending HorsePower
            Comparison<Car> arg1 = new Comparison<Car>((x, y) => y.Motor.HorsePower.CompareTo(x.Motor.HorsePower));
            myCarsList.Sort(arg1);

            Console.WriteLine("\nposortowane po mocy silika");
            foreach (var car in myCarsList)
            {
                Console.WriteLine(car.Model + " " + car.Motor.HorsePower);
            }

            // arg2 - Predicate for finding cars with "TDI" engines
            Predicate<Car> arg2 = new Predicate<Car>(car => car.Motor.Model == "TDI");

            // arg3 - Action for displaying each car in a MessageBox
            Action<Car> arg3 = new Action<Car>(car => MessageBox.Show(car.Model + " " + car.Year + " " + car.Motor));

            // Usage
            myCarsList.FindAll(arg2).ForEach(arg3);

            dataGrid.ItemsSource = myCars;
            LoadPropertiesToComboBox();
            Console.ReadLine();
            FreeConsole();
        }

        private void LoadPropertiesToComboBox()
        {
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(typeof(Car)))
            {
                if (prop.PropertyType == typeof(string) || prop.PropertyType == typeof(int) || prop.Name == "Motor")
                {
                    propertyComboBox.Items.Add(prop.Name);
                }
            }
        }

        private void OnToolBarSearchClick(object sender, RoutedEventArgs e)
        {
            string propertyName = propertyComboBox.SelectedItem.ToString();
            string valueText = valueTextBox.Text;

            if (propertyName == "Motor")
            {
                myCars.Filter(car =>
                {
                    return car.Motor.Model.ToLower().Contains(valueText.ToLower()) ||
                           car.Motor.Displacement.ToString().Contains(valueText) ||
                           car.Motor.HorsePower.ToString().Contains(valueText);
                });
            }
            else
            {
                if (int.TryParse(valueText, out int intValue))
                {
                    myCars.Filter(car =>
                    {
                        PropertyDescriptor prop = TypeDescriptor.GetProperties(typeof(Car)).Find(propertyName, true);
                        return prop != null && prop.GetValue(car).Equals(intValue);
                    });
                }
                else
                {
                    myCars.Filter(car =>
                    {
                        PropertyDescriptor prop = TypeDescriptor.GetProperties(typeof(Car)).Find(propertyName, true);
                        return prop != null && prop.GetValue(car).ToString().ToLower().Contains(valueText.ToLower());
                    });
                }
            }
        }

        private void OnToolBarSortClick(object sender, RoutedEventArgs e)
        {
            string propertyName = propertyComboBox.SelectedItem.ToString();
            myCars.Sort(propertyName, ListSortDirection.Ascending); // or ListSortDirection.Descending based on your requirement
        }




        private void OnResetFilterClick(object sender, RoutedEventArgs e)
        {
            myCars.ResetFilter();
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            // Sprawdzenie, czy zaznaczony jest jakikolwiek wiersz
            if (dataGrid.SelectedItem != null)
            {
                // Usunięcie zaznaczonego obiektu z kolekcji
                myCars.Remove((Car)dataGrid.SelectedItem);
            }
            else
            {
                MessageBox.Show("Select an item to delete.");
            }
        }

        private void OnAddClick(object sender, RoutedEventArgs e)
        {
            // Tworzenie nowego okna dialogowego
            var dialog = new CarDialog();

            // Wyświetlenie okna dialogowego i sprawdzenie, czy użytkownik kliknął przycisk Save
            if (dialog.ShowDialog() == true)
            {
                // Dodanie nowego obiektu Car do kolekcji
                myCars.Add(dialog.Car);
            }
        }

        private void OnEditClick(object sender, RoutedEventArgs e)
        {
            // Sprawdzenie, czy zaznaczony jest jakikolwiek wiersz
            if (dataGrid.SelectedItem != null)
            {
                // Pobranie zaznaczonego obiektu
                var selectedCar = (Car)dataGrid.SelectedItem;

                // Tworzenie nowego okna dialogowego z wybranym obiektem Car
                var dialog = new CarDialog(selectedCar);

                // Wyświetlenie okna dialogowego i sprawdzenie, czy użytkownik kliknął przycisk Save
                if (dialog.ShowDialog() == true)
                {
                    // Aktualizacja widoku (jeśli konieczne)
                    // Zazwyczaj nie jest to potrzebne, ponieważ zmiany zostaną odzwierciedlone automatycznie
                }
            }
            else
            {
                MessageBox.Show("Select an item to edit.");
            }
        }


    }
}