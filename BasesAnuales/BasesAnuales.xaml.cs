using Syncfusion.UI.Xaml.Grid;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{

    //    Sia.PublicarPnt(9645,"BasesAnuales");
    //    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9645,"BasesAnuales");
    //    ww.ShowInTaskbar = false;
    //    ww.Owner = Application.Current.MainWindow;
    //    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //    ww.ShowDialog();

    public partial class BasesAnuales : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        public BasesAnuales()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            LoadConfig();
            loadConsulta();
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
                this.Title = "Bases Anuales " + cod_empresa + "-" + nomempresa;
                cnEmp = SiaWin.Func.DatosEmp(idemp);
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        public void loadConsulta()
        {
            DataTable dt = SiaWin.Func.SqlDT("select idrow,año,smlv,uvt,n_smlv,n_uvt from afbases_a", "Existencia", idemp);
            GridConsulta.ItemsSource = dt.Rows.Count > 0 ? dt.DefaultView : null;
        }


        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime per = Convert.ToDateTime(Txdate.Value);
                string periodo = per.Year.ToString();

                DataTable dt = SiaWin.Func.SqlDT("select * from afbases_a where año='" + periodo + "' ", "Existencia", idemp);

                if (dt.Rows.Count > 0)
                {
                    MessageBox.Show("Año existente verifique", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                else
                {

                    string query = "insert into afbases_a (año,smlv,uvt,n_smlv,n_uvt) values ('" + periodo + "',0,0,0,0);";
                    if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                    {
                        loadConsulta();
                    }
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al adicionar:" + w);
            }
        }

        private void GridConsulta_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)GridConsulta.SelectedItems[0];
                double smlv = Convert.ToDouble(row["smlv"]);
                double uvt = Convert.ToDouble(row["uvt"]);
                double n_smlv = Convert.ToDouble(row["n_smlv"]);
                double n_uvt = Convert.ToDouble(row["n_uvt"]);
                string idrow = row["idrow"].ToString();

                string query = "";

                GridColumn colum = ((SfDataGrid)sender).CurrentColumn as GridColumn;

                if (colum.MappingName == "smlv")
                    query += "update afbases_a set  smlv=" + smlv + " where idrow='" + idrow + "'  ";
                if (colum.MappingName == "uvt")
                    query += "update afbases_a set  uvt=" + uvt + " where idrow='" + idrow + "'  ";
                if (colum.MappingName == "n_smlv")
                    query += "update afbases_a set  n_smlv=" + n_smlv + " where idrow='" + idrow + "'  ";
                if (colum.MappingName == "n_uvt")
                    query += "update afbases_a set n_uvt=" + n_uvt + " where idrow='" + idrow + "'  ";

                if (SiaWin.Func.SqlCRUD(query, idemp) == false) { MessageBox.Show("error al actualizar"); }

            }
            catch (Exception w)
            {
                MessageBox.Show("error en dataGrid_CurrentCellEndEdit:" + w);
            }
        }




    }
}
