using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

    //importacion de 

    public class DocumentCont : IDataErrorInfo
    {
        public string Cod_trn { get; set; }
        public string Num_trn { get; set; }
        public string Fec_trn { get; set; }
        public string Cuenta { get; set; }
        public string Cod_ter { get; set; }
        public string Des_mov { get; set; }
        public string Doc_ref { get; set; }
        public decimal Base { get; set; }
        public decimal Debito { get; set; }
        public decimal Credito { get; set; }
        public string Doc_Cruce { get; set; }        


        [Display(AutoGenerateField = false)]
        public string Error { get; set; }

        public string this[string columnName]
        {
            get
            {
                ImportacionSDAcancelacion principal = new ImportacionSDAcancelacion();
                if (columnName == "Cod_ref")
                {
                    var validacion = principal.GetTableVal(Cod_ref);

                    if (validacion.Item1 == false)
                    {
                        Error = "la referencia : " + this.Cod_ref + " no existe";
                        Cos_usd_ref = 0;
                        Vrunc_ref = 0;
                        Val_ref_ref = 0;
                        Vr_intem_ref = 0;
                        Val_ref2_ref = 0;
                        return "la referencia : " + this.Cod_ref + " no existe";
                    }
                    else
                    {
                        Cos_usd_ref = validacion.Item2;
                        Vrunc_ref = validacion.Item3;
                        Val_ref_ref = validacion.Item4;
                        Vr_intem_ref = validacion.Item5;
                        Val_ref2_ref = validacion.Item6;
                    }
                }

                return string.Empty;
            }
        }

        public Referencia(string cod_trn,string num_trn ,string fec_trn ,
        public string Cuenta { get; set; }
        public string Cod_ter { get; set; }
        public string Des_mov { get; set; }
        public string Doc_ref { get; set; }
        public decimal Base { get; set; }
        public decimal Debito { get; set; }
        public decimal Credito { get; set; }
        public string Doc_Cruce { get; set; }        
            )
        {
            Cod_ref = cod_ref;
            Cos_usd = cos_usd;
            Vrunc = vrunc;
            Val_ref = val_ref;
            Vr_intem = vr_intem;
            Val_ref2 = val_ref2;
            Estado = estado;
        }
    }


    public partial class ImportacionSDAcancelacion : Window
    {
        public ImportacionSDAcancelacion()
        {
            InitializeComponent();
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnImportar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btnexportar_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
