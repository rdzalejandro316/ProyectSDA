using System;
using System.Collections.Generic;
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

namespace NotasDocumentos
{

    public partial class NotaAdd : Window
    {

        dynamic SiaWin;
        int idemp = 0;

        public string idrow = "";
        public string cod_trn = "";
        public string num_trn = "";
        public string Modulo = "";
        public bool actualizo = false;

        public NotaAdd()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
        }

        
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string val = validacion();
                if (!string.IsNullOrEmpty(val))
                {
                    MessageBox.Show(val);
                    return;
                }
                

                string query = "insert into cab_notas (moduloid,idrowcab,cod_trn,num_trn,fecha,usuario,nota,title) values ('" + Modulo + "'," + idrow + ",'" + cod_trn + "','" + num_trn + "',getdate(),'" + SiaWin._UserId + "','" + TX_descr.Text + "','" + Tx_tit.Text + "')";

                if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                {
                    actualizo = true;
                    this.Close();
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al guardar:" + w);
            }
        }

        public string validacion()
        {
            string val = string.Empty;
            if (string.IsNullOrEmpty(Tx_tit.Text)) val = "ingrese un titulo";
            if (string.IsNullOrEmpty(TX_descr.Text)) val = "ingrese la nota a digitar";
            return val;
        }





    }
}
