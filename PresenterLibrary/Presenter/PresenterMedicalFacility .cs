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
    public class PresenterMedicalFacility : IPresenterCommon
    {
        private Window _view;
        private DataGrid _facilityGrid;
        private MedicalFacilityBd _facilityBd;
        private string BD;

        public PresenterMedicalFacility(Window view, string bd)
        {
            _facilityBd = new MedicalFacilityBd(bd);
            _view = view;
            BD = bd;

            StackPanel gridPanel = _view.FindName("GridPanel") as StackPanel;
            gridPanel.Children.Clear();

            _facilityGrid = new DataGrid();
            _facilityGrid.Width = double.NaN;
            _facilityGrid.CanUserAddRows = false;
            _facilityGrid.AutoGenerateColumns = false;
            _facilityGrid.IsReadOnly = true;
            _facilityGrid.Name = "Медицинские_учереждения";

            DataGridTextColumn idColumn = new DataGridTextColumn();
            idColumn.Header = "ID";
            idColumn.Binding = new System.Windows.Data.Binding("ID");

            DataGridTextColumn nameColumn = new DataGridTextColumn();
            nameColumn.Header = "Название";
            nameColumn.Binding = new System.Windows.Data.Binding("Name");

            DataGridTextColumn typeColumn = new DataGridTextColumn();
            typeColumn.Header = "Тип";
            typeColumn.Binding = new System.Windows.Data.Binding("Type");

            DataGridTextColumn addressColumn = new DataGridTextColumn();
            addressColumn.Header = "Адрес";
            addressColumn.Binding = new System.Windows.Data.Binding("Address");

            DataGridTextColumn parentHospitalIdColumn = new DataGridTextColumn();
            parentHospitalIdColumn.Header = "Прикреплена к больнице";
            parentHospitalIdColumn.Binding = new System.Windows.Data.Binding("ParentHospitalName");

            _facilityGrid.Columns.Add(idColumn);
            _facilityGrid.Columns.Add(nameColumn);
            _facilityGrid.Columns.Add(typeColumn);
            _facilityGrid.Columns.Add(addressColumn);
            _facilityGrid.Columns.Add(parentHospitalIdColumn);

            _facilityGrid.ItemsSource = _facilityBd.FacilityList;
            gridPanel.Children.Add(_facilityGrid);
        }

        public void AddObject()
        {
            AddMedicalFacility addWindow = new AddMedicalFacility(new MedicalFacilityBd.MedicalFacility(), BD);
            if (addWindow.ShowDialog() == true)
            {
                try
                {
                    _facilityBd.AddFacility(addWindow.Facility);
                    MessageBox.Show("Медицинское учреждение успешно добавлено", "Ок", MessageBoxButton.OK, MessageBoxImage.Information);
                    _facilityGrid.ItemsSource = _facilityBd.FacilityList;
                    _facilityGrid.Items.Refresh();
                }
                catch (Exception ex)
                {
                    // Handle the error by showing a message or logging it
                    MessageBox.Show($"Ошибка при добавлении учреждения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void EditObject()
        {
            if (_facilityGrid.SelectedItem != null)
            {
                int index = _facilityGrid.SelectedIndex;
                MedicalFacilityBd.MedicalFacility selectedFacility = _facilityBd.FacilityList[index];
                AddMedicalFacility editWindow = new AddMedicalFacility(selectedFacility, BD);

                if (editWindow.ShowDialog() == true)
                {
                    try
                    {
                        _facilityBd.UpdateFacility(editWindow.Facility);
                        MessageBox.Show("Медицинское учреждение успешно обновлено", "Ок", MessageBoxButton.OK, MessageBoxImage.Information);
                        _facilityGrid.ItemsSource = _facilityBd.FacilityList;
                        _facilityGrid.Items.Refresh();
                    }
                    catch (Exception ex)
                    {
                        // Handle the error by showing a message or logging it
                        MessageBox.Show($"Ошибка при добавлении учреждения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        public void DeleteObject()
        {
            if (_facilityGrid.SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить запись?", "Подтвердить", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        int index = _facilityGrid.SelectedIndex;
                        MedicalFacilityBd.MedicalFacility selectedFacility = _facilityBd.FacilityList[index];

                        _facilityBd.DeleteFacility(selectedFacility.ID);
                        _facilityGrid.ItemsSource = _facilityBd.FacilityList;
                        _facilityGrid.Items.Refresh();
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
            List<MedicalFacilityBd.MedicalFacility> filteredList = _facilityBd.FacilityList
                .Where(facility =>
                    facility.GetType().GetProperties()
                        .Any(prop =>
                            prop.GetValue(facility)?.ToString()?.ToLower().Contains(searchTerm.ToLower()) ?? false
                        )
                )
                .ToList();

            _facilityGrid.ItemsSource = filteredList;
            _facilityGrid.Items.Refresh();
        }
    }

}
