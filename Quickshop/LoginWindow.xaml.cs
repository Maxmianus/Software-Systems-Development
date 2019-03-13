using System;
using System.Collections.Generic;
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

//import SQLite DB Runtime
using System.Data.SQLite;

namespace QuickShop
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        string dbConnectionString = @"Data Source=Database.db;";
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            SQLiteConnection conn = new SQLiteConnection(dbConnectionString);
            try
            {
                //Perform Auth
                if(conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }
                String loginQuery = "SELECT COUNT(1) FROM Accounts WHERE Username=@Username AND Password=@Password";
                SQLiteCommand connCMD = new SQLiteCommand(loginQuery, conn);
                connCMD.CommandType = System.Data.CommandType.Text;
                connCMD.Parameters.AddWithValue("@Username", userNameTextBox.Text);
                connCMD.Parameters.AddWithValue("@Password", passwordTextBox.Password);
                //Check Callback result
                int result = Convert.ToInt32(connCMD.ExecuteScalar());
                if(result == 1)
                {
                    String retrieveUserDataQuery = "SELECT * FROM Accounts WHERE Username=@Username AND Password=@Password";
                    SQLiteCommand retrieveCMD = new SQLiteCommand(retrieveUserDataQuery, conn);
                    retrieveCMD.CommandType = System.Data.CommandType.Text;
                    retrieveCMD.Parameters.AddWithValue("@Username", userNameTextBox.Text);
                    retrieveCMD.Parameters.AddWithValue("@Password", passwordTextBox.Password);
                    SQLiteDataReader rdr = retrieveCMD.ExecuteReader();
                    if (rdr.Read())
                    {
                        MainWindow dashboard = new MainWindow(Convert.ToString(rdr[0]), Convert.ToString(rdr[1]), Convert.ToString(rdr[2]));
                        dashboard.Show();
                        this.Close();
                    } else
                    {
                        MessageBox.Show("Database Error, Please check with the developer.");
                        this.Close();
                    }
                } else
                {
                    MessageBox.Show("Username of Password is incorrect.");
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            } finally
            {
                conn.Close();
            }
        }
    }
}
