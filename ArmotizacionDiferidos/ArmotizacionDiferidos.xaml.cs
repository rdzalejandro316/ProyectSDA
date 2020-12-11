using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

    //Sia.PublicarPnt(9689,"ArmotizacionDiferidos");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9689, "ArmotizacionDiferidos");
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //ww.ShowDialog();

    public partial class ArmotizacionDiferidos : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        public ArmotizacionDiferidos()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            if (idemp <= 0) idemp = SiaWin._BusinessId;
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
                this.Title = "Maestra de armotizacion diferidos";


                DataTable dt = SiaWin.Func.SqlDT("select periodo,periodonombre from Periodos where TipoPeriodo='1' ", "tabla", 0);
                CBperiodos.ItemsSource = dt.DefaultView;

                DataTable dt_trn = SiaWin.Func.SqlDT("select rtrim(cod_trn) as cod_trn,rtrim(cod_trn)+'-'+rtrim(nom_trn) as nom_trn from comae_trn order by cod_trn", "tabla", idemp);
                CBtipotrn.ItemsSource = dt_trn.DefaultView;

                CBtipotrn.SelectedValue = "90";
                TxFecTrn.Text = DateTime.Now.ToString();
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private async void BtnProcess_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                #region validaciones
                if (string.IsNullOrWhiteSpace(TxDocumento.Text))
                {
                    MessageBox.Show("el campo de numero de transaccion debe de estar lleno", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (CBperiodos.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione un periodo", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (!string.IsNullOrWhiteSpace(TxDocumento.Text))
                {
                    string cod_trn = CBtipotrn.SelectedValue.ToString();
                    string num_trn = TxDocumento.Text;

                    DataTable dt = SiaWin.Func.SqlDT("select * from cocab_doc where cod_trn='" + cod_trn + "' and num_trn='" + num_trn + "' ", "cabeza", idemp);
                    if (dt.Rows.Count > 0)
                    {
                        MessageBox.Show("la transaccion " + num_trn + " ya existe ", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                }

                #endregion

                sfBusyIndicator.IsBusy = true;
                GridConfig.IsEnabled = false;
                GridConfig.Opacity = 0.5;

                string año = "";
                string periodo = "";
                string empresa = cod_empresa;
                int idmodulo = 1;

                var slowTask = Task<DataTable>.Factory.StartNew(() => LoadData(año, periodo, empresa));
                await slowTask;

                if (slowTask.IsCompleted)
                {
                    int idreg = document(slowTask.Result);
                    if (idreg > 0)
                    {
                        SiaWin.TabTrn(0, idemp, true, idreg, idmodulo, WinModal: true);
                        sfBusyIndicator.IsBusy = false;
                        GridConfig.IsEnabled = true;
                        GridConfig.Opacity = 1;
                    }
                    else
                    {
                        MessageBox.Show("no se genero ningun documento", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }

                sfBusyIndicator.IsBusy = false;
                GridConfig.IsEnabled = true;
                GridConfig.Opacity = 1;

            }
            catch (Exception w)
            {
                MessageBox.Show("error al procesar:" + w);
            }
        }

        public int document(DataTable cuerpo)
        {
            int bandera = -1;
            try
            {

                if (MessageBox.Show("Usted desea guardar el documento de armotizacion?", "alerta", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {

                    using (SqlConnection connection = new SqlConnection(cnEmp))
                    {
                        connection.Open();
                        SqlCommand command = connection.CreateCommand();
                        SqlTransaction transaction = connection.BeginTransaction("Transaction");
                        command.Connection = connection;
                        command.Transaction = transaction;

                        string sqlcab = "";
                        string sqlcue = "";


                        string cod_trn = CBtipotrn.SelectedValue.ToString();
                        string num_trn = TxDocumento.Text;
                        string fec_trn = TxFecTrn.Text;
                        DateTime tiempo = Convert.ToDateTime(Tx_ano.Value.ToString());
                        string año = tiempo.ToString("MM");
                        string per_doc = CBperiodos.SelectedValue.ToString();

                        sqlcab = @"INSERT INTO cocab_doc (cod_trn,num_trn,fec_trn) values ('" + cod_trn + "','" + num_trn + "','" + fec_trn + "');DECLARE @NewID INT;SELECT @NewID = SCOPE_IDENTITY();";


                        foreach (DataRow dr in cuerpo.Rows)
                        {
                            decimal cuo = Convert.ToDecimal(dr["cuotas"]);

                            if (cuo > 0)
                            {
                                string cod_dif = dr["cod_dif"].ToString().Trim();
                                string nom_dif = dr["nom_dif"].ToString().Trim();
                                string observ = dr["observ"].ToString().Trim();
                                string poliza = dr["poliza"].ToString().Trim();
                                string cta_dif = dr["cta_dif"].ToString().Trim();
                                string cta_amo = dr["cta_amo"].ToString().Trim();
                                string cod_ter = dr["cod_ter"].ToString().Trim();
                                string cod_cco = dr["cod_cco"].ToString().Trim();

                                string cuotas = cuo.ToString("F", CultureInfo.InvariantCulture);

                                string des_mov = "Dif.:" + cod_dif + "-" + nom_dif + "- Amort." + per_doc + " del " + año + "-" + observ;

                                sqlcue += @"INSERT INTO cocue_doc (idregcab,cod_cta,cod_ter,cod_cco,des_mov,doc_mov,deb_mov,cre_mov) values (@NewID,'" + cta_dif + "','" + cod_ter + "','" + cod_cco + "','" + observ + "','" + poliza + "',0," + cuotas + ");";

                                sqlcue += @"INSERT INTO cocue_doc (idregcab,cod_cta,cod_ter,cod_cco,des_mov,doc_mov,deb_mov,cre_mov) values (@NewID,'" + cta_amo + "','" + cod_ter + "','" + cod_cco + "','" + observ + "','" + poliza + "'," + cuotas + ",0);";
                            }
                        }

                        command.CommandText = sqlcab + sqlcue + @"select CAST(@NewId AS int);";

                        var r = new object();
                        r = command.ExecuteScalar();
                        transaction.Commit();
                        connection.Close();
                        bandera = Convert.ToInt32(r.ToString());
                    }
                }
                else
                {
                    bandera = -1;
                }

                return bandera;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al generar documento de soporte:" + w);
                return bandera;
            }
        }
        private DataTable LoadData(string ano, string periodo, string empresa)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataTable dt = new DataTable();
                cmd = new SqlCommand("_EmpAmortizacionDiferido", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ano", ano);
                cmd.Parameters.AddWithValue("@periodo", periodo);
                cmd.Parameters.AddWithValue("@codemp", empresa);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                con.Close();
                return dt;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }


        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}
