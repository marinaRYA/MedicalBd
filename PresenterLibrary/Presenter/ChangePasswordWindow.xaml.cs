using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SQLite;
using System.Security.Cryptography;

namespace Tools
{
    /// <summary>
    /// Логика взаимодействия для ChangePasswordWindow.xaml
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {
        private string ConnectionString;
        private int userId;
        public ChangePasswordWindow(string nameBd, int userId)
        {
            ConnectionString = $"Data Source={nameBd};Version=3;";
            this.userId = userId;
            InitializeComponent();
        }
        private void ResetClick(object sender, RoutedEventArgs e)
        {
            Reset();
        }
        public void Reset()
        {
            
            PasswordBoxFirst.Password = "";
            PasswordBoxConfirm.Password = "";
        }
        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
        public string CalculateMD5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }
        private void Submit()
        {
            try
            {
               
                  
                    string newPassword = PasswordBoxFirst.Password;
                    if (PasswordBoxFirst.Password.Length == 0)
                    {
                        errormessage.Text = "Введите новый пароль.";
                        PasswordBoxFirst.Focus();
                    }
                    else if (PasswordBoxConfirm.Password.Length == 0)
                    {
                        errormessage.Text = "Введите пароль заново.";
                        PasswordBoxConfirm.Focus();
                    }
                    else if (PasswordBoxFirst.Password != PasswordBoxConfirm.Password)
                    {
                        MessageBox.Show("Пароли не совпадают.");
                        PasswordBoxConfirm.Focus();
                    }
                    else
                    {
                        using (var connection = new SQLiteConnection(ConnectionString))
                        {
                            connection.Open();
                            using (SQLiteCommand command = new SQLiteCommand(connection))
                            {
                                command.CommandText = "UPDATE Users SET Password = @Password WHERE User_ID = @UserId";
                                command.Parameters.AddWithValue("@UserId", userId);
                                command.Parameters.AddWithValue("@Password", CalculateMD5Hash(newPassword));

                                object result = command.ExecuteScalar();
                                connection.Close();
                            }
                            MessageBox.Show("Пароль обновлен");
                            Close();
                        }
                    }
                
            }
            catch (Exception)
            {

                MessageBox.Show("Ошибка при обновлении");
            }
        }
        private void SubmitClick(object sender, RoutedEventArgs e)
        {
            Submit();
        }
    }
}

