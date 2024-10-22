using PresenterLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static ProfileLabBd;

public class PresenterProfileLab : IPresenterCommon
{
    private Window _view;
    private DataGrid _profileLabGrid;
    private ProfileLabBd _profileLabBd;
    private string _connectionString;

    public PresenterProfileLab(Window view, string connectionString)
    {
        _view = view;
        _connectionString = connectionString;
        _profileLabBd = new ProfileLabBd(_connectionString);

        StackPanel gridPanel = _view.FindName("GridPanel") as StackPanel;
        gridPanel.Children.Clear();

        _profileLabGrid = new DataGrid
        {
            Width = double.NaN,
            CanUserAddRows = false,
            AutoGenerateColumns = false,
            IsReadOnly = true,
            Name = "Профили лабораторий"
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

        _profileLabGrid.Columns.Add(idColumn);
        _profileLabGrid.Columns.Add(nameColumn);

        // Set the item source to the ProfileLab list
        _profileLabGrid.ItemsSource = _profileLabBd.ProfileLabList;
        gridPanel.Children.Add(_profileLabGrid);
    }

    public void AddObject()
    {
        string profileLabName = GetProfileLabNameFromUser("Введите название профиля лабораторий:");

        if (!string.IsNullOrEmpty(profileLabName))
        {
            ProfileLab newProfileLab = new ProfileLab
            {
                Name = profileLabName
            };

            _profileLabBd.AddProfileLab(newProfileLab);
            _profileLabGrid.ItemsSource = _profileLabBd.ProfileLabList;
            _profileLabGrid.Items.Refresh();
        }
    }

    public void EditObject()
    {
        if (_profileLabGrid.SelectedItem != null)
        {
            int index = _profileLabGrid.SelectedIndex;
            ProfileLab selectedProfileLab = _profileLabBd.ProfileLabList[index];

            string newProfileLabName = GetProfileLabNameFromUser("Введите новое название профиля лабораторий:", selectedProfileLab.Name);

            if (!string.IsNullOrEmpty(newProfileLabName))
            {
                selectedProfileLab.Name = newProfileLabName;
                _profileLabBd.UpdateProfileLab(selectedProfileLab);
                _profileLabGrid.ItemsSource = _profileLabBd.ProfileLabList;
                _profileLabGrid.Items.Refresh();
            }
        }
    }

    public void DeleteObject()
    {
        if (_profileLabGrid.SelectedItem != null)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить запись?", "Подтвердить", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    int index = _profileLabGrid.SelectedIndex;
                    ProfileLabBd.ProfileLab selectedProfileLab = _profileLabBd.ProfileLabList[index];

                    _profileLabBd.DeleteProfileLab(selectedProfileLab.ID);
               //    MessageBox.Show("ProfileLab successfully deleted", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    _profileLabGrid.ItemsSource = _profileLabBd.ProfileLabList;
                    _profileLabGrid.Items.Refresh();
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
        List<ProfileLabBd.ProfileLab> filteredList = _profileLabBd.ProfileLabList
            .Where(profileLab =>
                profileLab.Name.ToLower().Contains(searchTerm.ToLower())
            )
            .ToList();

        _profileLabGrid.ItemsSource = filteredList;
        _profileLabGrid.Items.Refresh();
    }

    private string GetProfileLabNameFromUser(string prompt, string defaultValue = "")
    {
        return Microsoft.VisualBasic.Interaction.InputBox(prompt, "User Input", defaultValue);
    }
}
