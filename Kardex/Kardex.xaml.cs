using Microsoft.Reporting.WinForms;
using Microsoft.Win32;
//using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9466,"Kardex");       
    //dynamic WinDescto = ((Inicio)Application.Current.MainWindow).WindowExt(9466,"Kardex");
    //WinDescto.ShowInTaskbar = false;
    //WinDescto.Owner = Application.Current.MainWindow;
    //WinDescto.WindowStartupLocation = WindowStartupLocation.CenterScreen;
    //WinDescto.ShowDialog(); 

    public partial class Kardex : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        public string codemp;
        public string idBod = string.Empty;
        public string codpvta = string.Empty;
        public string codref = string.Empty;
        public string codbod = string.Empty;

        public DateTime fechacorte = DateTime.Now.Date;
        int moduloid = 0;
        string cnEmp = "";
        DataSet ds = null;
        public Kardex()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            //idemp = SiaWin._BusinessId;
            TextBoxRef.Focus();
            k_o.IsChecked = true;
            //MessageBox.Show(fechacorte.ToShortDateString());
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                string tag = ((TextBox)sender).Tag.ToString();

                if (tag == "inmae_ref")
                {
                    if (e.Key == System.Windows.Input.Key.Enter & string.IsNullOrEmpty(TextBoxRef.Text.Trim()))
                    {
                        dynamic ww = SiaWin.WindowExt(9326, "InBuscarReferencia");
                        ww.Conexion = SiaWin.Func.DatosEmp(idemp);
                        ww.idEmp = idemp;
                        ww.idBod = idBod;
                        ww.ShowInTaskbar = false;
                        ww.Height = 600;
                        ww.Owner = Application.Current.MainWindow;
                        ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        ww.ShowDialog();
                        if (!string.IsNullOrEmpty(ww.Codigo))
                        {
                            TextBoxRef.Text = ww.Codigo;
                            TXNomRef.Text = ww.Nombre;
                            TextBoxbod.Focus();
                        }
                        ww = null;
                        e.Handled = true;
                    }
                }
                if (tag == "inmae_bod")
                {
                    if (e.Key == System.Windows.Input.Key.Enter & string.IsNullOrEmpty(TextBoxbod.Text.Trim()))
                    {
                        string cmptabla = tag; string cmpcodigo = "cod_bod"; string cmpnombre = "nom_bod"; string cmporden = "cod_bod"; string cmpidrow = "idrow"; string cmptitulo = "Maestra de bodegas"; string cmpconexion = cnEmp; Boolean mostrartodo = true; string cmpwhere = "";
                        int idr = 0; string code = ""; string nom = "";
                        dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, SiaWin.Func.DatosEmp(idemp), mostrartodo, cmpwhere, idEmp: idemp);
                        winb.ShowInTaskbar = false;
                        //winb.idemp = idemp;
                        winb.Owner = Application.Current.MainWindow;
                        winb.ShowDialog();
                        idr = winb.IdRowReturn;
                        code = winb.Codigo;
                        nom = winb.Nombre;
                        winb = null;

                        if (idr > 0)
                        {
                            TextBoxbod.Text = code;
                            TxNomBod.Text = nom;

                            //var uiElement = e.OriginalSource as UIElement;
                            //uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                            BtnConsultar.Focus();
                        }
                        e.Handled = true;
                    }
                }
                if (e.Key == Key.Enter & !string.IsNullOrEmpty(((TextBox)sender).Text.Trim()))
                {
                    var uiElement = e.OriginalSource as UIElement;
                    uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }
            }
            catch (Exception ex)
            {
                SiaWin.seguridad.ErrorLog("Error  ", "Kardex-PreviewKeyDown-" + ex.Message.ToString());
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void TextBoxRef_LostFocus(object sender, RoutedEventArgs e)
        {
            BuscarCod(TextBoxRef.Text.Trim());
        }

        private void TextBoxbod_LostFocus(object sender, RoutedEventArgs e)
        {
            BuscarBod(TextBoxbod.Text.Trim());
        }

        private bool BuscarCod(string codigo)
        {
            if (string.IsNullOrEmpty(codigo)) return false;
            bool ret = false;
            try
            {
                string cadena = "select cod_ref,nom_ref from inmae_ref where cod_ref='" + codigo + "' ";
                DataTable dt = SiaWin.Func.SqlDT(cadena, "referencia", idemp);

                //MessageBox.Show("SiaWin._BusinessId:" + SiaWin._BusinessId);
                if (dt.Rows.Count > 0)
                {
                    //TextBoxRef.Text = dt.Rows[0]["cod_ref"].ToString();
                    TXNomRef.Text = dt.Rows[0]["nom_ref"].ToString();
                    ret = true;
                }
                else
                {
                    MessageBox.Show("no existe esa referencia");
                    clean(1);
                    ret = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("erro al buscar:" + ex.Message);
            }
            return ret;
        }

        private bool BuscarBod(string codigo)
        {
            if (string.IsNullOrEmpty(codigo)) return false;
            bool ret = false;
            try
            {
                string cadena = "select cod_bod,nom_bod from InMae_bod where cod_bod='" + codigo + "' ";

                DataTable dt = SiaWin.Func.SqlDT(cadena, "bodega", idemp);

                if (dt.Rows.Count > 0)
                {
                    TextBoxbod.Text = dt.Rows[0]["cod_bod"].ToString();
                    TxNomBod.Text = dt.Rows[0]["nom_bod"].ToString();
                    ret = true;
                }
                else
                {
                    MessageBox.Show("no existe la bodega que ingreso");
                    clean(2);
                    ret = false;
                }
            }
            catch (Exception ex)
            {
                SiaWin.seguridad.ErrorLog("Error  ", "Kardex-ExportarXLS-" + ex.Message.ToString());

                MessageBox.Show("erro al buscar:" + ex.Message);
            }
            return ret;
        }
        public void clean(int codigo)
        {
            if (codigo == 1)
            {
                TextBoxRef.Text = "";
                TXNomRef.Text = "";
            }
            if (codigo == 2)
            {
                TextBoxbod.Text = "";
                TxNomBod.Text = "";
            }
        }
        private void ResetValue()
        {
            TxtTotalUnEnt.Text = "00.0";
            TxtTotalUncosEnt.Text = "00.0";
            TxtTotalUncosSal.Text = "00.0";
            TxtTotalUncosSaldo.Text = "00.0";
            TxtTotalUncostEnt.Text = "00.0";
            TxtTotalUncostSal.Text = "00.0";
            TxtTotalUncostSaldo.Text = "00.0";
            TxtTotalUnEnt.Text = "00.0";
            TxtTotalUnSal.Text = "00.0";
            TxtTotalUnSaldo.Text = "00.0";
        }
        private void execute()
        {
            StringBuilder xx = new StringBuilder();
            xx.Append("begin transaction");
            xx.Append("declare @fecdoc as datetime;");
            xx.Append("set @fecdoc = getdate();declare @ini as char(4);declare @num as varchar(12); declare @iConsecutivo char(12) = ''; declare @iFolioHost int = 0;");
            xx.Append("UPDATE COpventas SET fac_contado = ISNULL(fac_contado, 0) + 1  WHERE cod_pvt = '003'; declare @nomcmp as char(12) = 'fac_contado';");
            xx.Append("SELECT @iFolioHost = fac_contado, @ini = CASE @nomcmp WHEN 'fac_contado' THEN inicial   WHEN 'fac_credito' THEN ini_cred  ELSE '003'   END FROM Copventas");
            xx.Append(" WHERE cod_pvt = '003'; set @num = @iFolioHost;");
            xx.Append(" select @iConsecutivo = rtrim(@ini) + '-' + rtrim(convert(varchar, @num));");
            xx.Append(" INSERT INTO incab_doc(cod_trn, fec_trn, cod_cli, suc_cli, cod_ven, num_trn, doc_ref, for_pag, idregcabref, dia_pla, fec_ven, trn_anu, num_anu, cod_dev, fa_cufe, cod_cco, est_imp, des_mov, autoriza, bod_tra, cod_trans, tip_ref) values('004', @fecdoc, '860054978', '', 'AFR', @iConsecutivo, @iConsecutivo, '99', 0, 90, DATEADD(day, 90, convert(date, @fecdoc)), '', '', '', '', '026', 1, '', '', '003', '1', ' ');");
            xx.Append(" DECLARE @NewID INT;");
            xx.Append(" SELECT @NewID = SCOPE_IDENTITY();");
            xx.Append(" INSERT INTO incue_doc(idregcab, cod_trn, num_trn, cod_ref, cod_bod, cantidad, val_uni, subtotal, por_des, val_des, por_iva, cod_tiva, val_iva, tot_tot, cod_sub, val_ica, val_ret, val_riva, por_ica, por_ret, por_riva) values(@NewID, '004', @iConsecutivo, '4515FXB', '003', 1.00, 121910.00, 121910.00, 29.00, 0.00, 19.00, 'C', 23163.00, 137205.00, '001', 1345.89, 3047.75, 3474.45, 1.104, 2.5, 15);");
            xx.Append(" insert into indet_fpag(idregcab, cod_pag, vlr_pagado, doc_ref, cod_cta, cod_trn, num_trn) values(@NewId, '01', 137205.00, '', '11050501', '004', @iConsecutivo); ;");
            xx.Append(" insert into pruebacue(tipo, cod_ref, cod_bod, cantidad) values(2, '4515IN', '001', .2);");
            xx.Append(" select CAST(@NewId AS int);");
            //xx.Append(" commit transaction");
            string printOutput = "";
            using (SqlConnection connection = new SqlConnection(SiaWin.Func.DatosEmp(idemp)))
            {
                try
                {
                    connection.Open();
                    connection.InfoMessage += (object obj, SqlInfoMessageEventArgs e) =>
                    {
                        printOutput += e.Message;
                    };
                    connection.FireInfoMessageEventOnUserErrors = true;
                    StringBuilder errorMessages = new StringBuilder();
                    SqlCommand command = connection.CreateCommand();
                    SqlTransaction transaction;
                    transaction = connection.BeginTransaction("Transaction");
                    command.Connection = connection;
                    command.Transaction = transaction;

                    command.CommandText = xx.ToString() + ";insert into pruebacue(tipo,cod_ref,cod_bod,cantidad) values(2,'4515IN','001',20);" + @"select CAST(@NewId AS int);commit transaction;";
                    MessageBox.Show(command.CommandText.ToString());
                    Clipboard.SetText(command.CommandText.ToString());
                    var r = new object();
                    r = command.ExecuteScalar();
                    transaction.Commit();
                    connection.Close();
                    MessageBox.Show("genero el documento de inventario");
                    MessageBox.Show(Convert.ToInt32(r.ToString()).ToString());
                }
                catch (SqlException exsql)
                {
                    MessageBox.Show(exsql.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public bool validarCheck()
        {
            bool flag = false;
            foreach (CheckBox check in GridCheck.Children)
            {
                if (check.IsChecked == true) flag = true;
            }

            if (flag == false)
            {
                MessageBox.Show("seleccione tipo de kardex oficial-NIIF");
            }
            return flag;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //execute();

            try
            {
                if (validarCheck() == false) return;

                ResetValue();
                if (FecIni.Text.Length <= 0)
                {
                    MessageBox.Show("debe de ingresar la fecha de corte");
                    FecIni.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(TextBoxRef.Text.Trim()))
                {
                    MessageBox.Show("debe de ingresar una referencia");
                    TextBoxRef.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(TextBoxbod.Text.Trim()))
                {
                    MessageBox.Show("debe de ingresar una bodega");
                    TextBoxbod.Focus();
                    return;
                }

                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                ds = new DataSet();
                
                cmd = new SqlCommand("_EmpSpInConsultaKardex", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@codemp", codemp);
                cmd.Parameters.AddWithValue("@fecha", FecIni.Text);
                cmd.Parameters.AddWithValue("@codref", TextBoxRef.Text);
                cmd.Parameters.AddWithValue("@codbod", TextBoxbod.Text);


                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                //SiaWin.Browse(ds.Tables[0], true);
                con.Close();
                GridKardex.ItemsSource = ds.Tables[0];
                if (ds.Tables[0].Rows.Count > 0)
                {
                    GridKardex.Focus();
                    GridKardex.SelectedIndex = 0;

                    if (k_o.IsChecked == true)
                    {
                        actualizarTotales(true);
                    }
                    else
                    {
                        actualizarTotales(false);
                    }


                    #region antrerior totales


                    //decimal CantEnt = Convert.ToDecimal(ds.Tables[0].Compute("Sum(ent_uni)", "").ToString());
                    //decimal TotEnt = Convert.ToDecimal(ds.Tables[0].Compute("Sum(entc_tot)", "").ToString());
                    //TxtTotalUnEnt.Text = CantEnt.ToString("N2");
                    //TxtTotalUncostEnt.Text = TotEnt.ToString("N2");
                    //int promedioEntrada = 0;
                    //if (TotEnt > 0 & CantEnt > 0)
                    //{
                    //    TxtTotalUncosEnt.Text = (TotEnt / CantEnt).ToString("N2");
                    //    promedioEntrada = Convert.ToInt32(TotEnt / CantEnt);
                    //}
                    //else
                    //{
                    //    TxtTotalUncosEnt.Text = "0";
                    //}

                    //ProEnt.Text = promedioEntrada.ToString();
                    //decimal CantSal = Convert.ToDecimal(ds.Tables[0].Compute("Sum(sal_uni)", "").ToString());
                    //decimal TotSal = Convert.ToDecimal(ds.Tables[0].Compute("Sum(salc_tot)", "").ToString());
                    //TxtTotalUnSal.Text = CantSal.ToString("N2");
                    //TxtTotalUncostSal.Text = TotSal.ToString("N2");
                    //int promedioSalida = 0;
                    //if (TotSal > 0 & CantSal > 0)
                    //{
                    //    TxtTotalUncosSal.Text = (TotSal / CantSal).ToString("N2");
                    //    promedioSalida = Convert.ToInt32(TotSal / CantSal);
                    //}
                    //else
                    //{
                    //    TxtTotalUncosSal.Text = "0";
                    //}
                    //ProSal.Text = promedioSalida.ToString();
                    //DataRow drow = ds.Tables[0].AsEnumerable().Last();
                    //decimal CantSaldo = Convert.ToDecimal(drow["saldo_uni"].ToString());
                    //decimal TotSaldo = Convert.ToDecimal(drow["saldo_ctotal"].ToString());
                    //TxtTotalUnSaldo.Text = CantSaldo.ToString("N2");
                    //TxtTotalUncostSaldo.Text = TotSaldo.ToString("N2");
                    //int promedioSaldo = 0;
                    //if (TotSaldo > 0 & CantSaldo > 0)
                    //{
                    //    promedioSaldo = Convert.ToInt32(TotSaldo / CantSaldo);
                    //    TxtTotalUncosSaldo.Text = (TotSaldo / CantSaldo).ToString("N2");
                    //}
                    //else
                    //{
                    //    TxtTotalUncosSaldo.Text = "0";
                    //}
                    //ProSaldo.Text = promedioSaldo.ToString();
                    //if (VerCostos() == false)
                    //{

                    //    foreach (DataRow dr in ds.Tables[0].Rows)
                    //    {
                    //        dr["entc_uni"] = 0;
                    //        dr["entc_tot"] = 0;
                    //        dr["salc_uni"] = 0;
                    //        dr["salc_tot"] = 0;
                    //        dr["saldo_costou"] = 0;
                    //        dr["saldo_ctotal"] = 0;
                    //        //                            TxtTotalUnSaldo.Text = "00.0";
                    //    }
                    //    TxtTotalUncosEnt.Text = "00.0";
                    //    TxtTotalUncosSal.Text = "00.0";
                    //    TxtTotalUncosSaldo.Text = "00.0";
                    //    TxtTotalUncostEnt.Text = "00.0";
                    //    TxtTotalUncostSal.Text = "00.0";
                    //    TxtTotalUncostSaldo.Text = "00.0";
                    //}
                    #endregion

                }
                Total.Text = ds.Tables[0].Rows.Count.ToString();
                SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idemp, 0, 0, 0, "Consulto en Kardex -Producto:" + TextBoxRef.Text + "-Bod" + TextBoxbod.Text + " Fecha:" + fechacorte.ToString() + this.Title, "");
            }
            catch (Exception w)
            {
                SiaWin.seguridad.ErrorLog("Error  ", "Kardex-BtnConultar-" + w.Message.ToString());
                MessageBox.Show("error al cargar la consulta programada:" + w.ToString());
            }
        }
        private void ExportaXLS_Click(object sender, RoutedEventArgs e)
        {
            if (ds == null) return;

            if (ds.Tables[0].Rows.Count <= 0)
            {
                MessageBox.Show("No hay registros para exportar..");
                return;
            }
            //SiaWin.Browse(ds.Tables[0]);
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExportMode = ExportMode.Value;
                //options.ExportMergedCells = false;
                options.ExportStackedHeaders = true;

                options.ExcelVersion = ExcelVersion.Excel2013;
                options.CellsExportingEventHandler = CellExportingHandler;
                var excelEngine = GridKardex.ExportToExcel(GridKardex.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];
                //workBook.ActiveSheet.Columns[4].NumberFormat = "0.0";
                //workBook.ActiveSheet.Columns[5].NumberFormat = "0.0";
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
                    if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archvo", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        //Launching the Excel file using the default Application.[MS Excel Or Free ExcelViewer]
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
                SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idemp, 0, 0, 0, "Exporto a XLS Kardex -Producto:" + TextBoxRef.Text + "-Bod" + TextBoxbod.Text + " Fecha:" + fechacorte.ToString() + this.Title, "");
            }
            catch (Exception EX)
            {
                SiaWin.seguridad.ErrorLog("Error  ", "Kardex-ExportarXLS-" + EX.Message.ToString());
                MessageBox.Show(EX.Message + EX.StackTrace.ToString());
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (idemp <= 0) idemp = SiaWin._BusinessId;


                //MessageBox.Show(idemp.ToString());

                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();

                if (string.IsNullOrEmpty(codemp))
                {
                    codemp = foundRow["BusinessCode"].ToString().Trim();

                }
                else
                {

                    DataTable dt = SiaWin.Func.SqlDT("select * from Business where BusinessCode='" + codemp + "' ", "Empresas", 0);
                    int idEmpresa = 0;
                    if (dt.Rows.Count > 0)
                    {
                        idEmpresa = Convert.ToInt32(dt.Rows[0]["BusinessId"]);
                    }
                    System.Data.DataRow foundRowEmpresa = SiaWin.Empresas.Rows.Find(idEmpresa);
                    nomempresa = foundRowEmpresa["BusinessName"].ToString().Trim();

                }
                this.Title = "Kardex - Empresa:" + codemp + "-" + nomempresa;
                if (fechacorte != DateTime.Now.Date) FecIni.Text = fechacorte.Date.ToShortDateString();
                if (fechacorte == DateTime.Now.Date) FecIni.Text = DateTime.Now.ToShortDateString();
                // idmodulo

                DataRow[] drmodulo = SiaWin.Modulos.Select("ModulesCode='IN'");
                if (drmodulo == null) this.IsEnabled = false;
                moduloid = Convert.ToInt32(drmodulo[0]["ModulesId"].ToString());
                //MessageBox.Show("Modulo id:"+moduloid.ToString());
                if (!string.IsNullOrEmpty(codref))
                {
                    TextBoxbod.Text = codbod;
                    BuscarBod(codbod);
                    TextBoxRef.Text = codref;
                    BuscarCod(codref);
                    BtnConsultar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    k_o.IsChecked = true;
                }


                // visible btnexportarxls,btnverdocumentos
                string llaveIN = idemp.ToString() + "-2";
                bool iverXls = SiaWin.Acc.ContainsKey(llaveIN + "-167");
                bool iverConsultaDoc = SiaWin.Acc.ContainsKey(llaveIN + "-168");
                if (iverXls == false) BtnExportarXLS.Visibility = Visibility.Collapsed;
                if (iverConsultaDoc == false) BtnDocumento.Visibility = Visibility.Collapsed;
                SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idemp, 0, 0, 0, "Ingreso a Pantalla: Consulta Kardex " + codemp + "-" + nomempresa, "");
            }
            catch (Exception w)
            {
                SiaWin.seguridad.ErrorLog("Error  ", "Kardex-Loaded-" + w.Message.ToString());
                MessageBox.Show("error al cargar el Load:" + w);
            }
        }
        private bool VerCostos()
        {

            string llaveIN = idemp.ToString() + "-2";
            bool iver = SiaWin.Acc.ContainsKey(llaveIN + "-144");
            return iver;
        }


        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Usted desea salir de la pantalla Kardex..?", "Salir de Kardex", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                e.Handled = true;
                return;
            }            
            this.Close();
        }



        private void BtnDocumento_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                DataRowView row = (DataRowView)GridKardex.SelectedItems[0];
                if (row == null) return;
                int idreg = Convert.ToInt32(row["idreg"]);
                if (idreg <= 0) return;
                //public void TabTrn(int Pnt, int idemp, bool IntoWindows = false, int idregcab = 0, int idmodulo = 0, bool WinModal = true)
                SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idemp, 0, 0, 0, "Consulta Documento idreg:" + idreg.ToString() + this.Title, "");
                SiaWin.TabTrn(0, idemp, true, idreg, moduloid, WinModal: true);
            }
            catch (Exception w)
            {
                SiaWin.seguridad.ErrorLog("Error  ", "Kardex-BtnDocumenot-" + w.Message.ToString());
                MessageBox.Show("selecione una transaccion" + w);
            }
        }

        private void BtnImprimir_Click(object sender, RoutedEventArgs e)
        {
            if (ds == null) return;

            if (ds.Tables[0].Rows.Count <= 0)
            {
                MessageBox.Show("No hay registros para exportar..");
                return;
            }
            try
            {


                List<ReportParameter> parameters = new List<ReportParameter>();
                ReportParameter paramcodemp = new ReportParameter();
                paramcodemp.Values.Add(codemp);
                paramcodemp.Name = "codemp";
                parameters.Add(paramcodemp);

                ReportParameter paramfechaini = new ReportParameter();
                paramfechaini.Values.Add(FecIni.SelectedDate.Value.ToShortDateString());
                //fecha_ini.SelectedDate.Value.ToShortDateString()
                paramfechaini.Name = "Fecha";
                parameters.Add(paramfechaini);

                ReportParameter paramRef = new ReportParameter();
                paramRef.Name = "Ref";
                paramRef.Values.Add(TextBoxRef.Text.Trim());


                parameters.Add(paramRef);

                ReportParameter paramBod = new ReportParameter();
                paramBod.Values.Add(TextBoxbod.Text.Trim());
                paramBod.Name = "Bods";
                parameters.Add(paramBod);


                string TipoReporte = @"/Inventarios/InKardexProductoBodegasUnidades";
                if (VerCostos() == true) TipoReporte = @"/Inventarios/InKardexProductoBodegas";
                SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idemp, 0, 0, 0, "Kardes - Imprimio :Referencia:" + TextBoxRef.Text.Trim() + " Bodega:" + TextBoxbod.Text.Trim() + " Fecha:" + FecIni.SelectedDate.Value.ToShortDateString() + " Reporte:" + TipoReporte, "");

                SiaWin.Reportes(parameters, TipoReporte, Modal: true);
                //ReportCxC rp = new ReportCxC(parameters, TipoReporte);
                //parameters, @"/Contabilidad/Balances/BalanceGeneral"
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {

                string name = (sender as CheckBox).Name;

                function(name, false);

            }
            catch (Exception w)
            {
                MessageBox.Show("kardex en mantenito se esta agregando funcionalidades niif:" + w);
            }
        }



        private void k_o_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = (sender as CheckBox).Name;
                function(name, true);
            }
            catch (Exception w)
            {
                MessageBox.Show("error en el chequeo:" + w);
            }
        }

        public void function(string name, bool flag)
        {
            if (flag == true)
            {
                if (name == "k_o")
                {
                    k_n.IsChecked = true;
                }

                if (name == "k_n")
                {
                    k_o.IsChecked = true;
                }
            }
            else
            {
                foreach (CheckBox c in GridCheck.Children)
                {
                    if (c.Name == name)
                    {
                        if (c.Name == "k_o")
                        {

                            ent_uni.IsHidden = false;
                            entc_uni.IsHidden = false;
                            entc_tot.IsHidden = false;
                            //salidas
                            sal_uni.IsHidden = false;
                            salc_uni.IsHidden = false;
                            salc_tot.IsHidden = false;
                            //saldo
                            saldo_uni.IsHidden = false;
                            saldo_costou.IsHidden = false;
                            saldo_ctotal.IsHidden = false;

                            //niif -------------------------------
                            //entradas
                            ent_unin.IsHidden = true;
                            entc_unin.IsHidden = true;
                            entc_totn.IsHidden = true;
                            //salidas
                            sal_unin.IsHidden = true;
                            salc_unin.IsHidden = true;
                            salc_totn.IsHidden = true;
                            //saldo
                            saldo_unin.IsHidden = true;
                            saldo_costoun.IsHidden = true;
                            saldo_ctotaln.IsHidden = true;

                            actualizarTotales(true);
                        }

                        if (c.Name == "k_n")
                        {
                            //entradas
                            ent_uni.IsHidden = true;
                            entc_uni.IsHidden = true;
                            entc_tot.IsHidden = true;
                            //salidas
                            sal_uni.IsHidden = true;
                            salc_uni.IsHidden = true;
                            salc_tot.IsHidden = true;
                            //saldo
                            saldo_uni.IsHidden = true;
                            saldo_costou.IsHidden = true;
                            saldo_ctotal.IsHidden = true;

                            //niif -------------------------------
                            //entradas
                            ent_unin.IsHidden = false;
                            entc_unin.IsHidden = false;
                            entc_totn.IsHidden = false;
                            //salidas
                            sal_unin.IsHidden = false;
                            salc_unin.IsHidden = false;
                            salc_totn.IsHidden = false;
                            //saldo
                            saldo_unin.IsHidden = false;
                            saldo_costoun.IsHidden = false;
                            saldo_ctotaln.IsHidden = false;
                            actualizarTotales(false);
                        }
                    }
                    else
                    {
                        c.IsChecked = false;
                    }
                }
            }
        }


        public void actualizarTotales(bool flag)
        {
            try
            {
                if (ds != null)
                {
                    if (flag == true)
                    {

                        decimal CantEnt = Convert.ToDecimal(ds.Tables[0].Compute("Sum(ent_uni)", "").ToString());
                        decimal TotEnt = Convert.ToDecimal(ds.Tables[0].Compute("Sum(entc_tot)", "").ToString());
                        TxtTotalUnEnt.Text = CantEnt.ToString("N2");
                        TxtTotalUncostEnt.Text = TotEnt.ToString("N2");
                        int promedioEntrada = 0;
                        if (TotEnt > 0 & CantEnt > 0)
                        {
                            TxtTotalUncosEnt.Text = (TotEnt / CantEnt).ToString("N2");
                            promedioEntrada = Convert.ToInt32(TotEnt / CantEnt);
                        }
                        else
                        {
                            TxtTotalUncosEnt.Text = "0";
                        }

                        //---------------
                        ProEnt.Text = promedioEntrada.ToString();
                        decimal CantSal = Convert.ToDecimal(ds.Tables[0].Compute("Sum(sal_uni)", "").ToString());
                        decimal TotSal = Convert.ToDecimal(ds.Tables[0].Compute("Sum(salc_tot)", "").ToString());
                        TxtTotalUnSal.Text = CantSal.ToString("N2");
                        TxtTotalUncostSal.Text = TotSal.ToString("N2");
                        int promedioSalida = 0;
                        if (TotSal > 0 & CantSal > 0)
                        {
                            TxtTotalUncosSal.Text = (TotSal / CantSal).ToString("N2");
                            promedioSalida = Convert.ToInt32(TotSal / CantSal);
                        }
                        else
                        {
                            TxtTotalUncosSal.Text = "0";
                        }

                        //------------
                        ProSal.Text = promedioSalida.ToString();
                        DataRow drow = ds.Tables[0].AsEnumerable().Last();
                        decimal CantSaldo = Convert.ToDecimal(drow["saldo_uni"].ToString());
                        decimal TotSaldo = Convert.ToDecimal(drow["saldo_ctotal"].ToString());
                        TxtTotalUnSaldo.Text = CantSaldo.ToString("N2");
                        TxtTotalUncostSaldo.Text = TotSaldo.ToString("N2");
                        int promedioSaldo = 0;
                        if (TotSaldo > 0 & CantSaldo > 0)
                        {
                            promedioSaldo = Convert.ToInt32(TotSaldo / CantSaldo);
                            TxtTotalUncosSaldo.Text = (TotSaldo / CantSaldo).ToString("N2");
                        }
                        else
                        {
                            TxtTotalUncosSaldo.Text = "0";
                        }
                        ProSaldo.Text = promedioSaldo.ToString();
                        if (VerCostos() == false)
                        {

                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                dr["entc_uni"] = 0;
                                dr["entc_tot"] = 0;
                                dr["salc_uni"] = 0;
                                dr["salc_tot"] = 0;
                                dr["saldo_costou"] = 0;
                                dr["saldo_ctotal"] = 0;
                                //                            TxtTotalUnSaldo.Text = "00.0";
                            }
                            TxtTotalUncosEnt.Text = "00.0";
                            TxtTotalUncosSal.Text = "00.0";
                            TxtTotalUncosSaldo.Text = "00.0";
                            TxtTotalUncostEnt.Text = "00.0";
                            TxtTotalUncostSal.Text = "00.0";
                            TxtTotalUncostSaldo.Text = "00.0";
                        }

                        Total.Text = ds.Tables[0].Rows.Count.ToString();
                    }
                    else
                    {

                        decimal CantEnt = Convert.ToDecimal(ds.Tables[0].Compute("Sum(ent_unin)", "").ToString());
                        decimal TotEnt = Convert.ToDecimal(ds.Tables[0].Compute("Sum(entc_totn)", "").ToString());
                        TxtTotalUnEnt.Text = CantEnt.ToString("N2");
                        TxtTotalUncostEnt.Text = TotEnt.ToString("N2");
                        int promedioEntrada = 0;
                        if (TotEnt > 0 & CantEnt > 0)
                        {
                            TxtTotalUncosEnt.Text = (TotEnt / CantEnt).ToString("N2");
                            promedioEntrada = Convert.ToInt32(TotEnt / CantEnt);
                        }
                        else
                        {
                            TxtTotalUncosEnt.Text = "0";
                        }
                        //--
                        ProEnt.Text = promedioEntrada.ToString();
                        decimal CantSal = Convert.ToDecimal(ds.Tables[0].Compute("Sum(sal_unin)", "").ToString());
                        decimal TotSal = Convert.ToDecimal(ds.Tables[0].Compute("Sum(salc_totn)", "").ToString());
                        TxtTotalUnSal.Text = CantSal.ToString("N2");
                        TxtTotalUncostSal.Text = TotSal.ToString("N2");
                        int promedioSalida = 0;
                        if (TotSal > 0 & CantSal > 0)
                        {
                            TxtTotalUncosSal.Text = (TotSal / CantSal).ToString("N2");
                            promedioSalida = Convert.ToInt32(TotSal / CantSal);
                        }
                        else
                        {
                            TxtTotalUncosSal.Text = "0";
                        }
                        //--

                        ProSal.Text = promedioSalida.ToString();
                        DataRow drow = ds.Tables[0].AsEnumerable().Last();
                        decimal CantSaldo = Convert.ToDecimal(drow["saldo_unin"].ToString());
                        decimal TotSaldo = Convert.ToDecimal(drow["saldo_ctotaln"].ToString());
                        TxtTotalUnSaldo.Text = CantSaldo.ToString("N2");
                        TxtTotalUncostSaldo.Text = TotSaldo.ToString("N2");
                        int promedioSaldo = 0;
                        if (TotSaldo > 0 & CantSaldo > 0)
                        {
                            promedioSaldo = Convert.ToInt32(TotSaldo / CantSaldo);
                            TxtTotalUncosSaldo.Text = (TotSaldo / CantSaldo).ToString("N2");
                        }
                        else
                        {
                            TxtTotalUncosSaldo.Text = "0";
                        }
                        ProSaldo.Text = promedioSaldo.ToString();
                        if (VerCostos() == false)
                        {

                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                dr["entc_unin"] = 0;
                                dr["entc_totn"] = 0;
                                dr["salc_unin"] = 0;
                                dr["salc_totn"] = 0;
                                dr["saldo_costoun"] = 0;
                                dr["saldo_ctotaln"] = 0;
                                //                            TxtTotalUnSaldo.Text = "00.0";
                            }
                            TxtTotalUncosEnt.Text = "00.0";
                            TxtTotalUncosSal.Text = "00.0";
                            TxtTotalUncosSaldo.Text = "00.0";
                            TxtTotalUncostEnt.Text = "00.0";
                            TxtTotalUncostSal.Text = "00.0";
                            TxtTotalUncostSaldo.Text = "00.0";
                        }

                        Total.Text = ds.Tables[0].Rows.Count.ToString();

                    }
                }
            }
            catch (Exception w)
            {
                //MessageBox.Show("error al actualizar totales:" + w);
            }
        }




    }
}
