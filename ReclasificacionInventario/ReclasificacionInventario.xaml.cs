﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SiasoftAppExt
{

    //Sia.PublicarPnt(9520, "ReclasificacionInventario");   
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9520, "ReclasificacionInventario");
    //ww.ShowInTaskbar=false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
    //ww.ShowDialog();

    public partial class ReclasificacionInventario : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";


        DataSet dsTemporal = new DataSet();
        string tipo = "INV";
        string saldos = "InIn_acum";

        public ReclasificacionInventario()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            LoadConfig();

            this.MinHeight = 500;
            this.MaxHeight = 500;
            this.MaxWidth = 800;
            this.MinWidth = 800;
        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Reclasificacion Inventario " + cod_empresa + "-" + nomempresa;
                cnEmp = SiaWin.Func.DatosEmp(idemp);
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }
        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TipoCBX.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione el tipo de reclasificacion", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    (sender as ToggleButton).IsChecked = false;
                    return;
                }

                if (((ToggleButton)sender).IsChecked == true) Card.IsEnabled = true;

                string Name = ((ToggleButton)sender).Name.ToString();
                foreach (ToggleButton item in GridTogle.Children)
                {
                    if (item.Name != Name) item.IsChecked = false;
                }

                CodAnt.Text = "";
                CodNue.Text = "";
                CodAntName.Text = "";
                CodNueName.Text = "";

                BTNviewSaldosNormales.IsEnabled = TipoCBX.SelectedIndex == 1 && (Name == "Togle1" || Name == "Togle5") ? true : false;
                BTNviewSaldosReclasificados.IsEnabled = TipoCBX.SelectedIndex == 1 && (Name == "Togle1" || Name == "Togle5") ? true : false;

                Tab_reclas.Text = ((ToggleButton)sender).Tag.ToString();
                BTNreclasificar.Tag = ((ToggleButton)sender).Tag.ToString();

                cargarTemporal(BTNreclasificar.Tag.ToString());

            }
            catch (Exception W)
            {
                MessageBox.Show("NADA:" + W);
            }

        }
        public void cargarTemporal(string tag)
        {
            try
            {
                dsTemporal.Clear();
                string index = TipoCBX.SelectedIndex.ToString();
                SqlConnection con = new SqlConnection(SiaWin._cn);
                con.Open();
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                cmd = new SqlCommand("_EmpReclasificacion", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Tag", tag);
                cmd.Parameters.AddWithValue("@tipo", index);
                cmd.Parameters.AddWithValue("@modulo", tipo);
                cmd.Parameters.AddWithValue("@codemp", cod_empresa);
                da = new SqlDataAdapter(cmd);
                da.Fill(dsTemporal);
                con.Close();
            }
            catch (Exception w)
            {
                MessageBox.Show("error al traer la temporal:" + w);
            }
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Togle1.IsChecked == false && Togle2.IsChecked == false && Togle3.IsChecked == false && Togle4.IsChecked == false && Togle5.IsChecked == false)
            {
                clean();
            }
        }
        public void clean()
        {
            CodAnt.Text = "";
            CodNue.Text = "";
            CodAntName.Text = "";
            CodNueName.Text = "";
            Card.IsEnabled = false;
            Tab_reclas.Text = "Title";

            foreach (ToggleButton item in GridTogle.Children)
                item.IsChecked = false;
        }

        private async void BTNreclasificar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                #region validaciones

                if (TipoCBX.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione el tipo de reclasificacion");
                    return;
                }

                if (TipoCBX.SelectedIndex == 0)
                {
                    if (existencia() == true)
                    {
                        (sender as TextBox).Text = "";
                        MessageBox.Show("El codigo nuevo que ingreso existe");
                        return;
                    }
                }
                if (TipoCBX.SelectedIndex == 1)
                {
                    if (existencia() == false)
                    {
                        MessageBox.Show("el codigo nuevo ingresado no existe");
                        return;
                    }
                }


                if (CodAnt.Text == "" || string.IsNullOrEmpty(CodAnt.Text))
                {
                    MessageBox.Show("el codigo anterior esta vacio");
                    return;
                }
                if (CodNue.Text == "" || string.IsNullOrEmpty(CodNue.Text))
                {
                    MessageBox.Show("el codigo nuevo esta vacio");
                    return;
                }

                #endregion

                MessageBoxResult result = MessageBox.Show("Usted desea reclasificar el codigo " + CodAnt.Text.Trim() + " a " + CodNue.Text.Trim() + " de " + BTNreclasificar.Tag.ToString().Trim() + " ?", "Confirmacion", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {

                    Card.IsEnabled = false;
                    sfBusyIndicator.IsBusy = true;

                    string tag = BTNreclasificar.Tag.ToString().Trim();
                    string cod_ant = CodAnt.Text.Trim();
                    string cod_nue = CodNue.Text.Trim();


                    var slowTask = Task<int>.Factory.StartNew(() => SlowDude(cod_ant, cod_nue, dsTemporal.Tables[0]));
                    await slowTask;

                    if (slowTask.IsCompleted)
                    {

                        if (TipoCBX.SelectedIndex == 1 && (tag == "Referencia" || tag == "Bodega"))
                        {
                            string column = tag == "Referencia" ? "cod_ref" : "cod_bod";

                            var OtherslowTask = Task<DataSet>.Factory.StartNew(() => Saldos(cod_ant, cod_nue, "1", column));
                            await OtherslowTask;
                            if (((DataSet)OtherslowTask.Result).Tables[0].Rows.Count > 0)
                            {
                                MessageBox.Show("los saldos fueron pasados exitosamente", "alerta", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }


                        SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, -9, -1, -9, "GENERO RECLASIFICACION DE INVENTARIO DE LA TABLA:" + Tab_reclas.Text + " cod_anterior:" + cod_ant + " cod_nuevo:" + cod_nue, "");
                        sfBusyIndicator.IsBusy = false;
                        MessageBox.Show("Reclasificacion exitosa - # de registros actualizados:" + ((int)slowTask.Result));
                        clean();
                    }

                    Card.IsEnabled = true;
                    GridMain.IsEnabled = true;
                    TipoCBX.IsEnabled = true;
                    sfBusyIndicator.IsBusy = false;
                }
            }
            catch (Exception w)
            {
                sfBusyIndicator.IsBusy = false;
                MessageBox.Show("error en la reclasificacion identifique si los campos que ingreso son los correctos para poder hacer el cambio", "ERROR CONTACTE CON EL ADMINISTRADOR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private int SlowDude(string cod_ant, string cod_nue, DataTable dt)
        {
            int NumReg = 1;

            if (dt.Rows.Count > 0)
            {
                string ValAnt = cod_ant;
                string ValNue = cod_nue;

                string update = "";
                foreach (DataRow item in dt.Rows)
                {
                    string tabla = item["table_name"].ToString().Trim();
                    string column = item["column_name"].ToString().Trim();
                    update += "update " + tabla + " set " + column + "='" + ValNue + "'  where " + column + "='" + ValAnt + "';";
                }

                System.Data.SqlClient.SqlConnection Conec = new System.Data.SqlClient.SqlConnection(cnEmp);
                System.Data.SqlClient.SqlCommand Sqlcmd = new System.Data.SqlClient.SqlCommand();
                Sqlcmd.CommandType = System.Data.CommandType.Text;
                Sqlcmd.CommandText = update; Sqlcmd.Connection = Conec;
                Conec.Open();
                NumReg = Sqlcmd.ExecuteNonQuery();
                Conec.Close();
            }
            return NumReg;
        }

        private void CodNue_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBox).Text == "") return;

            if (TipoCBX.SelectedIndex == 0)
            {

                if (existencia() == true)
                {
                    MessageBox.Show("!El codigo nuevo que ingreso existe intente de nuevo¡");
                    (sender as TextBox).Text = "";
                    CodNueName.Text = "";
                    return;
                }
            }
            if (TipoCBX.SelectedIndex == 1)
            {
                if (existencia() == false)
                {
                    MessageBox.Show("el codigo nuevo ingresado no existe", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
            }

        }

        public bool existencia()
        {
            bool bandera = false;

            string texto = CodNue.Text.ToString();
            string tabla = ""; string codigo = ""; string nombre = "";

            string tag = BTNreclasificar.Tag.ToString();
            switch (tag)
            {
                case "Referencia":
                    tabla = "inmae_ref"; codigo = "cod_ref"; nombre = "nom_ref";
                    break;
                case "Linea":
                    tabla = "inmae_tip"; codigo = "cod_tip"; nombre = "nom_tip";
                    break;
                case "Grupo":
                    tabla = "InMae_gru"; codigo = "cod_gru"; nombre = "nom_gru";
                    break;
                case "SubGrupo":
                    tabla = "InMae_sgr"; codigo = "Cod_sgr"; nombre = "nom_sgr";
                    break;
                case "Bodega":
                    tabla = "InMae_bod"; codigo = "cod_bod"; nombre = "nom_bod";
                    break;

            }

            string query = "select * from " + tabla + " where " + codigo + "='" + texto + "' ";

            DataTable dt = SiaWin.Func.SqlDT(query, "Existencia", idemp);

            if (dt.Rows.Count > 0)
            {
                CodNueName.Text = dt.Rows[0][nombre].ToString();
                bandera = true;
            }
            else
            {
                CodNueName.Text = "";
                bandera = false;
            }

            return bandera;
        }

        private void CodAnt_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBox).Text == "") return;
            string texto = CodAnt.Text.ToString();
            string tag = BTNreclasificar.Tag.ToString();
            string tabla = ""; string codigo = ""; string nombre = "";
            switch (tag)
            {
                case "Referencia":
                    tabla = "inmae_ref"; codigo = "cod_ref"; nombre = "nom_ref";
                    break;
                case "Linea":
                    tabla = "inmae_tip"; codigo = "cod_tip"; nombre = "nom_tip";
                    break;
                case "Grupo":
                    tabla = "InMae_gru"; codigo = "cod_gru"; nombre = "nom_gru";
                    break;
                case "SubGrupo":
                    tabla = "InMae_sgr"; codigo = "Cod_sgr"; nombre = "nom_sgr";
                    break;
                case "Bodega":
                    tabla = "InMae_bod"; codigo = "cod_bod"; nombre = "nom_bod";
                    break;

            }
            string query = "select * from " + tabla + " where " + codigo + "='" + texto + "' ";
            DataTable dt = SiaWin.Func.SqlDT(query, "Existencia", idemp);
            if (dt.Rows.Count > 0)
            {
                CodAntName.Text = dt.Rows[0][nombre].ToString();
            }
            else
            {
                MessageBox.Show("el codigo que ingreso no existe en la maestra de " + tag, "Alera", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                CodAnt.Text = "";
                CodAntName.Text = "";
            }
        }

        private void TipoCBX_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TipoCBX.SelectedIndex >= 0)
            {
                CodAnt.Text = "";
                CodAntName.Text = "";
                CodNue.Text = "";
                CodNueName.Text = "";

                string tag = "";

                bool isCuenta = false;
                foreach (ToggleButton item in GridTogle.Children)
                {
                    if (item.IsChecked == true)
                        tag = item.Tag.ToString().Trim();

                    if (item.IsChecked == true && (item.Name == "Togle1" || item.Name == "Togle5") && TipoCBX.SelectedIndex == 1) isCuenta = true;
                }

                BTNviewSaldosNormales.IsEnabled = isCuenta;
                BTNviewSaldosReclasificados.IsEnabled = isCuenta;

                if (!string.IsNullOrEmpty(tag)) cargarTemporal(tag);
            }
        }

        private DataSet Saldos(string cod_ant, string cod_nue, string passa, string column)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpReclasificacionSaldos", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@cod_ant", cod_ant);
                cmd.Parameters.AddWithValue("@cod_nue", cod_nue);
                cmd.Parameters.AddWithValue("@modulo", tipo);
                cmd.Parameters.AddWithValue("@pass", passa);
                cmd.Parameters.AddWithValue("@columna", column);
                cmd.Parameters.AddWithValue("@codemp", "010");
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

        private void BTNviewSaldosNormales_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region validacion

                if (TipoCBX.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione el tipo de reclasificacion", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (TipoCBX.SelectedIndex == 0)
                {
                    if (string.IsNullOrWhiteSpace(CodAnt.Text))
                    {
                        MessageBox.Show("llene el campo codigo anterior", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(CodNue.Text))
                    {
                        MessageBox.Show("llene el campo codigo nuevo", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }

                    if (existencia() == true)
                    {
                        (sender as TextBox).Text = "";
                        MessageBox.Show("El codigo nuevo que ingreso existe", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                }

                if (TipoCBX.SelectedIndex == 1)
                {
                    if (string.IsNullOrWhiteSpace(CodAnt.Text))
                    {
                        MessageBox.Show("llene el campo codigo anterior", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(CodNue.Text))
                    {
                        MessageBox.Show("llene el campo codigo nuevo", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }

                    if (existencia() == false)
                    {
                        MessageBox.Show("el codigo nuevo ingresado no existe", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                }

                if (CodAnt.Text == "" || string.IsNullOrEmpty(CodAnt.Text))
                {
                    MessageBox.Show("el codigo anterior esta vacio", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (CodNue.Text == "" || string.IsNullOrEmpty(CodNue.Text))
                {
                    MessageBox.Show("el codigo nuevo esta vacio", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                #endregion

                string tag = BTNreclasificar.Tag.ToString().Trim();
                string column = tag == "Referencia" ? "cod_ref" : "cod_bod";
                string query = "select * From " + saldos + " where " + column + "= '" + CodAnt.Text + "' or " + column + " = '" + CodNue.Text + "'; ";
                DataTable dt = SiaWin.Func.SqlDT(query, "tabla", idemp);
                if (dt.Rows.Count > 0)
                {
                    SiaWin.Browse(dt);
                }
                else
                {
                    MessageBox.Show("Sin Saldos Iniciales", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar:" + w);
            }
        }

        private async void BTNviewSaldosReclasificados_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                #region validacion

                if (TipoCBX.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione el tipo de reclasificacion", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (string.IsNullOrEmpty(CodAnt.Text))
                {
                    MessageBox.Show("el codigo anterior esta vacio", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (string.IsNullOrEmpty(CodNue.Text))
                {
                    MessageBox.Show("el codigo nuevo esta vacio", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                #endregion

                CancellationTokenSource source = new CancellationTokenSource();
                Card.IsEnabled = false;
                GridMain.IsEnabled = false;
                TipoCBX.IsEnabled = false;
                sfBusyIndicator.IsBusy = true;

                string tag = BTNreclasificar.Tag.ToString().Trim();
                string cod_ant = CodAnt.Text.Trim();
                string cod_nue = CodNue.Text.Trim();
                string column = tag == "Referencia" ? "cod_ref" : "cod_bod";
                var slowTask = Task<DataSet>.Factory.StartNew(() => Saldos(cod_ant, cod_nue, "0", column));
                await slowTask;

                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {

                    SiaWin.Browse(((DataSet)slowTask.Result).Tables[0]);
                }


                Card.IsEnabled = true;
                GridMain.IsEnabled = true;
                TipoCBX.IsEnabled = true;
                sfBusyIndicator.IsBusy = false;

            }
            catch (Exception w)
            {

                MessageBox.Show("error al cargar saldos:" + w);
            }
        }

        private void BTNview_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dsTemporal.Tables[0].Rows.Count > 0)
                {
                    SiaWin.Browse(dsTemporal.Tables[0]);
                }
                else
                {
                    MessageBox.Show("no tiene ninguna tabla para afectar", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("erro al abrir tablas:" + w);
            }
        }





        private void Cod_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.F8)
                {

                    string tag = BTNreclasificar.Tag.ToString();
                    string tabla = ""; string codigo = ""; string nombre = "";
                    switch (tag)
                    {
                        case "Referencia":
                            tabla = "inmae_ref"; codigo = "cod_ref"; nombre = "nom_ref";
                            break;
                        case "Linea":
                            tabla = "inmae_tip"; codigo = "cod_tip"; nombre = "nom_tip";
                            break;
                        case "Grupo":
                            tabla = "InMae_gru"; codigo = "cod_gru"; nombre = "nom_gru";
                            break;
                        case "SubGrupo":
                            tabla = "InMae_sgr"; codigo = "Cod_sgr"; nombre = "nom_sgr";
                            break;
                        case "Bodega":
                            tabla = "InMae_bod"; codigo = "cod_bod"; nombre = "nom_bod";
                            break;
                    }

                    int idr = 0; string code = ""; string nom = "";
                    dynamic winb = SiaWin.WindowBuscar(tabla, codigo, nombre, codigo, "idrow", tag, SiaWin.Func.DatosEmp(idemp), false, "", idEmp: idemp);
                    winb.ShowInTaskbar = false;
                    winb.Owner = Application.Current.MainWindow;
                    winb.Width = 400;
                    winb.Height = 400;
                    winb.ShowDialog();
                    idr = winb.IdRowReturn;
                    code = winb.Codigo;
                    nom = winb.Nombre;

                    if (idr > 0)
                    {
                        (sender as TextBox).Text = code.Trim();
                        CodAntName.Text = nom;
                    }
                    else
                    {
                        (sender as TextBox).Text = "";
                        CodAntName.Text = "";
                    }



                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error key:" + w);
            }
        }





    }
}
