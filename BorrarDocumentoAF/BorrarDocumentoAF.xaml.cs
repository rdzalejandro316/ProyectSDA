using System;
using System.Collections.Generic;
using System.Data;
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

    //    Sia.PublicarPnt(9652,"BorrarDocumentoAF");
    //    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9652,"BorrarDocumentoAF");
    //    ww.ShowInTaskbar = false;
    //    ww.Owner = Application.Current.MainWindow;
    //    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //    ww.ShowDialog();

    public partial class BorrarDocumentoAF : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        string transaccion = "afmae_trn";
        string doc_cabeza = "afcab_doc";
        string doc_cuerpo = "afcue_doc";

        public BorrarDocumentoAF()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            LoadConfig();
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
                this.Title = "Eliminacion de documento Activos Fijos";

            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("error en el load" + e.Message);

                System.Windows.MessageBox.Show("","alerta", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Exclamation);
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                int idr = 0; string code = ""; string nom = "";
                dynamic winb = SiaWin.WindowBuscar(transaccion, "cod_trn", "nom_trn", "cod_trn", "idrow", "Maestra de transaciones", SiaWin.Func.DatosEmp(idemp), true, "", idEmp: idemp);
                winb.ShowInTaskbar = false;
                winb.Owner = Application.Current.MainWindow;
                winb.Height = 300;
                winb.Width = 400;
                winb.ShowDialog();
                idr = winb.IdRowReturn;
                code = winb.Codigo;
                nom = winb.Nombre;
                winb = null;
                if (idr > 0)
                {
                    (sender as TextBox).Text = code.Trim();
                    var uiElement = e.OriginalSource as UIElement;
                    uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                }
                e.Handled = true;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al buscar:" + w);
            }
        }



        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Tx_trns.Text))
                {
                    MessageBox.Show("el tipo de transaccion no debe de estar vacio", "ALERTA", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                if (string.IsNullOrWhiteSpace(Tx_doc.Text))
                {
                    MessageBox.Show("el numero del documento no debe de estar vacio", "ALERTA", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                string query = "select * from  " + doc_cabeza + " where num_trn='" + Tx_doc.Text + "'  and cod_trn='" + Tx_trns.Text + "' ";
                DataTable dt = SiaWin.Func.SqlDT(query, "tabla", idemp);
                if (dt.Rows.Count > 0)
                {
                    if (MessageBox.Show("Usted desea elimianr el documento" + Tx_doc.Text, "Eliminar Documento", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        string delete = "delete " + doc_cabeza + " where num_trn='" + Tx_doc.Text + "'  and cod_trn='" + Tx_trns.Text + "';  ";
                        delete += "delete " + doc_cuerpo + " where num_trn='" + Tx_doc.Text + "'  and cod_trn='" + Tx_trns.Text + "'  ";

                        if (SiaWin.Func.SqlCRUD(delete, idemp) == true)
                        {
                            MessageBox.Show("la eliminacion fue exitosa", "proceso", MessageBoxButton.OK, MessageBoxImage.Information);
                            Tx_doc.Text = "";
                            Tx_trns.Text = "";
                        }
                    }
                }
                else
                {
                    MessageBox.Show("el documento que ingreso no existe", "ALERTA", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }


            }
            catch (Exception w)
            {
                MessageBox.Show("error al eliminar el documento", "ALERTA", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }









    }


}
