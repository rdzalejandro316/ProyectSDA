using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;

namespace MenuReporteSetting
{

    public partial class Form : Window
    {
        dynamic SiaWin;
        int idemp = 0;
        string cnEmp = "";


        // edicion
        public int idrow = 0;
        public string nameParent = "";
        public DataRow[] Datos;
        public int nivel = 0;

        public string titleLevel = "";
        public string IdNameParent = "";
        public string IdRowParent = "";


        public bool flag = false;

        public Form()
        {
            InitializeComponent();
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

                loadItems();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void panelForm_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SiaWin = Application.Current.MainWindow;
                idemp = SiaWin._BusinessId;

                LoadConfig();
                TxTitle.Text = titleLevel;


                if (idrow > 0)
                {
                    if (Datos.Length > 0)
                    {


                        TxNombre.Text = Datos[0]["name_item"].ToString();
                        TxParent.Text = nameParent;
                        TxParent.Tag = Datos[0]["cod_itemP"].ToString();

                        int idserver = Datos[0]["idserver"] == DBNull.Value ? 0 : (int)Datos[0]["idserver"]; ;
                        string typePnt = Datos[0]["typePnt"].ToString();
                        string tag = Checks(typePnt, idserver);
                        foreach (CheckBox check in PanelCheck.Children)
                        {
                            if (check.Tag.ToString() == tag) check.IsChecked = true;
                        }

                        TxIdAcceso.Value = Datos[0]["id_acceso"] == DBNull.Value ? 0 : (int)Datos[0]["id_acceso"];
                        CbModulo.SelectedValue = Datos[0]["ModulesId"] == DBNull.Value ? -1 : (int)Datos[0]["ModulesId"];
                        TxUrlReport.Text = Datos[0]["reporte"].ToString();
                        CbServer.SelectedValue = idserver;
                        TxIdScreen.Value = Datos[0]["id_Screen"] == DBNull.Value ? 0 : (int)Datos[0]["id_Screen"];
                        TxStoredProcedure.Text = Datos[0]["stored_procedure"].ToString();
                        TxParaEmp.Text = Datos[0]["param_emp"].ToString();


                    }
                }
                else
                {
                    TxParent.Text = IdNameParent;
                    TxParent.Tag = IdRowParent;
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar:" + w);
            }
        }


