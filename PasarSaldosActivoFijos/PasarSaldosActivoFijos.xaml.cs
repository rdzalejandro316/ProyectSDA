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

    //Sia.PublicarPnt(9669,"PasarSaldosActivoFijos");
    //Sia.TabU(9669);

    public partial class PasarSaldosActivoFijos : UserControl
    {
        dynamic SiaWin;        
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        dynamic tabitem;

        public PasarSaldosActivoFijos(dynamic tabitem1)
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
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                tabitem.Title = "Pasar Saldos Activos Fijos-" + cod_empresa + "-" + nomempresa;
                tabitem.Logo(idLogo, ".png");
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load:" + e);
            }
        }
        
        private async void BTNconsultar_Click(object sender, RoutedEventArgs e)
        {            
            try
            {
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                ConfigGrid.IsEnabled = false;
                sfBusyIndicator.IsBusy = true;

                dataGridConsulta.ItemsSource = null;
                BTNconsultar.IsEnabled = false;
                source.CancelAfter(TimeSpan.FromSeconds(1));

                DateTime tiempo = Convert.ToDateTime(Fecha_Ano.Value.ToString());
                string empresa = "010";
                var pasarSald = ((ComboBoxItem)TipoSal.SelectedItem).Tag.ToString();


                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(tiempo.ToString("yyyy"), empresa, pasarSald.ToString(), source.Token), source.Token);
                await slowTask;

                BTNconsultar.IsEnabled = true;
                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    dataGridConsulta.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                    Total.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();

                    TabControl1.SelectedIndex = 2;
                    TabControl1.SelectedIndex = 1;
                }


                this.sfBusyIndicator.IsBusy = false;
                ConfigGrid.IsEnabled = true;
            }
            catch (SqlException w)
            {
                MessageBox.Show("error1-" + w);
            }
            catch (Exception ex)
            {
                MessageBox.Show("erro2-" + ex.Message);
                this.Opacity = 1;
            }
        }


        private DataSet LoadData(string ano, string empresa, string saldo, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpAF_PasarSaldosIniciales", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ano", ano);
                cmd.Parameters.AddWithValue("@PasarSaldos", saldo);
                cmd.Parameters.AddWithValue("@codEmpresa", empresa);
                da = new SqlDataAdapter(cmd);
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


        private void BTNcancelar_Click(object sender, RoutedEventArgs e)
        {
            tabitem.Cerrar(0);
        }

        private void Exportar_Click(object sender, RoutedEventArgs e)
        {
            var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
            options.ExcelVersion = ExcelVersion.Excel2013;
            var excelEngine = dataGridConsulta.ExportToExcel(dataGridConsulta.View, options);
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
                
                if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archvo",MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
            }

        }

        
    }
}
