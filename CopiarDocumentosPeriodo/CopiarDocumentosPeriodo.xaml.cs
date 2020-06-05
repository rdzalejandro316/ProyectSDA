using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

    //    Sia.PublicarPnt(9639,"CopiarDocumentosPeriodo");
    //    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9639,"CopiarDocumentosPeriodo");
    //    ww.ShowInTaskbar = false;
    //    ww.Owner = Application.Current.MainWindow;
    //    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //    ww.ShowDialog();

    public partial class CopiarDocumentosPeriodo : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        DataTable trn = new DataTable();

        public CopiarDocumentosPeriodo()
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
                this.Title = "Copiar Documentos a otro Periodo";

                trn = SiaWin.Func.SqlDT("SELECT rtrim(cod_trn) as cod_trn,rtrim(cod_trn)+'-'+rtrim(nom_trn) as nom_trn FROM comae_trn", "transacion", idemp);
                t_TrnCop.ItemsSource = trn.DefaultView;
                t_TrnCop.DisplayMemberPath = "nom_trn";
                t_TrnCop.SelectedValuePath = "cod_trn";


                t_TrnNue.ItemsSource = trn.DefaultView;
                t_TrnNue.DisplayMemberPath = "nom_trn";
                t_TrnNue.SelectedValuePath = "cod_trn";

            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }


        public bool GetNewDoc()
        {
            DataTable dt_cab = SiaWin.Func.SqlDT("select * from cocab_doc where num_trn='" + Tx_NumeroNue.Text + "' and cod_trn='" + t_TrnNue.SelectedValue + "' ", "cabeza", idemp);
            return dt_cab.Rows.Count > 0 ? true : false;
        }



        private void BtnProcesar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (t_TrnCop.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione el tipo de transaccion a copiar", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (string.IsNullOrEmpty(Tx_Numero.Text))
                {
                    MessageBox.Show("ingrese el documento a copiar", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }


                if (GetNewDoc() == true)
                {
                    MessageBox.Show("el documento nuevo a copiar:" + Tx_NumeroNue.Text + " ya existe en contabilidad ingrese un codigo diferente", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }



                DataTable dt_cab = SiaWin.Func.SqlDT("select * from cocab_doc where num_trn='" + Tx_Numero.Text + "' and cod_trn='" + t_TrnCop.SelectedValue + "' ", "cabeza", idemp);
                if (dt_cab.Rows.Count <= 0)
                {
                    MessageBox.Show("el documento ingresado:" + Tx_Numero.Text + " no existe", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                else
                {
                    DataTable dt_cue = SiaWin.Func.SqlDT("select * from cocue_doc where idregcab='" + dt_cab.Rows[0]["idreg"].ToString() + "' ", "cuerpo", idemp);
                    if (dt_cue.Rows.Count > 0)
                    {
                        using (SqlConnection connection = new SqlConnection(cnEmp))
                        {
                            string query = "INSERT INTO CoCab_doc (cod_trn,num_trn,fec_trn,factura,dia_plaz,fec_posf,fec_ven,cod_ven,cod_ban,otro_ter,detalle,num_imp,trm,suc_cli,ind_com,_usu) VALUES (@cod_trn,@num_trn,@fec_trn,@factura,@dia_plaz,@fec_posf,@fec_ven,@cod_ven,@cod_ban,@otro_ter,@detalle,@num_imp,@trm,@suc_cli,@ind_com,@_usu);SELECT CAST(scope_identity() AS int)";
                            using (SqlCommand cmd = new SqlCommand(query, connection))
                            {
                                cmd.Parameters.AddWithValue("@cod_trn", t_TrnNue.SelectedValue);
                                cmd.Parameters.AddWithValue("@num_trn", Tx_NumeroNue.Text);
                                cmd.Parameters.AddWithValue("@fec_trn", dt_cab.Rows[0]["fec_trn"].ToString());
                                cmd.Parameters.AddWithValue("@ano_doc", Convert.ToDateTime(Tx_anoNue.Value).Year);
                                cmd.Parameters.AddWithValue("@per_doc", Convert.ToDateTime(Tx_perNue.Value).Month);
                                cmd.Parameters.AddWithValue("@factura", dt_cab.Rows[0]["factura"].ToString());
                                cmd.Parameters.AddWithValue("@dia_plaz", Convert.ToInt32(dt_cab.Rows[0]["dia_plaz"]));
                                cmd.Parameters.AddWithValue("@fec_posf", dt_cab.Rows[0]["fec_posf"].ToString());
                                cmd.Parameters.AddWithValue("@fec_ven", dt_cab.Rows[0]["fec_ven"].ToString());
                                cmd.Parameters.AddWithValue("@cod_ven", dt_cab.Rows[0]["cod_ven"].ToString());
                                cmd.Parameters.AddWithValue("@cod_ban", dt_cab.Rows[0]["cod_ban"].ToString());
                                cmd.Parameters.AddWithValue("@otro_ter", dt_cab.Rows[0]["otro_ter"].ToString());
                                cmd.Parameters.AddWithValue("@detalle", Tx_DescNue.Text);
                                cmd.Parameters.AddWithValue("@num_imp", dt_cab.Rows[0]["num_imp"].ToString());
                                cmd.Parameters.AddWithValue("@trm", dt_cab.Rows[0]["trm"].ToString());
                                cmd.Parameters.AddWithValue("@suc_cli", dt_cab.Rows[0]["suc_cli"].ToString());
                                cmd.Parameters.AddWithValue("@ind_com", dt_cab.Rows[0]["ind_com"].ToString());
                                cmd.Parameters.AddWithValue("@_usu", getUser());

                                connection.Open();

                                int newID = (int)cmd.ExecuteScalar();

                                if (newID == 0) MessageBox.Show("la transacion no fue exitosa", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                else
                                {
                                    foreach (DataRow item in dt_cue.Rows)
                                    {
                                        string query_cu = "INSERT INTO Cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ciu,cod_suc,cod_cco,cod_ter,des_mov) values (@idregcab,@cod_trn,@num_trn,@cod_cta,@cod_ciu,@cod_suc,@cod_cco,@cod_ter,@des_mov)";
                                        using (SqlCommand cmd_cu = new SqlCommand(query_cu, connection))
                                        {
                                            cmd_cu.Parameters.AddWithValue("@idregcab", newID);
                                            cmd_cu.Parameters.AddWithValue("@cod_trn", t_TrnNue.SelectedValue);
                                            cmd_cu.Parameters.AddWithValue("@num_trn", Tx_NumeroNue.Text);
                                            cmd_cu.Parameters.AddWithValue("@cod_cta", item["cod_cta"].ToString());
                                            cmd_cu.Parameters.AddWithValue("@cod_ciu", item["cod_ciu"].ToString());
                                            cmd_cu.Parameters.AddWithValue("@cod_suc", item["cod_suc"].ToString());
                                            cmd_cu.Parameters.AddWithValue("@cod_cco", item["cod_cco"].ToString());
                                            cmd_cu.Parameters.AddWithValue("@cod_ter", item["cod_ter"].ToString());
                                            cmd_cu.Parameters.AddWithValue("@des_mov", item["des_mov"].ToString());
                                            cmd_cu.ExecuteScalar();
                                        }
                                    }

                                    MessageBox.Show("copia de documento exitosa", "procesos exitoso", MessageBoxButton.OK, MessageBoxImage.Information);
                                    clean();
                                }
                            }
                        }


                    }
                    else
                    {
                        MessageBox.Show("el documento ingresado:" + Tx_Numero.Text + " no tiene cuerpo consulte con el administrador del sistema", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al procesar:" + w);
            }
        }


        public void clean()
        {
            t_TrnCop.SelectedIndex = -1;
            Tx_Numero.Text = "";

            t_TrnNue.SelectedIndex = -1;
            Tx_NumeroNue.Text = "";
            Tx_DescNue.Text = "";
        }




        public string getUser()
        {
            string nameUsu = "";
            DataTable dt = SiaWin.Func.SqlDT("select UserName,UserAlias from Seg_User where UserId='" + SiaWin._UserId + "' ", "usuarios", 0);
            if (dt.Rows.Count > 0) nameUsu = dt.Rows[0]["username"].ToString().Trim();
            return nameUsu;
        }


        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int idr = 0; string code = ""; string nombre = "";
                dynamic xx = SiaWin.WindowBuscar("CoCab_doc", "cod_trn", "num_trn", "cod_trn", "idreg", "Documentos", cnEmp, false, "", idEmp: idemp);
                xx.ShowInTaskbar = false;
                xx.Owner = Application.Current.MainWindow;
                xx.Height = 400;
                xx.Width = 400;
                xx.ShowDialog();
                idr = xx.IdRowReturn;
                code = xx.Codigo;
                nombre = xx.Nombre;
                xx = null;
                if (idr > 0)
                {
                    Tx_Numero.Text = nombre;
                    selectedTrn(code);
                    DataTable dt = SiaWin.Func.SqlDT("select * from cocab_doc where num_trn='" + nombre + "' and cod_trn='" + t_TrnCop.SelectedValue + "' ", "table", idemp);
                    if (dt.Rows.Count > 0)
                    {                        
                        Tx_anoCop.Value = dt.Rows[0]["fec_trn"].ToString();
                        Tx_perCop.Value = dt.Rows[0]["fec_trn"].ToString();
                    }
                }
                if (string.IsNullOrEmpty(code)) e.Handled = false;
                if (string.IsNullOrEmpty(code)) return;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al buscar la transaccion:" + w);
            }

        }


        public void selectedTrn(string code)
        {
            string query = "select * from comae_trn  where cod_trn='" + code + "' ";
            DataTable dt = SiaWin.Func.SqlDT(query, "table", idemp);
            if (dt.Rows.Count > 0)
            {
                int i = 0;
                foreach (DataRow item in trn.Rows)
                {
                    if (item["cod_trn"].ToString().Trim() == code.Trim()) t_TrnCop.SelectedIndex = i;
                    i++;
                }
            }

        }

        private void Tx_Numero_LostFocus(object sender, RoutedEventArgs e)
        {
            string document = (sender as TextBox).Text.Trim();
            string tipo = (sender as TextBox).Tag.ToString();

            if (string.IsNullOrEmpty(document)) return;
            if (tipo == "doc_viejo")
            {

                if (t_TrnCop.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione el tipo de transaccion", "alerta", MessageBoxButton.OK, MessageBoxImage.Stop);
                    (sender as TextBox).Text = "";
                    return;
                }

                DataTable dt = SiaWin.Func.SqlDT("select * from cocab_doc where num_trn='" + document + "' and cod_trn='" + t_TrnCop.SelectedValue + "' ", "table", idemp);
                if (dt.Rows.Count > 0)
                {
                    selectedTrn(dt.Rows[0]["cod_trn"].ToString());
                    Tx_anoCop.Value = dt.Rows[0]["fec_trn"].ToString();
                    Tx_perCop.Value = dt.Rows[0]["fec_trn"].ToString();
                    (sender as TextBox).Foreground = Brushes.Black;
                }
                else
                {
                    MessageBox.Show("el documento ingresado no existe", "alerta", MessageBoxButton.OK, MessageBoxImage.Error);
                    (sender as TextBox).Foreground = Brushes.Red;
                    return;
                }
            }

        }









    }
}


