using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
namespace WindowPV
{

    public partial class Cotizaciones : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        public int idregcabReturn = -1;
        public string codtrn = string.Empty;
        public string numtrn = string.Empty;
        public Cotizaciones(int idEmpresa)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = idEmpresa;
            LoadConfig();
        }
        private void LoadConfig()
        {
            try
            {
                DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        private void Consulta_DropDownClosed(object sender, EventArgs e)
        {
            dataGridCabeza.SelectedItem = 0;
            dataGridCuerpo.ItemsSource = null;
            consultaCabeza();            
        }
        public void consultaCabeza()
        {
            try
            {
                var tag = ((ComboBoxItem)TextBxCB_consulta.SelectedItem).Tag.ToString();
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();                
                cmd = new SqlCommand("_EmpPvConsultaFactura", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@cod_trn", tag.ToString());
                cmd.Parameters.AddWithValue("@_codemp", cod_empresa);                
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                con.Close();
                dataGridCabeza.ItemsSource = ds.Tables[0];
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar el procedimiento almacenado"+w);
            }
        }
        public void consultaCuerpo()
        {
            try
            {
                var tag = ((ComboBoxItem)TextBxCB_consulta.SelectedItem).Tag.ToString();
                DataRowView row = (DataRowView)dataGridCabeza.SelectedItems[0];
                string idreg = row["idreg"].ToString();
                string cadena = "select cuerpo.cod_ref,referencia.nom_ref,cuerpo.cantidad,cuerpo.val_uni,cuerpo.subtotal,cuerpo.por_des,cuerpo.tot_tot from InCue_doc as cuerpo ";
                cadena += "inner join InMae_ref as referencia on cuerpo.cod_ref = referencia.cod_ref ";
                cadena += "inner join InCab_doc as cabeza on cuerpo.idregcab = cabeza.idreg ";
                cadena += "where cuerpo.idregcab='" + idreg + "' and cuerpo.cod_trn='"+ tag.ToString() +"' ";
                DataTable dt = SiaWin.Func.SqlDT(cadena, "Clientes", idemp);
                dataGridCuerpo.ItemsSource = dt.DefaultView;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al carcar el detalle del cuerpo:"+w);
            }
        }
        private void dataGridCabeza_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
           consultaCuerpo();
        }
        private void BTNfacturar_Click(object sender, RoutedEventArgs e)
        {
//            var tag = ((ComboBoxItem)TextBxCB_consulta.SelectedItem).Tag.ToString();

            DataRowView row = (DataRowView)dataGridCabeza.SelectedItems[0];
            if(row!=null)
            {
                idregcabReturn = Convert.ToInt32(row["idreg"].ToString());
                codtrn = row["cod_trn"].ToString();
                numtrn = row["num_trn"].ToString();
            }
            this.Close();
        }
    }
}
