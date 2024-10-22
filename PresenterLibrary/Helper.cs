using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OfficeOpenXml;
using PresenterLibrary;
using static PresenterLibrary.ClassBD.PatientAccountBd;
using Xceed.Document.NET;
using Xceed.Words.NET;
using PresenterLibrary.ClassBD;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Data;
using PresenterLibrary.Presenter;
using static PresenterLibrary.ClassBD.AppointmentBd;
using static DepartmentBd;
using static LaboratoryBd;
using static PresenterLibrary.ClassBD.BuildingBd;
using static PresenterLibrary.ClassBD.DoctorBd;
using static PresenterLibrary.ClassBD.MedicalFacilityBd;
using static PresenterLibrary.ClassBD.PatientBd;
using static PresenterLibrary.ClassBD.SurgeryBd;
using static ProfileLabBd;
using static SpecializationBd;
using static StaffBd;
using static WardBd;
using System.Data.SQLite;
namespace PresenterLibrary
{
    public class Helper
    {
        private static Window _view;
        private List<PatientAccount> patientAccounts;
        private StackPanel gridPanel;
        string bd;
        string connectionString;
        public Helper(Window view, string BD)
        {
            connectionString = $"Data Source={BD};Version=3;";
            bd = BD;
            _view = view;
            gridPanel = _view.FindName("GridPanel") as StackPanel;
        }
        public void Setting()
        {
            ChoiceUser choice = new ChoiceUser(connectionString);
            if (choice.ShowDialog() == true)
            {
                SettingWindow settingWindow = new SettingWindow(bd, choice.username);
                settingWindow.Show();
            }
        }
        public void Reference()
        {
            Reference about = new Reference();
            about.Show();
        }
        public void About()
        {
            AboutWindow about = new AboutWindow();
            about.Show();
        }
        public void ChangePassword()
        {

            string variableName = "usernameID";


            FieldInfo field = _view.GetType().GetField(variableName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field != null)
            {

                object fieldValue = field.GetValue(_view);

                if (fieldValue is int)
                {
                    ChangePasswordWindow passwordWindow = new ChangePasswordWindow(connectionString, (int)fieldValue);
                    passwordWindow.Show();
                }


            }
            else MessageBox.Show("Произошла ошибка");

        }
        public bool ShowPatientPreview()
        {
            List<string> columnHeaders = new List<string>
    {
        "ФИО",              // Имя пациента
        "Дата рождения",     // Дата рождения
        "Пол",               // Пол
        "Полис",             // Полис
        "История болезни"    // История болезни
    };

            string query = @"
        SELECT 
            p.Name AS 'ФИО',
            p.Birth_Date AS 'Дата рождения',
            p.Gender AS 'Пол',
            p.Polis AS 'Полис',
            pa.MedicalHistory AS 'История болезни'
        FROM 
            Patient p
        LEFT JOIN 
            PatientAccount pa ON p.ID = pa.Patient_Id;";

            var patients = ExecuteSQLiteQuery(query);
            var previewData = GetPreviewDataFromSql(patients, columnHeaders);
            var previewWindow = new PreviewWindow(previewData, "Пациенты");
            return previewWindow.ShowDialog() == true;
        }

        public bool ShowHospitalPolyclinicPreview()
        {
            List<string> columnHeaders = new List<string>
    {
        "Название больницы",
        "Адрес",
        "Прикрепленная поликлиника",
        "Профили прикрепленных лабораторий" 
    };

            string query = @"
    SELECT 
        mf.Name AS 'Название больницы',
        mf.Address AS 'Адрес',
        pc.Name AS 'Прикрепленная поликлиника', 
        GROUP_CONCAT(pl.ProfileLabName, ', ') AS 'Профили прикрепленных лабораторий'  
    FROM 
        MedicalFacility mf
    LEFT JOIN 
        MedicalFacility pc ON mf.ID = pc.Parent_Hospital_ID
    LEFT JOIN 
        Laboratory_HospitalClinic lh ON lh.HospitalClinic_ID = mf.ID
    LEFT JOIN 
        Laboratory l ON l.ID = lh.Laboratory_ID
    LEFT JOIN 
        ProfileLab pl ON l.ProfileID = pl.ID  
    WHERE 
        mf.Type = 'Больница'
    GROUP BY 
        mf.ID, pc.ID;";  

            var hospitalPolyclinicLaboratories = ExecuteSQLiteQuery(query);
            var previewData = GetPreviewDataFromSql(hospitalPolyclinicLaboratories, columnHeaders);
            var previewWindow = new PreviewWindow(previewData, "Больницы, поликлиники и лаборатории");
            return previewWindow.ShowDialog() == true;
        }
      






        private List<Dictionary<string, object>> ExecuteSQLiteQuery(string query)
        {
            var results = new List<Dictionary<string, object>>();
            using (var connection = new SQLiteConnection(connectionString)) // Ensure connectionString is defined
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.GetValue(i);
                            }
                            results.Add(row);
                        }
                    }
                }
            }
            return results;
        }

        // Универсальный метод для преобразования данных SQL-запроса в List<Dictionary<string, string>>
        private List<Dictionary<string, string>> GetPreviewDataFromSql(List<Dictionary<string, object>> sqlData, List<string> columnHeaders)
        {
            var previewData = new List<Dictionary<string, string>>();
            foreach (var row in sqlData)
            {
                var rowData = new Dictionary<string, string>();
                foreach (var columnHeader in columnHeaders)
                {
                    var value = row.ContainsKey(columnHeader) ? row[columnHeader] : null;
                    rowData[columnHeader] = value?.ToString() ?? string.Empty;
                }
                previewData.Add(rowData);
            }
            return previewData;
        }

    }

}
