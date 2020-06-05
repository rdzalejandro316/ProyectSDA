using EliminacionActivo;
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

namespace SiasoftAppExt
{

    //    Sia.PublicarPnt(9661,"EliminacionActivo");
    //    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9661,"EliminacionActivo");
    //    ww.ShowInTaskbar = false;
    //    ww.Owner = Application.Current.MainWindow;
    //    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //    ww.ShowDialog();

    public partial class EliminacionActivo : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        public EliminacionActivo()
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
                this.Title = "Eliminacion de activos";
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F8 || (e.Key == Key.Enter && string.IsNullOrEmpty((sender as TextBox).Text)))
            {
                int idr = 0; string code = ""; string nom = "";
                dynamic winb = SiaWin.WindowBuscar("afmae_act", "cod_act", "nom_act", "cod_act", "idrow", "Maestra de Activos", SiaWin.Func.DatosEmp(idemp), false, "", idEmp: idemp);
                winb.ShowInTaskbar = false;
                winb.Owner = Application.Current.MainWindow;
                winb.ShowDialog();
                idr = winb.IdRowReturn;
                code = winb.Codigo;
                nom = winb.Nombre;
                winb = null;
                if (idr > 0)
                {
                    (sender as TextBox).Text = code.Trim();
                    tx_name.Text = nom.Trim();
                    var uiElement = e.OriginalSource as UIElement;
                    uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                }
                e.Handled = true;
            }
            if (e.Key == Key.Enter)
            {
                var uiElement = e.OriginalSource as UIElement;
                uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
            }
        }

        private void Cuen_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty((sender as TextBox).Text)) return;

            if (!valid((sender as TextBox).Text))
            {
                MessageBox.Show("el activo ingresado no existe", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                int idr = 0; string code = ""; string nom = "";
                dynamic winb = SiaWin.WindowBuscar("afmae_act", "cod_act", "nom_act", "cod_act", "idrow", "Maestra de activos", SiaWin.Func.DatosEmp(idemp), false, "", idEmp: idemp);
                winb.ShowInTaskbar = false;
                winb.Owner = Application.Current.MainWindow;
                winb.ShowDialog();
                idr = winb.IdRowReturn;
                code = winb.Codigo;
                nom = winb.Nombre;
                winb = null;
                if (idr > 0)
                {
                    (sender as TextBox).Text = code.Trim();
                    tx_name.Text = nom.Trim();

                    var uiElement = e.OriginalSource as UIElement;
                    uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                }
                else { (sender as TextBox).Text = ""; tx_name.Text = ""; };
            }
        }

        public bool valid(string act)
        {
            bool val = false;
            System.Data.DataTable dt = SiaWin.Func.SqlDT("select * from afmae_act where cod_act='" + act + "'  ", "tabla", idemp);
            if (dt.Rows.Count > 0)
            {
                val = true;
                tx_name.Text = dt.Rows[0]["nom_act"].ToString().Trim();
            }

            return val;
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(tx_activo.Text))
                {
                    MessageBox.Show("ingrese un activo", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                System.Data.DataTable dt = SiaWin.Func.SqlDT("select * from Afcue_doc where cod_act='" + tx_activo.Text + "';", "tabla", idemp);
                if (dt.Rows.Count > 0)
                {
                    MessageBox.Show("el activo contiene movimientos", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    Documentos win = new Documentos();
                    win.activo = tx_activo.Text;
                    win.ShowInTaskbar = false;
                    win.Owner = Application.Current.MainWindow;
                    win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    win.ShowDialog();
                }
                else
                {
                    if (MessageBox.Show("Usted desea eliminar el activo:"+ tx_activo.Text, "Alerta eliminacion", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        string query = "delete afmae_act where cod_act='" + tx_activo.Text + "' ";
                        if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                        {
                            MessageBox.Show("la eliminacion del activo fue exitosa exitosa", "proceso", MessageBoxButton.OK, MessageBoxImage.Information);
                            tx_activo.Text = "";
                            tx_name.Text = "";
                        }
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al eliminar:" + w);
            }
        }







    }
}
