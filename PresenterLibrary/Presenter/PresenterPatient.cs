using PresenterLibrary.ClassBD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace PresenterLibrary.Presenter
{
    public class PresenterPatient : IPresenterCommon
    {
        private Window _view;
        private DataGrid _patientGrid;
        private PatientBd _patientBd;
        private string _connectionString;

        public PresenterPatient(Window view, string connectionString)
        {
            _patientBd = new PatientBd(connectionString);
            _view = view;
            _connectionString = connectionString;

            StackPanel gridPanel = _view.FindName("GridPanel") as StackPanel;
            gridPanel.Children.Clear();

            _patientGrid = new DataGrid();
            _patientGrid.Width = double.NaN;
            _patientGrid.CanUserAddRows = false;
            _patientGrid.AutoGenerateColumns = false;
            _patientGrid.IsReadOnly = true;
            _patientGrid.Name = "Пациенты";

            DataGridTextColumn idColumn = new DataGridTextColumn();
            idColumn.Header = "ID";
            idColumn.Binding = new System.Windows.Data.Binding("ID");

            DataGridTextColumn nameColumn = new DataGridTextColumn();
            nameColumn.Header = "ФИО";
            nameColumn.Binding = new System.Windows.Data.Binding("Name");

            DataGridTextColumn birthDateColumn = new DataGridTextColumn();
            birthDateColumn.Header = "Дата рождения";
            birthDateColumn.Binding = new System.Windows.Data.Binding("BirthDate")
            {
                StringFormat = "dd.MM.yyyy"
            };

            DataGridTextColumn addressColumn = new DataGridTextColumn();
            addressColumn.Header = "Адрес";
            addressColumn.Binding = new System.Windows.Data.Binding("Address");

            DataGridTextColumn clinicColumn = new DataGridTextColumn();
            clinicColumn.Header = "Клиника";
            clinicColumn.Binding = new System.Windows.Data.Binding("ClinicName");

            DataGridTextColumn hospitalColumn = new DataGridTextColumn();
            hospitalColumn.Header = "Больница";
            hospitalColumn.Binding = new System.Windows.Data.Binding("HospitalName");

            DataGridTextColumn doctorColumn = new DataGridTextColumn();
            doctorColumn.Header = "Лечающий врач";
            doctorColumn.Binding = new System.Windows.Data.Binding("AttendingDoctorName");

            DataGridTextColumn genderColumn = new DataGridTextColumn();
            genderColumn.Header = "Пол";
            genderColumn.Binding = new System.Windows.Data.Binding("Gender");

            DataGridTextColumn polisColumn = new DataGridTextColumn();
            polisColumn.Header = "Полис";
            polisColumn.Binding = new System.Windows.Data.Binding("Polis");

            _patientGrid.Columns.Add(idColumn);
            _patientGrid.Columns.Add(nameColumn);
            _patientGrid.Columns.Add(birthDateColumn);
            _patientGrid.Columns.Add(addressColumn);
            _patientGrid.Columns.Add(clinicColumn);
            _patientGrid.Columns.Add(hospitalColumn);
            _patientGrid.Columns.Add(doctorColumn);
            _patientGrid.Columns.Add(genderColumn);
            _patientGrid.Columns.Add(polisColumn);

            _patientGrid.ItemsSource = _patientBd.PatientList;
            gridPanel.Children.Add(_patientGrid);
        }

        public void AddObject()
        {
            AddWindowPatient addWindow = new AddWindowPatient(new PatientBd.Patient(), _connectionString);
            if (addWindow.ShowDialog() == true)
            {
                _patientBd.AddPatient(addWindow.Patient);
             //   MessageBox.Show("Patient successfully added", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                _patientGrid.ItemsSource = _patientBd.PatientList;
                _patientGrid.Items.Refresh();
            }
        }

        public void EditObject()
        {
            if (_patientGrid.SelectedItem != null)
            {
                int index = _patientGrid.SelectedIndex;
                PatientBd.Patient selectedPatient = _patientBd.PatientList[index];
               AddWindowPatient editWindow = new AddWindowPatient(selectedPatient, _connectionString);

                if (editWindow.ShowDialog() == true)
                {
                    _patientBd.UpdatePatient(editWindow.Patient);
              //      MessageBox.Show("Patient successfully updated", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    _patientGrid.ItemsSource = _patientBd.PatientList;
                    _patientGrid.Items.Refresh();
                }
            }
        }

        public void DeleteObject()
        {
            if (_patientGrid.SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить запись?", "Подтвердить", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        int index = _patientGrid.SelectedIndex;
                        PatientBd.Patient selectedPatient = _patientBd.PatientList[index];

                        _patientBd.DeletePatient(selectedPatient.ID);
                        _patientGrid.ItemsSource = _patientBd.PatientList;
                        _patientGrid.Items.Refresh();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        public void Search(string searchTerm)
        {
            List<PatientBd.Patient> filteredList = _patientBd.PatientList
                .Where(patient =>
                    patient.GetType().GetProperties()
                        .Any(prop =>
                            prop.GetValue(patient)?.ToString()?.ToLower().Contains(searchTerm.ToLower()) ?? false
                        )
                )
                .ToList();

            _patientGrid.ItemsSource = filteredList;
            _patientGrid.Items.Refresh();
        }
    }


}
