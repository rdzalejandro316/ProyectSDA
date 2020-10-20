using AfResumenActivos;
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
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{

    //Sia.PublicarPnt(9671, "AfResumenActivos");
    //Sia.TabU(9671);

    public partial class AfResumenActivos : UserControl
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        dynamic tabitem;
        string fec_ini = "";
        string fec_fin = "";
        string año = "";

        public DataTable DTserver;
        public AfResumenActivos(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            tabitem = tabitem1;
            tabitem.CerrarConEscape = false;
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
                tabitem.Title = "Resumen Activos";
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

        private async void BtnConsultar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;

                GridConfiguracion.IsEnabled = false;
                sfBusyIndicator.IsBusy = true;
                dataGridAutomatico.ItemsSource = null;

                int Year = Convert.ToDateTime(Tx_ano.Value).Year;
                int Month = Convert.ToDateTime(Tx_periodo.Value).Month;
                string periodo = Month >= 10 ? Month.ToString() : "0" + Month.ToString();
                int lastDayOfMonth = DateTime.DaysInMonth(Year, Month);
                string fecha = lastDayOfMonth + "/" + Month + "/" + Year;

                fec_ini = "01/01/" + Year;
                fec_fin = lastDayOfMonth + "/" + Month + "/" + Year;
                año = Year.ToString();

                string activo = TxActivo.Text;
                string grupo = TxGrupo.Text;
                int isretiro = CBretiro.SelectedIndex;

                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(fecha, activo, grupo, isretiro), source.Token);
                await slowTask;

                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    dataGridAutomatico.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                    Txtotal.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();

                    TxMes.Text = Month.ToString();
                    TxAño.Text = Year.ToString();

                    TabControl1.SelectedIndex = 2;
                    TabControl1.SelectedIndex = 1;
                }

                this.sfBusyIndicator.IsBusy = false;
                GridConfiguracion.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("erro2:" + ex);
                this.Opacity = 1;
            }
        }



        private DataSet LoadData(string fecha, string cod_act, string cod_gru, int isretiro)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpAF_SaldosActivos", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Fecha", fecha);
                cmd.Parameters.AddWithValue("@cod_act", cod_act);
                cmd.Parameters.AddWithValue("@cod_gru", cod_gru);
                cmd.Parameters.AddWithValue("@codemp", "010");
                cmd.Parameters.AddWithValue("@IsResumenActivos", "1");
                cmd.Parameters.AddWithValue("@IsRetirado", isretiro);
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


        private static void CellExportingHandler(object sender, GridCellExcelExportingEventArgs e)
        {
            e.Range.CellStyle.Font.Size = 10;
            e.Range.CellStyle.Font.FontName = "Segoe UI";
            if
            (
                e.ColumnName == "vr_ini" || e.ColumnName == "vr_mov" || e.ColumnName == "dep_ini" ||
                e.ColumnName == "dep_mov" || e.ColumnName == "valor" || e.ColumnName == "dep_ac" ||
                e.ColumnName == "vr_adq"
            )
            {
                double value = 0;
                if (double.TryParse(e.CellValue.ToString(), out value))
                {
                    e.Range.Number = value;
                }
                e.Handled = true;
            }
        }

        private void BtnExportar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;
                options.CellsExportingEventHandler = CellExportingHandler;
                var excelEngine = dataGridAutomatico.ExportToExcel(dataGridAutomatico.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];
                workBook.Worksheets[0].AutoFilters.FilterRange = workBook.Worksheets[0].UsedRange; ;
                
                workBook.ActiveSheet.Columns[2].NumberFormat = "0.00";
                workBook.ActiveSheet.Columns[3].NumberFormat = "0.00";
                workBook.ActiveSheet.Columns[4].NumberFormat = "0.00";
                workBook.ActiveSheet.Columns[5].NumberFormat = "0.00";
                workBook.ActiveSheet.Columns[10].NumberFormat = "0.00";
                workBook.ActiveSheet.Columns[11].NumberFormat = "0.00";
                workBook.ActiveSheet.Columns[14].NumberFormat = "0.00";


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
                    if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archvo", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
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

        private void Tx_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.F8)
                {
                    string name = (sender as TextBox).Name;

                    string tabla = "", codigo = "", nombre = "", idrow = "", titulo = "";
                    bool mostrarAll = false;

                    switch (name)
                    {
                        case "TxActivo":
                            tabla = "afmae_act"; codigo = "cod_act"; nombre = "nom_act"; idrow = "idrow"; titulo = "Maestra de activos"; mostrarAll = false;
                            break;
                        case "TxGrupo":
                            tabla = "afmae_gru"; codigo = "cod_gru"; nombre = "nom_gru"; idrow = "idrow"; titulo = "Maestra de grupos"; mostrarAll = true;
                            break;
                    }

                    dynamic winb = SiaWin.WindowBuscar(tabla, codigo, nombre, codigo, idrow, tabla, cnEmp, mostrarAll, "", idEmp: idemp);
                    winb.ShowInTaskbar = false;
                    winb.Owner = Application.Current.MainWindow;
                    winb.Height = 400;
                    winb.ShowDialog();
                    int id = winb.IdRowReturn;
                    string code = winb.Codigo;
                    string nom = winb.Nombre;

                    if (!string.IsNullOrWhiteSpace(code))
                    {
                        (sender as TextBox).Text = code;
                    }

                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al presionar F8:" + w);
            }
        }

        private void Tx_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = (sender as TextBox).Name;
                string valor = (sender as TextBox).Text;
                if (string.IsNullOrEmpty(valor)) return;

                string tbl = ""; string cod = "", tit = "";
                switch (name)
                {
                    case "TxActivo":
                        tbl = "afmae_act"; cod = "cod_act"; tit = "Maestra de Activos";
                        break;
                    case "TxGrupo":
                        tbl = "afmae_gru"; cod = "cod_gru"; tit = "Maestra de Activos";
                        break;
                }

                string query = "select * from " + tbl + " where  " + cod + "='" + (sender as TextBox).Text + "' ";
                DataTable dt = SiaWin.Func.SqlDT(query, "tabla", idemp);
                if (dt.Rows.Count > 0)
                    (sender as TextBox).Text = dt.Rows[0][cod].ToString();
                else
                {
                    MessageBox.Show("el codigo que ingreso no existe en la " + tit, "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    (sender as TextBox).Text = "";
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error en el lostfocus:" + w);
            }
        }

        private void BtnSaldos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridAutomatico.SelectedIndex >= 0)
                {

                    DataRowView row = (DataRowView)dataGridAutomatico.SelectedItems[0];
                    string cod_act = row["cod_act"].ToString().Trim();
                    Saldos win = new Saldos();
                    win.cnemp = cnEmp;
                    win.cod_act = cod_act;
                    win.año = año;
                    win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    win.Owner = Application.Current.MainWindow;
                    win.ShowInTaskbar = false;
                    win.ShowDialog();

                }
                else
                {
                    MessageBox.Show("seleccione un activo para poder ver los movientos realizado", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar movimientos:" + w);
            }
        }

        private void BtnView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridAutomatico.SelectedIndex >= 0)
                {

                    DataRowView row = (DataRowView)dataGridAutomatico.SelectedItems[0];
                    string cod_act = row["cod_act"].ToString().Trim();
                    Movimientos win = new Movimientos();
                    win.cnemp = cnEmp;
                    win.cod_act = cod_act;
                    win.fec_ini = fec_ini;
                    win.fec_fin = fec_fin;
                    win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    win.Owner = Application.Current.MainWindow;
                    win.ShowInTaskbar = false;
                    win.ShowDialog();

                }
                else
                {
                    MessageBox.Show("seleccione un activo para poder ver los movientos realizado", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar movimientos:" + w);
            }
        }


    }
}
