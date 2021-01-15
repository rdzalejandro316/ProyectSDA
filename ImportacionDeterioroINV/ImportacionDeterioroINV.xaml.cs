using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
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

    public partial class ImportacionDeterioroINV : UserControl
    {
        //Sia.PublicarPnt(9693,"ImportacionDeterioroINV");
        //Sia.TabU(9693);

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        dynamic tabitem;

        DataTable dt = new DataTable();
        DataTable dt_errores = new DataTable();

        public ImportacionDeterioroINV(dynamic tabitem1)
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
                tabitem.Title = "Importacion de deterioros y recuperaciones - " + nomempresa;
                tabitem.Logo(idLogo, ".png");
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void BtnPlantilla_Click(object sender, RoutedEventArgs e)
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

                    worksheet.Range["A1"].Text = "FEC_TRN";
                    worksheet.Range["B1"].Text = "COD_REF";
                    worksheet.Range["C1"].Text = "COSTO_PP";
                    worksheet.Range["D1"].Text = "COSTO_REP";
                    worksheet.Range["E1"].Text = "DETERIORO";
                    worksheet.Range["F1"].Text = "RECUPERA";



                    worksheet.Range["A1:F1"].CellStyle.Font.Bold = true;

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

            if (dt.Columns.Contains("FEC_TRN") == false || dt.Columns.IndexOf("FEC_TRN") != 0) flag = false;
            if (dt.Columns.Contains("COD_REF") == false || dt.Columns.IndexOf("COD_REF") != 1) flag = false;
            if (dt.Columns.Contains("COSTO_PP") == false || dt.Columns.IndexOf("COSTO_PP") != 2) flag = false;
            if (dt.Columns.Contains("COSTO_REP") == false || dt.Columns.IndexOf("COSTO_REP") != 3) flag = false;
            if (dt.Columns.Contains("DETERIORO") == false || dt.Columns.IndexOf("DETERIORO") != 4) flag = false;
            if (dt.Columns.Contains("RECUPERA") == false || dt.Columns.IndexOf("RECUPERA") != 5) flag = false;
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
                    dataGridExcel.ItemsSource = null;
                    dt.Clear(); dt_errores.Clear();

                    Tx_ref.Text = "";
                    Tx_estado.Text = "";
                    Tx_costopp.Text = "0";
                    Tx_costorep.Text = "0";
                    Tx_deterioro.Text = "0";
                    Tx_recupera.Text = "0";

                    Tx_total.Text = "0";
                    Tx_errores.Text = "0";
                    return;
                }

                PanelBtn.IsEnabled = false;

                var slowTask = Task<(DataTable, decimal, decimal, decimal, decimal)>.Factory.StartNew(() => Process(dt));
                await slowTask;

                if (slowTask.IsCompleted)
                {
                    if (slowTask.Result.Item1.Rows.Count > 0)
                    {
                        dataGridExcel.ItemsSource = slowTask.Result.Item1.DefaultView;

                        Tx_costopp.Text = Convert.ToDecimal(slowTask.Result.Item2).ToString("N");
                        Tx_costorep.Text = Convert.ToDecimal(slowTask.Result.Item3).ToString("N");
                        Tx_deterioro.Text = Convert.ToDecimal(slowTask.Result.Item4).ToString("N");
                        Tx_recupera.Text = Convert.ToDecimal(slowTask.Result.Item5).ToString("N");
                    }

                }

                MessageBox.Show(Application.Current.MainWindow, "Importacion Exitosa", "alerta", MessageBoxButton.OK, MessageBoxImage.Information);

                Tx_total.Text = slowTask.Result.Item1.Rows.Count.ToString();
                Tx_errores.Text = dt_errores.Rows.Count.ToString();

                PanelBtn.IsEnabled = true;
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


        private (DataTable, decimal, decimal, decimal, decimal) Process(DataTable dt)
        {
            try
            {

                decimal COSTO_PP = 0;
                decimal COSTO_REP = 0;
                decimal DETERIORO = 0;
                decimal RECUPERA = 0;

                dt.Columns.Add("NOM_REF", typeof(string));
                dt.Columns.Add("ESTADO", typeof(bool));


                int linea = 1;
                //validar campo por campo
                foreach (System.Data.DataRow dr in dt.Rows)
                {

                    #region fecha

                    string fechaxls = dr["FEC_TRN"].ToString().Trim();
                    DateTime fecha;

                    if (!string.IsNullOrEmpty(fechaxls))
                    {
                        if (DateTime.TryParse(fechaxls, out fecha) == false)
                            dt_errores.Rows.Add("el campo fecha debe de ser formato fecha 'dd/mm/yyyy' : (ERROR EN LA LINEA " + linea + ")");
                    }
                    else dt_errores.Rows.Add("el campo fecha debe de estar lleno: (ERROR EN LA LINEA " + linea + ")");

                    #endregion

                    #region referencia

                    string cod_ref = dr["COD_REF"].ToString().Trim();
                    if (!string.IsNullOrEmpty(cod_ref))
                    {
                        DataTable dt_ref = SiaWin.Func.SqlDT("select cod_ref,nom_ref,estado from inmae_ref where cod_ref='" + cod_ref + "';", "temp", idemp);
                        if (dt_ref.Rows.Count > 0)
                        {
                            int est;
                            bool estado =
                                dt_ref.Rows[0]["estado"] == DBNull.Value || int.TryParse(dt_ref.Rows[0]["estado"].ToString(), out est) == false
                                ? true : Convert.ToBoolean(dt_ref.Rows[0]["estado"]);
                            dr["ESTADO"] = estado;
                            dr["NOM_REF"] = dt_ref.Rows[0]["nom_ref"].ToString().Trim();
                        }
                        else
                        {
                            dr["ESTADO"] = false;
                            dr["NOM_REF"] = "NO EXISTE";
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "la referencia " + cod_ref + " no existe (ERROR EN LA LINEA " + linea + ")"; dt_errores.Rows.Add(row);
                        }
                    }
                    else
                    {
                        System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el codigo de referencia debe de estar lleno (ERROR EN LA LINEA " + linea + ")"; dt_errores.Rows.Add(row);
                    }

                    #endregion                  

                    #region COSTO_PP

                    string costo_ppxls = dr["COSTO_PP"].ToString().Trim();
                    decimal costo_pp = 0;

                    if (!string.IsNullOrEmpty(costo_ppxls))
                    {
                        if (decimal.TryParse(costo_ppxls, out costo_pp) == false)
                            dt_errores.Rows.Add("el campo COSTO_PP debe de ser numerico:" + cod_ref + " (ERROR EN LA LINEA " + linea + ")");
                        else COSTO_PP += costo_pp;
                    }
                    else dt_errores.Rows.Add("el campo COSTO_PP debe de estar lleno:" + cod_ref + " (ERROR EN LA LINEA " + linea + ")");

                    #endregion

                    #region COSTO_REP

                    string costo_repxls = dr["COSTO_REP"].ToString().Trim();
                    decimal costo_rep = 0;

                    if (!string.IsNullOrEmpty(costo_repxls))
                    {
                        if (decimal.TryParse(costo_repxls, out costo_rep) == false)
                            dt_errores.Rows.Add("el campo COSTO_REP debe de ser numerico:" + cod_ref + " (ERROR EN LA LINEA " + linea + ")");
                        else COSTO_REP += costo_rep;
                    }
                    else dt_errores.Rows.Add("el campo COSTO_REP debe de estar lleno:" + cod_ref + " (ERROR EN LA LINEA " + linea + ")");

                    #endregion

                    #region DETERIORO

                    string deterioroxls = dr["DETERIORO"].ToString().Trim();
                    decimal deterioro = 0;

                    if (!string.IsNullOrEmpty(deterioroxls))
                    {
                        if (decimal.TryParse(deterioroxls, out deterioro) == false)
                            dt_errores.Rows.Add("el campo DETERIORO debe de ser numerico:" + cod_ref + " (ERROR EN LA LINEA " + linea + ")");
                        else DETERIORO += deterioro;
                    }
                    else dt_errores.Rows.Add("el campo DETERIORO debe de estar lleno:" + cod_ref + " (ERROR EN LA LINEA " + linea + ")");

                    #endregion

                    #region RECUPERA

                    string recuperaxls = dr["RECUPERA"].ToString().Trim();
                    decimal recupera = 0;

                    if (!string.IsNullOrEmpty(recuperaxls))
                    {
                        if (decimal.TryParse(recuperaxls, out recupera) == false)
                            dt_errores.Rows.Add("el campo RECUPERA debe de ser numerico:" + cod_ref + " (ERROR EN LA LINEA " + linea + ")");
                        else RECUPERA += recupera;
                    }
                    else dt_errores.Rows.Add("el campo RECUPERA debe de estar lleno:" + cod_ref + " (ERROR EN LA LINEA " + linea + ")");

                    #endregion

                    linea++;
                }

                return (dt, COSTO_PP, COSTO_REP, DETERIORO, RECUPERA);
            }
            catch (Exception e)
            {
                MessageBox.Show("en la consulta:" + e.Message);
                return (null, 0, 0, 0, 0);
            }
        }



        private void BtnEjecutar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region validacion

                if (dataGridExcel.ItemsSource == null || dataGridExcel.View.Records.Count <= 0)
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

                if (MessageBox.Show("Usted desea generar importacion de deterioros o recuperaciones?", "Importacion", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {


                    if (dt.Rows.Count > 0)
                    {
                        string cab_deterioro = "";
                        string cue_deterioro = "";

                        //string cab_recuperacion = "";
                        //string cue_recuperacion = "";


                        cab_deterioro += "insert into incab_doc (cod_trn,num_trn,fec_trn,des_mov) values ('215','PROCESO DE IMPORTACION 751');DECLARE @NewIDdet INT;SELECT @NewIDdet = SCOPE_IDENTITY(); ";


                        foreach (System.Data.DataRow item in dt.Rows)
                        {

                            decimal deterioro = Convert.ToDecimal(item["DETERIORO"]);
                            decimal recupera = Convert.ToDecimal(item["RECUPERA"]);

                            string cod_ref = item["cod_ref"].ToString().Trim();


                            if (deterioro > 0)
                            {
                                cue_deterioro += "insert into incue_doc (idregcab,cod_trn,num_trn,cod_ref,cod_sub,cos_tot,cod_bod) values (NewIDdet,'','215','" + cod_ref + "','050'," + deterioro.ToString("F", CultureInfo.InvariantCulture) + ",'001')";
                            }

                            //if (recupera > 0)
                            //{
                            //    cue_recuperacion += "insert into incue_doc (idregcab,cod_trn,num_trn,cod_ref,cod_sub,cos_tot,cod_bod) values (NewIDdet,'','','" + cod_ref + "','050'," + recupera.ToString("F", CultureInfo.InvariantCulture) + ",'001')";
                            //}
                        }

                        string query = cab_deterioro + cue_deterioro;

                        if (SiaWin.Func.SqlCRUD(query, idemp) == true) { MessageBox.Show("la importacion fue exitosa", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    dataGridExcel.ItemsSource = null;
                    dt.Clear();
                    dt_errores.Clear();
                    Tx_total.Text = "0";
                    Tx_errores.Text = "0";
                    Tx_ref.Text = "";
                    Tx_estado.Text = "";
                    Tx_ref.Text = "";
                    Tx_costopp.Text = "";
                    Tx_costorep.Text = "";
                    Tx_deterioro.Text = "";
                    Tx_recupera.Text = "";
                }

                #endregion


            }
            catch (Exception w)
            {
                MessageBox.Show("ERROR AL EJECUTAR EL PROCESO:" + w);
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

        private void dataGridExcel_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            try
            {
                if ((sender as SfDataGrid).SelectedIndex >= 0)
                {
                    var reflector = (sender as SfDataGrid).View.GetPropertyAccessProvider();
                    var rowData = (sender as SfDataGrid).GetRecordAtRowIndex((sender as SfDataGrid).SelectedIndex + 1);
                    Tx_ref.Text = !string.IsNullOrEmpty(reflector.GetValue(rowData, "NOM_REF").ToString()) ? reflector.GetValue(rowData, "NOM_REF").ToString().ToUpper() : "---";

                    if (!string.IsNullOrEmpty(reflector.GetValue(rowData, "ESTADO").ToString()))
                    {
                        bool f = Convert.ToBoolean(reflector.GetValue(rowData, "ESTADO"));
                        Tx_estado.Text = f ? "ACTIVO" : "INACTIVO";
                    }

                }
            }
            catch (Exception w)
            {
                MessageBox.Show("errro al seleccionar:" + w);
            }
        }



    }
}
