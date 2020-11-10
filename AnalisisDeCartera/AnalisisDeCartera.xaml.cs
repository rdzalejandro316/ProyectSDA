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
using System.Text;

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
        DataTable DtCarteraD = new DataTable();
        string codpvta = string.Empty;
        bool columndto = false;
        public AnalisisDeCartera(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            tabitem = tabitem1;
            tabitem.Title = "Analisis de Cartera";
            tabitem.Logo(9, ".png");
            tabitem.MultiTab = false;
            if (tabitem.idemp > 0) idemp = tabitem.idemp;
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

                Cuentas = SiaWin.Func.SqlDT("SELECT rtrim(cod_cta) as cod_cta,rtrim(cod_cta)+'('+rtrim(nom_cta)+')' as nom_cta FROM COMAE_CTA WHERE ind_mod = 1 and (tip_apli = 3 or tip_apli = 4 ) ORDER BY COD_CTA", "Cuentas", idemp);
                comboBoxCuentas.ItemsSource = Cuentas.DefaultView;
                //vendedor
                //DataTable dt_ven = SiaWin.Func.SqlDT("select rtrim(cod_mer) as cod_mer,rtrim(nom_mer) as nom_mer from inmae_mer where estado=1", "vendedor", idemp);
                //comboBoxVendedor.ItemsSource = dt_ven.DefaultView;

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

        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                #region validaciones

                string Cta = "";
                if (comboBoxCuentas.SelectedIndex < 0)
                {
                    MessageBox.Show("Seleccione una cuenta");
                    comboBoxCuentas.Focus();
                    return;
                }

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
                string Ven = "";
                if (Cbx_Detalle.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione el tipo de consulta");
                    return;
                }

                #endregion

                bool detalle = Cbx_Detalle.Text == "No" ? false : true;
                CancellationTokenSource source = new CancellationTokenSource();
                sfBusyIndicator.IsBusy = true;
                if (detalle == true) DtCarteraD.Clear();
                if (detalle == false) DtCartera.Clear();
                BtnEjecutar.IsEnabled = false;
                Imprimir.IsEnabled = false;
                ExportarXls.IsEnabled = false;
                ConciliarCxcCo.IsEnabled = false;
                BtnvrAbonado.IsEnabled = false;
                BtnCalcularInt.IsEnabled = false;

                string ffi = FechaIni.Text.ToString();
                string Tercero = TextCod_Ter.Text.Trim();

                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(ffi, Cta, Tercero, "", Ven, detalle), source.Token);
                await slowTask;
                BtnEjecutar.IsEnabled = true;
                Imprimir.IsEnabled = true;
                ExportarXls.IsEnabled = true;
                ConciliarCxcCo.IsEnabled = true;
                BtnvrAbonado.IsEnabled = true;
                BtnCalcularInt.IsEnabled = true;
                resetTotales();

                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    TxtRecords.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();
                    if (detalle == false)
                    {
                        //DataTable dt = ((DataSet)slowTask.Result).Tables[0];
                        //SiaWin.Browse(dt);                        
                        DtCartera = ((DataSet)slowTask.Result).Tables["C"];
                        dataGridCxC.ItemsSource = ((DataSet)slowTask.Result).Tables["C"];
                        double valorCxC, valorCxCAnt = 0;
                        //double valorCxCAnt = 0;
                        double valorCxP = 0;
                        double valorCxPAnt = 0;
                        double saldoCxC = 0;
                        double saldoCxCAnt = 0;
                        double saldoCxP = 0;
                        double saldoCxPAnt = 0;
                        double.TryParse(((DataSet)slowTask.Result).Tables["C"].Compute("Sum(valor)", "tip_apli=3").ToString(), out valorCxC);
                        double.TryParse(((DataSet)slowTask.Result).Tables["C"].Compute("Sum(valor)", "tip_apli=4").ToString(), out valorCxCAnt);
                        double.TryParse(((DataSet)slowTask.Result).Tables["C"].Compute("Sum(valor)", "tip_apli=1").ToString(), out valorCxP);
                        double.TryParse(((DataSet)slowTask.Result).Tables["C"].Compute("Sum(valor)", "tip_apli=2").ToString(), out valorCxPAnt);
                        double.TryParse(((DataSet)slowTask.Result).Tables["C"].Compute("Sum(saldo)", "tip_apli=3").ToString(), out saldoCxC);
                        double.TryParse(((DataSet)slowTask.Result).Tables["C"].Compute("Sum(saldo)", "tip_apli=4").ToString(), out saldoCxCAnt);
                        double.TryParse(((DataSet)slowTask.Result).Tables["C"].Compute("Sum(saldo)", "tip_apli=1").ToString(), out saldoCxP);
                        double.TryParse(((DataSet)slowTask.Result).Tables["C"].Compute("Sum(saldo)", "tip_apli=2").ToString(), out saldoCxPAnt);
                        TextCxC.Text = valorCxC.ToString("C");
                        TextCxCAnt.Text = valorCxCAnt.ToString("C");
                        TextCxCAbono.Text = (valorCxC - saldoCxC).ToString("C");
                        TextCxCAntAbono.Text = (valorCxCAnt - saldoCxCAnt).ToString("C");
                        TextCxCSaldo.Text = saldoCxC.ToString("C");
                        TextCxCAntSaldo.Text = saldoCxCAnt.ToString("C");
                        TotalCxc.Text = (valorCxC - valorCxCAnt - valorCxP + valorCxPAnt).ToString("C");
                        TotalAbono.Text = ((valorCxC - saldoCxC) - (valorCxCAnt - saldoCxCAnt)).ToString("C");
                        TotalSaldo.Text = (saldoCxC - saldoCxCAnt - saldoCxP + saldoCxPAnt).ToString("C");

                    }
                    else
                    {
                        DtCarteraD = ((DataSet)slowTask.Result).Tables["D"];
                        dataGridCxCD.ItemsSource = ((DataSet)slowTask.Result).Tables["D"];
                        double valorCxC, valorCxCAnt = 0;
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
                        TextCxC.Text = valorCxC.ToString("C");
                        TextCxCAnt.Text = valorCxCAnt.ToString("C");
                        TextCxCAbono.Text = (valorCxC - saldoCxC).ToString("C");
                        TextCxCAntAbono.Text = (valorCxCAnt - saldoCxCAnt).ToString("C");
                        TextCxCSaldo.Text = saldoCxC.ToString("C");
                        TextCxCAntSaldo.Text = saldoCxCAnt.ToString("C");
                        TotalCxc.Text = (valorCxC - valorCxCAnt - valorCxP + valorCxPAnt).ToString("C");
                        TotalAbono.Text = ((valorCxC - saldoCxC) - (valorCxCAnt - saldoCxCAnt)).ToString("C");
                        TotalSaldo.Text = (saldoCxC - saldoCxCAnt - saldoCxP + saldoCxPAnt).ToString("C");
                    }

                }
                else
                {
                    TxtRecords.Text = "0";
                }


                this.sfBusyIndicator.IsBusy = false;
                SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idemp, 0, 0, 0, "Consulto Cartera cuentas:" + Cta + " Fecha:" + ffi.ToString() + " - " + tabitem.Title, "");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message, "Error SiasoftApp");
                tabitem.Progreso(false);
                BtnEjecutar.IsEnabled = true;
                Imprimir.IsEnabled = true;
                ExportarXls.IsEnabled = true;
                ConciliarCxcCo.IsEnabled = true;
                sfBusyIndicator.IsBusy = false;
                tabitem.Progreso(false);
                resetTotales();
                this.Opacity = 1;
            }
        }

        private DataSet LoadData(string Fi, string ctas, string cter, string cco, string ven, bool detalle)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds1 = new DataSet();
                cmd = new SqlCommand("_empSpCoAnalisisCxc", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Ter", cter);
                cmd.Parameters.AddWithValue("@Cta", ctas);
                cmd.Parameters.AddWithValue("@TipoApli", 1);
                cmd.Parameters.AddWithValue("@Resumen", detalle == true ? 1 : 0);
                cmd.Parameters.AddWithValue("@Fecha", Fi);
                cmd.Parameters.AddWithValue("@TrnCo", "");
                cmd.Parameters.AddWithValue("@NumCo", "");
                cmd.Parameters.AddWithValue("@Cco", cco);
                cmd.Parameters.AddWithValue("@Ven", ven);
                cmd.Parameters.AddWithValue("@codemp", codemp);
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                string dataName = "C";
                if (detalle == true) dataName = "D";
                da.Fill(ds, dataName);
                con.Close();
                return ds;
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

            string Cta = "";
            if (comboBoxCuentas.SelectedIndex > 0)
            {
                foreach (DataRowView ob in comboBoxCuentas.SelectedItems)
                {
                    String valueCta = ob["cod_cta"].ToString();
                    Cta += valueCta + ",";
                }
                string ss = Cta.Trim().Substring(Cta.Trim().Length - 1);
                if (ss == ",") Cta = Cta.Substring(0, Cta.Trim().Length - 1);
            }




            try
            {
                DataRowView row = dataGridCxC.Visibility == Visibility.Visible ?
                (DataRowView)dataGridCxC.SelectedItems[0] : (DataRowView)dataGridCxCD.SelectedItems[0];
                if (row == null)
                {
                    MessageBox.Show("Registro sin datos");
                    return;
                }
                string cod_cli = row[0].ToString();
                string cod_cta = row[2].ToString();
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds1 = new DataSet();

                //string Vendedor = comboBoxVendedor.Text.Trim();
                string Tercero = TextCod_Ter.Text.Trim();
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
                cmd.Parameters.AddWithValue("@Ven", "");//if you have parameters.
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
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;


                SfDataGrid grid = dataGridCxC.Visibility == Visibility.Visible ?
                                    dataGridCxC : dataGridCxCD;

                var excelEngine = grid.ExportToExcel(grid.View, options);
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

                    //Message box confirmation to view the created workbook.
                    if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archvo",
                                        MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
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
            dataGridCxCD.ClearFilters();
            dataGridCxCD.ItemsSource = null;
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


        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            tabitem.Cerrar(0);
        }
        private void TextCod_Ter_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TextCod_Ter.Text.Trim() == "") TextNombreTercero.Text = "";
        }

        public Boolean IsNumber(String s)
        {
            Boolean value = true;
            foreach (Char c in s.ToCharArray())
            {
                value = value && Char.IsDigit(c);
            }

            return value;
        }
        private void Imprimir_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                #region validacion
                bool mm = IsNumber(TxtAltura.Text.Trim());
                if (!mm)
                {
                    MessageBox.Show("Valor de altura tiene que ser un numero valido ");
                    TxtAltura.Text = "0";
                    return;
                }
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
                        String valueCta = ob["cod_cta"].ToString().Trim();
                        Cta += valueCta + ",";
                    }
                    string ss = Cta.Trim().Substring(Cta.Trim().Length - 1);
                    if (ss == ",") Cta = Cta.Substring(0, Cta.Trim().Length - 1);
                }
                if (Cta == "") return;
                string Ven = "";

                #endregion

                List<ReportParameter> parameters = new List<ReportParameter>();
                ReportParameter paramcodemp = new ReportParameter();
                paramcodemp.Values.Add(codemp);
                paramcodemp.Name = "codemp";
                parameters.Add(paramcodemp);
                ReportParameter paramfechaini = new ReportParameter();
                paramfechaini.Values.Add(FechaIni.SelectedDate.Value.ToShortDateString());

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

                ReportParameter paramVen = new ReportParameter();
                paramVen.Values.Add(Ven.Trim());
                paramVen.Name = "Ven";
                parameters.Add(paramVen);


                ReportParameter paramResumen = new ReportParameter();

                int baltercero = 0; //resumida 
                int tipoReporte = 0; //1= reporte por vendedor,ciudad
                if (CmbTipoDoc.SelectedIndex == 1) baltercero = 1; //detallada
                if (CmbTipoDoc.SelectedIndex == 2)
                {
                    baltercero = 1; //detallada
                    tipoReporte = 1;
                }
                if (CmbTipoDoc.SelectedIndex == 3)
                {
                    baltercero = 1; //detallada
                    tipoReporte = 2;
                }


                paramResumen.Values.Add(baltercero.ToString());
                paramResumen.Name = "Resumen";
                parameters.Add(paramResumen);

                ReportParameter paramTipApli = new ReportParameter();
                paramTipApli.Values.Add("1");
                paramTipApli.Name = "TipoApli";
                parameters.Add(paramTipApli);

                if (tipoReporte > 0)
                {
                    ReportParameter paramtipoReporte = new ReportParameter();
                    paramtipoReporte.Values.Add(tipoReporte.ToString());
                    paramtipoReporte.Name = "TipoReporte";
                    parameters.Add(paramtipoReporte);
                }
                if (CmbTipoDoc.SelectedIndex == 1)
                {
                    ReportParameter paramAltura = new ReportParameter();
                    paramAltura.Values.Add(TxtAltura.Text.Trim());
                    paramAltura.Name = "Altura";
                    parameters.Add(paramAltura);
                }

                string TipoReporte = @"/CuentasPorCobrar/CuentasPorCobrarResumida";
                if (CmbTipoDoc.SelectedIndex == 1) TipoReporte = @"/CuentasPorCobrar/CuentasPorCobrarDetalladas";
                //if (CmbTipoDoc.SelectedIndex == 2) TipoReporte = @"/CuentasPorCobrar/CuentasPorCobrarDetalladasVendedor";
                //if (CmbTipoDoc.SelectedIndex == 3) TipoReporte = @"/CuentasPorCobrar/CuentasPorCobrarResumenAlturaPorVendedor";

                SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idemp, 0, 0, 0, "Consulto Cartera - Imprimio :" + Cta + " Fecha:" + paramfechaini + " - " + tabitem.Title + " Reporte:" + TipoReporte, "");
                string TituloReport = "Cuentas por Cobrar Resumida -";
                if (CmbTipoDoc.SelectedIndex == 1) TituloReport = "Cuentas por Cobrar Detallada -";
                //if (CmbTipoDoc.SelectedIndex == 2) TituloReport = "Cuentas por Cobrar Detallada - Vendedor";
                //if (CmbTipoDoc.SelectedIndex == 3) TituloReport = "Cuentas por Cobrar Altura - Vendedor";

                SiaWin.Reportes(parameters, TipoReporte, TituloReporte: TituloReport, Modal: true, idemp: idemp);

            }
            catch (Exception ex)
            {
                MessageBox.Show("error en los parametros:" + ex);
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
                MessageBox.Show(e.Message, "Loaddata");
                return null;
            }
        }

        private async void ConciliarCxcCo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Cbx_Detalle.SelectedIndex < 0)
                {
                    MessageBox.Show("Seleccione Tipo de reporte detalle =No");
                    Cbx_Detalle.Focus();
                    return;
                }

                var tag = ((ComboBoxItem)Cbx_Detalle.SelectedItem).Tag.ToString();

                if (tag == "Si")
                {
                    MessageBox.Show("Seleccione Tipo de reporte detalle =No");
                    Cbx_Detalle.Focus();
                    return;
                }

                SfDataGrid grid = dataGridCxC.Visibility == Visibility.Visible ?
                dataGridCxC : dataGridCxCD;

                if (grid.SelectedIndex < 0) return;

                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;

                grid.Opacity = 0.5;
                sfBusyIndicator.IsBusy = true;


                string fec_ini = "01/01/" + FechaIni.SelectedDate.Value.Year.ToString();
                string fec_Corte = FechaIni.Text.ToString();
                string cuentas = CountSelected();
                SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idemp, 0, 0, 0, "Consulto Cartera - Conciliar Cuentas CxC:" + Cuentas + " Fecha:" + fec_ini.ToString() + "/" + fec_Corte.ToString() + " - " + tabitem.Title, "");
                //MessageBox.Show(cuentas+fec_ini.ToString()+" fecha-"+fec_Corte.ToString());
                var slowTask = Task<DataTable>.Factory.StartNew(() => conciliar(fec_ini, fec_Corte, cuentas, source.Token), source.Token);
                await slowTask;

                if (((DataTable)slowTask.Result).Rows.Count > 0)
                {
                    grid.Opacity = 1;
                    sfBusyIndicator.IsBusy = false;
                    BrowMini w = new BrowMini();
                    w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    w.ShowInTaskbar = false;
                    w.Owner = Application.Current.MainWindow;
                    w.dt = ((DataTable)slowTask.Result);
                    w.ShowDialog();

                }
                else
                {
                    MessageBox.Show("No existen diferencias entre modulo contable y cxc");
                    grid.Opacity = 1;
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
                    if (dr["tipo"].ToString().Trim().ToLower() == "t" && Convert.ToDecimal(dr["sal_fin"]) != 0)
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
                                if (Convert.ToDecimal(row["saldo"]) < 0 || Convert.ToDecimal(dr["sal_fin"]) < 0)
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
                MessageBox.Show(ex.Message, "Conciliar");
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

        private void BtnEjecutarD_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                //string valor = Cbx_Detalle.Text;
                var tag = ((ComboBoxItem)Cbx_Detalle.SelectedItem).Tag.ToString();

                if (tag == "No")
                {
                    dataGridCxC.Visibility = Visibility.Visible;
                    dataGridCxCD.Visibility = Visibility.Hidden;
                    ConciliarCxcCo.IsEnabled = true;
                }
                else
                {
                    dataGridCxC.Visibility = Visibility.Hidden;
                    dataGridCxCD.Visibility = Visibility.Visible;
                    ConciliarCxcCo.IsEnabled = false;
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("ERROR:" + w);
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
                //public void TabTrn(int Pnt, int idemp, bool IntoWindows = false, int idregcab = 0, int idmodulo = 0, bool WinModal = true)
                SiaWin.TabTrn(0, idemp, true, idreg, 1, WinModal: true);
            }
            catch (Exception w)
            {
                System.Windows.MessageBox.Show("Error ...." + w.Message);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Cbx_Detalle.SelectedIndex = 1;

            if (SiaWin._UserTag1.Trim() != "")
            {
                //comboBoxVendedor.SelectedValue = SiaWin._UserTag1.Trim();
                //comboBoxVendedor.IsEnabled = false;
            }
        }


        private void BtnvrAbonado_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                SfDataGrid grid = dataGridCxC.Visibility == Visibility.Visible ?
                dataGridCxC : dataGridCxCD;

                if (grid.SelectedIndex >= 0)
                {
                    AbonoDocumentos view = new AbonoDocumentos(idemp);
                    DataRowView row = (DataRowView)dataGridCxCD.SelectedItems[0];
                    view.num_trn = row["num_trn"].ToString();
                    view.cod_ter = row["cod_ter"].ToString();
                    view.cod_cta = row["cod_cta"].ToString();
                    view.idemp = idemp;
                    view.ShowInTaskbar = false;
                    view.Owner = Application.Current.MainWindow;
                    view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    view.ShowDialog();
                }
                else
                {
                    MessageBox.Show("seleccione una factura");
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir el abono:" + w);
            }
        }

        private void BtnCalcularInt_Click(object sender, RoutedEventArgs e)
        {

        }


        public string getvalue(string cod_cta)
        {
            DataTable dt = SiaWin.Func.SqlDT("select * From comae_cta where cod_cta='" + cod_cta + "'", "temp", idemp);
            return dt.Rows.Count > 0 ? dt.Rows[0]["nom_cta"].ToString().Trim() : "";
        }


        private void BtnAuxliar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                SfDataGrid grid = dataGridCxC.Visibility == Visibility.Visible ?
                dataGridCxC : dataGridCxCD;

                if (grid.SelectedIndex >= 0)
                {

                    DataRowView row = (DataRowView)grid.SelectedItems[0];

                    StringBuilder sb = new StringBuilder();

                    string cod_cli = row["cod_ter"].ToString().Trim();
                    string cod_cta = row["cod_cta"].ToString().Trim();

                    DateTime fecfin = Convert.ToDateTime(FechaIni.Text);
                    string fecini = "01/01/" + fecfin.Year;


                    sb.Append(" declare @fechaIni as date ; set @fechaIni='" + fecini + "';declare @fechaFin as date ; set @fechaFin='" + fecfin.ToString("dd/MM/yyyy") + "';");

                    sb.Append(" SELEct cab_doc.idreg ,cue_doc.idreg as idregcue,cab_doc.cod_trn,cab_doc.num_trn,cab_doc.fec_trn,cue_doc.cod_cta,cue_doc.cod_cco,cue_doc.cod_ter,comae_ter.nom_ter,");
                    sb.Append(" cue_doc.doc_ref,cue_doc.doc_cruc,cue_doc.num_chq,cue_doc.bas_mov,cue_doc.deb_mov,cue_doc.cre_mov, cab_DOC.factura,des_mov ");
                    sb.Append(" FROM coCUE_DOC cue_doc inner join cocab_doc as cab_doc on cab_doc.idreg = cue_doc.idregcab and cue_doc.cod_cta = '" + cod_cta.Trim() + "' and ");
                    if (cod_cli != "") sb.Append(" cue_doc.cod_ter='" + cod_cli.Trim() + "' and  ");
                    sb.Append(" convert(int,cab_doc.per_doc)<13 and  ");

                    sb.Append(" year(cab_doc.fec_trn) = year(@fechaIni) and convert(date, cab_doc.fec_trn) between  @FechaIni and @FechaFin inner join comae_trn as mae_trn on mae_trn.cod_trn = cab_doc.cod_trn ");
                    sb.Append(" and (mae_trn.tip_blc=0 or mae_trn.tip_blc=1)");
                    sb.Append(" left join comae_ter on comae_ter.cod_ter = cue_doc.cod_ter  inner join comae_cta as comae_cta on comae_cta.cod_cta = cue_doc.cod_cta ");
                    sb.Append(" and (comae_cta.tip_blc=0 or comae_cta.tip_blc=1)");
                    sb.Append(" ORDER BY cod_cta,cab_doc.fec_trn ");


                    DataTable DtAuxCtaTer = SiaWin.DB.SqlDT(sb.ToString(), "Dt", idemp);
                    if (DtAuxCtaTer.Rows.Count == 0)
                    {
                        MessageBox.Show("Sin informacion de cuenta");
                        return;
                    }


                    dynamic WinDetalle = SiaWin.WindowExt(9687, "BalanceAuxiliar");
                    WinDetalle.idemp = idemp;
                    WinDetalle.moduloid = 1;

                    WinDetalle.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    if (string.IsNullOrEmpty(cod_cli.Trim()))
                    {
                        WinDetalle.LabelTercero.Visibility = Visibility.Hidden;
                        WinDetalle.TextCodigoTer.Visibility = Visibility.Hidden;
                        WinDetalle.TextNombreTer.Visibility = Visibility.Hidden;
                        WinDetalle.TextCodigoTer.Text = cod_cli;
                        WinDetalle.TextNombreTer.Text = row["nom_ter"].ToString(); ;
                        WinDetalle.TextNombreTipoAux.Text = "Fiscal";
                    }
                    else
                    {
                        WinDetalle.LabelTercero.Visibility = Visibility.Visible;
                        WinDetalle.TextCodigoTer.Visibility = Visibility.Visible;
                        WinDetalle.TextNombreTer.Visibility = Visibility.Visible;
                        WinDetalle.TextCodigoTer.Text = cod_cli;
                        WinDetalle.TextNombreTer.Text = row["nom_ter"].ToString(); ;
                    }


                    WinDetalle.TextCodigoCta.Text = cod_cta;
                    WinDetalle.TextNombreCta.Text = getvalue(cod_cta);
                    WinDetalle.Title = "Auxiliar de Cuenta - Fecha De Corte:" + fecini.ToString() + " / " + fecfin.ToString("dd/MM/yyyy");
                    WinDetalle.dataGrid.ItemsSource = DtAuxCtaTer.DefaultView;

                    WinDetalle.fecha_ini = fecini.ToString();
                    WinDetalle.fecha_fin = fecfin.ToString("dd/MM/yyyy");
                    WinDetalle.codemp = codemp;
                    // TOTALIZA 
                    double valorBase;
                    //double valorCxCAnt = 0;
                    double valorDeb = 0;
                    double valorCre = 0;
                    double.TryParse(DtAuxCtaTer.Compute("Sum(bas_mov)", "").ToString(), out valorBase);
                    double.TryParse(DtAuxCtaTer.Compute("Sum(deb_mov)", "").ToString(), out valorDeb);
                    double.TryParse(DtAuxCtaTer.Compute("Sum(cre_mov)", "").ToString(), out valorCre);
                    WinDetalle.TextBase.Text = valorBase.ToString("C");
                    WinDetalle.TextDeb.Text = valorDeb.ToString("C");
                    WinDetalle.TextCre.Text = valorCre.ToString("C");
                    WinDetalle.TextSaldoAnterior.Text = "0";
                    WinDetalle.TextAcumDebito.Text = "0";
                    WinDetalle.TextAcumCredito.Text = "0";
                    WinDetalle.TextSaldoFin.Text = "0";
                    WinDetalle.incluircierre = true;
                    WinDetalle.Owner = SiaWin;
                    WinDetalle.ShowDialog();
                    WinDetalle = null;



                }
                else
                {
                    MessageBox.Show("seleccione un registro", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir el auxliar de contabilidad:" + w, "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }





    }

}