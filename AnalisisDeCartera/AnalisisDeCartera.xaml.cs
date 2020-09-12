using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AnalisisDeCartera;
using Syncfusion.XlsIO;
using Microsoft.Win32;
using System.IO;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.Data;
using System.Linq;
using Microsoft.Reporting.WinForms;
using System.Collections.Generic;
using System.Globalization;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9307,"AnalisisDeCartera");
    //Sia.TabU(9307);
    public partial class AnalisisDeCartera : UserControl
    {
        dynamic SiaWin;
        dynamic tabitem;
        public int idemp = 0;
        string cnEmp = "";
        string codemp = string.Empty;
        DataSet ds = new DataSet();
        DataTable Cuentas = new DataTable();

        DataTable DtCartera = new DataTable();
        DataTable DtCarteraD = new DataTable();
        string codpvta = string.Empty;
        bool columndto = false;
        public AnalisisDeCartera(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            tabitem = tabitem1;
            tabitem.Title = "Analisis de Cartera";
            tabitem.Logo(9, ".png");
            tabitem.MultiTab = false;
            //            idemp = SiaWin._BusinessId;
            if (tabitem.idemp > 0) idemp = tabitem.idemp;
            //if (tabitem.idemp <= 0) idemp = SiaWin._BusinessId;
            codpvta = SiaWin._UserTag;
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
                codemp = foundRow["BusinessCode"].ToString().Trim();
                tabitem.Logo(idLogo, ".png");
                tabitem.Title = "Analisis de Cartera(" + aliasemp + ")";
                //GroupId = 0;
                //ProjectId = 0;
                //BusinessId = 0;
                Cuentas = SiaWin.Func.SqlDT("SELECT rtrim(cod_cta) as cod_cta,rtrim(cod_cta)+'('+rtrim(nom_cta)+')' as nom_cta FROM COMAE_CTA WHERE ind_mod = 1 and (tip_apli = 3 or tip_apli = 4 ) ORDER BY COD_CTA", "Cuentas", idemp);
                comboBoxCuentas.ItemsSource = Cuentas.DefaultView;
                //vendedor
                DataTable dt_ven = SiaWin.Func.SqlDT("select rtrim(cod_mer) as cod_mer,rtrim(nom_mer) as nom_mer from inmae_mer where estado=1", "vendedor", idemp);
                comboBoxVendedor.ItemsSource = dt_ven.DefaultView;

                //comboBoxCuentas.DataContext = Cuentas;
                comboBoxCuentas.DisplayMemberPath = "nom_cta";
                comboBoxCuentas.SelectedValuePath = "cod_cta";
                FechaIni.Text = DateTime.Now.ToShortDateString();

                //seguridad
                //int grupo = SiaWin._UserGroup;
                //string cod_grupo = "";
                ////MessageBox.Show("grupo:"+grupo);
                //DataTable dtGrupo = SiaWin.Func.SqlDT("select* from Seg_Group where GroupId = '" + grupo + "'", "Cuentas", 0);
                //if (dtGrupo.Rows.Count > 0) cod_grupo = dtGrupo.Rows[0]["GroupCode"].ToString();

                //if (!string.IsNullOrEmpty(cod_grupo))
                //{
                //    bool flag = false;
                //    DataTable dtGrupoRango = SiaWin.Func.SqlDT(" select * from Seg_Group where GroupCode between '050' and '060'", "Cuentas", 0);
                //    foreach (System.Data.DataRow dr in dtGrupoRango.Rows)
                //    {
                //        if (dr["GroupCode"].ToString().Trim() == cod_grupo) flag = true;
                //    }

                //    //if (flag)
                //        //TextCod_Ven.IsEnabled = true;
                //}


                //string tag2 = SiaWin._UserTag2;
                //if (!String.IsNullOrEmpty(tag2))
                //{
                //    DataTable dt = SiaWin.Func.SqlDT("select * from inmae_mer where cod_mer='" + tag2 + "'", "Cuentas", idemp);
                //    if (dt.Rows.Count > 0)
                //    {
                //        //TextCod_Ven.Text = dt.Rows[0]["cod_mer"].ToString();
                //        //TextNombreVend.Text = dt.Rows[0]["nom_mer"].ToString();
                //        //TextCod_Ven.IsEnabled = false;
                //    }
                //    //else`mierdaa
                //       // TextCod_Ven.IsEnabled = true;
                //}

            }
            catch (Exception e)
            {
                SiaWin.seguridad.ErrorLog("Error  ", "AnalisisDeCartera-LoadConfig:" + e.Message.ToString());
                MessageBox.Show(e.Message);
            }
        }
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Salir de cartera");
            tabitem.Cerrar(0);
        }
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == System.Windows.Input.Key.F8)
                {
                    string tag = ((TextBox)sender).Tag.ToString();
                    string cmptabla = ""; string cmpcodigo = ""; string cmpnombre = ""; string cmporden = ""; string cmpidrow = ""; string cmptitulo = ""; string cmpconexion = ""; bool mostrartodo = false; string cmpwhere = "";
                    if (string.IsNullOrEmpty(tag)) return;
                    if (tag == "inmae_mer")
                    {
                        cmptabla = tag; cmpcodigo = "cod_mer"; cmpnombre = "nom_mer"; cmporden = "cod_mer"; cmpidrow = "idrow"; cmptitulo = "Maestra de vendedores"; cmpconexion = cnEmp; mostrartodo = false; cmpwhere = "";
                    }
                    if (tag == "comae_ter")
                    {
                        cmptabla = tag; cmpcodigo = "cod_ter"; cmpnombre = "nom_ter"; cmporden = "cod_ter"; cmpidrow = "idrow"; cmptitulo = "Maestra de Tercero"; cmpconexion = cnEmp; mostrartodo = false; cmpwhere = "";
                    }
                    //MessageBox.Show(cmptabla + "-" + cmpcodigo + "-" + cmpnombre + "-" + cmporden + "-" + cmpidrow + "-" + cmptitulo + "-" + cmpconexion + "-" + cmpwhere);
                    int idr = 0; string code = ""; string nom = "";
                    //dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, cnEmp, mostrartodo, cmpwhere);
                    dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, SiaWin.Func.DatosEmp(idemp), mostrartodo, cmpwhere, idEmp: idemp);
                    winb.ShowInTaskbar = false;
                    winb.Owner = Application.Current.MainWindow;
                    winb.ShowDialog();
                    idr = winb.IdRowReturn;
                    code = winb.Codigo;
                    nom = winb.Nombre;
                    winb = null;
                    if (idr > 0)
                    {
                        if (tag == "comae_ter")
                        {
                            TextCod_Ter.Text = code.Trim();
                            TextNombreTercero.Text = nom.Trim();
                            //TextCod_bod.Text = code; TextNombreBod.Text = nom;
                        }
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
                SiaWin.seguridad.ErrorLog("Error  ", "AnalisisDeCartera-PreviewKeyDown:" + ex.Message.ToString());
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (comboBoxCuentas.SelectedIndex < 0)
                {
                    MessageBox.Show("Seleccione una cuenta");
                    comboBoxCuentas.Focus();
                    return;
                }
                string Cta = "";
                if (comboBoxCuentas.SelectedIndex >= 0)
                {
                    foreach (DataRowView ob in comboBoxCuentas.SelectedItems)
                    {
                        String valueCta = ob["cod_cta"].ToString();
                        Cta += valueCta + ",";
                    }
                    string ss = Cta.Trim().Substring(Cta.Trim().Length - 1);
                    if (ss == ",") Cta = Cta.Substring(0, Cta.Trim().Length - 1);
                }
                string Ven = "";
                if (comboBoxVendedor.SelectedIndex >= 0)
                {
                    foreach (DataRowView ob in comboBoxVendedor.SelectedItems)
                    {
                        String valueCta = ob["cod_mer"].ToString();
                        Ven += valueCta + ",";

                    }
                    if (Ven.Trim() != "")
                    {
                        string ss = Ven.Trim().Substring(Ven.Trim().Length - 1);
                        if (ss == ",") Ven = Ven.Substring(0, Ven.Trim().Length - 1);
                    }
                }
                if (Cbx_Detalle.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione el tipo de consulta");
                    return;
                }

                bool detalle = Cbx_Detalle.Text == "No" ? false : true;
                string where = "";
                // carmar where
                if (string.IsNullOrEmpty(where)) where = " ";
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                //this.IsEnabled = false;
                sfBusyIndicator.IsBusy = true;
                if (detalle == true) DtCarteraD.Clear();
                if (detalle == false) DtCartera.Clear();
                //    LoadData(recordChanged());
                //dataGrid.Model.View.Refresh();
                //dataGridCxC.ClearFilters();
                //dataGridCxC.ItemsSource = null;
                //CharVentasBodega.DataContext = null;
                //ds.Clear();
                BtnEjecutar.IsEnabled = false;
                Imprimir.IsEnabled = false;
                ExportarXls.IsEnabled = false;
                ConciliarCxcCo.IsEnabled = false;
                BtnvrAbonado.IsEnabled = false;
                BtnvrDesc.IsEnabled = false;

                source.CancelAfter(TimeSpan.FromSeconds(1));
                //tabitem.Progreso(true);
                string ffi = FechaIni.Text.ToString();
                //string Vendedor = comboBoxVendedor.SelectedValue.ToString();
                string Tercero = TextCod_Ter.Text.Trim();

                //string procedure = columndto == true ? "s" : "_empSpCoAnalisisCxc";

                int exclinter = 0;
                if (CheckIncluirInter.IsChecked == true) exclinter = 1;

                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(ffi, Cta, Tercero, "", where, exclinter, Ven, detalle, source.Token), source.Token);
                await slowTask;
                BtnEjecutar.IsEnabled = true;
                Imprimir.IsEnabled = true;
                ExportarXls.IsEnabled = true;
                ConciliarCxcCo.IsEnabled = true;
                BtnvrAbonado.IsEnabled = true;
                BtnvrDesc.IsEnabled = true;
                //tabitem.Progreso(false);
                resetTotales();

                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {

                    if (detalle == false)
                    {
                        //DataTable dt = ((DataSet)slowTask.Result).Tables[0];
                        //SiaWin.Browse(dt);                        
                        DtCartera = ((DataSet)slowTask.Result).Tables["C"];
                        dataGridCxC.ItemsSource = ((DataSet)slowTask.Result).Tables["C"];
                        double valorCxC, valorCxCAnt = 0;
                        //double valorCxCAnt = 0;
                        double valorCxP = 0;
                        double valorCxPAnt = 0;
                        double saldoCxC = 0;
                        double saldoCxCAnt = 0;
                        double saldoCxP = 0;
                        double saldoCxPAnt = 0;
                        double.TryParse(((DataSet)slowTask.Result).Tables["C"].Compute("Sum(valor)", "tip_apli=3").ToString(), out valorCxC);
                        double.TryParse(((DataSet)slowTask.Result).Tables["C"].Compute("Sum(valor)", "tip_apli=4").ToString(), out valorCxCAnt);
                        double.TryParse(((DataSet)slowTask.Result).Tables["C"].Compute("Sum(valor)", "tip_apli=1").ToString(), out valorCxP);
                        double.TryParse(((DataSet)slowTask.Result).Tables["C"].Compute("Sum(valor)", "tip_apli=2").ToString(), out valorCxPAnt);
                        double.TryParse(((DataSet)slowTask.Result).Tables["C"].Compute("Sum(saldo)", "tip_apli=3").ToString(), out saldoCxC);
                        double.TryParse(((DataSet)slowTask.Result).Tables["C"].Compute("Sum(saldo)", "tip_apli=4").ToString(), out saldoCxCAnt);
                        double.TryParse(((DataSet)slowTask.Result).Tables["C"].Compute("Sum(saldo)", "tip_apli=1").ToString(), out saldoCxP);
                        double.TryParse(((DataSet)slowTask.Result).Tables["C"].Compute("Sum(saldo)", "tip_apli=2").ToString(), out saldoCxPAnt);
                        TextCxC.Text = valorCxC.ToString("C");
                        TextCxCAnt.Text = valorCxCAnt.ToString("C");
                        TextCxCAbono.Text = (valorCxC - saldoCxC).ToString("C");
                        TextCxCAntAbono.Text = (valorCxCAnt - saldoCxCAnt).ToString("C");
                        TextCxCSaldo.Text = saldoCxC.ToString("C");
                        TextCxCAntSaldo.Text = saldoCxCAnt.ToString("C");
                        TotalCxc.Text = (valorCxC - valorCxCAnt - valorCxP + valorCxPAnt).ToString("C");
                        TotalAbono.Text = ((valorCxC - saldoCxC) - (valorCxCAnt - saldoCxCAnt)).ToString("C");
                        TotalSaldo.Text = (saldoCxC - saldoCxCAnt - saldoCxP + saldoCxPAnt).ToString("C");


                        System.Data.DataTable AgruCuentas = new System.Data.DataTable();
                        System.Data.DataTable dtCxc = ((DataSet)slowTask.Result).Tables[0];

                        if (dtCxc.Rows.Count > 0)
                        {
                            AgruCuentas = dtCxc.AsEnumerable()
                                .GroupBy(a => a["cod_cta"].ToString().Trim())
                                .Select(c =>
                                {
                                    var row = ((DataSet)slowTask.Result).Tables[0].NewRow();
                                    row["cod_cta"] = c.Key;
                                    row["saldo"] = c.Sum(a => a.Field<decimal>("saldo"));
                                    return row;
                                }).CopyToDataTable();
                        }

                        ChartCircle.ItemsSource = AgruCuentas;
                        //ppppp
                        System.Data.DataTable AgruVendedor = new System.Data.DataTable();

                        if (dtCxc.Rows.Count > 0)
                        {
                            AgruVendedor = dtCxc.AsEnumerable()
                                .GroupBy(a => a["cod_cta"].ToString().Trim())
                                .Select(c =>
                                {
                                    var row = ((DataSet)slowTask.Result).Tables[0].NewRow();
                                    row["cod_ven"] = c.Key;
                                    row["saldo"] = c.Sum(a => a.Field<decimal>("saldo"));
                                    return row;
                                }).CopyToDataTable();
                        }

                        chartVende.ItemsSource = AgruVendedor;


                        double ven01 = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(ven01)", ""));
                        double ven02 = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(ven02)", ""));
                        double ven03 = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(ven03)", ""));
                        double ven04 = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(ven04)", ""));
                        double ven05 = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(ven05)", ""));

                        DataTable dtAltura = new DataTable();
                        dtAltura.Columns.Add("altura");
                        dtAltura.Columns.Add("valor", typeof(decimal));
                        dtAltura.Rows.Add("[1-30]", ven01);
                        dtAltura.Rows.Add("[31-60]", ven02);
                        dtAltura.Rows.Add("[61-90]", ven03);
                        dtAltura.Rows.Add("[91-120]", ven04);
                        dtAltura.Rows.Add("[+121]", ven01);

                        ChartCircleAltura.ItemsSource = dtAltura;



                    }
                    else
                    {
                        //DataTable dt = ((DataSet)slowTask.Result).Tables[0];
                        //SiaWin.Browse(dt);
                        DtCarteraD = ((DataSet)slowTask.Result).Tables["D"];
                        dataGridCxCD.ItemsSource = ((DataSet)slowTask.Result).Tables["D"];
                        double valorCxC, valorCxCAnt = 0;
                        double valorCxP = 0;
                        double valorCxPAnt = 0;
                        double saldoCxC = 0;
                        double saldoCxCAnt = 0;
                        double saldoCxP = 0;
                        double saldoCxPAnt = 0;
                        double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(valor)", "tip_apli=3").ToString(), out valorCxC);
                        double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(valor)", "tip_apli=4").ToString(), out valorCxCAnt);
                        double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(valor)", "tip_apli=1").ToString(), out valorCxP);
                        double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(valor)", "tip_apli=2").ToString(), out valorCxPAnt);
                        double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(saldo)", "tip_apli=3").ToString(), out saldoCxC);
                        double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(saldo)", "tip_apli=4").ToString(), out saldoCxCAnt);
                        double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(saldo)", "tip_apli=1").ToString(), out saldoCxP);
                        double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(saldo)", "tip_apli=2").ToString(), out saldoCxPAnt);
                        TextCxC.Text = valorCxC.ToString("C");
                        TextCxCAnt.Text = valorCxCAnt.ToString("C");
                        TextCxCAbono.Text = (valorCxC - saldoCxC).ToString("C");
                        TextCxCAntAbono.Text = (valorCxCAnt - saldoCxCAnt).ToString("C");
                        TextCxCSaldo.Text = saldoCxC.ToString("C");
                        TextCxCAntSaldo.Text = saldoCxCAnt.ToString("C");
                        TotalCxc.Text = (valorCxC - valorCxCAnt - valorCxP + valorCxPAnt).ToString("C");
                        TotalAbono.Text = ((valorCxC - saldoCxC) - (valorCxCAnt - saldoCxCAnt)).ToString("C");
                        TotalSaldo.Text = (saldoCxC - saldoCxCAnt - saldoCxP + saldoCxPAnt).ToString("C");
                    }

                }
                else
                {
                    //TextTotalDoc.Text = "0";
                    //TextSaldo.Text = "0";
                }
                this.sfBusyIndicator.IsBusy = false;
                SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idemp, 0, 0, 0, "Consulto Cartera cuentas:" + Cta + " Fecha:" + ffi.ToString() + " - " + tabitem.Title, "");

                //this.IsEnabled = true;
                //   dataGrid.Focus();
            }
            catch (Exception ex)
            {
                SiaWin.seguridad.ErrorLog("Error  ", "AnalisisDeCartera-ButtonRefresh:" + ex.Message.ToString());
                MessageBox.Show("Error :" + ex.Message, "Error SiasoftApp");
                tabitem.Progreso(false);
                BtnEjecutar.IsEnabled = true;
                Imprimir.IsEnabled = true;
                ExportarXls.IsEnabled = true;
                ConciliarCxcCo.IsEnabled = true;
                sfBusyIndicator.IsBusy = false;
                tabitem.Progreso(false);
                resetTotales();
                this.Opacity = 1;
            }
        }
        //private DataSet SlowDude(string procedure,string ffi, string ctas, string cter, string cco, string where, string ven, bool detalle, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        DataSet jj = LoadData(procedure, ffi, ctas, cter, cco, where, ven, detalle, cancellationToken);
        //        return jj;

        //    }
        //    catch (Exception e)
        //    {
        //        SiaWin.seguridad.ErrorLog("Error  ", "AnalisisDeCartera-SlowDude:" + e.Message.ToString());
        //        MessageBox.Show(e.Message);
        //    }
        //    return null;
        //}

        private DataSet LoadData(string Fi, string ctas, string cter, string cco, string where, int exclinteremp, string ven, bool detalle, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds1 = new DataSet();
                cmd = new SqlCommand("_empSpCoAnalisisCxc", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Ter", cter);//if you have parameters.
                cmd.Parameters.AddWithValue("@Cta", ctas);//if you have parameters.
                cmd.Parameters.AddWithValue("@TipoApli", 1);//if you have parameters.
                //cmd.Parameters.AddWithValue("@Resumen", 0);//if you have parameters.
                cmd.Parameters.AddWithValue("@Resumen", detalle == true ? 1 : 0);//if you have parameters.
                cmd.Parameters.AddWithValue("@Fecha", Fi);//if you have parameters.
                cmd.Parameters.AddWithValue("@TrnCo", "");//if you have parameters.
                cmd.Parameters.AddWithValue("@NumCo", "");//if you have parameters.
                cmd.Parameters.AddWithValue("@Cco", cco);//if you have parameters.
                cmd.Parameters.AddWithValue("@Ven", ven);//if you have parameters.
                cmd.Parameters.AddWithValue("@codemp", codemp);//if you have parameters.
                cmd.Parameters.AddWithValue("@ExcluirInterEmpresa", exclinteremp);
                //cmd.Parameters.AddWithValue("@Where", where);//if you have parameters.
                //if(ven!="")     
                //ds.Tables[0].Select("cod_ven='"+)
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                //if (cco != "") da.Fill(ds.Tables[0].Select("cod_ven='AFR'").CopyToDataTable());
                //if (cco=="") da.Fill(ds);
                //if (cco != "") da.Fill(ds.Tables[0].Select("cod_ven='"+cco+"'").CopyToDataTable());
                string dataName = "C";
                if (detalle == true) dataName = "D";
                da.Fill(ds, dataName);
                con.Close();
                return ds;
                //VentasPorProducto.ItemsSource = ds.Tables[0];
                //VentaPorBodega.ItemsSource = ds.Tables[1];
                //VentasPorCliente.ItemsSource = ds.Tables[2];
            }
            catch (Exception e)
            {
                SiaWin.seguridad.ErrorLog("Error  ", "AnalisisDeCartera-LoadData:" + e.Message.ToString());
                MessageBox.Show(e.Message);
                return null;
            }
        }

        private void BtnDetalle_Click(object sender, RoutedEventArgs e)
        {
            //if (comboBoxCuentas.SelectedIndex < 0)
            // {
            //   MessageBox.Show("Seleccione una cuenta...");
            // comboBoxCuentas.Focus();
            //comboBoxCuentas.IsDropDownOpen = true;
            //return;
            //}
            //            DataRowView drv = (DataRowView)comboBoxCuentas.SelectedItem;
            //            String valueOfItem = drv["cod_cta"].ToString();
            //            MessageBox.Show(valueOfItem);
            string Cta = "";
            if (comboBoxCuentas.SelectedIndex > 0)
            {
                foreach (DataRowView ob in comboBoxCuentas.SelectedItems)
                {
                    //dr["cod_ter"].ToString();
                    String valueCta = ob["cod_cta"].ToString();
                    Cta += valueCta + ",";
                    //MessageBox.Show(valueOfItem1.ToString());
                }
                string ss = Cta.Trim().Substring(Cta.Trim().Length - 1);
                if (ss == ",") Cta = Cta.Substring(0, Cta.Trim().Length - 1);
            }

            string Ven = "";
            if (comboBoxVendedor.SelectedIndex >= 0)
            {
                foreach (DataRowView ob in comboBoxVendedor.SelectedItems)
                {
                    String valueCta = ob["cod_mer"].ToString().Trim();
                    Ven += valueCta + ",";
                }
                string ss = Ven.Trim().Substring(Ven.Trim().Length - 1);
                if (ss == ",") Ven = Ven.Substring(0, Ven.Trim().Length - 1);
            }



            try
            {
                DataRowView row = dataGridCxC.Visibility == Visibility.Visible ?
                (DataRowView)dataGridCxC.SelectedItems[0] : (DataRowView)dataGridCxCD.SelectedItems[0];
                if (row == null)
                {
                    MessageBox.Show("Registro sin datos");
                    return;
                }
                string cod_cli = row[0].ToString();
                string cod_cta = row[2].ToString();
                //                var dr1 = dataGridCxC.SelectedItems;

                //                    string cod_cli = dr["cod_ter"].ToString();
                //                  if (string.IsNullOrEmpty(cod_cli)) return;
                //                string cod_cta = dr["cod_cta"].ToString();
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds1 = new DataSet();

                //string Vendedor = comboBoxVendedor.Text.Trim();
                string Tercero = TextCod_Ter.Text.Trim();
                //cmd = new SqlCommand("ConsultaCxcCxpDeta", con);
                cmd = new SqlCommand("_empSpCoAnalisisCxc", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Ter", cod_cli);//if you have parameters.
                cmd.Parameters.AddWithValue("@Cta", Cta);//if you have parameters.
                cmd.Parameters.AddWithValue("@TipoApli", 1);//if you have parameters.
                cmd.Parameters.AddWithValue("@Resumen", 1);//if you have parameters.
                cmd.Parameters.AddWithValue("@Fecha", FechaIni.Text);//if you have parameters.
                cmd.Parameters.AddWithValue("@TrnCo", "");//if you have parameters.
                cmd.Parameters.AddWithValue("@NumCo", "");//if you have parameters.
                cmd.Parameters.AddWithValue("@Cco", "");//if you have parameters.
                cmd.Parameters.AddWithValue("@Ven", Ven);//if you have parameters.
                cmd.Parameters.AddWithValue("codemp", codemp);
                //cmd.Parameters.AddWithValue("@Cco", TextCod_bod.Text.Trim());//if you have parameters.
                //cmd.Parameters.AddWithValue("@Where", where);//if you have parameters.
                da = new SqlDataAdapter(cmd);
                da.Fill(ds1);
                con.Close();
                if (ds1.Tables[0].Rows.Count == 0)
                {
                    MessageBox.Show("Sin informacion de cartera");
                    return;
                }
                AnalisisDeCarteraDetalle WinDetalle = new AnalisisDeCarteraDetalle();
                WinDetalle.TextCodigo.Text = cod_cli;
                WinDetalle.TextNombre.Text = row["nom_ter"].ToString();
                WinDetalle.TextCuenta.Text = Cta;
                WinDetalle.codemp = codemp;
                WinDetalle.fechacorte = FechaIni.Text;
                WinDetalle.Title = "Detalle de cartera - Fecha De Corte:" + FechaIni.Text.ToString();
                WinDetalle.dataGridCxC.ItemsSource = ds1.Tables[0];
                // TOTALIZA 

                double valorCxC, valorCxCAnt = 0;
                //double valorCxCAnt = 0;
                double valorCxP = 0;
                double valorCxPAnt = 0;
                double saldoCxC = 0;
                double saldoCxCAnt = 0;
                double saldoCxP = 0;
                double saldoCxPAnt = 0;
                double.TryParse(ds1.Tables[0].Compute("Sum(valor)", "tip_apli=3").ToString(), out valorCxC);
                double.TryParse(ds1.Tables[0].Compute("Sum(valor)", "tip_apli=4").ToString(), out valorCxCAnt);
                double.TryParse(ds1.Tables[0].Compute("Sum(valor)", "tip_apli=1").ToString(), out valorCxP);
                double.TryParse(ds1.Tables[0].Compute("Sum(valor)", "tip_apli=2").ToString(), out valorCxPAnt);
                double.TryParse(ds1.Tables[0].Compute("Sum(saldo)", "tip_apli=3").ToString(), out saldoCxC);
                double.TryParse(ds1.Tables[0].Compute("Sum(saldo)", "tip_apli=4").ToString(), out saldoCxCAnt);
                double.TryParse(ds1.Tables[0].Compute("Sum(saldo)", "tip_apli=1").ToString(), out saldoCxP);
                double.TryParse(ds1.Tables[0].Compute("Sum(saldo)", "tip_apli=2").ToString(), out saldoCxPAnt);
                WinDetalle.TextCxC.Text = valorCxC.ToString("C");
                WinDetalle.TextCxCAnt.Text = valorCxCAnt.ToString("C");
                WinDetalle.TextCxCAbono.Text = (valorCxC - saldoCxC).ToString("C");
                WinDetalle.TextCxCAntAbono.Text = (valorCxCAnt - saldoCxCAnt).ToString("C");
                WinDetalle.TextCxCSaldo.Text = saldoCxC.ToString("C");
                WinDetalle.TextCxCAntSaldo.Text = saldoCxCAnt.ToString("C");
                WinDetalle.TotalCxc.Text = (valorCxC - valorCxCAnt - valorCxP + valorCxPAnt).ToString("C");
                WinDetalle.TotalAbono.Text = ((valorCxC - saldoCxC) - (valorCxCAnt - saldoCxCAnt)).ToString("C");
                WinDetalle.TotalSaldo.Text = (saldoCxC - saldoCxCAnt - saldoCxP + saldoCxPAnt).ToString("C");


                WinDetalle.ShowInTaskbar = false;
                WinDetalle.Owner = Application.Current.MainWindow;
                WinDetalle.WindowStartupLocation = WindowStartupLocation.CenterScreen;



                //WinDetalle.dataGridCxC_FilterChanged1();
                WinDetalle.ShowDialog();

                WinDetalle = null;
                //ImprimirDoc(Convert.ToInt32(numtrn), "Reimpreso");

            }
            catch (Exception ex)
            {
                SiaWin.seguridad.ErrorLog("Error  ", "AnalisisDeCartera-BtnDetalle:" + ex.Message.ToString());
                MessageBox.Show(ex.Message.ToString());

            }
        }
        private void ExportarXls_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;

                var excelEngine = dataGridCxCD.ExportToExcel(dataGridCxCD.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];
                //workBook.Worksheets[0].AutoFilters.FilterRange = workBook.Worksheets[0].UsedRange;

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
            catch (Exception ex)
            {
                SiaWin.seguridad.ErrorLog("Error  ", "AnalisisDeCartera-ExportarXLS:" + ex.Message.ToString());
                MessageBox.Show(ex.Message);
            }
        }
        private static void CellExportingHandler(object sender, GridCellExcelExportingEventArgs e)
        {
            e.Range.CellStyle.Font.Size = 12;
            e.Range.CellStyle.Font.FontName = "Segoe UI";
            if (e.ColumnName == "valor" || e.ColumnName == "sinvenc" || e.ColumnName == "ven01" || e.ColumnName == "ven02" || e.ColumnName == "ven03" || e.ColumnName == "ven04" || e.ColumnName == "ven05" || e.ColumnName == "saldo")
            {
                double value = 0;
                if (double.TryParse(e.CellValue.ToString(), out value))
                {
                    e.Range.Number = value;
                }
                e.Handled = true;
            }
        }


        private void comboBoxCuentas_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            dataGridCxC.ClearFilters();
            dataGridCxC.ItemsSource = null;
            dataGridCxCD.ClearFilters();
            dataGridCxCD.ItemsSource = null;
            resetTotales();

        }
        private void resetTotales()
        {
            TextCxC.Text = "0.00";
            TextCxCAnt.Text = "0.00";
            TextCxCAbono.Text = "0.00";
            TextCxCAntAbono.Text = "0.00";
            TextCxCSaldo.Text = "0.00";
            TextCxCAntSaldo.Text = "0.00";
            TotalCxc.Text = "0.00";
            TotalAbono.Text = "0.00";
            TotalSaldo.Text = "0.00";
        }


        private void dataGridCxC_FilterChanged(object sender, GridFilterEventArgs e)
        {
            //MessageBox.Show("1");
            // MessageBox.Show("filter:"+( sender as SfDataGrid).View.Records.Count.ToString());
            //            var columnName = e.Column.MappingName;
            //          var filteredResult =(sender as SfDataGrid).View.Records.Select(recordentry => recordentry.Data);
            //        var recordEntry = (sender as SfDataGrid).View.Records;
            var provider = (sender as SfDataGrid).View.GetPropertyAccessProvider();
            var records = (sender as SfDataGrid).View.Records;
            //Gets the value for frozen rows count of corresponding column and removes it from FilterElement collection.
            double valorCxC = 0;
            double valorCxCAnt = 0;
            double valorCxP = 0;
            double valorCxPAnt = 0;
            double saldoCxC = 0;
            double saldoCxCAnt = 0;
            double saldoCxP = 0;
            double saldoCxPAnt = 0;

            for (int i = 0; i < (sender as SfDataGrid).View.Records.Count; i++)
            {
                int tipapli = Convert.ToInt32(provider.GetValue(records[i].Data, "tip_apli").ToString());
                if (tipapli == 3)
                {
                    valorCxC += Convert.ToDouble(provider.GetValue(records[i].Data, "valor").ToString());
                    saldoCxC += Convert.ToDouble(provider.GetValue(records[i].Data, "saldo").ToString());
                    //                    valordoc += Convert.ToDouble(provider.GetValue(records[i].Data, "valor").ToString());
                    //                    saldodoc += Convert.ToDouble(provider.GetValue(records[i].Data, "saldo").ToString());
                }
                if (tipapli == 4)
                {
                    valorCxCAnt += Convert.ToDouble(provider.GetValue(records[i].Data, "valor").ToString());
                    saldoCxCAnt += Convert.ToDouble(provider.GetValue(records[i].Data, "saldo").ToString());
                    //                    valordoc += Convert.ToDouble(provider.GetValue(records[i].Data, "valor").ToString());
                    //                    saldodoc += Convert.ToDouble(provider.GetValue(records[i].Data, "saldo").ToString());
                }

            }
            //double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(valor)", "tip_apli=3").ToString(), out valorCxC);
            //double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(valor)", "tip_apli=4").ToString(), out valorCxCAnt);
            //double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(valor)", "tip_apli=1").ToString(), out valorCxP);
            //double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(valor)", "tip_apli=2").ToString(), out valorCxPAnt);
            //double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(saldo)", "tip_apli=3").ToString(), out saldoCxC);
            //double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(saldo)", "tip_apli=4").ToString(), out saldoCxCAnt);
            //double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(saldo)", "tip_apli=4").ToString(), out saldoCxCAnt);
            //double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(saldo)", "tip_apli=1").ToString(), out saldoCxP);
            //double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(saldo)", "tip_apli=2").ToString(), out saldoCxPAnt);


            //double valorA = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(valor)", "tip_apli=1 or tip_apli=4").ToString());
            //double saldo = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(saldo)", "tip_apli=2 or tip_apli=3").ToString());
            TextCxC.Text = valorCxC.ToString("C");
            TextCxCAnt.Text = valorCxCAnt.ToString("C");
            TextCxCAbono.Text = (valorCxC - saldoCxC).ToString("C");
            TextCxCAntAbono.Text = (valorCxCAnt - saldoCxCAnt).ToString("C");
            TextCxCSaldo.Text = saldoCxC.ToString("C");
            TextCxCAntSaldo.Text = saldoCxCAnt.ToString("C");
            TotalCxc.Text = (valorCxC - valorCxCAnt - valorCxP + valorCxPAnt).ToString("C");
            TotalAbono.Text = ((valorCxC - saldoCxC) - (valorCxCAnt - saldoCxCAnt)).ToString("C");
            TotalSaldo.Text = (saldoCxC - saldoCxCAnt - saldoCxP + saldoCxPAnt).ToString("C");



            //TextTotalDoc.Text = (valordoc-valordocA).ToString("C");
            //TextSaldo.Text = (saldodoc-saldodocA).ToString("C");
        }

        private void BtnRCaja_Click(object sender, RoutedEventArgs e)
        {
            SiaWin.ValReturn = null;
            DataRowView row = dataGridCxC.Visibility == Visibility.Visible ?
                (DataRowView)dataGridCxC.SelectedItems[0] : (DataRowView)dataGridCxCD.SelectedItems[0];

            if (row == null)
            {
                MessageBox.Show("Registro sin datos");
                return;
            }
            string cod_cli = row["cod_ter"].ToString();
            string cod_cta = row["cod_cta"].ToString();
            if (string.IsNullOrEmpty(cod_cli)) return;
            //MessageBox.Show(cod_cli + "-" + cod_cta);

            SiaWin.ValReturn = cod_cli;
            //Window ww = SiaWin.WindowExt(9299, "RecibosDeCaja");  //carga desde sql

            //ww.ShowInTaskbar = false;
            //ww.Owner = Application.Current.MainWindow;
            //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //ww.ShowDialog();
            //ww = null;
            dynamic ww = SiaWin.WindowExt(9305, "RecibosDeCaja");  //carga desde sql
            ww.ShowInTaskbar = false;
            ww.Owner = Application.Current.MainWindow;
            ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ww.idemp = idemp;
            ww.fechaPublic = FechaIni.Text;
            ww.codpvta = codpvta;
            ww.codter = cod_cli;
            ww.Show();
            ww = null;
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            tabitem.Cerrar(0);
        }
        private void TextCod_Ter_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TextCod_Ter.Text.Trim() == "") TextNombreTercero.Text = "";
        }

        public Boolean IsNumber(String s)
        {
            Boolean value = true;
            foreach (Char c in s.ToCharArray())
            {
                value = value && Char.IsDigit(c);
            }

            return value;
        }
        private void Imprimir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool mm = IsNumber(TxtAltura.Text.Trim());
                if (!mm)
                {
                    MessageBox.Show("Valor de altura tiene que ser un numero valido ");
                    TxtAltura.Text = "0";
                    return;
                }
                if (comboBoxCuentas.SelectedIndex < 0)
                {
                    MessageBox.Show("Seleccione una cuenta");
                    comboBoxCuentas.Focus();
                    return;
                }
                if (CmbTipoDoc.SelectedIndex < 0)
                {
                    MessageBox.Show("Seleccione un reporte..");
                    CmbTipoDoc.Focus();
                    return;
                }
                string Cta = "";
                if (comboBoxCuentas.SelectedIndex >= 0)
                {
                    foreach (DataRowView ob in comboBoxCuentas.SelectedItems)
                    {
                        //dr["cod_ter"].ToString();
                        String valueCta = ob["cod_cta"].ToString().Trim();
                        Cta += valueCta + ",";
                        //MessageBox.Show(valueOfItem1.ToString());
                    }
                    string ss = Cta.Trim().Substring(Cta.Trim().Length - 1);
                    if (ss == ",") Cta = Cta.Substring(0, Cta.Trim().Length - 1);
                }
                if (Cta == "") return;
                string Ven = "";
                if (comboBoxVendedor.SelectedIndex >= 0)
                {
                    foreach (DataRowView ob in comboBoxVendedor.SelectedItems)
                    {
                        String valueCta = ob["cod_mer"].ToString();
                        Ven += valueCta + ",";
                    }
                    string ss = Ven.Trim().Substring(Ven.Trim().Length - 1);
                    if (ss == ",") Ven = Ven.Substring(0, Ven.Trim().Length - 1);
                }
                else
                {
                    if (CmbTipoDoc.SelectedIndex == 2)
                    {
                        MessageBox.Show("El Reporte seleccionado requere 1 vendedor..", "Mensaje SIA");
                        comboBoxVendedor.Focus();
                        return;
                    }
                }
                //MessageBox.Show(Cta);
                List<ReportParameter> parameters = new List<ReportParameter>();
                ReportParameter paramcodemp = new ReportParameter();
                paramcodemp.Values.Add(codemp);
                paramcodemp.Name = "codemp";
                parameters.Add(paramcodemp);
                ReportParameter paramfechaini = new ReportParameter();
                //MessageBox.Show("FECHA PASABLE:"+ FechaIni.Text);0
                //MessageBox.Show("FECHA COMO STRING:" + FechaIni.ToString());

                paramfechaini.Values.Add(FechaIni.SelectedDate.Value.ToShortDateString());
                //paramfechaini.Values.Add("08-15-2019");

                //string xx = DateTime.ParseExact(FechaIni.SelectedDate.Value.ToString(), "MM/dd/yyyy", CultureInfo.InvariantCulture).ToShortDateString();
                //MessageBox.Show(xx);
                //fecha_ini.SelectedDate.Value.ToShortDateString();
                paramfechaini.Values.Add(FechaIni.SelectedDate.Value.ToShortDateString());
                paramfechaini.Name = "Fecha";
                parameters.Add(paramfechaini);


                ReportParameter paramCtaIni = new ReportParameter();
                paramCtaIni.Name = "Cta";
                paramCtaIni.Values.Add(Cta);


                parameters.Add(paramCtaIni);

                ReportParameter paramTer = new ReportParameter();
                paramTer.Values.Add(TextCod_Ter.Text.Trim());
                paramTer.Name = "Ter";
                parameters.Add(paramTer);


                ReportParameter paramTrnCo = new ReportParameter();
                paramTrnCo.Values.Add("");
                paramTrnCo.Name = "TrnCo";
                parameters.Add(paramTrnCo);

                ReportParameter paramNumCo = new ReportParameter();
                paramNumCo.Values.Add("");
                paramNumCo.Name = "NumCo";
                parameters.Add(paramNumCo);

                ReportParameter paramCco = new ReportParameter();
                paramCco.Values.Add("");
                paramCco.Name = "Cco";
                parameters.Add(paramCco);

                ReportParameter paramVen = new ReportParameter();
                paramVen.Values.Add(Ven.Trim());
                paramVen.Name = "Ven";
                parameters.Add(paramVen);


                ReportParameter paramResumen = new ReportParameter();

                int baltercero = 0; //resumida 
                int tipoReporte = 0; //1= reporte por vendedor,ciudad
                if (CmbTipoDoc.SelectedIndex == 1) baltercero = 1; //detallada
                if (CmbTipoDoc.SelectedIndex == 2)
                {
                    baltercero = 1; //detallada
                    tipoReporte = 1;
                }
                if (CmbTipoDoc.SelectedIndex == 3)
                {
                    baltercero = 1; //detallada
                    tipoReporte = 2;
                }


                paramResumen.Values.Add(baltercero.ToString());
                paramResumen.Name = "Resumen";
                parameters.Add(paramResumen);

                ReportParameter paramTipApli = new ReportParameter();
                paramTipApli.Values.Add("1");
                paramTipApli.Name = "TipoApli";
                parameters.Add(paramTipApli);

                if (tipoReporte > 0)
                {
                    ReportParameter paramtipoReporte = new ReportParameter();
                    paramtipoReporte.Values.Add(tipoReporte.ToString());
                    paramtipoReporte.Name = "TipoReporte";
                    parameters.Add(paramtipoReporte);
                }
                if (CmbTipoDoc.SelectedIndex == 1)
                {
                    ReportParameter paramAltura = new ReportParameter();
                    paramAltura.Values.Add(TxtAltura.Text.Trim());
                    paramAltura.Name = "Altura";
                    parameters.Add(paramAltura);
                }

                string TipoReporte = @"/CuentasPorCobrar/CuentasPorCobrarResumida";
                if (CmbTipoDoc.SelectedIndex == 1) TipoReporte = @"/CuentasPorCobrar/CuentasPorCobrarDetalladas";
                if (CmbTipoDoc.SelectedIndex == 2) TipoReporte = @"/CuentasPorCobrar/CuentasPorCobrarDetalladasVendedor";
                if (CmbTipoDoc.SelectedIndex == 3) TipoReporte = @"/CuentasPorCobrar/CuentasPorCobrarResumenAlturaPorVendedor";

                SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idemp, 0, 0, 0, "Consulto Cartera - Imprimio :" + Cta + " Fecha:" + paramfechaini + " - " + tabitem.Title + " Reporte:" + TipoReporte, "");
                string TituloReport = "Cuentas por Cobrar Resumida -";
                if (CmbTipoDoc.SelectedIndex == 1) TituloReport = "Cuentas por Cobrar Detallada -";
                if (CmbTipoDoc.SelectedIndex == 2) TituloReport = "Cuentas por Cobrar Detallada - Vendedor";
                if (CmbTipoDoc.SelectedIndex == 3) TituloReport = "Cuentas por Cobrar Altura - Vendedor";

                //public Reportes(List<ReportParameter> parameters, string reporteNombre, string TituloReporte = "", bool DirecPrinter = false, int Copias = 1, string PrintName = "", int ZoomPercent = 0, int idemp = -1)
                SiaWin.Reportes(parameters, TipoReporte, TituloReporte: TituloReport, Modal: true, idemp: idemp);
                //-ReportCxC rp = new ReportCxC(parameters, TipoReporte);
                //parameters, @"/Contabilidad/Balances/BalanceGeneral"
                //-rp.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                //-rp.Owner = SiaWin;
                //-rp.Show();
                //-rp = null;

            }
            catch (Exception ex)
            {
                MessageBox.Show("error en los parametros:" + ex);
            }

        }
        private DataTable LoadData(string _Fi, string _Ff, string _C1, string _C2, string _N1, string _N2, string _tip, int _TipoBalNiif)
        {
            try
            {
                //MessageBox.Show(_C1 + "/" + _C2);
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpSpCoBalance", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fechaini", _Fi);
                cmd.Parameters.AddWithValue("@fechafin", _Ff);
                cmd.Parameters.AddWithValue("@ctaini", _C1);
                cmd.Parameters.AddWithValue("@ctafin", _C2);
                cmd.Parameters.AddWithValue("@ctanivini", _N1);
                cmd.Parameters.AddWithValue("@ctanivfin", _N2);
                cmd.Parameters.AddWithValue("@tipobalance", _tip);
                cmd.Parameters.AddWithValue("@balanceniif", _TipoBalNiif);
                cmd.Parameters.AddWithValue("@codEmp", codemp);
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                con.Close();
                //MessageBox.Show(ds.Tables[0].Rows.Count.ToString());
                return ds.Tables[0];

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Loaddata");
                return null;
            }
        }

        private async void ConciliarCxcCo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Cbx_Detalle.SelectedIndex < 0)
                {
                    MessageBox.Show("Seleccione Tipo de reporte detalle =No");
                    Cbx_Detalle.Focus();
                    return;
                }

                var tag = ((ComboBoxItem)Cbx_Detalle.SelectedItem).Tag.ToString();

                if (tag == "Si")
                {
                    MessageBox.Show("Seleccione Tipo de reporte detalle =No");
                    Cbx_Detalle.Focus();
                    return;
                }

                if (dataGridCxC.SelectedIndex < 0) return;

                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;

                dataGridCxC.Opacity = 0.5;
                sfBusyIndicator.IsBusy = true;


                string fec_ini = "01/01/" + FechaIni.SelectedDate.Value.Year.ToString();
                string fec_Corte = FechaIni.Text.ToString();
                string cuentas = CountSelected();
                SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idemp, 0, 0, 0, "Consulto Cartera - Conciliar Cuentas CxC:" + Cuentas + " Fecha:" + fec_ini.ToString() + "/" + fec_Corte.ToString() + " - " + tabitem.Title, "");
                //MessageBox.Show(cuentas+fec_ini.ToString()+" fecha-"+fec_Corte.ToString());
                var slowTask = Task<DataTable>.Factory.StartNew(() => conciliar(fec_ini, fec_Corte, cuentas, source.Token), source.Token);
                await slowTask;

                if (((DataTable)slowTask.Result).Rows.Count > 0)
                {
                    dataGridCxC.Opacity = 1;
                    sfBusyIndicator.IsBusy = false;
                    BrowMini w = new BrowMini();
                    w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    w.ShowInTaskbar = false;
                    w.Owner = Application.Current.MainWindow;
                    w.dt = ((DataTable)slowTask.Result);

                    w.ShowDialog();

                }
                else
                {
                    MessageBox.Show("No existen diferencias entre modulo contable y cxc");
                    dataGridCxC.Opacity = 1;
                    sfBusyIndicator.IsBusy = false;

                }

            }
            catch (Exception w)
            {
                MessageBox.Show("errro en el subproceso:" + w, "ConciliarCxcCo");
            }
        }

        public DataTable conciliar(string dateIni, string dataFec, string cuentas, CancellationToken cancellationToken)
        {
            try
            {

                DataTable DtConsiliado = new DataTable();
                DtConsiliado.Columns.Add("cuenta");
                DtConsiliado.Columns.Add("cod_ter");
                DtConsiliado.Columns.Add("nom_ter");
                DtConsiliado.Columns.Add("saldo_cartera");
                DtConsiliado.Columns.Add("saldo_contabilidad");
                //DtConsiliado.Rows.Add("510506","72181539" , 0,0);
                //DataTable DtSaldosCta = LoadData(dateIni, dataFec, CountSelected(), "", "1", "9", "1", 0);
                DataTable DtSaldosCta = LoadData(dateIni, dataFec, cuentas, "", "1", "9", "1", 0);
                DataTable DtCarteraTemp = DtCartera;
                //SiaWin.Browse(DtSaldosCta);
                foreach (System.Data.DataRow dr in DtSaldosCta.Rows)
                {
                    if (dr["tipo"].ToString().Trim().ToLower() == "t" && Convert.ToDecimal(dr["sal_fin"]) != 0)
                    {
                        System.Data.DataRow[] result = DtCarteraTemp.Select("cod_ter='" + dr["cod_ter"] + "' and cod_cta='" + dr["cod_cta"] + "' ");

                        if (result.Length > 0)
                        {
                            foreach (System.Data.DataRow row in result)
                            {
                                if (Convert.ToDecimal(row["saldo"]) != Convert.ToDecimal(dr["sal_fin"]))
                                {
                                    DtConsiliado.Rows.Add(row["cod_cta"].ToString(), row["cod_ter"].ToString(), row["nom_ter"].ToString(), row["saldo"].ToString(), dr["sal_fin"].ToString());
                                }
                                if (Convert.ToDecimal(row["saldo"]) < 0 || Convert.ToDecimal(dr["sal_fin"]) < 0)
                                {
                                    DtConsiliado.Rows.Add(row["cod_cta"].ToString(), row["cod_ter"].ToString(), row["nom_ter"].ToString(), row["saldo"].ToString(), dr["sal_fin"].ToString());
                                }

                            }
                        }
                        else
                        {
                            //agrego los que estan en contabilidad pero no en cartera 
                            if (Convert.ToDecimal(dr["sal_fin"]) != 0)
                            {
                                DtConsiliado.Rows.Add(dr["cod_cta"].ToString(), dr["cod_ter"].ToString(), dr["nom_ter"].ToString(), 0, dr["sal_fin"].ToString());
                            }
                        }
                    }
                }
                foreach (System.Data.DataRow dr in DtCartera.Rows)
                {
                    System.Data.DataRow[] result = DtSaldosCta.Select("cod_ter='" + dr["cod_ter"] + "' and cod_cta='" + dr["cod_cta"] + "' ");
                    if (result.Length > 0) { }
                    else
                    {
                        if (Convert.ToDecimal(dr["saldo"]) != 0)
                        {
                            DtConsiliado.Rows.Add(dr["cod_cta"].ToString(), dr["cod_ter"].ToString(), dr["nom_ter"].ToString(), dr["saldo"].ToString(), 0);
                        }
                    }
                }
                return DtConsiliado;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Conciliar");
                return null;
            }
        }
        public string CountSelected()
        {
            string Cta = "";
            if (comboBoxCuentas.SelectedIndex >= 0)
            {
                foreach (DataRowView ob in comboBoxCuentas.SelectedItems)
                {
                    String valueCta = ob["cod_cta"].ToString();
                    Cta += valueCta + ",";
                }
                string ss = Cta.Trim().Substring(Cta.Trim().Length - 1);
                if (ss == ",") Cta = Cta.Substring(0, Cta.Trim().Length - 1);
            }
            return Cta;
        }

        private void BtnEjecutarD_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                //string valor = Cbx_Detalle.Text;
                var tag = ((ComboBoxItem)Cbx_Detalle.SelectedItem).Tag.ToString();

                if (tag == "No")
                {
                    dataGridCxC.Visibility = Visibility.Visible;
                    dataGridCxCD.Visibility = Visibility.Hidden;
                    ConciliarCxcCo.IsEnabled = true;
                }
                else
                {
                    dataGridCxC.Visibility = Visibility.Hidden;
                    dataGridCxCD.Visibility = Visibility.Visible;
                    ConciliarCxcCo.IsEnabled = false;
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("ERROR:" + w);
            }
        }

        private void BtnDetalleD_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)dataGridCxCD.SelectedItems[0];
                if (row == null) return;
                int idreg = Convert.ToInt32(row["idreg"]);
                if (idreg <= 0) return;
                //public void TabTrn(int Pnt, int idemp, bool IntoWindows = false, int idregcab = 0, int idmodulo = 0, bool WinModal = true)
                SiaWin.TabTrn(0, idemp, true, idreg, 1, WinModal: true);
            }
            catch (Exception w)
            {
                System.Windows.MessageBox.Show("Error ...." + w.Message);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Cbx_Detalle.SelectedIndex = 1;

            if (SiaWin._UserTag1.Trim() != "")
            {
                //comboBoxVendedor.SelectedValue = SiaWin._UserTag1.Trim();
                //comboBoxVendedor.IsEnabled = false;
            }
        }


        private void BtnvrAbonado_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridCxCD.SelectedIndex >= 0)
                {
                    //AbonoDocumentos view = new AbonoDocumentos(idemp);
                    dynamic view = SiaWin.WindowExt(9659, "AbonoDocumentos");
                    //AbonoDocumentos view = new AbonoDocumentos(idemp);
                    DataRowView row = (DataRowView)dataGridCxCD.SelectedItems[0];
                    view.num_trn = row["num_trn"].ToString();
                    view.cod_ter = row["cod_ter"].ToString();
                    view.cod_cta = row["cod_cta"].ToString();
                    view.idemp = idemp;
                    view.ShowInTaskbar = false;
                    view.Owner = Application.Current.MainWindow;
                    view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    view.ShowDialog();
                }
                else
                {
                    MessageBox.Show("seleccione una factura");
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir el abono:" + w);
            }
        }

        private void BtnvrDesc_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (comboBoxVendedor.SelectedIndex<0)
                {
                    MessageBox.Show("seleccione un vendedor","alerta",MessageBoxButton.OK,MessageBoxImage.Exclamation);
                    return;
                }

                string direccion_formato = @"/CuentasPorCobrar/CxCdescuentos";


                List<ReportParameter> parameters = new List<ReportParameter>();

                ReportParameter paramcodemp = new ReportParameter();
                paramcodemp.Values.Add(codemp);
                paramcodemp.Name = "codemp";
                parameters.Add(paramcodemp);


                ReportParameter paramcodter = new ReportParameter();
                paramcodter.Values.Add(string.IsNullOrEmpty(TextCod_Ter.Text) ? "" : TextCod_Ter.Text);
                paramcodter.Name = "Ter";
                parameters.Add(paramcodter);

                ReportParameter paramcta = new ReportParameter();
                paramcta.Values.Add(CountSelected());
                paramcta.Name = "Cta";
                parameters.Add(paramcta);

                ReportParameter paramapli = new ReportParameter();
                paramapli.Values.Add("1");
                paramapli.Name = "TipoApli";
                parameters.Add(paramapli);

                ReportParameter paramares = new ReportParameter();
                paramares.Values.Add("1");
                paramares.Name = "Resumen";
                parameters.Add(paramares);


                ReportParameter paramafec = new ReportParameter();
                paramafec.Values.Add(FechaIni.Text);
                paramafec.Name = "Fecha";
                parameters.Add(paramafec);


                ReportParameter paramatrn = new ReportParameter();
                paramatrn.Values.Add("");
                paramatrn.Name = "TrnCo";
                parameters.Add(paramatrn);

                ReportParameter paramaNmt = new ReportParameter();
                paramaNmt.Values.Add("");
                paramaNmt.Name = "NumCo";
                parameters.Add(paramaNmt);

                ReportParameter paramacco = new ReportParameter();
                paramacco.Values.Add("");
                paramacco.Name = "Cco";
                parameters.Add(paramacco);


                string vendedor = string.IsNullOrEmpty(comboBoxVendedor.SelectedValue.ToString()) ? "" : getvend();


                ReportParameter paramaven = new ReportParameter();
                paramaven.Values.Add(vendedor);
                paramaven.Name = "Ven";
                parameters.Add(paramaven);

                ReportParameter paramatr = new ReportParameter();
                paramatr.Values.Add("0");
                paramatr.Name = "TipoReporte";
                parameters.Add(paramatr);


                ReportParameter paramexc = new ReportParameter();
                paramexc.Values.Add("0");
                paramexc.Name = "ExcluirInterEmpresa";
                parameters.Add(paramexc);


                string TituloReport = "titulo desde c#";

                SiaWin.Reportes(parameters, direccion_formato, TituloReporte: TituloReport, Modal: true, idemp: idemp, ZoomPercent: 50);

            }
            catch (Exception w)
            {
                MessageBox.Show("error en el docuemnto:" + w);
            }
        }


        public string getvend()
        {
            string Ven = "";
            if (comboBoxVendedor.SelectedIndex >= 0)
            {
                foreach (DataRowView ob in comboBoxVendedor.SelectedItems)
                {
                    String valueCta = ob["cod_mer"].ToString();
                    Ven += valueCta + ",";
                }
                if (Ven.Trim() != "")
                {
                    string ss = Ven.Trim().Substring(Ven.Trim().Length - 1);
                    if (ss == ",") Ven = Ven.Substring(0, Ven.Trim().Length - 1);
                }
            }

            return Ven;
        }







    }

}