        public void loadItems()
        {
            try
            {

                DataSet ds = LoadItems();

                if (ds.Tables.Count > 0)
                {

                    CbModulo.ItemsSource = ds.Tables[0].DefaultView;
                    CbModulo.DisplayMemberPath = "ModulesName";
                    CbModulo.SelectedValuePath = "ModulesId";

                    CbServer.ItemsSource = ds.Tables[1].DefaultView;
                    CbServer.SelectedValuePath = "idrow";
                    CbServer.DisplayMemberPath = "ServerIP";
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar items:" + w);
            }
        }

        public DataSet LoadItems()
        {
            try
            {
                DataSet ds = new DataSet();

                DataTable dtmod = SiaWin.Func.SqlDT("select rtrim(ModulesId) as ModulesId,rtrim(ModulesName) as ModulesName from Modules order by ModulesId", "modulos", 0);
                ds.Tables.Add(dtmod);

                DataTable dtconfig = SiaWin.Func.SqlDT("select rtrim(idrow) as idrow,rtrim(ServerIP) as ServerIP from ReportServer ", "config", 0);
                ds.Tables.Add(dtconfig);

                return ds;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar informacion:" + w);
                return null;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox check = ((CheckBox)sender);
            foreach (CheckBox item in PanelCheck.Children)
            {
                if (item.Content != check.Content) item.IsChecked = false;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                #region validaciones
                if (string.IsNullOrEmpty(TxNombre.Text))
                {
                    MessageBox.Show("el campo nombre debe de estar lleno", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                string tipo = "";
                foreach (CheckBox item in PanelCheck.Children)
                {
                    if (item.IsChecked == true) tipo = item.Tag.ToString();
                }

                var typepnt = GetTypePnt(tipo);

                if (string.IsNullOrEmpty(tipo))
                {
                    MessageBox.Show("debe de seleccionar algun tipo de pantalla", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }


                if (CbModulo.SelectedIndex < 0)
                {
                    MessageBox.Show("el campo id modulo debe de estar lleno", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }


                #endregion

                MessageBoxResult messageBox = MessageBox.Show("usted desea guardar los cambios", "alerta", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);


                if (messageBox == MessageBoxResult.Yes)
                {
                    using (SqlConnection connection = new SqlConnection(SiaWin._cn))
                    {

                        string message = "";
                        StringBuilder query = new StringBuilder();
                        if (idrow > 0)
                        {
                            query.Append($"UPDATE Menu_Reports SET ");
                            query.Append($"cod_itemP=@cod_itemP,name_item=@name_item,type_item=@type_item,id_Screen=@id_Screen,id_parm=@id_parm,reporte=@reporte, ");
                            query.Append($"typePnt=@typePnt,idserver=@idserver,id_acceso=@id_acceso,param_emp=@param_emp,stored_procedure=@stored_procedure,ModulesId=@ModulesId ");
                            query.Append($"where idrow={idrow}");
                            message = "se actualizo exisotamente la informacion";
                        }
                        else
                        {
                            query.Append($"INSERT INTO Menu_Reports ");
                            query.Append($"(cod_itemP,name_item,type_item,id_Screen,id_parm,reporte,typePnt,idserver,id_acceso,param_emp,stored_procedure,ModulesId) ");
                            query.Append($"VALUES");
                            query.Append($"(@cod_itemP,@name_item,@type_item,@id_Screen,@id_parm,@reporte,@typePnt,@idserver,@id_acceso,@param_emp,@stored_procedure,@ModulesId)");
                            message = "se guardo exisotamente la informacion";
                        }


                        using (SqlCommand command = new SqlCommand(query.ToString(), connection))
                        {
                            command.Parameters.AddWithValue("@cod_itemP", TxParent.Tag);
                            command.Parameters.AddWithValue("@name_item", TxNombre.Text);
                            command.Parameters.AddWithValue("@type_item", nivel);
                            command.Parameters.AddWithValue("@id_Screen", TxIdScreen.Value.ToString());
                            command.Parameters.AddWithValue("@id_parm", typepnt.Item2);
                            command.Parameters.AddWithValue("@reporte", TxUrlReport.Text);
                            command.Parameters.AddWithValue("@typePnt", typepnt.Item1);
                            command.Parameters.AddWithValue("@idserver", CbServer.SelectedIndex < 0 ? 0 : Convert.ToInt32(CbServer.SelectedValue));
                            command.Parameters.AddWithValue("@id_acceso", TxIdAcceso.Value.ToString());
                            command.Parameters.AddWithValue("@param_emp", TxParaEmp.Text);
                            command.Parameters.AddWithValue("@stored_procedure", TxStoredProcedure.Text);
                            command.Parameters.AddWithValue("@ModulesId", CbModulo.SelectedIndex<0 ? 0 : Convert.ToInt32(CbModulo.SelectedValue));
                            connection.Open();

                            StringBuilder sb = new StringBuilder();

                            //command.Parameters.Cast<DbParameter>()
                            //                  .ToList()
                            //                  .ForEach(p => sb.Append(
                            //                                   string.Format("{0} = {1}{2}",
                            //                                      p.ParameterName,
                            //                                      p.Value,
                            //                                      Environment.NewLine)));


                            //MessageBox.Show(command.CommandText);
                            //MessageBox.Show(sb.ToString());

                            int result = command.ExecuteNonQuery();


                            if (result > 0)
                            {
                                MessageBox.Show(message, "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                flag = true;
                                this.Close();
                            }

                        }
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al guardar:" + w);
            }
        }

        public Tuple<string, int> GetTypePnt(string tipo)
        {
            string type_return = "";
            int iserver = 0;
            switch (tipo)
            {
                case "0": type_return = "0"; break;
                case "1": type_return = "1"; iserver = 1; break;
                case "2": type_return = "3"; iserver = 1; break;
                case "3": type_return = "2"; iserver = 0; break;
                case "4": type_return = "3"; iserver = 0; break;
                case "5": type_return = "4"; iserver = 0; break;
                case "6": type_return = "5"; iserver = 1; break;
            }
            return new Tuple<string, int>(type_return, iserver);
        }

        public string Checks(string tipo, int idserver)
        {
            string tag = "";
            switch (tipo)
            {
                case "0": tag = "0"; break;
                case "1": tag = "1"; break;
                case "2": tag = "3"; break;
                case "3": tag = idserver == 1 ? "2" : "4"; break;
                case "4": tag = "5"; break;
                case "5": tag = "6"; break;
            }

            return tag;
        }



        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            flag = false;
            this.Close();
        }


    }
}
