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

namespace FoxPasarSql
{   
    public partial class DATAGRID : Window
    {
        public DataTable dtFallidas;
        public string TotIns = "";
        public string TotFall = "";
        public DATAGRID()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                TxInsert.Text = TotIns;
                TxFall.Text = TotFall;

                GridFallidas.ItemsSource = dtFallidas.DefaultView;
                Tx_Total.Text = dtFallidas.Rows.Count.ToString();
            }
            catch (Exception w)
            {
                MessageBox.Show("error en el load:"+w);
            }
        }


    }
}
