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
    //Sia.PublicarPnt(9694,"CruceHistoricoCartera");
    //Sia.TabU(9694);

    public partial class CruceHistoricoCartera : UserControl
    {
        dynamic SiaWin;
        dynamic tabitem;
        public int idemp = 0;
        string cnEmp = "";
        string codemp = string.Empty;
        int idmodulo = 1;
        DataTable dt_consulta = new DataTable();
        public CruceHistoricoCartera(dynamic tabitem1)
        {
            InitializeComponent();
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            tabitem = tabitem1;
            tabitem.Title = "Cruce Historico";
            tabitem.Logo(9, ".png");
            tabitem.MultiTab = false;
            if (tabitem.idemp > 0) idemp = tabitem.idemp;
            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                codemp = foundRow["BusinessCode"].ToString().Trim();
                tabitem.Logo(idLogo, ".png");
                tabitem.Title = "cruce historico de cartera (" + aliasemp + ")";

                FechaIni.Text = DateTime.Now.ToShortDateString();
                FechaFin.Text = DateTime.Now.ToShortDateString();
            }
            catch (Exception e)
            {
                SiaWin.seguridad.ErrorLog("Error  ", "AnalisisDeCartera-LoadConfig:" + e.Message.ToString());
                MessageBox.Show(e.Message);
            }
        }

        private async void BtnConsultar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                #region validaciones
                if (string.IsNullOrEmpty(FechaIni.Text))
                {
                    MessageBox.Show("el campo fecha inicial debe de estar lleno", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (string.IsNullOrEmpty(FechaFin.Text))
                {
                    MessageBox.Show("el campo fecha final debe de estar lleno", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                #endregion

                PanelA.IsEnabled = false;
                sfBusyIndicator.IsBusy = true;

                string fec_ini = FechaIni.Text.Trim();
                string fec_fin = FechaFin.Text.Trim();


                var slowTask = Task<DataTable>.Factory.StartNew(() => LoadData(fec_ini, fec_fin));
                await slowTask;

                if (slowTask.IsCompleted)
                {
                    if (slowTask.Result.Rows.Count > 0)
                    {
                        dt_consulta = slowTask.Result;
                        dataGridCxC.ItemsSource = dt_consulta.DefaultView;
                        TxRegistros.Text = dt_consulta.Rows.Count.ToString();
                    }
                    else
                    {
                        dataGridCxC.ItemsSource = null;
                        TxRegistros.Text = "0";
                    }
                }

                PanelA.IsEnabled = true;
                sfBusyIndicator.IsBusy = false;

            }
            catch (Exception w)
            {
                MessageBox.Show("erro al consutar:" + w);
            }
        }

        private DataTable LoadData(string fecini, string fecfin)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataTable dt = new DataTable();
                cmd = new SqlCommand("_EmpCruceHistoricoCartera", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fec_ini", fecini);
                cmd.Parameters.AddWithValue("@fec_fin", fecfin);
                cmd.Parameters.AddWithValue("@codemp", "010");
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(dt);
                con.Close();

                return dt;
            }
            catch (Exception e)
            {
                SiaWin.seguridad.ErrorLog("Error  ", "AnalisisDeCartera-LoadData:" + e.Message.ToString());
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
                var excelEngine = dataGridCxC.ExportToExcel(dataGridCxC.View, options);
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


        private async void BtnEjecutar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region validaciones
                if (dt_consulta.Rows.Count <= 0 || dataGridCxC.View.Records.Count <= 0)
                {
                    MessageBox.Show("no existen registros para actualizar su documento cruce", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                #endregion


                var slowTask = Task<DataTable>.Factory.StartNew(() => UpdateDocCruc(dt_consulta));
                await slowTask;
                if (slowTask.IsCompleted)
                {
                    if (slowTask.Result.Rows.Count > 0)
                    {
                        MessageBox.Show("los siguientes documentos fueron los que se le actualizo el documento cruce", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        SiaWin.Browse(slowTask.Result);
                    }
                }


            }
            catch (Exception w)
            {
                MessageBox.Show("error al ejecutar:" + w);
            }
        }


        private DataTable UpdateDocCruc(DataTable dt)
        {

            //documentos que fueron remplazdos su doc_cruc
            DataTable dt_doc = new DataTable();
            dt_doc.Columns.Add("transaccion");
            dt_doc.Columns.Add("documento");


            if (dt.Rows.Count > 0)
            {
                string update = "";
                foreach (DataRow item in dt.Rows)
                {
                    string factura = item["factura"].ToString().Trim();
                    string doc_cruc = item["doc_cruc"].ToString().Trim();
                    string cod_ter = item["cod_ter"].ToString().Trim();
                    string cod_cta = item["cod_cta"].ToString().Trim();
                    string _ref = item["ref"].ToString().Trim();
                    decimal cre_mov = Convert.ToDecimal(item["cre_mov"]);

                    if (string.IsNullOrEmpty(doc_cruc))
                    {
                        string query = "select * from cocue_doc";
                        query += "where cod_ter='" + cod_ter + "' and cod_cta='" + cod_cta + "' and doc_mov='" + _ref + "' and cre_mov=" + cre_mov + ";";
                        DataTable cuerpo = SiaWin.Func.SqlDT(query, "Existencia", idemp);
                        if (cuerpo.Rows.Count > 0)
                        {
                            foreach (DataRow dr in cuerpo.Rows)
                            {
                                int idreg = Convert.ToInt32(dr["idreg"]);
                                string cod_trn = dr["cod_trn"].ToString();
                                string num_trn = dr["num_trn"].ToString();

                                update += "update cocue_doc set doc_cruc='" + factura + "',doc_ref='" + factura + "'  where idreg=" + idreg + ";";

                                #region que documentos fueron los que se modificaron su documento cruce
                                DataRow row_transaccion = dt_doc.NewRow(); row_transaccion["transaccion"] = cod_trn;
                                DataRow row_documento = dt_doc.NewRow(); row_transaccion["documento"] = num_trn;
                                dt_doc.Rows.Add(row_transaccion);
                                dt_doc.Rows.Add(row_documento);
                                #endregion
                            }
                        }

                    }


                }

                System.Data.SqlClient.SqlConnection Conec = new System.Data.SqlClient.SqlConnection(cnEmp);
                System.Data.SqlClient.SqlCommand Sqlcmd = new System.Data.SqlClient.SqlCommand();
                Sqlcmd.CommandType = System.Data.CommandType.Text;
                Sqlcmd.CommandText = update; Sqlcmd.Connection = Conec;
                Conec.Open();
                Sqlcmd.ExecuteNonQuery();
                Conec.Close();
            }

            return dt_doc;
        }


    }
}
