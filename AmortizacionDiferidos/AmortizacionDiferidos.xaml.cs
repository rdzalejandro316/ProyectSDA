using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
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

    //    Sia.PublicarPnt(9646,"AmortizacionDiferidos");
    //    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9646,"AmortizacionDiferidos");
    //    ww.ShowInTaskbar = false;
    //    ww.Owner = Application.Current.MainWindow;
    //    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //    ww.ShowDialog();

    public partial class AmortizacionDiferidos : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        bool search = false;

        public AmortizacionDiferidos()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
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
                this.Title = "Amortizacion diferidos " + cod_empresa + "-" + nomempresa;
                cnEmp = SiaWin.Func.DatosEmp(idemp);
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void Btnadd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                search = false;
                BtnEdit.IsEnabled = true;
                clearValues();
                desblo();
                Panel2.Visibility = Visibility.Visible;
                Panel1.Visibility = Visibility.Hidden;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al agregar:" + w);
            }
        }



        private void Tx_codigo_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "select * from Comae_dif where cod_dif='" + (sender as TextBox).Text + "' ";
                DataTable dt = SiaWin.Func.SqlDT(query, "tabla", idemp);
                if (dt.Rows.Count > 0)
                {
                    MessageBox.Show("el codigo ingresado ya existe");
                    Tx_nombre.Text = "";
                    Tx_nombre.IsEnabled = false;
                    Tx_cta_dif.Text = "";
                    Tx_cta_dif.IsEnabled = false;
                    Tx_cta_amor.Text = "";
                    Tx_cta_amor.IsEnabled = false;
                    Tx_valor.Value = 0;
                    Tx_valor.IsEnabled = false;
                    Tx_tercero.Text = "";
                    Tx_tercero.IsEnabled = false;
                    Tx_observ.Text = "";
                    Tx_observ.IsEnabled = false;
                }
                else desblo();
            }
            catch (Exception w)
            {
                MessageBox.Show("error al buscar el campo al agregar nuevo diferido:" + w);
            }
        }

        public void desblo()
        {
            Tx_codigo.IsEnabled = true;
            Tx_nombre.IsEnabled = true;
            Tx_cta_dif.IsEnabled = true;
            Tx_cta_amor.IsEnabled = true;
            Tx_valor.IsEnabled = true;
            Tx_tercero.IsEnabled = true;
            Tx_observ.IsEnabled = true;
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                int idr = 0; string code = ""; string nombre = "";
                dynamic xx = SiaWin.WindowBuscar("Comae_dif", "cod_dif", "nom_dif", "nom_dif", "idrow", "Maestra de Diferidos", cnEmp, true, "", idEmp: idemp);
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
                    Tx_codigo.Text = code;
                    Tx_nombre.Text = nombre;
                    searchCabeza(code);
                    searchCuerpo(code);
                    search = true;
                    BtnEdit.IsEnabled = true;

                    if (!string.IsNullOrWhiteSpace(code))
                    {
                        Tx_codigo.IsEnabled = false;
                        Tx_nombre.IsEnabled = false;
                        Tx_cta_dif.IsEnabled = false;
                        Tx_cta_amor.IsEnabled = false;
                        Tx_valor.IsEnabled = false;
                        Tx_tercero.IsEnabled = false;                        
                        Tx_observ.IsEnabled = false;
                    }

                }
                if (string.IsNullOrEmpty(code)) e.Handled = false;
                if (string.IsNullOrEmpty(code)) return;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al buscar:" + w);
            }
        }

        public void searchCabeza(string code)
        {
            DataTable dt = SiaWin.Func.SqlDT("select * from Comae_dif where cod_dif='" + code + "' ", "table", idemp);
            if (dt.Rows.Count > 0)
            {
                Tx_cta_dif.Text = dt.Rows[0]["cta_dif"].ToString().Trim();
                Tx_cta_amor.Text = dt.Rows[0]["cta_amo"].ToString().Trim();
                Tx_valor.Value = Convert.ToDecimal(dt.Rows[0]["valor"]);
                Tx_tercero.Text = dt.Rows[0]["cod_ter"].ToString().Trim();
                Tx_observ.Text = dt.Rows[0]["observ"].ToString().Trim();
            }
        }


        public void searchCuerpo(string code)
        {

            string query = " select idrow, cod_dif, cod_cco, valor, cuotas, estado, cos_his, poliza, ";
            query += "CONVERT(varchar,fec_ini,103) as fec_ini, ";
            query += "CONVERT(varchar,fec_fin,103) as fec_fin, ";
            query += "CONVERT(varchar,fec_adq,103) as fec_adq  ";
            query += "from Corel_dif  ";
            query += "where cod_dif ='" + code + "'  ";

            DataTable dt = SiaWin.Func.SqlDT(query, "table", idemp);
            if (dt.Rows.Count > 0)
            {
                dataGridCue.ItemsSource = dt.DefaultView;
                Tx_registros.Text = dt.Rows.Count.ToString();
            }
        }

        private void Tx_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BtnEdit.IsEnabled == true || !string.IsNullOrWhiteSpace(Tx_codigo.Text))
                {
                    string tag = (sender as TextBox).Tag.ToString();
                    string update = "update Comae_dif set " + tag + "='" + (sender as TextBox).Text + "' where cod_dif='" + Tx_codigo.Text + "' ";                    
                    if (SiaWin.Func.SqlCRUD(update, idemp) == false) MessageBox.Show("errro al actualizar");                 
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al actualizar la cabeza:" + w);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (search == true && !string.IsNullOrEmpty(Tx_codigo.Text))
            {
                Tx_codigo.IsEnabled = true;
                Tx_nombre.IsEnabled = true;
                Tx_cta_dif.IsEnabled = true;
                Tx_cta_amor.IsEnabled = true;
                Tx_valor.IsEnabled = true;
                Tx_tercero.IsEnabled = true;
                Tx_observ.IsEnabled = true;

                dataGridCue.AllowEditing = true;
                dataGridCue.IsEnabled = true;
            }
        }



        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Tx_codigo.Text))
                {
                    if (MessageBox.Show("Usted desea eliminar el diferido " + Tx_codigo.Text.Trim() + " ", "Confirmacion", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        string delete_cab = "delete Comae_dif where cod_dif='" + Tx_codigo.Text.Trim() + "' ";
                        if (SiaWin.Func.SqlCRUD(delete_cab, idemp) == true)
                        {
                            string delete_cue = "delete Corel_dif  where cod_dif='" + Tx_codigo.Text.Trim() + "' ";
                            if (SiaWin.Func.SqlCRUD(delete_cue, idemp) == true)
                            {
                                MessageBox.Show("se elimino exitosamente el codigo de diferido:" + Tx_codigo.Text.Trim() + "", "alerta", MessageBoxButton.OK, MessageBoxImage.Information);
                                clearValues();
                                BtnEdit.IsEnabled = true;
                            }
                        }

                    }
                }
                else
                {
                    MessageBox.Show("debe de buscar el codigo de diferido que desea eliminar", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

            }
            catch (Exception)
            {
                MessageBox.Show("error al eliminar tablas");
            }
        }


        public void clearValues()
        {
            Tx_codigo.Text = "";
            Tx_codigo.IsEnabled = false;
            Tx_nombre.Text = "";
            Tx_nombre.IsEnabled = false;
            Tx_cta_dif.Text = "";
            Tx_cta_dif.IsEnabled = false;
            Tx_cta_amor.Text = "";
            Tx_cta_amor.IsEnabled = false;
            Tx_valor.Value = 0;
            Tx_valor.IsEnabled = false;
            Tx_tercero.Text = "";
            Tx_tercero.IsEnabled = false;
            Tx_observ.Text = "";
            Tx_observ.IsEnabled = false;
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (validarCampos() == false)
                {
                    MessageBox.Show("llene todos los campos para poder guardar", "alert", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }

                string query = "insert into Comae_dif (cod_dif,nom_dif,valor,cta_dif,cta_amo,cod_ter,observ) values ('" + Tx_codigo.Text + "','" + Tx_nombre.Text + "'," + Tx_valor.Value + ",'" + Tx_cta_dif.Text + "','" + Tx_cta_amor.Text + "','" + Tx_tercero.Text + "','" + Tx_observ.Text + "');  ";
                if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                {
                    MessageBox.Show("eliminacion exitosa");
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al guardar:" + w);
            }
        }


        public bool validarCampos()
        {
            bool flag = true;
            if (string.IsNullOrEmpty(Tx_codigo.Text)) flag = false;
            if (string.IsNullOrEmpty(Tx_nombre.Text)) flag = false;
            if (string.IsNullOrEmpty(Tx_cta_dif.Text)) flag = false;
            if (string.IsNullOrEmpty(Tx_cta_amor.Text)) flag = false;
            if (string.IsNullOrEmpty(Tx_tercero.Text)) flag = false;
            if (string.IsNullOrEmpty(Tx_observ.Text)) flag = false;
            return flag;
        }



        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Panel2.Visibility = Visibility.Hidden;
            Panel1.Visibility = Visibility.Visible;

            clearValues();
        }

        private void DataGridCue_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
        {
            try
            {
                bool newcol = this.dataGridCue.View.IsAddingNew;

                if (newcol == false)
                {
                    var reflector = this.dataGridCue.View.GetPropertyAccessProvider();
                    var rowData = dataGridCue.GetRecordAtRowIndex(e.RowColumnIndex.RowIndex);

                    GridColumn Colum = ((SfDataGrid)sender).CurrentColumn as GridColumn;

                    if (Colum.MappingName == "fec_adq" || Colum.MappingName == "fec_ini" || Colum.MappingName == "fec_fin")
                    {
                        DateTime fs; string format = "dd/MM/yyyy";

                        if (DBNull.Value.Equals(reflector.GetValue(rowData, Colum.MappingName))) return;
                        string fecha = reflector.GetValue(rowData, Colum.MappingName).ToString();

                        if (DateTime.TryParseExact(fecha, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out fs) == false)
                        {
                            MessageBox.Show("lo que introdujo en columna '" + Colum.HeaderText + "' no es una fecha por favor verifique el formato de la fecha es dd/mm/yyyy ", "alert", MessageBoxButton.OK, MessageBoxImage.Stop);
                            reflector.SetValue(rowData, Colum.MappingName, getdatetable(reflector.GetValue(rowData, "idrow").ToString(), Colum.MappingName));
                        }
                        else
                        {
                            string query = "update Corel_dif  set " + Colum.MappingName + "='" + fecha + "' where idrow='" + reflector.GetValue(rowData, "idrow").ToString() + "' ";
                            if (SiaWin.Func.SqlCRUD(query, idemp) == false)
                            {
                                MessageBox.Show("error al actualizar contacte con el administrador", "alert", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }

                    if (Colum.MappingName == "cos_his" || Colum.MappingName == "cuotas")
                    {
                        decimal valor = Convert.ToDecimal(reflector.GetValue(rowData, Colum.MappingName));
                        string query = "update Corel_dif  set " + Colum.MappingName + "=" + valor + " where idrow='" + reflector.GetValue(rowData, "idrow").ToString() + "' ";
                        if (SiaWin.Func.SqlCRUD(query, idemp) == false)
                        {
                            MessageBox.Show("error al actualizar contacte con el administrador", "alert", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    if (Colum.MappingName == "poliza")
                    {
                        string valor = reflector.GetValue(rowData, Colum.MappingName).ToString();
                        string query = "update Corel_dif  set " + Colum.MappingName + "='" + valor + "' where idrow='" + reflector.GetValue(rowData, "idrow").ToString() + "' ";
                        if (SiaWin.Func.SqlCRUD(query, idemp) == false)
                        {
                            MessageBox.Show("error al actualizar contacte con el administrador", "alert", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }



                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al editar:" + w);
            }
        }

        public string getdatetable(string id, string campo)
        {
            DataTable dt = SiaWin.Func.SqlDT("select convert(varchar," + campo + ",103) as " + campo + " From Corel_dif  where idrow='" + id + "' ", "tabla", idemp);
            return dt.Rows.Count > 0 ? dt.Rows[0][campo].ToString() : DateTime.Now.ToString("dd/MM/yyyy");
        }

        private void GridConfig_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {

                if (e.Key == Key.F8)
                {
                    GridColumn Colum = ((SfDataGrid)sender).CurrentColumn as GridColumn;

                    var reflector = this.dataGridCue.View.GetPropertyAccessProvider();
                    int columnIndex = (sender as SfDataGrid).SelectionController.CurrentCellManager.CurrentRowColumnIndex.RowIndex;
                    var rowData = dataGridCue.GetRecordAtRowIndex(columnIndex);

                    string tabla = ""; string codigo = ""; string nombre = ""; string title = ""; string where = "";

                    if (Colum.MappingName == "cod_cco")
                    {
                        tabla = "comae_cco"; codigo = "cod_cco"; nombre = "nom_cco"; title = "Maestra de Centro de costos";
                    }
                    if (Colum.MappingName == "cod_cco")
                    {
                        int idr = 0; string codi = ""; string nom = "";
                        dynamic xx = SiaWin.WindowBuscar(tabla, codigo, nombre, codigo, "idrow", title, SiaWin.Func.DatosEmp(idemp), true, where, idEmp: idemp);
                        xx.ShowInTaskbar = false;
                        xx.Owner = Application.Current.MainWindow;
                        xx.Height = 500;
                        xx.ShowDialog();
                        idr = xx.IdRowReturn;
                        codi = xx.Codigo;
                        nom = xx.Nombre;

                        reflector.SetValue(rowData, Colum.MappingName, codi);
                        dataGridCue.UpdateDataRow(columnIndex);
                        dataGridCue.UpdateLayout();
                        dataGridCue.Columns[Colum.MappingName].AllowEditing = true;

                        if (!string.IsNullOrWhiteSpace(codi))
                        {
                            string query = "update Corel_dif  set " + Colum.MappingName + "='" + codi + "' where idrow='" + reflector.GetValue(rowData, "idrow").ToString() + "' ";
                            if (SiaWin.Func.SqlCRUD(query, idemp) == false)
                            {
                                MessageBox.Show("error al actualizar contacte con el administrador", "alert", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }

                    }
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error en GridConfig_PreviewKeyDown:" + w);
            }
        }






    }
}



