using Basic_WPF_MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Basic_WPF_MVVM.ViewModel
{
    public class MyViewModel : INotifyPropertyChanged
    {
        private string connectionString;
        private SqlConnection connection;
        private User user;
        private ObservableCollection<UserFullName> _usersFN = new ObservableCollection<UserFullName>();
        public ObservableCollection<UserFullName> UsersFN
        {
            get { return _usersFN; }
            set
            {
                _usersFN = value;
                OnPropertyChange("UsersFN");
            }
        }

        public MyViewModel()
        {
            connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Administrador\\source\\repos\\Basic_WPF_MVVM\\Basic_WPF_MVVM\\DB.mdf;Integrated Security=True";
            this.connectDB();

            //usersFN = new ObservableCollection<UserFullName>();
            //this.LoadUsersVM();

            this.user = new User
            {
                FirstName = "John",
                LastName = "Doe",
                BirthDate = DateTime.Now.AddYears(-30),
            };
            CmdBoton = new RelayCommand(new Action<object>(CmdBotonAction));
        }

        public string FirstName
        {
            get { return user.FirstName; }
            set
            {
                if (user.FirstName != value)
                {
                    user.FirstName = value;
                    OnPropertyChange("FirstName");
                    OnPropertyChange("FullName");
                }
            }
        }

        public string LastName
        {
            get { return user.LastName; }
            set
            {
                if (user.LastName != value)
                {
                    user.LastName = value;
                    OnPropertyChange("LastName");
                    OnPropertyChange("FullName");
                }
            }
        }

        public int Age
        {
            get
            {
                DateTime today = DateTime.Today;
                int age = today.Year - user.BirthDate.Year;
                if (user.BirthDate > today.AddYears(-age)) age--;
                return age;
            }
        }

        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        private ICommand _CmdBoton;
        public ICommand CmdBoton
        {
            get
            {
                return _CmdBoton;
            }
            set
            {
                _CmdBoton = value;
            }
        }

        public void CmdBotonAction(object obj)
        {
            //List<string> par = obj.ToString().Split(',').ToList();
            //user.FirstName = par[0];
            //user.LastName = par[1];
            //OnPropertyChange("FirstName");
            //OnPropertyChange("LastName");
            //OnPropertyChange("FullName");

            string sql = "INSERT INTO [dbo].[User] ([FirstName], [LastName], [BirthDate]) " +
                        $"VALUES ('{user.FirstName}','{user.LastName}','{user.BirthDate.ToString("yyyyMMdd")}')";
            SqlCommand command = new SqlCommand(sql, connection);
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.InsertCommand = command;
            adapter.InsertCommand.ExecuteNonQuery();
            command.Dispose();

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region DataBase
        //Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Administrador\source\repos\Basic_WPF_MVVM\Basic_WPF_MVVM\DB.mdf;Integrated Security=True
        private void connectDB()
        {
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "connectDB", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        public List<UserFullName> LoadUsers()
        {
            List<UserFullName> users = new List<UserFullName>();
            string sql = "SELECT [FirstName] + ' ' + [LastName] AS FullName FROM [dbo].[User]";
            SqlCommand command = new SqlCommand(sql, connection);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                foreach (DbDataRecord record in reader)
                {
                    UserFullName usersFN = new UserFullName();
                    usersFN.FullName = record.GetString(0);
                    users.Add(usersFN);
                }
            }
            return users;
        }


        public void LoadUsersVM()
        {
            _usersFN.Clear();
            string sql = "SELECT [FirstName] + ' ' + [LastName] AS FullName FROM [dbo].[User]";
            SqlCommand command = new SqlCommand(sql, connection);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                foreach (DbDataRecord record in reader)
                {
                    UserFullName userFN = new UserFullName();
                    userFN.FullName = record.GetString(0);
                    _usersFN.Add(userFN);
                }
            }
        }


        #endregion
    }
}
