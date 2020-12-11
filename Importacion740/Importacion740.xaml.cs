using Importacion740;
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

    //   Sia.PublicarPnt(9668,"Importacion740");
    //   Sia.TabU(9668);


    public partial class Importacion740 : UserControl
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        string usuario_name = "";
        dynamic tabitem;

        DataTable dt = new DataTable();
        DataTable dt_errores = new DataTable();
        DataSet doc_agru = new DataSet();


        public Importacion740(dynamic tabitem1)
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
                tabitem.Title = "Importacion 740 - " + nomempresa;
                tabitem.Logo(idLogo, ".png");

                DataTable dt_use = SiaWin.Func.SqlDT("select UserName,UserAlias from Seg_User where UserId='" + SiaWin._UserId + "' ", "usuarios", 0);
                usuario_name = dt_use.Rows.Count > 0 ? dt_use.Rows[0]["username"].ToString().Trim() : "USUARIO INEXISTENTE";
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
                    worksheet.Range["A1"].Text = "COD_TRN";
                    worksheet.Range["B1"].Text = "NUM_TRN";
                    worksheet.Range["C1"].Text = "FEC_TRN";
                    worksheet.Range["D1"].Text = "COD_CTA";
                    worksheet.Range["E1"].Text = "COD_TER";
                    worksheet.Range["F1"].Text = "DES_MOV";
                    worksheet.Range["G1"].Text = "DOC_MOV";
                    worksheet.Range["H1"].Text = "BAS_MOV";
                    worksheet.Range["I1"].Text = "DEB_MOV";
                    worksheet.Range["J1"].Text = "CRE_MOV";
                    worksheet.Range["K1"].Text = "DOC_CRUC";
                    worksheet.Range["L1"].Text = "ORD_PAG";
                    worksheet.Range["M1"].Text = "COD_BANC";
                    worksheet.Range["N1"].Text = "FEC_VENC";
                    worksheet.Range["O1"].Text = "REG";
                    worksheet.Range["P1"].Text = "NUM_CHQ";
                    worksheet.Range["Q1"].Text = "FACTURA";
                    worksheet.Range["R1"].Text = "FEC_VEN";
                    worksheet.Range["S1"].Text = "COD_VEN";
                    worksheet.Range["T1"].Text = "COD_CIU";
                    worksheet.Range["U1"].Text = "COD_SUC";
                    worksheet.Range["V1"].Text = "COD_CCO";
                    worksheet.Range["W1"].Text = "DOC_REF";
                    worksheet.Range["X1"].Text = "FEC_SUSC";
                    worksheet.Range["A1:X1"].CellStyle.Font.Bold = true;

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
                MessageBox.Show("error al generar la plantilla:" + w);
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
            try
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
            catch (Exception w)
            {
                MessageBox.Show("ConvertExcelToDataTable:" + w);
                return null;
            }
        }

        public bool validarArchioExcel(DataTable dt)
        {
            bool flag = true;


            if (dt.Columns.Contains("COD_TRN") == false || dt.Columns.IndexOf("COD_TRN") != 0) flag = false;
            if (dt.Columns.Contains("NUM_TRN") == false || dt.Columns.IndexOf("NUM_TRN") != 1) flag = false;
            if (dt.Columns.Contains("FEC_TRN") == false || dt.Columns.IndexOf("FEC_TRN") != 2) flag = false;
            if (dt.Columns.Contains("COD_CTA") == false || dt.Columns.IndexOf("COD_CTA") != 3) flag = false;
            if (dt.Columns.Contains("COD_TER") == false || dt.Columns.IndexOf("COD_TER") != 4) flag = false;
            if (dt.Columns.Contains("DES_MOV") == false || dt.Columns.IndexOf("DES_MOV") != 5) flag = false;
            if (dt.Columns.Contains("DOC_MOV") == false || dt.Columns.IndexOf("DOC_MOV") != 6) flag = false;
            if (dt.Columns.Contains("BAS_MOV") == false || dt.Columns.IndexOf("BAS_MOV") != 7) flag = false;
            if (dt.Columns.Contains("DEB_MOV") == false || dt.Columns.IndexOf("DEB_MOV") != 8) flag = false;
            if (dt.Columns.Contains("CRE_MOV") == false || dt.Columns.IndexOf("CRE_MOV") != 9) flag = false;
            if (dt.Columns.Contains("DOC_CRUC") == false || dt.Columns.IndexOf("DOC_CRUC") != 10) flag = false;
            if (dt.Columns.Contains("ORD_PAG") == false || dt.Columns.IndexOf("ORD_PAG") != 11) flag = false;
            if (dt.Columns.Contains("COD_BANC") == false || dt.Columns.IndexOf("COD_BANC") != 12) flag = false;
            if (dt.Columns.Contains("FEC_VENC") == false || dt.Columns.IndexOf("FEC_VENC") != 13) flag = false;
            if (dt.Columns.Contains("REG") == false || dt.Columns.IndexOf("REG") != 14) flag = false;
            if (dt.Columns.Contains("NUM_CHQ") == false || dt.Columns.IndexOf("NUM_CHQ") != 15) flag = false;
            if (dt.Columns.Contains("FACTURA") == false || dt.Columns.IndexOf("FACTURA") != 16) flag = false;
            if (dt.Columns.Contains("FEC_VEN") == false || dt.Columns.IndexOf("FEC_VEN") != 17) flag = false;
            if (dt.Columns.Contains("COD_VEN") == false || dt.Columns.IndexOf("COD_VEN") != 18) flag = false;
            if (dt.Columns.Contains("COD_CIU") == false || dt.Columns.IndexOf("COD_CIU") != 19) flag = false;
            if (dt.Columns.Contains("COD_SUC") == false || dt.Columns.IndexOf("COD_SUC") != 20) flag = false;
            if (dt.Columns.Contains("COD_CCO") == false || dt.Columns.IndexOf("COD_CCO") != 21) flag = false;
            if (dt.Columns.Contains("DOC_REF") == false || dt.Columns.IndexOf("DOC_REF") != 22) flag = false;
            if (dt.Columns.Contains("FEC_SUSC") == false || dt.Columns.IndexOf("FEC_SUSC") != 23) flag = false;
            return flag;
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


                agruparDocumentos(dt);
                BtnImportar.IsEnabled = false;
                BtnGenerar.IsEnabled = false;
                BtnCrear.IsEnabled = false;


                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                var slowTask = Task<DataTable>.Factory.StartNew(() => Process(), source.Token);
                await slowTask;

                if (((DataTable)slowTask.Result).Rows.Count > 0)
                {
                    double deb_mov = Convert.ToDouble(((DataTable)slowTask.Result).Compute("Sum(DEB_MOV)", "").ToString());
                    double cre_mov = Convert.ToDouble(((DataTable)slowTask.Result).Compute("Sum(CRE_MOV)", "").ToString());
                    TxTot_deb.Text = deb_mov.ToString("N", CultureInfo.InvariantCulture);
                    TxTot_cre.Text = cre_mov.ToString("N", CultureInfo.InvariantCulture);
                    Txdif.Text = (deb_mov - cre_mov).ToString("N", CultureInfo.InvariantCulture);
                    dataGridRefe.ItemsSource = ((DataTable)slowTask.Result).DefaultView;
                }


                MessageBox.Show(Application.Current.MainWindow, "Importacion Exitosa", "alerta", MessageBoxButton.OK, MessageBoxImage.Information);

                Tx_total.Text = ((DataTable)slowTask.Result).Rows.Count.ToString();
                Tx_errores.Text = dt_errores.Rows.Count.ToString();

                BtnImportar.IsEnabled = true;
                BtnGenerar.IsEnabled = true;
                BtnCrear.IsEnabled = true;
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
                dd.Columns.Add("DOC_MOV");
                dd.Columns.Add("BAS_MOV");
                dd.Columns.Add("DEB_MOV", typeof(double));
                dd.Columns.Add("CRE_MOV", typeof(double));
                dd.Columns.Add("DOC_CRUC");
                dd.Columns.Add("ORD_PAG");
                dd.Columns.Add("COD_BANC");
                dd.Columns.Add("FEC_VENC");
                dd.Columns.Add("REG");
                dd.Columns.Add("NUM_CHQ");
                dd.Columns.Add("FACTURA");
                dd.Columns.Add("COD_VEN");
                dd.Columns.Add("COD_CIU");
                dd.Columns.Add("COD_SUC");
                dd.Columns.Add("COD_CCO");
                dd.Columns.Add("DOC_REF");
                dd.Columns.Add("FEC_SUSC");
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

                    if (val_ant_cod_trn == dr["COD_TRN"].ToString().Trim() && val_ant_doc_trn == dr["NUM_TRN"].ToString().Trim())
                    {
                        dd.Rows.Add(
                            dr["COD_TRN"].ToString(),
                            dr["NUM_TRN"].ToString(),
                            Convert.ToDateTime(dr["FEC_TRN"] == DBNull.Value || DateTime.TryParse(dr["FEC_TRN"].ToString(), out da) == false ? DateTime.Now.ToString("dd/MM/yyy") : dr["FEC_TRN"]).ToString("dd/MM/yyyy"),
                            dr["COD_CTA"].ToString(),
                            "",
                            dr["COD_TER"].ToString(),
                            "",
                            dr["DES_MOV"].ToString(),
                            dr["DOC_MOV"].ToString(),
                            baseco,
                            deb,
                            cre,
                            dr["DOC_CRUC"].ToString(),
                            dr["ORD_PAG"].ToString(),
                            dr["COD_BANC"].ToString(),
                            Convert.ToDateTime(dr["FEC_VENC"] == DBNull.Value || DateTime.TryParse(dr["FEC_VENC"].ToString(), out da) == false ? DateTime.Now.ToString("dd/MM/yyy") : dr["FEC_VENC"]).ToString("dd/MM/yyyy"),
                            dr["REG"].ToString(),
                            dr["NUM_CHQ"].ToString(),
                            dr["FACTURA"].ToString(),
                            dr["COD_VEN"].ToString(),
                            dr["COD_CIU"].ToString(),
                            dr["COD_SUC"].ToString(),
                            dr["COD_CCO"].ToString(),
                            dr["DOC_REF"].ToString(),
                            Convert.ToDateTime(dr["FEC_SUSC"] == DBNull.Value || DateTime.TryParse(dr["FEC_SUSC"].ToString(), out da) == false ? DateTime.Now.ToString("dd/MM/yyy") : dr["FEC_SUSC"]).ToString("dd/MM/yyyy")
                            );

                        if (i == sortedDT.Rows.Count) { doc_agru.Tables.Add(dd.Copy()); dd.Clear(); }//ultima columna
                    }
                    else
                    {
                        doc_agru.Tables.Add(dd.Copy()); dd.Clear();//agrega el documento completo a un datatable 

                        dd.Rows.Add(
                            dr["COD_TRN"].ToString(),
                            dr["NUM_TRN"].ToString(),
                            Convert.ToDateTime(dr["FEC_TRN"] == DBNull.Value || DateTime.TryParse(dr["FEC_TRN"].ToString(), out da) == false ? DateTime.Now.ToString("dd/MM/yyy") : dr["FEC_TRN"]).ToString("dd/MM/yyyy"),
                            dr["COD_CTA"].ToString(),
                            "",
                            dr["COD_TER"].ToString(),
                            "",
                            dr["DES_MOV"].ToString(),
                            dr["DOC_MOV"].ToString(),
                            baseco,
                            deb,
                            cre,
                            dr["DOC_CRUC"].ToString(),
                            dr["ORD_PAG"].ToString(),
                            dr["COD_BANC"].ToString(),
                            Convert.ToDateTime(dr["FEC_VENC"] == DBNull.Value || DateTime.TryParse(dr["FEC_VENC"].ToString(), out da) == false ? DateTime.Now.ToString("dd/MM/yyy") : dr["FEC_VENC"]).ToString("dd/MM/yyyy"),
                            dr["REG"].ToString(),
                            dr["NUM_CHQ"].ToString(),
                            dr["FACTURA"].ToString(),
                            dr["COD_VEN"].ToString(),
                            dr["COD_CIU"].ToString(),
                            dr["COD_SUC"].ToString(),
                            dr["COD_CCO"].ToString(),
                            dr["DOC_REF"].ToString(),
                            Convert.ToDateTime(dr["FEC_SUSC"] == DBNull.Value || DateTime.TryParse(dr["FEC_SUSC"].ToString(), out da) == false ? DateTime.Now.ToString("dd/MM/yyy") : dr["FEC_SUSC"]).ToString("dd/MM/yyyy")
                            );
                    }

                    val_ant_cod_trn = dr["COD_TRN"].ToString();
                    val_ant_doc_trn = dr["NUM_TRN"].ToString();
                }
                #endregion

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
                                System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el tercero  " + cod_ter + " no existe "; dt_errores.Rows.Add(row);
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
                                System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "la cuenta " + cod_cta + " si existe pero no es auxiliar"; dt_errores.Rows.Add(row);
                            }
                        }
                        else
                        {
                            dr["NOM_CTA"] = "";
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "la cuenta " + cod_cta + " no existe"; dt_errores.Rows.Add(row);
                        }

                        #endregion

                        #region codigo banco

                        string cod_ban = dr["COD_BANC"].ToString().Trim();

                        if (!string.IsNullOrWhiteSpace(cod_ban))
                        {
                            DataTable dt_banc = SiaWin.Func.SqlDT("select * from Comae_ban where cod_ban='" + cod_ban + "'  ", "bancos", idemp);
                            if (dt_banc.Rows.Count <= 0)
                            {
                                System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el codigo del banco " + cod_ban + " no existe "; dt_errores.Rows.Add(row);
                            }
                        }
                        #endregion

                        #region ciudad

                        string cod_ciu = dr["COD_CIU"].ToString().Trim();

                        if (!string.IsNullOrWhiteSpace(cod_ciu))
                        {
                            DataTable dt_ciu = SiaWin.Func.SqlDT("select * from Comae_ciu where cod_ciu='" + cod_ciu + "'  ", "ciudad", idemp);
                            if (dt_ciu.Rows.Count <= 0)
                            {
                                System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el codigo de la ciudad " + cod_ciu + " no existe "; dt_errores.Rows.Add(row);
                            }
                        }
                        #endregion

                        #region sucursal

                        string cod_suc = dr["COD_SUC"].ToString().Trim();

                        if (!string.IsNullOrWhiteSpace(cod_suc))
                        {
                            DataTable dt_suc = SiaWin.Func.SqlDT("select * from Comae_suc where cod_suc='" + cod_suc + "'  ", "sucursal", idemp);
                            if (dt_suc.Rows.Count <= 0)
                            {
                                System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el codigo de la sucursal " + cod_suc + " no existe "; dt_errores.Rows.Add(row);
                            }
                        }
                        #endregion

                        #region centro de costo

                        string cod_cco = dr["COD_CCO"].ToString().Trim();

                        if (!string.IsNullOrWhiteSpace(cod_cco))
                        {
                            DataTable dt_cco = SiaWin.Func.SqlDT("select * from Comae_cco where cod_cco='" + cod_cco + "'  ", "ccosto", idemp);
                            if (dt_cco.Rows.Count <= 0)
                            {
                                System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el codigo del centro de costo " + cod_cco + " no existe "; dt_errores.Rows.Add(row);
                            }
                        }
                        #endregion

                        #region descripcion

                        if (!string.IsNullOrEmpty(dr["DES_MOV"].ToString()))
                        {
                            string des_mov = dr["DES_MOV"].ToString();
                            if (des_mov.Length > 300)
                            {
                                System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "la descripcion no puede ser mayor a 300 caracteres : " + cod_trn + "-" + num_trn + "  "; dt_errores.Rows.Add(row);
                            }
                        }



                        #endregion

                        #region debito y credito

                        debito += Convert.ToDouble(dr["DEB_MOV"]);
                        credito += Convert.ToDouble(dr["CRE_MOV"]);
                        #endregion
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

                        sql_cab += @"INSERT INTO cocab_doc (cod_trn,num_trn,fec_trn,detalle) values ('" + cod_trn_cab + "','" + num_trn_cab + "','" + fec_trn_cab + "','IMPORTACION EXCEL PROCESOS 740');DECLARE @NewID INT;SELECT @NewID = SCOPE_IDENTITY();";


                        foreach (System.Data.DataRow dt in dt_cue.Rows)
                        {
                            string cod_cta = dt["cod_cta"].ToString().Trim();
                            string cod_ter = dt["cod_ter"].ToString().Trim();
                            string des_mov = dt["des_mov"].ToString().Trim();
                            string doc_mov = dt["doc_mov"].ToString().Trim();
                            double bas_mov = Convert.ToDouble(dt["bas_mov"]);
                            double deb_mov = Convert.ToDouble(dt["deb_mov"]);
                            double cre_mov = Convert.ToDouble(dt["cre_mov"]);
                            string doc_cruc = dt["doc_cruc"].ToString().Trim();
                            string ord_pag = dt["ord_pag"].ToString().Trim();
                            string cod_banc = dt["cod_banc"].ToString().Trim();
                            string num_chq = dt["num_chq"].ToString().Trim();
                            string factura = dt["factura"].ToString().Trim();
                            string cod_ciu = dt["cod_ciu"].ToString().Trim();
                            string cod_suc = dt["cod_suc"].ToString().Trim();
                            string cod_cco = dt["cod_cco"].ToString().Trim();
                            string doc_ref = dt["doc_ref"].ToString().Trim();
                            string fec_susc = dt["fec_susc"].ToString().Trim();


                            sql_cue += @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,cod_ciu,cod_suc,cod_cco,des_mov,num_chq,doc_mov,doc_cruc,bas_mov,deb_mov,cre_mov) values (@NewID,'" + cod_trn_cab + "','" + num_trn_cab + "','" + cod_cta + "','" + cod_ter + "','" + cod_ciu + "','" + cod_suc + "','" + cod_cco + "','" + des_mov + "','" + num_chq + "','" + doc_mov + "','" + doc_cruc + "'," + bas_mov.ToString("F", CultureInfo.InvariantCulture) + "," + deb_mov.ToString("F", CultureInfo.InvariantCulture) + "," + cre_mov.ToString("F", CultureInfo.InvariantCulture) + ");";
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
                    Tx_total.Text = "0";
                    Tx_errores.Text = "0";
                    TxTot_deb.Text = "-";
                    TxTot_cre.Text = "-";
                    Txdif.Text = "-";
                    Tx_ter.Text = "";
                    Tx_cuen.Text = "";
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






    }
}
