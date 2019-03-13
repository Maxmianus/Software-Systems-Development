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
    /// Interaction logic for AddEmployee.xaml
    /// </summary>
    public partial class AddEmployee : Window
    {
        string dbConnectionString = @"Data Source=Database.db;";
        public AddEmployee()
        {
            InitializeComponent();
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            SQLiteConnection conn = new SQLiteConnection(dbConnectionString);
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                conn.Open();
            }
            //Input Check
            if (String.IsNullOrWhiteSpace(fNameTextBox.Text) || String.IsNullOrWhiteSpace(lNameTextBox.Text)
                || String.IsNullOrWhiteSpace(usernameTextBox.Text) || String.IsNullOrWhiteSpace(passwordTextBox.Text))
            {
                MessageBox.Show("No Empty Fields!");
            } else {
                //Duplicate Account Check
                String checkQuery = "SELECT COUNT(1) FROM Accounts WHERE Username=@Username";
                SQLiteCommand connCheckAccountCMD = new SQLiteCommand(checkQuery, conn);
                connCheckAccountCMD.CommandType = System.Data.CommandType.Text;
                connCheckAccountCMD.Parameters.AddWithValue("@Username", usernameTextBox.Text);
                if (Convert.ToInt32(connCheckAccountCMD.ExecuteScalar()) == 1)
                {
                    MessageBox.Show("This User Name is already taken.");
                }
                else
                {
                    //Write Op
                    try
                    {
                        //Write New Account into database
                        String query = "INSERT INTO Accounts (FIRSTNAME, LASTNAME, USERNAME, PASSWORD) VALUES (@FirstName, @LastName, @Username, @Password)";
                        SQLiteCommand connCMD = new SQLiteCommand(query, conn);
                        connCMD.CommandType = System.Data.CommandType.Text;
                        connCMD.Parameters.AddWithValue("@FirstName", fNameTextBox.Text);
                        connCMD.Parameters.AddWithValue("@LastName", lNameTextBox.Text);
                        connCMD.Parameters.AddWithValue("@Username", usernameTextBox.Text);
                        connCMD.Parameters.AddWithValue("@Password", passwordTextBox.Text);
                        connCMD.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        conn.Close();
                        this.Close();
                    }
                    finally
                    {
                        MessageBox.Show("The employee has been added into Database.");
                        conn.Close();
                        this.Close();
                    }
                }
            }
        }
    }
}
