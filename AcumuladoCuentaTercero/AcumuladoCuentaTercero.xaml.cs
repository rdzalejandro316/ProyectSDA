using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid;
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

    //Sia.PublicarPnt(9691,"AcumuladoCuentaTercero");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9691,"AcumuladoCuentaTercero");
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();   

    public partial class AcumuladoCuentaTercero : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        int tipo = 0;
        public AcumuladoCuentaTercero()
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
                this.Title = "Acumulado Cuenta Tercero";

                TxFecFin.Text = DateTime.Now.ToString();

            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.F8)
                {


                    string tag = (sender as TextBox).Tag.ToString();

                    string table = "", code = "", name = "", title = "", idrow = "";
                    switch (tag)
                    {
                        case "comae_cta":
                            table = tag; code = "cod_cta"; name = "nom_cta"; title = "maestra de cuentas"; idrow = "idrow";
                            break;
                        case "comae_ter":
                            table = tag; code = "cod_ter"; name = "nom_ter"; title = "maestar de terceros"; idrow = "idrow";
                            break;
                        case "comae_ciu":
                            table = tag; code = "cod_ciu"; name = "nom_ciu"; title = "maestar de ciudades"; idrow = "idrow";
                            break;
                        case "comae_suc":
                            table = tag; code = "cod_suc"; name = "nom_suc"; title = "maestar de sucursales"; idrow = "idrow";
                            break;
                        case "comae_cco":
                            table = tag; code = "cod_cco"; name = "nom_cco"; title = "maestar de centro de costos"; idrow = "idrow";
                            break;
                    }


                    int xidr = 0; string xcode = ""; string xnom = "";
                    dynamic winb = SiaWin.WindowBuscar(table, code, name, code, idrow, title, SiaWin.Func.DatosEmp(idemp), false, "", idEmp: idemp);
                    winb.ShowInTaskbar = false;
                    winb.Owner = Application.Current.MainWindow;
                    winb.Height = 300;
                    winb.Width = 400;
                    winb.ShowDialog();
                    xidr = winb.IdRowReturn;
                    xcode = winb.Codigo;
                    xnom = winb.Nombre;
                    winb = null;

                    if (!string.IsNullOrEmpty(xcode))
                    {
                        (sender as TextBox).Text = xcode.Trim();
                        var uiElement = e.OriginalSource as UIElement;
                        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    }
                    e.Handled = true;
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al buscar:" + w);
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {

                string tag = (sender as TextBox).Tag.ToString();
                string value = (sender as TextBox).Text.ToString().Trim();
                if (string.IsNullOrEmpty(value)) return;

                string table = "", code = "", title = "";
                switch (tag)
                {
                    case "comae_cta":
                        table = tag; code = "cod_cta"; title = "maestra de cuentas";
                        break;
                    case "comae_ter":
                        table = tag; code = "cod_ter"; title = "maestar de terceros";
                        break;
                    case "comae_ciu":
                        table = tag; code = "cod_ciu"; title = "maestar de ciudades";
                        break;
                    case "comae_suc":
                        table = tag; code = "cod_suc"; title = "maestar de sucursales";
                        break;
                    case "comae_cco":
                        table = tag; code = "cod_cco"; title = "maestar de centro de costos";
                        break;
                }

                string query = "select * from " + table + " where " + code + "='" + value + "'; ";
                DataTable dt = SiaWin.Func.SqlDT(query, "temp", idemp);
                if (dt.Rows.Count <= 0)
                {
                    MessageBox.Show("el codigo de la " + title + " ingresado no existe", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    (sender as TextBox).Text = "";
                }


            }
            catch (Exception w)
            {
                MessageBox.Show("error al validar:" + w);
            }
        }

        private async void BtnConsultar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region validaciones

                if (string.IsNullOrEmpty(TxCod_Cta.Text))
                {
                    MessageBox.Show("el campo cuenta debe de estar lleno", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                DateTime xfecfin = Convert.ToDateTime(TxFecFin.Text);
                DateTime xano = Convert.ToDateTime(Tx_ano.Value.ToString());

                if (xfecfin.Year != xano.Year)
                {
                    MessageBox.Show("el campo fecha de corte debe de ser igual al año a consultar", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }


                #endregion

                TxCuenta.Text = "";
                TxTercero.Text = "";
                TxCiudad.Text = "";
                TxCcosto.Text = "";
                TxDebitos.Text = "";
                TxCreditos.Text = "";

                GridAcumulado.ItemsSource = null;
                GridMov.ItemsSource = null;

                sfBusyIndicator.IsBusy = true;


                DateTime tiempo = Convert.ToDateTime(Tx_ano.Value.ToString());
                string año = tiempo.Year.ToString();
                string fec_fin = TxFecFin.Text;
                string ter = TxCod_Ter.Text;
                string cta = TxCod_Cta.Text;
                string cod_ciu = TxCod_Ciu.Text;
                string cod_suc = TxCod_Suc.Text;
                string cod_cco = TxCod_Cco.Text;
                string codemp = cod_empresa;


                var slowTask = Task<DataTable>.Factory.StartNew(() => LoadData(año, fec_fin, ter, cta, cod_ciu, cod_suc, cod_cco, codemp));
                await slowTask;

                if (slowTask.IsCompleted)
                {
                    GridAcumulado.ItemsSource = slowTask.Result.DefaultView;
                }


                sfBusyIndicator.IsBusy = false;

            }
            catch (Exception w)
            {
                MessageBox.Show("error en la consulta:" + w);
            }
        }

        private DataTable LoadData(string ano, string fec_fin, string ter, string cta, string cod_ciu, string cod_suc, string cod_cco, string codemp)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                cmd.CommandTimeout = 0;
                SqlDataAdapter da = new SqlDataAdapter();
                DataTable dt = new DataTable();

                cmd = new SqlCommand("_EmpAcumuladoCuentaTerecero", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ano", ano);
                cmd.Parameters.AddWithValue("@fechafin", fec_fin);
                cmd.Parameters.AddWithValue("@ter", ter);
                cmd.Parameters.AddWithValue("@cta", cta);
                cmd.Parameters.AddWithValue("@cod_ciu", cod_ciu);
                cmd.Parameters.AddWithValue("@cod_suc", cod_suc);
                cmd.Parameters.AddWithValue("@cod_cco", cod_cco);
                cmd.Parameters.AddWithValue("@tipoblc", 0);
                cmd.Parameters.AddWithValue("@IncluirCierre", 1);
                cmd.Parameters.AddWithValue("@codemp", codemp);
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(dt);
                con.Close();
                return dt;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }
        


        private static void CellExportingHandler(object sender, GridCellExcelExportingEventArgs e)
        {
            e.Range.CellStyle.Font.Size = 10;
            e.Range.CellStyle.Font.FontName = "Segoe UI";
            if (
                e.ColumnName == "sal_ini" || e.ColumnName == "debitos" || e.ColumnName == "creditos" || e.ColumnName == "sal_fin" ||
                e.ColumnName == "deb_mov" || e.ColumnName == "cre_mov" || e.ColumnName == "bas_mov"
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

                string tag = (sender as Button).Tag.ToString();
                SfDataGrid grid = new SfDataGrid();

                switch (tag)
                {
                    case "saldos":
                        grid = GridAcumulado;
                        break;
                    case "movimiento":
                        grid = GridMov;
                        break;
                }


                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExportMode = ExportMode.Value;
                options.ExcelVersion = ExcelVersion.Excel2013;
                options.CellsExportingEventHandler = CellExportingHandler;


                var excelEngine = grid.ExportToExcel(grid.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];
                workBook.Worksheets[0].AutoFilters.FilterRange = workBook.Worksheets[0].UsedRange;

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

                    if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archvo", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al exportar:" + w);
            }
        }

        private void GridAcumulado_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            if (GridAcumulado.SelectedIndex > 0)
            {

                if (AllDocument.IsChecked == true) LoadMov();
            }


        }


        public async void LoadMov()
        {
            try
            {

                GridAcumulado.IsEnabled = false;
                GridMov.ItemsSource = null;




                TxCuenta.Text = "";
                TxTercero.Text = "";
                TxCiudad.Text = "";
                TxCcosto.Text = "";
                TxDebitos.Text = "";
                TxCreditos.Text = "";


                sfBusyIndicatorMov.IsBusy = true;

                DataRowView row = (DataRowView)GridAcumulado.SelectedItems[0];


                if (row["per_doc"].ToString() == "13")
                {
                    sfBusyIndicatorMov.IsBusy = false;
                    GridAcumulado.IsEnabled = true;
                    GridAcumulado.Focus();
                    return;
                }

                string cod_cli = TxCod_Ter.Text.Trim();
                string cod_cta = TxCod_Cta.Text.Trim();


                string dateInput = "01/" + row["per_doc"].ToString() + "/" + row["ano"].ToString();
                DateTime fecinicial = DateTime.Parse(dateInput);

                int mes = fecinicial.Month == 12 ? fecinicial.Month : fecinicial.Month + 1;

                DateTime fechafinal = fecinicial.Month == 12 ? new DateTime(fecinicial.Year, 12, 31) : new DateTime(fecinicial.Year, mes, 1).AddDays(-1);




                if (fecinicial.Month == 13) return;

                StringBuilder sb = new StringBuilder();
                sb.Append(" declare @fechaIni as date ; set @fechaIni='" + fecinicial.ToString("dd/MM/yyyy") + "';declare @fechaFin as date ; set @fechaFin='" + fechafinal.ToString("dd/MM/yyyy") + "'");
                sb.Append(" SELEct cab_doc.idreg ,cue_doc.idreg as idregcue,cab_doc.cod_trn,cab_doc.num_trn,cab_doc.fec_trn,cue_doc.cod_cta,cue_doc.cod_cco,cue_doc.cod_ter,comae_ter.nom_ter,");
                sb.Append(" comae_cta.nom_cta,comae_ciu.nom_ciu,comae_cco.nom_cco,comae_suc.nom_suc,cue_doc.doc_mov,cue_doc.cod_ciu,cue_doc.cod_cco,cue_doc.cod_suc, ");
                sb.Append(" cue_doc.doc_ref,cue_doc.doc_cruc,cue_doc.num_chq,cue_doc.bas_mov,cue_doc.deb_mov,cue_doc.cre_mov, cab_DOC.factura,des_mov ");
                sb.Append(" FROM coCUE_DOC cue_doc inner join cocab_doc as cab_doc on cab_doc.idreg = cue_doc.idregcab and cue_doc.cod_cta = '" + cod_cta.Trim() + "' and ");
                if (cod_cli != "") sb.Append(" cue_doc.cod_ter='" + cod_cli.Trim() + "' and  ");


                sb.Append(" year(cab_doc.fec_trn) = year(@fechaIni) and convert(date, cab_doc.fec_trn) between  @FechaIni and @FechaFin inner join comae_trn as mae_trn on mae_trn.cod_trn = cab_doc.cod_trn ");
                sb.Append(" and (mae_trn.tip_blc=0 or mae_trn.tip_blc=" + (tipo + 1).ToString() + ")");
                sb.Append(" inner join comae_cta as comae_cta on comae_cta.cod_cta = cue_doc.cod_cta ");
                sb.Append(" left join comae_ter on comae_ter.cod_ter = cue_doc.cod_ter  ");
                sb.Append(" left join comae_ciu on comae_ciu.cod_ciu = cue_doc.cod_ciu ");
                sb.Append(" left join comae_cco on comae_cco.cod_cco = cue_doc.cod_cco ");
                sb.Append(" left join comae_suc on comae_suc.cod_suc = cue_doc.cod_suc ");
                sb.Append(" and (comae_cta.tip_blc=0 or comae_cta.tip_blc=" + (tipo + 1).ToString() + ")");
                sb.Append(" ORDER BY cod_cta,cab_doc.fec_trn ");


                var slowTask = Task<DataTable>.Factory.StartNew(() => LoadDocMov(sb.ToString()));
                await slowTask;

                if (slowTask.IsCompleted)
                {
                    double deb_mov = 0;
                    double cre_mov = 0;

                    GridMov.ItemsSource = slowTask.Result.DefaultView;
                    double.TryParse(slowTask.Result.Compute("Sum(deb_mov)", "").ToString(), out deb_mov);
                    double.TryParse(slowTask.Result.Compute("Sum(cre_mov)", "").ToString(), out cre_mov);

                    TxDebitos.Text = deb_mov.ToString("N");
                    TxCreditos.Text = cre_mov.ToString("N");

                }

                sfBusyIndicatorMov.IsBusy = false;
                GridAcumulado.IsEnabled = true;
                GridAcumulado.Focus();

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar documentos:" + w);
            }
        }


        private DataTable LoadDocMov(string query)
        {
            try
            {
                System.Data.DataTable dt = SiaWin.Func.SqlDT(query, "tabla", idemp);
                return dt;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }

        private void GridMov_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            try
            {
                if (GridMov.SelectedIndex > 0)
                {
                    DataRowView row = (DataRowView)GridMov.SelectedItems[0];
                    TxCuenta.Text = row["nom_cta"].ToString().Trim();
                    TxTercero.Text = row["nom_ter"].ToString().Trim();
                    TxCiudad.Text = row["nom_ciu"].ToString().Trim();
                    TxCcosto.Text = row["nom_cco"].ToString().Trim();
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al ver movimiento:" + w);
            }
        }

        private void BtnViewDoc_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (GridMov.SelectedIndex >= 0)
                {
                    int id = 1;
                    DataRowView row = (DataRowView)GridMov.SelectedItems[0];
                    int idreg = Convert.ToInt32(row["idreg"]);
                    if (idreg <= 0) return;
                    SiaWin.TabTrn(0, idemp, true, idreg, id, WinModal: true);
                }
                else
                {
                    MessageBox.Show("debe de seleccionar algun documento para abrir", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("errro al abrir documento" + w);
            }
        }


    }
}
