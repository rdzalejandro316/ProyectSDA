using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.XlsIO;
using Syncfusion.UI.Xaml.Grid.Converter;
using Microsoft.Win32;
using System.IO;
using System.Windows.Input;
using AnalisisDeVenta;

namespace SiasoftAppExt
{
    /// <summary>
    /// Lógica de interacción para UserControl1.xaml
    /// </summary>
    /// Sia.PublicarPnt(9301,"AnalisisDeVenta");
    public partial class AnalisisDeVenta : UserControl
    {
        dynamic SiaWin;
        dynamic tabitem;
        int idemp = 0;
        int moduloid = 0;
        //        string codbod = "";
        string cnEmp = "";
        string cod_empresa = "";

        public AnalisisDeVenta(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            tabitem = tabitem1;
            idemp = SiaWin._BusinessId;
            LoadConfig();
            //tabitem.VisibleButtonClose=false;

            // Border1.Height = Application.Current.MainWindow.ActualHeight-150;
            //this.Height = SiaWin.Height-5;

        }

        private void LoadConfig()
        {
            try
            {

                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                //cnEmp = foundRow["BusinessCn"].ToString().Trim();
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                tabitem.Logo(idLogo, ".png");
                tabitem.Title = "Analisis de Venta(" + aliasemp + ")";

                System.Data.DataRow[] drmodulo = SiaWin.Modulos.Select("ModulesCode='IN'");
                if (drmodulo == null) this.IsEnabled = false;
                moduloid = Convert.ToInt32(drmodulo[0]["ModulesId"].ToString());

                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                //GroupId = 0;
                //ProjectId = 0;
                //BusinessId = 0;
                FecIni.Text = DateTime.Now.ToShortDateString();
                FecFin.Text = DateTime.Now.ToShortDateString();

                TabControl1.SelectedIndex = 0;


            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                MessageBox.Show("aqui88");


            }
        }



