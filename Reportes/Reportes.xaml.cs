using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using Reportes;

namespace SiasoftAppExt
{
    /// <summary>
    /// Lógica de interacción para UserControl1.xaml
    /// </summary>
    public partial class Reportes : Window
    {
        //Sia.PublicarPnt(9516,"Reportes");
        dynamic SiaWin;
        public int idEmp = 0;
        string codemp = string.Empty;
        string nomemp = string.Empty;
        public int DocumentoIdCab = -1;
        public string ReportPath = string.Empty;
        public string ReportServerUrl = string.Empty;
        public string UserCredencial = string.Empty;
        public string PassCredencial = string.Empty;
        public string TituloReporte = string.Empty;
        public string UserDB = string.Empty;
        public string PassDB = string.Empty;
        public bool PrintOk = false;

        //configuracion impresora
        public string printName = string.Empty;
        public int Copias = 3;
        public bool DirecPrinter = false;
        public int ZoomPercent = 0;

        public Reportes()
        {
            SiaWin = System.Windows.Application.Current.MainWindow;
            InitializeComponent();
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            // carga codigo de empresa
            DataRow foundRow = SiaWin.Empresas.Rows.Find(idEmp);
            nomemp = foundRow["BusinessName"].ToString().Trim();
            codemp = foundRow["BusinessCode"].ToString().Trim();

            this.Title = "Empesa:" + codemp + "-" + nomemp;
            
            // PERMITE PROGRAMAR BUTTON EXPORTAR 
            var toolStrip = (ToolStrip)viewer.Controls.Find("toolStrip1", true).First();
            ((ToolStripDropDownButton)toolStrip.Items["export"]).ShowDropDownArrow = false;
            ((ToolStripDropDownButton)toolStrip.Items["export"]).DropDownOpening += (obj, arg) =>
            {
                ((ToolStripDropDownButton)obj).DropDownItems.Clear();
            };
            ((ToolStripDropDownButton)toolStrip.Items["export"]).Click += (obj, arg) =>
            {
                var pdf = viewer.LocalReport.ListRenderingExtensions()
                    .Where(x => x.Name == "PDF").First();

                viewer.ExportDialog(pdf);
            };
            
            //System.Windows.MessageBox.Show(this.ReportPath);
            //http://192.168.0.12:7333/ReportServerGS/Pages/ReportViewer.aspx?%2FInventarios%2FListaPrecios&rc:showbackbutton=true
            viewer.ServerReport.ReportPath = this.ReportPath;
            viewer.ServerReport.ReportPath = @"/Inventarios/ListaPrecios";

            //if (ZoomPercent > 0) viewer.ZoomPercent = ZoomPercent;
            viewer.ZoomPercent = 25;
            //viewer.ReportServerUrl = this.ReportServerUrl;
            //this.DocumentoIdCab = 146048;
            loaddocumento();
            //AutoPrint();

            //if (DirecPrinter == true) AutoPrint();

            this.UpdateLayout();
            viewer.Focus();

        }
        private void AutoPrint()
        {
            ReportDirect autoprintme = new ReportDirect(viewer.ServerReport);
            if (!string.IsNullOrEmpty(printName.Trim())) autoprintme.PrinterSettings.PrinterName = printName.Trim();
            PrinterSettings ps1 = new PrinterSettings();
            //            ps1.PrinterName = "HP DeskJet 5820 series";
            ps1.Copies = Convert.ToInt16(Copias);
            //ps1.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Letter", 850, 1100);
            //ps1.DefaultPageSettings.Margins = new System.Drawing.Printing.Margins(3, 200, 3, 3);
            //autoprintme.PrinterSettings.PaperSizes = size;
            autoprintme.PrinterSettings = ps1;
            autoprintme.Print();
            PrintOk = true;
        }

