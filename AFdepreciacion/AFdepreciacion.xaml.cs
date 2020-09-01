using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

    //   Sia.PublicarPnt(9662,"AFdepreciacion");
    //   Sia.TabU(9662);
    
    public partial class AFdepreciacion : UserControl
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        dynamic tabitem;
        string codtrn = "901";

        public DataTable dt_depreciar = new DataTable();

        public AFdepreciacion(dynamic tabitem1)
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
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                tabitem.Title = "Depreciacion - " + cod_empresa + " - " + nomempresa;
                tabitem.Logo(idLogo, ".png");
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void BtnDepreciar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region validaciones


                int Year = Convert.ToDateTime(Tx_ano.Value).Year;
                int Month = Convert.ToDateTime(Tx_periodo.Value).Month;
                string periodo = Month >= 10 ? Month.ToString() : "0" + Month.ToString();


                if (dt_depreciar.Rows.Count <= 0)
                {
                    MessageBox.Show("Genere la consulta para poder depreciar", "alerta", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DataTable dt = SiaWin.Func.SqlDT("select * from Afcab_doc where ano_doc='" + Year + "' and per_doc='" + periodo + "' and cod_trn='"+ codtrn + "' ", "table", idemp);
                if (dt.Rows.Count > 0)
                {
                    MessageBox.Show("ya se genero la depreciacion para el periodo:" + Month + "-" + Year, "alerta", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                #endregion
                int id = documento();
                if (id > 0)
                {
                    MessageBox.Show("se genero el documento de depreciacion exitosamente", "alerta", MessageBoxButton.OK, MessageBoxImage.Information);
                    dataGrid.ItemsSource = null;
                    Tx_toact.Text = "0";

                    int idmodulo_af = 8;
                    SiaWin.TabTrn(0, idemp, true, id, idmodulo_af, WinModal: true);

                    //int idcontable = ContabilizaDepreciacionFISCAL(id);
                    //SiaWin.TabTrn(0, idemp, true, idcontable, 1, WinModal: true);

                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al depreciar:" + w);
            }
        }


        public int documento()
        {
            int Year = Convert.ToDateTime(Tx_ano.Value).Year;
            int Month = Convert.ToDateTime(Tx_periodo.Value).Month;
            DateTime mes = Convert.ToDateTime(Tx_periodo.Value);

            int idreg = 0;
            if (MessageBox.Show("Usted desea generar la depreciacion del año:" + Year + " periodo:" + Month, "Guardar Traslado", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                
                string fecha = "01" + "/" + Month + "/" + Year;
                DateTime fechaActual = Convert.ToDateTime(fecha);

                string año = Year.ToString();
                string periodo = Month >= 10 ? Month.ToString() : "0" + Month.ToString();

                using (SqlConnection connection = new SqlConnection(cnEmp))
                {

                    connection.Open();
                    StringBuilder errorMessages = new StringBuilder();
                    SqlCommand command = connection.CreateCommand();
                    SqlTransaction transaction;
                    // Start a local transaction.
                    transaction = connection.BeginTransaction("Transaction");
                    command.Connection = connection;
                    command.Transaction = transaction;


                    string sqlConsecutivo = @"declare @fecdoc as datetime;set @fecdoc = getdate();declare @ini as char(4);declare @num as varchar(12);declare @iConsecutivo char(12)=''
                    declare @iFolioHost int = 0;SELECT @iFolioHost =num_act,@ini=rtrim(inicial) FROM Afmae_trn WHERE cod_trn='"+codtrn+"' set @num=@iFolioHost;select @iConsecutivo=rtrim(@ini)+'" + mes.ToString("MM") + Year + "' ";


                    string sqlcab = sqlConsecutivo + @"INSERT INTO Afcab_doc (cod_trn,num_trn,fec_trn,ano_doc,per_doc)
                        values ('" + codtrn + "',@iConsecutivo,'" + fecha + "','" + Year + "','" + periodo + "');DECLARE @NewID INT;SELECT @NewID = SCOPE_IDENTITY();";

                    string sqlcue = "";
                    foreach (DataRow dr in dt_depreciar.Rows)
                    {
                        string cod_act = dr["cod_act"].ToString().Trim();
                        decimal val_depreciar = Math.Round(Convert.ToDecimal(dr["val_dep"]));                        
                        int mesxdep = val_depreciar > 0 ? -1 : 0;                        

                        sqlcue = sqlcue + @"INSERT INTO afcue_doc (idregcab,cod_trn,num_trn,ano_doc,per_doc,cod_act,dep_ac,mesxdep) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + año + "','" + periodo + "','" + cod_act + "'," + val_depreciar.ToString("F", CultureInfo.InvariantCulture) + "," + mesxdep + ");";
                    }

                    string actualzaConsecu = "UPDATE Afmae_trn SET num_act= ISNULL(num_act,0)+1  WHERE  cod_trn='" + codtrn + "';";
                    command.CommandText = sqlcab + sqlcue + actualzaConsecu + @"select CAST(@NewId AS int);";
                    //MessageBox.Show(command.CommandText.ToString());
                    var r = new object();
                    r = command.ExecuteScalar();
                    transaction.Commit();
                    connection.Close();
                    //MessageBox.Show("documento generado");
                    idreg = Convert.ToInt32(r.ToString());
                }

                return idreg;
            }
            else
            {
                MessageBox.Show("no se genero el Documento","alerta",MessageBoxButton.OK,MessageBoxImage.Exclamation);
                return 0;
            }
        }

        private int ContabilizaDepreciacionFISCAL(int idreg)
        {
            int idregreturn = -1;
            try
            {

                #region obtiene datos principales                
                string query = "select Afcab_doc.cod_prv,Afcab_doc.cod_trn,Afcab_doc.num_trn,Afmae_trn.cod_tdo from Afcab_doc  ";
                query += "inner join Afmae_trn on Afmae_trn.cod_trn = Afcab_doc.cod_trn ";
                query += "where idreg ='" + idreg + "' ";

                DataTable dt_trn = SiaWin.Func.SqlDT(query, "cuerpo", idemp);

                string cod_prv = dt_trn.Rows.Count > 0 ? dt_trn.Rows[0]["cod_prv"].ToString().Trim() : "";
                string cod_trn_af = dt_trn.Rows.Count > 0 ? dt_trn.Rows[0]["cod_trn"].ToString().Trim() : "";
                string cod_trn_co = dt_trn.Rows.Count > 0 ? dt_trn.Rows[0]["cod_tdo"].ToString().Trim() : "";
                string num_trn_co = dt_trn.Rows.Count > 0 ? dt_trn.Rows[0]["num_trn"].ToString().Trim() : "";

                #endregion

                #region obtiene cuerpo     

                int Year = Convert.ToDateTime(Tx_ano.Value).Year;
                int Month = Convert.ToDateTime(Tx_periodo.Value).Month;

                string querycue = "select cuerpo.cod_act,activo.sgr_act,cuerpo.dep_ac,subgrupo.cta_dep,subgrupo.cta_gtdep,cuerpo.dep  ";
                querycue += "from Afcue_doc as cuerpo ";
                querycue += "inner join Afmae_act as activo on activo.cod_act = cuerpo.cod_act ";
                querycue += "inner join Afmae_sgr as subgrupo on subgrupo.cod_sgr = activo.sgr_act ";
                querycue += "where cuerpo.idregcab='" + idreg + "' ";

                DataTable dt_cuerpo = SiaWin.Func.SqlDT(querycue, "cuerpo", idemp);

                string sqlcuerpo = "";

                foreach (System.Data.DataRow item in dt_cuerpo.Rows)
                {
                    decimal dep_ac = Convert.ToDecimal(item["dep"]);
                    string cod_act = item["cod_act"].ToString().Trim();

                    string cta_gtdep = item["cta_gtdep"].ToString().Trim();
                    string cta_dep = item["cta_dep"].ToString().Trim();

                    string des_mov = "Depreciacion :" + cod_act + " - Per:" + Month + " - Año:" + Year + "";

                    if (dep_ac > 0)
                    {
                        sqlcuerpo += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_ter,cod_cta,des_mov,deb_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cod_prv + "','" + cta_gtdep + "','" + des_mov + "'," + dep_ac.ToString("F", CultureInfo.InvariantCulture) + "); ";
                        sqlcuerpo += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_ter,cod_cta,des_mov,cre_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cod_prv + "','" + cta_dep + "','" + des_mov + "'," + dep_ac.ToString("F", CultureInfo.InvariantCulture) + "); ";
                    }
                }

                #endregion

                #region generar el documento contable
                using (SqlConnection connection = new SqlConnection(cnEmp))
                {
                    connection.Open();
                    StringBuilder errorMessages = new StringBuilder();
                    SqlCommand command = connection.CreateCommand();
                    SqlTransaction transaction;

                    transaction = connection.BeginTransaction("Transaction");
                    command.Connection = connection;
                    command.Transaction = transaction;

                    string fecha = DateTime.Now.Day + "/" + Month + "/" + Year;
                    DateTime _fecdoc = Convert.ToDateTime(fecha);

                    string sqlConsecutivo = @"declare @fecdoc as datetime;set @fecdoc = '" + _fecdoc.ToString() + "';declare @ini as char(4);DECLARE @NewTrn INT;";
                    //cabeza
                    string sqlcab001co = sqlConsecutivo + @" INSERT INTO cocab_doc (cod_trn,num_trn,fec_trn) values ('" + cod_trn_co + "','" + num_trn_co + "',@fecdoc);SELECT @NewTrn = SCOPE_IDENTITY();";


                    command.CommandText = sqlcab001co + sqlcuerpo + @"select CAST(@NewTrn AS int);";
                    //MessageBox.Show(command.CommandText.ToString());
                    var r = new object();
                    r = command.ExecuteScalar();
                    transaction.Commit();
                    connection.Close();
                    idregreturn = Convert.ToInt32(r.ToString());
                }
                #endregion

                return idregreturn;
            }
            catch (Exception w)
            {
                MessageBox.Show("error en  el documento contable:" + w);
                return -1;
            }
        }


        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            tabitem.Cerrar(0);
        }

        private void BtnExportar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;
                var excelEngine = dataGrid.ExportToExcel(dataGrid.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];
                options.ExportMode = ExportMode.Value;

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
                    //Message box confirmation to view the created workbook.
                    if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archvo", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        //Launching the Excel file using the default Application.[MS Excel Or Free ExcelViewer]
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void BtnConsultar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Tx_ano.IsEnabled = false;
                Tx_periodo.IsEnabled = false;

                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;

                sfBusyIndicator.IsBusy = true;
                dataGrid.ItemsSource = null;
                source.CancelAfter(TimeSpan.FromSeconds(1));

                DateTime now = DateTime.Now;
                int mes = Convert.ToDateTime(Tx_periodo.Value).Month;
                string periodo = mes >= 10 ? mes.ToString() : "0" + mes.ToString();
                string año = Convert.ToDateTime(Tx_ano.Value).Year.ToString();
                string fecha = "01/" + periodo + "/" + año;                

                string emp = cod_empresa;

                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(fecha, cod_empresa, source.Token), source.Token);
                await slowTask;

                if (dt_depreciar.Rows.Count > 0) dt_depreciar.Clear();
                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    dataGrid.ItemsSource = ((DataSet)slowTask.Result).Tables[0].DefaultView;
                    Tx_toact.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();
                    dt_depreciar = ((DataSet)slowTask.Result).Tables[0];
                }
                else dt_depreciar.Clear();
                sfBusyIndicator.IsBusy = false;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al consultar:" + w);
            }
        }

        private DataSet LoadData(string fecha, string emp, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                cmd.CommandTimeout = 0;
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpAF_depreciacion", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fecha", fecha);
                cmd.Parameters.AddWithValue("@codemp", emp);
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
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
            try
            {
                Tx_ano.IsEnabled = true;
                Tx_periodo.IsEnabled = true;
                dataGrid.ItemsSource = null;
                Tx_toact.Text = "...";
                dt_depreciar.Clear();
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cancelar:"+w);
            }
        }


    }
}
