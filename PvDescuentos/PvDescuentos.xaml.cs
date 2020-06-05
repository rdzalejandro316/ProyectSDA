using System.Windows;
using System.Windows.Input;

namespace SiasoftAppExt
{
    /// <summary>
    /// Lógica de interacción para UserControl1.xaml
    /// </summary>
    public partial class PvDescuentos : Window
    {
        public decimal Valor = 0;
        public bool Tipo = false;
        public string Codigo = string.Empty;
        public string Titulo = "Descuento por %";
        public PvDescuentos()
        {

            InitializeComponent();
            this.TxtCodigo.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(TxtCodigo.Text.Trim()))
            {
                MessageBox.Show("Falta Codigo de descuento...");
                TxtCodigo.Focus();
                e.Handled = true;
                return;
            }
            if(Tipo==true)
            {
                if(TxtValor.Value<=0)
                {
                    MessageBox.Show("Falta Valor de Descuento...");
                    TxtValor.Focus();
                    return;
                }
            }
            Codigo = TxtCodigo.Text.Trim();
            if (Tipo == true)
            {
                Valor = (decimal)TxtValor.Value;
            }
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Tipo)
            {
                TextValor.Visibility = Visibility.Visible;
                TxtValor.Visibility = Visibility.Visible;
                TxtTitulo.Text = "Descuento por Valor";
            }
        }

        private void TxtCodigo_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if(Tipo) TxtValor.Focus();
                if (!Tipo) BtnContinuar.Focus();

                //var uiElement = this as UIElement;
                //uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }

        }
    }
}
