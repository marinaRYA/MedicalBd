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
    /// Логика взаимодействия для AddWindowLaboratory.xaml
    /// </summary>
    public partial class AddWindowLaboratory : Window
    {
        public LaboratoryBd.Laboratory Laboratory;
        private string ConnectionString;

        public AddWindowLaboratory(LaboratoryBd.Laboratory laboratory, string bd)
        {
            InitializeComponent();
            Laboratory = laboratory ?? new LaboratoryBd.Laboratory();
            ConnectionString = $"Data Source={bd};Version=3;";
            LoadProfiles();
            LoadHospitals();
            DisplayLaboratoryDetails();
        }
        private void LoadHospitals()
        {
            List<string> hospitalNames = new List<string>();
            string query = "SELECT DISTINCT Name FROM MedicalFacility WHERE Type = 'Больница';";

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string hospitalName = reader["Name"] as string;
                            if (!string.IsNullOrWhiteSpace(hospitalName))
                            {
                                hospitalNames.Add(hospitalName);
                            }
                        }
                    }
                }
            }

            HospitalComboBox.ItemsSource = hospitalNames;
        }
        private void DisplayLaboratoryDetails()
        {
            if (Laboratory != null)
            {
                NameTextBox.Text = Laboratory.Name;
                ProfileComboBox.SelectedItem = Laboratory.Profile;
                HospitalComboBox.SelectedItem = Laboratory.HospitalClinicName;
            }
        }

        private void LoadProfiles()
        {
            List<string> profiles = new List<string>();
            string query = "SELECT DISTINCT ProfileLabName FROM ProfileLab;";

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string profile = reader["ProfileLabName"] as string;
                            if (!string.IsNullOrWhiteSpace(profile))
                            {
                                profiles.Add(profile);
                            }
                        }
                    }
                }
            }

            ProfileComboBox.ItemsSource = profiles;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(ProfileComboBox.SelectedItem as string) || string.IsNullOrWhiteSpace(HospitalComboBox.SelectedItem as string))
                {
                    MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    Laboratory.Name = NameTextBox.Text;
                    Laboratory.Profile = ProfileComboBox.SelectedItem as string;
                    Laboratory.HospitalClinicName = HospitalComboBox.SelectedItem as string;

                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }


    }
}
