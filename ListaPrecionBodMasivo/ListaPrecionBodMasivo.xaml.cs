using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

    //    Sia.PublicarPnt(9636,"ListaPrecionBodMasivo");
    //    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9636,"ListaPrecionBodMasivo");
    //    ww.ShowInTaskbar = false;
    //    ww.Owner = Application.Current.MainWindow;
    //    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //    ww.ShowDialog();

    public partial class ListaPrecionBodMasivo : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        DataTable dt = new DataTable();
        DataTable dt_errores = new DataTable();

        public ListaPrecionBodMasivo()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            LoadConfig();

            dt_errores.Columns.Add("fila", typeof(int));
            dt_errores.Columns.Add("error");
        }

        private void LoadConfig()
        {
            try
            {
                SiaWin = Application.Current.MainWindow;
                if (idemp <= 0) idemp = SiaWin._BusinessId;

                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                //idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Creacion Masiva de listado de precios - " + cod_empresa + "-" + nomempresa;
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
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

                    worksheet.Range["A1"].Text = "COD_BOD";
                    worksheet.Range["B1"].Text = "COD_REF";
                    worksheet.Range["C1"].Text = "REF_CLI";
                    //worksheet.Range["D1"].Text = "POR_DES";
                    //worksheet.Range["E1"].Text = "DESCTO";
                    //worksheet.Range["F1"].Text = "VALOR";
                    //worksheet.Range["G1"].Text = "IVA";
                    worksheet.Range["D1"].Text = "VAL_UNI";

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
                MessageBox.Show("error  al importar:" + w);
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
            CancellationToken token = source.Token;
            var slowTask = Task<DataTable>.Factory.StartNew(() => Process(dt), source.Token);
            await slowTask;

            if (((DataTable)slowTask.Result).Rows.Count > 0)
            {
                dataGridRefe.ItemsSource = ((DataTable)slowTask.Result).DefaultView;
            }

            Tx_total.Text = ((DataTable)slowTask.Result).Rows.Count.ToString();
            Tx_total_err.Text = dt_errores.Rows.Count.ToString();
            sfBusyIndicator.IsBusy = false;
        }

        private DataTable Process(DataTable dt)
        {
            try
            {
                dt.Columns.Add("COD_TER");
                dt.Columns.Add("VAL_REF", typeof(decimal));
                dt.Columns.Add("POR_DES", typeof(decimal));
                dt.Columns.Add("DESCTO", typeof(decimal));
                dt.Columns.Add("EXIST", typeof(bool));

                int i = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    // bodega --------------------------
                    string cod_bod = dr["Cod_bod"].ToString();
                    DataTable dt_bod = SiaWin.Func.SqlDT("select cod_bod,cod_ter from inmae_bod where cod_bod='" + cod_bod + "'  ", "bodegas", idemp);
                    if (dt_bod.Rows.Count > 0) dr["COD_TER"] = dt_bod.Rows[0]["cod_ter"].ToString();
                    else { DataRow row = dt_errores.NewRow(); row["fila"] = i; row["error"] = "la bodega " + dr["Cod_bod"] + " no existe"; dt_errores.Rows.Add(row); }


                    // referencia  ---------------------------------

                    string cod_ref = dr["Cod_ref"].ToString();
                    decimal val_ref = 0; decimal val = 0;

                    DataTable dt_ref = SiaWin.Func.SqlDT("select cod_ref,val_ref from inmae_ref where cod_ref='" + cod_ref + "';", "referencia", idemp);
                    if (dt_ref.Rows.Count > 0)
                    {
                        val_ref = Convert.ToDecimal(dt_ref.Rows[0]["val_ref"] == DBNull.Value || decimal.TryParse(dt_ref.Rows[0]["val_ref"].ToString(), out val) == false ? 0 : dt_ref.Rows[0]["val_ref"]);
                        dr["VAL_REF"] = val_ref;
                    }
                    else { DataRow row = dt_errores.NewRow(); row["fila"] = i; row["error"] = "la referencia " + dr["Cod_ref"] + " no existe"; dt_errores.Rows.Add(row); }
                    
                    //  -------------------------------

                    decimal dec;
                    decimal val_uni = Convert.ToDecimal(dr["Val_uni"] == DBNull.Value || decimal.TryParse(dr["Val_uni"].ToString(), out dec) == false ? 0 : dr["Val_uni"]);

                    decimal diferencia = val_ref - val_uni;
                    decimal por = val_ref == 0 ? 0 : (diferencia * 100) / val_ref;
                    decimal descto = por == 0 ? 0 : Math.Round((val_ref * por) / 100);
                    dr["POR_DES"] = por;
                    dr["DESCTO"] = descto;

                    //validacion si existe en la lista
                    DataTable dt_lista = SiaWin.Func.SqlDT("select * from InList_cli where cod_ref = '" + cod_ref + "' and Cod_bod = '" + cod_bod + "'", "lista", idemp);
                    dr["EXIST"] = dt_lista.Rows.Count > 0 ? true : false;

                    i++;
                }
                return dt;
            }
            catch (Exception e)
            {
                MessageBox.Show("en la consulta:" + e.Message);
                return null;
            }
        }



        public bool validarArchioExcel(DataTable dt)
        {
            bool flag = true;
            if (dt.Columns.Contains("Cod_bod") == false || dt.Columns.IndexOf("Cod_bod") != 0) flag = false;
            if (dt.Columns.Contains("Cod_ref") == false || dt.Columns.IndexOf("Cod_ref") != 1) flag = false;
            if (dt.Columns.Contains("Ref_cli") == false || dt.Columns.IndexOf("Ref_cli") != 2) flag = false;
            if (dt.Columns.Contains("Val_uni") == false || dt.Columns.IndexOf("Val_uni") != 3) flag = false;
            return flag;
        }


        private void BtnCrear_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridRefe.ItemsSource == null) return;
            if (dataGridRefe.View.Records.Count <= 0) return;

            try
            {
                string query = "";


                foreach (DataRow item in dt.Rows)
                {
                    string cod_ref = string.IsNullOrEmpty(item["COD_REF"].ToString()) ? " " : item["COD_REF"].ToString();

                    int error = dt_errores.Rows.Count;

                    if (error > 0)
                    {
                        MessageBox.Show("la importacion contiene errores debe de estar todo correcto", "alerta", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    };

                    if (error == 0)
                    {

                        string cod_bod = item["COD_BOD"].ToString().Trim();
                        string cod_ter = item["COD_TER"].ToString().Trim();
                        string ref_cli = item["REF_CLI"].ToString().Trim();
                        decimal val_ref = Convert.ToDecimal(item["VAL_REF"]);
                        decimal por_des = Convert.ToDecimal(item["POR_DES"]);
                        decimal descto = Convert.ToDecimal(item["DESCTO"]);
                        decimal valor = 0; decimal iva = 0;
                        decimal val_uni = Convert.ToDecimal(item["VAL_UNI"]);
                        bool flag = Convert.ToBoolean(item["EXIST"]);

                        if (flag == true)
                            query += "update InList_cli set Cod_bod='" + cod_bod + "',Cod_ter='" + cod_ter + "',Ref_cli='" + ref_cli + "',Val_ref=" + val_ref.ToString("F", CultureInfo.InvariantCulture) + ",Por_des=" + por_des.ToString("F", CultureInfo.InvariantCulture) + ",Descto=" + descto.ToString("F", CultureInfo.InvariantCulture) + ",Valor=" + valor.ToString("F", CultureInfo.InvariantCulture) + ",Iva=" + iva.ToString("F", CultureInfo.InvariantCulture) + ",Val_uni=" + val_uni.ToString("F", CultureInfo.InvariantCulture) + " where cod_bod='" + cod_bod + "' and  cod_ref='" + cod_ref + "'; ";
                        else
                            query += "insert into InList_cli (Cod_bod,Cod_ter,Cod_ref,Ref_cli,Val_ref,Por_des,Descto,Valor,Iva,Val_uni) values ('" + cod_bod + "','" + cod_ter + "','" + cod_ref + "','" + ref_cli + "'," + val_ref.ToString("F", CultureInfo.InvariantCulture) + "," + por_des.ToString("F", CultureInfo.InvariantCulture) + "," + descto.ToString("F", CultureInfo.InvariantCulture) + "," + valor.ToString("F", CultureInfo.InvariantCulture) + "," + iva.ToString("F", CultureInfo.InvariantCulture) + "," + val_uni.ToString("F", CultureInfo.InvariantCulture) + ");";
                    }
                }



                if (MessageBox.Show("usted desea subir las refrencias importadas al listado de precios", "Alerta", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    //if (SiaWin._UserId == 21)
                    //{
                        //MessageBox.Show("query:" + query);
                    //}

                    if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                    {
                        MessageBox.Show("el proceso se ejecuto exitosamente");
                    }
                    else
                    {
                        MessageBox.Show("fallo el proceso por favor verifique los campos");
                    }

                    dt.Clear();
                    dt_errores.Clear();
                    dataGridRefe.ItemsSource = null;
                    Tx_total.Text = "0";
                    Tx_total_err.Text = "0";
                }

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



    }
}
