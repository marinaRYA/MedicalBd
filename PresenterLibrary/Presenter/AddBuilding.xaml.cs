using PresenterLibrary.ClassBD;
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
using System.Xml.Linq;
using static PresenterLibrary.ClassBD.BuildingBd;
using static PresenterLibrary.ClassBD.MedicalFacilityBd;

namespace PresenterLibrary.Presenter
{
    /// <summary>
    /// Логика взаимодействия для AddMedicalFacility.xaml
    /// </summary>
    public partial class AddBuilding : Window
    {
        public BuildingBd.Building Building;
        private MedicalFacilityBd medicalBd;
        private string ConnectionString;

        public AddBuilding(BuildingBd.Building building, string bd)
        {
            InitializeComponent();
            medicalBd = new MedicalFacilityBd(bd);
            Building = building ?? new BuildingBd.Building();
            ConnectionString = $"Data Source={bd};Version=3;";
            LoadParentHospitals();
            DisplayBuildingDetails();
        }
            
        private void DisplayBuildingDetails()
        {
            if (Building != null)
            {
                BuildingNameTextBox.Text = Building.BuildingName;
                HospitalNameComboBox.SelectedItem =  Building.HospitalName;
                AddressTextBox.Text = Building.Address;
            }
        }

        private void LoadParentHospitals()
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

            HospitalNameComboBox.ItemsSource = hospitalNames;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(BuildingNameTextBox.Text) &&
                    string.IsNullOrWhiteSpace(HospitalNameComboBox.SelectedItem as string) &&
                    string.IsNullOrWhiteSpace(AddressTextBox.Text)) 
             {
                    MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
             else
                {

                    Building.BuildingName = BuildingNameTextBox.Text;
                    Building.Address = AddressTextBox.Text;
                    Building.HospitalName = HospitalNameComboBox.SelectedItem as string;

                    DialogResult = true; // Close window with OK status
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

