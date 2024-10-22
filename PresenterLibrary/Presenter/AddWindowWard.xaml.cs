using System;
using System.Collections.Generic;
using System.Data.SQLite;
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

namespace PresenterLibrary.Presenter
{
    /// <summary>
    /// Логика взаимодействия для AddWindowWard.xaml
    /// </summary>
    public partial class AddWindowWard : Window
    {
        public WardBd.Ward Ward { get; private set; }
        private string ConnectionString;

        public AddWindowWard(WardBd.Ward ward, string bd)
        {
            InitializeComponent();
            ConnectionString = $"Data Source={bd};Version=3;";
            Ward = ward ?? new WardBd.Ward();

            LoadDepartments();
            DisplayWardDetails();
        }

        // Загрузка списка отделений
        private void LoadDepartments()
        {
            List<string> departmentNames = new List<string>();
            string query = "SELECT Name FROM Department;";

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string departmentName = reader["Name"] as string;
                            if (!string.IsNullOrWhiteSpace(departmentName))
                            {
                                departmentNames.Add(departmentName);
                            }
                        }
                    }
                }
            }

            DepartmentComboBox.ItemsSource = departmentNames;
        }

        // Отображение текущих данных палаты
        private void DisplayWardDetails()
        {
            if (Ward != null)
            {
                NumberTextBox.Text = Ward.Number.ToString();
                BedCountTextBox.Text = Ward.BedCount.ToString();
                DepartmentComboBox.SelectedItem = Ward.DepartmentName;
            }
        }

        // Обработка нажатия кнопки сохранения
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка обязательных полей
                if (string.IsNullOrWhiteSpace(NumberTextBox.Text) ||
                    string.IsNullOrWhiteSpace(BedCountTextBox.Text) ||
                    DepartmentComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    Ward.Number = int.Parse(NumberTextBox.Text);
                    Ward.BedCount = int.Parse(BedCountTextBox.Text);
                    Ward.DepartmentName = DepartmentComboBox.SelectedItem as string;

                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработка нажатия кнопки отмены
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

}
