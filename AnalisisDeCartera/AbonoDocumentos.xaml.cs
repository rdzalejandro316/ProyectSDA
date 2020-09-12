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

namespace AnalisisDeCartera
{
    
    public partial class AbonoDocumentos : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public string num_trn = "";
        public string cod_ter = "";
        public string cod_cta = "";
        public AbonoDocumentos(int idEmpresa)
        {
            InitializeComponent();
            idemp = idEmpresa; 
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SiaWin = System.Windows.Application.Current.MainWindow;                        
            Tx_Document.Text = num_trn;
            LoadConfig(num_trn, cod_ter, cod_cta);
        }

        private void LoadConfig(string num_trn, string cod_ter, string cod_cta)
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);         
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Abonos " + cod_empresa + "-" + nomempresa;

                string query = "select cab.idreg,cab.cod_trn,cab.num_trn,cab.fec_trn,cab.cod_ven,cue.cod_cta,cue.cod_ter,des_mov,cue.deb_mov as valor,cue.cre_mov as abono from Cocue_doc as cue  ";
                query += "inner join CoCab_doc as cab on cab.idreg=cue.idregcab and cab.cod_trn=cue.cod_trn and cab.num_trn=cue.num_trn ";
                query += "where cue.cod_ter='"+ cod_ter + "' and cue.cod_cta='"+ cod_cta + "' and doc_ref='"+ num_trn + "' ";    

                DataTable dt = SiaWin.Func.SqlDT(query, "Cuentas", idemp);
                if (dt.Rows.Count>0)
                {
                    dataGridCxCD.ItemsSource = dt.DefaultView;
                    Tx_rows.Text = dt.Rows.Count.ToString();
                   
                }
                else
                {
                    MessageBox.Show("factura sin abonos");
                    Tx_rows.Text = "0";
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void BtnDetalleD_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)dataGridCxCD.SelectedItems[0];
                if (row == null) return;
                int idreg = Convert.ToInt32(row["idreg"]);
                if (idreg <= 0) return;                
                SiaWin.TabTrn(0, idemp, true, idreg, 1, WinModal: true);
            }
            catch (Exception w)
            {
                System.Windows.MessageBox.Show("Error ...." + w.Message);
            }
        }


    }
}
