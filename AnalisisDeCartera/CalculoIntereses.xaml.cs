using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
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
    public partial class CalculoIntereses : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        int idmodulo = 1;

        public string feccxc;
        public DataTable ctacxc;
        public DataTable cal_inte;
        public List<string> cue_select;

        public CalculoIntereses()
        {
            InitializeComponent();
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
                this.Title = "Calculo de intereses " + cod_empresa + "-" + nomempresa;
                
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
                SiaWin = System.Windows.Application.Current.MainWindow;
                if (idemp <= 0) idemp = SiaWin._BusinessId;

                comboBoxCuentas.ItemsSource = ctacxc.DefaultView;
                comboBoxCuentas.DisplayMemberPath = "nom_cta";
                comboBoxCuentas.SelectedValuePath = "cod_cta";
                
                FechaIni.Text = feccxc;
                
                LoadConfig();

            }
            catch (Exception w)
            {
                MessageBox.Show("error al load:" + w);
            }
        }

        public string getcuentas() 
        {
            string Cta = "";
            foreach (DataRowView ob in comboBoxCuentas.SelectedItems)
            {
                String valueCta = ob["cod_cta"].ToString();
                Cta += valueCta + ",";
            }
            return Cta;
        }

        private async void BtnConsultar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region validaciones

                if (comboBoxCuentas.SelectedIndex < 0)
                {
                    MessageBox.Show("Seleccione una cuenta", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                #endregion


                sfBusyIndicator.IsBusy = true;
                BtnConsultar.IsEnabled = false;
                BtnSalir.IsEnabled = false;


                string ffi = FechaIni.Text.ToString();
                string tercero = TextCod_Ter.Text.Trim();
                string cuenta = getcuentas();
                decimal tasa = Convert.ToDecimal(TxTasa.Value);
                string fec_cxc = feccxc;

                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(ffi, fec_cxc, cuenta, tercero, tasa));
                await slowTask;


                if (slowTask.IsCompleted)
                {
                    if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                    {
                        cal_inte = ((DataSet)slowTask.Result).Tables[0];
                        dataGridCxC.ItemsSource = ((DataSet)slowTask.Result).Tables[0].DefaultView;
                        TxRegistros.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();
                    }
                    else
                    {
                        dataGridCxC.ItemsSource = null;
                        cal_inte = null;
                        TxRegistros.Text = "0";
                        MessageBox.Show("no existen registros para calcular la tasa de interes", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }

                sfBusyIndicator.IsBusy = false;
                BtnConsultar.IsEnabled = true;
                BtnSalir.IsEnabled = true;


            }
            catch (Exception w)
            {
                MessageBox.Show("erro al consultar:" + w);
            }
        }


        private DataSet LoadData(string Fi, string Fcxc, string ctas, string cter, decimal tasa)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_empSpCoAnalisisCxcCalculoIntereses", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Ter", cter);
                cmd.Parameters.AddWithValue("@Cta", ctas);
                cmd.Parameters.AddWithValue("@TipoApli", 1);
                cmd.Parameters.AddWithValue("@Resumen", 1);
                cmd.Parameters.AddWithValue("@Fecha", Fi);
                cmd.Parameters.AddWithValue("@FechaPntCxc", Fcxc);
                cmd.Parameters.AddWithValue("@TrnCo", "");
                cmd.Parameters.AddWithValue("@NumCo", "");
                cmd.Parameters.AddWithValue("@Cco", "");
                cmd.Parameters.AddWithValue("@Ven", "");
                cmd.Parameters.AddWithValue("@tasa", tasa);
                cmd.Parameters.AddWithValue("@TipoReporte", 1);
                cmd.Parameters.AddWithValue("@codemp", "010");
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds);
                con.Close();
                return ds;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }

        private void TextCod_Ter_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TextCod_Ter.Text.Trim() == "") TextNombreTercero.Text = "";
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == System.Windows.Input.Key.F8)
                {
                    string tag = ((TextBox)sender).Tag.ToString();
                    string cmptabla = ""; string cmpcodigo = ""; string cmpnombre = ""; string cmporden = ""; string cmpidrow = ""; string cmptitulo = ""; string cmpconexion = ""; bool mostrartodo = false; string cmpwhere = "";
                    if (string.IsNullOrEmpty(tag)) return;
                    if (tag == "comae_ter")
                    {
                        cmptabla = tag; cmpcodigo = "cod_ter"; cmpnombre = "nom_ter"; cmporden = "cod_ter"; cmpidrow = "idrow"; cmptitulo = "Maestra de Tercero"; cmpconexion = cnEmp; mostrartodo = false; cmpwhere = "";
                    }
                    int idr = 0; string code = ""; string nom = "";
                    dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, SiaWin.Func.DatosEmp(idemp), mostrartodo, cmpwhere, idEmp: idemp);
                    winb.ShowInTaskbar = false;
                    winb.Owner = Application.Current.MainWindow;
                    winb.Width = 400;
                    winb.Height = 400;
                    winb.ShowDialog();
                    idr = winb.IdRowReturn;
                    code = winb.Codigo;
                    nom = winb.Nombre;
                    winb = null;
                    if (idr > 0)
                    {
                        if (tag == "comae_ter")
                        {
                            TextCod_Ter.Text = code.Trim();
                            TextNombreTercero.Text = nom.Trim();
                        }
                        var uiElement = e.OriginalSource as UIElement;
                        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    }
                    e.Handled = true;
                }
                if (e.Key == Key.Enter)
                {
                    var uiElement = e.OriginalSource as UIElement;
                    uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                }
            }
            catch (Exception ex)
            {
                SiaWin.seguridad.ErrorLog("Error  ", "AnalisisDeCartera-PreviewKeyDown:" + ex.Message.ToString());
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnCalcular_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                #region validacion

                if (dataGridCxC.ItemsSource == null || dataGridCxC.View.Records.Count <= 0)
                {
                    MessageBox.Show("no hay datos para generar calculo de intereses", "alerta", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (comboBoxCuentas.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione una cuenta", "alerta", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                };

                #endregion


                int idreg = Documento();
                if (idreg > 0)
                {
                    SiaWin.TabTrn(0, idemp, true, idreg, idmodulo, WinModal: true);
                    dataGridCxC.ItemsSource = null;
                }


            }
            catch (Exception w)
            {
                MessageBox.Show("erro al calcular los intereses:" + w);
            }
        }




        public int Documento()
        {
            int idreg = -1;

            if (MessageBox.Show("Usted desea generar el documento de calculo de intereses?", "Documentos", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                string sql_cab = ""; string sql_cue = "";

                if (cal_inte.Rows.Count > 0)
                {

                    using (SqlConnection connection = new SqlConnection(cnEmp))
                    {

                        connection.Open();
                        SqlCommand command = connection.CreateCommand();
                        SqlTransaction transaction = connection.BeginTransaction("Transaction");
                        command.Connection = connection;
                        command.Transaction = transaction;

                        string cod_trn = "21";
                        string fec_trn = FechaIni.Text;
                        DateTime fectrn = Convert.ToDateTime(FechaIni.Text);


                        string sqlConsecutivo = "declare @fecdoc as datetime;";
                        sqlConsecutivo += "update Comae_trn set num_act=num_act+1 where cod_trn='" + cod_trn + "';";
                        sqlConsecutivo += "set @fecdoc = getdate();declare @ini as char(4);declare @num as varchar(12);declare @iConsecutivo char(12)='';";
                        sqlConsecutivo += "declare @iFolioHost int = 0;";
                        sqlConsecutivo += "SELECT @iFolioHost=num_act,@ini=rtrim(inicial) FROM comae_trn WHERE cod_trn='" + cod_trn + "' set @num=@iFolioHost;";
                        sqlConsecutivo += "select @iConsecutivo=rtrim(@ini)+rtrim(@iFolioHost)";


                        sql_cab += sqlConsecutivo + @"INSERT INTO cocab_doc (cod_trn,num_trn,fec_trn,detalle) values ('" + cod_trn + "',@iConsecutivo,'" + fec_trn + "','Causación intereses moratorios');DECLARE @NewID INT;SELECT @NewID = SCOPE_IDENTITY();";


                        foreach (DataRow dr in cal_inte.Rows)
                        {
                            string xctacxc = dr["cod_cta"].ToString().Trim();

                            string _ctadeb = "";
                            string _ctacre = "";



                            if (xctacxc.Substring(0, 8) == "13110101")
                            {
                                _ctadeb = "1311030101"; _ctacre = "4110030101";
                            }

                            if (xctacxc.Substring(0, 8) == "13110102")
                            {
                                _ctadeb = "13110102"; _ctacre = "4110030102";
                            }

                            if (xctacxc == "1311010104")
                            {
                                _ctadeb = "8190030101"; _ctacre = "8905902301";
                            }

                            if (xctacxc == "1311010204")
                            {
                                _ctadeb = "8190030102"; _ctacre = "8905902302";
                            }



                            string cod_ter = dr["cod_ter"].ToString().Trim();
                            string factura = dr["factura"].ToString().Trim();
                            string fec_ven = dr["fec_ven"].ToString().Trim();
                            decimal val_int = Convert.ToDecimal(dr["val_int"]);

                            string desmov = "Causación intereses moratorios con corte al mes " + fectrn.Month + " de " + fectrn.Year + " ART 635 E:T CONC 00068 DE 2017 FRA " + factura + " ";

                            sql_cue += @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,des_mov,bas_mov,deb_mov,cre_mov,doc_mov,fec_venc) values (@NewID,'" + cod_trn + "',@iConsecutivo,'" + _ctadeb + "','" + cod_ter + "','" + desmov + "',0," + val_int.ToString("F", CultureInfo.InvariantCulture) + ",0,'" + factura + "','" + fec_ven + "');";

                            sql_cue += @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,des_mov,bas_mov,deb_mov,cre_mov,doc_mov,fec_venc) values (@NewID,'" + cod_trn + "',@iConsecutivo,'" + _ctacre + "','" + cod_ter + "','" + desmov + "',0,0," + val_int.ToString("F", CultureInfo.InvariantCulture) + ",'" + factura + "','" + fec_ven + "');";


                        }
                        command.CommandText = sql_cab + sql_cue + @"select CAST(@NewId AS int);";
                        var r = new object();
                        r = command.ExecuteScalar();
                        idreg = Convert.ToInt32(r);
                        transaction.Commit();
                        connection.Close();
                    }

                }

            }

            return idreg;
        }




    }
}
