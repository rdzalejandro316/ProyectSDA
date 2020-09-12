using Syncfusion.Windows.Controls;
using Syncfusion.Windows.Shared;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{

    //Sia.PublicarPnt(9639,"CopiarDocPeriodoCO");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9639,"CopiarDocPeriodoCO");
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //ww.ShowDialog();

    public partial class CopiarDocPeriodoCO : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        DataTable trn = new DataTable();
        string tablacab = "cocab_doc";
        string tablacue = "cocue_doc";
        string maetrn = "comae_trn";
        int moduloid = 1;

        List<string> cab_col_exception = new List<string>() { "idreg", "ano_doc", "per_doc", "idregcabref", "fecha_aded" };
        List<string> cue_col_exception = new List<string>() { "idreg", "ano_doc", "per_doc", "fecha_aded" };


        Dictionary<string, string> cab_colmn = new Dictionary<string, string>();
        Dictionary<string, string> cue_colmn = new Dictionary<string, string>();

        
        public CopiarDocPeriodoCO()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            LoadConfig();
        }

        public void loadcolumns()
        {
            try
            {
                cab_colmn.Clear();
                cab_colmn.Add("cod_trn", t_TrnNue.SelectedValue.ToString());
                cab_colmn.Add("num_trn", Tx_NumeroNue.Text);
                cab_colmn.Add("fec_trn", TxFecha.Text);
                cab_colmn.Add("detalle", Tx_DescNue.Text);
                cab_colmn.Add("userid", SiaWin._UserId.ToString());

                cue_colmn.Clear();
                cue_colmn.Add("idregcab", "0");
                cue_colmn.Add("cod_trn", t_TrnNue.SelectedValue.ToString());
                cue_colmn.Add("num_trn", Tx_NumeroNue.Text);
            }
            catch (Exception w)
            {
                MessageBox.Show("error al agrgar columnas:" + 2);
            }
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
                this.Title = "Copiar Documentos a otro Periodo " + nomempresa;

                trn = SiaWin.Func.SqlDT("SELECT rtrim(cod_trn) as cod_trn,rtrim(cod_trn)+'-'+rtrim(nom_trn) as nom_trn FROM "+ maetrn + " order by cod_trn", "transacion", idemp);
                t_TrnCop.ItemsSource = trn.DefaultView;
                t_TrnCop.DisplayMemberPath = "nom_trn";
                t_TrnCop.SelectedValuePath = "cod_trn";


                t_TrnNue.ItemsSource = trn.DefaultView;
                t_TrnNue.DisplayMemberPath = "nom_trn";
                t_TrnNue.SelectedValuePath = "cod_trn";

                TxFecha.Text = DateTime.Now.ToString();
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }


        public bool GetNewDoc()
        {
            DataTable dt_cab = SiaWin.Func.SqlDT("select * from " + tablacab + " where num_trn='" + Tx_NumeroNue.Text + "' and cod_trn='" + t_TrnNue.SelectedValue + "' ", "cabeza", idemp);
            return dt_cab.Rows.Count > 0 ? true : false;
        }



        private async void BtnProcesar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                #region validaciones

                if (t_TrnCop.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione el tipo de transaccion a copiar", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (string.IsNullOrEmpty(Tx_Numero.Text))
                {
                    MessageBox.Show("ingrese el documento a copiar", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }


                if (GetNewDoc() == true)
                {
                    MessageBox.Show("el documento nuevo a copiar:" + Tx_NumeroNue.Text + " ya existe en contabilidad ingrese un codigo diferente", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                #endregion


                DataTable dt_cab = SiaWin.Func.SqlDT("select * from " + tablacab + " where num_trn='" + Tx_Numero.Text + "' and cod_trn='" + t_TrnCop.SelectedValue + "' ", "cabeza", idemp);

                if (dt_cab.Rows.Count <= 0)
                {
                    MessageBox.Show("el documento ingresado:" + Tx_Numero.Text + " no existe", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                else
                {
                    DataTable dt_cue = SiaWin.Func.SqlDT("select * from " + tablacue + " where idregcab='" + dt_cab.Rows[0]["idreg"].ToString() + "' ", "cuerpo", idemp);


                    loadcolumns();

                    CancellationTokenSource source = new CancellationTokenSource();
                    sfBusyIndicator.IsBusy = true;
                    GridMain.IsEnabled = false;

                    if (dt_cue.Rows.Count > 0)
                    {
                        var slowTask = Task<int>.Factory.StartNew(() => ExecuteProcess(dt_cab, dt_cue), source.Token);
                        await slowTask;

                        if (((int)slowTask.Result) > 0)
                        {
                            MessageBox.Show("el documento se copio exitosamente", "procesos exitoso", MessageBoxButton.OK, MessageBoxImage.Information);
                            int idreg = ((int)slowTask.Result);
                            SiaWin.TabTrn(0, idemp, true, idreg, moduloid, WinModal: true);
                            clean();
                        }
                        else
                        {
                            MessageBox.Show("el documento no se copio contacte con el administrador", "procesos fallido", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }

                        #region query

                        //Dictionary<string, Type> listparm_cab = new Dictionary<string, Type>();

                        //#region recorre las columas de la cabeza para saber que es lo que ba a copiar y que no (id autoincrement no se debe de copiar entonces se da una excepción)                        
                        //foreach (DataColumn column in dt_cab.Columns)
                        //{
                        //    if (!cab_col_exception.Contains(column.ColumnName.Trim()))
                        //    {
                        //        listparm_cab.Add("@" + column.ColumnName, column.DataType);
                        //    }
                        //}
                        //#endregion 

                        //#region aca eliminina las columnas que contienen un valor null por q no son necesarias incluirlas en el sqlcommand                        

                        //var listparm_remove = new List<string>();
                        //foreach (var item in listparm_cab)
                        //{
                        //    string valor = item.Key.Replace("@", "");
                        //    if (!cab_colmn.ContainsKey(valor))
                        //    {
                        //        if (dt_cab.Rows[0][valor] == DBNull.Value) listparm_remove.Add(item.Key);
                        //    }
                        //}
                        ////elimina de la lista de paramentros las columnas que son nullas para no agregarlas en el sqlcommand
                        //foreach (var item in listparm_remove) listparm_cab.Remove(item);
                        //#endregion

                        //string cab_colm = String.Join(", ", listparm_cab.Keys.ToArray()).Replace("@", "");
                        //string cab_colm_parm = String.Join(", ", listparm_cab.Keys.ToArray());

                        //string cabeza = "insert into " + tablacab + " (" + cab_colm + ") values (" + cab_colm_parm + "); SELECT SCOPE_IDENTITY()";

                        //using (SqlConnection connection = new SqlConnection(cnEmp))
                        //{
                        //    using (SqlCommand cmd = new SqlCommand(cabeza, connection))
                        //    {
                        //        #region cabeza                                
                        //        foreach (var item in listparm_cab)
                        //        {
                        //            string valor = item.Key.Replace("@", "");
                        //            if (cab_colmn.ContainsKey(valor)) //identifica si se encuentra en una lista que tiene unos valor por defecto en este caso los q ingrese en pnt
                        //            {
                        //                cmd.Parameters.AddWithValue(item.Key, cab_colmn[valor]);
                        //            }
                        //            else // si no los encuentra en la lista ingresa los que estaban en el documento que ba a copiar
                        //            {
                        //                object val = new object();
                        //                Type tipo = item.Value;
                        //                SqlDbType sqlDb = new SqlDbType();

                        //                switch (Type.GetTypeCode(tipo))
                        //                {
                        //                    case TypeCode.String:
                        //                        val = dt_cab.Rows[0][valor] == DBNull.Value ? "" : dt_cab.Rows[0][valor].ToString();
                        //                        sqlDb = SqlDbType.VarChar;
                        //                        break;
                        //                    case TypeCode.Decimal:
                        //                        val = dt_cab.Rows[0][valor] == DBNull.Value ? 0 : Convert.ToDecimal(dt_cab.Rows[0][valor]);
                        //                        sqlDb = SqlDbType.Decimal;
                        //                        break;
                        //                    case TypeCode.Int32:
                        //                        val = dt_cab.Rows[0][valor] == DBNull.Value ? 0 : Convert.ToInt32(dt_cab.Rows[0][valor]);
                        //                        sqlDb = SqlDbType.Int;
                        //                        break;
                        //                    case TypeCode.Boolean:
                        //                        val = dt_cab.Rows[0][valor] == DBNull.Value ? 0 : Convert.ToInt32(dt_cab.Rows[0][valor]);
                        //                        sqlDb = SqlDbType.Int;
                        //                        break;
                        //                    case TypeCode.DateTime:
                        //                        val = dt_cab.Rows[0][valor] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dt_cab.Rows[0][valor]);
                        //                        sqlDb = SqlDbType.Date;
                        //                        break;
                        //                }


                        //                SqlParameter param = new SqlParameter();
                        //                param.ParameterName = item.Key;
                        //                param.Value = val;
                        //                param.SqlDbType = sqlDb;
                        //                cmd.Parameters.Add(param);
                        //            }
                        //        }
                        //        if (connection.State == ConnectionState.Closed) connection.Open();
                        //        #endregion

                        //        int newID = Convert.ToInt32(cmd.ExecuteScalar());

                        //        if (newID == 0) MessageBox.Show("la transacion no fue exitosa", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        //        else
                        //        {

                        //            #region cuerpo
                        //            foreach (DataRow dr in dt_cue.Rows)
                        //            {
                        //                Dictionary<string, Type> listparm_cue = new Dictionary<string, Type>();
                        //                foreach (DataColumn column in dt_cue.Columns)
                        //                {
                        //                    if (!cue_col_exception.Contains(column.ColumnName.Trim()))
                        //                        listparm_cue.Add("@" + column.ColumnName, column.DataType);
                        //                }
                        //                var listparm_remove_cue = new List<string>();
                        //                foreach (var item in listparm_cue)
                        //                {
                        //                    string valor = item.Key.Replace("@", "");
                        //                    if (!cue_colmn.ContainsKey(valor))
                        //                    {
                        //                        if (dr[valor] == DBNull.Value)
                        //                            listparm_remove_cue.Add(item.Key);
                        //                    }
                        //                }
                        //                foreach (var item in listparm_remove_cue) listparm_cue.Remove(item);
                        //                string cue_cl_parm = String.Join(", ", listparm_cue.Keys.ToArray());
                        //                string cue_cl = String.Join(", ", listparm_cue.Keys.ToArray()).Replace("@", "");

                        //                string query_cu = "insert into " + tablacue + " (" + cue_cl + ") values (" + cue_cl_parm + "); SELECT SCOPE_IDENTITY()";


                        //                using (SqlCommand cmd_cu = new SqlCommand(query_cu, connection))
                        //                {
                        //                    foreach (var item in listparm_cue)
                        //                    {
                        //                        string valor = item.Key.Replace("@", "");
                        //                        if (cue_colmn.ContainsKey(valor))
                        //                        {
                        //                            cmd_cu.Parameters.AddWithValue(item.Key, valor == "idregcab" ? newID.ToString() : cue_colmn[valor]);
                        //                        }
                        //                        else
                        //                        {
                        //                            object val = new object();
                        //                            Type tipo = item.Value;
                        //                            SqlDbType sqlDb = new SqlDbType();

                        //                            switch (Type.GetTypeCode(tipo))
                        //                            {
                        //                                case TypeCode.String:
                        //                                    val = dr[valor] == DBNull.Value ? "" : dr[valor].ToString();
                        //                                    sqlDb = SqlDbType.VarChar;
                        //                                    break;
                        //                                case TypeCode.Decimal:
                        //                                    val = dr[valor] == DBNull.Value ? 0 : Convert.ToDecimal(dr[valor]);
                        //                                    sqlDb = SqlDbType.Decimal;
                        //                                    break;
                        //                                case TypeCode.Int32:
                        //                                    val = dr[valor] == DBNull.Value ? 0 : Convert.ToInt32(dr[valor]);
                        //                                    sqlDb = SqlDbType.Int;
                        //                                    break;
                        //                                case TypeCode.Boolean:
                        //                                    val = dr[valor] == DBNull.Value ? 0 : Convert.ToInt32(dr[valor]);
                        //                                    sqlDb = SqlDbType.Bit;
                        //                                    break;
                        //                                case TypeCode.DateTime:
                        //                                    val = dr[valor] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr[valor]);
                        //                                    sqlDb = SqlDbType.Date;
                        //                                    break;
                        //                            }

                        //                            SqlParameter param = new SqlParameter();
                        //                            param.ParameterName = item.Key;
                        //                            param.Value = val;
                        //                            param.SqlDbType = sqlDb;
                        //                            cmd_cu.Parameters.Add(param);
                        //                        }
                        //                    }
                        //                    cmd_cu.ExecuteScalar();
                        //                    cmd_cu.Parameters.Clear();
                        //                }
                        //            }
                        //            #endregion

                        //            MessageBox.Show("copia de documento exitosa", "procesos exitoso", MessageBoxButton.OK, MessageBoxImage.Information);
                        //            clean();
                        //        }
                        //    }
                        //}
                        #endregion

                    }
                    else
                    {
                        MessageBox.Show("el documento ingresado:" + Tx_Numero.Text + " no tiene cuerpo consulte con el administrador del sistema", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }

                    sfBusyIndicator.IsBusy = false;
                    GridMain.IsEnabled = true;
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al procesar:" + w);
            }
        }

        private int ExecuteProcess(DataTable dt_cab, DataTable dt_cue)
        {
            try
            {
                int idrow = 0;

                Dictionary<string, Type> listparm_cab = new Dictionary<string, Type>();

                #region recorre las columas de la cabeza para saber que es lo que ba a copiar y que no (id autoincrement no se debe de copiar entonces se da una excepción)                        
                foreach (DataColumn column in dt_cab.Columns)
                {
                    if (!cab_col_exception.Contains(column.ColumnName.Trim()))
                    {
                        listparm_cab.Add("@" + column.ColumnName, column.DataType);
                    }
                }
                #endregion

                #region aca eliminina las columnas que contienen un valor null por q no son necesarias incluirlas en el sqlcommand                        

                var listparm_remove = new List<string>();
                foreach (var item in listparm_cab)
                {
                    string valor = item.Key.Replace("@", "");
                    if (!cab_colmn.ContainsKey(valor))
                    {
                        if (dt_cab.Rows[0][valor] == DBNull.Value) listparm_remove.Add(item.Key);
                    }
                }
                //elimina de la lista de paramentros las columnas que son nullas para no agregarlas en el sqlcommand
                foreach (var item in listparm_remove) listparm_cab.Remove(item);
                #endregion

                string cab_colm = String.Join(", ", listparm_cab.Keys.ToArray()).Replace("@", "");
                string cab_colm_parm = String.Join(", ", listparm_cab.Keys.ToArray());

                string cabeza = "insert into " + tablacab + " (" + cab_colm + ") values (" + cab_colm_parm + "); SELECT SCOPE_IDENTITY()";

                using (SqlConnection connection = new SqlConnection(cnEmp))
                {
                    using (SqlCommand cmd = new SqlCommand(cabeza, connection))
                    {
                        #region cabeza                                
                        foreach (var item in listparm_cab)
                        {
                            string valor = item.Key.Replace("@", "");
                            if (cab_colmn.ContainsKey(valor)) //identifica si se encuentra en una lista que tiene unos valor por defecto en este caso los q ingrese en pnt
                            {
                                cmd.Parameters.AddWithValue(item.Key, cab_colmn[valor]);
                            }
                            else // si no los encuentra en la lista ingresa los que estaban en el documento que ba a copiar
                            {
                                object val = new object();
                                Type tipo = item.Value;
                                SqlDbType sqlDb = new SqlDbType();
                                


                                switch (Type.GetTypeCode(tipo))
                                {
                                    case TypeCode.String:
                                        val = dt_cab.Rows[0][valor] == DBNull.Value ? "" : dt_cab.Rows[0][valor].ToString();
                                        sqlDb = SqlDbType.VarChar;
                                        break;
                                    case TypeCode.Decimal:
                                        val = dt_cab.Rows[0][valor] == DBNull.Value ? 0 : Convert.ToDecimal(dt_cab.Rows[0][valor]);
                                        sqlDb = SqlDbType.Decimal;
                                        break;
                                    case TypeCode.Int16:
                                        val = dt_cab.Rows[0][valor] == DBNull.Value ? 0 : Convert.ToInt16(dt_cab.Rows[0][valor]);
                                        sqlDb = SqlDbType.SmallInt;
                                        break;
                                    case TypeCode.Int32:
                                        val = dt_cab.Rows[0][valor] == DBNull.Value ? 0 : Convert.ToInt32(dt_cab.Rows[0][valor]);
                                        sqlDb = SqlDbType.Int;
                                        break;
                                    case TypeCode.Boolean:
                                        val = dt_cab.Rows[0][valor] == DBNull.Value ? 0 : Convert.ToInt32(dt_cab.Rows[0][valor]);
                                        sqlDb = SqlDbType.Int;
                                        break;
                                    case TypeCode.DateTime:
                                        val = dt_cab.Rows[0][valor] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dt_cab.Rows[0][valor]);
                                        sqlDb = SqlDbType.Date;
                                        break;
                                }

                                SqlParameter param = new SqlParameter();
                                param.ParameterName = item.Key;
                                param.Value = val;
                                param.SqlDbType = sqlDb;
                                cmd.Parameters.Add(param);                                
                            }
                        }
                        if (connection.State == ConnectionState.Closed) connection.Open();
                        #endregion


                        int newID = Convert.ToInt32(cmd.ExecuteScalar());

                        if (newID > 0)                        
                        {
                            idrow = newID;
                            #region cuerpo
                            foreach (DataRow dr in dt_cue.Rows)
                            {
                                Dictionary<string, Type> listparm_cue = new Dictionary<string, Type>();
                                foreach (DataColumn column in dt_cue.Columns)
                                {
                                    if (!cue_col_exception.Contains(column.ColumnName.Trim()))
                                        listparm_cue.Add("@" + column.ColumnName, column.DataType);
                                }
                                var listparm_remove_cue = new List<string>();
                                foreach (var item in listparm_cue)
                                {
                                    string valor = item.Key.Replace("@", "");
                                    if (!cue_colmn.ContainsKey(valor))
                                    {
                                        if (dr[valor] == DBNull.Value)
                                            listparm_remove_cue.Add(item.Key);
                                    }
                                }
                                foreach (var item in listparm_remove_cue) listparm_cue.Remove(item);
                                string cue_cl_parm = String.Join(", ", listparm_cue.Keys.ToArray());
                                string cue_cl = String.Join(", ", listparm_cue.Keys.ToArray()).Replace("@", "");

                                string query_cu = "insert into " + tablacue + " (" + cue_cl + ") values (" + cue_cl_parm + "); SELECT SCOPE_IDENTITY()";


                                using (SqlCommand cmd_cu = new SqlCommand(query_cu, connection))
                                {
                                    foreach (var item in listparm_cue)
                                    {
                                        string valor = item.Key.Replace("@", "");
                                        if (cue_colmn.ContainsKey(valor))
                                        {
                                            cmd_cu.Parameters.AddWithValue(item.Key, valor == "idregcab" ? newID.ToString() : cue_colmn[valor]);
                                        }
                                        else
                                        {
                                            object val = new object();
                                            Type tipo = item.Value;
                                            SqlDbType sqlDb = new SqlDbType();

                                            switch (Type.GetTypeCode(tipo))
                                            {
                                                case TypeCode.String:
                                                    val = dr[valor] == DBNull.Value ? "" : dr[valor].ToString();
                                                    sqlDb = SqlDbType.VarChar;
                                                    break;
                                                case TypeCode.Decimal:
                                                    val = dr[valor] == DBNull.Value ? 0 : Convert.ToDecimal(dr[valor]);
                                                    sqlDb = SqlDbType.Decimal;
                                                    break;
                                                case TypeCode.Int32:
                                                    val = dr[valor] == DBNull.Value ? 0 : Convert.ToInt32(dr[valor]);
                                                    sqlDb = SqlDbType.Int;
                                                    break;
                                                case TypeCode.Boolean:
                                                    val = dr[valor] == DBNull.Value ? 0 : Convert.ToInt32(dr[valor]);
                                                    sqlDb = SqlDbType.Bit;
                                                    break;
                                                case TypeCode.DateTime:
                                                    val = dr[valor] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr[valor]);
                                                    sqlDb = SqlDbType.Date;
                                                    break;
                                            }

                                            SqlParameter param = new SqlParameter();
                                            param.ParameterName = item.Key;
                                            param.Value = val;
                                            param.SqlDbType = sqlDb;
                                            cmd_cu.Parameters.Add(param);
                                        }
                                    }
                                    cmd_cu.ExecuteScalar();
                                    cmd_cu.Parameters.Clear();
                                }
                            }
                            #endregion
                        }
                    }
                }

                return idrow;
            }
            catch (Exception e)
            {
                MessageBox.Show("erro al procesar datos:" + e);
                return 0;
            }
        }



        public void clean()
        {
            t_TrnCop.SelectedIndex = -1;
            Tx_Numero.Text = "";

            t_TrnNue.SelectedIndex = -1;
            Tx_NumeroNue.Text = "";
            Tx_DescNue.Text = "";
        }

        public string getUser()
        {
            string nameUsu = "";
            DataTable dt = SiaWin.Func.SqlDT("select UserName,UserAlias from Seg_User where UserId='" + SiaWin._UserId + "' ", "usuarios", 0);
            if (dt.Rows.Count > 0) nameUsu = dt.Rows[0]["username"].ToString().Trim();
            return nameUsu;
        }


        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {

            //this.Close();
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int idr = 0; string code = ""; string nombre = "";
                dynamic xx = SiaWin.WindowBuscar(tablacab, "cod_trn", "num_trn", "cod_trn", "idreg", "Documentos", cnEmp, false, "", idEmp: idemp);
                xx.ShowInTaskbar = false;
                xx.Owner = Application.Current.MainWindow;
                xx.Height = 400;
                xx.Width = 400;
                xx.ShowDialog();
                idr = xx.IdRowReturn;
                code = xx.Codigo;
                nombre = xx.Nombre;
                xx = null;
                if (idr > 0)
                {
                    Tx_Numero.Text = nombre;
                    selectedTrn(code);
                    DataTable dt = SiaWin.Func.SqlDT("select * from " + tablacab + " where num_trn='" + nombre + "' and cod_trn='" + t_TrnCop.SelectedValue + "' ", "table", idemp);
                    if (dt.Rows.Count > 0)
                    {
                        Tx_anoCop.Value = dt.Rows[0]["fec_trn"].ToString();
                        Tx_perCop.Value = dt.Rows[0]["fec_trn"].ToString();
                    }
                }
                if (string.IsNullOrEmpty(code)) e.Handled = false;
                if (string.IsNullOrEmpty(code)) return;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al buscar la transaccion:" + w);
            }

        }


        public void selectedTrn(string code)
        {
            string query = "select * from "+ maetrn + " where cod_trn='" + code + "' ";
            DataTable dt = SiaWin.Func.SqlDT(query, "table", idemp);
            if (dt.Rows.Count > 0)
            {
                int i = 0;
                foreach (DataRow item in trn.Rows)
                {
                    if (item["cod_trn"].ToString().Trim() == code.Trim()) t_TrnCop.SelectedIndex = i;
                    i++;
                }
            }

        }

        private void Tx_Numero_LostFocus(object sender, RoutedEventArgs e)
        {
            string document = (sender as TextBox).Text.Trim();
            string tipo = (sender as TextBox).Tag.ToString();

            if (string.IsNullOrEmpty(document)) return;
            if (tipo == "doc_viejo")
            {

                if (t_TrnCop.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione el tipo de transaccion", "alerta", MessageBoxButton.OK, MessageBoxImage.Stop);
                    (sender as TextBox).Text = "";
                    return;
                }

                DataTable dt = SiaWin.Func.SqlDT("select * from " + tablacab + " where num_trn='" + document + "' and cod_trn='" + t_TrnCop.SelectedValue + "' ", "table", idemp);
                if (dt.Rows.Count > 0)
                {
                    selectedTrn(dt.Rows[0]["cod_trn"].ToString());
                    Tx_anoCop.Value = dt.Rows[0]["fec_trn"].ToString();
                    Tx_perCop.Value = dt.Rows[0]["fec_trn"].ToString();
                    (sender as TextBox).Foreground = Brushes.Black;
                }
                else
                {
                    MessageBox.Show("el documento ingresado no existe", "alerta", MessageBoxButton.OK, MessageBoxImage.Error);
                    (sender as TextBox).Foreground = Brushes.Red;
                    return;
                }
            }

        }









    }
}

