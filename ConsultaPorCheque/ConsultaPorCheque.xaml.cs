using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
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

    //    Sia.PublicarPnt(9635,"ConsultaPorCheque");
    //    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9635,"ConsultaPorCheque");
    //    ww.ShowInTaskbar = false;
    //    ww.Owner = Application.Current.MainWindow;
    //    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //    ww.ShowDialog();

    public partial class ConsultaPorCheque : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        public ConsultaPorCheque()
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
                this.Title = "Consulta Documento Referencia";
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private async void BtnConsultar_Click(object sender, RoutedEventArgs e)
        {


            CancellationTokenSource source = new CancellationTokenSource();

            CancellationToken token = source.Token;
            
            sfBusyIndicator.IsBusy = true;

            string _cheque = Tx_text.Text;
            var slowTask = Task<DataTable>.Factory.StartNew(() => LoadData(_cheque, source.Token), source.Token);
            await slowTask;


            if (((DataTable)slowTask.Result).Rows.Count > 0)
            {
                GridConsulta.ItemsSource = ((DataTable)slowTask.Result).DefaultView;
                TX_total.Text = ((DataTable)slowTask.Result).Rows.Count.ToString();
            }
            else
            {
                GridConsulta.ItemsSource = null;
                TX_total.Text = "0";
            }

            sfBusyIndicator.IsBusy = false;
        }


        private DataTable LoadData(string cheque, CancellationToken cancellationToken)
        {
            try
            {
                System.Data.DataTable dt = SiaWin.Func.SqlDT("select * from Cocue_doc WHERE num_chq='" + cheque + "'", "tabla", idemp);
                return dt;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
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
