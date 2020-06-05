using System;
using System.Windows;
using System.Windows.Controls;
using WindowPV;
namespace SiasoftAppExt
{
    public partial class WindowPV : Window
    {
        public int idemp = 0;
        public int idregcabReturn = -1;
        public string codtrn = string.Empty;
        public string numtrn = string.Empty;
        public WindowPV()
        {
            InitializeComponent();
            pantalla();

        }
        public void pantalla() {
            this.MaxHeight = 400;
            this.MinHeight = 400;
            this.MinWidth = 400;
            this.MaxWidth = 400;
        }
        //Sia.PublicarPnt(9460,"WindowPV");
        private void BTNcontizaciion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cotizaciones ventana = new Cotizaciones(idemp);
                ventana.idemp = idemp;
                ventana.ShowInTaskbar = false;
                ventana.Owner = Application.Current.MainWindow;
                var newItem = new ComboBoxItem();
                newItem.Tag = "505";
                newItem.Content = "Pedidos";
                ventana.TextBxCB_consulta.Items.Add(newItem);
                var newItem2 = new ComboBoxItem();
                newItem2.Tag = "011";
                newItem2.Content = "Cotizado";
                ventana.TextBxCB_consulta.Items.Add(newItem2);
                
                ventana.ShowDialog();
                idregcabReturn = Convert.ToInt32(ventana.idregcabReturn.ToString());
                codtrn = ventana.codtrn.ToString();
                numtrn = ventana.numtrn.ToString();
                this.Close();
            }
            catch (Exception w)
            {
                MessageBox.Show("error 55" + w);
            }
        }
    }
}
