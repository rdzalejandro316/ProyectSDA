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
    public partial class Movimientos : Window
    {

        dynamic SiaWin;
        public int idemp;
        
        public string cnemp;
        public string cod_act;
        public string fec_ini;
        public string fec_fin;


        int moduloaf = 8;
        public Movimientos()
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

                string fecha_ini = fec_ini;
                string fecha_fin = fec_fin;

                string activo = cod_act;

                var slowTask = Task<DataTable>.Factory.StartNew(() => LoadData(fecha_ini, fecha_fin, activo), source.Token);
                await slowTask;

                if (((DataTable)slowTask.Result).Rows.Count > 0)
                {

                    dataGridMov.ItemsSource = ((DataTable)slowTask.Result).DefaultView;
                    TxTotal.Text = ((DataTable)slowTask.Result).Rows.Count.ToString();

                    double vr_act = Convert.ToDouble(((DataTable)slowTask.Result).Compute("Sum(vr_act)", ""));
                    double dep_ac = Convert.ToDouble(((DataTable)slowTask.Result).Compute("Sum(dep_ac)", ""));
                    double mesxdep = Convert.ToDouble(((DataTable)slowTask.Result).Compute("Sum(mesxdep)", ""));

                    TxVract.Text = vr_act.ToString("N");
                    TxDepAct.Text = dep_ac.ToString("N2");
                    TxMesXdep.Text = mesxdep.ToString();
                }                                    
                else
                {
                    MessageBox.Show("el activo " + activo + " no contiene movimientos", "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    TxTotal.Text = "0";
                }
                sfBusyIndicator.IsBusy = false;

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar:" + w);
            }
        }

        private DataTable LoadData(string fec_ini, string fec_fin, string cod_act)
        {
            try
            {
                SqlConnection con = new SqlConnection(cnemp);
                con.Open();
                DataTable dt = new DataTable();

                StringBuilder query = new StringBuilder();
                query.Append("select afcab_doc.idreg,afcab_doc.cod_trn,afcab_doc.num_trn,afcab_doc.fec_trn,afcab_doc.des_mov,afcab_doc._usu, ");
                query.Append("sum(vr_act) as vr_act,sum(dep_ac) as dep_ac,sum(mesxdep) as mesxdep ");
                query.Append("from afcue_doc ");
                query.Append("inner join afcab_doc on afcab_doc.idreg = afcue_doc.idregcab ");
                query.Append("where afcue_doc.cod_act='" + cod_act + "' and   ");
                query.Append("convert(datetime,afcab_doc.fec_trn,103) between '" + fec_ini + "' and '" + fec_fin + "' ");
                query.Append("group by afcab_doc.idreg,afcab_doc.cod_trn,afcab_doc.num_trn,afcab_doc.fec_trn,afcab_doc.des_mov,afcab_doc._usu ");
        

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
