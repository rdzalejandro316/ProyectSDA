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

namespace SiasoftAppExt
{
    /// Sia.PublicarPnt(9488,"AuditoriaTrasladosInterempresa");
    /// Sia.TabU(9488);
    public partial class AuditoriaTrasladosInterempresa : UserControl
    {
        dynamic SiaWin;
        dynamic tabitem;
        int idemp = 0;

        string cnEmp = "";
        string cod_empresa = "";

        
        DataTable tipoBod = new DataTable();
        DataTable empresas = new DataTable();

        public AuditoriaTrasladosInterempresa(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            tabitem = tabitem1;
            idemp = SiaWin._BusinessId;

            CargarTipoBod();
            CargarEmpresas();

            LoadConfig();
        }

        public void CargarTipoBod()
        {
            tipoBod.Columns.Add("tipo", typeof(string));
            tipoBod.Columns.Add("Nom_tipo", typeof(string));
            tipoBod.Rows.Add("0", "Todas");
            tipoBod.Rows.Add("1", "Bodega Principal CND");
            tipoBod.Rows.Add("2", "Punto de Venta");
            comboBoxBodegas.ItemsSource = tipoBod.DefaultView;
        }
        public void CargarEmpresas()
        {
            empresas = SiaWin.Func.SqlDT("select BusinessCode,BusinessName,BusinessNit from Business where BusinessStatus='1' ", "Empresas", 0);
            comboBoxEmpresas.ItemsSource = empresas.DefaultView;
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
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                tabitem.Logo(idLogo, ".png");
                tabitem.Title = "Auditoria Traslado de Interpresas (" + aliasemp + ")";
                FecIni.Text = DateTime.Now.ToShortDateString();
                FecFin.Text = DateTime.Now.ToShortDateString();
                //FecFin.Text = DateTime.Now.ToShortDateString();
                TabControl1.SelectedIndex = 0;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private string ArmaWhere()
        {
            string cadenawhere = null;
            //string RefI = TextBoxRefI.Text.Trim();
            //string RefF = TextBoxRefF.Text.Trim();
            ////string BodI = TextBoxBodI.Text.Trim();
            ////string BodF = TextBoxBodF.Text.Trim();
            //string TerI = TextBoxTerI.Text.Trim();
            //string VenI = TextBoxVenI.Text.Trim();
            //string TipI = TextBoxTipI.Text.Trim();
            //string TipF = TextBoxTipF.Text.Trim();
            //string GruI = TextBoxGrpI.Text.Trim();
            //string GruF = TextBoxGrpF.Text.Trim();

            //if (!string.IsNullOrEmpty(RefI) && !string.IsNullOrEmpty(RefF))
            //{
            //    cadenawhere += " and  cue.cod_ref between '" + RefI + "' and '" + RefF + "'";
            //}
            //if (!string.IsNullOrEmpty(BodI) && !string.IsNullOrEmpty(BodF))
            //{
            //    cadenawhere += " and  cue.cod_bod between '" + BodI + "' and '" + BodF + "'";
            //}
            //if (!string.IsNullOrEmpty(TerI))
            //{
            //    cadenawhere += " and  cab.cod_cli='" + TerI + "'";
            //}
            //if (!string.IsNullOrEmpty(VenI))
            //{
            //    cadenawhere += " and  cab.cod_Ven='" + VenI + "'";
            //}
            //if (!string.IsNullOrEmpty(TipI) && !string.IsNullOrEmpty(TipF))
            //{
            //    cadenawhere += " and  ref.cod_tip between '" + TipI + "' and '" + TipF + "'";
            //}
            //if (!string.IsNullOrEmpty(GruI) && !string.IsNullOrEmpty(GruF))
            //{
            //    cadenawhere += " and  ref.cod_gru between '" + GruI + "' and '" + GruF + "'";
            //}

            return cadenawhere;
        }

        public string returnTipBod()
        {

            string tipos = "";
            if (comboBoxBodegas.SelectedIndex > 0)
            {
                foreach (DataRowView ob in comboBoxBodegas.SelectedItems)
                {
                    String valueCta = ob["tipo"].ToString();
                    tipos += valueCta + ",";
                }
                string ss = tipos.Trim().Substring(tipos.Trim().Length - 1);
                if (ss == ",") tipos = tipos.Substring(0, tipos.Trim().Length - 1);
            }
            return tipos;
        }

        public string returnEmpresas()
        {
            string empresas = "";
            //if (comboBoxEmpresas.SelectedIndex > 0)
            //{
            foreach (DataRowView ob in comboBoxEmpresas.SelectedItems)
            {
                String valueCta = ob["BusinessCode"].ToString();
                empresas += valueCta + ",";
            }
            string ss = empresas.Trim().Substring(empresas.Trim().Length - 1);
            if (ss == ",") empresas = empresas.Substring(0, empresas.Trim().Length - 1);
            //}
            return empresas;
        }

        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            //this.Opacity = 0.5;
            try
            {
                //string where = ArmaWhere();

                //if (string.IsNullOrEmpty(where)) where = " ";
                //MessageBox.Show("1");
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                GridConfiguracion.IsEnabled = false;
                sfBusyIndicator.IsBusy = true;

                VentasPorProducto.ItemsSource = null;



                BtnEjecutar.IsEnabled = false;
                source.CancelAfter(TimeSpan.FromSeconds(1));
                tabitem.Progreso(true);


                string fechaCon = FecIni.Text.ToString();
                string fechaConFin = FecFin.Text.ToString();
                //string fff = FecIni.Text.ToString();
                string tipbod = returnTipBod();
                string empresas = returnEmpresas();
                string where1 = "";
                //MessageBox.Show(tipbod);
                //MessageBox.Show(empresas);


                //var slowTask = Task<DataSet>.Factory.StartNew(() => SlowDude(ffi, fff, where, cod_empresa, source.Token), source.Token);
                var slowTask = Task<DataSet>.Factory.StartNew(() => SlowDude(fechaCon,fechaConFin, where1, tipbod, empresas, source.Token), source.Token);
                await slowTask;
                //MessageBox.Show(slowTask.Result.ToString());
                BtnEjecutar.IsEnabled = true;
                tabitem.Progreso(false);
                //MessageBox.Show(((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString());
                //MessageBox.Show("xx1");
                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {

                    VentasPorProducto.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                    //Total1.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();

                    //VentaPorBodega.ItemsSource = ((DataSet)slowTask.Result).Tables[1];
                    //Total2.Text = ((DataSet)slowTask.Result).Tables[1].Rows.Count.ToString();

                    //CharVentasBodega.DataContext = ((DataSet)slowTask.Result).Tables[1];
                    //AreaSeriesVta.ItemsSource = ((DataSet)slowTask.Result).Tables[1];

                    //VentasPorCliente.ItemsSource = ((DataSet)slowTask.Result).Tables[2];
                    //Total3.Text = ((DataSet)slowTask.Result).Tables[2].Rows.Count.ToString();

                    //VentasPorVendedor.ItemsSource = ((DataSet)slowTask.Result).Tables[3];
                    //Total4.Text = ((DataSet)slowTask.Result).Tables[3].Rows.Count.ToString();

                    //VentasPorLinea.ItemsSource = ((DataSet)slowTask.Result).Tables[4];
                    //Total5.Text = ((DataSet)slowTask.Result).Tables[4].Rows.Count.ToString();

                    //VentasPorGrupo.ItemsSource = ((DataSet)slowTask.Result).Tables[5];
                    //Total6.Text = ((DataSet)slowTask.Result).Tables[5].Rows.Count.ToString();

                    //VentasPorFPago.ItemsSource = ((DataSet)slowTask.Result).Tables[6];
                    //Total7.Text = ((DataSet)slowTask.Result).Tables[6].Rows.Count.ToString();

                    //VentasPorClienteRef.ItemsSource = ((DataSet)slowTask.Result).Tables[7];
                    //Total8.Text = ((DataSet)slowTask.Result).Tables[7].Rows.Count.ToString();

                    //dataGridFP_detallado.ItemsSource = ((DataSet)slowTask.Result).Tables[8];

                    //GridDocumen.ItemsSource = ((DataSet)slowTask.Result).Tables[9];
                    //Total9.Text = ((DataSet)slowTask.Result).Tables[9].Rows.Count.ToString();

                    //VentasProvedor.ItemsSource = ((DataSet)slowTask.Result).Tables[10];
                    //Total10.Text = ((DataSet)slowTask.Result).Tables[10].Rows.Count.ToString();

                    TabControl1.SelectedIndex = 2;
                    TabControl1.SelectedIndex = 1;

                    //TABLA 0
                    //double CantNeto = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(neto)", "").ToString());
                    //double sub = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(subtotal)", "").ToString());
                    //double descto = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(val_des)", "").ToString());
                    //double iva = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(val_iva)", "").ToString());
                    //double total = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(total)", "").ToString());

                    //llenarTotales(sub, descto, iva, total, CantNeto);

                }

                this.sfBusyIndicator.IsBusy = false;
                GridConfiguracion.IsEnabled = true;
            }
            catch (Exception ex)
            {
                //this.Opacity = 1;
                BtnEjecutar.IsEnabled = true;
                tabitem.Progreso(false);
                this.sfBusyIndicator.IsBusy = false;
                GridConfiguracion.IsEnabled = true;
                MessageBox.Show(ex.Message);

            }
        }

