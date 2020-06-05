using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

namespace GeneracionPedidosProvedores
{
    public partial class Detalle : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";

        public string bodega = string.Empty;
        public string referencia = string.Empty;
        public string mesini = string.Empty;
        public string backorder = string.Empty;
        public string empresa = string.Empty;

        public Detalle()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();

                Cod_Ref.Text = referencia;
                Cod_Bod.Text = bodega;
                TXT_mesini.Text = mesini;
                TXT_backorder.Text = backorder;
                TXT_empresa.Text = empresa;

                Name_Ref.Text = referencia;
                Name_Ref2.Text = referencia;

                cargarConsulta();
            }
            catch (Exception)
            {
                MessageBox.Show("error al cargar el Load");
            }
        }




        public void cargarConsulta()
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("GeneracionPedidosProvedoresDETALLE", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@cod_bod", Cod_Bod.Text);
                cmd.Parameters.AddWithValue("@cod_ref", Cod_Ref.Text);
                cmd.Parameters.AddWithValue("@mesIni", TXT_mesini.Text);
                cmd.Parameters.AddWithValue("@fec_back", TXT_backorder.Text);
                cmd.Parameters.AddWithValue("@cod_empresa", TXT_empresa.Text);
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                con.Close();


                dataGridCxC.ItemsSource = ds.Tables[0];
                Total.Text = ds.Tables[0].Rows.Count.ToString();

                dataGridbackorder.ItemsSource = ds.Tables[1];
                Total2.Text = ds.Tables[1].Rows.Count.ToString();
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar la consulata" + w);
            }
        }





    }
}
