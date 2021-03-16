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

    //Sia.PublicarPnt(9649,"BorrarDocumentoCO");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9649,"BorrarDocumentoCO");
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //ww.ShowDialog();

    public partial class BorrarDocumentoCO : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";


        const string titulo = "Contabilidad";
        const string transaccion = "comae_trn";
        const string doc_cabeza = "cocab_doc";
        string doc_cuerpo = "cocue_doc";
        const string modulo = "co";
        const int idmodulo = 1;

        public BorrarDocumentoCO()
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
                this.Title = "Eliminacion de documento de " + titulo;

                TxFecIni.Text = DateTime.Now.ToString();
                TxFecFin.Text = DateTime.Now.ToString();
                

            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                if (e.Key == Key.F8)
                {


                    string tag = (sender as TextBox).Tag.ToString();

                    string table = "", code = "", name = "", title = "", idrow = "";
                    switch (tag)
                    {
                        case (modulo + "mae_trn"):
                            table = tag; code = "cod_trn"; name = "nom_trn"; title = "maestra de tranascciones"; idrow = "idrow";
                            break;
                        case (modulo + "cab_doc"):
                            table = tag; code = "cod_trn"; name = "num_trn"; title = "documentos"; idrow = "idreg";
                            break;
                    }


                    int xidr = 0; string xcode = ""; string xnom = "";
                    dynamic winb = SiaWin.WindowBuscar(table, code, name, code, idrow, title, SiaWin.Func.DatosEmp(idemp), false, "", idEmp: idemp);
                    winb.ShowInTaskbar = false;
                    winb.Owner = Application.Current.MainWindow;
                    winb.Height = 300;
                    winb.Width = 400;
                    winb.ShowDialog();
                    xidr = winb.IdRowReturn;
                    xcode = winb.Codigo;
                    xnom = winb.Nombre;
                    winb = null;

                    if (!string.IsNullOrEmpty(xcode))
                    {
                        (sender as TextBox).Text = tag == (modulo + "mae_trn") ? xcode.Trim() : xnom.Trim();

                        var uiElement = e.OriginalSource as UIElement;
                        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    }
                    e.Handled = true;
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al buscar:" + w);
            }
        }


        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {

                string tag = (sender as TextBox).Tag.ToString().Trim();
                string value = (sender as TextBox).Text.ToString().Trim();

                if (string.IsNullOrEmpty(value)) return;


                string table = "", message = "";
                switch (tag)
                {
                    case (modulo + "mae_trn"):
                        table = tag; message = "codigo";
                        break;
                    case (modulo + "cab_doc"):
                        table = tag; message = "documento";
                        break;
                }

                string query = "select * from " + table + " where " + (tag == (modulo + "mae_trn") ? "cod_trn" : "num_trn") + "='" + value + "';  ";
                DataTable dt = SiaWin.Func.SqlDT(query, "temp", idemp);
                if (dt.Rows.Count <= 0)
                {
                    MessageBox.Show("el " + message + " ingresado no existe", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    (sender as TextBox).Text = "";
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al validar:" + w);
            }
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnConsutar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                string fec_ini = TxFecIni.Text;
                string fec_fin = TxFecFin.Text;
                string cod_ini = TrnIni.Text;
                string cod_fin = TrnFin.Text;
                string num_ini = NumIni.Text;
                string num_fin = NumFin.Text;

                string where = "";
                if (!string.IsNullOrEmpty(cod_ini) && !string.IsNullOrEmpty(cod_fin))
                    where += "and cod_trn between '" + cod_ini + "' and '" + cod_fin + "'  ";


                if (!string.IsNullOrEmpty(num_ini) && !string.IsNullOrEmpty(num_fin))
                    where += "and num_trn between '" + num_ini + "' and '" + num_fin + "'  ";


                StringBuilder query = new StringBuilder();
                query.Append("select cod_trn,num_trn,convert(char,fec_trn,103) as fec_trn from " + doc_cabeza + " ");
                query.Append("where CONVERT(date,fec_trn) between '" + fec_ini + "' and '" + fec_fin + "' ");
                query.Append(where);
                query.Append("order by cod_trn,num_trn  ");

                //MessageBox.Show(query.ToString());

                DataTable dt = SiaWin.Func.SqlDT(query.ToString(), "temp", idemp);
                if (dt.Rows.Count > 0) SiaWin.Browse(dt);
                else MessageBox.Show("no existen documentos con los filtros ingresados", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            }
            catch (Exception w)
            {
                MessageBox.Show("error al consultar:" + w);
            }
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                MessageBoxResult result = MessageBox.Show("Usted desea eliminar los documentos seleccionados en filtro?", "Confirmacion", MessageBoxButton.YesNo,
MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    string fec_ini = TxFecIni.Text;
                    string fec_fin = TxFecFin.Text;
                    string cod_ini = TrnIni.Text.Trim();
                    string cod_fin = TrnFin.Text.Trim();
                    string num_ini = NumIni.Text.Trim();
                    string num_fin = NumFin.Text.Trim();

                    string where = "";
                    if (!string.IsNullOrEmpty(cod_ini) && !string.IsNullOrEmpty(cod_fin))
                        where += "and cod_trn between '" + cod_ini + "' and '" + cod_fin + "'  ";


                    if (!string.IsNullOrEmpty(num_ini) && !string.IsNullOrEmpty(num_fin))
                        where += "and num_trn between '" + num_ini + "' and '" + num_fin + "'  ";


                    StringBuilder query = new StringBuilder();
                    query.Append("select idreg,cod_trn,num_trn,fec_trn from " + doc_cabeza + " ");
                    query.Append("where CONVERT(date,fec_trn) between '" + fec_ini + "' and '" + fec_fin + "' ");
                    query.Append(where);
                    query.Append("order by cod_trn,num_trn  ");


                    DataTable dt = SiaWin.Func.SqlDT(query.ToString(), "temp", idemp);
                    if (dt.Rows.Count > 0)
                    {
                        string auditoria = "ejecuto el proceso de eliminacion de documentos contables por fecha,tipo y numero. los siguientes fueron los documentos eliminados: " + Environment.NewLine;

                        string cuerpo = "";
                        foreach (DataRow dr in dt.Rows)
                        {
                            string idreg = dr["idreg"].ToString().Trim();
                            string cod_trn = dr["cod_trn"].ToString().Trim();
                            string num_trn = dr["num_trn"].ToString().Trim();
                            string fec_trn = dr["fec_trn"].ToString().Trim();
                            auditoria += cod_trn + "-" + num_trn + "; ";
                            cuerpo += "delete " + doc_cuerpo + "  where idregcab='" + idreg + "';";
                        }


                        StringBuilder delete = new StringBuilder();
                        delete.Append("delete " + doc_cabeza + " ");
                        delete.Append("where CONVERT(date,fec_trn) between '" + fec_ini + "' and '" + fec_fin + "' ");
                        delete.Append(where + ";");
                        delete.Append(cuerpo);


                        if (SiaWin.Func.SqlCRUD(delete.ToString(), idemp) == true)
                        {
                            MessageBox.Show("la eliminacion se ejecuto exitosamente", "alerta", MessageBoxButton.OK, MessageBoxImage.Information);
                            SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, idmodulo, -1, -9, auditoria, "");

                            TrnIni.Text = "";
                            TrnFin.Text = "";
                            NumIni.Text = "";
                            NumFin.Text = "";
                        }

                    }
                    else
                    {
                        MessageBox.Show("no se elimino ningun registro por que los filtros seleccionados no contienen ningun documento", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }

                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al eliminar documentos:" + w);
            }

        }


    }

}
