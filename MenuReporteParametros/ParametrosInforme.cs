using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MenuReporteParametros
{
    class ParametrosInforme : INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string property = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        #region validation        
        public string Error { get { return ""; } }
        public bool IsValid()
        {
            var context = new ValidationContext(this, null, null);
            var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            Validator.TryValidateObject(this, context, results, true);
            return results.Count > 0 ? true : false;
        }
        public string this[string columnName]
        {
            get
            {

                var validationResults = new List<ValidationResult>();

                if (Validator.TryValidateProperty
                    (
                        GetType().GetProperty(columnName).GetValue(this),
                        new ValidationContext(this)
                        { MemberName = columnName }
                        , validationResults
                    )
                   )
                    return null;

                return validationResults.First().ErrorMessage;
            }
        }

        #endregion


        private int _idrow_rep;
        public int idrow_rep { get { return _idrow_rep; } set { _idrow_rep = value; OnPropertyChanged();} }


        private string _parameter = "";
        [Required]
        [StringLength(50, ErrorMessage = "el campo (Parametro) no puede ser mayor a 50")]
        public string parameter { get { return _parameter; } set { _parameter = value; OnPropertyChanged(); } }

        private bool _isValid = false;
        public bool isValid { get { return _isValid; } set { _isValid = value; OnPropertyChanged(); } }

        private bool _isTable = false;
        public bool isTable { get { return _isTable; } set { _isTable = value; OnPropertyChanged(); } }


        private bool _isCombo = false;
        public bool isCombo { get { return _isCombo; } set { _isCombo = value; OnPropertyChanged(); } }

        private bool _isMultiValue = false;
        public bool isMultiValue { get { return _isMultiValue; } set { _isMultiValue = value; OnPropertyChanged(); } }



        private string _nameMaster = "";
        [StringLength(50, ErrorMessage = "el campo (Name Master) no puede ser mayor a 50")]
        public string nameMaster { get { return _nameMaster; } set { _nameMaster = value; OnPropertyChanged(); } }

        private string _tabla = "";
        [StringLength(50, ErrorMessage = "el campo (tabla) no puede ser mayor a 50")]
        public string tabla { get { return _tabla; } set { _tabla = value; OnPropertyChanged(); } }

        private string _cod_tbl = "";
        [StringLength(50, ErrorMessage = "el campo (codigo de tabla) no puede ser mayor a 50")]
        public string cod_tbl { get { return _cod_tbl; } set { _cod_tbl = value; OnPropertyChanged(); } }

        private string _nom_tbl = "";
        [StringLength(50, ErrorMessage = "el campo (nombre de tabla) no puede ser mayor a 50")]
        public string nom_tbl { get { return _nom_tbl; } set { _nom_tbl = value; OnPropertyChanged(); } }

        private string _whereMaster = "";
        public string whereMaster { get { return _whereMaster; } set { _whereMaster = value; OnPropertyChanged(); } }

        private string _orderMaster = "";
        public string orderMaster { get { return _orderMaster; } set { _orderMaster = value; OnPropertyChanged(); } }

        private string _columns = "";
        public string columns { get { return _columns; } set { _columns = value; OnPropertyChanged(); } }

        private string _dataDifferent = "";
        public string dataDifferent { get { return _dataDifferent; } set { _dataDifferent = value; OnPropertyChanged(); } }
        

        private bool _viewAll = false;
        public bool viewAll { get { return _viewAll; } set { _viewAll = value; OnPropertyChanged(); } }

        private bool _isBusiness = false;
        public bool isBusiness { get { return _isBusiness; } set { _isBusiness = value; OnPropertyChanged(); } }



    }
}