        public void loaddocumento()
        {
            try
            {

                viewer.Reset();
                //viewer.PaperHeight = 1056;
                //viewer.PaperWidth = 816;
                //ServerReport serverReport = viewer.ServerReport;
                //string xnameReporte = @"/Otros/FrmDocumentos/pvfacturapos";
                //        string xnameReporte=@"/Empresas/Lecollezioni/Cartera/coMaestraDeTerceros ";
                viewer.ServerReport.ReportPath = this.ReportPath;
                viewer.ServerReport.ReportServerUrl = new Uri("http://192.168.0.12:7333/Reportservergs");
                //viewer.ServerReport.ReportPath = @"/Inventarios/ListaPrecios";
                viewer.ServerReport.ReportPath = @"/Otros/FrmCierrePVentas/cierrepv1";
                viewer.SetDisplayMode(DisplayMode.PrintLayout);
                viewer.ProcessingMode = ProcessingMode.Remote;
                ReportServerCredentials rsCredentials = viewer.ServerReport.ReportServerCredentials;
                rsCredentials.NetworkCredentials = new System.Net.NetworkCredential(@"grupo\wilmer.barrios", "Siasoft2018*");
                List<DataSourceCredentials> crdentials = new List<DataSourceCredentials>();
                List<ReportParameter> parameters = new List<ReportParameter>();

                ReportParameter paramcodemp = new ReportParameter();
                paramcodemp.Values.Add(codemp);
                paramcodemp.Name = "codemp";
                parameters.Add(paramcodemp);

                viewer.ServerReport.SetParameters(parameters);
                //ReportDataSource[] xx = viewer.LocalReport.DataSources[0];
                //System.Windows.MessageBox.Show(xx[0].Value.ToString());
                viewer.ServerReport.GetDataSources();
                
                //DataSet ds =  this.viewer.LocalReport.DataSources();
                //rdsic.i
                //foreach (ReportDataSourceInfo rdsi in rdsic)
                //{
                  //  System.Windows.MessageBox.Show(rdsi.Prompt);
                    //Debug.WriteLine(rdsi.Name + ":" + rdsi.Prompt);
                //}



                foreach (var dataSource in viewer.ServerReport.GetDataSources())
                {

                    //ReportDataSourceInfoCollection xx = dataSource;
                    //viewer.LocalReport.DataSources.Add(rds);
                    //iewer.LocalReport.GetDocumentMap.GetItemDefinition
                    //    ReportDataSource rds = viewer.item new ReportDataSource("dsNewDataSet_Table", getData());
                    //string dsName = ds.Name;
                    DataSourceCredentials credn = new DataSourceCredentials();
                    credn.Name = dataSource.Name;
                    System.Windows.MessageBox.Show(dataSource.Name);
                    credn.UserId = "wilmer.barrios@siasoftsas.com";
                    credn.Password = "Camilo654321*";
                    crdentials.Add(credn);
                }
                viewer.ServerReport.SetDataSourceCredentials(crdentials);
                

                //viewer.ServerReport..PaperHeight = 1056;
                //viewer.PaperWidth = 816;
                viewer.Update();
                viewer.PrinterSettings.Copies = Convert.ToInt16(Copias);
                //viewer.ZoomPercent = 50;
                if (ZoomPercent > 0)
                {
                    viewer.ZoomMode = ZoomMode.Percent;

                    viewer.ZoomPercent = ZoomPercent;
                }
                viewer.ZoomMode = ZoomMode.Percent;

                System.Drawing.Printing.PageSettings ps = new System.Drawing.Printing.PageSettings();
                ps.Landscape = true;
                ps.PaperSize = new System.Drawing.Printing.PaperSize("A4", 827, 1170);
                ps.PaperSize.RawKind = (int)System.Drawing.Printing.PaperKind.Letter;
                ps.Margins.Top = 5;
                ps.Margins.Bottom = 5;
                ps.Margins.Left = 5;
                ps.Margins.Right = 5;

                viewer.SetPageSettings(ps);
                viewer.ZoomPercent = 25;
                //viewer.PrinterSettings.PrinterName = "HP DeskJet 5820 series";
                //            viewer.PrinterSettings.PrintRange = PrintRange..AllPages;
                viewer.PrinterSettings.Collate = false;
                viewer.RefreshReport();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message.ToString() + "! ERROR DE DON WILMER ¡", "DocumentosReportes-loaddocumento");
            }
        }

        private void viewer_Print(object sender, ReportPrintEventArgs e)
        {

            PrintOk = true;
            viewer.Focus();
            //AuditoriaDoc(DocumentoIdCab, "Imprimio ", idEmp);
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
                AutoPrint();
                PrintOk = true;
                viewer.Focus();
            }
        }
    }
}
