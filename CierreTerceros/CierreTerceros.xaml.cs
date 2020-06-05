using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

    //Sia.PublicarPnt(9665, "CierreTerceros");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9665, "CierreTerceros");  
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();



    public partial class CierreTerceros : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        public CierreTerceros()
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
                this.Title = "Cierre de terceros " + cod_empresa + "-" + nomempresa;
                CargarEmpresas();
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        public void CargarEmpresas()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select businessid, businesscode, businessname, Businessalias from business where (select Seg_AccProjectBusiness.Access from Seg_AccProjectBusiness where GroupId = " + SiaWin._UserGroup.ToString() + "  and ProjectId = " + SiaWin._ProyectId.ToString() + " and Access = 1 and Business.BusinessId = Seg_AccProjectBusiness.BusinessId)= 1");
            DataTable empresas = SiaWin.Func.SqlDT(sb.ToString(), "Empresas", 0);
            comboBoxEmpresas.ItemsSource = empresas.DefaultView;
            if (empresas.Rows.Count > 0) comboBoxEmpresas.SelectedIndex = 0;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace((sender as TextBox).Text)) return;
                else
                {
                    string table = (sender as TextBox).Tag.ToString().Trim();
                    string value = (sender as TextBox).Text.ToString().Trim();
                    string code = "";
                    switch (table)
                    {
                        case "comae_cta": code = "cod_cta"; break;
                        case "comae_ter": code = "cod_ter"; break;
                        case "comae_trn": code = "cod_trn"; break;
                    }

                    DataTable dt = SiaWin.Func.SqlDT("select * from  " + table + "  where  " + code + "='" + value + "' ", "Empresas", idemp);
                    if (dt.Rows.Count <= 0)
                    {
                        MessageBox.Show("el codigo ingresado no existe", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        (sender as TextBox).Text = "";
                    }
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("errro al buscar codigo:" + w);
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.F8 || e.Key == Key.Enter)
                {
                    e.Handled = true;
                    string table = (sender as TextBox).Tag.ToString().Trim();
                    string value = (sender as TextBox).Text.ToString().Trim();
                    string codetbl = ""; string nomtbl = ""; string tit = "";bool mostrar = false;
                    switch (table)
                    {
                        case "comae_trn": codetbl = "cod_trn"; nomtbl = "nom_trn"; tit = "Transacciones"; mostrar = true; break;
                        case "comae_cta": codetbl = "cod_cta"; nomtbl = "nom_cta"; tit = "Cuentas"; break;
                        case "comae_ter": codetbl = "cod_ter"; nomtbl = "nom_ter"; tit = "Terceros"; break;
                    }

                    
                    string cmptabla = table; string cmpcodigo = codetbl; string cmpnombre = nomtbl; string cmporden = "idrow"; string cmpidrow = "idrow"; string cmptitulo = "Maestra de " + tit; bool mostrartodo = mostrar; string cmpwhere = "";
                    int idr = 0; string code = ""; string nom = "";
                    dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, cnEmp, mostrartodo, cmpwhere, idEmp: idemp);
                    winb.ShowInTaskbar = false;
                    winb.Owner = Application.Current.MainWindow;
                    winb.Width = 500;
                    winb.Height = 400;
                    winb.ShowDialog();
                    idr = winb.IdRowReturn;
                    code = winb.Codigo;
                    nom = winb.Nombre;
                    winb = null;
                    if (!string.IsNullOrEmpty(code))
                    {
                        (sender as TextBox).Text = code.Trim();
                    }                    
                    if (e.Key == Key.Enter)
                    {
                        var uiElement = e.OriginalSource as UIElement;
                        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar:" + w);
            }

        }


        private async void BtnEjecutar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region validaciones
                if (string.IsNullOrWhiteSpace(tx_cta.Text))
                {
                    MessageBox.Show("ingrese una cuenta", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                if (string.IsNullOrWhiteSpace(tx_trn.Text))
                {
                    MessageBox.Show("ingrese una tipo de transaccion", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                if (string.IsNullOrWhiteSpace(tx_ter.Text))
                {
                    MessageBox.Show("ingrese una cuenta", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (comboBoxEmpresas.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione una empresa", "filtro", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                #endregion

                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                GridConfiguracion.IsEnabled = false;
                sfBusyIndicator.IsBusy = true;
                BtnEjecutar.IsEnabled = false;
                DateTime fec = Convert.ToDateTime(Fec.Value.ToString());
                int fecha = fec.Year;
                string codemp = comboBoxEmpresas.SelectedValue.ToString();
                string cuenta = tx_cta.Text;
                string ter = tx_ter.Text;
                string trn = tx_trn.Text;

                SiaWin.Auditor(0, "Ejecuto El cierre del tercero " + ter + " Año:" + fecha.ToString() + " cuenta:" + cuenta + " Empresa:" + codemp + "", 2, 194);
                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(fecha.ToString(), cuenta, ter, trn, codemp, source.Token), source.Token);
                await slowTask;
                
                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    tx_cta.Text = "";
                    tx_ter.Text = "";
                    int idreg = Convert.ToInt32(((DataSet)slowTask.Result).Tables[0].Rows[0]["idreg"]);
                    SiaWin.TabTrn(0, idemp, true, idreg, 1, WinModal: true);
                }

                BtnEjecutar.IsEnabled = true;
                this.sfBusyIndicator.IsBusy = false;
                GridConfiguracion.IsEnabled = true;
            }
            catch (Exception w)
            {
                MessageBox.Show("errror en el cierre del tercero:" + w);
            }
        }

        private DataSet LoadData(string anno, string cuenta, string ter, string trn,string empresas, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpSpCierreTerceros", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@anno", anno);
                cmd.Parameters.AddWithValue("@codcta", cuenta);
                cmd.Parameters.AddWithValue("@tercie", ter);
                cmd.Parameters.AddWithValue("@cod_trn", trn);
                cmd.Parameters.AddWithValue("@codemp", empresas);
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds);
                con.Close();
                return ds;
            }
            catch (SqlException ex)
            {
                MessageBox.Show("error sql:"+ex);
                return null;
            }
        }


        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }






    }
}
