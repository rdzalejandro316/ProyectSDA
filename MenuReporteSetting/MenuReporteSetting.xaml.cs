using MenuReporteSetting;
using System;
using System.Data;
using System.Windows;
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
                this.Title = "Config (" + aliasemp + ")";
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                string Name = ((ToggleButton)sender).Name.ToString().Trim();
                foreach (ToggleButton item in GridToogle.Children)
                {
                    if (item.Name != Name) item.IsChecked = false;
                }

                getDatagridItem(Name);
            }
            catch (Exception W)
            {
                MessageBox.Show("NADA:" + W);
            }

        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Togle1.IsChecked == false && Togle2.IsChecked == false && Togle3.IsChecked == false) GridItem.ItemsSource = null;
        }

        public void getDatagridItem(string nameToogle)
        {
            string select = "";
            if (nameToogle == "Togle1")
            {
                select = "select Menu_Reports.idrow as idrowparent,Menu_Reports.name_item as name_itemparent,id_Screen,type_item,id_parm,reporte,typePnt,idserver ";
                select += "from Menu_Reports ";
                select += "where type_item='1' ";
                column4.Visibility = Visibility.Hidden;
            }

            if (nameToogle == "Togle3" || nameToogle == "Togle2")
            {
                string tipo = nameToogle == "Togle2" ? "2" : "3";
                select = "select Menu_Reports.idrow as idrowchild,Menu_Reports.name_item as name_itemchild,";
                select += "Menu_Reports.id_Screen,Menu_Reports.type_item,Menu_Reports.id_parm,";
                select += "Menu_Reports.reporte,Menu_Reports.typePnt,Menu_Reports.idserver,";
                select += "menu.idrow as idrowparent,menu.name_item as name_itemparent,Menu_Reports.id_Screen ";
                select += "from Menu_Reports ";
                select += "inner join Menu_Reports as menu on menu.idrow = Menu_Reports.cod_itemp ";
                select += "where Menu_Reports.type_item='" + tipo + "';";
                column4.Visibility = Visibility.Visible;
            }

            DataTable dt = SiaWin.Func.SqlDT(select, "Existencia", 0);
            GridItem.ItemsSource = dt.DefaultView;
        }

        void ActualizarEventhandler(object sender, RoutedEventArgs e)
        {           
            getDatagridItem(nameToogle());
        }

        Form formulario;
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var verificacion = IsCheck();
            if (verificacion.check == false)
            {
                MessageBox.Show("Selecciona un item para agregar");
                return;
            }

            formulario = new Form();
            formulario.tipo = verificacion.itemToogle;
            formulario.edit = false;
            formulario.BTN_form.Content = "Guardar";
            formulario.ActualizarParentEventHandler += new RoutedEventHandler(ActualizarEventhandler);
            formulario.panelForm.Visibility = Visibility.Visible;
            Main.Children.Add(formulario);
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                var verificacion = IsCheck();
                if (verificacion.check == false)
                {
                    MessageBox.Show("Selecciona un item para agregar");
                    return;
                }


                if (GridItem.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)GridItem.SelectedItems[0];
                    //string id = row["idrowparent"].ToString().Trim();

                    Form formulario = new Form();

                    formulario.edit = true;
                    formulario.tipo = verificacion.itemToogle;
                    formulario.id = Togle1.IsChecked == true ? Convert.ToInt32(row["idrowparent"]) : Convert.ToInt32(row["idrowchild"]);
                    formulario.BTN_form.Content = "Modificar";
                    formulario.ActualizarParentEventHandler += new RoutedEventHandler(ActualizarEventhandler);
                    formulario.panelForm.Visibility = Visibility.Visible;
                    formulario.TX_nameitem.Text = Togle1.IsChecked == true ? row["name_itemparent"].ToString().Trim() : row["name_itemchild"].ToString().Trim();                    
                    formulario.server = Convert.ToInt32(row["idserver"]);
                    formulario.ParentIdRow = Convert.ToInt32(row["idrowparent"]);
                    Main.Children.Add(formulario);
                }
                else
                {
                    MessageBox.Show("Selecciona un item de la grilla para poder editar");
                }
            }
            catch (Exception W){MessageBox.Show("ERROR EN LA EDICION:"+W);}

        }

        public ValuesReturn IsCheck()
        {
            ValuesReturn valores;
            valores.check = false;
            valores.itemToogle = "ninguno";

            foreach (ToggleButton item in GridToogle.Children)
            {
                if (item.IsChecked == true)
                {
                    valores.check = true;
                    valores.itemToogle = item.Name;
                    break;
                }
            }

            return valores;
        }


        public struct ValuesReturn
        {
            public bool check;
            public string itemToogle;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var verificacion = IsCheck();
                if (verificacion.check == false)
                {
                    MessageBox.Show("Selecciona un item para eliminar");
                    return;
                }

                if (GridItem.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)GridItem.SelectedItems[0];
                    string item = row["name_itemparent"].ToString().Trim();

                    if (MessageBox.Show("Usted desea eliminar el item:" + item + "?", "Guardar Traslado", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        string delete = "";

                        int idDel = Togle1.IsChecked == true ? Convert.ToInt32(row["idrowparent"]) : Convert.ToInt32(row["idrowchild"]);

                        delete = "delete Menu_Reports where idrow=" + idDel+ ";";

                        if (SiaWin.Func.SqlCRUD(delete, 0) == true)
                        {
                            MessageBox.Show("Eliminacion de item exitosa");
                            getDatagridItem(nameToogle());
                        }
                                                    
                    }
                }


            }
            catch (Exception w)
            {
                MessageBox.Show("erro al eliminar:" + w);
            }
        }


        public bool ExisChild(int id)
        {
            bool bandera = false;
            DataTable dt = SiaWin.Func.SqlDT("select * from Menu_Reports where cod_itemP='"+id+"'", "Existencia", 0);
            if (dt.Rows.Count > 0) bandera = true;
            return bandera;
        }

        public string nameToogle()
        {
            string nameItem = "";
            foreach (ToggleButton item in GridToogle.Children)
            {
                if (item.IsChecked == true) nameItem = item.Name;
            }
            return nameItem.Trim();
        }






    }
}
