using PresenterLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using PresenterLibrary.Presenter;

public class PresenterStaff : IPresenterCommon
{
    private Window _view;
    private DataGrid _staffGrid;
    private StaffBd _staffBd;
    private string _connectionString;

    public PresenterStaff(Window view, string connectionString)
    {
        _staffBd = new StaffBd(connectionString);
        _view = view;
        _connectionString = connectionString;

        StackPanel gridPanel = _view.FindName("GridPanel") as StackPanel;
        gridPanel.Children.Clear();

        _staffGrid = new DataGrid
        {
            Width = double.NaN,
            CanUserAddRows = false,
            AutoGenerateColumns = false,
            IsReadOnly = true,
            Name = "Персонал"
        };

        DataGridTextColumn idColumn = new DataGridTextColumn
        {
            Header = "ID",
            Binding = new System.Windows.Data.Binding("ID")
        };

        DataGridTextColumn nameColumn = new DataGridTextColumn
        {
            Header = "ФИО",
            Binding = new System.Windows.Data.Binding("Name")
        };

        DataGridTextColumn roleColumn = new DataGridTextColumn
        {
            Header = "Работа",
            Binding = new System.Windows.Data.Binding("Role")
        };

        DataGridTextColumn hospitalNameColumn = new DataGridTextColumn
        {
            Header = "Больница/Поликлиника",
            Binding = new System.Windows.Data.Binding("HospitalName")
        };

        DataGridTextColumn specializationNameColumn = new DataGridTextColumn
        {
            Header = "Специализация",
            Binding = new System.Windows.Data.Binding("SpecializationName")
        };

      

        _staffGrid.Columns.Add(idColumn);
        _staffGrid.Columns.Add(nameColumn);
        _staffGrid.Columns.Add(roleColumn);
        _staffGrid.Columns.Add(hospitalNameColumn);
        _staffGrid.Columns.Add(specializationNameColumn);
       

        _staffGrid.ItemsSource = _staffBd.StaffList;
        gridPanel.Children.Add(_staffGrid);
    }

    public void AddObject()
    {
        AddWindowStaff addWindow = new AddWindowStaff(new StaffBd.Staff(), _connectionString);
        if (addWindow.ShowDialog() == true)
        {
            _staffBd.AddStaff(addWindow.Staff);
        //    MessageBox.Show("Staff successfully added", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            _staffGrid.ItemsSource = _staffBd.StaffList;
            _staffGrid.Items.Refresh();
        }
    }

    public void EditObject()
    {
        if (_staffGrid.SelectedItem != null)
        {
            int index = _staffGrid.SelectedIndex;
            StaffBd.Staff selectedStaff = _staffBd.StaffList[index];
            AddWindowStaff editWindow = new AddWindowStaff(selectedStaff, _connectionString);
            if (editWindow.ShowDialog() == true)
            {
                _staffBd.UpdateStaff(editWindow.Staff);
        //        MessageBox.Show("Staff successfully updated", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                _staffGrid.ItemsSource = _staffBd.StaffList;
                _staffGrid.Items.Refresh();
            }
        }
    }

    public void DeleteObject()
    {
        if (_staffGrid.SelectedItem != null)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить запись?", "Подтвердить", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    int index = _staffGrid.SelectedIndex;
                    StaffBd.Staff selectedStaff = _staffBd.StaffList[index];

                    _staffBd.DeleteStaff(selectedStaff.ID);
                    _staffGrid.ItemsSource = _staffBd.StaffList;
                    _staffGrid.Items.Refresh();
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
        List<StaffBd.Staff> filteredList = _staffBd.StaffList
            .Where(staff =>
                staff.GetType().GetProperties()
                    .Any(prop =>
                        prop.GetValue(staff)?.ToString()?.ToLower().Contains(searchTerm.ToLower()) ?? false
                    )
            )
            .ToList();

        _staffGrid.ItemsSource = filteredList;
        _staffGrid.Items.Refresh();
    }
}
