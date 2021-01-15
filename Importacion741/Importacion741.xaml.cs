using Importacion741;
using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
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
    //Sia.PublicarPnt(9667,"Importacion741");
    //Sia.TabU(9667);

    public partial class Importacion741 : UserControl
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        string usuario_name = "";
        dynamic tabitem;

        DataTable dt = new DataTable();
        DataTable dt_armado = new DataTable();
        DataTable dt_errores = new DataTable();
        DataSet doc_agru = new DataSet();


        public Importacion741(dynamic tabitem1)
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
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                tabitem.Title = "Importacion 741 - " + nomempresa;
                tabitem.Logo(idLogo, ".png");

                DataTable dt_use = SiaWin.Func.SqlDT("select UserName,UserAlias from Seg_User where UserId='" + SiaWin._UserId + "' ", "usuarios", 0);
                usuario_name = dt_use.Rows.Count > 0 ? dt_use.Rows[0]["username"].ToString().Trim() : "USUARIO INEXISTENTE";
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
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
                MessageBox.Show("error  al importar:" + w);
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
            if (dt.Columns.Contains("TRN") == false || dt.Columns.IndexOf("TRN") != 0) flag = false;
            if (dt.Columns.Contains("CONSE") == false || dt.Columns.IndexOf("CONSE") != 1) flag = false;
            if (dt.Columns.Contains("AÑO") == false || dt.Columns.IndexOf("AÑO") != 2) flag = false;
            if (dt.Columns.Contains("MES") == false || dt.Columns.IndexOf("MES") != 3) flag = false;
            if (dt.Columns.Contains("DIA") == false || dt.Columns.IndexOf("DIA") != 4) flag = false;
            if (dt.Columns.Contains("ORD") == false || dt.Columns.IndexOf("ORD") != 5) flag = false;
            if (dt.Columns.Contains("NIT") == false || dt.Columns.IndexOf("NIT") != 6) flag = false;
            if (dt.Columns.Contains("RAZON SOCIAL") == false || dt.Columns.IndexOf("RAZON SOCIAL") != 7) flag = false;
            if (dt.Columns.Contains("CONTRATO") == false || dt.Columns.IndexOf("CONTRATO") != 8) flag = false;
            if (dt.Columns.Contains("objeto") == false || dt.Columns.IndexOf("objeto") != 9) flag = false;
            if (dt.Columns.Contains("CCO") == false || dt.Columns.IndexOf("CCO") != 10) flag = false;
            if (dt.Columns.Contains("INVERSION") == false || dt.Columns.IndexOf("INVERSION") != 11) flag = false;
            return flag;
        }

        public async void impotar()
        {

            try
            {

                string root = "";
                OpenFileDialog openfile = new OpenFileDialog();
                openfile.DefaultExt = ".xlsx";
                openfile.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
                var browsefile = openfile.ShowDialog();
                root = openfile.FileName;

                if (string.IsNullOrEmpty(root)) return;

                sfBusyIndicator.IsBusy = true;
                dt.Clear();
                dt_errores.Clear();

                dt = ConvertExcelToDataTable(root);
                if (dt == null) { sfBusyIndicator.IsBusy = false; return; }

                if (validarArchioExcel(dt) == false)
                {
                    MessageBox.Show("La plantilla importada no corresponde a la que permite el sistema por favor verifique con la plantilla que genera esta pantalla", "alerta", MessageBoxButton.OK, MessageBoxImage.Error);
                    sfBusyIndicator.IsBusy = false;
                    dataGridRefe.ItemsSource = null;
                    dt.Clear(); dt_errores.Clear();
                    Tx_ter.Text = "";
                    Tx_cuen.Text = "";
                    TxTot_deb.Text = "-";
                    TxTot_cre.Text = "-";
                    Txdif.Text = "-";
                    Tx_total.Text = "0";
                    Tx_errores.Text = "0";
                    return;
                }

                BtnImportar.IsEnabled = false;
                BtnCrear.IsEnabled = false;

                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;

                var TaskOne = Task.Factory.StartNew(() => RearmarDoc(dt), source.Token);
                await TaskOne;

                if (TaskOne.IsCompleted)
                {
                    var TaskTwo = Task.Factory.StartNew(() => agruparDocumentos(dt_armado), source.Token);
                    await TaskTwo;

                    if (TaskTwo.IsCompleted)
                    {
                        var TaskThree = Task<DataTable>.Factory.StartNew(() => Process(), source.Token);
                        await TaskThree;
                        if (((DataTable)TaskThree.Result).Rows.Count > 0)
                        {
                            double deb_mov = Convert.ToDouble(((DataTable)TaskThree.Result).Compute("Sum(DEB_MOV)", "").ToString());
                            double cre_mov = Convert.ToDouble(((DataTable)TaskThree.Result).Compute("Sum(CRE_MOV)", "").ToString());
                            TxTot_deb.Text = deb_mov.ToString("N", CultureInfo.InvariantCulture);
                            TxTot_cre.Text = cre_mov.ToString("N", CultureInfo.InvariantCulture);
                            Txdif.Text = (deb_mov - cre_mov).ToString("N", CultureInfo.InvariantCulture);
                            dataGridRefe.ItemsSource = ((DataTable)TaskThree.Result).DefaultView;
                            Tx_total.Text = ((DataTable)TaskThree.Result).Rows.Count.ToString();
                            sfBusyIndicator.IsBusy = false;
                        }
                    }

                }

                
                MessageBox.Show(Application.Current.MainWindow, "Importacion Exitosa", "alerta", MessageBoxButton.OK, MessageBoxImage.Information);
                Tx_errores.Text = dt_errores.Rows.Count.ToString();
                sfBusyIndicator.IsBusy = false;
                BtnImportar.IsEnabled = true;
                BtnCrear.IsEnabled = true;

            }
            catch (Exception w)
            {
                MessageBox.Show("error al importar:" + w);
                sfBusyIndicator.IsBusy = false;
            }

        }


        public void RearmarDoc(DataTable dt)
        {
            try
            {

                //SiaWin.Browse(dt);
                dt_armado = new DataTable();
                dt_armado.Columns.Add("COD_TRN");
                dt_armado.Columns.Add("NUM_TRN");
                dt_armado.Columns.Add("ANO_DOC");
                dt_armado.Columns.Add("PER_DOC");
                dt_armado.Columns.Add("DIA_DOC");
                dt_armado.Columns.Add("COD_CTA");
                dt_armado.Columns.Add("COD_TER");
                dt_armado.Columns.Add("DES_MOV");
                dt_armado.Columns.Add("BAS_MOV", typeof(double));
                dt_armado.Columns.Add("DEB_MOV", typeof(double));
                dt_armado.Columns.Add("CRE_MOV", typeof(double));

                double dou;
                foreach (System.Data.DataRow dr in dt.Rows)
                {

                    #region variables debito

                    double debito1 = dr[11] == DBNull.Value || double.TryParse(dr[11].ToString(), out dou) == false ? 0 : Convert.ToDouble(dr[11]);//INVERSION
                    double debito2 = dr[12] == DBNull.Value || double.TryParse(dr[12].ToString(), out dou) == false ? 0 : Convert.ToDouble(dr[12]);//FUNCIO
                    double debito3 = dr[13] == DBNull.Value || double.TryParse(dr[13].ToString(), out dou) == false ? 0 : Convert.ToDouble(dr[13]);//DEBITO_3
                    double debito4 = dr[14] == DBNull.Value || double.TryParse(dr[14].ToString(), out dou) == false ? 0 : Convert.ToDouble(dr[14]);//DEBITO_4
                    double debito5 = dr[15] == DBNull.Value || double.TryParse(dr[15].ToString(), out dou) == false ? 0 : Convert.ToDouble(dr[15]);//DEBITO_5
                    double debito6 = dr[16] == DBNull.Value || double.TryParse(dr[16].ToString(), out dou) == false ? 0 : Convert.ToDouble(dr[16]);//VALOR DEBITO CTA DE ORDEN


                    string cta_deb1 = dr[32].ToString().Trim();//CTA. INVERS
                    string cta_deb2 = dr[33].ToString().Trim();//CTA. FUNCIO.
                    string cta_deb3 = dr[34].ToString().Trim();//CTADEBITO_3
                    string cta_deb4 = dr[35].ToString().Trim();//CTADEBITO_4
                    string cta_deb5 = dr[36].ToString().Trim();//CTADEBITO_5
                    string cta_deb6 = dr[37].ToString().Trim();//CTA_ORDEN DEBITO

                    #endregion

                    #region variables credito

                    double credito1 = dr[19] == DBNull.Value || double.TryParse(dr[19].ToString(), out dou) == false ? 0 : Convert.ToDouble(dr[19]);//CRED.1 RETEFU.
                    double credito2 = dr[21] == DBNull.Value || double.TryParse(dr[21].ToString(), out dou) == false ? 0 : Convert.ToDouble(dr[21]);//CRED.2 RETEIVA
                    double credito3 = dr[23] == DBNull.Value || double.TryParse(dr[23].ToString(), out dou) == false ? 0 : Convert.ToDouble(dr[23]);//CRED.3 RETEICA
                    double credito4 = dr[24] == DBNull.Value || double.TryParse(dr[24].ToString(), out dou) == false ? 0 : Convert.ToDouble(dr[24]);//CRED.4 U.DISTRIT
                    double credito5 = dr[25] == DBNull.Value || double.TryParse(dr[25].ToString(), out dou) == false ? 0 : Convert.ToDouble(dr[25]);//CRED.5 PROCUL
                    double credito6 = dr[26] == DBNull.Value || double.TryParse(dr[26].ToString(), out dou) == false ? 0 : Convert.ToDouble(dr[26]);//CRED.6 PROANCI
                    double credito7 = dr[27] == DBNull.Value || double.TryParse(dr[27].ToString(), out dou) == false ? 0 : Convert.ToDouble(dr[27]);//CRED.7 OBRA,salud,pen
                    double credito8 = dr[28] == DBNull.Value || double.TryParse(dr[28].ToString(), out dou) == false ? 0 : Convert.ToDouble(dr[28]);//CRED.8 CREE PEDAGOGICA
                    double credito9 = dr[29] == DBNull.Value || double.TryParse(dr[29].ToString(), out dou) == false ? 0 : Convert.ToDouble(dr[29]);//NETO INVERS.
                    double credito10 = dr[30] == DBNull.Value || double.TryParse(dr[30].ToString(), out dou) == false ? 0 : Convert.ToDouble(dr[30]);//NETO FUNCIO
                    double credito11 = dr[31] == DBNull.Value || double.TryParse(dr[31].ToString(), out dou) == false ? 0 : Convert.ToDouble(dr[31]);//VALOR CREDITO ORDEN

                    string cta_cre1 = dr[38].ToString().Trim();//CTACREDITO_1 RETEFUENTE
                    string cta_cre2 = dr[39].ToString().Trim();//CTACREDITO_2 RTEIVA
                    string cta_cre3 = dr[40].ToString().Trim();//CTACREDITO_3 RETEICA
                    string cta_cre4 = dr[41].ToString().Trim();//CTACREDITO_4 U. DISTRITAL
                    string cta_cre5 = dr[42].ToString().Trim();//CTACREDITO_5 PROCULTURA
                    string cta_cre6 = dr[43].ToString().Trim();//CTACREDITO_6 PROANCIANOS
                    string cta_cre7 = dr[44].ToString().Trim();//CTACREDITO_7 RETENC.OBRA
                    string cta_cre8 = dr[45].ToString().Trim();//CTACREDITO_8 RETENC.PEDAG NAL
                    string cta_cre9 = dr[46].ToString().Trim();//PAS.INVERS.
                    string cta_cre10 = dr[47].ToString().Trim();//PAS.FUNCIO.
                    string cta_cre11 = dr[48].ToString().Trim();//CTA_ORDEN CREDITO

                    #endregion

                    string cod_trn = dr[0].ToString().Trim();//TRN
                    string num_trn = dr[1].ToString().Trim();//CONSE
                    string ano_doc = dr[2].ToString().Trim();//AÑO
                    string per_doc = dr[3].ToString().Trim();//MES
                    string dia_doc = dr[4].ToString().Trim();//DIA
                    string cod_ter = dr[6].ToString().Trim();//NIT
                    string des_mov = dr[9].ToString().Trim();//objeto

                    double basen = dr[17] == DBNull.Value || double.TryParse(dr[17].ToString(), out dou) == false ? 0 : Convert.ToDouble(dr[17]); //BASE RETENC.
                    double base1 = dr[18] == DBNull.Value || double.TryParse(dr[18].ToString(), out dou) == false ? 0 : Convert.ToDouble(dr[18]);//BASE ESTAMPILLAS
                    double baseriva = dr[20] == DBNull.Value || double.TryParse(dr[20].ToString(), out dou) == false ? 0 : Convert.ToDouble(dr[20]);//BASE IVA
                    double baserica = dr[22] == DBNull.Value || double.TryParse(dr[22].ToString(), out dou) == false ? 0 : Convert.ToDouble(dr[22]);//BASE ICA


                    #region debitos                    

                    if (debito1 > 0)
                        dt_armado.Rows.Add(cod_trn, num_trn, ano_doc, per_doc, dia_doc, cta_deb1, cod_ter, des_mov, 0, debito1, 0);

                    if (debito2 > 0)
                        dt_armado.Rows.Add(cod_trn, num_trn, ano_doc, per_doc, dia_doc, cta_deb2, cod_ter, des_mov, 0, debito2, 0);

                    if (debito3 > 0)
                        dt_armado.Rows.Add(cod_trn, num_trn, ano_doc, per_doc, dia_doc, cta_deb3, cod_ter, des_mov, 0, debito3, 0);

                    if (debito4 > 0)
                        dt_armado.Rows.Add(cod_trn, num_trn, ano_doc, per_doc, dia_doc, cta_deb4, cod_ter, des_mov, 0, debito4, 0);


                    if (debito5 > 0)
                        dt_armado.Rows.Add(cod_trn, num_trn, ano_doc, per_doc, dia_doc, cta_deb5, cod_ter, des_mov, 0, debito5, 0);

                    if (debito6 > 0)
                        dt_armado.Rows.Add(cod_trn, num_trn, ano_doc, per_doc, dia_doc, cta_deb6, cod_ter, des_mov, 0, debito6, 0);

                    #endregion

                    #region creditos

                    if (credito1 > 0)
                        dt_armado.Rows.Add(cod_trn, num_trn, ano_doc, per_doc, dia_doc, cta_cre1, cod_ter, des_mov, basen, 0, credito1);

                    if (credito2 > 0)
                        dt_armado.Rows.Add(cod_trn, num_trn, ano_doc, per_doc, dia_doc, cta_cre2, cod_ter, des_mov, baseriva, 0, credito2);

                    if (credito3 > 0)
                        dt_armado.Rows.Add(cod_trn, num_trn, ano_doc, per_doc, dia_doc, cta_cre3, cod_ter, des_mov, baserica, 0, credito3);

                    if (credito4 > 0)
                        dt_armado.Rows.Add(cod_trn, num_trn, ano_doc, per_doc, dia_doc, cta_cre4, cod_ter, des_mov, base1, 0, credito4);

                    if (credito5 > 0)
                        dt_armado.Rows.Add(cod_trn, num_trn, ano_doc, per_doc, dia_doc, cta_cre5, cod_ter, des_mov, base1, 0, credito5);

                    if (credito6 > 0)
                        dt_armado.Rows.Add(cod_trn, num_trn, ano_doc, per_doc, dia_doc, cta_cre6, cod_ter, des_mov, base1, 0, credito6);

                    if (credito7 > 0)
                        dt_armado.Rows.Add(cod_trn, num_trn, ano_doc, per_doc, dia_doc, cta_cre7, cod_ter, des_mov, basen, 0, credito7);

                    if (credito8 > 0)
                        dt_armado.Rows.Add(cod_trn, num_trn, ano_doc, per_doc, dia_doc, cta_cre8, cod_ter, des_mov, basen, 0, credito8);

                    if (credito9 > 0)
                        dt_armado.Rows.Add(cod_trn, num_trn, ano_doc, per_doc, dia_doc, cta_cre9, cod_ter, des_mov, basen, 0, credito9);

                    if (credito10 > 0)
                        dt_armado.Rows.Add(cod_trn, num_trn, ano_doc, per_doc, dia_doc, cta_cre10, cod_ter, des_mov, basen, 0, credito10);

                    if (credito11 > 0)
                        dt_armado.Rows.Add(cod_trn, num_trn, ano_doc, per_doc, dia_doc, cta_cre11, cod_ter, des_mov, basen, 0, credito11);
                    #endregion


                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al rearmar la importacion:" + w);
            }
        }

        public void agruparDocumentos(DataTable dt)
        {
            try
            {
                DataView dv = dt.DefaultView;
                dv.Sort = "NUM_TRN desc";
                DataTable sortedDT = dv.ToTable();
                doc_agru.Tables.Clear();
                //SiaWin.Browse(sortedDT);


                #region algortimo el cual mete en un dataset los documentos separados por datatable                
                DataTable dd = new DataTable();

                #region columnas                
                dd.Columns.Add("COD_TRN");
                dd.Columns.Add("NUM_TRN");
                dd.Columns.Add("FEC_TRN");
                dd.Columns.Add("COD_CTA");
                dd.Columns.Add("NOM_CTA");
                dd.Columns.Add("COD_TER");
                dd.Columns.Add("NOM_TER");
                dd.Columns.Add("DES_MOV");
                dd.Columns.Add("BAS_MOV");
                dd.Columns.Add("DEB_MOV", typeof(double));
                dd.Columns.Add("CRE_MOV", typeof(double));
                //dd.Columns.Add("DOC_CRUC");                                             
                #endregion


                DateTime da; int i = 0; double dou;
                string val_ant_doc_trn = "", val_ant_cod_trn = "";
                foreach (System.Data.DataRow dr in sortedDT.Rows)
                {
                    i++;

                    double deb = dr["DEB_MOV"] == DBNull.Value || double.TryParse(dr["DEB_MOV"].ToString(), out dou) == false ? 0 : Convert.ToDouble(dr["DEB_MOV"]);
                    double cre = dr["CRE_MOV"] == DBNull.Value || double.TryParse(dr["CRE_MOV"].ToString(), out dou) == false ? 0 : Convert.ToDouble(dr["CRE_MOV"]);
                    double baseco = dr["BAS_MOV"] == DBNull.Value || double.TryParse(dr["BAS_MOV"].ToString(), out dou) == false ? 0 : Convert.ToDouble(dr["BAS_MOV"]);

                    if (string.IsNullOrEmpty(val_ant_doc_trn) && string.IsNullOrEmpty(val_ant_cod_trn))
                    {
                        val_ant_cod_trn = dr["COD_TRN"].ToString().Trim();
                        val_ant_doc_trn = dr["NUM_TRN"].ToString().Trim();
                    }

                    int val;
                    int dia = Convert.ToInt32(dr["DIA_DOC"] == DBNull.Value || int.TryParse(dr["DIA_DOC"].ToString(), out val) == false ? 1 : dr["DIA_DOC"]);
                    int mes = Convert.ToInt32(dr["PER_DOC"] == DBNull.Value || int.TryParse(dr["PER_DOC"].ToString(), out val) == false ? 1 : dr["PER_DOC"]);
                    int año = Convert.ToInt32(dr["ANO_DOC"] == DBNull.Value || int.TryParse(dr["ANO_DOC"].ToString(), out val) == false ? 2000 : dr["ANO_DOC"]);
                    DateTime fecha = new DateTime(año, mes, dia);


                    if (val_ant_cod_trn == dr["COD_TRN"].ToString().Trim() && val_ant_doc_trn == dr["NUM_TRN"].ToString().Trim())
                    {
                        dd.Rows.Add(
                            dr["COD_TRN"].ToString(),
                            dr["NUM_TRN"].ToString(),
                            fecha,
                            dr["COD_CTA"].ToString(),
                            "",
                            dr["COD_TER"].ToString(),
                            "",
                            dr["DES_MOV"].ToString(),
                            baseco,
                            deb,
                            cre
                            );

                        if (i == sortedDT.Rows.Count) { doc_agru.Tables.Add(dd.Copy()); dd.Clear(); }//ultima columna
                    }
                    else
                    {
                        doc_agru.Tables.Add(dd.Copy()); dd.Clear();//agrega el documento completo a un datatable 

                        dd.Rows.Add(
                            dr["COD_TRN"].ToString(),
                            dr["NUM_TRN"].ToString(),
                            fecha,
                            dr["COD_CTA"].ToString(),
                            "",
                            dr["COD_TER"].ToString(),
                            "",
                            dr["DES_MOV"].ToString(),
                            baseco,
                            deb,
                            cre
                            );
                    }

                    val_ant_cod_trn = dr["COD_TRN"].ToString();
                    val_ant_doc_trn = dr["NUM_TRN"].ToString();
                }
                #endregion


                //foreach (DataTable dtable in doc_agru.Tables)
                //{
                //  SiaWin.Browse(dtable);
                //}

            }
            catch (Exception w)
            {
                MessageBox.Show("error " + w);
            }
        }

        private DataTable Process()
        {
            try
            {
                int linea = 1;
                //VALIDAR DOCUMENTO si existe
                foreach (DataTable dtemp in doc_agru.Tables)
                {
                    string cod_trn = dtemp.Rows[0]["COD_TRN"].ToString().Trim();
                    string num_trn = dtemp.Rows[0]["NUM_TRN"].ToString().Trim();

                    DataTable dt_trn = SiaWin.Func.SqlDT("select * from cocab_doc where cod_trn='" + cod_trn + "' and num_trn='" + num_trn + "' ", "contabilidad", idemp);
                    if (dt_trn.Rows.Count > 0) { System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el documento " + num_trn + "- COD_TRN:" + cod_trn + " ya existe registrado"; dt_errores.Rows.Add(row); }

                    DataTable dt_codtrn = SiaWin.Func.SqlDT("select * from comae_trn where cod_trn='" + cod_trn + "'  ", "transaccion", idemp);
                    if (dt_codtrn.Rows.Count <= 0) { System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "la transaccion " + cod_trn + " no existe "; dt_errores.Rows.Add(row); }

                    double debito = 0;
                    double credito = 0;

                    //validar campo por campo                    
                    foreach (System.Data.DataRow dr in dtemp.Rows)
                    {
                        #region tercero

                        string cod_ter = dr["COD_TER"].ToString().Trim();
                        if (!string.IsNullOrEmpty(cod_ter))
                        {
                            DataTable dt_ter = SiaWin.Func.SqlDT("select cod_ter,nom_ter from comae_ter where cod_ter='" + cod_ter + "'  ", "tercero", idemp);
                            if (dt_ter.Rows.Count > 0)
                            {
                                dr["NOM_TER"] = dt_ter.Rows[0]["nom_ter"].ToString().Trim();
                            }
                            else
                            {
                                dr["NOM_TER"] = "";
                                System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el tercero  " + cod_ter + " no existe (ERROR EN LA LINEA " + linea + ")"; dt_errores.Rows.Add(row);
                            }
                        }

                        #endregion

                        #region cuenta

                        string cod_cta = dr["COD_CTA"].ToString().Trim();
                        DataTable dt_cta = SiaWin.Func.SqlDT("select cod_cta,nom_cta,tip_cta from comae_cta where cod_cta='" + cod_cta + "' ", "cuenta", idemp);

                        if (dt_cta.Rows.Count > 0)
                        {
                            string tipo = dt_cta.Rows[0]["tip_cta"].ToString().Trim();
                            if (tipo == "A")
                            {
                                dr["NOM_CTA"] = dt_cta.Rows[0]["nom_cta"].ToString().Trim();
                            }
                            else
                            {
                                dr["NOM_CTA"] = "";
                                System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "la cuenta " + cod_cta + " si existe pero no es auxiliar (ERROR EN LA LINEA " + linea + ")"; dt_errores.Rows.Add(row);
                            }
                        }
                        else
                        {
                            dr["NOM_CTA"] = "";
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "la cuenta " + cod_cta + " no existe (ERROR EN LA LINEA " + linea + ")"; dt_errores.Rows.Add(row);
                        }

                        #endregion

                        #region descripcion

                        if (!string.IsNullOrEmpty(dr["DES_MOV"].ToString()))
                        {
                            string des_mov = dr["DES_MOV"].ToString();
                            if (des_mov.Length > 300)
                            {
                                System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "la descripcion no puede ser mayor a 300 caracteres : " + cod_trn + "-" + num_trn + " (ERROR EN LA LINEA " + linea + ") "; dt_errores.Rows.Add(row);
                            }
                        }



                        #endregion

                        #region debito y credito

                        debito += Convert.ToDouble(dr["DEB_MOV"]);
                        credito += Convert.ToDouble(dr["CRE_MOV"]);
                        #endregion

                        linea++;
                    }

                    if (debito != credito)
                    {
                        System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el debito o el credito se encuentran descuadrados:" + cod_trn + "-" + num_trn + "  "; dt_errores.Rows.Add(row);
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

                if (MessageBox.Show("Usted desea generar los documentos de la importados realizada?", "Documentos", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    string sql_cab = ""; string sql_cue = "";

                    foreach (DataTable dt_cue in doc_agru.Tables)
                    {

                        string cod_trn_cab = dt_cue.Rows[0]["cod_trn"].ToString();
                        string num_trn_cab = dt_cue.Rows[0]["num_trn"].ToString();
                        string fec_trn_cab = dt_cue.Rows[0]["fec_trn"].ToString();

                        sql_cab += @"INSERT INTO cocab_doc (cod_trn,num_trn,fec_trn,detalle) values ('" + cod_trn_cab + "','" + num_trn_cab + "','" + fec_trn_cab + "','IMPORTACION EXCEL PROCESOS 741');DECLARE @NewID INT;SELECT @NewID = SCOPE_IDENTITY();";


                        foreach (System.Data.DataRow dt in dt_cue.Rows)
                        {
                            string cod_cta = dt["cod_cta"].ToString().Trim();
                            string cod_ter = dt["cod_ter"].ToString().Trim();
                            string des_mov = dt["des_mov"].ToString().Trim();
                            double bas_mov = Convert.ToDouble(dt["bas_mov"]);
                            double deb_mov = Convert.ToDouble(dt["deb_mov"]);
                            double cre_mov = Convert.ToDouble(dt["cre_mov"]);


                            sql_cue += @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,des_mov,bas_mov,deb_mov,cre_mov) values (@NewID,'" + cod_trn_cab + "','" + num_trn_cab + "','" + cod_cta + "','" + cod_ter + "','" + des_mov + "'," + bas_mov.ToString("F", CultureInfo.InvariantCulture) + "," + deb_mov.ToString("F", CultureInfo.InvariantCulture) + "," + cre_mov.ToString("F", CultureInfo.InvariantCulture) + ");";
                        }

                        string query = sql_cab + sql_cue;
                        //MessageBox.Show(query);

                        if (SiaWin.Func.SqlCRUD(query, idemp) == false) { MessageBox.Show("se genero un error en un documento por favor consulte"); }
                        sql_cab = ""; sql_cue = "";
                    }

                    AbrirDocGenerados();

                    dataGridRefe.ItemsSource = null;
                    doc_agru.Tables.Clear();
                    dt_errores.Clear();
                    TxTot_deb.Text = "";
                    TxTot_cre.Text = "";
                    Txdif.Text = "";
                    Tx_cuen.Text = "";
                    Tx_ter.Text = "";
                    Tx_ter.Text = "";
                    Tx_total.Text = "";

                }
                #endregion


            }
            catch (Exception w)
            {
                MessageBox.Show("error al generar transacciones:" + w);
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

                foreach (DataTable doc in doc_agru.Tables)
                {
                    dt.Rows.Add(doc.Rows[0]["COD_TRN"].ToString(), doc.Rows[0]["NUM_TRN"].ToString(), doc.Rows[0]["FEC_TRN"].ToString());
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



        private void DataGridRefe_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            try
            {
                if ((sender as SfDataGrid).SelectedIndex >= 0)
                {
                    var reflector = (sender as SfDataGrid).View.GetPropertyAccessProvider();
                    var rowData = (sender as SfDataGrid).GetRecordAtRowIndex((sender as SfDataGrid).SelectedIndex + 1);
                    Tx_ter.Text = !string.IsNullOrEmpty(reflector.GetValue(rowData, "Nom_ter").ToString()) ? reflector.GetValue(rowData, "Nom_ter").ToString().ToUpper() : "---";
                    Tx_cuen.Text = !string.IsNullOrEmpty(reflector.GetValue(rowData, "Nom_cta").ToString()) ? reflector.GetValue(rowData, "Nom_cta").ToString().ToUpper() : "---";
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("errro al seleccionar:" + w);
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
