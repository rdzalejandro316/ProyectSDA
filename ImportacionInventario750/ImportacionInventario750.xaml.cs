using ImportacionInventario750;
using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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

    //Sia.PublicarPnt(9677,"ImportacionInventario750");
    //Sia.TabU(9677);

    public partial class ImportacionInventario750 : UserControl
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        dynamic tabitem;

        string cabeza = "incab_doc";
        string cuerpo = "incue_doc";
        string transaccion = "inmae_trn";

        DataTable dt_errores = new DataTable();
        DataSet doc_agru = new DataSet();

        public ImportacionInventario750(dynamic tabitem1)
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
                tabitem.Title = "Importacion de documentos " + "-" + nomempresa;
                tabitem.Logo(idLogo, ".png");
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
                MessageBox.Show("error en la importacion" + w);
            }
        }

        public async void impotar()
        {
            try
            {

                DataTable dt = new DataTable();

                OpenFileDialog openfile = new OpenFileDialog();
                openfile.DefaultExt = ".xlsx";
                openfile.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
                var browsefile = openfile.ShowDialog();
                string root = openfile.FileName;

                if (string.IsNullOrEmpty(root)) return;
                sfBusyIndicator.IsBusy = true;
                if (dt.Rows.Count > 0) dt.Clear();
                if (dt_errores.Rows.Count > 0) dt_errores.Clear();
                dataGridRefe.ItemsSource = null;

                dt = ConvertExcelToDataTable(root);

                if (dt == null) { sfBusyIndicator.IsBusy = false; return; }

                if (validarArchioExcel(dt) == false)
                {
                    MessageBox.Show("La plantilla importada no corresponde a la que permite el sistema por favor verifique con la plantilla que genera esta pantalla", "alerta", MessageBoxButton.OK, MessageBoxImage.Error);
                    sfBusyIndicator.IsBusy = false;
                    return;
                }


                agruparDocumentos(dt);

                BtnCrear.IsEnabled = false;
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                var slowTask = Task<(DataTable, decimal, decimal)>.Factory.StartNew(() => Process());
                await slowTask;


                if ((slowTask.Result).Item1.Rows.Count > 0)
                {
                    decimal cos_uni = Convert.ToDecimal((slowTask.Result).Item2);
                    decimal cos_tot = Convert.ToDecimal((slowTask.Result).Item3);
                    TxTot_cosuni.Text = cos_uni.ToString("N", CultureInfo.InvariantCulture);
                    TxTot_costot.Text = cos_tot.ToString("N", CultureInfo.InvariantCulture);
                    dataGridRefe.ItemsSource = (slowTask.Result).Item1.DefaultView;
                }


                MessageBox.Show(Application.Current.MainWindow, "Importacion Exitosa", "alerta", MessageBoxButton.OK, MessageBoxImage.Information);

                Tx_total.Text = (slowTask.Result).Item1.Rows.Count.ToString();
                Tx_errores.Text = dt_errores.Rows.Count.ToString();

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
            catch (IOException w)
            {
                MessageBox.Show("el archivo esta abiero tiene que cerrarlo para poder importarlo", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return null;
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


            if (dt.Columns.Contains("Cod_trn") == false || dt.Columns.IndexOf("Cod_trn") != 0) flag = false;
            if (dt.Columns.Contains("Num_trn") == false || dt.Columns.IndexOf("Num_trn") != 1) flag = false;
            if (dt.Columns.Contains("Fec_trn") == false || dt.Columns.IndexOf("Fec_trn") != 2) flag = false;
            if (dt.Columns.Contains("Cod_ter") == false || dt.Columns.IndexOf("Cod_ter") != 3) flag = false;
            if (dt.Columns.Contains("Cod_ref") == false || dt.Columns.IndexOf("Cod_ref") != 4) flag = false;
            if (dt.Columns.Contains("Factura") == false || dt.Columns.IndexOf("Factura") != 5) flag = false;
            if (dt.Columns.Contains("Cantidad") == false || dt.Columns.IndexOf("Cantidad") != 6) flag = false;
            if (dt.Columns.Contains("Cos_uni") == false || dt.Columns.IndexOf("Cos_uni") != 7) flag = false;
            if (dt.Columns.Contains("Cos_tot") == false || dt.Columns.IndexOf("Cos_tot") != 8) flag = false;
            if (dt.Columns.Contains("Cod_bod") == false || dt.Columns.IndexOf("Cod_bod") != 9) flag = false;
            return flag;
        }

        public DataTable Limpiar(DataTable dt)
        {
            DataTable dt1 = dt.Clone();
            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                System.Data.DataRow currentRow = dt.Rows[i];
                foreach (var colValue in currentRow.ItemArray)
                {
                    if (!string.IsNullOrEmpty(colValue.ToString()))
                    {
                        dt1.ImportRow(currentRow);
                        break;
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


                #region algortimo el cual mete en un dataset los documentos separados por datatable                
                DataTable dd = new DataTable();

                #region columnas                
                dd.Columns.Add("COD_TRN");
                dd.Columns.Add("NUM_TRN");
                dd.Columns.Add("FEC_TRN");
                dd.Columns.Add("COD_TER");
                dd.Columns.Add("NOM_TER");
                dd.Columns.Add("COD_REF");
                dd.Columns.Add("NOM_REF");
                dd.Columns.Add("FACTURA");
                dd.Columns.Add("CANTIDAD");
                dd.Columns.Add("COS_UNI");
                dd.Columns.Add("COS_TOT");
                dd.Columns.Add("COD_BOD");
                dd.Columns.Add("NOM_BOD");
                #endregion


                DateTime da; int i = 0; double dou;
                //transaccion agrupada
                DataTable dt_gb = sortedDT.AsEnumerable()
               .GroupBy(r => new { Col1 = r["COD_TRN"], Col2 = r["NUM_TRN"] })
               .Select(g =>
               {
                   var row = dt.NewRow();
                   row["COD_TRN"] = g.Key.Col1;
                   row["NUM_TRN"] = g.Key.Col2;
                   return row;
               })
               .CopyToDataTable();


                if (dt_gb.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt_gb.Rows)
                    {
                        string cod_trn = dr["COD_TRN"].ToString();
                        string num_trn = dr["NUM_TRN"].ToString();

                        DataRow[] result = sortedDT.Select("COD_TRN='" + cod_trn + "' AND NUM_TRN='" + num_trn + "'");

                        foreach (DataRow row in result)
                        {
                            string fec_trn = row["FEC_TRN"].ToString();
                            string cod_ter = row["COD_TER"].ToString();
                            string cod_ref = row["COD_REF"].ToString();
                            string factura = row["FACTURA"].ToString();
                            string cod_bod = row["COD_BOD"].ToString();
                            string cantidad = row["CANTIDAD"].ToString();
                            string cos_uni = row["COS_UNI"].ToString();
                            string cos_tot = row["COS_TOT"].ToString();

                            dd.Rows.Add(
                                       cod_trn,
                                       num_trn,
                                       fec_trn,
                                       cod_ter,
                                       "",//nombre de tercero
                                       cod_ref,
                                       "",// nombre de referencia
                                       factura,
                                       cantidad,
                                       cos_uni,
                                       cos_tot,
                                       cod_bod,
                                       ""//nombre de bodega
                                       );
                        }

                        //SiaWin.Browse(dd.Copy());
                        doc_agru.Tables.Add(dd.Copy());
                        dd.Clear();
                    }
                }


                #endregion


                //foreach (DataTable dat in doc_agru.Tables)
                //{
                //    SiaWin.Browse(dat);
                //}

            }
            catch (Exception w)
            {
                MessageBox.Show("error " + w);
            }
        }

        private (DataTable, decimal, decimal) Process()
        {
            try
            {
                decimal ttuni = 0;
                decimal tttot = 0;

                //VALIDAR DOCUMENTO si existe
                int linea = 1;
                foreach (DataTable dtemp in doc_agru.Tables)
                {

                    #region validacion documento

                    string cod_trn = dtemp.Rows[0]["COD_TRN"].ToString().Trim();
                    string num_trn = dtemp.Rows[0]["NUM_TRN"].ToString().Trim();

                    DataTable dt_trn = SiaWin.Func.SqlDT("select * from " + cabeza + " where cod_trn='" + cod_trn + "' and num_trn='" + num_trn + "' ", "cabeza", idemp);
                    if (dt_trn.Rows.Count > 0) { System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el documento " + num_trn + "- COD_TRN:" + cod_trn + " ya existe registrado"; dt_errores.Rows.Add(row); }

                    DataTable dt_codtrn = SiaWin.Func.SqlDT("select * from " + transaccion + " where cod_trn='" + cod_trn + "'  ", "transaccion", idemp);
                    if (dt_codtrn.Rows.Count <= 0) { System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "la transaccion " + cod_trn + " no existe "; dt_errores.Rows.Add(row); }

                    #endregion
                  
                    //validar campo por campo
                    foreach (System.Data.DataRow dr in dtemp.Rows)
                    {                        
                        #region tercero

                        string cod_ter = dr["COD_TER"].ToString().Trim();
                        if (!string.IsNullOrEmpty(cod_ter))
                        {
                            DataTable dt_ter = SiaWin.Func.SqlDT("select cod_ter,nom_ter from comae_ter where cod_ter='" + cod_ter + "'  ", "tercero", idemp);
                            if (dt_ter.Rows.Count > 0) dr["NOM_TER"] = dt_ter.Rows[0]["nom_ter"].ToString().Trim();
                            else
                            {
                                dr["NOM_TER"] = "";
                                dt_errores.Rows.Add("el tercero  " + cod_ter + " no existe  (ERROR EN LA LINEA " + linea + ") ");
                            }
                        }
                        #endregion

                        #region fechas 

                        string fectrn = dr["FEC_TRN"].ToString().Trim();
                        DateTime dt;
                        if (string.IsNullOrEmpty(fectrn))
                        {
                            dt_errores.Rows.Add("el campo fecha de transaccion debe de estar lleno:" + num_trn + "  (ERROR EN LA LINEA " + linea + ")");
                        }
                        else
                        {
                            if (DateTime.TryParse(fectrn, out dt) == false)
                                dt_errores.Rows.Add("el campo fec_trn debe de ser una fecha:" + num_trn + " (ERROR EN LA LINEA " + linea + ")");
                        }


                        #endregion

                        #region referencia
                        string cod_ref = dr["COD_REF"].ToString().Trim();
                        DataTable dt_ref = SiaWin.Func.SqlDT("select cod_ref,nom_ref from inmae_ref where cod_ref='" + cod_ref + "' ", "referencia", idemp);

                        if (dt_ref.Rows.Count > 0) dr["NOM_REF"] = dt_ref.Rows[0]["nom_ref"].ToString().Trim();
                        else
                        {
                            dr["NOM_REF"] = "";
                            dt_errores.Rows.Add("la referencia " + cod_ref + " no existe (ERROR EN LA LINEA " + linea + ")");
                        }

                        #endregion

                        #region bodega

                        string cod_bod = dr["COD_BOD"].ToString().Trim();

                        DataTable dt_bod = SiaWin.Func.SqlDT("select cod_bod,nom_bod from inmae_bod where cod_bod='" + cod_bod + "' ", "bodega", idemp);

                        if (dt_bod.Rows.Count > 0) dr["NOM_BOD"] = dt_bod.Rows[0]["nom_bod"].ToString().Trim();
                        else
                        {
                            dr["NOM_BOD"] = "";
                            dt_errores.Rows.Add("la bodega " + cod_bod + " no existe  (ERROR EN LA LINEA " + linea + ")");
                        }

                        #endregion

                        #region cantidad

                        string cntxls = dr["CANTIDAD"].ToString().Trim();
                        decimal cantidad = 0;

                        if (!string.IsNullOrEmpty(cntxls))
                        {
                            if (decimal.TryParse(cntxls, out cantidad) == false)
                                dt_errores.Rows.Add("el campo cantidad debe de ser numerico:" + num_trn + " (ERROR EN LA LINEA " + linea + ")");
                        }
                        else dt_errores.Rows.Add("el campo cantidad debe de estar lleno:" + num_trn + " (ERROR EN LA LINEA " + linea + ")");

                        #endregion

                        #region cos_uni - cos_tot

                        string cosuni = dr["COS_UNI"].ToString().Trim();
                        decimal cos_uni = 0;

                        if (!string.IsNullOrEmpty(cosuni))
                        {
                            if (decimal.TryParse(cosuni, out cos_uni) == true)
                                cos_uni = Convert.ToDecimal(dr["COS_UNI"]);
                            else
                                dt_errores.Rows.Add("el campo costo unitario debe de ser numerico:" + num_trn + " (ERROR EN LA LINEA " + linea + ")");
                        }
                        else dt_errores.Rows.Add("el campo costo unitario debe de estar lleno:" + num_trn + " (ERROR EN LA LINEA " + linea + ")");



                        string costot = dr["COS_TOT"].ToString().Trim();
                        decimal cos_tot = 0;

                        if (!string.IsNullOrEmpty(costot))
                        {
                            if (decimal.TryParse(costot, out cos_tot) == true)
                                cos_tot = Convert.ToDecimal(dr["COS_TOT"]);
                            else
                                dt_errores.Rows.Add("el campo costo total debe de ser numerico:" + num_trn + " (ERROR EN LA LINEA " + linea + ")");

                        }
                        else dt_errores.Rows.Add("el campo costo total debe de estar lleno:" + num_trn + " (ERROR EN LA LINEA " + linea + ")");



                        ttuni += cos_uni;
                        tttot += cos_tot;

                        decimal operation = cos_uni * cantidad;

                        if (operation != cos_tot)
                        {
                            dt_errores.Rows.Add("el costo total no coincide con la operacion de costo unitario por cantidad :" + cod_trn + "-" + num_trn + " (ERROR EN LA LINEA " + linea + ") ");
                        }

                        #endregion

                        linea++;
                    }
                }

                DataTable dtreturn = new DataTable();
                foreach (DataTable dtemp in doc_agru.Tables) dtreturn.Merge(dtemp);

                return (dtreturn, ttuni, tttot);
            }
            catch (Exception e)
            {
                MessageBox.Show("en la consulta:" + e.Message);
                return (null, 0, 0);
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

                if (MessageBox.Show("Usted desea generar los documentos de la importacion realizada?", "Documentos", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    string sql_cab = ""; string sql_cue = "";

                    foreach (DataTable dt_cue in doc_agru.Tables)
                    {

                        string cod_trn_cab = dt_cue.Rows[0]["cod_trn"].ToString();
                        string num_trn_cab = dt_cue.Rows[0]["num_trn"].ToString();
                        string fec_trn_cab = dt_cue.Rows[0]["fec_trn"].ToString();
                        string factura = dt_cue.Rows[0]["factura"].ToString();
                        string cod_ter = dt_cue.Rows[0]["cod_ter"].ToString();

                        sql_cab += @"INSERT INTO " + cabeza + " (cod_trn,num_trn,fec_trn,des_mov,doc_ref,cod_prv) values ('" + cod_trn_cab + "','" + num_trn_cab + "','" + fec_trn_cab + "','IMPORTACION PROCESOS 750','" + factura + "','" + cod_ter + "');DECLARE @NewID INT;SELECT @NewID = SCOPE_IDENTITY();";


                        foreach (System.Data.DataRow dt in dt_cue.Rows)
                        {

                            string cod_ref = dt["cod_ref"].ToString().Trim();
                            string cod_bod = dt["cod_bod"].ToString().Trim();

                            decimal cos_uni = Convert.ToDecimal(dt["cos_uni"]);

                            decimal cantidad = Convert.ToDecimal(dt["cantidad"]);
                            decimal cos_tot = Convert.ToDecimal(dt["cos_tot"]);


                            sql_cue += @"INSERT INTO " + cuerpo + " (idregcab,cod_trn,num_trn,cod_ref,cod_bod,cantidad,cos_uni,cos_tot,cod_sub) values (@NewID,'" + cod_trn_cab + "','" + num_trn_cab + "','" + cod_ref + "','" + cod_bod + "'," + cantidad.ToString("F", CultureInfo.InvariantCulture) + "," + cos_uni.ToString("F", CultureInfo.InvariantCulture) + "," + cos_tot.ToString("F", CultureInfo.InvariantCulture) + ",'050');";
                        }

                        string query = sql_cab + sql_cue;


                        if (SiaWin.Func.SqlCRUD(query, idemp) == false) { MessageBox.Show("se genero un error en un documento por favor consulte"); }
                        sql_cab = ""; sql_cue = "";
                    }

                    AbrirDocGenerados();

                    dataGridRefe.ItemsSource = null;
                    doc_agru.Tables.Clear();
                    dt_errores.Clear();
                    Tx_total.Text = "";
                    Tx_errores.Text = "";
                    TxTot_cosuni.Text = "";
                    TxTot_costot.Text = "";
                    Tx_ref.Text = "";
                    Tx_bod.Text = "";


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

                    IWorkbook workbook = application.Workbooks.Create(1);
                    IWorksheet worksheet = workbook.Worksheets[0];

                    worksheet.IsGridLinesVisible = true;

                    worksheet.Range["A1"].Text = "COD_TRN";
                    worksheet.Range["B1"].Text = "NUM_TRN";
                    worksheet.Range["C1"].Text = "FEC_TRN";
                    worksheet.Range["D1"].Text = "COD_TER";
                    worksheet.Range["E1"].Text = "COD_REF";
                    worksheet.Range["F1"].Text = "FACTURA";
                    worksheet.Range["G1"].Text = "CANTIDAD";
                    worksheet.Range["H1"].Text = "COS_UNI";
                    worksheet.Range["I1"].Text = "COS_TOT";
                    worksheet.Range["J1"].Text = "COD_BOD";

                    worksheet.Range["A1:J1"].CellStyle.Font.Bold = true;

                    if (string.IsNullOrEmpty(ruta))
                        MessageBox.Show("Por favor, seleccione una ruta para guardar la plantilla");
                    else
                    {
                        workbook.SaveAs(ruta);
                        MessageBox.Show("Plantilla Guardado");
                    }

                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al guardar:" + w);
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

        private void DataGridRefe_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            try
            {
                if ((sender as Syncfusion.UI.Xaml.Grid.SfDataGrid).SelectedIndex >= 0)
                {
                    var reflector = (sender as Syncfusion.UI.Xaml.Grid.SfDataGrid).View.GetPropertyAccessProvider();
                    var rowData = (sender as Syncfusion.UI.Xaml.Grid.SfDataGrid).GetRecordAtRowIndex((sender as Syncfusion.UI.Xaml.Grid.SfDataGrid).SelectedIndex + 1);
                    Tx_ref.Text = !string.IsNullOrEmpty(reflector.GetValue(rowData, "NOM_REF").ToString()) ? reflector.GetValue(rowData, "NOM_REF").ToString().ToUpper() : "---";
                    Tx_bod.Text = !string.IsNullOrEmpty(reflector.GetValue(rowData, "NOM_BOD").ToString()) ? reflector.GetValue(rowData, "NOM_BOD").ToString().ToUpper() : "---";
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("errro al seleccionar:" + w);
            }
        }




    }
}
