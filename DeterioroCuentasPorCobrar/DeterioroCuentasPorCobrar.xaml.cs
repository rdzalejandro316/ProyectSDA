using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
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

    //Sia.PublicarPnt(9690,"DeterioroCuentasPorCobrar");
    //Sia.TabU(9690);

    public partial class DeterioroCuentasPorCobrar : UserControl
    {
        dynamic SiaWin;
        dynamic tabitem;
        public int idemp = 0;
        string cnEmp = "";
        string codemp = string.Empty;
        int idmodulo = 1;


        public DataTable dt_consulta = new DataTable();
        DataTable dt_importar = new DataTable();
        DataTable dt_errores = new DataTable();

        public DeterioroCuentasPorCobrar(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            tabitem = tabitem1;
            tabitem.Title = "Deterioro de cuentas por cobrar";
            tabitem.Logo(9, ".png");
            tabitem.MultiTab = false;
            if (tabitem.idemp > 0) idemp = tabitem.idemp;
            LoadConfig();

            dt_errores.Columns.Add("error");
        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                codemp = foundRow["BusinessCode"].ToString().Trim();
                tabitem.Logo(idLogo, ".png");
                tabitem.Title = "Deterioro de cartera (" + aliasemp + ")";

                DataTable Cuentas = SiaWin.Func.SqlDT("SELECT rtrim(cod_cta) as cod_cta,rtrim(cod_cta)+'('+rtrim(nom_cta)+')' as nom_cta FROM COMAE_CTA WHERE ind_mod = 1 and (tip_apli = 3 or tip_apli = 4 ) ORDER BY COD_CTA", "Cuentas", idemp);
                comboBoxCuentas.ItemsSource = Cuentas.DefaultView;


                FechaIni.Text = DateTime.Now.ToShortDateString();

            }
            catch (Exception e)
            {
                SiaWin.seguridad.ErrorLog("Error  ", "AnalisisDeCartera-LoadConfig:" + e.Message.ToString());
                MessageBox.Show(e.Message);
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                string tag = (sender as TextBox).Tag.ToString();

                if (e.Key == Key.F8)
                {
                    string tabla = "", codigo = "", nombre = "", idrow = "idrow", titulo = "";
                    switch (tag)
                    {
                        case "comae_ter":
                            tabla = tag; codigo = "cod_ter"; nombre = "nom_ter"; titulo = "maestra de terceros";
                            break;
                            //case "comae_cta":
                            //    tabla = tag; codigo = "cod_cta"; nombre = "nom_cta"; titulo = "maestra de cuentas";
                            //    break;
                    }

                    dynamic winb = SiaWin.WindowBuscar(tabla, codigo, nombre, codigo, idrow, titulo, cnEmp, false, "", idEmp: idemp);
                    winb.ShowInTaskbar = false;
                    winb.Owner = Application.Current.MainWindow;
                    winb.Height = 400;
                    winb.ShowDialog();
                    int id = winb.IdRowReturn;
                    string code = winb.Codigo;
                    string nom = winb.Nombre;

                    if (!string.IsNullOrEmpty(code))
                    {
                        (sender as TextBox).Text = code;
                        TextNombreTercero.Text = nom;
                    }
                    else
                    {
                        (sender as TextBox).Text = "";
                        TextNombreTercero.Text = "";
                    }
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir buscar:" + w);
            }
        }

        private void TextCod_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                string tag = (sender as TextBox).Tag.ToString();
                string valor = (sender as TextBox).Text.Trim();

                string tabla = "", codigo = "", nombre = "", titulo = "";
                switch (tag)
                {
                    case "comae_ter":
                        tabla = tag; codigo = "cod_ter"; nombre = "nom_ter"; titulo = "maestra de terceros";
                        break;
                }

                DataTable dt = SiaWin.Func.SqlDT("select * from " + tabla + "  where " + codigo + "='" + valor + "';", "temp", idemp);
                if (dt.Rows.Count <= 0)
                {
                    MessageBox.Show("el codigo " + valor + " no es valido en la " + titulo, "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    (sender as TextBox).Text = "";
                    TextNombreTercero.Text = "";
                }
                else
                {
                    TextNombreTercero.Text = dt.Rows[0][nombre].ToString().Trim();
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error en lostfocus:" + w);
            }
        }

        private async void BtnConsultar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                #region validaciones
                if (comboBoxCuentas.SelectedIndex < 0)
                {
                    MessageBox.Show("debe de ingresar una cuenta para consultar", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }


                #endregion

                PanelA.IsEnabled = false;
                PanelB.IsEnabled = false;
                PanelC.IsEnabled = false;
                sfBusyIndicator.IsBusy = true;

                string fecha = FechaIni.Text.Trim();
                string Tercero = TxCodTer.Text.Trim();
                string Cta = "";
                string storedprocedure = "_empSpCoAnalisisCxcDeterioroCartera";
                if (comboBoxCuentas.SelectedIndex >= 0)
                {
                    foreach (DataRowView ob in comboBoxCuentas.SelectedItems)
                    {
                        String valueCta = ob["cod_cta"].ToString();
                        Cta += valueCta + ",";
                    }
                    string ss = Cta.Trim().Substring(Cta.Trim().Length - 1);
                    if (ss == ",") Cta = Cta.Substring(0, Cta.Trim().Length - 1);
                }


                var slowTask = Task<DataTable>.Factory.StartNew(() => LoadData(fecha, Cta, Tercero, storedprocedure));
                await slowTask;

                if (slowTask.IsCompleted)
                {


                    if (slowTask.Result.Rows.Count > 0)
                    {
                        dt_consulta = slowTask.Result;
                        dataGridCxC.ItemsSource = dt_consulta.DefaultView;
                        TxRegistros.Text = dt_consulta.Rows.Count.ToString();
                    }
                    else
                    {
                        dataGridCxC.ItemsSource = null;
                        TxRegistros.Text = "0";
                    }
                }

                PanelA.IsEnabled = true;
                PanelB.IsEnabled = true;
                PanelC.IsEnabled = true;
                sfBusyIndicator.IsBusy = false;

            }
            catch (Exception w)
            {
                MessageBox.Show("erro al consutar:" + w);
            }
        }

        private DataTable LoadData(string Fi, string ctas, string cter, string storeprocedure)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataTable dt = new DataTable();
                //cmd = new SqlCommand("_empSpCoAnalisisCxcDeterioroCartera", con);
                cmd = new SqlCommand(storeprocedure, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@cod_cta", ctas);
                cmd.Parameters.AddWithValue("@cod_ter", cter);
                cmd.Parameters.AddWithValue("@fec_con", Fi);
                cmd.Parameters.AddWithValue("@codemp", "010");
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(dt);
                con.Close();

                if (dt.Rows.Count > 0)
                {
                    dt.Columns.Add("valarch", typeof(decimal));
                    dt.Columns.Add("det_arch", typeof(decimal));
                    dt.Columns.Add("difer", typeof(decimal));

                    foreach (DataRow item in dt.Rows)
                    {
                        item["valarch"] = 0;
                        item["det_arch"] = 0;
                        item["difer"] = 0;
                    }
                }

                return dt;
            }
            catch (Exception e)
            {
                SiaWin.seguridad.ErrorLog("Error  ", "AnalisisDeCartera-LoadData:" + e.Message.ToString());
                MessageBox.Show(e.Message);
                return null;
            }
        }



        private void BtnExportar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;
                var excelEngine = dataGridCxC.ExportToExcel(dataGridCxC.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];

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

                    if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archvo", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al exportar:" + w);
            }
        }

        private void BtnImprimir_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnPlantillas_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.DefaultExt = ".xlsx";
                saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
                saveFileDialog.Title = "Guardar Plantilla como...";
                saveFileDialog.ShowDialog();
                string ruta = saveFileDialog.FileName;

                if (string.IsNullOrEmpty(ruta)) return;

                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    IApplication application = excelEngine.Excel;
                    application.DefaultVersion = ExcelVersion.Excel2010;

                    IWorkbook workbook = application.Workbooks.Create(1);
                    IWorksheet worksheet = workbook.Worksheets[0];


                    worksheet.IsGridLinesVisible = true;

                    worksheet.Range["A1"].Text = "COD_TER";
                    worksheet.Range["B1"].Text = "COD_CTA";
                    worksheet.Range["C1"].Text = "DOC_MOV";
                    worksheet.Range["D1"].Text = "VALORPRES";
                    worksheet.Range["A1:D1"].CellStyle.Font.Bold = true;

                    if (string.IsNullOrEmpty(ruta))
                        MessageBox.Show("Por favor, seleccione una ruta para guardar la plantilla");
                    else
                    {
                        workbook.SaveAs(ruta);
                        MessageBox.Show("Documento Guardado");
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al guardar:" + w);
            }
        }


        public static System.Data.DataTable ConvertExcelToDataTable(string FileName)
        {
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                if (!application.IsSupported(FileName))
                {
                    MessageBox.Show("el tipo de extencion .xls no se admite por favor actualizarlo a .xlsx", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return null;
                }

                application.DefaultVersion = ExcelVersion.Excel2013;
                IWorkbook workbook = application.Workbooks.Open(FileName);
                IWorksheet worksheet = workbook.Worksheets[0];
                System.Data.DataTable customersTable = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ColumnNames);
                return customersTable;
            }
        }

        public bool validarArchioExcel(DataTable dt)
        {
            bool flag = true;

            if (dt.Columns.Contains("COD_TER") == false || dt.Columns.IndexOf("COD_TER") != 0) flag = false;
            if (dt.Columns.Contains("COD_CTA") == false || dt.Columns.IndexOf("COD_CTA") != 1) flag = false;
            if (dt.Columns.Contains("DOC_MOV") == false || dt.Columns.IndexOf("DOC_MOV") != 2) flag = false;
            if (dt.Columns.Contains("VALORPRES") == false || dt.Columns.IndexOf("VALORPRES") != 3) flag = false;
            return flag;
        }

        private async void BtnImportar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                OpenFileDialog openfile = new OpenFileDialog();
                openfile.DefaultExt = ".xlsx";
                openfile.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
                var browsefile = openfile.ShowDialog();
                string root = openfile.FileName;

                if (string.IsNullOrEmpty(root)) return;
                sfBusyIndicator.IsBusy = true;

                dt_importar = ConvertExcelToDataTable(root);
                if (dt_importar == null) { sfBusyIndicator.IsBusy = false; return; }

                if (validarArchioExcel(dt_importar) == false)
                {
                    MessageBox.Show("La plantilla importada no corresponde a la que permite el sistema por favor verifique con la plantilla que genera esta pantalla", "alerta", MessageBoxButton.OK, MessageBoxImage.Error);
                    sfBusyIndicator.IsBusy = false;

                    TxCodCta.Text = "";
                    TxNomCta.Text = "";
                    TxRegistros.Text = "0";
                    return;
                }

                PanelA.IsEnabled = false;
                PanelB.IsEnabled = false;
                sfBusyIndicator.IsBusy = true;


                var slowTask = Task.Factory.StartNew(() => Distribuir(dt_importar));
                await slowTask;

                MessageBox.Show(Application.Current.MainWindow, "Importacion Exitosa", "alerta", MessageBoxButton.OK, MessageBoxImage.Information);

                TxImportados.Text = dt_importar.Rows.Count.ToString();
                Tx_errores.Text = dt_errores.Rows.Count.ToString();

                PanelA.IsEnabled = true;
                PanelB.IsEnabled = true;
                sfBusyIndicator.IsBusy = false;

            }
            catch (ArgumentException w)
            {
                MessageBox.Show("ArgumentException:" + w);
                sfBusyIndicator.IsBusy = false;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al importar:" + w);
                sfBusyIndicator.IsBusy = false;
            }
        }

        private void Distribuir(DataTable dt)
        {
            try
            {


                foreach (System.Data.DataRow dr in dt.Rows)
                {

                    #region  valor

                    string valorpres = dr["VALORPRES"].ToString().Trim();
                    decimal _valorpres;
                    if (!string.IsNullOrEmpty(valorpres))
                    {
                        if (decimal.TryParse(valorpres, out _valorpres) == false)
                        {
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el valor a deteriorar debe de ser numerico"; dt_errores.Rows.Add(row);
                        }
                        else
                        {
                            string cod_ter = dr["COD_TER"].ToString().Trim();
                            string cod_cta = dr["COD_CTA"].ToString().Trim();
                            string doc_mov = dr["DOC_MOV"].ToString().Trim();

                            DataRow dr_find = dt_consulta.Select("cod_ter='" + cod_ter + "' and cod_cta='" + cod_cta + "' and doc_ref='" + doc_mov + "' ").FirstOrDefault();
                            if (dr_find != null)
                            {

                                dr_find["valarch"] = _valorpres;
                                decimal saldo = Convert.ToDecimal(dr_find["saldo"]);
                                decimal deterioro = Convert.ToDecimal(dr_find["deterioro"]);
                                decimal det_arch = saldo - _valorpres;
                                dr_find["det_arch"] = det_arch;
                                dr_find["difer"] = det_arch - deterioro;

                            }
                        }
                    }

                    #endregion

                }

            }
            catch (Exception e)
            {
                MessageBox.Show("en la distribucion:" + e.Message);
            }
        }


        #region ajuste niif esto si importan el excel

        private void BtnCrearAjusteNiif_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region validacion

                if (dataGridCxC.ItemsSource == null || dataGridCxC.View.Records.Count <= 0)
                {
                    MessageBox.Show("no hay datos para importar", "alerta", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                if (dt_errores.Rows.Count > 0)
                {
                    MessageBox.Show("la importacion contiene errores debe de estar todo correcto", "alerta", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                };


                if (dt_importar.Rows.Count <= 0)
                {
                    MessageBox.Show("debe de importar por lo menos un deterioro a realizar", "alerta", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                };

                #endregion


                #region insercion

                int idreg = DocumentoAjusteNiif();
                if (idreg > 0)
                {
                    SiaWin.TabTrn(0, idemp, true, idreg, idmodulo, WinModal: true);

                    dataGridCxC.ItemsSource = null;
                    dt_consulta.Clear();
                    dt_importar.Clear();
                    dt_errores.Clear();
                    TxRegistros.Text = "0";
                    TxImportados.Text = "0";
                    Tx_errores.Text = "0";
                    TxCodCta.Text = "";
                    TxNomCta.Text = "";

                    Tx30.Text = "---";
                    Tx60.Text = "---";
                    Tx90.Text = "---";
                    Tx120.Text = "---";
                    Tx150.Text = "---";
                    Tx180.Text = "---";
                    Tx360.Text = "---";
                    Txm360.Text = "---";
                }

                #endregion



            }
            catch (Exception w)
            {
                MessageBox.Show("errro al generar el documento:" + w);
            }
        }

        public int DocumentoAjusteNiif()
        {
            int idreg = -1;

            if (MessageBox.Show("Usted desea generar los documentos de la importados realizada?", "Documentos", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                string sql_cab = ""; string sql_cue = "";

                if (dt_consulta.Rows.Count > 0)
                {

                    using (SqlConnection connection = new SqlConnection(cnEmp))
                    {

                        connection.Open();
                        SqlCommand command = connection.CreateCommand();
                        SqlTransaction transaction = connection.BeginTransaction("Transaction");
                        command.Connection = connection;
                        command.Transaction = transaction;

                        string cod_trn = "80";
                        string fec_trn = DateTime.Now.ToString();


                        string sqlConsecutivo = "declare @fecdoc as datetime;";
                        sqlConsecutivo += "update Comae_trn set num_act=num_act+1 where cod_trn='" + cod_trn + "';";
                        sqlConsecutivo += "set @fecdoc = getdate();declare @ini as char(4);declare @num as varchar(12);declare @iConsecutivo char(12)='';";
                        sqlConsecutivo += "declare @iFolioHost int = 0;";
                        sqlConsecutivo += "SELECT @iFolioHost=num_act,@ini=rtrim(inicial) FROM comae_trn WHERE cod_trn='" + cod_trn + "' set @num=@iFolioHost;";
                        sqlConsecutivo += "select @iConsecutivo=rtrim(@ini)+rtrim(@iFolioHost)";


                        sql_cab += sqlConsecutivo + @"INSERT INTO cocab_doc (cod_trn,num_trn,fec_trn,detalle) values ('" + cod_trn + "',@iConsecutivo,'" + fec_trn + "','DETERIORO CUENTAS POR COBRAR');DECLARE @NewID INT;SELECT @NewID = SCOPE_IDENTITY();";


                        foreach (System.Data.DataRow dt in dt_consulta.Rows)
                        {
                            string cod_cta = dt["cod_cta"].ToString().Trim();

                            string cta_gdet = dt["cta_gdet"].ToString().Trim();
                            string cta_det = dt["cta_det"].ToString().Trim();

                            string cod_ter = dt["cod_ter"].ToString().Trim();
                            string doc_mov = dt["doc_mov"].ToString().Trim();
                            string des_mov = "Deterioro Cartera - " + doc_mov;

                            double difer = Convert.ToDouble(dt["difer"]);
                            double valor = Convert.ToDouble(dt["det_arch"]) < 0 ? difer * -1 : difer;

                            if (valor > 0)
                            {
                                sql_cue += @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,des_mov,bas_mov,deb_mov,cre_mov) values (@NewID,'" + cod_trn + "',@iConsecutivo,'" + cta_gdet + "','" + cod_ter + "','" + des_mov + "',0," + valor.ToString("F", CultureInfo.InvariantCulture) + ",0);";

                                sql_cue += @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,des_mov,bas_mov,deb_mov,cre_mov) values (@NewID,'" + cod_trn + "',@iConsecutivo,'" + cta_det + "','" + cod_ter + "','" + des_mov + "',0,0," + valor.ToString("F", CultureInfo.InvariantCulture) + ");";
                            }

                        }

                        command.CommandText = sql_cab + sql_cue + @"select CAST(@NewId AS int);";
                        var r = new object();
                        r = command.ExecuteScalar();
                        idreg = Convert.ToInt32(r);
                        transaction.Commit();
                        connection.Close();
                    }

                }

            }

            return idreg;
        }

        #endregion

        private async void BtnCrearRecup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region validacion

                if (string.IsNullOrEmpty(FechaIni.Text))
                {
                    MessageBox.Show("debe de ingresar una fecha a consultar", "alerta", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                };


                if (dataGridCxC.ItemsSource == null)
                {
                    MessageBox.Show("no se ha generado ninguna consulta", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                #endregion


                #region trae la temporal de recuperacion


                PanelA.IsEnabled = false;
                PanelB.IsEnabled = false;
                PanelC.IsEnabled = false;
                PanelGrid.IsEnabled = false;
                sfBusyIndicator.IsBusy = true;

                string fecha = FechaIni.Text.Trim();
                string Tercero = TxCodTer.Text.Trim();
                string Cta = "";
                string storeprocedure = "_empSpCoAnalisisCxcDeterioroCarteraRecupera";
                if (comboBoxCuentas.SelectedIndex >= 0)
                {
                    foreach (DataRowView ob in comboBoxCuentas.SelectedItems)
                    {
                        String valueCta = ob["cod_cta"].ToString();
                        Cta += valueCta + ",";
                    }
                    string ss = Cta.Trim().Substring(Cta.Trim().Length - 1);
                    if (ss == ",") Cta = Cta.Substring(0, Cta.Trim().Length - 1);
                }


                var slowTask = Task<DataTable>.Factory.StartNew(() => LoadData(fecha, Cta, Tercero, storeprocedure));
                await slowTask;

                if (slowTask.Result.Rows.Count > 0)
                {
                    var message = MessageBox.Show("Existen deterioros de cartera por recuerar, desea generar documento ????", "alerta", MessageBoxButton.YesNo, MessageBoxImage.Information);

                    if (message == MessageBoxResult.Yes)
                    {

                        #region insercion

                        int idreg = DocumentoRecuperacion(slowTask.Result);
                        if (idreg > 0)
                        {
                            SiaWin.TabTrn(0, idemp, true, idreg, idmodulo, WinModal: true);

                            dataGridCxC.ItemsSource = null;
                            dt_consulta.Clear();
                            dt_importar.Clear();
                            dt_errores.Clear();
                            TxRegistros.Text = "0";
                            TxImportados.Text = "0";
                            Tx_errores.Text = "0";
                            TxCodCta.Text = "";
                            TxNomCta.Text = "";

                            Tx30.Text = "---";
                            Tx60.Text = "---";
                            Tx90.Text = "---";
                            Tx120.Text = "---";
                            Tx150.Text = "---";
                            Tx180.Text = "---";
                            Tx360.Text = "---";
                            Txm360.Text = "---";
                        }

                        #endregion

                    }
                }
                else
                {
                    MessageBox.Show("no existe deterioros para recuperar", "alerta", MessageBoxButton.OK, MessageBoxImage.Information);
                }


                PanelGrid.IsEnabled = true;
                PanelA.IsEnabled = true;
                PanelB.IsEnabled = true;
                PanelC.IsEnabled = true;
                sfBusyIndicator.IsBusy = false;

                #endregion



            }
            catch (Exception w)
            {
                MessageBox.Show("errro al generar el documento BtnCrearRecup_Click:" + w);
            }
        }


        public int DocumentoRecuperacion(DataTable dt_recu)
        {
            int idreg = -1;

            string sql_cab = ""; string sql_cue = "";

            if (dt_recu.Rows.Count > 0)
            {

                using (SqlConnection connection = new SqlConnection(cnEmp))
                {

                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    SqlTransaction transaction = connection.BeginTransaction("Transaction");
                    command.Connection = connection;
                    command.Transaction = transaction;

                    string cod_trn = "80A";
                    string fec_trn = DateTime.Now.ToString();


                    string sqlConsecutivo = "declare @fecdoc as datetime;";
                    sqlConsecutivo += "update Comae_trn set num_act=num_act+1 where cod_trn='" + cod_trn + "';";
                    sqlConsecutivo += "set @fecdoc = getdate();declare @ini as char(4);declare @num as varchar(12);declare @iConsecutivo char(12)='';";
                    sqlConsecutivo += "declare @iFolioHost int = 0;";
                    sqlConsecutivo += "SELECT @iFolioHost=num_act,@ini=rtrim(inicial) FROM comae_trn WHERE cod_trn='" + cod_trn + "' set @num=@iFolioHost;";
                    sqlConsecutivo += "select @iConsecutivo=rtrim(@ini)+rtrim(@iFolioHost)";


                    sql_cab += sqlConsecutivo + @"INSERT INTO cocab_doc (cod_trn,num_trn,fec_trn,detalle,_usu) values ('" + cod_trn + "',@iConsecutivo,'" + fec_trn + "','Recuperación Deterioro Cartera','" + SiaWin._UserName + "');DECLARE @NewID INT;SELECT @NewID = SCOPE_IDENTITY();";


                    foreach (System.Data.DataRow dt in dt_recu.Rows)
                    {
                        string cod_cta = dt["cod_cta"].ToString().Trim();
                        string cta_recu = dt["cta_recu"].ToString().Trim();
                        string cod_ter = dt["cod_ter"].ToString().Trim();
                        string doc_mov = dt["doc_mov"].ToString().Trim();
                        string des_mov = "Recuperación Deterioro Cartera:" + doc_mov;


                        double saldo = Convert.ToDouble(dt["saldo_positivo"]);

                        if (saldo > 0)
                        {
                            sql_cue += @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,des_mov,bas_mov,deb_mov,cre_mov,doc_mov) values (@NewID,'" + cod_trn + "',@iConsecutivo,'" + cod_cta + "','" + cod_ter + "','" + des_mov + "',0," + saldo.ToString("F", CultureInfo.InvariantCulture) + ",0,'" + doc_mov + "');";

                            sql_cue += @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,des_mov,bas_mov,deb_mov,cre_mov,doc_mov) values (@NewID,'" + cod_trn + "',@iConsecutivo,'" + cta_recu + "','" + cod_ter + "','" + des_mov + "',0,0," + saldo.ToString("F", CultureInfo.InvariantCulture) + ",'"+doc_mov+"');";
                        }

                    }

                    command.CommandText = sql_cab + sql_cue + @"select CAST(@NewId AS int);";
                    var r = new object();
                    r = command.ExecuteScalar();
                    idreg = Convert.ToInt32(r);
                    transaction.Commit();
                    connection.Close();
                }

            }

            return idreg;
        }


        private void BtnErrores_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SiaWin.Browse(dt_errores);
            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir la lista de errores:" + w);
            }
        }

        private void dataGridCxC_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            try
            {
                if (dataGridCxC.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];
                    TxCodCta.Text = row["cod_cta"].ToString().Trim();
                    TxNomCta.Text = row["nom_cta"].ToString().Trim();

                    Tx30.Text = row["A1_30"].ToString().Trim();
                    Tx60.Text = row["A31_60"].ToString().Trim();
                    Tx90.Text = row["A61_90"].ToString().Trim();
                    Tx120.Text = row["A91_120"].ToString().Trim();
                    Tx150.Text = row["A120_150"].ToString().Trim();
                    Tx180.Text = row["A150_180"].ToString().Trim();
                    Tx360.Text = row["A181_360"].ToString().Trim();
                    Txm360.Text = row["mas_360"].ToString().Trim();



                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al seleccionar:" + w);
            }
        }




    }
}
