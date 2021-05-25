using MenuReporteParametros;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
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
    //Sia.PublicarPnt(9696, "MenuReporteParametros");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9696, "MenuReporteParametros");  
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();
    public partial class MenuReporteParametros : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";


        DataTable dtpara = new DataTable();
        DataTable dtConfigRep = new DataTable();

        public MenuReporteParametros()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
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
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadConfig();
                loadItems();
            }
            catch (Exception w)
            {
                MessageBox.Show("error Window_Loaded:" + w);
            }
        }

        public async void loadItems()
        {
            try
            {
                GridMenu.ItemsSource = null;
                GridParametros.ItemsSource = null;
                dtpara.Clear();

                sfBusyIndicator1.IsBusy = true;
                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadItems());
                await slowTask;

                if (slowTask.Result.Tables[0].Rows.Count > 0)
                {
                    GridMenu.ItemsSource = slowTask.Result.Tables[0].DefaultView;
                    dtpara.Clear();
                    dtpara = slowTask.Result.Tables[1];
                    dtConfigRep.Clear();
                    dtConfigRep = slowTask.Result.Tables[2];
                }
                sfBusyIndicator1.IsBusy = false;

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar items:" + w);
            }
        }

        public DataSet LoadItems()
        {
            try
            {
                DataSet ds = new DataSet();

                string select = "select idrow,name_item,reporte from Menu_Reports where idserver=1 ";
                DataTable dtmenu = SiaWin.Func.SqlDT(select, "temp", 0);
                ds.Tables.Add(dtmenu);

                string param = "select idrow,idrow_rep,parameter,isValid,isTable,isCombo,isMultiValue,nameMaster,tabla,cod_tbl,nom_tbl,whereMaster,[columns],orderMaster,viewAll,isBusiness from Menu_Reports_Parameter ";
                DataTable dtparm = SiaWin.Func.SqlDT(param, "parm", 0);
                ds.Tables.Add(dtparm);


                string config  = "select ServerIP,UserServer,UserServerPassword,UserSql,UserSqlPassword from ReportServer;";
                DataTable dtconfig = SiaWin.Func.SqlDT(config, "config", 0);
                ds.Tables.Add(dtconfig);

                return ds;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar informacion:" + w);
                return null;
            }
        }

        private void GridMenu_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            try
            {
                if (GridMenu.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)GridMenu.SelectedItems[0];

                    string idmenu = row["idrow"].ToString();
                    DataRow[] rows = dtpara.Select("idrow_rep='" + idmenu + "' ");
                    if (rows.Length > 0)
                    {
                        DataTable dtp = rows.CopyToDataTable();
                        GridParametros.ItemsSource = dtp.DefaultView;
                    }
                    else
                    {
                        GridParametros.ItemsSource = null;
                    }


                }


            }
            catch (Exception w)
            {
                MessageBox.Show("error al seleccioanr:" + w);
            }
        }


        public ReportParameterInfoCollection LoadParameters(string reporte)
        {
            try
            {

                Microsoft.Reporting.WinForms.ReportViewer viewer = new Microsoft.Reporting.WinForms.ReportViewer();
                viewer.ServerReport.ReportPath = reporte;
                viewer.ServerReport.ReportServerUrl = new Uri(dtConfigRep.Rows[0]["ServerIP"].ToString().Trim());
                ReportServerCredentials rsCredentials = viewer.ServerReport.ReportServerCredentials;
                rsCredentials.NetworkCredentials = new System.Net.NetworkCredential(dtConfigRep.Rows[0]["UserServer"].ToString().Trim(), dtConfigRep.Rows[0]["UserServerPassword"].ToString().Trim());
                List<Microsoft.Reporting.WinForms.DataSourceCredentials> crdentials = new List<Microsoft.Reporting.WinForms.DataSourceCredentials>();
                viewer.SetDisplayMode(DisplayMode.Normal);
                viewer.ProcessingMode = Microsoft.Reporting.WinForms.ProcessingMode.Remote;
                foreach (var dataSource in viewer.ServerReport.GetDataSources())
                {
                    Microsoft.Reporting.WinForms.DataSourceCredentials credn = new Microsoft.Reporting.WinForms.DataSourceCredentials();
                    credn.Name = dataSource.Name;
                    credn.UserId = dtConfigRep.Rows[0]["UserSql"].ToString().Trim();
                    credn.Password = dtConfigRep.Rows[0]["UserSqlPassword"].ToString().Trim();
                    crdentials.Add(credn);
                }
                ReportParameterInfoCollection parameters = viewer.ServerReport.GetParameters();
                return parameters;

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar parametro:" + w);
                return null;
            }
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (GridMenu.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)GridMenu.SelectedItems[0];
                    int idrow = Convert.ToInt32(row["idrow"]);
                    string name = row["name_item"].ToString();
                    string reporte = row["reporte"].ToString();

                    WinParm w = new WinParm();
                    w.par_report = LoadParameters(reporte);
                    w.ShowInTaskbar = false;
                    w.idrow_rep = idrow;
                    w.name_rep = name;
                    w.Owner = Application.Current.MainWindow;
                    w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    w.ShowDialog();

                    if (w.flag) loadItems();

                }
                else
                {
                    MessageBox.Show("debe de seleccionar un reporte para agregar los parametros", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }


            }
            catch (Exception w)
            {
                MessageBox.Show("error en BtnAgregar_Click:" + w);
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (GridParametros.SelectedIndex >= 0)
                {
                    DataRowView rowMenu = (DataRowView)GridMenu.SelectedItems[0];
                    int idrowrep = Convert.ToInt32(rowMenu["idrow"]);
                    string namerep = rowMenu["name_item"].ToString();
                    string reporte = rowMenu["reporte"].ToString();

                    DataRowView row = (DataRowView)GridParametros.SelectedItems[0];
                    int idrow = Convert.ToInt32(row["idrow"]);
                    WinParm w = new WinParm();
                    w.par_report = LoadParameters(reporte);
                    w.idrowpar = idrow;
                    w.idrow_rep = idrowrep;
                    w.name_rep = namerep;
                    w.ShowInTaskbar = false;
                    w.Owner = Application.Current.MainWindow;
                    w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    w.ShowDialog();

                    if (w.flag) loadItems();

                }
                else
                {
                    MessageBox.Show("debe de seleccionar un parametro del reporte", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error en BtnAgregar_Click:" + w);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (GridParametros.SelectedIndex >= 0)
                {


                    DataRowView row = (DataRowView)GridParametros.SelectedItems[0];
                    int idrow = Convert.ToInt32(row["idrow"]);
                    string parameter = row["parameter"].ToString();

                    MessageBoxResult message = MessageBox.Show($"ustede desea eliminar el parametro:{parameter}", "alerta", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

                    if (message == MessageBoxResult.Yes)
                    {
                        string delete = $"delete Menu_Reports_Parameter where idrow={idrow}";

                        if (SiaWin.Func.SqlCRUD(delete, 0) == true)
                        {
                            loadItems();
                        }
                    }




                }
                else
                {
                    MessageBox.Show("debe de seleccionar un parametro del reporte", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error en BtnDelete_Click:" + w);
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}
