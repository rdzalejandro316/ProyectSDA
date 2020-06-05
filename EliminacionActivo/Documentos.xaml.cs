using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;

namespace EliminacionActivo
{
    public partial class Documentos : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public string activo = "";

        public Documentos()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SiaWin = Application.Current.MainWindow;
                idemp = SiaWin._BusinessId;
                Tx_activo.Text = activo;
                consulta(activo);
            }
            catch (Exception w)
            {
                MessageBox.Show("errro en el load:" + w);
            }

        }


        public async void consulta(string code)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            sfBusyIndicator.IsBusy = true;
            string codigo = code;

            var slowTask = Task<DataTable>.Factory.StartNew(() => LoadData(codigo, source.Token), source.Token);
            await slowTask;

            if (((DataTable)slowTask.Result).Rows.Count > 0)
            {
                GridConfig.ItemsSource = ((DataTable)slowTask.Result).DefaultView;
            }
            else
            {
                MessageBox.Show("no hay moviminentos", "Alerta", MessageBoxButton.OK, MessageBoxImage.Warning);
                GridConfig.ItemsSource = null;
            }

            sfBusyIndicator.IsBusy = false;
        }


        private DataTable LoadData(string cod_act, CancellationToken cancellationToken)
        {
            try
            {
                string query = "select Afcab_doc.idreg,Afcab_doc.cod_trn,Afmae_trn.nom_trn,Afcab_doc.num_trn,convert(varchar,Afcab_doc.fec_trn,103) as fec_trn,Afcab_doc.des_mov ";
                query += "from Afcue_doc ";
                query += "inner join Afcab_doc on Afcab_doc.idreg = Afcue_doc.idregcab ";
                query += "inner join Afmae_trn on Afcab_doc.cod_trn = Afmae_trn.cod_trn ";
                query += "where cod_act = '" + cod_act + "' ";
                query += "group by Afcab_doc.idreg,Afcab_doc.cod_trn,Afmae_trn.nom_trn,Afcab_doc.num_trn,Afcab_doc.fec_trn,Afcab_doc.des_mov ";

                System.Data.DataTable dt = SiaWin.Func.SqlDT(query, "tabla", idemp);
                return dt;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }

        }

        private void BtnDetalle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)GridConfig.SelectedItems[0];
                if (row == null) return;
                int idreg = Convert.ToInt32(row["idreg"]);

                if (idreg <= 0) return;
                SiaWin.TabTrn(0, idemp, true, idreg, 8, WinModal: true);
            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir el detalle:" + w);
            }
        }







    }
}
