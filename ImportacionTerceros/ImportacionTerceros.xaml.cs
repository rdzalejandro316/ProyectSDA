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

    public partial class ImportacionTerceros : UserControl
    {
        //Sia.PublicarPnt(9629,"ImportacionTerceros");
        //Sia.TabU(9629);

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        dynamic tabitem;

        DataTable dt = new DataTable();
        DataTable dt_errores = new DataTable();

        public ImportacionTerceros(dynamic tabitem1)
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
                tabitem.Title = "Importacion de terceros - " + nomempresa;
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

                    worksheet.Range["A1"].Text = "COD_TER";
                    worksheet.Range["B1"].Text = "NOM_TER";
                    worksheet.Range["C1"].Text = "DIR1";
                    worksheet.Range["D1"].Text = "TEL1";
                    worksheet.Range["E1"].Text = "EMAIL";
                    worksheet.Range["F1"].Text = "FEC_ING";
                    worksheet.Range["G1"].Text = "TIP_PRV";
                    worksheet.Range["H1"].Text = "ESTADO";
                    worksheet.Range["I1"].Text = "CLASIFIC";
                    worksheet.Range["J1"].Text = "TDOC";
                    worksheet.Range["K1"].Text = "APL1";
                    worksheet.Range["L1"].Text = "APL2";
                    worksheet.Range["M1"].Text = "NOM1";
                    worksheet.Range["N1"].Text = "NOM2";
                    worksheet.Range["O1"].Text = "RAZ";
                    worksheet.Range["P1"].Text = "DIR";
                    worksheet.Range["Q1"].Text = "TIP_PERS";
                    worksheet.Range["R1"].Text = "DV";
                    worksheet.Range["S1"].Text = "COD_CIU";
                    worksheet.Range["T1"].Text = "COD_PAIS";

                    worksheet.Range["A1:T1"].CellStyle.Font.Bold = true;

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

        private void BtnTercero_Click(object sender, RoutedEventArgs e)
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

            if (dt.Columns.Contains("COD_TER") == false || dt.Columns.IndexOf("COD_TER") != 0) flag = false;
            if (dt.Columns.Contains("NOM_TER") == false || dt.Columns.IndexOf("NOM_TER") != 1) flag = false;
            if (dt.Columns.Contains("DIR1") == false || dt.Columns.IndexOf("DIR1") != 2) flag = false;
            if (dt.Columns.Contains("TEL1") == false || dt.Columns.IndexOf("TEL1") != 3) flag = false;
            if (dt.Columns.Contains("EMAIL") == false || dt.Columns.IndexOf("EMAIL") != 4) flag = false;
            if (dt.Columns.Contains("FEC_ING") == false || dt.Columns.IndexOf("FEC_ING") != 5) flag = false;
            if (dt.Columns.Contains("TIP_PRV") == false || dt.Columns.IndexOf("TIP_PRV") != 6) flag = false;
            if (dt.Columns.Contains("ESTADO") == false || dt.Columns.IndexOf("ESTADO") != 7) flag = false;
            if (dt.Columns.Contains("CLASIFIC") == false || dt.Columns.IndexOf("CLASIFIC") != 8) flag = false;
            if (dt.Columns.Contains("TDOC") == false || dt.Columns.IndexOf("TDOC") != 9) flag = false;
            if (dt.Columns.Contains("APL1") == false || dt.Columns.IndexOf("APL1") != 10) flag = false;
            if (dt.Columns.Contains("APL2") == false || dt.Columns.IndexOf("APL2") != 11) flag = false;
            if (dt.Columns.Contains("NOM1") == false || dt.Columns.IndexOf("NOM1") != 12) flag = false;
            if (dt.Columns.Contains("NOM2") == false || dt.Columns.IndexOf("NOM2") != 13) flag = false;
            if (dt.Columns.Contains("RAZ") == false || dt.Columns.IndexOf("RAZ") != 14) flag = false;
            if (dt.Columns.Contains("DIR") == false || dt.Columns.IndexOf("DIR") != 15) flag = false;
            if (dt.Columns.Contains("TIP_PERS") == false || dt.Columns.IndexOf("TIP_PERS") != 16) flag = false;
            if (dt.Columns.Contains("DV") == false || dt.Columns.IndexOf("DV") != 17) flag = false;
            if (dt.Columns.Contains("COD_CIU") == false || dt.Columns.IndexOf("COD_CIU") != 18) flag = false;
            if (dt.Columns.Contains("COD_PAIS") == false || dt.Columns.IndexOf("COD_PAIS") != 19) flag = false;
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

                    Tx_prv.Text = "";
                    Tx_exist.Text = "";
                    Tx_ciudad.Text = "";
                    Tx_pais.Text = "-";
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

                dt.Columns.Add("TER_EXIST", typeof(bool));
                dt.Columns.Add("TIP_PRV_NOM", typeof(string));
                dt.Columns.Add("NOM_TDO", typeof(string));
                dt.Columns.Add("NOM_MUNI", typeof(string));
                dt.Columns.Add("NOM_PAIS", typeof(string));

                //validar campo por campo
                foreach (System.Data.DataRow dr in dt.Rows)
                {

                    #region tercero

                    string cod_ter = dr["COD_TER"].ToString().Trim();
                    if (!string.IsNullOrEmpty(cod_ter))
                    {
                        DataTable dt_ter = SiaWin.Func.SqlDT("select cod_ter,nom_ter from comae_ter where cod_ter='" + cod_ter + "';", "tercero", idemp);
                        dr["TER_EXIST"] = dt_ter.Rows.Count > 0 ? true : false;
                    }
                    else
                    {
                        System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el codigo de tercero debe de estar lleno"; dt_errores.Rows.Add(row);
                    }

                    #endregion

                    #region nombre

                    string nom_ter = dr["NOM_TER"].ToString().Trim();
                    if (!string.IsNullOrEmpty(nom_ter))
                    {
                        if (nom_ter.Length > 100)
                        {
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo nombre de tercero debe ser menor de 100 caracteres (" + cod_ter + ")"; dt_errores.Rows.Add(row);
                        }
                    }
                    else
                    {
                        System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo nombre de tercero debe de estar lleno"; dt_errores.Rows.Add(row);
                    }

                    #endregion

                    #region direccion

                    string dir1 = dr["DIR1"].ToString().Trim();
                    if (!string.IsNullOrEmpty(dir1))
                    {
                        if (dir1.Length > 120)
                        {
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo de direccion debe ser menor de 120 caracteristicas (" + cod_ter + ") "; dt_errores.Rows.Add(row);
                        }
                    }
                    else
                    {
                        System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo de direccion debe de estar lleno (" + cod_ter + ")"; dt_errores.Rows.Add(row);
                    }


                    #endregion

                    #region telefono

                    string tel1 = dr["TEL1"].ToString().Trim();
                    if (!string.IsNullOrEmpty(tel1))
                    {
                        if (tel1.Length > 50)
                        {
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo de telefono debe ser menor de 50 caracteristicas (" + cod_ter + ")"; dt_errores.Rows.Add(row);
                        }
                    }
                    else
                    {
                        System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo de telefono debe de estar lleno (" + cod_ter + ")"; dt_errores.Rows.Add(row);
                    }


                    #endregion
                    

                    #region  fecha de ingreso

                    string fec_ing = dr["FEC_ING"].ToString().Trim();
                    DateTime fecing;
                    if (!string.IsNullOrEmpty(fec_ing))
                    {
                        if (DateTime.TryParse(fec_ing, out fecing) == false)
                        {
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo de fecha de ingreso debe ser de tipo fecha (dd/MM/yyyy) (" + cod_ter + ")"; dt_errores.Rows.Add(row);
                        }
                    }
                    else
                    {
                        System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo de fecha de ingreso debe de estar lleno (" + cod_ter + ")"; dt_errores.Rows.Add(row);
                    }


                    #endregion

                    #region tipo de provedor

                    string tip_prv = dr["TIP_PRV"].ToString().Trim();
                    int tipprv;
                    if (!string.IsNullOrEmpty(tip_prv))
                    {
                        if (int.TryParse(tip_prv, out tipprv))
                        {
                            if (!(tipprv >= 0 && tipprv <= 3))
                            {
                                System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el tipo de provedor debe de ser el siguiente (0)-Regimen Comun (1)-Simplificado (2)-Gran Contribuyente (3)-Entidad Oficial (" + cod_ter + ")"; dt_errores.Rows.Add(row);
                                dr["TIP_PRV_NOM"] = "";
                            }
                            else
                            {
                                switch (tipprv)
                                {
                                    case 0: dr["TIP_PRV_NOM"] = "Regimen Comun"; break;
                                    case 1: dr["TIP_PRV_NOM"] = "Simplificado"; break;
                                    case 2: dr["TIP_PRV_NOM"] = "Gran Contribuyente"; break;
                                    case 3: dr["TIP_PRV_NOM"] = "Entidad Oficial"; break;
                                }

                            }
                        }
                        else
                        {
                            dr["TIP_PRV_NOM"] = "";
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo tipo de provedor debe de ser numerico (" + cod_ter + ")"; dt_errores.Rows.Add(row);
                        }
                    }
                    else
                    {
                        System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo tipo de provedor debe de estar lleno (" + cod_ter + ")"; dt_errores.Rows.Add(row);
                    }

                    #endregion

                    #region estado

                    string estado = dr["ESTADO"].ToString().Trim();
                    int est;
                    if (!string.IsNullOrEmpty(estado))
                    {
                        if (int.TryParse(estado, out est))
                        {
                            if (!(est >= 0 && est <= 1))
                            {
                                System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo estado debe de ser (0)-inactivo (1)-activo (" + cod_ter + ")"; dt_errores.Rows.Add(row);
                            }
                        }
                        else
                        {
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo de estado debe ser numerico (" + cod_ter + ")"; dt_errores.Rows.Add(row);
                        }
                    }
                    else
                    {
                        System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo de estado debe de estar lleno (" + cod_ter + ")"; dt_errores.Rows.Add(row);
                    }

                    #endregion

                    #region clasific

                    string clasific = dr["CLASIFIC"].ToString().Trim();
                    int clasi;
                    if (!string.IsNullOrEmpty(clasific))
                    {
                        if (int.TryParse(clasific, out clasi))
                        {
                            if (!(clasi >= 0 && clasi <= 5))
                            {
                                System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo clasific debe de ser (0)-Todos (1)-Cliente (2)-Proveedor (3)-Empleado (4)-Socio (5)-Estado (" + cod_ter + ")"; dt_errores.Rows.Add(row);
                            }
                        }
                        else
                        {
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo de clasific debe ser numerico (" + cod_ter + ")"; dt_errores.Rows.Add(row);
                        }
                    }
                    else
                    {
                        System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo clasific debe de estar lleno (" + cod_ter + ")"; dt_errores.Rows.Add(row);
                    }

                    #endregion

                    #region tdoc

                    string tdoc = dr["TDOC"].ToString().Trim();
                    if (!string.IsNullOrEmpty(tdoc))
                    {
                        DataTable dt_doc = SiaWin.Func.SqlDT("select cod_tdo,nom_tdo from InMae_tdoc where cod_tdo='" + tdoc + "' ;", "tdoc", idemp);
                        if (dt_doc.Rows.Count > 0)
                        {
                            dr["NOM_TDO"] = dt_doc.Rows[0]["nom_tdo"].ToString().Trim();
                        }
                        else
                        {
                            dr["NOM_TDO"] = "";
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo tdoc debe de tener los codigos de la maestra de tipo de documentos"; dt_errores.Rows.Add(row);
                        }

                    }
                    else
                    {
                        System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo tdoc debe de estar lleno"; dt_errores.Rows.Add(row);
                    }

                    #endregion
                 
                    #region raz

                    string raz = dr["RAZ"].ToString().Trim();
                    if (!string.IsNullOrEmpty(raz))
                    {
                        if (raz.Length > 150)
                        {
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo razon social debe ser menor de 150 caracteres (" + cod_ter + ")"; dt_errores.Rows.Add(row);
                        }
                    }

                    #endregion

                    #region dir

                    string dir = dr["DIR"].ToString().Trim();
                    if (!string.IsNullOrEmpty(dir))
                    {
                        if (dir.Length > 200)
                        {
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo dir debe ser menor de 200 caracteres (" + cod_ter + ")"; dt_errores.Rows.Add(row);
                        }
                    }

                    #endregion

                    #region tipo persona

                    string tip_pers = dr["TIP_PERS"].ToString().Trim();
                    int tippers;
                    if (!string.IsNullOrEmpty(tip_pers))
                    {
                        if (int.TryParse(tip_pers, out tippers))
                        {
                            if (!(tippers >= 0 && tippers <= 1))
                            {
                                System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo tipo de persona debe de ser (0)-Natural (1)-Juridica (" + cod_ter + ")"; dt_errores.Rows.Add(row);
                            }
                        }
                        else
                        {
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo tipo de persona debe de ser numerico (" + cod_ter + ")"; dt_errores.Rows.Add(row);
                        }
                    }
                    else
                    {
                        System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo tipo de persona debe de estar lleno (" + cod_ter + ")"; dt_errores.Rows.Add(row);
                    }

                    #endregion

                    #region dv

                    string dv = dr["DV"].ToString().Trim();
                    if (!string.IsNullOrEmpty(dv))
                    {
                        if (dv.Length > 1)
                        {
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo digito de verificacion debe ser menor de 1 caracteres (" + cod_ter + ")"; dt_errores.Rows.Add(row);
                        }
                    }

                    #endregion

                    #region codigo ciudad

                    string cod_ciu = dr["COD_CIU"].ToString().Trim();
                    if (!string.IsNullOrEmpty(cod_ciu))
                    {
                        string query = "select cod_muni,nom_muni from MmMae_muni where cod_muni='" + cod_ciu + "' ;";

                        DataTable dt_ciu = SiaWin.Func.SqlDT(query, "ciudad", idemp);

                        if (dt_ciu.Rows.Count > 0)
                        {
                            dr["NOM_MUNI"] = dt_ciu.Rows[0]["nom_muni"].ToString().Trim();
                        }
                        else
                        {
                            dr["NOM_MUNI"] = "";
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo codigo de ciudad debe de tener los codigos de la maestra de ciudades  (" + cod_ter + ")"; dt_errores.Rows.Add(row);
                        }

                    }

                    #endregion

                    #region codigo pais

                    string cod_pais = dr["COD_PAIS"].ToString().Trim();
                    if (!string.IsNullOrEmpty(cod_pais))
                    {
                        DataTable dt_pais = SiaWin.Func.SqlDT("select cod_pais,nom_pais from MmMae_pais where cod_pais='" + cod_pais + "' ;", "pais", idemp);
                        if (dt_pais.Rows.Count > 0)
                        {
                            dr["NOM_PAIS"] = dt_pais.Rows[0]["nom_pais"].ToString().Trim();
                        }
                        else
                        {
                            dr["NOM_PAIS"] = "";
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo codigo de pais debe de tener los codigos de la maestra de paises  (" + cod_ter + ")"; dt_errores.Rows.Add(row);
                        }

                    }

                    #endregion

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

                if (MessageBox.Show("Usted desea generar importacion y/o verificacion de terceros?", "Importacion", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {


                    if (dt.Rows.Count > 0)
                    {
                        string query = "";
                        foreach (System.Data.DataRow item in dt.Rows)
                        {
                            bool existe = Convert.ToBoolean(item["TER_EXIST"]);

                            string cod_ter = item["COD_TER"].ToString().Trim();
                            string nom_ter = item["NOM_TER"].ToString().Trim();
                            string dir1 = item["DIR1"].ToString().Trim();
                            string tel1 = item["TEL1"].ToString().Trim();
                            string email = item["EMAIL"].ToString().Trim();
                            string fec_ing = item["FEC_ING"].ToString().Trim();
                            string tip_prv = item["TIP_PRV"].ToString().Trim();
                            string estado = item["ESTADO"].ToString().Trim();
                            string clasific = item["CLASIFIC"].ToString().Trim();
                            string tdoc = item["TDOC"].ToString().Trim();
                            string apl1 = item["APL1"].ToString().Trim();
                            string apl2 = item["APL2"].ToString().Trim();
                            string nom1 = item["NOM1"].ToString().Trim();
                            string nom2 = item["NOM2"].ToString().Trim();
                            string raz = item["RAZ"].ToString().Trim();
                            string dir = item["DIR"].ToString().Trim();
                            string tip_pers = item["TIP_PERS"].ToString().Trim();
                            string dv = item["DV"].ToString().Trim();
                            string cod_ciu = item["cod_ciu"].ToString().Trim();
                            string cod_pais = item["cod_pais"].ToString().Trim();

                            if (existe)
                            {
                                query += "update Comae_ter set nom_ter='" + nom_ter + "',dir1='" + dir1 + "',tel1='" + tel1 + "',email='" + email + "',fec_ing='" + fec_ing + "',tip_prv='" + tip_prv + "',estado=" + estado + ",clasific=" + clasific + ",tdoc='" + tdoc + "'," +
                                                         "apl1='" + apl1 + "',apl2='" + apl2 + "',nom1='" + nom1 + "',nom2='" + nom2 + "',RAZ='" + raz + "',dir='" + dir + "',tip_pers='" + tip_pers + "',dv='" + dv + "',cod_ciu='" + cod_ciu + "',cod_pais='" + cod_pais + "' where cod_ter='" + cod_ter + "';";
                            }
                            else
                            {
                                query += "insert into Comae_ter (cod_ter,nom_ter,dir1,tel1,email,fec_ing,tip_prv,estado,clasific,tdoc,apl1,apl2,nom1,nom2,RAZ,dir,tip_pers,dv,cod_ciu,cod_pais) " +
                           "values ('" + cod_ter + "', '" + nom_ter + "','" + dir1 + "','" + tel1 + "','" + email + "','" + fec_ing + "','" + tip_prv + "','" + estado + "'," +
                           "'" + clasific + "','" + tdoc + "','" + apl1 + "','" + apl2 + "','" + nom1 + "','" + nom2 + "','" + raz + "','" + dir + "','" + tip_pers + "'," +
                           "'" + dv + "','" + cod_ciu + "','" + cod_pais + "');";
                            }
                        }

                        if (SiaWin.Func.SqlCRUD(query, idemp) == true) { MessageBox.Show("la importacion fue exitosa", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    dataGridExcel.ItemsSource = null;
                    dt.Clear();
                    dt_errores.Clear();
                    Tx_total.Text = "0";
                    Tx_errores.Text = "0";
                    Tx_prv.Text = "-";
                    Tx_exist.Text = "";
                    Tx_ciudad.Text = "";
                    Tx_pais.Text = "";
                    Tx_doc.Text = "";
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
                    Tx_prv.Text = !string.IsNullOrEmpty(reflector.GetValue(rowData, "TIP_PRV_NOM").ToString()) ? reflector.GetValue(rowData, "TIP_PRV_NOM").ToString().ToUpper() : "---";
                    Tx_ciudad.Text = !string.IsNullOrEmpty(reflector.GetValue(rowData, "NOM_MUNI").ToString()) ? reflector.GetValue(rowData, "NOM_MUNI").ToString().ToUpper() : "---";
                    Tx_pais.Text = !string.IsNullOrEmpty(reflector.GetValue(rowData, "NOM_PAIS").ToString()) ? reflector.GetValue(rowData, "NOM_PAIS").ToString().ToUpper() : "---";
                    Tx_doc.Text = !string.IsNullOrEmpty(reflector.GetValue(rowData, "NOM_TDO").ToString()) ? reflector.GetValue(rowData, "NOM_TDO").ToString().ToUpper() : "---";

                    if (!string.IsNullOrEmpty(reflector.GetValue(rowData, "TER_EXIST").ToString()))
                    {
                        bool f = Convert.ToBoolean(reflector.GetValue(rowData, "TER_EXIST"));
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
