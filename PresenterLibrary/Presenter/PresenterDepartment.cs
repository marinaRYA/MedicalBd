using PresenterLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using PresenterLibrary.Presenter;

public class PresenterDepartment : IPresenterCommon
{
    private Window _view;
    private DataGrid _departmentGrid;
    private DepartmentBd _departmentBd;
    private string _connectionString;

    public PresenterDepartment(Window view, string connectionString)
    {
        _departmentBd = new DepartmentBd(connectionString);
        _view = view;
        _connectionString = connectionString;

        StackPanel gridPanel = _view.FindName("GridPanel") as StackPanel;
        gridPanel.Children.Clear();

        _departmentGrid = new DataGrid
        {
            Width = double.NaN,
            CanUserAddRows = false,
            AutoGenerateColumns = false,
            IsReadOnly = true,
            Name = "Отделения"
        };

        DataGridTextColumn idColumn = new DataGridTextColumn
        {
            Header = "ID",
            Binding = new System.Windows.Data.Binding("ID")
        };

        DataGridTextColumn nameColumn = new DataGridTextColumn
        {
            Header = "Отделение",
            Binding = new System.Windows.Data.Binding("Name")
        };

        DataGridTextColumn buildingNameColumn = new DataGridTextColumn
        {
            Header = "Корпус",
            Binding = new System.Windows.Data.Binding("BuildingName")
        };

        

        _departmentGrid.Columns.Add(idColumn);
        _departmentGrid.Columns.Add(nameColumn);
        _departmentGrid.Columns.Add(buildingNameColumn);
        //_departmentGrid.Columns.Add(specializationColumn);

        _departmentGrid.ItemsSource = _departmentBd.DepartmentList;
        gridPanel.Children.Add(_departmentGrid);
    }

    public void AddObject()
    {
        AddWindowDepartment addWindow = new AddWindowDepartment(new DepartmentBd.Department(), _connectionString);
        if (addWindow.ShowDialog() == true)
        {
            _departmentBd.AddDepartment(addWindow.Department);
        //    MessageBox.Show("Department successfully added", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            _departmentGrid.ItemsSource = _departmentBd.DepartmentList;
            _departmentGrid.Items.Refresh();
        }
    }

    public void EditObject()
    {
        if (_departmentGrid.SelectedItem != null)
        {
            int index = _departmentGrid.SelectedIndex;
            DepartmentBd.Department selectedDepartment = _departmentBd.DepartmentList[index];
            AddWindowDepartment editWindow = new AddWindowDepartment(selectedDepartment, _connectionString);
            if (editWindow.ShowDialog() == true)
            {
                _departmentBd.UpdateDepartment(editWindow.Department);
            //    MessageBox.Show("Department successfully updated", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                _departmentGrid.ItemsSource = _departmentBd.DepartmentList;
                _departmentGrid.Items.Refresh();
            }
        }
    }

    public void DeleteObject()
    {
        if (_departmentGrid.SelectedItem != null)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить запись?", "Подтвердить", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    int index = _departmentGrid.SelectedIndex;
                    DepartmentBd.Department selectedDepartment = _departmentBd.DepartmentList[index];

                    _departmentBd.DeleteDepartment(selectedDepartment.ID);
                    _departmentGrid.ItemsSource = _departmentBd.DepartmentList;
                    _departmentGrid.Items.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении записи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
    public void Search(string searchTerm)
    {
        List<DepartmentBd.Department> filteredList = _departmentBd.DepartmentList
            .Where(department =>
                department.GetType().GetProperties()
                    .Any(prop =>
                        prop.GetValue(department)?.ToString()?.ToLower().Contains(searchTerm.ToLower()) ?? false
                    )
            )
            .ToList();

        _departmentGrid.ItemsSource = filteredList;
        _departmentGrid.Items.Refresh();
    }
}
