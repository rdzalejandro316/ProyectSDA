using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
namespace Co_Balance
{
    /// <summary>
    /// Lógica de interacción para Reporte.xaml
    /// </summary>
    public partial class ReporteBalance
    {
        public bool PrintOk = false;
        //public string ReporteNombre = string.Empty;
        public ReporteBalance(List<ReportParameter> parameters, string reporteNombre)
        {
            InitializeComponent();
            loaddocumento(parameters, reporteNombre);
        }
        public int ZoomPercent { get; private set; } = 50;
        public void loaddocumento(List<ReportParameter> parameter, string reporteNombre)
        {
            try
            {
                

                viewer.Reset();
                //string xnameReporte = @"/Contabilidad/Balances/BalanceGeneral";
                string xnameReporte = reporteNombre;
                viewer.ServerReport.ReportPath = xnameReporte;
                viewer.ServerReport.ReportServerUrl = new Uri("http://siasoft:8080/ReportServer");
                viewer.SetDisplayMode(DisplayMode.PrintLayout);
                viewer.ProcessingMode = ProcessingMode.Remote;
                viewer.ServerReport.SetParameters(parameter);
                //ReportServerCredentials rsCredentials = viewer.ServerReport.ReportServerCredentials;
                //rsCredentials.NetworkCredentials = new System.Net.NetworkCredential(@"grupo\ReportesGS", "Grupos44*!");
                //List<DataSourceCredentials> crdentials = new List<DataSourceCredentials>();                
                //foreach (var dataSource in viewer.ServerReport.GetDataSources())
                //{
                //    DataSourceCredentials credn = new DataSourceCredentials();
                //    credn.Name = dataSource.Name;
                //    credn.UserId = "wilmer.barrios@siasoftsas.com";
                //    credn.Password = "Camilo654321*";
                //    crdentials.Add(credn);
                //}
                //viewer.ServerReport.SetDataSourceCredentials(crdentials);

                if (ZoomPercent > 0)
                {
                    viewer.ZoomMode = ZoomMode.Percent;
                    viewer.ZoomPercent = ZoomPercent;
                }
                
                viewer.PrinterSettings.Collate = false;
                viewer.RefreshReport();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message.ToString(), "DocumentosReportes-loaddocumento");
            }
        }


        private void winFormsHost_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                this.Close();
                e.Handled = true;
            }
            if (e.Key == System.Windows.Input.Key.F6)
            {
                //AutoPrint();
                PrintOk = true;
                viewer.Focus();
            }
        }
        private void viewer_Print(object sender, ReportPrintEventArgs e)
        {

            PrintOk = true;
            viewer.Focus();
            //AuditoriaDoc(DocumentoIdCab, "Imprimio ", idEmp);
        }



    }
}
