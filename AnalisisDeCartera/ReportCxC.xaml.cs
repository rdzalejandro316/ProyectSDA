using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
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
//using Microsoft.Reporting.WinForms;

namespace AnalisisDeCartera
{
    /// <summary>
    /// Lógica de interacción para ReportCxC.xaml
    /// </summary>
    public partial class ReportCxC : Window
    {
        dynamic SiaWin;


        public bool PrintOk = false;
        public ReportCxC(List<ReportParameter> parameters, string reporteNombre)
        {
            InitializeComponent();
            loaddocumento(parameters, reporteNombre);
            SiaWin = Application.Current.MainWindow;
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
                viewer.ServerReport.ReportServerUrl = new Uri("http://192.168.0.12:7333/Reportservergs");
                viewer.SetDisplayMode(DisplayMode.PrintLayout);
                viewer.ProcessingMode = ProcessingMode.Remote;
                ReportServerCredentials rsCredentials = viewer.ServerReport.ReportServerCredentials;
                rsCredentials.NetworkCredentials = new System.Net.NetworkCredential(@"grupo\ReportesGS", "Grupos44*!");
                //rsCredentials.NetworkCredentials = new System.Net.NetworkCredential(@"grupo\wilmer.barrios", "Siasoft2018*");
                List<DataSourceCredentials> crdentials = new List<DataSourceCredentials>();
                //List<ReportParameter> parameters = new List<ReportParameter>();
                
                viewer.ServerReport.SetParameters(parameter);
                
                foreach (var dataSource in viewer.ServerReport.GetDataSources())
                {
                    DataSourceCredentials credn = new DataSourceCredentials();
                    credn.Name = dataSource.Name;
                    //credn.UserId = "wilmer.barrios@siasoftsas.com";
                    //credn.Password = "Camilo654321*";
                    credn.UserId = "ReportesGS";
                    credn.Password = "Gs800061347.";
                    

                    crdentials.Add(credn);
                }
                viewer.ServerReport.SetDataSourceCredentials(crdentials);
                //                viewer.Update();
                //viewer.PrinterSettings.Copies = Convert.ToInt16(Copias);
                //viewer.ZoomPercent = 50;
                if (ZoomPercent > 0)
                {
                    viewer.ZoomMode = ZoomMode.Percent;

                    viewer.ZoomPercent = ZoomPercent;
                }
                //viewer.PrinterSettings.PrinterName = "HP DeskJet 5820 series";
                //            viewer.PrinterSettings.PrintRange = PrintRange..AllPages;

                viewer.PrinterSettings.Collate = false;
                
                viewer.RefreshReport();
                
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("llego mal el parrametro:"+ex);

                System.Windows.MessageBox.Show(ex.Message.ToString(), "DocumentosReportes-loaddocumento");
            }
        }

        private void winFormsHost_PreviewKeyDown(object sender, KeyEventArgs e)
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

        }
    }
}
