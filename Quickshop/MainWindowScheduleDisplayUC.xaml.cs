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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;

namespace QuickShop
{
    /// <summary>
    /// Interaction logic for MainWindowScheduleDisplayUC.xaml
    /// </summary>
    public partial class MainWindowScheduleDisplayUC : UserControl
    {
        public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(
        nameof(ItemsSource), typeof(IEnumerable), typeof(MainWindowScheduleDisplayUC));

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public MainWindowScheduleDisplayUC()
        {
            InitializeComponent();
        }
    }
}
