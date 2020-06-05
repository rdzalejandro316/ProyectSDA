using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using Syncfusion.XlsIO;
using Syncfusion.UI.Xaml.Grid.Converter;
using Microsoft.Win32;
using System.IO;
using System.Data;
using System.Linq;

namespace SiasoftAppExt
{
    public partial class AnalisisPV : Window
    {
        dynamic SiaWin;
        int idemp = 0;
        string cnEmp = "";
        public string cod_clie;
        public string nom_clie;
        bool salirPantalla = false;
        //Sia.PublicarPnt(9414,"AnalisisPV");
        DataSet ds = new DataSet();
        public AnalisisPV()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
//            this.MaxHeight = 600;
            this.MinHeight = 600;
            this.MinWidth = 1200;
//            this.MaxWidth = 1200;
            LoadConfig();
        }
        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                FecIni.Text = DateTime.Today.AddYears(-1).ToString();
                FecFin.Text = DateTime.Now.ToShortDateString();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TextBoxTerI.Text = nom_clie;
            TextBoxTerCod.Text = cod_clie;
            Consulta();
        }
        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(string.IsNullOrEmpty(TextBoxTerCod.Text.Trim()))
                {
                    MessageBox.Show("Falta codigo de cliente..");
                    TextBoxTerCod.Focus();
                    return;
                }
                Consulta();

            }
            catch (Exception ex)
            {
                MessageBox.Show("error3:" + ex.Message,"ButtonRefresh");
                
            }
        }

        private void Consulta()
        {
            try
            {
                SqlConnection con = new SqlConnection(cnEmp);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                ds.Clear();
                cmd = new SqlCommand("SpConsultaInAnalisisDeVentasPvCliente", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaIni", FecIni.Text);
                cmd.Parameters.AddWithValue("@FechaFin", FecFin.Text);
                cmd.Parameters.AddWithValue("@codter", TextBoxTerCod.Text.Trim());
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                con.Close();
                if (ds.Tables[0].Rows.Count <= 0)
                {
                    MessageBox.Show("Cliente no tiene registros en el rango de fecha seleccionado");
                    TextBoxTerCod.Text = "";
                    TextBoxTerCod.Focus();
                    return;
                }
                VentasPorProducto.ItemsSource = ds.Tables[0];

                TextSubtotal.Text = "0";
                TextDescuento.Text = "0";
                TextIva.Text = "0";
                TextTotal.Text = "0";

                double sub = Convert.ToDouble(ds.Tables[0].Compute("Sum(subtotal)", "cod_trn='005'").ToString());
                double descto = Convert.ToDouble(ds.Tables[0].Compute("Sum(val_des)", "cod_trn='005'").ToString());
                double iva = Convert.ToDouble(ds.Tables[0].Compute("Sum(val_iva)", "cod_trn='005'").ToString());
                double total = Convert.ToDouble(ds.Tables[0].Compute("Sum(tot_tot)", "cod_trn='005'").ToString());
                double sub1 = Convert.ToDouble(ds.Tables[0].Compute("Sum(subtotal)", "cod_trn<>'005'").ToString());
                double descto1 = Convert.ToDouble(ds.Tables[0].Compute("Sum(val_des)", "cod_trn<>'005'").ToString());
                double iva1 = Convert.ToDouble(ds.Tables[0].Compute("Sum(val_iva)", "cod_trn<>'005'").ToString());
                double total1 = Convert.ToDouble(ds.Tables[0].Compute("Sum(tot_tot)", "cod_trn<>'005'").ToString());

                TextSubtotal.Text = (sub - sub1).ToString("C");
                TextDescuento.Text = (descto - descto1).ToString("C");
                TextIva.Text = (iva - iva1).ToString("C");
                TextTotal.Text = (total - total1).ToString("C");
                /// graficos resumen
                /// 

                 

                //AreaChart.Series.Clear();
                
                
                CharVentasLinea.DataContext = ds.Tables[1];
                AreaSeriesVta.ItemsSource = ds.Tables[1];
                CharVentasGrupo.DataContext= ds.Tables[2];
                AreaSeriesVtaGrupo.ItemsSource= ds.Tables[2];

                CharVentasAno.DataContext = ds.Tables[3];
                AreaSeriesVtaAno.ItemsSource = ds.Tables[3];







            }
            catch (Exception ex)
            {
                MessageBox.Show("error3:" + ex.Message, "ButtonRefresh");

            }

        }


        private void ExportaXLS_Click(object sender, RoutedEventArgs e)
        {
            
            var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
            options.ExcelVersion = ExcelVersion.Excel2013;
            var excelEngine = VentasPorProducto.ExportToExcel(VentasPorProducto.View, options);
            var workBook = excelEngine.Excel.Workbooks[0];

            SaveFileDialog sfd = new SaveFileDialog
            {
                FilterIndex = 2,
                Filter = "Excel 97 to 2003 Files(*.xls)|*.xls|Excel 2007 to 2010 Files(*.xlsx)|*.xlsx|Excel 2013 File(*.xlsx)|*.xlsx"
            };

            if (sfd.ShowDialog() == true)
            {
                using (Stream stream = sfd.OpenFile())
                {
                    if (sfd.FilterIndex == 1)
                        workBook.Version = ExcelVersion.Excel97to2003;
                    else if (sfd.FilterIndex == 2)
                        workBook.Version = ExcelVersion.Excel2010;
                    else
                        workBook.Version = ExcelVersion.Excel2013;
                    workBook.SaveAs(stream);
                }

                //Message box confirmation to view the created workbook.
                if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archvo",
                                    MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    //Launching the Excel file using the default Application.[MS Excel Or Free ExcelViewer]
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
            }

            
        }

        private void TextBoxTerCod_LostFocus(object sender, RoutedEventArgs e)
        {

            if (salirPantalla == true)
            {
                e.Handled = false;
                return;

            }
            
            TextBox textbox = ((TextBox)sender);

            //TextBox textbox = ((TextBox)sender);
            if (textbox.Text.Trim() == "")
            {
                int idr = 0; string code = ""; string nombre = "";
                dynamic xx = SiaWin.WindowBuscar("comae_ter", "cod_ter", "nom_ter", "nom_ter", "idrow", "Maestra de clientes", cnEmp, false, "");
                xx.ShowInTaskbar = false;
                xx.Owner = Application.Current.MainWindow;
                xx.ShowDialog();
                idr = xx.IdRowReturn;
                code = xx.Codigo;
                nombre = xx.Nombre;
                xx = null;
                if (idr > 0)
                {
                    TextBoxTerCod.Text = code;
                    TextBoxTerI.Text = nombre;

                }
                if (string.IsNullOrEmpty(code)) e.Handled = false;

                //ConsultaSaldoCartera();
                //if (!string.IsNullOrEmpty(TextBoxTerCod.Text.Trim())) TextBoxTerCod.Focusable = false;
                ActualizaCampos(TextBoxTerCod.Text.Trim().ToString());
            }
            else
            {
                if (!ActualizaCampos(textbox.Text.Trim()))
                {
                    MessageBox.Show("El codigo de tercereo:" + textbox.Text.Trim() + " no existe");

                    textbox.Text = "";
                    textbox.Dispatcher.BeginInvoke((Action)(() => { textbox.Focus(); }));
                }
                else
                {

                    //if (!string.IsNullOrEmpty(TextBoxTerCod.Text.Trim())) 
                }
            }
            if (TextBoxTerCod.Text.Trim().Length == 0)
            {
                textbox.Dispatcher.BeginInvoke((Action)(() => { textbox.Focus(); }));
                //e.Handled = true;
                return;
            }
        }
        private bool ActualizaCampos(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id)) return false;
                SqlDataReader dr = SiaWin.Func.SqlDR("SELECT idrow,cod_ter,nom_ter,dir1,tel1,observ FROM comae_ter where cod_ter='" + Id.ToString() + "' or idrow=" + Id.ToString(), idemp);
                int idrow = 0;
                while (dr.Read())
                {
                    idrow = Convert.ToInt32(dr["idrow"]);
                    TextBoxTerCod.Text = dr["cod_ter"].ToString().Trim();
                    TextBoxTerI.Text = dr["nom_ter"].ToString().Trim();
                }
                dr.Close();
                if (idrow == 0) return false;
                if (idrow > 0) return true;
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (System.Exception _error)
            {
                MessageBox.Show(_error.Message);
            }
            return false;
        }

        private void TextBoxTerCod_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                salirPantalla = true;
                e.Handled = true;
                this.Close();
            }
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (string.IsNullOrEmpty(TextBoxTerCod.Text.Trim()))
                {
                    int idr = 0; string code = ""; string nombre = "";
                    dynamic xx = SiaWin.WindowBuscar("comae_ter", "cod_ter", "nom_ter", "nom_ter", "idrow", "Maestra de clientes", cnEmp, false, "");
                    xx.ShowInTaskbar = false;
                    xx.Owner = Application.Current.MainWindow;
                    xx.ShowDialog();
                    idr = xx.IdRowReturn;
                    code = xx.Codigo;
                    nombre = xx.Nombre;
                    xx = null;
                    if (idr > 0)
                    {
                        TextBoxTerCod.Text = code;
                        TextBoxTerI.Text = nombre;

                    }
                }
                else
                {
                    BtnEjecutar.Focus();
                }
            }
        }

        private void TextBoxTerCod_GotFocus(object sender, RoutedEventArgs e)
        {
            
            TextBoxTerCod.Text = "";
            TextBoxTerI.Text = "";
            ds.Clear();


            //AreaChart.Series.Clear();

            //AreaSeriesVta.ItemsSource = null;
            //CharVentasGrupo.DataContext = null;

            //AreaSeriesVtaGrupo.ItemsSource = null;

            //CharVentasAno.DataContext = null;
            //AreaSeriesVtaAno.ItemsSource = null;
            //CharVentasAno.DataContext = null;
            


        }
    }


}
