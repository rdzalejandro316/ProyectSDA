using Microsoft.Win32;
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
                tabitem.Title = "Importacion 740 - " + nomempresa;
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

                    Tx_ter.Text = "";
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

                //validar campo por campo
                foreach (System.Data.DataRow dr in dt.Rows)
                {

                    #region tercero

                    string cod_ter = dr["COD_TER"].ToString().Trim();
                    if (!string.IsNullOrEmpty(cod_ter))
                    {
                        DataTable dt_ter = SiaWin.Func.SqlDT("select cod_ter,nom_ter from comae_ter where cod_ter='" + cod_ter + "'  ", "tercero", idemp);
                        dr["TER_EXIST"] = dt_ter.Rows.Count > 0 ? true : false;
                    }
                    else
                    {
                        System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el codigo de tercero debe de estar lleno"; dt_errores.Rows.Add(row);
                    }

                    #endregion

                    #region direccion

                    string dir1 = dr["DIR1"].ToString().Trim();
                    if (!string.IsNullOrEmpty(dir1))
                    {
                        if (dir1.Length > 120)
                        {
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo de direccion debe ser menor de 120 caracteristicas"; dt_errores.Rows.Add(row);
                        }
                    }
                    else
                    {
                        System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo de direccion debe de estar lleno"; dt_errores.Rows.Add(row);
                    }


                    #endregion

                    #region telefono

                    string tel1 = dr["TEL1"].ToString().Trim();
                    if (!string.IsNullOrEmpty(tel1))
                    {
                        if (tel1.Length > 50)
                        {
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo de telefono debe ser menor de 50 caracteristicas"; dt_errores.Rows.Add(row);
                        }
                    }
                    else
                    {
                        System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo de telefono debe de estar lleno"; dt_errores.Rows.Add(row);
                    }


                    #endregion

                    #region email

                    string email = dr["EMAIL"].ToString().Trim();
                    if (!string.IsNullOrEmpty(email))
                    {
                        if (email.Length > 100)
                        {
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo de email debe ser menor de 100 caracteristicas"; dt_errores.Rows.Add(row);
                        }
                    }
                    else
                    {
                        System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo de email debe de estar lleno"; dt_errores.Rows.Add(row);
                    }


                    #endregion

                    #region  fecha de ingreso

                    string fec_ing = dr["FEC_ING"].ToString().Trim();
                    DateTime fecing;
                    if (!string.IsNullOrEmpty(fec_ing))
                    {
                        if (DateTime.TryParse(fec_ing, out fecing) == false)
                        {
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo de fecha debe ser de tipo fecha (dd/MM/yyyy)"; dt_errores.Rows.Add(row);
                        }
                    }
                    else
                    {
                        System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo de fecha debe de estar lleno"; dt_errores.Rows.Add(row);
                    }


                    #endregion

                    #region tipo de provedor

                    string tip_prv = dr["TIP_PRV"].ToString().Trim();
                    int tipprv;
                    if (!string.IsNullOrEmpty(tip_prv))
                    {
                        if (int.TryParse(tip_prv, out tipprv))
                        {
                            if (!(tipprv > 0 && tipprv <= 3))
                            {
                                System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el tipo de provedor debe de ser el siguiente (0)-Regimen Comun (1)-Simplificado (2)-Gran Contribuyente (3)-Entidad Oficial "; dt_errores.Rows.Add(row);
                            }
                        }
                        else
                        {
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo tipo de provedor debe de ser numerico"; dt_errores.Rows.Add(row);
                        }
                    }
                    #endregion

                    #region estado

                    string estado = dr["ESTADO"].ToString().Trim();
                    int est;
                    if (!string.IsNullOrEmpty(estado))
                    {
                        if (int.TryParse(estado, out est))
                        {
                            if (!(est == 0 && est == 1))
                            {
                                System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo estado debe de ser (0)-inactivo (1)-activo"; dt_errores.Rows.Add(row);
                            }
                        }
                        else
                        {
                            System.Data.DataRow row = dt_errores.NewRow(); row["error"] = "el campo de estado debe ser numerico"; dt_errores.Rows.Add(row);
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
            //try
            //{
            //    string query = "";
            //    foreach (var item in _ter)
            //    {

            //        string cod_ter = item.Cod_ter;
            //        string nom_ter = item.Nom_ter;
            //        string dir1 = item.Dir1;
            //        string tel1 = item.Tel1;
            //        string email = item.Email;
            //        string fec_ing = item.Fec_ing;

            //        int temp_tip_prv;
            //        string tip_prv = int.TryParse(item.Tip_prv, out temp_tip_prv) ? item.Tip_prv : "0";

            //        int temp_estado;
            //        string estado = int.TryParse(item.Estado, out temp_estado) ? item.Estado : "0";

            //        int temp_clasi;
            //        string clasific = int.TryParse(item.Clasific, out temp_clasi) ? item.Clasific : "0";

            //        string tdoc = item.Tdoc;
            //        string apl1 = item.Apl1;
            //        string apl2 = item.Apl2;
            //        string nom1 = item.Nom1;
            //        string nom2 = item.Nom2;
            //        string raz = item.Raz;
            //        string dir = item.Dir;

            //        int temp_tip_pers;
            //        string tip_pers = int.TryParse(item.Tip_pers, out temp_tip_pers) ? item.Tip_pers : "0";

            //        string Dv = item.Dv;
            //        string cod_ciu = item.Cod_ciu;
            //        string cod_pais = item.Cod_pais;


            //        if (string.IsNullOrEmpty(item.Error))
            //        {
            //            query += "update Comae_ter set nom_ter='" + nom_ter + "',dir1='" + dir + "',tel1='" + tel1 + "',email='" + email + "',fec_ing='" + fec_ing + "',tip_prv=" + tip_prv + ",estado=" + estado + ",clasific=" + clasific + ",tdoc='" + tdoc + "'," +
            //                "apl1='" + apl1 + "',apl2='" + apl2 + "',nom1='" + nom1 + "',nom2='" + nom2 + "',RAZ='" + raz + "',dir='" + dir + "',tip_pers='" + tip_pers + "',dv='" + Dv + "',cod_ciu='" + cod_ciu + "',cod_pais='" + cod_pais + "' where cod_ter='" + cod_ter + "';";
            //        }
            //        else
            //        {
            //            query += "insert into Comae_ter (cod_ter,nom_ter,dir1,tel1,email,fec_ing,tip_prv,estado,clasific,tdoc,apl1,apl2,nom1,nom2,RAZ,dir,tip_pers,dv,cod_ciu,cod_pais) " +
            //                "values ('" + cod_ter + "', '" + nom_ter + "','" + dir1 + "','" + tel1 + "','" + email + "','" + fec_ing + "','" + tip_prv + "','" + estado + "'," +
            //                "'" + clasific + "','" + tdoc + "','" + apl1 + "','" + apl2 + "','" + nom1 + "','" + nom2 + "','" + raz + "','" + dir + "','" + tip_pers + "'," +
            //                "'" + Dv + "','" + cod_ciu + "','" + cod_pais + "');";
            //        }
            //    }


            //    //MessageBox.Show(query);

            //    if (SiaWin.Func.SqlCRUD(query, idemp) == true)
            //    {
            //        MessageBox.Show("el proceso se ejecuto exitosamente");
            //        dataGridExcel.ItemsSource = null;
            //    }
            //    else
            //    {
            //        MessageBox.Show("fallo el proceso por favor verifique los campos");
            //    }


            //}
            //catch (Exception w)
            //{
            //    MessageBox.Show("ERROR AL EJECUTAR EL PROCESO:" + w);
            //}
        }

        private void BtnErrores_Click(object sender, RoutedEventArgs e)
        {

        }



    }
}