        private string ArmaWhere()
        {
            string cadenawhere = null;
            string RefI = TextBoxRefI.Text.Trim();
            string RefF = TextBoxRefF.Text.Trim();
            string BodI = TextBoxBodI.Text.Trim();
            string BodF = TextBoxBodF.Text.Trim();
            string TerI = TextBoxTerI.Text.Trim();
            string VenI = TextBoxVenI.Text.Trim();
            string TipI = TextBoxTipI.Text.Trim();
            string TipF = TextBoxTipF.Text.Trim();
            string GruI = TextBoxGrpI.Text.Trim();
            string GruF = TextBoxGrpF.Text.Trim();

            //string ImpI = TextBoxImpI.Text.Trim();
            if (!string.IsNullOrEmpty(RefI) && !string.IsNullOrEmpty(RefF))
            {
                cadenawhere += " and  cue.cod_ref between '" + RefI + "' and '" + RefF + "'";
            }
            if (!string.IsNullOrEmpty(BodI) && !string.IsNullOrEmpty(BodF))
            {
                cadenawhere += " and  cue.cod_bod between '" + BodI + "' and '" + BodF + "'";
            }
            if (!string.IsNullOrEmpty(TerI))
            {
                cadenawhere += " and  cab.cod_cli='" + TerI + "'";
            }
            if (!string.IsNullOrEmpty(VenI))
            {
                cadenawhere += " and  cab.cod_Ven='" + VenI + "'";
            }
            if (!string.IsNullOrEmpty(TipI) && !string.IsNullOrEmpty(TipF))
            {
                cadenawhere += " and  ref.cod_tip between '" + TipI + "' and '" + TipF + "'";
            }
            if (!string.IsNullOrEmpty(GruI) && !string.IsNullOrEmpty(GruF))
            {
                cadenawhere += " and  ref.cod_gru between '" + GruI + "' and '" + GruF + "'";
            }

            //if (!string.IsNullOrEmpty(ImpI))
            //{
            //    cadenawhere += " and  ref.im='" + ImpI + "'";
            //}


            return cadenawhere;
        }

        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            //this.Opacity = 0.5;
            try
            {
                string where = ArmaWhere();

                if (string.IsNullOrEmpty(where)) where = " ";


                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                GridConfiguracion.IsEnabled = false;
                sfBusyIndicator.IsBusy = true;

                VentasPorProducto.ItemsSource = null;
                VentaPorBodega.ItemsSource = null;
                VentasPorCliente.ItemsSource = null;
                VentasPorLinea.ItemsSource = null;
                VentasPorGrupo.ItemsSource = null;

                CharVentasBodega.DataContext = null;
                AreaSeriesVta.ItemsSource = null;

                BtnEjecutar.IsEnabled = false;
                source.CancelAfter(TimeSpan.FromSeconds(1));
                tabitem.Progreso(true);
                string ffi = FecIni.Text.ToString();
                string fff = FecFin.Text.ToString();
                var slowTask = Task<DataSet>.Factory.StartNew(() => SlowDude(ffi, fff, where, cod_empresa, source.Token), source.Token);
                await slowTask;
                //MessageBox.Show(slowTask.Result.ToString());
                BtnEjecutar.IsEnabled = true;
                tabitem.Progreso(false);
                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {

                    VentasPorProducto.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                    Total1.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();

                    VentaPorBodega.ItemsSource = ((DataSet)slowTask.Result).Tables[1];
                    Total2.Text = ((DataSet)slowTask.Result).Tables[1].Rows.Count.ToString();

                    CharVentasBodega.DataContext = ((DataSet)slowTask.Result).Tables[1];
                    AreaSeriesVta.ItemsSource = ((DataSet)slowTask.Result).Tables[1];

                    VentasPorCliente.ItemsSource = ((DataSet)slowTask.Result).Tables[2];
                    Total3.Text = ((DataSet)slowTask.Result).Tables[2].Rows.Count.ToString();

                    VentasPorVendedor.ItemsSource = ((DataSet)slowTask.Result).Tables[3];
                    Total4.Text = ((DataSet)slowTask.Result).Tables[3].Rows.Count.ToString();

                    VentasPorLinea.ItemsSource = ((DataSet)slowTask.Result).Tables[4];
                    Total5.Text = ((DataSet)slowTask.Result).Tables[4].Rows.Count.ToString();

                    VentasPorGrupo.ItemsSource = ((DataSet)slowTask.Result).Tables[5];
                    Total6.Text = ((DataSet)slowTask.Result).Tables[5].Rows.Count.ToString();

                    VentasPorFPago.ItemsSource = ((DataSet)slowTask.Result).Tables[6];
                    Total7.Text = ((DataSet)slowTask.Result).Tables[6].Rows.Count.ToString();                    

                    VentasPorClienteRef.ItemsSource = ((DataSet)slowTask.Result).Tables[7];
                    Total8.Text = ((DataSet)slowTask.Result).Tables[7].Rows.Count.ToString();

                    dataGridFP_detallado.ItemsSource = ((DataSet)slowTask.Result).Tables[8];

                    GridDocumen.ItemsSource = ((DataSet)slowTask.Result).Tables[9];
                    Total9.Text = ((DataSet)slowTask.Result).Tables[9].Rows.Count.ToString();

                    TabControl1.SelectedIndex = 2;
                    TabControl1.SelectedIndex = 1;

                    //TABLA 0
                    double CantNeto = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(neto)", "").ToString());
                    double sub = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(subtotal)", "").ToString());
                    double descto = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(val_des)", "").ToString());
                    double iva = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(val_iva)", "").ToString());
                    double total = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(total)", "").ToString());

                    llenarTotales(sub, descto, iva, total, CantNeto);

                }

                this.sfBusyIndicator.IsBusy = false;
                GridConfiguracion.IsEnabled = true;
            }
            catch (Exception ex)
            {                
                this.Opacity = 1;
                MessageBox.Show("aqui 2.1" + ex);

            }
        }

