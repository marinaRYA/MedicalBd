using PresenterLibrary;
using PresenterLibrary.ClassBD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using PresenterLibrary.Presenter;

public class PresenterSurgery : IPresenterCommon
    {
        private Window _view;
        private DataGrid _surgeryGrid;
        private SurgeryBd _surgeryBd;
        private string _connectionString;

        public PresenterSurgery(Window view, string connectionString)
        {
            _surgeryBd = new SurgeryBd(connectionString);
            _view = view;
            _connectionString = connectionString;

            StackPanel gridPanel = _view.FindName("GridPanel") as StackPanel;
            gridPanel.Children.Clear();

            _surgeryGrid = new DataGrid();
            _surgeryGrid.Width = double.NaN;
            _surgeryGrid.CanUserAddRows = false;
            _surgeryGrid.AutoGenerateColumns = false;
            _surgeryGrid.IsReadOnly = true;
            _surgeryGrid.Name = "Операции";

            DataGridTextColumn idColumn = new DataGridTextColumn();
            idColumn.Header = "ID";
            idColumn.Binding = new System.Windows.Data.Binding("ID");

            DataGridTextColumn NameColumn = new DataGridTextColumn();
            NameColumn.Header = "Название";
            NameColumn.Binding = new System.Windows.Data.Binding("Name");

        DataGridTextColumn patientNameColumn = new DataGridTextColumn();
            patientNameColumn.Header = "ФИО Пациента";
            patientNameColumn.Binding = new System.Windows.Data.Binding("PatientName");

            DataGridTextColumn doctorNameColumn = new DataGridTextColumn();
            doctorNameColumn.Header = "Имя доктора";
            doctorNameColumn.Binding = new System.Windows.Data.Binding("DoctorName");

            DataGridTextColumn hospitalNameColumn = new DataGridTextColumn();
            hospitalNameColumn.Header = "Медицинское учреждение";
            hospitalNameColumn.Binding = new System.Windows.Data.Binding("HospitalName");

            DataGridTextColumn dateColumn = new DataGridTextColumn();
            dateColumn.Header = "Дата:";
            dateColumn.Binding = new System.Windows.Data.Binding("Date")
            {
                StringFormat = "dd.MM.yyyy"
            };


        DataGridTextColumn outcomeColumn = new DataGridTextColumn();
            outcomeColumn.Header = "Исход";
            outcomeColumn.Binding = new System.Windows.Data.Binding("Outcome");

            _surgeryGrid.Columns.Add(idColumn);
            _surgeryGrid.Columns.Add(NameColumn);
            _surgeryGrid.Columns.Add(patientNameColumn);
            _surgeryGrid.Columns.Add(doctorNameColumn);
            _surgeryGrid.Columns.Add(hospitalNameColumn);
            _surgeryGrid.Columns.Add(dateColumn);
            _surgeryGrid.Columns.Add(outcomeColumn);

            _surgeryGrid.ItemsSource = _surgeryBd.SurgeryList;
            gridPanel.Children.Add(_surgeryGrid);
        }

        public void AddObject()
        {
            AddWindowSurgery addWindow = new AddWindowSurgery(new SurgeryBd.Surgery(), _connectionString);
            if (addWindow.ShowDialog() == true)
            {
                _surgeryBd.AddSurgery(addWindow.Surgery);
           //     MessageBox.Show("Surgery successfully added", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                _surgeryGrid.ItemsSource = _surgeryBd.SurgeryList;
                _surgeryGrid.Items.Refresh();
            }
        }

        public void EditObject()
        {
            if (_surgeryGrid.SelectedItem != null)
            {
                int index = _surgeryGrid.SelectedIndex;
                SurgeryBd.Surgery selectedSurgery = _surgeryBd.SurgeryList[index];
                AddWindowSurgery editWindow = new AddWindowSurgery(selectedSurgery, _connectionString);
                if (editWindow.ShowDialog() == true)
                {
                    _surgeryBd.UpdateSurgery(editWindow.Surgery);
           //         MessageBox.Show("Surgery successfully updated", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    _surgeryGrid.ItemsSource = _surgeryBd.SurgeryList;
                    _surgeryGrid.Items.Refresh();
                }
            }
        }

        public void DeleteObject()
        {
            if (_surgeryGrid.SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить запись?", "Подтвердить", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        int index = _surgeryGrid.SelectedIndex;
                        SurgeryBd.Surgery selectedSurgery = _surgeryBd.SurgeryList[index];

                        _surgeryBd.DeleteSurgery(selectedSurgery.ID);
                        _surgeryGrid.ItemsSource = _surgeryBd.SurgeryList;
                        _surgeryGrid.Items.Refresh();
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
            List<SurgeryBd.Surgery> filteredList = _surgeryBd.SurgeryList
                .Where(surgery =>
                    surgery.GetType().GetProperties()
                        .Any(prop =>
                            prop.GetValue(surgery)?.ToString()?.ToLower().Contains(searchTerm.ToLower()) ?? false
                        )
                )
                .ToList();

            _surgeryGrid.ItemsSource = filteredList;
            _surgeryGrid.Items.Refresh();
        }
    }

