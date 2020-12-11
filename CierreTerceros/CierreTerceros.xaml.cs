using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{
 
    //Sia.PublicarPnt(9665, "CierreTerceros");
    //Sia.TabU(9665);


    public partial class CierreTerceros : UserControl
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        string nomempresa = "";
        dynamic tabitem;
        public int idmodulo = 1;

        public CierreTerceros(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            tabitem = tabitem1;
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
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                nomempresa = foundRow["BusinessName"].ToString().Trim();
                tabitem.Title = "Cierre de Terceros - " + nomempresa;
                tabitem.Logo(idLogo, ".png");

                DataTable dt_trn = SiaWin.Func.SqlDT("select rtrim(cod_trn) as cod_trn,rtrim(cod_trn)+'-'+rtrim(nom_trn) as nom_trn from comae_trn order by cod_trn", "tabla", idemp);
                CBtipotrn.ItemsSource = dt_trn.DefaultView;


                #region valores por defecto

                TxFecIni.Text = DateTime.Now.ToString();
                TxFecFin.Text = DateTime.Now.ToString();
                CBtipotrn.SelectedValue = "98";
                TxDocumento.Text = "CIE-TER-" + DateTime.Now.Year.ToString();
                TxFecTrn.Text = DateTime.Now.ToString();
                TxCtaCierre.Text = "31050301";
                TxTerCierre.Text = "999000999";

                #endregion

            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace((sender as TextBox).Text)) return;
                else
                {
                    string table = (sender as TextBox).Tag.ToString().Trim();
                    string value = (sender as TextBox).Text.ToString().Trim();
                    string code = "";
                    switch (table)
                    {
                        case "comae_cta": code = "cod_cta"; break;
                        case "comae_ter": code = "cod_ter"; break;
                    }

                    DataTable dt = SiaWin.Func.SqlDT("select * from  " + table + "  where  " + code + "='" + value + "' ", "Empresas", idemp);
                    if (dt.Rows.Count <= 0)
                    {
                        MessageBox.Show("el codigo ingresado no existe", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        (sender as TextBox).Text = "";
                    }
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("errro al buscar codigo:" + w);
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.F8 || e.Key == Key.Enter)
                {
                    e.Handled = true;
                    string table = (sender as TextBox).Tag.ToString().Trim();
                    string value = (sender as TextBox).Text.ToString().Trim();
                    string codetbl = ""; string nomtbl = ""; string tit = ""; bool mostrar = false;
                    switch (table)
                    {
                        case "comae_cta": codetbl = "cod_cta"; nomtbl = "nom_cta"; tit = "Cuentas"; break;
                        case "comae_ter": codetbl = "cod_ter"; nomtbl = "nom_ter"; tit = "Terceros"; break;
                    }


                    string cmptabla = table; string cmpcodigo = codetbl; string cmpnombre = nomtbl; string cmporden = "idrow"; string cmpidrow = "idrow"; string cmptitulo = "Maestra de " + tit; bool mostrartodo = mostrar; string cmpwhere = "";
                    int idr = 0; string code = ""; string nom = "";
                    dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, cnEmp, mostrartodo, cmpwhere, idEmp: idemp);
                    winb.ShowInTaskbar = false;
                    winb.Owner = Application.Current.MainWindow;
                    winb.Width = 500;
                    winb.Height = 400;
                    winb.ShowDialog();
                    idr = winb.IdRowReturn;
                    code = winb.Codigo;
                    nom = winb.Nombre;
                    winb = null;
                    (sender as TextBox).Text = !string.IsNullOrEmpty(code) ? code.Trim() : "";

                    if (e.Key == Key.Enter)
                    {
                        var uiElement = e.OriginalSource as UIElement;
                        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar:" + w);
            }

        }


        private async void BtnEjecutar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region validaciones
                if (string.IsNullOrEmpty(tx_cta_desde.Text) || string.IsNullOrEmpty(tx_cta_hasta.Text))
                {
                    MessageBox.Show("ingrese un rango de cuentas a cerrar", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (string.IsNullOrEmpty(TxDocumento.Text))
                {
                    MessageBox.Show("ingrese el numero de transaccion para generar el cierre", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                else
                {
                    string numtrn = TxDocumento.Text.Trim();
                    DataTable dt = SiaWin.Func.SqlDT("select * from cocab_doc where num_trn='" + numtrn + "' ", "cabeza", idemp);
                    if (dt.Rows.Count > 0)
                    {
                        MessageBox.Show("el documento de cierre de terceros " + numtrn + " ya existe en el sistema", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                }

                if (string.IsNullOrEmpty(TxCtaCierre.Text))
                {
                    MessageBox.Show("ingrese una cuenta de cierre", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (string.IsNullOrEmpty(TxTerCierre.Text))
                {
                    MessageBox.Show("ingrese una tercero de cierre", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }


                #endregion

                PanelExecute.IsEnabled = false;
                sfBusyIndicator.IsBusy = true;
                BtnEjecutar.IsEnabled = false;
                GridConfiguracion.IsEnabled = false;

                string cta_ini = tx_cta_desde.Text.Trim();
                string cta_fin = tx_cta_desde.Text.Trim();

                string fec_ini = TxFecIni.Text;
                string fec_fin = TxFecFin.Text;

                string cod_trn = CBtipotrn.SelectedValue.ToString().Trim();
                string num_trn = TxDocumento.Text.Trim();
                string fec_trn = TxFecTrn.Text;

                string cta_cie = TxCtaCierre.Text;
                string ter_cie = TxTerCierre.Text;

                int isexecute = CbCierre.SelectedIndex;

                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(cta_ini, cta_fin, fec_ini, fec_fin, cod_trn, num_trn, fec_trn, cta_cie, ter_cie, isexecute, cod_empresa));
                await slowTask;

                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    string messageaudit = "EJECUTO EL CIERRE DE TERCEROS DOC:" + num_trn + " FECHA:" + fec_trn + " CUENTA CIERRE:" + cta_cie + " TERCERO CIERRE:" + ter_cie + "";
                    if (isexecute == 1)  
                        SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idemp, 0, 0, 0, messageaudit, "");

                    dataGridConsulta.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                    Total.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();

                    double debito = 0;
                    double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(deb_mov)", "").ToString(), out debito);

                    double credito = 0;
                    double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(cre_mov)", "").ToString(), out credito);

                    TxDebito.Text = debito.ToString("N");
                    TxCredito.Text = credito.ToString("N");

                    TabControl1.SelectedIndex = 2;
                    TabControl1.SelectedIndex = 1;
                }

                PanelExecute.IsEnabled = true;
                BtnEjecutar.IsEnabled = true;
                this.sfBusyIndicator.IsBusy = false;
                GridConfiguracion.IsEnabled = true;

            }
            catch (Exception w)
            {
                MessageBox.Show("errror en el cierre del tercero:" + w);
            }
        }

        private DataSet LoadData(string cta_ini, string cta_fin, string fec_ini, string fec_fin, string cod_trn, string num_trn, string fec_trn, string cta_cie, string ter_cie, int isexecute, string empresa)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpSpCierreTerceros", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@cta_ini", cta_ini);
                cmd.Parameters.AddWithValue("@cta_fin", cta_fin);
                cmd.Parameters.AddWithValue("@fec_ini", fec_ini);
                cmd.Parameters.AddWithValue("@fec_fin", fec_fin);
                cmd.Parameters.AddWithValue("@cod_trn", cod_trn);
                cmd.Parameters.AddWithValue("@num_trn", num_trn);
                cmd.Parameters.AddWithValue("@fec_trn", fec_trn);
                cmd.Parameters.AddWithValue("@cta_cie", cta_cie);
                cmd.Parameters.AddWithValue("@ter_cie", ter_cie);
                cmd.Parameters.AddWithValue("@isExecute", isexecute);
                cmd.Parameters.AddWithValue("@codemp", empresa);
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds);
                con.Close();
                return ds;
            }
            catch (SqlException ex)
            {
                MessageBox.Show("error sql:" + ex);
                return null;
            }
        }


        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            tabitem.Cerrar(0);
        }

        private void BtnViewDoc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string numtrn = TxDocumento.Text.Trim();
                string codtrn = CBtipotrn.SelectedValue.ToString();

                string query = "select * From cocab_doc where cod_trn='" + codtrn + "' and num_trn='" + numtrn + "' ";
                DataTable dt = SiaWin.Func.SqlDT(query, "cabeza", idemp);
                if (dt.Rows.Count > 0)
                {
                    int idreg = Convert.ToInt32(dt.Rows[0]["idreg"]);
                    SiaWin.TabTrn(0, idemp, true, idreg, idmodulo, WinModal: true);
                }
                else
                {
                    MessageBox.Show("el documento " + numtrn + " no se encuentra", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al ver el documento:" + w);
            }
        }

        private void Exportar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;
                var excelEngine = dataGridConsulta.ExportToExcel(dataGridConsulta.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];

                SaveFileDialog sfd = new SaveFileDialog
                {
                    FilterIndex = 2,
                    Filter = "Excel 97 to 2003 Files(*.xls)|*.xls|Excel 2007 to 2010 Files(*.xlsx)|*.xlsx|Excel 2013 File(*.xlsx)|*.xlsx"
                };

                if (sfd.ShowDialog() == true)
                {
                    using (Stream stream = sfd.OpenFile())
                    {
                        if (sfd.FilterIndex == 1)
                            workBook.Version = ExcelVersion.Excel97to2003;
                        else if (sfd.FilterIndex == 2)
                            workBook.Version = ExcelVersion.Excel2010;
                        else
                            workBook.Version = ExcelVersion.Excel2013;
                        workBook.SaveAs(stream);
                    }

                    if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archvo", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al exportar:" + w);
            }
        }


    }
}