        public void llenarTotales(double p1, double p2, double p3, double p4, double ca)
        {


            TextCantidad1.Text = ca.ToString();
            TextSubtotal1.Text = p1.ToString("C");
            TextDescuento1.Text = p2.ToString("C");
            TextIva1.Text = p3.ToString("C");
            TextTotal1.Text = p4.ToString("C");

            TextCantidad2.Text = ca.ToString();
            TextSubtotal2.Text = p1.ToString("C");
            TextDescuento2.Text = p2.ToString("C");
            TextIva2.Text = p3.ToString("C");
            TextTotal2.Text = p4.ToString("C");

            TextCantidad3.Text = ca.ToString();
            TextSubtotal3.Text = p1.ToString("C");
            TextDescuento3.Text = p2.ToString("C");
            TextIva3.Text = p3.ToString("C");
            TextTotal3.Text = p4.ToString("C");

            TextCantidad4.Text = ca.ToString();
            TextSubtotal4.Text = p1.ToString("C");
            TextDescuento4.Text = p2.ToString("C");
            TextIva4.Text = p3.ToString("C");
            TextTotal4.Text = p4.ToString("C");

            TextCantidad5.Text = ca.ToString();
            TextSubtotal5.Text = p1.ToString("C");
            TextDescuento5.Text = p2.ToString("C");
            TextIva5.Text = p3.ToString("C");
            TextTotal5.Text = p4.ToString("C");

            TextCantidad6.Text = ca.ToString();
            TextSubtotal6.Text = p1.ToString("C");
            TextDescuento6.Text = p2.ToString("C");
            TextIva6.Text = p3.ToString("C");
            TextTotal6.Text = p4.ToString("C");

            TextCantidad7.Text = ca.ToString();
            TextSubtotal7.Text = p1.ToString("C");
            TextDescuento7.Text = p2.ToString("C");
            TextIva7.Text = p3.ToString("C");
            TextTotal7.Text = p4.ToString("C");

            TextCantidad8.Text = ca.ToString();
            TextSubtotal8.Text = p1.ToString("C");
            TextDescuento8.Text = p2.ToString("C");
            TextIva8.Text = p3.ToString("C");
            TextTotal8.Text = p4.ToString("C");

            TextCantidad9.Text = ca.ToString();
            TextSubtotal9.Text = p1.ToString("C");
            TextDescuento9.Text = p2.ToString("C");
            TextIva9.Text = p3.ToString("C");
            TextTotal9.Text = p4.ToString("C");
        }

