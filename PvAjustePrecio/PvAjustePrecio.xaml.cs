using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{

    //Sia.PublicarPnt(9473,"PvAjustePrecio");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9473,"PvAjustePrecio");    
    //ww.ShowInTaskbar=false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
    //ww.ShowDialog(); 

    public partial class PvAjustePrecio : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public string codref = "";
        public string nomref = "";
        public decimal valrefactual = 0;
        public decimal valreturn = 0;

        public double iva = 0;
        public double precioLista = 0;

        public string tercero = "";

        decimal DecuentoLinea = 0;
        public decimal porcentajeNuevo = 0;


        public PvAjustePrecio()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            //idemp = SiaWin._BusinessId;           
        }

        //public void Getlinea(string referencia)
        //{
        //    try
        //    {
        //        string cadena = "select cod_tip from InMae_ref where cod_ref='" + referencia + "' ";
        //        DataTable dt = SiaWin.Func.SqlDT(cadena, "Clientes", idemp);
        //        string linea = dt.Rows[0]["cod_tip"].ToString();

        //        string porcentaje = "select por_desc from InMae_tip where cod_tip='" + linea + "' ";
        //        DataTable dtPor = SiaWin.Func.SqlDT(porcentaje, "Clientes", idemp);
        //        DecuentoLinea = Convert.ToDecimal(dtPor.Rows[0]["por_desc"]);                
        //    }
        //    catch (Exception)
        //    {
        //        MessageBox.Show("error q1");
        //    }
        //}

        private void LoadConfig()
        {
            try
            {
                DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Ajuste de Precio - Empresa:" + cod_empresa + "-" + nomempresa;
                TxtNombre.Text = nomref.Trim();

                TxtValorUnitario.Culture = new CultureInfo("en-US");
                TxtValorUnitario.Value = valrefactual;

                TxtVlrAjuste.Culture = new CultureInfo("en-US");
                TxtVlrAjuste.Value = valrefactual;
                //TxtVlrAjuste.DataContext = this;
                TxtVlrAjuste.Focus();

                TX_referencia.Text = codref;
                //Getlinea(codref);
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {            
            LoadConfig();
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

        private void TxtVlrAjuste_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = !IsNumberKey(e.Key) && !IsDelOrBackspaceOrTabKey(e.Key);
        }

        private void BTNterminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double valor = Convert.ToDouble(TxtVlrAjuste.Value);
                validarPor(valor);

                valreturn = Convert.ToDecimal(TxtVlrAjuste.Value);
                this.Close();
            }
            catch (Exception w)
            {

                MessageBox.Show("PPPPPPPPPP:"+w);
            }
            
            //if (validarPor(valor) == false)
            //{
            //    valreturn = Convert.ToDecimal(TxtVlrAjuste.Value);
            //    this.Close();
            //}
            //MessageBox.Show("TxtVlrAjuste:" + TxtVlrAjuste.Value);
        }

        public void validarPor(Double NueVal)
        {
            try
            {
                //MessageBox.Show("1");
                string IvaPor = "1." + iva;
                //MessageBox.Show("2:"+ IvaPor);

                decimal ValorIvaCon = Convert.ToDecimal(IvaPor);
                //MessageBox.Show("2.5");
                double ValorNuevo = NueVal * Convert.ToDouble(ValorIvaCon);
                //MessageBox.Show("3");
                double val = (precioLista - ValorNuevo) / precioLista;
                double Porcentaje = val * 100;
                //MessageBox.Show("4");
                porcentajeNuevo = decimal.Round(Convert.ToDecimal(Porcentaje), 2, MidpointRounding.AwayFromZero);
                //MessageBox.Show("5");
            }
            catch (Exception w)
            {

                MessageBox.Show("Valiiii:"+w);
            }
            

            //MessageBox.Show("ivaSu" + ivaSu);
            //if (porcentajeNuevo > DecuentoLinea)
            //{
            //    MessageBox.Show("el procentaje es mas alto que el decuento permitido por la linea ");
            //    MessageBox.Show("porcentajeNuevo:"+ porcentajeNuevo + " DecuentoLinea:"+ DecuentoLinea);
            //    return true;
            //}
            //else
            //{
            //    MessageBox.Show("es menor");
            //    MessageBox.Show("porcentajeNuevo:" + porcentajeNuevo + " DecuentoLinea:" + DecuentoLinea);
            //    return false;
            //}

        }


        private void Btncancelar_Click(object sender, RoutedEventArgs e)
        {
            valrefactual = 0;
            this.Close();
        }




    }
}
