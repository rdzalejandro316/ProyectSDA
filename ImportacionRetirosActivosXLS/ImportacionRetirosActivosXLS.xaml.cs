using ImportacionRetirosActivosXLS;
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
    //Sia.PublicarPnt(9660,"ImportacionRetirosActivosXLS");
    //Sia.TabU(9660);

    public partial class ImportacionRetirosActivosXLS : UserControl
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        dynamic tabitem;

        string usuario_name = "";
        string cod_trncont = "";
        string cod_trn = "999";

        string cabeza = "afcab_doc";
        string cuerpo = "afcue_doc";
        string transaccion = "afmae_trn";


        DataTable dt = new DataTable();
        DataTable dt_errores = new DataTable();
        DataSet doc_agru = new DataSet();

        public ImportacionRetirosActivosXLS(dynamic tabitem1)
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
                tabitem.Title = "Importacion de Retiros";
                tabitem.Logo(idLogo, ".png");

                usuario_name = SiaWin._UserName;

                DataTable dtcon = SiaWin.Func.SqlDT("select cod_tdo from Afmae_trn where cod_trn='" + cod_trn + "'", "usuarios", idemp);
                cod_trncont = dtcon.Rows.Count > 0 ? dtcon.Rows[0]["cod_tdo"].ToString().Trim() : "";
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
                    worksheet.Range["D1"].Text = "DOC_INT";
                    worksheet.Range["E1"].Text = "COD_TER";
                    worksheet.Range["F1"].Text = "COD_CON";
                    worksheet.Range["G1"].Text = "VR_ACT";
                    worksheet.Range["H1"].Text = "DEP_ACT";
                    worksheet.Range["I1"].Text = "MESXDEP";
                    worksheet.Range["A1:I1"].CellStyle.Font.Bold = true;

                    worksheet.Range["A1:A500"].NumberFormat = "m/d/yyyy";
                    worksheet.Range["B1:B500"].NumberFormat = "@";//formato texto
                    worksheet.Range["C1:C500"].NumberFormat = "@";//formato texto
                    worksheet.Range["D1:D500"].NumberFormat = "@";//formato texto                    
                    worksheet.Range["E1:F500"].NumberFormat = "@";//formato texto
                    worksheet.Range["F1:F500"].NumberFormat = "@";//formato texto
                    worksheet.Range["G1:G500"].NumberFormat = "0.00";//formato numero
                    worksheet.Range["H1:H500"].NumberFormat = "0.00";//formato numero
                    worksheet.Range["I1:I500"].NumberFormat = "0";//formato numero                    

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

        private void BtnImportar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                impotar();
            }
            catch (Exception w)
            {
                MessageBox.Show("error al importar:" + w);
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
            if (dt.Columns.Contains("Doc_int") == false || dt.Columns.IndexOf("Doc_int") != 3) flag = false;
            if (dt.Columns.Contains("Cod_ter") == false || dt.Columns.IndexOf("Cod_ter") != 4) flag = false;
            if (dt.Columns.Contains("Cod_con") == false || dt.Columns.IndexOf("Cod_con") != 5) flag = false;
            if (dt.Columns.Contains("Vr_act") == false || dt.Columns.IndexOf("Vr_act") != 6) flag = false;
            if (dt.Columns.Contains("Dep_act") == false || dt.Columns.IndexOf("Dep_act") != 7) flag = false;
            if (dt.Columns.Contains("Mesxdep") == false || dt.Columns.IndexOf("Mesxdep") != 8) flag = false;
            return flag;
        }

        public async void impotar()
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
            var slowTask = Task<DataTable>.Factory.StartNew(() => Process(dt), source.Token);
            await slowTask;

            dataGridRefe.ItemsSource = ((DataTable)slowTask.Result).DefaultView;
            MessageBox.Show("Importacion Exitosa", "alerta", MessageBoxButton.OK, MessageBoxImage.Information);

            Tx_total.Text = ((DataTable)slowTask.Result).Rows.Count.ToString();
            Tx_errores.Text = dt_errores.Rows.Count.ToString();
            Tx_tercero.Text = ""; Tx_activo.Text = ""; Tx_concepto.Text = "";

            sfBusyIndicator.IsBusy = false;

        }

        private DataTable Process(DataTable dt)
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
                    bool act_vali = true;
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
                            act_vali = false;
                        }
                        else
                        {
                            string query_act = "select act.cod_act,act.nom_act from Afmae_act act where act.cod_act='" + cod_act + "'";

                            DataTable dt_act = SiaWin.Func.SqlDT(query_act, "activo", idemp);
                            if (dt_act.Rows.Count > 0)
                            {
                                dr["NOM_ACT"] = dt_act.Rows[0]["nom_act"].ToString().Trim();
                            }
                            else
                            {
                                System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el activo  " + cod_act + " no existe #" + num_trn + "#"; dt_errores.Rows.Add(row);
                                dr["NOM_ACT"] = "";
                                act_vali = false;
                            }
                        }


                        #endregion

                        #region tercero

                        string cod_ter = dr["cod_ter"].ToString().Trim();

                        if (!string.IsNullOrEmpty(cod_ter))
                        {
                            DataTable dt_ter = SiaWin.Func.SqlDT("select cod_ter,nom_ter from comae_ter where cod_ter='" + cod_ter + "' ", "tercero", idemp);
                            if (dt_ter.Rows.Count > 0)
                            {
                                dr["COD_TER"] = dt_ter.Rows[0]["cod_ter"].ToString().Trim();
                                dr["NOM_TER"] = dt_ter.Rows[0]["nom_ter"].ToString().Trim();
                            }
                            else
                            {
                                System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el tercero " + cod_ter + " no existe #" + num_trn + "#"; dt_errores.Rows.Add(row);
                                dr["COD_TER"] = string.IsNullOrEmpty(dr["COD_TER"].ToString().Trim()) ? "" : dr["COD_TER"].ToString().Trim();
                                dr["NOM_TER"] = "";
                            }
                        }

                        #endregion

                        #region concepto
                        string cod_con = dr["COD_CON"].ToString().Trim();

                        if (string.IsNullOrEmpty(cod_con))
                        {
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo de concepto debe de estar lleno #" + num_trn + "# "; dt_errores.Rows.Add(row);
                            dr["NOM_CON"] = "";
                        }
                        else
                        {
                            string query_con = "select cod_con,nom_con,valid from Afing_ret act where cod_con='" + cod_con + "'";

                            DataTable dt_con = SiaWin.Func.SqlDT(query_con, "concepto", idemp);
                            if (dt_con.Rows.Count > 0)
                            {
                                bool f = Convert.ToBoolean(dt_con.Rows[0]["valid"]);
                                if (f)
                                {
                                    System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el concepto " + cod_con + " no es el valido para las transacciones de retiro #" + num_trn + "# "; dt_errores.Rows.Add(row);
                                }
                                dr["NOM_CON"] = dt_con.Rows[0]["nom_con"].ToString();
                            }
                            else
                            {
                                System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el concepto " + cod_con + " no existe #" + num_trn + "#"; dt_errores.Rows.Add(row);
                                dr["NOM_CON"] = "";
                            }
                        }


                        #endregion

                        #region vr_act,dep_ac,mesxdep


                        if (string.IsNullOrEmpty(fec_trn))
                        {
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo fecha debe de estar lleno para determinar el valor del activo a dicha fecha #" + num_trn + "#"; dt_errores.Rows.Add(row);
                        }

                        if (string.IsNullOrEmpty(cod_act))
                        {
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo de activo debe de estar lleno para poder determinar el valor del activo #" + num_trn + "# "; dt_errores.Rows.Add(row);
                            dr["NOM_ACT"] = "";
                        }

                        //si es valido la fecha y si es valido el activo
                        if (fec_vali && act_vali)
                        {
                            DateTime _fecdoc = Convert.ToDateTime(fec_trn);
                            DateTime dia_ant = _fecdoc.AddDays(-1);

                            DataTable dt_saldo = SiaWin.Func.SaldoActivo(cod_act, dia_ant.ToString("dd/MM/yyyy"), 0);
                            if (dt_saldo.Rows.Count > 0)
                            {
                                double vr_act = Convert.ToDouble(dt_saldo.Rows[0]["vr_act"]);
                                double depreciado = Convert.ToDouble(dt_saldo.Rows[0]["dep_ac"]);
                                int mesxdep = Convert.ToInt32(dt_saldo.Rows[0]["mesxdep"]);

                                dr["MESXDEP"] = mesxdep * (-1);
                                dr["VR_ACT"] = vr_act * (-1);
                                dr["DEP_AC"] = depreciado * (-1);
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
                DataTable d = Limpiar(dt);
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
                dd.Columns.Add("DOC_INT");
                dd.Columns.Add("COD_TER");
                dd.Columns.Add("NOM_TER");
                dd.Columns.Add("COD_CON");
                dd.Columns.Add("NOM_CON");
                dd.Columns.Add("VR_ACT");
                dd.Columns.Add("DEP_AC");
                dd.Columns.Add("MESXDEP");
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
                            string doc_int = row["DOC_INT"].ToString();
                            string cod_ter = row["COD_TER"].ToString();
                            string cod_con = row["COD_CON"].ToString();

                            dd.Rows.Add(
                                fec_trn,
                                cod_trn,
                                num_trn,
                                cod_act,
                                "",//nombre activo                                
                                doc_int,
                                cod_ter,
                                "",//nombre de tercero
                                cod_con,
                                "",//nombre de concepto
                                0, 0, 0// vr_act,dep_ac,mesxdep
                                );
                        }

                        doc_agru.Tables.Add(dd.Copy());
                        dd.Clear();
                    }
                }


                #endregion

            }
            catch (Exception w)
            {
                MessageBox.Show("error " + w);
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


                if (MessageBox.Show("Usted desea generar los documentos de retiros de activos?", "Documentos", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    string sql_cab = ""; string sql_cue = "";

                    foreach (DataTable dt_cue in doc_agru.Tables)
                    {
                        string num_trn_cab = dt_cue.Rows[0]["num_trn"].ToString();
                        string fecha = dt_cue.Rows[0]["fec_trn"].ToString();
                        DateTime date = Convert.ToDateTime(fecha);

                        sql_cab += @"INSERT INTO afcab_doc (cod_trn,fec_trn,num_trn,des_mov,_usu,ano_doc,per_doc) values ('" + cod_trn + "','" + fecha + "','" + num_trn_cab + "','ECHO DESDE EL PROCESO DE IMPORTACION','" + usuario_name + "','" + date.Year + "','" + date.Month + "');DECLARE @NewID INT;SELECT @NewID = SCOPE_IDENTITY();";

                        foreach (System.Data.DataRow dtcue in dt_cue.Rows)
                        {
                            string cod_act = dtcue["cod_act"].ToString().Trim();
                            string doc_int = dtcue["doc_int"].ToString().Trim();
                            string cod_ter = dtcue["cod_ter"].ToString().Trim();
                            string cod_con = dtcue["cod_con"].ToString().Trim();
                            decimal vr_act = Convert.ToDecimal(dtcue["vr_act"]);
                            decimal dep_ac = Convert.ToDecimal(dtcue["dep_ac"]);
                            int mesxdep = Convert.ToInt32(dtcue["mesxdep"]);

                            sql_cue += @"INSERT INTO afcue_doc (idregcab,cod_trn,num_trn,cod_act,doc_int,cod_ter,cod_con,vr_act,dep_ac,mesxdep) values (@NewID,'" + cod_trn + "','" + num_trn_cab + "','" + cod_act + "','" + doc_int + "','" + cod_ter + "','" + cod_con + "'," + vr_act.ToString("F", CultureInfo.InvariantCulture) + "," + dep_ac.ToString("F", CultureInfo.InvariantCulture) + "," + mesxdep + ");";
                        }

                        string query = sql_cab + sql_cue;
                        if (SiaWin.Func.SqlCRUD(query, idemp) == false) { MessageBox.Show("se genero un error en un documento por favor consulte"); }

                        sql_cab = ""; sql_cue = "";
                    }

                    contabilizar();
                    AbrirDocGenerados();
                    dataGridRefe.ItemsSource = null;
                    doc_agru.Tables.Clear();
                }
                #endregion

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
                    ContabilizaRetiro(cod_trn, tabla.Rows[0]["NUM_TRN"].ToString().Trim());
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al generar documento contable:" + w);
            }
        }


        private int ContabilizaRetiro(string cod_trn, string num_trn)
        {
            int idregreturn = -1;
            try
            {


                #region obtiene datos principales                
                string query = "select Afcab_doc.cod_trn,Afcab_doc.num_trn,Afcab_doc.fec_trn,Afmae_trn.cod_tdo from Afcab_doc  ";
                query += "inner join Afmae_trn on Afmae_trn.cod_trn = Afcab_doc.cod_trn ";
                query += "where Afcab_doc.cod_trn='" + cod_trn + "' and Afcab_doc.num_trn='" + num_trn + "'  ";

                DataTable dt_trn = SiaWin.Func.SqlDT(query, "cuerpo", idemp);

                string cod_trn_af = dt_trn.Rows.Count > 0 ? dt_trn.Rows[0]["cod_trn"].ToString().Trim() : "";
                string cod_trn_co = dt_trn.Rows.Count > 0 ? dt_trn.Rows[0]["cod_tdo"].ToString().Trim() : "";
                string num_trn_co = dt_trn.Rows.Count > 0 ? dt_trn.Rows[0]["num_trn"].ToString().Trim() : "";
                DateTime _fecdoc = Convert.ToDateTime(dt_trn.Rows.Count > 0 ? dt_trn.Rows[0]["fec_trn"].ToString().Trim() : DateTime.Now.ToString("dd/MM/yyyy"));
                DateTime dia_ant = _fecdoc.AddDays(-1);

                #endregion

                #region obtiene cuerpo

                string cuerpo_contable = "";

                string querycue = "select Afcue_doc.cod_act,Afcue_doc.cod_ter,Afcue_doc.doc_int,Afcue_doc.cod_con,Afcue_doc.act_tras, ";
                querycue += "Afmae_gru.cta_act,Afmae_gru.cta_dep,";
                querycue += "Afing_ret.cta_per,Afing_ret.cta_ord,Afing_ret.cta_orc, ";
                querycue += "Afmae_gru.cta_ordd,Afmae_gru.cta_ordc, ";
                querycue += "Afmae_gru.cta_gdp ";
                querycue += "from Afcab_doc  ";
                querycue += "inner join Afcue_doc on Afcue_doc.idregcab = Afcab_doc.idreg ";
                querycue += "inner join Afmae_act on Afcue_doc.cod_act = Afmae_act.cod_act ";
                querycue += "inner join Afmae_gru on Afmae_gru.cod_gru = Afmae_act.cod_gru ";
                querycue += "inner join Afing_ret on Afing_ret.cod_con = Afcue_doc.cod_con ";
                querycue += "where Afcab_doc.cod_trn='" + cod_trn + "' and  Afcab_doc.num_trn='" + num_trn + "' ";


                DataTable dt_cuerpo = SiaWin.Func.SqlDT(querycue, "cuerpo", idemp);

                string update = "";
                foreach (System.Data.DataRow item in dt_cuerpo.Rows)
                {

                    string cod_act = item["cod_act"].ToString().Trim();
                    string act_tras = item["act_tras"].ToString().Trim();
                    string cod_con = item["cod_con"].ToString().Trim();
                    string doc_int = item["doc_int"].ToString().Trim();
                    string cod_ter = item["cod_ter"].ToString().Trim();


                    string cta_act = item["cta_act"].ToString().Trim();
                    string cta_dep = item["cta_dep"].ToString().Trim();
                    string cta_per = item["cta_per"].ToString().Trim();
                    string cta_ord = item["cta_ord"].ToString().Trim();
                    string cta_orc = item["cta_orc"].ToString().Trim();

                    string cta_ordd = item["cta_ordd"].ToString().Trim();
                    string cta_ordc = item["cta_ordc"].ToString().Trim();

                    string cta_gdp = item["cta_gdp"].ToString().Trim();

                    update += "update afmae_act set ind_ret=1 where cod_act='" + cod_act + "';";

                    DataTable dt_depreciado = SiaWin.Func.SaldoActivo(cod_act, dia_ant.ToString("dd/MM/yyyy"), 0);

                    if (dt_depreciado.Rows.Count > 0)
                    {
                        double vr_act = Convert.ToDouble(dt_depreciado.Rows[0]["vr_act"]);
                        double depreciado = Convert.ToDouble(dt_depreciado.Rows[0]["dep_ac"]);
                        double faltante = vr_act - depreciado;

                        cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,cod_ter,des_mov,cre_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_act + "','" + doc_int + "','" + cod_ter + "','Retiro - " + cod_act + " '," + vr_act.ToString("F", CultureInfo.InvariantCulture) + "); ";

                        if (depreciado > 0)
                            cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,cod_ter,des_mov,deb_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_dep + "','" + doc_int + "','" + cod_ter + "','Retiro - " + cod_act + " '," + depreciado.ToString("F", CultureInfo.InvariantCulture) + "); ";

                        if (faltante > 0)
                            cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,cod_ter,des_mov,deb_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_per + "','" + doc_int + "','" + cod_ter + "','Retiro - " + cod_act + " '," + faltante.ToString("F", CultureInfo.InvariantCulture) + "); ";


                        if (cod_con == "51")
                        {
                            double val = faltante > 0 ? faltante : vr_act;
                            cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,des_mov,deb_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_ord + "','" + doc_int + "','Retiro - " + cod_act + "'," + val.ToString("F", CultureInfo.InvariantCulture) + "); ";
                            cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,des_mov,cre_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_orc + "','" + doc_int + "','Retiro - " + cod_act + "'," + val.ToString("F", CultureInfo.InvariantCulture) + "); ";
                        }

                        if (cod_con == "52")
                        {
                            cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,des_mov,deb_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_ordd + "','" + doc_int + "','Retiro - " + cod_act + "',1); ";
                            cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,des_mov,cre_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_ordc + "','" + doc_int + "','Retiro - " + cod_act + "',1); ";
                        }

                        if (cod_con == "60")
                        {
                            DataTable dt_g = SiaWin.Func.UltimoActivo(act_tras, _fecdoc.ToString("dd/MM/yyyy"), 0);
                            if (dt_g.Rows.Count > 0)
                            {
                                string ctaact = dt_g.Rows[0]["cta_act"].ToString().Trim();
                                string ctadep = dt_g.Rows[0]["cta_dep"].ToString().Trim();
                                cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,des_mov,deb_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + ctaact + "','" + doc_int + "','Adicion a: " + act_tras + " - Placa:" + cod_act + "'," + vr_act.ToString("F", CultureInfo.InvariantCulture) + "); ";
                                cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,des_mov,cre_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + ctadep + "','" + doc_int + "','Adicion a: " + act_tras + " - Placa:" + cod_act + "'," + depreciado.ToString("F", CultureInfo.InvariantCulture) + "); ";
                            }

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


                #region cambiar indicador de retiro en la maestra             

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

        
        private void DataGridRefe_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            try
            {
                if ((sender as Syncfusion.UI.Xaml.Grid.SfDataGrid).SelectedIndex >= 0)
                {
                    var reflector = (sender as Syncfusion.UI.Xaml.Grid.SfDataGrid).View.GetPropertyAccessProvider();
                    var rowData = (sender as Syncfusion.UI.Xaml.Grid.SfDataGrid).GetRecordAtRowIndex((sender as Syncfusion.UI.Xaml.Grid.SfDataGrid).SelectedIndex + 1);
                    Tx_tercero.Text = !string.IsNullOrEmpty(reflector.GetValue(rowData, "Nom_ter").ToString()) ? reflector.GetValue(rowData, "Nom_ter").ToString().ToUpper() : "NO EXISTE";
                    Tx_activo.Text = !string.IsNullOrEmpty(reflector.GetValue(rowData, "Nom_act").ToString()) ? reflector.GetValue(rowData, "Nom_act").ToString().ToUpper() : "NO EXISTE";
                    Tx_concepto.Text = !string.IsNullOrEmpty(reflector.GetValue(rowData, "Nom_con").ToString()) ? reflector.GetValue(rowData, "Nom_con").ToString().ToUpper() : "NO EXISTE";
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("errro al seleccionar:" + w);
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

