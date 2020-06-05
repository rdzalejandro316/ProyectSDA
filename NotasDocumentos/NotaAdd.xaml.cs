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
        public string Modulo = "";
        public bool actualizo = false;

        public NotaAdd()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
        }


        public string table(string modul)
        {
            string tabla = "";
            switch (modul)
            {
                case "INV":tabla = "incab_notas"; break;
                case "CON":tabla = "cocab_notas"; break;
                case "ACF":tabla = "afcab_notas"; break;
                case "MMA":tabla = "Mmcab_notas"; break;
                case "NII":tabla = "NIcab_notas"; break;
            }
            return tabla;
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

                string tabla = table(Modulo);


                string query = "insert into "+ tabla + " (idrowcab,fecha,usuario,nota,title) values (" + idrow + ",getdate(),'" + SiaWin._UserId + "','" + TX_descr.Text + "','" + Tx_tit.Text + "')";
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
