using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;

namespace MenuReporteSetting
{

    public partial class Form : UserControl
    {
        dynamic SiaWin;
        int idemp = 0;
        string cnEmp = "";

        //edit es true - y insert es false
        public bool edit = false;

        public string tipo = "";
        public int server = 0;
        public int ParentIdRow = 0;

        public int id = 0;
        public Form()
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
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void PanelForm_Loaded(object sender, RoutedEventArgs e)
        {
            SetTitle(tipo, edit);
            loadServer(server, edit);
            cargardatos(edit, id);
        }

        public void SetTitle(string tipo, bool edit)
        {
            if (tipo == "Togle1")
            {
                TX_title.Text = "Items Principales";
                TX_parent.Visibility = Visibility.Hidden;
            }
            if (tipo == "Togle2")
            {
                TX_title.Text = "Items Segundarios";
                TX_parent.Visibility = Visibility.Visible;
                cargarParents(tipo, edit);
            }
            if (tipo == "Togle3")
            {
                TX_title.Text = "Items Terciarios";
                TX_parent.Visibility = Visibility.Visible;
                cargarParents(tipo, edit);
            }
        }
        public void loadServer(int server, bool edit)
        {
            //MessageBox.Show("server:" + server);
            string where = server == 0 ? "" : "where idrow=" + server + "";
            string query = "select * from ReportServer " + where + " ;";
            DataTable dt = SiaWin.Func.SqlDT(query, "Existencia", 0);
            TX_server.ItemsSource = dt.DefaultView;
            if (edit == true && server != 0) TX_server.SelectedValue = server;
        }

        public void cargarParents(string tipo, bool edit)
        {
            string where = tipo == "Togle2" ? "where type_item='1'" : "where type_item='2'";

            DataTable dt = SiaWin.Func.SqlDT("select idrow,name_item from Menu_Reports " + where + "", "Existencia", 0);
            TX_parent.ItemsSource = dt.DefaultView;
            TX_parent.DisplayMemberPath = "name_item";
            TX_parent.SelectedValuePath = "idrow";
            if (edit == true) TX_parent.SelectedValue = ParentIdRow;
        }

