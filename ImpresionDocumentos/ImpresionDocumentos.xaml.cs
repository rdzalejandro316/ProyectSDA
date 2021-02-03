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
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9676, "ImpresionDocumentos");    
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9676, "ImpresionDocumentos");
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //ww.ShowDialog();


    public partial class ImpresionDocumentos : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public int idreg = 0;

        public ImpresionDocumentos()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
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
                this.Title = "Impresion de documentos - " + nomempresa;

                DataTable dt = SiaWin.Func.SqlDT("select NameFormat as name,id as id from FormatDoc", "tabla", 0);
                CbTipo.ItemsSource = dt.DefaultView;

            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void BtnImprimir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CbTipo.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione un tipo de impresion", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }


                string query = "select ModulesCode, NameFormat, ViewParameters, PathFormart, idserver, serv.ServerIP,UserServer,UserServerPassword ";
                query += "from FormatDoc ";
                query += "inner join ReportServer serv on serv.idrow = FormatDoc.idserver ";
                query += "where FormatDoc.id='" + CbTipo.SelectedValue.ToString().Trim() + "'  ";

                DataTable dt = SiaWin.Func.SqlDT(query, "tabla", 0);
                if (dt.Rows.Count > 0)
                {
                    string server = dt.Rows[0]["ServerIP"].ToString().Trim();
                    string path = dt.Rows[0]["PathFormart"].ToString().Trim();

                    Window w = new Window();

                    List<ReportParameter> parameters = new List<ReportParameter>();
                    parameters.Add(new ReportParameter("idreg", idreg.ToString()));
                    parameters.Add(new ReportParameter("usuario", SiaWin._UserName));

                    if (CbTipo.SelectedIndex == 3) parameters.Add(new ReportParameter("valorpesos", SiaWin._UserName));


                    WindowsFormsHost winFormsHost = new WindowsFormsHost();
                    ReportViewer viewer = new ReportViewer();
                    
                    viewer.ServerReport.ReportServerUrl = new Uri(server);
                    viewer.ServerReport.ReportPath = path;
                    viewer.ProcessingMode = ProcessingMode.Remote;
                    viewer.ServerReport.SetParameters(parameters);
                    viewer.SetDisplayMode(DisplayMode.PrintLayout);
                    switch (CbShowParm.SelectedIndex)
                    {
                        case 0: viewer.ShowParameterPrompts = false; break;
                        case 1: viewer.ShowParameterPrompts = true; break;
                    }                    
                    
                    viewer.RefreshReport();

                                                      
                  

                    winFormsHost.Child = viewer;
                    w.Content = winFormsHost;

                    w.ShowInTaskbar = false;
                    w.Owner = Application.Current.MainWindow;
                    w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    w.ShowDialog();

                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al imprimir:" + w);
            }
        }




        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }




    }
}
