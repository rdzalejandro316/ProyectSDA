using MaestraTerceros;
using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Parser.Biff_Records;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Contexts;
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
    //Sia.PublicarPnt(9666,"MaestraTerceros");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9666, "MaestraTerceros");
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();   
        

public partial class MaestraTerceros : Window
{
    dynamic SiaWin;
    public int idemp = 0;
    string cnEmp = "";
    string cod_empresa = "";
    Tercero MTer = new Tercero();
    Tercero _MTer = new Tercero();

    public string cod_ter = "";
    string tabla = "comae_ter";
    string codigo = "cod_ter";
    string nombre = "nom_ter";
    string idrow = "idrow";
    //columans q se debe ignoran al recorrer la clase
    List<string> col_ignorar = new List<string>() { "Error", "Item", "tdocm" };
    //columnas que tiene un valor predeterminado por ejemplo la fecha de actualizacion
    Dictionary<string, string> col_valor = new Dictionary<string, string>();

    public MaestraTerceros()
    {
        InitializeComponent();
        pantalla();
        loadcolumns();
    }

    public void loadcolumns()
    {
        try
        {
            col_valor.Clear();
            col_valor.Add("fec_act", DateTime.Now.ToString());
        }
        catch (Exception w)
        {
            MessageBox.Show("error al agrgar columnas:" + 2);
        }
    }

    void pantalla()
    {
        this.MinWidth = 1100;
        this.MinHeight = 500;
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
            this.Title = "Maestra de terceros " + cod_empresa + "-" + nomempresa;

            //llena combos                                
            MTer.tdocm = LlenaCombo("select cod_tdo,rtrim(cod_tdo)+'('+rtrim(nom_tdo)+')' as nom_tdo from InMae_tdoc  order by cod_tdo");

            //seguridad
            ////ParamAcc 1=lRun,2=lNew,3=lEdit,4=lDelete,5=lSearch,5=Renum,6=lPrint,7=lExport,8=lOpc1,9=lOpc2,10=lOpc3
            string pk = idemp.ToString() + "-2";

            BtnBuscar.Visibility = SiaWin.Func.Acceso(SiaWin._UserGroup, SiaWin._ProyectId, 6, idemp, 1, "lSearch") == true ?
                Visibility.Visible : Visibility.Collapsed;

            BtnNuevo.Visibility = SiaWin.Func.Acceso(SiaWin._UserGroup, SiaWin._ProyectId, 6, idemp, 1, "lNew") == true ?
                 Visibility.Visible : Visibility.Collapsed;

            BtnEditar.Visibility = SiaWin.Func.Acceso(SiaWin._UserGroup, SiaWin._ProyectId, 6, idemp, 1, "lEdit") == true ?
                Visibility.Visible : Visibility.Collapsed;

            BtnEliminar.Visibility = SiaWin.Func.Acceso(SiaWin._UserGroup, SiaWin._ProyectId, 6, idemp, 1, "lDelete") == true ?
                    Visibility.Visible : Visibility.Collapsed;


            string llave = idemp.ToString() + "-" + 1;
            bool flagGBimp = SiaWin.Acc.ContainsKey(llave + "-221") == true ? true : false;

        }
        catch (Exception e)
        {
            MessageBox.Show("error en el load" + e.Message);
        }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            SiaWin = System.Windows.Application.Current.MainWindow;
            if (idemp <= 0) idemp = SiaWin._BusinessId;



            LoadConfig();
            this.DataContext = MTer;

            if (string.IsNullOrEmpty(cod_ter)) return;

