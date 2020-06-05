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

namespace FoxPasarSql
{

    public partial class Totales : Window
    {
        public string TotIns = "";
        public string TotFall = "";

        public Totales()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TxInsert.Text = TotIns;
            TxFall.Text = TotFall;
        }


    }
}
