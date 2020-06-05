using Syncfusion.Windows.Controls.Grid.Converter;
using Syncfusion.XlsIO;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Syncfusion.UI.Xaml.Grid.Converter;
using Microsoft.Win32;
using System.IO;
using RecibosProvisionales;
using Syncfusion.UI.Xaml.Grid;
using System.Drawing.Printing;
using System.Drawing;
using Syncfusion.UI.Xaml.ScrollAxis;
//using RecibosDeCaja;

namespace SiasoftAppExt
{
    //RecibosdeCaja
    //Sia.PublicarPnt(9535,"RecibosProvisionales");

    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9535,"RecibosProvisionales");  //carga desde sql
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
    //ww.idemp="1";
    //ww.codpvta="003";
    //ww.codter="01";
    //ww.ShowDialog();
    public partial class RecibosProvisionales : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        int moduloid = 0;
        public string codter = "";
        string nomter = "";
        string dirter = "";
        string telter = "";
        public string codbod = "";
        public string codpvta = "";
        string nompvta = "";
        string codcco = "";
        string nitemp = "";
        string BusinessCode = "";
        string cnEmp = "";
        int idLogo = 0;
        DataSet ds = new DataSet();
        DataTable dtVen = new DataTable();
        DataTable dtVen1 = new DataTable();
        DataTable dtBan = new DataTable();
        DataTable dtCue = new DataTable();
        double valorCxC = 0;
        double valorCxCAnt = 0;
        double valorCxP = 0;
        double valorCxPAnt = 0;
        double saldoCxC = 0;
        double saldoCxCAnt = 0;
        double saldoCxP = 0;
        double saldoCxPAnt = 0;
        double abonoCxC = 0;
        double abonoCxCAnt = 0;
        double abonoCxP = 0;
        double abonoCxPAnt = 0;
        double Retefte = 0;
        double Reteica = 0;
        double Reteiva = 0;
        double Mayorvlr = 0;
        double Menorvlr = 0;
        double VlrRecibido = 0;
        double Anticipo = 0;
        double dtosImal = 0;
        double dtosIncol = 0;
        double dtosTmk = 0;
        double dtosGab = 0;
        double dtosVcd = 0;
        double dtosSic = 0;
        double dtosOt = 0;
        public string codcliente = "";
        DataTable fPago = new DataTable();
        int regcab = 0;

        public RecibosProvisionales()
        {
            InitializeComponent();
            TextFecha.Text = DateTime.Now.ToString();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            codpvta = SiaWin._UserTag;
            //LoadInfo();
            ActivaDesactivaControles(0);
            //this.DataContext = this;
            FechaIni.Text = DateTime.Now.ToShortDateString();
            FechaFin.Text = DateTime.Now.ToShortDateString();
            BtbGrabar.Focus();

            //string valorr = ((Inicio)Application.Current.MainWindow).ValReturn;
        }

        public void LoadInfo()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                nitemp = foundRow["BusinessNit"].ToString().Trim();
                BusinessCode = foundRow["BusinessCode"].ToString().Trim();
                TxtEmpresa.Text = SiaWin._BusinessName.ToString().Trim();
                TxtPVenta.Text = codpvta;
                TxtUser.Text = SiaWin._UserAlias;

                System.Data.DataRow[] drmodulo = SiaWin.Modulos.Select("ModulesCode='IN'");
                if (drmodulo == null) this.IsEnabled = false;
                moduloid = Convert.ToInt32(drmodulo[0]["ModulesId"].ToString());

                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Recibo provisional - Empresa:" + BusinessCode + "-" + nomempresa;
                if (codpvta == string.Empty)
                {
                    //_usercontrol.Opacity = 0.5;
                    MessageBox.Show("El usuario no tiene asignado un punto de venta, Pantalla Bloqueada");
                    this.IsEnabled = false;
                    //_usercontrol.IsEnabled=false;
                }
                else
                {
                    nompvta = SiaWin.Func.cmpCodigo("copventas", "cod_pvt", "nom_pvt", codpvta, idemp);
                    TxtPVenta.Text = codpvta + "-" + nompvta;
                    codbod = SiaWin.Func.cmpCodigo("copventas", "cod_pvt", "cod_bod", codpvta, idemp);
                    codcco = SiaWin.Func.cmpCodigo("copventas", "cod_pvt", "cod_cco", codpvta, idemp);
                    if (string.IsNullOrEmpty(codbod))
                    {
                        //_usercontrol.Opacity = 0.5;
                        MessageBox.Show("El punto de venta Asignado no tiene bodega , Pantalla Bloqueada");
                        //usercontrol.IsEnabled=false;
                    }
                    TxtBod.Text = codbod;
                }
                dtVen = SiaWin.Func.SqlDT("select cod_mer as cod_ven,cod_mer+'-'+nom_mer as nom_ven from inmae_mer where estado=1  order by cod_mer", "inmae_mer", idemp);
                dtVen.PrimaryKey = new System.Data.DataColumn[] { dtVen.Columns["cod_mer"] };
                dtVen1 = SiaWin.Func.SqlDT("select cod_mer as cod_ven,cod_mer+'-'+nom_mer as nom_ven from inmae_mer where estado=1  order by cod_mer", "inmae_mer", idemp);
                dtVen1.PrimaryKey = new System.Data.DataColumn[] { dtVen1.Columns["cod_mer"] };

                // establecer paths
                CmbVen.ItemsSource = dtVen.DefaultView;
                CmbVen.DisplayMemberPath = "nom_ven";
                CmbVen.SelectedValuePath = "cod_ven";

                CmbVen1.ItemsSource = dtVen1.DefaultView;
                CmbVen1.DisplayMemberPath = "nom_ven";
                CmbVen1.SelectedValuePath = "cod_ven";

                //dtBan = SiaWin.Func.SqlDT("select cod_ban,cod_ban+'-'+nom_ban as nom_ban,cod_cta from comae_ban  order by cod_ban", "comae_ban", idemp);
                //dtBan.PrimaryKey = new System.Data.DataColumn[] { dtBan.Columns["cod_ban"] };                
                //CmbBan.ItemsSource = dtBan.DefaultView;
                //CmbBan.DisplayMemberPath = "nom_ban";
                //CmbBan.SelectedValuePath = "cod_ban";
            }
            catch (Exception e)
            {
                SiaWin.Func.SiaExeptionGobal(e);
                MessageBox.Show(e.Message);
            }
        }
        public void ActivaDesactivaControles(int estado)
        {
            if (estado == 0)
            {
                TextCodeCliente.Text = string.Empty;
                TextNomCliente.Text = string.Empty;
                TextRProv.Text = string.Empty;
                TextNota.Text = string.Empty;
                CmbVen.SelectedIndex = -1;
                CmbVen1.SelectedIndex = -1;
                TextNota.IsEnabled = false;
                CmbVen.IsEnabled = false;
                CmbVen1.IsEnabled = false;
                BtbGrabar.Content = "Nuevo";
                BtbCancelar.Content = "Salir";
                dataGrid.AllowEditing = true;
                dtCue.Clear();
                TextCxC.Text = "0,00";
                TextCxCAnt.Text = "0,00";
                TextCxP.Text = "0,00";
                TextCxPAnt.Text = "0,00";
                TotalCxc.Text = "0,00";
                TextCxCAbono.Text = "0,00";
                TextCxCAntAbono.Text = "0,00";
                TextCxPAbono.Text = "0,00";
                TextCxPAntAbono.Text = "0,00";
                TotalAbono.Text = "0,00";
                TextCxCSaldo.Text = "0,00";
                TextCxCAntSaldo.Text = "0,00";
                TextCxPSaldo.Text = "0,00";
                TextCxPAntSaldo.Text = "0,00";
                TotalSaldo.Text = "0,00";
                TotalRecaudo.Text = "0,00";



                TextCodeCliente.Focusable = false;
                TextRProv.Focusable = false;
                TxtBDtoImal.Text = "0,00";
                TxtBDtoIncol.Text = "0,00";
                TxtBDtoTmk.Text = "0,00";
                TxtBDtoGab.Text = "0,00";
                TxtBDtoVcd.Text = "0,00";
                TxtBDtoSic.Text = "0,00";
                TxtBDtoOt.Text = "0,00";
            }
            if (estado == 1) //creando
            {
                TextCodeCliente.Text = string.Empty;
                TextNomCliente.Text = string.Empty;
                TextRProv.Text = string.Empty;
                TextNota.Text = "Cancelacion/Abono Facturas";
                CmbVen.SelectedIndex = -1;
                CmbVen1.SelectedIndex = -1;
                CmbVen.IsEnabled = true;
                CmbVen1.IsEnabled = true;
                TextNota.IsEnabled = true;
                BtbGrabar.Content = "Grabar";
                BtbCancelar.Content = "Cancelar";
                dataGrid.AllowEditing = false;
                dtCue.Clear();
                //dataGrid.up.CommitEdit();
                dataGrid.UpdateLayout();
                //dataGrid.SelectedIndex = 0;
                TextCodeCliente.Focusable = true;

                TextCodeCliente.Focusable = true;
                TotalRecaudo.Text = "0,00";
                TextRetefte.Text = "0,00";
                TextIca.Text = "0,00";
                TextReteIva.Text = "0,00";
                TextVlrRecibido.Text = "0,00";
                TextMayorVlr.Text = "0,00";
                TextMenorVlr.Text = "0,00";
                TextAnticipo.Text = "0,00";
                TextRProv.Focusable = true;
                TextCodeCliente.Focus();
            }
        }
        public bool ValExistReciboProv(string recibo)
        {


            if (string.IsNullOrEmpty(recibo) && (CmbVen.SelectedValue.ToString().Trim() == "A1" || CmbVen.SelectedValue.ToString().Trim() == "A2"))
                return false;

            bool flag = false;
            string query = "select * from cocabrcpv where rcprov='" + recibo + "' ";
            DataTable dt = SiaWin.Func.SqlDT(query, "table", idemp);
            if (dt.Rows.Count > 0) flag = true;
            return flag;
        }

        public bool ValExistReciboConta(string recibo)
        {
            if (string.IsNullOrEmpty(recibo) && (CmbVen.SelectedValue.ToString().Trim() == "A1" || CmbVen.SelectedValue.ToString().Trim() == "A2"))
                return false;

            bool flag = false;
            string query = "select * from CoCab_doc where rc_prov='" + recibo + "' ";
            DataTable dt = SiaWin.Func.SqlDT(query, "table", idemp);
            if (dt.Rows.Count > 0) flag = true;
            if (CmbVen.SelectedValue.ToString().Trim() == "A1" || CmbVen.SelectedValue.ToString().Trim() == "A2") flag = false;
            return flag;
        }




        private void BtbGrabar_Click(object sender, RoutedEventArgs e)
        {
            //   MessageBox.Show("ini click");
            try
            {
                if (BtbGrabar.Content.ToString() == "Nuevo")
                {
                    ActivaDesactivaControles(1);
                }
                else
                {

                    if (string.IsNullOrEmpty(cnEmp))
                    {
                        MessageBox.Show("Error - Cadena de Conexion nulla");
                        return;
                    }
                    string _CodeCliente = TextCodeCliente.Text;
                    if (string.IsNullOrEmpty(_CodeCliente))
                    {
                        MessageBox.Show("Falta Nit/cc del cliente..");
                        TextCodeCliente.Focus();
                        return;
                    }
                    if (CmbVen.SelectedIndex < 0)
                    {
                        MessageBox.Show("Seleccione Vendedor.....");
                        CmbVen.Focus();
                        return;
                    }
                    if (CmbVen1.SelectedIndex < 0)
                    {
                        MessageBox.Show("Seleccione Vendedor.....");
                        CmbVen1.Focus();
                        return;
                    }
                    if (dtCue.Rows.Count == 0)
                    {
                        MessageBox.Show("No hay registros en el cuerpo de documentos...");
                        TextCodeCliente.Focus();
                        return;
                    }


                    if (CmbVen.SelectedIndex <= 0)
                    {
                        MessageBox.Show("seleccione el Vendedor Recaudor ");
                        return;
                    }

                    if (validarReciboProvi(TextRProv.Text) == false)
                    {
                        MessageBox.Show("complete el campo de recibo provisional");
                        TextRProv.Dispatcher.BeginInvoke((Action)(() => { TextRProv.Focus(); }));
                        return;
                    }

                    decimal ValorRecibido = Convert.ToDecimal(TextVlrRecibido.Value);
                    decimal totalRecibido = Math.Truncate(ValorRecibido);

                    var valor = TotalRecaudo.Text;
                    decimal TotalRec = decimal.Parse(valor, NumberStyles.Currency);

                    if (totalRecibido != TotalRec)
                    {
                        MessageBox.Show("el valor recibido no es igual al total de recaudo");
                        return;
                    }

                    if (ValExistReciboProv(TextRProv.Text) == true)
                    {
                        MessageBox.Show("el recibo provicional ya existe");
                        return;
                    }

                    if (ValExistReciboConta(TextRProv.Text) == true)
                    {
                        MessageBox.Show("el recibo provicional ya existe en contabilidad");
                        return;
                    }


                    //// valida valor recaudado y cruces
                    double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=3").ToString(), out saldoCxC);
                    double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=4").ToString(), out saldoCxCAnt);
                    double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=1").ToString(), out saldoCxP);
                    double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=2").ToString(), out saldoCxPAnt);
                    double.TryParse(dtCue.Compute("Sum(dto_imal)", "tip_apli=3").ToString(), out dtosImal);
                    double.TryParse(dtCue.Compute("Sum(dto_incol)", "tip_apli=3").ToString(), out dtosIncol);
                    double.TryParse(dtCue.Compute("Sum(dto_tmk)", "tip_apli=3").ToString(), out dtosTmk);
                    double.TryParse(dtCue.Compute("Sum(dto_gab)", "tip_apli=3").ToString(), out dtosGab);
                    double.TryParse(dtCue.Compute("Sum(dto_vcd)", "tip_apli=3").ToString(), out dtosVcd);
                    double.TryParse(dtCue.Compute("Sum(dto_sic)", "tip_apli=3").ToString(), out dtosSic);
                    double.TryParse(dtCue.Compute("Sum(dto_ot)", "tip_apli=3").ToString(), out dtosOt);
                    Retefte = Convert.ToDouble(TextRetefte.Value);
                    Reteica = Convert.ToDouble(TextIca.Value);
                    Reteiva = Convert.ToDouble(TextReteIva.Value);
                    Mayorvlr = Convert.ToDouble(TextMayorVlr.Value);
                    Menorvlr = Convert.ToDouble(TextMenorVlr.Value);
                    VlrRecibido = Convert.ToDouble(TextVlrRecibido.Value);
                    Anticipo = Convert.ToDouble(TextAnticipo.Value);
                    double _abono = (saldoCxC + saldoCxPAnt + Anticipo + Mayorvlr) - (saldoCxCAnt + saldoCxP + Retefte + Reteica + Reteiva + Menorvlr + dtosImal + dtosIncol + dtosTmk + dtosGab + dtosVcd + dtosSic + dtosOt);

                    if (_abono < 0)
                    {
                        MessageBox.Show("Valor Abono no puede ser menor a 0");
                        dataGrid.Focus();
                        dataGrid.SelectedIndex = 0;
                        //dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.Items[0], dataGrid.Columns[8]);
                        return;
                    }
                    if (saldoCxC <= 0)
                    {
                        MessageBox.Show("Valor Abonos de factura debe ser mayor a 0");
                        dataGrid.Focus();
                        dataGrid.SelectedIndex = 0;
                        //dataGrid.CurrentCellInfo = new DataGridCellInfo(dataGrid.Items[0], dataGrid.Columns[8]);
                        return;

                    }
                    double abono = Convert.ToDouble(dtCue.Compute("Sum(abono)", "").ToString());
                    if (abono <= 0)
                    {
                        MessageBox.Show("No hay Abonos...");
                        dataGrid.Focus();
                        dataGrid.SelectedIndex = 0;
                        //dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.Items[0], dataGrid.Columns[8]);
                        return;
                    }
                    if (MessageBox.Show("Usted desea guardar el documento..?", "Guardar Recibo de Provicional", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        try
                        {
                            int iddocumento = 0;
                            //if (!ValidaSaldosDoc()) return;  //Valida que los documentos no fueron cancelados por otro usuario
                            //                           MessageBox.Show("aqui0");
                            double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=3").ToString(), out saldoCxC);
                            double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=4").ToString(), out saldoCxCAnt);
                            double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=1").ToString(), out saldoCxP);
                            double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=2").ToString(), out saldoCxPAnt);
                            double.TryParse(dtCue.Compute("Sum(dto_imal)", "tip_apli=3").ToString(), out dtosImal);
                            double.TryParse(dtCue.Compute("Sum(dto_incol)", "tip_apli=3").ToString(), out dtosIncol);
                            double.TryParse(dtCue.Compute("Sum(dto_tmk)", "tip_apli=3").ToString(), out dtosTmk);
                            double.TryParse(dtCue.Compute("Sum(dto_gab)", "tip_apli=3").ToString(), out dtosGab);
                            double.TryParse(dtCue.Compute("Sum(dto_vcd)", "tip_apli=3").ToString(), out dtosVcd);
                            double.TryParse(dtCue.Compute("Sum(dto_sic)", "tip_apli=3").ToString(), out dtosSic);
                            double.TryParse(dtCue.Compute("Sum(dto_ot)", "tip_apli=3").ToString(), out dtosOt);
                            Retefte = Convert.ToDouble(TextRetefte.Value);
                            Reteica = Convert.ToDouble(TextIca.Value);
                            Reteiva = Convert.ToDouble(TextReteIva.Value);
                            Mayorvlr = Convert.ToDouble(TextMayorVlr.Value);
                            Menorvlr = Convert.ToDouble(TextMenorVlr.Value);
                            Anticipo = Convert.ToDouble(TextAnticipo.Value);
                            // MessageBox.Show("aqui1");
                            double _abonototal = (saldoCxC + saldoCxPAnt + Anticipo + Mayorvlr) - (saldoCxCAnt + saldoCxP + Retefte + Reteica + Reteiva + Menorvlr + dtosImal + dtosIncol + dtosTmk + dtosGab + dtosVcd + dtosSic + dtosOt);
                            // descontar o sumar otros valores                            


                            SiaWin.ValReturn = _abonototal;
                            //MessageBox.Show("aqui2");
                            //Window wFpago = SiaWin.WindowExt(9341, "FormasDePago");
                            FormasDePago wFpago = new FormasDePago();

                            if (wFpago == null)
                            {
                                MessageBox.Show("Windows Null");
                                return;
                            }
                            string[] strArrayParam = new string[] { TextCodeCliente.Text.Trim(), TextNomCliente.Text.Trim(), TotalAbono.Text };
                            wFpago.ShowInTaskbar = false;
                            wFpago.Owner = Application.Current.MainWindow;
                            wFpago.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                            wFpago.ShowDialog();
                            wFpago = null;
                            if (SiaWin.ValReturn == null) return; // cancelo forma de pago
                            fPago = (DataTable)SiaWin.ValReturn;
                            //SiaWin.Browse(fPago);
                            //iddocumento = ExecuteSqlTransaction(_CodeCliente.ToString(), ctaban.ToString(), _abono);

                            ExecuteSqlTransaction(_CodeCliente.ToString(), _abono);
                            SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, moduloid, -1, -9, "GENERO RECIBO PROVISIONAL:" + TextRProv.Text, "");

                            ActivaDesactivaControles(0);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    else
                    {
                        dataGrid.Focus();
                    }
                }
            }
            catch (Exception exx)
            {
                SiaWin.Func.SiaExeptionGobal(exx);
                MessageBox.Show(exx.Message);
            }
        }
        private bool ValidaSaldosDoc()
        {
            try
            {
                StringBuilder errorMessages = new StringBuilder();
                foreach (System.Data.DataRow var in dtCue.Rows)
                {


                }
                if (errorMessages.ToString() != string.Empty)
                {
                    MessageBox.Show(errorMessages.ToString());
                    dataGrid.Focus();
                    dataGrid.SelectedIndex = 0;
                    return false;
                }
            }
            catch (Exception e)
            {
                SiaWin.Func.SiaExeptionGobal(e);
                MessageBox.Show(e.Message);
            }
            return true;
        }
        private void BtbCancelar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BtbCancelar.Content.ToString() == "Cancelar")
                {
                    if (dtCue.Rows.Count > 0)
                    {
                        if (MessageBox.Show("Usted desea cancelar este documento..?", "Cancelar Recibo de Caja", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                        {
                            e.Handled = true;
                            return;
                        }
                    }
                    ActivaDesactivaControles(0);
                    BtbGrabar.Focus();
                    e.Handled = true;
                    return;
                }
                else
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                SiaWin.Func.SiaExeptionGobal(ex);
                MessageBox.Show(ex.Message);
            }
        }
        private bool IsNumberKey(Key inKey)
        {
            if (inKey < Key.D0 || inKey > Key.D9)
            {
                if (inKey < Key.NumPad0 || inKey > Key.NumPad9)
                {
                    return false;
                }
            }
            return true;
        }
        private bool IsDelOrBackspaceOrTabKey(Key inKey)
        {
            return inKey == Key.Delete || inKey == Key.Back || inKey == Key.Tab || inKey == Key.Up || inKey == Key.Left || inKey == Key.Right || inKey == Key.Up || inKey == Key.Down || inKey == Key.Home || inKey == Key.End;
        }
        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {


            GridCurrencyColumn xx = ((SfDataGrid)sender).CurrentColumn as GridCurrencyColumn;
            if (xx.MappingName != "vlrabono") return;
            if (e.Key == Key.F8)
            {
                System.Data.DataRow dr = dtCue.Rows[dataGrid.SelectedIndex];
                if (dr != null)
                {
                    dr.BeginEdit();
                    dr["abono"] = Convert.ToDouble(dr["saldo"].ToString());
                    dr.EndEdit();
                    double _abono = Convert.ToDouble(dr["abono"].ToString());
                    double _saldo = Convert.ToDouble(dr["saldo"].ToString());
                    if (_abono > _saldo)
                    {
                        MessageBox.Show("Valor abono es mayor al valor del saldo...");
                        dr.BeginEdit();
                        dr["abono"] = 0;
                        dr.EndEdit();
                    }
                    dataGrid.UpdateLayout();
                    sumaAbonos();
                }
                //uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));
            }
        }
        private void dataGrid_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.SelectionChangedEventArgs e)
        {
            if (dataGrid.SelectedItem == null) return;
        }
        private void Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                TextBox s = e.Source as TextBox;
                if (s != null)
                {
                    s.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    e.Handled = true;
                }
            }
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (dtCue.Rows.Count > 0) e.Cancel = true;
        }
        private void CmbTipoDoc_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ComboBox cs = e.Source as ComboBox;
                if (cs != null)
                {
                    if (cs.SelectedIndex >= 0) cs.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }
                base.OnPreviewKeyDown(e);
            }
        }

        private void ExecuteSqlTransaction(string codter, double abonoBco)
        {
            try
            {


                string sqlcabeza = "";
                string sqlcuerpo = "";
                string sqlformaspago = "";



                if (string.IsNullOrEmpty(cnEmp))
                {
                    MessageBox.Show("Error - Cadena de Conexion nulla"); return;
                }

                using (SqlConnection connection = new SqlConnection(cnEmp))
                {
                    connection.Open();
                    StringBuilder errorMessages = new StringBuilder();
                    SqlCommand command = connection.CreateCommand();
                    SqlTransaction transaction;
                    transaction = connection.BeginTransaction("Transaction");
                    command.Connection = connection;
                    command.Transaction = transaction;


                    sqlcabeza += "INSERT INTO cocabrcpv (rcprov,cod_ven,cod_cli,rte_fte,rte_ica,rte_iva,rte_tim,mypag,mnpag,antic,vr_rec,fecha,cod_bod,dto_imal,dto_incol,dto_tmk,dto_gab,dto_vcd,dto_sic,dto_ot,fec_aded) " +
                        "VALUES ('" + TextRProv.Text + "','" + CmbVen.SelectedValue + "','" + TextCodeCliente.Text + "'," + TextRetefte.Value + "," + TextIca.Value + "," + TextReteIva.Value + ",0," + TextMayorVlr.Value + "," + TextMenorVlr.Value + "," + TextAnticipo.Value + "," + abonoBco + ",'" + TextFecha.Text + "','" + TxtBod.Text + "'," + ReturnValue(TxtBDtoImal.Text) + "," + ReturnValue(TxtBDtoIncol.Text) + "," + ReturnValue(TxtBDtoTmk.Text) + "," + ReturnValue(TxtBDtoGab.Text) + "," + ReturnValue(TxtBDtoVcd.Text) + "," + ReturnValue(TxtBDtoSic.Text) + "," + ReturnValue(TxtBDtoOt.Text) + ",'" + DateTime.Now.ToString() + "');";

                    foreach (System.Data.DataRow row in dtCue.Rows)
                    {
                        double abono = Convert.ToDouble(row["abono"].ToString());

                        if (abono > 0)
                        {

                            sqlcuerpo += "insert into cocuercpv (rcprov,cod_trn,num_trn,cod_cta,vr_abono,doc_ref,cod_ven,dto_imal,dto_incol,dto_tmk,dto_gab,dto_vcd,dto_sic,dto_ot,fec_aded) values " +
                                "('" + TextRProv.Text + "','" + row["cod_trn"].ToString().Trim() + "','" + row["num_trn"].ToString().Trim() + "','" + row["cod_cta"].ToString().Trim() + "'," + abono + ",'','" + CmbVen.SelectedValue + "'," + Convert.ToDecimal(row["dto_imal"]) + "," + Convert.ToDecimal(row["dto_incol"]) + "," + Convert.ToDecimal(row["dto_tmk"]) + "," + Convert.ToDecimal(row["dto_gab"]) + "," + Convert.ToDecimal(row["dto_vcd"]) + "," + Convert.ToDecimal(row["dto_sic"]) + "," + Convert.ToDecimal(row["dto_ot"]) + ",'" + DateTime.Now.ToString() + "');";
                        }
                    }

                    foreach (System.Data.DataRow row in fPago.Rows)
                    {

                        string value = row["valor"].ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            double abono = Convert.ToDouble(row["valor"].ToString());
                            if (abono > 0)
                            {

                                string cod_ban = row["cod_ban"].ToString().Trim();
                                string fec_venc = cod_ban == "45" || cod_ban == "50" ? row["fec_venc"].ToString().Trim() : " ";
                                string fec_consig = cod_ban == "45" || cod_ban == "50" ? " " : row["fec_con"].ToString().Trim();
                                string codigo_banco = cod_ban == "45" || cod_ban == "50" ? row["cod_banco"].ToString().Trim() : " ";

                                sqlformaspago += "insert into cofpagrpv(rcprov, cod_ban, nom_ban, pagado, dias, fec_ven, doc_ref, cod_banc, fec_con, cod_cta, cod_ven, fec_aded) " +
                                    "values('" + TextRProv.Text + "', '" + row["cod_ban"].ToString().Trim() + "', '" + row["nom_ban"].ToString().Trim() + "', " + Convert.ToDecimal(row["valor"]) + ", '', '" + fec_venc + "', '" + row["documento"].ToString().Trim() + "', '" + codigo_banco + "', '" + fec_consig + "', '" + row["cod_cta"].ToString().Trim() + "', '" + CmbVen.SelectedValue + "','" + DateTime.Now.ToString() + "');";

                            }
                        }
                    }


                    var r = new object();
                    command.CommandText = sqlcabeza + sqlcuerpo + sqlformaspago;
                    //MessageBox.Show(command.CommandText.ToString());
                    r = command.ExecuteScalar();
                    transaction.Commit();
                    MessageBox.Show("Recivo provisional guardado");
                    connection.Close();
                }
            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error al guardar:" + w);
            }

        }


        public decimal ReturnValue(string text)
        {
            var valor = text;
            decimal value = decimal.Parse(valor, NumberStyles.Currency);
            return value;
        }


        ////// CONSULTA DE TRASLADOS
        private void LoadData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(cnEmp))
                {
                    connection.Open();
                    //connectionString.Open();
                    //DataSet ds = new DataSet();
                    StringBuilder _sql = new StringBuilder();
                    ds.Clear();
                    //_sql.Append("select cab.cod_trn,cab.num_trn,cab.fec_trn,cab.bod_tra,cab.bod_tra+'-'+bod.ini_bod as bod_dest,cue.cod_bod,cue.cod_ref,rtrim(ref.nom_ref) as nom_ref,rtrim(tip.nom_tip) as nom_tip,iif(trn.tip_trn=1,cue.cantidad,-cue.cantidad) as cantidad,trn.tip_trn,iif(cab.tip_traslado=0,'Tienda',iif(cab.tip_traslado=1,'GerenteProducto',iif(cab.tip_traslado=2,'GerenteAdmon','Ninguno'))) as tipotraslado,cab.idreg from incue_doc as cue ");
                    // _sql.Append(" inner join incab_doc as cab on cab.idreg = cue.idregcab inner join inmae_ref as ref on ref.cod_ref = cue.cod_ref inner join inmae_bod as bod on bod.cod_bod = cab.bod_tra ");
                    // _sql.Append(" inner join inmae_trn as trn on trn.cod_trn=cab.cod_trn inner join inmae_tip as tip on tip.cod_tip =ref.cod_tip where convert(date,cab.fec_trn) between '" + FechaIni.Text + "' and '" + FechaFin.Text + "' and (cab.cod_trn = '051' or cab.cod_trn = '141')");
                    //_sql.Append(" and cue.cod_bod = '" + codbod.Trim() + "' order by cab.fec_trn ");

                    _sql.Append("select cod_cli,nom_ter,cocabrcpv.cod_ven,nom_mer,rte_fte,rte_ica,rte_iva,rte_tim,antic,mypag,mnpag,cocabrcpv.cod_bod ");
                    _sql.Append("from cocabrcpv ");
                    _sql.Append("inner join comae_ter on cocabrcpv.cod_cli = comae_ter.cod_ter ");
                    _sql.Append("inner join inmae_mer on cocabrcpv.cod_ven = inmae_mer.cod_mer ");
                    _sql.Append("where fecha between '" + FechaIni.Text + "' and '" + FechaFin.Text + " 23:59:59'  ");

                    SqlDataAdapter adapter = new SqlDataAdapter(_sql.ToString(), connection);
                    adapter.Fill(ds, "RCaja");
                    dataGridSF.ItemsSource = ds.Tables["RCaja"];
                    Tx_reg.Text = ds.Tables["RCaja"].Rows.Count.ToString();
                    double totcxc = 0;
                    double totant = 0;
                    //double.TryParse(ds.Tables["RCaja"].Compute("Sum(valor)", "tip_apli=3").ToString(), out totcxc);
                    //double.TryParse(ds.Tables["RCaja"].Compute("Sum(valor)", "tip_apli=4").ToString(), out totant);


                }
            }
            catch (Exception ex)
            {
                SiaWin.Func.SiaExeptionGobal(ex);
                MessageBox.Show(ex.Message);
            }
        }
        private void Ejecutar_Click(object sender, RoutedEventArgs e)
        {
            // validar fecha
            LoadData();
        }
        private void ReImprimir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Data.DataRow dr = ds.Tables["RCaja"].Rows[dataGridSF.SelectedIndex];
                if (dr != null)
                {
                    string numtrn = dr["idreg"].ToString();
                    string codterc = dr["cod_ter"].ToString();
                    //                  MessageBox.Show(codterc);
                    ImprimeDocumento(Convert.ToInt32(numtrn), codterc);
                    //ImprimirDoc(Convert.ToInt32(numtrn), "Reimp");
                }
            }
            catch (Exception ex)
            {
                SiaWin.Func.SiaExeptionGobal(ex);
                MessageBox.Show(ex.Message);
            }
        }
        private void ExportaXLS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;

                SaveFileDialog sfd = new SaveFileDialog
                {
                    FilterIndex = 2,
                    Filter = "Excel 97 to 2003 Files(*.xls)|*.xls|Excel 2007 to 2010 Files(*.xlsx)|*.xlsx|Excel 2013 File(*.xlsx)|*.xlsx"
                };
                if (sfd.ShowDialog() == true)
                {
                    using (Stream stream = sfd.OpenFile())
                    {
                        //                    if (sfd.FilterIndex == 1)
                        ////                        workBook.Version = ExcelVersion.Excel97to2003;
                        //                    else if (sfd.FilterIndex == 2)
                        //                        workBook.Version = ExcelVersion.Excel2010;
                        //                    else
                        //                        workBook.Version = ExcelVersion.Excel2013;
                        //                    workBook.SaveAs(stream);
                    }
                    //Message box confirmation to view the created workbook.
                    if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archvo", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        //Launching the Excel file using the default Application.[MS Excel Or Free ExcelViewer]
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error al exportar:" + w);
            }
        }
        private void ImprimirDoc(int idregcab, string tipoImp)
        {
            string[] strArrayParam = new string[] { idregcab.ToString(), idemp.ToString(), tipoImp };
            SiaWin.Tab(9291, strArrayParam);
            //((Inicio)Application.Current.MainWindow).Tab(9279);832005853
            //if(usercontrol.Tag.ToString()=="-1")
            //{
            // ((Inicio)Application.Current.MainWindow).Tab(9279);
            //MessageBox.Show("ddd");
            //   e.Handled = true;
            // return;
            //}
        }
        private void TextCodeCliente_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                return;
            }
            if ((e.Key == Key.Enter || e.Key == Key.Return || e.Key == Key.Tab))
            {
                TextBox s = e.Source as TextBox;
                if (s != null)
                {
                    s.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    e.Handled = true;
                }
            }
        }
        private void ConsultaSaldoCartera()
        {

            SqlConnection con = new SqlConnection(SiaWin._cn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet ds1 = new DataSet();
            //cmd = new SqlCommand("ConsultaCxcCxpDeta", con);
            cmd = new SqlCommand("_empSpCoAnalisisCxc", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Ter", TextCodeCliente.Text.Trim());//if you have parameters.
            cmd.Parameters.AddWithValue("@Cta", "");//if you have parameters.                                                    
            cmd.Parameters.AddWithValue("@TipoApli", -1);//if you have parameters. 1=cxc
            cmd.Parameters.AddWithValue("@Resumen", 1);//if you have parameters.
            cmd.Parameters.AddWithValue("@Fecha", TextFecha.Text);//if you have parameters.
            cmd.Parameters.AddWithValue("@TrnCo", "");//if you have parameters.
            cmd.Parameters.AddWithValue("@NumCo", "");//if you have parameters.
            cmd.Parameters.AddWithValue("@Cco", "");//if you have parameters.
            cmd.Parameters.AddWithValue("@codemp", BusinessCode);
            dtCue.Clear();

            //JESUS
            da = new SqlDataAdapter(cmd);
            da.Fill(dtCue);
            con.Close();

            //SiaWin.Browse(dtCue);

            if (dtCue.Rows.Count == 0)
            {

                MessageBox.Show("Sin informacion de cartera");
                dataGrid.ItemsSource = null;
                TextCodeCliente.Text = "";
                TextNomCliente.Text = "";
                //return;
            }
            try
            {
                sumaTotal();
                dataGrid.ItemsSource = dtCue.DefaultView;
            }
            catch (Exception W)
            {
                SiaWin.Func.SiaExeptionGobal(W);
                MessageBox.Show("Actualiza Grid www:" + W);
            }



        }
        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                if (dataGrid.AllowEditing == true) return;
                if (e.Column.Header.ToString().Trim() == "Vlr Abono")
                {
                    if (e.EditAction == DataGridEditAction.Commit)
                    {
                        System.Data.DataRow dr = dtCue.Rows[dataGrid.SelectedIndex];
                        if (dr != null)
                        {
                            double _abono = Convert.ToDouble(dr["abono"].ToString());
                            double _saldo = Convert.ToDouble(dr["saldo"].ToString());
                            if (_abono > _saldo)
                            {
                                MessageBox.Show("Valor abono es mayor al valor del saldo...");
                                dr.BeginEdit();
                                dr["abono"] = 0;
                                dr.EndEdit();
                            }
                            dataGrid.UpdateLayout();
                            sumaAbonos();
                        }
                    }
                }
            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error:" + w);
            }
        }
        private void sumaTotal()
        {
            double.TryParse(dtCue.Compute("Sum(valor)", "tip_apli=3").ToString(), out valorCxC);
            double.TryParse(dtCue.Compute("Sum(valor)", "tip_apli=4").ToString(), out valorCxCAnt);
            double.TryParse(dtCue.Compute("Sum(valor)", "tip_apli=1").ToString(), out valorCxP);
            double.TryParse(dtCue.Compute("Sum(valor)", "tip_apli=2").ToString(), out valorCxPAnt);
            double.TryParse(dtCue.Compute("Sum(saldo)", "tip_apli=3").ToString(), out saldoCxC);
            double.TryParse(dtCue.Compute("Sum(saldo)", "tip_apli=4").ToString(), out saldoCxCAnt);
            double.TryParse(dtCue.Compute("Sum(saldo)", "tip_apli=1").ToString(), out saldoCxP);
            double.TryParse(dtCue.Compute("Sum(saldo)", "tip_apli=2").ToString(), out saldoCxPAnt);

            //double valorA = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(valor)", "tip_apli=1 or tip_apli=4").ToString());
            //double saldo = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(saldo)", "tip_apli=2 or tip_apli=3").ToString());
            TextCxC.Text = saldoCxC.ToString("C");
            TextCxCAnt.Text = saldoCxCAnt.ToString("C");
            TextCxP.Text = saldoCxP.ToString("C");
            TextCxPAnt.Text = saldoCxPAnt.ToString("C");
            //TextCxCAbono.Text = (valorCxC - saldoCxC).ToString("C");
            //TextCxCAntAbono.Text = (valorCxCAnt - saldoCxCAnt).ToString("C");
            TextCxCSaldo.Text = saldoCxC.ToString("C");
            TextCxCAntSaldo.Text = saldoCxCAnt.ToString("C");
            TotalCxc.Text = (valorCxC - valorCxCAnt - valorCxP + valorCxPAnt).ToString("C");
            TotalSaldo.Text = (saldoCxC - saldoCxCAnt - saldoCxP + saldoCxPAnt).ToString("C");
        }
        private void sumaAbonos()
        {
            try
            {


                double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=3").ToString(), out abonoCxC);
                double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=4").ToString(), out abonoCxCAnt);
                double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=1").ToString(), out abonoCxP);
                double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=2").ToString(), out abonoCxPAnt);
                double.TryParse(dtCue.Compute("Sum(dto_imal)", "tip_apli=3").ToString(), out dtosImal);
                double.TryParse(dtCue.Compute("Sum(dto_incol)", "tip_apli=3").ToString(), out dtosIncol);
                double.TryParse(dtCue.Compute("Sum(dto_tmk)", "tip_apli=3").ToString(), out dtosTmk);
                double.TryParse(dtCue.Compute("Sum(dto_gab)", "tip_apli=3").ToString(), out dtosGab);
                double.TryParse(dtCue.Compute("Sum(dto_vcd)", "tip_apli=3").ToString(), out dtosVcd);
                double.TryParse(dtCue.Compute("Sum(dto_sic)", "tip_apli=3").ToString(), out dtosSic);
                double.TryParse(dtCue.Compute("Sum(dto_ot)", "tip_apli=3").ToString(), out dtosOt);
                Retefte = Convert.ToDouble(TextRetefte.Value);
                Reteica = Convert.ToDouble(TextIca.Value);
                Reteiva = Convert.ToDouble(TextReteIva.Value);
                Mayorvlr = Convert.ToDouble(TextMayorVlr.Value);
                Menorvlr = Convert.ToDouble(TextMenorVlr.Value);
                Anticipo = Convert.ToDouble(TextAnticipo.Value);
                TextCxCAbono.Text = abonoCxC.ToString("C");
                TextCxCAntAbono.Text = abonoCxCAnt.ToString("C");
                TextCxPAbono.Text = abonoCxP.ToString("C");
                TextCxPAntAbono.Text = abonoCxPAnt.ToString("C");
                TxtBDtoImal.Text = dtosImal.ToString("C");
                TxtBDtoIncol.Text = dtosIncol.ToString("C");
                TxtBDtoTmk.Text = dtosTmk.ToString("C");
                TxtBDtoGab.Text = dtosGab.ToString("C");
                TxtBDtoVcd.Text = dtosVcd.ToString("C");
                TxtBDtoSic.Text = dtosSic.ToString("C");
                TxtBDtoOt.Text = dtosOt.ToString("C");
                TextCxCSaldo.Text = (saldoCxC - abonoCxC).ToString("C");

                TextCxCAntSaldo.Text = (saldoCxCAnt - abonoCxCAnt).ToString("C");
                TextCxPSaldo.Text = (saldoCxP - abonoCxP).ToString("C");
                TextCxPAntSaldo.Text = (saldoCxPAnt - abonoCxPAnt).ToString("C");
                TotalCxc.Text = (valorCxC - valorCxCAnt - valorCxP + valorCxPAnt).ToString("C");
                TotalAbono.Text = (abonoCxC - abonoCxCAnt - abonoCxP + abonoCxPAnt).ToString("C");
                TotalSaldo.Text = ((valorCxC - valorCxCAnt - valorCxP + valorCxPAnt) - (abonoCxC - abonoCxCAnt - abonoCxP + abonoCxPAnt)).ToString("C"); ;
                TotalRecaudo.Text = (abonoCxC - abonoCxCAnt - abonoCxP + abonoCxPAnt + Anticipo + Mayorvlr - Retefte - Reteica - Reteiva - Menorvlr - dtosImal - dtosIncol - dtosTmk - dtosGab - dtosVcd - dtosSic - dtosOt).ToString("C");

            }
            catch (Exception W)
            {
                MessageBox.Show("sUMA DE ABONOS www:" + W);
            }
        }
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (BtbGrabar.Content.ToString().Trim() == "Nuevo") return;
            if (e.Key == Key.F5 && BtbGrabar.Content.ToString().Trim() == "Grabar")
            {
                if (e.Key == System.Windows.Input.Key.F5)
                {
                    BtbGrabar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    return;
                }
            }
            if (e.Key == Key.F9)
            {
                if (dtCue.Rows.Count > 0)
                {
                    if (MessageBox.Show("Usted desea cruzar todos los documentos ?", "Cruzar pagos", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.No) return;
                    foreach (System.Data.DataRow dr in dtCue.Rows)
                    {
                        double _saldo = Convert.ToDouble(dr["saldo"].ToString());
                        dr.BeginEdit();
                        dr["abono"] = _saldo;
                        dr.EndEdit();
                    }
                    dataGrid.UpdateLayout();
                    sumaAbonos();
                    dataGrid.Focus();
                    dataGrid.SelectedIndex = 0;
                    //    dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.Items[0], dataGrid.Columns[8]);
                }
            }
            if (e.Key == Key.F6)
            {
                if (dtCue.Rows.Count > 0)
                {
                    if (MessageBox.Show("Usted desea cancelar abonos .... ?", "Cancela Cruces de pagos", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.No) return;
                    foreach (System.Data.DataRow dr in dtCue.Rows)
                    {
                        dr.BeginEdit();
                        dr["abono"] = 0;
                        dr.EndEdit();
                    }
                    dataGrid.UpdateLayout();
                    sumaAbonos();
                    dataGrid.Focus();
                    dataGrid.SelectedIndex = 0;
                    //      dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.Items[0], dataGrid.Columns[8]);
                }
            }

            if (e.Key == Key.Escape)
            {
                if (BtbGrabar.Content.ToString().Trim() == "Grabar")
                {
                    BtbCancelar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    e.Handled = false;
                    return;
                }
            }
        }
        private bool ActualizaCampos(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id)) return false;
                SqlDataReader dr = SiaWin.Func.SqlDR("SELECT idrow,cod_ter,nom_ter,dir1,tel1,observ FROM comae_ter where cod_ter='" + Id.ToString() + "' ", idemp);
                int idrow = 0;
                //string codter = "";
                //string nomter = "";
                while (dr.Read())
                {
                    idrow = Convert.ToInt32(dr["idrow"]);
                    codter = dr["cod_ter"].ToString();
                    nomter = dr["nom_ter"].ToString();
                    dirter = dr["dir1"].ToString();
                    telter = dr["tel1"].ToString();
                    TextNomCliente.Text = nomter;
                }
                dr.Close();
                if (idrow == 0) return false;
                if (idrow > 0) return true;
            }
            catch (System.Exception _error)
            {
                SiaWin.Func.SiaExeptionGobal(_error);
                MessageBox.Show(_error.Message);
            }
            return false;
        }
        private void TextCodeCliente_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BtbCancelar.Content.ToString().Trim() == "Salir") return;

                TextBox textbox = ((TextBox)sender);
                if (textbox.Text.Trim() == "")
                {
                    int idr = 0; string code = ""; string nombre = "";
                    dynamic xx = SiaWin.WindowBuscar("comae_ter", "cod_ter", "nom_ter", "nom_ter", "idrow", "Maestra de clientes", cnEmp, false, "", idEmp: idemp);
                    xx.ShowInTaskbar = false;
                    xx.Owner = Application.Current.MainWindow;
                    xx.ShowDialog();
                    idr = xx.IdRowReturn;
                    code = xx.Codigo;
                    nombre = xx.Nombre;
                    xx = null;
                    if (idr > 0)
                    {
                        TextCodeCliente.Text = code;
                        TextNomCliente.Text = nombre;
                    }
                    if (string.IsNullOrEmpty(code)) e.Handled = false;
                    if (!string.IsNullOrEmpty(TextCodeCliente.Text.Trim())) TextCodeCliente.Focusable = false;
                    if (string.IsNullOrEmpty(code)) return;
                    ConsultaSaldoCartera();
                }
                else
                {
                    if (!ActualizaCampos(textbox.Text.Trim()))
                    {
                        MessageBox.Show("El codigo de tercereo:" + textbox.Text.Trim() + " no existe");
                        textbox.Text = "";
                    }
                    else
                    {
                        ConsultaSaldoCartera();
                        if (!string.IsNullOrEmpty(TextCodeCliente.Text.Trim())) TextCodeCliente.Focusable = false;
                    }
                }
                if (TextCodeCliente.Text.Trim().Length == 0)
                {
                    textbox.Dispatcher.BeginInvoke((Action)(() => { textbox.Focus(); }));
                    //e.Handled = true;
                    return;
                }
            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("ERROR LOSFOCUSTERCERO:" + w);
            }
        }


        public bool validarReciboProvi(string texto)
        {
            bool bandera = true;
            try
            {

                if (CmbVen.SelectedIndex <= -1) return false;



                if (CmbVen.SelectedValue.ToString().Trim() == "A1" || CmbVen.SelectedValue.ToString().Trim() == "A2")
                {
                    if (string.IsNullOrEmpty(texto) || texto == "") bandera = true;
                }
                else
                {
                    string valor = TextRProv.Text;
                    string query = "select * from cotalon_rc where '" + valor + "' between desde and hasta";
                    DataTable dt = SiaWin.Func.SqlDT(query, "table", idemp);
                    //SiaWin.Browse(dt);

                    if (dt.Rows.Count > 0)
                    {
                        string VenTabla = dt.Rows[0]["cod_ven"].ToString().Trim().ToUpper();
                        string VenSele = CmbVen.SelectedValue.ToString().Trim().ToUpper();

                        if (VenTabla != VenSele)
                        {
                            MessageBox.Show("este recibo provisional le pertenece a otro vendedor:" + VenTabla);
                            TextRProv.Text = "";
                            bandera = false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("El recibo provisional no existe");
                        TextRProv.Text = "";
                        bandera = false;
                    }
                }
            }
            catch (Exception W)
            {
                SiaWin.Func.SiaExeptionGobal(W);
                MessageBox.Show("ERROR EN LA CONSULTA cotalon_rc:" + W);
            }
            return bandera;
        }




        private void TextRProv_LostFocus(object sender, RoutedEventArgs e)
        {
            string texto = (sender as TextBox).Text;

            if (CmbVen.SelectedIndex <= 0)
            {
                MessageBox.Show("seleccione el Vendedor Recaudor ");
                return;
            }


            if (validarReciboProvi(texto) == false) return;

            if (ValExistReciboProv(TextRProv.Text) == true)
            {
                MessageBox.Show("el recibo provicional ya existe");
                return;
            }

            if (ValExistReciboConta(TextRProv.Text) == true)
            {
                MessageBox.Show("el recibo provicional ya existe en contabilidad");
                return;
            }


        }


        private void TextRProv_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            string nameControl = (sender as TextBox).Name;

            if (Name == "TextRProv")
            {
                if (e.Key == Key.OemMinus || e.Key == Key.Subtract || e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 || e.Key == Key.Back || e.Key == Key.Left || e.Key == Key.Right)
                    e.Handled = false;
                else
                    MessageBox.Show("este campo solo admite valores numericos");
                e.Handled = true;
            }

        }

        private void dataGrid_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {

        }
        private void dataGrid_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {

            try
            {


                if (e.Key == Key.F8)
                {
                    GridNumericColumn Colum = ((SfDataGrid)sender).CurrentColumn as GridNumericColumn;
                    if (Colum.MappingName == "abono" || Colum.MappingName == "dto_imal" || Colum.MappingName == "dto_incol" || Colum.MappingName == "dto_tmk" || Colum.MappingName == "dto_gab" || Colum.MappingName == "dto_vcd" || Colum.MappingName == "dto_sic" || Colum.MappingName == "dto_ot")
                    {
                        System.Data.DataRow dr = dtCue.Rows[dataGrid.SelectedIndex];
                        dr.BeginEdit();
                        VlrRecibido = Convert.ToDouble(TextVlrRecibido.Value);
                        double vrRecaudo = (abonoCxC - abonoCxCAnt - abonoCxP + abonoCxPAnt + Anticipo + Mayorvlr - Retefte - Reteica - Reteiva - Menorvlr - dtosImal - dtosIncol - dtosTmk - dtosGab - dtosVcd - dtosSic - dtosOt);
                        VlrRecibido = VlrRecibido - vrRecaudo;

                        double _cnt = Convert.ToDouble(dr["saldo"].ToString());
                        if (VlrRecibido >= _cnt)
                            dr["abono"] = _cnt;
                        else
                            dr["abono"] = VlrRecibido;


                        dr.EndEdit();
                        e.Handled = true;
                    }
                    dataGrid.UpdateLayout();

                    sumaAbonos();
                }

            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("11: F8" + w);
            }
        }
        private void dataGrid_CurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs e)
        {
            try
            {



                GridNumericColumn Colum = ((SfDataGrid)sender).CurrentColumn as GridNumericColumn;

                if (Colum.MappingName == "abono" || Colum.MappingName == "dto_imal" || Colum.MappingName == "dto_incol" || Colum.MappingName == "dto_tmk" || Colum.MappingName == "dto_gab" || Colum.MappingName == "dto_vcd" || Colum.MappingName == "dto_sic" || Colum.MappingName == "dto_ot")
                {

                    System.Data.DataRow dr = dtCue.Rows[dataGrid.SelectedIndex];
                    decimal _saldo = Convert.ToDecimal(dr["saldo"].ToString());
                    decimal _abono = Convert.ToDecimal(dr["abono"].ToString());
                    if (_abono > _saldo)
                    {
                        MessageBox.Show("El valor abonado es mayor al saldo...");
                        dr.BeginEdit();
                        dr["abono"] = 0;
                        dr.EndEdit();

                    }


                    dataGrid.UpdateLayout();

                    sumaAbonos();
                }
            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("22:" + w);
            }
        }

        private void ImprimeDocumento(int iddoc, string codter)
        {
            if (!ActualizaCampos(codter)) return;
            regcab = iddoc;
            PrintDocument pd = new PrintDocument();
            System.Drawing.Printing.PaperSize ps = new PaperSize("", 475, 550);
            pd.PrintPage += new PrintPageEventHandler(pd_imprimefactura);
            pd.PrintController = new StandardPrintController();
            pd.DefaultPageSettings.Margins.Left = 0;
            pd.DefaultPageSettings.Margins.Right = 0;
            pd.DefaultPageSettings.Margins.Top = 0;
            pd.DefaultPageSettings.Margins.Bottom = 0;
            pd.DefaultPageSettings.PaperSize = ps;
            System.Windows.Controls.PrintDialog printDialog1 = new System.Windows.Controls.PrintDialog();
            System.Windows.Forms.PrintPreviewDialog printPreviewDialog1 = new System.Windows.Forms.PrintPreviewDialog();
            printPreviewDialog1.Width = 400;
            printPreviewDialog1.Height = 600;
            printPreviewDialog1.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            printPreviewDialog1.Document = pd;
            printPreviewDialog1.ShowDialog();

            //pd.Print();
        }
        //********** IMPRIME FACTURAS
        private void pd_imprimefactura(object sender, PrintPageEventArgs e)
        {
            try
            {

                //trae cabeza
                SqlDataReader dr = SiaWin.Func.SqlDR("SELECT * from cocab_doc where idreg=" + regcab.ToString(), idemp);
                if (dr == null)
                {
                    MessageBox.Show("Documento no existe.....");
                    return;
                }
                string trn = "";
                string num = "";
                string fecha = "";
                string detalle = "";
                double totalrecaudo = abonoCxC - abonoCxCAnt - abonoCxP + abonoCxPAnt;
                while (dr.Read())
                {
                    trn = dr["cod_trn"].ToString();
                    num = dr["num_trn"].ToString();
                    fecha = dr["fec_trn"].ToString();
                    detalle = dr["detalle"].ToString().Trim();
                }
                dr.Close();
                SqlDataReader drCue = SiaWin.Func.SqlDR("SELECT cocue_doc.cod_cta,des_mov,doc_cruc,deb_mov,cre_mov,rtrim(comae_cta.nom_cta) as nom_cta from cocue_doc inner join comae_cta on comae_cta.cod_cta=cocue_doc.cod_cta where cocue_doc.idregcab=" + regcab.ToString(), idemp);
                if (drCue == null)
                {
                    MessageBox.Show("Documento no existe.....");
                    return;
                }
                string rowValue1 = "";
                int pos1 = 10;
                string pathlogo = SiaWin._PathApp + @"\imagenes\" + idLogo.ToString() + "..png";
                //Image newImage = Image.FromFile("SampImag.jpg");
                // Create rectangle for displaying image.
                Rectangle destRect = new Rectangle(100, 100, 50, 50);
                // Create coordinates of rectangle for source image.                
                // Draw image to screen.
                //e.Graphics.DrawImage(System.Drawing.Image.FromFile(pathlogo), destRect, x, y, width, height, units);
                e.Graphics.DrawImage(System.Drawing.Image.FromFile(pathlogo), 100, 5, 70, 70);
                //string nompvta = nompvta; SiaWin.Func.cmpCodigo("copventas", "cod_pvt", "nom_pvt", codpvta, idemp);
                System.Drawing.Graphics g = e.Graphics;
                //g.DrawImage(System.Drawing.Image.FromFile(pathlogo), 1, 7);
                System.Drawing.Font fTitulo = new System.Drawing.Font("Lucida Console", 12, System.Drawing.FontStyle.Bold);
                System.Drawing.Font fCAB = new System.Drawing.Font("Lucida Console", 7, System.Drawing.FontStyle.Bold);
                System.Drawing.Font fBody = new System.Drawing.Font("Lucida Console", 7, System.Drawing.FontStyle.Regular);
                System.Drawing.Font fBody1 = new System.Drawing.Font("Lucida Console", 7, System.Drawing.FontStyle.Bold);
                pos1 = pos1 + 80;
                System.Drawing.SolidBrush sb = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
                g.DrawString("RECIBO DE CAJA - CREDITOS", fTitulo, sb, 1, pos1);
                pos1 = pos1 + 12;
                g.DrawString("----------------------------------------------", fBody, sb, 1, pos1);
                pos1 = pos1 + 12;
                g.DrawString(TxtEmpresa.Text.Trim(), fTitulo, sb, 1, pos1);
                pos1 = pos1 + 20;
                g.DrawString("Nit:" + nitemp, fCAB, sb, 1, pos1);
                pos1 = pos1 + 12;
                g.DrawString("Tienda:" + codpvta + "-" + nompvta.Trim(), fCAB, sb, 1, pos1);
                pos1 = pos1 + 10;
                g.DrawString("----------------------------------------------", fBody, sb, 1, pos1);
                pos1 = pos1 + 10;
                g.DrawString("RECIBO CAJA No..:", fBody, sb, 1, pos1);
                g.DrawString(num, fBody1, sb, 105, pos1);
                pos1 = pos1 + 10;
                g.DrawString("FECHA           :", fBody, sb, 1, pos1);
                g.DrawString(fecha, fCAB, sb, 105, pos1);
                pos1 = pos1 + 10;
                //MessageBox.Show(TotalRecaudo.Text.ToString());
                //decimal xval = Convert.ToDecimal(TotalRecaudo.Text.ToString());
                //string valo = xval.ToString("C2");

                g.DrawString("VALOR PAGO/ABONO:", fBody, sb, 1, pos1);
                g.DrawString(totalrecaudo.ToString("C2"), fCAB, sb, 105, pos1);
                pos1 = pos1 + 10;

                g.DrawString("----------------------------------------------", fBody, sb, 1, pos1);
                pos1 = pos1 + 10;
                g.DrawString("CLIENTE  :", fBody, sb, 1, pos1);
                g.DrawString(nomter.Trim(), fBody, sb, 60, pos1);
                pos1 = pos1 + 10;
                g.DrawString("NIT/C.C  :", fBody, sb, 1, pos1);
                g.DrawString(codter, fBody, sb, 60, pos1);
                pos1 = pos1 + 10;
                g.DrawString("DIRECCION:", fBody, sb, 1, pos1);
                g.DrawString(dirter.Trim(), fBody, sb, 60, pos1);
                pos1 = pos1 + 10;
                g.DrawString("TELEFONO :", fBody, sb, 1, pos1);
                g.DrawString(telter.Trim(), fBody, sb, 60, pos1);
                pos1 = pos1 + 10;
                g.DrawString("USUARIO  :", fBody, sb, 1, pos1);
                g.DrawString(SiaWin._UserAlias, fBody, sb, 60, pos1);
                pos1 = pos1 + 10;
                g.DrawString("----------------------------------------------", fBody, sb, 1, pos1);
                pos1 = pos1 + 10;
                g.DrawString("DETALLE                              VALOR    ", fBody, sb, 1, pos1);
                pos1 = pos1 + 10;
                g.DrawString("----------------------------------------------", fBody, sb, 1, pos1);
                pos1 = pos1 + 10;
                while (drCue.Read())
                {
                    decimal valdeb = Convert.ToDecimal(drCue["deb_mov"].ToString());
                    decimal valcre = Convert.ToDecimal(drCue["cre_mov"].ToString());
                    string tipocta = "D";
                    if (valcre > 0) tipocta = "C";
                    if (valcre > 0) rowValue1 = drCue["des_mov"].ToString().Substring(0, 30) + " " + tipocta + valcre.ToString("C2");
                    if (valdeb > 0) rowValue1 = drCue["des_mov"].ToString().Substring(0, 30) + " " + tipocta + valdeb.ToString("C2");
                    g.DrawString(rowValue1, fBody, sb, 1, pos1);
                    pos1 = pos1 + 10;
                }
                g.DrawString("----------------------------------------------", fBody, sb, 1, pos1);
                pos1 = pos1 + 35;
                g.DrawString("ELABORO :_____________________________ ", fBody, sb, 1, pos1);
                pos1 = pos1 + 35;
                g.DrawString("REVISADO:_____________________________ ", fBody, sb, 1, pos1);
                pos1 = pos1 + 25;
                g.DrawString(".", fBody, sb, 1, pos1);
            }
            catch (Exception ex)
            {
                SiaWin.Func.SiaExeptionGobal(ex);
                MessageBox.Show("Error Imprime Factura:" + ex.ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadInfo();
            if (string.IsNullOrEmpty(codter)) return;
            try
            {
                this.dataGrid.MoveCurrentCell(new RowColumnIndex(1, 8), false);
                if (!string.IsNullOrEmpty(codter))
                {
                    //MessageBox.Show("focus");
                    //CmbBan.Focus();
                }
                //CmbBan.Focus();
            }
            catch (Exception ex)
            {
                SiaWin.Func.SiaExeptionGobal(ex);
                MessageBox.Show(ex.Message.ToString());
            }

        }

        private void CmbVen_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            TextRProv.Text = "";
        }


        private void ActualizaTotal(object sender, RoutedEventArgs e)
        {
            sumaAbonos();
        }



    }
}




