using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace RecibosDeCaja
{
    /// <summary>
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class EntradasLista : Window
    {
        public int idregcab = 0;
        public EntradasLista(string bod)
        {
            InitializeComponent();
            LoadEntradas(bod);
        }
        private void LoadEntradas(string bod)
        {


        }

        private void BtnLoadEntrada_Click(object sender, RoutedEventArgs e)
        {
            if (CmbListaEntrada.SelectedIndex >= 0)
            {
                idregcab = Convert.ToInt32(CmbListaEntrada.SelectedValue);
                this.Close();
            }
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
