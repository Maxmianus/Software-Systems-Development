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
    /// Interaction logic for InsertOrder.xaml
    /// </summary>
    public partial class InsertOrder : Window
    {
        //List of Food Departments
        List<String> FoodDepartments = new List<String>
        {
            "General Groceries", "Deli", "Baked Goods", "Medicine", "Seafood"
        };

        string dbConnectionString = @"Data Source=Database.db;";
        String CreatorID;
        public InsertOrder(String userID)
        {
            InitializeComponent();
            CreatorID = userID;

            //Setup Date Picker
            DateTime Today = DateTime.Now;
            CalendarDateRange cdr = new CalendarDateRange(DateTime.MinValue, Today.AddDays(-1));
            DeliveryTimeDatePicker.BlackoutDates.Add(cdr);

            //Adds departments to combobox
            AddDepartments();
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            DateTime? date = DeliveryTimeDatePicker.SelectedDate;
            String Delivery;
            SQLiteConnection conn = new SQLiteConnection(dbConnectionString);
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                conn.Open();
            }
            //Delivery Option Check
            if (deliveryButton.IsChecked == true)
            {
                Delivery = "Yes";
            } else
            {
                Delivery = "No";
            }
            //Input Check
            if (String.IsNullOrWhiteSpace(fNameTextBox.Text) || String.IsNullOrWhiteSpace(lNameTextBox.Text)
                || String.IsNullOrWhiteSpace(phoneTextBox.Text) || String.IsNullOrWhiteSpace(emailTextBox.Text)
                || String.IsNullOrWhiteSpace(deptTextBox.Text) || String.IsNullOrWhiteSpace(orderNameTextBox.Text)
                || String.IsNullOrWhiteSpace(addressTextBox.Text) || date == null)
            {
                MessageBox.Show("No Empty Fields!");
            }  else
            {
                //write op
                try
                {
                    //Write New Order into database
                    String query = "INSERT INTO Orders (FIRSTNAME, LASTNAME, PHONE, EMAIL, FOODDEPT, ORDERNAME, DELIVERY, ADDRESS, DELIVERYTIME, INSTRUCTIONS, CREATORID)" +
                        " VALUES (@FirstName, @LastName, @Phone, @Email, @FoodDept, @OrderName, @Delivery, @Address, @DeliveryTime, @Instructions, @CreatorID)";
                    SQLiteCommand connCMD = new SQLiteCommand(query, conn);
                    connCMD.CommandType = System.Data.CommandType.Text;
                    connCMD.Parameters.AddWithValue("@FirstName", fNameTextBox.Text);
                    connCMD.Parameters.AddWithValue("@LastName", lNameTextBox.Text);
                    connCMD.Parameters.AddWithValue("@Phone", phoneTextBox.Text);
                    connCMD.Parameters.AddWithValue("@Email", emailTextBox.Text);
                    connCMD.Parameters.AddWithValue("@FoodDept", deptTextBox.Text);
                    connCMD.Parameters.AddWithValue("@OrderName", orderNameTextBox.Text);
                    connCMD.Parameters.AddWithValue("@Delivery", Delivery);
                    connCMD.Parameters.AddWithValue("@Address", addressTextBox.Text);
                    connCMD.Parameters.AddWithValue("@DeliveryTime", date.Value.ToShortDateString());
                    connCMD.Parameters.AddWithValue("@Instructions", instructionTextBox.Text);
                    connCMD.Parameters.AddWithValue("@CreatorID", CreatorID);
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
                    MessageBox.Show("New Order has been created.");
                    conn.Close();
                    this.Close();
                }
            }
        }

        private void AddDepartments()
        {
            foreach (var dep in FoodDepartments)
            {
                deptTextBox.Items.Add(dep);
            }
        }
    }
}
