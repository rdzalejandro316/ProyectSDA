using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
    //    Sia.PublicarPnt(9632,"ConsultaMovimientoDoc");
    //    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9632,"ConsultaMovimientoDoc");
    //    ww.ShowInTaskbar = false;
    //    ww.Owner = Application.Current.MainWindow;
    //    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //    ww.ShowDialog();

    public partial class ConsultaMovimientoDoc : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        int modulo = 1;

        public ConsultaMovimientoDoc()
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
                this.Title = "Consulta Movimiento Documento Referencia";
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private async void BtnConsultar_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(Tx_text.Text))
            {
                MessageBox.Show("ingrese un documento para la consulta","alert",MessageBoxButton.OK,MessageBoxImage.Stop);
                return;
            }

            CancellationTokenSource source = new CancellationTokenSource();

            CancellationToken token = source.Token;

            sfBusyIndicator.IsBusy = true;

            string doc_mov = Tx_text.Text;
            var slowTask = Task<DataTable>.Factory.StartNew(() => LoadData(doc_mov, source.Token), source.Token);
            await slowTask;


            if (((DataTable)slowTask.Result).Rows.Count > 0)
            {
                GridConsulta.ItemsSource = ((DataTable)slowTask.Result).DefaultView;
                TX_total.Text = ((DataTable)slowTask.Result).Rows.Count.ToString();


                double deb = Convert.ToDouble(((DataTable)slowTask.Result).Compute("Sum(deb_mov)", ""));
                double cre = Convert.ToDouble(((DataTable)slowTask.Result).Compute("Sum(cre_mov)", ""));
                double dif = deb - cre;

                Tx_deb.Text = deb.ToString("N", CultureInfo.CreateSpecificCulture("es-ES"));
                Tx_cre.Text = cre.ToString("N", CultureInfo.CreateSpecificCulture("es-ES"));
                Tx_tot.Text = dif.ToString("N", CultureInfo.CreateSpecificCulture("es-ES"));

            }
            else
            {
                GridConsulta.ItemsSource = null;
                TX_total.Text = "0";
                Tx_deb.Text = "-";
                Tx_cre.Text = "-";
                Tx_tot.Text = "-";
            }

            sfBusyIndicator.IsBusy = false;
        }

        private DataTable LoadData(string doc_mov, CancellationToken cancellationToken)
        {
            try
            {
                string query = "select cab.idreg,cab.cod_trn,cab.num_trn,cab.fec_trn,cue.cod_cta,cue.cod_ter,cue.des_mov, ";
                query += "cue.doc_ref,cue.doc_cruc,cue.deb_mov,cue.cre_mov ";
                query += "from Cocue_doc as cue ";
                query += "inner join cocab_doc as cab on cab.idreg = cue.idregcab ";
                query += "WHERE cue.doc_mov='" + doc_mov + "' ";

                System.Data.DataTable dt = SiaWin.Func.SqlDT(query, "tabla", idemp);
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

        private void BtnView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (GridConsulta.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)GridConsulta.SelectedItems[0];
                    int idreg = Convert.ToInt32(row["idreg"]);
                    SiaWin.TabTrn(0, idemp, true, idreg, modulo, WinModal: true);
                }
                else
                {
                    MessageBox.Show("seleccione un documento de la grilla", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrri el documento:" + w);
            }
        }

    }
}
