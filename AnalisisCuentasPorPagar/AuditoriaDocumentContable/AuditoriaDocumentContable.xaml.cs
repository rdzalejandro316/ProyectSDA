using Microsoft.Reporting.WinForms;
using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.Windows.Tools.Controls;
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
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{

    //Sia.PublicarPnt(9640, "AuditoriaDocumentContable");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9640, "AuditoriaDocumentContable");  
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();
    public partial class AuditoriaDocumentContable : Window
    {
       dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        int cosn = 0;
        public DataTable DTserver;
        public AuditoriaDocumentContable()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
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
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Auditoria Documentos Contables" + cod_empresa + "-" + nomempresa;

                Tx_fecini.Text = DateTime.Now.AddMonths(-1).ToString(); 
                Tx_fecfin.Text = DateTime.Now.ToString();

                DTserver = cargarDatosSerividor();
                CargarEmpresas();
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

        public void CargarEmpresas()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select businessid, businesscode, businessname, Businessalias from business where (select Seg_AccProjectBusiness.Access from Seg_AccProjectBusiness where GroupId = " + SiaWin._UserGroup.ToString() + "  and ProjectId = " + SiaWin._ProyectId.ToString() + " and Access = 1 and Business.BusinessId = Seg_AccProjectBusiness.BusinessId)= 1");
            DataTable empresas = SiaWin.Func.SqlDT(sb.ToString(), "Empresas", 0);
            comboBoxEmpresas.ItemsSource = empresas.DefaultView;
        }

        private void BtnConsultar_Click(object sender, RoutedEventArgs e)
        {

            if (comboBoxEmpresas.SelectedIndex < 0)
            {
                MessageBox.Show("seleccione una o mas empresas", "filtro", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            try
            {
                List<ReportParameter> parameters = new List<ReportParameter>();
                TabItemExt tabItemExt1 = new TabItemExt();
                tabItemExt1.Header = "Consulta - "+ comboBoxEmpresas.SelectedValue.ToString();
                tabItemExt1.Name = "tab1";
                parameters.Add(new ReportParameter("fechaini", Tx_fecini.Text));
                parameters.Add(new ReportParameter("fechafin", Tx_fecfin.Text));                
                parameters.Add(new ReportParameter("codemp", comboBoxEmpresas.SelectedValue.ToString()));

                WindowsFormsHost winFormsHost = new WindowsFormsHost();
                ReportViewer viewer = new ReportViewer();
                viewer.ServerReport.ReportServerUrl = new Uri("http://192.168.0.12:7333/ReportserverGS");
                viewer.ServerReport.ReportPath = "/Contabilidad/AuditoriaContable";

                viewer.ProcessingMode = ProcessingMode.Remote;
                ReportServerCredentials rsCredentials = viewer.ServerReport.ReportServerCredentials;
                rsCredentials.NetworkCredentials = new System.Net.NetworkCredential(DTserver.Rows[0]["UserServer"].ToString(), DTserver.Rows[0]["UserServerPassword"].ToString());
                List<DataSourceCredentials> crdentials = new List<DataSourceCredentials>();

                foreach (var dataSource in viewer.ServerReport.GetDataSources())
                {
                    DataSourceCredentials credn = new DataSourceCredentials();
                    credn.Name = dataSource.Name;
                    System.Windows.MessageBox.Show(dataSource.Name);
                    credn.UserId = DTserver.Rows[0]["UserSql"].ToString();
                    credn.Password = DTserver.Rows[0]["UserSqlPassword"].ToString();
                    crdentials.Add(credn);
                }

                viewer.ServerReport.SetDataSourceCredentials(crdentials);
                viewer.ServerReport.SetParameters(parameters);
                viewer.RefreshReport();


                winFormsHost.Child = viewer;
                tabItemExt1.Content = winFormsHost;
                TabControl1.Items.Add(tabItemExt1);
                UpdateLayout();

            }
            catch (Exception w)
            {
                MessageBox.Show("error en el reporte 1:" + w);
            }
        }        

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception w)
            {
                MessageBox.Show("error al consultar:" + w);
            }
        }





    }
}
