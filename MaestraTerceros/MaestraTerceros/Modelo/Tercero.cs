using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
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
        private string ValidationDir2()
        {
            string result = null;
            if (string.IsNullOrEmpty(this.dir2))
                result = "el campo (Dirreccion 2) es requerido";
            else if (this.dir2.Length > 100)
                result = "el campo (Dirreccion 2) no puede ser mayor a 100";
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
            if (string.IsNullOrEmpty(this.cel))
                result = "el campo (Celular) es requerido";
            else if (this.cel.Length > 50)
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
        private string ValidationDepa()
        {
            string result = null;
            if (string.IsNullOrEmpty(this.depa))
                result = "el campo (Departamento) es requerido";
            else if (this.depa.Length > 100)
                result = "el campo (Departamento) no puede ser mayor a 100";
            return result;
        }
        private string ValidationPais()
        {
            string result = null;
            if (string.IsNullOrEmpty(this.pais))
                result = "el campo (Pais) es requerido";
            else if (this.pais.Length > 50)
                result = "el campo (Pais) no puede ser mayor a 50";
            return result;
        }
        private string ValidationCodDep()
        {
            string result = null;
            if (string.IsNullOrEmpty(this.cod_depa))
                result = "el campo (Codigo de Departamento) es requerido";
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
        private string ValidationConta()
        {
            string result = null;
            if (string.IsNullOrEmpty(this.conta))
                result = "el campo (Contac/Secret) es requerido";
            else if (this.conta.Length > 100)
                result = "el campo (Contac/Secret) no puede ser mayor a 100";
            return result;
        }
        private string ValidationObserv()
        {
            string result = null;
            if (observ.Length > 200)
                result = "el campo (Observacion) no puede ser mayor a 200 caracteres";
            return result;
        }
        private string ValidationCont_cxc()
        {
            string result = null;
            if (cont_cxc.Length > 150)
                result = "el campo (Contacto CxC) no puede ser mayor a 200 caracteres";
            return result;
        }        
        
        public bool IsValid()
        {
            var _cod_ter = this.ValidationCodTer();
            var _nom_ter = this.ValidationNomTer();
            var _repres = ValidationRepres();
            var _dir1 = ValidationDir1();
            var _dir2 = ValidationDir2();
            var _tel1 = ValidationTel1(); 
            var _cel = ValidationCel();
            var _email = ValidationEmail();
            var _ciudad = ValidationCiudad();
            var _depa = ValidationDepa();
            var _pais = ValidationPais();
            var _cod_ciu = ValidationCodCiu();
            var _cod_dep = ValidationCodDep();            
            var _cod_pais = ValidationCodPais();
            var _conta = ValidationConta();         
            var _observ = ValidationObserv();
            var _cont_cxc = ValidationCont_cxc();

            var result =
                _cod_ter == null &&
                _nom_ter == null &&
                _repres == null &&
                _dir1 == null &&
                _dir2 == null &&
                _tel1 == null &&
                _cel == null &&
                _email == null &&
                _ciudad == null &&
                _depa == null &&
                _pais == null &&
                _cod_ciu == null &&
                _cod_dep == null &&
                _cod_pais == null &&
                _conta == null &&
                _observ == null &&
                _cont_cxc == null;                
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
                    case "dir2":
                        errorMessage = ValidationDir2();
                        break;
                    case "tel1":
                        errorMessage = ValidationTel1();
                        break;
                    case "cel":
                        errorMessage = ValidationCel();
                        break;
                    case "email":
                        errorMessage = ValidationEmail();
                        break;
                    case "ciudad":
                        errorMessage = ValidationCiudad();
                        break;
                    case "depa":
                        errorMessage = ValidationDepa();
                        break;
                    case "pais":
                        errorMessage = ValidationPais();
                        break;
                    case "cod_ciu":
                        errorMessage = ValidationCodCiu();
                        break;
                    case "cod_depa":
                        errorMessage = ValidationCodDep();
                        break;
                    case "cod_pais":
                        errorMessage = ValidationCodPais();
                        break;
                    case "conta":
                        errorMessage = ValidationConta();
                        break;                    
                    case "observ":
                        errorMessage = ValidationObserv();
                        break;
                    case "cont_cxc":
                        errorMessage = ValidationCont_cxc();
                        break;                     
                }
                return errorMessage;
            }
        }

        #endregion


        public DataTable vendedores { get; set; }
        public DataTable zona { get; set; }
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
        string _dir2;
        public string dir2 { get { return _dir2; } set { _dir2 = value; OnPropertyChanged("dir2"); } }
        string _tel1;
        public string tel1 { get { return _tel1; } set { _tel1 = value; OnPropertyChanged("tel1"); } }
        string _cel;
        public string cel { get { return _cel; } set { _cel = value; OnPropertyChanged("cel"); } }
        string _email;
        public string email { get { return _email; } set { _email = value; OnPropertyChanged("email"); } }
        string _ciudad;
        public string ciudad { get { return _ciudad; } set { _ciudad = value; OnPropertyChanged("ciudad"); } }
        string _depa;
        public string depa { get { return _depa; } set { _depa = value; OnPropertyChanged("depa"); } }
        string _pais;
        public string pais { get { return _pais; } set { _pais = value; OnPropertyChanged("pais"); } }
        string _conta;
        public string conta { get { return _conta; } set { _conta = value; OnPropertyChanged("conta"); } }
        Int16 _estado = -1;
        public Int16 estado { get { return _estado; } set { _estado = value; OnPropertyChanged("estado"); } }


        Int16 _tip_prv = -1;
        public Int16 tip_prv { get { return _tip_prv; } set { _tip_prv = value; OnPropertyChanged("tip_prv"); } } // Tipo Regimen

        Int16 _ind_ret = 0;
        public Int16 ind_ret { get { return _ind_ret; } set { _ind_ret = value; OnPropertyChanged("ind_ret"); } } //retefuente
        Int16 _ret_iva = 0;
        public Int16 ret_iva { get { return _ret_iva; } set { _ret_iva = value; OnPropertyChanged("ret_iva"); } } //ret_iva
        Int16 _ret_ica = 0;
        public Int16 ret_ica { get { return _ret_ica; } set { _ret_ica = value; OnPropertyChanged("ret_ica"); } } //ret_ica

        Int16 _rtiva = 0;
        public Int16 rtiva { get { return _rtiva; } set { _rtiva = value; OnPropertyChanged("rtiva"); } } //ret_iva
        Int16 _rtica = -1;
        public Int16 rtica { get { return _rtica; } set { _rtica = value; OnPropertyChanged("rtica"); } } //ret_ica

        Int16 _aut_ret = -1;
        public Int16 aut_ret { get { return _aut_ret; } set { _aut_ret = value; OnPropertyChanged("aut_ret"); } } //autoretenedor
        Int16 _ind_rete = -1;
        public Int16 ind_rete { get { return _ind_rete; } set { _ind_rete = value; OnPropertyChanged("ind_rete"); } } //fijar retencion
        Int16 _ind_iva = 1;
        public Int16 ind_iva { get { return _ind_iva; } set { _ind_iva = value; OnPropertyChanged("ind_iva"); } } //Maneja iva 
        decimal _por_ica;
        public decimal por_ica { get { return _por_ica; } set { _por_ica = value; OnPropertyChanged("por_ica"); } } //% ica
        string _cod_ban;
        public string cod_ban { get { return _cod_ban; } set { _cod_ban = value; OnPropertyChanged("cod_ban"); } }
        string _cta;
        public string cta { get { return _cta; } set { _cta = value; OnPropertyChanged("cta"); } }
        bool _ind_suc = false;
        public bool ind_suc { get { return _ind_suc; } set { _ind_suc = value; OnPropertyChanged("ind_suc"); } }// Maneja sucursales
        bool _i_cupocc = false;
        public bool i_cupocc { get { return _i_cupocc; } set { _i_cupocc = value; OnPropertyChanged("i_cupocc"); } }// Controlar credito cliente i_cupocc
        decimal _cupo_cxc;
        public decimal cupo_cxc { get { return _cupo_cxc; } set { _cupo_cxc = value; OnPropertyChanged("cupo_cxc"); } } //cupo credito cupo_cxc
        bool _i_cupocp = false;
        public bool i_cupocp { get { return _i_cupocp; } set { _i_cupocp = value; OnPropertyChanged("i_cupocp"); } } //Controla credito proveedor i_cupocp
        decimal _cupo_cxp;
        public decimal cupo_cxp { get { return _cupo_cxp; } set { _cupo_cxp = value; OnPropertyChanged("cupo_cxp"); } }//Cupo credito proveedor cupo_cxp
        Int16 _bloqueo = -1;
        public Int16 bloqueo { get { return _bloqueo; } set { _bloqueo = value; OnPropertyChanged("bloqueo"); } }// Bloquear en CXC
        Int16 _lista_prec = -1;
        public Int16 lista_prec { get { return _lista_prec; } set { _lista_prec = value; OnPropertyChanged("lista_prec"); } }// Lista de precio  
        Int16 _ind_mayor = -1;
        public Int16 ind_mayor { get { return _ind_mayor; } set { _ind_mayor = value; OnPropertyChanged("ind_mayor"); } }////IND_MAYOR  ind_mayor
        string _cod_zona;
        public string cod_zona { get { return _cod_zona; } set { _cod_zona = value; OnPropertyChanged("cod_zona"); } }
        string _cod_ven;
        public string cod_ven { get { return _cod_ven; } set { _cod_ven = value; OnPropertyChanged("cod_ven"); } }
        Int16 _dia_plaz = 0;
        public Int16 dia_plaz { get { return _dia_plaz; } set { _dia_plaz = value; OnPropertyChanged("dia_plaz"); } }// Dias plazo//Int
        decimal _por_des;
        public decimal por_des { get { return _por_des; } set { _por_des = value; OnPropertyChanged("por_des"); } } //% Desc
        string _cod_can;
        public string cod_can { get { return _cod_can; } set { _cod_can = value; OnPropertyChanged("cod_can"); } }
        string _tdoc = "";
        public string tdoc { get { return _tdoc; } set { _tdoc = value; OnPropertyChanged("tdoc"); } }// Tipo documento // tipo de dato char en sql
        Int16 _tip_pers = -1;
        public Int16 tip_pers { get { return _tip_pers; } set { _tip_pers = value; OnPropertyChanged("tip_pers"); } }// Tipo personas

        string _cod_depa;
        public string cod_depa { get { return _cod_depa; } set { _cod_depa = value; OnPropertyChanged("cod_depa"); } }
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
        string _razon_soc;
        public string razon_soc { get { return _razon_soc; } set { _razon_soc = value; OnPropertyChanged("razon_soc"); } }//Razon social
        string _dir_comer;
        public string dir_comer { get { return _dir_comer; } set { _dir_comer = value; OnPropertyChanged("dir_comer"); } } ////Direccion razon social dir
        string _observ;
        public string observ { get { return _observ; } set { _observ = value; OnPropertyChanged("observ"); } }//observaciones
        string _cont_cxc;
        public string cont_cxc { get { return _cont_cxc; } set { _cont_cxc = value; OnPropertyChanged("cont_cxc"); } }//Contacto cobro cont_cxc
        string _fec_cump = DateTime.Now.Date.ToString("dd/MM/yyyy");
        public string fec_cump { get { return _fec_cump; } set { _fec_cump = value; OnPropertyChanged("fec_cump"); } }//FEC_CUMP fecha de cumpleaños fec_cump

        string _fec_ing = DateTime.Now.Date.ToString("dd/MM/yyyy");
        public string fec_ing { get { return _fec_ing; } set { _fec_ing = value; OnPropertyChanged("fec_ing"); } }//FEC_CUMP fecha de cumpleaños fec_cump

        Int16 _uni_fra = 0;
        public Int16 uni_fra { get { return _uni_fra; } set { _uni_fra = value; OnPropertyChanged("uni_fra"); } } //tipo de procuto
        Int16 _esp_gab = 0;
        public Int16 esp_gab { get { return _esp_gab; } set { _esp_gab = value; OnPropertyChanged("esp_gab"); } } //precio especial gabriel
        string _email_fe;
        public string email_fe { get { return _email_fe; } set { _email_fe = value; OnPropertyChanged("email_fe"); } }
        string _fec_act = DateTime.Now.Date.ToString("dd/MM/yyyy");
        public string fec_act { get { return _fec_act; } set { _fec_act = value; OnPropertyChanged("fec_act"); } }






    }
}
