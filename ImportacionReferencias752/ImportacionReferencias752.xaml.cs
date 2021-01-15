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

    public partial class ImportacionReferencias752 : UserControl
    {
        //Sia.PublicarPnt(9692,"ImportacionReferencias752");
        //Sia.TabU(9692);

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        dynamic tabitem;

        DataTable dt = new DataTable();
        DataTable dt_errores = new DataTable();

        public ImportacionReferencias752(dynamic tabitem1)
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
                tabitem.Title = "Importacion de referencias - " + nomempresa;
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

                    worksheet.Range["A1"].Text = "COD_REF";
                    worksheet.Range["B1"].Text = "NOM_REF";
                    worksheet.Range["C1"].Text = "COSTO";
                    worksheet.Range["D1"].Text = "COD_TIP";


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

            if (dt.Columns.Contains("COD_REF") == false || dt.Columns.IndexOf("COD_REF") != 0) flag = false;
            if (dt.Columns.Contains("NOM_REF") == false || dt.Columns.IndexOf("NOM_REF") != 1) flag = false;
            if (dt.Columns.Contains("COSTO") == false || dt.Columns.IndexOf("COSTO") != 2) flag = false;
            if (dt.Columns.Contains("COD_TIP") == false || dt.Columns.IndexOf("COD_TIP") != 3) flag = false;
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

                    Tx_lin.Text = "";
                    Tx_exist.Text = "";
                    Tx_total.Text = "0";
                    Tx_errores.Text = "0";
                    return;
                }

                PanelBtn.IsEnabled = false;

                var slowTask = Task<DataTable>.Factory.StartNew(() => Process(dt));
                await slowTask;

                if (((DataTable)slowTask.Result).Rows.Count > 0)
                {
                    dataGridExcel.ItemsSource = ((DataTable)slowTask.Result).DefaultView;
                }

                MessageBox.Show(Application.Current.MainWindow, "Importacion Exitosa", "alerta", MessageBoxButton.OK, MessageBoxImage.Information);

                Tx_total.Text = ((DataTable)slowTask.Result).Rows.Count.ToString();
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


        private DataTable Process(DataTable dt)
        {
            try
            {

                dt.Columns.Add("REF_EXIST", typeof(bool));
                dt.Columns.Add("NOM_TIP", typeof(string));

                int linea = 1;
                //validar campo por campo
                foreach (System.Data.DataRow dr in dt.Rows)
                {

                    #region referencia

                    string cod_ref = dr["COD_REF"].ToString().Trim();
                    if (!string.IsNullOrEmpty(cod_ref))
                    {
                        DataTable dt_ref = SiaWin.Func.SqlDT("select cod_ref,nom_ref from inmae_ref where cod_ref='" + cod_ref + "';", "temp", idemp);
                        dr["REF_EXIST"] = dt_ref.Rows.Count > 0 ? true : false;
                    }
                    else
                    {
                        System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el codigo de referencia debe de estar lleno (ERROR EN LA LINEA " + linea + ")"; dt_errores.Rows.Add(row);
                    }

                    #endregion

                    #region nombre referencia

                    string nom_ref = dr["NOM_REF"].ToString().Trim();
                    if (!string.IsNullOrEmpty(nom_ref))
                    {
                        if (nom_ref.Length > 100)
                        {
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo nombre de referencia debe ser menor de 100 caracteres (" + cod_ref + ") (ERROR EN LA LINEA " + linea + ")"; dt_errores.Rows.Add(row);
                        }
                    }
                    else
                    {
                        System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo nombre de referencia debe de estar lleno (ERROR EN LA LINEA " + linea + ")"; dt_errores.Rows.Add(row);
                    }

                    #endregion                  

                    #region linea

                    string cod_tip = dr["COD_TIP"].ToString().Trim();
                    if (!string.IsNullOrEmpty(cod_tip))
                    {
                        DataTable dt_tip = SiaWin.Func.SqlDT("select cod_tip,nom_tip from inmae_tip where cod_tip='" + cod_tip + "';", "temp", idemp);
                        if (dt_tip.Rows.Count > 0)
                        {
                            dr["NOM_TIP"] = dt_tip.Rows[0]["nom_tip"].ToString().Trim();
                        }
                        else
                        {
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el codigo de linea " + cod_tip + " no existe en la maestra de lineas (ERROR EN LA LINEA " + linea + ")"; dt_errores.Rows.Add(row);
                        }

                    }
                    else
                    {
                        System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el codigo de linea debe de estar lleno (ERROR EN LA LINEA " + linea + ")"; dt_errores.Rows.Add(row);
                    }

                    #endregion

                    #region costo

                    string costoxls = dr["COSTO"].ToString().Trim();
                    decimal costo = 0;

                    if (!string.IsNullOrEmpty(costoxls))
                    {
                        if (decimal.TryParse(costoxls, out costo) == false)
                            dt_errores.Rows.Add("el campo costo debe de ser numerico:" + cod_ref + " (ERROR EN LA LINEA " + linea + ")");
                    }
                    else dt_errores.Rows.Add("el campo costo debe de estar lleno:" + cod_ref + " (ERROR EN LA LINEA " + linea + ")");

                    #endregion

                    linea++;
                }

                return dt;
            }
            catch (Exception e)
            {
                MessageBox.Show("en la consulta:" + e.Message);
                return null;
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

                if (MessageBox.Show("Usted desea generar importacion y/o verificacion de referencias?", "Importacion", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {


                    if (dt.Rows.Count > 0)
                    {
                        string query = "";
                        foreach (System.Data.DataRow item in dt.Rows)
                        {
                            bool existe = Convert.ToBoolean(item["REF_EXIST"]);

                            string cod_ref = item["COD_REF"].ToString().Trim();
                            string nom_ref = item["NOM_REF"].ToString().Trim();
                            string cod_tip = item["COD_TIP"].ToString().Trim();
                            decimal costo = Convert.ToDecimal(item["COSTO"]);


                            if (existe)
                            {
                                query += "update inmae_ref set cod_ref='" + cod_ref + "',nom_ref='" + nom_ref + "',cod_tip='" + cod_tip + "',vrunc=" + costo.ToString("F", CultureInfo.InvariantCulture) + ",cod_tiva='A',ind_iva=1,estado=1,cod_med='UND' where cod_ref='" + cod_ref + "';";
                            }
                            else
                            {
                                query += "insert into inmae_ref (cod_ref,nom_ref,cod_tip,vrunc,cod_tiva,ind_iva,estado,cod_med) " +
                           "values ('" + cod_ref + "', '" + nom_ref + "','" + cod_tip + "'," + costo.ToString("F", CultureInfo.InvariantCulture) + ",'A',1,1,'UND');";
                            }
                        }
                        

                        if (SiaWin.Func.SqlCRUD(query, idemp) == true) { MessageBox.Show("la importacion fue exitosa", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    dataGridExcel.ItemsSource = null;
                    dt.Clear();
                    dt_errores.Clear();
                    Tx_total.Text = "0";
                    Tx_errores.Text = "0";
                    Tx_lin.Text = "-";
                    Tx_exist.Text = "";
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
                    Tx_lin.Text = !string.IsNullOrEmpty(reflector.GetValue(rowData, "NOM_TIP").ToString()) ? reflector.GetValue(rowData, "NOM_TIP").ToString().ToUpper() : "---";

                    if (!string.IsNullOrEmpty(reflector.GetValue(rowData, "REF_EXIST").ToString()))
                    {
                        bool f = Convert.ToBoolean(reflector.GetValue(rowData, "REF_EXIST"));
                        Tx_exist.Text = f ? "SI" : "NO";
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
