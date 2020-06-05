using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AnalisisDeCartera;
using Syncfusion.XlsIO;
using Microsoft.Win32;
using System.IO;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.Data;
using System.Linq;
using Microsoft.Reporting.WinForms;
using System.Collections.Generic;
using System.Globalization;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9307,"AnalisisDeCartera");
    //Sia.TabU(9307);
    public partial class AnalisisDeCartera : UserControl
    {
        dynamic SiaWin;
        dynamic tabitem;
        public int idemp = 0;
        string cnEmp = "";
        string codemp = string.Empty;
        DataSet ds = new DataSet();
        DataTable Cuentas = new DataTable();

        DataTable DtCartera = new DataTable();
        string codpvta = string.Empty;
        public AnalisisDeCartera(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            tabitem = tabitem1;
            tabitem.Title = "Analisis de Cartera";
            tabitem.Logo(9, ".png");
            tabitem.MultiTab = false;
            //            idemp = SiaWin._BusinessId;
            if (tabitem.idemp > 0) idemp = tabitem.idemp;
            if (tabitem.idemp <= 0) idemp = SiaWin._BusinessId;

            codpvta = SiaWin._UserTag;
            LoadConfig();
        }
        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                codemp = foundRow["BusinessCode"].ToString().Trim();
                tabitem.Logo(idLogo, ".png");
                tabitem.Title = "Analisis de Cartera(" + aliasemp + ")";
                //GroupId = 0;
                //ProjectId = 0;
                //BusinessId = 0;
                Cuentas = SiaWin.Func.SqlDT("SELECT rtrim(cod_cta) as cod_cta,rtrim(cod_cta)+'('+rtrim(nom_cta)+')' as nom_cta FROM COMAE_CTA WHERE ind_mod = 1 and (tip_apli = 3 or tip_apli = 4 ) ORDER BY COD_CTA", "Cuentas", idemp);
                comboBoxCuentas.ItemsSource = Cuentas.DefaultView;
                //comboBoxCuentas.DataContext = Cuentas;
                comboBoxCuentas.DisplayMemberPath = "nom_cta";
                comboBoxCuentas.SelectedValuePath = "cod_cta";
                FechaIni.Text = DateTime.Now.ToShortDateString();
            }
            catch (Exception e)
            {
                SiaWin.seguridad.ErrorLog("Error  ", "AnalisisDeCartera-LoadConfig:" + e.Message.ToString());
                MessageBox.Show(e.Message);
            }
        }
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Salir de cartera");
            tabitem.Cerrar(0);
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
                    if (tag == "inmae_mer")
                    {
                        cmptabla = tag; cmpcodigo = "cod_mer"; cmpnombre = "nom_mer"; cmporden = "cod_mer"; cmpidrow = "idrow"; cmptitulo = "Maestra de vendedores"; cmpconexion = cnEmp; mostrartodo = false; cmpwhere = "";
                    }
                    if (tag == "comae_ter")
                    {
                        cmptabla = tag; cmpcodigo = "cod_ter"; cmpnombre = "nom_ter"; cmporden = "cod_ter"; cmpidrow = "idrow"; cmptitulo = "Maestra de Tercero"; cmpconexion = cnEmp; mostrartodo = false; cmpwhere = "";
                    }
                    //MessageBox.Show(cmptabla + "-" + cmpcodigo + "-" + cmpnombre + "-" + cmporden + "-" + cmpidrow + "-" + cmptitulo + "-" + cmpconexion + "-" + cmpwhere);
                    int idr = 0; string code = ""; string nom = "";
                    //dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, cnEmp, mostrartodo, cmpwhere);
                    dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, SiaWin.Func.DatosEmp(idemp), mostrartodo, cmpwhere, idEmp: idemp);
                    winb.ShowInTaskbar = false;
                    winb.Owner = Application.Current.MainWindow;
                    winb.ShowDialog();
                    idr = winb.IdRowReturn;
                    code = winb.Codigo;
                    nom = winb.Nombre;
                    winb = null;
                    if (idr > 0)
                    {
                        //((TextBox)sender).Text = code;
                        if (tag == "inmae_mer")
                        {
                            //   TextCod_Ven.Text = code; TextNombre.Text = nom;
                            TextCod_Ven.Text = code.Trim();
                            TextNombreVend.Text = nom.Trim();
                        }
                        if (tag == "comae_ter")
                        {
                            TextCod_Ter.Text = code.Trim();
                            TextNombreTercero.Text = nom.Trim();
                            //TextCod_bod.Text = code; TextNombreBod.Text = nom;
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

        private void TextCod_Ven_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TextCod_Ven.Text.Trim() == "") TextNombreVend.Text = "";
            //string tag = ((TextBox)sender).Tag.ToString();
            //if (tag == "inmae_mer")
            //{
            //    if (TextCod_Ven.Text.Trim() == "") TextNombre.Text = "F8=Consultar";
            //}
            //if (tag == "comae_ter")
            //{
            //    //if (TextCod_bod.Text.Trim() == "") TextNombreBod.Text = "F8=Consultar";
            //}
        }

        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxCuentas.SelectedIndex < 0)
            {
                MessageBox.Show("Seleccione una cuenta");
                comboBoxCuentas.Focus();
                return;
            }

            //if(comboBoxCuentas.SelectedIndex<0)
            //{
            //  MessageBox.Show("Seleccione una cuenta...");
            //comboBoxCuentas.Focus();
            //comboBoxCuentas.IsDropDownOpen = true;
            //return;
            //}
            //            DataRowView drv = (DataRowView)comboBoxCuentas.SelectedItem;
            //            String valueOfItem = drv["cod_cta"].ToString();
            //            MessageBox.Show(valueOfItem);
            string Cta = "";
            if (comboBoxCuentas.SelectedIndex >= 0)
            {
                foreach (DataRowView ob in comboBoxCuentas.SelectedItems)
                {
                    //dr["cod_ter"].ToString();
                    String valueCta = ob["cod_cta"].ToString();

                    Cta += valueCta + ",";
                    //MessageBox.Show(valueOfItem1.ToString());
                }
                string ss = Cta.Trim().Substring(Cta.Trim().Length - 1);
                if (ss == ",") Cta = Cta.Substring(0, Cta.Trim().Length - 1);

            }
            //MessageBox.Show(Cta);
            //            Cta = "";
            //this.Opacity = 0.5;
            try
            {
                //string where = ArmaWhere();
                string where = "";
                //if (where==null) return;
                //MessageBox.Show(where);
                // carmar where
                if (string.IsNullOrEmpty(where)) where = " ";

                //               busy.IsBusy = true;
                //       busy.Visibility=Visibility.Visible;
                //dataGrid.Opacity = 0.5;
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;

                //this.IsEnabled = false;
                sfBusyIndicator.IsBusy = true;
                DtCartera.Clear();
                //    LoadData(recordChanged());
                //dataGrid.Model.View.Refresh();
                dataGridCxC.ClearFilters();
                dataGridCxC.ItemsSource = null;

                //CharVentasBodega.DataContext = null;
                //AreaSeriesVta.ItemsSource = null;
                //ds.Clear();
                BtnEjecutar.IsEnabled = false;
                Imprimir.IsEnabled = false;
                ExportarXls.IsEnabled = false;
                ConciliarCxcCo.IsEnabled = false;
                source.CancelAfter(TimeSpan.FromSeconds(1));
                tabitem.Progreso(true);
                string ffi = FechaIni.Text.ToString();

                string Vededor = TextCod_Ven.Text.Trim();
                string Tercero = TextCod_Ter.Text.Trim();

                var slowTask = Task<DataSet>.Factory.StartNew(() => SlowDude(ffi, Cta, Tercero, Vededor, where, source.Token), source.Token);
                await slowTask;
                //MessageBox.Show(slowTask.Result.ToString());
                BtnEjecutar.IsEnabled = true;
                Imprimir.IsEnabled = true;
                ExportarXls.IsEnabled = true;
                ConciliarCxcCo.IsEnabled = true;

                tabitem.Progreso(false);
                resetTotales();


                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    DtCartera = ((DataSet)slowTask.Result).Tables[0];

                    //ds.Tables.Add(((DataSet)slowTask.Result).Tables[0]);
                    //ds.Tables[0] = ((DataSet)slowTask.Result).Tables[0];
                    //dataGridCxC.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                    dataGridCxC.ItemsSource = DtCartera.DefaultView;
                    //((DataSet)slowTask.Result).Tables[0];
                    //CharVentasBodega.DataContext = ((DataSet)slowTask.Result).Tables[1];
                    // AreaSeriesVta.ItemsSource = ((DataSet)slowTask.Result).Tables[1];
                    double valorCxC, valorCxCAnt = 0;
                    //double valorCxCAnt = 0;
                    double valorCxP = 0;
                    double valorCxPAnt = 0;
                    double saldoCxC = 0;
                    double saldoCxCAnt = 0;
                    double saldoCxP = 0;
                    double saldoCxPAnt = 0;
                    double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(valor)", "tip_apli=3").ToString(), out valorCxC);
                    double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(valor)", "tip_apli=4").ToString(), out valorCxCAnt);
                    double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(valor)", "tip_apli=1").ToString(), out valorCxP);
                    double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(valor)", "tip_apli=2").ToString(), out valorCxPAnt);
                    double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(saldo)", "tip_apli=3").ToString(), out saldoCxC);
                    double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(saldo)", "tip_apli=4").ToString(), out saldoCxCAnt);
                    double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(saldo)", "tip_apli=1").ToString(), out saldoCxP);
                    double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(saldo)", "tip_apli=2").ToString(), out saldoCxPAnt);
                    //double valorA = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(valor)", "tip_apli=1 or tip_apli=4").ToString());
                    //double saldo = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(saldo)", "tip_apli=2 or tip_apli=3").ToString());
                    TextCxC.Text = valorCxC.ToString("C");
                    TextCxCAnt.Text = valorCxCAnt.ToString("C");
                    TextCxCAbono.Text = (valorCxC - saldoCxC).ToString("C");
                    TextCxCAntAbono.Text = (valorCxCAnt - saldoCxCAnt).ToString("C");
                    TextCxCSaldo.Text = saldoCxC.ToString("C");
                    TextCxCAntSaldo.Text = saldoCxCAnt.ToString("C");
                    TotalCxc.Text = (valorCxC - valorCxCAnt - valorCxP + valorCxPAnt).ToString("C");
                    TotalAbono.Text = ((valorCxC - saldoCxC) - (valorCxCAnt - saldoCxCAnt)).ToString("C");
                    TotalSaldo.Text = (saldoCxC - saldoCxCAnt - saldoCxP + saldoCxPAnt).ToString("C");
                    //double saldoA = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(saldo)", "tip_apli=1 or tip_apli=4").ToString());
                    //TextTotalDoc.Text = (valor-valorA).ToString("C");
                    //TextSaldo.Text = (saldo-saldoA).ToString("C");
                }
                else
                {
                    //TextTotalDoc.Text = "0";
                    //TextSaldo.Text = "0";
                }
                //dataGrid.ItemsSource = Referencias;
                //        return;
                //    recordChanged();
                //    updateRow(9619);

                //    var slowTask = Task<string>.Factory.StartNew(() => LoadData(""));

                //     await slowTask;
                //     Txt.Content += slowTask.Result.ToString();
                //        busy.IsBusy = false;
                //    busy.Visibility=Visibility.Collapsed;
                //this.Opacity = 1;
                this.sfBusyIndicator.IsBusy = false;
                SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idemp, 0, 0, 0, "Consulto Cartera cuentas:" + Cta+" Fecha:" + ffi.ToString()+" - "+ tabitem.Title, "");

                //this.IsEnabled = true;
                //   dataGrid.Focus();
            }
            catch (Exception ex)
            {
                SiaWin.seguridad.ErrorLog("Error  ", "AnalisisDeCartera-ButtonRefresh:" + ex.Message.ToString());
                MessageBox.Show(ex.Message);
                this.Opacity = 1;
            }
        }
        private DataSet SlowDude(string ffi, string ctas, string cter, string cco, string where, CancellationToken cancellationToken)
        {
            try
            {
                DataSet jj = LoadData(ffi, ctas, cter, cco, where, cancellationToken);
                return jj;

            }
            catch (Exception e)
            {
                SiaWin.seguridad.ErrorLog("Error  ", "AnalisisDeCartera-SlowDude:" + e.Message.ToString());
                MessageBox.Show(e.Message);
            }
            return null;
        }
        private DataSet LoadData(string Fi, string ctas, string cter, string cco, string where, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                //DataSet ds1 = new DataSet();
                //cmd = new SqlCommand("ConsultaCxcCxpAll", con);
                cmd = new SqlCommand("_empSpCoAnalisisCxc", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Ter", cter);//if you have parameters.
                cmd.Parameters.AddWithValue("@Cta", ctas);//if you have parameters.
                cmd.Parameters.AddWithValue("@TipoApli", 1);//if you have parameters.
                cmd.Parameters.AddWithValue("@Resumen", 0);//if you have parameters.
                cmd.Parameters.AddWithValue("@Fecha", Fi);//if you have parameters.
                cmd.Parameters.AddWithValue("@TrnCo", "");//if you have parameters.
                cmd.Parameters.AddWithValue("@NumCo", "");//if you have parameters.
                cmd.Parameters.AddWithValue("@Cco", cco);//if you have parameters.
                cmd.Parameters.AddWithValue("@codemp", codemp);//if you have parameters.
                //cmd.Parameters.AddWithValue("@Where", where);//if you have parameters.
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                con.Close();
                return ds;
                //VentasPorProducto.ItemsSource = ds.Tables[0];
                //VentaPorBodega.ItemsSource = ds.Tables[1];
                //VentasPorCliente.ItemsSource = ds.Tables[2];
            }
            catch (Exception e)
            {
                SiaWin.seguridad.ErrorLog("Error  ", "AnalisisDeCartera-LoadData:" + e.Message.ToString());
                MessageBox.Show(e.Message);
                return null;
            }
        }

        private void BtnDetalle_Click(object sender, RoutedEventArgs e)
        {
            //if (comboBoxCuentas.SelectedIndex < 0)
            // {
            //   MessageBox.Show("Seleccione una cuenta...");
            // comboBoxCuentas.Focus();
            //comboBoxCuentas.IsDropDownOpen = true;
            //return;
            //}
            //            DataRowView drv = (DataRowView)comboBoxCuentas.SelectedItem;
            //            String valueOfItem = drv["cod_cta"].ToString();
            //            MessageBox.Show(valueOfItem);
            string Cta = "";
            if (comboBoxCuentas.SelectedIndex > 0)
            {
                foreach (DataRowView ob in comboBoxCuentas.SelectedItems)
                {
                    //dr["cod_ter"].ToString();
                    String valueCta = ob["cod_cta"].ToString();
                    Cta += valueCta + ",";
                    //MessageBox.Show(valueOfItem1.ToString());
                }
                string ss = Cta.Trim().Substring(Cta.Trim().Length - 1);
                if (ss == ",") Cta = Cta.Substring(0, Cta.Trim().Length - 1);
            }
            try
            {
                DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];
                if (row == null)
                {
                    MessageBox.Show("Registro sin datos");
                    return;
                }
                string cod_cli = row[0].ToString();
                string cod_cta = row[2].ToString();
                //                var dr1 = dataGridCxC.SelectedItems;

                //                    string cod_cli = dr["cod_ter"].ToString();
                //                  if (string.IsNullOrEmpty(cod_cli)) return;
                //                string cod_cta = dr["cod_cta"].ToString();
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds1 = new DataSet();
                //cmd = new SqlCommand("ConsultaCxcCxpDeta", con);
                cmd = new SqlCommand("_empSpCoAnalisisCxc", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Ter", cod_cli);//if you have parameters.
                cmd.Parameters.AddWithValue("@Cta", Cta);//if you have parameters.
                cmd.Parameters.AddWithValue("@TipoApli", 1);//if you have parameters.
                cmd.Parameters.AddWithValue("@Resumen", 1);//if you have parameters.
                cmd.Parameters.AddWithValue("@Fecha", FechaIni.Text);//if you have parameters.
                cmd.Parameters.AddWithValue("@TrnCo", "");//if you have parameters.
                cmd.Parameters.AddWithValue("@NumCo", "");//if you have parameters.
                cmd.Parameters.AddWithValue("@Cco", "");//if you have parameters.
                cmd.Parameters.AddWithValue("codemp", codemp);
                //cmd.Parameters.AddWithValue("@Cco", TextCod_bod.Text.Trim());//if you have parameters.
                //cmd.Parameters.AddWithValue("@Where", where);//if you have parameters.
                da = new SqlDataAdapter(cmd);
                da.Fill(ds1);
                con.Close();
                if (ds1.Tables[0].Rows.Count == 0)
                {
                    MessageBox.Show("Sin informacion de cartera");
                    return;
                }
                AnalisisDeCarteraDetalle WinDetalle = new AnalisisDeCarteraDetalle();
                WinDetalle.TextCodigo.Text = cod_cli;
                WinDetalle.TextNombre.Text = row["nom_ter"].ToString();
                WinDetalle.TextCuenta.Text = Cta;
                WinDetalle.codemp = codemp;
                WinDetalle.fechacorte = FechaIni.Text;
                WinDetalle.Title = "Detalle de cartera - Fecha De Corte:" + FechaIni.Text.ToString();
                WinDetalle.dataGridCxC.ItemsSource = ds1.Tables[0];
                // TOTALIZA 

                double valorCxC, valorCxCAnt = 0;
                //double valorCxCAnt = 0;
                double valorCxP = 0;
                double valorCxPAnt = 0;
                double saldoCxC = 0;
                double saldoCxCAnt = 0;
                double saldoCxP = 0;
                double saldoCxPAnt = 0;
                double.TryParse(ds1.Tables[0].Compute("Sum(valor)", "tip_apli=3").ToString(), out valorCxC);
                double.TryParse(ds1.Tables[0].Compute("Sum(valor)", "tip_apli=4").ToString(), out valorCxCAnt);
                double.TryParse(ds1.Tables[0].Compute("Sum(valor)", "tip_apli=1").ToString(), out valorCxP);
                double.TryParse(ds1.Tables[0].Compute("Sum(valor)", "tip_apli=2").ToString(), out valorCxPAnt);
                double.TryParse(ds1.Tables[0].Compute("Sum(saldo)", "tip_apli=3").ToString(), out saldoCxC);
                double.TryParse(ds1.Tables[0].Compute("Sum(saldo)", "tip_apli=4").ToString(), out saldoCxCAnt);
                double.TryParse(ds1.Tables[0].Compute("Sum(saldo)", "tip_apli=1").ToString(), out saldoCxP);
                double.TryParse(ds1.Tables[0].Compute("Sum(saldo)", "tip_apli=2").ToString(), out saldoCxPAnt);
                WinDetalle.TextCxC.Text = valorCxC.ToString("C");
                WinDetalle.TextCxCAnt.Text = valorCxCAnt.ToString("C");
                WinDetalle.TextCxCAbono.Text = (valorCxC - saldoCxC).ToString("C");
                WinDetalle.TextCxCAntAbono.Text = (valorCxCAnt - saldoCxCAnt).ToString("C");
                WinDetalle.TextCxCSaldo.Text = saldoCxC.ToString("C");
                WinDetalle.TextCxCAntSaldo.Text = saldoCxCAnt.ToString("C");
                WinDetalle.TotalCxc.Text = (valorCxC - valorCxCAnt - valorCxP + valorCxPAnt).ToString("C");
                WinDetalle.TotalAbono.Text = ((valorCxC - saldoCxC) - (valorCxCAnt - saldoCxCAnt)).ToString("C");
                WinDetalle.TotalSaldo.Text = (saldoCxC - saldoCxCAnt - saldoCxP + saldoCxPAnt).ToString("C");


                WinDetalle.ShowInTaskbar = false;
                WinDetalle.Owner = Application.Current.MainWindow;
                WinDetalle.WindowStartupLocation = WindowStartupLocation.CenterScreen;



                //WinDetalle.dataGridCxC_FilterChanged1();
                WinDetalle.ShowDialog();

                WinDetalle = null;
                //ImprimirDoc(Convert.ToInt32(numtrn), "Reimpreso");

            }
            catch (Exception ex)
            {
                SiaWin.seguridad.ErrorLog("Error  ", "AnalisisDeCartera-BtnDetalle:" + ex.Message.ToString());
                MessageBox.Show(ex.Message.ToString());

            }
        }
        private void ExportarXls_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DtCartera == null) return;
                if (DtCartera.Rows.Count <= 0) return;

                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExportMode = ExportMode.Value;
                options.ExcelVersion = ExcelVersion.Excel2013;
                options.CellsExportingEventHandler = CellExportingHandler;
                var excelEngine = dataGridCxC.ExportToExcel(dataGridCxC.View, options);

                var workBook = excelEngine.Excel.Workbooks[0];
                workBook.ActiveSheet.Columns[4].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[5].NumberFormat = "0.0";
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
                    SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idemp, 0, 0, 0, "Consulto Cartera - Exportar XLS - Fecha:" + FechaIni.ToString() + " - " + tabitem.Title , "");
                    if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archvo", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        //Launching the Excel file using the default Application.[MS Excel Or Free ExcelViewer]
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                SiaWin.seguridad.ErrorLog("Error  ", "AnalisisDeCartera-ExportarXLS:" + ex.Message.ToString());
                MessageBox.Show(ex.Message);
            }
        }
        private static void CellExportingHandler(object sender, GridCellExcelExportingEventArgs e)
        {
            e.Range.CellStyle.Font.Size = 12;
            e.Range.CellStyle.Font.FontName = "Segoe UI";

            if (e.ColumnName == "valor" || e.ColumnName == "sinvenc" || e.ColumnName == "ven01" || e.ColumnName == "ven02" || e.ColumnName == "ven03" || e.ColumnName == "ven04" || e.ColumnName == "ven05" || e.ColumnName == "saldo")
            {
                double value = 0;
                if (double.TryParse(e.CellValue.ToString(), out value))
                {
                    e.Range.Number = value;
                }
                e.Handled = true;
            }
        }


        private void comboBoxCuentas_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            dataGridCxC.ClearFilters();
            dataGridCxC.ItemsSource = null;
            resetTotales();

        }
        private void resetTotales()
        {
            TextCxC.Text = "0.00";
            TextCxCAnt.Text = "0.00";
            TextCxCAbono.Text = "0.00";
            TextCxCAntAbono.Text = "0.00";
            TextCxCSaldo.Text = "0.00";
            TextCxCAntSaldo.Text = "0.00";
            TotalCxc.Text = "0.00";
            TotalAbono.Text = "0.00";
            TotalSaldo.Text = "0.00";
        }


        private void dataGridCxC_FilterChanged(object sender, GridFilterEventArgs e)
        {
            //MessageBox.Show("1");
            // MessageBox.Show("filter:"+( sender as SfDataGrid).View.Records.Count.ToString());
            //            var columnName = e.Column.MappingName;
            //          var filteredResult =(sender as SfDataGrid).View.Records.Select(recordentry => recordentry.Data);
            //        var recordEntry = (sender as SfDataGrid).View.Records;
            var provider = (sender as SfDataGrid).View.GetPropertyAccessProvider();
            var records = (sender as SfDataGrid).View.Records;
            //Gets the value for frozen rows count of corresponding column and removes it from FilterElement collection.
            double valorCxC = 0;
            double valorCxCAnt = 0;
            double valorCxP = 0;
            double valorCxPAnt = 0;
            double saldoCxC = 0;
            double saldoCxCAnt = 0;
            double saldoCxP = 0;
            double saldoCxPAnt = 0;

            for (int i = 0; i < (sender as SfDataGrid).View.Records.Count; i++)
            {
                int tipapli = Convert.ToInt32(provider.GetValue(records[i].Data, "tip_apli").ToString());
                if (tipapli == 3)
                {
                    valorCxC += Convert.ToDouble(provider.GetValue(records[i].Data, "valor").ToString());
                    saldoCxC += Convert.ToDouble(provider.GetValue(records[i].Data, "saldo").ToString());
                    //                    valordoc += Convert.ToDouble(provider.GetValue(records[i].Data, "valor").ToString());
                    //                    saldodoc += Convert.ToDouble(provider.GetValue(records[i].Data, "saldo").ToString());
                }
                if (tipapli == 4)
                {
                    valorCxCAnt += Convert.ToDouble(provider.GetValue(records[i].Data, "valor").ToString());
                    saldoCxCAnt += Convert.ToDouble(provider.GetValue(records[i].Data, "saldo").ToString());
                    //                    valordoc += Convert.ToDouble(provider.GetValue(records[i].Data, "valor").ToString());
                    //                    saldodoc += Convert.ToDouble(provider.GetValue(records[i].Data, "saldo").ToString());
                }

            }
            //double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(valor)", "tip_apli=3").ToString(), out valorCxC);
            //double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(valor)", "tip_apli=4").ToString(), out valorCxCAnt);
            //double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(valor)", "tip_apli=1").ToString(), out valorCxP);
            //double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(valor)", "tip_apli=2").ToString(), out valorCxPAnt);
            //double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(saldo)", "tip_apli=3").ToString(), out saldoCxC);
            //double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(saldo)", "tip_apli=4").ToString(), out saldoCxCAnt);
            //double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(saldo)", "tip_apli=1").ToString(), out saldoCxP);
            //double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(saldo)", "tip_apli=2").ToString(), out saldoCxPAnt);


            //double valorA = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(valor)", "tip_apli=1 or tip_apli=4").ToString());
            //double saldo = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(saldo)", "tip_apli=2 or tip_apli=3").ToString());
            TextCxC.Text = valorCxC.ToString("C");
            TextCxCAnt.Text = valorCxCAnt.ToString("C");
            TextCxCAbono.Text = (valorCxC - saldoCxC).ToString("C");
            TextCxCAntAbono.Text = (valorCxCAnt - saldoCxCAnt).ToString("C");
            TextCxCSaldo.Text = saldoCxC.ToString("C");
            TextCxCAntSaldo.Text = saldoCxCAnt.ToString("C");
            TotalCxc.Text = (valorCxC - valorCxCAnt - valorCxP + valorCxPAnt).ToString("C");
            TotalAbono.Text = ((valorCxC - saldoCxC) - (valorCxCAnt - saldoCxCAnt)).ToString("C");
            TotalSaldo.Text = (saldoCxC - saldoCxCAnt - saldoCxP + saldoCxPAnt).ToString("C");



            //TextTotalDoc.Text = (valordoc-valordocA).ToString("C");
            //TextSaldo.Text = (saldodoc-saldodocA).ToString("C");
        }

        private void BtnRCaja_Click(object sender, RoutedEventArgs e)
        {
            SiaWin.ValReturn = null;
            DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];
            if (row == null)
            {
                MessageBox.Show("Registro sin datos");
                return;
            }
            string cod_cli = row[0].ToString();
            string cod_cta = row[2].ToString();
            if (string.IsNullOrEmpty(cod_cli)) return;
            //MessageBox.Show(cod_cli + "-" + cod_cta);

            SiaWin.ValReturn = cod_cli;
            //Window ww = SiaWin.WindowExt(9299, "RecibosDeCaja");  //carga desde sql

            //ww.ShowInTaskbar = false;
            //ww.Owner = Application.Current.MainWindow;
            //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //ww.ShowDialog();
            //ww = null;


            dynamic ww = SiaWin.WindowExt(9305, "RecibosDeCaja");  //carga desde sql
            ww.ShowInTaskbar = false;
            ww.Owner = Application.Current.MainWindow;
            ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ww.idemp = idemp;
            ww.fechaPublic = FechaIni.Text;
            ww.codpvta = codpvta;
            ww.codter = cod_cli;
            ww.ShowDialog();
            ww = null;






        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            tabitem.Cerrar(0);


        }

        private void TextCod_Ter_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TextCod_Ter.Text.Trim() == "") TextNombreTercero.Text = "";
        }

        private void Imprimir_Click(object sender, RoutedEventArgs e)
        {
            if (DtCartera == null) return;
            if (DtCartera.Rows.Count <= 0) return;


            try
            {
                if (comboBoxCuentas.SelectedIndex < 0)
                {
                    MessageBox.Show("Seleccione una cuenta");
                    comboBoxCuentas.Focus();
                    return;
                }
                if (CmbTipoDoc.SelectedIndex < 0)
                {
                    MessageBox.Show("Seleccione un reporte..");
                    CmbTipoDoc.Focus();
                    return;
                }
                string Cta = "";
                if (comboBoxCuentas.SelectedIndex >= 0)
                {
                    foreach (DataRowView ob in comboBoxCuentas.SelectedItems)
                    {
                        //dr["cod_ter"].ToString();
                        String valueCta = ob["cod_cta"].ToString().Trim();

                        Cta += valueCta + ",";
                        //MessageBox.Show(valueOfItem1.ToString());
                    }
                    string ss = Cta.Trim().Substring(Cta.Trim().Length - 1);
                    if (ss == ",") Cta = Cta.Substring(0, Cta.Trim().Length - 1);

                }

                if (Cta == "") return;
                //MessageBox.Show(Cta);
                List<ReportParameter> parameters = new List<ReportParameter>();
                ReportParameter paramcodemp = new ReportParameter();
                paramcodemp.Values.Add(codemp);
                paramcodemp.Name = "codemp";
                parameters.Add(paramcodemp);

                ReportParameter paramfechaini = new ReportParameter();

                //MessageBox.Show("FECHA PASABLE:"+ FechaIni.Text);0
                //MessageBox.Show("FECHA COMO STRING:" + FechaIni.ToString());

                paramfechaini.Values.Add(FechaIni.SelectedDate.Value.ToShortDateString());
                //paramfechaini.Values.Add("08-15-2019");
                
                //string xx = DateTime.ParseExact(FechaIni.SelectedDate.Value.ToString(), "MM/dd/yyyy", CultureInfo.InvariantCulture).ToShortDateString();
                //MessageBox.Show(xx);
                //fecha_ini.SelectedDate.Value.ToShortDateString();
                paramfechaini.Values.Add(FechaIni.SelectedDate.Value.ToShortDateString());
                paramfechaini.Name = "Fecha";
                parameters.Add(paramfechaini);
                

                ReportParameter paramCtaIni = new ReportParameter();
                paramCtaIni.Name = "Cta";
                paramCtaIni.Values.Add(Cta);


                parameters.Add(paramCtaIni);

                ReportParameter paramTer = new ReportParameter();
                paramTer.Values.Add(TextCod_Ter.Text.Trim());
                paramTer.Name = "Ter";
                parameters.Add(paramTer);


                ReportParameter paramTrnCo = new ReportParameter();
                paramTrnCo.Values.Add("");
                paramTrnCo.Name = "TrnCo";
                parameters.Add(paramTrnCo);

                ReportParameter paramNumCo = new ReportParameter();
                paramNumCo.Values.Add("");
                paramNumCo.Name = "NumCo";
                parameters.Add(paramNumCo);

                ReportParameter paramCco = new ReportParameter();
                paramCco.Values.Add("");
                paramCco.Name = "Cco";
                parameters.Add(paramCco);

                ReportParameter paramResumen = new ReportParameter();

                int baltercero = 0; //resumida 
                if (CmbTipoDoc.SelectedIndex == 1) baltercero = 1; //detallada

                paramResumen.Values.Add(baltercero.ToString());
                paramResumen.Name = "Resumen";
                parameters.Add(paramResumen);

                ReportParameter paramTipApli = new ReportParameter();
                paramTipApli.Values.Add("1");
                paramTipApli.Name = "TipoApli";
                parameters.Add(paramTipApli);

                string TipoReporte = @"/CuentasPorCobrar/CuentasPorCobrarResumida";
                if (CmbTipoDoc.SelectedIndex == 1) TipoReporte = @"/CuentasPorCobrar/CuentasPorCobrarDetalladas";
                SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idemp, 0, 0, 0, "Consulto Cartera - Imprimio :" + Cta + " Fecha:" + paramfechaini + " - " + tabitem.Title+" Reporte:"+ TipoReporte, "");
                string TituloReport = "Cuentas por Cobrar Resumida -";
                if (CmbTipoDoc.SelectedIndex == 1) TituloReport = "Cuentas por Cobrar Detallada -";

                //public Reportes(List<ReportParameter> parameters, string reporteNombre, string TituloReporte = "", bool DirecPrinter = false, int Copias = 1, string PrintName = "", int ZoomPercent = 0, int idemp = -1)
                SiaWin.Reportes(parameters, TipoReporte, TituloReporte:TituloReport ,Modal: true, idemp:idemp);
                //-ReportCxC rp = new ReportCxC(parameters, TipoReporte);
                //parameters, @"/Contabilidad/Balances/BalanceGeneral"
                //-rp.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                //-rp.Owner = SiaWin;
                //-rp.Show();
                //-rp = null;

            }
            catch (Exception ex)
            {
                MessageBox.Show("error en los parametros:"+ex);
            }

        }
        private DataTable LoadData(string _Fi, string _Ff, string _C1, string _C2, string _N1, string _N2, string _tip, int _TipoBalNiif)
        {
            try
            {
                //MessageBox.Show(_C1 + "/" + _C2);
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpSpCoBalance", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fechaini", _Fi);
                cmd.Parameters.AddWithValue("@fechafin", _Ff);
                cmd.Parameters.AddWithValue("@ctaini", _C1);
                cmd.Parameters.AddWithValue("@ctafin", _C2);
                cmd.Parameters.AddWithValue("@ctanivini", _N1);
                cmd.Parameters.AddWithValue("@ctanivfin", _N2);
                cmd.Parameters.AddWithValue("@tipobalance", _tip);
                cmd.Parameters.AddWithValue("@balanceniif", _TipoBalNiif);
                cmd.Parameters.AddWithValue("@codEmp", codemp);
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                con.Close();
                //MessageBox.Show(ds.Tables[0].Rows.Count.ToString());
                return ds.Tables[0];
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message,"Loaddata");
                return null;
            }
        }


        private async void ConciliarCxcCo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DtCartera == null) return;
                if (DtCartera.Rows.Count <= 0) return;

                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;

                dataGridCxC.Opacity = 0.5;
                sfBusyIndicator.IsBusy = true;


                string fec_ini = "01/01/"+FechaIni.SelectedDate.Value.Year.ToString();
                string fec_Corte = FechaIni.Text.ToString();
                string cuentas = CountSelected();
                SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idemp, 0, 0, 0, "Consulto Cartera - Conciliar Cuentas CxC:" + Cuentas + " Fecha:" + fec_ini.ToString() + "/" + fec_Corte.ToString() + " - " + tabitem.Title, "");
                //MessageBox.Show(cuentas+fec_ini.ToString()+" fecha-"+fec_Corte.ToString());
                var slowTask = Task<DataTable>.Factory.StartNew(() => conciliar(fec_ini, fec_Corte, cuentas, source.Token), source.Token);
                await slowTask;

                if (((DataTable)slowTask.Result).Rows.Count > 0)
                {
                    dataGridCxC.Opacity = 1;
                    sfBusyIndicator.IsBusy = false;
                    BrowMini w = new BrowMini();
                    w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    w.dt = ((DataTable)slowTask.Result);

                    w.ShowDialog();

                }
                else
                {
                    MessageBox.Show("No existen diferencias entre modulo contable y cxc");
                    dataGridCxC.Opacity = 1;
                    sfBusyIndicator.IsBusy = false;

                }


            }
            catch (Exception w)
            {
                MessageBox.Show("errro en el subproceso:" + w, "ConciliarCxcCo");
            }
        }

        public DataTable conciliar(string dateIni, string dataFec, string cuentas, CancellationToken cancellationToken)
        {
            try
            {
                
                DataTable DtConsiliado = new DataTable();
                DtConsiliado.Columns.Add("cuenta");
                DtConsiliado.Columns.Add("cod_ter");
                DtConsiliado.Columns.Add("nom_ter");
                DtConsiliado.Columns.Add("saldo_cartera");
                DtConsiliado.Columns.Add("saldo_contabilidad");
                //DtConsiliado.Rows.Add("510506","72181539" , 0,0);
                //DataTable DtSaldosCta = LoadData(dateIni, dataFec, CountSelected(), "", "1", "9", "1", 0);
                DataTable DtSaldosCta = LoadData(dateIni, dataFec, cuentas, "", "1", "9", "1", 0);
                DataTable DtCarteraTemp = DtCartera;
                //SiaWin.Browse(DtSaldosCta);
                foreach (System.Data.DataRow dr in DtSaldosCta.Rows)
                {
                    if (dr["tipo"].ToString().Trim() == "t" && Convert.ToDecimal(dr["sal_fin"]) != 0)
                    {
                        System.Data.DataRow[] result = DtCarteraTemp.Select("cod_ter='" + dr["cod_ter"] + "' and cod_cta='" + dr["cod_cta"] + "' ");

                        if (result.Length > 0)
                        {
                            foreach (System.Data.DataRow row in result)
                            {
                                if (Convert.ToDecimal(row["saldo"]) != Convert.ToDecimal(dr["sal_fin"]))
                                {
                                    DtConsiliado.Rows.Add(row["cod_cta"].ToString(), row["cod_ter"].ToString(), row["nom_ter"].ToString(), row["saldo"].ToString(), dr["sal_fin"].ToString());
                                }
                            }
                        }
                        else
                        {
                            //agrego los que estan en contabilidad pero no en cartera 
                            if (Convert.ToDecimal(dr["sal_fin"]) != 0)
                            {
                                DtConsiliado.Rows.Add(dr["cod_cta"].ToString(), dr["cod_ter"].ToString(), dr["nom_ter"].ToString(), 0, dr["sal_fin"].ToString());
                            }
                        }
                    }
                }
                foreach (System.Data.DataRow dr in DtCartera.Rows)
                {
                    System.Data.DataRow[] result = DtSaldosCta.Select("cod_ter='" + dr["cod_ter"] + "' and cod_cta='" + dr["cod_cta"] + "' ");
                    if (result.Length > 0) { }
                    else
                    {
                        if (Convert.ToDecimal(dr["saldo"]) != 0)
                        {
                            DtConsiliado.Rows.Add(dr["cod_cta"].ToString(), dr["cod_ter"].ToString(), dr["nom_ter"].ToString(), dr["saldo"].ToString(), 0);
                        }
                    }
                }
                return DtConsiliado;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message , "Conciliar");
                return null;
            }
        }
            public string CountSelected()
            {
                string Cta = "";
                if (comboBoxCuentas.SelectedIndex >= 0)
                {
                    foreach (DataRowView ob in comboBoxCuentas.SelectedItems)
                    {
                        String valueCta = ob["cod_cta"].ToString();
                        Cta += valueCta + ",";
                    }
                    string ss = Cta.Trim().Substring(Cta.Trim().Length - 1);
                    if (ss == ",") Cta = Cta.Substring(0, Cta.Trim().Length - 1);
                }
                return Cta;
            }
        }
   
}