        private DataSet SlowDude(string ffi, string fff, string where, string cod_empresa, CancellationToken cancellationToken)
        {
            try
            {

                DataSet jj = LoadData(ffi, fff, where, cod_empresa, cancellationToken);
                return jj;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return null;
        }
        private DataSet LoadData(string Fi, string Ff, string where, string cod_empresa, CancellationToken cancellationToken)
        {

            try
            {

                //MessageBox.Show(cnEmp.ToString());
                //string cn_emp = cnEmp.Substring(0, 65) + "_SiaApp"+cnEmp.Substring(72,86);
                //MessageBox.Show(cn_emp.ToString());

                SqlConnection con1 = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                //cmd = new SqlCommand("SpConsultaInAnalisisDeVentas", con);
                cmd = new SqlCommand("_EmpSpConsultaInAnalisisDeVentas", con1);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaIni", Fi);//if you have parameters.
                cmd.Parameters.AddWithValue("@FechaFin", Ff);//if you have parameters.
                cmd.Parameters.AddWithValue("@Where", where);//if you have parameters.
                cmd.Parameters.AddWithValue("@codemp", cod_empresa);//if you have parameters.
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                con1.Close();
                //foreach (DataTable table in ds.Tables)
                //{
                //    //            newColumn.DefaultValue = "Your DropDownList value";
                //    System.Data.DataColumn newColumn = new System.Data.DataColumn("ven_net", typeof(System.Double));
                //    System.Data.DataColumn newColumn1 = new System.Data.DataColumn("util", typeof(System.Double));
                //    System.Data.DataColumn newColumn2 = new System.Data.DataColumn("por_util", typeof(System.Double));
                //    System.Data.DataColumn newColumn3 = new System.Data.DataColumn("por_parti", typeof(System.Double));
                //    System.Data.DataColumn newColumn4 = new System.Data.DataColumn("can_net", typeof(System.Double));
                //    ds.Tables[table.TableName].Columns.Add(newColumn);
                //    ds.Tables[table.TableName].Columns.Add(newColumn1);
                //    ds.Tables[table.TableName].Columns.Add(newColumn2);
                //    ds.Tables[table.TableName].Columns.Add(newColumn3);
                //    ds.Tables[table.TableName].Columns.Add(newColumn4);
                //}
                return ds;
                //VentasPorProducto.ItemsSource = ds.Tables[0];
                //VentaPorBodega.ItemsSource = ds.Tables[1];
                //VentasPorCliente.ItemsSource = ds.Tables[2];
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
               /// MessageBox.Show("aqui 44");
                return null;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
            options.ExcelVersion = ExcelVersion.Excel2013;
            //            MessageBox.Show(((Button)sender).Tag.ToString());
            SfDataGrid sfdg = new SfDataGrid();
            if (((Button)sender).Tag.ToString() == "1") sfdg = VentasPorProducto;
            if (((Button)sender).Tag.ToString() == "2") sfdg = VentaPorBodega;
            if (((Button)sender).Tag.ToString() == "3") sfdg = VentasPorCliente;
            if (((Button)sender).Tag.ToString() == "4") sfdg = VentasPorVendedor;
            if (((Button)sender).Tag.ToString() == "5") sfdg = VentasPorLinea;
            if (((Button)sender).Tag.ToString() == "6") sfdg = VentasPorGrupo;
            if (((Button)sender).Tag.ToString() == "7") sfdg = VentasPorFPago;
            if (((Button)sender).Tag.ToString() == "8") sfdg = VentasPorClienteRef;
            if (((Button)sender).Tag.ToString() == "9") sfdg = GridDocumen;
            //string nameFileXLS = "Kardex" + DateTime.Now.ToLocalTime().ToString();
            var excelEngine = sfdg.ExportToExcel(sfdg.View, options);
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

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == System.Windows.Input.Key.F8)
                {
                    string tag = ((TextBox)sender).Tag.ToString();

                    if (string.IsNullOrEmpty(tag)) return;
                    string cmptabla = ""; string cmpcodigo = ""; string cmpnombre = ""; string cmporden = ""; string cmpidrow = ""; string cmptitulo = ""; string cmpconexion = ""; bool mostrartodo = true; string cmpwhere = "";
                    if (tag == "inmae_ref")
                    {
                        cmptabla = tag; cmpcodigo = "cod_ref"; cmpnombre = "nom_ref"; cmporden = "nom_ref"; cmpidrow = "idrow"; cmptitulo = "Maestra de productos"; cmpconexion = cnEmp; mostrartodo = false; cmpwhere = "estado=1";
                    }
                    if (tag == "inmae_bod")
                    {
                        cmptabla = tag; cmpcodigo = "cod_bod"; cmpnombre = "nom_bod"; cmporden = "cod_bod"; cmpidrow = "idrow"; cmptitulo = "Maestra de bodegas"; cmpconexion = cnEmp; mostrartodo = true; cmpwhere = "estado=1 and ind_vta=1";
                    }
                    if (tag == "comae_ter")
                    {
                        cmptabla = tag; cmpcodigo = "cod_ter"; cmpnombre = "nom_ter"; cmporden = "nom_ter"; cmpidrow = "idrow"; cmptitulo = "Maestra de terceros"; cmpconexion = cnEmp; mostrartodo = false; cmpwhere = "";
                    }
                    if (tag == "inmae_mer")
                    {
                        cmptabla = tag; cmpcodigo = "cod_mer"; cmpnombre = "nom_mer"; cmporden = "cod_mer"; cmpidrow = "idrow"; cmptitulo = "Maestra de vendedores"; cmpconexion = cnEmp; mostrartodo = true; cmpwhere = "";
                    }
                    if (tag == "inmae_tip")
                    {
                        cmptabla = tag; cmpcodigo = "cod_tip"; cmpnombre = "nom_tip"; cmporden = "cod_tip"; cmpidrow = "idrow"; cmptitulo = "Maestra de lineas"; cmpconexion = cnEmp; mostrartodo = true; cmpwhere = "";
                    }
                    if (tag == "inmae_gru")
                    {
                        cmptabla = tag; cmpcodigo = "cod_gru"; cmpnombre = "nom_gru"; cmporden = "cod_gru"; cmpidrow = "idrow"; cmptitulo = "Maestra de grupo"; cmpconexion = cnEmp; mostrartodo = true; cmpwhere = "";
                    }

                    //MessageBox.Show(cmptabla + "-" + cmpcodigo + "-" + cmpnombre + "-" + cmporden + "-" + cmpidrow + "-" + cmptitulo + "-" + cmpconexion + "-" + cmpwhere);
                    int idr = 0; string code = "";
                    dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, cnEmp, mostrartodo, cmpwhere, idemp);
                    winb.ShowInTaskbar = false;
                    winb.Owner = Application.Current.MainWindow;
                    winb.ShowDialog();
                    idr = winb.IdRowReturn;
                    code = winb.Codigo;
                    winb = null;
                    if (idr > 0)
                    {
                        ((TextBox)sender).Text = code;
                        if (tag == "inmae_ref") TextBoxRefF.Text = code;
                        if (tag == "inmae_bod") TextBoxBodF.Text = code;
                        if (tag == "inmae_tip") TextBoxTipF.Text = code;
                        if (tag == "inmae_gru") TextBoxGrpF.Text = code;
                        var uiElement = e.OriginalSource as UIElement;
                        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    }
                    e.Handled = true;
                }
                if (e.Key == Key.Enter)
                {
                    var uiElement = e.OriginalSource as UIElement;
                    uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                MessageBox.Show("aqui45");
            }

        }

