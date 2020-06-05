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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.XlsIO;
using Syncfusion.UI.Xaml.Grid.Converter;
using Microsoft.Win32;
using System.IO;


namespace SiasoftAppExt
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MaestrasCoCuentas : UserControl
    {
        dynamic SiaWin;
        dynamic tabitem;
        int idemp = 0;

        //        string codbod = "";
        string cnEmp = "";

        public MaestrasCoCuentas(dynamic tabitem1)
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
                //cnEmp = foundRow["BusinessCn"].ToString().Trim();
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow["BusinessCn"].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                tabitem.Logo(idLogo, ".png");
                tabitem.Title = "Analisis de Venta(" + aliasemp + ")";
                //GroupId = 0;
                //ProjectId = 0;
                //BusinessId = 0;
                TabControl1.SelectedIndex = 0;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

            }
        }
        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {

            tabitem.Cerrar(0);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
            options.ExcelVersion = ExcelVersion.Excel2013;
            //            MessageBox.Show(((Button)sender).Tag.ToString());
            SfDataGrid sfdg = new SfDataGrid();
            var excelEngine = sfdg.ExportToExcel(sfdg.View, options);
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

                //Message box confirmation to view the created workbook.
                if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archvo",
                                    MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    //Launching the Excel file using the default Application.[MS Excel Or Free ExcelViewer]
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
            }

        }
        private string ArmaWhere()
        {
            return "";
        }
        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            //this.Opacity = 0.5;
            try
            {
                string where = ArmaWhere();
                //if (where==null) return;
                //MessageBox.Show(where);
                // carmar where
                if (string.IsNullOrEmpty(where)) where = " ";

                //               busy.IsBusy = true;
                //       busy.Visibility=Visibility.Visible;
                //dataGrid.Opacity = 0.5;
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                tabItemExt1.IsEnabled = false;
                sfBusyIndicator.IsBusy = true;
                //    LoadData(recordChanged());
                //dataGrid.Model.View.Refresh();
                dataGridMae.ItemsSource = null;
                BtnEjecutar.IsEnabled = false;
                source.CancelAfter(TimeSpan.FromSeconds(1));
                tabitem.Progreso(true);
                string ffi = "";
                string fff = "";
                var slowTask = Task<DataSet>.Factory.StartNew(() => SlowDude(ffi, fff, where, source.Token), source.Token);
                await slowTask;
                //MessageBox.Show(slowTask.Result.ToString());
                BtnEjecutar.IsEnabled = true;
                tabitem.Progreso(false);
                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    dataGridMae.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                    //Formulario.DataContext = ((DataSet)slowTask.Result).Tables[0];
//                    TabControl1.SelectedIndex = 2;
                    TabControl1.SelectedIndex = 0;
//                    double sub = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(subtotal)", "").ToString());
                    //TextTotal.Text = total.ToString("C");
                }
                //dataGrid.ItemsSource = Referencias;
                //        return;
                //    recordChanged();
                //    updateRow(9619);
                //    var slowTask = Task<string>.Factory.StartNew(() => LoadData(""));
                //     await slowTask;
                //     Txt.Content += slowTask.Result.ToString();
                //        busy.IsBusy = false;
                //    busy.Visibility=Visibility.Collapsed;
                //this.Opacity = 1;
                this.sfBusyIndicator.IsBusy = false;
                tabItemExt1.IsEnabled = true;
                //   dataGrid.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Opacity = 1;
            }
        }
        private DataSet SlowDude(string ffi, string fff, string where, CancellationToken cancellationToken)
        {
            try
            {
                DataSet jj = LoadData(ffi, fff, where, cancellationToken);
                return jj;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return null;
        }
        private DataSet LoadData(string Fi, string Ff, string where, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(cnEmp);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("select * from comae_cta order by cod_cta", con);
                cmd.CommandType = CommandType.Text;
                //cmd.Parameters.AddWithValue("@FechaIni", Fi);//if you have parameters.
                //cmd.Parameters.AddWithValue("@FechaFin", Ff);//if you have parameters.
                //cmd.Parameters.AddWithValue("@Where", where);//if you have parameters.
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                con.Close();
                foreach (DataTable table in ds.Tables)
                {
                    //            newColumn.DefaultValue = "Your DropDownList value";
//                    System.Data.DataColumn newColumn = new System.Data.DataColumn("ven_net", typeof(System.Double));
  //                  ds.Tables[table.TableName].Columns.Add(newColumn);
                }
                return ds;
                //VentasPorProducto.ItemsSource = ds.Tables[0];
                //VentaPorBodega.ItemsSource = ds.Tables[1];
                //VentasPorCliente.ItemsSource = ds.Tables[2];
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }


    }

}
