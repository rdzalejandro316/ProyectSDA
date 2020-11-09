using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
//using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9684,"ConsultaDocumento");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9684, "ConsultaDocumento");
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();   


    public partial class ConsultaDocumento : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        int modulo = 1;

        public ConsultaDocumento()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            LoadConfig();
        }


        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Consulta Documento Contable";


                DataTable dt = SiaWin.Func.SqlDT("select rtrim(cod_trn) as cod_trn,rtrim(cod_trn)+'-'+rtrim(nom_trn) as nom_trn from comae_trn order by cod_trn", "tabla", idemp);
                CBtrn.ItemsSource = dt.DefaultView;

            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void CBtrn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                cabeza();
            }
            catch (Exception w)
            {
                MessageBox.Show("error al seleccionar :" + w);
            }
        }


        private void Fecha_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (CBtrn.SelectedIndex >= 0) cabeza();
            }
            catch (Exception w)
            {
                MessageBox.Show("error al seleccionar :" + w);
            }
        }


        public async void cabeza()
        {
            try
            {

                this.IsEnabled = false;

                int Year = Convert.ToDateTime(Tx_ano.Value).Year;
                int Month = Convert.ToDateTime(Tx_periodo.Value).Month;
                string periodo = Month >= 10 ? Month.ToString() : "0" + Month.ToString();
                string trn = CBtrn.SelectedValue.ToString();

                string cab = "select * from cocab_doc where ano_doc='" + Year + "' and  per_doc='" + periodo + "' and cod_trn='" + trn + "'; ";


                var slowTask = Task<DataTable>.Factory.StartNew(() => LoadData(cab));
                await slowTask;

                if (((DataTable)slowTask.Result).Rows.Count > 0)
                {
                    DataGridCabeza.ItemsSource = ((DataTable)slowTask.Result).DefaultView;
                    TxRegCab.Text = ((DataTable)slowTask.Result).Rows.Count.ToString();
                }
                else
                {
                    DataGridCabeza.ItemsSource = null;
                    DataGridCuerpo.ItemsSource = null;
                    TxRegCab.Text = "0";
                    TxRegCue.Text = "0";
                    TxDebito.Text = "0";
                    TxCredito.Text = "0";
                    TxCuenta.Text = "--------";
                    TxTercero.Text = "--------";
                }


                this.IsEnabled = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show("error al cargar la cabeza :" + ex);
            }
        }

        public async void cuerpo(string idreg)
        {
            try
            {

                this.IsEnabled = false;
                string cue = "select cue.cod_cta,cta.nom_cta,cue.cod_ciu,cue.cod_suc,cue.cod_cco,cue.cod_ter,ter.nom_ter,cue.des_mov,cue.num_chq,cue.doc_mov,cue.bas_mov,cue.deb_mov,cue.cre_mov,cue.doc_cruc,cue.cod_trn,cue.num_trn   ";
                cue += "from Cocue_doc cue ";
                cue += "left join Comae_cta cta on cta.cod_cta = cue.cod_cta ";
                cue += "left join Comae_ter ter on ter.cod_ter = cue.cod_ter ";
                cue += "where idregcab='" + idreg + "'; ";

                var slowTask = Task<DataTable>.Factory.StartNew(() => LoadData(cue));
                await slowTask;

                if (((DataTable)slowTask.Result).Rows.Count > 0)
                {
                    DataGridCuerpo.ItemsSource = ((DataTable)slowTask.Result).DefaultView;
                    TxRegCue.Text = ((DataTable)slowTask.Result).Rows.Count.ToString();
                    double deb_mov = Convert.ToDouble(((DataTable)slowTask.Result).Compute("Sum(deb_mov)", ""));
                    double cre_mov = Convert.ToDouble(((DataTable)slowTask.Result).Compute("Sum(cre_mov)", ""));
                    TxDebito.Text = deb_mov.ToString("N");
                    TxCredito.Text = cre_mov.ToString("N");
                }
                else
                {
                    DataGridCuerpo.ItemsSource = null;
                    TxRegCue.Text = "0";
                    TxDebito.Text = "0";
                    TxCredito.Text = "0";
                    TxCuenta.Text = "--------";
                    TxTercero.Text = "--------";
                }


                this.IsEnabled = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show("error al cargar la cabeza :" + ex);
            }
        }

        private DataTable LoadData(string query)
        {
            try
            {
                SqlConnection con = new SqlConnection(cnEmp);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataTable dt = new DataTable();
                cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.Text;
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(dt);
                con.Close();
                return dt;
            }
            catch (Exception e)
            {

                MessageBox.Show("en la consulta:" + e.Message);
                return null;
            }
        }

        private void DataGridCabeza_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            try
            {
                if (DataGridCabeza.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)DataGridCabeza.SelectedItems[0];
                    string idreg = row["idreg"].ToString().Trim();
                    cuerpo(idreg);
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al buscar el cuerpo:" + w);
            }
        }

        private void DataGridCuerpo_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            try
            {
                if (DataGridCuerpo.SelectedIndex >= 0)
                {

                    DataRowView row = (DataRowView)DataGridCuerpo.SelectedItems[0];
                    string nom_cta = row["nom_cta"].ToString().Trim();
                    string nom_ter = row["nom_ter"].ToString().Trim();

                    TxCuenta.Text = nom_cta;
                    TxTercero.Text = nom_ter;

                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar el cuerpo:" + w);
            }
        }

        private void BtnDocument_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DataGridCabeza.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)DataGridCabeza.SelectedItems[0];
                    int idreg = Convert.ToInt32(row["idreg"]);
                    SiaWin.TabTrn(0, idemp, true, idreg, modulo, WinModal: true);

                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrri el documento:" + w);
            }
        }




    }
}
