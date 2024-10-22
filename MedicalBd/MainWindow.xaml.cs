using System.ComponentModel;
using System.Reflection.Emit;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;
using PresenterLibrary;
using System.Net;
using OfficeOpenXml;
namespace MedicalBd
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public int tag;
        public struct Root
        {
            public int R;
            public int W;
            public int E;
            public int D;
            public Root(int r, int w, int e, int d)
            {
                R = r;
                W = w;
                E = e;
                D = d;
            }
        }
        public string connectstring;
        public object presenter;
        public int usernameID;

        public Root root;
        MenuBuilder mn;
        public MainWindow()
        {
            AutorizationWindow autorization = new AutorizationWindow();
            if (autorization.ShowDialog() != true)
            {
                System.Windows.Application.Current.Shutdown();
                return;
            }
            connectstring = autorization.nameBd;
            usernameID = autorization.usernameID;
            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            InitializeComponent();
            mn = new MenuBuilder(this);
            MenuPanel.Children.Add(mn.menu);
            
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            root = mn.GetUserPermissions(tag);
            if (root.W == 1)
            {
                if (presenter is IPresenterCommon) ((IPresenterCommon)presenter).AddObject();
            }
            else MessageBox.Show("Данный пользователь не обладает правами администратора");
        }
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            root = mn.GetUserPermissions(tag);
            if (root.E == 1)
            {
                if (presenter is IPresenterCommon) ((IPresenterCommon)presenter).EditObject();
            }
            else MessageBox.Show("Данный пользователь не обладает правами администратора");
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            root = mn.GetUserPermissions(tag);
            if (root.D == 1)
            {
                if (presenter is IPresenterCommon) ((IPresenterCommon)presenter).DeleteObject();
            }
            else MessageBox.Show("Данный пользователь не обладает правами администратора");
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string search = searchTextBox.Text;
            if (presenter is IPresenterCommon)
            {
                if ((string.IsNullOrEmpty(search) || search != "")) ((IPresenterCommon)presenter).Search(search);
                else MessageBox.Show("Пустое значение");
            }
        }

    }
}