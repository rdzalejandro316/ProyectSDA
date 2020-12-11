using MaestraDiferidos.Modelo;
using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.UI.Xaml.ScrollAxis;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9646,"MaestraDiferidos");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9646,"MaestraDiferidos");
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //ww.ShowDialog();

    public partial class MaestraDiferidos : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        Diferidos Mdif = new Diferidos();
        Diferidos _Mdif = new Diferidos();

        public string cod_dif = "";
        string tabla = "comae_dif";
        string tabla_relacion = "Corel_dif";
        string codigo = "cod_dif";
        string nombre = "nom_dif";
        string idrow = "idrow";
        string titleTabla = "Maestra de armotizacion de diferidos";

        //columans q se debe ignoran al recorrer la clase
        List<string> col_ignorar = new List<string>() { "Error", "Item", "Relacion", "relacion" };
        //columnas que tiene un valor predeterminado por ejemplo la fecha de actualizacion
        Dictionary<string, string> col_valor = new Dictionary<string, string>();

        public MaestraDiferidos()
        {
            InitializeComponent();
            pantalla();


        }

        void pantalla()
        {
            this.MinWidth = 1000;
            this.MinHeight = 550;
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
                this.Title = "Maestra de armotizacion diferidos";
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            SiaWin = System.Windows.Application.Current.MainWindow;
            if (idemp <= 0) idemp = SiaWin._BusinessId;
            LoadConfig();

            this.DataContext = Mdif;

            if (string.IsNullOrEmpty(cod_dif)) return;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                switch (e.Key)
                {
                    case Key.F1:
                        BtnBuscar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        break;
                    case Key.F2:
                        BtnNuevo.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        break;
                    case Key.F3:
                        BtnEditar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        break;
                    case Key.F4:
                        BtnEliminar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        break;
                    case Key.F5:
                        BtnSave.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        break;
                    case Key.F6:
                        BtnCancel.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        break;
                    case Key.Escape:
                        BtnCancel.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        break;
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al tomar atajo:" + w);
            }
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dynamic winb = SiaWin.WindowBuscar(tabla, codigo, nombre, codigo, idrow, titleTabla, cnEmp, true, "", idEmp: idemp);
                winb.ShowInTaskbar = false;
                winb.Owner = Application.Current.MainWindow;
                winb.Height = 400;
                winb.ShowDialog();
                int id = winb.IdRowReturn;
                string code = winb.Codigo;
                string nom = winb.Nombre;
                if (id > 0)
                {
                    ActualizaCampos(id, string.Empty);
                    GetRelation(code);
                    bloquear(false);
                    editdel(true);
                }
                if (string.IsNullOrEmpty(code)) e.Handled = false;
                e.Handled = true;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al buscar:" + w);
            }
        }

        void GetRelation(string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code)) return;

                string query = "SELECT * FROM " + tabla_relacion + " where " + codigo + "='" + code + "';";

                SqlDataReader dr = SiaWin.Func.SqlDR(query, idemp);
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Mdif.Relacion.Add(new Relacion()
                        {
                            cod_dif = dr["cod_dif"].ToString(),
                            fec_adq = Convert.ToDateTime(dr["fec_adq"]),
                            cos_his = Convert.ToDecimal(dr["cos_his"]),
                            fec_ini = Convert.ToDateTime(dr["fec_ini"]),
                            fec_fin = Convert.ToDateTime(dr["fec_fin"]),
                            cod_cco = dr["cod_cco"].ToString(),
                            valor = Convert.ToDecimal(dr["valor"]),
                            cuotas = Convert.ToDecimal(dr["cuotas"]),
                            estado = Convert.ToBoolean(dr["estado"]),
                            poliza = dr["poliza"].ToString(),
                        });
                    }

                }


            }
            catch (Exception w)
            {
                MessageBox.Show("error al encontrar la relacion:" + w);
            }
        }

        void ActualizaCampos(int Id, string _Sql)
        {
            try
            {
                SqlDataReader dr;

                dr = _Sql == string.Empty ?
                    SiaWin.Func.SqlDR("SELECT * FROM " + tabla + " where " + idrow + "=" + Id.ToString(), idemp) :
                    dr = SiaWin.Func.SqlDR(_Sql, idemp);

                dr.Read();



                foreach (var item in Mdif.GetType().GetProperties())
                {
                    if (col_ignorar.Contains(item.Name)) continue;


                    Type tipo = item.PropertyType;

                    Type examType = typeof(Diferidos);
                    PropertyInfo piInstance = examType.GetProperty(item.Name);

                    switch (Type.GetTypeCode(tipo))
                    {
                        case TypeCode.String:
                            piInstance.SetValue(Mdif, dr[item.Name] == DBNull.Value ? "" : dr[item.Name].ToString().Trim());
                            break;
                        case TypeCode.Int16:
                            piInstance.SetValue(Mdif, dr[item.Name] == DBNull.Value ? 0 : Convert.ToInt16(dr[item.Name].ToString().Trim()));
                            break;
                        case TypeCode.Int32:
                            piInstance.SetValue(Mdif, dr[item.Name] == DBNull.Value ? 0 : Convert.ToInt32(dr[item.Name].ToString().Trim()));
                            break;
                        case TypeCode.Int64:
                            piInstance.SetValue(Mdif, dr[item.Name] == DBNull.Value ? 0 : Convert.ToInt64(dr[item.Name].ToString().Trim()));
                            break;
                        case TypeCode.Decimal:
                            piInstance.SetValue(Mdif, dr[item.Name] == DBNull.Value ? 0 : Convert.ToDecimal(dr[item.Name].ToString().Trim()));
                            break;
                        case TypeCode.Double:
                            piInstance.SetValue(Mdif, dr[item.Name] == DBNull.Value ? 0 : Convert.ToDouble(dr[item.Name].ToString().Trim()));
                            break;
                        case TypeCode.Boolean:
                            piInstance.SetValue(Mdif, dr[item.Name] == DBNull.Value ? false : Convert.ToBoolean(dr[item.Name].ToString().Trim()));
                            break;
                    }
                }


                dr.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("erro sql ActualizaCampos():" + ex.Message);
            }
            catch (System.Exception _error)
            {
                MessageBox.Show("error exception ActualizaCampos():" + _error.Message);
            }
        }

        public void activecontrol(bool control, string boton)
        {
            CtrlA.Visibility = control ? Visibility.Visible : Visibility.Hidden;
            CtrlB.Visibility = !control ? Visibility.Visible : Visibility.Hidden;
            BtnSave.Content = boton;
        }

        public void editdel(bool control)
        {
            BtnEditar.IsEnabled = control;
            BtnEliminar.IsEnabled = control;
        }

        public void bloquear(bool flag)
        {
            PanelA.IsEnabled = flag;
            PanelB.IsEnabled = flag;
        }

        void ClearClas()
        {

            try
            {
                Mdif.Relacion.Clear();
                foreach (var item in Mdif.GetType().GetProperties())
                {
                    Type examType = typeof(Diferidos);
                    PropertyInfo piInstance = examType.GetProperty(item.Name);
                    Type tipo = item.PropertyType;

                    if (col_ignorar.Contains(item.Name)) continue;

                    switch (Type.GetTypeCode(tipo))
                    {
                        case TypeCode.String:
                            piInstance.SetValue(Mdif, string.Empty);
                            break;
                        case TypeCode.Decimal:
                            piInstance.SetValue(Mdif, 0);
                            break;
                        case TypeCode.Int32:
                            piInstance.SetValue(Mdif, -1);
                            break;
                        case TypeCode.Double:
                            piInstance.SetValue(Mdif, 0);
                            break;
                        case TypeCode.Boolean:
                            piInstance.SetValue(Mdif, false);
                            break;
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error ClearClas():" + w);
            }
        }

        void ClearClasOld()
        {
            try
            {
                foreach (var item in Mdif.GetType().GetProperties())
                {
                    Type examType = typeof(Diferidos);
                    PropertyInfo piInstance = examType.GetProperty(item.Name);
                    Type tipo = item.PropertyType;

                    if (col_ignorar.Contains(item.Name)) continue;

                    switch (Type.GetTypeCode(tipo))
                    {
                        case TypeCode.String:
                            piInstance.SetValue(_Mdif, string.Empty);
                            break;
                        case TypeCode.Int32:
                            piInstance.SetValue(_Mdif, -1);
                            break;
                        case TypeCode.Decimal:
                            piInstance.SetValue(_Mdif, 0);
                            break;
                        case TypeCode.Double:
                            piInstance.SetValue(_Mdif, 0);
                            break;
                        case TypeCode.Boolean:
                            piInstance.SetValue(_Mdif, false);
                            break;
                    }
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error ClearClas():" + w);
            }
        }

        void Clone()
        {

            try
            {

                foreach (var item in Mdif.GetType().GetProperties())
                {
                    if (col_ignorar.Contains(item.Name)) continue;
                    Type examType = typeof(Diferidos);
                    PropertyInfo piInstance = examType.GetProperty(item.Name);
                    var propertyInfo = typeof(Diferidos).GetProperties().Where(p => p.Name == item.Name).Single();
                    var valueA = propertyInfo.GetValue(Mdif, null);
                    piInstance.SetValue(_Mdif, valueA);
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error Clone():" + w);
            }
        }

        private void txcod_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty((sender as TextBox).Text)) return;

                string query = "select * from " + tabla + " where " + codigo + "= '" + (sender as TextBox).Text + "'";
                DataTable dt = SiaWin.Func.SqlDT(query, "temp", idemp);
                if (dt.Rows.Count > 0)
                {
                    if (SiaWin.Func.Acceso(SiaWin._UserGroup, SiaWin._ProyectId, 6, idemp, 1, "lEdit") == true)
                    {
                        int id = (int)dt.Rows[0][idrow];
                        string cod = dt.Rows[0][codigo].ToString();

                        ActualizaCampos(id, string.Empty);

                        GetRelation(cod);

                        activecontrol(false, "Modificar");
                        Clone();
                    }
                    else
                    {
                        MessageBox.Show("este usuario no tiene permisos para editar", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        (sender as TextBox).Text = "";
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error el el foco:" + w);
            }
        }

        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            ClearClas();
            activecontrol(false, "Guardar");
            bloquear(true);
            txcod.Focus();
        }
        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            bloquear(true);
            activecontrol(false, "Modificar");
            Clone();
        }
        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Usted desea eliminar el registro....?", "Confirmacion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    if (SiaWin.Func.DeleteMaestra(tabla, codigo, idrow, Mdif.cod_dif, idemp)) ClearClas();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (BtnSave.Content.ToString() == "Modificar")
                {
                    if (!Mdif.IsValid())
                    {
                        MessageBox.Show("no se puede modificar por que faltan algunos campos que son requeridos", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }

                    if (!ComparaDatos()) return;


                    int query = Modificar();
                    if (query > 0)
                    {
                        RelacionDiferidos(Mdif.cod_dif);
                        SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idemp, 0, 0, 0, "actulizo exitosamente el Diferidos" + Mdif.cod_dif, "");
                        MessageBox.Show("actualizo exitosamente la informacion del Diferidos:" + Mdif.cod_dif, "alerta", MessageBoxButton.OK, MessageBoxImage.Information);
                        ClearClas();
                        editdel(true);
                        bloquear(false);
                        activecontrol(true, "");
                    }
                }
                else
                {
                    if (!Mdif.IsValid())
                    {
                        MessageBox.Show("no se puede guardar por que faltan algunos campos que son requeridos", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }


                    int query = Insertar();
                    if (query > 0)
                    {
                        RelacionDiferidos(Mdif.cod_dif);
                        SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idemp, 0, 0, 0, "Inserto exitosamente el Diferidos" + Mdif.cod_dif, "");
                        MessageBox.Show("inserto exitosamente el Diferidos:" + Mdif.cod_dif, "alerta", MessageBoxButton.OK, MessageBoxImage.Information);
                        ClearClas();
                        editdel(false);
                        bloquear(false);
                        activecontrol(true, "");
                    }

                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al realizar el query:" + w);
            }
        }


        int Insertar()
        {
            try
            {
                int valor = 0;
                using (SqlConnection connection = new SqlConnection(SiaWin.Func.DatosEmp(idemp)))
                {
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        try
                        {
                            Dictionary<string, Type> campos = new Dictionary<string, Type>();

                            foreach (var item in Mdif.GetType().GetProperties())
                            {
                                if (!col_ignorar.Contains(item.Name))
                                {
                                    if (idrow == item.Name) continue;
                                    campos.Add("@" + item.Name, item.PropertyType);
                                }
                            }

                            string cab_colm = String.Join(", ", campos.Keys.ToArray()).Replace("@", "");
                            string cab_colm_parm = String.Join(", ", campos.Keys.ToArray());

                            cmd.CommandText = "INSERT INTO " + tabla + " (" + cab_colm + ")  VALUES (" + cab_colm_parm + ")";

                            foreach (var item in campos)
                            {
                                var key = item.Key.Replace("@", "");

                                object val = new object();
                                Type tipo = item.Value;
                                SqlDbType sqlDb = new SqlDbType();

                                var propertyInfo = typeof(Diferidos).GetProperties().Where(p => p.Name == key).Single();
                                var valueA = propertyInfo.GetValue(Mdif, null);
                                if (!col_ignorar.Contains(key))
                                {
                                    if (idrow == key) continue;
                                    switch (Type.GetTypeCode(tipo))
                                    {
                                        case TypeCode.String:
                                            val = val == DBNull.Value ? "" : valueA;
                                            sqlDb = SqlDbType.VarChar;
                                            break;
                                        case TypeCode.Decimal:
                                            val = valueA == DBNull.Value ? 0 : Convert.ToDecimal(valueA);
                                            sqlDb = SqlDbType.Decimal;
                                            break;
                                        case TypeCode.Double:
                                            val = valueA == DBNull.Value ? 0 : Convert.ToDouble(valueA);
                                            sqlDb = SqlDbType.Float;
                                            break;
                                        case TypeCode.Int32:
                                            val = valueA == DBNull.Value ? 0 : Convert.ToInt32(valueA);
                                            sqlDb = SqlDbType.Int;
                                            break;
                                        case TypeCode.Int16:
                                            val = valueA == DBNull.Value ? 0 : Convert.ToInt16(valueA);
                                            sqlDb = SqlDbType.Int;
                                            break;
                                        case TypeCode.Int64:
                                            val = valueA == DBNull.Value ? 0 : Convert.ToInt64(valueA);
                                            sqlDb = SqlDbType.Int;
                                            break;
                                        case TypeCode.Boolean:
                                            val = valueA == DBNull.Value ? 0 : Convert.ToInt32(valueA);
                                            sqlDb = SqlDbType.Int;
                                            break;
                                        case TypeCode.DateTime:
                                            val = valueA == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(valueA);
                                            sqlDb = SqlDbType.Date;
                                            break;
                                    }
                                }

                                if (col_valor.ContainsKey(key))
                                {
                                    cmd.Parameters.AddWithValue(item.Key, col_valor[key]);
                                }
                                else
                                {
                                    SqlParameter param = new SqlParameter();
                                    param.ParameterName = item.Key;
                                    param.Value = val;
                                    param.SqlDbType = sqlDb;
                                    cmd.Parameters.Add(param);
                                }
                            }

                            connection.Open();
                            valor = cmd.ExecuteNonQuery();

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Error Interno Sia", MessageBoxButton.OK, MessageBoxImage.Stop);

                        }
                    }
                }
                return valor;
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Error Interno Sia", MessageBoxButton.OK, MessageBoxImage.Stop);
                return 0;
            }
        }


        int Modificar()
        {
            try
            {
                int valor = 0;
                using (SqlConnection connection = new SqlConnection(SiaWin.Func.DatosEmp(idemp)))
                {
                    using (SqlCommand cmd = connection.CreateCommand())
                    {

                        string query = " ";
                        Dictionary<string, Type> campos = new Dictionary<string, Type>();

                        foreach (var item in Mdif.GetType().GetProperties())
                        {
                            if (!col_ignorar.Contains(item.Name))
                            {
                                if (idrow == item.Name) continue;
                                query += item.Name + "=@" + item.Name + ",";
                                campos.Add(item.Name, item.PropertyType);
                            }
                        }

                        query = query.Remove(query.Length - 1);

                        cmd.CommandText = "UPDATE " + tabla + " SET " + query + " WHERE " + idrow + "=" + Mdif.idrow.ToString();

                        foreach (var item in campos)
                        {
                            object val = new object();
                            Type tipo = item.Value;
                            SqlDbType sqlDb = new SqlDbType();


                            var propertyInfo = typeof(Diferidos).GetProperties().Where(p => p.Name == item.Key.ToString()).Single();
                            var valueA = propertyInfo.GetValue(Mdif, null);

                            if (!col_ignorar.Contains(item.Key))
                            {
                                if (idrow == item.Key) continue;
                                switch (Type.GetTypeCode(tipo))
                                {
                                    case TypeCode.String:
                                        val = val == DBNull.Value ? "" : valueA;
                                        sqlDb = SqlDbType.VarChar;
                                        break;
                                    case TypeCode.Decimal:
                                        val = valueA == DBNull.Value ? 0 : Convert.ToDecimal(valueA);
                                        sqlDb = SqlDbType.Decimal;
                                        break;
                                    case TypeCode.Double:
                                        val = valueA == DBNull.Value ? 0 : Convert.ToDouble(valueA);
                                        sqlDb = SqlDbType.Float;
                                        break;
                                    case TypeCode.Int32:
                                        val = valueA == DBNull.Value ? 0 : Convert.ToInt32(valueA);
                                        sqlDb = SqlDbType.Int;
                                        break;
                                    case TypeCode.Int16:
                                        val = valueA == DBNull.Value ? 0 : Convert.ToInt16(valueA);
                                        sqlDb = SqlDbType.Int;
                                        break;
                                    case TypeCode.Int64:
                                        val = valueA == DBNull.Value ? 0 : Convert.ToInt64(valueA);
                                        sqlDb = SqlDbType.Int;
                                        break;
                                    case TypeCode.Boolean:
                                        val = valueA == DBNull.Value ? 0 : Convert.ToInt32(valueA);
                                        sqlDb = SqlDbType.Int;
                                        break;
                                    case TypeCode.DateTime:
                                        val = valueA == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(valueA);
                                        sqlDb = SqlDbType.Date;
                                        break;
                                }


                                if (col_valor.ContainsKey(item.Key))
                                {
                                    cmd.Parameters.AddWithValue("@" + item.Key, col_valor[item.Key]);
                                }
                                else
                                {
                                    SqlParameter param = new SqlParameter();
                                    param.ParameterName = "@" + item.Key;
                                    param.Value = val;
                                    param.SqlDbType = sqlDb;
                                    cmd.Parameters.Add(param);
                                }

                            }
                        }


                        connection.Open();
                        valor = cmd.ExecuteNonQuery();
                    }
                }
                return valor;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Interno Sia", MessageBoxButton.OK, MessageBoxImage.Stop);
                return 0;
            }

        }


        void RelacionDiferidos(string cod_dif)
        {
            try
            {
                string delete = "delete Corel_dif where cod_dif='" + cod_dif + "';";
                if (SiaWin.Func.SqlCRUD(delete, idemp) == true)
                {

                    //linea para poder terminar la edicion de una fila en la grilla
                    if (this.GridRelDif.View.IsAddingNew)
                    {
                        RowColumnIndex rowColumnIndex = new RowColumnIndex();

                        if (this.GridRelDif.SelectionController.CurrentCellManager.CurrentCell.IsEditing)
                            this.GridRelDif.SelectionController.CurrentCellManager.EndEdit(true);

                        rowColumnIndex = this.GridRelDif.SelectionController.CurrentCellManager.CurrentRowColumnIndex;

                        var addNewRowController = this.GridRelDif.GetAddNewRowController();
                        addNewRowController.CommitAddNew();
                    }

                    string insert = "";
                    foreach (Relacion item in Mdif.Relacion)
                    {
                        string valor = item.valor.ToString("F", CultureInfo.InvariantCulture);
                        string cuotas = item.cuotas.ToString("F", CultureInfo.InvariantCulture);
                        string cos_his = item.cos_his.ToString("F", CultureInfo.InvariantCulture);
                        int estado = Convert.ToInt32(item.estado);
                        string cod_cco = item.cod_cco.Trim();
                        string poliza = item.poliza.Trim();
                        DateTime fec_ini = Convert.ToDateTime(item.fec_ini);
                        DateTime fec_fin = Convert.ToDateTime(item.fec_fin);
                        DateTime fec_adq = Convert.ToDateTime(item.fec_adq);

                        insert += "insert into Corel_dif (cod_dif,cod_cco,valor,cuotas,estado,fec_ini,fec_fin,cos_his,fec_adq,poliza) values ";
                        insert += "('" + cod_dif + "','" + cod_cco + "'," + valor + "," + cuotas + "," + estado + ",'" + fec_ini + "','" + fec_fin + "',";
                        insert += cos_his + ",'" + fec_adq + "','" + poliza + "'); ";
                    }

                    if (!string.IsNullOrEmpty(insert))
                    {
                        if (SiaWin.Func.SqlCRUD(insert, idemp) == true)
                        {
                            MessageBox.Show("se actualizo la relacion de diferidos exitosamente", "alerta", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }

                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al actualizar los di:" + w);
            }
        }

        bool ComparaDatos()
        {
            StringBuilder sbRed = new StringBuilder();
            StringBuilder sbLocal = new StringBuilder();
            Diferidos __Mdif = new Diferidos();
            try
            {
                SqlDataReader dr;
                dr = SiaWin.Func.SqlDR("SELECT * FROM " + tabla + "  where idrow=" + Mdif.idrow.ToString(), idemp);

                dr.Read();
                foreach (var item in Mdif.GetType().GetProperties())
                {
                    if (col_ignorar.Contains(item.Name)) continue;
                    if (col_valor.ContainsKey(item.Name)) continue;

                    //MessageBox.Show("item.Name:" + item.Name);

                    Type tipo = item.PropertyType;
                    Type examType = typeof(Diferidos);
                    PropertyInfo piInstance = examType.GetProperty(item.Name);
                    switch (Type.GetTypeCode(tipo))
                    {
                        case TypeCode.String:
                            piInstance.SetValue(__Mdif, dr[item.Name] == DBNull.Value ? "" : dr[item.Name].ToString().Trim());
                            break;
                        case TypeCode.Int16:
                            piInstance.SetValue(__Mdif, dr[item.Name] == DBNull.Value ? 0 : Convert.ToInt16(dr[item.Name].ToString().Trim()));
                            break;
                        case TypeCode.Int32:
                            piInstance.SetValue(__Mdif, dr[item.Name] == DBNull.Value ? 0 : Convert.ToInt32(dr[item.Name].ToString().Trim()));
                            break;
                        case TypeCode.Int64:
                            piInstance.SetValue(__Mdif, dr[item.Name] == DBNull.Value ? 0 : Convert.ToInt64(dr[item.Name].ToString().Trim()));
                            break;
                        case TypeCode.Decimal:
                            piInstance.SetValue(__Mdif, dr[item.Name] == DBNull.Value ? 0 : Convert.ToDecimal(dr[item.Name].ToString().Trim()));
                            break;
                        case TypeCode.Double:
                            piInstance.SetValue(__Mdif, dr[item.Name] == DBNull.Value ? 0 : Convert.ToDouble(dr[item.Name].ToString().Trim()));
                            break;
                        case TypeCode.Boolean:
                            piInstance.SetValue(__Mdif, dr[item.Name] == DBNull.Value ? false : Convert.ToBoolean(dr[item.Name].ToString().Trim()));
                            break;
                    }
                }

                dr.Close();

                //// recorre campos de la clase
                Mdif.GetType().GetProperties().ToList().ForEach(f =>
                {
                    try
                    {
                        //compara si cambiaron los campos del registro en le servidor
                        var propertyInfo = typeof(Diferidos).GetProperties().Where(p => p.Name == f.Name).Single();
                        var valueA = propertyInfo.GetValue(_Mdif, null); //ORIGINAL EN MEMORIA
                        var valueB = propertyInfo.GetValue(Mdif, null);  //ACTUAL ///
                        var valueC = propertyInfo.GetValue(__Mdif, null); //REAL SQL DATA

                        //if (col_ignorar.Contains(propertyInfo.Name)) return;
                        if (!col_valor.ContainsKey(propertyInfo.Name))
                        {

                            if (!valueA.Equals(valueC)) sbRed.Append("Tabla:" + tabla + Environment.NewLine + "Cambio Campo " + f.Name + Environment.NewLine + "Anterior :" + valueA + Environment.NewLine + "Nuevo: " + valueC);

                            if (!valueA.Equals(valueB)) sbLocal.Append("Tabla:" + tabla + " : Cambio Campo " + f.Name + Environment.NewLine + " Anterior :" + valueA + " - Nuevo: " + valueB + Environment.NewLine);
                        };


                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });

                //registra si ya alguien modifico este Diferidos
                if (!string.IsNullOrWhiteSpace(sbRed.ToString()))
                {
                    MessageBoxResult result = MessageBox.Show("Otro usuario ha cambiado ya este registro los cambios fueron " + sbRed.ToString() + ", Usted desea guardar sus cambios?", "Confirmacion", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result != MessageBoxResult.Yes) return false;

                }
                // registra en auditoria los cambio hechos por el usuario                
                if (!string.IsNullOrWhiteSpace(sbLocal.ToString()))
                {
                    SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idemp, 0, -1, 0, sbLocal.ToString(), "");
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Error al Actualizar datos", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }
            catch (System.Exception _error)
            {
                MessageBox.Show(_error.Message);
                return false;
            }
            return true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            activecontrol(true, "Guardar");
            editdel(false);
            bloquear(false);
            ClearClas();
            ClearClasOld();
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Mdif.cod_dif))
                {
                    MessageBox.Show("el campo del tercero esta vacio", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                DataTable dt = SiaWin.Func.SqlDT("select * from " + tabla + " where " + codigo + "='" + Mdif.cod_dif + "'", "temp", idemp);
                if (dt.Rows.Count > 0)
                {

                    using (ExcelEngine excelEngine = new ExcelEngine())
                    {
                        IApplication application = excelEngine.Excel;
                        IWorkbook workbook = application.Workbooks.Create(1);
                        IWorksheet sheet = workbook.Worksheets[0];
                        DataTable dataTable = dt;
                        sheet.ImportDataTable(dataTable, true, 1, 1, true);
                        sheet.UsedRange.AutofitColumns();

                        SaveFileDialog sfd = new SaveFileDialog
                        {
                            FilterIndex = 2,
                            Filter = "Excel 97 to 2003 Files(*.xls)|*.xls|Excel 2007 to 2010 Files(*.xlsx)|*.xlsx|Excel 2013 File(*.xlsx)|*.xlsx"
                        };
                        if (sfd.ShowDialog() == true)
                        {
                            using (Stream stream = sfd.OpenFile())
                            {
                                if (sfd.FilterIndex == 1)
                                    workbook.Version = ExcelVersion.Excel97to2003;
                                else if (sfd.FilterIndex == 2)
                                    workbook.Version = ExcelVersion.Excel2010;
                                else
                                    workbook.Version = ExcelVersion.Excel2013;

                                workbook.SaveAs(stream);
                            }
                            if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archivo", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                            {
                                System.Diagnostics.Process.Start(sfd.FileName);
                            }
                        }
                        else
                        {
                            MessageBox.Show("el tercero no existe", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            return;
                        }
                    }




                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al exportar:" + w);
            }
        }

        private void GridRelDif_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.F3)
                {
                    if (MessageBox.Show("Borrar Registro seleccionado...?", "Siasoft", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {

                        Relacion row = (Relacion)GridRelDif.SelectedItems[0];
                        Mdif.Relacion.Remove(row);
                        GridRelDif.UpdateLayout();
                    }
                }

                if (e.Key == Key.F8)
                {

                    dynamic winb = SiaWin.WindowBuscar("comae_cco", "cod_cco", "nom_cco", "cod_cco", "idrow", "maestra de centro de costos", cnEmp, true, "", idEmp: idemp);
                    winb.ShowInTaskbar = false;
                    winb.Owner = Application.Current.MainWindow;
                    winb.Height = 400;
                    winb.ShowDialog();
                    int id = winb.IdRowReturn;
                    string code = winb.Codigo;
                    string nom = winb.Nombre;
                    if (id > 0)
                    {
                        bool newcol = this.GridRelDif.View.IsAddingNew;

                        if (this.GridRelDif.View.IsAddingNew)
                        {
                            var reflector = this.GridRelDif.View.GetPropertyAccessProvider();
                            int columnIndex = (sender as SfDataGrid).SelectionController.CurrentCellManager.CurrentRowColumnIndex.RowIndex;
                            var rowData = GridRelDif.GetRecordAtRowIndex(columnIndex);

                            reflector.SetValue(rowData, "cod_cco", code);
                            GridRelDif.UpdateDataRow(columnIndex);
                            GridRelDif.UpdateLayout();
                            GridRelDif.Columns["cod_cco"].AllowEditing = true;
                        }
                        else
                        {
                            var data = ((SfDataGrid)sender).SelectedItem as Relacion;
                            data.cod_cco = code;
                        }
                        
                    }
                    if (string.IsNullOrEmpty(code)) e.Handled = false;
                    e.Handled = true;
                }


            }
            catch (Exception w)
            {
                MessageBox.Show("error al eliminar registro:" + w);
            }
        }

        private void GridRelDif_AddNewRowInitiating(object sender, AddNewRowInitiatingEventArgs e)
        {
            var data = e.NewObject as Relacion;
            data.cod_dif = Mdif.cod_dif;
            data.fec_ini = DateTime.Now;
            data.fec_fin = DateTime.Now;
            data.fec_adq = DateTime.Now;
            data.valor = 0;
            data.estado = true;

        }


    }
}
