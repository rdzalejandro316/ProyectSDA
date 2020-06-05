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

    //    Sia.PublicarPnt(9604,"ImportacionInventario750");
    //    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9604,"ImportacionInventario750");
    //    ww.ShowInTaskbar = false;
    //    ww.Owner = Application.Current.MainWindow;
    //    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //    ww.ShowDialog();
    public class Documetos : IDataErrorInfo
    {

        public string Cod_trn { get; set; }
        public string Num_trn { get; set; }
        public string Fec_trn { get; set; }
        public string Cod_ter { get; set; }
        public string Cod_ref { get; set; }
        public string Nom_ref { get; set; }
        public string Factura { get; set; }
        public double Cantidad { get; set; }
        public double Cos_Uni { get; set; }
        public double Cos_tot { get; set; }


        //[Display(AutoGenerateField = false)]
        public string Error { get; set; }

        public string this[string columnName]
        {
            get
            {
                ImportacionInventario750 principal = new ImportacionInventario750();

                if (columnName == "Cod_trn")
                {
                    if (principal.GetTableVal(Cod_trn, "cod_trn") == false)
                    {
                        Error = "El Codigo:" + this.Cod_trn + " no existe";
                        return "El Codigo:" + this.Cod_trn + " no existe";
                    }
                    return string.Empty;
                }

                if (columnName == "Cod_ter")
                {
                    if (principal.GetTableVal(Cod_ter, "cod_ter") == false)
                    {
                        Error = "El tercero:" + this.Cod_trn + " no existe";
                        return "El tercero:" + this.Cod_trn + " no existe";
                    }
                    return string.Empty;
                }

                if (columnName == "Cod_ref")
                {
                    if (principal.GetTableVal(Cod_ref, "cod_ref") == false)
                    {
                        Error = "la referencia:" + this.Cod_trn + " no existe";
                        return "la referencia:" + this.Cod_trn + " no existe";
                    }
                    return string.Empty;
                }

                return string.Empty;
            }
        }

        public Documetos(string cod_trn, string num_trn, string fec_trn, string cod_ter, string cod_ref, string nom_ref, string factura, double cantidad, double cos_uni, double cos_tot)
        {
            Cod_trn = string.IsNullOrEmpty(cod_trn) ? " " : cod_trn;
            Num_trn = string.IsNullOrEmpty(num_trn) ? " " : num_trn;
            Fec_trn = string.IsNullOrEmpty(fec_trn) ? " " : fec_trn;
            Cod_ter = string.IsNullOrEmpty(cod_ter) ? " " : cod_ter;
            Cod_ref = string.IsNullOrEmpty(cod_ref) ? " " : cod_ref;
            Nom_ref = string.IsNullOrEmpty(nom_ref) ? " " : nom_ref;
            Factura = string.IsNullOrEmpty(factura) ? " " : factura;
            Cantidad = cantidad;
            Cos_Uni = cos_uni;
            Cos_tot = cos_tot;
        }



    }




    public partial class ImportacionInventario750 : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        private ObservableCollection<Documetos> _docu;
        public ObservableCollection<Documetos> docu
        {
            get { return _docu; }
            set { _docu = value; }
        }


        public ImportacionInventario750()
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
                this.Title = "Importacion de documentos " + "-" + nomempresa;
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        public bool GetTableVal(string valor, string column)
        {
            var valores = ditribucion(column);
            string select = "select " + valores.Item2 + "  from  " + valores.Item1 + " where  " + valores.Item2 + "='" + valor.Trim() + "'  ";            
            System.Data.DataTable dt = SiaWin.Func.SqlDT(select, "tabla", idemp);
            return dt.Rows.Count > 0 ? true : false;
        }

        public Tuple<string, string> ditribucion(string column)
        {
            string tabla = ""; string campo = "";

            switch (column)
            {
                case "cod_trn":
                    tabla = "inmae_trn"; campo = "cod_trn";
                    break;
                case "cod_ter":
                    tabla = "comae_ter"; campo = "cod_ter";
                    break;
                case "cod_ref":
                    tabla = "inmae_ref"; campo = "cod_ref";
                    break;
            }

            var tuple = new Tuple<string, string>(tabla, campo);
            return tuple;
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

        public void impotar()
        {

            _docu = new ObservableCollection<Documetos>();

            OpenFileDialog openfile = new OpenFileDialog();
            openfile.DefaultExt = ".xlsx";
            openfile.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            var browsefile = openfile.ShowDialog();
            string root = openfile.FileName;

            if (string.IsNullOrEmpty(root)) return;
            //MessageBox.Show("A1");
            DataTable dt = ConvertExcelToDataTable(root);
            //MessageBox.Show("A2");
            foreach (System.Data.DataRow row in dt.Rows)
            {
                _docu.Add(new Documetos(
                    row[0].ToString(),
                    row[1].ToString(),
                    row[2].ToString(),
                    row[3].ToString(),
                    row[4].ToString(),
                    row[5].ToString(),
                    row[6].ToString(),
                    Convert.ToDouble(row[7] == DBNull.Value ? 0 : row[7]),
                    Convert.ToDouble(row[8] == DBNull.Value ? 0 : row[8]),
                    Convert.ToDouble(row[9] == DBNull.Value ? 0 : row[9])
                    ));
            }
            dataGridExcel.ItemsSource = _docu;

            Tx_ruta.Text = root;
            TX_total.Text = _docu.Count.ToString();
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

                    worksheet.Range["A1"].Text = "COD_TRN";
                    worksheet.Range["B1"].Text = "NUM_TRB";
                    worksheet.Range["C1"].Text = "FEC_TRN";
                    worksheet.Range["D1"].Text = "COD_TER";
                    worksheet.Range["E1"].Text = "COD_REF";
                    worksheet.Range["F1"].Text = "NOM_REF";
                    worksheet.Range["G1"].Text = "FACTURA";
                    worksheet.Range["H1"].Text = "CANTIDAD";
                    worksheet.Range["I1"].Text = "COS_UNI";
                    worksheet.Range["J1"].Text = "COS_TOT";

                    worksheet.Range["A1:J1"].CellStyle.Font.Bold = true;

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
            if (dataGridExcel.ItemsSource == null) return;

            bool flag = false;
            foreach (var item in _docu)
            {
                if (!string.IsNullOrEmpty(item.Error))
                    flag = true;
            }

            if (flag)
            {
                MessageBox.Show("la importacion contiene algunos errores por favor arreglarlos para poder realizar el proceso de creacion");
                return;
            }
            else
            {
                string query = "";


                foreach (var item in _docu)
                    query += "insert into comae_cta (cod_cta,nom_cta,nat_cta,ind_act,ind_ter,ind_bal) values ('" + item.Cod_cta + "','" + item.Nom_cta + "','" + item.Nat_cta + "','1','1','1');";

                if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                {
                    MessageBox.Show("la creacion de cuentas fue exitosa");
                    dataGridExcel.ItemsSource = null;
                }
            }
        }


      







    }
}
