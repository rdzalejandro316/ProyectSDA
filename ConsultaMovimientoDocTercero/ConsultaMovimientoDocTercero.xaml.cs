using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid.Converter;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9633,"ConsultaMovimientoDocTercero");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9633, "ConsultaMovimientoDocTercero");
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //ww.ShowDialog();

    public partial class ConsultaMovimientoDocTercero : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        int modulo = 1;

        public ConsultaMovimientoDocTercero()
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
                this.Title = "Consulta Movimiento Documento Tercero";
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
                winb.Height = 300;
                winb.ShowDialog();
                idr = winb.IdRowReturn;
                code = winb.Codigo;
                nom = winb.Nombre;
                winb = null;
                if (idr > 0)
                {
                    switch ((sender as TextBox).Name)
                    {
                        case "Tx_tercero":
                            Tx_tercero.Text = code.Trim();
                            TxNameTer.Text = nom.Trim();
                            break;
                        case "Tx_cuenta":
                            Tx_cuenta.Text = code.Trim();
                            TxNameCta.Text = nom.Trim();
                            break;
                    }

                    var uiElement = e.OriginalSource as UIElement;
                    uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                }
                e.Handled = true;
            }
            if (e.Key == Key.Enter)
            {
                var uiElement = e.OriginalSource as UIElement;
                uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {


            if ((sender as TextBox).Name == "Tx_cuenta")
            {
                if (string.IsNullOrEmpty((sender as TextBox).Text))
                    return;
            }



            string tabla = ""; string campo = "";

            switch ((sender as TextBox).Name)
            {
                case "Tx_tercero": tabla = "comae_ter"; campo = "cod_ter"; TxNameTer.Text = ""; break;
                case "Tx_cuenta": tabla = "comae_cta"; campo = "cod_cta"; TxNameCta.Text = ""; break;
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
                winb.Height = 300;
                winb.ShowDialog();
                idr = winb.IdRowReturn;
                code = winb.Codigo;
                nom = winb.Nombre;
                winb = null;
                if (idr > 0)
                {
                    switch ((sender as TextBox).Name)
                    {
                        case "Tx_tercero":
                            Tx_tercero.Text = code.Trim();
                            TxNameTer.Text = nom.Trim();
                            break;
                        case "Tx_cuenta":
                            Tx_cuenta.Text = code.Trim();
                            TxNameCta.Text = nom.Trim();
                            break;
                    }
                    var uiElement = e.OriginalSource as UIElement;
                    uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                }
                else
                {

                    switch ((sender as TextBox).Name)
                    {
                        case "Tx_tercero":
                            Tx_tercero.Text = "";
                            TxNameTer.Text = "";
                            break;
                        case "Tx_cuenta":
                            Tx_cuenta.Text = "";
                            TxNameCta.Text = "";
                            break;
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


        private async void BtnConsultar_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(Tx_tercero.Text))
            {
                MessageBox.Show("el campo tercero debe de estar lleno", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }


            CancellationTokenSource source = new CancellationTokenSource();

            CancellationToken token = source.Token;

            sfBusyIndicator.IsBusy = true;
            FilterConsulta.IsEnabled = false;

            string doc_mov = Tx_Doc.Text;
            string tercero = Tx_tercero.Text;
            string cuenta = Tx_cuenta.Text;

            var slowTask = Task<DataTable>.Factory.StartNew(() => LoadData(tercero, doc_mov, cuenta));
            await slowTask;

            if (((DataTable)slowTask.Result).Rows.Count > 0)
            {
                GridConsulta.ItemsSource = ((DataTable)slowTask.Result).DefaultView;
                TX_total.Text = ((DataTable)slowTask.Result).Rows.Count.ToString();


                double deb = Convert.ToDouble(((DataTable)slowTask.Result).Compute("Sum(deb_mov)", ""));
                double cre = Convert.ToDouble(((DataTable)slowTask.Result).Compute("Sum(cre_mov)", ""));

                Tx_deb.Text = deb.ToString("N", CultureInfo.CreateSpecificCulture("es-ES"));
                Tx_cre.Text = cre.ToString("N", CultureInfo.CreateSpecificCulture("es-ES"));

            }
            else
            {
                GridConsulta.ItemsSource = null;
                TX_total.Text = "0";
                Tx_deb.Text = "-";
                Tx_cre.Text = "-";

            }

            sfBusyIndicator.IsBusy = false;
            FilterConsulta.IsEnabled = true;
        }


        private DataTable LoadData(string codter, string docmov, string codcta)
        {
            try
            {
                string where = "";
                if (!string.IsNullOrEmpty(codcta)) where += " and cue.cod_cta='" + codcta + "'  ";
                if (!string.IsNullOrEmpty(docmov)) where += " and cue.doc_mov='" + docmov + "'  ";


                string query = "select cue.idregcab, cue.idreg,cue.cod_trn,cue.num_trn,cab.fec_trn,cue.cod_cta,cue.cod_ter,cue.des_mov,cue.doc_ref,cue.doc_cruc,cue.deb_mov,cue.cre_mov, ";
                query += "fec_venc,doc_mov ";
                query += "from Cocue_doc cue ";
                query += "inner join cocab_doc cab on cab.idreg = cue.idregcab ";
                query += "WHERE cue.cod_ter='" + codter + "'  " + where + " ;";

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
                var excelEngine = GridConsulta.ExportToExcel(GridConsulta.View, options);
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

        private void BtnView_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (GridConsulta.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)GridConsulta.SelectedItems[0];
                    int idreg = Convert.ToInt32(row["idreg"]);
                    SiaWin.TabTrn(0, idemp, true, idreg, modulo, WinModal: true);
                }
                else
                {
                    MessageBox.Show("seleccione un documento de la grilla","alerta",MessageBoxButton.OK,MessageBoxImage.Exclamation);
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrri el documento:" + w);
            }
        }





    }
}
