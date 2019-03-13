using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

//import SQLite DB
using System.Data.SQLite;

namespace QuickShop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string dbConnectionString = @"Data Source=Database.db;";
        //init User variable
        String userID;
        String userFirstName;
        String userLastName;
        //Binding list for all data in the database (Extra Step for LINQ)
        public ObservableCollection<Account> accountList { get; set; } = new ObservableCollection<Account>();
        public ObservableCollection<Order> orderList { get; set; } = new ObservableCollection<Order>();
        public ObservableCollection<Payment> currentAccountPaymentInfoList { get; set; } = new ObservableCollection<Payment>();

        public ObservableCollection<Order> logList { get; set; } = new ObservableCollection<Order>();

        public MainWindow(String id, String firstName, String lastName)
        {
            InitializeComponent();
            userID = id;
            userFirstName = firstName;
            userLastName = lastName;
            currentUserNameLabel.Content = "Welcome, " + userFirstName + " " + userLastName + ". User ID: " + userID;

            //Populate Lists
            populateAccountList(accountList);
            populateOrderList(orderList);
            populatePaymentInfoList(currentAccountPaymentInfoList);

            //Sort list by time
            IEnumerable<Order> sortedList =
            from Order in orderList
            orderby Order.Deliverytime
            select Order;

            //Sort Orders
            FindDeliveryOrders(sortedList);
            FindPickupOrders(sortedList);

        }

        //
        private void FindPickupOrders(IEnumerable<Order> sortList)
        {
            ObservableCollection<Order> SortedPickUpOrderList = new ObservableCollection<Order>();
            foreach (var order in sortList)
            {
                if (order.Delivery.Equals("No"))
                {
                    //deliveryOrders.Add(order);
                    SortedPickUpOrderList.Add(new Order { Firstname = order.Firstname, Lastname = order.Lastname, Ordername = order.Ordername, Deliverytime = order.Deliverytime, Phone = order.Phone, Email = order.Email });
                }
            }
            PickUpOrdersUC.DataContext = SortedPickUpOrderList;
        }

        //
        private void FindDeliveryOrders(IEnumerable<Order> sortList)
        {
            ObservableCollection<Order> SortedDeliveryOrderList = new ObservableCollection<Order>();
            foreach (var order in sortList)
            {
                if (order.Delivery.Equals("Yes"))
                {
                    //deliveryOrders.Add(order);
                    SortedDeliveryOrderList.Add(new Order { Firstname = order.Firstname, Lastname = order.Lastname, Ordername = order.Ordername, Deliverytime = order.Deliverytime, Phone = order.Phone, Address = order.Address, Email = order.Email });
                    
                }
            }
            DeliveryOrdersUC.DataContext = SortedDeliveryOrderList;
        }

        //List Population Methods
        //ORDERS TABLE
        private void populateOrderList(ObservableCollection<Order> list)
        {
            SQLiteConnection conn = new SQLiteConnection(dbConnectionString);
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                conn.Open();
            }
            try
            {
                String ordersQuery = "SELECT [Id],[Firstname],[Lastname],[Phone],[Email],[FoodDept],[Ordername],[Delivery],[Address],[Deliverytime],[Instructions],[CreatorID] FROM [Orders]";
                SQLiteCommand connCMD = new SQLiteCommand(ordersQuery, conn);
                connCMD.CommandType = System.Data.CommandType.Text;
                SQLiteDataReader reader = connCMD.ExecuteReader();
                while (reader.Read())
                {
                    int ID = reader.GetInt32(0);
                    String Firstname = reader.GetString(1);
                    String Lastname = reader.GetString(2);
                    String Phone = reader.GetString(3);
                    String Email = reader.GetString(4);
                    String FoodDept = reader.GetString(5);
                    String Ordername = reader.GetString(6);
                    String Delivery = reader.GetString(7);
                    String Address = reader.GetString(8);
                    String Deliverytime = reader.GetString(9);
                    String Instructions = reader.GetString(10);
                    String CreatorID = reader.GetString(11);

                    //insert into list
                    list.Add(new Order(ID, Firstname, Lastname, Phone, Email, FoodDept, Ordername, Delivery, Address, Deliverytime, Instructions, CreatorID));
                }
                conn.Close();
            } catch (Exception ex)
            {
                MessageBox.Show("Having Trouble Populating List." + ex.Message);
                conn.Close();
            }
        }

        //ACCOUNTS TABLE
        private void populateAccountList(ObservableCollection<Account> list)
        {
            SQLiteConnection conn = new SQLiteConnection(dbConnectionString);
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                conn.Open();
            }
            try
            {
                String ordersQuery = "SELECT [Id],[Firstname],[Lastname] FROM [Accounts]";
                SQLiteCommand connCMD = new SQLiteCommand(ordersQuery, conn);
                connCMD.CommandType = System.Data.CommandType.Text;
                SQLiteDataReader reader = connCMD.ExecuteReader();
                while (reader.Read())
                {
                    int ID = reader.GetInt32(0);
                    String Firstname = reader.GetString(1);
                    String Lastname = reader.GetString(2);

                    //insert into list
                    list.Add(new Account(ID, Firstname, Lastname));
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Having Trouble Populating List." + ex.Message);
                conn.Close();
            }
        }

        //PAYMENT_INFO TABLE
        private void populatePaymentInfoList(ObservableCollection<Payment> list)
        {
            SQLiteConnection conn = new SQLiteConnection(dbConnectionString);
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                conn.Open();
            }
            try
            {
                String ordersQuery = "SELECT [Account_Id],[Card_Type],[Card_Number],[CVV2_Code],[Expiration_Date] FROM [Payment_Info] " +
                    "WHERE [Account_Id] = @CurrentAccount";
                SQLiteCommand connCMD = new SQLiteCommand(ordersQuery, conn);
                connCMD.CommandType = System.Data.CommandType.Text;
                connCMD.Parameters.AddWithValue("@CurrentAccount", Int32.Parse(userID));
                SQLiteDataReader reader = connCMD.ExecuteReader();
                while (reader.Read())
                {
                    int ID = reader.GetInt32(0);
                    String cardType = reader.GetString(1);
                    String cardNumber = reader.GetString(2);
                    String cvv2Code = reader.GetString(3);
                    String expDate = reader.GetString(4);

                    //insert into list
                    list.Add(new Payment(ID, cardType, cardNumber, cvv2Code, expDate));
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Having Trouble Populating List." + ex.Message);
                conn.Close();
            }
        }

        private void LogOutBtn_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow newLogin = new LoginWindow();
            newLogin.Show();
            this.Close();
        }

        private void AddNewOrderItem_Click(object sender, RoutedEventArgs e)
        {
            InsertOrder newOrderWindow = new InsertOrder(userID);

            newOrderWindow.ShowDialog();

            //Repopulates list after new order is created
            orderList.Clear();
            populateOrderList(orderList);

            //Sort list by time
            IEnumerable<Order> sortedList =
            from Order in orderList
            orderby Order.Deliverytime
            select Order;

            //Display updated Orders
            FindDeliveryOrders(orderList);
            FindPickupOrders(orderList);

            this.Show();
        }

        //Search Button
        private void SearchOrDeleteOrderItem_Click(object sender, RoutedEventArgs e)
        {
            SearchWindow newSearchWindow = new SearchWindow(orderList);
            newSearchWindow.ShowDialog();
            this.Show();
        }

        private void AddNewEmployeeItem_Click(object sender, RoutedEventArgs e)
        {
            AddEmployee newEmployeeWindow = new AddEmployee();
            newEmployeeWindow.Show();
        }

        //Payment Info (New)
        private void PaymentInfoBtn_Click(object sender, RoutedEventArgs e)
        {
            PaymentInformation payInfoWindow = new PaymentInformation(userID, currentAccountPaymentInfoList);
            payInfoWindow.Show();
            populatePaymentInfoList(currentAccountPaymentInfoList);
        }

        //Confirmed Orders (New)
        private void LogsButton_Click(object sender, RoutedEventArgs e)
        {
            LogsWindow logsWin = new LogsWindow(logList);
            logsWin.ShowDialog();
        }

        //Confirm order (New)
        private void ConfirmOrder_Click(object sender, RoutedEventArgs e)
        {
            //DataBase Deletion
            //Create SQL connection
            SQLiteConnection conn = new SQLiteConnection(dbConnectionString);
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                conn.Open();
            }

            Order tmp;

            //Sort list by time
            IEnumerable<Order> sortedList =
            from Order in orderList
            orderby Order.Deliverytime
            select Order;

            switch (TabCollection.SelectedIndex)
            {
                case 0:
                    tmp = PickUpOrdersUC.ListViewText.SelectedItem as Order;
                    foreach(var ord in orderList)
                    {
                        if(tmp.Email.Equals(ord.Email))
                        {
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

                            logList.Add(tmp);
                            orderList.Remove(ord);
                            FindPickupOrders(sortedList);
                            break;
                        }
                    }
                    //PickUpOrdersUC.ListViewText.SelectedIndex //works
                    break;
                case 1:
                    tmp = DeliveryOrdersUC.ListViewText.SelectedItem as Order;

                    foreach (var ord in orderList)
                    {
                        if (tmp.Email.Equals(ord.Email))
                        {
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
                            
                            logList.Add(tmp);
                            orderList.Remove(ord);
                            FindDeliveryOrders(sortedList);
                            break;
                            
                        }
                    }
                    return;
            }
            conn.Close();
        }
    }

    //Constructors
    public class Account {

        //Empty Constructor
        public Account() { }

        //
        public Account(int ID, String Firstname, String Lastname)
        {
            this.ID = ID;
            this.Firstname = Firstname;
            this.Lastname = Lastname;
        }

        public int ID { get; set; }
        public String Firstname { get; set; }
        public String Lastname { get; set; }

    }

    public class Payment : Account
    {
        public Payment(int ID, String cardType, String cardNumber, String cvv2Code, String expDate)
        {
            this.ID = ID;
            this.cardType = cardType;
            this.cardNumber = cardNumber;
            this.cvv2Code = cvv2Code;
            this.expDate = expDate;
        }

        public String cardType { get; set; }
        public String cardNumber { get; set; }
        public String cvv2Code { get; set; }
        public String expDate { get; set; }
    }

    public class Order
    {

        //Empty Constructor
        public Order() { }

        //
        public Order(int ID, String Firstname, String Lastname, String Phone, String Email, String FoodDept, String Ordername,
            String Delivery, String Address, String Deliverytime, String Instructions, String CreatorID)
        {
            this.ID = ID;
            this.Firstname = Firstname;
            this.Lastname = Lastname;
            this.Phone = Phone;
            this.Email = Email;
            this.FoodDept = FoodDept;
            this.Ordername = Ordername;
            this.Delivery = Delivery;
            this.Address = Address;
            this.Deliverytime = Deliverytime;
            this.Instructions = Instructions;
            this.CreatorID = CreatorID;
        }

        public int ID { get; set; }
        public String Firstname { get; set; }
        public String Lastname { get; set; }
        public String Phone { get; set; }
        public String Email { get; set; }
        public String FoodDept { get; set; }
        public String Ordername { get; set; }
        public String Delivery { get; set; }
        public String Address { get; set; }
        public String Deliverytime { get; set; }
        public String Instructions { get; set; }
        public String CreatorID { get; set; }

    }

}