        public void cargardatos(bool bandera, int id)
        {
            try
            {
                if (bandera == true && id > 0)
                {
                    TX_title.Tag = id;
                    DataTable dt = SiaWin.Func.SqlDT("select * from Menu_Reports where idrow=" + id + ";", "Existencia", 0);
                    if (dt.Rows.Count > 0)
                    {
                        int typePNt = Convert.ToInt32(dt.Rows[0]["typePnt"]);
                        int isRep = Convert.ToInt32(dt.Rows[0]["id_parm"]);
                        switch (typePNt)
                        {
                            case 0:
                                Check1.IsChecked = true; Ck1.IsChecked = true;
                                break;
                            case 1:
                                Check2.IsChecked = true; Ck2.IsChecked = true; TX_urlreporte.Text = dt.Rows[0]["reporte"].ToString();
                                break;
                            case 2:
                                Check3.IsChecked = true; Ck7.IsChecked = true; TX_idscreen.Text = dt.Rows[0]["id_Screen"].ToString();
                                break;
                            case 3:
                                if (isRep == 1) { Check2.IsChecked = true; TX_urlreporte.Text = dt.Rows[0]["reporte"].ToString(); } //else Check3.IsChecked = true;
                                if (isRep == 0) { Check3.IsChecked = true; TX_idscreen.Text = dt.Rows[0]["id_Screen"].ToString(); } //else Ck8.IsChecked = true;
                                break;
                            case 4:
                                Check4.IsChecked = true; Ck9.IsChecked = true; TX_urlreporte.Text = dt.Rows[0]["reporte"].ToString();
                                break;
                            case 5:
                                Check4.IsChecked = true; Ck10.IsChecked = true; TX_urlreporte.Text = dt.Rows[0]["reporte"].ToString();
                                break;
                        }

                        TX_idAcceso.Text = dt.Rows[0]["id_acceso"].ToString();

                    }
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar datos:" + w);
            }
        }

        private void BtnAtras_Click(object sender, RoutedEventArgs e)
        {
            panelForm.Visibility = Visibility.Hidden;
        }

        public event RoutedEventHandler ActualizarParentEventHandler;

        private void BTN_form_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string typeItem = "";
                string cod_ItemP = "";
                switch (tipo)
                {
                    case "Togle1": typeItem = "1"; cod_ItemP = ""; break;
                    case "Togle2": typeItem = "2"; cod_ItemP = TX_parent.SelectedValue.ToString(); break;
                    case "Togle3": typeItem = "3"; cod_ItemP = TX_parent.SelectedValue.ToString(); break;
                }

                int id_screen = 0;
                int isreport = 0;
                string reporte = "";
                string typePnt = devTagcheking();
                int idServer = 0;

                string chek = devNameCheck();
                switch (chek)
                {
                    case "Check1": id_screen = 0; isreport = 0; reporte = ""; idServer = 0; break;
                    case "Check2": id_screen = 0; isreport = 1; reporte = TX_urlreporte.Text; idServer = Convert.ToInt32(TX_server.SelectedValue); break;
                    case "Check3": id_screen = Convert.ToInt32(TX_idscreen.Text); isreport = 0; reporte = ""; idServer = 0; break;
                    case "Check4": id_screen = 0; isreport = 0; reporte = TX_urlreporte.Text; idServer = 0; break;
                }



                if (BTN_form.Content.ToString().Trim() == "Guardar")
                {
                    string query = "insert into Menu_Reports (cod_itemP,name_item,type_item,id_Screen,id_parm,reporte,typePnt,idserver,id_acceso) " +
                        "values ('" + cod_ItemP + "','" + TX_nameitem.Text.Trim() + "','" + typeItem + "'," + id_screen + "," + isreport + ",'" + reporte + "','" + typePnt + "'," + idServer + ",'" + TX_idAcceso.Text + "')";

                    if (SiaWin.Func.SqlCRUD(query, 0) == true)
                    {
                        MessageBox.Show("Item Agregado exitosamente");
                        BTNatras.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));                        
                        if (ActualizarParentEventHandler != null)                            
                            ActualizarParentEventHandler(sender, e);
                    }


                }
                if (BTN_form.Content.ToString() == "Modificar")
                {
                    string update = "update Menu_Reports set cod_itemP='" + cod_ItemP + "',name_item='" + TX_nameitem.Text.Trim() + "',type_item='" + typeItem + "'," +
                        "id_Screen='" + id_screen + "',id_parm=" + isreport + ",reporte='" + reporte + "',typePnt='" + typePnt + "',idserver=" + idServer + ",id_acceso=" + TX_idAcceso.Text + " where idrow=" + TX_title.Tag + ";";

                    //MessageBox.Show("update:" + update);

                    if (SiaWin.Func.SqlCRUD(update, 0) == true)
                    {
                        MessageBox.Show("Actualizacion exitosa");
                        BTNatras.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        if (ActualizarParentEventHandler != null)
                            ActualizarParentEventHandler(sender, e);
                    }
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("erro en el CRUD:" + w);
            }



        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            string Name = ((CheckBox)sender).Name.ToString().Trim();
            foreach (CheckBox item in GridTipo.Children)
                if (item.Name != Name) item.IsChecked = false;
            visibiliGrid(Name);
        }


        public string devNameCheck()
        {
            string name = "";
            foreach (CheckBox item in GridTipo.Children)
                if (item.IsChecked == true) name = item.Name;
            return name;
        }


        public void visibiliGrid(string name)
        {
            if (name == "Check1")
            {
                GridNada.Visibility = Visibility.Visible;
                GridReport.Visibility = Visibility.Hidden;
                GridPnt.Visibility = Visibility.Hidden;
                GridNavegador.Visibility = Visibility.Hidden;

                foreach (CheckBox item in GridNada.Children) item.IsChecked = false;
                foreach (CheckBox item in GridReport.Children) item.IsChecked = false;
                foreach (CheckBox item in GridPnt.Children) item.IsChecked = false;

                TX_urlreporte.IsEnabled = false; TX_server.IsEnabled = false; TX_idscreen.IsEnabled = false;
            }
            if (name == "Check2")
            {
                GridReport.Visibility = Visibility.Visible;
                GridNada.Visibility = Visibility.Hidden;
                GridPnt.Visibility = Visibility.Hidden;
                GridNavegador.Visibility = Visibility.Hidden;

                foreach (CheckBox item in GridNada.Children) item.IsChecked = false;
                foreach (CheckBox item in GridReport.Children) item.IsChecked = false;
                foreach (CheckBox item in GridPnt.Children) item.IsChecked = false;
                foreach (CheckBox item in GridNavegador.Children) item.IsChecked = false;

                TX_urlreporte.IsEnabled = true; TX_server.IsEnabled = true; TX_idscreen.IsEnabled = false;
            }
            if (name == "Check3")
            {
                GridPnt.Visibility = Visibility.Visible;
                GridReport.Visibility = Visibility.Hidden;
                GridNada.Visibility = Visibility.Hidden;
                GridNavegador.Visibility = Visibility.Hidden;

                foreach (CheckBox item in GridNada.Children) item.IsChecked = false;
                foreach (CheckBox item in GridReport.Children) item.IsChecked = false;
                foreach (CheckBox item in GridPnt.Children) item.IsChecked = false;
                foreach (CheckBox item in GridNavegador.Children) item.IsChecked = false;

                TX_urlreporte.IsEnabled = false; TX_server.IsEnabled = false; TX_idscreen.IsEnabled = true;
            }
            if (name == "Check4")
            {
                GridNavegador.Visibility = Visibility.Visible;
                GridPnt.Visibility = Visibility.Hidden;
                GridReport.Visibility = Visibility.Hidden;
                GridNada.Visibility = Visibility.Hidden;

                foreach (CheckBox item in GridNada.Children) item.IsChecked = false;
                foreach (CheckBox item in GridReport.Children) item.IsChecked = false;
                foreach (CheckBox item in GridPnt.Children) item.IsChecked = false;
                foreach (CheckBox item in GridNavegador.Children) item.IsChecked = false;

                TX_urlreporte.IsEnabled = true; TX_server.IsEnabled = false; TX_idscreen.IsEnabled = false;

            }

        }


        private void Cheking(object sender, RoutedEventArgs e)
        {
            string Name = ((CheckBox)sender).Name.ToString().Trim();
            FrameworkElement parent = (FrameworkElement)((CheckBox)sender).Parent;
            StackPanel panel = (StackPanel)this.FindName(parent.Name);
            foreach (CheckBox item in panel.Children) if (item.Name != Name) item.IsChecked = false;
        }

        public string devTagcheking()
        {
            string tag = "";
            if (Check1.IsChecked == true)
                foreach (CheckBox item in GridNada.Children) if (item.IsChecked == true) tag = item.Tag.ToString();
            if (Check2.IsChecked == true)
                foreach (CheckBox item in GridReport.Children) if (item.IsChecked == true) tag = item.Tag.ToString();
            if (Check3.IsChecked == true)
                foreach (CheckBox item in GridPnt.Children) if (item.IsChecked == true) tag = item.Tag.ToString();
            if (Check4.IsChecked == true)
                foreach (CheckBox item in GridNavegador.Children) if (item.IsChecked == true) tag = item.Tag.ToString();
            return tag;
        }


        private void ValidacionNumeros(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab || e.Key == Key.OemMinus || e.Key == Key.Subtract || e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 || e.Key == Key.Back || e.Key == Key.Left || e.Key == Key.Right)
                e.Handled = false;
            else { MessageBox.Show("este campo solo admite valores numericos"); e.Handled = true; }
        }








    }
}
