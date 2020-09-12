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

        string titulo = "Activos Fijos";
        string transaccion = "afmae_trn";
        string doc_cabeza = "afcab_doc";
        string doc_cuerpo = "afcue_doc";
        int idmodulo = 8;

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

                TxFecIni.Text = DateTime.Now.ToString();
                TxFecFin.Text = DateTime.Now.ToString();
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("error en el load" + e.Message);                
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
                    if (MessageBox.Show("Usted desea eliminar el documento" + Tx_doc.Text, "Eliminar Documento", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        string delete = "delete " + doc_cabeza + " where num_trn='" + Tx_doc.Text + "'  and cod_trn='" + Tx_trns.Text + "';  ";
                        delete += "delete " + doc_cuerpo + " where num_trn='" + Tx_doc.Text + "'  and cod_trn='" + Tx_trns.Text + "'  ";

                        if (SiaWin.Func.SqlCRUD(delete, idemp) == true)
                        {
                            SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, idmodulo, -1, -9, "ELIMINO EXITOSAMENTE DOCU:" + Tx_doc.Text+ "- TRN" + Tx_trns.Text + " DE:" + titulo, "");

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

        // segun tab ----------------------------

        private void BtnEliminarRange_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                #region validacion                

                if (string.IsNullOrEmpty(TxFecIni.Text) || string.IsNullOrEmpty(TxFecFin.Text))
                {
                    MessageBox.Show(this, "debe de llenar los campos de fechas", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (string.IsNullOrEmpty(Tx_Rangotrns.Text))
                {
                    MessageBox.Show(this, "debe de llenar el campos de transacciones", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                #endregion

                if (MessageBox.Show("Usted desea eliminar el rango de transacciones " + Tx_Rangotrns.Text + " de las fechas " + TxFecIni.Text + "-" + TxFecFin.Text, "Eliminar Documento", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    string delete = "delete cue from " + doc_cuerpo + " cue inner join " + doc_cabeza + " cab on cab.idreg = cue.idregcab  ";
                    delete += "where convert(date,cab.fec_trn,105) between '" + TxFecIni.Text + "' and '" + TxFecFin.Text + "' and cab.cod_trn='" + Tx_Rangotrns.Text + "' ";

                    if (SiaWin.Func.SqlCRUD(delete, idemp) == true)
                    {
                        string delecab = "delete " + doc_cabeza + " where convert(date," + doc_cabeza + ".fec_trn,105) between '" + TxFecIni.Text + "' and '" + TxFecFin.Text + "' and cod_trn='" + Tx_Rangotrns.Text + "' ";

                        if (SiaWin.Func.SqlCRUD(delecab, idemp) == true)
                        {

                            SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, idmodulo, -1, -9, "ELIMINO EXITOSAMENTE FECHA INI" + TxFecIni.Text + "- FECHA FINAL" + TxFecFin.Text + "  TRN:" + Tx_Rangotrns.Text + " DE:" + titulo, "");

                            MessageBox.Show("la eliminacion fue exitosa", "proceso", MessageBoxButton.OK, MessageBoxImage.Information);
                            Tx_Rangotrns.Text = "";
                        }
                    }

                }


            }
            catch (Exception w)
            {
                MessageBox.Show("error al eliminar rango de documentos:" + w);
            }
        }






    }


}