        private void TextBoxRefI_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            MessageBox.Show(e.Key.ToString());
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {

            tabitem.Cerrar(0);
        }


        //*****************************************************************



        private void BTNdetalle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tag = ((Button)sender).Tag.ToString();
                Detalle Windows_Detalle = new Detalle();

                if (tag == "1")
                {
                    DataRowView row = (DataRowView)VentasPorProducto.SelectedItems[0];
                    Windows_Detalle.fecha_ini = FecIni.Text;
                    Windows_Detalle.fecha_fin = FecFin.Text;
                    Windows_Detalle.codigo = row["cod_ref"].ToString();
                    Windows_Detalle.nombre = row["nom_ref"].ToString();
                    Windows_Detalle.cnEmpExt = cnEmp;
                }
                if (tag == "2")
                {
                    DataRowView row = (DataRowView)VentaPorBodega.SelectedItems[0];
                    Windows_Detalle.fecha_ini = FecIni.Text;
                    Windows_Detalle.fecha_fin = FecFin.Text;
                    Windows_Detalle.codigo = row["cod_bod"].ToString();
                    Windows_Detalle.nombre = row["nom_bod"].ToString();
                    Windows_Detalle.cnEmpExt = cnEmp;
                }
                if (tag == "3")
                {
                    DataRowView row = (DataRowView)VentasPorCliente.SelectedItems[0];
                    Windows_Detalle.fecha_ini = FecIni.Text;
                    Windows_Detalle.fecha_fin = FecFin.Text;
                    Windows_Detalle.codigo = row["cod_cli"].ToString();
                    Windows_Detalle.nombre = row["nom_cli"].ToString();
                    Windows_Detalle.cnEmpExt = cnEmp;

                }
                if (tag == "4")
                {
                    DataRowView row = (DataRowView)VentasPorLinea.SelectedItems[0];
                    Windows_Detalle.fecha_ini = FecIni.Text;
                    Windows_Detalle.fecha_fin = FecFin.Text;
                    Windows_Detalle.codigo = row["cod_tip"].ToString();
                    Windows_Detalle.nombre = row["nom_tip"].ToString();
                    Windows_Detalle.cnEmpExt = cnEmp;
                }
                if (tag == "5")
                {
                    DataRowView row = (DataRowView)VentasPorGrupo.SelectedItems[0];
                    Windows_Detalle.fecha_ini = FecIni.Text;
                    Windows_Detalle.fecha_fin = FecFin.Text;
                    Windows_Detalle.codigo = row["cod_gru"].ToString();
                    Windows_Detalle.nombre = row["nom_gru"].ToString();
                    Windows_Detalle.cnEmpExt = cnEmp;
                }
                if (tag == "6")
                {
                    DataRowView row = (DataRowView)VentasPorFPago.SelectedItems[0];
                    Windows_Detalle.fecha_ini = FecIni.Text;
                    Windows_Detalle.fecha_fin = FecFin.Text;
                    Windows_Detalle.codigo = row["cod_fpag"].ToString();
                    Windows_Detalle.nombre = row["nom_pag"].ToString();
                    Windows_Detalle.cnEmpExt = cnEmp;
                }


