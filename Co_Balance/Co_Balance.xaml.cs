using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Text.RegularExpressions;
using Co_Balance;
using System.Text;
using System.Collections.Generic;
using Microsoft.Reporting.WinForms;
using System.ComponentModel;

namespace SiasoftAppExt
{
    /// Sia.PublicarPnt(9453,"Co_Balance");
    /// Sia.TabU(9453);
    public partial class Co_Balance : UserControl
    {

        public bool PrintOk = false;
        dynamic SiaWin;
        dynamic tabitem;
        public int idemp = 0;
        string codemp = string.Empty;
        int moduloid = 0;
        string cnEmp = "";
        DataTable DtAuxCtaTer = new DataTable();
        DataTable DtBalance = new DataTable();
        bool loaded = false;

        public Co_Balance(dynamic tabitem1)
        {
            InitializeComponent();



            SiaWin = Application.Current.MainWindow;
            tabitem = tabitem1;
            tabitem.MultiTab = true;
            if (tabitem.idemp > 0) idemp = tabitem.idemp;
            if (tabitem.idemp <= 0) idemp = SiaWin._BusinessId;

        }
        public int ZoomPercent { get; private set; } = 125;
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
                tabitem.Title = "Balance(" + aliasemp + ")";
                //TituloBalance.Text = "Balance Empresa:" + codemp + "-" + foundRow["BusinessName"].ToString().Trim();
                // fecha_ini.Text = DateTime.Now.AddMonths(-1).ToString();
                DateTime fechatemp = DateTime.Today;
                fechatemp = new DateTime(fechatemp.Year, 1, 1);
                fecha_ini.Text = fechatemp.ToString();
                fecha_fin.Text = DateTime.Now.ToString();
                C1.Text = "1";
                C2.Text = "9";
                NV1.Text = "1";
                NV2.Text = "9";
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 || e.Key == Key.Back || e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.F8 || e.Key == Key.Tab || e.Key == Key.OemComma)
            {
                e.Handled = false;
            }
            else
            {
                MessageBox.Show("este campo solo admite valores numericos");
                e.Handled = true;
            }
            try
            {
                if (e.Key == System.Windows.Input.Key.F8)
                {
                    string idTab = ((TextBox)sender).Tag.ToString();
                    if (idTab.Length > 0)
                    {
                        string tag = ((TextBox)sender).Tag.ToString();
                        string cmptabla = ""; string cmpcodigo = ""; string cmpnombre = ""; string cmporden = ""; string cmpidrow = ""; string cmptitulo = ""; string cmpconexion = ""; bool mostrartodo = true; string cmpwhere = "";
                        if (string.IsNullOrEmpty(tag)) return;
                        if (tag == "comae_cta1")
                        {
                            cmptabla = "comae_cta"; cmpcodigo = "cod_cta"; cmpnombre = "UPPER(nom_cta)"; cmporden = "cod_cta"; cmpidrow = "cod_cta"; cmptitulo = "Maestra de Cuentas"; cmpconexion = cnEmp; mostrartodo = true; cmpwhere = "";
                        }
                        if (tag == "comae_cta2")
                        {
                            cmptabla = "comae_cta"; cmpcodigo = "cod_cta"; cmpnombre = "UPPER(nom_cta)"; cmporden = "cod_cta"; cmpidrow = "cod_cta"; cmptitulo = "Maestra de Cuentas"; cmpconexion = cnEmp; mostrartodo = true; cmpwhere = "";
                        }
                        int idr = 0; string code = ""; string nom = "";
                        dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, cnEmp, mostrartodo, cmpwhere);
                        winb.ShowInTaskbar = false;
                        winb.Owner = Application.Current.MainWindow;
                        winb.ShowDialog();
                        idr = winb.IdRowReturn;
                        code = winb.Codigo;
                        nom = winb.Nombre;
                        winb = null;
                        if (idr > 0)
                        {
                            if (tag == "comae_cta1")
                            {
                                C1.Text = code.Trim(); //TBX_name_cam.Text = nom;                            
                            }
                            if (tag == "comae_cta2")
                            {
                                C2.Text = code.Trim(); //TBX_name_cam.Text = nom;                            
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        private void ValidacionNumeros(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemMinus || e.Key == Key.Subtract || e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 || e.Key == Key.Back || e.Key == Key.Left || e.Key == Key.Right)
            {
                e.Handled = false;
            }
            else
            {
                MessageBox.Show("este campo solo admite valores numericos");
                e.Handled = true;
            }
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        public Boolean validarCAmpos()
        {
            if (fecha_ini.Text.Length > 0 && fecha_fin.Text.Length > 0 && C1.Text.Length > 0 && C2.Text.Length > 0 && NV1.Text.Length > 0 && NV2.Text.Length > 0 && TipoBal.Text.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public Boolean validarFechas()
        {
            DateTime fecha1 = Convert.ToDateTime(fecha_ini.Text);
            int year1 = fecha1.Year;
            DateTime fecha2 = Convert.ToDateTime(fecha_fin.Text);
            int year2 = fecha2.Year;

            if (year1 == year2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                #region validaciones

                /// validaciones
                if (Convert.ToDateTime(fecha_ini.Text.ToString()) > Convert.ToDateTime(fecha_fin.Text.ToString()))
                {
                    MessageBox.Show("La fecha inicial debe ser menor a la fecha final....");
                    fecha_ini.Focus();
                    return;
                }
                if (fecha_ini.SelectedDate.Value.Year != fecha_fin.SelectedDate.Value.Year)
                {
                    MessageBox.Show("El año debe ser el mismo para fecha inicial y fecha final");
                    fecha_ini.Focus();
                    return;
                }
                string c1 = C1.Text.Trim();
                string c2 = C2.Text.Trim();
                if (TipoBal.SelectedIndex == 1) NV1.Text = "1";
                if (TipoBal.SelectedIndex == 1) NV2.Text = "9";

                string N1 = NV1.Text.Trim();
                string N2 = NV2.Text.Trim();
                if (string.IsNullOrEmpty(c1))
                {
                    MessageBox.Show("Falta codigo de cuenta inicial..");
                    C1.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(c2))
                {
                    MessageBox.Show("Falta codigo de cuenta final..");
                    C2.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(N1))
                {
                    MessageBox.Show("Falta nivel de cuenta inicial..");
                    NV1.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(N2))
                {
                    MessageBox.Show("Falta nivel de cuenta final..");
                    NV2.Focus();
                    return;
                }
                if (Convert.ToInt16(N1) > Convert.ToInt16(N2))
                {
                    MessageBox.Show("El nivel de cuenta inicial debe ser mayor al nivel de cuenta final...");
                    NV1.Focus();
                    return;
                }
                int __TipoBalNiif = TipoBalNiif.SelectedIndex;
                //MessageBox.Show("__TipoBalNiif"+__TipoBalNiif.ToString());
                if (__TipoBalNiif < 0)
                {
                    MessageBox.Show("Seleccione un tipo de Balance Fiscal o Niif");
                    TipoBalNiif.Focus();
                    return;
                }
                #endregion


                TxFecIni.Text = fecha_ini.Text.ToString();
                TxFecFin.Text = fecha_fin.Text.ToString();
                TxCtaIni.Text = c1;
                TxCtaFin.Text = c2;
                TxNivIni.Text = N1;
                TxNivFin.Text = N2;
                TxTer.Text = TipoBal.SelectedIndex == 0 ? "NO" : "SI";
                TxTipo.Text = TipoBalNiif.SelectedIndex == 0 ? "FISCAL" : "NIIF";
                int tipo = TipoIncluir.SelectedIndex;


                CancellationTokenSource source = new CancellationTokenSource();
                DtBalance.Clear();
                GridConfiguracion.IsEnabled = false;
                sfBusyIndicator.IsBusy = true;
                dataGridConsulta.ItemsSource = null;
                BtnEjecutar.IsEnabled = false;
                string ffi = fecha_ini.Text.ToString();
                string fff = fecha_fin.Text.ToString();
                string tipoBal = TipoBal.SelectedIndex.ToString();
                int _TipoBalNiif = TipoBalNiif.SelectedIndex;
                dataGridConsulta.ClearFilters();

                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(ffi, fff, c1, c2, N1, N2, tipoBal, _TipoBalNiif, tipo), source.Token);
                await slowTask;


                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    DtBalance = ((DataSet)slowTask.Result).Tables[0];

                    int redondeo = CbxRedondeo.SelectedIndex;

                    foreach (System.Data.DataRow item in DtBalance.Rows)
                    {
                        switch (redondeo)
                        {
                            case 1:
                                decimal sal_ant = Convert.ToDecimal(item["sal_ant"]);
                                item["sal_ant"] = Math.Round(sal_ant);
                                break;
                        }

                    }


                    dataGridConsulta.ItemsSource = DtBalance.DefaultView;
                    Total.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();

                    dataGridConsultaDetalle.ItemsSource = DtBalance.DefaultView;
                    Total.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();

                    TabControl1.SelectedIndex = 2;
                    TabControl1.SelectedIndex = 1;

                }

                BtnEjecutar.IsEnabled = true;
                //tabitem.Progreso(false);
                this.sfBusyIndicator.IsBusy = false;
                GridConfiguracion.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Opacity = 1;
            }
        }

        private DataSet LoadData(string _Fi, string _Ff, string _C1, string _C2, string _N1, string _N2, string _tip, int _TipoBalNiif, int tipo)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                cmd.CommandTimeout = 0;
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
                cmd.Parameters.AddWithValue("@IncluirCierre", tipo);
                cmd.Parameters.AddWithValue("@codEmp", codemp);
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds);

                con.Close();
                //MessageBox.Show(ds.Tables[0].Rows.Count.ToString());
                return ds;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }
        private void Cuen_GotFocus(object sender, RoutedEventArgs e)
        {
            string tag = ((TextBox)sender).Tag.ToString();

            if (tag == "comae_cta1")
            {
                F8_1.Visibility = Visibility.Visible;
            }
            if (tag == "comae_cta2")
            {
                F8_2.Visibility = Visibility.Visible;
            }
        }
        private void Cuen_LostFocus(object sender, RoutedEventArgs e)
        {
            string tag = ((TextBox)sender).Tag.ToString();

            if (tag == "comae_cta1")
            {
                F8_1.Visibility = Visibility.Hidden;

            }
            if (tag == "comae_cta2")
            {
                F8_2.Visibility = Visibility.Hidden;
            }
        }

        private static void CellExportingHandler(object sender, GridCellExcelExportingEventArgs e)
        {
            e.Range.CellStyle.Font.Size = 10;

            //e.Range.CellStyle.Font.FontName = "Segoe UI";
            if (e.ColumnName == "sal_ant" || e.ColumnName == "debito" || e.ColumnName == "credito" || e.ColumnName == "sal_fin")
            {
                double value = 0;
                if (double.TryParse(e.CellValue.ToString(), out value))
                {
                    e.Range.Number = value;
                }
                e.Handled = true;
            }
        }
        private void BTNexpo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;
                var excelEngine = dataGridConsulta.ExportToExcel(dataGridConsulta.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];
                options.ExportMode = ExportMode.Value;
                //options.CellsExportingEventHandler = CellExportingHandler;
                workBook.ActiveSheet.Columns[0].HorizontalAlignment = ExcelHAlign.HAlignLeft;
                workBook.ActiveSheet.Columns[5].NumberFormat = "0.00";
                workBook.ActiveSheet.Columns[6].NumberFormat = "0.00";
                workBook.ActiveSheet.Columns[7].NumberFormat = "0.00";
                workBook.ActiveSheet.Columns[8].NumberFormat = "0.00";
                workBook.ActiveSheet.Columns[5].HorizontalAlignment = ExcelHAlign.HAlignRight;
                workBook.ActiveSheet.Columns[6].HorizontalAlignment = ExcelHAlign.HAlignRight;
                workBook.ActiveSheet.Columns[7].HorizontalAlignment = ExcelHAlign.HAlignRight;
                workBook.ActiveSheet.Columns[8].HorizontalAlignment = ExcelHAlign.HAlignRight;

                SaveFileDialog sfd = new SaveFileDialog
                {
                    FilterIndex = 2,
                    Filter = "Excel 97 to 2003 Files(*.xls)|*.xls|Excel 2007 to 2010 Files(*.xlsx)|*.xlsx|Excel 2013 File(*.xlsx)|*.xlsx"
                };
                if (sfd.ShowDialog() == true)
                {
                    using (Stream stream = sfd.OpenFile())
                    {
                        MessageBox.Show(sfd.FilterIndex.ToString());
                        if (sfd.FilterIndex == 1)
                            workBook.Version = ExcelVersion.Excel97to2003;
                        else if (sfd.FilterIndex == 2)
                            workBook.Version = ExcelVersion.Excel2010;
                        else
                            workBook.Version = ExcelVersion.Excel2013;
                        workBook.SaveAs(stream);
                    }
                    //Message box confirmation to view the created workbook.
                    if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archvo", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        //Launching the Excel file using the default Application.[MS Excel Or Free ExcelViewer]
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void BtnDetalle_Click(object sender, RoutedEventArgs e)
        {
            DetalleCta();
        }
        private void DetalleCta()
        {
            try
            {
                DataRowView row = (DataRowView)dataGridConsulta.SelectedItems[0];
                if (row == null)
                {
                    MessageBox.Show("Registro sin datos");
                    return;
                }
                if (row["tip_cta"].ToString() == "M")
                {
                    MessageBox.Show("Solo cuentas auxiliares");
                    return;
                }
                string cod_cli = row["cod_ter"].ToString().Trim();
                string cod_cta = row["cod_cta"].ToString().Trim();


                StringBuilder sb = new StringBuilder();
                sb.Append(" declare @fechaIni as date ; set @fechaIni='" + fecha_ini.SelectedDate.Value.Date.ToShortDateString() + "';declare @fechaFin as date ; set @fechaFin='" + fecha_fin.SelectedDate.Value.Date.ToShortDateString() + "'");
                sb.Append(" SELEct cab_doc.idreg ,cue_doc.idreg as idregcue,cab_doc.cod_trn,cab_doc.num_trn,cab_doc.fec_trn,cue_doc.cod_cta,cue_doc.cod_cco,cue_doc.cod_ter,comae_ter.nom_ter,");
                sb.Append(" cue_doc.doc_ref,cue_doc.doc_cruc,cue_doc.num_chq,cue_doc.bas_mov,cue_doc.deb_mov,cue_doc.cre_mov, cab_DOC.factura,des_mov ");
                sb.Append(" FROM coCUE_DOC cue_doc inner join cocab_doc as cab_doc on cab_doc.idreg = cue_doc.idregcab and cue_doc.cod_cta = '" + cod_cta.Trim() + "' and ");
                if (cod_cli != "") sb.Append(" cue_doc.cod_ter='" + cod_cli.Trim() + "' and  ");
                if (TipoIncluir.SelectedIndex == 0) sb.Append(" convert(int,cab_doc.per_doc)<13 and  ");

                sb.Append(" year(cab_doc.fec_trn) = year(@fechaIni) and convert(date, cab_doc.fec_trn) between  @FechaIni and @FechaFin inner join comae_trn as mae_trn on mae_trn.cod_trn = cab_doc.cod_trn ");
                sb.Append(" and (mae_trn.tip_blc=0 or mae_trn.tip_blc=" + (TipoBalNiif.SelectedIndex + 1).ToString() + ")");
                sb.Append(" left join comae_ter on comae_ter.cod_ter = cue_doc.cod_ter  inner join comae_cta as comae_cta on comae_cta.cod_cta = cue_doc.cod_cta ");
                sb.Append(" and (comae_cta.tip_blc=0 or comae_cta.tip_blc=" + (TipoBalNiif.SelectedIndex + 1).ToString() + ")");
                sb.Append(" ORDER BY cod_cta,cab_doc.fec_trn ");


                DtAuxCtaTer = SiaWin.DB.SqlDT(sb.ToString(), "Dt", idemp);
                if (DtAuxCtaTer.Rows.Count == 0)
                {
                    MessageBox.Show("Sin informacion de cuenta");
                    return;
                }
                Co_BalanceAux WinDetalle = new Co_BalanceAux(idemp, moduloid);
                WinDetalle.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                if (string.IsNullOrEmpty(cod_cli.Trim()))
                {
                    WinDetalle.LabelTercero.Visibility = Visibility.Hidden;
                    WinDetalle.TextCodigoTer.Visibility = Visibility.Hidden;
                    WinDetalle.TextNombreTer.Visibility = Visibility.Hidden;
                    WinDetalle.TextCodigoTer.Text = cod_cli;
                    WinDetalle.TextNombreTer.Text = row["nom_ter"].ToString(); ;
                    if (TipoBalNiif.SelectedIndex == 0) WinDetalle.TextNombreTipoAux.Text = "Fiscal";
                    if (TipoBalNiif.SelectedIndex == 1) WinDetalle.TextNombreTipoAux.Text = "NIIF";
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
                WinDetalle.TextNombreCta.Text = row["nomcta"].ToString();
                WinDetalle.Title = "Auxiliar de Cuenta - Fecha De Corte:" + fecha_ini.ToString() + " / " + fecha_fin.Text.ToString();
                WinDetalle.dataGrid.ItemsSource = DtAuxCtaTer.DefaultView;
                // parametros reportes
                WinDetalle.fecha_ini = fecha_ini.SelectedDate.Value.ToShortDateString();
                WinDetalle.fecha_fin = fecha_fin.SelectedDate.Value.ToShortDateString();
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
                WinDetalle.TextSaldoAnterior.Text = Convert.ToDouble(row["sal_ant"].ToString()).ToString("C");
                WinDetalle.TextAcumDebito.Text = Convert.ToDouble(row["debito"].ToString()).ToString("C");
                WinDetalle.TextAcumCredito.Text = Convert.ToDouble(row["credito"].ToString()).ToString("C");
                WinDetalle.TextSaldoFin.Text = Convert.ToDouble(row["sal_fin"].ToString()).ToString("C");
                WinDetalle.Owner = SiaWin;
                WinDetalle.ShowDialog();
                WinDetalle = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }


        private void dataGridConsulta_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //            DetalleCta();            
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (loaded == true) return;
            loaded = true;
            System.Data.DataRow[] drmodulo = SiaWin.Modulos.Select("ModulesCode='CO'");
            if (drmodulo == null) this.IsEnabled = false;
            moduloid = Convert.ToInt32(drmodulo[0]["ModulesId"].ToString());
            LoadConfig();
            //LoadReporte();
            //MessageBox.Show(moduloid.ToString());
        }
        private void BTNimprimir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<ReportParameter> parameters = new List<ReportParameter>();
                ReportParameter paramcodemp = new ReportParameter();
                paramcodemp.Values.Add(codemp);
                paramcodemp.Name = "codEmp";

                parameters.Add(paramcodemp);
                ReportParameter paramfechaini = new ReportParameter();
                paramfechaini.Values.Add(fecha_ini.SelectedDate.Value.ToShortDateString());
                //fecha_ini.SelectedDate.Value.ToShortDateString()
                paramfechaini.Name = "fechaini";
                parameters.Add(paramfechaini);
                ReportParameter paramfechafin = new ReportParameter();
                paramfechafin.Values.Add(fecha_fin.SelectedDate.Value.ToShortDateString());
                //fecha_ini.SelectedDate.Value.ToShortDateString()
                paramfechafin.Name = "fechafin";
                parameters.Add(paramfechafin);
                ReportParameter paramCtaIni = new ReportParameter();
                paramCtaIni.Values.Add(C1.Text.Trim());
                paramCtaIni.Name = "ctaini";
                parameters.Add(paramCtaIni);
                ReportParameter paramCtaFin = new ReportParameter();
                paramCtaFin.Values.Add(C2.Text.Trim());
                paramCtaFin.Name = "ctafin";
                parameters.Add(paramCtaFin);
                ReportParameter paramTipBalance = new ReportParameter();
                //MessageBox.Show(TipoBal.SelectedIndex.ToString());
                string baltercero = "False";
                if (TipoBal.SelectedIndex == 1) baltercero = "True";
                paramTipBalance.Values.Add(baltercero);
                paramTipBalance.Name = "tipobalance";
                parameters.Add(paramTipBalance);
                ReportParameter paramTipBalanceNiif = new ReportParameter();
                paramTipBalanceNiif.Values.Add(TipoBalNiif.SelectedIndex.ToString());
                paramTipBalanceNiif.Name = "balanceniif";
                parameters.Add(paramTipBalanceNiif);
                ReportParameter paramCtaNivIni = new ReportParameter();
                //MessageBox.Show("NIVEL INI:" + NV1.Text.ToString().Trim());
                paramCtaNivIni.Values.Add(NV1.Text.ToString().Trim());
                paramCtaNivIni.Name = "ctanivini";
                parameters.Add(paramCtaNivIni);
                ReportParameter paramCtaNivFin = new ReportParameter();
                paramCtaNivFin.Values.Add(NV2.Text.ToString().Trim());
                paramCtaNivFin.Name = "ctanivfin";
                parameters.Add(paramCtaNivFin);

                //SiaWin.Reportes(parameters, @"/Contabilidad/Balances/BalanceGeneral", TituloReporte: "Balance General", Modal: true, idemp: idemp, ZoomPercent:50);


                //ReporteBalance rp = new ReporteBalance(parameters, getPntRepor("CO-1"), GetServer());
                ReporteBalance rp = new ReporteBalance(parameters, @"/Contabilidad/Balances/BalanceGeneral");
                rp.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                rp.Owner = SiaWin;
                rp.Show();
                rp = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }


        //public string getPntRepor(string code)
        //{
        //    string select = "select cod_Screen,code_report,name_report,ruta_report,idrowServer  from ReportPnt  where code_report='"+code+"' ";
        //    DataTable dt = SiaWin.Func.SqlDT(select, "tabla", idemp);
        //    return dt.Rows.Count > 0 ? dt.Rows[0]["name_report"].ToString() : string.Empty;
        //}

        //public Tuple<string, string, string> GetServer()
        //{
        //    string query = "select ServerIP,UserServer,UserServerPassword from ReportServer where idrow='1'  ";           
        //    DataTable dt = SiaWin.Func.SqlDT(query, "Conceptos", idemp);
        //    string serv_ip = string.Empty;
        //    string serv_user = string.Empty;
        //    string serv_pass = string.Empty;

        //    if (dt.Rows.Count > 0)
        //    {
        //        serv_ip = dt.Rows[0]["ServerIP"].ToString().Trim();
        //        serv_user = dt.Rows[0]["UserServer"].ToString().Trim();
        //        serv_pass = dt.Rows[0]["UserServerPassword"].ToString().Trim();
        //    }            
        //    var tuple = new Tuple<string, string, string>(serv_ip, serv_user, serv_pass);
        //    return tuple;
        //}



        private void TabControl1_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void viewer_Print(object sender, ReportPrintEventArgs e)
        {

            PrintOk = true;
            viewer.Focus();
            //AuditoriaDoc(DocumentoIdCab, "Imprimio ", idEmp);
        }

        private void LoadReporte()
        {
            try
            {
                viewer.Reset();
                string xnameReporte = @"/Contabilidad/Balances/AuxiliarCuenta";
                viewer.ServerReport.ReportPath = xnameReporte;
                viewer.ServerReport.ReportServerUrl = new Uri("http://siasoft:8080/ReportServer");
                viewer.SetDisplayMode(DisplayMode.Normal);
                viewer.ProcessingMode = ProcessingMode.Remote;
                ReportServerCredentials rsCredentials = viewer.ServerReport.ReportServerCredentials;

                if (ZoomPercent > 0)
                {
                    viewer.ZoomMode = ZoomMode.Percent;
                    viewer.ZoomPercent = ZoomPercent;
                }



                viewer.PrinterSettings.Collate = false;
                viewer.RefreshReport();



                // auxiliar cuenta tercero

                viewer1.Reset();
                string xnameReporte1 = @"/Contabilidad/Balances/AuxiliarTerceroCuenta";
                viewer1.ServerReport.ReportPath = xnameReporte1;
                viewer1.ServerReport.ReportServerUrl = new Uri("http://siasoft:8080/ReportServer");
                viewer1.SetDisplayMode(DisplayMode.Normal);
                viewer1.ProcessingMode = ProcessingMode.Remote;

                if (ZoomPercent > 0)
                {
                    viewer1.ZoomMode = ZoomMode.Percent;
                    viewer1.ZoomPercent = ZoomPercent;
                }
                viewer1.PrinterSettings.Collate = false;
                viewer1.RefreshReport();


                viewer2.Reset();
                string xnameReporte904 = @"/Contabilidad/Balances/ImpuestosAuxiliarCuenta904";
                viewer2.ServerReport.ReportPath = xnameReporte904;
                viewer2.ServerReport.ReportServerUrl = new Uri("http://siasoft:8080/ReportServer");
                viewer2.SetDisplayMode(DisplayMode.Normal);
                viewer2.ProcessingMode = ProcessingMode.Remote;

                if (ZoomPercent > 0)
                {
                    viewer2.ZoomMode = ZoomMode.Percent;
                    viewer2.ZoomPercent = ZoomPercent;
                }

                viewer2.PrinterSettings.Collate = false;
                viewer2.RefreshReport();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void BTNReportesAux_Click(object sender, RoutedEventArgs e)
        {
            tabItemExt3.Visibility = Visibility.Visible;
            tabItemExt4.Visibility = Visibility.Visible;
            tabItemExt5.Visibility = Visibility.Visible;
            tabItemExt3.IsSelected = true;
            LoadReporte();


        }


        private void BtnAcumAno_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                DataRowView row = (DataRowView)dataGridConsulta.SelectedItems[0];
                if (row == null)
                {
                    MessageBox.Show("Registro sin datos");
                    return;
                }

                BalanceAcumuladoCuenta win = Activator.CreateInstance<BalanceAcumuladoCuenta>();

                win.cuenta = row["cod_cta"].ToString();
                win.fechaba = fecha_ini.Text;
                win.fechafin = fecha_fin.DisplayDate;
                win.tercero = row["cod_ter"].ToString().Trim();
                win.tipo = TipoBalNiif.SelectedIndex;
                win.idemp = idemp;
                win.moduloid = moduloid;
                win.incluirCierre = TipoIncluir.SelectedIndex;
                win.nomcta = row["nom_cta"].ToString();
                win.nomter = row["nom_ter"].ToString();
                win.ShowInTaskbar = false;
                win.Owner = Application.Current.MainWindow;
                win.ShowDialog();
                //win.Close();
            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir acumulados:" + w);
            }
        }


        private void dataGridConsultaDetalle_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            try
            {
                if (dataGridConsultaDetalle.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)dataGridConsultaDetalle.SelectedItems[0];
                    string fechaba = fecha_ini.Text;
                    DateTime fechafin = fecha_fin.DisplayDate;
                    string tercero = row["cod_ter"].ToString().Trim();
                    string cuenta = row["cod_cta"].ToString();
                    int tipo = TipoBalNiif.SelectedIndex;
                    DateTime fec = Convert.ToDateTime(fechaba.ToString());
                    LoadAño(fec.Year.ToString(), fechafin.ToString(), tercero, cuenta, tipo, codemp);

                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al ver el detalle por año:" + w);
            }
        }

        public async void LoadAño(string fecha, string fechafin, string ter, string cta, int tipoblc, string cod_empresa)
        {
            try
            {

                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                sfBusyIndicatorPeriodo.IsBusy = true;
                GridBalance.ClearFilters();
                GridBalance.ItemsSource = null;
                int cierre = TipoIncluir.SelectedIndex;

                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadDataDetalleAño(fecha, fechafin.ToString(), ter, cta, tipoblc, cod_empresa, cierre), source.Token);
                await slowTask;

                if (((DataSet)slowTask.Result) == null)
                {
                    this.sfBusyIndicator.IsBusy = false;
                    //MessageBox.Show("cuenta si movientos", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    GridBalance.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                }

                this.sfBusyIndicatorPeriodo.IsBusy = false;
            }
            catch (SqlException ex)
            {
                this.sfBusyIndicator.IsBusy = false;
                MessageBox.Show(ex.Message);
            }

            catch (Exception ex)
            {
                this.sfBusyIndicator.IsBusy = false;
                MessageBox.Show(ex.Message);
            }
        }

        private DataSet LoadDataDetalleAño(string fecha, string fechafin, string ter, string cta, int tipblc, string empresas, int cierre)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpSpMovCuenta", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ano", fecha);
                cmd.Parameters.AddWithValue("@fechafin", Convert.ToDateTime(fechafin));
                cmd.Parameters.AddWithValue("@ter", ter);
                cmd.Parameters.AddWithValue("@cta", cta);
                cmd.Parameters.AddWithValue("@tipoblc", tipblc);
                cmd.Parameters.AddWithValue("@codemp", empresas);
                cmd.Parameters.AddWithValue("@IncluirCierre", cierre);
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds);
                con.Close();
                return ds;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void BTNhidden_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tag = (sender as Button).Tag.ToString().Trim();
                if (tag == "A")
                {
                    (sender as Button).Tag = "B";
                    Grid.SetRowSpan(dataGridConsulta, 2);
                    GridParameter.Visibility = Visibility.Hidden;
                }
                else
                {
                    (sender as Button).Tag = "A";
                    Grid.SetRowSpan(dataGridConsulta, 1);
                    GridParameter.Visibility = Visibility.Visible;
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cambiar posiciones:" + w);
            }
        }

        private void BtnDetalleAño_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //MessageBox.Show("la pantalla esta en mantenimineto por favor espere");

                string fechaba = fecha_ini.Text;
                string fechafin = fecha_fin.DisplayDate.ToString();



                DataRowView row = (DataRowView)dataGridConsultaDetalle.SelectedItems[0];
                string cod_cli = row["cod_ter"].ToString().Trim();
                string tercero = row["nom_ter"].ToString().Trim();

                int tipo = TipoBalNiif.SelectedIndex;
                string cod_cta = row["cod_cta"].ToString().Trim();


                DataRowView rowPeriodo = (DataRowView)GridBalance.SelectedItems[0];

                string dateInput = "01/" + rowPeriodo["per_doc"].ToString() + "/" + rowPeriodo["ano"].ToString();
                DateTime fecinicial = DateTime.Parse(dateInput);

                int mes = fecinicial.Month == 12 ? fecinicial.Month : fecinicial.Month + 1;
                DateTime fechafinal = fecinicial.Month == 12 ? new DateTime(fecinicial.Year, 12, 31) : new DateTime(fecinicial.Year, mes, 1).AddDays(-1);



                if (fecinicial.Month == 13) return;

                StringBuilder sb = new StringBuilder();
                sb.Append(" declare @fechaIni as date ; set @fechaIni='" + fecinicial.ToString("dd/MM/yyyy") + "';declare @fechaFin as date ; set @fechaFin='" + fechafinal.ToString("dd/MM/yyyy") + "'");
                sb.Append(" SELEct cab_doc.idreg ,cue_doc.idreg as idregcue,cab_doc.cod_trn,cab_doc.num_trn,cab_doc.fec_trn,cue_doc.cod_cta,cue_doc.cod_cco,cue_doc.cod_ter,comae_ter.nom_ter,");
                sb.Append(" cue_doc.doc_ref,cue_doc.doc_cruc,cue_doc.num_chq,cue_doc.bas_mov,cue_doc.deb_mov,cue_doc.cre_mov, cab_DOC.factura,des_mov ");
                sb.Append(" FROM coCUE_DOC cue_doc inner join cocab_doc as cab_doc on cab_doc.idreg = cue_doc.idregcab and cue_doc.cod_cta = '" + cod_cta.Trim() + "' and ");
                if (cod_cli != "") sb.Append(" cue_doc.cod_ter='" + cod_cli.Trim() + "' and  ");
                if (TipoIncluir.SelectedIndex == 0) sb.Append(" convert(int,cab_doc.per_doc)<13 and  ");

                sb.Append(" year(cab_doc.fec_trn) = year(@fechaIni) and convert(date, cab_doc.fec_trn) between  @FechaIni and @FechaFin inner join comae_trn as mae_trn on mae_trn.cod_trn = cab_doc.cod_trn ");
                sb.Append(" and (mae_trn.tip_blc=0 or mae_trn.tip_blc=" + (tipo + 1).ToString() + ")");
                sb.Append(" left join comae_ter on comae_ter.cod_ter = cue_doc.cod_ter  inner join comae_cta as comae_cta on comae_cta.cod_cta = cue_doc.cod_cta ");
                sb.Append(" and (comae_cta.tip_blc=0 or comae_cta.tip_blc=" + (tipo + 1).ToString() + ")");
                sb.Append(" ORDER BY cod_cta,cab_doc.fec_trn ");

                DataTable DtAuxCtaTer = SiaWin.DB.SqlDT(sb.ToString(), "Dt", idemp);

                if (DtAuxCtaTer.Rows.Count == 0)
                {
                    MessageBox.Show("Sin informacion de cuenta", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                Co_BalanceAux WinDetalle = new Co_BalanceAux(idemp, 1);
                //dynamic WinDetalle = SiaWin.WindowExt(9657, "Co_BalanceAux");
                WinDetalle.idemp = idemp;
                //WinDetalle.moduloid = moduloid;

                WinDetalle.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                //MessageBox.Show("A2.1");
                if (string.IsNullOrEmpty(cod_cli.Trim()))
                {
                    WinDetalle.LabelTercero.Visibility = Visibility.Hidden;
                    WinDetalle.TextCodigoTer.Visibility = Visibility.Hidden;
                    WinDetalle.TextNombreTer.Visibility = Visibility.Hidden;
                    WinDetalle.TextCodigoTer.Text = cod_cli;
                    if (tipo == 0) WinDetalle.TextNombreTipoAux.Text = "Fiscal";
                    if (tipo == 1) WinDetalle.TextNombreTipoAux.Text = "NIIF";
                }
                else
                {
                    WinDetalle.LabelTercero.Visibility = Visibility.Visible;
                    WinDetalle.TextCodigoTer.Visibility = Visibility.Visible;
                    WinDetalle.TextNombreTer.Visibility = Visibility.Visible;
                    WinDetalle.TextCodigoTer.Text = cod_cli;
                    WinDetalle.TextNombreTer.Text = tercero;
                }

                WinDetalle.TextCodigoCta.Text = cod_cta;
                WinDetalle.TextNombreCta.Text = cod_cta;
                WinDetalle.Title = "Auxiliar de Cuenta - Fecha De Corte:" + fechaba.ToString() + " / " + fechafin.ToString();
                WinDetalle.dataGrid.ItemsSource = DtAuxCtaTer.DefaultView;

                WinDetalle.fecha_ini = fechaba.ToString();
                WinDetalle.fecha_fin = fechafin.ToString();
                WinDetalle.codemp = codemp;

                double valorBase;
                double valorDeb = 0;
                double valorCre = 0;
                double.TryParse(DtAuxCtaTer.Compute("Sum(bas_mov)", "").ToString(), out valorBase);
                double.TryParse(DtAuxCtaTer.Compute("Sum(deb_mov)", "").ToString(), out valorDeb);
                double.TryParse(DtAuxCtaTer.Compute("Sum(cre_mov)", "").ToString(), out valorCre);
                WinDetalle.TextBase.Text = valorBase.ToString("C");
                WinDetalle.TextDeb.Text = valorDeb.ToString("C");
                WinDetalle.TextCre.Text = valorCre.ToString("C");
                WinDetalle.TextSaldoAnterior.Text = Convert.ToDouble(rowPeriodo["sal_ini"].ToString()).ToString("C");
                WinDetalle.TextAcumDebito.Text = Convert.ToDouble(rowPeriodo["debitos"].ToString()).ToString("C");
                WinDetalle.TextAcumCredito.Text = Convert.ToDouble(rowPeriodo["creditos"].ToString()).ToString("C");
                WinDetalle.TextSaldoFin.Text = Convert.ToDouble(rowPeriodo["sal_fin"].ToString()).ToString("C");
                WinDetalle.Owner = SiaWin;
                WinDetalle.ShowDialog();
                WinDetalle = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void BTNmaeCta_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SiaWin.Tab(9277, idEmp: idemp);
            }
            catch (Exception w)
            {
                MessageBox.Show("erro al abrir la maestra de cuentas:" + w);
            }
        }



    }
}
