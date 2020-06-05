using System;
using System.Collections.Generic;
using System.Data;
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

namespace AnalisisDeCuentasPorPagar
{    
    public partial class BrowMini : Window
    {
        public DataTable dt = new DataTable();
        public BrowMini()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataGridCxC.ItemsSource = dt.DefaultView;
            
            if (dt.Rows.Count>0)            
                Tx_Total.Text = dt.Rows.Count.ToString();
            else            
                Tx_Total.Text = "0";                                     
        }





    }
}
