﻿using ImportacionTrasladosXls;
using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
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

    //   Sia.PublicarPnt(9664,"ImportacionTrasladosXls");
    //    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9664,"ImportacionTrasladosXls");
    //    ww.ShowInTaskbar = false;
    //    ww.Owner = Application.Current.MainWindow;
    //    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //    ww.ShowDialog();

    public partial class ImportacionTrasladosXls : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        string usuario_name = "";
        string cod_trncont = "";
        string cod_trn = "900";

        DataTable dt = new DataTable();
        DataTable dt_errores = new DataTable();

        DataSet doc_agru = new DataSet();

        public ImportacionTrasladosXls()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            LoadConfig();
            dt_errores.Columns.Add("error");
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
                this.Title = "Importacion de Traslados " + cod_empresa + "-" + nomempresa;

                DataTable dt_use = SiaWin.Func.SqlDT("select UserName,UserAlias from Seg_User where UserId='" + SiaWin._UserId + "' ", "usuarios", 0);
                usuario_name = dt_use.Rows.Count > 0 ? dt_use.Rows[0]["username"].ToString().Trim() : "USUARIO INEXISTENTE";

                DataTable dt_con = SiaWin.Func.SqlDT("select cod_tdo from Afmae_trn where cod_trn='900'", "usuarios", idemp);
                cod_trncont = dt_con.Rows.Count > 0 ? dt_con.Rows[0]["cod_tdo"].ToString().Trim() : "";
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }


        private void BtnGenerar_Click(object sender, RoutedEventArgs e)
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

                    var workbook = application.Workbooks.Create(1);
                    IWorksheet worksheet = workbook.Worksheets[0];
                    worksheet.IsGridLinesVisible = true;
                    worksheet.Range["A1"].Text = "FEC_TRN";
                    worksheet.Range["B1"].Text = "NUM_TRN";
                    worksheet.Range["C1"].Text = "COD_ACT";
                    worksheet.Range["D1"].Text = "COD_GRU";
                    worksheet.Range["E1"].Text = "DOC_INT";
                    worksheet.Range["A1:E1"].CellStyle.Font.Bold = true;

                    worksheet.Range["A1:A500"].NumberFormat = "m/d/yyyy";
                    worksheet.Range["B1:B500"].NumberFormat = "@";//formato texto
                    worksheet.Range["C1:C500"].NumberFormat = "@";//formato texto
                    worksheet.Range["D1:D500"].NumberFormat = "@";//formato texto
                    worksheet.Range["E1:E500"].NumberFormat = "@";//formato texto


                    if (string.IsNullOrEmpty(ruta)) MessageBox.Show("Por favor, seleccione una ruta para guardar la plantilla");
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
            if (dt.Columns.Contains("Fec_trn") == false || dt.Columns.IndexOf("Fec_trn") != 0) flag = false;
            if (dt.Columns.Contains("Num_trn") == false || dt.Columns.IndexOf("Num_trn") != 1) flag = false;
            if (dt.Columns.Contains("Cod_act") == false || dt.Columns.IndexOf("Cod_act") != 2) flag = false;
            if (dt.Columns.Contains("Cod_gru") == false || dt.Columns.IndexOf("Cod_gru") != 3) flag = false;
            if (dt.Columns.Contains("Doc_int") == false || dt.Columns.IndexOf("Doc_int") != 4) flag = false;
            return flag;
        }

        public DataTable Limpiar(DataTable dt)
        {
            DataTable dt1 = dt.Clone(); //copy the structure 
            for (int i = 0; i <= dt.Rows.Count - 1; i++) //iterate through the rows of the source
            {
                System.Data.DataRow currentRow = dt.Rows[i];  //copy the current row 
                foreach (var colValue in currentRow.ItemArray)//move along the columns 
                {
                    if (!string.IsNullOrEmpty(colValue.ToString())) // if there is a value in a column, copy the row and finish
                    {
                        dt1.ImportRow(currentRow);
                        break; //break and get a new row                        
                    }
                }
            }
            return dt1;
        }


        private void BtnImportar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                impotar();
            }
            catch (Exception w)
            {
                MessageBox.Show("error  al importar:" + w);
            }
        }

        public async void impotar()
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
                dt.Clear(); dt_errores.Clear();
                dt = ConvertExcelToDataTable(root);

                if (validarArchioExcel(dt) == false)
                {
                    MessageBox.Show("La plantilla importada no corresponde a la que permite el sistema por favor verifique con la plantilla que genera esta pantalla", "alerta", MessageBoxButton.OK, MessageBoxImage.Error);
                    sfBusyIndicator.IsBusy = false;
                    return;
                }

                agruparDocumentos(dt);

                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                var slowTask = Task<DataTable>.Factory.StartNew(() => Process(), source.Token);
                await slowTask;


                dataGridRefe.ItemsSource = ((DataTable)slowTask.Result).DefaultView;

                MessageBox.Show("Importacion Exitosa", "alerta", MessageBoxButton.OK, MessageBoxImage.Information);

                Tx_total.Text = ((DataTable)slowTask.Result).Rows.Count.ToString();
                Tx_errores.Text = dt_errores.Rows.Count.ToString();

                sfBusyIndicator.IsBusy = false;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al importar:" + w);
                sfBusyIndicator.IsBusy = false;
            }

        }

        private DataTable Process()
        {
            try
            {

                //VALIDAR DOCUMENTO si existe
                foreach (DataTable dtemp in doc_agru.Tables)
                {
                    string num_trn = dtemp.Rows[0]["NUM_TRN"].ToString().Trim();

                    DataTable dt_trn = SiaWin.Func.SqlDT("select * from afcab_doc where cod_trn='" + cod_trn + "' and num_trn='" + num_trn + "' ", "trn", idemp);
                    if (dt_trn.Rows.Count > 0) { System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el documento " + num_trn + "- COD_TRN:" + cod_trn + " ya existe registrado"; dt_errores.Rows.Add(row); }


                    //validar campo por campo
                    foreach (System.Data.DataRow dr in dtemp.Rows)
                    {
                        //activo
                        string cod_act = dr["COD_ACT"].ToString().Trim();

                        string query_act = "select act.cod_act,act.nom_act,act.cod_gru,gru.nom_gru from Afmae_act act ";
                        query_act += "inner join Afmae_gru gru on act.cod_gru = gru.cod_gru ";
                        query_act += "where act.cod_act='" + cod_act + "'  ";

                        DataTable dt_act = SiaWin.Func.SqlDT(query_act, "activo", idemp);
                        if (dt_act.Rows.Count > 0)
                        {
                            dr["NOM_ACT"] = dt_act.Rows[0]["nom_act"].ToString().Trim();
                            dr["GRU_ANT"] = dt_act.Rows[0]["cod_gru"].ToString().Trim();
                            dr["NOM_ANT"] = dt_act.Rows[0]["nom_gru"].ToString().Trim();
                        }
                        else
                        {
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el activo  " + cod_act + " no existe "; dt_errores.Rows.Add(row);
                            dr["NOM_ACT"] = "";
                            dr["GRU_ANT"] = "";
                            dr["NOM_ANT"] = "";
                        }

                        //grupo nuevo
                        string cod_gru = dr["COD_GRU"].ToString().Trim();
                        DataTable dt_gru = SiaWin.Func.SqlDT("select * from afmae_gru where cod_gru='" + cod_gru + "' ", "grupo", idemp);
                        if (dt_gru.Rows.Count > 0)
                        {
                            dr["COD_GRU"] = dt_gru.Rows[0]["cod_gru"].ToString().Trim();
                            dr["NOM_GRU"] = dt_gru.Rows[0]["nom_gru"].ToString().Trim();
                        }
                        else
                        {
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el grupo " + cod_gru + " no existe "; dt_errores.Rows.Add(row);
                            dr["COD_GRU"] = string.IsNullOrEmpty(dr["COD_GRU"].ToString().Trim()) ? "" : dr["COD_GRU"].ToString().Trim();
                            dr["NOM_GRU"] = "";
                        }
                    }

                }



                DataTable dtreturn = new DataTable();
                foreach (DataTable dtemp in doc_agru.Tables)
                {
                    dtreturn.Merge(dtemp);
                }
                return dtreturn;
            }
            catch (Exception e)
            {
                MessageBox.Show("en la consulta:" + e.Message);
                return null;
            }
        }

        public void agruparDocumentos(DataTable dt)
        {
            try
            {

                DataTable d = Limpiar(dt);//limpia rows en blanco                
                DataView dv = d.DefaultView;
                dv.Sort = "NUM_TRN desc";
                DataTable sortedDT = dv.ToTable();
                doc_agru.Tables.Clear();
                //SiaWin.Browse(sortedDT);


                #region algortimo el cual mete en un dataset los documentos separados por datatable
                string documento = "";
                DataTable dd = new DataTable();
                dd.Columns.Add("FEC_TRN"); dd.Columns.Add("COD_TRN"); dd.Columns.Add("NUM_TRN"); dd.Columns.Add("COD_ACT"); dd.Columns.Add("NOM_ACT"); dd.Columns.Add("COD_GRU"); dd.Columns.Add("NOM_GRU"); dd.Columns.Add("DOC_INT"); dd.Columns.Add("GRU_ANT"); dd.Columns.Add("NOM_ANT");

                DateTime da; int i = 0;
                foreach (System.Data.DataRow item in sortedDT.Rows)
                {
                    i++;

                    if (string.IsNullOrEmpty(documento)) { documento = item["NUM_TRN"].ToString().Trim(); }

                    if (documento == item["NUM_TRN"].ToString().Trim().ToUpper())
                    {
                        dd.Rows.Add(
                            Convert.ToDateTime(item["FEC_TRN"] == DBNull.Value || DateTime.TryParse(item["FEC_TRN"].ToString(), out da) == false ? DateTime.Now.ToString("dd/MM/yyy") : item["FEC_TRN"]).ToString("dd/MM/yyyy"),
                            cod_trn,
                            item["NUM_TRN"].ToString().ToUpper(),
                            item["COD_ACT"].ToString(),
                            "",
                            item["COD_GRU"].ToString(),
                            "",//nombre de grupo actual
                            item["DOC_INT"].ToString(),
                            "",//grupo anterior
                            ""//nombre de grupo anterior
                            );
                        if (i == sortedDT.Rows.Count) { doc_agru.Tables.Add(dd.Copy()); dd.Clear(); }//ultima columna
                    }
                    else
                    {
                        doc_agru.Tables.Add(dd.Copy()); dd.Clear();//agrega el documento completo a un datatable 
                        dd.Rows.Add(Convert.ToDateTime(item["FEC_TRN"] == DBNull.Value || DateTime.TryParse(item["FEC_TRN"].ToString(), out da) == false ? DateTime.Now.ToString("dd/MM/yyy") : item["FEC_TRN"]).ToString("dd/MM/yyyy"),
                            cod_trn,
                            item["NUM_TRN"].ToString().ToUpper(),
                            item["COD_ACT"].ToString(),
                            "",
                            item["COD_GRU"].ToString(),
                            "",//nombre de grupo actual
                            item["DOC_INT"].ToString(),
                            "",//grupo anterior
                            "");//nombre de grupo anterior
                    }
                    documento = item["NUM_TRN"].ToString().Trim().ToUpper();

                }
                #endregion



            }
            catch (Exception w)
            {
                MessageBox.Show("error " + w);
            }
        }

        private void DataGridRefe_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            try
            {
                if ((sender as SfDataGrid).SelectedIndex >= 0)
                {
                    var reflector = (sender as SfDataGrid).View.GetPropertyAccessProvider();
                    var rowData = (sender as SfDataGrid).GetRecordAtRowIndex((sender as SfDataGrid).SelectedIndex + 1);
                    Tx_gruact.Text = !string.IsNullOrEmpty(reflector.GetValue(rowData, "Nom_gru").ToString()) ? reflector.GetValue(rowData, "Nom_gru").ToString().ToUpper() : "NO EXISTE";
                    Tx_grupoAnt.Text = !string.IsNullOrEmpty(reflector.GetValue(rowData, "Nom_ant").ToString()) ? reflector.GetValue(rowData, "Nom_ant").ToString().ToUpper() : "NO EXISTE";
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("errro al seleccionar:" + w);
            }
        }



        private void BtnGenerarDoc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region validacion

                if (dataGridRefe.ItemsSource == null || dataGridRefe.View.Records.Count <= 0)
                {
                    MessageBox.Show("no hay datos para importar", "alerta", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                if (dt_errores.Rows.Count > 0)
                {
                    MessageBox.Show("la importacion contiene errores debe de estar todo correcto", "alerta", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                };


                #endregion

                #region insercion

                if (MessageBox.Show("Usted desea generar los traslado de los activos importados?", "Documentos", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    string sql_cab = ""; string sql_cue = "";

                    foreach (DataTable dt_cue in doc_agru.Tables)
                    {
                        
                        string num_trn_cab = dt_cue.Rows[0]["num_trn"].ToString();
                        string fecha = dt_cue.Rows[0]["fec_trn"].ToString();
                        DateTime date = Convert.ToDateTime(fecha);

                        sql_cab += @"INSERT INTO afcab_doc (cod_trn,fec_trn,num_trn,des_mov,_usu,ano_doc,per_doc) values ('" + cod_trn + "','" + fecha + "','" + num_trn_cab + "','ECHO DESDE EL PROCESO DE IMPORTACION TRASLADO','" + usuario_name + "','" + date.Year + "','" + date.Month + "');DECLARE @NewID INT;SELECT @NewID = SCOPE_IDENTITY();";

                        foreach (System.Data.DataRow dt in dt_cue.Rows)
                        {
                            string num_trn = dt["num_trn"].ToString().Trim();
                            string cod_act = dt["cod_act"].ToString().Trim();
                            string cod_gru = dt["cod_gru"].ToString().Trim();
                            string doc_int = dt["doc_int"].ToString().Trim();
                            string gru_ant = dt["gru_ant"].ToString().Trim();

                            sql_cue += @"INSERT INTO afcue_doc (idregcab,cod_trn,num_trn,cod_act,cod_gru,doc_int,gru_ant) values (@NewID,'" + cod_trn + "','" + num_trn + "','" + cod_act + "','" + cod_gru + "','" + doc_int + "','" + gru_ant + "');";
                        }


                        string query = sql_cab + sql_cue;
                        //MessageBox.Show(query);

                        if (SiaWin.Func.SqlCRUD(query, idemp) == false) { MessageBox.Show("se genero un error en un documento por favor consulte"); }
                        sql_cab = ""; sql_cue = "";
                    }

                    contabilizar();
                    AbrirDocGenerados();

                    dataGridRefe.ItemsSource = null;
                    doc_agru.Tables.Clear();


                    #endregion

                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al generar transacciones:" + w);
            }
        }


        public void contabilizar()
        {
            try
            {
                //var gb_numtrn = _DocAfijo.GroupBy(x => x.Num_trn);

                foreach (DataTable tabla in doc_agru.Tables)
                {                    
                    ContabilizaTrasladoGrupo(cod_trn, tabla.Rows[0]["NUM_TRN"].ToString().Trim());
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al generar documento contable:" + w);
            }
        }

        private int ContabilizaTrasladoGrupo(string cod_trn, string num_trn)
        {
            int idregreturn = -1;
            try
            {

                #region obtiene datos principales                
                string query = "select Afcab_doc.cod_trn,Afcab_doc.num_trn,Afmae_trn.cod_tdo,Afcab_doc.fec_trn ";
                query += "from Afcab_doc ";
                query += "inner join Afmae_trn on Afmae_trn.cod_trn = Afcab_doc.cod_trn ";
                query += "where Afcab_doc.cod_trn='" + cod_trn + "' and Afcab_doc.num_trn='" + num_trn + "' ";

                DataTable dt_trn = SiaWin.Func.SqlDT(query, "cuerpo", idemp);

                string cod_trn_af = dt_trn.Rows.Count > 0 ? dt_trn.Rows[0]["cod_trn"].ToString().Trim() : "";
                string cod_trn_co = dt_trn.Rows.Count > 0 ? dt_trn.Rows[0]["cod_tdo"].ToString().Trim() : "";
                string num_trn_co = dt_trn.Rows.Count > 0 ? dt_trn.Rows[0]["num_trn"].ToString().Trim() : "";
                DateTime _fecdoc = dt_trn.Rows.Count > 0 ? Convert.ToDateTime(dt_trn.Rows[0]["fec_trn"]) : DateTime.Now;

                #endregion

                #region obtiene cuerpo

                string cuerpo_contable = "";

                string querycue = "select cuerpo.cod_act,activo.vr_act,cuerpo.vr_mc,cuerpo.doc_int, ";
                querycue += "cuerpo.cod_gru,grupo.cta_act as g_cta,cuerpo.gru_ant,grupo_ant.cta_act as gan_cta,grupo_ant.cta_dep,grupo_ant.cta_depant ";
                querycue += "from Afcab_doc as cabeza  ";
                querycue += "inner join Afcue_doc as cuerpo on cuerpo.idregcab = cabeza.idreg  ";
                querycue += "inner join Afmae_act as activo on cuerpo.cod_act = activo.cod_act ";
                querycue += "inner join Afmae_gru as grupo on grupo.cod_gru = cuerpo.cod_gru  ";
                querycue += "inner join Afmae_gru as grupo_ant on grupo_ant.cod_gru = cuerpo.gru_ant ";
                querycue += "where cabeza.cod_trn='" + cod_trn + "' and cabeza.num_trn='" + num_trn + "' ";

                DataTable dt_cuerpo = SiaWin.Func.SqlDT(querycue, "cuerpo", idemp);

                foreach (System.Data.DataRow item in dt_cuerpo.Rows)
                {
                    decimal valor_act = Convert.ToDecimal(item["vr_act"]);
                    string cod_act = item["cod_act"].ToString().Trim();

                    string cod_gru = item["cod_gru"].ToString().Trim();
                    string gru_ant = item["gru_ant"].ToString().Trim();

                    string ctagru_nu = item["g_cta"].ToString().Trim();
                    string ctagru_an = item["gan_cta"].ToString().Trim();
                    string doc_int = item["doc_int"].ToString().Trim();

                    string cta_dep = item["cta_dep"].ToString().Trim();
                    string cta_depant = item["cta_depant"].ToString().Trim();

                    cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,des_mov,deb_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + ctagru_nu + "','" + doc_int + "','TRASLADO ACTIVO :" + cod_act + "'," + valor_act.ToString("F", CultureInfo.InvariantCulture) + "); ";
                    cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,des_mov,cre_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + ctagru_an + "','" + doc_int + "','GRUPO ANTIGUO/" + gru_ant + " GRUPO NUEVO/" + cod_gru + "'," + valor_act.ToString("F", CultureInfo.InvariantCulture) + "); ";

                    DataTable dt_depreciado = SiaWin.Func.SaldoActivo(cod_act, _fecdoc.ToString("dd/MM/yyyy"), idemp);

                    if (dt_depreciado.Rows.Count > 0)
                    {
                        double depreciado = Convert.ToDouble(dt_depreciado.Rows[0]["depreciacion"]);
                        if (depreciado > 0)
                        {
                            cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,cod_ter,des_mov,deb_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_dep + "','" + doc_int + "','1','TRASLADO DEPRECIACION " + cod_act + "'," + depreciado.ToString("F", CultureInfo.InvariantCulture) + "); ";
                            cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,cod_ter,des_mov,cre_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_depant + "','" + doc_int + "','1','TRASLADO DEPRECIACION " + cod_act + "'," + depreciado.ToString("F", CultureInfo.InvariantCulture) + "); ";
                        }
                    }

                }
                #endregion


                #region generar el documento contable
                using (SqlConnection connection = new SqlConnection(cnEmp))
                {
                    connection.Open();
                    StringBuilder errorMessages = new StringBuilder();
                    SqlCommand command = connection.CreateCommand();
                    SqlTransaction transaction;

                    transaction = connection.BeginTransaction("Transaction");
                    command.Connection = connection;
                    command.Transaction = transaction;

                    string sqlConsecutivo = @"declare @fecdoc as datetime;set @fecdoc = '" + _fecdoc.ToString() + "';declare @ini as char(4);DECLARE @NewTrn INT;";

                    //cabeza
                    string sqlcab001co = sqlConsecutivo + @" INSERT INTO cocab_doc (cod_trn,num_trn,fec_trn) values ('" + cod_trn_co + "','" + num_trn_co + "',@fecdoc);SELECT @NewTrn = SCOPE_IDENTITY();";

                    string sqlcue001co = cuerpo_contable;

                    command.CommandText = sqlcab001co + sqlcue001co + @"select CAST(@NewTrn AS int);";
                    //MessageBox.Show(command.CommandText.ToString());
                    var r = new object();
                    r = command.ExecuteScalar();
                    transaction.Commit();
                    connection.Close();
                    idregreturn = Convert.ToInt32(r.ToString());
                }
                #endregion

                return idregreturn;
            }
            catch (Exception w)
            {
                MessageBox.Show("error en  el documento contable:" + w);
                return -1;
            }
        }


        public void AbrirDocGenerados()
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("COD_TRN");
                dt.Columns.Add("NUM_TRN");
                dt.Columns.Add("FEC_TRN");
                dt.Columns.Add("COD_TDO");

                foreach (DataTable doc in doc_agru.Tables)
                {
                    dt.Rows.Add(cod_trn, doc.Rows[0]["NUM_TRN"].ToString(), doc.Rows[0]["FEC_TRN"].ToString(), cod_trncont);
                }

                BrowDocumentos win = new BrowDocumentos();
                win.dt = dt;
                win.Owner = Application.Current.MainWindow;
                win.ShowInTaskbar = false;
                win.ShowDialog();
            }
            catch (Exception w)
            {
                MessageBox.Show("erro al abrir:" + w);
            }
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







    }
}
