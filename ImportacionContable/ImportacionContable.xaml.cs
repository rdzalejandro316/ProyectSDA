using Microsoft.Win32;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Globalization;

namespace SiasoftAppExt
{

    public class CuerpoContable : IDataErrorInfo
    {


        public string Cod_cta { get; set; }


        [Display(AutoGenerateField = false)]
        public string Ind_ciu { get; set; }
        public string Cod_ciu { get; set; }

        [Display(AutoGenerateField = false)]
        public string Ind_suc { get; set; }
        public string Cod_suc { get; set; }


        [Display(AutoGenerateField = false)]
        public string Ind_cco { get; set; }
        public string Cod_cco { get; set; }


        [Display(AutoGenerateField = false)]
        public string Ind_ter { get; set; }
        public string Cod_ter { get; set; }

        public string Des_mov { get; set; }
        public string Num_chq { get; set; }
        public string Doc_Mov { get; set; }
        public double Bas_mov { get; set; }
        public double Deb_mov { get; set; }
        public double Cre_mov { get; set; }
        public string Doc_cruc { get; set; }
        public string Doc_ref { get; set; }
        public string Fec_venc { get; set; }
        public string Cod_banc { get; set; }
        public string Fec_con { get; set; }



        //[Display(AutoGenerateField = false)]
        public string Error { get; set; }



        public string this[string columnName]
        {
            get
            {
                
                ImportacionContable principal = new ImportacionContable();
                if (columnName == "Cod_cta")
                {
                    var tuple = principal.GetTableValCuenta(Cod_cta);
                    Ind_ciu = tuple.Item2;
                    Ind_suc = tuple.Item3;
                    Ind_cco = tuple.Item4;
                    Ind_ter = tuple.Item5;

                    if (tuple.Item1 == false)
                    {
                        Error = "la cuenta no existe: " + this.Cod_cta;
                        return "la cuenta no existe: " + this.Cod_cta;
                    }

                }


                if (columnName == "Cod_ciu")
                {

                    if (Ind_ciu == "1")
                    {
                        if (principal.GetTableVal(Cod_ciu, "cod_ciu") == false)
                        {
                            Error = "la ciudad no existe: " + this.Cod_ciu;
                            return "la ciudad no existe: " + this.Cod_ciu;
                        }                        
                    }
                    else
                    {
                        //principal.dataGridExcel.View.BeginInit();
                        Cod_ciu = " ";
                        //principal.dataGridExcel.View.EndInit();
                        //principal.dataGridExcel.View.Refresh();                        
                        //principal.dataGridExcel.ItemsSource = null;
                        //principal.dataGridExcel.ItemsSource = principal.Cuerpo;                        
                        //principal.dataGridExcel.UpdateLayout();
                        //principal.dataGridExcel.ItemsSource = principal.Cuerpo;
                        //principal.dataGridExcel.UpdateLayout();
                        //principal.dataGridExcel.View.Refresh();
                    }

                }

                if (columnName == "Cod_suc")
                {
                    if (Ind_suc == "1")
                    {
                        if (principal.GetTableVal(Cod_suc, "cod_suc") == false)
                        {
                            Error = "la sucursal no existe: " + this.Cod_suc;
                            return "la sucursal no existe: " + this.Cod_suc;
                        }
                            
                    }
                    else
                        Cod_suc = " ";
                }

                if (columnName == "Cod_cco")
                {
                    if (Ind_cco == "1")
                    {
                        if (principal.GetTableVal(Cod_cco, "cod_cco") == false)
                        {
                            Error = "el centro de costo no existe: " + this.Cod_cco;
                            return "el centro de costo no existe: " + this.Cod_cco;
                        }
                            
                    }
                    else
                        Cod_cco = " ";

                }


                if (columnName == "Cod_ter")
                {
                    if (Ind_ter == "1")
                    {
                        if (principal.GetTableVal(Cod_ter, "cod_ter") == false)
                        {
                            Error = "el tercero no existe: " + this.Cod_ter;
                            return "el tercero no existe: " + this.Cod_ter;
                        }
                        
                    }
                    else
                        Cod_ter = " ";
                }

                return string.Empty;
            }
        }

