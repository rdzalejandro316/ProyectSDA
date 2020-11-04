using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MaestraTerceros
{
    class Tercero : INotifyPropertyChanged, IDataErrorInfo
    {        

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        #region validation        
        public string Error{get { return "";}}
        private string ValidationCodTer()
        {
            string result = null;
            if (string.IsNullOrEmpty(this.cod_ter))
                result = "el campo (NIT/CC) es requerido";
            else if (this.cod_ter.Length > 15)
                result = "el campo (NIT/CC) no puede ser mayor a 15";
            return result;
        }
        private string ValidationNomTer()
        {
            string result = null;
            if (string.IsNullOrEmpty(this.nom_ter))
                result = "el campo (Nombre) es requerido";
            else if (this.nom_ter.Length > 100)
                result = "el campo (Nombre) no puede ser mayor a 100";
            return result;
        }
        private string ValidationRepres()
        {
            string result = null;
            if (string.IsNullOrEmpty(this.repres))
                result = "el campo (Representante) es requerido";
            else if (this.repres.Length > 100)
                result = "el campo (Representante) no puede ser mayor a 15";
            return result;
        }
        private string ValidationDir1()
        {
            string result = null;
            if (string.IsNullOrEmpty(this.dir1))
                result = "el campo (Dirreccion 1) es requerido";
            else if (this.dir1.Length > 100)
                result = "el campo (Dirreccion 1) no puede ser mayor a 100";
            return result;
        }
        
        private string ValidationTel1()
        {
            string result = null;
            if (string.IsNullOrEmpty(this.tel1))
                result = "el campo (Telefono) es requerido";
            else if (this.tel1.Length > 50)
                result = "el campo (Telefono) no puede ser mayor a 50";
            return result;
        }
        private string ValidationCel()
        {
            string result = null;
            if (string.IsNullOrEmpty(this.cel1))
                result = "el campo (Celular) es requerido";
            else if (this.cel1.Length > 50)
                result = "el campo (Celular) no puede ser mayor a 50";
            return result;
        }
        private string ValidationEmail()
        {
            string result = null;
            if (string.IsNullOrEmpty(this.email))
                result = "el campo (Email) es requerido";
            else if (this.email.Length > 50)
                result = "el campo (Email) no puede ser mayor a 50";
            return result;
        }
        private string ValidationCiudad()
        {
            string result = null;
            if (string.IsNullOrEmpty(this.ciudad))
                result = "el campo (Ciudad) es requerido";
            else if (this.ciudad.Length > 30)
                result = "el campo (Ciudad) no puede ser mayor a 30";
            return result;
        }        
        
        private string ValidationCodCiu()
        {
            string result = null;
            if (string.IsNullOrEmpty(this.cod_ciu))
                result = "el campo (Codigo de Ciudad) es requerido";
            return result;
        }
        private string ValidationCodPais()
        {
            string result = null;
            if (string.IsNullOrEmpty(this.cod_pais))
                result = "el campo (Codigo de Pais) es requerido";
            return result;
        }
        
        

        public bool IsValid()
        {
            var _cod_ter = this.ValidationCodTer();
            var _nom_ter = this.ValidationNomTer();
            var _repres = ValidationRepres();
            var _dir1 = ValidationDir1();            
            var _tel1 = ValidationTel1(); 
            var _cel = ValidationCel();
            var _email = ValidationEmail();
            var _ciudad = ValidationCiudad();            
            var _cod_ciu = ValidationCodCiu();           
            var _cod_pais = ValidationCodPais();            

            var result =
                _cod_ter == null &&
                _nom_ter == null &&
                _repres == null &&
                _dir1 == null &&                
                _tel1 == null &&
                _cel == null &&
                _email == null &&
                _ciudad == null &&                
                _cod_ciu == null &&
                _cod_pais == null;                
            return result;
        }
        public string this[string columnName]
        {
            get
            {
                String errorMessage = String.Empty;

                switch (columnName)
                {
                    case "cod_ter":
                        errorMessage = this.ValidationCodTer();
                        break;
                    case "nom_ter":
                        errorMessage = this.ValidationNomTer();
                        break;
                    case "repres":
                        errorMessage = ValidationRepres();
                        break;
                    case "dir1":
                        errorMessage = ValidationDir1();
                        break;
                    case "tel1":
                        errorMessage = ValidationTel1();
                        break;
                    case "cel1":
                        errorMessage = ValidationCel();
                        break;
                    case "email":
                        errorMessage = ValidationEmail();
                        break;
                    case "ciudad":
                        errorMessage = ValidationCiudad();
                        break;
                    case "cod_ciu":
                        errorMessage = ValidationCodCiu();
                        break;
                    case "cod_pais":
                        errorMessage = ValidationCodPais();
                        break;
                }
                return errorMessage;
            }
        }

        #endregion
              
        public DataTable tdocm { get; set; }

        int _idrow = -1;
        public int idrow { get { return _idrow; } set { _idrow = value; OnPropertyChanged("idrow"); } }

        string _cod_ter = "";
        public string cod_ter { get { return _cod_ter; } set { _cod_ter = value; OnPropertyChanged("cod_ter"); } }

        string _dv;
        public string dv { get { return _dv; } set { _dv = value; OnPropertyChanged("dv"); } }

        string _nom_ter = "";
        public string nom_ter { get { return _nom_ter; } set { _nom_ter = value; OnPropertyChanged("nom_ter"); } }

        int _clasific = -1;
        public int clasific { get { return _clasific; } set { _clasific = value; OnPropertyChanged("clasific"); } }

        string _repres;
        public string repres { get { return _repres; } set { _repres = value; OnPropertyChanged("repres"); } }

        string _dir1;
        public string dir1 { get { return _dir1; } set { _dir1 = value; OnPropertyChanged("dir1"); } }
        
        string _tel1;
        public string tel1 { get { return _tel1; } set { _tel1 = value; OnPropertyChanged("tel1"); } }

        string _cel1;
        public string cel1 { get { return _cel1; } set { _cel1 = value; OnPropertyChanged("cel1"); } }

        string _email;
        public string email { get { return _email; } set { _email = value; OnPropertyChanged("email"); } }

        string _ciudad;
        public string ciudad { get { return _ciudad; } set { _ciudad = value; OnPropertyChanged("ciudad"); } }
                
        int _estado = -1;
        public int estado { get { return _estado; } set { _estado = value; OnPropertyChanged("estado"); } }

        int _tip_prv = -1;
        public int tip_prv { get { return _tip_prv; } set { _tip_prv = value; OnPropertyChanged("tip_prv"); } } // Tipo Regimen
                
        int _aut_ret = -1;
        public int aut_ret { get { return _aut_ret; } set { _aut_ret = value; OnPropertyChanged("aut_ret"); } } //autoretenedor

        int _ind_ret = -1;
        public int ind_ret { get { return _ind_ret; } set { _ind_ret = value; OnPropertyChanged("ind_ret"); } } //fijar retencion
                        
        int _bloqueo = -1;
        public int bloqueo { get { return _bloqueo; } set { _bloqueo = value; OnPropertyChanged("bloqueo"); } }// Bloquear en CXC
                
        int _dia_plaz = 0;
        public int dia_plaz { get { return _dia_plaz; } set { _dia_plaz = value; OnPropertyChanged("dia_plaz"); } }// Dias plazo//Int
                
        string _tdoc = "";
        public string tdoc { get { return _tdoc; } set { _tdoc = value; OnPropertyChanged("tdoc"); } }// Tipo documento // tipo de dato char en sql

        int _tip_pers = -1;
        public int tip_pers { get { return _tip_pers; } set { _tip_pers = value; OnPropertyChanged("tip_pers"); } }// Tipo personas

        
        string _cod_ciu;
        public string cod_ciu { get { return _cod_ciu; } set { _cod_ciu = value; OnPropertyChanged("cod_ciu"); } }

        string _cod_pais;
        public string cod_pais { get { return _cod_pais; } set { _cod_pais = value; OnPropertyChanged("cod_pais"); } }

        string _apl1;
        public string apl1 { get { return _apl1; } set { _apl1 = value; OnPropertyChanged("apl1"); } }//primer apellido
        string _apl2;
        public string apl2 { get { return _apl2; } set { _apl2 = value; OnPropertyChanged("apl2"); } }//segundo apellido
        string _nom1;
        public string nom1 { get { return _nom1; } set { _nom1 = value; OnPropertyChanged("nom1"); } }//primer nombre
        string _nom2;
        public string nom2 { get { return _nom2; } set { _nom2 = value; OnPropertyChanged("nom2"); } }//segundo nombre
        
        string _raz;
        public string raz { get { return _raz; } set { _raz = value; OnPropertyChanged("raz"); } }

        string _dir;
        public string dir { get { return _dir; } set { _dir = value; OnPropertyChanged("dir"); } }

        string _fec_ing = DateTime.Now.Date.ToString("dd/MM/yyyy");
        public string fec_ing { get { return _fec_ing; } set { _fec_ing = value; OnPropertyChanged("fec_ing"); } }//FEC_CUMP fecha de cumpleaños fec_cump
               

        string _fec_act = DateTime.Now.Date.ToString("dd/MM/yyyy");
        public string fec_act { get { return _fec_act; } set { _fec_act = value; OnPropertyChanged("fec_act"); } }

        bool _ind_recip = false;
        public bool ind_recip { get { return _ind_recip; } set { _ind_recip = value; OnPropertyChanged("ind_recip"); } } //operaciones reciprocras

        string _cod_recip;
        public string cod_recip { get { return _cod_recip; } set { _cod_recip = value; OnPropertyChanged("cod_recip"); } }
        



    }
}
