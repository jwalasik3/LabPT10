using System.Windows;

namespace lab10
{
    public partial class CarDialog : Window
    {
        public Car Car { get; private set; }

        public CarDialog()
        {
            InitializeComponent();
        }

        public CarDialog(Car car) : this()
        {
            Car = car;
            modelTextBox.Text = Car.Model;
            yearTextBox.Text = Car.Year.ToString();
            displacementTextBox.Text = Car.Motor.Displacement.ToString();
            horsePowerTextBox.Text = Car.Motor.HorsePower.ToString();
            engineModelTextBox.Text = Car.Motor.Model;
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            if (Car == null)
            {
                Car = new Car();
            }

            Car.Model = modelTextBox.Text;
            Car.Year = int.Parse(yearTextBox.Text);
            Car.Motor = new Engine
            (
                double.Parse(displacementTextBox.Text),
                double.Parse(horsePowerTextBox.Text),
                engineModelTextBox.Text
            );

            DialogResult = true;
            Close();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
