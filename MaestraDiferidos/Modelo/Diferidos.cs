using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MaestraDiferidos.Modelo
{
    public class Diferidos : INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string property = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        #region validation

        public string Error { get { return ""; } }
        private string ValidationCodDif()
        {
            string result = null;
            if (string.IsNullOrEmpty(this.cod_dif))
                result = "el campo (Codigo) es requerido";
            else if (this.cod_dif.Length > 3)
                result = "el campo (Codigo) no puede ser mayor a 3";
            return result;
        }

        private string ValidationNomDif()
        {
            string result = null;
            if (string.IsNullOrEmpty(this.nom_dif))
                result = "el campo (Nombre) es requerido";
            else if (this.nom_dif.Length > 50)
                result = "el campo (Codigo) no puede ser mayor a 50";
            return result;
        }

        private string ValidationCtaDif()
        {
            string result = null;
            if (this.cta_dif.Length > 15)
                result = "el campo (Cuenta de diferido) no puede ser mayor a 15";            
            return result;
        }

        private string ValidationCtaAmo()
        {
            string result = null;
            if (this.cta_amo.Length > 15)
                result = "el campo (Cuenta de amortizacion) no puede ser mayor a 15";
            return result;
        }

        #endregion

       
        private ObservableCollection<Relacion> relacion = new ObservableCollection<Relacion>();
        public ObservableCollection<Relacion> Relacion
        {
            get { return relacion; }
            set { relacion = value; OnPropertyChanged("relacion"); }
        }


        public bool IsValid()
        {
            var _cod_dif = this.ValidationCodDif();
            var _nom_dif = this.ValidationNomDif();
            var _cta_dif = this.ValidationCtaDif();
            var _cta_amo = this.ValidationCtaAmo();
            
            var result =
                _cod_dif == null &&
                _nom_dif == null &&
                _cta_dif == null &&
                _cta_amo == null;
            return result;
        }

        public string this[string columnName]
        {
            get
            {
                String errorMessage = String.Empty;

                switch (columnName)
                {
                    case "cod_dif":
                        errorMessage = this.ValidationCodDif();
                        break;
                    case "nom_dif":
                        errorMessage = this.ValidationNomDif();
                        break;
                    case "cta_dif":
                        errorMessage = ValidationCtaDif();
                        break;
                    case "cta_amo":
                        errorMessage = ValidationCtaAmo();
                        break;
                }
                return errorMessage;
            }
        }


        int _idrow = -1;
        public int idrow { get { return _idrow; } set { _idrow = value; OnPropertyChanged(); } }

        string _cod_dif = "";
        public string cod_dif { get { return _cod_dif; } set { _cod_dif = value; OnPropertyChanged(); } }

        string _nom_dif = "";
        public string nom_dif { get { return _nom_dif; } set { _nom_dif = value; OnPropertyChanged(); } }

        double _valor;
        public double  valor { get { return _valor; } set { _valor = value; OnPropertyChanged(); } }

        string _cta_dif = "";
        public string cta_dif { get { return _cta_dif; } set { _cta_dif = value; OnPropertyChanged(); } }

        string _cta_amo = "";
        public string cta_amo { get { return _cta_amo; } set { _cta_amo = value; OnPropertyChanged(); } }

        string _cod_ter = "";
        public string cod_ter { get { return _cod_ter; } set { _cod_ter = value; OnPropertyChanged(); } }

        string _observ = "";
        public string observ { get { return _observ; } set { _observ = value; OnPropertyChanged(); } }


    }
}
