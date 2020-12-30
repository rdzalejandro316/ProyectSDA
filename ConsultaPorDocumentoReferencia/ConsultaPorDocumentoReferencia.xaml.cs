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

    //Sia.PublicarPnt(9686,"ConsultaPorDocumentoReferencia");
    //Sia.TabU(9686);

    public partial class ConsultaPorDocumentoReferencia : UserControl
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        int modulo = 1;
        dynamic tabitem;

        public ConsultaPorDocumentoReferencia(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            tabitem = tabitem1;
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
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                tabitem.Title = "Consulta Documento Contable";
                tabitem.Logo(idLogo, ".png");                
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
                if (!string.IsNullOrEmpty(TxCheque.Text))
                {

                    sfBusyIndicator.IsBusy = true;

                    string cue = "select * from cocue_doc where DOC_MOV='" + TxCheque.Text + "'; ";


                    var slowTask = Task<DataTable>.Factory.StartNew(() => LoadData(cue));
                    await slowTask;

                    if (((DataTable)slowTask.Result).Rows.Count > 0)
                    {
                        DataGridCuerpo.ItemsSource = ((DataTable)slowTask.Result).DefaultView;
                        TxTotal.Text = ((DataTable)slowTask.Result).Rows.Count.ToString();
                    }
                    else
                    {
                        DataGridCuerpo.ItemsSource = null;
                        TxTotal.Text = "0";
                    }

                    sfBusyIndicator.IsBusy = false;

                }
                else
                {
                    MessageBox.Show("tiene que ingresar un numero de cheque", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show("error al cargar la cabeza :" + ex);
            }
        }


        private DataTable LoadData(string query)
        {
            try
            {
                SqlConnection con = new SqlConnection(cnEmp);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataTable dt = new DataTable();
                cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.Text;
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(dt);
                con.Close();
                return dt;
            }
            catch (Exception e)
            {

                MessageBox.Show("en la consulta:" + e.Message);
                return null;
            }
        }


        private void BtnExportar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;
                var excelEngine = DataGridCuerpo.ExportToExcel(DataGridCuerpo.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];
                options.ExportMode = ExportMode.Value;

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

        private void DataGridCuerpo_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {

                if (DataGridCuerpo.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)DataGridCuerpo.SelectedItems[0];
                    int idreg = Convert.ToInt32(row["idregcab"]);
                    SiaWin.TabTrn(0, idemp, true, idreg, modulo, WinModal: true);
                }
                else
                {
                    MessageBox.Show("seleccione un documento de la grilla", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir lo documentos:" + w);
            }
        }


    }
}
