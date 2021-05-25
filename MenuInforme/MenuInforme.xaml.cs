using Microsoft.Reporting.WinForms;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
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
    //Sia.PublicarPnt(9697, "MenuInforme");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9697, "Informe");  
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();
    public class Parametros
    {
        public string parameter { get; set; }
        public bool is_valid { get; set; }
        public bool is_table { get; set; }
        public bool is_combo { get; set; }
        public bool is_multivalue { get; set; }

        public string name_master { get; set; }
        public string tabla { get; set; }
        public string cod_tbl { get; set; }
        public string nom_tbl { get; set; }
        public string whereMaster { get; set; }
        public string orderMaster { get; set; }
        public string columns { get; set; }
        public string dataDifferent { get; set; }
        public bool viewall { get; set; }
        public bool is_business { get; set; }
    }

    public partial class MenuInforme : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";


        public string stored_p;
        public string param_emp;
        public int idrowReport;
        public string reportName;
        public ReportViewer report;
        public ReportParameterInfoCollection report_parameter;
        List<ReportParameter> parameters = new List<ReportParameter>();
        DataTable dt_param = new DataTable();
        DataTable dtserver = new DataTable();

        public class MultiTag
        {
            public ReportParameterInfo reportParm { get; set; }
            public Parametros parametros { get; set; }
            public bool MultiValue { get; set; }//este es el multivalue del reporte no el de la tabla de parametros
        }

        public MenuInforme()
        {
            try
            {
                InitializeComponent();
                SiaWin = Application.Current.MainWindow;
            }
            catch (Exception w)
            {
                MessageBox.Show("error constructor:" + w);
            }
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
                Title = reportName.Trim() + "-" + nomempresa.Trim();

                if (idrowReport > 0)
                {

                    dt_param = SiaWin.Func.SqlDT("select * From Menu_Reports_Parameter where idrow_rep=" + idrowReport + ";", "temp", 0);
                    dtserver = SiaWin.Func.SqlDT("select ServerIP,UserServer,UserServerPassword,UserSql,UserSqlPassword from ReportServer;", "temp", 0);

                }

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

                LoadConfig();


                if (report_parameter != null)
                {


                    #region forma columnas en un grilla dinamica                    
                    RowDefinitionCollection rowDefCollection = GridMain.RowDefinitions;
                    ColumnDefinitionCollection colDefCollection = GridMain.ColumnDefinitions;
                    if (report_parameter.Count > 0)
                    {
                        for (int j = 0; j < report_parameter.Count; j++)
                        {
                            rowDefCollection.Add(new RowDefinition() { Height = new GridLength(35) });
                        }

                        colDefCollection.Add(new ColumnDefinition() { Width = new GridLength(200) });
                        colDefCollection.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    }
                    #endregion



                    int i = 0;
                    foreach (var item in report_parameter)
                    {
                        string textdefault = "";

                        if (item.Values.Count > 0)
                        {
                            foreach (var f in item.Values) textdefault = f.ToString();
                        }



                        // valida si esta oculto entonces se salta este campo
                        if (string.IsNullOrEmpty(item.Prompt)) continue;

                        TextBlock TxTitle = new TextBlock()
                        { Text = item.Prompt, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center, FontWeight = FontWeights.Bold };

                        Grid.SetRow(TxTitle, i);
                        GridMain.Children.Add(TxTitle);

                        bool flagF8 = false;
                        string name = item.Name;
                        Parametros parm_temp = new Parametros();
                        if (dt_param.Rows.Count > 0)
                        {
                            DataRow[] rows = dt_param.Select("parameter='" + item.Name + "'");
                            if (rows.Length > 0)
                            {
                                flagF8 = true;
                                foreach (DataRow row in rows)
                                {
                                    parm_temp.parameter = row["parameter"].ToString().Trim();
                                    parm_temp.is_valid = Convert.ToBoolean(row["isValid"]);
                                    parm_temp.is_table = Convert.ToBoolean(row["isTable"]);
                                    parm_temp.is_combo = Convert.ToBoolean(row["isCombo"]);
                                    parm_temp.is_multivalue = Convert.ToBoolean(row["isMultiValue"]);
                                    parm_temp.name_master = row["nameMaster"].ToString().Trim();
                                    parm_temp.tabla = row["tabla"].ToString().Trim();
                                    parm_temp.cod_tbl = row["cod_tbl"].ToString().Trim();
                                    parm_temp.nom_tbl = row["nom_tbl"].ToString().Trim();
                                    parm_temp.whereMaster = row["whereMaster"].ToString().Trim();
                                    parm_temp.orderMaster = row["orderMaster"].ToString().Trim();
                                    parm_temp.columns = row["columns"].ToString().Trim();
                                    parm_temp.dataDifferent = row["dataDifferent"].ToString().Trim();
                                    parm_temp.viewall = Convert.ToBoolean(row["viewAll"]);
                                    parm_temp.is_business = Convert.ToBoolean(row["isBusiness"]);
                                }
                            }
                            else
                            {
                                parm_temp.parameter = item.Name;
                                parm_temp.is_valid = false;
                                parm_temp.is_table = false;
                                parm_temp.is_combo = false;
                                parm_temp.is_multivalue = false;
                                parm_temp.name_master = "";
                                parm_temp.tabla = "";
                                parm_temp.cod_tbl = "";
                                parm_temp.nom_tbl = "";
                                parm_temp.whereMaster = "";
                                parm_temp.columns = "";
                                parm_temp.viewall = false;
                                parm_temp.is_business = false;
                            }

                        }

                        switch (item.DataType)
                        {
                            case ParameterDataType.Boolean:
                                CheckBox checkBox = new CheckBox()
                                { Name = item.Name, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(5, 0, 0, 0) };
                                checkBox.Tag = new MultiTag() { parametros = parm_temp, reportParm = item, MultiValue = false };
                                Grid.SetRow(checkBox, i);
                                Grid.SetColumn(checkBox, 1);
                                GridMain.Children.Add(checkBox);
                                break;
                            case ParameterDataType.DateTime:
                                DatePicker date = new DatePicker()
                                { Name = item.Name, Width = 100, Height = 25, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(5, 0, 0, 0) };
                                date.Tag = new MultiTag() { parametros = parm_temp, reportParm = item, MultiValue = false };
                                date.Text = string.IsNullOrEmpty(textdefault) ? DateTime.Now.ToString() : textdefault;
                                Grid.SetRow(date, i);
                                Grid.SetColumn(date, 1);
                                GridMain.Children.Add(date);
                                break;
                            case ParameterDataType.Float:
                                break;
                            case ParameterDataType.Integer:

                                UpDown down = new UpDown()
                                { Width = 100, Height = 25, Margin = new Thickness(5, 0, 0, 0), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center, MinValue = 0, NumberDecimalDigits = 0 };
                                down.Tag = new MultiTag() { parametros = parm_temp, reportParm = item, MultiValue = false };
                                Grid.SetRow(down, i);
                                Grid.SetColumn(down, 1);
                                GridMain.Children.Add(down);
                                break;
                            case ParameterDataType.String:

                                if (flagF8)
                                {

                                    if (parm_temp.is_table)
                                    {
                                        TextBox textBox = new TextBox()
                                        { Width = 200, Height = 25, Name = item.Name, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(5, 0, 0, 0), ToolTip = "F8 buscar" };
                                        textBox.Text = textdefault;
                                        textBox.Tag = new MultiTag() { parametros = parm_temp, reportParm = item, MultiValue = item.MultiValue };
                                        textBox.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(TextBox_PreviewKeyDown);

                                        Grid.SetRow(textBox, i);
                                        Grid.SetColumn(textBox, 1);
                                        GridMain.Children.Add(textBox);
                                    }

                                    if (parm_temp.is_combo)
                                    {

                                        ComboBoxAdv combo = new ComboBoxAdv()
                                        { Name = item.Name, SelectedValueDelimiter = ",", Width = 200, Height = 25, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(5, 0, 0, 0) };

                                        combo.Tag = new MultiTag() { parametros = parm_temp, reportParm = item, MultiValue = item.MultiValue };
                                        if (parm_temp.is_multivalue)
                                        {
                                            combo.AllowSelectAll = true;
                                            combo.AllowMultiSelect = true;
                                        }


                                        if (!string.IsNullOrEmpty(param_emp))
                                        {
                                            if (item.Name == param_emp) combo.SelectionChanged += new SelectionChangedEventHandler(ComboBoxChanged);
                                        }

                                        string campos = parm_temp.columns;
                                        string tabla = parm_temp.tabla;
                                        string where = !string.IsNullOrEmpty(parm_temp.whereMaster) ? " where " + parm_temp.whereMaster : "";
                                        string order = !string.IsNullOrEmpty(parm_temp.orderMaster) ? " order by " + parm_temp.orderMaster : "";
                                        int id_emp = parm_temp.is_business ? idemp : 0;

                                        string query = string.IsNullOrEmpty(parm_temp.dataDifferent) ? $"select {campos} from {tabla}  {where} {order} " : parm_temp.dataDifferent;

                                        DataTable dt = SiaWin.Func.SqlDT(query, "Menu", id_emp);

                                        if (dt.Rows.Count > 0)
                                        {
                                            combo.ItemsSource = dt.DefaultView;
                                            combo.DisplayMemberPath = parm_temp.nom_tbl;
                                            combo.SelectedValuePath = parm_temp.cod_tbl;
                                        }



                                        Grid.SetRow(combo, i);
                                        Grid.SetColumn(combo, 1);
                                        GridMain.Children.Add(combo);
                                    }
                                }
                                else
                                {

                                    TextBox textBox = new TextBox()
                                    { Width = 200, Height = 25, Name = item.Name, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(5, 0, 0, 0) };

                                    textBox.Tag = new MultiTag() { parametros = parm_temp, reportParm = item, MultiValue = item.MultiValue };

                                    Grid.SetRow(textBox, i);
                                    Grid.SetColumn(textBox, 1);
                                    GridMain.Children.Add(textBox);
                                }
                                break;
                        }

                        i++;

                    }

                }
                else
                {
                    MessageBox.Show("nullo");
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("erro loaded:" + w);
            }

        }

        private void ComboBoxChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBoxAdv combo = (sender as ComboBoxAdv);

                if (combo.SelectedIndex >= 0)
                {
                    string value = combo.SelectedValue.ToString();
                    DataRow[] rows = SiaWin.Empresas.Select("BusinessCode='" + value + "'");

                    if (rows.Length > 0)
                    {
                        foreach (DataRow row in rows)
                            idemp = Convert.ToInt32(row["BusinessId"]);

                        if (!string.IsNullOrEmpty(param_emp))
                        {

                            foreach (FrameworkElement panel in GridMain.Children)
                            {
                                if (panel is TextBlock) continue;

                                if (panel is ComboBoxAdv)
                                {
                                    var comb_parm = ((ComboBoxAdv)panel);

                                    MultiTag tag = (MultiTag)((ComboBoxAdv)panel).Tag;
                                    if (tag.parametros.is_combo)
                                    {
                                        string campos = tag.parametros.columns;
                                        string tabla = tag.parametros.tabla;
                                        string where = !string.IsNullOrEmpty(tag.parametros.whereMaster) ? " where " + tag.parametros.whereMaster : "";
                                        string order = !string.IsNullOrEmpty(tag.parametros.orderMaster) ? " order by " + tag.parametros.orderMaster : "";

                                        if (tag.parametros.parameter != param_emp)
                                        {
                                            if (string.IsNullOrEmpty(tag.parametros.dataDifferent))
                                            {
                                                string query = $"select {campos} from {tabla}  {where} {order} ";

                                                DataTable dt = SiaWin.Func.SqlDT(query, "temp", idemp);
                                                if (dt.Rows.Count > 0)
                                                {
                                                    comb_parm.ItemsSource = dt.DefaultView;
                                                    comb_parm.DisplayMemberPath = tag.parametros.nom_tbl;
                                                    comb_parm.SelectedValuePath = tag.parametros.cod_tbl;
                                                }
                                                else
                                                {
                                                    comb_parm.ItemsSource = null;
                                                }
                                            }
                                        }
                                    }
                                }

                            }

                        }

                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al en el seleccition changed:" + w);
            }
        }


        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {

                if (e.Key == Key.F8)
                {
                    var parm = (MultiTag)(sender as TextBox).Tag;

                    dynamic winb = SiaWin.WindowBuscar(parm.parametros.tabla, parm.parametros.cod_tbl, parm.parametros.nom_tbl, parm.parametros.cod_tbl, parm.parametros.cod_tbl, parm.parametros.name_master, cnEmp, parm.parametros.viewall, parm.parametros.whereMaster, idEmp: idemp);
                    winb.ShowInTaskbar = false;
                    winb.Owner = Application.Current.MainWindow;
                    winb.Height = 400;
                    winb.ShowDialog();
                    int id = winb.IdRowReturn;
                    string code = winb.Codigo;
                    string nom = winb.Nombre;
                    if (!string.IsNullOrEmpty(code))
                    {
                        if (parm.parametros.is_multivalue)
                        {
                            if (string.IsNullOrEmpty((sender as TextBox).Text))
                                (sender as TextBox).Text = code;
                            else
                                (sender as TextBox).Text += "," + code;
                        }
                        else
                            (sender as TextBox).Text = code;
                    }

                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error en preview keydown:" + w);
            }
        }

        private void BtnConsultar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                //mierda
                #region validaciones

                foreach (FrameworkElement panel in GridMain.Children)
                {


                    if (panel is TextBox)
                    {
                        MultiTag tag = (MultiTag)((TextBox)panel).Tag;

                        if (!tag.reportParm.AllowBlank)
                        {

                            if (string.IsNullOrWhiteSpace(((TextBox)panel).Text))
                            {
                                MessageBox.Show($"debe de ingresar un valor en el campo: {tag.reportParm.Prompt}", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                return;
                            }

                        }
                    }

                    if (panel is ComboBoxAdv)
                    {
                        MultiTag tag = (MultiTag)((ComboBoxAdv)panel).Tag;

                        if (!tag.reportParm.AllowBlank)
                        {
                            if (((ComboBoxAdv)panel).AllowMultiSelect)
                            {

                                if (((ComboBoxAdv)panel).SelectedItems == null)
                                {
                                    MessageBox.Show($"debe de ingresar un valor en el campo: {tag.reportParm.Prompt}", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                    return;
                                }
                                else
                                {
                                    string val = "";
                                    foreach (DataRowView ob in ((ComboBoxAdv)panel).SelectedItems) val += ob[0].ToString();

                                    if (string.IsNullOrEmpty(val))
                                    {
                                        MessageBox.Show($"debe de ingresar un valor en el campo: {tag.reportParm.Prompt}", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                        return;
                                    }
                                }

                            }
                            else
                            {
                                if (((ComboBoxAdv)panel).SelectedIndex < 0)
                                {
                                    MessageBox.Show($"debe de ingresar un valor en el campo: {tag.reportParm.Prompt}", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                    return;
                                }
                            }
                        }
                    }

                    if (panel is DatePicker)
                    {
                        MultiTag tag = (MultiTag)((DatePicker)panel).Tag;

                        if (!tag.reportParm.AllowBlank)
                        {
                            if (string.IsNullOrWhiteSpace(((DatePicker)panel).Text))
                            {
                                MessageBox.Show($"debe de ingresar un valor en el campo: {tag.reportParm.Prompt}", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                return;
                            }
                        }
                    }



                }



                #endregion

                if (parameters.Count > 0)
                    parameters.Clear();


                foreach (FrameworkElement panel in GridMain.Children)
                {
                    if (panel is TextBlock) continue;


                    if (panel is ComboBoxAdv)
                    {
                        MultiTag tag = (MultiTag)((ComboBoxAdv)panel).Tag;

                        if (tag.MultiValue)
                        {
                            string val = "";
                            foreach (DataRowView ob in ((ComboBoxAdv)panel).SelectedItems)
                            {
                                String valueCta = ob[0].ToString();
                                val += valueCta + ",";
                            }
                            string ss = val.Trim().Substring(val.Trim().Length - 1);
                            if (ss == ",") val = val.Substring(0, val.Trim().Length - 1);

                            ReportParameter param = new ReportParameter(((ComboBoxAdv)panel).Name);
                            string[] values = val.Split(',');
                            param.Values.AddRange(values);
                            parameters.Add(param);
                        }
                        else
                        {
                            parameters.Add(new ReportParameter(((ComboBoxAdv)panel).Name, ((ComboBoxAdv)panel).SelectedValue.ToString()));
                        }

                    }

                    if (panel is TextBox)
                    {
                        MultiTag tag = (MultiTag)((TextBox)panel).Tag;
                        if (tag.MultiValue)
                        {
                            string cadena = ((TextBox)panel).Text;
                            ReportParameter param = new ReportParameter(((TextBox)panel).Name);
                            string[] values = cadena.Split(',');
                            param.Values.AddRange(values);
                            parameters.Add(param);
                        }
                        else
                        {
                            parameters.Add(new ReportParameter(((TextBox)panel).Name, ((TextBox)panel).Text));
                        }
                    }

                    if (panel is DatePicker)
                    {
                        parameters.Add(new ReportParameter(((DatePicker)panel).Name, ((DatePicker)panel).Text));
                    }

                    if (panel is CheckBox)
                    {
                        parameters.Add(new ReportParameter(((CheckBox)panel).Name, ((CheckBox)panel).IsChecked == true ? true.ToString() : false.ToString()));
                    }
                }



                TabMain.SelectedIndex = 2;
                TabMain.SelectedIndex = 1;

                viewer.ServerReport.ReportPath = report.ServerReport.ReportPath;
                viewer.ServerReport.ReportServerUrl = new Uri(report.ServerReport.ReportServerUrl.ToString());
                viewer.ProcessingMode = Microsoft.Reporting.WinForms.ProcessingMode.Remote;
                ReportServerCredentials rsCredentials = viewer.ServerReport.ReportServerCredentials;

                string UserServer = dtserver.Rows[0]["UserServer"].ToString().Trim();
                string UserServerPassword = dtserver.Rows[0]["UserServerPassword"].ToString().Trim();
                string UserSql = dtserver.Rows[0]["UserSql"].ToString().Trim();
                string UserSqlPassword = dtserver.Rows[0]["UserSqlPassword"].ToString().Trim();

                rsCredentials.NetworkCredentials = new System.Net.NetworkCredential(UserServer, UserServerPassword);
                List<Microsoft.Reporting.WinForms.DataSourceCredentials> crdentials = new List<Microsoft.Reporting.WinForms.DataSourceCredentials>();

                foreach (var dataSource in viewer.ServerReport.GetDataSources())
                {
                    Microsoft.Reporting.WinForms.DataSourceCredentials credn = new Microsoft.Reporting.WinForms.DataSourceCredentials();
                    credn.Name = dataSource.Name;
                    credn.UserId = UserSql;
                    credn.Password = UserSqlPassword;
                    crdentials.Add(credn);
                }


                viewer.ServerReport.SetDataSourceCredentials(crdentials);
                viewer.ServerReport.SetParameters(parameters);
                viewer.RefreshReport();


            }
            catch (Exception w)
            {
                MessageBox.Show("eror_" + w);
            }
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                SiaWin.Browse(SiaWin.Empresas);

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar:" + w);
            }
        }

        private async void BtnExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region validaciones

                if (string.IsNullOrEmpty(stored_p))
                {
                    MessageBox.Show("debe de ingresar un procedimiento almacenado relacionado al informe", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                #endregion

                sfBusyIndicator.IsBusy = true;

                //var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(stored_p));
                //await slowTask;

                //if (slowTask.Result.Tables[0].Rows.Count > 0)
                //{
                //  SiaWin.Browse(slowTask.Result.Tables[0]);



                DataTable dt = LoadData(stored_p).Tables[0];
                SiaWin.Browse(dt);

                //}
                //else
                //{
                //  MessageBox.Show("no existen registros", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                //}


                sfBusyIndicator.IsBusy = false;

            }
            catch (Exception w)
            {
                MessageBox.Show("error al exportar a excel:" + w);
            }
        }


        private DataSet LoadData(string stored)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand(stored, con);
                cmd.CommandType = CommandType.StoredProcedure;

                foreach (FrameworkElement panel in GridMain.Children)
                {
                    if (panel is TextBlock) continue;

                    if (panel is TextBox)
                    {
                        TextBox tx = ((TextBox)panel);
                        MultiTag tag = (MultiTag)tx.Tag;
                        cmd.Parameters.AddWithValue("@" + tag.parametros.parameter, tx.Text);
                    }

                    if (panel is DatePicker)
                    {
                        DatePicker date = ((DatePicker)panel);
                        MultiTag tag = (MultiTag)date.Tag;
                        cmd.Parameters.AddWithValue("@" + tag.parametros.parameter, date.Text);
                    }


                    if (panel is CheckBox)
                    {
                        CheckBox check = ((CheckBox)panel);
                        MultiTag tag = (MultiTag)check.Tag;
                        cmd.Parameters.AddWithValue("@" + tag.parametros.parameter, check.IsChecked);
                    }

                    if (panel is UpDown)
                    {
                        UpDown up = ((UpDown)panel);
                        MultiTag tag = (MultiTag)up.Tag;
                        cmd.Parameters.AddWithValue("@" + tag.parametros.parameter, up.Value);
                    }

                    if (panel is ComboBoxAdv)
                    {
                        ComboBoxAdv combo = ((ComboBoxAdv)panel);
                        MultiTag tag = (MultiTag)combo.Tag;

                        if (tag.MultiValue)
                        {

                            string val = "";
                            foreach (DataRowView ob in ((ComboBoxAdv)panel).SelectedItems)
                            {
                                String valueCta = ob[0].ToString();
                                val += valueCta + ",";
                            }
                            string ss = val.Trim().Substring(val.Trim().Length - 1);
                            if (ss == ",") val = val.Substring(0, val.Trim().Length - 1);

                            cmd.Parameters.AddWithValue("@" + tag.parametros.parameter, val);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@" + tag.parametros.parameter, combo.SelectedValue);
                        }

                    }
                }


                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds, "temp");
                con.Close();
                return ds;
            }
            catch (Exception e)
            {
                MessageBox.Show("error en loadData:" + e.Message);
                return null;
            }
        }



    }
}
