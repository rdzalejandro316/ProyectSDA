using Microsoft.Reporting.WinForms;
using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{

    //Sia.PublicarPnt(9671, "AfResumenGrupoActivos");
    //Sia.TabU(9671);

    public partial class AfResumenGrupoActivos : UserControl
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        int cosn = 0;
        dynamic tabitem;

        public DataTable DTserver;
        public AfResumenGrupoActivos(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            tabitem = tabitem1;
            LoadConfig();
        }


        private void LoadConfig()
        {
            try
            {
                SiaWin = Application.Current.MainWindow;
                if (idemp <= 0) idemp = SiaWin._BusinessId;

                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                tabitem.Title = "Devolcion de facturas" + cod_empresa + "-" + nomempresa;
                tabitem.Logo(idLogo, ".png");
                DTserver = cargarDatosSerividor();

            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        public DataTable cargarDatosSerividor()
        {
            DataTable dt = SiaWin.Func.SqlDT("select ServerIP, UserServer, UserServerPassword, UserSql, UserSqlPassword from ReportServer", "Empresas", 0);
            return dt;
        }



        private void BtnConsultar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CheGridGeneral.IsChecked == true)
                {
                    tabItemExt2.IsSelected = true;
                    consulta();
                }
                else
                {
                    cosn++;
                    List<ReportParameter> parameters = new List<ReportParameter>();
                    TabItemExt tabItemExt1 = new TabItemExt();
                    tabItemExt1.Header = "Consulta General - " + cosn.ToString();

                    int Year = Convert.ToDateTime(Tx_ano.Value).Year;
                    int Month = Convert.ToDateTime(Tx_periodo.Value).Month;
                    string periodo = Month >= 10 ? Month.ToString() : "0" + Month.ToString();

                    parameters.Add(new ReportParameter("año", Year.ToString()));
                    parameters.Add(new ReportParameter("periodo", periodo));

                    WindowsFormsHost winFormsHost = new WindowsFormsHost();
                    ReportViewer viewer = new ReportViewer();
                    viewer.ServerReport.ReportServerUrl = new Uri(DTserver.Rows[0]["ServerIP"].ToString().Trim());

                    viewer.ServerReport.ReportPath = "/ActivosFijos/ResumenPorGruposActivos";

                    viewer.ProcessingMode = ProcessingMode.Remote;
                    //ReportServerCredentials rsCredentials = viewer.ServerReport.ReportServerCredentials;
                    //rsCredentials.NetworkCredentials = new System.Net.NetworkCredential(DTserver.Rows[0]["UserServer"].ToString(), DTserver.Rows[0]["UserServerPassword"].ToString());
                    //List<DataSourceCredentials> crdentials = new List<DataSourceCredentials>();

                    //foreach (var dataSource in viewer.ServerReport.GetDataSources())
                    //{
                    //    DataSourceCredentials credn = new DataSourceCredentials();
                    //    credn.Name = dataSource.Name;
                    //    System.Windows.MessageBox.Show(dataSource.Name);
                    //    credn.UserId = DTserver.Rows[0]["UserSql"].ToString();
                    //    credn.Password = DTserver.Rows[0]["UserSqlPassword"].ToString();
                    //    crdentials.Add(credn);
                    //}

                    //viewer.ServerReport.SetDataSourceCredentials(crdentials);
                    viewer.ServerReport.SetParameters(parameters);
                    viewer.RefreshReport();

                    winFormsHost.Child = viewer;
                    tabItemExt1.Content = winFormsHost;
                    TabControl1.Items.Add(tabItemExt1);
                    UpdateLayout();
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al consultar:" + w);
            }
        }




        public async void consulta()
        {
            try
            {
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                sfBusyIndicator.IsBusy = true;
                dataGridAutomatico.ItemsSource = null;

                int Year = Convert.ToDateTime(Tx_ano.Value).Year;
                int Month = Convert.ToDateTime(Tx_periodo.Value).Month;
                string periodo = Month >= 10 ? Month.ToString() : "0" + Month.ToString();



                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(Year.ToString(), periodo), source.Token);
                await slowTask;

                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    dataGridAutomatico.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                    Txtotal.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();
                }

                this.sfBusyIndicator.IsBusy = false;
            }
            catch (Exception ex)
            {                
                MessageBox.Show("erro2:" + ex);
                this.Opacity = 1;
            }
        }



        private DataSet LoadData(string año, string mes)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpFacDevolucionCon", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@año", año);
                cmd.Parameters.AddWithValue("periodo", mes);
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds);
                con.Close();
                return ds;
            }
            catch (Exception e)
            {
                
                MessageBox.Show("en la consulta:" + e.Message);
                return null;
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            tabitem.Cerrar(0);
        }

        //private static void CellExportingHandler(object sender, GridCellExcelExportingEventArgs e)
        //{
        //    e.Range.CellStyle.Font.Size = 10;
        //    e.Range.CellStyle.Font.FontName = "Segoe UI";
        //    if (e.ColumnName == "val_uni" || e.ColumnName == "cantidad" || e.ColumnName == "subtotal")
        //    {
        //        double value = 0;
        //        if (double.TryParse(e.CellValue.ToString(), out value))
        //        {
        //            e.Range.Number = value;
        //        }
        //        e.Handled = true;
        //    }
        //    if (e.ColumnName == "cod_dev" || e.ColumnName == "cod_tip" || e.ColumnName == "cod_bod" || e.ColumnName == "cod_ref" || e.ColumnName == "cod_trn")
        //    {
        //        string value = e.CellValue.ToString();

        //        e.Range.Text = value;
        //        e.Handled = true;
        //    }
        //}

        private void BtnExportar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;
                //options.CellsExportingEventHandler = CellExportingHandler;
                var excelEngine = dataGridAutomatico.ExportToExcel(dataGridAutomatico.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];
                workBook.Worksheets[0].AutoFilters.FilterRange = workBook.Worksheets[0].UsedRange;;
                //workBook.ActiveSheet.Columns[14].NumberFormat = "000";


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
                    if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archvo",MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {                       
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }

            }
            catch (Exception w)
            {
                
                MessageBox.Show("error al exportar");
            }
        }








    }
}
