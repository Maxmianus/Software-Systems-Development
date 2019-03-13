using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace QuickShop
{
    /// <summary>
    /// Interaction logic for LogsWindow.xaml
    /// </summary>
    public partial class LogsWindow : Window
    {
        ObservableCollection<Order> storedOrders = new ObservableCollection<Order>();
        XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Order>));
        string Path = "LogsList.xml";

        public LogsWindow(ObservableCollection<Order> passedList)
        {
            InitializeComponent();
            ReadFromFile(Path);
            AddtoLogs(passedList);
            WriteToFile(Path);
        }

        public void AddtoLogs(ObservableCollection<Order> passedList)
        {
            foreach (var ord in storedOrders)
            {
                ListViewText.Items.Add(ord);
            }

            foreach (var ord in passedList)
            {
                ListViewText.Items.Add(ord);
                storedOrders.Add(ord);
            }

        }


        //Function to Write to XML file
        private void WriteToFile(string path)
        {
            if (storedOrders.Count == 0 && File.Exists(path))
            {
                File.Delete(path);
            }
            else
            {
                //Changed FileAcess to .Write from .ReadandWrite
                using (FileStream filestream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    serializer.Serialize(filestream, storedOrders);
                }
            }
        }

        //Read from item file
        private void ReadFromFile(string path)
        {
            if (File.Exists(path))
            {
                using (FileStream filestream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    storedOrders = serializer.Deserialize(filestream) as ObservableCollection<Order>;
                }
            }
        }

        private void ListViewText_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ListViewText.SelectedItems.Count > 0)
            {
                ListViewText.SelectedItems[0] = false;
            }
        }
    }
}
