using ImagenesDocumento;
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
    //Sia.PublicarPnt(9503, "ImagenesDocumento");  
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9503, "ImagenesDocumento");
    //ww.ShowInTaskbar=false;    
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
    //ww.ShowDialog();
    
    public partial class ImagenesDocumento : Window
    {

        dynamic SiaWin;
        
        int idemp = 0;
        string cnEmp = "";

        
        public int idregcab = 0;


        public ImagenesDocumento()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;

            System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
            idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
            cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
            string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (idregcab == 0 || idregcab == -1)
            {

                GridVis.Visibility = Visibility.Hidden;

                MessageBox.Show("no existe el documento ");
                WinCam.IsEnabled = false;
                return;
            }


            string select = "select * from incab_doc where idreg='" + idregcab + "' ";
            DataTable tabla = SiaWin.Func.SqlDT(select, "Clientes", idemp);
            if (tabla.Rows.Count > 0)
            {
                TX_NumDoc.Text = tabla.Rows[0]["num_trn"].ToString().Trim();
                TX_IdCab.Text = tabla.Rows[0]["idreg"].ToString().Trim();                       
            }                        

        }



        private void BTNgaleria_Click(object sender, RoutedEventArgs e)
        {
            Galeria ventana = new Galeria();
            ventana.ShowInTaskbar = false;
            ventana.Owner = Application.Current.MainWindow;
            ventana.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ventana.idrowcab = idregcab;
            ventana.ShowDialog();
        }

        private void BTNinsertar_Click(object sender, RoutedEventArgs e)
        {
            InsertarImage ventana = new InsertarImage();
            ventana.ShowInTaskbar = false;
            ventana.Owner = Application.Current.MainWindow;
            ventana.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ventana.idrowcab = idregcab;
            ventana.ShowDialog();
        }

        private void Camara_Click(object sender, RoutedEventArgs e)
        {
            Camara ventana = new Camara();
            ventana.ShowInTaskbar = false;
            ventana.Owner = Application.Current.MainWindow;
            ventana.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ventana.idrowcab = idregcab;
            ventana.ShowDialog();
        }



    }
}
