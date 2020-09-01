using Microsoft.Reporting.WinForms;
using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
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
using System.Windows.Shapes;

namespace Co_Balance
{ 
    public partial class BalanceAcumuladoCuenta : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        public int moduloid = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public string fechaba = "";
        public DateTime fechafin;
        public string tercero = "";
        public string cuenta = "";
        public string nomcta = "";
        public string nomter = "";
        public int tipo = 0;


        DateTime fec;
        string fecha;
        string fefin;
        string ter;
        string cta;
        int tipoblc;
        public BalanceAcumuladoCuenta()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
        }

        private void LoadConfig()
        {
            try
            {
                SiaWin = Application.Current.MainWindow;
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                this.Title = "Balance Acumulado Cuenta:" + cuenta.Trim() + "-" + nomcta.Trim() + " -Empresa:" + cod_empresa + " - " + aliasemp;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadConfig();

                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                sfBusyIndicator.IsBusy = true;
                GridBalance.ClearFilters();
                GridBalance.ItemsSource = null;

                fec = Convert.ToDateTime(fechaba.ToString());
                fecha = fec.Year.ToString();
                fefin = fechafin.ToString();
                ter = tercero;
                cta = cuenta;
                tipoblc = tipo;

                //if (SiaWin._UserId == 21 || SiaWin._UserId == 235)
                //{
                //MessageBox.Show("fec:"+ fec);
                //MessageBox.Show("fecha:" + fecha);
                //MessageBox.Show("fefin:" + fefin);
                //MessageBox.Show("ter:" + ter);
                //MessageBox.Show("cta:" + cta);
                //MessageBox.Show("tipoblc:" + tipoblc);
                //}

                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(fecha, fechafin.ToString(), ter, cta, tipoblc, cod_empresa, source.Token), source.Token);
                await slowTask;

                if (((DataSet)slowTask.Result) == null)
                {
                    this.sfBusyIndicator.IsBusy = false;
                    Tx_registros.Text = "0";
                    MessageBox.Show("cuenta si movientos", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    GridBalance.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                    Tx_registros.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();
                }

                this.sfBusyIndicator.IsBusy = false;
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



        private DataSet LoadData(string fecha, string fechafin, string ter, string cta, int tipblc, string empresas, CancellationToken cancellationToken)
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


        private static void CellExportingHandler(object sender, GridCellExcelExportingEventArgs e)
        {
            e.Range.CellStyle.Font.Size = 10;
            e.Range.CellStyle.Font.FontName = "Segoe UI";
            if (e.ColumnName == "sal_ini" || e.ColumnName == "debitos" || e.ColumnName == "creditos" || e.ColumnName == "sal_fin")
            {
                double value = 0;
                if (double.TryParse(e.CellValue.ToString(), out value))
                {
                    e.Range.Number = value;
                }
                e.Handled = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExportMode = ExportMode.Value;
                options.ExcelVersion = ExcelVersion.Excel2013;
                options.CellsExportingEventHandler = CellExportingHandler;


                var excelEngine = GridBalance.ExportToExcel(GridBalance.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];
                workBook.Worksheets[0].AutoFilters.FilterRange = workBook.Worksheets[0].UsedRange;
                workBook.ActiveSheet.Columns[2].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[3].NumberFormat = "0.0";
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

        private void BtnDetalle_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                DetalleCta();
            }
            catch (Exception w)
            {
                MessageBox.Show("error el el detalle:" + w);
            }
        }

        private void DetalleCta()
        {
            try
            {
                //MessageBox.Show("la pantalla esta en mantenimineto por favor espere");


                DataRowView row = (DataRowView)GridBalance.SelectedItems[0];
                string cod_cli = tercero;
                string cod_cta = row["cod_cta"].ToString().Trim();

                //MessageBox.Show("fechaba.ToString():"+ fechaba.ToString());
                //MessageBox.Show("fechafin.ToString() :" + fechafin.ToString());
                //MessageBox.Show("cod_cta.ToString() :" + cod_cta.ToString());
                //MessageBox.Show("tipo.ToString() :" + tipo);
                //MessageBox.Show("cod_cli :" + cod_cli);
                //"Jan 1, 2009";
                string dateInput = "01/" + row["per_doc"].ToString() + "/" + row["ano"].ToString();
                DateTime fecinicial = DateTime.Parse(dateInput);

                int mes = fecinicial.Month == 12 ? fecinicial.Month : fecinicial.Month + 1;

                DateTime fechafinal = fecinicial.Month == 12 ? new DateTime(fecinicial.Year, 12, 31) : new DateTime(fecinicial.Year, mes, 1).AddDays(-1);

                //DateTime fechafinal = new DateTime(fecinicial.Year, mes, 1).AddDays(-1);

                if (fecinicial.Month == 13) return;

                StringBuilder sb = new StringBuilder();
                sb.Append(" declare @fechaIni as date ; set @fechaIni='" + fecinicial.ToString("dd/MM/yyyy") + "';declare @fechaFin as date ; set @fechaFin='" + fechafinal.ToString("dd/MM/yyyy") + "'");
                sb.Append(" SELEct cab_doc.idreg ,cue_doc.idreg as idregcue,cab_doc.cod_trn,cab_doc.num_trn,cab_doc.fec_trn,cue_doc.cod_cta,cue_doc.cod_cco,cue_doc.cod_ter,comae_ter.nom_ter,");
                sb.Append(" cue_doc.doc_ref,cue_doc.doc_cruc,cue_doc.num_chq,cue_doc.bas_mov,cue_doc.deb_mov,cue_doc.cre_mov, cab_DOC.factura,des_mov ");
                sb.Append(" FROM coCUE_DOC cue_doc inner join cocab_doc as cab_doc on cab_doc.idreg = cue_doc.idregcab and cue_doc.cod_cta = '" + cod_cta.Trim() + "' and ");
                if (cod_cli != "") sb.Append(" cue_doc.cod_ter='" + cod_cli.Trim() + "' and  ");

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

                //Co_BalanceAux WinDetalle = new Co_BalanceAux(idemp, 1, SiaWin);
                dynamic WinDetalle = SiaWin.WindowExt(9657, "Co_BalanceAux");
                WinDetalle.idemp = idemp;
                WinDetalle.moduloid = moduloid;

                WinDetalle.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                //MessageBox.Show("A2.1");
                if (string.IsNullOrEmpty(cod_cli.Trim()))
                {
                    WinDetalle.LabelTercero.Visibility = Visibility.Hidden;
                    WinDetalle.TextCodigoTer.Visibility = Visibility.Hidden;
                    WinDetalle.TextNombreTer.Visibility = Visibility.Hidden;
                    WinDetalle.TextCodigoTer.Text = cod_cli;
                    WinDetalle.TextNombreTer.Text = tercero;
                    if (tipo == 0) WinDetalle.TextNombreTipoAux.Text = "Fiscal";
                    if (tipo == 1) WinDetalle.TextNombreTipoAux.Text = "NIIF";
                }
                else
                {
                    //MessageBox.Show("A2.4");
                    WinDetalle.LabelTercero.Visibility = Visibility.Visible;
                    WinDetalle.TextCodigoTer.Visibility = Visibility.Visible;
                    WinDetalle.TextNombreTer.Visibility = Visibility.Visible;
                    WinDetalle.TextCodigoTer.Text = cod_cli;
                    WinDetalle.TextNombreTer.Text = tercero;
                }

                //MessageBox.Show("A2.5");
                WinDetalle.TextCodigoCta.Text = cod_cta;
                WinDetalle.TextNombreCta.Text = cod_cta;
                WinDetalle.Title = "Auxiliar de Cuenta - Fecha De Corte:" + fechaba.ToString() + " / " + fechafin.ToString();
                WinDetalle.dataGrid.ItemsSource = DtAuxCtaTer.DefaultView;
                // parametros reportes
                WinDetalle.fecha_ini = fechaba.ToString();
                WinDetalle.fecha_fin = fechafin.ToString();
                WinDetalle.codemp = cod_empresa;

                //MessageBox.Show("A3");
                // TOTALIZA 
                //popo
                //MessageBox.Show("la pantalla esta en mantenimineto por favor espere");
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
                WinDetalle.TextSaldoAnterior.Text = Convert.ToDouble(row["sal_ini"].ToString()).ToString("C");
                WinDetalle.TextAcumDebito.Text = Convert.ToDouble(row["debitos"].ToString()).ToString("C");
                WinDetalle.TextAcumCredito.Text = Convert.ToDouble(row["creditos"].ToString()).ToString("C");
                WinDetalle.TextSaldoFin.Text = Convert.ToDouble(row["sal_fin"].ToString()).ToString("C");
                WinDetalle.Owner = SiaWin;
                //WinDetalle.dataGridCxC_FilterChanged1();
                WinDetalle.ShowDialog();
                WinDetalle = null;
                //ImprimirDoc(Convert.ToInt32(numtrn), "Reimpreso");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void BtnReporte_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<ReportParameter> parameters = new List<ReportParameter>();
                ReportParameter paramcodemp = new ReportParameter();
                paramcodemp.Values.Add(fecha);
                paramcodemp.Name = "ano";
                parameters.Add(paramcodemp);
                ReportParameter param1 = new ReportParameter();
                param1.Values.Add(fefin);
                param1.Name = "fechafin";
                parameters.Add(param1);
                ReportParameter param2 = new ReportParameter();
                string tercero = string.IsNullOrEmpty(ter) ? " " : ter;
                param2.Values.Add(tercero);
                param2.Name = "ter";
                parameters.Add(param2);
                ReportParameter param3 = new ReportParameter();
                param3.Values.Add(cta);
                param3.Name = "cta";
                parameters.Add(param3);
                ReportParameter param4 = new ReportParameter();
                param4.Values.Add(tipoblc.ToString());
                param4.Name = "tipoblc";
                parameters.Add(param4);
                ReportParameter paramEmpresa = new ReportParameter();
                paramEmpresa.Values.Add(cod_empresa);
                paramEmpresa.Name = "codemp";
                parameters.Add(paramEmpresa);
                string repnom = @"/Contabilidad/Balances/Acmulados";
                string TituloReport = "Acumulados";
                SiaWin.Reportes(parameters, repnom, TituloReporte: TituloReport, Modal: true, idemp: idemp, ZoomPercent: 50);
            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir el reporte:" + w);
            }
        }






    }
}
