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

namespace Egreso
{
    
    public partial class ViewDocument : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public string document = "";
        public ViewDocument()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            idemp = SiaWin._BusinessId; ;
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
                this.Title = "Abonos " + cod_empresa + "-" + nomempresa;                
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string doc = document;
                string select = "select CoCab_doc.idreg,CoCab_doc.cod_trn,CoCab_doc.num_trn,CoCab_doc.fec_trn,CoCab_doc.cod_ven,InMae_mer.nom_mer,CoCab_doc.detalle ";
                select += "from Cocue_doc ";
                select += "inner join CoCab_doc on Cocue_doc.idregcab =  CoCab_doc.idreg ";
                select += "inner join inmae_mer on CoCab_doc.cod_ven = InMae_mer.cod_mer ";
                select += "where doc_cruc='"+ doc+ "' ";

                DataTable dt= SiaWin.Func.SqlDT(select, "cabeza", idemp);
                dataGridCab.ItemsSource = dt.DefaultView;
            }
            catch (Exception w)
            {
                MessageBox.Show("error en la consulta:"+w);
            }
        }

        private void DataGridCab_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            try
            {
                DataRowView GridCab = (DataRowView)dataGridCab.SelectedItems[0];
                string idcab = GridCab["idreg"].ToString();

                string select = "select cod_cta,cod_cco,des_mov,bas_mov,deb_mov,cre_mov,doc_cruc from Cocue_doc  ";
                select +=  "where idregcab = '"+idcab+"' ";

                DataTable dt = SiaWin.Func.SqlDT(select, "cabeza", idemp);
                dataGridCue.ItemsSource = dt.DefaultView;

                double tot_deb = Convert.ToDouble(dt.Compute("Sum(deb_mov)", ""));
                TotDeb.Text = tot_deb.ToString();
                double tot_cre = Convert.ToDouble(dt.Compute("Sum(cre_mov)", ""));
                TotCre.Text = tot_deb.ToString();
                double dif = tot_deb - tot_cre;
                Dife.Text = dif.ToString();

            }
            catch (Exception w)
            {
                MessageBox.Show("error al consultar el cuerpo:"+w);
            }

        }




    }
}
