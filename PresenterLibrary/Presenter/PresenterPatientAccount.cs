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
    public class PresenterPatientAccount : IPresenterCommon
    {
        private Window _view;
        private DataGrid _patientAccountGrid;
        public PatientAccountBd _patientAccountBd;
        private string _connectionString;

        public PresenterPatientAccount(Window view, string connectionString)
        {
            _patientAccountBd = new PatientAccountBd(connectionString);
            _view = view;
            _connectionString = connectionString;

            StackPanel gridPanel = _view.FindName("GridPanel") as StackPanel;
            gridPanel.Children.Clear();

            _patientAccountGrid = new DataGrid
            {
                Width = double.NaN,
                CanUserAddRows = false,
                AutoGenerateColumns = false,
                IsReadOnly = true,
                Name = "Учёт"
            };

            // Определение столбцов для DataGrid
            DataGridTextColumn idColumn = new DataGridTextColumn
            {
                Header = "ID",
                Binding = new System.Windows.Data.Binding("ID")
            };

            DataGridTextColumn patientNameColumn = new DataGridTextColumn
            {
                Header = "Имя пациента",
                Binding = new System.Windows.Data.Binding("PatientName")
            };

            DataGridTextColumn medicalHistoryColumn = new DataGridTextColumn
            {
                Header = "История болезни",
                Binding = new System.Windows.Data.Binding("MedicalHistory")
            };

            DataGridTextColumn doctorNameColumn = new DataGridTextColumn
            {
                Header = "Имя доктора",
                Binding = new System.Windows.Data.Binding("DoctorName")
            };

            DataGridTextColumn hospitalNameColumn = new DataGridTextColumn
            {
                Header = "Больница",
                Binding = new System.Windows.Data.Binding("HospitalName")
            };

            DataGridTextColumn surgeriesColumn = new DataGridTextColumn
            {
                Header = "Операции",
                Binding = new System.Windows.Data.Binding("SurgeriesDisplay")
            };

            DataGridTextColumn recommendationsColumn = new DataGridTextColumn
            {
                Header = "Рекомендации",
                Binding = new System.Windows.Data.Binding("RecommendationsDisplay")
            };

            _patientAccountGrid.Columns.Add(idColumn);
            _patientAccountGrid.Columns.Add(patientNameColumn);
            _patientAccountGrid.Columns.Add(medicalHistoryColumn);
            _patientAccountGrid.Columns.Add(doctorNameColumn);
            _patientAccountGrid.Columns.Add(hospitalNameColumn);
            _patientAccountGrid.Columns.Add(surgeriesColumn);
            _patientAccountGrid.Columns.Add(recommendationsColumn);

            _patientAccountGrid.ItemsSource = _patientAccountBd.PatientAccountList;
            gridPanel.Children.Add(_patientAccountGrid);
        }

        public void AddObject()
        {
            try
            {
                AddPatientAccountWindow addWindow = new AddPatientAccountWindow(new PatientAccountBd.PatientAccount(), _connectionString);
                if (addWindow.ShowDialog() == true)
                {
                    _patientAccountBd.AddPatientAccount(addWindow.patientAccount.PatientName, addWindow.patientAccount.MedicalHistory);
                    RefreshGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Значения должна быть уникальными, произошла ошибка");
            }
        }

        public void EditObject()
        {
            try
            {


                if (_patientAccountGrid.SelectedItem != null)
                {
                    int index = _patientAccountGrid.SelectedIndex;
                    var selectedPatientAccount = _patientAccountBd.PatientAccountList[index];

                    AddPatientAccountWindow editWindow = new AddPatientAccountWindow(selectedPatientAccount, _connectionString);
                    if (editWindow.ShowDialog() == true)
                    {
                        _patientAccountBd.UpdatePatientAccount(editWindow.patientAccount);
                        RefreshGrid();
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Значения должна быть уникальными, произошла ошибка");
            }
        }

        public void DeleteObject()
        {
            if (_patientAccountGrid.SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить запись?", "Подтвердить", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        int index = _patientAccountGrid.SelectedIndex;
                        var selectedPatientAccount = _patientAccountBd.PatientAccountList[index];

                        _patientAccountBd.DeletePatientAccount(selectedPatientAccount.ID);
                        RefreshGrid();
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
            var filteredList = _patientAccountBd.PatientAccountList
                .Where(account =>
                    account.GetType().GetProperties()
                        .Any(prop =>
                            prop.GetValue(account)?.ToString()?.ToLower().Contains(searchTerm.ToLower()) ?? false
                        )
                )
                .ToList();

            _patientAccountGrid.ItemsSource = filteredList;
            _patientAccountGrid.Items.Refresh();
        }


        private void RefreshGrid()
        {
            var pA = _patientAccountBd.GetPatientAccountData();
            _patientAccountGrid.ItemsSource = pA;
            _patientAccountGrid.Items.Refresh();
        }
    }


}
