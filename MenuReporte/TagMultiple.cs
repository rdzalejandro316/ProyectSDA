using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MenuReporte
{
    public class TagMultiple
    {
        public string Id_Row { get; set; }
        public string NamePnt { get; set; }
        public string TipoPnt { get; set; }
        public bool IsRep { get; set; }
        public string urlRep { get; set; }
        public int Id_screen { get; set; }                     
        public string typePnt { get; set; }
        public int idserver { get; set; }
        public string serverIp { get; set; }
        public string userServer { get; set; }
        public string userServerPass { get; set; }



        /* 
          type_item = tipo de item del menu 1 es padre 2 es hijo y 3 es hijo del hijo
          id_parm = 1 si es reporte 0 no es reporte
          reporte = direccion del reporte en las carpetas
          typePnt = 0 no hace nada 1 es empotrado a la pantalla de reporte 2 es un user control de siasoft y 3 es windows

         
         
         */



    }


}
