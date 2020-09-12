using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{

    //Sia.PublicarPnt(9674, "AuxiliarTerceroCuenta");    
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9674, "AuxiliarTerceroCuenta");
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //ww.ShowDialog();

    public partial class AuxiliarTerceroCuenta : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public AuxiliarTerceroCuenta()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                SiaWin = Application.Current.MainWindow;
                if (idemp <= 0) idemp = SiaWin._BusinessId;

                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Auxliar Tercero Cuenta " + cod_empresa + "-" + nomempresa;

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
            if (e.Key == Key.F8 || e.Key == Key.Enter)
            {
                int idr = 0; string code = ""; string nom = "";


                string cmptabla = ""; string cmpcodigo = ""; string cmpnombre = ""; string cmporden = ""; string cmpidrow = ""; string cmptitulo = "";
                switch ((sender as TextBox).Name)
                {
                    case "Tx_tercero":
                        cmptabla = "comae_ter"; cmpcodigo = "cod_ter"; cmpnombre = "nom_ter"; cmporden = "cod_ter"; cmpidrow = "idrow"; cmptitulo = "Maestra de terceros";
                        break;
                    case "Tx_cuenta":
                        cmptabla = "comae_cta"; cmpcodigo = "cod_cta"; cmpnombre = "nom_cta"; cmporden = "cod_cta"; cmpidrow = "idrow"; cmptitulo = "Maestra de cuentas";
                        break;
                }


                dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, SiaWin.Func.DatosEmp(idemp), false, "", idEmp: idemp);
                winb.ShowInTaskbar = false;
                winb.Owner = Application.Current.MainWindow;
                winb.ShowDialog();
                idr = winb.IdRowReturn;
                code = winb.Codigo;
                nom = winb.Nombre;
                winb = null;
                if (idr > 0)
                {
                    (sender as TextBox).Text = code.Trim();
                    switch ((sender as TextBox).Name)
                    {
                        case "Tx_tercero": TxNameTer.Text = nom; break;
                        case "Tx_cuenta": TxNameCta.Text = nom; break;
                    }

                    var uiElement = e.OriginalSource as UIElement;
                    uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                }
                else
                {
                    switch ((sender as TextBox).Name)
                    {
                        case "Tx_tercero": TxNameTer.Text = ""; break;
                        case "Tx_cuenta": TxNameCta.Text = ""; break;
                    }
                }
                e.Handled = true;
            }
            if (e.Key == Key.Enter)
            {
                var uiElement = e.OriginalSource as UIElement;
                uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
            }
        }

        private void Tx_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {

                if ((sender as TextBox).Name == "Tx_tercero")
                {
                    if (string.IsNullOrEmpty((sender as TextBox).Text))
                        return;
                }



                string tabla = ""; string campo = "";

                switch ((sender as TextBox).Name)
                {
                    case "Tx_tercero": tabla = "comae_ter"; campo = "cod_ter"; break;
                    case "Tx_cuenta": tabla = "comae_cta"; campo = "cod_cta"; break;
                }

                System.Data.DataTable dt = SiaWin.Func.SqlDT("select * from " + tabla + "  where " + campo + "='" + (sender as TextBox).Text + "' ", "tabla", idemp);
                if (dt.Rows.Count <= 0)
                {

                    MessageBox.Show((sender as TextBox).Name == "Tx_tercero" ? "el tercero no existe seleccione uno de la lista" : "la cuenta ingresada no existe seleccione una cuanta de la lista");


                    int idr = 0; string code = ""; string nom = "";
                    string cmptabla = ""; string cmpcodigo = ""; string cmpnombre = ""; string cmporden = ""; string cmpidrow = ""; string cmptitulo = "";
                    switch ((sender as TextBox).Name)
                    {
                        case "Tx_tercero":
                            cmptabla = "comae_ter"; cmpcodigo = "cod_ter"; cmpnombre = "nom_ter"; cmporden = "cod_ter"; cmpidrow = "idrow"; cmptitulo = "Maestra de terceros";
                            break;
                        case "Tx_cuenta":
                            cmptabla = "comae_cta"; cmpcodigo = "cod_cta"; cmpnombre = "nom_cta"; cmporden = "cod_cta"; cmpidrow = "idrow"; cmptitulo = "Maestra de cuentas";
                            break;
                    }

                    dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, SiaWin.Func.DatosEmp(idemp), false, "", idEmp: idemp);
                    winb.ShowInTaskbar = false;
                    winb.Owner = Application.Current.MainWindow;
                    winb.ShowDialog();
                    idr = winb.IdRowReturn;
                    code = winb.Codigo;
                    nom = winb.Nombre;
                    winb = null;
                    if (idr > 0)
                    {
                        (sender as TextBox).Text = code.Trim();
                        switch ((sender as TextBox).Name)
                        {
                            case "Tx_tercero": TxNameTer.Text = nom; break;
                            case "Tx_cuenta": TxNameCta.Text = nom; break;
                        }

                        var uiElement = e.OriginalSource as UIElement;
                        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    }
                    else
                    {
                        (sender as TextBox).Text = "";
                        switch ((sender as TextBox).Name)
                        {
                            case "Tx_tercero": TxNameTer.Text = ""; break;
                            case "Tx_cuenta": TxNameCta.Text = ""; break;
                        }
                    }
                }
                else
                {
                    switch ((sender as TextBox).Name)
                    {
                        case "Tx_tercero": TxNameTer.Text = dt.Rows[0]["nom_ter"].ToString().Trim(); break;
                        case "Tx_cuenta": TxNameCta.Text = dt.Rows[0]["nom_cta"].ToString().Trim(); break;
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al salir del elemento:" + w);
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(Tx_cuenta.Text))
                {
                    MessageBox.Show("el campo cuenta debe de estar lleno", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (string.IsNullOrEmpty(Tx_tercero.Text))
                {
                    MessageBox.Show("el campo tercero debe de estar lleno", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                string name = (sender as Button).Name;

                CancellationTokenSource source = new CancellationTokenSource();
                sfBusyIndicator.IsBusy = true;
                dataGridRefe.ItemsSource = null;
                PanelBlock.IsEnabled = false;



                //string tercero = name == "BtnTercero" ? Tx_tercero.Text : "";
                //string cuenta = Tx_cuenta.Text;

                string tercero = "";
                string cuenta = "";
                switch (name)
                {
                    case "BtnCuenta":
                        tercero = "";
                        cuenta = Tx_cuenta.Text.Trim();
                        break;
                    case "BtnTercero":
                        tercero = Tx_tercero.Text.Trim();
                        cuenta = "";
                        break;                    
                    case "BtnTerCta":
                        tercero = Tx_tercero.Text.Trim();
                        cuenta = Tx_cuenta.Text.Trim();
                        break;
                }


                string fecini = TxFecIni.Text;
                string fecfin = TxFecFin.Text;


                var slowTask = Task<DataTable>.Factory.StartNew(() => LoadData(tercero, cuenta, fecini, fecfin), source.Token);
                await slowTask;

                if (((DataTable)slowTask.Result).Rows.Count > 0)
                {
                    dataGridRefe.ItemsSource = ((DataTable)slowTask.Result).DefaultView;
                    Tx_total.Text = ((DataTable)slowTask.Result).Rows.Count.ToString();

                    double deb = Convert.ToDouble(((DataTable)slowTask.Result).Compute("Sum(deb_mov)", ""));
                    double cre = Convert.ToDouble(((DataTable)slowTask.Result).Compute("Sum(cre_mov)", ""));

                    TxTot_deb.Text = deb.ToString("N", CultureInfo.CreateSpecificCulture("es-ES"));
                    TxTot_cre.Text = cre.ToString("N", CultureInfo.CreateSpecificCulture("es-ES"));
                }
                else
                {
                    dataGridRefe.ItemsSource = null;
                    Tx_total.Text = "0";
                    TxTot_deb.Text = "-";
                    TxTot_cre.Text = "-";
                }

                PanelBlock.IsEnabled = true;
                sfBusyIndicator.IsBusy = false;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al consutlar:" + w);
            }
        }

        private DataTable LoadData(string cod_ter, string cod_cta, string fec_ini, string fec_fin)
        {
            try
            {
                string where = "";
                if (string.IsNullOrEmpty(cod_ter) && !string.IsNullOrEmpty(cod_cta))
                {
                    where += " cue.cod_cta='" + cod_cta + "' ";
                }

                if (!string.IsNullOrEmpty(cod_ter) && string.IsNullOrEmpty(cod_cta))
                {
                    where += " cue.cod_ter='" + cod_ter+ "' ";
                }

                if (!string.IsNullOrEmpty(cod_ter) && !string.IsNullOrEmpty(cod_cta))
                {
                    where += " cue.cod_ter='" + cod_ter + "' and  cue.cod_cta='" + cod_cta + "' ";
                }



                string query = "select cab.idreg,cue.per_doc,cue.ano_doc,cue.cod_ter,cue.cod_cta,cta.nom_cta,cue.cod_ciu,ciu.nom_ciu,cue.cod_trn,trn.nom_trn,cue.num_trn,cab.fec_trn,cue.des_mov,cue.bas_mov,cue.deb_mov,cue.cre_mov ";
                query += "from Cocue_doc cue ";
                query += "inner join cocab_doc cab on cab.idreg = cue.idregcab ";
                query += "inner join comae_trn trn on trn.cod_trn = cab.cod_trn ";
                query += "inner join comae_cta cta on cta.cod_cta = cue.cod_cta ";
                query += "left join Comae_ciu ciu on ciu.cod_ciu = cue.cod_ciu ";
                query += "WHERE  " + where + " ";
                query += "AND convert(date,cab.fec_trn,105) between '" + fec_ini + "' and '" + fec_fin + "'; ";

                               
                System.Data.DataTable dt = SiaWin.Func.SqlDT(query, "tabla", idemp);
                return dt;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }


        private void BtnExportar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;
                var excelEngine = dataGridRefe.ExportToExcel(dataGridRefe.View, options);
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
                        MessageBox.Show(sfd.FilterIndex.ToString());
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dataGridRefe_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            try
            {
                if ((sender as SfDataGrid).SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)dataGridRefe.SelectedItems[0];
                    string nom_trn = row["nom_trn"].ToString().Trim();
                    string nom_cta = row["nom_cta"].ToString().Trim();
                    string nom_ciu = row["nom_ciu"].ToString().Trim();

                    Tx_trn.Text = string.IsNullOrEmpty(nom_trn) ? "---" : nom_trn;
                    Tx_cuen.Text = string.IsNullOrEmpty(nom_cta) ? "---" : nom_cta;
                    Tx_Ciudad.Text = string.IsNullOrEmpty(nom_ciu) ? "---" : nom_ciu;
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar:" + w);
            }
        }

        private void BtnDoc_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (dataGridRefe.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)dataGridRefe.SelectedItems[0];
                    int idreg = Convert.ToInt32(row["idreg"]);
                    int moduloid = 1;
                    SiaWin.TabTrn(0, idemp, true, idreg, moduloid, WinModal: true);
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir documento:" + w);
            }
        }


    }
}
