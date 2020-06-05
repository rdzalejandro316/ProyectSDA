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
//using RecibosDeCaja;
using Syncfusion.UI.Xaml.Grid;
using System.Drawing.Printing;
using System.Drawing;
using Syncfusion.UI.Xaml.ScrollAxis;
using RecibosDeCaja;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Reporting.WinForms;

namespace SiasoftAppExt
{
    //RecibosdeCaja
    //Sia.PublicarPnt(9305,"RecibosDeCaja");

    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9305, "RecibosDeCaja);  //carga desde sql
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
    //ww.idemp="1";
    //ww.codpvta="003";
    //ww.codter="01";
    //ww.ShowDialog();
    public partial class RecibosDeCaja : Window
    {

        public bool isPuntoVen = false;

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


        public string fechaPublic = "";
        DataTable fPago = new DataTable();
        int regcab = 0;


        public bool is_reciboProv = false;


        public RecibosDeCaja()
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
                this.Title = "Recibo de caja - Empresa:" + BusinessCode + "-" + nomempresa;

                if (isPuntoVen == true)
                {

                    if (codpvta == string.Empty)
                    {
                        MessageBox.Show("El usuario no tiene asignado un punto de venta, Pantalla Bloqueada");
                        this.IsEnabled = false;
                    }
                    else
                    {
                        nompvta = SiaWin.Func.cmpCodigo("copventas", "cod_pvt", "nom_pvt", codpvta, idemp);
                        TxtPVenta.Text = codpvta + "-" + nompvta;
                        codbod = SiaWin.Func.cmpCodigo("copventas", "cod_pvt", "cod_bod", codpvta, idemp);
                        codcco = SiaWin.Func.cmpCodigo("copventas", "cod_pvt", "cod_cco", codpvta, idemp);
                        if (string.IsNullOrEmpty(codbod))
                        {
                            MessageBox.Show("El punto de venta Asignado no tiene bodega , Pantalla Bloqueada");
                        }
                        TxtBod.Text = codbod;
                    }
                }
                else
                {
                    nompvta = SiaWin.Func.cmpCodigo("copventas", "cod_pvt", "nom_pvt", codpvta, idemp);
                    TxtPVenta.Text = codpvta + "---" + nompvta;
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
            }
            catch (Exception e)
            {
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
                TextNumeroDoc.Text = string.Empty;
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

                TotalRecaudo.Text = "0,00";
                TextRetefte.Text = "0,00";
                TextIca.Text = "0,00";
                TextReteIva.Text = "0,00";
                TextVlrRecibido.Text = "0,00";
                TextMayorVlr.Text = "0,00";
                TextMenorVlr.Text = "0,00";
                TextAnticipo.Text = "0,00";
                TextCodeCliente.Focusable = false;
                TextRProv.Focusable = false;
                TxtBDtoImal.Text = "0,00";
                TxtBDtoIncol.Text = "0,00";
                TxtBDtoTmk.Text = "0,00";
                TxtBDtoGab.Text = "0,00";
                TxtBDtoVcd.Text = "0,00";
                TxtBDtoSic.Text = "0,00";
                TxtBDtoOt.Text = "0,00";
                is_reciboProv = false;
            }
            if (estado == 1) //creando
            {
                is_reciboProv = false;
                TextCodeCliente.Text = string.Empty;
                TextNomCliente.Text = string.Empty;
                TextRProv.Text = string.Empty;
                TextNota.Text = "Cancelacion/Abono Facturas";
                TextNumeroDoc.Text = "";
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

                //TextNumeroDoc.Text = SiaWin.Func.ConsecutivoPv(codpvta, 0, 10, BusinessCode);
                TextNumeroDoc.Text = consecutivo();

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


        //popo
        public string consecutivo()
        {

            string con = "";
            try
            {
                string sqlConsecutivo = "";
                string TipoConsecutivo = "";

                if (isPuntoVen == true)
                {
                    TipoConsecutivo = "rcaja";

                    sqlConsecutivo += @"declare @fecdoc as datetime;set @fecdoc = getdate(); ";
                    sqlConsecutivo += "declare @fecdocsecond as datetime;set @fecdocsecond = DATEADD(second,1,GETDATE()); ";
                    sqlConsecutivo += "declare @ini as char(4);declare @num as varchar(12);  ";
                    sqlConsecutivo += "declare @iConsecutivo char(12) = '' ;declare @iFolioHost int = 0; ";
                    sqlConsecutivo += "SELECT @iFolioHost = " + TipoConsecutivo + ",@ini=rtrim('" + codbod + "') FROM Copventas  WHERE cod_pvt='" + codpvta + "';";
                    sqlConsecutivo += "set @num=@iFolioHost ";
                    sqlConsecutivo += "select @iConsecutivo=rtrim(@ini)+'-'+REPLICATE ('0',11-len(rtrim(@ini))-len(rtrim(convert(varchar,@num))))+rtrim(convert(varchar,@num)); ";
                    sqlConsecutivo += "select @iConsecutivo as consecutivo;  ";
                }
                if (isPuntoVen == false)
                {
                    sqlConsecutivo = @"declare @fecdoc as datetime;set @fecdoc = getdate(); ";
                    sqlConsecutivo += "declare @fecdocsecond as datetime;set @fecdocsecond = DATEADD(second,1,GETDATE()); ";
                    sqlConsecutivo += "declare @ini as char(4);declare @num as varchar(12);  ";
                    sqlConsecutivo += "declare @iConsecutivo char(12) = '' ;declare @iFolioHost int = 0; ";
                    sqlConsecutivo += "SELECT @iFolioHost= isnull(num_act,0)+1,@ini=rtrim(inicial) FROM Comae_trn WHERE cod_trn='01'; ";
                    sqlConsecutivo += "set @num=@iFolioHost ";
                    sqlConsecutivo += "select @iConsecutivo=rtrim(@ini)+REPLICATE ('0',12-len(rtrim(@ini))-len(rtrim(convert(varchar,@num))))+rtrim(convert(varchar,@num)); ";
                    sqlConsecutivo += "select @iConsecutivo as consecutivo;  ";
                }


                DataTable dt = SiaWin.DB.SqlDT(sqlConsecutivo, "cons", idemp);

                if (dt.Rows.Count > 0)
                {
                    con = dt.Rows[0]["consecutivo"].ToString();
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error en el consecutivo:" + w);
                con = "***";
            }

            return con;
        }


        public bool valiDocumExis(string num_trn)
        {
            bool flag = false;
            string select = "select * from cocab_doc where num_trn='" + num_trn + "' and cod_trn='01' ";
            DataTable dt = SiaWin.DB.SqlDT(select, "documento", idemp);
            if (dt.Rows.Count > 0) flag = true;
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

                    if (CmbVen.SelectedValue.ToString().Trim() == "A1" || CmbVen.SelectedValue.ToString().Trim() == "A2")
                    { }
                    else
                    {
                        if (existenciaConbleReciboPrv(TextRProv.Text.Trim()) == true)
                        {
                            MessageBox.Show("el recibo:" + TextRProv.Text.Trim() + " ya ha sido generado en contrabilidad");
                            return;
                        }
                    }


                    string cons = consecutivo().Trim();
                    if (valiDocumExis(cons) == true)
                    {
                        MessageBox.Show("el recibo " + cons + " ya existe consulte con el administrador");
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
                    if (MessageBox.Show("Usted desea guardar el documento..?", "Guardar Recibo de Caja", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
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

                            double valorPasar = Math.Round(_abonototal);
                            // descontar o sumar otros valores                            


                            SiaWin.ValReturn = valorPasar;
                            //MessageBox.Show("aqui2");
                            //Window wFpago = SiaWin.WindowExt(9341, "FormasDePago");
                            FormasDePago wFpago = new FormasDePago();

                            if (wFpago == null)
                            {
                                MessageBox.Show("Windows Null");
                                return;
                            }
                            string[] strArrayParam = new string[] { TextCodeCliente.Text.Trim(), TextNomCliente.Text.Trim(), TotalAbono.Text };

                            wFpago.recibo_prov = is_reciboProv == true ? TextRProv.Text.Trim() : "";
                            wFpago.vendedor = is_reciboProv == true ? CmbVen.SelectedValue.ToString() : "";

                            wFpago.ShowInTaskbar = false;
                            wFpago.Owner = Application.Current.MainWindow;
                            wFpago.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                            wFpago.ShowDialog();
                            wFpago = null;
                            if (SiaWin.ValReturn == null) return; // cancelo forma de pago
                            fPago = (DataTable)SiaWin.ValReturn;
                            //SiaWin.Browse(fPago);
                            //iddocumento = ExecuteSqlTransaction(_CodeCliente.ToString(), ctaban.ToString(), _abono);

                            iddocumento = ExecuteSqlTransaction(_CodeCliente.ToString(), _abono);

                            if (iddocumento <= 0) return;
                            if (iddocumento > 0)  SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, moduloid, -1, -9, "GENERO UNA RECIBO DE CAJA: #Recibo:" + iddocumento + "", "");
                            ImprimeRC(iddocumento);

                            //ImprimeDocumento(iddocumento, TextCodeCliente.Text.Trim());
                            //ImprimirDoc(iddocumento, "Impresion Original");
                            //MessageBox.Show("Documento Guardado:" + iddocumento.ToString());
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
                MessageBox.Show(exx.Message);
            }
        }

        private void ImprimeRC(int idregcab)
        {
            try
            {
                DataTable dtAud = new DataTable();
                dtAud = SiaWin.DB.SqlDT("select cod_trn,num_trn,fec_trn,cod_ven,inmae_mer.nom_mer from " + "co" + "cab_doc inner join inmae_mer on inme_mer.cod_mer=cocab_doc.cod_ven where idreg=" + idregcab, "tmp", idemp);
                string _codtrn = "";
                string _numtrn = "";
                string _codven = "";
                string _nomven = "";
                if (dtAud.Rows.Count > 0)
                {
                    _codtrn = dtAud.Rows[0]["cod_trn"].ToString();
                    _numtrn = dtAud.Rows[0]["num_trn"].ToString();
                    _codven = dtAud.Rows[0]["cod_ven"].ToString();
                    _nomven = dtAud.Rows[0]["nom_mer"].ToString();
                }
                
                
                if(_codtrn=="")
                {
                    MessageBox.Show("El documento no existe...", "ImprimeRC");
                    return;
                }
                // trae factuas canceladas

                string sqltext = @"select string_agg(rtrim(doc_cruc),',') as facturas from cocab_doc inner join cocue_doc on cocue_doc.idregcab=cocab_doc.idreg where cocab_doc.cod_trn='" + _codtrn + "' and cocab_doc.num_trn='" + _numtrn + "' and  rtrim(doc_cruc)<>''";

                DataTable dtfacturas = SiaWin.DB.SqlDT(sqltext, "tmp", idemp);
                string _Facturas = "";
                if (dtfacturas.Rows.Count > 0)
                {
                    _Facturas = dtfacturas.Rows[0]["facturas"].ToString();
                }
                string sqltexttotal = @"select sum(iif(substring(cod_cta, 1, 2) = '11', deb_mov, 0)) as total from cocab_doc inner join cocue_doc on cocue_doc.idregcab=cocab_doc.idreg where cocab_doc.cod_trn='" + _codtrn + "' and cocab_doc.num_trn='" + _numtrn + "'";
                DataTable dtTotal = SiaWin.DB.SqlDT(sqltexttotal, "tmp", idemp);
                decimal totalfac = 0;
                if (dtTotal.Rows.Count > 0)
                {
                    totalfac  = (decimal)dtTotal.Rows[0]["total"];
                }

                string enletras = SiaWin.Func.enletras(totalfac.ToString());  //valor en letra

                List<ReportParameter> parameters = new List<ReportParameter>();
                ReportParameter paramcodemp = new ReportParameter();
                paramcodemp.Values.Add(BusinessCode);
                paramcodemp.Name = "codemp";
                parameters.Add(paramcodemp);

                ReportParameter paramcodtrn = new ReportParameter();
                paramcodtrn.Values.Add(_codtrn);
                paramcodtrn.Name = "codtrn";
                parameters.Add(paramcodtrn);
                ReportParameter paramnumtrn = new ReportParameter();
                paramnumtrn.Values.Add(_numtrn);
                paramnumtrn.Name = "numtrn";
                parameters.Add(paramnumtrn);

                ReportParameter paramFacturas = new ReportParameter();
                paramFacturas.Values.Add(_Facturas);
                paramFacturas.Name = "Facturas";
                parameters.Add(paramFacturas);

                ReportParameter paramValorLetras = new ReportParameter();
                paramValorLetras.Values.Add(enletras);
                paramValorLetras.Name = "ValorLetras";
                parameters.Add(paramValorLetras);


                string repnom = @"/Contabilidad/ReciboDeCajaOficial";
                string TituloReport = "Recibo de Caja Oficial -";
                SiaWin.Reportes(parameters, repnom, TituloReporte: TituloReport, Modal: true, idemp: idemp, ZoomPercent: 50);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message.ToString());
            }

        }
        public bool existenciaConbleReciboPrv(string recibo)
        {
            if (string.IsNullOrEmpty(recibo) && (CmbVen.SelectedValue.ToString().Trim() == "A1" || CmbVen.SelectedValue.ToString().Trim() == "A2"))
                return false;

            bool bandera = false;
            string query = "select * from CoCab_doc where rc_prov='" + recibo + "' ";
            DataTable dt = SiaWin.Func.SqlDT(query, "table", idemp);
            if (dt.Rows.Count > 0) bandera = true;
            if (CmbVen.SelectedValue.ToString().Trim() == "A1" || CmbVen.SelectedValue.ToString().Trim() == "A2") bandera = false;
            return bandera;
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

        private int ExecuteSqlTransaction(string codter, double abonoBco)
        {
            if (string.IsNullOrEmpty(cnEmp))
            {
                MessageBox.Show("Error - Cadena de Conexion nulla");
                return -1;
            }
            string TipoConsecutivo = "rcaja";
            string codtrn = "01";
            using (SqlConnection connection = new SqlConnection(cnEmp))
            {
                connection.Open();
                StringBuilder errorMessages = new StringBuilder();
                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;
                // Start a local transaction.
                transaction = connection.BeginTransaction("Transaction");
                command.Connection = connection;
                command.Transaction = transaction;
                try
                {

                    string sqlConsecutivo = @"declare @fecdoc as datetime;set @fecdoc = getdate();";
                    sqlConsecutivo += "declare @fecdocsecond as datetime;set @fecdocsecond = DATEADD(second,1,GETDATE()); ";
                    sqlConsecutivo += "declare @ini as char(4);declare @num as varchar(12);  ";
                    sqlConsecutivo += "declare @iConsecutivo char(12) = '' ;declare @iFolioHost int = 0; ";

                    if (isPuntoVen == true)
                    {
                        sqlConsecutivo += "SELECT @iFolioHost = " + TipoConsecutivo + ",@ini=rtrim('" + codbod + "') FROM Copventas  WHERE cod_pvt='" + codpvta + "';";
                        sqlConsecutivo += "set @num=@iFolioHost ";
                        sqlConsecutivo += "select @iConsecutivo=rtrim(@ini)+'-'+REPLICATE ('0',11-len(rtrim(@ini))-len(rtrim(convert(varchar,@num))))+rtrim(convert(varchar,@num)); ";
                        sqlConsecutivo += "UPDATE COpventas SET " + TipoConsecutivo + " = ISNULL(" + TipoConsecutivo + ", 0) + 1  WHERE cod_pvt='" + codpvta + "'; ";
                    }
                    else
                    {
                        sqlConsecutivo += "SELECT @iFolioHost= isnull(num_act,0)+1,@ini=rtrim(inicial) FROM Comae_trn WHERE cod_trn='01'; ";
                        sqlConsecutivo += "set @num=@iFolioHost ";
                        sqlConsecutivo += "select @iConsecutivo=rtrim(@ini)+REPLICATE ('0',12-len(rtrim(@ini))-len(rtrim(convert(varchar,@num))))+rtrim(convert(varchar,@num)); ";
                        sqlConsecutivo += "UPDATE comae_trn SET num_act=ISNULL(num_act, 0) + 1  WHERE cod_trn='01'; ";
                    }



                    string sqlcab = sqlConsecutivo + @"INSERT INTO cocab_doc (cod_trn,fec_trn,num_trn,detalle,cod_ven,rc_prov,ven_com,pun_ven) values ('" + codtrn + "',@fecdoc,@iConsecutivo,'" + TextNota.Text.Trim() + "','" + CmbVen.SelectedValue + "','" + TextRProv.Text.Trim() + "','" + CmbVen1.SelectedValue + "','"+ codpvta + "');DECLARE @NewID INT;SELECT @NewID = SCOPE_IDENTITY();";
                    string sql = "";
                    foreach (System.Data.DataRow item in dtCue.Rows)
                    {

                        double abono = Convert.ToDouble(item["abono"].ToString());
                        if (abono > 0)
                        {
                            int tipapli = Convert.ToInt32(item["tip_apli"].ToString());
                            if (tipapli == 2 || tipapli == 3) sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,doc_cruc,doc_ref,cre_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + item["cod_cta"].ToString() + "','" + item["cod_cco"].ToString() + "','" + item["cod_ter"].ToString() + "','Pago/Abono Doc:" + item["num_trn"].ToString() + "','" + item["num_trn"].ToString() + "','" + item["num_trn"].ToString() + "'," + abono.ToString("F", CultureInfo.InvariantCulture) + ");";
                            if (tipapli == 1 || tipapli == 4) sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,doc_cruc,doc_ref,deb_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + item["cod_cta"].ToString() + "','" + item["cod_cco"].ToString() + "','" + item["cod_ter"].ToString() + "','Pago/Abono Doc:" + item["num_trn"].ToString() + "','" + item["num_trn"].ToString() + "','" + item["num_trn"].ToString() + "'," + abono.ToString("F", CultureInfo.InvariantCulture) + ");";
                        }
                        double dtoImal = Convert.ToDouble(item["dto_imal"].ToString());
                        if (dtoImal > 0)
                        {
                            int tipapli = Convert.ToInt32(item["tip_apli"].ToString());
                            if (tipapli == 2 || tipapli == 3) sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,doc_cruc,doc_ref,deb_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'530535','" + item["cod_cco"].ToString() + "','" + item["cod_ter"].ToString() + "','Dto Imal Doc:" + item["num_trn"].ToString() + "','" + item["num_trn"].ToString() + "','" + item["num_trn"].ToString() + "'," + dtoImal.ToString("F", CultureInfo.InvariantCulture) + ");";
                        }
                        double dtoIncol = Convert.ToDouble(item["dto_incol"].ToString());
                        if (dtoIncol > 0)
                        {
                            int tipapli = Convert.ToInt32(item["tip_apli"].ToString());
                            if (tipapli == 2 || tipapli == 3) sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,doc_cruc,doc_ref,deb_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'530535','" + item["cod_cco"].ToString() + "','" + item["cod_ter"].ToString() + "','Dto Incol Doc:" + item["num_trn"].ToString() + "','" + item["num_trn"].ToString() + "','" + item["num_trn"].ToString() + "'," + dtoIncol.ToString("F", CultureInfo.InvariantCulture) + ");";
                        }
                        double dtoTmk = Convert.ToDouble(item["dto_tmk"].ToString());
                        if (dtoTmk > 0)
                        {
                            int tipapli = Convert.ToInt32(item["tip_apli"].ToString());
                            if (tipapli == 2 || tipapli == 3) sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,doc_cruc,doc_ref,deb_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'530535','" + item["cod_cco"].ToString() + "','" + item["cod_ter"].ToString() + "','Dto Tmk Doc:" + item["num_trn"].ToString() + "','" + item["num_trn"].ToString() + "','" + item["num_trn"].ToString() + "'," + dtoTmk.ToString("F", CultureInfo.InvariantCulture) + ");";
                        }
                        double dtoGab = Convert.ToDouble(item["dto_gab"].ToString());
                        if (dtoGab > 0)
                        {
                            int tipapli = Convert.ToInt32(item["tip_apli"].ToString());
                            if (tipapli == 2 || tipapli == 3) sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,doc_cruc,doc_ref,deb_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'530535','" + item["cod_cco"].ToString() + "','" + item["cod_ter"].ToString() + "','Dto Gab Doc:" + item["num_trn"].ToString() + "','" + item["num_trn"].ToString() + "','" + item["num_trn"].ToString() + "'," + dtoGab.ToString("F", CultureInfo.InvariantCulture) + ");";
                        }
                        double dtoVcd = Convert.ToDouble(item["dto_vcd"].ToString());
                        if (dtoVcd > 0)
                        {
                            int tipapli = Convert.ToInt32(item["tip_apli"].ToString());
                            if (tipapli == 2 || tipapli == 3) sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,doc_cruc,doc_ref,deb_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'530535','" + item["cod_cco"].ToString() + "','" + item["cod_ter"].ToString() + "','Dto Vcd Doc:" + item["num_trn"].ToString() + "','" + item["num_trn"].ToString() + "','" + item["num_trn"].ToString() + "'," + dtoVcd.ToString("F", CultureInfo.InvariantCulture) + ");";
                        }
                        double dtoSic = Convert.ToDouble(item["dto_sic"].ToString());
                        if (dtoSic > 0)
                        {
                            int tipapli = Convert.ToInt32(item["tip_apli"].ToString());
                            if (tipapli == 2 || tipapli == 3) sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,doc_cruc,doc_ref,deb_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'530535','" + item["cod_cco"].ToString() + "','" + item["cod_ter"].ToString() + "','Dto Sicolsa Doc:" + item["num_trn"].ToString() + "','" + item["num_trn"].ToString() + "','" + item["num_trn"].ToString() + "'," + dtoSic.ToString("F", CultureInfo.InvariantCulture) + ");";
                        }
                        double dtoOt = Convert.ToDouble(item["dto_ot"].ToString());
                        if (dtoOt > 0)
                        {
                            int tipapli = Convert.ToInt32(item["tip_apli"].ToString());
                            if (tipapli == 2 || tipapli == 3) sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,doc_cruc,doc_ref,deb_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'530535','" + item["cod_cco"].ToString() + "','" + item["cod_ter"].ToString() + "','Dto Otros Doc:" + item["num_trn"].ToString() + "','" + item["num_trn"].ToString() + "','" + item["num_trn"].ToString() + "'," + dtoOt.ToString("F", CultureInfo.InvariantCulture) + ");";
                        }


                    }
                    if (Retefte > 0)
                    {
                        sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,deb_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'13551505','','" + codter.Trim() + "','ReteFte:" + nomter + "'," + Retefte.ToString("F", CultureInfo.InvariantCulture) + ");";
                    }
                    if (Reteica > 0)
                    {
                        sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,deb_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'135518','','" + codter.Trim() + "','ReteIca" + nomter + "'," + Reteica.ToString("F", CultureInfo.InvariantCulture) + ");";
                    }
                    if (Reteiva > 0)
                    {
                        sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,deb_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'135517','','" + codter.Trim() + "','ReteIva:" + nomter + "'," + Reteiva.ToString("F", CultureInfo.InvariantCulture) + ");";
                    }
                    if (Mayorvlr > 0)
                    {
                        sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,cre_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'429505','','" + codter.Trim() + "','Mayor Vr Recibido:" + nomter + "'," + Mayorvlr.ToString("F", CultureInfo.InvariantCulture) + ");";
                    }
                    if (Menorvlr > 0)
                    {
                        sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,deb_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'530535','','" + codter.Trim() + "','Menor Vr Recibido:" + nomter + "'," + Menorvlr.ToString("F", CultureInfo.InvariantCulture) + ");";
                    }
                    if (Anticipo > 0)
                    {
                        sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,cre_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'280505','','" + codter.Trim() + "','Anticipo:" + nomter + "'," + Anticipo.ToString("F", CultureInfo.InvariantCulture) + ");";
                    }


                    string sqlban = "";
                    foreach (System.Data.DataRow item1 in fPago.Rows)
                    {
                        string value = item1["valor"].ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            double abono = Convert.ToDouble(item1["valor"].ToString());
                            if (abono > 0)
                            {
                                string _cta = item1["cod_cta"].ToString().Trim();
                                string cod_ban = item1["cod_ban"].ToString().Trim();

                                string fec_venc = item1["fec_venc"].ToString().Trim();

                                string fec_con = item1["fec_con"].ToString().Trim();

                                string documento = item1["documento"].ToString().Trim();

                                string cod_banco = item1["cod_banco"].ToString().Trim();


                                if (cod_ban == "45" || cod_ban == "50")                                
                                    sqlban = sqlban + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,deb_mov,fec_venc,num_chq,cod_banc,cod_pag) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + _cta.Trim() + "','','" + codter.Trim() + "','Pago/Abono:" + nomter + "'," + abono.ToString("F", CultureInfo.InvariantCulture) + ",'" + fec_venc + "','" + documento + "','" + cod_banco + "','" + cod_ban + "');";
                                else                                
                                    sqlban = sqlban + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,deb_mov,fec_con,cod_pag) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + _cta.Trim() + "','','" + codter.Trim() + "','Pago/Abono:" + nomter + "'," + abono.ToString("F", CultureInfo.InvariantCulture) + ",'" + fec_con + "','" + cod_ban + "');";
                                

                            }
                        }
                    }
                    command.CommandText = sqlcab + sql + sqlban + @"select CAST(@NewId AS int);";

                    //                    MessageBox.Show(command.CommandText.ToString());
                    var r = new object();
                    r = command.ExecuteScalar();
                    transaction.Commit();
                    connection.Close();
                    return Convert.ToInt32(r.ToString());
                }
                catch (SqlException ex)
                {
                    //for (int i = 0; i < ex.Errors.Count; i++)
                    //{
                    //    errorMessages.Append(" SQL-Index #" + i + "\n" + "Message: " + ex.Errors[i].Message + "\n" + "LineNumber: " + ex.Errors[i].LineNumber + "\n" + "Source: " + ex.Errors[i].Source + "\n" + "Procedure: " + ex.Errors[i].Procedure + "\n");
                    //}
                    //transaction.Rollback();
                    MessageBox.Show("error al guardar el documento contacte al administrador");
                    //MessageBox.Show(errorMessages.ToString());
                    return -1;
                }
                catch (Exception ex)
                {
                    errorMessages.Append("Error:" + ex.StackTrace + "-" + ex.Message.ToString());
                    transaction.Rollback();
                    MessageBox.Show(errorMessages.ToString());
                    return -1;
                }
            }
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
                    _sql.Append("select cab.cod_trn,cab.num_trn,cab.fec_trn,cue.cod_cco,cco.alias,cab.cod_ven,cab.detalle,cue.cod_cta,cue.cod_ter,rtrim(ter.nom_ter) as nom_ter,doc_cruc,deb_mov + cre_mov as valor,cab.idreg,dto_imal,dto_incol,dto_tmk,dto_gab,dto_sic,dto_vcd,dto_ot,");
                    _sql.Append("CASE cta.tip_apli WHEN 3 THEN 'CxC'  ELSE 'CxCAnt' END as tipo,cta.tip_apli,cab.idreg ");
                    _sql.Append(" from cocue_doc as cue  inner join cocab_doc as cab on cab.idreg = cue.idregcab and cab.cod_trn = '01 ' ");
                    _sql.Append("inner join comae_cta as cta on cta.cod_cta = cue.cod_cta and cta.tip_apli between 3 and 4 ");
                    _sql.Append("inner join comae_ter as ter on ter.cod_ter = cue.cod_ter inner join comae_cco as cco on cco.cod_cco = cue.cod_cco ");
                    _sql.Append("inner join comae_trn as trn on trn.cod_trn = cab.cod_trn  where convert(date,cab.fec_trn) between '" + FechaIni.Text + "' and '" + FechaFin.Text + "' ");
                    _sql.Append(" order by cab.fec_trn,cod_trn,num_trn ");
                    SqlDataAdapter adapter = new SqlDataAdapter(_sql.ToString(), connection);
                    adapter.Fill(ds, "RCaja");
                    dataGridSF.ItemsSource = ds.Tables["RCaja"];
                    double totcxc = 0;
                    double totant = 0;
                    double.TryParse(ds.Tables["RCaja"].Compute("Sum(valor)", "tip_apli=3").ToString(), out totcxc);
                    double.TryParse(ds.Tables["RCaja"].Compute("Sum(valor)", "tip_apli=4").ToString(), out totant);

                    TextTotalCxC.Text = totcxc.ToString("C");
                    TextTotalAnticipos.Text = totant.ToString("C");
                }
            }
            catch (Exception ex)
            {
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
                    int idreg = (int)dr["idreg"];
                    if (idreg > 0) ImprimeRC(idreg);
                    //                  MessageBox.Show(codterc);
                    //ImprimeDocumento(Convert.ToInt32(numtrn), codterc);
                    //ImprimirDoc(Convert.ToInt32(numtrn), "Reimp");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ExportaXLS_Click(object sender, RoutedEventArgs e)
        {
            var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
            options.ExcelVersion = ExcelVersion.Excel2013;
            //           var excelEngine = dataGridSF.ExportToExcel(dataGridSF.View, options);
            //            var workBook = excelEngine.Excel.Workbooks[0];
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
        private async void ConsultaSaldoCartera()
        {

            dataGrid.ItemsSource = 0;

            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            sfBusyIndicator.IsBusy = true;

            string tercero = TextCodeCliente.Text.Trim();

            string fecha = TextFecha.Text;

            var slowTask = Task<DataTable>.Factory.StartNew(() => load(tercero, fecha, BusinessCode, source.Token), source.Token);
            await slowTask;

            if (((DataTable)slowTask.Result).Rows.Count > 0)
            {

                if (((DataTable)slowTask.Result).Rows.Count == 0)
                {
                    MessageBox.Show("Sin informacion de cartera");
                    dataGrid.ItemsSource = null;
                    TextCodeCliente.Text = "";
                    TextNomCliente.Text = "";
                }
                try
                {
                    sumaTotal();
                    dataGrid.ItemsSource = ((DataTable)slowTask.Result).DefaultView;
                }
                catch (Exception W)
                {
                    MessageBox.Show("Actualiza Grid www:" + W);
                }
            }
            sfBusyIndicator.IsBusy = false;
        }



        public DataTable load(string ter, string fecha, string empre, CancellationToken cancellationToken)
        {
            SqlConnection con = new SqlConnection(SiaWin._cn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet ds1 = new DataSet();
            cmd = new SqlCommand("_empSpCoAnalisisCxc", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Ter", ter);
            cmd.Parameters.AddWithValue("@Cta", "");
            cmd.Parameters.AddWithValue("@TipoApli", -1);
            cmd.Parameters.AddWithValue("@Resumen", 1);
            cmd.Parameters.AddWithValue("@Fecha", fecha);
            cmd.Parameters.AddWithValue("@TrnCo", "");
            cmd.Parameters.AddWithValue("@NumCo", "");
            cmd.Parameters.AddWithValue("@Cco", "");
            cmd.Parameters.AddWithValue("@codemp", empre);
            dtCue.Clear();
            da = new SqlDataAdapter(cmd);
            da.Fill(dtCue);
            con.Close();

            //consulllll
            //MessageBox.Show("cont:"+ dtCue.Rows.Count.ToString());
            return dtCue;
        }


        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
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

            //MessageBox.Show("sumaTotal()");
        }
        private void sumaAbonos()
        {
            try
            {
                if (string.IsNullOrEmpty(TextCodeCliente.Text)) return;

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

                //MessageBox.Show("sumaAbonos()");
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
                    TextCodeCliente.Text = codter;
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
                    xx.Height = 500;
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
                MessageBox.Show("ERROR LOSFOCUSTERCERO:" + w);
            }
        }


        public bool validarReciboProvi(string texto)
        {
            bool bandera = true;

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

            return bandera;
        }

        private void TextRProv_LostFocus(object sender, RoutedEventArgs e)
        {
            string texto = (sender as TextBox).Text;

            if (CmbVen.SelectedIndex < 0) return;

            if (validarReciboProvi(texto) == false) return;

            if (existenciaConbleReciboPrv(TextRProv.Text.Trim()) == true)
            {
                MessageBox.Show("el recibo:" + TextRProv.Text.Trim() + " ya ha sido generado en contabilidad");
                return;
            }

            ActualizaRecibosProvisionales(TextRProv.Text.ToString().Trim(), CmbVen.SelectedValue.ToString().Trim());
        }


        public void ActualizaRecibosProvisionales(string recibo, string vendedor)
        {
            try
            {
                if (string.IsNullOrEmpty(recibo) && (vendedor == "A1" || vendedor == "A2"))
                {
                    is_reciboProv = false;
                    TextVlrRecibido.Value = 0;
                    TextRetefte.Value = 0;
                    TextIca.Value = 0;
                    TextReteIva.Value = 0;
                    TextReteIva.Value = 0;
                    TextMayorVlr.Value = 0;
                    TextMenorVlr.Value = 0;
                    TextAnticipo.Value = 0;
                    double value = 0;
                    TxtBDtoImal.Text = value.ToString("C");
                    TxtBDtoIncol.Text = value.ToString("C");
                    TxtBDtoTmk.Text = value.ToString("C");
                    TxtBDtoGab.Text = value.ToString("C");
                    TxtBDtoVcd.Text = value.ToString("C");
                    TxtBDtoSic.Text = value.ToString("C");
                    TxtBDtoOt.Text = value.ToString("C");
                    foreach (System.Data.DataRow cue in dtCue.Rows)
                    {
                        cue["abono"] = 0;
                        cue["dto_imal"] = 0;
                        cue["dto_incol"] = 0;
                        cue["dto_tmk"] = 0;
                        cue["dto_gab"] = 0;
                        cue["dto_vcd"] = 0;
                        cue["dto_sic"] = 0;
                        cue["dto_ot"] = 0;
                    }
                    sumaAbonos();
                    return;
                }


                //vococ
                DataTable dt_cabeza = SiaWin.Func.SqlDT("select * from cocabrcpv where rcprov='" + recibo + "' and cod_ven='" + vendedor + "';", "table", idemp);
                if (dt_cabeza.Rows.Count > 0)
                {
                    is_reciboProv = true;
                    TextVlrRecibido.Value = Convert.ToDecimal(dt_cabeza.Rows[0]["vr_rec"]);
                    TextRetefte.Value = Convert.ToDecimal(dt_cabeza.Rows[0]["rte_fte"]);
                    TextIca.Value = Convert.ToDecimal(dt_cabeza.Rows[0]["rte_ica"]);
                    TextReteIva.Value = Convert.ToDecimal(dt_cabeza.Rows[0]["rte_iva"]);
                    TextReteIva.Value = Convert.ToDecimal(dt_cabeza.Rows[0]["rte_iva"]);
                    TextMayorVlr.Value = Convert.ToDecimal(dt_cabeza.Rows[0]["mypag"]);
                    TextMenorVlr.Value = Convert.ToDecimal(dt_cabeza.Rows[0]["mnpag"]);
                    TextAnticipo.Value = Convert.ToDecimal(dt_cabeza.Rows[0]["antic"]);
                    double imal = Convert.ToDouble(dt_cabeza.Rows[0]["dto_imal"]);
                    TxtBDtoImal.Text = imal.ToString("C");
                    double incol = Convert.ToDouble(dt_cabeza.Rows[0]["dto_incol"]);
                    TxtBDtoIncol.Text = incol.ToString("C");
                    double tmk = Convert.ToDouble(dt_cabeza.Rows[0]["dto_tmk"]);
                    TxtBDtoTmk.Text = tmk.ToString("C");
                    double gabriel = Convert.ToDouble(dt_cabeza.Rows[0]["dto_gab"]);
                    TxtBDtoGab.Text = gabriel.ToString("C");
                    double victor = Convert.ToDouble(dt_cabeza.Rows[0]["dto_vcd"]);
                    TxtBDtoVcd.Text = victor.ToString("C");
                    double sic = Convert.ToDouble(dt_cabeza.Rows[0]["dto_sic"]);
                    TxtBDtoSic.Text = sic.ToString("C");
                    double ot = Convert.ToDouble(dt_cabeza.Rows[0]["dto_ot"]);
                    TxtBDtoOt.Text = ot.ToString("C");


                    DataTable dt_cuerpo = SiaWin.Func.SqlDT("select * from cocuercpv where rcprov='" + recibo + "' and cod_ven='" + vendedor + "';", "table", idemp);
                    //SiaWin.Browse(dt_cuerpo);
                    if (dt_cuerpo.Rows.Count > 0)
                    {
                        foreach (System.Data.DataRow item in dt_cuerpo.Rows)
                        {
                            string cod_trn = item["cod_trn"].ToString().Trim();
                            string num_trn = item["num_trn"].ToString().Trim();
                            foreach (System.Data.DataRow cue in dtCue.Rows)
                            {
                                string cod_trn_cue = cue["cod_trn"].ToString().Trim();
                                string num_trn_cue = cue["num_trn"].ToString().Trim();

                                if (cod_trn_cue == cod_trn && num_trn_cue == num_trn)
                                {
                                    cue["abono"] = item["vr_abono"].ToString();
                                    cue["dto_imal"] = item["dto_imal"].ToString();
                                    cue["dto_incol"] = item["dto_incol"].ToString();
                                    cue["dto_tmk"] = item["dto_tmk"].ToString();
                                    cue["dto_gab"] = item["dto_gab"].ToString();
                                    cue["dto_vcd"] = item["dto_vcd"].ToString();
                                    cue["dto_sic"] = item["dto_sic"].ToString();
                                    cue["dto_ot"] = item["dto_ot"].ToString();
                                }
                            }
                        }


                    }

                    sumaAbonos();
                }
                else
                {
                    is_reciboProv = false;
                    TextVlrRecibido.Value = 0;
                    TextRetefte.Value = 0;
                    TextIca.Value = 0;
                    TextReteIva.Value = 0;
                    TextReteIva.Value = 0;
                    TextMayorVlr.Value = 0;
                    TextMenorVlr.Value = 0;
                    TextAnticipo.Value = 0;
                    double value = 0;
                    TxtBDtoImal.Text = value.ToString("C");
                    TxtBDtoIncol.Text = value.ToString("C");
                    TxtBDtoTmk.Text = value.ToString("C");
                    TxtBDtoGab.Text = value.ToString("C");
                    TxtBDtoVcd.Text = value.ToString("C");
                    TxtBDtoSic.Text = value.ToString("C");
                    TxtBDtoOt.Text = value.ToString("C");
                    foreach (System.Data.DataRow cue in dtCue.Rows)
                    {
                        cue["abono"] = 0;
                        cue["dto_imal"] = 0;
                        cue["dto_incol"] = 0;
                        cue["dto_tmk"] = 0;
                        cue["dto_gab"] = 0;
                        cue["dto_vcd"] = 0;
                        cue["dto_sic"] = 0;
                        cue["dto_ot"] = 0;
                    }
                    sumaAbonos();
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al actualizar:" + w);
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

            //if ((e.Key == Key.Enter || e.Key == Key.Return || e.Key == Key.Tab))
            //{
            //    TextBox s = e.Source as TextBox;
            //    if (s != null)
            //    {                               
            //        dataGrid.Focus();
            //        dataGrid.SelectedIndex = 0;
            //        e.Handled = true;
            //    }
            //}

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
                MessageBox.Show("Error Imprime Factura:" + ex.ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadInfo();
            if (string.IsNullOrEmpty(codter)) return;
            try
            {
                if (!string.IsNullOrEmpty(fechaPublic)) TextFecha.Text = fechaPublic;


                //this.dataGrid.MoveCurrentCell(new RowColumnIndex(1, 8), false);
                if (!string.IsNullOrEmpty(codter))
                {
                    if (!ActualizaCampos(codter))
                    {
                        MessageBox.Show("El codigo de tercereo:" + codter + " no existe");
                    }
                    else
                    {
                        //ConsultaSaldoCartera();
                        ActivaDesactivaControles(1);
                        if (!string.IsNullOrEmpty(TextCodeCliente.Text.Trim())) TextCodeCliente.Focusable = false;
                        TextCodeCliente.Text = codter;
                        BtbGrabar.Content = "Grabar";
                        TextNota.Focus();


                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ppp:" + ex.Message.ToString());
            }

        }

        private void CmbVen_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            TextRProv.Text = "";
            is_reciboProv = false;
            TextVlrRecibido.Value = 0;
            TextRetefte.Value = 0;
            TextIca.Value = 0;
            TextReteIva.Value = 0;
            TextReteIva.Value = 0;
            TextMayorVlr.Value = 0;
            TextMenorVlr.Value = 0;
            TextAnticipo.Value = 0;
            double value = 0;
            TxtBDtoImal.Text = value.ToString("C");
            TxtBDtoIncol.Text = value.ToString("C");
            TxtBDtoTmk.Text = value.ToString("C");
            TxtBDtoGab.Text = value.ToString("C");
            TxtBDtoVcd.Text = value.ToString("C");
            TxtBDtoSic.Text = value.ToString("C");
            TxtBDtoOt.Text = value.ToString("C");
            foreach (System.Data.DataRow cue in dtCue.Rows)
            {
                cue["abono"] = 0;
                cue["dto_imal"] = 0;
                cue["dto_incol"] = 0;
                cue["dto_tmk"] = 0;
                cue["dto_gab"] = 0;
                cue["dto_vcd"] = 0;
                cue["dto_sic"] = 0;
                cue["dto_ot"] = 0;
            }
            sumaAbonos();
        }


        private void ActualizaTotal(object sender, RoutedEventArgs e)
        {
            sumaAbonos();
        }





    }
}




