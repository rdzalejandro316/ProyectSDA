using ImportacionRetirosActivosXLS;
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



    //   Sia.PublicarPnt(9660,"ImportacionRetirosActivosXLS");
    //   Sia.TabU(9660);

    public partial class ImportacionRetirosActivosXLS : UserControl 
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        dynamic tabitem;

        string usuario_name = "";
        string cod_trncont = "";
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
                tabitem.Title = "Importacion de Documentos -" + cod_empresa + "-" + nomempresa;
                tabitem.Logo(idLogo, ".png");


                DataTable dtemp = SiaWin.Func.SqlDT("select UserName,UserAlias from Seg_User where UserId='" + SiaWin._UserId + "' ", "usuarios", 0);
                usuario_name = dtemp.Rows.Count > 0 ? dtemp.Rows[0]["username"].ToString().Trim() : "USUARIO INEXISTENTE";

                DataTable dtcon = SiaWin.Func.SqlDT("select cod_tdo from Afmae_trn where cod_trn='999'", "usuarios", idemp);
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
                    worksheet.Range["D1"].Text = "COD_GRU";
                    worksheet.Range["E1"].Text = "DOC_INT";
                    worksheet.Range["F1"].Text = "COD_TER";
                    worksheet.Range["G1"].Text = "COD_CON";
                    worksheet.Range["H1"].Text = "VR_ACT";
                    worksheet.Range["I1"].Text = "DEP_ACT";
                    worksheet.Range["J1"].Text = "MESXDEP";
                    worksheet.Range["A1:J1"].CellStyle.Font.Bold = true;

                    worksheet.Range["A1:A500"].NumberFormat = "m/d/yyyy";
                    worksheet.Range["B1:B500"].NumberFormat = "@";//formato texto
                    worksheet.Range["C1:C500"].NumberFormat = "@";//formato texto
                    worksheet.Range["D1:D500"].NumberFormat = "@";//formato texto
                    worksheet.Range["E1:E500"].NumberFormat = "@";//formato texto
                    worksheet.Range["F1:F500"].NumberFormat = "@";//formato texto
                    worksheet.Range["G1:G500"].NumberFormat = "@";//formato texto
                    worksheet.Range["H1:H500"].NumberFormat = "0.00";//formato numero
                    worksheet.Range["I1:I500"].NumberFormat = "0.00";//formato numero
                    worksheet.Range["J1:J500"].NumberFormat = "0";//formato numero

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
            if (dt.Columns.Contains("Cod_ter") == false || dt.Columns.IndexOf("Cod_ter") != 5) flag = false;
            if (dt.Columns.Contains("Cod_con") == false || dt.Columns.IndexOf("Cod_con") != 6) flag = false;
            if (dt.Columns.Contains("Vr_act") == false || dt.Columns.IndexOf("Vr_act") != 7) flag = false;
            if (dt.Columns.Contains("Dep_act") == false || dt.Columns.IndexOf("Dep_act") != 8) flag = false;
            if (dt.Columns.Contains("Mesxdep") == false || dt.Columns.IndexOf("Mesxdep") != 9) flag = false;
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


            if (validarArchioExcel(dt) == false)
            {
                MessageBox.Show("La plantilla importada no corresponde a la que permite el sistema por favor verifique con la plantilla que genera esta pantalla", "alerta", MessageBoxButton.OK, MessageBoxImage.Error);
                sfBusyIndicator.IsBusy = false;
                return;
            }


            CancellationTokenSource source = new CancellationTokenSource();
            var slowTask = Task<DataTable>.Factory.StartNew(() => Process(dt), source.Token);
            await slowTask;



            decimal n; DateTime d; int e;
            int rows = 0;
            foreach (System.Data.DataRow row in dt.Rows)
            {
                if (!string.IsNullOrEmpty(row[0].ToString()))
                {
                    //rows++;
                    //_DocAfijo.Add(new Documentos(
                    //    "999",
                    //    row["Num_trn"].ToString(),
                    //    Convert.ToDateTime(row["Fec_trn"] == DBNull.Value || DateTime.TryParse(row["Fec_trn"].ToString(), out d) == false ? DateTime.Now.ToString("dd/MM/yyy") : row["Fec_trn"]).ToString("dd/MM/yyyy"),
                    //    row["Cod_ter"].ToString(),
                    //    row["Cod_act"].ToString(),
                    //    row["Doc_int"].ToString(),
                    //    row["Cod_gru"].ToString(),
                    //    row["Cod_con"].ToString(),
                    //    Convert.ToDecimal(row["Vr_act"] == DBNull.Value || decimal.TryParse(row["Vr_act"].ToString(), out n) == false ? 0 : row["Vr_act"]),
                    //    Convert.ToDecimal(row["Dep_act"] == DBNull.Value || decimal.TryParse(row["Dep_act"].ToString(), out n) == false ? 0 : row["Dep_act"]),
                    //    Convert.ToInt32(row["Mesxdep"] == DBNull.Value || int.TryParse(row["Mesxdep"].ToString(), out e) == false ? 0 : row["Mesxdep"])
                    //    ));
                }
            }


            agruparDocumentos(dt);



            #region grilla
            //dataGridRefe.ItemsSource = DocAfijo;
            dataGridRefe.View.Refresh();
            dataGridRefe.Columns["Cod_trn"].Width = 70;
            dataGridRefe.Columns["Num_trn"].Width = 100;
            dataGridRefe.Columns["Num_trn"].HeaderText = "Documento";
            dataGridRefe.Columns["Fec_trn"].Width = 80;
            dataGridRefe.Columns["Fec_trn"].HeaderText = "Fecha";
            dataGridRefe.Columns["Cod_ter"].Width = 100;
            dataGridRefe.Columns["Cod_ter"].HeaderText = "NIT/CC";
            dataGridRefe.Columns["Cod_act"].Width = 80;
            dataGridRefe.Columns["Cod_act"].HeaderText = "Codigo";
            dataGridRefe.Columns["Doc_int"].Width = 80;
            dataGridRefe.Columns["Doc_int"].HeaderText = "Doc Refe";
            dataGridRefe.Columns["Cod_gru"].Width = 80;
            dataGridRefe.Columns["Cod_gru"].HeaderText = "Cod grupo";
            dataGridRefe.Columns["Cod_con"].Width = 70;
            dataGridRefe.Columns["Cod_con"].HeaderText = "Concepto";
            dataGridRefe.Columns["Vr_act"].Width = 80;
            dataGridRefe.Columns["Vr_act"].HeaderText = "Valor";
            dataGridRefe.Columns["Dep_act"].Width = 80;
            dataGridRefe.Columns["Dep_act"].HeaderText = "Dep Activo";
            dataGridRefe.Columns["Mesxdep"].Width = 80;
            dataGridRefe.Columns["Nom_act"].Width = 0;
            dataGridRefe.Columns["Nom_ter"].Width = 0;
            dataGridRefe.Columns["Nom_gru"].Width = 0;
            dataGridRefe.Columns["Nom_con"].Width = 0;

            #endregion;
            MessageBox.Show("Importacion Exitosa", "alerta", MessageBoxButton.OK, MessageBoxImage.Information);

            Tx_total.Text = rows.ToString();

            int tot_erro = 0;
            //foreach (var item in _DocAfijo)
            //{
            //    if (!string.IsNullOrEmpty(item.Error)) tot_erro++;
            //}

            Tx_errores.Text = tot_erro.ToString();
            Tx_tercero.Text = ""; Tx_activo.Text = "";
            Tx_grupo.Text = ""; Tx_concepto.Text = "";

        }

        private DataTable Process(DataTable dt)
        {
            try
            {

                foreach (System.Data.DataRow dr in dt.Rows)
                {

                    //---- valida existencias
                    //cod_trn y num_trn

                    string cod_trn = dr["COD_TRN"].ToString().Trim();
                    //string num_trn = dr["NUM_TRN"].ToString().Trim();

                    //DataTable dt_bod = SiaWin.Func.SqlDT("select cod_bod,cod_ter from inmae_bod where cod_bod='" + cod_bod + "'  ", "bodegas", idemp);







                }



                return dt;
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
                //SiaWin.Browse(d);
                doc_agru.Tables.Clear();

                #region algortimo el cual mete en un dataset los documentos separados por datatable
                string documento = "";
                DataTable dd = new DataTable();
                dd.Columns.Add("FEC_TRN"); dd.Columns.Add("NUM_TRN"); dd.Columns.Add("COD_ACT"); dd.Columns.Add("COD_GRU");
                dd.Columns.Add("DOC_INT"); dd.Columns.Add("COD_TER"); dd.Columns.Add("COD_CON"); dd.Columns.Add("VR_ACT"); dd.Columns.Add("DEP_ACT"); dd.Columns.Add("MESXDEP");

                decimal n; DateTime da; int e; int i = 0;
                foreach (System.Data.DataRow item in sortedDT.Rows)
                {
                    i++;
                    if (string.IsNullOrEmpty(documento)) { documento = item["NUM_TRN"].ToString().Trim(); }
                    if (documento == item["NUM_TRN"].ToString().Trim())
                    {
                        dd.Rows.Add(Convert.ToDateTime(item["FEC_TRN"] == DBNull.Value || DateTime.TryParse(item["FEC_TRN"].ToString(), out da) == false ? DateTime.Now.ToString("dd/MM/yyy") : item["FEC_TRN"]).ToString("dd/MM/yyyy"), item["NUM_TRN"].ToString(), item["COD_ACT"].ToString(), item["COD_GRU"].ToString(), item["DOC_INT"].ToString(), item["COD_TER"].ToString(), item["COD_CON"].ToString(), Convert.ToDecimal(item["VR_ACT"] == DBNull.Value || decimal.TryParse(item["VR_ACT"].ToString(), out n) == false ? 0 : item["VR_ACT"]), Convert.ToDecimal(item["DEP_ACT"] == DBNull.Value || decimal.TryParse(item["DEP_ACT"].ToString(), out n) == false ? 0 : item["DEP_ACT"]), Convert.ToInt32(item["MESXDEP"] == DBNull.Value || int.TryParse(item["MESXDEP"].ToString(), out e) == false ? 0 : item["MESXDEP"]));
                        if (i == sortedDT.Rows.Count) { doc_agru.Tables.Add(dd.Copy()); dd.Clear(); }//ultima columna
                    }
                    else
                    {
                        doc_agru.Tables.Add(dd.Copy()); dd.Clear();//agrega el documento completo a un datatable 
                        dd.Rows.Add(Convert.ToDateTime(item["FEC_TRN"] == DBNull.Value || DateTime.TryParse(item["FEC_TRN"].ToString(), out da) == false ? DateTime.Now.ToString("dd/MM/yyy") : item["FEC_TRN"]).ToString("dd/MM/yyyy"), item["NUM_TRN"].ToString(), item["COD_ACT"].ToString(), item["COD_GRU"].ToString(), item["DOC_INT"].ToString(), item["COD_TER"].ToString(), item["COD_CON"].ToString(), Convert.ToDecimal(item["VR_ACT"] == DBNull.Value || decimal.TryParse(item["VR_ACT"].ToString(), out n) == false ? 0 : item["VR_ACT"]), Convert.ToDecimal(item["DEP_ACT"] == DBNull.Value || decimal.TryParse(item["DEP_ACT"].ToString(), out n) == false ? 0 : item["DEP_ACT"]), Convert.ToInt32(item["MESXDEP"] == DBNull.Value || int.TryParse(item["MESXDEP"].ToString(), out e) == false ? 0 : item["MESXDEP"]));
                    }
                    documento = item["NUM_TRN"].ToString().Trim();
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

                if (dataGridRefe.ItemsSource == null)
                {
                    MessageBox.Show("no hay datos para importar", "alerta", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (dataGridRefe.View.Records.Count <= 0)
                {
                    MessageBox.Show("no hay datos para importar", "alerta", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                //if (documentos() == true) return;

                #endregion

                #region insercion


                if (MessageBox.Show("Usted desea generar los documentos de retiros de activos?", "Documentos", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    string sql_cab = ""; string sql_cue = "";

                    foreach (DataTable dt_cue in doc_agru.Tables)
                    {
                        string cod_trn_cab = "999";
                        string num_trn_cab = dt_cue.Rows[0]["num_trn"].ToString();
                        string fecha = dt_cue.Rows[0]["fec_trn"].ToString();
                        DateTime date = Convert.ToDateTime(fecha);

                        sql_cab += @"INSERT INTO afcab_doc (cod_trn,fec_trn,num_trn,des_mov,_usu,ano_doc,per_doc) values ('" + cod_trn_cab + "','" + fecha + "','" + num_trn_cab + "','ECHO DESDE EL PROCESO DE IMPORTACION','" + usuario_name + "','" + date.Year + "','" + date.Month + "');DECLARE @NewID INT;SELECT @NewID = SCOPE_IDENTITY();";

                        foreach (System.Data.DataRow dtcue in dt_cue.Rows)
                        {
                            string num_trn = dtcue["num_trn"].ToString().Trim();
                            string cod_act = dtcue["cod_act"].ToString().Trim();
                            string cod_gru = dtcue["cod_gru"].ToString().Trim();
                            string doc_int = dtcue["doc_int"].ToString().Trim();
                            string cod_ter = dtcue["cod_ter"].ToString().Trim();
                            string cod_con = dtcue["cod_con"].ToString().Trim();
                            decimal vr_act = Convert.ToDecimal(dtcue["dep_act"]);
                            decimal dep_ac = Convert.ToDecimal(dtcue["dep_act"]);
                            int mesxdep = Convert.ToInt32(dtcue["mesxdep"]);

                            sql_cue += @"INSERT INTO afcue_doc (idregcab,cod_trn,num_trn,cod_act,cod_gru,doc_int,cod_ter,cod_con,vr_act,dep_ac,mesxdep) values (@NewID,'" + cod_trn_cab + "','" + num_trn + "','" + cod_act + "','" + cod_gru + "','" + doc_int + "','" + cod_ter + "','" + cod_con + "'," + vr_act.ToString("F", CultureInfo.InvariantCulture) + "," + dep_ac.ToString("F", CultureInfo.InvariantCulture) + "," + mesxdep + ");";
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

        public Tuple<string, string, string> getGrupo(string acti_tra)
        {
            string query = "select cod_act,Afmae_act.cod_gru,afmae_gru.cta_act  ";
            query += "From Afmae_act ";
            query += "inner join afmae_gru on afmae_gru.cod_gru = Afmae_act.cod_gru ";
            query += "where Afmae_act.cod_act='" + acti_tra + "' ";

            DataTable dt = SiaWin.Func.SqlDT(query, "cuerpo", idemp);
            return new Tuple<string, string, string>(
            dt.Rows.Count > 0 ? dt.Rows[0]["cod_act"].ToString().Trim() : "",
            dt.Rows.Count > 0 ? dt.Rows[0]["cod_gru"].ToString().Trim() : "",
            dt.Rows.Count > 0 ? dt.Rows[0]["cta_act"].ToString().Trim() : ""
            );
        }


        public void contabilizar()
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("cod_trn");
                dt.Columns.Add("num_trn");


                foreach (DataTable doc in doc_agru.Tables)
                    dt.Rows.Add("999", doc.Rows[0]["num_trn"].ToString());

                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    ContabilizaRetiro(dr["num_trn"].ToString().Trim(), dr["cod_trn"].ToString().Trim());
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al generar documento contable:" + w);
            }
        }


        private int ContabilizaRetiro(string num_trn, string cod_trn)
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

                #endregion

                #region obtiene cuerpo

                string cuerpo_contable = "";

                string querycue = "select Afcue_doc.cod_act,Afmae_act.vr_act,Afcue_doc.cod_ter,Afcue_doc.doc_int,Afcue_doc.cod_con,Afcue_doc.act_tras, ";
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


                foreach (System.Data.DataRow item in dt_cuerpo.Rows)
                {


                    double valor_act = Convert.ToDouble(item["vr_act"]);
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



                    DataTable dt_depreciado = SiaWin.Func.SaldoActivo(cod_act, _fecdoc.ToString("dd/MM/yyyy"), idemp);


                    if (dt_depreciado.Rows.Count > 0)
                    {
                        double vr_act = Convert.ToDouble(dt_depreciado.Rows[0]["vr_act"]);
                        double depreciado = Convert.ToDouble(dt_depreciado.Rows[0]["depreciacion"]);
                        double faltante = vr_act - depreciado;

                        cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,cod_ter,des_mov,cre_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_act + "','" + doc_int + "','" + cod_ter + "','Retiro - " + cod_act + " '," + vr_act.ToString("F", CultureInfo.InvariantCulture) + "); ";

                        if (depreciado > 0)
                            cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,cod_ter,des_mov,deb_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_dep + "','" + doc_int + "','" + cod_ter + "','Retiro - " + cod_act + " '," + depreciado.ToString("F", CultureInfo.InvariantCulture) + "); ";

                        if (faltante > 0)
                            cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,cod_ter,des_mov,deb_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_per + "','" + doc_int + "','" + cod_ter + "','Retiro - " + cod_act + " '," + faltante.ToString("F", CultureInfo.InvariantCulture) + "); ";


                        if (cod_con == "51")
                        {
                            double val = faltante > 0 ? faltante : valor_act;
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
                            var t = getGrupo(act_tras);
                            cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,des_mov,deb_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + t.Item3 + "','" + doc_int + "','Adicion a: " + act_tras + " - Placa:" + cod_act + "'," + vr_act.ToString("F", CultureInfo.InvariantCulture) + "); ";
                            cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,des_mov,cre_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_gdp + "','" + doc_int + "','Adicion a: " + act_tras + " - Placa:" + cod_act + "'," + depreciado.ToString("F", CultureInfo.InvariantCulture) + "); ";
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


        private void DataGridRefe_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            try
            {
                if ((sender as SfDataGrid).SelectedIndex >= 0)
                {
                    var reflector = (sender as SfDataGrid).View.GetPropertyAccessProvider();
                    var rowData = (sender as SfDataGrid).GetRecordAtRowIndex((sender as SfDataGrid).SelectedIndex + 1);
                    Tx_tercero.Text = !string.IsNullOrEmpty(reflector.GetValue(rowData, "Nom_ter").ToString()) ? reflector.GetValue(rowData, "Nom_ter").ToString().ToUpper() : "NO EXISTE";
                    Tx_activo.Text = !string.IsNullOrEmpty(reflector.GetValue(rowData, "Nom_act").ToString()) ? reflector.GetValue(rowData, "Nom_act").ToString().ToUpper() : "NO EXISTE";
                    Tx_grupo.Text = !string.IsNullOrEmpty(reflector.GetValue(rowData, "Nom_gru").ToString()) ? reflector.GetValue(rowData, "Nom_gru").ToString().ToUpper() : "NO EXISTE";
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
                    dt.Rows.Add("999", doc.Rows[0]["NUM_TRN"].ToString(), doc.Rows[0]["FEC_TRN"].ToString(), cod_trncont);
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

        private void RemoveEmptyRows(DataTable source)
        {
            for (int i = source.Rows.Count; i >= 1; i--)
            {
                System.Data.DataRow currentRow = source.Rows[i - 1];
                foreach (var colValue in currentRow.ItemArray)
                {
                    if (!string.IsNullOrEmpty(colValue.ToString())) break;
                    source.Rows[i - 1].Delete();
                }
            }
        }

        public bool documentos()
        {
            bool flag = false;
            //foreach (var item in _DocAfijo)
            //{
            //    string num_trn = item.Num_trn.Trim();
            //    string query = "select * from afcab_doc where cod_trn='999' and num_trn='" + num_trn + "' ";
            //    System.Data.DataTable dt = SiaWin.Func.SqlDT(query, "tabla", idemp);
            //    if (dt.Rows.Count > 0)
            //    {
            //        string fec_trn = dt.Rows[0]["fec_trn"].ToString().Trim();
            //        MessageBox.Show("el documento:" + num_trn + " ya ha sido ingresado en la fecha:" + fec_trn, "alerta", MessageBoxButton.OK, MessageBoxImage.Warning);
            //        flag = true;
            //    }
            //}
            return flag;
        }










    }

}

