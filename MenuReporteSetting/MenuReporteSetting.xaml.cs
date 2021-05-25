using MenuReporteSetting;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SiasoftAppExt
{

    //Sia.PublicarPnt(9521,"MenuReporteSetting");       
    //dynamic WinDescto = ((Inicio)Application.Current.MainWindow).WindowExt(9521, "MenuReporteSetting");
    //WinDescto.ShowInTaskbar = false;
    //WinDescto.Owner = Application.Current.MainWindow;
    //WinDescto.WindowStartupLocation = WindowStartupLocation.CenterScreen;
    //WinDescto.ShowDialog(); 

    public partial class MenuReporteSetting : Window
    {
        dynamic SiaWin;
        int idemp = 0;
        string cnEmp = "";
        DataTable dtitems;

        public MenuReporteSetting()
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
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                this.Title = "Configuracion";
                loadItems();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public async void loadItems()
        {
            try
            {

                var slowTask = Task<DataTable>.Factory.StartNew(() => LoadItems());
                await slowTask;

                GridNivel2.ItemsSource = null;
                GridNivel3.ItemsSource = null;

                if (slowTask.Result.Rows.Count > 0)
                {
                    dtitems = slowTask.Result;
                    DataRow[] row = dtitems.Select("type_item=1");

                    if (row.Length > 0)
                    {
                        GridNivel1.ItemsSource = row.CopyToDataTable().DefaultView;
                    }

                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar items:" + w);
            }
        }
        public DataTable LoadItems()
        {
            try
            {
                string select = "select idrow,cod_itemP,name_item,type_item,id_Screen,id_parm,reporte,typePnt,idserver,id_acceso,param_emp,stored_procedure,ModulesId from Menu_Reports";
                DataTable dt = SiaWin.Func.SqlDT(select, "temp", 0);
                return dt;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar informacion:" + w);
                return null;
            }
        }
        private void GridNivel1_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            try
            {
                if (GridNivel1.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)GridNivel1.SelectedItems[0];
                    string idrowparent = row["idrow"].ToString();
                    DataRow[] temp = dtitems.Select("type_item=2 and cod_itemP=" + idrowparent + "");

                    GridNivel3.ItemsSource = null;
                    if (temp.Length > 0)
                    {
                        GridNivel2.ItemsSource = temp.CopyToDataTable().DefaultView;
                    }
                    else
                    {
                        GridNivel2.ItemsSource = null;
                    }

                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar:" + w);
            }
        }

        private void GridNivel2_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            try
            {
                if (GridNivel2.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)GridNivel2.SelectedItems[0];
                    string idrowparent = row["idrow"].ToString();
                    DataRow[] temp = dtitems.Select("type_item=3 and cod_itemP=" + idrowparent + "");

                    if (temp.Length > 0)
                    {
                        GridNivel3.ItemsSource = temp.CopyToDataTable().DefaultView;
                    }
                    else
                    {
                        GridNivel3.ItemsSource = null;
                    }

                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar:" + w);
            }
        }



        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                int nivel = Convert.ToInt32(((Button)sender).Tag);

                Syncfusion.UI.Xaml.Grid.SfDataGrid sfData = new Syncfusion.UI.Xaml.Grid.SfDataGrid();

                string idrow = "";
                string name = "";

                switch (nivel)
                {
                    case 1: sfData = null; break;
                    case 2:
                        sfData = GridNivel1;
                        if (GridNivel1.SelectedIndex < 0)
                        {
                            MessageBox.Show($"debe de seleccionar algun item del nivel 1 para poder agregar", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            return;
                        }
                        break;
                    case 3:
                        sfData = GridNivel2;
                        if (GridNivel2.SelectedIndex < 0)
                        {
                            MessageBox.Show($"debe de seleccionar algun item del nivel 2 para poder agregar", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            return;
                        }
                        break;
                }

                if (sfData != null)
                {
                    DataRowView row = (DataRowView)sfData.SelectedItems[0];
                    idrow = row["idrow"].ToString();
                    name = row["name_item"].ToString();
                }


                Form form = new Form();
                form.IdRowParent = idrow;
                form.IdNameParent = name;
                form.titleLevel = $"Nivel {nivel}";
                form.nivel = nivel;
                form.ShowInTaskbar = false;
                form.Owner = Application.Current.MainWindow;
                form.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                form.ShowDialog();

                if (form.flag) loadItems();

            }
            catch (Exception w)
            {
                MessageBox.Show("error en BtnAdd_Click:" + w);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int nivel = Convert.ToInt32(((Button)sender).Tag);

                Syncfusion.UI.Xaml.Grid.SfDataGrid sfData = new Syncfusion.UI.Xaml.Grid.SfDataGrid();

                int idrow = 0;
                string name = "";

                switch (nivel)
                {
                    case 1: sfData = GridNivel1; break;
                    case 2: sfData = GridNivel2; break;
                    case 3: sfData = GridNivel3; break;
                }


                if (sfData.SelectedIndex < 0)
                {
                    MessageBox.Show($"debe de seleccionar algun item del nivel {nivel} para poder editar", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }


                DataRowView row = (DataRowView)sfData.SelectedItems[0];
                idrow = (int)row["idrow"];
                name = row["name_item"].ToString();

                
                DataRow[] temp = dtitems.Select($"idrow={idrow}");
                string nameparent = "";                
                if (temp.Length > 0)
                {                    
                    string cod_itemp = temp[0]["cod_itemp"].ToString().Trim();                    
                    if (!string.IsNullOrWhiteSpace(cod_itemp))
                    {                        
                        DataRow[] parent = dtitems.Select($"idrow={cod_itemp}");                        
                        if (parent.Length > 0) nameparent = parent[0]["name_item"].ToString();
                    }                    
                }

                Form form = new Form();
                form.nameParent = nameparent;
                form.titleLevel = $"Nivel {nivel}";
                form.idrow = idrow;
                form.Datos = temp;
                form.nivel = nivel;
                form.ShowInTaskbar = false;
                form.Owner = Application.Current.MainWindow;
                form.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                form.ShowDialog();

                if (form.flag) loadItems();

            }
            catch (Exception w)
            {
                MessageBox.Show("error en BtnAdd_Click:" + w);
            }

        }


        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int nivel = Convert.ToInt32(((Button)sender).Tag);

                Syncfusion.UI.Xaml.Grid.SfDataGrid sfData = new Syncfusion.UI.Xaml.Grid.SfDataGrid();

                switch (nivel)
                {
                    case 1: sfData = GridNivel1; break;
                    case 2: sfData = GridNivel2; break;
                    case 3: sfData = GridNivel3; break;
                }


                if (sfData.SelectedIndex < 0)
                {
                    MessageBox.Show($"debe de seleccionar algun item del nivel {nivel} para poder eliminarlo", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                DataRowView row = (DataRowView)sfData.SelectedItems[0];

                string idrow = row["idrow"].ToString();
                string name = row["name_item"].ToString();

                DataRow[] temp = dtitems.Select($"type_item={nivel + 1} and cod_itemP={idrow}");
                if (temp.Length > 0)
                {
                    MessageBox.Show($"el item {name} tiene elementos anidados debe eliminarlos primero dichos elementos", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }


                MessageBoxResult message = MessageBox.Show($"Usted desea eliminar el item:{name} del nivel {nivel} ?", "Confitmacion", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (message == MessageBoxResult.Yes)
                {
                    string delete = $"delete Menu_Reports where idrow={idrow};";                    

                    if (SiaWin.Func.SqlCRUD(delete, 0) == true)
                    {
                        MessageBox.Show("Eliminacion de item exitosa", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        loadItems();
                    }

                }

            }
            catch (Exception w)
            {
                MessageBox.Show("erro al eliminar:" + w);
            }
        }


    }
}
