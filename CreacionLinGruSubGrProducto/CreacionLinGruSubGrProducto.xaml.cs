using CreacionLinGruSubGrProducto;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
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
    //Sia.PublicarPnt(9678,"CreacionLinGruSubGrProducto");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9678,"CreacionLinGruSubGrProducto");    
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();   
    public partial class CreacionLinGruSubGrProducto : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        int moduloid = 2;
        public CreacionLinGruSubGrProducto()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            pantalla();
            LoadConfig();
            LoadLineas();
        }

        void pantalla()
        {
            this.MinWidth = 1000;
            this.MinHeight = 600;
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
                this.Title = "Creacion de lineas,grupos,sub grupos,referencias -" + cod_empresa + "-" + nomempresa;

                ////ParamAcc 1=lRun,2=lNew,3=lEdit,4=lDelete,5=lSearch,5=Renum,6=lPrint,7=lExport,8=lOpc1,9=lOpc2,10=lOpc3
                int idmaelinea = 75;
                BtnAddLinea.IsEnabled = SiaWin.Func.Acceso(SiaWin._UserGroup, SiaWin._ProyectId, idmaelinea, idemp, moduloid, "lNew") == true ? true : false;
                BtnEditLinea.IsEnabled = SiaWin.Func.Acceso(SiaWin._UserGroup, SiaWin._ProyectId, idmaelinea, idemp, moduloid, "lEdit") == true ? true : false;
                BtnEliminarLinea.IsEnabled = SiaWin.Func.Acceso(SiaWin._UserGroup, SiaWin._ProyectId, idmaelinea, idemp, moduloid, "lDelete") == true ? true : false;

                int idmaegrupo = 73;
                BtnAddGrupo.IsEnabled = SiaWin.Func.Acceso(SiaWin._UserGroup, SiaWin._ProyectId, idmaegrupo, idemp, moduloid, "lNew") == true ? true : false;
                BtnEditGrupo.IsEnabled = SiaWin.Func.Acceso(SiaWin._UserGroup, SiaWin._ProyectId, idmaegrupo, idemp, moduloid, "lEdit") == true ? true : false;
                BtnEliminarGrupo.IsEnabled = SiaWin.Func.Acceso(SiaWin._UserGroup, SiaWin._ProyectId, idmaegrupo, idemp, moduloid, "lDelete") == true ? true : false;

                int idmaesubgrupo = 80;
                BtnAddSubGrupo.IsEnabled = SiaWin.Func.Acceso(SiaWin._UserGroup, SiaWin._ProyectId, idmaesubgrupo, idemp, moduloid, "lNew") == true ? true : false;
                BtnEditSubGrupo.IsEnabled = SiaWin.Func.Acceso(SiaWin._UserGroup, SiaWin._ProyectId, idmaesubgrupo, idemp, moduloid, "lEdit") == true ? true : false;
                BtnEliminarSubGrupo.IsEnabled = SiaWin.Func.Acceso(SiaWin._UserGroup, SiaWin._ProyectId, idmaesubgrupo, idemp, moduloid, "lDelete") == true ? true : false;

                int idmaereferencia = 79;
                BtnAddRef.IsEnabled = SiaWin.Func.Acceso(SiaWin._UserGroup, SiaWin._ProyectId, idmaereferencia, idemp, moduloid, "lNew") == true ? true : false;
                BtnAddEdit.IsEnabled = SiaWin.Func.Acceso(SiaWin._UserGroup, SiaWin._ProyectId, idmaereferencia, idemp, moduloid, "lEdit") == true ? true : false;
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        public void LoadLineas()
        {
            try
            {
                Tx_linea.Text = "";
                Tx_grupo.Text = "";
                Tx_subgrupo.Text = "";
                TxCodRef.Text = "";
                TxCodAnt.Text = "";


                dataGridGrupo.ItemsSource = null;
                dataGridSubGrupo.ItemsSource = null;
                DataTable lineas = SiaWin.Func.SqlDT("select idrow,cod_tip,nom_tip From inmae_tip order by cod_tip", "Lineas", idemp);
                if (lineas.Rows.Count > 0)
                {
                    dataGridLineas.ItemsSource = lineas.DefaultView;
                    Reg_linea.Text = lineas.Rows.Count.ToString();
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al carga las lineas:" + w);
            }
        }

        public DataTable LoadGrupo(string linea)
        {
            try
            {
                DataTable grupos = SiaWin.Func.SqlDT("select idrow,cod_gru,nom_gru,cod_tip From inmae_gru where cod_tip='" + linea + "' order by cod_gru", "Lineas", idemp);
                return grupos;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al carga las lineas:" + w);
                return null;
            }
        }

        public DataTable LoadSubGrupo(string grupo, string linea)
        {
            try
            {
                DataTable subgrupos = SiaWin.Func.SqlDT("select idrow,cod_sgr,nom_sgr,cod_tip,cod_gru From InMae_sgr where cod_tip='" + linea + "' and cod_gru='" + grupo + "' order by cod_sgr;", "subgrupo", idemp);
                return subgrupos;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al carga las lineas:" + w);
                return null;
            }
        }

        public DataTable LoadReferencias(string linea)
        {
            try
            {
                DataTable referencia = SiaWin.Func.SqlDT("select * From inmae_ref where cod_tip='" + linea + "';", "referencia", idemp);
                return referencia;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al carga las lineas:" + w);
                return null;
            }
        }

        public DataSet LoadReferGrup(string linea)
        {
            try
            {
                DataSet ds = new DataSet();
                DataTable grupo = LoadGrupo(linea);
                DataTable referencia = LoadReferencias(linea);
                ds.Tables.Add(referencia);
                ds.Tables.Add(grupo);
                return ds;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al carga las lineas:" + w);
                return null;
            }
        }

        private async void dataGridLineas_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            try
            {
                if (dataGridLineas.SelectedIndex >= 0)
                {
                    dataGridLineas.IsEnabled = false;
                    sfBusyIndicator.IsBusy = true;
                    dataGridReferencia.ItemsSource = null;
                    dataGridGrupo.ItemsSource = null;
                    dataGridSubGrupo.ItemsSource = null;

                    CancellationTokenSource source = new CancellationTokenSource();
                    DataRowView row = (DataRowView)dataGridLineas.SelectedItems[0];
                    string linea = row["cod_tip"].ToString().Trim();
                    Tx_linea.Text = linea;
                    Tx_grupo.Text = "";
                    Tx_subgrupo.Text = "";

                    var slowTask = Task<DataSet>.Factory.StartNew(() => LoadReferGrup(linea), source.Token);
                    await slowTask;
                    if (slowTask.IsCompleted)
                    {

                        //referencia
                        if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                        {
                            dataGridReferencia.ItemsSource = ((DataSet)slowTask.Result).Tables[0].DefaultView;
                            Reg_referencias.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();
                        }
                        else
                        {
                            dataGridReferencia.ItemsSource = null;
                            Reg_referencias.Text = "0";
                        }

                        //grupo
                        if (((DataSet)slowTask.Result).Tables[1].Rows.Count > 0)
                        {
                            dataGridGrupo.ItemsSource = ((DataSet)slowTask.Result).Tables[1].DefaultView;
                            Reg_grupo.Text = ((DataSet)slowTask.Result).Tables[1].Rows.Count.ToString();
                        }
                        else
                        {
                            dataGridGrupo.ItemsSource = null;
                            Reg_grupo.Text = "0";
                        }
                    }
                    dataGridLineas.IsEnabled = true;
                    sfBusyIndicator.IsBusy = false;
                    dataGridLineas.Focus();
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al seleccionar la grilla:" + w);
            }
        }

        private async void dataGridGrupo_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            try
            {
                if (dataGridLineas.SelectedIndex >= 0 && dataGridGrupo.SelectedIndex >= 0)
                {
                    DataRowView rowgrup = (DataRowView)dataGridGrupo.SelectedItems[0];
                    string grupo = rowgrup["cod_gru"].ToString().Trim();
                    Tx_grupo.Text = grupo;
                    Tx_subgrupo.Text = "";
                    DataRowView rowlin = (DataRowView)dataGridLineas.SelectedItems[0];
                    string linea = rowlin["cod_tip"].ToString().Trim();


                    dataGridLineas.IsEnabled = false;
                    dataGridGrupo.IsEnabled = false;
                    dataGridSubGrupo.ItemsSource = null;

                    CancellationTokenSource source = new CancellationTokenSource();
                    var slowTask = Task<DataTable>.Factory.StartNew(() => LoadSubGrupo(grupo, linea), source.Token);
                    await slowTask;
                    if (slowTask.IsCompleted)
                    {
                        if (((DataTable)slowTask.Result).Rows.Count > 0)
                        {
                            dataGridSubGrupo.ItemsSource = ((DataTable)slowTask.Result).DefaultView;
                            Reg_Subgrupo.Text = ((DataTable)slowTask.Result).Rows.Count.ToString();
                        }
                        else
                        {
                            dataGridSubGrupo.ItemsSource = null;
                            Reg_Subgrupo.Text = "0";
                        }
                    }
                    dataGridLineas.IsEnabled = true;
                    dataGridGrupo.IsEnabled = true;
                    dataGridGrupo.Focus();
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al seleccionar la grilla:" + w);
            }
        }

        private void dataGridSubGrupo_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            try
            {
                if (dataGridSubGrupo.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)dataGridSubGrupo.SelectedItems[0];
                    Tx_subgrupo.Text = row["cod_sgr"].ToString().Trim();
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al seleccionar grilla de sub_grupo:" + w);
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                AddNew win = new AddNew();
                string tag = (sender as Button).Tag.ToString().Trim();
                switch (tag)
                {
                    case "linea":
                        win.linea = true;
                        break;
                    case "grupo":
                        if (string.IsNullOrEmpty(Tx_linea.Text))
                        {
                            MessageBox.Show("seleccione una linea para agregar un grupo", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            win.Close();
                            return;
                        }
                        else
                        {
                            win.TxLinea.Text = Tx_linea.Text;
                        }
                        win.grupo = true;
                        break;
                    case "subgrupo":
                        if (string.IsNullOrEmpty(Tx_grupo.Text))
                        {
                            MessageBox.Show("seleccione una grupo para agregar un sub grupo", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            win.Close();
                            return;
                        }
                        else
                        {
                            win.TxLinea.Text = Tx_linea.Text;
                            win.TxGrupo.Text = Tx_grupo.Text;
                        }
                        win.subgrupo = true;
                        break;
                }


                win.edicion = false;
                win.ShowInTaskbar = false;
                win.Owner = Application.Current.MainWindow;
                win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                win.ShowDialog();
                if (win.actualizargrilla) LoadLineas();
            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir pantalla:" + w);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                AddNew win = new AddNew();

                string tag = (sender as Button).Tag.ToString().Trim();
                int index = 0;
                string messa = "";
                SfDataGrid grid = new SfDataGrid();
                switch (tag)
                {
                    case "linea":
                        win.linea = true; index = dataGridLineas.SelectedIndex; grid = dataGridLineas; messa = "seleccione una linea";
                        break;
                    case "grupo":
                        win.grupo = true; index = dataGridGrupo.SelectedIndex; grid = dataGridGrupo; messa = "seleccione un grupo";
                        break;
                    case "subgrupo":
                        win.subgrupo = true; index = dataGridSubGrupo.SelectedIndex; grid = dataGridSubGrupo; messa = "seleccione un subgrupo";
                        break;
                }
                if (index >= 0)
                {
                    DataRowView row = (DataRowView)grid.SelectedItems[0];

                    if (tag == "linea")
                    {
                        win.TxLinea.Text = row["cod_tip"].ToString().Trim();
                        win.TxNombre.Text = row["nom_tip"].ToString().Trim();
                    }

                    if (tag == "grupo")
                    {
                        win.TxLinea.Text = row["cod_tip"].ToString().Trim();
                        win.TxGrupo.Text = row["cod_gru"].ToString().Trim();
                        win.TxNombre.Text = row["nom_gru"].ToString().Trim();
                    }

                    if (tag == "subgrupo")
                    {
                        win.TxLinea.Text = row["cod_tip"].ToString().Trim();
                        win.TxGrupo.Text = row["cod_gru"].ToString().Trim();
                        win.TxSubGrupo.Text = row["cod_sgr"].ToString().Trim();
                        win.TxNombre.Text = row["nom_sgr"].ToString().Trim();
                    }
                }
                else
                {
                    MessageBox.Show(messa, "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                win.edicion = true;
                win.ShowInTaskbar = false;
                win.Owner = Application.Current.MainWindow;
                win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                win.ShowDialog();
                if (win.actualizargrilla) LoadLineas();

            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir pantalla:" + w);
            }
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tag = (sender as Button).Tag.ToString();
                string tabla = "";
                string codigo = "";
                int idrow = 0;
                string message = "";
                string message_exito = "";
                switch (tag)
                {
                    case "linea":
                        if (dataGridLineas.SelectedIndex >= 0)
                        {
                            DataRowView row = (DataRowView)dataGridLineas.SelectedItems[0];
                            codigo = row["cod_tip"].ToString().Trim();
                            message = "usted desea eliminar la linea:" + codigo + "?";
                            tabla = "inmae_tip";
                            idrow = Convert.ToInt32(row["idrow"]);
                            message_exito = "linea";
                        }
                        else
                        {
                            MessageBox.Show("seleccione la linea que desea eliminar", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            return;
                        }
                        break;
                    case "grupo":
                        if (dataGridGrupo.SelectedIndex >= 0)
                        {
                            DataRowView row = (DataRowView)dataGridGrupo.SelectedItems[0];
                            codigo = row["cod_gru"].ToString().Trim();
                            message = "usted desea eliminar el grupo:" + codigo + "?";
                            tabla = "InMae_gru";
                            idrow = Convert.ToInt32(row["idrow"]);
                            message_exito = "grupo";
                        }
                        else
                        {
                            MessageBox.Show("seleccione el grupo que desea eliminar", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            return;
                        }
                        break;
                    case "subgrupo":
                        if (dataGridSubGrupo.SelectedIndex >= 0)
                        {
                            DataRowView row = (DataRowView)dataGridSubGrupo.SelectedItems[0];
                            codigo = row["cod_sgr"].ToString().Trim();
                            message = "usted desea eliminar el sub grupo:" + codigo + "?";
                            tabla = "inmae_sgr";
                            idrow = Convert.ToInt32(row["idrow"]);
                            message_exito = "sub grupo";
                        }
                        else
                        {
                            MessageBox.Show("seleccione el sub grupo que desea eliminar", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            return;
                        }
                        break;
                }


                MessageBoxResult result = MessageBox.Show(message, "Confirmacion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    if (SiaWin.Func.DeleteInMaestra(idrow, tabla, "idrow", idemp))
                    {


                        SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, moduloid, -1, -9, "eliminacion de " + message_exito + " - codigo:" + codigo, "");
                        MessageBox.Show("se elimino exitosamente - " + message_exito + ":" + codigo);
                        LoadLineas();
                    }
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al eliminar:" + w);
            }
        }

        private void BtnAddRef_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region validaciones

                if (string.IsNullOrEmpty(TxCodRef.Text) || string.IsNullOrEmpty(TxCodAnt.Text))
                {
                    MessageBox.Show("el campo referencia y codigo anterior deben de esta llenos para poder crear la referencia", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (string.IsNullOrEmpty(Tx_linea.Text))
                {
                    MessageBox.Show("la referencia por lo menos debe de tener una linea asociada", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                DataTable dt = SiaWin.Func.SqlDT("select * from inmae_ref where cod_ref='" + TxCodRef.Text + "'", "referencia", idemp);
                if (dt.Rows.Count > 0)
                {
                    MessageBox.Show("la referencia " + TxCodRef.Text + " ya existe", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                #endregion

                MessageBoxResult result = MessageBox.Show("usted desea crear la referencia:" + TxCodRef.Text, "Confirmacion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    string query = "insert into inmae_ref (cod_ref,nom_ref,cod_ant,cod_tip,cod_gru,cod_sgr) values ('" + TxCodRef.Text + "','" + TxCodAnt.Text + "','" + TxCodAnt.Text + "','" + Tx_linea.Text + "','" + Tx_grupo.Text + "','" + Tx_subgrupo.Text + "');";
                    if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                    {
                        SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, moduloid, -1, -9, "se inserto la referencia exitosamente :" + TxCodRef.Text + "", "");
                        MessageBox.Show("se inserto la referencia exitosamente", "alerta", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadLineas();
                    }
                }


            }
            catch (Exception w)
            {
                MessageBox.Show("error al crear referencias:" + w);
            }
        }

        private void BtnEditRef_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridReferencia.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)dataGridReferencia.SelectedItems[0];
                    string cod_reg = row["cod_ref"].ToString();
                    string[] strArrayParam = new string[] { row["cod_ref"].ToString().Trim(), row["nom_ref"].ToString().Trim(), idemp.ToString(), "0" };
                    SiaWin.Tab(9614, strArrayParam, idEmp: idemp);
                }
                else
                {
                    MessageBox.Show("seleccione una referencias de la lista para editar:", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al editar referencias:" + w);
            }
        }





    }
}
