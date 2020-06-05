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
    //    Sia.PublicarPnt(9630,"InactivarCuentasPorRango");
    //    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9630,"InactivarCuentasPorRango");
    //    ww.ShowInTaskbar = false;
    //    ww.Owner = Application.Current.MainWindow;
    //    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //    ww.ShowDialog();

    public partial class InactivarCuentasPorRango : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public InactivarCuentasPorRango()
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
                this.Title = "Inactivar cuentas";
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F8 || e.Key == Key.Enter)
            {
                int idr = 0; string code = ""; string nom = "";
                dynamic winb = SiaWin.WindowBuscar("comae_cta", "cod_cta", "nom_cta", "cod_cta", "idrow", "Maestra de cuentas", SiaWin.Func.DatosEmp(idemp), true, "", idEmp: idemp);
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



        private void BtnInactivar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (string.IsNullOrEmpty(cuen_des.Text) || string.IsNullOrEmpty(cuen_hast.Text))
                {
                    MessageBox.Show("los rangos de las cuentas estan vacias");
                    return;
                }

                if (!validad(cuen_des.Text) || !validad(cuen_hast.Text))
                {
                    MessageBox.Show("las cuentas registradas no existen");
                    return;
                }


                System.Data.DataTable dt = SiaWin.Func.SqlDT("select * from comae_cta where cod_cta between '" + cuen_des.Text + "' and '" + cuen_hast.Text + "' ", "tabla", idemp);
                string query = "";
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                        query += "update comae_cta set ind_act='0' where cod_cta='" + dr["cod_cta"].ToString() + "';";
                }

                if (!string.IsNullOrEmpty(query))
                {
                    if (SiaWin.Func.SqlCRUD(query, idemp) == true) MessageBox.Show("inactivacion exitosa");
                    else MessageBox.Show("erro al inactivar contacte con el administrador");
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error en la inactivacion:" + w);
            }
        }

        private void Cuen_LostFocus(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty((sender as TextBox).Text)) return;

            if (!validad((sender as TextBox).Text))
            {
                MessageBox.Show("la cuenta ingresada no existe");
                (sender as TextBox).Text = "";
            }
        }



        public bool validad(string tx)
        {
            System.Data.DataTable dt = SiaWin.Func.SqlDT("select * from comae_cta where cod_cta='" + tx + "' ", "tabla", idemp);
            return dt.Rows.Count > 0 ? true : false;
        }





    }
}
