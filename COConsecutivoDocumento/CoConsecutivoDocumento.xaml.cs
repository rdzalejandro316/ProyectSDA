using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9647, "CoConsecutivoDocumento");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9647, "CoConsecutivoDocumento");
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();
    public partial class CoConsecutivoDocumento : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";        
        public DataTable dt = new DataTable();

        public CoConsecutivoDocumento()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                SiaWin = Application.Current.MainWindow;
                if (idemp <= 0) idemp = SiaWin._BusinessId;

                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Consecutivo de Documentos Contabilidad " + cod_empresa + "-" + nomempresa;
                dt = SiaWin.Func.SqlDT("select cod_trn,nom_trn,ind_con,num_act,lon_num,ind_modi,inicial,num_01,num_02,num_03,num_04,num_05,num_06,num_07,num_08,num_09,num_10,num_11,num_12  from comae_trn order by cod_trn", "transacciones", idemp);
                dataGridDoc.ItemsSource = dt.DefaultView;

            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnExample_Click(object sender, RoutedEventArgs e)
        {
            //DataRowView row = (DataRowView)dataGridDoc.SelectedItems[0];
            //string cod_trn = row["cod_trn"].ToString();
            //int ind_con = Convert.ToInt32(row["ind_con"]);
            //string num_act = row["num_act"].ToString();
            //int LON_NUM = Convert.ToInt32(row["lon_num"]);
            //string INICIAL = row["inicial"].ToString();            

            //string campo = "num_act";


            //string query = "declare @fecdoc as datetime;set @fecdoc = getdate(); ";
            //query += "declare @fecdocsecond as datetime;set @fecdocsecond = DATEADD(second,1,GETDATE());  ";
            //query += "declare @ini as char(4);declare @num as varchar(12);   ";
            //query += "declare @iConsecutivo char(12) = '' ;declare @iFolioHost int = 0;  ";
            //query += "SELECT @iFolioHost= isnull(" + campo + ",0),@ini=rtrim(inicial) FROM inmae_trn WHERE cod_trn='" + cod_trn + "';   ";
            //query += "set @num=@iFolioHost   ";
            //query += "select @iConsecutivo=rtrim(@ini)+REPLICATE ('0'," + LON_NUM
            //    + "-len(rtrim(@ini))-len(rtrim(convert(varchar,@num))))+rtrim(convert(varchar,@num));  ";
            //query += "select @iConsecutivo as tt ";
            //DataTable dt = SiaWin.Func.SqlDT(query, "transacciones", idemp);


            //if (dt.Rows.Count > 0)
            //{
            //    SiaWin.Browse(dt);
            //    Tx_example.Text = dt.Rows[0]["tt"].ToString();
            //}
        }

        private void dataGridDoc_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            try
            {
                if (dataGridDoc.SelectedIndex>=0)
                {
                    GridPanelEdit.IsEnabled = true;
                    DataRowView row = (DataRowView)dataGridDoc.SelectedItems[0];
                    Cb_consec.SelectedIndex = Convert.ToInt32(row["ind_con"]);
                    Tx_consecutivo.Text = row["num_act"].ToString().Trim();
                    Cb_long.SelectedIndex = Convert.ToInt32(row["lon_num"]);
                    Tx_ini.Text = row["inicial"].ToString().Trim();
                    Cb_mod.SelectedIndex = Convert.ToInt32(row["Ind_modi"]);

                    mes1.Text = row["num_01"].ToString();
                    mes2.Text = row["num_02"].ToString();
                    mes3.Text = row["num_03"].ToString();
                    mes4.Text = row["num_04"].ToString();
                    mes5.Text = row["num_05"].ToString();
                    mes6.Text = row["num_06"].ToString();
                    mes7.Text = row["num_07"].ToString();
                    mes8.Text = row["num_08"].ToString();
                    mes9.Text = row["num_09"].ToString();
                    mes10.Text = row["num_10"].ToString();
                    mes11.Text = row["num_11"].ToString();
                    mes12.Text = row["num_12"].ToString();
                }
                                
            }
            catch (Exception w)
            {
                MessageBox.Show("error _" + w);
            }
        }


        private void ValidBox(object sender, RoutedEventArgs e)
        {
            try
            {
                string tag = (sender as TextBox).Tag.ToString();

                if (string.IsNullOrEmpty((sender as TextBox).Text)) return;
                
                if (dataGridDoc.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)dataGridDoc.SelectedItems[0];
                    string update = "update comae_trn set " + tag + "='" + (sender as TextBox).Text + "' where cod_trn='" + row["cod_trn"].ToString() + "'  ";
                    if (SiaWin.Func.SqlCRUD(update, idemp) == true)
                    {
                        System.Data.DataRow dr = dt.Rows[dataGridDoc.SelectedIndex];
                        dr.BeginEdit();
                        dr[tag] = (sender as TextBox).Text;
                        dr.EndEdit();
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al actualizar:"+w);
            }
        }

        private void Number_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)dataGridDoc.SelectedItems[0];
                string cod_trn = row["cod_trn"].ToString();
                int index = (sender as ComboBox).SelectedIndex;
                string campo = (sender as ComboBox).Tag.ToString(); ;

                string update = "update comae_trn set " + campo + "='" + index + "' where cod_trn='" + cod_trn + "' ";

                if (SiaWin.Func.SqlCRUD(update, idemp) == true)
                {
                    System.Data.DataRow dr = dt.Rows[dataGridDoc.SelectedIndex];
                    dr.BeginEdit();
                    dr[campo] = index.ToString();
                    dr.EndEdit();
                }


                if (campo == "ind_con")
                {
                    if (Cb_consec.SelectedIndex == 3)
                    {
                        GridDates.IsEnabled = true;
                    }
                    else
                    {
                        GridDates.IsEnabled = false;
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error en el change"+w);
            }
        }












    }
}
