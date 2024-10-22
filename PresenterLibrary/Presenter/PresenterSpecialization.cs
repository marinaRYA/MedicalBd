using PresenterLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static SpecializationBd;

public class PresenterSpecialization : IPresenterCommon
{
    private Window _view;
    private DataGrid _specializationGrid;
    private SpecializationBd _specializationBd;
    private string _connectionString;

    public PresenterSpecialization(Window view, string connectionString)
    {
        _view = view;
        _connectionString = connectionString;
        _specializationBd = new SpecializationBd(_connectionString);

        StackPanel gridPanel = _view.FindName("GridPanel") as StackPanel;
        gridPanel.Children.Clear();

        _specializationGrid = new DataGrid
        {
            Width = double.NaN,
            CanUserAddRows = false,
            AutoGenerateColumns = false,
            IsReadOnly = true,
            Name = "Специализации"
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

        _specializationGrid.Columns.Add(idColumn);
        _specializationGrid.Columns.Add(nameColumn);

        // Set the item source to the Specialization list
        _specializationGrid.ItemsSource = _specializationBd.SpecializationList;
        gridPanel.Children.Add(_specializationGrid);
    }

    public void AddObject()
    {
        string specializationName = GetSpecializationNameFromUser("Введите название специализации:");

        if (!string.IsNullOrEmpty(specializationName))
        {
            Specialization newSpecialization = new Specialization
            {
                Name = specializationName
            };

            _specializationBd.AddSpecialization(newSpecialization);
            _specializationGrid.ItemsSource = _specializationBd.SpecializationList;
            _specializationGrid.Items.Refresh();
        }
    }

    public void EditObject()
    {
        if (_specializationGrid.SelectedItem != null)
        {
            int index = _specializationGrid.SelectedIndex;
            Specialization selectedSpecialization = _specializationBd.SpecializationList[index];

            string newSpecializationName = GetSpecializationNameFromUser("Введите новое название специализации:", selectedSpecialization.Name);

            if (!string.IsNullOrEmpty(newSpecializationName))
            {
                selectedSpecialization.Name = newSpecializationName;
                _specializationBd.UpdateSpecialization(selectedSpecialization);
                _specializationGrid.ItemsSource = _specializationBd.SpecializationList;
                _specializationGrid.Items.Refresh();
            }
        }
    }

    public void DeleteObject()
    {
        if (_specializationGrid.SelectedItem != null)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить запись?", "Подтвердить", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    int index = _specializationGrid.SelectedIndex;
                    SpecializationBd.Specialization selectedSpecialization = _specializationBd.SpecializationList[index];

                    _specializationBd.DeleteSpecialization(selectedSpecialization.ID);
                 //   MessageBox.Show("Specialization successfully deleted", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    _specializationGrid.ItemsSource = _specializationBd.SpecializationList;
                    _specializationGrid.Items.Refresh();
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
        List<SpecializationBd.Specialization> filteredList = _specializationBd.SpecializationList
            .Where(specialization =>
                specialization.Name.ToLower().Contains(searchTerm.ToLower())
            )
            .ToList();

        _specializationGrid.ItemsSource = filteredList;
        _specializationGrid.Items.Refresh();
    }

    private string GetSpecializationNameFromUser(string prompt, string defaultValue = "")
    {
        return Microsoft.VisualBasic.Interaction.InputBox(prompt, "User Input", defaultValue);
    }
}
