using Microsoft.Win32;
using Syncfusion.XlsIO;
using System.Windows;
using Syncfusion.UI.Xaml.Grid.Converter;
using System.IO;
using Syncfusion.UI.Xaml.Grid;
using System;
using Microsoft.Reporting.WinForms;
using System.Collections.Generic;
using AnalisisDeCuentasPorPagar;

namespace AnalisisDeCartera
{
    /// <summary>
    /// Lógica de interacción para AnalisisDeCarteraDetalle.xaml
    /// </summary>
    public partial class AnalisisDeCuentasPorPagarDetalle : Window
    {
        dynamic SiaWin;
        public string codemp = string.Empty;
        public string fechacorte = string.Empty;

        public AnalisisDeCuentasPorPagarDetalle()
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
                if (tipapli == 1)
                {
                    valorCxP += Convert.ToDouble(provider.GetValue(records[i].Data, "valor").ToString());
                    saldoCxP += Convert.ToDouble(provider.GetValue(records[i].Data, "saldo").ToString());
                    //                    valordoc += Convert.ToDouble(provider.GetValue(records[i].Data, "valor").ToString());
                    //                    saldodoc += Convert.ToDouble(provider.GetValue(records[i].Data, "saldo").ToString());
                }
                if (tipapli == 2)
                {
                    valorCxPAnt += Convert.ToDouble(provider.GetValue(records[i].Data, "valor").ToString());
                    saldoCxPAnt += Convert.ToDouble(provider.GetValue(records[i].Data, "saldo").ToString());
                    //                    valordoc += Convert.ToDouble(provider.GetValue(records[i].Data, "valor").ToString());
                    //                    saldodoc += Convert.ToDouble(provider.GetValue(records[i].Data, "saldo").ToString());
                }

            }
            TextCxP.Text = valorCxC.ToString("C");
            TextCxPAnt.Text = valorCxCAnt.ToString("C");
            TextCxPAbono.Text = (valorCxC - saldoCxC).ToString("C");
            TextCxPAntAbono.Text = (valorCxCAnt - saldoCxCAnt).ToString("C");
            TextCxPSaldo.Text = saldoCxC.ToString("C");
            TextCxPAntSaldo.Text = saldoCxCAnt.ToString("C");
            TotalCxP.Text = (valorCxC - valorCxCAnt - valorCxP + valorCxPAnt).ToString("C");
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
                //MessageBox.Show(Cta.ToString());
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

                paramCtaIni.Values.Add(cta.ToString());
                //paramCtaIni.Values.Add("220505");
                //paramCtaIni.Values.AddRange(values.ToArray());

                //paramCtaIni.Values.AddRange(values);
                //paramCtaIni.Values[1] = "220505";

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
                paramTipApli.Values.Add("2");
                paramTipApli.Name = "TipoApli";
                parameters.Add(paramTipApli);

                //string TipoReporte = @"/CuentasPorPagar/CuentasPorPagarDetalladas";

                SiaWin.Reportes(parameters, @"/CuentasPorPagar/CuentasPorPagarDetalladas", TituloReporte: "Cuentas por Pagar -", Modal: true);
                //-ReporteCxP rp = new ReporteCxP(parameters, TipoReporte);
                //parameters, @"/Contabilidad/Balances/BalanceGeneral"
                //-rp.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                //-rp.ShowInTaskbar = false;
                //-rp.Owner = SiaWin;
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
