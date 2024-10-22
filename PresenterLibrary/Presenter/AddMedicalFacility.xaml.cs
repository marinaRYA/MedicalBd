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

namespace PresenterLibrary.Presenter
{
    /// <summary>
    /// Логика взаимодействия для AddMedicalFacility.xaml
    /// </summary>

    public partial class AddMedicalFacility : Window
    {
        //private MedicalFacilityBd _facilityBd;
        public MedicalFacilityBd.MedicalFacility Facility { get; private set; }
        private string ConnectionString;
        public AddMedicalFacility(MedicalFacilityBd.MedicalFacility facility, string bd)
        {
            InitializeComponent();
            ConnectionString = $"Data Source={bd};Version=3;";;
            
            Facility = facility ?? new MedicalFacilityBd.MedicalFacility();

            LoadType();
            LoadParentHospitals();
            DisplayFacilityDetails();
        }

        private void LoadType()
        {
            List<string> type = new List<string> { "Поликлиника", "Больница" };


            TypeTextBox.ItemsSource = type;
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

            ParentHospitalComboBox.ItemsSource = hospitalNames;
        }

       
        private void DisplayFacilityDetails()
        {
            if (Facility != null)
            {
                NameTextBox.Text = Facility.Name;
                TypeTextBox.SelectedItem = Facility.Type;
                AddressTextBox.Text = Facility.Address;
                ParentHospitalComboBox.SelectedItem = Facility.ParentHospitalName;
            }
        }

        // Save button click handler
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
              
                if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                    TypeTextBox.SelectedItem == null ||
                    string.IsNullOrWhiteSpace(AddressTextBox.Text)) 
                {
                    MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                   
                    Facility.Name = NameTextBox.Text;
                    Facility.Type = TypeTextBox.SelectedItem as string;
                    Facility.Address = AddressTextBox.Text;
                    Facility.ParentHospitalName = ParentHospitalComboBox.SelectedItem as string;

                    DialogResult = true; 
                    Close();
                }
            }
            catch (Exception ex)
            {
                
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                
            }
        }

        // Cancel button click handler
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; 
            Close();
        }
    }

}

