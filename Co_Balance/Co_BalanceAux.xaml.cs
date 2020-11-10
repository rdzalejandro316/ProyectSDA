using Microsoft.Reporting.WinForms;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace Co_Balance
{
    /// <summary>
    /// Lógica de interacción para Co_BalanceAux.xaml
    /// </summary>
    public partial class Co_BalanceAux : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        public string codemp;
        int moduloid = 0;
        public string codigoCta = string.Empty;
        public string nombreCta = string.Empty;
        public string codigoTer = string.Empty;
        public string nombreTer = string.Empty;
        public DateTime FechaCorte = DateTime.Now.Date;
        public int tipoBalance = 0;
        // parametros
        public string fecha_ini = string.Empty;
        public string fecha_fin = string.Empty;
        public bool incluircierre = true;


        public Co_BalanceAux(int IdEmp, int modid)
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            idemp = IdEmp;
            moduloid = modid;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dataGridCxC_FilterChanged(object sender, Syncfusion.UI.Xaml.Grid.GridFilterEventArgs e)
        {

        }

        private void ExportarXls_Click(object sender, RoutedEventArgs e)
        {

        }
        private void BtnDetalle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)dataGrid.SelectedItems[0];
                if (row == null) return;
                int idreg = Convert.ToInt32(row["idreg"]);

                if (idreg <= 0) return;
                //public void TabTrn(int Pnt, int idemp, bool IntoWindows = false, int idregcab = 0, int idmodulo = 0, bool WinModal = true)
                SiaWin.TabTrn(0, idemp, true, idreg, moduloid, WinModal: true);
            }
            catch (Exception w)
            {
                System.Windows.MessageBox.Show("selecione una transaccion" + w);
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                List<ReportParameter> parameters = new List<ReportParameter>();

                ReportParameter paramcodemp = new ReportParameter();
                paramcodemp.Values.Add(codemp);
                paramcodemp.Name = "codEmp";
                parameters.Add(paramcodemp);

                ReportParameter paramfechaini = new ReportParameter();
                paramfechaini.Values.Add(fecha_ini);
                paramfechaini.Name = "fechaini";
                parameters.Add(paramfechaini);

                ReportParameter paramfechafin = new ReportParameter();
                paramfechafin.Values.Add(fecha_fin);
                paramfechafin.Name = "fechafin";
                parameters.Add(paramfechafin);

                ReportParameter paramCtaIni = new ReportParameter();
                paramCtaIni.Values.Add(TextCodigoCta.Text.Trim());
                paramCtaIni.Name = "ctas";
                parameters.Add(paramCtaIni);
                
                ReportParameter paramTers = new ReportParameter();
                paramTers.Values.Add(TextCodigoTer.Text.Trim());
                paramTers.Name = "ters";
                parameters.Add(paramTers);

                ReportParameter paramCierre = new ReportParameter();
                paramCierre.Values.Add(incluircierre.ToString());
                paramCierre.Name = "IncluirCierre";
                parameters.Add(paramCierre);

                


                string repnom = string.Empty;
                if (TextCodigoTer.Text.Trim() == "") repnom = @"/Contabilidad/Balances/AuxiliarCuenta";
                if (TextCodigoTer.Text.Trim() != "") repnom = @"/Contabilidad/Balances/AuxiliarCuentaTercero";

                //System.Windows.MessageBox.Show("repnom:"+repnom);

                string TituloReport = "Auxiliar de Cuenta -";
                if (TextCodigoTer.Text.Trim() != "") TituloReport = "Auxiliar de Cuenta - Tercero -";

                //SiaWin.Reportes(parameters, repnom, TituloReporte: TituloReport, Modal: true, idemp: idemp, ZoomPercent: 50);

                ReporteBalance rp = new ReporteBalance(parameters, repnom);
                rp.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                rp.Owner = SiaWin;
                rp.Show();
                rp = null;

                //-ReporteBalance rp = new ReporteBalance(parameters, repnom);
                //-rp.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                //-rp.Owner = SiaWin;
                //-rp.Show();
                //-rp = null;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message.ToString());
            }
        }
        private static void CellExportingHandler(object sender, GridCellExcelExportingEventArgs e)
        {
            e.Range.CellStyle.Font.Size = 12;
            e.Range.CellStyle.Font.FontName = "Segoe UI";

            if (e.ColumnName == "bas_mov" || e.ColumnName == "deb_mov" || e.ColumnName == "cre_mov")
            {
                double value = 0;
                if (double.TryParse(e.CellValue.ToString(), out value))
                {
                    e.Range.Number = value;
                }
                e.Handled = true;
            }
        }

        private void BtnExportarXLS_Click(object sender, RoutedEventArgs e)
        {
            var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
            options.ExcelVersion = ExcelVersion.Excel2013;
            var excelEngine = dataGrid.ExportToExcel(dataGrid.View, options);
            var workBook = excelEngine.Excel.Workbooks[0];
            options.ExportMode = ExportMode.Value;
            options.CellsExportingEventHandler = CellExportingHandler;
            workBook.ActiveSheet.Columns[4].NumberFormat = "0.0";
            workBook.ActiveSheet.Columns[5].NumberFormat = "0.0";

            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog
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
                if (System.Windows.MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archvo", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    //Launching the Excel file using the default Application.[MS Excel Or Free ExcelViewer]
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (tipoBalance == 1) TextNombreTipoAux.Text = "Fiscal";
            if (tipoBalance == 2) TextNombreTipoAux.Text = "NIIF";
        }
    }
}
