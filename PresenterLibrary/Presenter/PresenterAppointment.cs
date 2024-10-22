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
    public class PresenterAppointment : IPresenterCommon
    {
        private Window _view;
        private DataGrid _appointmentGrid;
        private AppointmentBd _appointmentBd;
        private string _connectionString;

        public PresenterAppointment(Window view, string connectionString)
        {
            _appointmentBd = new AppointmentBd(connectionString);
            _view = view;
            _connectionString = connectionString;

            StackPanel gridPanel = _view.FindName("GridPanel") as StackPanel;
            gridPanel.Children.Clear();

            _appointmentGrid = new DataGrid();
            _appointmentGrid.Width = double.NaN;
            _appointmentGrid.CanUserAddRows = false;
            _appointmentGrid.AutoGenerateColumns = false;
            _appointmentGrid.IsReadOnly = true;
            _appointmentGrid.Name = "Назначения";

            DataGridTextColumn idColumn = new DataGridTextColumn();
            idColumn.Header = "ID";
            idColumn.Binding = new System.Windows.Data.Binding("ID");

            DataGridTextColumn patientNameColumn = new DataGridTextColumn();
            patientNameColumn.Header = "ФИО Пациента";
            patientNameColumn.Binding = new System.Windows.Data.Binding("PatientName");

            DataGridTextColumn doctorNameColumn = new DataGridTextColumn();
            doctorNameColumn.Header = "ФИО, доктора, что назначил";
            doctorNameColumn.Binding = new System.Windows.Data.Binding("DoctorName");

          

            DataGridTextColumn dateColumn = new DataGridTextColumn();
            dateColumn.Header = "Дата";
            dateColumn.Binding = new System.Windows.Data.Binding("Date");

            DataGridTextColumn descriptionColumn = new DataGridTextColumn();
            descriptionColumn.Header = "Описание";
            descriptionColumn.Binding = new System.Windows.Data.Binding("Description");

            _appointmentGrid.Columns.Add(idColumn);
            _appointmentGrid.Columns.Add(patientNameColumn);
            _appointmentGrid.Columns.Add(doctorNameColumn);
            _appointmentGrid.Columns.Add(dateColumn);
            _appointmentGrid.Columns.Add(descriptionColumn);

            _appointmentGrid.ItemsSource = _appointmentBd.AppointmentList;
            gridPanel.Children.Add(_appointmentGrid);
        }

        public void AddObject()
        {
            AddWindowAppointment addWindow = new AddWindowAppointment(new AppointmentBd.Appointment(), _connectionString);
            if (addWindow.ShowDialog() == true)
            {
                _appointmentBd.AddAppointment(addWindow.Appointment);
                //MessageBox.Show("Appointment successfully added", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                _appointmentGrid.ItemsSource = _appointmentBd.AppointmentList;
                _appointmentGrid.Items.Refresh();
            }
        }

        public void EditObject()
        {
            if (_appointmentGrid.SelectedItem != null)
            {
                int index = _appointmentGrid.SelectedIndex;
                AppointmentBd.Appointment selectedAppointment = _appointmentBd.AppointmentList[index];
                AddWindowAppointment editWindow = new AddWindowAppointment(selectedAppointment, _connectionString);
                if (editWindow.ShowDialog() == true)
                {
                    _appointmentBd.UpdateAppointment(editWindow.Appointment);
                //    MessageBox.Show("Appointment successfully updated", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    _appointmentGrid.ItemsSource = _appointmentBd.AppointmentList;
                    _appointmentGrid.Items.Refresh();
                }
            }
        }

        public void DeleteObject()
        {
            if (_appointmentGrid.SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить запись?", "Подтвердить", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        int index = _appointmentGrid.SelectedIndex;
                        AppointmentBd.Appointment selectedAppointment = _appointmentBd.AppointmentList[index];

                        _appointmentBd.DeleteAppointment(selectedAppointment.ID);
                        _appointmentGrid.ItemsSource = _appointmentBd.AppointmentList;
                        _appointmentGrid.Items.Refresh();
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
            List<AppointmentBd.Appointment> filteredList = _appointmentBd.AppointmentList
                .Where(appointment =>
                    appointment.GetType().GetProperties()
                        .Any(prop =>
                            prop.GetValue(appointment)?.ToString()?.ToLower().Contains(searchTerm.ToLower()) ?? false
                        )
                )
                .ToList();

            _appointmentGrid.ItemsSource = filteredList;
            _appointmentGrid.Items.Refresh();
        }
    }
}
