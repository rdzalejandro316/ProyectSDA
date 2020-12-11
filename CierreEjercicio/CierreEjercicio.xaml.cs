using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

    //Sia.PublicarPnt(9673,"CierreEjercicio");
    //Sia.TabU(9673);

    public partial class CierreEjercicio : UserControl
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        dynamic tabitem;
        public int idmodulo = 1;
        public CierreEjercicio(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            tabitem = tabitem1;
            LoadConfig();
        }


        private void LoadConfig()
        {
            try
            {
                SiaWin = Application.Current.MainWindow;
                if (idemp <= 0) idemp = SiaWin._BusinessId;
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                tabitem.Title = "Cierre de Ejercicio - " + nomempresa;
                tabitem.Logo(idLogo, ".png");

                TxFecDoc.Text = DateTime.Now.ToString();

                DataTable dt = SiaWin.Func.SqlDT("select periodo,periodonombre from Periodos where TipoPeriodo='1' ", "tabla", 0);
                CBperiodos.ItemsSource = dt.DefaultView;

                DataTable dt_trn = SiaWin.Func.SqlDT("select rtrim(cod_trn) as cod_trn,rtrim(cod_trn)+'-'+rtrim(nom_trn) as nom_trn from comae_trn order by cod_trn", "tabla", idemp);
                CBtipotrn.ItemsSource = dt_trn.DefaultView;


                #region valores por defecto

                CBperiodos.SelectedValue = "13";
                CBtipotrn.SelectedValue = "98";

                DateTime tiempo = Convert.ToDateTime(Tx_ano.Value.ToString());
                string año = tiempo.ToString("yyyy");
                TxDocumento.Text = "CIE-" + año;
                TxFecDoc.Text = "31/12/" + año;
                #endregion


            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load:" + e);
            }
        }

        private void Tx_ano_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            try
            {
                DateTime tiempo = Convert.ToDateTime(Tx_ano.Value.ToString());
                string año = tiempo.ToString("yyyy");
                TxDocumento.Text = "CIE-" + año;
                TxFecDoc.Text = "31/12/" + año;
            }
            catch (Exception)
            {
            }

        }

        private async void BtnConsultar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                GridConfiguracion.IsEnabled = false;
                sfBusyIndicator.IsBusy = true;

                dataGridConsulta.ItemsSource = null;
                BtnConsultar.IsEnabled = false;
                source.CancelAfter(TimeSpan.FromSeconds(1));

                DateTime tiempo = Convert.ToDateTime(Tx_ano.Value.ToString());
                string empresa = "010";
                string año = tiempo.ToString("yyyy");
                int isExecute = CbCierre.SelectedIndex;


                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(año, isExecute, empresa), source.Token);
                await slowTask;

                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    if (isExecute == 1)
                    {
                        SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idemp, 0, 0, 0, "PASARON LOS SALDOS DE CONTABILIDAD:" + tiempo.ToString("yyyy"), "");
                    }

                    double debito = 0;
                    double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(deb_mov)", "").ToString(), out debito);

                    double credito = 0;
                    double.TryParse(((DataSet)slowTask.Result).Tables[0].Compute("Sum(cre_mov)", "").ToString(), out credito);

                    dataGridConsulta.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                    Total.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();

                    TxDebito.Text = debito.ToString("N");
                    TxCredito.Text = credito.ToString("N");

                    TabControl1.SelectedIndex = 2;
                    TabControl1.SelectedIndex = 1;
                }

                BtnConsultar.IsEnabled = true;
                this.sfBusyIndicator.IsBusy = false;
                GridConfiguracion.IsEnabled = true;
            }
            catch (SqlException w)
            {
                MessageBox.Show("error1-" + w);
            }
            catch (Exception ex)
            {
                MessageBox.Show("erro2-" + ex.Message);
                this.Opacity = 1;
            }
        }


        private DataSet LoadData(string ano, int isexecute, string empresa)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpSpCierreAnual", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@anno", ano);
                cmd.Parameters.AddWithValue("@isExecute", isexecute);
                cmd.Parameters.AddWithValue("@codEmpresa", empresa);
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                con.Close();
                return ds;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }


        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            tabitem.Cerrar(0);
        }

        private void Exportar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;
                var excelEngine = dataGridConsulta.ExportToExcel(dataGridConsulta.View, options);
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
            catch (Exception w)
            {
                MessageBox.Show("error al exportar:" + w);
            }
        }

        private void BtnViewDoc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string numtrn = TxDocumento.Text.Trim();
                string codtrn = CBtipotrn.SelectedValue.ToString();

                string query = "select * From cocab_doc where cod_trn='" + codtrn + "' and num_trn='" + numtrn + "' ";
                DataTable dt = SiaWin.Func.SqlDT(query, "cabeza", idemp);
                if (dt.Rows.Count > 0)
                {
                    int idreg = Convert.ToInt32(dt.Rows[0]["idreg"]);
                    SiaWin.TabTrn(0, idemp, true, idreg, idmodulo, WinModal: true);
                }
                else
                {
                    MessageBox.Show("el documento " + numtrn + " no se encuentra", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al ver el documento:" + w);
            }
        }



    }
}
