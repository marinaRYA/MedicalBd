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
    public class PresenterDoctor : IPresenterCommon
    {
        private Window _view;
        private DataGrid _doctorGrid;
        private DoctorBd _doctorBd;
        private string _connectionString;

        public PresenterDoctor(Window view, string connectionString)
        {
            _doctorBd = new DoctorBd(connectionString);
            _view = view;
            _connectionString = connectionString;

            // Найти и очистить StackPanel для размещения DataGrid
            StackPanel gridPanel = _view.FindName("GridPanel") as StackPanel;
            gridPanel.Children.Clear();

            // Создать DataGrid для отображения данных докторов
            _doctorGrid = new DataGrid();
            _doctorGrid.Width = double.NaN;
            _doctorGrid.CanUserAddRows = false;
            _doctorGrid.AutoGenerateColumns = false;
            _doctorGrid.IsReadOnly = true;
            _doctorGrid.Name = "Доктора";

            // Определение столбцов для DataGrid
            DataGridTextColumn idColumn = new DataGridTextColumn();
            idColumn.Header = "ID";
            idColumn.Binding = new System.Windows.Data.Binding("ID");

            DataGridTextColumn nameColumn = new DataGridTextColumn();
            nameColumn.Header = "ФИО";
            nameColumn.Binding = new System.Windows.Data.Binding("Name");

            DataGridTextColumn specializationColumn = new DataGridTextColumn();
            specializationColumn.Header = "Специализация";
            specializationColumn.Binding = new System.Windows.Data.Binding("Specialization");

            DataGridTextColumn degreeColumn = new DataGridTextColumn();
            degreeColumn.Header = "Степень";
            degreeColumn.Binding = new System.Windows.Data.Binding("Degree");

            DataGridTextColumn titleColumn = new DataGridTextColumn();
            titleColumn.Header = "Звание";
            titleColumn.Binding = new System.Windows.Data.Binding("Title");

            DataGridTextColumn clinicColumn = new DataGridTextColumn();
            clinicColumn.Header = "Место работы";
            clinicColumn.Binding = new System.Windows.Data.Binding("HospitalClinic");

            DataGridTextColumn operationCountColumn = new DataGridTextColumn();
            operationCountColumn.Header = "Количество операций";
            operationCountColumn.Binding = new System.Windows.Data.Binding("OperationCount");

            DataGridTextColumn operationDeathCountColumn = new DataGridTextColumn();
            operationDeathCountColumn.Header = "Операции со смертным исходом";
            operationDeathCountColumn.Binding = new System.Windows.Data.Binding("OperationDeathCount");

            DataGridTextColumn hazardousWorkColumn = new DataGridTextColumn();
            hazardousWorkColumn.Header = "Коэффициент к зарплате за вредные условия труда";
            hazardousWorkColumn.Binding = new System.Windows.Data.Binding("HazardousWorkAllowance");

            DataGridTextColumn vacationDaysColumn = new DataGridTextColumn();
            vacationDaysColumn.Header = "Дни отпуска";
            vacationDaysColumn.Binding = new System.Windows.Data.Binding("VacationDays");

            DataGridTextColumn isConsultantColumn = new DataGridTextColumn();
            isConsultantColumn.Header = "Консультирует";
            isConsultantColumn.Binding = new System.Windows.Data.Binding("IsConsultant");

        
            _doctorGrid.Columns.Add(idColumn);
            _doctorGrid.Columns.Add(nameColumn);
            _doctorGrid.Columns.Add(specializationColumn);
            _doctorGrid.Columns.Add(degreeColumn);
            _doctorGrid.Columns.Add(titleColumn);
            _doctorGrid.Columns.Add(clinicColumn);
            _doctorGrid.Columns.Add(operationCountColumn);
            _doctorGrid.Columns.Add(operationDeathCountColumn);
            _doctorGrid.Columns.Add(hazardousWorkColumn);
            _doctorGrid.Columns.Add(vacationDaysColumn);
            _doctorGrid.Columns.Add(isConsultantColumn);

            // Установить источник данных для DataGrid
            _doctorGrid.ItemsSource = _doctorBd.DoctorList;

            // Добавить DataGrid на панель
            gridPanel.Children.Add(_doctorGrid);
        }

        
        public void AddObject()
        {
            AddDoctorWindow addWindow = new AddDoctorWindow(new DoctorBd.Doctor(), _connectionString);
            if (addWindow.ShowDialog() == true)
            {
                _doctorBd.AddDoctor(addWindow.Doctor);
             //  MessageBox.Show("Doctor successfully added", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                _doctorGrid.ItemsSource = _doctorBd.DoctorList;
                _doctorGrid.Items.Refresh();
            }
        }

        public void EditObject()
        {
            if (_doctorGrid.SelectedItem != null)
            {
                int index = _doctorGrid.SelectedIndex;
                DoctorBd.Doctor selectedDoctor = _doctorBd.DoctorList[index];
                AddDoctorWindow editWindow = new AddDoctorWindow(selectedDoctor, _connectionString);
                if (editWindow.ShowDialog() == true)
                {
                    _doctorBd.UpdateDoctor(editWindow.Doctor);
             //       MessageBox.Show("Doctor successfully updated", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    _doctorGrid.ItemsSource = _doctorBd.DoctorList;
                    _doctorGrid.Items.Refresh();
                }
            }
        }

        // Метод для удаления доктора
        public void DeleteObject()
        {
            if (_doctorGrid.SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить запись?", "Подтвердить", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        int index = _doctorGrid.SelectedIndex;
                        DoctorBd.Doctor selectedDoctor = _doctorBd.DoctorList[index];

                        _doctorBd.DeleteDoctor(selectedDoctor.ID);
                        _doctorGrid.ItemsSource = _doctorBd.DoctorList;
                        _doctorGrid.Items.Refresh();
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
            List<DoctorBd.Doctor> filteredList = _doctorBd.DoctorList
                .Where(doctor =>
                    doctor.GetType().GetProperties()
                        .Any(prop =>
                            prop.GetValue(doctor)?.ToString()?.ToLower().Contains(searchTerm.ToLower()) ?? false
                        )
                )
                .ToList();

            _doctorGrid.ItemsSource = filteredList;
            _doctorGrid.Items.Refresh();
        }
    }

}
