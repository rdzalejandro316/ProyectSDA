using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
    //    Sia.PublicarPnt(9644,"Bases_Menor_Cuantia");
    //    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9644,"Bases_Menor_Cuantia");
    //    ww.ShowInTaskbar = false;
    //    ww.Owner = Application.Current.MainWindow;
    //    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //    ww.ShowDialog();
    public partial class Bases_Menor_Cuantia : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public Bases_Menor_Cuantia()
        {
            InitializeComponent();
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
                this.Title = "Bases Menor Cuantia" + cod_empresa + "-" + nomempresa;
                cnEmp = SiaWin.Func.DatosEmp(idemp);
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        public void loadConsulta()
        {
            DataTable dt = SiaWin.Func.SqlDT("select idrow,año,b_mca,b_mcd,convert(varchar,fecha,103) as fecha from afbases_mc", "Existencia", idemp);
            GridConsulta.ItemsSource = dt.Rows.Count > 0 ? dt.DefaultView : null;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime per = Convert.ToDateTime(Txdate.Value);
                string periodo = per.Year.ToString();

                DataTable dt = SiaWin.Func.SqlDT("select * from afbases_mc where año='" + periodo+ "' ", "Existencia", idemp);

                if (dt.Rows.Count > 0)
                {
                    MessageBox.Show("Año existente verifique", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                else
                {                    
                    string query = "insert into afbases_mc (año,b_mca,b_mcd) values ('" + periodo + "',0,0);";
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
                double b_mca = Convert.ToDouble(row["b_mca"]);
                double b_mcd = Convert.ToDouble(row["b_mcd"]);
                string fecha = row["fecha"].ToString();
                string idrow = row["idrow"].ToString();
                

                string query = "";

                GridColumn colum = ((SfDataGrid)sender).CurrentColumn as GridColumn;
               
                if (colum.MappingName == "b_mca")
                    query += "update afbases_mc set  b_mca="+ b_mca + " where idrow='"+idrow+"'  ";
                if (colum.MappingName == "b_mcd")    
                    query += "update afbases_mc set  b_mcd=" + b_mcd + " where idrow='" + idrow + "'  ";


                if (colum.MappingName == "fecha")
                {
                    DateTime fs; string format = "dd/MM/yyyy";

                    if (DateTime.TryParseExact(fecha, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out fs) == false)
                    {
                        MessageBox.Show("lo que introdujo en el campo 'fecha ini' no es una fecha por favor verifique el formato dela fecha es dd/mm/yyyy ", "alert", MessageBoxButton.OK, MessageBoxImage.Stop);
                        row["fecha"] = "";
                        return;
                    }
                    else
                    {
                        query += "update afbases_mc set  fecha='" + fecha + "' where idrow='" + idrow + "'  ";                        
                        if (SiaWin.Func.SqlCRUD(query, idemp) == false){ MessageBox.Show("error al actualizar"); }
                    }
                }
                else
                {                    
                    if (SiaWin.Func.SqlCRUD(query, idemp) == false) { MessageBox.Show("error al actualizar"); }
                }


            }
            catch (Exception w)
            {
                MessageBox.Show("error en dataGrid_CurrentCellEndEdit:" + w);
            }
        }




    }
}
