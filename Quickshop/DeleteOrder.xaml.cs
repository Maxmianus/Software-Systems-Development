using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

//SQL
using System.Data.SQLite;

namespace QuickShop
{
    /// <summary>
    /// Interaction logic for DeleteOrder.xaml
    /// </summary>
    public partial class DeleteOrder : Window
    {
        Order ord;
        ObservableCollection<Order> orderList;
        string dbConnectionString = @"Data Source=Database.db";

        public DeleteOrder(Order obj, ObservableCollection<Order> passedList)
        {
            InitializeComponent();
            ord = obj;
            orderList = passedList;
            Display();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            foreach(var order in orderList)
            {
                if(ord.Email.Equals(order.Email))
                {
                    //DataBase Deletion
                    //Create SQL connection
                    SQLiteConnection conn = new SQLiteConnection(dbConnectionString);
                    if (conn.State == System.Data.ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    try
                    {
                        //Query to be done
                        String query = "DELETE FROM [Orders] WHERE Id = @id";
                        //Create command and bind
                        SQLiteCommand command = new SQLiteCommand(query, conn);
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.AddWithValue("@id", ord.ID);
                        command.ExecuteNonQuery();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        conn.Close();
                    }
                    finally
                    {
                        conn.Close();
                    }

                    orderList.Remove(order);
                    MessageBox.Show("Successfully Deleted Order");
                    this.Close();
                    break;
                }
            }
        }

        private void Display()
        {
            foreach (var order in orderList)
            {
                if(ord.Email.Equals(order.Email))
                {
                    listBox.Items.Add(order.Firstname + " " + order.Lastname);
                    listBox.Items.Add(order.Ordername);
                    listBox.Items.Add(order.FoodDept);
                    listBox.Items.Add(order.Email);
                    listBox.Items.Add(order.Phone);
                    listBox.Items.Add(order.Instructions);

                }
            }
        }
    }
}
