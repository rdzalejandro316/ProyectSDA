using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Drawing.Printing;
using System.Data.SqlClient;
using System.Threading;
using Syncfusion.XlsIO;
using Syncfusion.UI.Xaml.Grid.Converter;
using Microsoft.Win32;
using System.IO;
using System.Linq;
//using Syncfusion.UI.Xaml.Grid;

namespace SiasoftAppExt
{
    /// <summary>
    /// Lógica de interacción para UserControl1.xaml
    /// </summary>
    public partial class PvInventariosPorGrupoPrint : Window
    {
        bool load = true;
        dynamic SiaWin;
        static int idEmp = 0;
        public int idemp = 0;
        string nitemp = "";
        string nomemp = "";
        string codemp = "";
        DataTable dt = new DataTable();
        private string idbod;
        public string idBod = "";
        string codbod = "";
        public string codpvta = "";
        string nompvta = "";
        string cnEmp = "";
        int idLogo = 0;
        string _nomtipo = string.Empty;
        string _nomgru = string.Empty;
        string _nomimpor = string.Empty;
        string _nombodega = string.Empty;
        public DataSet ds1 = new DataSet();
        public string Conexion;
        DataSet dsPrintSaldosGrupo = new DataSet();
        //Sia.PublicarPnt(9456,"PvInventariosPorGrupoPrint");
        public PvInventariosPorGrupoPrint()
        {
            SiaWin = Application.Current.MainWindow;
            idEmp = SiaWin._BusinessId;
            InitializeComponent();
            //codpvta = SiaWin._UserTag;
            FechaIni.Text = DateTime.Now.Date.ToShortDateString();
            //LoadInfo();
            //CmbTipoRep.SelectedIndex = 0;
            CmbTipoRep.Focus();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            idbod = idBod;
            idEmp = idemp;
            //CmbTipoDoc.SelectedIndex = 0;
            TxtBodega.Text = codbod;
            LoadInfo();
            TxtBodega.Text = codbod;
            this.UpdateLayout();
            //CmbTipoRep.SelectedIndex = 1;
        }
        private void Imprimir_Click(object sender, RoutedEventArgs e)
        {
            if (CmbTipoRep.SelectedIndex!=0)
            {
                MessageBox.Show("Solo se permite imprimir resumen por linea");
                return;
            }
            try
            {
                if (Ejecutar.IsEnabled == false) return;
                if (dsPrintSaldosGrupo == null) return;
                if (dsPrintSaldosGrupo.Tables[0].Rows.Count <= 0)
                {
                    MessageBox.Show("No hay saldos de inventarios en la fecha y bodega seleccionada..");
                    return;
                }
                int LongPapel = ((dsPrintSaldosGrupo.Tables[0].Rows.Count) * 65)+1500;
                //MessageBox.Show((dsPrintSaldosGrupo.Tables[0].Rows.Count * 25).ToString() + "-" + dsPrintSaldosGrupo.Tables[0].Rows.Count.ToString());
                PrintDocument pd = new PrintDocument();
                System.Drawing.Printing.PaperSize ps = new PaperSize("", 475, LongPapel);
                pd.PrintPage += new PrintPageEventHandler(pd_printGrupoPOS);
                pd.PrintController = new StandardPrintController();
                pd.DefaultPageSettings.PaperSize = ps;
                pd.DefaultPageSettings.Margins.Left = 0;
                pd.DefaultPageSettings.Margins.Right = 0;
                pd.DefaultPageSettings.Margins.Top = 0;
                pd.DefaultPageSettings.Margins.Bottom = 1;
                pd.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void pd_printGrupoPOS(object sender, PrintPageEventArgs e)
        {
            try
            {
                string fi = FechaIni.Text;
                System.Drawing.Font fBody = new System.Drawing.Font("Lucida Console", 7, System.Drawing.FontStyle.Bold);
                System.Drawing.Font fBody1 = new System.Drawing.Font("Lucida Console", 7, System.Drawing.FontStyle.Regular);
                System.Drawing.Font fBody2 = new System.Drawing.Font("Lucida Console", 9, System.Drawing.FontStyle.Bold);
                /// alinear valores derecha-izquierda
                System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
                drawFormat.Alignment = System.Drawing.StringAlignment.Far;
                drawFormat.LineAlignment = System.Drawing.StringAlignment.Near;
                System.Drawing.SolidBrush sb = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
                System.Drawing.Graphics g = e.Graphics;
                int pos1 = 0;
                string rowValue1 = "";
                string pathlogo = SiaWin._PathApp + @"\imagenes\" + idLogo.ToString() + "..png";
                e.Graphics.DrawImage(System.Drawing.Image.FromFile(pathlogo), 100, 1, 75, 75);
                String s = nomemp.Trim();
                System.Drawing.Font f = new System.Drawing.Font("Arial", 12);
                System.Drawing.StringFormat sf = new System.Drawing.StringFormat();
                sf.Alignment = System.Drawing.StringAlignment.Center;        // horizontal alignment
                sf.LineAlignment = System.Drawing.StringAlignment.Near;    // vertical alignment
                pos1 += 65;
                System.Drawing.Rectangle r = new System.Drawing.Rectangle(10, 75, 270, f.Height * 1);
                g.DrawRectangle(System.Drawing.Pens.Black, r);
                g.DrawString(s, f, System.Drawing.Brushes.Black, r, sf);
                pos1 += 40;
                s = "Nit:" +nitemp.Trim();
                g.DrawString(s, fBody1, sb, 110, pos1);
                pos1 += 40;
                g.DrawString("NOMBRE BODEGA :", fBody1, sb, 10, pos1);
                g.DrawString(_nombodega.Trim(), fBody1, sb, 110, pos1);
                pos1 += 10;
                g.DrawString("CODIGO BODEGA :", fBody1, sb, 10, pos1);
                g.DrawString(codbod.Trim(), fBody1, sb, 110, pos1);
                pos1 += 10;

                g.DrawString("FECHA CORTE   :", fBody1, sb, 10, pos1);
                g.DrawString(fi, fBody1, sb, 110, pos1);
                pos1 += 10;
                g.DrawString("USUARIO       :", fBody1, sb, 10, pos1);
                string nomuser = SiaWin._UserName;
                g.DrawString(nomuser.Trim(), fBody1, sb, 110, pos1);
                pos1 += 10;
                g.DrawString("FECHA CONSULTA:", fBody1, sb, 10, pos1);
                string dateTimePrint = DateTime.Now.ToString();
                g.DrawString(dateTimePrint.Trim(), fBody1, sb, 110, pos1);
                if(!string.IsNullOrEmpty(_nomtipo))
                {
                    pos1 += 10;
                    g.DrawString("LINEA:", fBody1, sb, 10, pos1);
                    g.DrawString(TxtTip.Text+"-"+ _nomtipo.Trim(), fBody1, sb, 110, pos1);
                }
                if (!string.IsNullOrEmpty(_nomgru))
                {
                    pos1 += 10;
                    g.DrawString("GRUPO:", fBody1, sb, 10, pos1);
                    g.DrawString(TxtGru.Text+"-"+_nomgru.Trim(), fBody1, sb, 110, pos1);
                }
                pos1 += 10;
                string titulo = TextTituloReporte.Text.Trim();
                if (CmbTipoRep.SelectedIndex == 0) titulo= "" + titulo;
                if (CmbTipoRep.SelectedIndex == 1) titulo = "" + titulo;
                g.DrawString(titulo, fBody1, sb, 10, pos1);
                pos1 += 10;
                string __codgru = string.Empty;
                string __nomgru = string.Empty;
                string __codref = string.Empty;
                string __nomref = string.Empty;
                string __serial = string.Empty;
                decimal __saldofin = 0;
                decimal __saldototal = 0;
                g.DrawString("----------------------------------------------", fBody1, sb, 1, pos1);
                pos1 += 10;
                if (CmbTipoRep.SelectedIndex == 0)
                {
                    foreach (System.Data.DataRow row in dsPrintSaldosGrupo.Tables[0].Rows)
                    {
                        __codgru = row["cod_tip"].ToString();
                        __nomgru = row["nom_tip"].ToString();
                        __saldofin = Convert.ToDecimal(row["saldo_fin"].ToString());
                        __saldototal = __saldototal + __saldofin;
                        g.DrawString(__codgru + "-" + __nomgru.Trim() + "   ", fBody1, sb, 10, pos1);
                        rowValue1 = __saldofin.ToString("N0");
                        g.DrawString(rowValue1 + " ____", fBody1, sb, 260, pos1, drawFormat);
                        pos1 += 12;
                    }
                }
                if (CmbTipoRep.SelectedIndex == 1)
                {
                    DataTable dt = dsPrintSaldosGrupo.Tables[0].AsEnumerable().GroupBy(rr => new { Col1 = rr["cod_gru"] }).Select(gr => gr.OrderBy(rr => rr["cod_gru"]).First()).CopyToDataTable();
                    foreach (System.Data.DataRow row1 in dt.Rows)
                    {
                        __codgru = row1["cod_gru"].ToString().Trim();
                        __nomgru = row1["nom_gru"].ToString().Trim();
                        g.DrawString("Grupo:"+__codgru + "-" + __nomgru.Trim() + "   ", fBody, sb, 10, pos1);
                        pos1 += 15;
                        DataRow[] result = dsPrintSaldosGrupo.Tables[0].Select("cod_gru='"+__codgru+"'");
                        decimal _cnt = 0;
                        foreach (DataRow row2 in result)
                        {
                            __codref = row2["cod_ref"].ToString().Trim();
                            __nomref = row2["nom_ref"].ToString().Trim();
                            __saldofin = Convert.ToDecimal(row2["saldo_fin"].ToString());
                            _cnt += __saldofin;
                            g.DrawString(__codref + " -S:" + __serial.Trim() + "   ", fBody1, sb, 1, pos1);
                            rowValue1 = __saldofin.ToString("N0");
                            g.DrawString(rowValue1 + " ____", fBody1, sb, 280, pos1, drawFormat);
                            pos1 += 12;
                            g.DrawString(__nomref , fBody1, sb, 1, pos1);
                            pos1 += 15;
                        }
                        g.DrawString("Total Grupo:"+__codgru+"-"+__nomgru, fBody, sb, 1, pos1);
                        rowValue1 = _cnt.ToString("N0");
                        g.DrawString(rowValue1 + " ____", fBody, sb, 280, pos1, drawFormat);
                        pos1 += 15;
                    }
                }
                pos1 += 15;
                pos1 += 12;
                g.DrawString("----------------------------------------------", fBody1, sb, 1, pos1);
                pos1 += 10;
                g.DrawString("Total Inventario Unidades:", fBody1, sb, 10, pos1);
                rowValue1 = __saldototal.ToString("N0");
                g.DrawString(rowValue1 +" ____", fBody1, sb, 260, pos1, drawFormat);
                pos1 += 10;
                g.DrawString("----------------------------------------------", fBody1, sb, 1, pos1);
                pos1 += 40;
                g.DrawString("Elaborado Por         Revisado Por", fBody1, sb, 1, pos1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void LoadInfo()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idEmp);
                idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                nitemp = foundRow["BusinessNit"].ToString().Trim();
                nomemp = foundRow["BusinessName"].ToString().Trim();
                codemp = foundRow["BusinessCode"].ToString().Trim();
                //MessageBox.Show(codemp);
                //        _usercontrol.Seg.Auditor(0,_usercontrol.ProjectId,idUser,_usercontrol.GroupId,idEmp,_usercontrol.ModuleId,_usercontrol.AccesoId,0,"Ingreso a: Punto de venta"+" - " +_titulo,"");
                if (codpvta == string.Empty)
                {
                    //_usercontrol.Opacity = 0.5;
                    MessageBox.Show("El usuario no tiene asignado un punto de venta, Pantalla Bloqueada");
                    Ejecutar.IsEnabled = false;

                    //_usercontrol.IsEnabled=false;
                }
                else
                {
                    nompvta = SiaWin.Func.cmpCodigo("copventas", "cod_pvt", "nom_pvt", codpvta, idEmp);
                    codbod = SiaWin.Func.cmpCodigo("copventas", "cod_pvt", "cod_bod", codpvta, idEmp);
                    if (string.IsNullOrEmpty(codbod))
                    {
                        //_usercontrol.Opacity = 0.5;
                        MessageBox.Show("El punto de venta Asignado no tiene bodega , Pantalla Bloqueada");
                        Ejecutar.IsEnabled = false;
                        
                        //usercontrol.IsEnabled=false;
                    }
                    TxtNomBod.Text = nompvta;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private async void Ejecutar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _nomimpor = string.Empty;
                _nombodega = string.Empty;
                _nomtipo = string.Empty;
                _nomgru = string.Empty;
                int tiporep = CmbTipoRep.SelectedIndex;
                if (tiporep < 0)
                {
                    MessageBox.Show("Seleccone un Tipo de reporte");
                    CmbTipoRep.Focus();
                    return;
                }
                dsPrintSaldosGrupo.Clear();
                // validaciones
                if (string.IsNullOrEmpty(TxtBodega.Text.Trim()))
                {
                    MessageBox.Show("Digita codigo de bodega..");
                    TxtBodega.Focus();
                    return;
                }
                string _tipo = TxtTip.Text.Trim();
                string _grupo = TxtGru.Text.Trim();
                string _bodega = TxtBodega.Text.Trim();
                if (!string.IsNullOrEmpty(_bodega))
                {
                    _nombodega = ValidaCampo(_bodega, "nom_bod", "cod_bod", "inmae_bod");
                    if (string.IsNullOrEmpty(_nombodega)) return;
                }
                if (!string.IsNullOrEmpty(_tipo))
                {
                    _nomtipo = ValidaCampo(_tipo, "nom_tip", "cod_tip", "inmae_tip");
                    if (string.IsNullOrEmpty(_nomtipo)) return;
                }
                if (!string.IsNullOrEmpty(_grupo))
                {
                    _nomgru  = ValidaCampo(_grupo, "nom_gru", "cod_gru", "inmae_gru");
                    if (string.IsNullOrEmpty(_nomgru)) return;
                }

                string fi = FechaIni.Text;
                string bod = TxtBodega.Text.Trim();
                string tip = TxtTip.Text.Trim();
                string gru = TxtGru.Text.Trim();

                //string sex = CmbTipoDoc.Text.Trim().Substring(0, 1);
                //if (sex == "T") sex = string.Empty;
                TextTotalEntradas.Text = "0";
                this.UpdateLayout();
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                sfBusyIndicator.IsBusy = true;
                Ejecutar.IsEnabled = false;
                source.CancelAfter(TimeSpan.FromSeconds(1));
                
                var slowTask = Task<DataSet>.Factory.StartNew(() => SlowDude(tiporep, fi, bod, tip, gru, source.Token), source.Token);
                await slowTask;
                TextTotalEntradas.Text = "0";                
                //MessageBox.Show(slowTask.Result.ToString());
                Ejecutar.IsEnabled = true;
                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {

                    dsPrintSaldosGrupo = (DataSet)slowTask.Result;
                    dataGridSF.ItemsSource = dsPrintSaldosGrupo.Tables[0].DefaultView;
                    TextTotalEntradas.Text = ((DataSet)slowTask.Result).Tables[0].Compute("Sum(saldo_fin)", "").ToString();
                    dataGridSF.Focus();
                    dataGridSF.SelectedIndex = 0;
                    dataGridSF.Focus();
                }
                sfBusyIndicator.IsBusy = false;
            }
            catch (Exception ex)
            {
                dsPrintSaldosGrupo.Clear();
                sfBusyIndicator.IsBusy = false;
                MessageBox.Show(ex.Message);
            }
        }
        private DataSet SlowDude(int tiporep,string fi, string bod, string tip,string gru,  CancellationToken cancellationToken)
        {
            try
            {
                DataSet jj = LoadData(tiporep, fi,bod,tip,gru, cancellationToken);
                return jj;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return null;
        }
        private DataSet LoadData(int tiporep,string fi, string bod, string tip,string gru,CancellationToken cancellationToken)
        {
            try
            {
               
                string SpTipoRep = string.Empty;
                string subtit = "";
                if (tiporep == 0) subtit = "R";
                if (tiporep == 0) SpTipoRep = "_EmpSaldosInventariosPorBodegaLineaPOS";
                if (tiporep == 1) SpTipoRep = "_EmpSaldosInventariosPorBodegaLinea";
                //MessageBox.Show(SpTipoRep);
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand(SpTipoRep, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Fecha" + subtit, fi);
                cmd.Parameters.AddWithValue("@Bod" + subtit, bod);
                cmd.Parameters.AddWithValue("@Tip" + subtit, tip);
                cmd.Parameters.AddWithValue("@codemp" + subtit, codemp);
                //cmd.Parameters.AddWithValue("@Gru" + subtit, gru);
                //cmd.Parameters.AddWithValue("@Prv" + subtit, imp);
                //if (tiporep==1) cmd.Parameters.AddWithValue("@TipoReporte" + subtit, 1);//if you have parameters.
                //cmd.Parameters.AddWithValue("@Sexo" + subtit, sex);//if you have parameters.
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds);
                con.Close();
                return ds;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }
        private void Txt_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var uiElement = e.OriginalSource as UIElement;
                uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
            if (e.Key == Key.F8)
            {                            
                try
                {
                    string idTab = ((TextBox)sender).Tag.ToString();
                    if (idTab.Length > 0)
                    {
                        string tag = ((TextBox)sender).Tag.ToString();
                        string cmptabla = ""; string cmpcodigo = ""; string cmpnombre = ""; string cmporden = ""; string cmpidrow = ""; string cmptitulo = ""; string cmpconexion = ""; bool mostrartodo = true; string cmpwhere = "";
                        if (string.IsNullOrEmpty(tag)) return;

                        if (tag == "inmae_tip")
                        {
                            cmptabla = tag; cmpcodigo = "cod_tip"; cmpnombre = "UPPER(nom_tip)"; cmporden = "cod_tip"; cmpidrow = "cod_tip"; cmptitulo = "Maestra de Tipos"; cmpconexion = cnEmp; mostrartodo = true; cmpwhere = "";
                        }
                        if (tag == "inmae_gru")
                        {
                            cmptabla = tag; cmpcodigo = "cod_gru"; cmpnombre = "UPPER(nom_gru)"; cmporden = "cod_gru"; cmpidrow = "cod_tip"; cmptitulo = "Maestra de Tipos"; cmpconexion = cnEmp; mostrartodo = true; cmpwhere = "";
                        }
                        if (tag == "inmae_bod")
                        {
                            cmptabla = tag; cmpcodigo = "cod_bod"; cmpnombre = "UPPER(nom_bod)"; cmporden = "cod_bod"; cmpidrow = "cod_bod"; cmptitulo = "Maestra de Bodegas"; cmpconexion = cnEmp; mostrartodo = true; cmpwhere = "";
                        }
                        int idr = 0; string code = ""; string nom = "";
                        dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, cnEmp, mostrartodo, cmpwhere,idEmp:idemp);
                        winb.ShowInTaskbar = false;
                        winb.Owner = Application.Current.MainWindow;
                        winb.ShowDialog();
                        idr = winb.IdRowReturn;
                        code = winb.Codigo;
                        nom = winb.Nombre;
                        winb = null;
                        if (idr > 0)
                        {
                            if (tag == "inmae_tip")
                            {
                                TxtTip.Text = code; //TextBx_con.Text = nom;
                                TxtNomTip.Text = nom;
                            }
                            if (tag == "inmae_gru")
                            {
                                TxtGru.Text = code; //TextBx_con.Text = nom;
                                TxtNomGru.Text = nom;
                            }
                            if (tag == "inmae_bod")
                            {
                                TxtBodega.Text = code; //TextBx_ActSig.Text = nom;
                                TxtNomBod.Text = nom;
                            }
                            var uiElement = e.OriginalSource as UIElement;
                            uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                        }
                        e.Handled = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }
        private void ExportarXLS_Click(object sender, RoutedEventArgs e)
        {
            var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
            options.ExcelVersion = ExcelVersion.Excel2013;
            var excelEngine = dataGridSF.ExportToExcel(dataGridSF.View, options);
            var workBook = excelEngine.Excel.Workbooks[0];
            workBook.ActiveSheet.Columns[2].NumberFormat = "0.00";
            workBook.ActiveSheet.Columns[2].HorizontalAlignment = ExcelHAlign.HAlignRight;


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
                if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archvo", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    //Launching the Excel file using the default Application.[MS Excel Or Free ExcelViewer]
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
            }
        }
        private string ValidaCampo(string codigo,string cmpnombre,string cmpcodigo,string tabla)
        {
            try
            {
                if (string.IsNullOrEmpty(codigo)) return string.Empty;
                if (string.IsNullOrEmpty(cmpnombre)) return string.Empty;
                if (string.IsNullOrEmpty(cmpcodigo)) return string.Empty;
                if (string.IsNullOrEmpty(tabla)) return string.Empty;
                //if(ConfigCSource.EnabledCodTer==false) return true;
                //                    dr =((Inicio)Application.Current.MainWindow).Func.SqlDR("SELECT idrow,cod_ter,nom_ter,dir1,tel1,observ,i_cupocc,cupo_cxc,dia_plaz,ind_suc,ind_ret,ret_iva,ret_ica,bloq_aut,bloq_tmk,bloq_ate,cod_ven,bloqueo,estado,email FROM comae_ter where cod_ter='"+Id.ToString()+"' or idrow="+Id.ToString(),idEmp);
                //MessageBox.Show(@"select " + cmpnombre + " from " + tabla + " where " + cmpcodigo + "='" + codigo + "'");
                SqlDataReader dr = SiaWin.DB.SqlDR(@"select "+cmpnombre+" from "+tabla+" where "+cmpcodigo+"='"+codigo+"'" , idEmp);
                string nomreturn = string.Empty;
                while (dr.Read())
                {
                    nomreturn= dr[cmpnombre].ToString().Trim();
                }
                dr.Close();
                if(string.IsNullOrEmpty(nomreturn))
                {
                    MessageBox.Show("No existe codigo:" + codigo + " en tabla: " + tabla);
                    return string.Empty;
                }
                if (tabla == "inmae_tip" & !string.IsNullOrEmpty(nomreturn))
                {
                    TxtNomTip.Text = nomreturn;
                }
                if (tabla == "inmae_gru" & !string.IsNullOrEmpty(nomreturn))
                {
                    TxtNomGru.Text = nomreturn;
                }
                if (tabla == "inmae_bod" & !string.IsNullOrEmpty(nomreturn))
                {
                    TxtNomBod.Text = nomreturn;
                }

                return nomreturn;
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (System.Exception _error)
            {
                MessageBox.Show(_error.Message);
            }
            return string.Empty;
        }

        private void TxtTip_LostFocus(object sender, RoutedEventArgs e)
        {
            string idTab = ((TextBox)sender).Tag.ToString();
            if (idTab.Length > 0)
            {
                string tag = ((TextBox)sender).Tag.ToString();
                string value = ((TextBox)sender).Text.Trim();
                if (tag == "inmae_tip" & string.IsNullOrEmpty(value) )
                {
                    TxtNomTip.Text = string.Empty;
                }
                if (tag == "inmae_tip" & !string.IsNullOrEmpty(value))
                {
                    TxtNomTip.Text = ValidaCampo(value, "nom_tip", "cod_tip", "inmae_tip");
                    if (string.IsNullOrEmpty(TxtNomTip.Text.Trim())) TxtTip.Text = "";
                }
                if (tag == "inmae_gru" & !string.IsNullOrEmpty(value))
                {
                    TxtNomGru.Text = ValidaCampo(value, "nom_gru", "cod_gru", "inmae_gru");
                    if (string.IsNullOrEmpty(TxtNomGru.Text.Trim())) TxtGru.Text = "";
                }
                if (tag == "inmae_bod" & string.IsNullOrEmpty(value))
                {
                    TxtNomBod.Text = string.Empty;                    
                }
                if (tag == "inmae_bod" & !string.IsNullOrEmpty(value))
                {
                    TxtNomBod.Text = ValidaCampo(value, "nom_bod", "cod_bod", "inmae_bod");
                    if (string.IsNullOrEmpty(TxtNomBod.Text.Trim())) TxtBodega.Text = "";
                }
            }
        }
        private bool CreaColumnas(int tipo)
        {
            if(tipo==0)
            {
                TextTituloReporte.Text = "Saldos de Inventario Por Linea";
                dataGridSF.Columns.Clear();
                dataGridSF.Columns.Add(new Syncfusion.UI.Xaml.Grid.GridTextColumn() { Width=110, HeaderText = "Linea", MappingName = "cod_tip", AllowFiltering = true });
                dataGridSF.Columns.Add(new Syncfusion.UI.Xaml.Grid.GridTextColumn() { Width = 450, HeaderText = "Nombre Linea", MappingName = "nom_tip", AllowFiltering = true });
                dataGridSF.Columns.Add(new Syncfusion.UI.Xaml.Grid.GridTextColumn() { Width = 110, HeaderText = "Saldo", MappingName = "saldo_fin" });
            }
            if (tipo==1)
            {
                TextTituloReporte.Text = "Saldos de Inventario Por Linea - Detallado";
                dataGridSF.Columns.Clear();
                dataGridSF.Columns.Add(new Syncfusion.UI.Xaml.Grid.GridTextColumn() { Width = 100, HeaderText = "Linea", MappingName = "nom_tip", AllowFiltering = true });
                dataGridSF.Columns.Add(new Syncfusion.UI.Xaml.Grid.GridTextColumn() { Width = 110, HeaderText = "Codigo", MappingName = "cod_ref",AllowFiltering=true });
                dataGridSF.Columns.Add(new Syncfusion.UI.Xaml.Grid.GridTextColumn() { Width = 280,HeaderText = "Nombre Producto", MappingName = "nom_ref" , AllowFiltering = true });
                dataGridSF.Columns.Add(new Syncfusion.UI.Xaml.Grid.GridTextColumn() { Width = 70, HeaderText = "Saldo", MappingName = "saldo_fin" });
                dataGridSF.Columns.Add(new Syncfusion.UI.Xaml.Grid.GridCurrencyColumn() { Width = 90, HeaderText = "ValorUnit", MappingName = "val_ref", CurrencyDecimalSeparator = ".", CurrencyGroupSeparator = ",", CurrencyGroupSizes = System.Windows.Media.Int32Collection.Parse("3")      });
            }
            return true;
        }

        private void CmbTipoRep_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (load == false)
            {
                dsPrintSaldosGrupo.Clear();
                sfBusyIndicator.IsBusy = false;
                TextTotalEntradas.Text = "0";
                CreaColumnas(((ComboBox)sender).SelectedIndex);
            }
            load = false;
        }
    }
}
