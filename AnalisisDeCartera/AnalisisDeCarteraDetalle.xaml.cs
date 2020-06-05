using Microsoft.Win32;
using Syncfusion.XlsIO;
using System.Windows;
using Syncfusion.UI.Xaml.Grid.Converter;
using System.IO;
using Syncfusion.UI.Xaml.Grid;
using System;
using Microsoft.Reporting.WinForms;
using System.Collections.Generic;

namespace AnalisisDeCartera
{
    /// <summary>
    /// Lógica de interacción para AnalisisDeCarteraDetalle.xaml
    /// </summary>
    public partial class AnalisisDeCarteraDetalle : Window
    {
        public string codemp = string.Empty;
        public string fechacorte = string .Empty;
        dynamic SiaWin;
        public AnalisisDeCarteraDetalle()
        {
            InitializeComponent();
            dataGridCxC.ClearFilters();
            SiaWin = Application.Current.MainWindow;
            //dataGridCxC_FilterChanged1();
        }
        private void Button_Click_Xls(object sender, RoutedEventArgs e)
        {
            var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
            options.ExcelVersion = ExcelVersion.Excel2013;
            var excelEngine = dataGridCxC.ExportToExcel(dataGridCxC.View, options);
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
                    //Launching the Excel file using the default Application.[MS Excel Or Free ExcelViewer]
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
            }
        }

        private void ExportarXls_Click(object sender, RoutedEventArgs e)
        {
            var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
            options.ExcelVersion = ExcelVersion.Excel2013;
            var excelEngine = dataGridCxC.ExportToExcel(dataGridCxC.View, options);
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
                    //Launching the Excel file using the default Application.[MS Excel Or Free ExcelViewer]
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
            }
        }

        private void dataGridCxC_FilterChanged(object sender, Syncfusion.UI.Xaml.Grid.GridFilterEventArgs e)
        {
            dataGridCxC_FilterChanged1();

        }
        public void dataGridCxC_FilterChanged1()
        {
            //MessageBox.Show("1");
            // MessageBox.Show("filter:"+( sender as SfDataGrid).View.Records.Count.ToString());
            //            var columnName = e.Column.MappingName;
            //          var filteredResult =(sender as SfDataGrid).View.Records.Select(recordentry => recordentry.Data);
            //        var recordEntry = (sender as SfDataGrid).View.Records;
            var provider = dataGridCxC.View.GetPropertyAccessProvider();
            var records = dataGridCxC.View.Records;
            //Gets the value for frozen rows count of corresponding column and removes it from FilterElement collection.
            double valorCxC = 0;
            double valorCxCAnt = 0;
            double valorCxP = 0;
            double valorCxPAnt = 0;
            double saldoCxC = 0;
            double saldoCxCAnt = 0;
            double saldoCxP = 0;
            double saldoCxPAnt = 0;
            for (int i = 0; i < dataGridCxC.View.Records.Count; i++)
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ReImprimir_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                string cta = TextCuenta.Text.Trim();
                List<ReportParameter> parameters = new List<ReportParameter>();
                ReportParameter paramcodemp = new ReportParameter();
                paramcodemp.Values.Add(codemp);
                paramcodemp.Name = "codemp";
                parameters.Add(paramcodemp);

                ReportParameter paramfechaini = new ReportParameter();
                paramfechaini.Values.Add(fechacorte);
                //fecha_ini.SelectedDate.Value.ToShortDateString()
                paramfechaini.Name = "Fecha";
                parameters.Add(paramfechaini);

                ReportParameter paramCtaIni = new ReportParameter();
                paramCtaIni.Name = "Cta";
                //MessageBox.Show(TextCuenta.Text);
                paramCtaIni.Values.Add(TextCuenta.Text.Trim());
                parameters.Add(paramCtaIni);

                ReportParameter paramTer = new ReportParameter();
                paramTer.Values.Add(TextCodigo.Text.Trim());
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
                paramResumen.Values.Add("1");
                paramResumen.Name = "Resumen";
                parameters.Add(paramResumen);

                ReportParameter paramTipApli = new ReportParameter();
                paramTipApli.Values.Add("1");
                paramTipApli.Name = "TipoApli";
                parameters.Add(paramTipApli);
                //public Reportes(List<ReportParameter> parameters, string reporteNombre, string TituloReporte = "", bool DirecPrinter = false, int Copias = 1, string PrintName = "", int ZoomPercent = 0, int idemp = -1)
               SiaWin.Reportes(parameters, @"/CuentasPorCobrar/CuentasPorCobrarDetalladas", TituloReporte: "Cuentas por Cobrar -", Modal: true);

                //-ReportCxC rp = new ReportCxC(parameters, @"/CuentasPorCobrar/CuentasPorCobrarDetalladas");
                //parameters, @"/Contabilidad/Balances/BalanceGeneral"
                //-rp.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                //-rp.Owner = this;
                //-rp.Show();
                //-rp = null;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }


    }

}
