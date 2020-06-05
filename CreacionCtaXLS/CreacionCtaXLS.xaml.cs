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

    //    Sia.PublicarPnt(9634,"CreacionCtaXLS");
    //    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9634,"CreacionCtaXLS");
    //    ww.ShowInTaskbar = false;
    //    ww.Owner = Application.Current.MainWindow;
    //    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //    ww.ShowDialog();

    public class cuentas : IDataErrorInfo
    {

        public string Cod_cta { get; set; }
        public string Nom_cta { get; set; }
        public string Nat_cta { get; set; }

        [Display(AutoGenerateField = false)]
        public string Error { get; set; }

        public string this[string columnName]
        {
            get
            {

                CreacionCtaXLS principal = new CreacionCtaXLS();

                if (columnName == "Cod_cta")
                {
                    if (principal.GetTableVal(Cod_cta) == true)
                    {
                        Error = "la cuenta ya existe: " + this.Cod_cta;
                        return "la cuenta ya existe: " + this.Cod_cta;
                    }

                    string vali = principal.validacion(Cod_cta.Trim());
                    if (!string.IsNullOrWhiteSpace(vali))
                    {
                        Error = vali;
                        return vali;
                    }
                }


                if (columnName == "Nat_cta")
                {
                    string val_nat = principal.valNatura(Nat_cta);
                    if (!string.IsNullOrEmpty(val_nat))
                    {
                        Error = "la naturaleza de la cuenta es incorrecta";
                        return "la naturaleza de la cuenta es incorrecta";
                    }
                }

                return string.Empty;
            }
        }

        public cuentas(string cta, string nta, string natu)
        {
            Cod_cta = cta;
            Nom_cta = nta;
            Nat_cta = natu;
        }

    }

    public partial class CreacionCtaXLS : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";


        private ObservableCollection<cuentas> _cue;
        public ObservableCollection<cuentas> Cuen
        {
            get { return _cue; }
            set { _cue = value; }
        }


        public CreacionCtaXLS()
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
                this.Title = "Creacion de cuentas XLS" + "-" + nomempresa;
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

        public void impotar()
        {

            _cue = new ObservableCollection<cuentas>();

            OpenFileDialog openfile = new OpenFileDialog();
            openfile.DefaultExt = ".xlsx";
            openfile.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            var browsefile = openfile.ShowDialog();
            string root = openfile.FileName;

            if (string.IsNullOrEmpty(root)) return;

            DataTable dt = ConvertExcelToDataTable(root);

            foreach (System.Data.DataRow row in dt.Rows)
            {
                _cue.Add(new cuentas(
                    row[0].ToString(),
                    row[1].ToString(),
                    row[2].ToString()
                    ));
            }
            dataGridExcel.ItemsSource = _cue;

            Tx_ruta.Text = root;
            TX_total.Text = _cue.Count.ToString();
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


        public bool GetTableVal(string valor)
        {
            string select = "select * from comae_cta  where  cod_cta='" + valor.Trim() + "'  ";
            System.Data.DataTable dt = SiaWin.Func.SqlDT(select, "tabla", idemp);
            return dt.Rows.Count > 0 ? true : false;
        }


        public string validacion(string valor)
        {
            string ret = "";
            if (string.IsNullOrEmpty(valor))
                ret = "el campo de cuenta es encuentra en vacio";
            if (valor.Length > 15)
                ret = "el campo de cuenta es mayor a 15 caracteres";

            return ret;
        }

        public string valNatura(string valor)
        {
            //MessageBox.Show("valida:"+valor);
            string ret = "";
            if (valor == "D" || valor == "C")
                ret = string.Empty;
            else
                ret = "la naturaleza de la cuenta se encuentra mal";
            return ret;
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
                    worksheet.Range["C1"].Text = "NAT_CTA";

                    worksheet.Range["A1:C1"].CellStyle.Font.Bold = true;

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
            foreach (var item in _cue)
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
                foreach (var item in _cue)
                    query += "insert into comae_cta (cod_cta,nom_cta,nat_cta,ind_act,ind_ter,ind_bal) values ('"+item.Cod_cta+ "','" + item.Nom_cta + "','" + item.Nat_cta + "','1','1','1');";

                if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                {
                    MessageBox.Show("la creacion de cuentas fue exitosa");
                    dataGridExcel.ItemsSource = null;
                }
            }
        }












    }
}