        public void llenarTotales(double p1, double p2, double p3, double p4, double ca)
        {
            //TextCantidad1.Text = ca.ToString();
            //TextSubtotal1.Text = p1.ToString("C");
            //TextDescuento1.Text = p2.ToString("C");
            //TextIva1.Text = p3.ToString("C");
            //TextTotal1.Text = p4.ToString("C");
        }


        //var slowTask = Task<DataSet>.Factory.StartNew(() => SlowDude(fechaCon, where1, tipbod, empresas, source.Token), source.Token);        
        private DataSet SlowDude(string fechaCon, string fechaConFin, string where, string tipbod, string empresas, CancellationToken cancellationToken)
        {
            try
            {

                //MessageBox.Show("llego 2");
                DataSet jj = LoadData(fechaCon,fechaConFin, where, tipbod, empresas, cancellationToken);
                return jj;

            }
            catch (Exception e)
            {
                BtnEjecutar.IsEnabled = true;
                tabitem.Progreso(false);
                this.sfBusyIndicator.IsBusy = false;
                GridConfiguracion.IsEnabled = true;

                MessageBox.Show(e.Message);
            }
            return null;
        }

        private DataSet LoadData(string fechaCon, string fechaConFin, string where, string tipbod, string empresas, CancellationToken cancellationToken)
        {
            try
            {
                // MessageBox.Show("llego 1");
                //string cn_emp = cnEmp.Substring(0, 65) + "_SiaApp"+cnEmp.Substring(72,86);
                //MessageBox.Show(cn_emp.ToString());
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                //cmd = new SqlCommand("SpConsultaInAnalisisDeVentas", con);
                cmd = new SqlCommand("_EmpAuditoriaTrasladosInterempresa", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaIni", fechaCon);
                cmd.Parameters.AddWithValue("@FechaFin", fechaConFin);
                cmd.Parameters.AddWithValue("@BodTip", tipbod);
                cmd.Parameters.AddWithValue("@codemp", empresas);
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                con.Close();
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
                BtnEjecutar.IsEnabled = true;
                tabitem.Progreso(false);
                this.sfBusyIndicator.IsBusy = false;
                GridConfiguracion.IsEnabled = true;

                MessageBox.Show(e.Message);
                //MessageBox.Show("aqui 44");
                return null;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
            options.ExcelVersion = ExcelVersion.Excel2013;
            SfDataGrid sfdg = new SfDataGrid();
            if (((Button)sender).Tag.ToString() == "1") sfdg = VentasPorProducto;


            var excelEngine = sfdg.ExportToExcel(sfdg.View, options);
            var workBook = excelEngine.Excel.Workbooks[0];
            workBook.Worksheets[0].AutoFilters.FilterRange = workBook.Worksheets[0].UsedRange;


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
                    if (tag == "inmae_prv")
                    {
                        cmptabla = tag; cmpcodigo = "cod_prv"; cmpnombre = "nom_prv"; cmporden = "cod_prv"; cmpidrow = "idrow"; cmptitulo = "Maestra de proveedores"; cmpconexion = cnEmp; mostrartodo = true; cmpwhere = "";
                    }

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
                        //if (tag == "inmae_ref") TextBoxRefF.Text = code;
                        //if (tag == "inmae_bod") TextBoxBodF.Text = code;
                        //if (tag == "inmae_tip") TextBoxTipF.Text = code;
                        //if (tag == "inmae_prv") TextBoxGrpF.Text = code;
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
                //MessageBox.Show("aqui45");
            }

        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            tabitem.Cerrar(0);
        }

        private void BTNdetalle_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    string tag = ((Button)sender).Tag.ToString();
            //    Detalle Windows_Detalle = new Detalle();

            //    if (tag == "1")
            //    {
            //        DataRowView row = (DataRowView)VentasPorProducto.SelectedItems[0];
            //        Windows_Detalle.fecha_ini = FecIni.Text;
            //        Windows_Detalle.fecha_fin = FecFin.Text;
            //        Windows_Detalle.codigo = row["cod_ref"].ToString();
            //        Windows_Detalle.nombre = row["nom_ref"].ToString();
            //        Windows_Detalle.cnEmpExt = SiaWin._cn;
            //    }
            //    if (tag == "2")
            //    {
            //        DataRowView row = (DataRowView)VentaPorBodega.SelectedItems[0];
            //        Windows_Detalle.fecha_ini = FecIni.Text;
            //        Windows_Detalle.fecha_fin = FecFin.Text;
            //        Windows_Detalle.codigo = row["cod_bod"].ToString();
            //        Windows_Detalle.nombre = row["nom_bod"].ToString();
            //        Windows_Detalle.cnEmpExt = SiaWin._cn;
            //    }
            //    if (tag == "3")
            //    {
            //        DataRowView row = (DataRowView)VentasPorCliente.SelectedItems[0];
            //        Windows_Detalle.fecha_ini = FecIni.Text;
            //        Windows_Detalle.fecha_fin = FecFin.Text;
            //        Windows_Detalle.codigo = row["cod_cli"].ToString();
            //        Windows_Detalle.nombre = row["nom_cli"].ToString();
            //        Windows_Detalle.cnEmpExt = SiaWin._cn;

            //    }
            //    if (tag == "4")
            //    {
            //        DataRowView row = (DataRowView)VentasPorLinea.SelectedItems[0];
            //        Windows_Detalle.fecha_ini = FecIni.Text;
            //        Windows_Detalle.fecha_fin = FecFin.Text;
            //        Windows_Detalle.codigo = row["cod_tip"].ToString();
            //        Windows_Detalle.nombre = row["nom_tip"].ToString();
            //        Windows_Detalle.cnEmpExt = SiaWin._cn;
            //    }
            //    if (tag == "5")
            //    {
            //        DataRowView row = (DataRowView)VentasPorGrupo.SelectedItems[0];
            //        Windows_Detalle.fecha_ini = FecIni.Text;
            //        Windows_Detalle.fecha_fin = FecFin.Text;
            //        Windows_Detalle.codigo = row["cod_gru"].ToString();
            //        Windows_Detalle.nombre = row["nom_gru"].ToString();
            //        Windows_Detalle.cnEmpExt = SiaWin._cn;
            //    }
            //    if (tag == "6")
            //    {
            //        DataRowView row = (DataRowView)VentasPorFPago.SelectedItems[0];
            //        Windows_Detalle.fecha_ini = FecIni.Text;
            //        Windows_Detalle.fecha_fin = FecFin.Text;
            //        Windows_Detalle.codigo = row["cod_fpag"].ToString();
            //        Windows_Detalle.nombre = row["nom_pag"].ToString();
            //        Windows_Detalle.cnEmpExt = SiaWin._cn;
            //    }
            //    if (tag == "7")
            //    {
            //        DataRowView row = (DataRowView)VentasPorVendedor.SelectedItems[0];
            //        Windows_Detalle.fecha_ini = FecIni.Text;
            //        Windows_Detalle.fecha_fin = FecFin.Text;
            //        Windows_Detalle.codigo = row["cod_ven"].ToString().Trim();
            //        Windows_Detalle.nombre = row["nom_ven"].ToString().Trim();
            //        Windows_Detalle.cnEmpExt = SiaWin._cn;
            //    }


            //    Windows_Detalle.cod_empresa = cod_empresa;
            //    Windows_Detalle.tagBTN = tag;
            //    Windows_Detalle.ShowInTaskbar = false;
            //    Windows_Detalle.ShowDialog();

            //}
            //catch (Exception)
            //{
            //    MessageBox.Show("Selecione una casilla del Grid");
            //}
        }

        private void dataGrid_FilterChanged(object sender, GridFilterEventArgs e)
        {
            try
            {
                return;
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
                    //TextCantidad1.Text = cantidadX.ToString();
                    //TextSubtotal1.Text = subtotalX.ToString("C");
                    //TextDescuento1.Text = descuentoX.ToString("C");
                    //TextIva1.Text = ivaX.ToString("C");
                    //TextTotal1.Text = totalX.ToString("C");
                    //Total1.Text = VentasPorProducto.View.Records.Count.ToString();
                }
                if (tag == "2")
                {
                    ///                    TextCantidad2.Text = cantidadX.ToString();
                    //                 TextSubtotal2.Text = subtotalX.ToString("C");
                    //               TextDescuento2.Text = descuentoX.ToString("C");
                    //             TextIva2.Text = ivaX.ToString("C");
                    //           TextTotal2.Text = totalX.ToString("C");
                    //         Total2.Text = VentaPorBodega.View.Records.Count.ToString();
                }


            }
            catch (Exception w)
            {
                MessageBox.Show(w.Message);
            }

        }


    }
}