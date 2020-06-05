using Microsoft.Reporting.WinForms;
using Syncfusion.Windows.Reports;
using Syncfusion.Windows.Reports.Data;
using Syncfusion.Windows.Reports.Viewer;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
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
using ProcessingMode = Microsoft.Reporting.WinForms.ProcessingMode;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9531, "MenuReporteWindow");  
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9531, "MenuReporteWindow");    
    //ww.ShowInTaskbar=false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
    //ww.ShowDialog();  
    public partial class MenuReporteWindow : Window
    {

        public bool tipo = false;
        public string Server = "";
        public string UserServer = "";
        public string UserServerPass = "";
        public string carpeta = "";

        dynamic SiaWin;
        int idemp = 0;
        string cnEmp = "";



        public MenuReporteWindow()
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
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SkinStorage.SetVisualStyle(this, "Metro");
                if (tipo == true)
                {
                    //MessageBox.Show("1");
                    viewer.Reset();
                    string xnameReporte = carpeta;
                    string usuario = UserServer;
                    string contraseña = UserServerPass;

                    viewer.ServerReport.ReportPath = xnameReporte;
                    viewer.ServerReport.ReportServerUrl = new Uri(Server); ;                    
                    viewer.SetDisplayMode(DisplayMode.Normal);
                    viewer.ProcessingMode = ProcessingMode.Remote;

                   
                    //ReportServerCredentials rsCredentials = viewer.ServerReport.ReportServerCredentials;
                    //rsCredentials.NetworkCredentials = new System.Net.NetworkCredential(usuario, contraseña);
                    //List<Microsoft.Reporting.WinForms.DataSourceCredentials> crdentials = new List<Microsoft.Reporting.WinForms.DataSourceCredentials>();
                    //foreach (var dataSource in viewer.ServerReport.GetDataSources())
                    //{
                    //Microsoft.Reporting.WinForms.DataSourceCredentials credn = new Microsoft.Reporting.WinForms.DataSourceCredentials();
                    //credn.UserId = "sa";
                    //credn.Password = "W654321*";
                    //crdentials.Add(credn);
                    //}

                    viewer.PrinterSettings.Collate = false;
                    viewer.RefreshReport();

                }
                //if (tipo == false) Navegador.Visibility = Visibility.Hidden;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar reporte:" + w);
            }


        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //if (tipo == true) web.Close();            
        }



    }
}
