using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MaestraDiferidos.Modelo
{
    public class Relacion : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string property = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        string _cod_dif = "";
        public string cod_dif { get { return _cod_dif; } set { _cod_dif = value; OnPropertyChanged(); } }

        DateTime _fec_adq = DateTime.Now;
        public DateTime fec_adq { get { return _fec_adq; } set { _fec_adq = value; OnPropertyChanged(); } }

        decimal _cos_his = 0;
        public decimal cos_his { get { return _cos_his; } set { _cos_his = value; OnPropertyChanged(); } }

        DateTime _fec_ini = DateTime.Now;
        public DateTime fec_ini { get { return _fec_ini; } set { _fec_ini = value; OnPropertyChanged(); } }

        DateTime _fec_fin = DateTime.Now;
        public DateTime fec_fin { get { return _fec_fin; } set { _fec_fin = value; OnPropertyChanged(); } }

        string _cod_cco = "";
        public string cod_cco { get { return _cod_cco; } set { _cod_cco = value; OnPropertyChanged(); } }

        decimal _valor = 0;
        public decimal valor { get { return _valor; } set { _valor = value; OnPropertyChanged(); } }

        decimal _cuotas = 0;
        public decimal cuotas { get { return _cuotas; } set { _cuotas = value; OnPropertyChanged(); } }

        bool _estado = false;
        public bool estado { get { return _estado; } set { _estado = value; OnPropertyChanged(); } }

        string _poliza = "";
        public string poliza { get { return _poliza; } set { _poliza = value; OnPropertyChanged(); } }



    }
}
