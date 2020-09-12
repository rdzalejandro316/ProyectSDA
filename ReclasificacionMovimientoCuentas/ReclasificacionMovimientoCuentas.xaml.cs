using System;
using System.Collections.Generic;
using System.Data;
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

    //    Sia.PublicarPnt(9631,"ReclasificacionMovimientoCuentas");
    //    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9631,"ReclasificacionMovimientoCuentas");
    //    ww.ShowInTaskbar = false;
    //    ww.Owner = Application.Current.MainWindow;
    //    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //    ww.ShowDialog();

    public partial class ReclasificacionMovimientoCuentas : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        int idmodulo = 1;
        public ReclasificacionMovimientoCuentas()
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
                this.Title = "Reclasificacion Movimientos Cuentas Por Fecha";

                fec_ini.Text = DateTime.Now.ToString();
                fec_fin.Text = DateTime.Now.ToString();
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
                dynamic winb = SiaWin.WindowBuscar("comae_cta", "cod_cta", "nom_cta", "cod_cta", "idrow", "Maestra de cuentas", SiaWin.Func.DatosEmp(idemp), false, "", idEmp: idemp);
                winb.ShowInTaskbar = false;
                winb.Owner = Application.Current.MainWindow;
                winb.Width = 400;
                winb.Height = 400;
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

        private void Cuen_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty((sender as TextBox).Text)) return;

            if (!valid((sender as TextBox).Text))
            {
                MessageBox.Show("la cuenta ingresada no es valida ingrese una cuenta de la lista");
                int idr = 0; string code = ""; string nom = "";
                dynamic winb = SiaWin.WindowBuscar("comae_cta", "cod_cta", "nom_cta", "cod_cta", "idrow", "Maestra de cuentas", SiaWin.Func.DatosEmp(idemp), false, "", idEmp: idemp);
                winb.ShowInTaskbar = false;
                winb.Owner = Application.Current.MainWindow;
                winb.Width = 400;
                winb.Height = 400;
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
                else (sender as TextBox).Text = "";
            }
        }

        public bool valid(string cta)
        {
            System.Data.DataTable dt = SiaWin.Func.SqlDT("select * from comae_cta where cod_cta='" + cta + "'  ", "tabla", idemp);
            return dt.Rows.Count > 0 ? true : false;
        }


        private async void BtnReclasificacion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(fec_ini.Text) || string.IsNullOrEmpty(fec_fin.Text) || string.IsNullOrEmpty(cuen_ant.Text) || string.IsNullOrEmpty(cuen_nueva.Text))
                {
                    MessageBox.Show("llene todos los campos para realizar la reclasificacion", "Alerta", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                MessageBoxResult result = MessageBox.Show("Usted desea reclasificar la cuenta " + cuen_ant.Text.Trim() + " a " + cuen_nueva.Text.Trim() + " desde las fechas " + fec_ini.Text + " a " + fec_fin.Text + " ", "Confirmacion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    CancellationTokenSource source = new CancellationTokenSource();
                    CancellationToken token = source.Token;

                    GridConfiguracion.IsEnabled = false;
                    sfBusyIndicator.IsBusy = true;


                    string fec_inicial = fec_ini.Text.Trim();
                    string fec_final = fec_fin.Text.Trim();

                    string cta_ant = cuen_ant.Text.Trim();
                    string cta_nue = cuen_nueva.Text.Trim();

                    var slowTask = Task<DataTable>.Factory.StartNew(() => LoadData(fec_inicial, fec_final, cta_ant, source.Token), source.Token);
                    await slowTask;

                    if (((DataTable)slowTask.Result).Rows.Count > 0)
                    {
                        string query = "";
                        foreach (DataRow dr in ((DataTable)slowTask.Result).Rows)
                            query += "update Cocue_doc set cod_cta=replace(cod_cta,'" + cta_ant + "','" + cta_nue + "')   where idreg='" + dr["idreg"].ToString().Trim() + "';";

                        if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                        {
                            string tx = "Reclasifico Documentos en las cuentas  - codigo anterior: " + cta_ant + " a codigo nuevo:" + cta_nue + "  de la fecha " + fec_inicial + " a " + fec_final + "";
                            SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, idmodulo, -1, -9, tx, "");

                            MessageBox.Show("la reclasificacion fue exitosa de cuentas", "proceso", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else MessageBox.Show("fallo el proceso de reclasificacion por favor verifique con el administrador", "fallo del proceso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        MessageBox.Show("el rango de fechas que se ingreso para realizar la reclasificacion no contiene ningun documento", "Alerta", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }

                    GridConfiguracion.IsEnabled = true;
                    sfBusyIndicator.IsBusy = false;
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al realizar el proceso de reclasificar:" + w);
            }
        }


        private DataTable LoadData(string fec_ini, string fec_fin, string cuenta, CancellationToken cancellationToken)
        {
            try
            {
                string query = "select cue.idreg,cue.cod_trn,cue.num_trn,cab.fec_trn  ";
                query += "from Cocue_doc cue ";
                query += "inner join cocab_doc cab on cab.idreg = cue.idregcab ";
                query += "where convert(date,cab.fec_trn,105) between '" + fec_ini + "' and '" + fec_fin + "'  ";
                query += "and cue.cod_cta='" + cuenta + "' ";

                System.Data.DataTable dt = SiaWin.Func.SqlDT(query, "tabla", idemp);
                return dt;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }

        private async void BtnView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(fec_ini.Text) || string.IsNullOrEmpty(fec_fin.Text) || string.IsNullOrEmpty(cuen_ant.Text))
                {
                    MessageBox.Show("llene todos los campos para realizar la reclasificacion", "Alerta", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;

                GridConfiguracion.IsEnabled = false;
                sfBusyIndicator.IsBusy = true;


                string fec_inicial = fec_ini.Text;
                string fec_final = fec_fin.Text;

                string cta_ant = cuen_ant.Text;
                string cta_nue = cuen_nueva.Text;

                var slowTask = Task<DataTable>.Factory.StartNew(() => LoadData(fec_inicial, fec_final, cta_ant, source.Token), source.Token);
                await slowTask;

                if (((DataTable)slowTask.Result).Rows.Count > 0) SiaWin.Browse(((DataTable)slowTask.Result));
                else MessageBox.Show("no existe documentos con los filtros ingresados");

                GridConfiguracion.IsEnabled = true;
                sfBusyIndicator.IsBusy = false;

            }
            catch (Exception w)
            {
                MessageBox.Show("error:" + w);
            }
        }


    }
}
