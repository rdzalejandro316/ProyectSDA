using Microsoft.Win32;
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
    //    Sia.PublicarPnt(9594,"ActualizacionPreciosReferencias");
    //    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9594,"ActualizacionPreciosReferencias");
    //    ww.ShowInTaskbar = false;
    //    ww.Owner = Application.Current.MainWindow;
    //    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //    ww.ShowDialog();

    public class Referencia : IDataErrorInfo
    {
        public string Cod_ref { get; set; }
        public decimal Cos_usd { get; set; }
        public decimal Vrunc { get; set; }
        public decimal Val_ref { get; set; }
        public decimal Vr_intem { get; set; }
        public decimal Val_ref2 { get; set; }
        public int Estado { get; set; }

        public decimal Cos_usd_ref { get; set; }
        public decimal Vrunc_ref { get; set; }
        public decimal Val_ref_ref { get; set; }
        public decimal Vr_intem_ref { get; set; }
        public decimal Val_ref2_ref { get; set; }


        [Display(AutoGenerateField = false)]
        public string Error { get; set; }

        public string this[string columnName]
        {
            get
            {
                ActualizacionPreciosReferencias principal = new ActualizacionPreciosReferencias();
                if (columnName == "Cod_ref")
                {
                    var validacion = principal.GetTableVal(Cod_ref);

                    if (validacion.Item1 == false)
                    {
                        Error = "la referencia : " + this.Cod_ref + " no existe";
                        Cos_usd_ref = 0;
                        Vrunc_ref = 0;
                        Val_ref_ref = 0;
                        Vr_intem_ref = 0;
                        Val_ref2_ref = 0;
                        return "la referencia : " + this.Cod_ref + " no existe";                        
                    }
                    else
                    {
                        Cos_usd_ref = validacion.Item2;
                        Vrunc_ref = validacion.Item3;
                        Val_ref_ref = validacion.Item4;
                        Vr_intem_ref = validacion.Item5;
                        Val_ref2_ref = validacion.Item6;
                    }
                }

                return string.Empty;
            }
        }

        public Referencia(string cod_ref, decimal cos_usd, decimal vrunc, decimal val_ref, decimal vr_intem, decimal val_ref2,int estado)
        {
            Cod_ref = cod_ref;
            Cos_usd = cos_usd;
            Vrunc = vrunc;
            Val_ref = val_ref;
            Vr_intem = vr_intem;
            Val_ref2 = val_ref2;
            Estado = estado;
        }
    }


    public partial class ActualizacionPreciosReferencias : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        private ObservableCollection<Referencia> _Refe;
        public ObservableCollection<Referencia> Refe
        {
            get { return _Refe; }
            set { _Refe = value; }
        }

        public ActualizacionPreciosReferencias()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
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
                this.Title = "Actualizacion de precios masiva Masiva de referencias " + cod_empresa + "-" + nomempresa;
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        public Tuple<bool,decimal, decimal, decimal, decimal, decimal, int>  GetTableVal(string valor)
        {
            bool flag = false;
            string select = "select * from inmae_ref where cod_ref='" + valor.Trim() + "'  ";
            System.Data.DataTable dt = SiaWin.Func.SqlDT(select, "tabla", idemp);
            flag = dt.Rows.Count > 0 ? true : false;

            return new Tuple<bool, decimal, decimal, decimal, decimal, decimal,int>
                (
                    flag,
                    flag == true ? Convert.ToDecimal(dt.Rows[0]["cos_usd"]) : 0,
                    flag == true ? Convert.ToDecimal(dt.Rows[0]["vrunc"]) : 0,
                    flag == true ? Convert.ToDecimal(dt.Rows[0]["val_ref"]) : 0,
                    flag == true ? Convert.ToDecimal(dt.Rows[0]["vr_intem"]) : 0,
                    flag == true ? Convert.ToDecimal(dt.Rows[0]["val_ref2"]) : 0,
                    flag == true ? Convert.ToInt32(dt.Rows[0]["estado"]) : 0
                );
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

        public bool validarArchioExcel(DataTable dt)
        {
            bool flag = true;
            if (dt.Columns.Contains("Cod_ref") == false || dt.Columns.IndexOf("Cod_ref") != 0) flag = false;
            if (dt.Columns.Contains("Cos_usd") == false || dt.Columns.IndexOf("Cos_usd") != 1) flag = false;
            if (dt.Columns.Contains("Vrunc") == false || dt.Columns.IndexOf("Vrunc") != 2) flag = false;
            if (dt.Columns.Contains("Val_ref") == false || dt.Columns.IndexOf("Val_ref") != 3) flag = false;
            if (dt.Columns.Contains("Vr_intem") == false || dt.Columns.IndexOf("Vr_intem") != 4) flag = false;
            if (dt.Columns.Contains("Val_ref2") == false || dt.Columns.IndexOf("Val_ref2") != 5) flag = false;
            if (dt.Columns.Contains("Estado") == false || dt.Columns.IndexOf("Estado") != 6) flag = false;
            return flag;
        }

        public void impotar()
        {

            _Refe = new ObservableCollection<Referencia>();

            OpenFileDialog openfile = new OpenFileDialog();
            openfile.DefaultExt = ".xlsx";
            openfile.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            var browsefile = openfile.ShowDialog();
            string root = openfile.FileName;

            if (string.IsNullOrEmpty(root)) return;

            DataTable dt = ConvertExcelToDataTable(root);

            if (validarArchioExcel(dt) == false)
            {
                MessageBox.Show("La plantilla importada no corresponde a la que permite el sistema por favor verifique con la plantilla que genera esta pantalla", "alerta", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            

            decimal n; DateTime d;int e;
            foreach (System.Data.DataRow row in dt.Rows)
            {
                if (!string.IsNullOrEmpty(row[0].ToString()))
                {
                    _Refe.Add(new Referencia(
                        row[0].ToString(),
                        Convert.ToDecimal(row[1] == DBNull.Value || decimal.TryParse(row[1].ToString(), out n) == false ? 0 : row[1]),
                        Convert.ToDecimal(row[2] == DBNull.Value || decimal.TryParse(row[2].ToString(), out n) == false ? 0 : row[2]),
                        Convert.ToDecimal(row[3] == DBNull.Value || decimal.TryParse(row[3].ToString(), out n) == false ? 0 : row[3]),
                        Convert.ToDecimal(row[4] == DBNull.Value || decimal.TryParse(row[4].ToString(), out n) == false ? 0 : row[4]),
                        Convert.ToDecimal(row[5] == DBNull.Value || decimal.TryParse(row[5].ToString(), out n) == false ? 0 : row[5]),
                        Convert.ToInt32(row[6] == DBNull.Value || int.TryParse(row[6].ToString(), out e) == false ? 0 : row[6])
                        ));
                }
            }
            dataGridRefe.ItemsSource = Refe;
            Tx_total.Text = dt.Rows.Count.ToString();
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

                    worksheet.Range["A1"].Text = "COD_REF";
                    worksheet.Range["B1"].Text = "COS_USD";
                    worksheet.Range["C1"].Text = "VRUNC";
                    worksheet.Range["D1"].Text = "VAL_REF";
                    worksheet.Range["E1"].Text = "VR_INTEM";
                    worksheet.Range["F1"].Text = "VAL_REF2";
                    worksheet.Range["G1"].Text = "ESTADO";
                    worksheet.Range["A1:G1"].CellStyle.Font.Bold = true;

                    if (string.IsNullOrEmpty(ruta))
                        MessageBox.Show("Por favor, seleccione una ruta para guardar la plantilla");
                    else
                    {
                        workbook.SaveAs(ruta);
                    }

                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al guardar:" + w);
            }
        }

        private void BtnCrear_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridRefe.ItemsSource == null) return;
            if (dataGridRefe.View.Records.Count <= 0) return;

            try
            {
                string query = "";

                foreach (var item in _Refe)
                {
                    string cod_ref = string.IsNullOrEmpty(item.Cod_ref) ? " " : item.Cod_ref;                    

                    if (!string.IsNullOrEmpty(item.Error))

                    {
                        MessageBox.Show("la importacion contiene errores debe de estar todo correcto", "alerta", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    };

                    if (string.IsNullOrEmpty(item.Error))
                    {
                        string cos_usd = item.Cos_usd == 0 ? 
                            item.Cos_usd_ref.ToString("F", CultureInfo.InvariantCulture) : item.Cos_usd.ToString("F", CultureInfo.InvariantCulture);

                        string vrunc = item.Vrunc  == 0 ?
                            item.Vrunc_ref.ToString("F", CultureInfo.InvariantCulture) : item.Vrunc.ToString("F", CultureInfo.InvariantCulture);

                        string val_ref = item.Val_ref == 0 ?
                            item.Val_ref_ref.ToString("F", CultureInfo.InvariantCulture) : item.Val_ref.ToString("F", CultureInfo.InvariantCulture);

                        string vr_intem = item.Vr_intem == 0 ?
                            item.Vr_intem_ref.ToString("F", CultureInfo.InvariantCulture) : item.Vr_intem.ToString("F", CultureInfo.InvariantCulture);

                        string val_ref2 = item.Val_ref2 == 0 ?
                            item.Val_ref2_ref.ToString("F", CultureInfo.InvariantCulture) : item.Val_ref2.ToString("F", CultureInfo.InvariantCulture);

                        string estado = item.Estado == 0 ?
                            "0" : "1";

                        query += "update inmae_ref set cos_usd="+ cos_usd + ",vrunc=" + vrunc + ",val_ref=" + val_ref + ",vr_intem=" + vr_intem + ",val_ref2=" + val_ref2 + ",estado="+ estado + "  where cod_ref='" + cod_ref + "';";
                    }
                }



                if (MessageBox.Show("usted desea actualizar los precios de las referencias importadas", "Alerta", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                 

                    if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                    {
                        MessageBox.Show("el proceso se ejecuto exitosamente");
                    }
                    else
                    {
                        MessageBox.Show("fallo el proceso por favor verifique los campos");
                    }
                    dataGridRefe.ItemsSource = null;
                    Tx_total.Text = "0";
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("ERROR AL EJECUTAR EL PROCESO:" + w);
            }

        }
















    }
}
