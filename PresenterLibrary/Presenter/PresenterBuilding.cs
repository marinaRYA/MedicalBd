using PresenterLibrary.ClassBD;
using PresenterLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using PresenterLibrary.Presenter;

public class PresenterBuilding : IPresenterCommon
{
    private Window _view;
    private DataGrid _buildingGrid;
    private BuildingBd _buildingBd;
    private string _connectionString;

    public PresenterBuilding(Window view, string connectionString)
    {
        _buildingBd = new BuildingBd(connectionString);
        _view = view;
        _connectionString = connectionString;

        StackPanel gridPanel = _view.FindName("GridPanel") as StackPanel;
        gridPanel.Children.Clear();

        _buildingGrid = new DataGrid
        {
            Width = double.NaN,
            CanUserAddRows = false,
            AutoGenerateColumns = false,
            IsReadOnly = true,
            Name = "Корпуса"
        };

        DataGridTextColumn idColumn = new DataGridTextColumn
        {
            Header = "ID",
            Binding = new System.Windows.Data.Binding("ID")
        };

        DataGridTextColumn hospitalNameColumn = new DataGridTextColumn
        {
            Header = "Медицинское учреждение",
            Binding = new System.Windows.Data.Binding("HospitalName")
        };

        DataGridTextColumn buildingNameColumn = new DataGridTextColumn
        {
            Header = "Корпус",
            Binding = new System.Windows.Data.Binding("BuildingName")
        };

        DataGridTextColumn addressColumn = new DataGridTextColumn
        {
            Header = "Адрес",
            Binding = new System.Windows.Data.Binding("Address")
        };

        _buildingGrid.Columns.Add(idColumn);
        _buildingGrid.Columns.Add(hospitalNameColumn);
        _buildingGrid.Columns.Add(buildingNameColumn);
        _buildingGrid.Columns.Add(addressColumn);

        _buildingGrid.ItemsSource = _buildingBd.BuildingList;
        gridPanel.Children.Add(_buildingGrid);
    }

    public void AddObject()
    {
        AddBuilding addWindow = new AddBuilding(new BuildingBd.Building(), _connectionString);
        if (addWindow.ShowDialog() == true)
        {
            _buildingBd.AddBuilding(addWindow.Building);
           // MessageBox.Show("Building successfully added", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            _buildingGrid.ItemsSource = _buildingBd.BuildingList;
            _buildingGrid.Items.Refresh();
        }
    }

    public void EditObject()
    {
        if (_buildingGrid.SelectedItem != null)
        {
            int index = _buildingGrid.SelectedIndex;
            BuildingBd.Building selectedBuilding = _buildingBd.BuildingList[index];
            AddBuilding editWindow = new AddBuilding(selectedBuilding, _connectionString);
            if (editWindow.ShowDialog() == true)
            {
                _buildingBd.UpdateBuilding(editWindow.Building);
           //     MessageBox.Show("Building successfully updated", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                _buildingGrid.ItemsSource = _buildingBd.BuildingList;
                _buildingGrid.Items.Refresh();
            }
        }
    }

    public void DeleteObject()
    {
        if (_buildingGrid.SelectedItem != null)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить запись?", "Подтвердить", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    int index = _buildingGrid.SelectedIndex;
                    BuildingBd.Building selectedBuilding = _buildingBd.BuildingList[index];

                    _buildingBd.DeleteBuilding(selectedBuilding.ID);
                    _buildingGrid.ItemsSource = _buildingBd.BuildingList;
                    _buildingGrid.Items.Refresh();
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
        List<BuildingBd.Building> filteredList = _buildingBd.BuildingList
            .Where(building =>
                building.GetType().GetProperties()
                    .Any(prop =>
                        prop.GetValue(building)?.ToString()?.ToLower().Contains(searchTerm.ToLower()) ?? false
                    )
            )
            .ToList();

        _buildingGrid.ItemsSource = filteredList;
        _buildingGrid.Items.Refresh();
    }
}
