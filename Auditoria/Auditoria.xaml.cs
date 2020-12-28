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

    //Sia.PublicarPnt(9339,"Auditoria");
    //dynamic WinDescto = ((Inicio)Application.Current.MainWindow).WindowExt(9339, "Auditoria");
    //WinDescto.ShowInTaskbar = false;
    //WinDescto.Owner = Application.Current.MainWindow;
    //WinDescto.WindowStartupLocation = WindowStartupLocation.CenterScreen;
    //WinDescto.ShowDialog(); 

    public partial class Auditoria : UserControl
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        public string eventBuscar = "";
        

        public Auditoria()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, 0, 0, 0, "Ingreso Auditoria SiasoftApp ", "");
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
                this.Title = "Cierre de Traslado - Empresa:" + cod_empresa + "-" + nomempresa;

                Fec_Ini.Text = DateTime.Now.ToString();
                Fec_Fin.Text = DateTime.Now.ToString();
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
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

                GrillaAuditoria.ItemsSource = null;

                string fi = Fec_Ini.Text.ToString();
                string ff = Fec_Fin.Text.ToString();

                SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idemp, 0, 0, 0, "Consulto Auditoria Fecha Inicial:" + fi.ToString() + "- Fecha Final:" + ff.ToString(), "");
                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(fi, ff, eventBuscar, source.Token), source.Token);
                await slowTask;
                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    GrillaAuditoria.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                    TotalReg.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();
                    TabControl1.SelectedIndex = 2;
                    TabControl1.SelectedIndex = 1;
                }
                ConfigGrid.IsEnabled = true;
                this.sfBusyIndicator.IsBusy = false;
            }
            catch (SqlException w)
            {
                MessageBox.Show("error1:" + w);
            }
            catch (Exception ex)
            {
                MessageBox.Show("erro2:" + ex);
                this.Opacity = 1;
            }
        }
        

        private DataSet LoadData(string FechaIN, string FechaFI, string filtro, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpAuditoria", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaIni", FechaIN);
                cmd.Parameters.AddWithValue("@FechaFin", FechaFI);
                cmd.Parameters.AddWithValue("@Filtro", filtro);
                da = new SqlDataAdapter(cmd);
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



        private void BTNexportar_Click(object sender, RoutedEventArgs e)
        {
            var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
            options.ExportMode = ExportMode.Value;
            options.ExcelVersion = ExcelVersion.Excel2013;



            var excelEngine = GrillaAuditoria.ExportToExcel(GrillaAuditoria.View, options);
            var workBook = excelEngine.Excel.Workbooks[0];
            workBook.ActiveSheet.Columns[4].NumberFormat = "0.0";
            workBook.ActiveSheet.Columns[5].NumberFormat = "0.0";
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
                    //Launching the Excel file using the default Application.[MS Excel Or Free ExcelViewer]
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (eventBuscar != "")
            {
                BTNconsultar.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }
        // ROWS automatico ******************************************************************************

        GridRowSizingOptions gridRowResizingOptions = new GridRowSizingOptions();

        double autoHeight = 20;

        List<string> excludeColumns = new List<string>() { "UserId", "UserAlias", "UserName", "GroupName", "BusinessName", "ModulesName", "Date_Event", "UserWindows", "MachineName", "EventError" };

        private void dataGridCxC_QueryRowHeight(object sender, Syncfusion.UI.Xaml.Grid.QueryRowHeightEventArgs e)
        {
            if (this.GrillaAuditoria.GridColumnSizer.GetAutoRowHeight(e.RowIndex, gridRowResizingOptions, out autoHeight))
            {
                if (autoHeight > 24)
                {
                    e.Height = autoHeight;
                    e.Handled = true;
                }

                if (e.RowIndex == 0)
                {
                    e.Height = 30;
                    e.Handled = true;
                }
            }
        }





    }
}

