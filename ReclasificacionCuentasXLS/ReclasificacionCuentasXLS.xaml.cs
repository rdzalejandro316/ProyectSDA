using Microsoft.Win32;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
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
    //Sia.PublicarPnt(9688,"ReclasificacionCuentasXLS");
    //Sia.TabU(9688,"ReclasificacionCuentasXLS");

    public partial class ReclasificacionCuentasXLS : UserControl
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        dynamic tabitem;
        DataTable dt = new DataTable();
        DataTable dt_errores = new DataTable();

        public ReclasificacionCuentasXLS(dynamic tab)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            tabitem = tab;
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
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                tabitem.Title = "Reclasificacion Cuentas " + cod_empresa + "-" + nomempresa;
                tabitem.Logo(idLogo, ".png");
                dt_errores.Columns.Add("error");
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
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

        public static DataTable ConvertExcelToDataTable(string FileName)
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
            if (dt.Columns.Contains("COD_CTA") == false || dt.Columns.IndexOf("COD_CTA") != 0) flag = false;
            if (dt.Columns.Contains("NOM_CTA") == false || dt.Columns.IndexOf("NOM_CTA") != 1) flag = false;
            if (dt.Columns.Contains("CTA_NIIF") == false || dt.Columns.IndexOf("CTA_NIIF") != 2) flag = false;
            if (dt.Columns.Contains("NOM_NIIF") == false || dt.Columns.IndexOf("NOM_NIIF") != 3) flag = false;
            if (dt.Columns.Contains("IND_RECLA") == false || dt.Columns.IndexOf("IND_RECLA") != 4) flag = false;
            if (dt.Columns.Contains("ACCION") == false || dt.Columns.IndexOf("ACCION") != 5) flag = false;
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
                    Tx_total.Text = "0";
                    Tx_errores.Text = "0";
                    return;
                }

                BtnImport.IsEnabled = false;
                BtnPlantilla.IsEnabled = false;
                BtnEjecuter.IsEnabled = false;

                var slowTask = Task<DataTable>.Factory.StartNew(() => Process(dt));
                await slowTask;

                if (((DataTable)slowTask.Result).Rows.Count > 0)
                {
                    dataGridExcel.ItemsSource = ((DataTable)slowTask.Result).DefaultView;
                }

                Tx_total.Text = ((DataTable)slowTask.Result).Rows.Count.ToString();
                Tx_errores.Text = dt_errores.Rows.Count.ToString();

                MessageBox.Show(Application.Current.MainWindow, "Importacion Exitosa", "alerta", MessageBoxButton.OK, MessageBoxImage.Information);


                BtnImport.IsEnabled = true;
                BtnPlantilla.IsEnabled = true;
                BtnEjecuter.IsEnabled = true;
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
                if (dt.Rows.Count > 0)
                {

                    dt.Columns.Add("CTA_EXIST", typeof(bool));

                    foreach (System.Data.DataRow dr in dt.Rows)
                    {

                        #region cuenta

                        string cod_cta = dr["COD_CTA"].ToString().Trim();
                        int cta;
                        if (!string.IsNullOrWhiteSpace(cod_cta))
                        {
                            if (cod_cta.Length > 15)
                            {
                                System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "la cuenta " + cod_cta + " es mayor a 15 caracteres"; dt_errores.Rows.Add(row);
                            }

                            if (int.TryParse(cod_cta, out cta) == false)
                            {
                                System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "la column cuenta (" + cod_cta + ") debe de ser de tipo numerica"; dt_errores.Rows.Add(row);
                            }

                        }


                        DataTable dt_cta = SiaWin.Func.SqlDT("select cod_cta,nom_cta,tip_cta from comae_cta where cod_cta='" + cod_cta + "' ", "cuenta", idemp);
                        dr["CTA_EXIST"] = dt_cta.Rows.Count > 0 ? true : false;

                        #endregion

                        #region nombre cuenta

                        string nom_cta = dr["NOM_CTA"].ToString().Trim();

                        if (nom_cta.Length > 60)
                        {
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el nombre de la cuenta " + cod_cta + " es mayor a 60 caracteres"; dt_errores.Rows.Add(row);
                        }
                        #endregion

                        #region cuenta niif

                        string cta_niif = dr["CTA_NIIF"].ToString().Trim();

                        if (cta_niif.Length > 15)
                        {
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "la cuenta NIIF " + cta_niif + " es mayor a 15 caracteres"; dt_errores.Rows.Add(row);
                        }

                        #endregion

                        #region nombre cuenta niif

                        string nom_niif = dr["NOM_NIIF"].ToString().Trim();

                        if (nom_niif.Length > 100)
                        {
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el nombre de la cuenta NIIF " + cta_niif + " es mayor a 60 caracteres"; dt_errores.Rows.Add(row);
                        }
                        #endregion

                        #region ind_recla y accion

                        string ind_recla = dr["IND_RECLA"].ToString().Trim();
                        string accion = dr["ACCION"].ToString().Trim();
                        int i, a;
                        if (!string.IsNullOrEmpty(ind_recla))
                        {
                            if (int.TryParse(ind_recla, out i) == false)
                            {
                                System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "la columna ind_recla debe ser numerica valor:" + ind_recla + " (0)-ninguna (1)-si (2)-no "; dt_errores.Rows.Add(row);
                            }
                        }

                        if (!string.IsNullOrEmpty(accion))
                        {
                            if (int.TryParse(accion, out a) == false)
                            {
                                System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "la columna accion debe ser numerica"; dt_errores.Rows.Add(row);
                            }
                        }
                        #endregion

                    }
                }

                return dt;
            }
            catch (Exception e)
            {
                MessageBox.Show("en la consulta:" + e.Message);
                return null;
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

                    worksheet.Range["A1"].Text = "COD_CTA";
                    worksheet.Range["B1"].Text = "NOM_CTA";
                    worksheet.Range["C1"].Text = "CTA_NIIF";
                    worksheet.Range["D1"].Text = "NOM_NIIF";
                    worksheet.Range["E1"].Text = "IND_RECLA";
                    worksheet.Range["F1"].Text = "ACCION";

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

        private void BtnEjecuter_Click(object sender, RoutedEventArgs e)
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


                if (MessageBox.Show("Usted desea continuar .........?", "Documentos", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    string query = "";
                    foreach (DataRow dr in dt.Rows)
                    {
                        bool exist = Convert.ToBoolean(dr["CTA_EXIST"]);

                        string cod_cta = dr["COD_CTA"].ToString().Trim();
                        string nom_cta = dr["NOM_CTA"].ToString().Trim();
                        string cta_niif = dr["CTA_NIIF"].ToString().Trim();
                        string nom_niif = dr["NOM_NIIF"].ToString().Trim();
                        string ind_recla = dr["IND_RECLA"].ToString().Trim();
                        string accion = dr["ACCION"].ToString().Trim();

                        if (exist)
                        {
                            if (!string.IsNullOrEmpty(cta_niif))
                            {
                                query += "update comae_cta set cta_niif='" + cta_niif + "',nom_niif='" + nom_niif + "',ind_recla=" + ind_recla + ",cod_cta='" + cod_cta + "',nom_ant='" + nom_cta + "',nom_cta='" + nom_cta + "',accion=" + accion + "  where cod_cta='" + cod_cta + "';";
                            }
                        }
                        else
                        {
                            string ini = cod_cta.Substring(0, 1).Trim();
                            string naturaleza = ini == "1" || ini == "5" || ini == "6" || ini == "7" ? "D" : "C";
                            query += "insert into comae_cta (cod_cta,nom_cta,ind_act,nat_cta,ind_ter,ind_bal,accion) values ('" + cod_cta + "','" + nom_cta + "',1,'" + naturaleza + "',1,1," + accion + ");";
                        }
                    }

                    if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                    {
                        MessageBox.Show("reclasificacion exitosa", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idemp, 0, 0, 0, "ejecuto proceso de reclisificacion cuentas proceso-700", "");
                    }


                    dataGridExcel.ItemsSource = null;
                    dt.Clear();
                    dt_errores.Clear();
                    Tx_total.Text = "0";
                    Tx_errores.Text = "0"; ;
                }


            }
            catch (Exception w)
            {
                MessageBox.Show("error al ejecutar:" + w);
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
