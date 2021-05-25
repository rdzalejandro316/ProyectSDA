using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MenuReporteParametros
{

    public partial class WinParm : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        string tabla = "Menu_Reports_Parameter";
        string idrow = "idrow";


        public bool flag = false;
        public int idrowpar = 0;//id del parametro
        public int idrow_rep = 0;//id del reporte
        public string name_rep = "";//nombre del reporte

        public ReportParameterInfoCollection par_report;

        List<string> col_ignorar = new List<string>() { "Error", "Item" };
        ParametrosInforme info = new ParametrosInforme();



        public WinParm()
        {
            InitializeComponent();
        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string noinforesa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Parametros " + cod_empresa + "-" + noinforesa;

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
                this.DataContext = info;
                LoadConfig();


                if (par_report != null)
                {
                    List<string> list = new List<string>();
                    foreach (var item in par_report)
                    {
                        list.Add(item.Name);
                        CbParametros.Items.Add(item.Name);
                    }



                }


                TxName.Text = name_rep;
                info.idrow_rep = idrow_rep;

                if (idrowpar > 0)
                {
                    ActualizaCampos(idrowpar, "");
                }


            }
            catch (Exception w)
            {
                MessageBox.Show("errro en Window_Loaded:" + w);
            }
        }

        void ActualizaCampos(int Id, string _Sql)
        {
            try
            {
                SqlDataReader dr;

                dr = _Sql == string.Empty ? SiaWin.Func.SqlDR("SELECT * FROM " + tabla + " where idrow=" + Id.ToString(), 0) : dr = SiaWin.Func.SqlDR(_Sql, idemp);

                dr.Read();
                foreach (var item in info.GetType().GetProperties())
                {
                    if (col_ignorar.Contains(item.Name)) continue;

                    Type tipo = item.PropertyType;

                    Type examType = typeof(ParametrosInforme);
                    PropertyInfo piInstance = examType.GetProperty(item.Name);
                    switch (Type.GetTypeCode(tipo))
                    {
                        case TypeCode.String:
                            piInstance.SetValue(info, dr[item.Name] == DBNull.Value ? "" : dr[item.Name].ToString().Trim());
                            break;
                        case TypeCode.Int16:
                            piInstance.SetValue(info, dr[item.Name] == DBNull.Value ? 0 : Convert.ToInt16(dr[item.Name].ToString().Trim()));
                            break;
                        case TypeCode.Int32:
                            piInstance.SetValue(info, dr[item.Name] == DBNull.Value ? 0 : Convert.ToInt32(dr[item.Name].ToString().Trim()));
                            break;
                        case TypeCode.Int64:
                            piInstance.SetValue(info, dr[item.Name] == DBNull.Value ? 0 : Convert.ToInt64(dr[item.Name].ToString().Trim()));
                            break;
                        case TypeCode.Decimal:
                            piInstance.SetValue(info, dr[item.Name] == DBNull.Value ? 0 : Convert.ToDecimal(dr[item.Name].ToString().Trim()));
                            break;
                        case TypeCode.Double:
                            piInstance.SetValue(info, dr[item.Name] == DBNull.Value ? 0 : Convert.ToDouble(dr[item.Name].ToString().Trim()));
                            break;
                        case TypeCode.Boolean:
                            piInstance.SetValue(info, dr[item.Name] == DBNull.Value ? false : Convert.ToBoolean(dr[item.Name].ToString().Trim()));
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




        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (idrowpar > 0)
                {
                    if (info.IsValid())
                    {
                        MessageBox.Show("no se puede modificar por que faltan algunos campos que son requeridos", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }

                    int query = Modificar();
                    if (query > 0)
                    {
                        flag = true;
                        MessageBox.Show("actualizo exitosamente la informacion del Parametro:" + info.parameter, "alerta", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }

                }
                else
                {
                    if (info.IsValid())
                    {
                        MessageBox.Show("no se puede guardar por que faltan algunos campos que son requeridos", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }


                    int query = Insertar();
                    if (query > 0)
                    {
                        flag = true;
                        MessageBox.Show("inserto exitosamente la informacion del parametro:" + info.parameter, "alerta", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }

                }


            }
            catch (Exception w)
            {
                MessageBox.Show("error al guardar:" + w);
            }

        }


        int Insertar()
        {
            try
            {
                int valor = 0;
                using (SqlConnection connection = new SqlConnection(SiaWin._cn))
                {
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        try
                        {
                            Dictionary<string, Type> campos = new Dictionary<string, Type>();

                            foreach (var item in info.GetType().GetProperties())
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

                                var propertyInfo = typeof(ParametrosInforme).GetProperties().Where(p => p.Name == key).Single();
                                var valueA = propertyInfo.GetValue(info, null);
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


                                SqlParameter param = new SqlParameter();
                                param.ParameterName = item.Key;
                                param.Value = val;
                                param.SqlDbType = sqlDb;
                                cmd.Parameters.Add(param);

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
                using (SqlConnection connection = new SqlConnection(SiaWin._cn))
                {
                    using (SqlCommand cmd = connection.CreateCommand())
                    {

                        string query = " ";
                        Dictionary<string, Type> campos = new Dictionary<string, Type>();

                        foreach (var item in info.GetType().GetProperties())
                        {
                            if (!col_ignorar.Contains(item.Name))
                            {
                                if (idrow == item.Name) continue;
                                query += item.Name + "=@" + item.Name + ",";
                                campos.Add(item.Name, item.PropertyType);
                            }
                        }

                        query = query.Remove(query.Length - 1);

                        cmd.CommandText = "UPDATE " + tabla + " SET " + query + " WHERE " + idrow + "=" + idrowpar.ToString();

                        foreach (var item in campos)
                        {
                            object val = new object();
                            Type tipo = item.Value;
                            SqlDbType sqlDb = new SqlDbType();


                            var propertyInfo = typeof(ParametrosInforme).GetProperties().Where(p => p.Name == item.Key.ToString()).Single();
                            var valueA = propertyInfo.GetValue(info, null);

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



                                SqlParameter param = new SqlParameter();
                                param.ParameterName = "@" + item.Key;
                                param.Value = val;
                                param.SqlDbType = sqlDb;
                                cmd.Parameters.Add(param);

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



        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            flag = false;
            this.Close();
        }

        private void CbParametros_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (CbParametros.SelectedIndex >= 0)
                {                    
                    info.parameter = CbParametros.SelectedValue.ToString();
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al selecionar:CbParametros_SelectionChanged" + w);
            }
        }


    }
}
