using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid.Converter;
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
    //    Sia.PublicarPnt(9653,"AuditoriaDocumentos");
    //    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9653,"AuditoriaDocumentos");
    //    ww.ShowInTaskbar = false;
    //    ww.Owner = Application.Current.MainWindow;
    //    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //    ww.ShowDialog();

    public partial class AuditoriaDocumentos : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public AuditoriaDocumentos()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Auditoria de documentos";

                Tx_fecini.Text = DateTime.Now.AddMonths(-1).ToString();
                Tx_fecfin.Text = DateTime.Now.ToString();
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private async void BtnConsultar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;

                sfBusyIndicator.IsBusy = true;

                string fec_ini = Tx_fecini.Text;
                string fec_fin = Tx_fecfin.Text;
                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(fec_ini, fec_fin, source.Token), source.Token);
                await slowTask;

                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    GridConsulta.ItemsSource = ((DataSet)slowTask.Result).Tables[0].DefaultView;
                    TabControl1.SelectedIndex = 2;
                    TabControl1.SelectedIndex = 1;
                    Tx_total.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();
                }

                sfBusyIndicator.IsBusy = false;
            }
            catch (Exception w)
            {
                MessageBox.Show("error en la consulta");
            }
        }


        private DataSet LoadData(string _Fi, string _Ff, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(cnEmp);
                SqlCommand cmd = new SqlCommand();
                cmd.CommandTimeout = 0;
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_SpAuditoriaDocumento", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fecha_ini", _Fi);
                cmd.Parameters.AddWithValue("@fecha_fin", _Ff);
                cmd.Parameters.AddWithValue("@listaTransaccion", "");
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds);
                con.Close();
                return ds;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnExportar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;
                var excelEngine = GridConsulta.ExportToExcel(GridConsulta.View, options);
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
                        MessageBox.Show(sfd.FilterIndex.ToString());
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

                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }






    }
}
