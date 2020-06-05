using Egreso;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{

    //Sia.PublicarPnt(9541,"Egreso");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9541, "Egreso");
    //ww.codpvta = "003";
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();

    public partial class Egreso : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public string codbod = "";
        public string codpvta = "";

        DataTable dtCue = new DataTable();
        DataTable dtBanco = new DataTable();

        DataTable dt_egreso = new DataTable();


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
        double Descuento = 0;

        double VlrSaldo = 0;



        public Egreso()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            idemp = SiaWin._BusinessId; ;
            LoadConfig();

            ActivaDesactivaControles(0);
            BtbGrabar.Focus();
            loadEgreso();
            act(1);

            this.DataContext = this;


        }

        public void loadEgreso()
        {
            dt_egreso.Columns.Add("cod_cta");
            dt_egreso.Columns.Add("cod_ter");
            dt_egreso.Columns.Add("cod_cco");
            dt_egreso.Columns.Add("des_mov");
            dt_egreso.Columns.Add("doc_cruc");
            dt_egreso.Columns.Add("bas_mov", typeof(double));
            dt_egreso.Columns.Add("deb_mov", typeof(double));
            dt_egreso.Columns.Add("cre_mov", typeof(double));
            GridConfig.ItemsSource = dt_egreso.DefaultView;
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
                this.Title = "Egresos " + cod_empresa + "-" + nomempresa;
                TxtUser.Text = SiaWin._UserAlias;


                GridConfig.SelectionController = new GridSelectionControllerExt(GridConfig);

            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }


        public class GridSelectionControllerExt : GridSelectionController
        {
            private SfDataGrid grid;
            public GridSelectionControllerExt(SfDataGrid datagrid) : base(datagrid)
            {
                grid = datagrid;
            }
            protected override void ProcessKeyDown(KeyEventArgs args)
            {
                try
                {
                    var currentKey = args.Key;
                    var arguments = new KeyEventArgs(args.KeyboardDevice, args.InputSource, args.Timestamp, Key.Tab)
                    {
                        RoutedEvent = args.RoutedEvent
                    };
                    if (currentKey == Key.Enter)
                    {
                        if (grid.IsReadOnly == false && grid.CurrentColumn is GridTextColumn) { }
                        base.ProcessKeyDown(arguments);
                        args.Handled = arguments.Handled;
                        return;
                    }

                    if (currentKey == Key.Up)
                    {
                        if (grid.View.IsAddingNew == true && grid.View.IsCurrentBeforeFirst == true)
                        {
                            grid.View.CancelEdit();
                            grid.View.CancelNew();
                        }
                        grid.UpdateLayout();
                    }


                    base.ProcessKeyDown(args);
                }
                catch (Exception w)
                {
                    MessageBox.Show("errro:::" + w);
                }
            }
        }

        void MoveToNextUIElement(KeyEventArgs e)
        {
            try
            {
                FocusNavigationDirection focusDirection = FocusNavigationDirection.Next;
                TraversalRequest request = new TraversalRequest(focusDirection);
                UIElement elementWithFocus = Keyboard.FocusedElement as UIElement;
                if (elementWithFocus != null)
                    if (elementWithFocus.MoveFocus(request)) e.Handled = true;
            }
            catch (Exception w)
            {
                MessageBox.Show("error :" + w);
            }

        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //if (BtbGrabar.Content.ToString().Trim() == "Nuevo") return;



            if (e.Key == Key.F5 && Tab1.IsSelected == true)
            {
                if (BtbGrabar.Content.ToString().Trim() == "Grabar")
                {
                    BtbGrabar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    return;
                }
            }
            else
            {
                if (e.Key == Key.F5 && Btn_Save.Content.ToString().Trim() == "Guardar")
                {
                    Btn_Save.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    return;
                }
            }

            if (e.Key == Key.F9)
            {
                if (dtCue.Rows.Count > 0)
                {
                    if (MessageBox.Show("Usted desea cruzar todos los documentos ?", "Cruzar pagos", MessageBoxButton.OK, MessageBoxImage.Exclamation) == MessageBoxResult.No) return;
                    foreach (System.Data.DataRow dr in dtCue.Rows)
                    {
                        double _saldo = Convert.ToDouble(dr["saldo"].ToString());
                        dr.BeginEdit();
                        dr["abono"] = _saldo;
                        dr.EndEdit();
                    }
                    (sender as SfDataGrid).UpdateLayout();



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

                if (Tab1.IsSelected == true)
                {
                    if (BtbGrabar.Content.ToString().Trim() == "Grabar")
                    {
                        BtbCancelar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        e.Handled = false;
                        return;
                    }
                }


                if (Tab2.IsSelected == true)
                {
                    if (Btn_Save.Content.ToString().Trim() == "Guardar")
                    {
                        Btn_Cancel.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        e.Handled = false;
                        return;
                    }
                }
            }

        }




        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            getCombBanc();

        }

        public void getCombBanc()
        {
            try
            {
                dtBanco = SiaWin.Func.SqlDT("select * from comae_ban", "bancos", idemp);
                CbBanco.ItemsSource = dtBanco.DefaultView;
                CbBanco.DisplayMemberPath = "nom_ban";
                CbBanco.SelectedValuePath = "cod_ban";
                CbBanco.SelectedIndex = 1;

                ComBo_Banco.ItemsSource = dtBanco.DefaultView;
                ComBo_Banco.DisplayMemberPath = "nom_ban";
                ComBo_Banco.SelectedValuePath = "cod_ban";
                ComBo_Banco.SelectedIndex = 1;

            }
            catch (Exception w)
            {
                MessageBox.Show(w.Message);
            }
        }


        public void ActivaDesactivaControles(int estado)
        {
            if (estado == 0)
            {
                TextCodeCliente.Text = string.Empty;
                TextNomCliente.Text = string.Empty;
                TextNumeroDoc.Text = string.Empty;
                CbTrans.IsEnabled = false;
                TxtCheque.Text = "";
                TXotroTer.Text = "";
                TextNota.Text = "";
                BtbGrabar.Content = "Nuevo";
                BtbCancelar.Content = "Salir";
                dataGrid.AllowEditing = true;
                dtCue.Clear();

                TextReteIva.Text = "0,00";
                TextRetefte.Text = "0,00";
                TextIca.Text = "0,00";
                txDes.Text = "0,00";
                TextVlrRecibido.Text = "0,00";

                Cta_ref.Text = "";
                Cta_Riva.Text = "";
                Cta_rivaDT.Text = "";
                CtaRica.Text = "";


                TextCodeCliente.Focusable = false;
                TextNomCliente.Focusable = false;
                TextNota.Focusable = false;
                TXotroTer.Focusable = false;
                CbBanco.Focusable = false;
                CbTrans.Focusable = false;
                DtFec.Focusable = false;
                TxtCheque.Focusable = false;




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


            }
            if (estado == 1) //creando
            {
                TextCodeCliente.Text = string.Empty;
                TextNomCliente.Text = string.Empty;
                TextNumeroDoc.Text = "";
                CbTrans.IsEnabled = true;
                DtFec.Text = DateTime.Now.ToString();

                BtbGrabar.Content = "Grabar";
                BtbCancelar.Content = "Cancelar";
                dataGrid.AllowEditing = false;
                dtCue.Clear();
                dataGrid.UpdateLayout();
                TextCodeCliente.Focusable = true;


                TextCodeCliente.Focusable = true;
                TextNomCliente.Focusable = true;
                TextNota.Focusable = true;
                TXotroTer.Focusable = true;
                CbBanco.Focusable = true;
                CbTrans.Focusable = true;
                DtFec.Focusable = true;
                TxtCheque.Focusable = true;


                TextNumeroDoc.Text = consecutivo();

                TextCodeCliente.Focusable = true;
                TextRetefte.Text = "0,00";
                TextIca.Text = "0,00";
                TextVlrRecibido.Text = "0,00";

                TextNota.Text = "";
                TXotroTer.Text = "";
                TxtCheque.Text = "";

                Cta_ref.Text = "";
                Cta_Riva.Text = "";
                Cta_rivaDT.Text = "";
                CtaRica.Text = "";

                TextCodeCliente.Focus();

            }
        }

        public string consecutivo()
        {
            string con = "";
            try
            {
                string sqlConsecutivo = @"declare @fecdoc as datetime;set @fecdoc = getdate(); ";
                sqlConsecutivo += "declare @fecdocsecond as datetime;set @fecdocsecond = DATEADD(second,1,GETDATE()); ";
                sqlConsecutivo += "declare @ini as char(4);declare @num as varchar(12);  ";
                sqlConsecutivo += "declare @iConsecutivo char(12) = '' ;declare @iFolioHost int = 0; ";
                sqlConsecutivo += "SELECT @iFolioHost= isnull(num_act,0)+1,@ini=rtrim(inicial) FROM Comae_trn WHERE cod_trn='02'; ";
                sqlConsecutivo += "set @num=@iFolioHost ";
                sqlConsecutivo += "select @iConsecutivo=rtrim(@ini)+'-'+REPLICATE ('0',11-len(rtrim(@ini))-len(rtrim(convert(varchar,@num))))+rtrim(convert(varchar,@num)); ";
                sqlConsecutivo += "select @iConsecutivo as consecutivo;  ";

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

        private void Tx_ter_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tercero = (sender as TextBox);
            validTer(tercero);

        }

        public void validTer(TextBox ter)
        {
            try
            {
                if (ter.Text.Length > 0)
                {
                    var tp = getTercero(ter.Text);
                    if (tp.Item1 == false)
                    {
                        MessageBox.Show("el tercero ingresado no existe ingrese uno nuevamente");
                        TextCodeCliente.Text = "";
                        int idr = 0; string code = ""; string nombre = "";
                        dynamic xx = SiaWin.WindowBuscar("comae_ter", "cod_ter", "nom_ter", "nom_ter", "idrow", "Maestra de clientes", cnEmp, false, "", idEmp: idemp);
                        xx.ShowInTaskbar = false;
                        xx.Owner = Application.Current.MainWindow;
                        xx.Height = 400;
                        xx.ShowDialog();
                        idr = xx.IdRowReturn;
                        code = xx.Codigo;
                        nombre = xx.Nombre;
                        xx = null;
                        if (idr > 0)
                        {
                            if (ter.Name == "TextCodeCliente") { TextCodeCliente.Text = code; TextNomCliente.Text = nombre; ConsultaSaldoCartera(); }
                            else { tx_Clie.Text = code; Tx_NomCli.Text = nombre; }
                        }

                    }
                    else
                    {
                        if (tp.Item1 == true && ter.Name == "TextCodeCliente") ConsultaSaldoCartera();

                        if (ter.Name == "TextCodeCliente") TextNomCliente.Text = tp.Item2;
                        else Tx_NomCli.Text = tp.Item2;
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error ww" + w);
            }
        }


        public Tuple<bool, string> getTercero(string ter)
        {
            bool flag = false;
            string select = "select * from comae_ter where cod_ter='" + ter + "'";
            DataTable dt = SiaWin.Func.SqlDT(select, "tercero", SiaWin._BusinessId);
            if (dt.Rows.Count > 0) flag = true;
            string nombre = dt.Rows.Count > 0 ? dt.Rows[0]["nom_ter"].ToString() : "";
            var tuple = new Tuple<bool, string>(flag, nombre);
            return tuple;
        }

        private void ConsultaSaldoCartera()
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();

                cmd = new SqlCommand("_empSpCoAnalisisCxc", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Ter", TextCodeCliente.Text.Trim());
                cmd.Parameters.AddWithValue("@Cta", "");
                cmd.Parameters.AddWithValue("@TipoApli", -1);
                cmd.Parameters.AddWithValue("@Resumen", 1);
                cmd.Parameters.AddWithValue("@Fecha", DateTime.Now.ToString());
                cmd.Parameters.AddWithValue("@TrnCo", "");
                cmd.Parameters.AddWithValue("@NumCo", "");
                cmd.Parameters.AddWithValue("@Cco", "");
                cmd.Parameters.AddWithValue("@codemp", cod_empresa);
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
                }

                dataGrid.ItemsSource = dtCue.DefaultView;
            }
            catch (Exception W)
            {
                MessageBox.Show("Actualiza Grid www:" + W);
            }
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

                double.TryParse(dtCue.Compute("Sum(saldo)", "tip_apli=3").ToString(), out saldoCxC);
                double.TryParse(dtCue.Compute("Sum(saldo)", "tip_apli=4").ToString(), out saldoCxCAnt);
                double.TryParse(dtCue.Compute("Sum(saldo)", "tip_apli=1").ToString(), out saldoCxP);
                double.TryParse(dtCue.Compute("Sum(saldo)", "tip_apli=2").ToString(), out saldoCxPAnt);

                TextCxC.Text = saldoCxC.ToString("C");
                TextCxCAnt.Text = saldoCxCAnt.ToString("C");
                TextCxP.Text = saldoCxP.ToString("C");
                TextCxPAnt.Text = saldoCxPAnt.ToString("C");

                TextCxCAbono.Text = abonoCxC.ToString("C");
                TextCxCAntAbono.Text = abonoCxCAnt.ToString("C");
                TextCxPAbono.Text = abonoCxP.ToString("C");
                TextCxPAntAbono.Text = abonoCxPAnt.ToString("C");
                TextCxCSaldo.Text = (saldoCxC - abonoCxC).ToString("C");

                TextCxCAntSaldo.Text = (saldoCxCAnt - abonoCxCAnt).ToString("C");
                TextCxPSaldo.Text = (saldoCxP - abonoCxP).ToString("C");
                TextCxPAntSaldo.Text = (saldoCxPAnt - abonoCxPAnt).ToString("C");
                TotalCxc.Text = (valorCxC - valorCxCAnt - valorCxP + valorCxPAnt).ToString("C");
                TotalAbono.Text = (abonoCxC - abonoCxCAnt - abonoCxP + abonoCxPAnt).ToString("C");
                TotalSaldo.Text = ((valorCxC - valorCxCAnt - valorCxP + valorCxPAnt) - (abonoCxC - abonoCxCAnt - abonoCxP + abonoCxPAnt)).ToString("C"); ;



                double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=3").ToString(), out abonoCxC);
                double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=4").ToString(), out abonoCxCAnt);
                double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=1").ToString(), out abonoCxP);
                double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=2").ToString(), out abonoCxPAnt);

                Reteica = Convert.ToDouble(TextIca.Value);
                Reteiva = Convert.ToDouble(TextReteIva.Value);
                Retefte = Convert.ToDouble(TextRetefte.Value);
                Descuento = Convert.ToDouble(txDes.Value);

                #region totales otros
                TextCxCAbono.Text = abonoCxC.ToString("C");
                TextCxCAntAbono.Text = abonoCxCAnt.ToString("C");
                TextCxPAbono.Text = abonoCxP.ToString("C");
                TextCxPAntAbono.Text = abonoCxPAnt.ToString("C");

                TextCxCSaldo.Text = (saldoCxC - abonoCxC).ToString("C");

                TextCxCAntSaldo.Text = (saldoCxCAnt - abonoCxCAnt).ToString("C");
                TextCxPSaldo.Text = (saldoCxP - abonoCxP).ToString("C");
                TextCxPAntSaldo.Text = (saldoCxPAnt - abonoCxPAnt).ToString("C");
                TotalCxc.Text = (valorCxC - valorCxCAnt - valorCxP + valorCxPAnt).ToString("C");
                TotalAbono.Text = (abonoCxC - abonoCxCAnt - abonoCxP + abonoCxPAnt).ToString("C");
                TotalSaldo.Text = ((valorCxC - valorCxCAnt - valorCxP + valorCxPAnt) - (abonoCxC - abonoCxCAnt - abonoCxP + abonoCxPAnt)).ToString("C");
                //TotalRecaudo.Text = (abonoCxC - abonoCxCAnt - abonoCxP + abonoCxPAnt - Retefte - Reteica - Reteiva - Descuento).ToString("C");
                #endregion

                double ret = Retefte + Reteica + Descuento;
                //double operation = (cxpSum + cxcantSum - cxcSum - cxpantSum) - (ret);
                double operation = (abonoCxP + abonoCxCAnt - abonoCxC - abonoCxPAnt) - (ret);

                TotalRecaudo.Text = operation.ToString("C");
            }
            catch (Exception W)
            {
                MessageBox.Show("sUMA DE ABONOS www:" + W);
            }
        }

        private void Tx_ter_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.F8 || e.Key == Key.Enter)
            {
                if (string.IsNullOrEmpty((sender as TextBox).Text))
                {
                    int idr = 0; string code = ""; string nombre = "";
                    dynamic xx = SiaWin.WindowBuscar("comae_ter", "cod_ter", "nom_ter", "nom_ter", "idrow", "Maestra de clientes", cnEmp, false, "", idEmp: idemp);
                    xx.ShowInTaskbar = false;
                    xx.Owner = Application.Current.MainWindow;
                    xx.Height = 400;
                    xx.ShowDialog();
                    idr = xx.IdRowReturn;
                    code = xx.Codigo;
                    nombre = xx.Nombre;
                    xx = null;
                    if (idr > 0)
                    {
                        if ((sender as TextBox).Name == "TextCodeCliente") { TextCodeCliente.Text = code; TextNomCliente.Text = nombre; ConsultaSaldoCartera(); }
                        else { tx_Clie.Text = code; Tx_NomCli.Text = nombre; }
                    }
                }
            }
        }

        private void DataGrid_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
        {
            try
            {
                GridNumericColumn Colum = ((SfDataGrid)sender).CurrentColumn as GridNumericColumn;

                if (Colum.MappingName == "abono")
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

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.F8)
                {
                    GridNumericColumn Colum = ((SfDataGrid)sender).CurrentColumn as GridNumericColumn;
                    if (Colum.MappingName == "abono")
                    {
                        System.Data.DataRow dr = dtCue.Rows[dataGrid.SelectedIndex];
                        dr.BeginEdit();
                        //double reduccion = (Retefte + Reteiva + Reteica + Descuento);
                        double saldo = Convert.ToDouble(dr["saldo"].ToString());
                        VlrSaldo = saldo;
                        dr["abono"] = VlrSaldo;

                        dr.EndEdit();
                        e.Handled = true;
                    }
                    dataGrid.UpdateLayout();

                    sumaAbonos();
                }
                if (e.Key == Key.F3)
                {
                    GridNumericColumn Colum = ((SfDataGrid)sender).CurrentColumn as GridNumericColumn;
                    if (Colum.MappingName == "abono")
                    {
                        System.Data.DataRow dr = dtCue.Rows[dataGrid.SelectedIndex];
                        dr.BeginEdit();
                        double reduccion = (Retefte + Reteiva + Reteica + Descuento);
                        double saldo = Convert.ToDouble(dr["saldo"].ToString());
                        VlrSaldo = saldo - reduccion;

                        string tipo = dr["abono"].ToString();
                        MessageBox.Show("tipo:" + tipo);


                    }
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("11: F8" + w);
            }

        }

        private void Cta_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.F8)
                {
                    int idr = 0; string code = ""; string nombre = "";
                    dynamic xx = SiaWin.WindowBuscar("Comae_cta", "cod_cta", "nom_cta", "nom_cta", "idrow", "Maestra de cuentas", cnEmp, false, "", idEmp: idemp);
                    xx.ShowInTaskbar = false;
                    xx.Owner = Application.Current.MainWindow;
                    xx.Height = 400;
                    xx.ShowDialog();
                    idr = xx.IdRowReturn;
                    code = xx.Codigo;
                    nombre = xx.Nombre;
                    xx = null;
                    if (idr > 0)
                        (sender as TextBox).Text = code;

                    if (string.IsNullOrEmpty(code)) e.Handled = false;
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir cuentas:" + w);
            }
        }

        public bool whileCuent()
        {
            bool flag = true;

            double val_rfte = Convert.ToDouble(TextRetefte.Value);
            if (val_rfte > 0) if (string.IsNullOrEmpty(Cta_ref.Text)) flag = false;

            double val_riva = Convert.ToDouble(TextReteIva.Value);
            if (val_riva > 0) if (string.IsNullOrEmpty(Cta_Riva.Text) || string.IsNullOrEmpty(Cta_rivaDT.Text)) flag = false;

            double val_rica = Convert.ToDouble(TextIca.Value);
            if (val_rica > 0) if (string.IsNullOrEmpty(CtaRica.Text)) flag = false;

            return flag;
        }

        private void BtbGrabar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BtbGrabar.Content.ToString() == "Nuevo")
                {
                    ActivaDesactivaControles(1);
                }
                else
                {

                    string _CodeCliente = TextCodeCliente.Text;
                    if (string.IsNullOrEmpty(_CodeCliente))
                    {
                        MessageBox.Show("Falta Nit/cc del cliente..");
                        TextCodeCliente.Focus();
                        return;
                    }
                    if (CbBanco.SelectedIndex < 0)
                    {
                        MessageBox.Show("Seleccione Vendedor.....");
                        CbBanco.Focus();
                        return;
                    }
                    if (CbTrans.SelectedIndex < 0)
                    {
                        MessageBox.Show("Seleccione SI o NO en transferencia.....");
                        CbBanco.Focus();
                        return;
                    }
                    if (dtCue.Rows.Count == 0)
                    {
                        MessageBox.Show("No hay registros en el cuerpo de documentos...");
                        TextCodeCliente.Focus();
                        return;
                    }

                    var valor = TotalRecaudo.Text;
                    decimal TotalPag = decimal.Parse(valor, NumberStyles.Currency);
                    if (TotalPag <= 0)
                    {
                        MessageBox.Show("el total a pagar tiene que ser positivo");
                        return;
                    }



                    if (whileCuent() == false)
                    {
                        MessageBox.Show("llene todos los campo de las cuentas respectivamente");
                        return;
                    }


                    if (MessageBox.Show("Usted desea guardar el documento..?", "Guardar Recibo de Caja", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        try
                        {
                            int iddocumento = 0;

                            iddocumento = ExecuteSqlTransaction(_CodeCliente.ToString());

                            if (iddocumento <= 0) return;
                            if (iddocumento > 0)
                            {
                                MessageBox.Show("Egreso Generado");
                            }

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

        private int ExecuteSqlTransaction(string codter)
        {

            string TipoConsecutivo = "num_act";
            string codtrn = "02";
            using (SqlConnection connection = new SqlConnection(cnEmp))
            {
                connection.Open();
                StringBuilder errorMessages = new StringBuilder();
                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;
                transaction = connection.BeginTransaction("Transaction");
                command.Connection = connection;
                command.Transaction = transaction;

                try
                {
                    double bas_mov = Convert.ToDouble(TextVlrRecibido.Value);

                    string sqlConsecutivo = @"declare @fecdoc as datetime;set @fecdoc = getdate();";
                    sqlConsecutivo += "declare @ini as char(4);declare @num as varchar(12);declare @iConsecutivo char(12) = '' ;";
                    sqlConsecutivo += "declare @iFolioHost int = 0;";
                    sqlConsecutivo += "UPDATE Comae_trn SET " + TipoConsecutivo + " = ISNULL(" + TipoConsecutivo + ", 0) + 1  WHERE cod_trn='" + codtrn + "';";
                    sqlConsecutivo += "SELECT @iFolioHost = " + TipoConsecutivo + ",@ini=rtrim(inicial) FROM Comae_trn  WHERE cod_trn='" + codtrn + "';";
                    sqlConsecutivo += "set @num=@iFolioHost;";
                    sqlConsecutivo += "select @iConsecutivo=rtrim(@ini)+'-'+REPLICATE ('0',11-len(rtrim(@ini))-len(rtrim(convert(varchar,@num))))+rtrim(convert(varchar,@num));";

                    string sqlcab = sqlConsecutivo + @"INSERT INTO cocab_doc (cod_trn,fec_trn,num_trn,detalle,otro_ter,fec_posf) values ('" + codtrn + "',@fecdoc,@iConsecutivo,'" + TextNota.Text.Trim() + "','" + TXotroTer.Text + "','" + DtFec.Text + "');DECLARE @NewID INT;SELECT @NewID = SCOPE_IDENTITY();";
                    string sql = "";

                    foreach (System.Data.DataRow item in dtCue.Rows)
                    {

                        double abono = Convert.ToDouble(item["abono"].ToString());

                        if (abono > 0)
                        {
                            double saldo = Convert.ToDouble(item["saldo"].ToString());

                            int tipapli = Convert.ToInt32(item["tip_apli"].ToString());
                            //tipapli = 1-- cxp,//tipapli = 2 -- cxpant,//tipapli = 3 -- cxc,//tipapli = 4 -- cxcant

                            if (tipapli == 2 || tipapli == 3)
                            {
                                sql += @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,doc_cruc,doc_ref,bas_mov,cre_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + item["cod_cta"].ToString() + "','" + item["cod_cco"].ToString() + "','" + item["cod_ter"].ToString() + "','Pago/Abono credito Doc:" + item["num_trn"].ToString() + "','" + item["num_trn"].ToString() + "','" + item["num_trn"].ToString() + "'," + bas_mov.ToString("F", CultureInfo.InvariantCulture) + "," + abono.ToString("F", CultureInfo.InvariantCulture) + ");";
                            }
                            if (tipapli == 1 || tipapli == 4)
                            {
                                sql += @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,doc_cruc,doc_ref,bas_mov,deb_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + item["cod_cta"].ToString() + "','" + item["cod_cco"].ToString() + "','" + item["cod_ter"].ToString() + "','Pago/Abono debito Doc:" + item["num_trn"].ToString() + "','" + item["num_trn"].ToString() + "','" + item["num_trn"].ToString() + "'," + bas_mov.ToString("F", CultureInfo.InvariantCulture) + ", " + abono.ToString("F", CultureInfo.InvariantCulture) + ");";
                            }
                        }
                    }

                    if (Retefte > 0)
                    {
                        string cntRetefte = string.IsNullOrEmpty(Cta_ref.Text) ? "236540" : Cta_ref.Text;
                        sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,bas_mov,cre_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + cntRetefte + "','','" + codter.Trim() + "','ReteFte:" + codter + "'," + bas_mov.ToString("F", CultureInfo.InvariantCulture) + "," + Retefte.ToString("F", CultureInfo.InvariantCulture) + ");";
                    }
                    if (Reteica > 0)
                    {
                        string cntReteica = string.IsNullOrEmpty(CtaRica.Text) ? "237807" : CtaRica.Text;
                        sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,bas_mov,cre_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + cntReteica + "','','" + codter.Trim() + "','ReteIca" + codter + "'," + bas_mov.ToString("F", CultureInfo.InvariantCulture) + "," + Reteica.ToString("F", CultureInfo.InvariantCulture) + ");";
                    }
                    if (Reteiva > 0)
                    {
                        string cntReteivaDeb = string.IsNullOrEmpty(Cta_Riva.Text) ? "237715" : Cta_Riva.Text;
                        string cntReteivaCre = string.IsNullOrEmpty(Cta_rivaDT.Text) ? "237715" : Cta_rivaDT.Text;

                        sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,bas_mov,deb_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + cntReteivaDeb + "','','" + codter.Trim() + "','ReteIva DEB:" + codter + "'," + bas_mov.ToString("F", CultureInfo.InvariantCulture) + "," + Reteiva.ToString("F", CultureInfo.InvariantCulture) + ");";
                        sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,bas_mov,cre_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + cntReteivaCre + "','','" + codter.Trim() + "','ReteIva CRE:" + codter + "'," + bas_mov.ToString("F", CultureInfo.InvariantCulture) + "," + Reteiva.ToString("F", CultureInfo.InvariantCulture) + ");";
                    }


                    if (Descuento > 0)
                    {
                        string cntDescuento = "429505";
                        sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,bas_mov,cre_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + cntDescuento + "','','" + codter.Trim() + "','Descto:" + codter + "'," + bas_mov.ToString("F", CultureInfo.InvariantCulture) + "," + Descuento.ToString("F", CultureInfo.InvariantCulture) + ");";
                    }


                    string sqlban = "";

                    string slect = "select * from comae_ban where cod_ban='" + CbBanco.SelectedValue + "' ";

                    DataTable dt = SiaWin.Func.SqlDT(slect, "bancos", idemp);


                    if (dt.Rows.Count > 0)
                    {


                        string cta = dt.Rows[0]["cod_cta"].ToString();

                        double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=3").ToString(), out abonoCxC);
                        double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=4").ToString(), out abonoCxCAnt);
                        double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=1").ToString(), out abonoCxP);
                        double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=2").ToString(), out abonoCxPAnt);


                        double ret = Retefte + Reteica + Descuento;
                        double tot_pagar = (abonoCxP + abonoCxCAnt - abonoCxC - abonoCxPAnt) - (ret);

                        sqlban = sqlban + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,cre_mov,num_chq,bas_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + cta + "','','" + codter.Trim() + "','Pago/Abono:" + codter + "'," + tot_pagar.ToString("F", CultureInfo.InvariantCulture) + ",'" + TxtCheque.Text + "'," + bas_mov.ToString("F", CultureInfo.InvariantCulture) + ");";
                    }

                    string valor = (CbTrans.SelectedItem as ComboBoxItem).Content.ToString();
                    if (valor == "No")
                    {
                        sqlban = sqlban + @"UPDATE comae_ban SET  num_act=ISNULL(num_act, 0)+1  WHERE cod_ban='" + CbBanco.SelectedValue + "';";
                    }

                    command.CommandText = sqlcab + sql + sqlban + @"select CAST(@NewId AS int);";

                    var r = new object();
                    r = command.ExecuteScalar();
                    transaction.Commit();
                    connection.Close();
                    return Convert.ToInt32(r.ToString());
                }
                catch (SqlException ex)
                {
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        errorMessages.Append(" SQL-Index #" + i + "\n" + "Message: " + ex.Errors[i].Message + "\n" + "LineNumber: " + ex.Errors[i].LineNumber + "\n" + "Source: " + ex.Errors[i].Source + "\n" + "Procedure: " + ex.Errors[i].Procedure + "\n");
                    }
                    transaction.Rollback();
                    MessageBox.Show(errorMessages.ToString());
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

        private void BtbCancelar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BtbCancelar.Content.ToString() == "Cancelar")
                {
                    if (dtCue.Rows.Count > 0)
                    {
                        if (MessageBox.Show("Usted desea aaa ........?", "Cancelar Recibo de Caja", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
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

        private void ActualizaTotal(object sender, RoutedEventArgs e)
        {
            sumaAbonos();
        }

        private void CbTrans_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string valor = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content.ToString();
            ComboBox cb = (sender as ComboBox);
            ComboBox cb_bancos = new ComboBox();

            cb_bancos = cb.Name == "CbTrans" ? (ComboBox)this.FindName("CbBanco") : (ComboBox)this.FindName("ComBo_Banco");
            if (valor == "No")
            {
                if (cb_bancos.SelectedIndex >= 0)
                {
                    string consecutivo = numero_cheque(cb_bancos.SelectedValue.ToString().Trim());
                    if (cb.Name == "CbTrans") TxtCheque.Text = consecutivo;
                    else Tx_Cheque.Text = consecutivo;
                }
            }
            else
                if (cb.Name == "CbTrans") TxtCheque.Text = ""; else Tx_Cheque.Text = "";

        }

        public string numero_cheque(string banco)
        {
            string con = "";
            string select = "select ISNULL(num_act,0)+1 as consecutivo from comae_ban where cod_ban = '" + banco + "'";
            DataTable dt = SiaWin.Func.SqlDT(select, "consecutivo", idemp);
            if (dt.Rows.Count > 0) con = dt.Rows[0]["consecutivo"].ToString();
            return con;
        }

        private void BtnGetDocument_Click(object sender, RoutedEventArgs e)
        {
            DataRowView GridCab = (DataRowView)dataGrid.SelectedItems[0];
            string num_trn = GridCab["num_trn"].ToString();

            ViewDocument view = new ViewDocument();
            view.document = num_trn;
            view.ShowInTaskbar = false;
            view.Owner = Application.Current.MainWindow;
            view.ShowDialog();
        }

        private void cuenta_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                string cnt = (sender as TextBox).Text.Trim();
                if (string.IsNullOrEmpty(cnt) || cnt == "") return;

                bool ban = GetCuentas(cnt);

                if (ban == false)
                {
                    MessageBox.Show("la cuenta ingresada no existe");
                    (sender as TextBox).Text = "";
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error en el lost focus:" + w);
            }
        }

        public bool GetCuentas(string cnt)
        {
            bool flag = false;
            DataTable dt = SiaWin.Func.SqlDT("select * from Comae_cta where cod_cta='" + cnt + "'", "cuenta", idemp);
            if (dt.Rows.Count > 0) flag = true;
            return flag;
        }
        // ----------------- tab 2 ----------------------------------------------------------------------

        //public double sum_deb { get; set; }
        //public double sum_cre { get; set; }
        //public double diferencia { get; set; }



        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            if (Btn_Save.Content.ToString() == "Nuevo")
            {

                act(2);
                if (dt_egreso.Rows.Count == 0)
                {
                    dt_egreso.Rows.Add("", "", "", "Ninguno", "", 0, 0, 0);
                }
                tx_Clie.Focus();
            }
            else
            {

                if (val_cam() == false)
                {
                    MessageBox.Show("llene todos los campos ingresados");
                    return;
                }

                if (valDebCre() == false)
                {
                    MessageBox.Show("la sumatoria total de los debitos debe ser mayor a las sumatoria total de los creditos");
                    return;
                }

                if (MessageBox.Show("usted desea generar el egreso sin causacion", "generar", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    int id_doc = DocumentEgeCaus();
                    if (id_doc > 0)
                    {
                        MessageBox.Show("egreso sin causacion generado exitosamente", "transaccion exitosa", MessageBoxButton.OK, MessageBoxImage.None);
                        act(1);
                    }

                }

            }
        }



        public int gerMontGru(string grupo)
        {
            int mes = 0;
            DataTable dt = ((Inicio)Application.Current.MainWindow).Func.SqlDT("select mes_dep from afmae_gru where cod_gru='" + grupo + "' ", "grupo", _trn.BusinessId);
            if (dt.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(dt.Rows[0]["mes_dep"].ToString()) && dt.Rows[0]["mes_dep"] != DBNull.Value)
                    Convert.ToInt32(mes);
            }
            return mes;
        }




        public void moveValueActi(bool edit_save)
        {

            DateTime _fecdoc = Convert.ToDateTime(_trn.dsDoc.Tables["Cab"].Rows[0]["fec_trn"].ToString().Trim());
            string query = "select * from afbases_a where año='" + _fecdoc.Year.ToString() + "'  ";
            DataTable dt_base = ((Inicio)Application.Current.MainWindow).Func.SqlDT(query, "anobase", _trn.BusinessId);
            decimal slm = dt_base.Rows.Count > 0 ? Convert.ToDecimal(dt_base.Rows[0]["smlv"]) : 0;
            decimal n_slm = dt_base.Rows.Count > 0 ? Convert.ToDecimal(dt_base.Rows[0]["n_smlv"]) : 0;
            decimal valor = slm * n_slm;

            foreach (System.Data.DataRow dr in _trn.dsDoc.Tables["Cue"].Rows)
            {
                if (edit_save == true)
                {
                    decimal vr_act = Convert.ToDecimal(dr["vr_act"]);
                    if (vr_act <= valor)
                    {
                        dr["vr_mc"] = dr["vr_act"];
                        dr["vr_act"] = 0;
                    }
                    else
                    {
                        dr["vr_mc"] = 0;
                    }
                }
                else
                {
                    decimal vr_mc = Convert.ToDecimal(dr["vr_mc"]);
                    if (vr_mc > 0)
                    {
                        dr["vr_act"] = vr_mc;
                    }
                }


            }

        }


        public bool ValidNameGroup()
        {
            bool flag = false;
            foreach (System.Data.DataRow dr in _trn.dsDoc.Tables["Cue"].Rows)
            {
                string nom_act = dr["nom_act"].ToString().Trim();
                string cod_gru = dr["cod_gru"].ToString().Trim();
                if (string.IsNullOrEmpty(nom_act) || string.IsNullOrEmpty(cod_gru)) flag = true;
            }
            return flag;
        }



        public DataTable SaldoActivo(string cod_act, string fecha, int IdBuss)
        {
            //Consecutivo(4,3,1);
            DataTable dt = new DataTable();
            try
            {
                string cn = null;

                if (IdBuss <= 0) cn = ConfiguracionApp();
                if (IdBuss > 0) cn = DatosEmp(IdBuss);

                SqlConnection _conn = new SqlConnection(cn);
                _conn.Open();
                SqlCommand cmd = new SqlCommand("_EmpSaldosActivos", _conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@cod_act", cod_act);
                cmd.Parameters.AddWithValue("@fec_trn", fecha);
                SqlDataAdapter adapter = new SqlDataAdapter();

                adapter.SelectCommand = cmd;
                adapter.Fill(dt);

                _conn.Close();
                return dt;
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
            catch (System.Exception _error)
            {
                MessageBox.Show(_error.Message + "-" + _error.InnerException.Message);
                return null;
            }
        }


        //////// CONTABILIZACION COMPRAS Y DEVOLUCIONES ///////////////

        private int ContabilizaCompraAf(string idreg, string trn)
        {
            int idregreturn = -1;
            try
            {
                bool bandera = false;

                #region obtiene datos principales                
                string query = "select Afcab_doc.cod_trn,Afcab_doc.num_trn,Afmae_trn.cod_tdo from Afcab_doc  ";
                query += "inner join Afmae_trn on Afmae_trn.cod_trn = Afcab_doc.cod_trn ";
                query += "where idreg ='" + idreg + "' ";

                DataTable dt_trn = ((Inicio)Application.Current.MainWindow).Func.SqlDT(query, "cuerpo", _trn.BusinessId);

                string cod_trn_af = dt_trn.Rows.Count > 0 ? dt_trn.Rows[0]["cod_trn"].ToString().Trim() : "";
                string cod_trn_co = dt_trn.Rows.Count > 0 ? dt_trn.Rows[0]["cod_tdo"].ToString().Trim() : "";
                string num_trn_co = dt_trn.Rows.Count > 0 ? dt_trn.Rows[0]["num_trn"].ToString().Trim() : "";

                #endregion

                #region obtiene cuerpo
                string sqlcue_grupo = "";
                string sqlcue_concepto = "";

                string querycue = "select cuerpo.cod_act,cuerpo.vr_act,cuerpo.vr_mc,cuerpo.cod_ter,cuerpo.doc_int,  ";
                querycue += "cuerpo.cod_gru,grupo.cta_act,grupo.cta_gasmc,grupo.cta_gasmcp, ";
                querycue += "cuerpo.cod_con,concepto.ind_cor,concepto.cta_afec,cta_gan,cta_per,cta_ord,cta_orc,cuerpo.cod_res,cuerpo.vr_rep ";
                querycue += "from Afcab_doc as cabeza ";
                querycue += "inner join Afcue_doc as cuerpo on cuerpo.idregcab = cabeza.idreg ";
                querycue += "inner join Afmae_gru as grupo on grupo.cod_gru = cuerpo.cod_gru ";
                querycue += "inner join Afing_ret as concepto on concepto.cod_con = cuerpo.cod_con ";
                querycue += "where cabeza.idreg='" + idreg + "' ";


                DataTable dt_cuerpo = ((Inicio)Application.Current.MainWindow).Func.SqlDT(querycue, "cuerpo", _trn.BusinessId);

                foreach (System.Data.DataRow item in dt_cuerpo.Rows)
                {
                    decimal valor_act = Convert.ToDecimal(item["vr_act"]);
                    decimal vr_mc = Convert.ToDecimal(item["vr_mc"]);
                    decimal valor = vr_mc > 0 ? vr_mc : valor_act;


                    decimal vr_rep = Convert.ToDecimal(item["vr_rep"]);
                    string cod_act = item["cod_act"].ToString().Trim();
                    string cta_con = item["cod_con"].ToString().Trim();

                    string cta_gru = cta_con == "04" ? item["cta_gasmcp"].ToString().Trim() : cta_gru = item["cta_gasmc"].ToString().Trim();


                    string cod_ter = item["cod_ter"].ToString().Trim();
                    string doc_int = item["doc_int"].ToString().Trim();
                    //concepto 05
                    string cod_res = item["cod_res"].ToString().Trim();
                    string cta_ord = item["cta_ord"].ToString().Trim();
                    string cta_orc = item["cta_orc"].ToString().Trim();

                    string deb_cre_gru = cod_trn_af == "001" || cod_trn_af == "002" ? "deb_mov" : "cre_mov";
                    sqlcue_grupo += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,num_chq,des_mov," + deb_cre_gru + ") values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_gru + "','" + cod_ter + "','" + doc_int + "','COMPRA :" + cod_act + "'," + valor.ToString("F", CultureInfo.InvariantCulture) + "); ";


                    //string deb_cre_con = cod_trn_af == "001" || cod_trn_af == "002" ? "cre_mov" : "deb_mov";
                    //sqlcue_concepto += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,num_chq,des_mov," + deb_cre_con + ") values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_con + "','" + cod_ter + "','" + doc_int + "','CXP/" + cod_trn_af + "/" + num_trn_co + "'," + valor.ToString("F", CultureInfo.InvariantCulture) + "); ";


                    bool ind_cor = Convert.ToBoolean(item["ind_cor"]);
                    if (ind_cor == true)
                    {
                        //aqui se lleva las otras cuentas de orden con los saldos depreciados del activo    
                        string debcre_ord = cod_trn_af == "001" ? "cre_mov" : "deb_mov";
                        sqlcue_grupo += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,num_chq,des_mov," + debcre_ord + ") values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_ord + "','" + cod_res + "','" + doc_int + "','CXP/" + cod_trn_af + "/" + num_trn_co + "'," + vr_rep.ToString("F", CultureInfo.InvariantCulture) + "); ";

                        string debcre_orc = cod_trn_af == "001" ? "deb_mov" : "cre_mov";
                        sqlcue_concepto += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,num_chq,des_mov," + debcre_orc + ") values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_orc + "','" + cod_res + "','" + doc_int + "','CXP/" + cod_trn_af + "/" + num_trn_co + "'," + vr_rep.ToString("F", CultureInfo.InvariantCulture) + "); ";

                    }

                }

                System.Data.DataTable CuentaCxp = new System.Data.DataTable();
                if (dt_cuerpo.Rows.Count > 0)
                {
                    CuentaCxp = dt_cuerpo.AsEnumerable()
                        .GroupBy(a => a["cod_con"].ToString().Trim())
                        .Select(c =>
                        {
                            var row = dt_cuerpo.NewRow();
                            row["cod_con"] = c.Key;
                            row["cta_afec"] = c.Max(a => a.Field<string>("cta_afec"));
                            row["cod_ter"] = c.Max(a => a.Field<string>("cod_ter"));
                            row["doc_int"] = c.Max(a => a.Field<string>("doc_int"));
                            row["vr_act"] = c.Sum(a => a.Field<decimal>("vr_act"));
                            row["vr_mc"] = c.Sum(a => a.Field<decimal>("vr_mc"));
                            return row;
                        }).CopyToDataTable();
                }


                if (CuentaCxp.Rows.Count > 0)
                {
                    foreach (System.Data.DataRow dr in CuentaCxp.Rows)
                    {
                        string cta_con = dr["cta_afec"].ToString().Trim();
                        string cod_ter = dr["cod_ter"].ToString().Trim();
                        string doc_int = dr["doc_int"].ToString().Trim();

                        decimal valor_act = Convert.ToDecimal(dr["vr_act"]);
                        decimal vr_mc = Convert.ToDecimal(dr["vr_mc"]);
                        decimal valor = vr_mc > 0 ? vr_mc : valor_act;

                        string deb_cre_con = cod_trn_af == "001" || cod_trn_af == "002" ? "cre_mov" : "deb_mov";
                        sqlcue_concepto += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,num_chq,des_mov," + deb_cre_con + ") values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_con + "','" + cod_ter + "','" + doc_int + "','CXP/" + cod_trn_af + "/" + num_trn_co + "'," + valor.ToString("F", CultureInfo.InvariantCulture) + "); ";
                    }
                }
                //((Inicio)Application.Current.MainWindow).Browse(CuentaCxp);


                #endregion

                #region generar el documento contable
                using (SqlConnection connection = new SqlConnection(_trn.CnEmp))
                {

                    connection.Open();
                    StringBuilder errorMessages = new StringBuilder();
                    SqlCommand command = connection.CreateCommand();
                    SqlTransaction transaction;

                    transaction = connection.BeginTransaction("Transaction");
                    command.Connection = connection;
                    command.Transaction = transaction;
                    DateTime _fecdoc = Convert.ToDateTime(_trn.dsDoc.Tables["Cab"].Rows[0]["fec_trn"].ToString().Trim());

                    string sqlConsecutivo = @"declare @fecdoc as datetime;set @fecdoc = '" + _fecdoc.ToString() + "';declare @ini as char(4);DECLARE @NewTrn INT;";

                    //cabeza
                    string sqlcab001co = sqlConsecutivo + @" INSERT INTO cocab_doc (cod_trn,num_trn,fec_trn) values ('" + cod_trn_co + "','" + num_trn_co + "',@fecdoc);SELECT @NewTrn = SCOPE_IDENTITY();";

                    string sqlcue001co = sqlcue_grupo + sqlcue_concepto;
                    //sqlcue001co += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,bas_mov," + dc_ctaobra + ") values (@NewTrn,'" + TranConta + "','" + numero_trn + "','" + cta_obr + "','" + tercero + "',0," + _total.ToString("F", CultureInfo.InvariantCulture) + "); ";

                    command.CommandText = sqlcab001co + sqlcue001co + @"select CAST(@NewTrn AS int);";
                    //                    MessageBox.Show(command.CommandText.ToString());
                    var r = new object();
                    r = command.ExecuteScalar();
                    transaction.Commit();
                    connection.Close();
                    idregreturn = Convert.ToInt32(r.ToString());
                    if (idregreturn > 0) bandera = true;
                }
                #endregion


                #region crea ativos

                if (cod_trn_af == "000" || cod_trn_af == "001")
                {

                    string ins_updt_act = "";

                    string query_act = "select Afcue_doc.cod_act,Afcue_doc.nom_act,Afmae_act.cod_act as existe, ";
                    query_act += "Afcue_doc.vr_act,Afcue_doc.cod_gru,Afcue_doc.mes_dep,Afcue_doc.mesxdep,Afcue_doc.fec_adq,Afcue_doc.cod_loc ";
                    query_act += "from Afcue_doc  ";
                    query_act += "left join Afmae_act on Afmae_act.cod_act = Afcue_doc.cod_act ";
                    query_act += "where idregcab='" + idreg + "' ";

                    DataTable dt_act = ((Inicio)Application.Current.MainWindow).Func.SqlDT(query_act, "activo", _trn.BusinessId);

                    if (dt_act.Rows.Count > 0)
                    {
                        foreach (System.Data.DataRow item in dt_act.Rows)
                        {
                            string cod_act = item["cod_act"].ToString().Trim();
                            string nom_act = item["nom_act"].ToString().Trim();
                            string cod_gru = item["cod_gru"].ToString().Trim();
                            string mes_dep = item["mes_dep"].ToString().Trim();
                            string mesxdep = item["mesxdep"].ToString().Trim();
                            string fec_adq = item["fec_adq"].ToString().Trim();
                            string cod_loc = item["cod_loc"].ToString().Trim();
                            decimal vr_act = Convert.ToDecimal(item["vr_act"]);

                            string activo = item["existe"].ToString();
                            if (string.IsNullOrWhiteSpace(activo))
                            {
                                ins_updt_act += "insert into Afmae_act (cod_act,nom_act,cod_gru,vr_act,mes_dep,mesxdep,fec_adq,cod_loc) values ('" + cod_act + "','" + nom_act + "','" + cod_gru + "'," + vr_act.ToString("F", CultureInfo.InvariantCulture) + "," + mes_dep + "," + mesxdep + ",'" + fec_adq + "','" + cod_loc + "'); ";
                            }
                            else
                            {
                                ins_updt_act += "update Afmae_act set cod_act='" + cod_act + "',nom_act='" + nom_act + "',cod_gru='" + cod_gru + "',vr_act=" + vr_act.ToString("F", CultureInfo.InvariantCulture) + ",mes_dep=" + mes_dep + ",mesxdep=" + mesxdep + ",fec_adq='" + fec_adq + "',cod_loc='" + cod_loc + "' where  cod_act='" + cod_act + "';";
                            }
                        }

                        //                  MessageBox.Show("crea o actuliazar:"+ins_updt_act);

                        if (bandera == true && !string.IsNullOrWhiteSpace(ins_updt_act))
                        {
                            if (((Inicio)Application.Current.MainWindow).Func.SqlCRUD(ins_updt_act, _trn.BusinessId) == true)
                            {
                                //MessageBox.Show("inserto o guardo bien");
                            }
                        };
                    }

                }


                #endregion

                return idregreturn;
            }
            catch (Exception w)
            {
                MessageBox.Show("error en  el documento contable:" + w);
                return -1;
            }
        }






        private int ContabilizaTrasladoGrupo(string idreg, string trn)
        {
            int idregreturn = -1;
            try
            {

                #region obtiene datos principales                
                string query = "select Afcab_doc.cod_trn,Afcab_doc.num_trn,Afmae_trn.cod_tdo from Afcab_doc  ";
                query += "inner join Afmae_trn on Afmae_trn.cod_trn = Afcab_doc.cod_trn ";
                query += "where idreg ='" + idreg + "' ";

                DataTable dt_trn = ((Inicio)Application.Current.MainWindow).Func.SqlDT(query, "cuerpo", _trn.BusinessId);

                string cod_trn_af = dt_trn.Rows.Count > 0 ? dt_trn.Rows[0]["cod_trn"].ToString().Trim() : "";
                string cod_trn_co = dt_trn.Rows.Count > 0 ? dt_trn.Rows[0]["cod_tdo"].ToString().Trim() : "";
                string num_trn_co = dt_trn.Rows.Count > 0 ? dt_trn.Rows[0]["num_trn"].ToString().Trim() : "";
                DateTime _fecdoc = Convert.ToDateTime(_trn.dsDoc.Tables["Cab"].Rows[0]["fec_trn"].ToString().Trim());

                #endregion


                #region obtiene cuerpo

                string cuerpo_contable = "";

                string querycue = "select cuerpo.cod_act,activo.vr_act,cuerpo.vr_mc,cuerpo.doc_int, ";
                querycue += "cuerpo.cod_gru,grupo.cta_act as g_cta,cuerpo.gru_ant,grupo_ant.cta_act as gan_cta,grupo_ant.cta_dep,grupo_ant.cta_depant ";
                querycue += "from Afcab_doc as cabeza  ";
                querycue += "inner join Afcue_doc as cuerpo on cuerpo.idregcab = cabeza.idreg  ";
                querycue += "inner join Afmae_act as activo on cuerpo.cod_act = activo.cod_act ";
                querycue += "inner join Afmae_gru as grupo on grupo.cod_gru = cuerpo.cod_gru  ";
                querycue += "inner join Afmae_gru as grupo_ant on grupo_ant.cod_gru = cuerpo.gru_ant ";
                querycue += "where cabeza.idreg='" + idreg + "' ";


                DataTable dt_cuerpo = ((Inicio)Application.Current.MainWindow).Func.SqlDT(querycue, "cuerpo", _trn.BusinessId);

                foreach (System.Data.DataRow item in dt_cuerpo.Rows)
                {
                    decimal valor_act = Convert.ToDecimal(item["vr_act"]);
                    string cod_act = item["cod_act"].ToString().Trim();

                    string cod_gru = item["cod_gru"].ToString().Trim();
                    string gru_ant = item["gru_ant"].ToString().Trim();

                    string ctagru_nu = item["g_cta"].ToString().Trim();
                    string ctagru_an = item["gan_cta"].ToString().Trim();
                    string doc_int = item["doc_int"].ToString().Trim();

                    string cta_dep = item["cta_dep"].ToString().Trim();
                    string cta_depant = item["cta_depant"].ToString().Trim();

                    cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,des_mov,deb_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + ctagru_nu + "','" + doc_int + "','TRASLADO ACTIVO :" + cod_act + "'," + valor_act.ToString("F", CultureInfo.InvariantCulture) + "); ";
                    cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,des_mov,cre_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + ctagru_an + "','" + doc_int + "','GRUPO ANTIGUO/" + gru_ant + " GRUPO NUEVO/" + cod_gru + "'," + valor_act.ToString("F", CultureInfo.InvariantCulture) + "); ";

                    DataTable dt_depreciado = ((Inicio)Application.Current.MainWindow).Func.SaldoActivo(cod_act, _fecdoc.ToString("dd/MM/yyyy"), _trn.BusinessId);
                    if (dt_depreciado.Rows.Count > 0)
                    {
                        double depreciado = Convert.ToDouble(dt_depreciado.Rows[0]["depreciacion"]);
                        if (depreciado > 0)
                        {
                            cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,cod_ter,des_mov,deb_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_dep + "','" + doc_int + "','1','TRASLADO DEPRECIACION " + cod_act + "'," + depreciado.ToString("F", CultureInfo.InvariantCulture) + "); ";
                            cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,cod_ter,des_mov,cre_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_depant + "','" + doc_int + "','1','TRASLADO DEPRECIACION " + cod_act + "'," + depreciado.ToString("F", CultureInfo.InvariantCulture) + "); ";
                        }
                    }

                }
                #endregion


                #region generar el documento contable
                using (SqlConnection connection = new SqlConnection(_trn.CnEmp))
                {

                    connection.Open();
                    StringBuilder errorMessages = new StringBuilder();
                    SqlCommand command = connection.CreateCommand();
                    SqlTransaction transaction;

                    transaction = connection.BeginTransaction("Transaction");
                    command.Connection = connection;
                    command.Transaction = transaction;

                    string sqlConsecutivo = @"declare @fecdoc as datetime;set @fecdoc = '" + _fecdoc.ToString() + "';declare @ini as char(4);DECLARE @NewTrn INT;";

                    //cabeza
                    string sqlcab001co = sqlConsecutivo + @" INSERT INTO cocab_doc (cod_trn,num_trn,fec_trn) values ('" + cod_trn_co + "','" + num_trn_co + "',@fecdoc);SELECT @NewTrn = SCOPE_IDENTITY();";

                    string sqlcue001co = cuerpo_contable;

                    command.CommandText = sqlcab001co + sqlcue001co + @"select CAST(@NewTrn AS int);";
                    //MessageBox.Show(command.CommandText.ToString());
                    var r = new object();
                    r = command.ExecuteScalar();
                    transaction.Commit();
                    connection.Close();
                    idregreturn = Convert.ToInt32(r.ToString());
                }
                #endregion

                return idregreturn;
            }
            catch (Exception w)
            {
                MessageBox.Show("error en  el documento contable:" + w);
                return -1;
            }
        }


        private int ContabilizaRetiro(string idreg, string trn)
        {
            int idregreturn = -1;
            try
            {

                #region obtiene datos principales                
                string query = "select Afcab_doc.cod_trn,Afcab_doc.num_trn,Afmae_trn.cod_tdo from Afcab_doc  ";
                query += "inner join Afmae_trn on Afmae_trn.cod_trn = Afcab_doc.cod_trn ";
                query += "where idreg ='" + idreg + "' ";

                DataTable dt_trn = ((Inicio)Application.Current.MainWindow).Func.SqlDT(query, "cuerpo", _trn.BusinessId);

                string cod_trn_af = dt_trn.Rows.Count > 0 ? dt_trn.Rows[0]["cod_trn"].ToString().Trim() : "";
                string cod_trn_co = dt_trn.Rows.Count > 0 ? dt_trn.Rows[0]["cod_tdo"].ToString().Trim() : "";
                string num_trn_co = dt_trn.Rows.Count > 0 ? dt_trn.Rows[0]["num_trn"].ToString().Trim() : "";
                DateTime _fecdoc = Convert.ToDateTime(_trn.dsDoc.Tables["Cab"].Rows[0]["fec_trn"].ToString().Trim());

                #endregion


                #region obtiene cuerpo

                string cuerpo_contable = "";

                string querycue = "select Afcue_doc.cod_act,Afmae_act.vr_act,Afcue_doc.cod_ter,Afcue_doc.doc_int,Afcue_doc.cod_con,Afcue_doc.act_tras, ";
                querycue += "Afmae_gru.cta_act,Afmae_gru.cta_dep,";
                querycue += "Afing_ret.cta_per,Afing_ret.cta_ord,Afing_ret.cta_orc, ";
                querycue += "Afmae_gru.cta_ordd,Afmae_gru.cta_ordc, ";
                querycue += "Afmae_gru.cta_gdp ";
                querycue += "from Afcab_doc  ";
                querycue += "inner join Afcue_doc on Afcue_doc.idregcab = Afcab_doc.idreg ";
                querycue += "inner join Afmae_act on Afcue_doc.cod_act = Afmae_act.cod_act ";
                querycue += "inner join Afmae_gru on Afmae_gru.cod_gru = Afmae_act.cod_gru ";
                querycue += "inner join Afing_ret on Afing_ret.cod_con = Afcue_doc.cod_con ";
                querycue += "where Afcab_doc.idreg='" + idreg + "' ";

                MessageBox.Show("a1");

                DataTable dt_cuerpo = ((Inicio)Application.Current.MainWindow).Func.SqlDT(querycue, "cuerpo", _trn.BusinessId);

                foreach (System.Data.DataRow item in dt_cuerpo.Rows)
                {
                    double valor_act = Convert.ToDouble(item["vr_act"]);
                    string cod_act = item["cod_act"].ToString().Trim();
                    string act_tras = item["act_tras"].ToString().Trim();
                    string cod_con = item["cod_con"].ToString().Trim();
                    string doc_int = item["doc_int"].ToString().Trim();
                    string cod_ter = item["cod_ter"].ToString().Trim();


                    string cta_act = item["cta_act"].ToString().Trim();
                    string cta_dep = item["cta_dep"].ToString().Trim();
                    string cta_per = item["cta_per"].ToString().Trim();
                    string cta_ord = item["cta_ord"].ToString().Trim();
                    string cta_orc = item["cta_orc"].ToString().Trim();

                    string cta_ordd = item["cta_ordd"].ToString().Trim();
                    string cta_ordc = item["cta_ordc"].ToString().Trim();

                    string cta_gdp = item["cta_gdp"].ToString().Trim();




                    DataTable dt_depreciado = ((Inicio)Application.Current.MainWindow).Func.SaldoActivo(cod_act, _fecdoc.ToString("dd/MM/yyyy"), _trn.BusinessId);
                    if (dt_depreciado.Rows.Count > 0)
                    {
                        double vr_act = Convert.ToDouble(dt_depreciado.Rows[0]["vr_act"]);
                        double depreciado = Convert.ToDouble(dt_depreciado.Rows[0]["depreciacion"]);
                        double faltante = vr_act - depreciado;


                        cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,cod_ter,des_mov,cre_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_act + "','" + doc_int + "','" + cod_ter + "','Retiro - " + cod_act + " '," + vr_act.ToString("F", CultureInfo.InvariantCulture) + "); ";

                        if (depreciado > 0)
                            cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,cod_ter,des_mov,deb_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_dep + "','" + doc_int + "','" + cod_ter + "','Retiro - " + cod_act + " '," + depreciado.ToString("F", CultureInfo.InvariantCulture) + "); ";

                        if (vr_act > 0)
                            cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,cod_ter,des_mov,deb_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_per + "','" + doc_int + "','" + cod_ter + "','Retiro - " + cod_act + " '," + faltante.ToString("F", CultureInfo.InvariantCulture) + "); ";


                        if (cod_con == "51")
                        {
                            double val = faltante > 0 ? faltante : valor_act;
                            cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,des_mov,deb_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_ord + "','" + doc_int + "','Retiro - " + cod_act + "'," + val.ToString("F", CultureInfo.InvariantCulture) + "); ";
                            cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,des_mov,cre_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_orc + "','" + doc_int + "','Retiro - " + cod_act + "'," + val.ToString("F", CultureInfo.InvariantCulture) + "); ";
                        }

                        if (cod_con == "52")
                        {
                            cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,des_mov,deb_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_ordd + "','" + doc_int + "','Retiro - " + cod_act + "',1); ";
                            cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,des_mov,cre_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_ordc + "','" + doc_int + "','Retiro - " + cod_act + "',1); ";
                        }

                        if (cod_con == "60")
                        {
                            var t = getGrupo(act_tras);
                            cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,des_mov,deb_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + t.Item3 + "','" + doc_int + "','Adicion a: " + act_tras + " - Placa:" + cod_act + "'," + vr_act.ToString("F", CultureInfo.InvariantCulture) + "); ";
                            cuerpo_contable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,num_chq,des_mov,cre_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_gdp + "','" + doc_int + "','Adicion a: " + act_tras + " - Placa:" + cod_act + "'," + depreciado.ToString("F", CultureInfo.InvariantCulture) + "); ";
                        }




                    }
                }
                #endregion


                #region generar el documento contable
                using (SqlConnection connection = new SqlConnection(_trn.CnEmp))
                {

                    connection.Open();
                    StringBuilder errorMessages = new StringBuilder();
                    SqlCommand command = connection.CreateCommand();
                    SqlTransaction transaction;

                    transaction = connection.BeginTransaction("Transaction");
                    command.Connection = connection;
                    command.Transaction = transaction;

                    string sqlConsecutivo = @"declare @fecdoc as datetime;set @fecdoc = '" + _fecdoc.ToString() + "';declare @ini as char(4);DECLARE @NewTrn INT;";

                    //cabeza
                    string sqlcab001co = sqlConsecutivo + @" INSERT INTO cocab_doc (cod_trn,num_trn,fec_trn) values ('" + cod_trn_co + "','" + num_trn_co + "',@fecdoc);SELECT @NewTrn = SCOPE_IDENTITY();";

                    string sqlcue001co = cuerpo_contable;

                    command.CommandText = sqlcab001co + sqlcue001co + @"select CAST(@NewTrn AS int);";
                    //MessageBox.Show(command.CommandText.ToString());
                    var r = new object();
                    r = command.ExecuteScalar();
                    transaction.Commit();
                    connection.Close();
                    idregreturn = Convert.ToInt32(r.ToString());
                }
                #endregion

                return idregreturn;
            }
            catch (Exception w)
            {
                MessageBox.Show("error en  el documento contable:" + w);
                return -1;
            }
        }




        public void retirarActivo()
        {
            try
            {
                foreach (System.Data.DataRow dr in _trn.dsDoc.Tables["Cue"].Rows)
                {
                    DateTime _fecdoc = Convert.ToDateTime(_trn.dsDoc.Tables["Cab"].Rows[0]["fec_trn"].ToString().Trim());
                    string cod_act = dr["cod_act"].ToString().Trim();
                    DataTable dt_depreciado = ((Inicio)Application.Current.MainWindow).Func.SaldoActivo(cod_act, _fecdoc.ToString("dd/MM/yyyy"), _trn.BusinessId);
                    if (dt_depreciado.Rows.Count > 0)
                    {
                        double vr_act = Convert.ToDouble(dt_depreciado.Rows[0]["vr_act"]);
                        double depreciado = Convert.ToDouble(dt_depreciado.Rows[0]["depreciacion"]);
                        int mesdep = Convert.ToInt32(dt_depreciado.Rows[0]["mesdep"]);
                        dr["mesxdep"] = mesdep * (-1);
                        dr["vr_act"] = vr_act * (-1);
                        dr["dep_ac"] = depreciado * (-1);
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al calcular la depreciacion:"+w);
            }            
        }






        public Tuple<string, string, string> getGrupo(string acti_tra)
        {
            string query = "select cod_act,Afmae_act.cod_gru,afmae_gru.cta_act  ";
            query += "From Afmae_act ";
            query += "inner join afmae_gru on afmae_gru.cod_gru = Afmae_act.cod_gru ";
            query += "where Afmae_act.cod_act='" + acti_tra + "' ";

            DataTable dt = ((Inicio)Application.Current.MainWindow).Func.SqlDT(query, "cuerpo", _trn.BusinessId);
            return new Tuple<string, string, string>(
                dt.Rows.Count > 0 ? dt.Rows[0]["cod_act"].ToString().Trim() : "",
                dt.Rows.Count > 0 ? dt.Rows[0]["cod_gru"].ToString().Trim() : "",
                dt.Rows.Count > 0 ? dt.Rows[0]["cta_act"].ToString().Trim() : ""
                );
        }

        public bool validaConcepto()
        {
            bool flag = false;
            string concepto = _trn.dsDoc.Tables["Cue"].Rows[0]["cod_con"].ToString().Trim();

            MessageBox.Show("concepto:" + concepto);
            foreach (System.Data.DataRow dr in _trn.dsDoc.Tables["Cue"].Rows)
            {
                string con_cue = dr["cod_con"].ToString().Trim();
                MessageBox.Show("con_cue:" + con_cue);
                if (concepto != con_cue) flag = true;
            }
            return flag;
        }


        public bool validaCampoActivo()
        {
            bool flag = false;
            foreach (System.Data.DataRow dr in _trn.dsDoc.Tables["Cue"].Rows)
            {
                string con_act = dr["cod_act"].ToString().Trim();
                if (string.IsNullOrEmpty(con_act)) flag = true;
            }
            return flag;
        }



        private int DocumentEgeCaus()
        {



            string TipoConsecutivo = "num_act";
            string codtrn = "02";
            using (SqlConnection connection = new SqlConnection(cnEmp))
            {
                connection.Open();
                StringBuilder errorMessages = new StringBuilder();
                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;
                transaction = connection.BeginTransaction("Transaction");
                command.Connection = connection;
                command.Transaction = transaction;

                try
                {

                    string sqlConsecutivo = @"declare @fecdoc as datetime;set @fecdoc = getdate();";
                    sqlConsecutivo += "declare @ini as char(4);declare @num as varchar(12);declare @iConsecutivo char(12) = '' ;";
                    sqlConsecutivo += "declare @iFolioHost int = 0;";
                    sqlConsecutivo += "UPDATE Comae_trn SET " + TipoConsecutivo + " = ISNULL(" + TipoConsecutivo + ", 0) + 1  WHERE cod_trn='" + codtrn + "';";
                    sqlConsecutivo += "SELECT @iFolioHost = " + TipoConsecutivo + ",@ini=rtrim(inicial) FROM Comae_trn  WHERE cod_trn='" + codtrn + "';";
                    sqlConsecutivo += "set @num=@iFolioHost;";
                    sqlConsecutivo += "select @iConsecutivo=rtrim(@ini)+'-'+REPLICATE ('0',11-len(rtrim(@ini))-len(rtrim(convert(varchar,@num))))+rtrim(convert(varchar,@num));";

                    string sqlcab = sqlConsecutivo + @"INSERT INTO cocab_doc (cod_trn,fec_trn,num_trn,detalle,otro_ter,fec_posf) values ('" + codtrn + "',@fecdoc,@iConsecutivo,'" + Tx_Nota.Text.Trim() + "','" + TX_ot_Ter.Text + "','" + tx_Fec_pos.Text + "');DECLARE @NewID INT;SELECT @NewID = SCOPE_IDENTITY();";
                    string sql = "";
                    string sqlban = "";

                    double debito = 0;
                    double credito = 0;




                    foreach (System.Data.DataRow item in dt_egreso.Rows)
                    {
                        debito += item.IsNull("deb_mov") ? 0 : Convert.ToDouble(item["deb_mov"]);
                        credito += item.IsNull("cre_mov") ? 0 : Convert.ToDouble(item["cre_mov"]);

                        string cod_cta = item.IsNull("cod_cta") ? "" : item["cod_cta"].ToString();
                        string cod_cco = item.IsNull("cod_cco") ? "" : item["cod_cco"].ToString();
                        string cod_ter = item.IsNull("cod_ter") ? "" : item["cod_ter"].ToString();
                        string des_mov = item.IsNull("des_mov") ? "" : item["des_mov"].ToString();
                        string doc_cruc = item.IsNull("doc_cruc") ? "" : item["doc_cruc"].ToString();
                        decimal bas_mov = item.IsNull("bas_mov") ? 0 : Convert.ToDecimal(item["bas_mov"]);
                        decimal deb_mov = item.IsNull("deb_mov") ? 0 : Convert.ToDecimal(item["deb_mov"]);
                        decimal cre_mov = item.IsNull("cre_mov") ? 0 : Convert.ToDecimal(item["cre_mov"]);

                        sql += @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,doc_cruc,bas_mov,deb_mov,cre_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + cod_cta + "','" + cod_cco + "','" + cod_ter + "','" + des_mov + "','" + doc_cruc + "'," + bas_mov + "," + deb_mov + "," + cre_mov + ");";
                    }

                    double contraBanc = debito - credito;

                    if (contraBanc > 0)
                    {
                        DataTable dt = dtBanco.Select("cod_ban='" + ComBo_Banco.SelectedValue + "'").CopyToDataTable();
                        //SiaWin.Browse(dt);                     
                        string cnt = dt.Rows[0]["cod_cta"].ToString().Trim();
                        sql += @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,doc_cruc,cre_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + cnt + "','','" + tx_Clie.Text + "','BANCO',''," + contraBanc + ");";
                    }

                    string valor = (Cb_Trans.SelectedItem as ComboBoxItem).Content.ToString();
                    if (valor == "No")
                        sqlban = sqlban + @"UPDATE comae_ban SET  num_act=ISNULL(num_act, 0)+1  WHERE cod_ban='" + ComBo_Banco.SelectedValue + "';";


                    command.CommandText = sqlcab + sql + sqlban + @"select CAST(@NewId AS int);";
                    //MessageBox.Show(command.CommandText);

                    var r = new object();
                    r = command.ExecuteScalar();
                    transaction.Commit();
                    connection.Close();
                    return Convert.ToInt32(r.ToString());
                }
                catch (SqlException ex)
                {
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        errorMessages.Append(" SQL-Index #" + i + "\n" + "Message: " + ex.Errors[i].Message + "\n" + "LineNumber: " + ex.Errors[i].LineNumber + "\n" + "Source: " + ex.Errors[i].Source + "\n" + "Procedure: " + ex.Errors[i].Procedure + "\n");
                    }
                    transaction.Rollback();
                    MessageBox.Show(errorMessages.ToString());
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


        public bool val_cam()
        {
            bool flag = true;
            if (string.IsNullOrEmpty(tx_Clie.Text)) flag = false;
            if (string.IsNullOrEmpty(Tx_NomCli.Text)) flag = false;
            if (ComBo_Banco.SelectedIndex < 0) flag = false;
            //if (string.IsNullOrEmpty(Tx_Nota.Text)) flag = false;
            //if (string.IsNullOrEmpty(TX_ot_Ter.Text)) flag = false;
            if (Cb_Trans.SelectedIndex < 0) flag = false;
            if (string.IsNullOrEmpty(tx_Fec_pos.Text)) flag = false;
            //if (string.IsNullOrEmpty(Tx_Cheque.Text)) flag = false;
            return flag;
        }



        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (Btn_Cancel.Content.ToString() == "Salir")
            {
                this.Close();
            }
            else
            {
                act(1);
            }
        }

        public void act(int val)
        {
            if (val == 1)
            {
                dt_egreso.Clear();
                Btn_Save.Content = "Nuevo";
                Btn_Cancel.Content = "Salir";
                Txt_usaurio.Text = "---";

                Text_Ndoc.Text = "---";

                tx_Clie.Focusable = false;
                tx_Clie.Text = "";
                Tx_NomCli.Focusable = false;
                Tx_NomCli.Text = "";
                ComBo_Banco.IsEnabled = false;
                Tx_Nota.Focusable = false;
                Tx_Nota.Text = "";
                TX_ot_Ter.Focusable = false;
                TX_ot_Ter.Text = "";
                Cb_Trans.IsEnabled = false;
                tx_Fec_pos.Focusable = false;
                tx_Fec_pos.Text = "";
                Tx_Cheque.IsEnabled = false;
                Tx_Cheque.Text = "";
            }
            if (val == 2)
            {

                Btn_Save.Content = "Guardar";
                Btn_Cancel.Content = "Cancelar";
                Txt_usaurio.Text = SiaWin._UserAlias;
                Text_Ndoc.Text = consecutivo();

                tx_Clie.Focusable = true;
                Tx_NomCli.Focusable = false;
                ComBo_Banco.IsEnabled = true;
                Tx_Nota.Focusable = true;
                TX_ot_Ter.Focusable = true;
                Cb_Trans.IsEnabled = true;
                tx_Fec_pos.Focusable = true;
                tx_Fec_pos.Text = DateTime.Now.ToString();
                Tx_Cheque.Focusable = true;
            }

        }

        private void GridConfig_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {


                GridColumn Colum = ((SfDataGrid)sender).CurrentColumn as GridColumn;

                var reflector = this.GridConfig.View.GetPropertyAccessProvider();
                int columnIndex = (sender as SfDataGrid).SelectionController.CurrentCellManager.CurrentRowColumnIndex.RowIndex;
                var rowData = GridConfig.GetRecordAtRowIndex(columnIndex);

                string tabla = ""; string codigo = ""; string nombre = ""; string title = ""; string where = "";


                if ((sender as SfDataGrid).SelectedIndex == -1) return;

                string t = getTabla(Colum);
                if (string.IsNullOrEmpty(t)) return;


                if (e.Key == Key.F8)
                {
                    if (Colum.MappingName == "cod_cta")
                    {
                        tabla = "comae_cta"; codigo = "cod_cta"; nombre = "nom_cta"; title = "Maestra de cuentas";
                    }
                    if (Colum.MappingName == "cod_ter")
                    {
                        tabla = "comae_ter"; codigo = "cod_ter"; nombre = "nom_ter"; title = "Maestra de tercero";
                    }
                    if (Colum.MappingName == "cod_cco")
                    {
                        tabla = "comae_cco"; codigo = "cod_cco"; nombre = "nom_cco"; title = "Maestra de Centro de costos";
                    }

                    if (GridConfig.SelectedIndex == -1)
                        this.GridConfig.SelectionController.CurrentCellManager.BeginEdit();

                    if (Colum.MappingName == "cod_ter" || Colum.MappingName == "cod_cco" || Colum.MappingName == "cod_cta")
                    {
                        int idr = 0; string codi = ""; string nom = "";
                        dynamic xx = SiaWin.WindowBuscar(tabla, codigo, nombre, codigo, "idrow", title, SiaWin.Func.DatosEmp(idemp), false, where, idEmp: idemp);
                        xx.ShowInTaskbar = false;
                        xx.Owner = Application.Current.MainWindow;
                        xx.Height = 500;
                        xx.ShowDialog();
                        idr = xx.IdRowReturn;
                        idr = xx.IdRowReturn;
                        codi = xx.Codigo;
                        nom = xx.Nombre;


                        reflector.SetValue(rowData, Colum.MappingName, codi);

                        GridConfig.UpdateDataRow(columnIndex);
                        GridConfig.UpdateLayout();
                        GridConfig.Columns[Colum.MappingName].AllowEditing = true;
                    }

                    if (Colum.MappingName == "doc_cruc")
                    {
                        dynamic ww = SiaWin.WindowExt(9381, "TrnDocumentoCruce");  //carga desde sql
                        ww.codcliente = tx_Clie.Text;
                        ww.nomter = Tx_NomCli.Text;
                        //ww.codcta = cod_cta;
                        ww.fechacorte = DateTime.Now;
                        ww.idemp = idemp;
                        //ww.FilasRegistros = _trn.dsDoc.Tables["Cue"].Select("cod_cta='" + cod_cta.Trim() + "' and cod_ter='" + cod_cli + "' and doc_cruc<>''");
                        ww.ShowInTaskbar = false;
                        ww.Owner = Application.Current.MainWindow;
                        ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        ww.ShowDialog();

                    }



                }
            }
            catch (Exception w)
            {
                MessageBox.Show("****" + w);
            }
        }

        public string getTabla(GridColumn col)
        {
            string map = col.MappingName.ToString();
            string tabla = "";
            switch (map)
            {
                case "cod_cta": tabla = "comae_cta"; break;
                case "cod_ter": tabla = "comae_ter"; break;
                case "cod_cco": tabla = "comae_cco"; break;
                case "doc_cruc": tabla = "tabla"; break;
            }
            return tabla;
        }

        private void GridConfig_CurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs e)
        {
            try
            {



                GridColumn Colum = ((SfDataGrid)sender).CurrentColumn as GridColumn;



                var reflector = this.GridConfig.View.GetPropertyAccessProvider();
                var rowData = GridConfig.GetRecordAtRowIndex(e.RowColumnIndex.RowIndex);
                string valor = reflector.GetValue(rowData, Colum.MappingName).ToString();

                string tabla = getTabla(Colum);

                if (string.IsNullOrEmpty(getTabla(Colum)) || string.IsNullOrEmpty(valor)) return;

                if (validar(tabla, valor) == true)
                {
                    reflector.SetValue(rowData, Colum.MappingName, valor).ToString();
                    GridConfig.UpdateDataRow(e.RowColumnIndex.RowIndex);
                    GridConfig.UpdateLayout();
                    GridConfig.Columns[Colum.MappingName].AllowEditing = true;

                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "cod_cta"))) reflector.SetValue(rowData, "cod_cta", "");
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "cod_ter"))) reflector.SetValue(rowData, "cod_ter", "");
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "cod_ter"))) reflector.SetValue(rowData, "cod_ter", "");
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "cod_cco"))) reflector.SetValue(rowData, "cod_cco", "");
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "des_mov"))) reflector.SetValue(rowData, "des_mov", "");
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "doc_cruc"))) reflector.SetValue(rowData, "doc_cruc", "");
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "bas_mov"))) reflector.SetValue(rowData, "bas_mov", 0);
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "deb_mov"))) reflector.SetValue(rowData, "deb_mov", 0);
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "cre_mov"))) reflector.SetValue(rowData, "cre_mov", 0);
                }
                else
                {
                    MessageBox.Show("el codigo ingresado no existe");
                    reflector.SetValue(rowData, Colum.MappingName, "").ToString();

                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "cod_cta"))) reflector.SetValue(rowData, "cod_cta", "");
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "cod_ter"))) reflector.SetValue(rowData, "cod_ter", "");
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "cod_cco"))) reflector.SetValue(rowData, "cod_cco", "");
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "des_mov"))) reflector.SetValue(rowData, "des_mov", "");
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "doc_cruc"))) reflector.SetValue(rowData, "doc_cruc", 0);
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "bas_mov"))) reflector.SetValue(rowData, "bas_mov", "");
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "deb_mov"))) reflector.SetValue(rowData, "deb_mov", 0);
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "cre_mov"))) reflector.SetValue(rowData, "cre_mov", 0);

                    GridConfig.UpdateDataRow(e.RowColumnIndex.RowIndex);
                    GridConfig.UpdateLayout();
                    GridConfig.Columns[Colum.MappingName].AllowEditing = true;
                }
                updTot();
            }
            catch (Exception w)
            {
                MessageBox.Show("error al editar:" + w);
            }

        }


        public void updTot()
        {
            if (dt_egreso.Rows.Count > 0)
            {
                double deb = Convert.ToDouble(dt_egreso.Compute("Sum(deb_mov)", ""));
                double cred = Convert.ToDouble(dt_egreso.Compute("Sum(cre_mov)", ""));
                double dif = deb - cred;
                Tot_Deb.Text = deb.ToString("N", CultureInfo.CreateSpecificCulture("es-ES"));
                Tot_Cre.Text = cred.ToString("N", CultureInfo.CreateSpecificCulture("es-ES"));
                Tot_Dif.Text = dif.ToString("N", CultureInfo.CreateSpecificCulture("es-ES"));
            }
        }

        public bool valDebCre()
        {
            bool flag = false;
            if (dt_egreso.Rows.Count > 0)
            {
                double deb = Convert.ToDouble(dt_egreso.Compute("Sum(deb_mov)", ""));
                double cred = Convert.ToDouble(dt_egreso.Compute("Sum(cre_mov)", ""));
                if (deb > cred) flag = true;
            }
            return flag;
        }


        public bool validar(string table, string value)
        {
            bool flag = false;
            string campo = "";
            string where = "";
            switch (table)
            {
                case "comae_cta": campo = "cod_cta"; where = "and tip_cta='A' "; break;
                case "comae_ter": campo = "cod_ter"; where = ""; break;
                case "comae_cco": campo = "cod_cco"; where = ""; break;
            }


            string select = "select * from " + table + " where " + campo + "='" + value + "' " + where + "; ";
            DataTable dt = SiaWin.Func.SqlDT(select, "table", idemp);
            if (dt.Rows.Count > 0) flag = true;


            return flag;
        }

        private void GridConfig_CurrentCellActivating(object sender, CurrentCellActivatingEventArgs e)
        {

            if (e.CurrentRowColumnIndex.ColumnIndex == 1 || e.CurrentRowColumnIndex.ColumnIndex == 8)
                GridConfig.AddNewRowPosition = AddNewRowPosition.Bottom;
            else
                GridConfig.AddNewRowPosition = AddNewRowPosition.None;
            GridConfig.UpdateLayout();
            updTot();
        }

        private void ComBo_Banco_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Cb_Trans.SelectedIndex = 0;
        }

        private void Tx_Fec_pos_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            MoveToNextUIElement(e);
            MoveToNextUIElement(e);
            MoveToNextUIElement(e);
        }

        private void GridConfig_CurrentCellActivated(object sender, CurrentCellActivatedEventArgs e)
        {
            try
            {
                bool t = this.GridConfig.View.IsAddingNew;
                if (!t)
                {
                    if ((e.CurrentRowColumnIndex.RowIndex) > GridConfig.View.Records.Count)
                    {
                        if (e.CurrentRowColumnIndex.ColumnIndex > 0)
                            this.GridConfig.SelectionController.CurrentCellManager.BeginEdit();
                    }
                    else
                    {
                        GridConfig.UpdateLayout();
                        var reflector = this.GridConfig.View.GetPropertyAccessProvider();
                        int columnIndex = (sender as SfDataGrid).SelectionController.CurrentCellManager.CurrentRowColumnIndex.RowIndex;
                        var rowData = GridConfig.GetRecordAtRowIndex(columnIndex);
                        string cod_cta = reflector.GetValue(rowData, "cod_cta").ToString().Trim();
                        if (string.IsNullOrEmpty(cod_cta))
                        {
                            this.GridConfig.SelectionController.CurrentCellManager.BeginEdit();
                            //return;
                        }

                        string cod_ter = reflector.GetValue(rowData, "cod_ter").ToString().Trim();
                        if (string.IsNullOrEmpty(cod_ter))
                        {
                            this.GridConfig.SelectionController.CurrentCellManager.BeginEdit();
                            //return;
                        }
                    }
                }

                if (Keyboard.IsKeyDown(Key.Tab) || Keyboard.IsKeyDown(Key.Right) || Keyboard.IsKeyDown(Key.Return))
                {
                    //MessageBox.Show("A1");
                    var reflector = this.GridConfig.View.GetPropertyAccessProvider();
                    int columnIndex = (sender as SfDataGrid).SelectionController.CurrentCellManager.CurrentRowColumnIndex.RowIndex;
                    var rowData = GridConfig.GetRecordAtRowIndex(columnIndex);
                    //MessageBox.Show("A2"+ e.OriginalSender);
                    //MessageBox.Show("A3" + e.PreviousRowColumnIndex.ColumnIndex);
                    //MessageBox.Show("A4" + e.ActivationTrigger);


                    GridColumn Colum = ((SfDataGrid)sender).CurrentColumn as GridColumn;
                    string tabla = ""; string codigo = ""; string nombre = ""; string title = ""; string where = "";
                    //MessageBox.Show("A3");

                    if (e.PreviousRowColumnIndex.ColumnIndex == 1)
                    {
                        if (DBNull.Value.Equals(reflector.GetValue(rowData, "cod_cta")))
                        {
                            MessageBox.Show("nullo");
                            tabla = "comae_cta"; codigo = "cod_cta"; nombre = "nom_cta"; title = "Maestra de cuentas";
                            int idr = 0; string codi = ""; string nom = "";
                            dynamic xx = SiaWin.WindowBuscar(tabla, codigo, nombre, codigo, "idrow", title, SiaWin.Func.DatosEmp(idemp), false, where, idEmp: idemp);
                            xx.ShowInTaskbar = false;
                            xx.Owner = Application.Current.MainWindow;
                            xx.Height = 500;
                            xx.ShowDialog();
                            idr = xx.IdRowReturn;
                            codi = xx.Codigo;
                            nom = xx.Nombre;
                            //GridConfig.MoveCurrentCell(e.PreviousRowColumnIndex, false);
                            reflector.SetValue(rowData, "cod_cta", codi);
                            GridConfig.UpdateDataRow(columnIndex);
                            GridConfig.UpdateLayout();
                            GridConfig.Columns["cod_cta"].AllowEditing = true;
                            return;
                        }

                        string cod_cta = reflector.GetValue(rowData, "cod_cta").ToString().Trim();
                        if (string.IsNullOrEmpty(cod_cta))
                        {

                            //GridConfig.MoveCurrentCell(e.PreviousRowColumnIndex, false);

                            tabla = "comae_cta"; codigo = "cod_cta"; nombre = "nom_cta"; title = "Maestra de cuentas";
                            int idr = 0; string codi = ""; string nom = "";
                            dynamic xx = SiaWin.WindowBuscar(tabla, codigo, nombre, codigo, "idrow", title, SiaWin.Func.DatosEmp(idemp), false, where, idEmp: idemp);
                            xx.ShowInTaskbar = false;
                            xx.Owner = Application.Current.MainWindow;
                            xx.Height = 500;
                            xx.ShowDialog();
                            idr = xx.IdRowReturn;
                            codi = xx.Codigo;
                            nom = xx.Nombre;
                            //GridConfig.MoveCurrentCell(e.PreviousRowColumnIndex, false);
                            reflector.SetValue(rowData, "cod_cta", codi);
                            GridConfig.UpdateDataRow(columnIndex);
                            GridConfig.UpdateLayout();
                            GridConfig.Columns["cod_cta"].AllowEditing = true;
                            return;
                        }
                    }

                    if (e.PreviousRowColumnIndex.ColumnIndex == 2)
                    {
                        if (DBNull.Value.Equals(reflector.GetValue(rowData, "cod_ter")))
                        {
                            //MessageBox.Show("tercero nullo");
                            //GridConfig.MoveCurrentCell(e.PreviousRowColumnIndex, false);
                            tabla = "comae_ter"; codigo = "cod_ter"; nombre = "nom_ter"; title = "Maestra de tercero";
                            int idr = 0; string codi = ""; string nom = "";
                            dynamic xx = SiaWin.WindowBuscar(tabla, codigo, nombre, codigo, "idrow", title, SiaWin.Func.DatosEmp(idemp), false, where, idEmp: idemp);
                            xx.ShowInTaskbar = false;
                            xx.Owner = Application.Current.MainWindow;
                            xx.Height = 500;
                            xx.ShowDialog();
                            idr = xx.IdRowReturn;
                            codi = xx.Codigo;
                            nom = xx.Nombre;
                            reflector.SetValue(rowData, "cod_ter", codi);
                            GridConfig.UpdateDataRow(columnIndex);
                            GridConfig.UpdateLayout();
                            GridConfig.Columns["cod_ter"].AllowEditing = true;
                            return;
                        }

                        string cod_ter = reflector.GetValue(rowData, "cod_ter").ToString().Trim();
                        if (string.IsNullOrEmpty(cod_ter))
                        {
                            //MessageBox.Show("tercero vacio");
                            //GridConfig.MoveCurrentCell(e.PreviousRowColumnIndex, false);
                            tabla = "comae_ter"; codigo = "cod_ter"; nombre = "nom_ter"; title = "Maestra de tercero";
                            int idr = 0; string codi = ""; string nom = "";
                            dynamic xx = SiaWin.WindowBuscar(tabla, codigo, nombre, codigo, "idrow", title, SiaWin.Func.DatosEmp(idemp), false, where, idEmp: idemp);
                            xx.ShowInTaskbar = false;
                            xx.Owner = Application.Current.MainWindow;
                            xx.Height = 500;
                            xx.ShowDialog();
                            idr = xx.IdRowReturn;
                            codi = xx.Codigo;
                            nom = xx.Nombre;
                            reflector.SetValue(rowData, "cod_ter", codi);
                            GridConfig.UpdateDataRow(columnIndex);
                            GridConfig.UpdateLayout();
                            GridConfig.Columns["cod_ter"].AllowEditing = true;
                            return;
                        }
                    }


                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error ??" + w);
            }
        }

        private void DataGrid_RecordDeleted(object sender, RecordDeletedEventArgs e)
        {

        }

        private void DataGrid_CurrentCellValueChanged(object sender, CurrentCellValueChangedEventArgs e)
        {

        }

        private void DataGrid_SelectionChanging(object sender, GridSelectionChangingEventArgs e)
        {

            if (dr.RowState == DataRowState.Deleted) return;
            if (dr.RowState == DataRowState.Added) return;

        }





    }
}


