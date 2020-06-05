using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using Reportes;

namespace SiasoftAppExt
{
    /// <summary>
    /// Lógica de interacción para UserControl1.xaml
    /// </summary>
    public partial class ReportesCierrePv : Window
    {
        //Sia.PublicarPnt(9516,"ReportesCierrePv");
        dynamic SiaWin;
        public int idEmp = 0;
        string codemp = string.Empty;
        string nomemp = string.Empty;
        public string codpvta = string.Empty;
        public string codbod = string.Empty;
        public string  fechaCorte = DateTime.Now.ToShortDateString();
        public int ReporteId = 0;
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
        List<ReportParameter> parameters = new List<ReportParameter>();
        public ReportesCierrePv()
        {

            SiaWin = System.Windows.Application.Current.MainWindow;
            InitializeComponent();

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(ReporteId<=0)
            {
                System.Windows.MessageBox.Show("Falta ReporteId");
                this.IsEnabled = false;
                return;
            }
            if (idEmp<=0)
            {
                System.Windows.MessageBox.Show("Id Empresa:"+idEmp.ToString()+" no existe");
                this.IsEnabled = false;
                return;
            }

            if (string.IsNullOrEmpty(codbod) || codbod =="" )
            {
                System.Windows.MessageBox.Show("Bodega:" + codbod + " no existe");
                this.IsEnabled = false;
                //return;
            }

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
            loaddocumento(ReporteId);
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

        public void loaddocumento(int reporteId)
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
                
                if(reporteId == 1) viewer.ServerReport.ReportPath = @"/Otros/FrmCierrePVentas/cierrepv1";
                if(reporteId == 2) viewer.ServerReport.ReportPath = @"/Otros/FrmCierrePVentas/cierrepv1";
                viewer.SetDisplayMode(DisplayMode.PrintLayout);
                viewer.ProcessingMode = ProcessingMode.Remote;
                ReportServerCredentials rsCredentials = viewer.ServerReport.ReportServerCredentials;
                rsCredentials.NetworkCredentials = new System.Net.NetworkCredential(@"grupo\wilmer.barrios", "Siasoft2018*");
                List<DataSourceCredentials> crdentials = new List<DataSourceCredentials>();
                //List<ReportParameter> parameters = new List<ReportParameter>();

                ReportParameter paramcodemp = new ReportParameter();
                paramcodemp.Values.Add(codemp);
                paramcodemp.Name = "codemp";
                parameters.Add(paramcodemp);
                ReportParam(reporteId);
                //ReportDataSource[] xx = viewer.LocalReport.DataSources[0];
                //System.Windows.MessageBox.Show(xx[0].Value.ToString());
                //viewer.ServerReport.GetDataSources();

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
                //viewer.Update();
                viewer.PrinterSettings.Copies = Convert.ToInt16(Copias);
                //viewer.ZoomPercent = 50;
                if (ZoomPercent > 0)
                {
                    viewer.ZoomMode = ZoomMode.Percent;

                    viewer.ZoomPercent = ZoomPercent;
                }
                //viewer.ZoomMode = ZoomMode.Percent;

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
                //ReportCierre01();

                viewer.LocalReport.DataSources.Clear();
                
                //viewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", ReportCierre01()));
                viewer.ServerReport.SetParameters(parameters);

                //viewer.ServerReport.Refresh();

                viewer.RefreshReport();
                UpdateLayout();
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

        private void ReportParam(int reporteId)
        {
            ReportParameter _repoparam = new ReportParameter();
            try
            {
                if(reporteId==1)  //cierrepv  
                {
                    parameters.Add(new ReportParameter("Tag1", ReportCierre(11)));
                    parameters.Add(new ReportParameter("Tag2",ReportCierre(12)));
                }
                if (reporteId == 2)  //cierrepv  
                {
                    parameters.Add(new ReportParameter("Tag1", ReportCierre(11)));
                }


            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message.ToString());
            }
        }
        private string ReportCierre(int reporteId)
        {
            try
            {

                StringBuilder SbSql = new StringBuilder();
                if (reporteId == 11) // cierrepv1- DataSet1
                {
                    SbSql.Append("select incab.for_pag,incue.cod_trn,incue.num_trn,DENSE_RANK() OVER(ORDER BY incab.for_pag,incab.cod_trn, incab.num_trn) As num_col, iif(inmae_ref.cod_tip <> '000', incue.subtotal, 0)*iif(incue.cod_trn = '005', 1, -1) as gravada,iif(inmae_ref.cod_tip <> '000', SUBTOTAL, 0) * iif(incue.cod_trn = '005', 1, -1) as exenta,iif(incue.cod_tiva = 'A', '16', iif(incue.cod_tiva = 'C', '19', '')) as tar_iva,incue.VAL_IVA* iif(incue.cod_trn= '005',1,-1) as val_iva,incue.VAL_RET* iif(incue.cod_trn= '005',1,-1) as val_ret,incue.VAL_ICA* iif(incue.cod_trn= '005',1,-1) as val_ica,incue.VAL_RIVA* iif(incue.cod_trn= '005',1,-1) as val_riva,round(incue.SUBTOTAL + incue.VAL_IVA - round(incue.VAL_RET, 0) - incue.VAL_ICA - incue.VAL_RIVA, 0) * iif(incue.cod_trn = '005', 1, -1) as total,incue.cantidad* iif(incue.cod_trn= '005',1,-1) as cantidad,incue.val_uni,inmae_ref.cod_tip,comae_ter.cod_ciu as ciudad  ");
                    SbSql.Append("from incab_doc as incab inner join incue_doc as incue on incue.idregcab = incab.idreg inner join inmae_ref on inmae_ref.cod_ref = incue.cod_ref inner join comae_ter on comae_ter.cod_ter = incab.cod_cli ");
                    SbSql.Append("where (incue.cod_bod = '" + codbod + "' or incab.bod_tra = '" + codbod + "') and(incab.cod_trn BETWEEN '004' and '009') ");
                    SbSql.Append("and(convert(date, incab.fec_trn, 103) = '" + fechaCorte + "' )" ) ;
                    SbSql.Append("order by incab.for_pag,incab.cod_trn,incab.num_trn ");
                    
                }
                if (reporteId == 12)
                {

                    SbSql.Append("select incab.for_pag,incue.cod_trn,incue.num_trn,DENSE_RANK() OVER(ORDER BY incab.for_pag,incab.cod_trn, incab.num_trn) As num_col, iif(inmae_ref.cod_tip <> '000', incue.subtotal, 0)*iif(incue.cod_trn = '005', 1, -1) as gravada,iif(inmae_ref.cod_tip <> '000', SUBTOTAL, 0) * iif(incue.cod_trn = '005', 1, -1) as exenta,iif(incue.cod_tiva = 'A', '16', iif(incue.cod_tiva = 'C', '19', '')) as tar_iva,incue.VAL_IVA* iif(incue.cod_trn= '005',1,-1) as val_iva,incue.VAL_RET* iif(incue.cod_trn= '005',1,-1) as val_ret,incue.VAL_ICA* iif(incue.cod_trn= '005',1,-1) as val_ica,incue.VAL_RIVA* iif(incue.cod_trn= '005',1,-1) as val_riva,round(incue.SUBTOTAL + incue.VAL_IVA - round(incue.VAL_RET, 0) - incue.VAL_ICA - incue.VAL_RIVA, 0) * iif(incue.cod_trn = '005', 1, -1) as total,incue.cantidad* iif(incue.cod_trn= '005',1,-1) as cantidad,incue.val_uni,inmae_ref.cod_tip,comae_ter.cod_ciu as ciudad  ");
                    SbSql.Append("from incab_doc as incab inner join incue_doc as incue on incue.idregcab = incab.idreg inner join inmae_ref on inmae_ref.cod_ref = incue.cod_ref inner join comae_ter on comae_ter.cod_ter = incab.cod_cli ");
                    SbSql.Append("where (incue.cod_bod = '" + codbod + "' or incab.bod_tra = '" + codbod + "') and(incab.cod_trn BETWEEN '004' and '009') ");
                    SbSql.Append("and(convert(date, incab.fec_trn, 103) = '" + fechaCorte + "' )");
                    SbSql.Append("order by incab.for_pag,incab.cod_trn,incab.num_trn ");

                }


                if (reporteId == 21)
                {
                    SbSql.Append("select _detfpagCie.for_pag,_mvpvcie.cod_trn,_mvpvcie.num_trn,DENSE_RANK() OVER(ORDER BY _detfpagCie.FOR_PAG, _detfpagCie.cod_trn, _detfpagCie.num_trn) As num_col, iif(inmae_ref.cod_tip <> '000', _mvpvcie.SUBTOTAL, 0)*iif(_mvpvcie.cod_trn = '005', 1, -1) as gravada,iif(inmae_ref.cod_tip <> '000', SUBTOTAL, 0) * iif(_mvpvcie.cod_trn = '005', 1, -1) as exenta, iif(_mvpvcie.cod_tiva = 'A', '16', iif(_mvpvcie.cod_tiva = 'C', '19', '')) as tar_iva,_mvpvcie.VAL_IVA* iif(_mvpvcie.cod_trn= '005',1,-1) as val_iva,_mvpvCie.VAL_RET* iif(_mvpvcie.cod_trn= '005',1,-1) as val_ret,_mvpvCie.VAL_ICA* iif(_mvpvcie.cod_trn= '005',1,-1) as val_ica,_mvpvCie.VAL_RIVA* iif(_mvpvcie.cod_trn= '005',1,-1) as val_riva,round(_mvpvcie.SUBTOTAL + _mvpvcie.VAL_IVA - round(_mvpvCie.VAL_RET, 0) - _mvpvCie.VAL_ICA - _mvpvCie.VAL_RIVA, 0) * iif(_mvpvcie.cod_trn = '005', 1, -1) as total,_mvpvCie.cantidad* iif(_mvpvcie.cod_trn= '005',1,-1) as cantidad,_mvpvcie.val_uni,inmae_ref.cod_tip,comae_ter.cod_ciu as ciudad  ");
                    SbSql.Append("from incab_doc _detfpagCie,incue_doc _mvpvcie, comae_ter, inmae_ref ");
                    SbSql.Append("where _detfpagCie.ANO_DOC + _detfpagCie.PER_DOC + _detfpagCie.cod_trn + _detfpagCie.num_trn = _mvpvcie.ANO_DOC + _mvpvcie.PER_DOC + _mvpvcie.cod_trn + _mvpvcie.num_trn ");
                    SbSql.Append("and(_mvpvcie.cod_BOD = '" + codbod + "' or _detfpagCie.bod_tra = '" + codbod + "') and(_detfpagCie.cod_trn BETWEEN '004' and '009') ");
                    SbSql.Append("and(convert(date, _detfpagCie.fec_TRN, 103) = '" + fechaCorte + "') and _detfpagCie.cod_CLI = comae_ter.cod_ter ");
                    SbSql.Append("and _mvpvcie.cod_ref = inmae_ref.cod_ref order by _detfpagCie.FOR_PAG,_detfpagCie.cod_trn,_detfpagCie.num_trn ");
                }

                return SbSql.ToString();
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message.ToString());
                return string.Empty;
            }

        }
    }
}