        public CuerpoContable
        (
            string cod_cta, string cod_ciu, string cod_suc, string cod_cco, string cod_ter, string des_mov, string num_chq,
            string doc_mov, double bas_mov, double deb_mov, double cre_mov,string doc_cruc,string doc_ref,string fec_venc, string cod_banc, string fec_con
        )
        {
            Cod_cta = cod_cta;
            Cod_ciu = string.IsNullOrEmpty(cod_ciu) ? " " : cod_ciu;
            Cod_suc = string.IsNullOrEmpty(cod_suc) ? " " : cod_suc;
            Cod_cco = string.IsNullOrEmpty(cod_cco) ? " " : cod_cco;
            Cod_ter = string.IsNullOrEmpty(cod_ter) ? " " : cod_ter;

            Des_mov = string.IsNullOrEmpty(des_mov) ? " " : des_mov;
            Num_chq = string.IsNullOrEmpty(num_chq) ? " " : num_chq; 
            Doc_Mov = string.IsNullOrEmpty(doc_mov) ? " " : doc_mov;
            Bas_mov = bas_mov;
            Deb_mov = deb_mov;
            Cre_mov = cre_mov;

            Doc_cruc = string.IsNullOrEmpty(doc_cruc) ? " " : doc_cruc;
            Doc_ref = string.IsNullOrEmpty(doc_ref) ? " " : doc_ref;
            Fec_venc = string.IsNullOrEmpty(fec_venc) ? " " : fec_venc;
            Cod_banc = string.IsNullOrEmpty(cod_banc) ? " " : cod_banc;
            Fec_con = string.IsNullOrEmpty(fec_con) ? " " : fec_con;
        }
    }

    public partial class ImportacionContable : Window
    {
        //    Sia.PublicarPnt(9569,"ImportacionContable");
        //    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9569, "ImportacionContable");
        //    ww.ShowInTaskbar = false;
        //    ww.Owner = Application.Current.MainWindow;
        //    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
        //    ww.ShowDialog();

        private ObservableCollection<CuerpoContable> _cuerpo;
        public ObservableCollection<CuerpoContable> Cuerpo
        {
            get { return _cuerpo; }
            set { _cuerpo = value; }
        }

        public System.Data.DataTable TblCon_XLS = new System.Data.DataTable();

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";



        public bool generar_contable = false;

        public ImportacionContable()
        {
            InitializeComponent();
            

            this.DataContext = this;

            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            LoadConfig();
            loadColumns();
        }

