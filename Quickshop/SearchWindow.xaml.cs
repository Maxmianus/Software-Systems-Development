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
using System.Windows.Shapes;

namespace QuickShop
{
    /// <summary>
    /// Interaction logic for SearchWindow.xaml
    /// </summary>
    public partial class SearchWindow : Window
    {
        ObservableCollection<Order> orderList;
        int select; //Selects which search to perform

        public SearchWindow(ObservableCollection<Order> PassedList)
        {
            InitializeComponent();
            orderList = PassedList;
            select = 3; //set to 3 so error message shows if no select happens
        }

        //Search button
        private void searchbtn_Click(object sender, RoutedEventArgs e)
        {
            bool found = false;

            if(Validate())
            {
                foreach (var order in orderList)
                {
                    //Searches by name
                    if (select == 0)
                    {
                        if (FirstNameText.Text.Equals(order.Firstname) && LastNameText.Text.Equals(order.Lastname))
                        {
                            DeleteOrder newDelete = new DeleteOrder(order, orderList);

                            found = true;
                            this.Hide();
                            newDelete.ShowDialog();
                            this.Show();
                            break;
                        }
                    }
                    //Searchs by Phone number
                    else if (select == 1)
                    {
                        if (PhoneNumberText.Text.Equals(order.Phone))
                        {
                            DeleteOrder newDelete = new DeleteOrder(order, orderList);

                            found = true;
                            this.Hide();
                            newDelete.ShowDialog();
                            this.Show();
                            break;
                        }
                    }
                    //Searches by Email
                    else if (select == 2)
                    {
                        if (EmailText.Text.Equals(order.Email))
                        {
                            DeleteOrder newDelete = new DeleteOrder(order, orderList);

                            found = true;
                            this.Hide();
                            newDelete.ShowDialog();
                            this.Show();
                            break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid select");
                    }
                }

                if(!found)
                {
                    MessageBox.Show("Order not found");
                }
            }
        }

        //Checks to see if a valid entry was entered and what to search by, Name/Phone#/Email
        private bool Validate()
        {
            bool chk = false;

            //If first name and last name entered return
            if(!string.IsNullOrWhiteSpace(FirstNameText.Text) && !string.IsNullOrWhiteSpace(LastNameText.Text))
            {
                chk = true;
                select = 0; //Search by name
            }
            else if(!string.IsNullOrWhiteSpace(PhoneNumberText.Text)) //Checks if phone number is entered
            {
                chk = true;
                select = 1; //Search by phone number
            }
            else if(!string.IsNullOrWhiteSpace(EmailText.Text)) //Checks if email is entered
            {
                chk = true;
                select = 2; //Search by email
            }
            else
            {
                MessageBox.Show("Please enter a name, email, or phone number");
            }

            return chk;
        }

    }
}
