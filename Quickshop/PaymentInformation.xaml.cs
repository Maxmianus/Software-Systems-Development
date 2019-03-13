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
using System.Data;
using System.Collections.ObjectModel;

//import SQLite DB Runtime
using System.Data.SQLite;

namespace QuickShop
{
    /// <summary>
    /// Interaction logic for PaymentInformation.xaml
    /// </summary>
    public partial class PaymentInformation : Window
    {
        DataTable dt = new DataTable();
        String account_ID;
        string dbConnectionString = @"Data Source=Database.db;";

        public PaymentInformation(String id, ObservableCollection<Payment> paymentInfoList)
        {
            InitializeComponent();
            dt.Columns.Add(new DataColumn("cardType", typeof(string)));
            dt.Columns.Add(new DataColumn("cardNumber", typeof(string)));
            dt.Columns.Add(new DataColumn("CVV2", typeof(string)));
            dt.Columns.Add(new DataColumn("expDate", typeof(string)));

            account_ID = id;
            //Inject from Database
            try
            {
                foreach (var element in paymentInfoList)
                {
                    var newRow = dt.NewRow();

                    // fill the properties into the cells
                    newRow["cardType"] = element.cardType;
                    newRow["cardNumber"] = element.cardNumber;
                    newRow["CVV2"] = element.cvv2Code;
                    newRow["expDate"] = element.expDate;

                    dt.Rows.Add(newRow);
                }

                // Do Datagridview
                dataGrid.ItemsSource = dt.DefaultView;
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this card information?", "Warning",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes) {
                try
                {
                    DataRowView dataRowView = (DataRowView)((Button)e.Source).DataContext;
                    String cardNumber = dataRowView[1].ToString();
                    dataRowView.Delete();

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
                        String query = "DELETE FROM [Payment_Info] WHERE Card_Number = @cardNumber";
                        //Create command and bind
                        SQLiteCommand command = new SQLiteCommand(query, conn);
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.AddWithValue("@cardNumber", cardNumber);
                        command.ExecuteNonQuery();

                    } catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        conn.Close();
                    } finally
                    {
                        conn.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void SubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            int dupCheck = 0;
            int checkInt;
            long checkInt64;
            //Duplicate Validation
            foreach (DataRow row in dt.Rows)
            {
                if(row["cardNumber"].ToString() == CardNumberTextBox.Text)
                {
                    dupCheck = 1;
                }
            }
            //Input Validation
            if (CardTypeComboBox.SelectedItem == null || String.IsNullOrWhiteSpace(CardNumberTextBox.Text) || String.IsNullOrWhiteSpace(CVV2CodeTextBox.Text)
                || String.IsNullOrWhiteSpace(ExpDateMonthTextBox.Text) || String.IsNullOrWhiteSpace(ExpDateYearTextBox.Text))
            {
                MessageBox.Show("Please Fill Up All Fields");
            } else if (!Int64.TryParse(CardNumberTextBox.Text, out checkInt64) || !int.TryParse(CVV2CodeTextBox.Text, out checkInt)
                || !int.TryParse(ExpDateMonthTextBox.Text, out checkInt) || !int.TryParse(ExpDateYearTextBox.Text, out checkInt))
            {
                MessageBox.Show("Card Number, CVV and Expiration Date Must Be Numbers.");
            } else if (CardNumberTextBox.Text.Length > 16 || ExpDateMonthTextBox.Text.Length > 2 || ExpDateYearTextBox.Text.Length > 2
                || CVV2CodeTextBox.Text.Length > 4)
            {
                MessageBox.Show("Card Number, CVV and Expiration Date Must Be in Correct Format.");
            } else if (int.TryParse(ExpDateMonthTextBox.Text, out checkInt) && Int32.Parse(ExpDateMonthTextBox.Text) >= 13)
            {
                MessageBox.Show("You only have 12 months in a Year.");
            }
            else if (dupCheck == 1)
            {
                MessageBox.Show("You already registered this card!");
            }
            else
            {
                ComboBoxItem typeItem = (ComboBoxItem)CardTypeComboBox.SelectedItem;
                string cardType = typeItem.Content.ToString();
                SQLiteConnection conn = new SQLiteConnection(dbConnectionString);
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }
                try
                {
                    //Store Into Database (Database Integration)
                    String query = "INSERT INTO Payment_Info (ACCOUNT_ID, CARD_TYPE, CARD_NUMBER, CVV2_CODE, EXPIRATION_DATE)" +
                        " VALUES (@Account_Id, @Card_Type, @Card_Number, @CVV2_Code, @Expiration_Date)";
                    SQLiteCommand connCMD = new SQLiteCommand(query, conn);
                    connCMD.CommandType = System.Data.CommandType.Text;
                    connCMD.Parameters.AddWithValue("@Account_Id", Int32.Parse(account_ID));
                    connCMD.Parameters.AddWithValue("@Card_Type", cardType);
                    connCMD.Parameters.AddWithValue("@Card_Number", CardNumberTextBox.Text);
                    connCMD.Parameters.AddWithValue("@CVV2_Code", CVV2CodeTextBox.Text);
                    connCMD.Parameters.AddWithValue("@Expiration_Date", ExpDateMonthTextBox.Text + "/" + ExpDateYearTextBox.Text);
                    connCMD.ExecuteNonQuery();

                    //DataGridView Data Injection
                    var newRow = dt.NewRow();
                    newRow["cardType"] = cardType;
                    newRow["cardNumber"] = CardNumberTextBox.Text;
                    newRow["CVV2"] = CVV2CodeTextBox.Text;
                    newRow["expDate"] = ExpDateMonthTextBox.Text + "/" + ExpDateYearTextBox.Text;

                    dt.Rows.Add(newRow);

                    MessageBox.Show("New Card Added.");
                    dataGrid.ItemsSource = dt.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    conn.Close();
                } finally
                {
                    conn.Close();
                }
            }
        }
    }
}
