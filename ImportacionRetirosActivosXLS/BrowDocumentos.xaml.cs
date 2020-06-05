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

namespace ImportacionRetirosActivosXLS
{    
    public partial class BrowDocumentos : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        public DataTable dt;
        public BrowDocumentos()
        {
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {            
            dataGrid.ItemsSource = dt;
            Tx_totales.Text = dt.Rows.Count.ToString();
        }

        private void BtnGetDocument_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                string tag = (sender as Button).Tag.ToString().Trim();

                string tabla = tag == "af" ? "afcab_doc" : "cocab_doc";
                int id = tag == "af" ? 8 : 1;

                DataRowView row = (DataRowView)dataGrid.SelectedItems[0];
                string cod_trn = tag == "af" ? row["cod_trn"].ToString() : row["COD_TDO"].ToString();
                string num_trn = row["num_trn"].ToString();

                System.Data.DataTable dt = SiaWin.Func.SqlDT("select * from "+ tabla + " where num_trn='" +num_trn+ "' and cod_trn='" + cod_trn+ "';", "tabla", idemp);
                if (dt.Rows.Count>0)
                {
                    int idreg = Convert.ToInt32(dt.Rows[0]["idreg"]);
                    if (idreg <= 0) return;
                    SiaWin.TabTrn(0, idemp, true, idreg, id, WinModal: true);                   
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("errro al abrir documento"+w);
            }
        }











    }
}
