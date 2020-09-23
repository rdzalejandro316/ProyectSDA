using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
using System.Windows.Shapes;

namespace CreacionLinGruSubGrProducto
{
    public partial class AddNew : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        int moduloid = 2;

        public bool linea = false;
        public bool grupo = false;
        public bool subgrupo = false;
        public bool edicion = false;
        public bool actualizargrilla = false;

        public AddNew()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            pantalla();
            LoadConfig();
        }

        void pantalla()
        {
            this.MinWidth = 500;
            this.MinHeight = 400;
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
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadConfig();
                if (linea)
                {
                    this.Title = "Creacion y Edicion de Linea";
                    TxLinea.IsEnabled = edicion ? false : true;
                    TxLinea.Background = edicion ? Brushes.WhiteSmoke : Brushes.White;
                    TxGrupo.IsEnabled = false; TxGrupo.Background = Brushes.WhiteSmoke;
                    TxSubGrupo.IsEnabled = false; TxSubGrupo.Background = Brushes.WhiteSmoke;
                    TxNombre.IsEnabled = true;
                }

                if (grupo)
                {
                    this.Title = "Creacion y Edicion de Grupos";
                    TxLinea.IsEnabled = false; TxLinea.Background = Brushes.WhiteSmoke;
                    TxGrupo.IsEnabled = edicion ? false : true;
                    TxGrupo.Background = edicion ? Brushes.WhiteSmoke : Brushes.White;
                    TxSubGrupo.IsEnabled = false; TxSubGrupo.Background = Brushes.WhiteSmoke;
                    TxNombre.IsEnabled = true;
                }

                if (subgrupo)
                {
                    this.Title = "Creacion y Edicion de SubGrupos";
                    TxLinea.IsEnabled = false; TxLinea.Background = Brushes.WhiteSmoke;
                    TxGrupo.IsEnabled = false; TxGrupo.Background = Brushes.WhiteSmoke;
                    TxSubGrupo.IsEnabled = edicion ? false : true;
                    TxSubGrupo.Background = edicion ? Brushes.WhiteSmoke : Brushes.White;
                    TxNombre.IsEnabled = true;
                }

                BtnSave.Content = edicion ? "Modificar" : "Guardar";

            }
            catch (Exception w)
            {
                MessageBox.Show("erro al cargar load:" + w);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region validacion

                if (linea)
                {
                    if (string.IsNullOrEmpty(TxLinea.Text) || string.IsNullOrEmpty(TxNombre.Text))
                    {
                        MessageBox.Show("el campo Linea y Nombre tienen que estar llenos", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                }

                if (grupo)
                {
                    if (string.IsNullOrEmpty(TxGrupo.Text) || string.IsNullOrEmpty(TxNombre.Text))
                    {
                        MessageBox.Show("el campo Grupo y Nombre tienen que estar llenos", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                }

                if (subgrupo)
                {
                    if (string.IsNullOrEmpty(TxSubGrupo.Text) || string.IsNullOrEmpty(TxNombre.Text))
                    {
                        MessageBox.Show("el campo SubGrupo y Nombre tienen que estar llenos", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                }

                #endregion

                string select = "";
                if (linea) select = "select * from inmae_tip where cod_tip='" + TxLinea.Text + "' ";
                if (grupo) select = "select * from InMae_gru where cod_gru='" + TxGrupo.Text + "' ";
                if (subgrupo) select = "select * from InMae_sgr where cod_sgr='" + TxSubGrupo.Text + "' ";

                DataTable dt = SiaWin.Func.SqlDT(select, "existencias", idemp);
                string query = "";
                if (dt.Rows.Count > 0)
                {
                    if (linea) query = "update inmae_tip set nom_tip='" + TxNombre.Text + "' where cod_tip='" + TxLinea.Text + "';";
                    if (grupo) query = "update inmae_gru set nom_gru='" + TxNombre.Text + "' where cod_gru='" + TxGrupo.Text + "' and cod_tip='" + TxLinea.Text + "';";
                    if (subgrupo) query = "update inmae_sgr set nom_sgr='" + TxNombre.Text + "' where cod_sgr='" + TxSubGrupo.Text + "' and cod_gru='" + TxGrupo.Text + "' and cod_tip='" + TxLinea.Text + "';";
                    if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                    {
                        string mess = "";
                        if (linea) mess = "actualizo exitosamente la linea:" + TxLinea.Text;
                        if (grupo) mess = "actualizo exitosamente el grupo:" + TxGrupo.Text;
                        if (subgrupo) mess = "actualizo exitosamente el sub grupo:" + TxSubGrupo.Text;
                        SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, moduloid, -1, -9, mess, "");
                        MessageBox.Show("Se actualizo exitosamente", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        actualizargrilla = true;
                        this.Close();
                    }
                }
                else
                {
                    if (linea) query = "insert into inmae_tip (cod_tip,nom_tip) values ('" + TxLinea.Text + "','" + TxNombre.Text + "');";
                    if (grupo) query = "insert into inmae_gru (cod_gru,nom_gru,cod_tip) values ('" + TxGrupo.Text + "','" + TxNombre.Text + "','" + TxLinea.Text + "');";
                    if (subgrupo) query = "insert into inmae_sgr  (cod_sgr,nom_sgr,cod_tip,cod_gru) values ('" + TxSubGrupo.Text + "','" + TxNombre.Text + "','" + TxLinea.Text + "','" + TxGrupo.Text + "');";

                    if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                    {
                        string mess = "";
                        if (linea) mess = "inserto exitosamente la linea:" + TxLinea.Text;
                        if (grupo) mess = "inserto exitosamente el grupo:" + TxGrupo.Text;
                        if (subgrupo) mess = "inserto exitosamente el sub grupo:" + TxSubGrupo.Text;

                        SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, moduloid, -1, -9, mess, "");                        
                        MessageBox.Show("Se inserto exitosamente ", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        actualizargrilla = true;
                        this.Close();
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al guardar:" + w);
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



    }
}
