using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace SiasoftAppExt
{
    /// <summary>
    /// Lógica de interacción para UserControl1.xaml
    /// </summary>
    public partial class InConsultaProductoBodega : Window
    {
        dynamic SiaWin;
        static int idEmp = 0;
        DataTable dt = new DataTable();
        private string idbod;
        public string idBod = "";
        public DataSet ds1 = new DataSet();
        private bool Salir = false;
        public string Conexion;
        public InConsultaProductoBodega()
        {
            SiaWin = Application.Current.MainWindow;
            InitializeComponent();
            TxtCodigo.Focus();
            idEmp = SiaWin._BusinessId;
        }
        private void TxtCodigo_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Salir == true) return;
            if (string.IsNullOrEmpty(TxtCodigo.Text.Trim()))
            {
                ///// nuevo- busqueda externa
                dynamic ww = SiaWin.WindowExt(9326, "InBuscarReferencia");  //carga desde sql
                ww.Conexion = SiaWin.Func.DatosEmp(idEmp);
                ww.idEmp = idEmp;
                ww.idBod = "";
                ww.UltBusqueda = "";
                ww.ShowInTaskbar = false;
                ww.Owner = Application.Current.MainWindow;
                ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                TxtCodigo.Text=ww.Codigo;
                TxtNombre.Text = ww.Nombre;
                ww.ShowDialog();
                TxtCodigo.Text = ww.Codigo.ToString().Trim();
                TxtNombre.Text = ww.Nombre.ToString();
                if (string.IsNullOrEmpty(ww.Codigo.ToString())) e.Handled = false;
                if (!ActualizaCamposRef(TxtCodigo.Text.Trim())) e.Handled = false;
                e.Handled = true;
                ww = null;
            }
            else
            {
                if (!ActualizaCamposRef(TxtCodigo.Text.Trim()))
                {
                    MessageBox.Show("Codigo :" + TxtCodigo.Text.Trim() + " No existe...");
                    TxtNombre.Text="";
                    e.Handled = true;
                    return;
                }
            }
        }
        private bool ActualizaCamposRef(string Id)
        {
            // MessageBox.Show("Ref: "+Id);
            bool Resp = false;
            try
            {
                if (string.IsNullOrEmpty(Id)) return false;
                //                        dr =((Inicio)Application.Current.MainWindow).Func.SqlDR("SELECT idrow,cod_ref,rtrim(nom_ref) as nom_ref,idrowtip,val_ref FROM inmae_ref where cod_ref='"+Id.ToString()+"' or idrow="+Id.ToString(),idEmp);
                SqlDataReader dr = SiaWin.Func.SqlDR("select inmae_ref.idrow,cod_ref,rtrim(nom_ref) as nom_ref,inmae_ref.cod_tip,val_ref,inmae_ref.cod_tiva,inmae_tiva.por_iva,nom_tip,nom_prv,inmae_tip.por_des as tippor_des,inmae_tip.por_desc as tippor_desc,inmae_ref.imageid,inmae_ref.por_des FROM inmae_ref inner join inmae_tiva on inmae_tiva.cod_tiva=inmae_ref.cod_tiva inner join inmae_tip on inmae_tip.cod_tip=inmae_ref.cod_tip left join inmae_prv on inmae_prv.cod_prv=inmae_ref.cod_prv where  inmae_ref.cod_ref='" + Id.ToString() + "'", idEmp);
                while (dr.Read())
                {
                    //       MessageBox.Show(((Referencia)((DataGrid)datagrid).SelectedItem).val_ref.ToString()+" - "+Convert.ToDouble(dr["val_ref"]).ToString() );
                    TxtNombre.Text= dr["nom_ref"].ToString().Trim();
                    Resp = true;
                }
                dr.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (System.Exception _error)
            {
                MessageBox.Show(_error.Message);
            }
            return Resp;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            idbod = idBod;
        }
        private DataSet LoadData(string refe)
        {
            try
            {
                ds1.Clear();
                Conexion = SiaWin.Func.DatosEmp(idEmp);
                SqlConnection con = new SqlConnection(Conexion);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                //DataSet ds1 = new DataSet();
                //cmd = new SqlCommand("ConsultaCxcCxpAll", con);
                cmd = new SqlCommand("SpInventariosSaldosProductoBodegaReportWEB", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idref", refe);//if you have parameters.
                //cmd.Parameters.AddWithValue("@Where", where);//if you have parameters.
                da = new SqlDataAdapter(cmd);
                da.Fill(ds1);
                con.Close();
                return ds1;
            }
            catch (SqlException SQLex)
            {
                MessageBox.Show("Error SQL:" + SQLex.Message);

            }
            catch (Exception e)
            {
                MessageBox.Show("Error App:" + e.Message);
            }
            return null;
        }
        private void BtnConsultar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtCodigo.Text))
            {
                TxtCodigo.Focus();
                return;
            }
            DataSet ds1 = LoadData(TxtCodigo.Text.Trim());
            if (ds1.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("Producto:" + TxtCodigo.Text + "-" + TxtNombre.Text.Trim() + " Sin saldos en bodegas..");
                return;
            }
            double sum = 0;
            //foreach (System.Data.DataColumn col in ds1.Tables[0].Columns) col.ReadOnly = false;
            foreach (DataRow dr in ds1.Tables[0].Rows) // search whole table
            {
                double saldoin = Convert.ToDouble(dr["saldo_fin"]);
                sum = sum + saldoin;
            }
            TxtSaldo.Text=sum.ToString("N2");
            dataGrid.ItemsSource = ds1.Tables[0].DefaultView;
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Salir = true;
                this.Close();
            }

        }

        private void TxtCodigo_GotFocus(object sender, RoutedEventArgs e)
        {
            TxtCodigo.Text = "";
            TxtNombre.Text = "";
            TxtSaldo.Text = "0";
            ds1.Clear();
        }
    }
}
