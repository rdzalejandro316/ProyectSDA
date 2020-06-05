using FoxPasarSql;
using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace SiasoftAppExt
{

    //Sia.PublicarPnt(9540,"FoxPasarSql");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9540, "FoxPasarSql");
    //ww.ShowInTaskbar=false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
    //ww.ShowDialog();        

    //pruebas--------------
    //Sia.PublicarPnt(9546,"FoxPasarSql");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9546, "FoxPasarSql");
    //ww.ShowInTaskbar=false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
    //ww.ShowDialog();   


    public partial class FoxPasarSql : Window
    {
        dynamic SiaWin;
        int idemp = 0;
        string cnEmp = "";

        DataTable dtRutas = new DataTable();

        DispatcherTimer disp = new DispatcherTimer();

        public FoxPasarSql()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            LoadConfig();
            loadRoot();
        }
        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        public void loadRoot()
        {
            dtRutas = SiaWin.Func.SqlDT("select * from root_fox", "rotas", 0);
            CbModulo.ItemsSource = dtRutas.DefaultView;
        }
        private void CbModulo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                string root = (sender as ComboBox).SelectedValue.ToString();
                loadCombo(root);
            }
            catch (Exception w) { MessageBox.Show("error en  el CbModulo_SelectionChanged:" + w); }

        }
        public void loadCombo(string root)
        {
            try
            {
                CbTableFox.ItemsSource = null;
                //string strCon = @"Provider=VFPOLEDB.1;Data Source=C:\\SDA\\SiasoftAppSDA\\DataSDA\\CO.SIA\\CO.SIA\\cod_DBF.DBC;";
                string strCon = @"Provider=VFPOLEDB.1;Data Source=" + root + ";";
                DataTable tableInfo;
                using (OleDbConnection con = new OleDbConnection(strCon))
                {
                    con.Open();
                    tableInfo = con.GetSchema("Tables");
                    con.Close();
                }

                CbTableFox.ItemsSource = tableInfo.DefaultView;
            }
            catch (Exception w)
            {
                MessageBox.Show("eror:" + w);
            }
        }
        public Tuple<string, string> getTable(string TableFox)
        {
            string query = "select  * from sql_fox where tablaFox='" + TableFox + "'";
            DataTable rutas = SiaWin.Func.SqlDT(query, "query", 0);

            var tuple = new Tuple<string, string>("", "");

            string select = ""; string tableSql = "";
            if (rutas.Rows.Count > 0)
            {
                select = rutas.Rows[0]["selecTable"].ToString();
                tableSql = rutas.Rows[0]["tablaSQL"].ToString();
            }

            tuple = new Tuple<string, string>(select, tableSql);
            return tuple;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CbTableFox.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione una tabla para consultar");
                    return;
                }

                string dir = CbModulo.SelectedValue.ToString();
                string tabla = CbTableFox.SelectedValue.ToString();

                var table = getTable(tabla);
                if (string.IsNullOrEmpty(table.Item2) || table.Item2 == "")
                {
                    MessageBox.Show("la tabla que esta consultando no se encuetra en la configuracion de sql_fox debes de ingresar la tabla y su configuracion");
                    return;
                }

                getConsulta(table.Item1, dir);


            }
            catch (SqlException w)
            {
                MessageBox.Show("error en SqlException:" + w);
            }
            catch (Exception w)
            {
                MessageBox.Show("error en Exception:" + w);
            }

        }

        public async void getConsulta(string sqlquery, string dir)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            TXrows.Text = "---";
            GridCon.ItemsSource = null;
            sfBusyIndicatorCons.IsBusy = true;
            var slowTask = Task<DataTable>.Factory.StartNew(() => load(sqlquery, dir, source.Token), source.Token);
            await slowTask;

            if (((DataTable)slowTask.Result).Rows.Count > 0)
            {
                GridCon.ItemsSource = ((DataTable)slowTask.Result).DefaultView;
                TXrows.Text = ((DataTable)slowTask.Result).Rows.Count.ToString();
            }

            sfBusyIndicatorCons.IsBusy = false;

        }

        public DataTable load(string sqlQuery, string dir, CancellationToken cancellationToken)
        {
            try
            {
                OleDbConnection oleDbConnection1 = new OleDbConnection("Provider=VFPOLEDB.1;Data Source=" + dir + ";");
                oleDbConnection1.Open();
                DataSet ds = new DataSet();
                //string sql = table.Item1;
                string sql = sqlQuery;
                OleDbDataAdapter da = new OleDbDataAdapter(sql, oleDbConnection1);
                da.Fill(ds, "miTabla");
                DataTable dt = ds.Tables[0];
                return dt;
            }
            catch (Exception w)
            {
                MessageBox.Show("error en la consulta:" + w);
                return null;
            }
        }


        //--------------------------- config
        public TagMultiple getTabla(string tableFox)
        {
            string select = "select * from sql_fox ";
            select += "inner join root_fox on sql_fox.idModu = root_fox.idModu ";
            select += "where tablaFox='" + tableFox + "' ";

            DataTable dataTable = SiaWin.Func.SqlDT(select, "selects", 0);

            TagMultiple MultInf = new TagMultiple()
            {
                tablaFox = dataTable.Rows[0]["tablaFox"].ToString(),
                tablaSql = dataTable.Rows[0]["tablaSQL"].ToString(),
                SelectFox = dataTable.Rows[0]["selecTable"].ToString(),
                InsertSql = dataTable.Rows[0]["inserTable"].ToString(),
                SelectCamp = dataTable.Rows[0]["selectCamp"].ToString(),
                root = dataTable.Rows[0]["rutaFox"].ToString(),
            };
            return MultInf;
        }
        private void BtnProcess_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (ToggleButton item in Panel.Children)
                {
                    if (item.IsChecked == true)
                    {
                        string tablaFOX = item.Tag.ToString();
                        string tablaSQL = item.Name.ToString();
                        TagMultiple values = getTabla(tablaFOX);

                        if (delete(tablaSQL) == true)
                        {
                            insert(tablaSQL, tablaFOX, values);
                        }

                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("ha ocurrido un error contacte con el administrador");
            }
        }


        public bool delete(string table)
        {
            try
            {
                string delete = "TRUNCATE TABLE " + table + "";
                //string cn = "Data Source=64.250.116.210,8334;Initial Catalog=SDA_Emp010;Persist Security Info=True;User ID=wilmer1104@yahoo.com;Password=Q1w2e3r4*/*;";
                string cn = cnEmp;

                System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(cn);
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = delete;
                cmd.Connection = conn;
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                //MessageBox.Show("Tabla:" + table + " Eliminada");
                NotificationOn("Tabla:" + table + " Eliminada");
                return true;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al eliminar tabla:" + w);
                return false;
            }
        }
        public async void insert(string tableSql, string tableFox, TagMultiple val)
        {
            try
            {
                //DataTable tabFox = GetFoxTable(val);
                //SiaWin.Browse(tabFox);                    

                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                sfBusyIndicator.IsBusy = true;
                //var slowTask = Task<Tuple<int, int, DataTable>>.Factory.StartNew(() => SqlCRUD(val, tabFox, source.Token), source.Token);
                var slowTask = Task<Tuple<int, int, DataTable, string, string>>.Factory.StartNew(() => SqlCRUD(val, source.Token), source.Token);
                await slowTask;

                int valorIns = 0;
                int valorFall = 0;
                string ini = "";
                string fin = "";

                DataTable dtFall = new DataTable();
                if (((Tuple<int, int, DataTable, string, string>)slowTask.Result).Item1 > 0)
                {
                    valorIns = ((Tuple<int, int, DataTable, string, string>)slowTask.Result).Item1;
                    valorFall = ((Tuple<int, int, DataTable, string, string>)slowTask.Result).Item2;
                    dtFall = ((Tuple<int, int, DataTable, string, string>)slowTask.Result).Item3;
                    ini = ((Tuple<int, int, DataTable, string, string>)slowTask.Result).Item4;
                    fin = ((Tuple<int, int, DataTable, string, string>)slowTask.Result).Item5;

                    Grid GridPrincipal = new Grid();
                    RowDefinition row1 = new RowDefinition() { Height = new GridLength(150, GridUnitType.Star) };
                    RowDefinition row2 = new RowDefinition() { Height = new GridLength(250, GridUnitType.Star) };
                    GridPrincipal.RowDefinitions.Add(row1);
                    GridPrincipal.RowDefinitions.Add(row2);

                    Grid Grid_1 = new Grid();
                    Grid.SetRow(Grid_1, 0);
                    ColumnDefinition colm1 = new ColumnDefinition() { Width = new GridLength(350, GridUnitType.Star) };
                    ColumnDefinition colm2 = new ColumnDefinition() { Width = new GridLength(350, GridUnitType.Star) };
                    ColumnDefinition colm3 = new ColumnDefinition() { Width = new GridLength(300, GridUnitType.Star) };
                    Grid_1.ColumnDefinitions.Add(colm1);
                    Grid_1.ColumnDefinitions.Add(colm2);
                    Grid_1.ColumnDefinitions.Add(colm3);


                    #region GRID COLUMN 1                        
                    Grid Grid_1_1 = new Grid();
                    RowDefinition row1_1 = new RowDefinition() { Height = new GridLength(100, GridUnitType.Star) };
                    RowDefinition row1_2 = new RowDefinition() { Height = new GridLength(100, GridUnitType.Star) };
                    Grid_1_1.RowDefinitions.Add(row1_1);
                    Grid_1_1.RowDefinitions.Add(row1_2);

                    TextBlock title_insert = new TextBlock() { Text = "Total Datos Insertados", Foreground = Brushes.Green, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, FontSize = 18, FontWeight = FontWeights.Bold };
                    Grid.SetRow(title_insert, 0);
                    Grid_1_1.Children.Add(title_insert);

                    TextBlock val_insert = new TextBlock() { Text = valorIns.ToString(), Foreground = Brushes.Green, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Center, FontSize = 16 };
                    Grid.SetRow(val_insert, 1);
                    Grid_1_1.Children.Add(val_insert);
                    #endregion
                    #region GRID COLUMN 2                        
                    Grid Grid_1_2 = new Grid();
                    RowDefinition row2_1 = new RowDefinition() { Height = new GridLength(100, GridUnitType.Star) };
                    RowDefinition row2_2 = new RowDefinition() { Height = new GridLength(100, GridUnitType.Star) };
                    Grid_1_2.RowDefinitions.Add(row2_1);
                    Grid_1_2.RowDefinitions.Add(row2_2);
                    Grid.SetColumn(Grid_1_2, 1);

                    TextBlock title_fall = new TextBlock() { Text = "Total Datos Fallidos", Foreground = Brushes.Red, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, FontSize = 18, FontWeight = FontWeights.Bold };
                    Grid.SetRow(title_fall, 0);
                    Grid_1_2.Children.Add(title_fall);

                    TextBlock val_fall = new TextBlock() { Text = valorFall.ToString(), Foreground = Brushes.Red, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Center, FontSize = 16 };
                    Grid.SetRow(val_fall, 1);
                    Grid_1_2.Children.Add(val_fall);
                    #endregion
                    #region GRID COLUMN 3
                    Grid Grid_1_3 = new Grid();
                    RowDefinition row3_1 = new RowDefinition() { Height = new GridLength(100, GridUnitType.Star) };
                    RowDefinition row3_2 = new RowDefinition() { Height = new GridLength(100, GridUnitType.Star) };
                    Grid_1_3.RowDefinitions.Add(row3_1);
                    Grid_1_3.RowDefinitions.Add(row3_2);
                    Grid.SetColumn(Grid_1_3, 2);

                    TextBlock tx_inici = new TextBlock() { Text = "Inicio: "+ini, Foreground = Brushes.DodgerBlue, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, FontSize = 16, FontWeight = FontWeights.DemiBold };
                    Grid.SetRow(tx_inici, 0);
                    Grid_1_3.Children.Add(tx_inici);

                    TextBlock tx_fin = new TextBlock() { Text = "Fin: "+fin, Foreground = Brushes.DodgerBlue, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Center, FontSize = 16, FontWeight = FontWeights.DemiBold };
                    Grid.SetRow(tx_fin, 1);
                    Grid_1_3.Children.Add(tx_fin);
                    #endregion

                    Grid_1.Children.Add(Grid_1_1);
                    Grid_1.Children.Add(Grid_1_2);
                    Grid_1.Children.Add(Grid_1_3);
                    #region tabla de fallidos
                    Grid Grid_2 = new Grid();
                    Grid.SetRow(Grid_2, 1);

                    SfDataGrid data = new SfDataGrid() { AutoGenerateColumns = true, AllowFiltering = true, ItemsSource = dtFall.DefaultView };
                    Grid_2.Children.Add(data);
                    #endregion


                    GridPrincipal.Children.Add(Grid_1);
                    GridPrincipal.Children.Add(Grid_2);


                    TabItem tabItemExt = new TabItem();
                    tabItemExt.Header = tableFox;
                    tabItemExt.Content = GridPrincipal;
                    Tab_resul.Items.Add(tabItemExt);
                }

                sfBusyIndicator.IsBusy = false;

            }
            catch (Exception w)
            {
                MessageBox.Show("error en el insert:" + w);
            }
        }


        #region anterior
        //public Tuple<int, int, DataTable> SqlCRUD(TagMultiple MulVal, CancellationToken cancellationToken)
        //{

        //    DataTable tabFox = GetFoxTable(MulVal);

        //    System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(cnEmp);
        //    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
        //    conn.Open();
        //    int numberInsert = 0;
        //    int numberFall = 0;
        //    string ListSelect = MulVal.SelectCamp;
        //    List<string> listaSel = new List<string>(ListSelect.Split(','));

        //    DataTable dtfallidos = new DataTable();

        //    for (int i = 0; i < listaSel.Count; i++)
        //        dtfallidos.Columns.Add(listaSel[i]);


        //    var tuple = new Tuple<int, int, DataTable>(numberInsert, numberFall, dtfallidos);


        //    cmd.CommandText = MulVal.InsertSql;

        //    foreach (System.Data.DataRow row in tabFox.Rows)
        //    {
        //        for (int i = 0; i < listaSel.Count; i++)
        //            cmd.Parameters.AddWithValue("@" + listaSel[i], row[listaSel[i]]);

        //        cmd.Connection = conn;
        //        try { cmd.ExecuteNonQuery(); numberInsert++; cmd.Parameters.Clear(); }
        //        catch (Exception)
        //        {
        //            numberFall++;

        //            System.Data.DataRow dtrow = dtfallidos.NewRow();
        //            for (int i = 0; i < listaSel.Count; i++)
        //                dtrow[i] = row[listaSel[i]];

        //            dtfallidos.Rows.Add(dtrow);
        //            cmd.Parameters.Clear();
        //        }

        //        //Tx_Total.Text = numberInsert.ToString();
        //    }
        //    conn.Close();
        //    tuple = new Tuple<int, int, DataTable>(numberInsert, numberFall, dtfallidos);
        //    return tuple;
        //}
        //public DataTable GetFoxTable(TagMultiple table)
        //{
        //    string dir = table.root;
        //    OleDbConnection oleDbConnection1 = new OleDbConnection("Provider=VFPOLEDB.1;Data Source=" + dir + ";");
        //    oleDbConnection1.Open();
        //    DataSet ds = new DataSet();
        //    string sql = table.SelectFox;
        //    OleDbDataAdapter da = new OleDbDataAdapter(sql, oleDbConnection1);
        //    da.Fill(ds, "miTabla");
        //    oleDbConnection1.Close();
        //    oleDbConnection1.Dispose();
        //    DataTable dt = ds.Tables["miTabla"];
        //    //SiaWin.Browse(dt);
        //    return dt;
        //}
        #endregion

        public Tuple<int, int, DataTable, string, string> SqlCRUD(TagMultiple MulVal, CancellationToken cancellationToken)
        {

            //OleDbDataReader tabFox = GetFoxTable(MulVal);
            string dir = MulVal.root;
            OleDbConnection oleDbConnection1 = new OleDbConnection("Provider=VFPOLEDB.1;Data Source=" + dir + ";");
            oleDbConnection1.Open();
            string sql = MulVal.SelectFox;
            OleDbDataReader dr = new OleDbCommand(sql, oleDbConnection1).ExecuteReader();


            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(cnEmp);
            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
            conn.Open();
            int numberInsert = 0;
            int numberFall = 0;
            string inicio = DateTime.Now.ToString();
            string fin = "";

            string ListSelect = MulVal.SelectCamp;
            List<string> listaSel = new List<string>(ListSelect.Split(','));

            DataTable dtfallidos = new DataTable();

            for (int i = 0; i < listaSel.Count; i++)
                dtfallidos.Columns.Add(listaSel[i]);


            var tuple = new Tuple<int, int, DataTable, string, string>(numberInsert, numberFall, dtfallidos, inicio, fin);


            cmd.CommandText = MulVal.InsertSql;

            while (dr.Read())
            {
                for (int i = 0; i < listaSel.Count; i++)
                    //cmd.Parameters.AddWithValue("@" + listaSel[i], row[listaSel[i]]);
                    cmd.Parameters.AddWithValue("@" + listaSel[i], dr.GetValue(i));

                cmd.Connection = conn;
                try { cmd.ExecuteNonQuery(); numberInsert++; cmd.Parameters.Clear(); }
                catch (Exception)
                {
                    numberFall++;

                    System.Data.DataRow dtrow = dtfallidos.NewRow();
                    for (int i = 0; i < listaSel.Count; i++)
                        dtrow[i] = dr.GetValue(i);

                    dtfallidos.Rows.Add(dtrow);
                    cmd.Parameters.Clear();
                }

                //Tx_Total.Text = numberInsert.ToString();
            }
            conn.Close();
            fin = DateTime.Now.ToString();
            tuple = new Tuple<int, int, DataTable, string, string>(numberInsert, numberFall, dtfallidos, inicio, fin);
            return tuple;
        }
        public OleDbDataReader GetFoxTable(TagMultiple table)
        {
            string dir = table.root;
            OleDbConnection oleDbConnection1 = new OleDbConnection("Provider=VFPOLEDB.1;Data Source=" + dir + ";");
            oleDbConnection1.Open();
            //DataSet ds = new DataSet();
            string sql = table.SelectFox;
            OleDbDataReader dr = new OleDbCommand(sql, oleDbConnection1).ExecuteReader();
            oleDbConnection1.Close();
            //OleDbDataAdapter da = new OleDbDataAdapter(sql, oleDbConnection1);
            //da.Fill(ds, "miTabla");
            //oleDbConnection1.Close();
            //oleDbConnection1.Dispose();
            //DataTable dt = ds.Tables["miTabla"];
            //SiaWin.Browse(dt);
            return dr;
        }

        public void get()
        {

            string cn = "Data Source=64.250.116.210,8334;Initial Catalog=SDA_Emp010;Persist Security Info=True;User ID=wilmer1104@yahoo.com;Password=Q1w2e3r4*/*;";
            DataTable dt = new DataTable("tabla1");
            try
            {
                using (SqlConnection sqlCon = new System.Data.SqlClient.SqlConnection(cn))
                {
                    using (SqlDataAdapter SqlDa = new SqlDataAdapter("SELECT * FROM Comae_cta", sqlCon))
                    {
                        SqlDa.Fill(dt);
                    }
                }
            }
            catch (SqlException SQLex)
            {
                MessageBox.Show(SQLex.Message);
                dt = null;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                dt = null;
            }

            MessageBox.Show("dt:" + dt.Rows.Count);
        }

        private void SnackbarMessage_ActionClick(object sender, RoutedEventArgs e)
        {
            Notificaction.IsActive = false;
        }

        public void NotificationOn(string messaage, int tipo = 0)
        {
            Notificaction.IsActive = true;
            NotiMessa.Content = messaage.Trim();
            disp.Interval = TimeSpan.FromMilliseconds(2000);
            disp.Tick += (sender, args) =>
            {
                Notificaction.IsActive = false;
                disp.Stop();
            };
            disp.Start();

            if (tipo == 0) disp.Start();
        }

        private void Config_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Config ven = new Config();
                ven.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                ven.ShowInTaskbar = false;
                ven.Owner = Application.Current.MainWindow;
                ven.ShowDialog();
            }
            catch (Exception w)
            {
                MessageBox.Show("error click:" + w);
            }
        }

        private void BtnExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;
                //SfDataGrid sfdg = new SfDataGrid();

                var excelEngine = GridCon.ExportToExcel(GridCon.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];
                workBook.Worksheets[0].AutoFilters.FilterRange = workBook.Worksheets[0].UsedRange;


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
                            workBook.Version = ExcelVersion.Excel97to2003;
                        else if (sfd.FilterIndex == 2)
                            workBook.Version = ExcelVersion.Excel2010;
                        else
                            workBook.Version = ExcelVersion.Excel2013;
                        workBook.SaveAs(stream);
                    }

                    //Message box confirmation to view the created workbook.
                    if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archvo",
                                        MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        //Launching the Excel file using the default Application.[MS Excel Or Free ExcelViewer]
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error en la exportacion" + w);
            }

        }






    }
}


