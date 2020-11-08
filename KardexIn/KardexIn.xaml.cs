using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.Windows.Controls.Grid.Converter;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{

    /// Sia.PublicarPnt(9683,"KardexIn");
    /// Sia.TabU(9683);
    public partial class KardexIn : UserControl
    {

        dynamic SiaWin;
        dynamic tabitem;
        int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        string sqlerror = "";
        string nitEmp = "";
        public KardexIn(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            tabitem = tabitem1;
            idemp = SiaWin._BusinessId;            
            LoadConfig();          

        }
        
        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                nitEmp = foundRow["BusinessNit"].ToString().Trim();
                tabitem.Logo(idLogo, ".png");
                tabitem.Title = "Kardex Inv";
                Fec.Value = DateTime.Now.ToShortDateString();
                TabControl1.SelectedIndex = 0;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


        private async void BtnEjecutar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(Fec.Value.ToString()))
                {
                    MessageBox.Show("llene los campos de las fecha", "filtro", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                

                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                GridConfiguracion.IsEnabled = false;
                sfBusyIndicator.IsBusy = true;
                GridCosteo.ClearFilters();
                GridCosteo.ItemsSource = null;
                BtnEjecutar.IsEnabled = false;

                DateTime fec = Convert.ToDateTime(Fec.Value.ToString());
                int fecha = fec.Year;
                DateTime per = Convert.ToDateTime(Periodo.Value);                
                int periodo = per.Month;
                sqlerror = "";

                string codemp = cod_empresa;

                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(fecha, periodo, codemp, source.Token), source.Token);
                await slowTask;
                BtnEjecutar.IsEnabled = true;


                if (((DataSet)slowTask.Result) == null)
                {
                    BtnEjecutar.IsEnabled = true;
                    tabitem.Progreso(false);
                    this.sfBusyIndicator.IsBusy = false;
                    GridConfiguracion.IsEnabled = true;
                    TxRsgistros.Text = "0";
                    if (sqlerror == "") MessageBox.Show("Error al cargar datos ó Periodo sin información:" + sqlerror);
                    if (sqlerror != "") MessageBox.Show(sqlerror);
                    return;
                }

                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    GridCosteo.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                    TxRsgistros.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();
                    TabControl1.SelectedIndex = 2;
                    TabControl1.SelectedIndex = 1;

                }
                this.sfBusyIndicator.IsBusy = false;
                GridConfiguracion.IsEnabled = true;
            }
            catch (SqlException ex)
            {
                BtnEjecutar.IsEnabled = true;
                tabitem.Progreso(false);
                this.sfBusyIndicator.IsBusy = false;
                GridConfiguracion.IsEnabled = true;
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                //this.Opacity = 1;
                BtnEjecutar.IsEnabled = true;
                tabitem.Progreso(false);
                this.sfBusyIndicator.IsBusy = false;
                GridConfiguracion.IsEnabled = true;
                MessageBox.Show(ex.Message);
            }
        }

        private DataSet LoadData(int fecha, int periodo, string empresas, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpSpInKardex", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Ano", fecha);
                cmd.Parameters.AddWithValue("@Per", periodo);
                cmd.Parameters.AddWithValue("@codemp", empresas);
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds);
                con.Close();
                return ds;
            }
            catch (SqlException ex)
            {
                sqlerror = ex.Message;
                return null;
            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExportMode = Syncfusion.UI.Xaml.Grid.Converter.ExportMode.Value;
                options.ExcelVersion = ExcelVersion.Excel2013;
                options.CellsExportingEventHandler = CellExportingHandler;
                var excelEngine = GridCosteo.ExportToExcel(GridCosteo.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];
                options.StartColumnIndex = 1;
                //options.StartRowIndex = 3;

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
                    if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archvo",
                                        MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        //Launching the Excel file using the default Application.[MS Excel Or Free ExcelViewer]
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("erro2:" + ex);
                this.Opacity = 1;
            }




        }



        private static void CellExportingHandler(object sender, GridCellExcelExportingEventArgs e)
        {
            e.Range.CellStyle.Font.Size = 10;
            e.Range.CellStyle.Font.FontName = "Segoe UI";
            if (e.ColumnName == "cantidad" || e.ColumnName == "cos_uni" || e.ColumnName == "cos_tot" || e.ColumnName == "subtotal")
            {
                double value = 0;

                if (double.TryParse(e.CellValue.ToString(), out value)) e.Range.Number = value;
                else e.Range.Text = e.CellValue.ToString();
                e.Handled = true;
            }
            if (e.ColumnName == "ord_trn" || e.ColumnName == "cod_trn" || e.ColumnName == "cod_tip" || e.ColumnName == "cod_bod" || e.ColumnName == "cod_ref" || e.ColumnName == "cod_ant" || e.ColumnName == "codprvcli" || e.ColumnName == "suc_cli" || e.ColumnName == "cod_gru" || e.ColumnName == "per_doc")
            {
                string value = e.CellValue.ToString();

                e.Range.Text = value;
                e.Handled = true;
            }
        }



        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            tabitem.Cerrar(0);
        }


    }
}
