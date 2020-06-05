using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.UI.Xaml.ScrollAxis;
using System;
using System.Collections.Generic;
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

namespace FoxPasarSql
{

    public partial class Config : Window
    {
        dynamic SiaWin;
        int idemp = 0;
        string cnEmp = "";

        public Config()
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
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
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
                cargar();
            }
            catch (Exception w)
            {
                MessageBox.Show(":::" + w);
            }

        }

        public void cargar()
        {
            try
            {
                GridConfig.ItemsSource = null;
                string select = "select  rtrim(idrow) as idrow,rtrim(tablaFox) as tablaFox,rtrim(tablaSQL) as tablaSQL,";
                select += "rtrim(idModu) as idModu,rtrim(selecTable) as selecTable,RTRIM(inserTable) as inserTable,";
                select += "rtrim(selectCamp) as selectCamp from sql_fox";
                DataTable dt = SiaWin.Func.SqlDT(select, "tables", 0);
                GridConfig.ItemsSource = dt.DefaultView;
            }
            catch (Exception w)
            {
                MessageBox.Show("ERROR al cargar:" + w);
            }
        }

        private void GridConfig_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
        {
            try
            {              

                if (GridConfig.View.IsAddingNew == true)
                {
                    var reflector = this.GridConfig.View.GetPropertyAccessProvider();
                    var rowData = GridConfig.GetRecordAtRowIndex(e.RowColumnIndex.RowIndex);
                    var tablaFox = reflector.GetValue(rowData, "tablaFox");
                    var tablaSQL = reflector.GetValue(rowData, "tablaSQL");
                    var idModu = reflector.GetValue(rowData, "idModu");
                    var selecTable = reflector.GetValue(rowData, "selecTable");
                    var inserTable = reflector.GetValue(rowData, "inserTable");
                    var selectCamp = reflector.GetValue(rowData, "selectCamp");

                    

                    string insert = "insert into sql_fox (tablaFox,tablaSQL,idModu,selecTable,inserTable,selectCamp) values " +
                        "('" + tablaFox + "','" + tablaSQL + "','" + idModu + "','" + selecTable + "','" + inserTable + "','" + selectCamp + "')";


                    if (SiaWin.Func.SqlCRUD(insert, 0) == false)
                        MessageBox.Show("no se pudo insertar");


                    GridConfig.BeginInit();
                    GridConfig.EndInit();
                    GridConfig.UpdateLayout();

                    this.GridConfig.MoveCurrentCell(new RowColumnIndex(e.RowColumnIndex.RowIndex, e.RowColumnIndex.ColumnIndex));

                    CargarID(e.RowColumnIndex.RowIndex);
                    //cargar();


                }
                else
                {
                    
                    DataRowView row = (DataRowView)GridConfig.SelectedItems[0];
                    string id = row["idrow"].ToString();
                    string tablaFox = row["tablaFox"].ToString().Trim();
                    string tablaSQL = row["tablaSQL"].ToString().Trim();
                    string idModu = row["idModu"].ToString().Trim();
                    string selecTable = row["selecTable"].ToString().Trim();
                    string inserTable = row["inserTable"].ToString().Trim();
                    string selectCamp = row["selectCamp"].ToString().Trim();

                    string update = "update sql_fox set tablaFox='" + tablaFox + "',tablaSQL='" + tablaSQL + "',idModu='" + idModu + "'," +
                        "selecTable='" + selecTable + "',inserTable='" + inserTable + "',selectCamp='" + selectCamp + "' where idrow='" + id + "';";

                    if (SiaWin.Func.SqlCRUD(update, 0) == false)
                        MessageBox.Show("no se pudo actualizar");
                }


            }
            catch (Exception w)
            {
                MessageBox.Show("error en la actualizacion:" + w);
            }
        }


        public void CargarID(int ind)
        {
            DataTable dt = SiaWin.Func.SqlDT("select MAX(idrow) as id from sql_fox", "tables", 0);
            string idMax = dt.Rows[0]["id"].ToString();

            var reflector = this.GridConfig.View.GetPropertyAccessProvider();
            var rowData = GridConfig.GetRecordAtRowIndex(ind);
            reflector.SetValue(rowData, "idrow", idMax);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)GridConfig.SelectedItems[0];
                string tablaFox = row["tablaFox"].ToString().Trim();
                string id = row["idrow"].ToString();

                if (MessageBox.Show("deseas borrar la tabla " + tablaFox + "?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (SiaWin.Func.SqlCRUD("delete sql_fox where idrow='" + id + "'", 0) == false) MessageBox.Show("no se pudo eliminar");

                    cargar();
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al eliminar:" + w);
            }
        }





    }
}