        public void loadColumns()
        {
            TblCon_XLS.Columns.Add("cod_cta");
            TblCon_XLS.Columns.Add("cod_ciu");
            TblCon_XLS.Columns.Add("cod_suc");
            TblCon_XLS.Columns.Add("cod_cco");
            TblCon_XLS.Columns.Add("cod_ter");
            TblCon_XLS.Columns.Add("des_mov");
            TblCon_XLS.Columns.Add("num_chq");
            TblCon_XLS.Columns.Add("doc_mov");
            TblCon_XLS.Columns.Add("bas_mov", typeof(double));
            TblCon_XLS.Columns.Add("deb_mov", typeof(double));
            TblCon_XLS.Columns.Add("cre_mov", typeof(double));
            TblCon_XLS.Columns.Add("doc_cruc");
            TblCon_XLS.Columns.Add("doc_ref");
            TblCon_XLS.Columns.Add("fec_venc");
            TblCon_XLS.Columns.Add("cod_banc");
            TblCon_XLS.Columns.Add("fec_con");            
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
                this.Title = "Importacion Contable " + cod_empresa + "-" + nomempresa;
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);                
            }
        }

        private void BtnImpo_Click(object sender, RoutedEventArgs e)
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

            _cuerpo = new ObservableCollection<CuerpoContable>();

            OpenFileDialog openfile = new OpenFileDialog();
            openfile.DefaultExt = ".xlsx";
            openfile.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            var browsefile = openfile.ShowDialog();
            string root = openfile.FileName;

            if (string.IsNullOrEmpty(root)) return;

            DataTable dt = ConvertExcelToDataTable(root);

            foreach (System.Data.DataRow row in dt.Rows)
            {
                _cuerpo.Add(new CuerpoContable(
                    row[0].ToString(),
                    row[1].ToString(),
                    row[2].ToString(),
                    row[3].ToString(),
                    row[4].ToString(),
                    row[5].ToString(),
                    row[6].ToString(),
                    row[7].ToString(),
                    Convert.ToDouble(row[8] == DBNull.Value ? 0 : row[8]),
                    Convert.ToDouble(row[9] == DBNull.Value ? 0 : row[9]),
                    Convert.ToDouble(row[10] == DBNull.Value ? 0 : row[10]),
                    row[11].ToString(),
                    row[12].ToString(),
                    row[13].ToString(),
                    row[14].ToString(),
                    row[15].ToString()
                    ));
            }

            //validarCamposEntrantes(_cuerpo);
            dataGridExcel.ItemsSource = Cuerpo;

            double deb = Cuerpo.Sum(x => x.Deb_mov);
            double cre = Cuerpo.Sum(x => x.Cre_mov);

            Tx_deb.Text = deb.ToString("N", CultureInfo.CreateSpecificCulture("es-ES"));
            Tx_cre.Text = cre.ToString("N", CultureInfo.CreateSpecificCulture("es-ES"));




            message();

        }

        public void message()
        {

            bool flag = true;
            foreach (var item in _cuerpo)
            {

                //MessageBox.Show("yesaaaa:"+ item.Error);
                if (!string.IsNullOrEmpty(item.Error))
                {
                  //  MessageBox.Show("yes");
                    flag = false;
                }
                
            }


            Val.Visibility = Visibility.Visible;
            Val.Background = flag == false ? Brushes.Red : Brushes.Green;
            Tx_val.Text = flag == false ? "la exportacion contiene algunos errores" : "la importacion ha sido exitosa";
        }




        public bool GetTableVal(string valor, string column)
        {
            var valores = ditribucion(column);
            string select = "select " + valores.Item2 + "  from  " + valores.Item1 + " where  " + valores.Item2 + "='" + valor.Trim() + "'  ";
            if (column=="cod_suc")
            {
                MessageBox.Show(select);
            }

            System.Data.DataTable dt = SiaWin.Func.SqlDT(select, "tabla", idemp);



            return dt.Rows.Count > 0 ? true : false;
        }

        public Tuple<bool, string, string, string, string> GetTableValCuenta(string valor)
        {

            string select = "select * from comae_cta where cod_cta='" + valor + "';";
            System.Data.DataTable dt = SiaWin.Func.SqlDT(select, "tabla", idemp);

            string ind_ciu = string.Empty; string ind_suc = string.Empty; string ind_cco = string.Empty; string ind_ter = string.Empty; bool exis = false;

            if (dt.Rows.Count > 0)
            {
                ind_ciu = dt.Rows[0]["ind_ciu"].ToString().Trim();
                ind_suc = dt.Rows[0]["ind_suc"].ToString().Trim();
                ind_cco = dt.Rows[0]["ind_cco"].ToString().Trim();
                ind_ter = dt.Rows[0]["ind_ter"].ToString().Trim();
                exis = true;
            }

            var tup = new Tuple<bool, string, string, string, string>(exis, ind_ciu, ind_suc, ind_cco, ind_ter);
            return tup;
        }


        public Tuple<string, string> ditribucion(string column)
        {
            string tabla = ""; string campo = "";

            switch (column)
            {
                case "cod_cta":
                    tabla = "comae_cta"; campo = "cod_cta";
                    break;
                case "cod_ciu":
                    tabla = "comae_ciu"; campo = "cod_ciu";
                    break;
                case "cod_suc":
                    tabla = "comae_suc"; campo = "cod_suc";
                    break;
                case "cod_cco":
                    tabla = "comae_cco"; campo = "cod_cco";
                    break;
                case "cod_ter":
                    tabla = "comae_ter"; campo = "cod_ter";
                    break;
            }

            var tuple = new Tuple<string, string>(tabla, campo);
            return tuple;
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

        private void BtnPlant_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.DefaultExt = ".xlsx";
                saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
                saveFileDialog.Title = "Guardar Plantilla como...";
                saveFileDialog.ShowDialog();
                string ruta = saveFileDialog.FileName;

                if (string.IsNullOrEmpty(ruta))return;

                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    IApplication application = excelEngine.Excel;
                    application.DefaultVersion = ExcelVersion.Excel2010;
                    
                    IWorkbook workbook = application.Workbooks.Create(1);
                    IWorksheet worksheet = workbook.Worksheets[0];


                    worksheet.IsGridLinesVisible = true;

                    worksheet.Range["A1"].Text = "COD_CTA";
                    worksheet.Range["B1"].Text = "COD_CIU";
                    worksheet.Range["C1"].Text = "COD_SUC";
                    worksheet.Range["D1"].Text = "COD_CCO";
                    worksheet.Range["E1"].Text = "COD_TER";
                    worksheet.Range["F1"].Text = "DES_MOV";
                    worksheet.Range["G1"].Text = "NUM_CHQ";
                    worksheet.Range["H1"].Text = "DOC_MOV";
                    worksheet.Range["I1"].Text = "BAS_MOV";
                    worksheet.Range["J1"].Text = "DEB_MOV";
                    worksheet.Range["K1"].Text = "CRE_MOV";
                    worksheet.Range["L1"].Text = "DOC_CRUC";
                    worksheet.Range["M1"].Text = "DOC_REF";
                    worksheet.Range["N1"].Text = "FEC_VENC";
                    worksheet.Range["O1"].Text = "COD_BANC";                                        
                    worksheet.Range["P1"].Text = "FEC_CON";                    
                    worksheet.Range["A1:P1"].CellStyle.Font.Bold = true;

                    if (string.IsNullOrEmpty(ruta))
                        MessageBox.Show("Por favor, seleccione una ruta para guardar la plantilla");
                    else {
                        workbook.SaveAs(ruta);
                        MessageBox.Show("Documento Guardado");
                    }
                    
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al guardar:"+w);
            }
        }

        private void BtnGenerar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool flag = true;
                foreach (var item in _cuerpo)
                {
                    
                    if (!string.IsNullOrEmpty(item.Error))
                        flag = false;
                }

                if (flag == false)
                {
                    MessageBox.Show("se encontraron algunos errores en la exportacion por favor vea los cuadros con rojo los cuales son los que estan mal");
                    return;
                }
                else
                {                    

                    llenarDataTable();
                    generar_contable = true;
                    this.Close();
                    //SiaWin.Browse(TblCon_XLS);
                }

            }
            catch (Exception w)
            {

                MessageBox.Show("error consulte con el administrador" + w);
            }
        }



        public void llenarDataTable()
        {

            TblCon_XLS.Clear();

            foreach (var data in _cuerpo)
            {
                TblCon_XLS.Rows.Add
                    (
                        data.Cod_cta, 
                        data.Cod_ciu, 
                        data.Cod_suc, 
                        data.Cod_cco, 
                        data.Cod_ter, 
                        data.Des_mov, 
                        data.Num_chq, 
                        data.Doc_Mov, 
                        data.Bas_mov, 
                        data.Deb_mov, 
                        data.Cre_mov
                    );
            }
        }





    }
}


