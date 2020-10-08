using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AfResumenActivos
{
    public partial class Saldos : Window
    {

        dynamic SiaWin;
        public int idemp;

        public string cnemp;
        public string cod_act;
        public string año;

        int moduloaf = 8;
        public Saldos()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
        }

        private void BtnDocumento_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridMov.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)dataGridMov.SelectedItems[0];
                    int idreg = Convert.ToInt32(row["idreg"]);
                    SiaWin.TabTrn(0, idemp, true, idreg, moduloaf, WinModal: true);

                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar documentos:" + w);
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SiaWin = Application.Current.MainWindow;
                idemp = SiaWin._BusinessId;

                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;

                sfBusyIndicator.IsBusy = true;

                string año_con = año;
                string activo = cod_act;

                var slowTask = Task<DataTable>.Factory.StartNew(() => LoadData(año_con, activo), source.Token);
                await slowTask;

                if (((DataTable)slowTask.Result).Rows.Count > 0)
                {
                    dataGridMov.ItemsSource = ((DataTable)slowTask.Result).DefaultView;
                    TxTotal.Text = ((DataTable)slowTask.Result).Rows.Count.ToString();
                }
                else
                {
                    MessageBox.Show("el activo " + activo + " no contiene saldos inicial", "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    TxTotal.Text = "0";
                }
                sfBusyIndicator.IsBusy = false;

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar:" + w);
            }
        }

        private DataTable LoadData(string año, string cod_act)
        {
            try
            {
                SqlConnection con = new SqlConnection(cnemp);
                con.Open();
                DataTable dt = new DataTable();
                StringBuilder query = new StringBuilder();
                query.Append("select acum.ano_acu,acum.cod_act,act.nom_act,acum.vr_act,acum.dep_ac,acum.mesxdep ");
                query.Append("from af_acum acum ");
                query.Append("inner join Afmae_act as act on act.cod_act = acum.cod_act ");
                query.Append("where acum.cod_act='" + cod_act + "' and acum.ano_acu='" + año + "' ");
                

                SqlCommand cmd = new SqlCommand(query.ToString(), con);
                dt.Load(cmd.ExecuteReader());
                con.Close();
                return dt;
            }
            catch (Exception e)
            {
                MessageBox.Show("error en la consulta:" + e);
                return null;
            }
        }






    }
}