            DataTable dt = SiaWin.Func.SqlDT("select * From comae_ter where cod_ter='" + cod_ter + "'", "tercero", idemp);
            if (dt.Rows.Count > 0)
            {
                int id = (int)dt.Rows[0]["idrow"];
                ActualizaCampos(id, string.Empty);
                bloquear(false);
                editdel(true);
            }
            else
            {
                ClearClas();
                activecontrol(false, "Guardar");
                bloquear(true);
                MTer.cod_ter = cod_ter;
                TXname.Focus();
            }
        }
        catch (Exception w)
        {
            MessageBox.Show("error al cargar:" + w);
        }
    }

    DataTable LlenaCombo(string _Sql)
    {
        DataTable dt = SiaWin.Func.SqlDT(_Sql, "tabla", idemp);
        return dt;
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
            dynamic winb = SiaWin.WindowBuscar(tabla, codigo, nombre, codigo, idrow, "maestra de tereros", cnEmp, false, "", idEmp: idemp);
            winb.ShowInTaskbar = false;
            winb.Owner = Application.Current.MainWindow;
            winb.Height = 400;
            winb.ShowDialog();
            int id = winb.IdRowReturn;
            string code = winb.Codigo;
            string nom = winb.Nombre;
            //winb = null;
            if (id > 0)
            {
                ActualizaCampos(id, string.Empty);
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

    void ActualizaCampos(int Id, string _Sql)
    {
        try
        {
            SqlDataReader dr;

            dr = _Sql == string.Empty ?
                SiaWin.Func.SqlDR("SELECT * FROM " + tabla + " where idrow=" + Id.ToString(), idemp) :
                dr = SiaWin.Func.SqlDR(_Sql, idemp);

            dr.Read();
            foreach (var item in MTer.GetType().GetProperties())
            {

                if (col_ignorar.Contains(item.Name)) continue;

                Type tipo = item.PropertyType;

                Type examType = typeof(Tercero);
                PropertyInfo piInstance = examType.GetProperty(item.Name);
                switch (Type.GetTypeCode(tipo))
                {
                    case TypeCode.String:
                        piInstance.SetValue(MTer, dr[item.Name] == DBNull.Value ? "" : dr[item.Name].ToString().Trim());
                        break;
                    case TypeCode.Int16:
                        piInstance.SetValue(MTer, dr[item.Name] == DBNull.Value ? 0 : Convert.ToInt16(dr[item.Name].ToString().Trim()));
                        break;
                    case TypeCode.Int32:
                        piInstance.SetValue(MTer, dr[item.Name] == DBNull.Value ? 0 : Convert.ToInt32(dr[item.Name].ToString().Trim()));
                        break;
                    case TypeCode.Int64:
                        piInstance.SetValue(MTer, dr[item.Name] == DBNull.Value ? 0 : Convert.ToInt64(dr[item.Name].ToString().Trim()));
                        break;
                    case TypeCode.Decimal:
                        piInstance.SetValue(MTer, dr[item.Name] == DBNull.Value ? 0 : Convert.ToDecimal(dr[item.Name].ToString().Trim()));
                        break;
                    case TypeCode.Double:
                        piInstance.SetValue(MTer, dr[item.Name] == DBNull.Value ? 0 : Convert.ToDouble(dr[item.Name].ToString().Trim()));
                        break;
                    case TypeCode.Boolean:
                        piInstance.SetValue(MTer, dr[item.Name] == DBNull.Value ? false : Convert.ToBoolean(dr[item.Name].ToString().Trim()));
                        break;
                }


            }
            dr.Close();
        }
        catch (SqlException ex)
        {
            MessageBox.Show("erro sql:" + ex.Message);
        }
        catch (System.Exception _error)
        {
            MessageBox.Show("error exception:" + _error.Message);
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
        PanelC.IsEnabled = flag;
    }
    void ClearClas()
    {
        try
        {
            foreach (var item in MTer.GetType().GetProperties())
            {
                Type examType = typeof(Tercero);
                PropertyInfo piInstance = examType.GetProperty(item.Name);
                Type tipo = item.PropertyType;

                if (col_ignorar.Contains(item.Name)) continue;

                switch (Type.GetTypeCode(tipo))
                {
                    case TypeCode.String:
                        piInstance.SetValue(MTer, string.Empty);
                        break;
                    case TypeCode.Int32:
                        piInstance.SetValue(MTer, -1);
                        break;
                    case TypeCode.Decimal:
                        piInstance.SetValue(MTer, 0);
                        break;
                    case TypeCode.Double:
                        piInstance.SetValue(MTer, 0);
                        break;
                    case TypeCode.Boolean:
                        piInstance.SetValue(MTer, false);
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
            foreach (var item in MTer.GetType().GetProperties())
            {
                Type examType = typeof(Tercero);
                PropertyInfo piInstance = examType.GetProperty(item.Name);
                Type tipo = item.PropertyType;

                if (col_ignorar.Contains(item.Name)) continue;

                switch (Type.GetTypeCode(tipo))
                {
                    case TypeCode.String:
                        piInstance.SetValue(_MTer, string.Empty);
                        break;
                    case TypeCode.Int32:
                        piInstance.SetValue(_MTer, -1);
                        break;
                    case TypeCode.Decimal:
                        piInstance.SetValue(_MTer, 0);
                        break;
                    case TypeCode.Double:
                        piInstance.SetValue(_MTer, 0);
                        break;
                    case TypeCode.Boolean:
                        piInstance.SetValue(_MTer, false);
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

            foreach (var item in MTer.GetType().GetProperties())
            {
                if (col_ignorar.Contains(item.Name)) continue;
                Type examType = typeof(Tercero);
                PropertyInfo piInstance = examType.GetProperty(item.Name);
                var propertyInfo = typeof(Tercero).GetProperties().Where(p => p.Name == item.Name).Single();
                var valueA = propertyInfo.GetValue(MTer, null);
                piInstance.SetValue(_MTer, valueA);
            }

        }
        catch (Exception w)
        {
            MessageBox.Show("error Clone():" + w);
        }
    }

    private void BtnNuevo_Click(object sender, RoutedEventArgs e)
    {
        ClearClas();
        activecontrol(false, "Guardar");
        bloquear(true);
        txter.Focus();
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
                if (SiaWin.Func.DeleteInMaestra(MTer.idrow, "Comae_ter", "idrow", idemp)) ClearClas();
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
                if (!MTer.IsValid())
                {
                    MessageBox.Show("no se puede modificar por que faltan algunos campos que son requeridos", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (!ComparaDatos()) return;


                int query = Modificar();
                if (query > 0)
                {
                    SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idemp, 0, 0, 0, "actulizo exitosamente el tercero" + MTer.cod_ter, "");
                    MessageBox.Show("actualizo exitosamente la informacion del tercero:" + MTer.cod_ter, "alerta", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearClas();
                    editdel(true);
                    bloquear(false);
                    activecontrol(true, "");
                }
            }
            else
            {
                if (!MTer.IsValid())
                {
                    MessageBox.Show("no se puede guardar por que faltan algunos campos que son requeridos", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }


                int query = Insertar();
                if (query > 0)
                {
                    SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idemp, 0, 0, 0, "Inserto exitosamente el tercero" + MTer.cod_ter, "");
                    MessageBox.Show("inserto exitosamente el tercero:" + MTer.cod_ter, "alerta", MessageBoxButton.OK, MessageBoxImage.Information);
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

                        foreach (var item in MTer.GetType().GetProperties())
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

                            var propertyInfo = typeof(Tercero).GetProperties().Where(p => p.Name == key).Single();
                            var valueA = propertyInfo.GetValue(MTer, null);
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

                    foreach (var item in MTer.GetType().GetProperties())
                    {
                        if (!col_ignorar.Contains(item.Name))
                        {
                            if (idrow == item.Name) continue;
                            query += item.Name + "=@" + item.Name + ",";
                            campos.Add(item.Name, item.PropertyType);
                        }
                    }

                    query = query.Remove(query.Length - 1);

                    cmd.CommandText = "UPDATE " + tabla + " SET " + query + " WHERE " + idrow + "=" + MTer.idrow.ToString();

                    foreach (var item in campos)
                    {
                        object val = new object();
                        Type tipo = item.Value;
                        SqlDbType sqlDb = new SqlDbType();


                        var propertyInfo = typeof(Tercero).GetProperties().Where(p => p.Name == item.Key.ToString()).Single();
                        var valueA = propertyInfo.GetValue(MTer, null);

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


    bool ComparaDatos()
    {
        StringBuilder sbRed = new StringBuilder();
        StringBuilder sbLocal = new StringBuilder();
        Tercero __MTer = new Tercero();
        try
        {
            SqlDataReader dr;
            dr = SiaWin.Func.SqlDR("SELECT * FROM Comae_ter  where idrow=" + MTer.idrow.ToString(), idemp);

            dr.Read();
            foreach (var item in MTer.GetType().GetProperties())
            {
                if (col_ignorar.Contains(item.Name)) continue;
                if (col_valor.ContainsKey(item.Name)) continue;
                Type tipo = item.PropertyType;
                Type examType = typeof(Tercero);
                PropertyInfo piInstance = examType.GetProperty(item.Name);
                switch (Type.GetTypeCode(tipo))
                {
                    case TypeCode.String:
                        piInstance.SetValue(__MTer, dr[item.Name] == DBNull.Value ? "" : dr[item.Name].ToString().Trim());
                        break;
                    case TypeCode.Int16:
                        piInstance.SetValue(__MTer, dr[item.Name] == DBNull.Value ? 0 : Convert.ToInt16(dr[item.Name].ToString().Trim()));
                        break;
                    case TypeCode.Int32:
                        piInstance.SetValue(__MTer, dr[item.Name] == DBNull.Value ? 0 : Convert.ToInt32(dr[item.Name].ToString().Trim()));
                        break;
                    case TypeCode.Int64:
                        piInstance.SetValue(__MTer, dr[item.Name] == DBNull.Value ? 0 : Convert.ToInt64(dr[item.Name].ToString().Trim()));
                        break;
                    case TypeCode.Decimal:
                        piInstance.SetValue(__MTer, dr[item.Name] == DBNull.Value ? 0 : Convert.ToDecimal(dr[item.Name].ToString().Trim()));
                        break;
                    case TypeCode.Double:
                        piInstance.SetValue(__MTer, dr[item.Name] == DBNull.Value ? 0 : Convert.ToDouble(dr[item.Name].ToString().Trim()));
                        break;
                    case TypeCode.Boolean:
                        piInstance.SetValue(__MTer, dr[item.Name] == DBNull.Value ? false : Convert.ToBoolean(dr[item.Name].ToString().Trim()));
                        break;
                }
            }

            dr.Close();

            //// recorre campos de la clase
            MTer.GetType().GetProperties().ToList().ForEach(f =>
            {
                try
                {
                        //compara si cambiaron los campos del registro en le servidor
                        var propertyInfo = typeof(Tercero).GetProperties().Where(p => p.Name == f.Name).Single();
                    var valueA = propertyInfo.GetValue(_MTer, null); //ORIGINAL EN MEMORIA
                        var valueB = propertyInfo.GetValue(MTer, null);  //ACTUAL ///
                        var valueC = propertyInfo.GetValue(__MTer, null); //REAL SQL DATA

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

            //registra si ya alguien modifico este tercero
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

    private void txter_LostFocus(object sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty((sender as TextBox).Text)) return;

            string query = "select * from comae_ter where cod_ter = '" + (sender as TextBox).Text + "'";
            DataTable dt = SiaWin.Func.SqlDT(query, "tercero", idemp);
            if (dt.Rows.Count > 0)
            {
                if (SiaWin.Func.Acceso(SiaWin._UserGroup, SiaWin._ProyectId, 6, idemp, 1, "lEdit") == true)
                {
                    int id = (int)dt.Rows[0]["idrow"];
                    ActualizaCampos(id, string.Empty);
                    activecontrol(false, "Modificar");
                    Clone();
                }
                else
                {
                    MessageBox.Show("este usaurio no tiene permisos para editar por favor digite un cliente nuevo", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    (sender as TextBox).Text = "";
                }
            }
        }
        catch (Exception w)
        {
            MessageBox.Show("error el el foco:" + w);
        }
    }

    private void CBtipoPerso_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (CtrlA.Visibility == Visibility.Hidden)
            {
                int selectedIndex = MTer.tip_pers;
                if (selectedIndex == 0)
                {
                    string nombre = MTer.nom_ter;
                    string[] split = nombre.Split(new Char[] { ' ', ',' });

                    if (split.Length <= 3)
                    {
                        MTer.nom1 = split[0];
                        MTer.apl1 = split[1];
                        MTer.apl2 = split[2];
                        MTer.raz = "";
                    }
                    else
                    {
                        MTer.nom1 = split[0];
                        MTer.nom2 = split[1];
                        MTer.apl1 = split[2];
                        MTer.apl2 = split[3];
                        MTer.raz = "";
                    }

                }
                if (selectedIndex == 1)
                {
                    MTer.raz = MTer.nom_ter;
                    MTer.apl1 = "";
                    MTer.nom1 = "";
                    MTer.apl2 = "";
                    MTer.nom2 = "";
                }
            }
        }
        catch (Exception)
        {

        }
    }

    private void BtnBuscarElement_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string tbl = (sender as Button).Tag.ToString();
            string cod = "", nom = "", id = "", tit = "";

            switch (tbl)
            {
                case "MmMae_muni":
                    cod = "cod_muni"; nom = "nom_muni"; id = "idrow"; tit = "Maestra de Municipios";
                    break;
                case "MmMae_pais":
                    cod = "cod_pais"; nom = "nom_pais"; id = "cod_pais"; tit = "Maestra de Pais";
                    break;
            }

            dynamic winb = SiaWin.WindowBuscar(tbl, cod, nom, cod, id, tit, cnEmp, false, "", idEmp: idemp);
            winb.ShowInTaskbar = false;
            winb.Owner = Application.Current.MainWindow;
            winb.Height = 400;
            winb.ShowDialog();
            int idrow = winb.IdRowReturn;
            string codigo = winb.Codigo.Trim();
            string nombre = winb.Nombre.Trim().ToUpper();
            //winb = null;
            if (idrow > 0)
            {
                switch (tbl)
                {
                    case "MmMae_muni":
                        MTer.cod_ciu = codigo; MTer.ciudad = nombre;
                        break;
                    case "MmMae_pais":
                        MTer.cod_pais = codigo;
                        break;
                }

            }
            if (string.IsNullOrEmpty(codigo)) e.Handled = false;
            e.Handled = true;
        }
        catch (Exception w)
        {
            MessageBox.Show("error al buscar:" + w);
        }
    }

    private void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty((sender as TextBox).Text)) return;

            string tbl = (sender as TextBox).Tag.ToString();

            string cod = "", nom = "", tit = "";
            switch (tbl)
            {
                case "MmMae_muni":
                    cod = "cod_muni"; nom = "nom_muni"; tit = "Maestra de Municipios";
                    break;
                case "MmMae_pais":
                    cod = "cod_pais"; nom = "nom_pais"; tit = "Maestra de Pais";
                    break;
            }

            string query = "select * from " + tbl + " where  " + cod + "='" + (sender as TextBox).Text + "' ";
            DataTable dt = SiaWin.Func.SqlDT(query, "tabla", idemp);
            if (dt.Rows.Count > 0)
            {
                string code = dt.Rows[0][cod].ToString();
                string name = dt.Rows[0][nom].ToString();
                switch (tbl)
                {
                    case "MmMae_muni":
                        MTer.cod_ciu = code; MTer.ciudad = name;
                        break;
                    case "MmMae_pais":
                        MTer.cod_pais = code;
                        break;
                }
            }
            else
            {
                MessageBox.Show("el codigo que ingreso no existe en la " + tit, "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                (sender as TextBox).Text = "";
            }

        }
        catch (Exception w)
        {
            MessageBox.Show("error al buscar:" + w);
        }
    }

    private void BtnDigVer_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(MTer.cod_ter))
            {
                MessageBox.Show("el campo de NIT/CC debe de estar lleno para agregar el digito de verificacion", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            string procedure = "EXEC	[dbo].[dig_verif] @nit ='" + MTer.cod_ter + "' ";
            DataTable dt = SiaWin.Func.SqlDT(procedure, "tabla", 0);
            if (dt.Rows.Count > 0)
            {
                MTer.dv = dt.Rows[0]["digito"].ToString();
            }
        }
        catch (Exception w)
        {
            MessageBox.Show("errro en el digito de verificacion" + w);
        }
    }

    private void BtnExport_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(MTer.cod_ter))
            {
                MessageBox.Show("el campo del tercero esta vacio", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            DataTable dt = SiaWin.Func.SqlDT("select * from comae_ter where cod_ter='" + MTer.cod_ter + "'", "tabla", idemp);
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

    private void TextBoxNom_LostFocus(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace((sender as TextBox).Text))
        {
            MTer.repres = ((sender as TextBox).Text);
        }
    }


}
}

