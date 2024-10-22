using PresenterLibrary;
using PresenterLibrary.Presenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

public class PresenterLaboratory : IPresenterCommon
{
    private Window _view;
    private DataGrid _laboratoryGrid;
    private LaboratoryBd _laboratoryBd;
    private string _connectionString;

    public PresenterLaboratory(Window view, string connectionString)
    {
        _view = view;
        _connectionString = connectionString;
        _laboratoryBd = new LaboratoryBd(_connectionString);

        StackPanel gridPanel = _view.FindName("GridPanel") as StackPanel;
        gridPanel.Children.Clear();

        _laboratoryGrid = new DataGrid
        {
            Width = double.NaN,
            CanUserAddRows = false,
            AutoGenerateColumns = false,
            IsReadOnly = true,
            Name = "Лаборатории"
        };

        // Create and add columns to the DataGrid
        DataGridTextColumn idColumn = new DataGridTextColumn
        {
            Header = "ID",
            Binding = new System.Windows.Data.Binding("ID")
        };
        DataGridTextColumn nameColumn = new DataGridTextColumn
        {
            Header = "Название",
            Binding = new System.Windows.Data.Binding("Name")
        };
        DataGridTextColumn profileColumn = new DataGridTextColumn
        {
            Header = "Профиль",
            Binding = new System.Windows.Data.Binding("Profile")
        };
        DataGridTextColumn HospitalColumn = new DataGridTextColumn
        {
            Header = "Больница",
            Binding = new System.Windows.Data.Binding("HospitalClinicName")
        };

        _laboratoryGrid.Columns.Add(idColumn);
        _laboratoryGrid.Columns.Add(nameColumn);
        _laboratoryGrid.Columns.Add(profileColumn);
        _laboratoryGrid.Columns.Add(HospitalColumn);


        // Set the item source to the Laboratory list
        _laboratoryGrid.ItemsSource = _laboratoryBd.LaboratoryList;
        gridPanel.Children.Add(_laboratoryGrid);
    }

    public void AddObject()
    {
        AddWindowLaboratory addWindow = new AddWindowLaboratory(new LaboratoryBd.Laboratory(), _connectionString);
        if (addWindow.ShowDialog() == true)
        {
            _laboratoryBd.AddLaboratory(addWindow.Laboratory);
         //   MessageBox.Show("Laboratory successfully added", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            _laboratoryGrid.ItemsSource = _laboratoryBd.LaboratoryList;
            _laboratoryGrid.Items.Refresh();
        }
    }

    public void EditObject()
    {
        if (_laboratoryGrid.SelectedItem != null)
        {
            int index = _laboratoryGrid.SelectedIndex;
            LaboratoryBd.Laboratory selectedLaboratory = _laboratoryBd.LaboratoryList[index];
            AddWindowLaboratory editWindow = new AddWindowLaboratory(selectedLaboratory, _connectionString);
            editWindow.Title = "Изменить лабораторию";
            if (editWindow.ShowDialog() == true)
            {
                _laboratoryBd.UpdateLaboratory(editWindow.Laboratory);
           //     MessageBox.Show("Laboratory successfully updated", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                _laboratoryGrid.ItemsSource = _laboratoryBd.LaboratoryList;
                _laboratoryGrid.Items.Refresh();
            }
        }
    }

    public void DeleteObject()
    {
        if (_laboratoryGrid.SelectedItem != null)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить запись?", "Подтвердить", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    int index = _laboratoryGrid.SelectedIndex;
                    LaboratoryBd.Laboratory selectedLaboratory = _laboratoryBd.LaboratoryList[index];

                    _laboratoryBd.DeleteLaboratory(selectedLaboratory.ID);
                  //  MessageBox.Show("Laboratory successfully deleted", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    _laboratoryGrid.ItemsSource = _laboratoryBd.LaboratoryList;
                    _laboratoryGrid.Items.Refresh();
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
        List<LaboratoryBd.Laboratory> filteredList = _laboratoryBd.LaboratoryList
            .Where(laboratory =>
                laboratory.Name.ToLower().Contains(searchTerm.ToLower()) ||
                laboratory.Profile.ToString().ToLower().Contains(searchTerm.ToLower())
            )
            .ToList();

        _laboratoryGrid.ItemsSource = filteredList;
        _laboratoryGrid.Items.Refresh();
    }
}
