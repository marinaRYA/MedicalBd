using PresenterLibrary.ClassBD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using PresenterLibrary;
using PresenterLibrary.Presenter;

public class PresenterWard : IPresenterCommon
{
    private Window _view;
    private DataGrid _wardGrid;
    private WardBd _wardBd;
    private string _connectionString;

    public PresenterWard(Window view, string connectionString)
    {
        _wardBd = new WardBd(connectionString);
        _view = view;
        _connectionString = connectionString;

        StackPanel gridPanel = _view.FindName("GridPanel") as StackPanel;
        gridPanel.Children.Clear();

        _wardGrid = new DataGrid();
        _wardGrid.Width = double.NaN;
        _wardGrid.CanUserAddRows = false;
        _wardGrid.AutoGenerateColumns = false;
        _wardGrid.IsReadOnly = true;
        _wardGrid.Name = "Палаты";

        DataGridTextColumn idColumn = new DataGridTextColumn();
        idColumn.Header = "ID";
        idColumn.Binding = new System.Windows.Data.Binding("ID");

        DataGridTextColumn numberColumn = new DataGridTextColumn();
        numberColumn.Header = "Номер";
        numberColumn.Binding = new System.Windows.Data.Binding("Number");

        DataGridTextColumn departmentNameColumn = new DataGridTextColumn();
        departmentNameColumn.Header = "Отделение";
        departmentNameColumn.Binding = new System.Windows.Data.Binding("DepartmentName");

        DataGridTextColumn bedCountColumn = new DataGridTextColumn();
        bedCountColumn.Header = "Количество коек";
        bedCountColumn.Binding = new System.Windows.Data.Binding("BedCount");

        _wardGrid.Columns.Add(idColumn);
        _wardGrid.Columns.Add(numberColumn);
        _wardGrid.Columns.Add(departmentNameColumn);
        _wardGrid.Columns.Add(bedCountColumn);

        _wardGrid.ItemsSource = _wardBd.WardList;
        gridPanel.Children.Add(_wardGrid);
    }

    public void AddObject()
    {
        AddWindowWard addWindow = new AddWindowWard(new WardBd.Ward(), _connectionString);
        if (addWindow.ShowDialog() == true)
        {
            _wardBd.AddWard(addWindow.Ward);
          //  MessageBox.Show("Ward successfully added", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            _wardGrid.ItemsSource = _wardBd.WardList;
            _wardGrid.Items.Refresh();
        }
    }

    public void EditObject()
    {
        if (_wardGrid.SelectedItem != null)
        {
            int index = _wardGrid.SelectedIndex;
            WardBd.Ward selectedWard = _wardBd.WardList[index];
            AddWindowWard editWindow = new AddWindowWard(selectedWard, _connectionString);
            if (editWindow.ShowDialog() == true)
            {
                _wardBd.UpdateWard(editWindow.Ward);
                //         MessageBox.Show("Surgery successfully updated", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                _wardGrid.ItemsSource = _wardBd.WardList;
                _wardGrid.Items.Refresh();
            }
        }
    }

    public void DeleteObject()
    {
        if (_wardGrid.SelectedItem != null)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить запись?", "Подтвердить", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    int index = _wardGrid.SelectedIndex;
                    WardBd.Ward selectedWard = _wardBd.WardList[index];

                    _wardBd.DeleteWard(selectedWard.ID);
                    _wardGrid.ItemsSource = _wardBd.WardList;
                    _wardGrid.Items.Refresh();
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
        List<WardBd.Ward> filteredList = _wardBd.WardList
            .Where(ward =>
                ward.GetType().GetProperties()
                    .Any(prop =>
                        prop.GetValue(ward)?.ToString()?.ToLower().Contains(searchTerm.ToLower()) ?? false
                    )
            )
            .ToList();

        _wardGrid.ItemsSource = filteredList;
        _wardGrid.Items.Refresh();
    }
}
