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
    public class Terceros : IDataErrorInfo
    {


        public string Cod_ter { get; set; }
        public string Nom_ter { get; set; }
        public string Dir1 { get; set; }
        public string Tel1 { get; set; }
        public string Email { get; set; }
        public string Fec_ing { get; set; }
        public string Tip_prv { get; set; }
        public string Estado { get; set; }
        public string Clasific { get; set; }
        public string Tdoc { get; set; }
        public string Apl1 { get; set; }
        public string Apl2 { get; set; }
        public string Nom1 { get; set; }
        public string Nom2 { get; set; }
        public string Raz { get; set; }
        public string Dir { get; set; }
        public string Tip_pers { get; set; }
        public string Dv { get; set; }
        public string Cod_ciu { get; set; }
        public string Cod_pais { get; set; }


        [Display(AutoGenerateField = false)]
        public string Error { get; set; }

        public string this[string columnName]
        {
            get
            {

                ImportacionTerceros principal = new ImportacionTerceros();

                if (columnName == "Cod_ter")
                {
                    if (principal.GetTableVal(Cod_ter, "cod_ter") == false)
                    {
                        Error = "El tercero no existe: " + this.Cod_ter;
                        return "El tercero no existe: " + this.Cod_ter;
                    }


                }

                //if (columnName == "Cod_pais")
                //{
                //if (principal.GetTableVal(Cod_pais, "cod_pais") == false)
                //return "el codigo del pais no existe: " + this.Cod_ter;
                //}

                //if (columnName == "Cod_ciu")
                //{
                //if (principal.GetTableVal(Cod_ciu, "cod_ciu") == false)
                //return "el codigo de ciudad no existe: " + this.Cod_ter;
                //}
                return string.Empty;
            }
        }

        public Terceros(
            string cod_ter, string nom_ter, string dir1, string tel1, string email, string fec_ing, string tip_prv, string estado, string clasific, string tdoc, string apl1, string apl2,
            string nom1, string nom2, string raz, string dir, string tip_pers, string dv, string cod_ciu, string cod_pais
        )
        {
            Cod_ter = cod_ter;
            Nom_ter = nom_ter;
            Dir1 = dir1;
            Tel1 = tel1;
            Email = email;
            Fec_ing = fec_ing;
            Tip_prv = tip_prv;
            Estado = estado;
            Clasific = clasific;
            Tdoc = tdoc;
            Apl1 = apl1;
            Apl2 = apl2;
            Nom1 = nom1;
            Nom2 = nom2;
            Raz = raz;
            Dir = dir;
            Tip_pers = tip_pers;
            Dv = dv;
            Cod_ciu = cod_ciu;
            Cod_pais = cod_pais;
        }
    }

    public partial class ImportacionTerceros : Window
    {
        //    Sia.PublicarPnt(9629,"ImportacionTerceros");
        //    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9629,"ImportacionTerceros");
        //    ww.ShowInTaskbar = false;
        //    ww.Owner = Application.Current.MainWindow;
        //    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
        //    ww.ShowDialog();

        private ObservableCollection<Terceros> _ter;
        public ObservableCollection<Terceros> Ter
        {
            get { return _ter; }
            set { _ter = value; }
        }

        public System.Data.DataTable TblCon_XLS = new System.Data.DataTable();

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public ImportacionTerceros()
        {
            InitializeComponent();
            this.DataContext = this;
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
                this.Title = "Importacion Terceros empresa " + "-" + nomempresa;
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
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

        public void impotar()
        {

            _ter = new ObservableCollection<Terceros>();

            OpenFileDialog openfile = new OpenFileDialog();
            openfile.DefaultExt = ".xlsx";
            openfile.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            var browsefile = openfile.ShowDialog();
            string root = openfile.FileName;

            if (string.IsNullOrEmpty(root)) return;

            DataTable dt = ConvertExcelToDataTable(root);

            foreach (System.Data.DataRow row in dt.Rows)
            {
                _ter.Add(new Terceros(
                    row[0].ToString(),
                    row[1].ToString(),
                    row[2].ToString(),
                    row[3].ToString(),
                    row[4].ToString(),
                    row[5].ToString(),
                    row[6].ToString(),
                    row[7].ToString(),
                    row[8].ToString(),
                    row[9].ToString(),
                    row[10].ToString(),
                    row[11].ToString(),
                    row[12].ToString(),
                    row[13].ToString(),
                    row[14].ToString(),
                    row[15].ToString(),
                    row[16].ToString(),
                    row[17].ToString(),
                    row[18].ToString(),
                    row[19].ToString()
                    ));
            }
            //validarCamposEntrantes(_cuerpo);
            dataGridExcel.ItemsSource = Ter;
        }



        public bool GetTableVal(string valor, string column)
        {
            var valores = ditribucion(column);
            string select = "select " + valores.Item2 + "  from  " + valores.Item1 + " where  " + valores.Item2 + "='" + valor.Trim() + "'  ";
            if (column == "cod_suc")
            {
                MessageBox.Show(select);
            }

            System.Data.DataTable dt = SiaWin.Func.SqlDT(select, "tabla", idemp);
            return dt.Rows.Count > 0 ? true : false;
        }


        public Tuple<string, string> ditribucion(string column)
        {
            string tabla = ""; string campo = "";

            switch (column)
            {
                case "cod_ter":
                    tabla = "comae_ter"; campo = "cod_ter";
                    break;
                case "cod_ciu":
                    tabla = "comae_ciu"; campo = "cod_ciu";
                    break;
                case "cod_pais":
                    tabla = "comae_pais"; campo = "cod_pais";
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


        private void BtnEjecuter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "";
                foreach (var item in _ter)
                {

                    string cod_ter = item.Cod_ter;
                    string nom_ter = item.Nom_ter;
                    string dir1 = item.Dir1;
                    string tel1 = item.Tel1;
                    string email = item.Email;
                    string fec_ing = item.Fec_ing;

                    int temp_tip_prv;
                    string tip_prv = int.TryParse(item.Tip_prv, out temp_tip_prv) ? item.Tip_prv : "0";
                                                          
                    int temp_estado;
                    string estado = int.TryParse(item.Estado, out temp_estado) ? item.Estado : "0";

                    int temp_clasi;
                    string clasific = int.TryParse(item.Clasific, out temp_clasi) ? item.Clasific : "0";
                   
                    string tdoc = item.Tdoc;
                    string apl1 = item.Apl1;
                    string apl2 = item.Apl2;
                    string nom1 = item.Nom1;
                    string nom2 = item.Nom2;
                    string raz = item.Raz;
                    string dir = item.Dir;
                    
                    int temp_tip_pers;
                    string tip_pers = int.TryParse(item.Tip_pers, out temp_tip_pers) ? item.Tip_pers : "0";

                    string Dv = item.Dv;
                    string cod_ciu = item.Cod_ciu;
                    string cod_pais = item.Cod_pais;


                    if (string.IsNullOrEmpty(item.Error))
                    {
                        query += "update Comae_ter set nom_ter='" + nom_ter + "',dir1='" + dir + "',tel1='" + tel1 + "',email='" + email + "',fec_ing='" + fec_ing + "',tip_prv=" + tip_prv + ",estado=" + estado + ",clasific=" + clasific + ",tdoc='" + tdoc + "'," +
                            "apl1='" + apl1 + "',apl2='" + apl2 + "',nom1='" + nom1 + "',nom2='" + nom2 + "',RAZ='" + raz + "',dir='" + dir + "',tip_pers='" + tip_pers + "',dv='" + Dv + "',cod_ciu='" + cod_ciu + "',cod_pais='" + cod_pais + "' where cod_ter='" + cod_ter + "';";
                    }
                    else
                    {
                        query += "insert into Comae_ter (cod_ter,nom_ter,dir1,tel1,email,fec_ing,tip_prv,estado,clasific,tdoc,apl1,apl2,nom1,nom2,RAZ,dir,tip_pers,dv,cod_ciu,cod_pais) " +
                            "values ('" + cod_ter + "', '" + nom_ter + "','" + dir1 + "','" + tel1 + "','" + email + "','" + fec_ing + "','" + tip_prv + "','" + estado + "'," +
                            "'" + clasific + "','" + tdoc + "','" + apl1 + "','" + apl2 + "','" + nom1 + "','" + nom2 + "','" + raz + "','" + dir + "','" + tip_pers + "'," +
                            "'" + Dv + "','" + cod_ciu + "','" + cod_pais + "');";                        
                    }
                }


                //MessageBox.Show(query);

                if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                {
                    MessageBox.Show("el proceso se ejecuto exitosamente");
                    dataGridExcel.ItemsSource = null;
                }
                else
                {
                    MessageBox.Show("fallo el proceso por favor verifique los campos");
                }


            }
            catch (Exception w)
            {
                MessageBox.Show("ERROR AL EJECUTAR EL PROCESO:" + w);
            }
        }



    }
}
