using Microsoft.Reporting.WinForms;
using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.UI.Xaml.ScrollAxis;
using Syncfusion.Windows.Controls.Grid.Converter;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9303,"PvTrasladosBodega");
    //((Inicio)Application.Current.MainWindow).PublicarPnt(9303,"PvTrasladosBodega")
    public partial class PvTrasladosBodega : Window
    {
        dynamic SiaWin;
        private int idemp;
        private string idbod;
        int numregcab = 0;  //idreg a imprimir
        int idLogo = 0;
        DataSet dsImprimir = new DataSet();
        string codbod = "";
        string nompvta = "";
        string cnEmp = "";
        public string idBod = "";
        public int idEmp = 0;
        public string codpvta = "";
        //DataView dtComboBodDestino;
        DataSet ds = new DataSet();
        DataTable dd = new DataTable();
        DataTable dtBod = new DataTable();
        int CantidadMaxRegEnFactura = 0;
        bool SaltoAutomaticoAlSiguienteRegistro = false;
        static string codcco = string.Empty;
        static string BusinessName = string.Empty;
        static string BusinessNit = string.Empty;
        static string BusinessCode = string.Empty;
        public string bodegaNitDestino = "";
        public string bodegaNitNombreDestino = "";
        public double bodegaNitcupoCxC = 0;
        int TipoBodega = 0;
        string codigoBodegaOrigen = "";
        private Ref RefgdcSource = new Ref();
        public Ref RefGDCSource
        {
            get { return RefgdcSource; }
            set { RefgdcSource = value; }
        }

        public PvTrasladosBodega()
        {
            try
            {
                InitializeComponent();
                TextFecha.Text = DateTime.Now.ToString();
                SiaWin = Application.Current.MainWindow;
                idemp = SiaWin._BusinessId;
                codpvta = SiaWin._UserTag;
                //LoadInfo();
                ActivaDesactivaControles(0);
                this.DataContext = this;
                FechaIni.Text = DateTime.Now.ToShortDateString();
                FechaFin.Text = DateTime.Now.ToShortDateString();
                DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                BusinessName = foundRow["BusinessName"].ToString().Trim();
                BusinessNit = foundRow["BusinessNit"].ToString().Trim();
                BtbGrabar.Focus();
            }
            catch (Exception ex)
            {
                SiaWin.Func.SiaExeptionGobal(ex);
                MessageBox.Show("C# error1Constructor:" + ex.Message);
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                idbod = idBod;
                idemp = idEmp;
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                BusinessCode = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Traslado de Bodega - Empresa:" + BusinessCode + "-" + nomempresa;
                LoadInfo();
            }
            catch (Exception ex)
            {
                SiaWin.Func.SiaExeptionGobal(ex);
                MessageBox.Show(ex.Message, "Load-PvTrasladosBodea");
            }
        }
        public void LoadInfo()
        {
            try
            {
                DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                TxtEmpresa.Text = SiaWin._BusinessName.ToString().Trim();
                TxtPVenta.Text = codpvta;
                TxtUser.Text = SiaWin._UserAlias;

                if (codpvta == string.Empty)
                {
                    MessageBox.Show("El usuario no tiene asignado un punto de venta, Pantalla Bloqueada");
                    this.IsEnabled = false;
                    return;
                }
                else
                {
                    nompvta = SiaWin.Func.cmpCodigo("copventas", "cod_pvt", "nom_pvt", codpvta, idemp);
                    TxtPVenta.Text = codpvta + "-" + nompvta;
                    codbod = SiaWin.Func.cmpCodigo("copventas", "cod_pvt", "cod_bod", codpvta, idemp).ToString().Trim();
                    if (string.IsNullOrEmpty(codbod))
                    {
                        MessageBox.Show("El punto de venta Asignado no tiene bodega , Pantalla Bloqueada");
                    }
                    TxtBod.Text = codbod;
                }

                TipoBodega = Convert.ToInt32(SiaWin.Func.cmpCodigo("inmae_bod", "cod_bod", "tipo_bod", codbod, idEmp));
                TxtCND.Text = TipoBodega == 1 ? "Si" : "No";
                TxtCND.Tag = TipoBodega.ToString().Trim();
                dtBod = SiaWin.DB.SqlDT("select cod_bod,cod_bod+'-'+nom_bod as nom_bod,tipo_bod,aut_ent_trasl,cod_emp from inmae_bod where estado=1 and (cod_emp='" + BusinessCode + "' or cod_emp='') order by cod_bod", "inmae_ref", idemp);
                dtBod.PrimaryKey = new DataColumn[] { dtBod.Columns["cod_bod"] };
                CmbBodDestino.DisplayMemberPath = "nom_bod";
                CmbBodDestino.SelectedValuePath = "cod_bod";
            }
            catch (Exception e)
            {
                SiaWin.Func.SiaExeptionGobal(e);
                MessageBox.Show(e.Message);
            }
        }

        private void CmbBodDestino_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (CmbBodDestino.SelectedIndex >= 0)
                {
                    string bodega = CmbBodDestino.SelectedValue.ToString();
                    GetNitbodega(bodega);
                }
                if (RefGDCSource.Count > 0)
                {
                    //limpiarGrillaActu();
                }

            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("Error en el change del destino:" + w);
            }
        }

        public void limpiarGrillaActu()
        {
            foreach (var item in RefGDCSource)
            {
                if (string.IsNullOrEmpty(item.cod_ref)) return;

                ActualizaCamposRef(item.cod_ref, dataGrid);
            }
        }

        public void GetNitbodega(string bodega)
        {
            string cadena = "select inmae_bod.cod_ter as terce,comae_ter.nom_ter as nom_ter,comae_ter.cupo_cxc as cupo from inmae_bod  ";
            cadena += "inner join comae_ter on inmae_bod.cod_ter=comae_ter.cod_ter ";
            cadena += "where inmae_bod.cod_bod='" + bodega + "' ";

            DataTable dt = SiaWin.Func.SqlDT(cadena, "Bodega", idemp);
            bodegaNitDestino = dt.Rows.Count > 0 ? dt.Rows[0]["terce"].ToString().Trim() : "";
            bodegaNitcupoCxC = dt.Rows.Count > 0 ? Convert.ToDouble(dt.Rows[0]["cupo"]) : 0;
            bodegaNitNombreDestino = dt.Rows.Count > 0 ? dt.Rows[0]["nom_ter"].ToString().Trim() : "";
        }

        void LlenaCombo(ComboBox _Combo, DataTable dt, string cmpId, string cmpName)
        {
            _Combo.Items.Clear();
            _Combo.DisplayMemberPath = cmpName;
            _Combo.SelectedValuePath = cmpId;
            _Combo.ItemsSource = dt.DefaultView;
        }

        public int ActivaDesactivaControles(int estado)
        {
            if (estado == 0)
            {
                TextNota.Text = "";
                TextNumeroDoc.Text = "";
                CmbBodDestino.SelectedIndex = -1;
                CmbBodOrigen.SelectedIndex = -1;
                CmbTipoDoc.SelectedIndex = -1;
                TextNota.IsEnabled = false;
                CmbTipoDoc.IsEnabled = false;
                CmbBodOrigen.IsEnabled = false;
                CmbBodDestino.IsEnabled = false;
                BtbGrabar.Content = "Nuevo";
                BtbCancelar.Content = "Salir";
                dataGrid.IsReadOnly = true;
                RefGDCSource.Clear();
                TextItem.Text = "0";
                TextCantidades.Text = "0";
                TextSaldoU.Text = "0";
                LabelBodegaDestino.Text = "Bodega Destino:";
                LabelBodegaOrigen.Text = "Bodega Origen:";
                CmbTipoTraslado.IsEnabled = false;
                CmbTipoTraslado.SelectedIndex = -1;
            }
            if (estado == 1) //creando
            {
                LabelBodegaDestino.Text = "Bodega Destino:";
                LabelBodegaOrigen.Text = "Bodega Origen:";
                TextNota.Text = "Traslado Bodega";
                TextNumeroDoc.Text = "";
                CmbBodDestino.SelectedIndex = -1;
                CmbBodOrigen.SelectedIndex = -1;
                CmbTipoDoc.SelectedIndex = 0;
                CmbTipoDoc.IsEnabled = true;
                CmbBodOrigen.IsEnabled = true;
                CmbBodDestino.IsEnabled = true;
                TextNota.IsEnabled = true;
                BtbGrabar.Content = "Grabar";
                BtbCancelar.Content = "Cancelar";
                dataGrid.IsReadOnly = false;
                RefGDCSource.Add(new Referencia() { nom_ref = "--" });
                dataGrid.CommitEdit();
                dataGrid.UpdateLayout();
                dataGrid.SelectedIndex = 0;
                TextItem.Text = "0";
                TextCantidades.Text = "0";
                TextSaldoU.Text = "0";
                CmbBodOrigen.SelectedValue = codbod;
                CmbTipoTraslado.IsEnabled = true;
                CmbTipoTraslado.SelectedIndex = -1;
            }
            return estado;
        }

        public class Referencia : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            private void OnPropertyChanged(string property)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
            int _item;
            public int item { get { return _item; } set { _item = value; OnPropertyChanged("item"); } }
            int _idrow;
            public int idrow { get { return _idrow; } set { _idrow = value; OnPropertyChanged("idrow"); } }
            string _nom_ref;
            public string nom_ref { get { return _nom_ref; } set { _nom_ref = value; OnPropertyChanged("nom_ref"); } }
            string _cod_ref;
            public string cod_ref { get { return _cod_ref; } set { _cod_ref = value; OnPropertyChanged("cod_ref"); } }
            string _cod_bod;
            public string cod_bod { get { return _cod_bod; } set { _cod_bod = value; OnPropertyChanged("cod_bod"); } }
            decimal _val_ref;
            public decimal val_ref { get { return _val_ref; } set { _val_ref = value; OnPropertyChanged("val_ref"); OnPropertyChanged("subtotal"); subtotal = _cantidad * _val_ref; OnPropertyChanged("valdescto"); valdescto = (subtotal * _pordescto) / 100; OnPropertyChanged("valiva"); valiva = Math.Round(((subtotal - valdescto) * _poriva) / 100, 0); OnPropertyChanged("total"); total = Math.Round((subtotal - valdescto + valiva), 0); } }
            decimal _cantidad;
            public decimal cantidad { get { return _cantidad; } set { _cantidad = value; OnPropertyChanged("cantidad"); OnPropertyChanged("subtotal"); subtotal = _cantidad * _val_ref; OnPropertyChanged("valdescto"); valdescto = Math.Round((subtotal * _pordescto) / 100, 0); OnPropertyChanged("valiva"); valiva = Math.Round(((subtotal - valdescto) * _poriva) / 100, 0); OnPropertyChanged("total"); total = Math.Round((subtotal - valdescto + valiva), 0); } }
            decimal _subtotal;
            public decimal subtotal { get { return _subtotal; } set { _subtotal = value; OnPropertyChanged("subtotal"); } }
            decimal _pordescto;
            public decimal pordescto { get { return _pordescto; } set { _pordescto = value; OnPropertyChanged("pordescto"); OnPropertyChanged("subtotal"); subtotal = _cantidad * _val_ref; OnPropertyChanged("valdescto"); valdescto = Math.Round((subtotal * _pordescto) / 100, 0); OnPropertyChanged("valiva"); valiva = Math.Round(((subtotal - valdescto) * _poriva) / 100, 0); OnPropertyChanged("total"); total = Math.Round((subtotal - valdescto + valiva), 0); } }
            decimal _valdescto;
            public decimal valdescto { get { return _valdescto; } set { _valdescto = value; OnPropertyChanged("valdescto"); } }
            string _cod_tiva;
            public string cod_tiva { get { return _cod_tiva; } set { _cod_tiva = value; OnPropertyChanged("cod_tiva"); } }
            decimal _poriva;
            public decimal poriva { get { return _poriva; } set { _poriva = value; OnPropertyChanged("poriva"); OnPropertyChanged("subtotal"); subtotal = _cantidad * _val_ref; OnPropertyChanged("valdescto"); valdescto = Math.Round((subtotal * _pordescto) / 100, 0); OnPropertyChanged("valiva"); valiva = Math.Round(((subtotal - valdescto) * _poriva) / 100, 0); OnPropertyChanged("total"); total = Math.Round((subtotal - valdescto + valiva), 0); } }
            decimal _valiva;
            public decimal valiva { get { return _valiva; } set { _valiva = value; OnPropertyChanged("valiva"); } }
            decimal _total;
            public decimal total { get { return _total; } set { _total = value; OnPropertyChanged("total"); } }
            decimal _salref = 0;
            public decimal salref { get { return _salref; } set { _salref = value; OnPropertyChanged("salref"); } }
            DateTime _fechahora;
            public DateTime fechahora { get { return _fechahora; } set { _fechahora = value; OnPropertyChanged("fechahora"); } }
            bool _Estado = false;
            public bool Estado { get { return _Estado; } set { _Estado = value; OnPropertyChanged("Estado"); } }
            string _nom_tip;
            public string nom_tip { get { return _nom_tip; } set { _nom_tip = value; OnPropertyChanged("nom_tip"); } }
            string _nom_prv;
            public string nom_prv { get { return _nom_prv; } set { _nom_prv = value; OnPropertyChanged("nom_prv"); } }
        }
        public class Ref : ObservableCollection<Referencia>
        {
            //ObservableCollection<Referencia> Referencias = new ObservableCollection<Referencia>();
            public decimal Total()
            {
                decimal _tuni = 0;
                foreach (var item in this)
                {
                    _tuni += item.cantidad;
                }
                return _tuni;
            }
        }

        private void BtbGrabar_Click(object sender, RoutedEventArgs e)
        {
            if (BtbGrabar.Content.ToString() == "Nuevo")
            {
                ActivaDesactivaControles(1);
                CmbTipoDoc.Focus();
                CmbTipoDoc.IsDropDownOpen = true;
            }
            else
            {
                if (string.IsNullOrEmpty(cnEmp))
                {
                    MessageBox.Show("Error - Cadena de Conexion nulla");
                    return;
                }


                try
                {
                    int _TipoDoc = CmbTipoDoc.SelectedIndex;
                    if (_TipoDoc < 0)
                    {
                        MessageBox.Show("Seleccione un Tipo de Documento..");
                        CmbTipoDoc.Focus();
                        CmbTipoDoc.IsDropDownOpen = true;
                        return;
                    }
                    if (CmbBodOrigen.SelectedIndex < 0)
                    {
                        MessageBox.Show("Seleccione Bodega de Origen..");
                        CmbBodOrigen.Focus();
                        CmbBodOrigen.IsDropDownOpen = true;
                        return;
                    }
                    if (CmbBodDestino.SelectedIndex < 0)
                    {
                        MessageBox.Show("Seleccione Bodega de Origen..");
                        CmbBodDestino.Focus();
                        CmbBodDestino.IsDropDownOpen = true;
                        return;
                    }
                    if (RefGDCSource.Count == 0)
                    {
                        MessageBox.Show("No hay registros de productos...");
                        dataGrid.Focus();
                        return;
                    }
                    CmbTipoTraslado.SelectedIndex = 0;
                    int _TipoTrasl = CmbTipoTraslado.SelectedIndex;
                    if (_TipoTrasl < 0)
                    {
                        MessageBox.Show("Seleccione un Tipo de Traslado..");
                        CmbTipoTraslado.Focus();
                        CmbTipoTraslado.IsDropDownOpen = true;
                        return;
                    }

                    if (TotalCnt(0) <= 0) return;
                    if (TotalCnt(1) <= 0) return;
                    //int iddocumento = 0;
                    if (CmbTipoDoc.SelectedIndex == 0 || CmbTipoDoc.SelectedIndex == 1 || CmbTipoDoc.SelectedIndex == 2)
                    {

                        if (!ValidaExistencias()) return;

                        if (CmbTipoDoc.SelectedIndex == 1)
                        {
                            if (!ValidaCartera(bodegaNitDestino, totalFacturar()))
                            {
                                MessageBox.Show("Cancelacion por Validacion de Cartera");
                                return;
                            }
                        }

                        int iddoc = ExecuteSqlTransaction();
                        if (iddoc > 0)
                        {
                            SqlDataReader dr = SiaWin.DB.SqlDR("SELECT cod_trn,num_trn  FROM  incab_doc where idreg=" + iddoc.ToString(), idEmp);
                            string codtrn = "";
                            string numtrn = "";

                            while (dr.Read())
                            {
                                codtrn = dr["cod_trn"].ToString();
                                numtrn = dr["num_trn"].ToString();
                            }
                            dr.Close();
                            if (codtrn != string.Empty)
                            {
                                string name = ((ComboBoxItem)CmbTipoDoc.SelectedItem).Content.ToString();
                                SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, -9, -1, -9, "GENERO TRASLADO:" + codtrn + "/" + numtrn + " - tipo de traslado:" + name + " ", "");
                                ImprimeDocumentoTraslado(codtrn, numtrn, 0, true);

                            }
                        }
                    }

                    ActivaDesactivaControles(0);
                }
                catch (Exception ex)
                {
                    SiaWin.Func.SiaExeptionGobal(ex);
                    MessageBox.Show("C# err 2:" + ex.Message);
                }


            }
        }

        public double totalFacturar()
        {
            decimal valor = 0;
            foreach (var item in RefGDCSource)
            {
                valor += item.total;
            }
            return Convert.ToDouble(valor);
        }

        public decimal ValidaCarteraPromedioPago(string codter, string codemp)
        {
            decimal t_return = 0;
            SqlConnection _conn = new SqlConnection(SiaWin.Func.ConfiguracionApp());
            _conn.Open();
            try
            {
                string _ctaCredito = "13050505";
                SqlCommand cmd = new SqlCommand("_EmpCxCPromedioPago", _conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ter", codter);
                cmd.Parameters.AddWithValue("@cta", _ctaCredito);
                cmd.Parameters.AddWithValue("@codemp", codemp);
                SqlDataReader dr = cmd.ExecuteReader();
                // double _saldo = 0.00 ; 
                while (dr.Read())
                {

                    t_return = Convert.ToDecimal(dr["promedio"].ToString());
                }


                _conn.Close();
            }
            catch (Exception ex)
            {
                SiaWin.Func.SiaExeptionGobal(ex);
                _conn.Dispose();
                MessageBox.Show(ex.Message);
            }
            return t_return;
        }

        private bool ValidaCartera(string cod_ter, double valor)
        {
            try
            {
                decimal promedioPago = ValidaCarteraPromedioPago(cod_ter, BusinessCode);
                //ConfigCSource.promedioPago = promedioPago;

                string _codctacxc = "13050505";
                double SaldoCartera = SiaWin.Func.CarteraSaldo(cod_ter, _codctacxc, DateTime.Now, BusinessCode);

                double FacturasVencidas = SiaWin.Func.CarteraSaldo(cod_ter, _codctacxc, DateTime.Now, BusinessCode, tipo: 1);


                double saldocxc = SaldoCartera;
                int facturasVencidas = Convert.ToInt32(FacturasVencidas);
                double cupo_disp = bodegaNitcupoCxC - saldocxc;

                if (promedioPago > 100)
                {
                    MessageBox.Show("Promedio de pago alto..:" + promedioPago.ToString());
                }
                if ((bodegaNitcupoCxC - SaldoCartera) < valor || facturasVencidas > 0)  // notiene cupo o tiene factuas vencidas
                {
                    StringBuilder sbMsg = new StringBuilder();
                    if ((bodegaNitcupoCxC - SaldoCartera) < valor) sbMsg.Append("Valor no Autorizado, Cupo disponible=" + (bodegaNitcupoCxC - SaldoCartera).ToString("C2") + Environment.NewLine);
                    if (facturasVencidas > 0) sbMsg.Append("Cliente con " + facturasVencidas.ToString() + " Factuas Vencidas." + Environment.NewLine);

                    if (MessageBox.Show(sbMsg.ToString(), "Siasoft", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {

                        if (!ValidaClaveUsuario(132))
                        {
                            MessageBox.Show("Usuario no puede autorizar Facturas con cartera vencida o Sobrecupo");
                            ActivaDesactivaControles(0);
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        ActivaDesactivaControles(0);
                        return false;
                    }



                }

                if ((bodegaNitcupoCxC - SaldoCartera) < valor)
                {
                    MessageBox.Show("Valor no Autorizado, Cupo disponible=" + (bodegaNitcupoCxC - SaldoCartera).ToString());
                    if (MessageBox.Show("Usted desea ver estado de cartera del cliente.......?", "Siasoft", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        string[] strArrayParam = new string[] { bodegaNitDestino, bodegaNitNombreDestino, idEmp.ToString() };
                        SiaWin.Tab(9243, strArrayParam);
                        SiaWin.ValReturn = null;
                    }
                    //if (ConfigCSource.cod_trn == "005")
                    //{
                    //    CancelaDocumento();
                    //    return false;
                    //}
                }
            }
            catch (System.Exception _error)
            {
                SiaWin.Func.SiaExeptionGobal(_error);
                MessageBox.Show(_error.Message + "-:-" + _error.InnerException.Message);
                return false;
            }

            return true;

        }

        private bool ValidaClaveUsuario(int accid)
        {
            try
            {
                return SiaWin.Func.ShowSecurity(accid);
            }
            catch (Exception ex)
            {
                SiaWin.Func.SiaExeptionGobal(ex);
                MessageBox.Show(ex.Message.ToString());
                return false;
            }

        }

        public decimal TotalCnt(int tipo) //tipo = 0 suma cantidades, tipo=1 cuenta items
        {
            
            decimal _cnt = 0;
            
            foreach (var item in RefGDCSource)
            {
            
                if (tipo == 0) _cnt += item.cantidad;
                if (tipo == 1 && item.cantidad > 0) _cnt++;
            }
            
            if (tipo == 0) TextCantidades.Text = _cnt.ToString("N2");
            if (tipo == 1) TextItem.Text = _cnt.ToString("N2");
            
            return _cnt;
        }

        private bool ValidaExistencias()
        {
            try
            {
                var q = from b in RefGDCSource
                        group b by b.idrow into g
                        select new
                        {
                            idrow = g.Key,
                            cod_ref = g.Max(item => item.cod_ref),
                            cantidad = g.Sum(item => item.cantidad)
                        };
                StringBuilder errorMessages = new StringBuilder();
                foreach (var item in q)
                {
                    if (item.cantidad > 0)
                    {
                        //decimal saldoin = SiaWin.Func.SaldoInv(item.cod_ref, codbod, BusinessCode);
                        decimal saldoin = SiaWin.Func.SaldoInv(item.cod_ref, codigoBodegaOrigen, BusinessCode);

                        if (item.cantidad > saldoin)
                        {
                            errorMessages.Append("Codigo:" + item.cod_ref.ToString() + " /Cantidad a Facturar:" + item.cantidad.ToString() + " /Saldo Inv:" + saldoin.ToString() + "\n");
                        };
                    }
                }
                if (errorMessages.ToString() != string.Empty)
                {
                    MessageBox.Show(errorMessages.ToString());
                    dataGrid.Focus();
                    //Abrir InterEmpresa
                    if (TxtCND.Tag.ToString() == "1")
                    {

                        if (!ValidaTraslados())
                        {
                            MessageBox.Show("llego validacionde traslado");
                        }

                    }
                    else
                    {
                        MessageBox.Show("No Puede Abrir El traslado InterEmpresa Por que la Bodega de origen no es una bodega del CND");
                    }
                    dataGrid.SelectedIndex = 0;
                    return false;
                }
            }
            catch (Exception e)
            {
                SiaWin.Func.SiaExeptionGobal(e);
                MessageBox.Show("Error en el traladoPPP:" + e.Message);
            }
            return true;
        }

        private DataTable GetTableSaldosInv()
        {
            DataTable table = new DataTable();
            try
            {

                table.Columns.Add("cod_ref", typeof(string));
                table.Columns.Add("cod_ant", typeof(string));
                table.Columns.Add("nom_ref", typeof(string));
                table.Columns.Add("cod_bod", typeof(string));
                table.Columns.Add("cantidad", typeof(decimal));
                table.Columns.Add("saldo", typeof(decimal));
                table.Columns.Add("faltante", typeof(decimal));
                table.Columns.Add("saldoEmp1", typeof(decimal));
                table.Columns.Add("traslEmp1", typeof(decimal));
                table.Columns.Add("saldoEmp2", typeof(decimal));
                table.Columns.Add("traslEmp2", typeof(decimal));
                table.Columns.Add("saldoEmp3", typeof(decimal));
                table.Columns.Add("traslEmp3", typeof(decimal));
                table.Columns.Add("saldoEmp4", typeof(decimal));
                table.Columns.Add("traslEmp4", typeof(decimal));
                table.Columns.Add("traslTotal", typeof(decimal));
                table.Columns.Add("saldoB001", typeof(decimal));
                table.Columns.Add("saldoB005", typeof(decimal));
                table.Columns.Add("saldoB008", typeof(decimal));
                table.Columns.Add("saldoB010", typeof(decimal));
                //default value
                table.Columns["cantidad"].DefaultValue = 0;
                table.Columns["saldo"].DefaultValue = 0;
                table.Columns["faltante"].DefaultValue = 0;
                table.Columns["saldoEmp1"].DefaultValue = 0;
                table.Columns["traslEmp1"].DefaultValue = 0;
                table.Columns["saldoEmp2"].DefaultValue = 0;
                table.Columns["traslEmp2"].DefaultValue = 0;
                table.Columns["saldoEmp3"].DefaultValue = 0;
                table.Columns["traslEmp3"].DefaultValue = 0;
                table.Columns["saldoEmp4"].DefaultValue = 0;
                table.Columns["traslEmp4"].DefaultValue = 0;

                return table;
            }
            catch (Exception ex)
            {
                SiaWin.Func.SiaExeptionGobal(ex);
                MessageBox.Show(ex.Message, "GetTableSaldosInv");
            }
            return table;
        }

        public bool ValidaTraslados()
        {

            if (TipoBodega == 1)
            {
                DataTable dtSaldosInvEmpresa = GetTableSaldosInv();
                var q = from b in RefGDCSource
                        group b by b.idrow into g
                        select new
                        {
                            idrow = g.Key,
                            cod_ref = g.Max(item => item.cod_ref),
                            nom_ref = g.Max(item => item.nom_ref),
                            //cod_ant = g.Max(item => item.cod_ant),
                            cantidad = g.Sum(item => item.cantidad)
                        };

                StringBuilder errorMessages = new StringBuilder();
                foreach (var item in q)
                {
                    if (item.cantidad > 0)
                    {
                        decimal saldoin = SiaWin.Func.SaldoInv(item.cod_ref, codbod, BusinessCode);

                        if (Convert.ToDouble(item.cantidad) > Convert.ToDouble(saldoin))
                        {
                            decimal diferencia = Convert.ToDecimal(item.cantidad) - saldoin;
                            DataRow row;
                            row = dtSaldosInvEmpresa.NewRow();
                            row["cod_ref"] = item.cod_ref;
                            row["nom_ref"] = item.nom_ref;
                            row["cod_ant"] = item.nom_ref;//item.cod_ant;
                            row["cantidad"] = Convert.ToDecimal(item.cantidad);
                            row["saldo"] = saldoin;
                            row["faltante"] = diferencia;
                            dtSaldosInvEmpresa.Rows.Add(row);
                            //adiciona a tabla tmp
                        }
                    }

                }


                if (dtSaldosInvEmpresa.Rows.Count > 0)
                {

                    if (TipoBodega == 1)
                    {
                        dynamic Pnt9467 = SiaWin.WindowExt(9467, "PvTrasladosAutomaticosEntreEmpresas");  //valida traslados
                        Pnt9467.idEmp = idEmp;
                        Pnt9467.codbod = codbod;
                        Pnt9467.DtCue = dtSaldosInvEmpresa;
                        Pnt9467.codpvta = codpvta;
                        Pnt9467.ShowInTaskbar = false;
                        Pnt9467.Owner = Application.Current.MainWindow;
                        Pnt9467.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        Pnt9467.ShowDialog();
                        Pnt9467 = null;
                        //return true;

                        //decimal saldoin = SiaWin.Func.SaldoInv(item.cod_ref, codbod, BusinessCode);                        
                        foreach (var item in RefGDCSource)
                        {
                            if (!string.IsNullOrEmpty(item.cod_ref))
                                ActualizaCamposRef(item.cod_ref, dataGrid);
                        }

                        return true;
                    }

                }

            }
            return true;
        }



        private void BtbCancelar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BtbCancelar.Content.ToString() == "Cancelar")
                {
                    TotalCnt(0);
                    decimal totcnt = Convert.ToDecimal(TextCantidades.Text.ToString());
                    if (totcnt > 0)
                    {
                        if (MessageBox.Show("Usted desea cancelar este documento..?", "Cancelar Traslado", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
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
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error al cancelar:" + w);
            }
        }

        private bool IsNumberKey(Key inKey)
        {
            if (inKey == Key.Decimal) return true;
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
            try
            {
                if (dataGrid.IsReadOnly == true) return;
                if (e.Key == System.Windows.Input.Key.F5)
                {
                    BtbGrabar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    return;
                }
                //dataGrid.UpdateLayout();
                var data = ((DataGrid)sender).SelectedItem as Referencia;
                if (data == null)
                {
                    e.Handled = true;
                }
                var uiElement = e.OriginalSource as UIElement;
                if ((e.Key == Key.Enter || e.Key == Key.Return || e.Key == Key.Right || e.Key == Key.Tab)) //&& ((DataGrid)sender).CurrentColumn.DisplayIndex == 0)
                {
                    if (string.IsNullOrEmpty(data.cod_ref))
                    {
                        /////////////
                        dynamic ww = SiaWin.WindowExt(9326, "InBuscarReferencia");  //carga desde sql
                        ww.Conexion = SiaWin.Func.DatosEmp(idemp);
                        ww.idEmp = idemp;
                        //ww.idBod = codbod;
                        ww.idBod = CmbBodOrigen.SelectedValue.ToString();
                        ww.UltBusqueda = "";
                        ww.ShowInTaskbar = false;
                        ww.Owner = Application.Current.MainWindow;
                        ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        ww.Height = 400;
                        ww.ShowDialog();
                        //                    UltBusquedaRef = ww.UltBusqueda;
                        if (!string.IsNullOrEmpty(ww.Codigo))
                        {
                            data.cod_ref = ww.Codigo.ToString();
                        }
                        ww = null;
                        if (string.IsNullOrEmpty(data.cod_ref))
                        {
                            e.Handled = false;
                            data.cantidad = 0; data.val_ref = 0;
                            data.Estado = false;
                        }
                        if (!ActualizaCamposRef(data.cod_ref, sender)) e.Handled = false;
                        e.Handled = true;
                    }
                    else
                    {
                        if (!ActualizaCamposRef(data.cod_ref, sender))
                        {
                            MessageBox.Show("Codigo :" + data.cod_ref + " No existe...");
                            data.cantidad = 0; data.val_ref = 0; data.Estado = false;
                            e.Handled = true;
                            return;
                        }
                    }
                    if (SaltoAutomaticoAlSiguienteRegistro == true)
                    {
                        
                        int add = 0;
                        if (CantidadMaxRegEnFactura == 0)
                        {
                            if (dataGrid.SelectedIndex == RefGDCSource.Count - 1)
                            {
                                RefGDCSource.Add(new Referencia() { nom_ref = "--" });
                            }
                            add = 1;
                        }
                        if (CantidadMaxRegEnFactura > 0)
                        {
                            if (RefGDCSource.Count < CantidadMaxRegEnFactura)
                            {
                                RefGDCSource.Add(new Referencia() { nom_ref = "--" });
                                add = 1;
                            }
                        }
                        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                        dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.Items[dataGrid.SelectedIndex + add], dataGrid.Columns[0]);
                        dataGrid.CommitEdit();
                        dataGrid.SelectedIndex = dataGrid.SelectedIndex + add;
                        e.Handled = true;
                        return;
                    }
                    // valida que la referencia no este doble en el documento
                    int count = (from x in RefGDCSource where x.cod_ref == data.cod_ref select x).Count();  // count: 2
                    if (count > 1)
                    {
                        MessageBox.Show("El codigo:" + data.cod_ref + " esta doble en el documento");
                        data.cod_ref = string.Empty; data.nom_prv = string.Empty; data.nom_tip = string.Empty; data.nom_ref = string.Empty; data.cantidad = 0; data.val_ref = 0; data.Estado = false;
                        e.Handled = true;
                        return;
                    }
                }

                if (((DataGrid)sender).CurrentColumn.DisplayIndex != 0)
                {
                    e.Handled = !IsNumberKey(e.Key) && !IsDelOrBackspaceOrTabKey(e.Key);
                }

                int column = ((DataGrid)sender).CurrentColumn.DisplayIndex + 1;
                int columntot = ((DataGrid)sender).Columns.Count;
                
                if (CmbTipoDoc.SelectedIndex == 0) columntot = 3;
                int fila1 = ((DataGrid)sender).SelectedIndex;
                int fila = ((DataGrid)sender).Items.IndexOf(((DataGrid)sender).SelectedItem);

                if ((e.Key == Key.Enter || e.Key == Key.Return || e.Key == Key.Tab) && uiElement != null && (column < columntot))
                {

                    if (!string.IsNullOrEmpty(data.cod_ref) && ((DataGrid)sender).CurrentColumn.DisplayIndex == columntot)
                    {
                        Int32 countref = RefGDCSource.Count;
                        //MessageBox.Show("adicion");
                        if (countref == dataGrid.SelectedIndex + 1)
                        {
                            actualizar(sender);
                                
                            RefGDCSource.Add(new Referencia() { nom_ref = "--" });                            
                            uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                            dataGrid.SelectedIndex = dataGrid.SelectedIndex + 1;
                            dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.Items[dataGrid.SelectedIndex], dataGrid.Columns[0]);
                            dataGrid.CommitEdit();
                            dataGrid.UpdateLayout();
                           
                            
                            //var a = ((DataGrid)sender).Items[((DataGrid)sender).SelectedIndex-1];
                            //MessageBox.Show("a1");
                            //object senderlast = ((DataGrid)sender).SelectedIndex - 1;
                            //MessageBox.Show("a2");
                            //var dato = (senderlast as DataGrid).SelectedItem as Referencia;
                            //MessageBox.Show("a3"+dato.cod_ref);
                            //MessageBox.Show("a4" + dato.cantidad);                            
                        }
                    }

                    if (((DataGrid)sender).CurrentColumn.DisplayIndex >= 0)
                    {
                        //MessageBox.Show("enter");
                        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                        e.Handled = true;
                        return;
                    }
                }

                if (e.Key == Key.Right && ((DataGrid)sender).CurrentColumn.DisplayIndex == 0 && !string.IsNullOrEmpty(data.cod_ref))
                {
                    uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    e.Handled = true;
                }
                if (e.Key == Key.Left && uiElement != null && (column > 1))
                {
                    uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Left));
                    e.Handled = true;
                }
                if ((e.Key == Key.Enter || e.Key == Key.Return || e.Key == Key.Right || e.Key == Key.Tab) && uiElement != null && (column == columntot))
                {
                    //MessageBox.Show("entra");
                    dataGrid.CommitEdit();
                    dataGrid.UpdateLayout();

                    int add = 0;
                    if (fila + 1 == RefGDCSource.Count)
                    {
                        if (CantidadMaxRegEnFactura == 0)
                        {
                            RefGDCSource.Add(new Referencia() { nom_ref = "--" });
                            add = 1;
                        }
                        if (CantidadMaxRegEnFactura > 0)
                        {
                            MessageBox.Show("agrego 1"); 
                            if (RefGDCSource.Count < CantidadMaxRegEnFactura)
                            {
                                MessageBox.Show("agrego 2");
                                RefGDCSource.Add(new Referencia() { nom_ref = "--" });
                                add = 1;
                            }
                        }
                    }

                    if (add > 0) uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.Items[dataGrid.SelectedIndex + add], dataGrid.Columns[0]);
                    dataGrid.CommitEdit();
                    dataGrid.UpdateLayout();
                    dataGrid.SelectedIndex = dataGrid.SelectedIndex + add;
                    e.Handled = true;
                }
                
                //if (e.Key == Key.Down && !string.IsNullOrEmpty(data.cod_ref))
                //{                    
                //    Int32 columnIndex = dataGrid.SelectedIndex;
                //    Int32 countref = RefGDCSource.Count;                                        
                //    if (fila == countref - 1)
                //    {
                //        if (CantidadMaxRegEnFactura == 0) RefGDCSource.Add(new Referencia() { nom_ref = "--" });
                //        if (CantidadMaxRegEnFactura > 0)
                //        {
                //            actualizar(sender);
                //            if (RefGDCSource.Count < CantidadMaxRegEnFactura) RefGDCSource.Add(new Referencia() { nom_ref = "--" });
                //            dataGrid.CommitEdit();
                //            dataGrid.UpdateLayout();
                //        }
                //    }
                //}

                if (e.Key == Key.Up && dataGrid.CurrentColumn.DisplayIndex == 0 && string.IsNullOrEmpty(data.cod_ref))
                {
                    var selectedItem = dataGrid.SelectedItem as Referencia;
                    if (selectedItem != null)
                    {
                        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Up));
                        dataGrid.SelectedIndex = dataGrid.SelectedIndex - 1;
                        //dataGrid.CommitEdit();                        
                        RefGDCSource.Remove(selectedItem);
                        //dataGrid.CommitEdit();                        
                        dataGrid.UpdateLayout();
                        var selectedItemnew = dataGrid.SelectedItem as Referencia;
                        if (selectedItemnew.cantidad > 0)
                        {
                            dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.Items[dataGrid.SelectedIndex], dataGrid.Columns[2]);
                            dataGrid.CancelEdit();
                            dataGrid.UpdateLayout();
                        }
                        e.Handled = true;
                    }
                }
                if (e.Key == Key.Up)
                {                    
                    var selectedItemnew = dataGrid.SelectedItem as Referencia;                   
                    if (selectedItemnew.cantidad > 0)
                    {                                         
                        dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.Items[dataGrid.SelectedIndex], dataGrid.Columns[2]);
                        dataGrid.CancelEdit();
                        dataGrid.UpdateLayout();
                    }
                }

                if (e.Key == Key.F8)
                {
                    uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));
                }
                if (e.Key == Key.F3)  //eliminar registro
                {
                    if (((DataGrid)sender).SelectedIndex == 0 && RefGDCSource.Count == 1) return;
                    if (MessageBox.Show("Borrar Registro actual?", "Siasoft", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        var selectedItem = dataGrid.SelectedItem as Referencia;                        
                        if (selectedItem != null)
                        {
                            int fila1x = ((DataGrid)sender).SelectedIndex;
                            Int32 countrefx = RefGDCSource.Count;
                            if (((DataGrid)sender).SelectedIndex == 0 && RefGDCSource.Count > 1)
                            {
                                uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                            }
                            else
                            {
                                if (((DataGrid)sender).SelectedIndex > 0 && RefGDCSource.Count > 1) uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                                if (((DataGrid)sender).SelectedIndex == RefGDCSource.Count - 1) uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Up));
                            }
                            RefGDCSource.Remove(selectedItem);
                        }
                        e.Handled = true;
                    }
                }
                TotalCnt(0);
                TotalCnt(1);
            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error:"+w);
            }
        }

        void actualizar(object sender)
        {

            //var dato = (sender as DataGrid).SelectedItem as Referencia;           
            //ActualizaCamposRef(dato.cod_ref, sender);
        }


        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            
            //if (e.Column.Header.ToString() == "Cantidad" && CmbTipoDoc.SelectedIndex == 1)            

            //if (e.Column.Header.ToString() == "Cantidad" )
            //{
            //    var data = ((DataGrid)sender).SelectedItem as Referencia;

            //    MessageBox.Show("data1:"+data.cod_ref);
            //    MessageBox.Show("data2:"+data.cantidad);

            //    ActualizaCamposRef(data.cod_ref, sender);
            //}
        }

        

        private void DataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {                
                if (dataGrid.CurrentCell.Column == null)return;
                                                              
                if (dataGrid.CurrentCell.Column.Header.ToString() == "Codigo" || dataGrid.CurrentCell.Column.Header.ToString() == "Cantidad" || dataGrid.CurrentCell.Column.Header.ToString() == "ColValRef")
                {                                     
                    var data = ((DataGrid)sender).SelectedItem as Referencia;                    
                    ActualizaCamposRef(data.cod_ref, sender);                
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("DataGrid_CurrentCellChanged:" + w);
            }        
        }





        private bool ActualizaCamposRef(string Id, object datagrid)
        {
            bool Resp = false;
            try
            {

                codigoBodegaOrigen = CmbBodOrigen.SelectedValue.ToString();
                //MessageBox.Show("codigoBodegaOrigen:"+ codigoBodegaOrigen);

                if (string.IsNullOrEmpty(Id)) return false;

                string cmpval_uni = "inmae_ref.val_ref as val_ref";
                string campoDescTip = "por_des";
                string campoDescuentoLinea = "por_desc";

                string query = "select inmae_ref.idrow,inmae_ref.cod_ref,inmae_ref.cod_ant,rtrim(nom_ref) as nom_ref,inmae_ref.cod_tip,inmae_ref.cod_tiva, ";
                query = query + "inmae_tiva.por_iva,inmae_ref.val_ref as precioLista," + cmpval_uni + ",isnull(InList_cli.Val_uni,0) as val_refList, ";
                query = query + "nom_tip,nom_prv,inmae_tip." + campoDescuentoLinea + " as '" + campoDescuentoLinea + "', ";
                query = query + "isnull(inter_tip." + campoDescTip + ",0) as '" + campoDescTip + "', ";
                query = query + "isnull(InList_cli.Por_des,0) as decuentoLista ";
                query = query + "FROM inmae_ref ";
                query = query + "inner join inmae_tiva on inmae_tiva.cod_tiva=inmae_ref.cod_tiva  ";
                query = query + "inner join inmae_tip on inmae_tip.cod_tip=inmae_ref.cod_tip  ";
                query = query + "left join inmae_prv on inmae_prv.cod_prv=inmae_ref.cod_prv  ";
                query = query + "left join inter_tip on inter_tip.Cod_ter='" + bodegaNitDestino + "' and inter_tip.cod_tip=inmae_Ref.cod_tip  ";
                query = query + "left join InList_cli on InList_cli.Cod_ter='" + bodegaNitDestino + "' and InList_cli.Cod_ref='" + Id.Trim() + "'  ";
                query = query + "where  inmae_ref.cod_ref='" + Id.Trim() + "' ";


                //SqlDataReader dr = SiaWin.DB.SqlDR("select inmae_ref.idrow,cod_ref,rtrim(nom_ref) as nom_ref,val_ref,inmae_ref.cod_tiva,inmae_tiva.por_iva,nom_tip,nom_prv,inmae_tip.por_des as tippor_des,inmae_tip.por_desc as tippor_desc FROM inmae_ref inner join inmae_tiva on inmae_tiva.cod_tiva=inmae_ref.cod_tiva inner join inmae_tip on inmae_tip.cod_tip=inmae_ref.cod_tip left join inmae_prv on inmae_prv.cod_prv=inmae_ref.cod_prv where  inmae_ref.cod_ref='" + Id.ToString() + "'", idemp);
                //DataTable dt = SiaWin.DB.SqlDT(query, idemp);
                //SiaWin.Browse(dt);


                SqlDataReader dr = SiaWin.DB.SqlDR(query, idemp);
                //DataTable dt = new DataTable();
                //dt.Load(dr);
                //SiaWin.Browse(dt);


                while (dr.Read())
                {
                    ((Referencia)((DataGrid)datagrid).SelectedItem).idrow = Convert.ToInt32(dr["idrow"]);
                    ((Referencia)((DataGrid)datagrid).SelectedItem).cod_ref = dr["cod_ref"].ToString().Trim();
                    ((Referencia)((DataGrid)datagrid).SelectedItem).nom_ref = dr["nom_ref"].ToString().Trim();

                    decimal DecLista = Convert.ToDecimal(dr["val_refList"]);
                    double val_uni = 0;
                    double cantidad = Convert.ToDouble(((Referencia)((DataGrid)datagrid).SelectedItem).cantidad);
                    //MessageBox.Show("cantidad:" + cantidad);

                    double iva = Convert.ToDouble(dr["por_iva"]);

                    double procentaje_desc = 0;

                    if (Convert.ToDouble(dr["decuentoLista"]) > 0)
                    {
                        procentaje_desc = Convert.ToDouble(dr["decuentoLista"]);
                    }
                    else if (Convert.ToDouble(dr[campoDescTip]) > 0)
                    {
                        procentaje_desc = Convert.ToDouble(dr[campoDescTip]);
                    }
                    else if (Convert.ToDouble(dr[campoDescuentoLinea]) > 0)
                    {
                        procentaje_desc = Convert.ToDouble(dr[campoDescuentoLinea]);
                    }

                    string valorRef = DecLista > 0 ? "val_refList" : "val_ref";


                    if (valorRef == "val_refList")
                    {
                        if (iva > 0)
                        {
                            double _valref = Convert.ToDouble(dr[valorRef]) / (1 + (Convert.ToDouble(dr["por_iva"]) / 100));
                            val_uni = Math.Round(_valref, 0);
                        }
                        if (iva == 0)
                        {
                            double _valref = Convert.ToDouble(dr[valorRef]);
                            val_uni = Math.Round(_valref, 0);
                        }

                    }
                    else
                    {
                        if (iva > 0)
                        {
                            double _desc = 1 - (Convert.ToDouble(procentaje_desc)) / 100;
                            double _valref = Convert.ToDouble(dr["val_ref"]) * _desc / (1 + (Convert.ToDouble(dr["por_iva"]) / 100));
                            val_uni = Math.Round(_valref, 0);
                        }
                        if (iva == 0)
                        {
                            double _valref = Convert.ToDouble(dr["val_ref"]);
                            val_uni = Math.Round(_valref, 0);
                        }
                        //ConfigCSource.ValUnitMasIva = _valref * (1 + (Convert.ToDouble(dr["por_iva"]) / 100));
                    }



                    ((Referencia)((DataGrid)datagrid).SelectedItem).val_ref = Math.Round(Convert.ToDecimal(val_uni), 0);
                    double subtotal = val_uni * cantidad;
                    ((Referencia)((DataGrid)datagrid).SelectedItem).subtotal = Math.Round(Convert.ToDecimal(subtotal), 0);
                    ((Referencia)((DataGrid)datagrid).SelectedItem).pordescto = Convert.ToDecimal(procentaje_desc);
                    ((Referencia)((DataGrid)datagrid).SelectedItem).poriva = Math.Round(Convert.ToDecimal(iva), 0);
                    double valorIva = (subtotal * iva) / 100;
                    ((Referencia)((DataGrid)datagrid).SelectedItem).valiva = Convert.ToDecimal(valorIva);

                    //MessageBox.Show("subtotal:"+ subtotal);
                    //MessageBox.Show("poriva:" + ((Referencia)((DataGrid)datagrid).SelectedItem).poriva);
                    //MessageBox.Show("valorIva:" + valorIva);


                    double total = subtotal + valorIva;
                    ((Referencia)((DataGrid)datagrid).SelectedItem).total = Math.Round(Convert.ToDecimal(total), 0);

                    int filaindex = ((DataGrid)datagrid).SelectedIndex;
                    TotalCnt(0);
                    TotalCnt(1);
                    if (((Referencia)((DataGrid)datagrid).SelectedItem).cod_ref != string.Empty)
                    {
                        decimal saldoin = SiaWin.Func.SaldoInv(((Referencia)((DataGrid)datagrid).SelectedItem).cod_ref, codigoBodegaOrigen, BusinessCode);
                        TextSaldoU.Text = saldoin.ToString("N2");
                    }
                    else
                    {
                        TextSaldoU.Text = "0";
                    }
                    Resp = true;
                }
                dr.Close();
            }
            catch (System.Exception _error)
            {
                SiaWin.Func.SiaExeptionGobal(_error);
                MessageBox.Show("c# error3:" + _error.Message);
            }
            return Resp;
        }

        private void dataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            try
            {
                if (e.Column.DisplayIndex == 0)
                {
                    if (((Referencia)((DataGrid)sender).SelectedItem).cantidad > 0 || ((Referencia)((DataGrid)sender).SelectedItem).Estado == true)
                    {
                        //((Referencia)((DataGrid)sender).SelectedItem).cantidad = 0;
                        //((Referencia)((DataGrid)sender).SelectedItem).Estado = false;
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("dataGrid_BeginningEdit:" + w);
            }
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {                
                if (dataGrid.SelectedItem == null) return;
             
                var _RefDG = dataGrid.SelectedItem as Referencia;
             
                if (_RefDG != null)
                {             
                    string reg = _RefDG.cod_ref;
                    if (!string.IsNullOrEmpty(reg))
                    {
                        decimal saldoin = SiaWin.Func.SaldoInv(_RefDG.cod_ref, codbod, BusinessCode);
                        TextSaldoU.Text = saldoin.ToString();
                    }
                }
                else                
                    TextSaldoU.Text = "0";
                
            }
            catch (Exception w)
            {
                MessageBox.Show("dataGrid_SelectionChanged:"+w);
            }
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
            TotalCnt(0);
            decimal totcnt = Convert.ToDecimal(TextCantidades.Text.ToString());
            if (totcnt > 0) e.Cancel = true;

        }

        private void CmbTipoDoc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selec = CmbTipoDoc.SelectedIndex;
            int tipodoc = -1;
            if (selec == -1) TextNumeroDoc.Text = "";
            if (selec == 0) tipodoc = 7;
            if (selec == 1) tipodoc = 8;
            if (selec == 2) tipodoc = 12;

            CmbBodOrigen.SelectedIndex = -1;
            CmbBodDestino.SelectedIndex = -1;

            if (CmbTipoDoc.SelectedIndex != -1)
            {
                TextNumeroDoc.Text = consecutivo(codpvta, 0, tipodoc, idemp);
                LimpiarGrillaCampos(selec);
                CargarBodegasOrigneDestino(selec);
            }
        }

        public void CargarBodegasOrigneDestino(int tipo)
        {
            try
            {
                if (tipo == 0)
                {
                    CmbBodOrigen.ItemsSource = null;
                    CmbBodDestino.ItemsSource = null;

                    dd = dtBod.Select("cod_bod=" + codbod + " and (tipo_bod=1 or tipo_bod=2) and cod_emp='" + BusinessCode + "'").CopyToDataTable();
                    LlenaCombo(CmbBodOrigen, dd, "cod_bod", "nom_bod");

                    DataTable d1 = dtBod.Select("cod_bod<>" + codbod + " and (tipo_bod=1 or tipo_bod=2) and cod_emp='" + BusinessCode + "'").CopyToDataTable();
                    CmbBodDestino.ItemsSource = d1.DefaultView;
                }
                if (tipo == 1)
                {
                    CmbBodOrigen.ItemsSource = null;
                    CmbBodDestino.ItemsSource = null;


                    dd = dtBod.Select("cod_bod=" + codbod).CopyToDataTable();
                    LlenaCombo(CmbBodOrigen, dd, "cod_bod", "nom_bod");

                    DataTable d1 = dtBod.Select("cod_bod<>" + codbod + " and tipo_bod=4").CopyToDataTable();
                    //dtComboBodDestino = new DataView(d1);
                    CmbBodDestino.ItemsSource = d1.DefaultView;
                }
                if (tipo == 2)
                {
                    CmbBodOrigen.ItemsSource = null;
                    CmbBodDestino.ItemsSource = null;

                    dd = dtBod.Select("cod_bod=" + codbod).CopyToDataTable();
                    LlenaCombo(CmbBodDestino, dd, "cod_bod", "nom_bod");

                    DataTable d1 = dtBod.Select("cod_bod<>" + codbod + " and tipo_bod=4").CopyToDataTable();
                    CmbBodOrigen.ItemsSource = d1.DefaultView;
                }

            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("Error en el channnnnnnn" + w);
            }
        }

        public void LimpiarGrillaCampos(int index)
        {
            if (index == 1)
            {
                ColValRef.Visibility = Visibility.Visible;
                ColSubtotal.Visibility = Visibility.Visible;
                ColProDesc.Visibility = Visibility.Visible;
                ColIva.Visibility = Visibility.Visible;
                ColValIva.Visibility = Visibility.Visible;
                ColTotal.Visibility = Visibility.Visible;
            }
            else
            {
                ColValRef.Visibility = Visibility.Hidden;
                ColSubtotal.Visibility = Visibility.Hidden;
                ColProDesc.Visibility = Visibility.Hidden;
                ColIva.Visibility = Visibility.Hidden;
                ColValIva.Visibility = Visibility.Hidden;
                ColTotal.Visibility = Visibility.Hidden;
            }

        }

        public string consecutivo(string codPv, int Aumenta, int TipoDoc, int IdBuss)
        {
            string consecutivo = "0";
            try
            {

                SqlConnection _conn = new SqlConnection(SiaWin._cn);
                _conn.Open();
                SqlCommand cmd = new SqlCommand("_empConsecutivoPv", _conn);
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Pvt", codPv);
                cmd.Parameters.AddWithValue("@TipoDoc", TipoDoc);
                cmd.Parameters.AddWithValue("@Aumenta", Aumenta);
                cmd.Parameters.AddWithValue("@_codEmp", BusinessCode);
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                _conn.Close();

                if (ds.Tables[0].Rows.Count > 0) consecutivo = ds.Tables[0].Rows[0]["iConsecutivo"].ToString();

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar el consecutivo:" + w);
            }

            return consecutivo;
        }

        private void CmbTipoDoc_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    ComboBox cs = e.Source as ComboBox;
                    if (cs != null)
                    {
                        if (cs.SelectedIndex >= 0)
                        {
                            cs.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

                        }
                        
                    }
                    base.OnPreviewKeyDown(e);
                }
            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("55ERROR");
            }
        }

        private int ExecuteSqlTransaction()
        {
            int bandera = -1;

            if (MessageBox.Show("Usted desea guardar el documento..?", "Guardar Traslado", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {

                string _bodOrigen = "";
                string _bodDestino = "";

                _bodOrigen = CmbBodOrigen.SelectedValue.ToString();
                _bodDestino = CmbBodDestino.SelectedValue.ToString();

                string TipoConsecutivo = "";
                string codtrn = "";
                string codtrncontra = "";
                string clientenit = "";

                if (CmbTipoDoc.SelectedIndex == 0) codtrn = "141";
                if (CmbTipoDoc.SelectedIndex == 0) codtrncontra = "016";

                if (CmbTipoDoc.SelectedIndex == 1) codtrn = "145";
                if (CmbTipoDoc.SelectedIndex == 1) codtrncontra = "051";

                if (CmbTipoDoc.SelectedIndex == 2) codtrn = "146";
                if (CmbTipoDoc.SelectedIndex == 2) codtrncontra = "052";

                if (codtrn == "141")
                {
                    TipoConsecutivo = "sal_trasl";
                    clientenit = "";

                }
                if (codtrn == "145")
                {
                    TipoConsecutivo = "sal_consg";
                    clientenit = bodegaNitDestino;

                }
                if (codtrn == "146")
                {
                    TipoConsecutivo = "dev_consg";
                    clientenit = "";
                }

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
                        string sqlcabContra = "";
                        string sqlcab = "";

                        string sqlConsecutivo = @"declare @fecdoc as datetime;set @fecdoc = getdate();";
                        sqlConsecutivo = sqlConsecutivo + "declare @fecdocsecond as datetime;set @fecdocsecond = DATEADD(second,1,GETDATE()); ";
                        sqlConsecutivo = sqlConsecutivo + "declare @ini as char(4);declare @num as varchar(12); ";
                        sqlConsecutivo = sqlConsecutivo + "declare @iConsecutivo char(12) = '' ;declare @iFolioHost int = 0; ";
                        sqlConsecutivo = sqlConsecutivo + "UPDATE COpventas SET " + TipoConsecutivo + "=ISNULL(" + TipoConsecutivo + ", 0) + 1  WHERE cod_pvt='" + codpvta + "'; ";
                        sqlConsecutivo = sqlConsecutivo + "SELECT @iFolioHost = " + TipoConsecutivo + ",@ini=rtrim('" + codpvta + "') FROM Copventas  WHERE cod_pvt='" + codpvta + "'; set @num=@iFolioHost; ";
                        sqlConsecutivo = sqlConsecutivo + "select @iConsecutivo=rtrim(@ini)+'-'+REPLICATE ('0',11-len(rtrim(@ini))-len(rtrim(convert(varchar,@num))))+rtrim(convert(varchar,@num));";
                        sqlcab = sqlConsecutivo + @"INSERT INTO incab_doc (cod_trn,fec_trn,num_trn,doc_ref,des_mov,bod_tra,tip_traslado,est_imp,cod_cli) values ('" + codtrn + "',@fecdoc,@iConsecutivo,@iConsecutivo,'" + TextNota.Text.Trim() + "','" + _bodDestino + "'," + CmbTipoTraslado.SelectedIndex.ToString() + ",1,'" + clientenit + "');DECLARE @NewID INT;SELECT @NewID = SCOPE_IDENTITY();";
                        sqlcabContra = @"INSERT INTO incab_doc (cod_trn,fec_trn,num_trn,doc_ref,des_mov,bod_tra,tip_traslado) values ('" + codtrncontra + "',@fecdocsecond,@iConsecutivo,@iConsecutivo,'" + TextNota.Text.Trim() + "','" + _bodOrigen + "'," + CmbTipoTraslado.SelectedIndex.ToString() + ");DECLARE @NewIDContra INT;SELECT @NewIDContra = SCOPE_IDENTITY();";
                        string sql = "";
                        string sqlcontra = "";
                        var q = from b in RefGDCSource
                                group b by b.cod_ref into g
                                select new
                                {
                                    cod_ref = g.Key,
                                    cantidad = g.Sum(item => item.cantidad),
                                    val_ref = g.Sum(item => item.val_ref),
                                    valiva = g.Sum(item => item.valiva),
                                    pordescto = g.Sum(item => item.pordescto),
                                    poriva = g.Sum(item => item.poriva),
                                    subtotal = g.Sum(item => item.subtotal),
                                    total = g.Sum(item => item.total),
                                };


                        foreach (var item in q)
                        {
                            if (item.cantidad > 0)
                            {
                                if (codtrn == "145")
                                {
                                    sql = sql + @"INSERT INTO incue_doc (idregcab,cod_trn,num_trn,cod_ref,cod_bod,cantidad,val_uni,val_iva,por_des,por_iva,subtotal,tot_tot,fecha_aded) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + item.cod_ref.ToString() + "','" + _bodOrigen + "'," + item.cantidad.ToString("F", CultureInfo.InvariantCulture) + "," + item.val_ref + "," + item.valiva.ToString("F", CultureInfo.InvariantCulture) + "," + item.pordescto.ToString("F", CultureInfo.InvariantCulture) + "," + item.poriva.ToString("F", CultureInfo.InvariantCulture) + "," + item.subtotal + "," + item.total.ToString("F", CultureInfo.InvariantCulture) + ",@fecdoc);";
                                    sqlcontra = sqlcontra + @"INSERT INTO incue_doc (idregcab,cod_trn,num_trn,cod_ref,cod_bod,cantidad,val_uni,val_iva,por_des,por_iva,subtotal,tot_tot,fecha_aded) values (@NewIDContra,'" + codtrncontra + "',@iConsecutivo,'" + item.cod_ref.ToString() + "','" + _bodDestino + "'," + item.cantidad.ToString("F", CultureInfo.InvariantCulture) + "," + item.val_ref + "," + item.valiva.ToString("F", CultureInfo.InvariantCulture) + "," + item.pordescto.ToString("F", CultureInfo.InvariantCulture) + "," + item.poriva.ToString("F", CultureInfo.InvariantCulture) + "," + item.subtotal + "," + item.total.ToString("F", CultureInfo.InvariantCulture) + ",@fecdoc);";
                                }
                                else
                                {
                                    sql = sql + @"INSERT INTO incue_doc (idregcab,cod_trn,num_trn,cod_ref,cod_bod,cantidad,fecha_aded) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + item.cod_ref.ToString() + "','" + _bodOrigen + "'," + item.cantidad.ToString("F", CultureInfo.InvariantCulture) + ",@fecdoc);";
                                    sqlcontra = sqlcontra + @"INSERT INTO incue_doc (idregcab,cod_trn,num_trn,cod_ref,cod_bod,cantidad,fecha_aded) values (@NewIDContra,'" + codtrncontra + "',@iConsecutivo,'" + item.cod_ref.ToString() + "','" + _bodDestino + "'," + item.cantidad.ToString("F", CultureInfo.InvariantCulture) + ",@fecdocsecond);";
                                }
                            }
                        }
                        command.CommandText = sqlcab + sql + sqlcabContra + sqlcontra + @"select CAST(@NewId AS int);";
                        //MessageBox.Show(command.CommandText.ToString());
                        var r = new object();
                        r = command.ExecuteScalar();
                        transaction.Commit();
                        connection.Close();
                        return Convert.ToInt32(r.ToString());
                        //bandera = true;
                    }
                    catch (Exception ex)
                    {
                        SiaWin.Func.SiaExeptionGobal(ex);
                        errorMessages.Append("c Error:#" + ex.Message.ToString());
                        transaction.Rollback();
                        MessageBox.Show(errorMessages.ToString());
                        bandera = -1;
                    }
                }
            }
            else
            {
                bandera = -1;
                dataGrid.Focus();
            }


            return bandera;
        }

        private void LoadData()
        {
            try
            {
                string TipoTrn = "";
                switch (CmbTipoCons.SelectedIndex)
                {
                    case 0:
                        TipoTrn = "141";
                        break;
                    case 1:
                        TipoTrn = "145";
                        break;
                    case 2:
                        TipoTrn = "146";
                        break;
                }

                StringBuilder _sql = new StringBuilder();
                ds.Clear();
                ds.Tables.Clear();
                _sql.Append("select InCab_doc.idreg,InCab_doc.cod_trn,InCab_doc.num_trn,InCab_doc.fec_trn,InCab_doc.bod_tra as cod_boddes,bodegaDes.nom_bod as bodegades,InCue_doc.cod_bod as cod_bodorg,bodegaOrigen.nom_bod as bodegaorigen, ");
                _sql.Append("InCue_doc.cod_ref,InMae_ref.nom_ref,InCue_doc.cantidad,InCue_doc.val_uni,InCue_doc.por_des,subtotal,tot_tot ");
                _sql.Append("from InCab_doc ");
                _sql.Append("inner join InCue_doc on InCab_doc.idreg = InCue_doc.idregcab ");
                _sql.Append("inner join InMae_bod as bodegaDes on InCab_doc.bod_tra = bodegaDes.cod_bod ");
                _sql.Append("inner join inmae_bod as bodegaOrigen on InCue_doc.cod_bod = bodegaOrigen.cod_bod ");
                _sql.Append("inner join InMae_ref on InCue_doc.cod_ref = InMae_ref.cod_ref ");
                _sql.Append("where InCab_doc.cod_trn='" + TipoTrn + "' and incab_doc.fec_trn between '" + FechaIni.Text + "' and '" + FechaFin.Text + " 23:59:59' order by InCab_doc.fec_trn,InCab_doc.num_trn");


                ds.Tables.Add(SiaWin.DB.SqlDT(_sql.ToString(), "Traslados", idemp));

                dataGridSF.ItemsSource = ds.Tables["Traslados"];
                TX_Total.Text = ds.Tables["Traslados"].Rows.Count.ToString();
                if (ds.Tables["Traslados"].Rows.Count > 0)
                {


                    dataGridSF.Focus();

                    dataGridSF.SelectedItem = 1;
                    dataGridSF.UpdateLayout();
                    //int id1x = dg.SelectedIndex;
                    dataGridSF.MoveCurrentCell(new RowColumnIndex(1, 1), false);


                }
            }
            catch (Exception ex)
            {
                SiaWin.Func.SiaExeptionGobal(ex);
                MessageBox.Show(ex.Message, "-PvTrasladosBodega-LoadData");
            }
        }

        private void Ejecutar_Click(object sender, RoutedEventArgs e)
        {

            if (CmbTipoCons.SelectedIndex < 0)
            {
                MessageBox.Show("Seleccione el tipo de transaccion");
                return;
            }
            dataGridSF.ClearFilters();
            LoadData();
        }

        private void ReImprimir_Click(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)dataGridSF.SelectedItems[0];
            if (row == null)
            {
                MessageBox.Show("Registro sin datos");
                return;
            }
            int numtrn = (int)row["idreg"];
            //string xter = (string)row["cod_ter"];
            ImprimeDocumentoTraslado((string)row["cod_trn"], (string)row["num_trn"], 1, false);
            //ImprimeDocumentoInv(numtrn, "TRASLADO BODEGA");
            //ImprimeDocumento(Convert.ToInt32(numtrn));
        }

        private void ExportaXLS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;
                var excelEngine = dataGridSF.ExportToExcel(dataGridSF.View, options);
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

                    if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archvo", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                throw;
            }
        }

        private void ImprimirDoc(int idregcab, string tipoImp)
        {
            string[] strArrayParam = new string[] { idregcab.ToString(), idemp.ToString(), tipoImp };
            SiaWin.Tab(9291, strArrayParam);
        }

        private void ImprimeDocumento(int iddoc)
        {
            // **** IMPRESION DE ENTRADA Y SALIDA DE TRASLADO
            numregcab = iddoc;
            //MessageBox.Show(ConfigCSource.numregcab.ToString());
            SqlConnection con = new SqlConnection(cnEmp);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            //DataSet dsImprimir = new DataSet();
            cmd = new SqlCommand("PvTraslados", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@_NumRegCab", numregcab);//if you have parameters.
            da = new SqlDataAdapter(cmd);
            dsImprimir.Clear();
            da.Fill(dsImprimir);
            int nItems = dsImprimir.Tables[1].Rows.Count;
            //int nItemsFpago = dsImprimir.Tables[1].Rows.Count;

            PrintDocument pd = new PrintDocument();

            System.Drawing.Printing.PaperSize ps = new PaperSize("", 290, 600 + (nItems * 20) + (120));
            pd.PrintPage += new PrintPageEventHandler(pd_imprimefactura);

            pd.PrintController = new StandardPrintController();
            pd.DefaultPageSettings.Margins.Left = 0;
            pd.DefaultPageSettings.Margins.Right = 0;
            pd.DefaultPageSettings.Margins.Top = 0;
            pd.DefaultPageSettings.Margins.Bottom = 0;
            pd.DefaultPageSettings.PaperSize = ps;
            pd.Print();
            ExecuteSqlTransactionCabReeimprime(numregcab);
        }

        //********** IMPRIME FACTURAS

        private void ExecuteSqlTransactionCabReeimprime(int idcab)
        {
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
                    string sqlcab = @"update incab_doc set est_imp=est_imp+1 where idreg=" + idcab.ToString();
                    command.CommandText = sqlcab;
                    command.ExecuteScalar();
                    transaction.Commit();
                    connection.Close();
                }
                catch (SqlException ex)
                {
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        errorMessages.Append(" SQL-Index #" + i + "\n" + "Message: " + ex.Errors[i].Message + "\n" + "LineNumber: " + ex.Errors[i].LineNumber + "\n" + "Source: " + ex.Errors[i].Source + "\n" + "Procedure: " + ex.Errors[i].Procedure + "\n");
                    }
                    transaction.Rollback();
                    MessageBox.Show(errorMessages.ToString());

                }
                catch (Exception ex)
                {
                    errorMessages.Append("c Error:#" + ex.Message.ToString());
                    transaction.Rollback();
                    MessageBox.Show(errorMessages.ToString());
                }

            }
        }

        private void pd_imprimefactura(object sender, PrintPageEventArgs e)
        {
            try
            {
                string rowValue1 = "";
                int pos1 = 0;
                System.Drawing.Graphics g = e.Graphics;
                System.Drawing.Font fBody = new System.Drawing.Font("Lucida Console", 7, System.Drawing.FontStyle.Bold);
                System.Drawing.Font fBody1 = new System.Drawing.Font("Lucida Console", 7, System.Drawing.FontStyle.Bold);
                System.Drawing.Font fTitulo1 = new System.Drawing.Font("Lucida Console", 12, System.Drawing.FontStyle.Bold);
                System.Drawing.SolidBrush sb = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
                /// alinear valores derecha-izquierda
                System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
                drawFormat.Alignment = System.Drawing.StringAlignment.Far;
                drawFormat.LineAlignment = System.Drawing.StringAlignment.Near;
                /// alinear al centro
                System.Drawing.StringFormat drawFormatCenter = new System.Drawing.StringFormat();
                drawFormatCenter.Alignment = System.Drawing.StringAlignment.Center;
                drawFormatCenter.LineAlignment = System.Drawing.StringAlignment.Near;
                string pathlogo = SiaWin._PathApp + @"\imagenes\" + idLogo.ToString() + "..png";
                e.Graphics.DrawImage(System.Drawing.Image.FromFile(pathlogo), 100, 1, 75, 75);
                String s = BusinessName.Trim();
                //      s += "Nit:"+BusinessNit.Trim();
                System.Drawing.Font f = new System.Drawing.Font("Arial", 12);
                System.Drawing.StringFormat sf = new System.Drawing.StringFormat();

                sf.Alignment = System.Drawing.StringAlignment.Center;        // horizontal alignment
                sf.LineAlignment = System.Drawing.StringAlignment.Near;    // vertical alignment
                pos1 = 15;
                System.Drawing.Rectangle r = new System.Drawing.Rectangle(10, 75, 270, f.Height * 1);
                g.DrawRectangle(System.Drawing.Pens.Black, r);
                g.DrawString(s, f, System.Drawing.Brushes.Black, r, sf);


                //     g.DrawString(BusinessName.Trim(),  fTitulo1,sb,100,pos1);
                //     pos1=25;
                //    int ancho=4+BusinessNit.Trim().Length;   
                //    g.DrawString("Nit:"+BusinessNit.Trim(), fTitulo1,sb,(300-ancho)/2,pos1);

                int _Reimpresion = Convert.ToInt32(dsImprimir.Tables[0].Rows[0]["est_imp"].ToString());

                string _TipoDoc = dsImprimir.Tables[0].Rows[0]["cod_trn"].ToString();
                string _BodTra = dsImprimir.Tables[0].Rows[0]["bod_tra"].ToString();
                string _NumDocAnula = dsImprimir.Tables[0].Rows[0]["des_mov"].ToString();
                string _TituloDoc = "SALIDA TRASLADO:";
                if (_TipoDoc == "051") _TituloDoc = "ENTRADA TRASLADO:";
                String nombodtra = string.Empty;
                DataRow foundRow = dtBod.Rows.Find(_BodTra);
                if (foundRow != null)
                {
                    nombodtra = foundRow["nom_bod"].ToString().Trim();
                }


                pos1 = 85;
                pos1 = pos1 + 10;
                g.DrawString("                 Nit:" + BusinessNit, fBody1, sb, 1, pos1);
                pos1 = pos1 + 10;
                g.DrawString("          TIPO IVA", fBody1, sb, 1, pos1);
                pos1 = pos1 + 10;
                g.DrawString("              REGIMEN EMPRESA", fBody1, sb, 1, pos1);
                pos1 = pos1 + 10;
                g.DrawString("               RES 0000", fBody1, sb, 1, pos1);
                pos1 = pos1 + 10;

                g.DrawString("----------------------------------------------", fBody1, sb, 1, pos1);
                pos1 = pos1 + 10;
                g.DrawString(_TituloDoc, fBody1, sb, 1, pos1);
                rowValue1 = _TipoDoc + "/" + dsImprimir.Tables[0].Rows[0]["num_trn"].ToString();
                g.DrawString(rowValue1, fBody1, sb, 110, pos1);
                pos1 = pos1 + 10;
                g.DrawString("FECHA          :", fBody1, sb, 1, pos1);
                rowValue1 = dsImprimir.Tables[0].Rows[0]["fec_trn"].ToString();
                g.DrawString(rowValue1, fBody1, sb, 110, pos1);
                pos1 = pos1 + 10;
                g.DrawString("BODEGA ORIGEN  :", fBody1, sb, 1, pos1);
                g.DrawString(nompvta.Trim(), fBody1, sb, 110, pos1);
                pos1 = pos1 + 10;
                if (_TipoDoc == "141")
                {
                    g.DrawString("BODEGA DESTINO :", fBody1, sb, 1, pos1);
                    g.DrawString(nombodtra, fBody1, sb, 110, pos1);
                    pos1 = pos1 + 10;
                }
                g.DrawString("----------------------------------------------", fBody1, sb, 1, pos1);
                pos1 = pos1 + 10;
                g.DrawString("REFERENCIA", fBody1, sb, 1, pos1);
                pos1 = pos1 + 10;
                g.DrawString("CANT                DESCRIPCION               ", fBody1, sb, 1, pos1);
                pos1 = pos1 + 10;
                g.DrawString("----------------------------------------------", fBody1, sb, 1, pos1);
                pos1 = pos1 + 10;

                int itemCount = 0;
                foreach (DataRow row in dsImprimir.Tables[1].Rows)
                {
                    itemCount = itemCount + 1;

                    rowValue1 = row["cantidad"].ToString() + " -" + row["cod_ref"].ToString();
                    g.DrawString(rowValue1, fBody1, sb, 1, pos1);
                    pos1 = pos1 + 10;
                    //         rowValue1 =row["cantidad"].ToString()+" "+row["nom_ref"].ToString()+" "+row["val_uni"].ToString()+" "+row["total_"].ToString();
                    rowValue1 = row["nom_ref"].ToString();
                    g.DrawString(rowValue1, fBody1, sb, 1, pos1);
                    if (dsImprimir.Tables[1].Rows.Count > 1)
                    {
                        if (itemCount < dsImprimir.Tables[1].Rows.Count)
                        {
                            pos1 = pos1 + 10;
                            g.DrawString("- - - - - - - - - - - - - - - - - - - - - - ", fBody1, sb, 1, pos1);
                        }
                    }

                    pos1 = pos1 + 10;
                }

                g.DrawString("----------------------------------------------", fBody1, sb, 1, pos1);
                pos1 = pos1 + 10;
                g.DrawString("NUMERO DE ARTICULOS TRASLADADOS ", fBody1, sb, 1, pos1);
                rowValue1 = dsImprimir.Tables[2].Rows[0]["gcantidad"].ToString();
                g.DrawString(rowValue1, fBody1, sb, 211, pos1);
                pos1 = pos1 + 10;
                g.DrawString("----------------------------------------------", fBody1, sb, 1, pos1);
                g.DrawString("", fBody1, sb, 5, pos1 + 20);
                pos1 = pos1 + 10;
                string _cabesa = "*" + numregcab.ToString().Trim() + "*";
                pos1 = pos1 + 10;

                System.Drawing.Font CbarFree = new System.Drawing.Font("IDAHC39M Code 39 Barcode", 13, System.Drawing.FontStyle.Regular);
                g.DrawString(_cabesa, CbarFree, sb, 80, pos1);
                //codigo de barras *********************************************************************************************************
                pos1 = pos1 + 140;
                g.DrawString("----------------------------------------------", fBody1, sb, 1, pos1);
                pos1 = pos1 + 10;
                g.DrawString(" ELABORADO POR           REVISADO POR        ", fBody1, sb, 1, pos1);
                pos1 = pos1 + 20;
                //       g.DrawString("", fBody1,sb,5,pos1+20);
                //     pos1=pos1+20;
                //   g.DrawString("*", fBody1,sb,5,pos1+10);
                //       g.DrawString("*** REIMPRESA *** ", fTitulo1,sb,5,pos1+30);



                //g.DrawString("*", fBody1, sb, 5, pos1 + 10);
                if (_Reimpresion > 1) g.DrawString("*** REIMPRESA *** ", fTitulo1, sb, 55, pos1 + 10);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Imprime Factura:" + ex.ToString());
            }
        }

        void ImprimeDocumentoInv(int iddocu, string titulo)
        {
            //Sia.PublicarPnt(9461,"DocumentosReportes");
            try
            {
                dynamic Pnt9461 = SiaWin.WindowExt(9461, "DocumentosReportes");  //carga desde sql
                Pnt9461.TituloReporte = titulo;
                Pnt9461.idEmp = idEmp;
                Pnt9461.DocumentoIdCab = iddocu;
                Pnt9461.ReportPath = @"/Otros/FrmDocumentos/PvTraslado" + BusinessCode;
                Pnt9461.Copias = 2;
                Pnt9461.DirecPrinter = false;
                //Pnt9461.codemp = BusinessCode;
                //string nameprinterreport = Pventas.Rows[0]["nameprint"].ToString().Trim();
                //if (!string.IsNullOrEmpty(nameprinterreport)) Pnt9461.printName = nameprinterreport;
                Pnt9461.ShowInTaskbar = false;
                Pnt9461.Owner = Application.Current.MainWindow;
                Pnt9461.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                Pnt9461.Show();
                Pnt9461 = null;
            }
            catch (System.Exception _error)
            {
                MessageBox.Show(_error.Message);
            }
        }

        private void CmbTipoCons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (CmbTipoCons.SelectedIndex >= 0)
            {
                switch (CmbTipoCons.SelectedIndex)
                {
                    case 0:
                        COLMval_uni.IsHidden = true;
                        COLMsubtotal.IsHidden = true;
                        COLMpor_des.IsHidden = true;
                        COLMtot_tot.IsHidden = true;
                        dataGridSF.ItemsSource = null;
                        TX_Total.Text = "-";
                        break;
                    case 1:
                        COLMval_uni.IsHidden = false;
                        COLMsubtotal.IsHidden = false;
                        COLMpor_des.IsHidden = false;
                        COLMtot_tot.IsHidden = false;
                        dataGridSF.ItemsSource = null;
                        TX_Total.Text = "-";
                        break;
                    case 2:
                        COLMval_uni.IsHidden = true;
                        COLMsubtotal.IsHidden = true;
                        COLMpor_des.IsHidden = true;
                        COLMtot_tot.IsHidden = true;
                        dataGridSF.ItemsSource = null;
                        TX_Total.Text = "-";
                        break;
                    default:
                        break;
                }

            }

        }
        private void ImprimeDocumentoTraslado(string codtrn, string numtrn, int Reimprimir, bool traslado)
        {

            if (string.IsNullOrEmpty(codtrn)) return;
            if (string.IsNullOrEmpty(numtrn)) return;


            if (traslado == false)
            {
                if (dataGridSF.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione el documento a imprimir");
                    return;
                }

                if (ds.Tables[0].Rows.Count <= 0)
                {
                    MessageBox.Show("No hay registros para exportar..");
                    return;
                }
            }

            try
            {

                List<ReportParameter> parameters = new List<ReportParameter>();
                ReportParameter paramcodemp = new ReportParameter();
                paramcodemp.Values.Add(BusinessCode);
                paramcodemp.Name = "codemp";
                parameters.Add(paramcodemp);
                ReportParameter paramtrn = new ReportParameter();
                paramtrn.Name = "codtrn";
                paramtrn.Values.Add(codtrn);
                parameters.Add(paramtrn);
                ReportParameter paramnum = new ReportParameter();
                paramnum.Values.Add(numtrn);
                paramnum.Name = "numtrn";
                parameters.Add(paramnum);
                ReportParameter paramReim = new ReportParameter();
                paramReim.Values.Add(Reimprimir.ToString());
                paramReim.Name = "Reimprime";
                parameters.Add(paramReim);
                if (codtrn != "141")
                {
                    int impvalores = 1;
                    if (MessageBox.Show("Imprime Valores", "Siasoft", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        impvalores = 0;
                    }
                    ReportParameter paramValores = new ReportParameter();
                    paramValores.Values.Add(impvalores.ToString());
                    paramValores.Name = "ImprimeValores";
                    parameters.Add(paramValores);

                }

                string TipoReporte = @"/Otros/FrmDocumentos/PvTrasladosBodega141";
                if (codtrn == "145") TipoReporte = @"/Otros/FrmDocumentos/PvTrasladosBodega145";
                if (codtrn == "146") TipoReporte = @"/Otros/FrmDocumentos/PvTrasladosBodega145";
                string TituloAuditoria = "Traslado de Bodega:";
                if (codtrn == "145") TituloAuditoria = "Traslado Bodega Consignacion";
                if (codtrn == "146") TituloAuditoria = "Traslado Bodega Consignacion - Anulacion";

                SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idemp, 0, 0, 0, TituloAuditoria+":" + codtrn + "-" + numtrn, "");
                //Reportes(List<ReportParameter> parameters, string reporteNombre, bool Modal = true, string TituloReporte = "", bool DirecPrinter = false, int Copias = 1, string PrintName = "", int ZoomPercent = 0, int idemp = -1)
                SiaWin.Reportes(parameters, TipoReporte, Modal: true);
                //ReportCxC rp = new ReportCxC(parameters, TipoReporte);
                //parameters, @"/Contabilidad/Balances/BalanceGeneral"
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void dataGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            CmbTipoDoc.IsEnabled = false;
        }
    }
}