                Windows_Detalle.tagBTN = tag;
                Windows_Detalle.ShowInTaskbar = false;
                Windows_Detalle.ShowDialog();

            }
            catch (Exception)
            {
                MessageBox.Show("Selecione una casilla del Grid");
            }
        }



        private void dataGrid_FilterChanged(object sender, GridFilterEventArgs e)
        {
            try
            {
                string tag = ((SfDataGrid)sender).Tag.ToString();

                var provider = (sender as SfDataGrid).View.GetPropertyAccessProvider();
                var records = (sender as SfDataGrid).View.Records;

                double cantidadX = 0;
                double subtotalX = 0;
                double descuentoX = 0;
                double ivaX = 0;
                double totalX = 0;

                for (int i = 0; i < (sender as SfDataGrid).View.Records.Count; i++)
                {
                    cantidadX += Convert.ToDouble(provider.GetValue(records[i].Data, "neto").ToString());
                    subtotalX += Convert.ToDouble(provider.GetValue(records[i].Data, "subtotal").ToString());
                    descuentoX += Convert.ToDouble(provider.GetValue(records[i].Data, "val_des").ToString());
                    ivaX += Convert.ToDouble(provider.GetValue(records[i].Data, "val_iva").ToString());
                    totalX += Convert.ToDouble(provider.GetValue(records[i].Data, "total").ToString());
                }

                if (tag == "1")
                {
                    TextCantidad1.Text = cantidadX.ToString();
                    TextSubtotal1.Text = subtotalX.ToString("C");
                    TextDescuento1.Text = descuentoX.ToString("C");
                    TextIva1.Text = ivaX.ToString("C");
                    TextTotal1.Text = totalX.ToString("C");
                    Total1.Text = VentasPorProducto.View.Records.Count.ToString();
                }
                if (tag == "2")
                {
                    TextCantidad2.Text = cantidadX.ToString();
                    TextSubtotal2.Text = subtotalX.ToString("C");
                    TextDescuento2.Text = descuentoX.ToString("C");
                    TextIva2.Text = ivaX.ToString("C");
                    TextTotal2.Text = totalX.ToString("C");
                    Total2.Text = VentaPorBodega.View.Records.Count.ToString();
                }
                if (tag == "3")
                {
                    TextCantidad3.Text = cantidadX.ToString();
                    TextSubtotal3.Text = subtotalX.ToString("C");
                    TextDescuento3.Text = descuentoX.ToString("C");
                    TextIva3.Text = ivaX.ToString("C");
                    TextTotal3.Text = totalX.ToString("C");
                    Total3.Text = VentasPorCliente.View.Records.Count.ToString();
                }
                if (tag == "4")
                {
                    TextCantidad4.Text = cantidadX.ToString();
                    TextSubtotal4.Text = subtotalX.ToString("C");
                    TextDescuento4.Text = descuentoX.ToString("C");
                    TextIva4.Text = ivaX.ToString("C");
                    TextTotal4.Text = totalX.ToString("C");
                    Total4.Text = VentasPorVendedor.View.Records.Count.ToString();
                }
                if (tag == "5")
                {
                    TextCantidad5.Text = cantidadX.ToString();
                    TextSubtotal5.Text = subtotalX.ToString("C");
                    TextDescuento5.Text = descuentoX.ToString("C");
                    TextIva5.Text = ivaX.ToString("C");
                    TextTotal5.Text = totalX.ToString("C");
                    Total5.Text = VentasPorLinea.View.Records.Count.ToString();
                }
                if (tag == "6")
                {
                    TextCantidad6.Text = cantidadX.ToString();
                    TextSubtotal6.Text = subtotalX.ToString("C");
                    TextDescuento6.Text = descuentoX.ToString("C");
                    TextIva6.Text = ivaX.ToString("C");
                    TextTotal6.Text = totalX.ToString("C");
                    Total6.Text = VentasPorGrupo.View.Records.Count.ToString();
                }
                if (tag == "7")
                {
                    TextCantidad7.Text = cantidadX.ToString();
                    TextSubtotal7.Text = subtotalX.ToString("C");
                    TextDescuento7.Text = descuentoX.ToString("C");
                    TextIva7.Text = ivaX.ToString("C");
                    TextTotal7.Text = totalX.ToString("C");
                    Total7.Text = VentasPorFPago.View.Records.Count.ToString();
                }
                if (tag == "8")
                {
                    TextCantidad8.Text = cantidadX.ToString();
                    TextSubtotal8.Text = subtotalX.ToString("C");
                    TextDescuento8.Text = descuentoX.ToString("C");
                    TextIva8.Text = ivaX.ToString("C");
                    TextTotal8.Text = totalX.ToString("C");
                    Total8.Text = VentasPorClienteRef.View.Records.Count.ToString();
                }
                if (tag == "9")
                {
                    TextCantidad9.Text = cantidadX.ToString();
                    TextSubtotal9.Text = subtotalX.ToString("C");
                    TextDescuento9.Text = descuentoX.ToString("C");
                    TextIva9.Text = ivaX.ToString("C");
                    TextTotal9.Text = totalX.ToString("C");
                    Total9.Text = GridDocumen.View.Records.Count.ToString();
                }


            }
            catch (Exception w)
            {
                MessageBox.Show("error-f" + w);
            }


            //TextSubtotal.Text = subtotalX.ToString("C");
            //TextDescuento.Text = descuentoX.ToString("C");
            //TextIva.Text = ivaX.ToString("C");
            //TextTotal.Text = totalX.ToString("C");

        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }


        private void BtnDocumento_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)GridDocumen.SelectedItems[0];
                string num_trn = row["num_trn"].ToString().Trim();
                string cod_trn = row["cod_trn"].ToString().Trim();
                int id = idreg(cod_trn, num_trn);
                if (id > 0)
                {
                    SiaWin.TabTrn(1, idemp, true, id, moduloid, WinModal: true);
                }


            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir documento:" + w);
            }
        }

        public int idreg(string cod_trn, string num_trn)
        {
            int id = 0;
            DataTable tabla = SiaWin.Func.SqlDT("select * from incab_doc where cod_trn='" + cod_trn + "' and num_trn='" + num_trn + "' ", "doc", idemp);
            if (tabla.Rows.Count > 0) id = Convert.ToInt32(tabla.Rows[0]["idreg"]);
            return id;
        }




    }
}

