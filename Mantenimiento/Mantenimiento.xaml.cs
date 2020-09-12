using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Sia.PublicarPnt(9675,"Mantenimiento");
    /// Sia.TabU(9675);
    public partial class Mantenimiento : UserControl
    {
        dynamic SiaWin;
        dynamic tabitem;
        int idemp = 0;
        string cnEmp = "";

        public Mantenimiento(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            tabitem = tabitem1;
            tabitem.MultiTab = true;
            if (tabitem.idemp > 0) idemp = tabitem.idemp;
            if (tabitem.idemp <= 0) idemp = SiaWin._BusinessId;
            LoadConfig();
            CargarEmpresas();
        }

        public void CargarEmpresas()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select database_nam,businessname  from business where (select Seg_AccProjectBusiness.Access from Seg_AccProjectBusiness where GroupId = " + SiaWin._UserGroup.ToString() + "  and ProjectId = " + SiaWin._ProyectId.ToString() + " and Access = 1 and Business.BusinessId = Seg_AccProjectBusiness.BusinessId)= 1");
            DataTable empresas = SiaWin.Func.SqlDT(sb.ToString(), "Empresas", 0);
            comboBoxEmpresas.ItemsSource = empresas.DefaultView;
        }

        private void LoadConfig()
        {
            try
            {

                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                tabitem.Title = "Mantenimiento";
                TabControl1.SelectedIndex = 0;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                MessageBox.Show("aqui88");


            }
        }


        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {

            tabitem.Cerrar(0);
        }
        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                if (comboBoxEmpresas.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione una o mas empresas", "filtro", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }


                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                GridConfiguracion.IsEnabled = false;
                sfBusyIndicator.IsBusy = true;
                GridMantenimiento.ItemsSource = null;
                BtnEjecutar.IsEnabled = false;


                string empresa = comboBoxEmpresas.SelectedValue.ToString();
                int execute = Convert.ToInt32(ChExecute.IsChecked);


                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(execute, empresa, source.Token), source.Token);
                await slowTask;

                BtnEjecutar.IsEnabled = true;
                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {

                    GridMantenimiento.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                    TabControl1.SelectedIndex = 2;
                    TabControl1.SelectedIndex = 1;

                }

                this.sfBusyIndicator.IsBusy = false;
                GridConfiguracion.IsEnabled = true;
            }
            catch (Exception ex)
            {
                this.Opacity = 1;
                MessageBox.Show("aqui 2.1" + ex);

            }
        }

        private DataSet LoadData(int exe, string empresas, CancellationToken cancellationToken)
        {

            try
            {

                SqlConnection con1 = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("BusinessMaintenance", con1);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Databases", empresas);
                cmd.Parameters.AddWithValue("@ejecutar", exe);
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;                
                da.Fill(ds);
                con1.Close();

                return ds;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }
      










    }
}
