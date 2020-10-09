using ImportacionTrasladosXls;
using Microsoft.Win32;
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
using System.IO;
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
    //   Sia.TabU(9664);

    public partial class ImportacionTrasladosXls : UserControl
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        dynamic tabitem;

        string cod_empresa = "";
        string usuario_name = "";
        string cod_trncont = "";
        string cod_trn = "900";

        string cabeza = "afcab_doc";
        string cuerpo = "afcue_doc";
        string transaccion = "afmae_trn";

        DataTable dt = new DataTable();
        DataTable dt_errores = new DataTable();

        DataSet doc_agru = new DataSet();

        public ImportacionTrasladosXls(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            tabitem = tabitem1;
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
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                tabitem.Title = "Importacion de Traslados " + cod_empresa + "-" + nomempresa;
                tabitem.Logo(idLogo, ".png");

                usuario_name = SiaWin._UserName;

                DataTable dt_con = SiaWin.Func.SqlDT("select cod_tdo from Afmae_trn where cod_trn='900'", "usuarios", idemp);
                cod_trncont = dt_con.Rows.Count > 0 ? dt_con.Rows[0]["cod_tdo"].ToString().Trim() : "";
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load:" + e.Message);
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
                if (dt == null) { sfBusyIndicator.IsBusy = false; return; }

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
                Tx_gruact.Text = "";
                Tx_grupoAnt.Text = "";

                sfBusyIndicator.IsBusy = false;
            }
            catch (IOException)
            {
                MessageBox.Show("cierre el archivo que desea importar para poder continuar con el procesos", "alerta", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    #region validacion documento

                    string num_trn = dtemp.Rows[0]["NUM_TRN"].ToString().Trim();

                    DataTable dt_trn = SiaWin.Func.SqlDT("select * from " + cabeza + " where cod_trn='" + cod_trn + "' and num_trn='" + num_trn + "' ", "trn", idemp);
                    if (dt_trn.Rows.Count > 0) { System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el documento " + num_trn + "- COD_TRN:" + cod_trn + " ya existe registrado"; dt_errores.Rows.Add(row); }

                    #endregion


                    DateTime date;
                    bool fec_vali = true;
                    //validar campo por campo
                    foreach (System.Data.DataRow dr in dtemp.Rows)
                    {


                        #region fecha
                        string fec_trn = dr["FEC_TRN"].ToString().Trim();

                        if (string.IsNullOrEmpty(fec_trn))
                        {
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo fecha debe de estar lleno #" + num_trn + "#"; dt_errores.Rows.Add(row);
                            fec_vali = false;
                        }
                        else
                        {
                            if (dr["FEC_TRN"] == DBNull.Value || DateTime.TryParse(dr["FEC_TRN"].ToString(), out date) == false)
                            {
                                System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "la fecha " + fec_trn + " ingresada no cuenta con el formato (dd/MM/yyyy) correcto #" + num_trn + "#"; dt_errores.Rows.Add(row);
                                fec_vali = false;
                            }
                        }

                        #endregion

                        #region activo
                        string cod_act = dr["COD_ACT"].ToString().Trim();

                        if (string.IsNullOrEmpty(cod_act))
                        {
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo de activo debe de estar lleno #" + num_trn + "# "; dt_errores.Rows.Add(row);
                            dr["NOM_ACT"] = "";
                            dr["GRU_ANT"] = "";
                            dr["NOM_ANT"] = "";
                        }
                        else
                        {
                            string query_act = "select act.cod_act,act.nom_act from Afmae_act act where act.cod_act='" + cod_act + "'";

                            DataTable dt_act = SiaWin.Func.SqlDT(query_act, "activo", idemp);
                            if (dt_act.Rows.Count > 0)
                            {
                                dr["NOM_ACT"] = dt_act.Rows[0]["nom_act"].ToString().Trim();

                                if (fec_vali)
                                {
                                    DateTime f = Convert.ToDateTime(fec_trn);
                                    DataTable dt_ultgru = SiaWin.Func.UltimoActivo(cod_act, f.ToString("dd/MM/yyyy"), 0);
                                    dr["GRU_ANT"] = dt_ultgru.Rows[0]["cod_gru"].ToString().Trim();
                                    dr["NOM_ANT"] = dt_ultgru.Rows[0]["nom_gru"].ToString().Trim();
                                }
                                else
                                {
                                    System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo fecha debe de estar lleno para obtener apartir de dicha fecha el ultimo grupo del activo #" + num_trn + "# "; dt_errores.Rows.Add(row);
                                    dr["GRU_ANT"] = "";
                                    dr["NOM_ANT"] = "";
                                }

                            }
                            else
                            {
                                System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el activo  " + cod_act + " no existe #" + num_trn + "#"; dt_errores.Rows.Add(row);
                                dr["NOM_ACT"] = "";
                                dr["GRU_ANT"] = "";
                                dr["NOM_ANT"] = "";
                            }
                        }


                        #endregion

                        #region grupo nuevo

                        string cod_gru = dr["COD_GRU"].ToString().Trim();

                        if (string.IsNullOrEmpty(cod_gru))
                        {
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo grupo debe de estar lleno #" + num_trn + "#"; dt_errores.Rows.Add(row);
                            dr["COD_GRU"] = string.IsNullOrEmpty(dr["COD_GRU"].ToString().Trim()) ? "" : dr["COD_GRU"].ToString().Trim();
                            dr["NOM_GRU"] = "";
                        }
                        else
                        {
                            DataTable dt_gru = SiaWin.Func.SqlDT("select * from afmae_gru where cod_gru='" + cod_gru + "' ", "grupo", idemp);
                            if (dt_gru.Rows.Count > 0)
                            {
                                dr["COD_GRU"] = dt_gru.Rows[0]["cod_gru"].ToString().Trim();
                                dr["NOM_GRU"] = dt_gru.Rows[0]["nom_gru"].ToString().Trim();
                            }
                            else
                            {
                                System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el grupo " + cod_gru + " no existe #" + num_trn + "#"; dt_errores.Rows.Add(row);
                                dr["COD_GRU"] = string.IsNullOrEmpty(dr["COD_GRU"].ToString().Trim()) ? "" : dr["COD_GRU"].ToString().Trim();
                                dr["NOM_GRU"] = "";
                            }
                        }

                        #endregion

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


                #region columnas                
                DataTable dd = new DataTable();
                dd.Columns.Add("FEC_TRN");
                dd.Columns.Add("COD_TRN");
                dd.Columns.Add("NUM_TRN");
                dd.Columns.Add("COD_ACT");
                dd.Columns.Add("NOM_ACT");
                dd.Columns.Add("COD_GRU");
                dd.Columns.Add("NOM_GRU");
                dd.Columns.Add("DOC_INT");
                dd.Columns.Add("GRU_ANT");
                dd.Columns.Add("NOM_ANT");
                #endregion

                #region algortimo el cual mete en un dataset los documentos separados por datatable

                #region transaccion agrupada

                DataTable dt_gb = sortedDT.AsEnumerable()
               .GroupBy(r => new { Col1 = r["NUM_TRN"] })
               .Select(g =>
               {
                   var row = dt.NewRow();
                   row["NUM_TRN"] = g.Key.Col1;
                   return row;
               })
               .CopyToDataTable();

                #endregion

                if (dt_gb.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt_gb.Rows)
                    {
                        string num_trn = dr["NUM_TRN"].ToString();

                        DataRow[] result = sortedDT.Select("NUM_TRN='" + num_trn + "'");

                        foreach (DataRow row in result)
                        {
                            string fec_trn = row["FEC_TRN"].ToString();
                            string cod_act = row["COD_ACT"].ToString();
                            string cod_gru = row["COD_GRU"].ToString();
                            string doc_int = row["DOC_INT"].ToString();

                            dd.Rows.Add(
                                fec_trn,
                                cod_trn,
                                num_trn,
                                cod_act,
                                "",//nombre activo
                                cod_gru,
                                "",//nombre grupo
                                doc_int,
                                "",//grupo anterior
                                ""//nombre grupo anterior
                                );
                        }

                        doc_agru.Tables.Add(dd.Copy());
                        dd.Clear();
                    }
                }

                #endregion

                //MessageBox.Show("FFF");
                //foreach (DataTable item in doc_agru.Tables)
                //{
                //    SiaWin.Browse(item);
                //}
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
                if ((sender as Syncfusion.UI.Xaml.Grid.SfDataGrid).SelectedIndex >= 0)
                {
                    var reflector = (sender as Syncfusion.UI.Xaml.Grid.SfDataGrid).View.GetPropertyAccessProvider();
                    var rowData = (sender as Syncfusion.UI.Xaml.Grid.SfDataGrid).GetRecordAtRowIndex((sender as Syncfusion.UI.Xaml.Grid.SfDataGrid).SelectedIndex + 1);

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
                    Tx_total.Text = "0";
                    Tx_errores.Text = "0";
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
                string cod_trn_co = dt_trn.Rows.Count > 0 ? dt_trn.Rows[0]["cod_tdo"].ToString().Trim() : cod_trncont;
                string num_trn_co = dt_trn.Rows.Count > 0 ? dt_trn.Rows[0]["num_trn"].ToString().Trim() : "";
                DateTime _fecdoc = dt_trn.Rows.Count > 0 ? Convert.ToDateTime(dt_trn.Rows[0]["fec_trn"]) : DateTime.Now;

                #endregion

                #region obtiene cuerpo

                string cuerpo_contable = "";

                string querycue = "select cuerpo.cod_act,activo.vr_act,cuerpo.vr_mc,cuerpo.doc_int, ";
                querycue += "cuerpo.cod_gru,grupo.cta_act as cta_act_act,grupo.cta_dep as cta_dep_act, ";
                querycue += "cuerpo.gru_ant,grupo_ant.cta_act as cta_act_ant,grupo_ant.cta_dep as cta_dep_ant ";
                querycue += "from Afcab_doc as cabeza  ";
                querycue += "inner join Afcue_doc as cuerpo on cuerpo.idregcab = cabeza.idreg  ";
                querycue += "inner join Afmae_act as activo on cuerpo.cod_act = activo.cod_act ";
                querycue += "inner join Afmae_gru as grupo on grupo.cod_gru = cuerpo.cod_gru  ";
                querycue += "inner join Afmae_gru as grupo_ant on grupo_ant.cod_gru = cuerpo.gru_ant ";
                querycue += "where cabeza.cod_trn='" + cod_trn + "' and cabeza.num_trn='" + num_trn + "' ";

                DataTable dt_cuerpo = SiaWin.Func.SqlDT(querycue, "cuerpo", idemp);

                string update = " ";

                foreach (System.Data.DataRow item in dt_cuerpo.Rows)
                {
                    string cod_act = item["cod_act"].ToString().Trim();
                    string doc_int = item["doc_int"].ToString().Trim();

                    string cod_gru = item["cod_gru"].ToString().Trim();
                    string cta_act_act = item["cta_act_act"].ToString().Trim();
                    string cta_dep_act = item["cta_dep_act"].ToString().Trim();


                    string gru_ant = item["gru_ant"].ToString().Trim();
                    string cta_act_ant = item["cta_act_ant"].ToString().Trim();
                    string cta_dep_ant = item["cta_dep_ant"].ToString().Trim();

                    update += "update afmae_act set cod_gru='" + cod_gru + "' where cod_act ='" + cod_act + "';";
                    
                    DataTable dt_saldo = SiaWin.Func.SaldoActivo(cod_act, _fecdoc.ToString("dd/MM/yyyy"), 0);

                    if (dt_saldo.Rows.Count > 0)
                    {
                        double valor_act = Convert.ToDouble(dt_saldo.Rows[0]["vr_act"]);

                        cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,des_mov,deb_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_act_act + "','" + doc_int + "','TRASLADO ACTIVO :" + cod_act + "'," + valor_act.ToString("F", CultureInfo.InvariantCulture) + "); ";
                        cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,des_mov,cre_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_act_ant + "','" + doc_int + "','GRUPO ANTIGUO/" + gru_ant + " GRUPO NUEVO/" + cod_gru + "'," + valor_act.ToString("F", CultureInfo.InvariantCulture) + "); ";


                        double depreciado = Convert.ToDouble(dt_saldo.Rows[0]["dep_ac"]);
                        if (depreciado > 0)
                        {
                            cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,cod_ter,des_mov,deb_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_dep_act + "','" + doc_int + "','1','TRASLADO DEPRECIACION " + cod_act + "'," + depreciado.ToString("F", CultureInfo.InvariantCulture) + "); ";
                            cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,cod_ter,des_mov,cre_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_dep_ant + "','" + doc_int + "','1','TRASLADO DEPRECIACION " + cod_act + "'," + depreciado.ToString("F", CultureInfo.InvariantCulture) + "); ";
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

                #region update grupo

                SiaWin.Func.SqlCRUD(update, idemp);

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
