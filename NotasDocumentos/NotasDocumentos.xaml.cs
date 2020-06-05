using NotasDocumentos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9573,"NotasDocumentos");    
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9573, "NotasDocumentos");
    //ww.ShowInTaskbar=false;    
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
    //ww.numero_trn = "CMI-00020043";
    //ww.codigo_trn = "001";
    //ww.ShowDialog(); 

    public partial class NotasDocumentos : Window
    {

        dynamic SiaWin;
        int idemp = 0;
        string cnEmp = "";

        public int idrowcab = 0;
        public string modulo = ""; //INV,CON,ACF,NII;MMA

        public NotasDocumentos()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadConfig();

                if (idrowcab == 0 || idrowcab == -1)
                {
                    Win.IsEnabled = false;
                    Txt_ocu.Visibility = Visibility.Visible;
                    return;
                }

                var cmp = modulos(modulo);

                if (!string.IsNullOrEmpty(cmp.Item1) && !string.IsNullOrEmpty(cmp.Item2))
                {
                    string select = "select * from " + cmp.Item1 + " where " + cmp.Item2 + "='" + idrowcab + "' ";
                    DataTable tabla = SiaWin.Func.SqlDT(select, "Clientes", idemp);
                    if (tabla.Rows.Count > 0)
                    {
                        TX_Docum.Text = tabla.Rows[0]["num_trn"].ToString().Trim();
                        TX_Cod.Text = tabla.Rows[0]["cod_trn"].ToString().Trim();
                        TX_Docum.Tag = idrowcab;
                        getList(idrowcab.ToString(), modulo);
                    }
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar");
            }
        }

        public Tuple<string, string> modulos(string modulo)
        {
            string tabla = ""; string campo = "";
            switch (modulo)
            {
                case "INV":
                    tabla = "incab_doc"; campo = "idreg"; break;
                case "CON":
                    tabla = "cocab_doc"; campo = "idreg"; break;
                case "ACF":
                    tabla = "afcab_doc"; campo = "idreg"; break;
                case "MMA":
                    tabla = "Mmcab_doc"; campo = "idreg"; break;
                case "NII":
                    tabla = "NIcab_doc"; campo = "idreg"; break;
            }
            return new Tuple<string, string>(tabla, campo);
        }





        public void getList(string idrow, string modulo)
        {
            try
            {
                string cmp = tablaNotas(modulo);

                if (!string.IsNullOrEmpty(cmp))
                {
                    string select = "select ROW_NUMBER() OVER(ORDER BY idrowcab ASC) AS id,fecha,title,nota,idrow from " + cmp + " where idrowcab='" + idrow + "' ";
                    DataTable tabla = SiaWin.Func.SqlDT(select, "Clientes", idemp);
                    list.ItemsSource = tabla.Rows.Count > 0 ? tabla.DefaultView : null;
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error en la consulta:" + w);
            }
        }


        public string tablaNotas(string modulo)
        {
            string tabla = "";
            switch (modulo)
            {
                case "INV":
                    tabla = "incab_notas"; break;
                case "CON":
                    tabla = "cocab_notas"; break;
                case "ACF":
                    tabla = "afcab_notas"; break;
                case "MMA":
                    tabla = "Mmcab_notas"; break;
                case "NII":
                    tabla = "NIcab_notas"; break;
            }
            return tabla;
        }


        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            NotaAdd ww = new NotaAdd();
            ww.ShowInTaskbar = false;
            ww.Owner = Application.Current.MainWindow;
            ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ww.idrow = TX_Docum.Tag.ToString();
            ww.Modulo = modulo;
            ww.ShowDialog();

            if (ww.actualizo == true) getList(TX_Docum.Tag.ToString(), modulo);
        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool flag = false;
                string query = "";
                for (int i = 0; i < list.Items.Count; i++)
                {
                    ContentPresenter c = (ContentPresenter)list.ItemContainerGenerator.ContainerFromItem(list.Items[i]);
                    ToggleButton tb = c.ContentTemplate.FindName("btnYourButtonName", c) as ToggleButton;
                    { }

                    if (tb.IsChecked.Value)
                    {
                        query += "delete incab_notas where idrow='" + tb.Tag + "';";
                        flag = true;
                    }
                }
                if (flag == true)
                {
                    if (MessageBox.Show("Usted desea eliminar las notas seleccionadas?", "Eliminar Notas", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                        {
                            MessageBox.Show("Eliminacion exitosa", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            getList(TX_Docum.Tag.ToString(), modulo);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("seleccione las notas que desea eliminar", "Opcion", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("ERROR AL ELIMINAR");
            }
        }








    }
}
