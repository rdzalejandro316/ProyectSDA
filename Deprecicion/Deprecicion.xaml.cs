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

    //   Sia.PublicarPnt(9662,"Deprecicion");
    //    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9662,"Deprecicion");
    //    ww.ShowInTaskbar = false;
    //    ww.Owner = Application.Current.MainWindow;
    //    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //    ww.ShowDialog();

    public partial class Deprecicion : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public DataTable dt_depreciar = new DataTable();

        public Deprecicion()
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
                this.Title = "Depreciacion";


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

                DataTable dt = SiaWin.Func.SqlDT("select * from Afcab_doc where ano_doc='" + Year + "' and per_doc='" + Month + "' and cod_trn='901' ", "table", idemp);
                if (dt.Rows.Count > 0)
                {
                    MessageBox.Show("ya se genero la depreciacion para el periodo:" + Month + "-" + Year, "alerta", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (dt_depreciar.Rows.Count<=0)
                {
                    MessageBox.Show("Genere la consulta para poder depreciar", "alerta", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                #endregion
                int id = documento();
                if (id>0) 
                {
                    int idcontable = ContabilizaCompraAf(id);
                    SiaWin.TabTrn(0, idemp, true, idcontable, 1, WinModal: true);
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
            int idreg = 0;
            if (MessageBox.Show("Usted desea generar la depreciacion del año:"+Year+" periodo:"+Month, "Guardar Traslado", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {

                string codtrn = "901";
                string fecha = DateTime.Now.Day + "/" + Month + "/" + Year;
                DateTime fechaActual = Convert.ToDateTime(fecha);

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


                    string sqlConsecutivo = @"declare @fecdoc as datetime;
                    set @fecdoc = getdate();declare @ini as char(4);
                    declare @num as varchar(12);declare @iConsecutivo char(12) = ''
                    declare @iFolioHost int = 0;
                    SELECT @iFolioHost =num_act,@ini=rtrim(inicial) FROM Afmae_trn WHERE cod_trn='901'
                    set @num=@iFolioHost;select @iConsecutivo=rtrim(@ini)+'-'+rtrim(convert(varchar,@num));                    
                     ";


                    string sqlcab = sqlConsecutivo + @"INSERT INTO Afcab_doc (cod_trn,num_trn,fec_trn)
                        values ('" + codtrn + "',@iConsecutivo,@fecdoc);DECLARE @NewID INT;SELECT @NewID = SCOPE_IDENTITY();";

                    string sqlcue = "";
                    foreach (DataRow  dr in dt_depreciar.Rows)
                    {
                        string cod_act = dr["cod_act"].ToString().Trim();
                        decimal mes_ini = Convert.ToDecimal(dr["mesini"]);
                        decimal vr_act = Convert.ToDecimal(dr["vr_act"]);
                        decimal val_depreciar = vr_act / mes_ini;

                        sqlcue = sqlcue + @"INSERT INTO afcue_doc (idregcab,cod_trn,num_trn,cod_act,dep_ac,mesxdep) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + cod_act+ "'," + val_depreciar.ToString("F", CultureInfo.InvariantCulture) + ",-1);";
                    }
                                                        

                    string actualzaConsecu = "UPDATE Afmae_trn SET  num_act= ISNULL(num_act,0)+1  WHERE where cod_trn='901';";
                    command.CommandText = sqlcab + sqlcue + actualzaConsecu + @"select CAST(@NewId AS int);";
                    MessageBox.Show(command.CommandText.ToString());
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
                MessageBox.Show("no se genero el Documento");
                return 0;
            }
        }

        private int ContabilizaCompraAf(int idreg)
        {
            int idregreturn = -1;
            try
            {                

                #region obtiene datos principales                
                string query = "select Afcab_doc.cod_trn,Afcab_doc.num_trn,Afmae_trn.cod_tdo from Afcab_doc  ";
                query += "inner join Afmae_trn on Afmae_trn.cod_trn = Afcab_doc.cod_trn ";
                query += "where idreg ='" + idreg + "' ";

                DataTable dt_trn = SiaWin.Func.SqlDT(query, "cuerpo", idemp);

                string cod_trn_af = dt_trn.Rows.Count > 0 ? dt_trn.Rows[0]["cod_trn"].ToString().Trim() : "";
                string cod_trn_co = dt_trn.Rows.Count > 0 ? dt_trn.Rows[0]["cod_tdo"].ToString().Trim() : "";
                string num_trn_co = dt_trn.Rows.Count > 0 ? dt_trn.Rows[0]["num_trn"].ToString().Trim() : "";

                #endregion

                #region obtiene cuerpo     

                int Year = Convert.ToDateTime(Tx_ano.Value).Year;
                int Month = Convert.ToDateTime(Tx_periodo.Value).Month;

                string querycue = "select cuerpo.cod_act,activo.cod_gru,grupo.cta_dep,grupo.cta_gdp,cuerpo.dep_ac  ";
                querycue += "from Afcue_doc as cuerpo ";
                querycue += "inner join Afmae_act as activo on activo.COD_ACT = cuerpo.cod_act ";
                querycue += "inner join Afmae_gru as grupo on grupo.cod_gru = activo.cod_gru ";
                querycue += "where cuerpo.idregcab='" + idreg + "' ";

                DataTable dt_cuerpo = SiaWin.Func.SqlDT(querycue, "cuerpo", idemp);

                string sqlcuerpo = "";

                foreach (System.Data.DataRow item in dt_cuerpo.Rows)
                {
                    decimal dep_ac = Convert.ToDecimal(item["dep_ac"]);
                    string cod_act = item["cod_act"].ToString().Trim();

                    string cta_gdp = item["cta_gdp"].ToString().Trim();
                    string cta_dep = item["cta_dep"].ToString().Trim();

                    sqlcuerpo += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,des_mov,deb_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cta_gdp + "','Depreciacion :" + cod_act + " - Per:" + Month + " - Año:'" + Year + "," + dep_ac.ToString("F", CultureInfo.InvariantCulture) + "); ";
                    sqlcuerpo += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,des_mov,cre_mov) values (@NewTrn,'" + cod_trn_co + "','" + num_trn_co + "','" + cod_act + "','Depreciacion :" + cod_act + " - Per:"+Month+" - Año:'"+Year+"," + dep_ac.ToString("F", CultureInfo.InvariantCulture) + "); ";
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
            this.Close();
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
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;

                sfBusyIndicator.IsBusy = true;
                dataGrid.ItemsSource = null;
                source.CancelAfter(TimeSpan.FromSeconds(1));

                DateTime now = DateTime.Now;
                string mes = Convert.ToDateTime(Tx_periodo.Value).Month.ToString();
                string año = Convert.ToDateTime(Tx_ano.Value).Year.ToString();
                string fecha = now.Day.ToString() + "/" + mes + "/" + año;

                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(fecha, source.Token), source.Token);
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

        private DataSet LoadData(string fecha, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(cnEmp);
                SqlCommand cmd = new SqlCommand();
                cmd.CommandTimeout = 0;
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpSaldosActivosTodos", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fec_trn", fecha);
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








    }
}
