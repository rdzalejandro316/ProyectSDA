using MaestraTerceros;
using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;  
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9680,"MaestraTerceros");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9680,"MaestraTerceros");    
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();   

    public partial class MaestraTerceros : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        Tercero MTer = new Tercero();
        Tercero _MTer = new Tercero();

        public string cod_ter = "";
        string tabla = "comae_ter";
        string codigo = "cod_ter";
        string nombre = "nom_ter";
        string idrow = "idrow";

        public MaestraTerceros()
        {
            InitializeComponent();
            pantalla();
            //TimeSpan()

            
        }

        void pantalla()
        {
            this.MinWidth = 1200;
            this.MinHeight = 650;
        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Maestra de terceros " + cod_empresa + "-" + nomempresa;

                //llena combos
                MTer.vendedores = LlenaCombo("select rtrim(cod_mer) as cod_mer,rtrim(nom_mer) as nom_mer from InMae_mer  order by nom_mer");
                MTer.zona = LlenaCombo("select cod_zona,rtrim(Nom_zona) as nom_zona from InMae_zona  order by Nom_zona");
                MTer.tdocm = LlenaCombo("select cod_tdo,rtrim(cod_tdo)+'('+rtrim(nom_tdo)+')' as nom_tdo from InMae_tdoc  order by cod_tdo");

                //seguridad
                ////ParamAcc 1=lRun,2=lNew,3=lEdit,4=lDelete,5=lSearch,5=Renum,6=lPrint,7=lExport,8=lOpc1,9=lOpc2,10=lOpc3
                string pk = idemp.ToString() + "-2";

                BtnBuscar.Visibility = SiaWin.Func.Acceso(SiaWin._UserGroup, SiaWin._ProyectId, 6, idemp, 1, "lSearch") == true ?
                    Visibility.Visible : Visibility.Collapsed;

                BtnNuevo.Visibility = SiaWin.Func.Acceso(SiaWin._UserGroup, SiaWin._ProyectId, 6, idemp, 1, "lNew") == true ?
                     Visibility.Visible : Visibility.Collapsed;

                BtnEditar.Visibility = SiaWin.Func.Acceso(SiaWin._UserGroup, SiaWin._ProyectId, 6, idemp, 1, "lEdit") == true ?
                    Visibility.Visible : Visibility.Collapsed;

                BtnEliminar.Visibility = SiaWin.Func.Acceso(SiaWin._UserGroup, SiaWin._ProyectId, 6, idemp, 1, "lDelete") == true ?
                        Visibility.Visible : Visibility.Collapsed;


                string llave = idemp.ToString() + "-" + 1;
                bool flagGBimp = SiaWin.Acc.ContainsKey(llave + "-221") == true ? true : false;
                GBimpuesto.IsEnabled = flagGBimp;
                if (!flagGBimp) GBimpuesto.Header = "Informacion Impuestos Cliente (BLOQUEADO)";

                bool flaginfcom = SiaWin.Acc.ContainsKey(llave + "-222") == true ? true : false;
                GBinfcom.IsEnabled = flaginfcom;
                if (!flaginfcom) GBinfcom.Header = "Informacion Comercial (BLOQUEADO)";
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SiaWin = System.Windows.Application.Current.MainWindow;                
                if (idemp <= 0) idemp = SiaWin._BusinessId;



                LoadConfig();
                this.DataContext = MTer;

                if (string.IsNullOrEmpty(cod_ter)) return;

                DataTable dt = SiaWin.Func.SqlDT("select * From comae_ter where cod_ter='" + cod_ter + "'", "tercero", idemp);
                if (dt.Rows.Count > 0)
                {
                    int id = (int)dt.Rows[0]["idrow"];
                    ActualizaCampos(id, string.Empty);
                    bloquear(false);
                    editdel(true);
                }
                else
                {
                    ClearClas();
                    activecontrol(false, "Guardar");
                    bloquear(true);
                    MTer.cod_ter = cod_ter;
                    TXname.Focus();
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar:" + w);
            }
        }




        DataTable LlenaCombo(string _Sql)
        {
            DataTable dt = SiaWin.Func.SqlDT(_Sql, "tabla", idemp);
            return dt;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {

                switch (e.Key)
                {
                    case Key.F1:
                        BtnBuscar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        break;
                    case Key.F2:
                        BtnNuevo.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        break;
                    case Key.F3:
                        BtnEditar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        break;
                    case Key.F4:
                        BtnEliminar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        break;
                    case Key.F5:
                        BtnSave.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        break;
                    case Key.F6:
                        BtnCancel.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        break;
                    case Key.Escape:
                        BtnCancel.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        break;
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al tomar atajo:" + w);
            }
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dynamic winb = SiaWin.WindowBuscar(tabla, codigo, nombre, codigo, idrow, "maestra de tereros", cnEmp, false, "", idEmp: idemp);
                winb.ShowInTaskbar = false;
                winb.Owner = Application.Current.MainWindow;
                winb.Height = 400;
                winb.ShowDialog();
                int id = winb.IdRowReturn;
                string code = winb.Codigo;
                string nom = winb.Nombre;
                //winb = null;
                if (id > 0)
                {
                    ActualizaCampos(id, string.Empty);
                    bloquear(false);
                    editdel(true);
                }
                if (string.IsNullOrEmpty(code)) e.Handled = false;
                e.Handled = true;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al buscar:" + 2);
            }
        }

        void ActualizaCampos(int Id, string _Sql)
        {
            try
            {
                SqlDataReader dr;

                dr = _Sql == string.Empty ?
                    SiaWin.Func.SqlDR("SELECT * FROM Comae_ter where idrow=" + Id.ToString(), idemp) :
                    dr = SiaWin.Func.SqlDR(_Sql, idemp);


                while (dr.Read())
                {
                    MTer.idrow = Convert.ToInt32(dr["Idrow"]);
                    MTer.cod_ter = dr["cod_ter"].ToString().Trim();
                    MTer.dv = dr["dv"].ToString().Trim();
                    MTer.nom_ter = dr["nom_ter"].ToString().Trim();

                    MTer.clasific = Convert.ToInt32(dr["clasific"]);

                    MTer.repres = dr["repres"].ToString().Trim();
                    MTer.dir1 = dr["dir1"].ToString().Trim();
                    MTer.dir2 = dr["dir2"].ToString().Trim();
                    MTer.tel1 = dr["tel1"].ToString().Trim();
                    MTer.cel = dr["cel"].ToString().Trim();
                    MTer.email = dr["email"].ToString().Trim();
                    MTer.ciudad = dr["ciudad"].ToString().Trim();
                    MTer.depa = dr["depa"].ToString().Trim();
                    MTer.pais = dr["pais"].ToString().Trim();
                    MTer.conta = dr["conta"].ToString().Trim();
                    MTer.estado = Convert.ToInt16(dr["estado"] is DBNull ? -1 : dr["estado"]);

                    MTer.fec_ing = dr["fec_ing"].ToString().Trim();
                    MTer.fec_cump = dr["fec_cump"].ToString().Trim();
                    MTer.fec_act = dr["fec_act"].ToString().Trim();

                    MTer.tip_prv = Convert.ToInt16(dr["tip_prv"] is DBNull ? -1 : dr["tip_prv"]);
                    MTer.ind_ret = Convert.ToInt16(dr["ind_ret"] is DBNull ? -1 : dr["ind_ret"]);
                    MTer.ret_iva = Convert.ToInt16(dr["ret_iva"] is DBNull ? -1 : dr["ret_iva"]);
                    MTer.ret_ica = Convert.ToInt16(dr["ret_ica"] is DBNull ? -1 : dr["ret_ica"]);

                    MTer.rtiva = Convert.ToInt16(dr["rtiva"] is DBNull ? -1 : dr["rtiva"]);
                    MTer.rtica = Convert.ToInt16(dr["rtica"] is DBNull ? -1 : dr["rtica"]);

                    MTer.aut_ret = Convert.ToInt16(dr["aut_ret"] is DBNull ? -1 : dr["aut_ret"]);
                    MTer.ind_rete = Convert.ToInt16(dr["ind_rete"] is DBNull ? -1 : dr["ind_rete"]);
                    MTer.ind_iva = Convert.ToInt16(dr["ind_iva"] is DBNull ? 1 : dr["ind_iva"]);
                    MTer.por_ica = Convert.ToDecimal(dr["por_ica"] is DBNull ? 0 : dr["por_ica"]);
                    MTer.cod_ban = dr["cod_ban"].ToString().Trim();
                    MTer.cta = dr["cta"].ToString().Trim();

                    MTer.ind_suc = Convert.ToBoolean(dr["ind_suc"] is DBNull ? 0 : dr["ind_suc"]);
                    MTer.i_cupocc = Convert.ToBoolean(dr["i_cupocc"] is DBNull ? 0 : dr["i_cupocc"]);
                    MTer.cupo_cxc = Convert.ToInt32(dr["cupo_cxc"] is DBNull ? 0 : dr["cupo_cxc"]);
                    MTer.i_cupocp = Convert.ToBoolean(dr["i_cupocp"] is DBNull ? 0 : dr["i_cupocp"]);
                    MTer.cupo_cxp = Convert.ToInt32(dr["cupo_cxp"] is DBNull ? 0 : dr["cupo_cxp"]);
                    MTer.bloqueo = Convert.ToInt16(dr["bloqueo"] is DBNull ? -1 : dr["bloqueo"]);
                    MTer.lista_prec = Convert.ToInt16(dr["lista_prec"] is DBNull ? -1 : dr["lista_prec"]);
                    MTer.ind_mayor = Convert.ToInt16(dr["ind_mayor"] is DBNull ? -1 : dr["ind_mayor"]);
                    MTer.cod_zona = dr["cod_zona"].ToString().Trim();
                    MTer.cod_ven = dr["cod_ven"].ToString().Trim();
                    MTer.dia_plaz = Convert.ToInt16(dr["dia_plaz"] is DBNull ? 0 : dr["dia_plaz"]);
                    MTer.por_des = Convert.ToInt32(dr["por_des"] is DBNull ? 0 : dr["por_des"]);
                    MTer.cod_can = dr["cod_can"].ToString().Trim();
                    MTer.tdoc = dr["tdoc"].ToString();
                    MTer.razon_soc = dr["razon_soc"].ToString().Trim();
                    MTer.apl1 = dr["apl1"].ToString().Trim();
                    MTer.apl2 = dr["apl2"].ToString().Trim();
                    MTer.nom1 = dr["nom1"].ToString().Trim();
                    MTer.nom2 = dr["nom2"].ToString().Trim();
                    MTer.tip_pers = Convert.ToInt16(dr["tip_pers"] is DBNull ? -1 : dr["tip_pers"]);
                    MTer.cod_ciu = dr["cod_ciu"].ToString().Trim();
                    MTer.cod_depa = dr["cod_depa"].ToString().Trim();
                    MTer.cod_pais = dr["cod_pais"].ToString().Trim();
                    MTer.dir_comer = dr["dir_comer"].ToString().Trim();//Direccion razon social
                    MTer.observ = dr["observ"].ToString().Trim();
                    MTer.cont_cxc = dr["cont_cxc"].ToString().Trim();//Contacto cobro                    
                    MTer.uni_fra = Convert.ToInt16(dr["uni_fra"] is DBNull ? -1 : dr["uni_fra"]);
                    MTer.esp_gab = Convert.ToInt16(dr["esp_gab"] is DBNull ? 0 : dr["esp_gab"]);
                    MTer.email_fe = dr["email_fe"].ToString().Trim();
                }
                dr.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (System.Exception _error)
            {
                MessageBox.Show(_error.Message);
            }
        }

        public void activecontrol(bool control, string boton)
        {
            CtrlA.Visibility = control ? Visibility.Visible : Visibility.Hidden;
            CtrlB.Visibility = !control ? Visibility.Visible : Visibility.Hidden;
            BtnSave.Content = boton;
        }
        public void editdel(bool control)
        {
            BtnEditar.IsEnabled = control;
            BtnEliminar.IsEnabled = control;
        }
        public void bloquear(bool flag)
        {
            PanelA.IsEnabled = flag;
            PanelB.IsEnabled = flag;
            PanelC.IsEnabled = flag;
        }
        void ClearClas()
        {
            MTer.idrow = -1;
            MTer.cod_ter = string.Empty;
            MTer.dv = string.Empty;
            MTer.nom_ter = string.Empty;
            MTer.clasific = -1;
            MTer.repres = string.Empty;
            MTer.dir1 = string.Empty;
            MTer.dir2 = string.Empty;
            MTer.tel1 = string.Empty;
            MTer.cel = string.Empty;
            MTer.email = string.Empty;
            MTer.ciudad = string.Empty;
            MTer.depa = string.Empty;
            MTer.pais = string.Empty;
            MTer.conta = string.Empty;
            MTer.estado = 1;
            MTer.fec_ing = DateTime.Now.Date.ToString("dd/MM/yyyy");
            MTer.tip_prv = -1;
            MTer.ind_ret = 0;
            MTer.ret_iva = 0;
            MTer.ret_ica = 0;
            MTer.rtiva = 0;
            MTer.rtica = 0;
            MTer.aut_ret = 0;
            MTer.ind_rete = 0;
            MTer.ind_iva = 1;
            MTer.por_ica = 0;
            MTer.cod_ban = string.Empty;
            MTer.cta = string.Empty;
            MTer.ind_suc = false;
            MTer.i_cupocc = false;
            MTer.cupo_cxc = 0;
            MTer.i_cupocp = false;
            MTer.cupo_cxp = 0;
            MTer.bloqueo = -1;
            MTer.lista_prec = -1;
            MTer.ind_mayor = -1;
            MTer.cod_zona = string.Empty;
            MTer.cod_ven = string.Empty;
            MTer.dia_plaz = 0;
            MTer.por_des = 0;
            MTer.cod_can = string.Empty;
            MTer.tdoc = string.Empty;
            MTer.tip_pers = -1;
            MTer.cod_ciu = string.Empty;
            MTer.cod_depa = string.Empty;
            MTer.cod_pais = string.Empty;
            MTer.apl1 = string.Empty;
            MTer.apl2 = string.Empty;
            MTer.nom1 = string.Empty;
            MTer.nom2 = string.Empty;
            MTer.razon_soc = string.Empty;
            MTer.dir_comer = string.Empty;
            MTer.observ = string.Empty;
            MTer.cont_cxc = string.Empty;
            MTer.fec_cump = DateTime.Now.Date.ToString("dd/MM/yyyy");
            MTer.uni_fra = 0;
            MTer.esp_gab = 0;
            MTer.email_fe = string.Empty;
            MTer.fec_act = DateTime.Now.Date.ToString("dd/MM/yyyy");
        }
        void ClearClasOld()
        {
            _MTer.idrow = -1;
            _MTer.cod_ter = string.Empty;
            _MTer.dv = string.Empty;
            _MTer.nom_ter = string.Empty;
            _MTer.clasific = -1;
            _MTer.repres = string.Empty;
            _MTer.dir1 = string.Empty;
            _MTer.dir2 = string.Empty;
            _MTer.tel1 = string.Empty;
            _MTer.cel = string.Empty;
            _MTer.email = string.Empty;
            _MTer.ciudad = string.Empty;
            _MTer.depa = string.Empty;
            _MTer.pais = string.Empty;
            _MTer.conta = string.Empty;
            _MTer.estado = 1;
            _MTer.fec_ing = DateTime.Now.Date.ToString("dd/MM/yyyy");
            _MTer.tip_prv = -1;
            _MTer.ind_ret = 0;
            _MTer.ret_iva = 0;
            _MTer.ret_ica = 0;
            _MTer.rtiva = 0;
            _MTer.rtica = 0;
            _MTer.aut_ret = 0;
            _MTer.ind_rete = 0;
            _MTer.ind_iva = 1;
            _MTer.por_ica = 0;
            _MTer.cod_ban = string.Empty;
            _MTer.cta = string.Empty;
            _MTer.ind_suc = false;
            _MTer.i_cupocc = false;
            _MTer.cupo_cxc = 0;
            _MTer.i_cupocp = false;
            _MTer.cupo_cxp = 0;
            _MTer.bloqueo = -1;
            _MTer.lista_prec = -1;
            _MTer.ind_mayor = -1;
            _MTer.cod_zona = string.Empty;
            _MTer.cod_ven = string.Empty;
            _MTer.dia_plaz = 0;
            _MTer.por_des = 0;
            _MTer.cod_can = string.Empty;
            _MTer.tdoc = string.Empty;
            _MTer.tip_pers = -1;
            _MTer.cod_ciu = string.Empty;
            _MTer.cod_depa = string.Empty;
            _MTer.cod_pais = string.Empty;
            _MTer.apl1 = string.Empty;
            _MTer.apl2 = string.Empty;
            _MTer.nom1 = string.Empty;
            _MTer.nom2 = string.Empty;
            _MTer.razon_soc = string.Empty;
            _MTer.dir_comer = string.Empty;
            _MTer.observ = string.Empty;
            _MTer.cont_cxc = string.Empty;
            _MTer.fec_cump = DateTime.Now.Date.ToString("dd/MM/yyyy");
            _MTer.uni_fra = 0;
            _MTer.esp_gab = 0;
            _MTer.email_fe = string.Empty;
            _MTer.fec_act = DateTime.Now.Date.ToString("dd/MM/yyyy");
        }
        void Clone()
        {
            _MTer.idrow = MTer.idrow;
            _MTer.cod_ter = MTer.cod_ter;
            _MTer.dv = MTer.dv;
            _MTer.nom_ter = MTer.nom_ter;
            _MTer.clasific = MTer.clasific;
            _MTer.repres = MTer.repres;
            _MTer.dir1 = MTer.dir1;
            _MTer.dir2 = MTer.dir2;
            _MTer.tel1 = MTer.tel1;
            _MTer.cel = MTer.cel;
            _MTer.email = MTer.email;
            _MTer.ciudad = MTer.ciudad;
            _MTer.depa = MTer.depa;
            _MTer.pais = MTer.pais;
            _MTer.conta = MTer.conta;
            _MTer.estado = MTer.estado;
            _MTer.fec_ing = MTer.fec_ing;//fecha ingreso
            _MTer.tip_prv = MTer.tip_prv;
            _MTer.ind_ret = MTer.ind_ret;
            _MTer.ret_iva = MTer.ret_iva;
            _MTer.ret_ica = MTer.ret_ica;

            _MTer.rtiva = MTer.rtiva;
            _MTer.rtica = MTer.rtica;

            _MTer.aut_ret = MTer.aut_ret;
            _MTer.ind_rete = MTer.ind_rete;
            _MTer.ind_iva = MTer.ind_iva;
            _MTer.por_ica = MTer.por_ica;
            _MTer.cod_ban = MTer.cod_ban;
            _MTer.cta = string.Empty;
            //_MTer.cta_ban = MTer.cta_ban;// Cuenta
            _MTer.ind_suc = MTer.ind_suc;
            _MTer.i_cupocc = MTer.i_cupocc;
            _MTer.cupo_cxc = MTer.cupo_cxc;
            _MTer.i_cupocp = MTer.i_cupocp;//Controla credito proveedor
            _MTer.cupo_cxp = MTer.cupo_cxp;//Cupo credito proveedor
            _MTer.bloqueo = MTer.bloqueo;
            _MTer.lista_prec = MTer.lista_prec;
            _MTer.ind_mayor = MTer.ind_mayor;
            _MTer.cod_zona = MTer.cod_zona;
            _MTer.cod_ven = MTer.cod_ven;
            _MTer.dia_plaz = MTer.dia_plaz;
            _MTer.por_des = MTer.por_des;
            _MTer.cod_can = MTer.cod_can;
            _MTer.tdoc = MTer.tdoc;
            _MTer.tip_pers = MTer.tip_pers;
            _MTer.cod_ciu = MTer.cod_ciu;
            _MTer.cod_pais = MTer.cod_pais;
            _MTer.apl1 = MTer.apl1;
            _MTer.apl2 = MTer.apl2;
            _MTer.nom1 = MTer.nom1;
            _MTer.nom2 = MTer.nom2;
            _MTer.razon_soc = MTer.razon_soc;
            _MTer.dir_comer = MTer.dir_comer;//Direccion razon social
            _MTer.observ = MTer.observ;
            _MTer.cont_cxc = MTer.cont_cxc;//Contacto cobro
            _MTer.fec_cump = MTer.fec_cump;//FEC_CUMP fecha de cumpleaños
            _MTer.uni_fra = MTer.uni_fra;
            _MTer.esp_gab = MTer.esp_gab;
            _MTer.email_fe = MTer.email_fe;
            _MTer.fec_act = MTer.fec_act;//FEC_CUMP fecha de cumpleaños
        }

        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            ClearClas();
            activecontrol(false, "Guardar");
            bloquear(true);
            txter.Focus();
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            bloquear(true);
            activecontrol(false, "Modificar");
            Clone();
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                MessageBoxResult result = MessageBox.Show("Usted desea eliminar el registro....?", "Confirmacion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    if (SiaWin.Func.DeleteInMaestra(MTer.idrow, "Comae_ter", "idrow", idemp)) ClearClas();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {                

                if (BtnSave.Content.ToString() == "Modificar")
                {
                    if (!MTer.IsValid())
                    {
                        MessageBox.Show("no se puede modificar por que faltan algunos campos que son requeridos", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }

                    int query = Modificar();
                    if (query > 0)
                    {
                        ComparaDatos();
                        SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idemp, 0, 0, 0, "actulizo exitosamente el tercero" + MTer.cod_ter, "");                        
                        MessageBox.Show("actualizo exitosamente la informacion del tercero:" + MTer.cod_ter, "alerta", MessageBoxButton.OK, MessageBoxImage.Information);
                        ClearClas();
                        editdel(true);
                        bloquear(false);
                        activecontrol(true, "");
                    }
                }
                else
                {
                    if (!MTer.IsValid())
                    {
                        MessageBox.Show("no se puede guardar por que faltan algunos campos que son requeridos", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                    
                    
                    int query = Insertar();
                    if (query > 0)
                    {
                        SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idemp, 0, 0, 0, "Inserto exitosamente el tercero" + MTer.cod_ter, "");                        
                        MessageBox.Show("inserto exitosamente el tercero:" + MTer.cod_ter, "alerta", MessageBoxButton.OK, MessageBoxImage.Information);
                        ClearClas();
                        editdel(false);
                        bloquear(false);
                        activecontrol(true, "");
                    }

                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al realizar el query:" + w);
            }
        }

        int Insertar()
        {
            try
            {
                int valor = 0;
                using (SqlConnection connection = new SqlConnection(SiaWin.Func.DatosEmp(idemp)))
                {
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        try
                        {
                            cmd.CommandText = "INSERT INTO Comae_ter (cod_ter, dv, nom_ter, clasific, repres, dir1, dir2, tel1, cel, email, ciudad, depa, pais, conta, estado, fec_ing, tip_prv, ind_ret, ret_iva, ret_ica, rtiva, rtica, aut_ret, ind_rete, ind_iva, por_ica, cod_ban, cta, ind_suc, i_cupocc, cupo_cxc, i_cupocp, cupo_cxp, bloqueo, lista_prec, ind_mayor, cod_zona, cod_ven, dia_plaz, por_des, cod_can, tdoc, tip_pers, cod_ciu, cod_depa,cod_pais, apl1, apl2, nom1, nom2, razon_soc, dir_comer, observ, cont_cxc, fec_cump, uni_fra, esp_gab,email_fe,fec_act) VALUES (@cod_ter,@dv,@nom_ter,@clasific,@repres,@dir1,@dir2,@tel1,@cel,@email,@ciudad,@depa,@pais,@conta,@estado,@fec_ing,@tip_prv,@ind_ret,@ret_iva,@ret_ica, @rtiva, @rtica,@aut_ret,@ind_rete,@ind_iva,@por_ica,@cod_ban,@cta,@ind_suc,@i_cupocc,@cupo_cxc,@i_cupocp,@cupo_cxp,@bloqueo,@lista_prec,@ind_mayor,@cod_zona,@cod_ven,@dia_plaz,@por_des,@cod_can,@tdoc,@tip_pers,@cod_ciu,@cod_depa,@cod_pais,@apl1,@apl2,@nom1,@nom2,@razon_soc,@dir_comer,@observ,@cont_cxc,@fec_cump, @uni_fra,@esp_gab,@email_fe,@fec_act)";
                            cmd.Parameters.AddWithValue("@cod_ter", MTer.cod_ter);
                            cmd.Parameters.AddWithValue("@dv", MTer.dv);
                            cmd.Parameters.AddWithValue("@nom_ter", MTer.nom_ter);
                            cmd.Parameters.AddWithValue("@clasific", MTer.clasific);
                            cmd.Parameters.AddWithValue("@repres", MTer.repres);
                            cmd.Parameters.AddWithValue("@dir1", MTer.dir1);
                            cmd.Parameters.AddWithValue("@dir2", MTer.dir2);
                            cmd.Parameters.AddWithValue("@tel1", MTer.tel1);
                            cmd.Parameters.AddWithValue("@cel", MTer.cel);
                            cmd.Parameters.AddWithValue("@email", MTer.email);
                            cmd.Parameters.AddWithValue("@ciudad", MTer.ciudad);
                            cmd.Parameters.AddWithValue("@depa", MTer.depa);
                            cmd.Parameters.AddWithValue("@pais", MTer.pais);
                            cmd.Parameters.AddWithValue("@conta", MTer.conta);
                            cmd.Parameters.AddWithValue("@estado", MTer.estado);

                            cmd.Parameters.AddWithValue("@fec_ing", MTer.fec_ing);
                            cmd.Parameters.AddWithValue("@fec_cump", MTer.fec_cump);
                            cmd.Parameters.AddWithValue("@fec_act", DateTime.Now.ToString("dd/MM/yyyy"));

                            cmd.Parameters.AddWithValue("@tip_prv", MTer.tip_prv);
                            cmd.Parameters.AddWithValue("@ind_ret", MTer.ind_ret);
                            cmd.Parameters.AddWithValue("@ret_iva", MTer.ret_iva);
                            cmd.Parameters.AddWithValue("@ret_ica", MTer.ret_ica);

                            cmd.Parameters.AddWithValue("@rtiva", MTer.rtiva);
                            cmd.Parameters.AddWithValue("@rtica", MTer.rtica);

                            cmd.Parameters.AddWithValue("@aut_ret", MTer.aut_ret);
                            cmd.Parameters.AddWithValue("@ind_rete", MTer.ind_rete);
                            cmd.Parameters.AddWithValue("@ind_iva", MTer.ind_iva);
                            cmd.Parameters.AddWithValue("@por_ica", MTer.por_ica);
                            cmd.Parameters.AddWithValue("@cod_ban", MTer.cod_ban);
                            cmd.Parameters.AddWithValue("@cta", MTer.cta);

                            cmd.Parameters.AddWithValue("@ind_suc", MTer.ind_suc);
                            cmd.Parameters.AddWithValue("@i_cupocc", MTer.i_cupocc);
                            cmd.Parameters.AddWithValue("@cupo_cxc", MTer.cupo_cxc);
                            cmd.Parameters.AddWithValue("@i_cupocp", MTer.i_cupocp);
                            cmd.Parameters.AddWithValue("@cupo_cxp", MTer.cupo_cxp);
                            cmd.Parameters.AddWithValue("@bloqueo", MTer.bloqueo);
                            cmd.Parameters.AddWithValue("@lista_prec", MTer.lista_prec);
                            cmd.Parameters.AddWithValue("@ind_mayor", MTer.ind_mayor);
                            cmd.Parameters.AddWithValue("@cod_zona", MTer.cod_zona);
                            cmd.Parameters.AddWithValue("@cod_ven", MTer.cod_ven);
                            cmd.Parameters.AddWithValue("@dia_plaz", MTer.dia_plaz);
                            cmd.Parameters.AddWithValue("@por_des", MTer.por_des);
                            cmd.Parameters.AddWithValue("@cod_can", MTer.cod_can);
                            cmd.Parameters.AddWithValue("@tdoc", MTer.tdoc);
                            cmd.Parameters.AddWithValue("@tip_pers", MTer.tip_pers);
                            cmd.Parameters.AddWithValue("@cod_ciu", MTer.cod_ciu);
                            cmd.Parameters.AddWithValue("@cod_depa", MTer.cod_depa);
                            cmd.Parameters.AddWithValue("@cod_pais", MTer.cod_pais);
                            cmd.Parameters.AddWithValue("@apl1", MTer.apl1);
                            cmd.Parameters.AddWithValue("@apl2", MTer.apl2);
                            cmd.Parameters.AddWithValue("@nom1", MTer.nom1);
                            cmd.Parameters.AddWithValue("@nom2", MTer.nom2);
                            cmd.Parameters.AddWithValue("@razon_soc", MTer.razon_soc);
                            cmd.Parameters.AddWithValue("@dir_comer", MTer.dir_comer);//Direccion razon social
                            cmd.Parameters.AddWithValue("@observ", MTer.observ);
                            cmd.Parameters.AddWithValue("@cont_cxc", MTer.cont_cxc);//Contacto cobro


                            cmd.Parameters.AddWithValue("@uni_fra", MTer.uni_fra);
                            cmd.Parameters.AddWithValue("@esp_gab", MTer.esp_gab);
                            cmd.Parameters.AddWithValue("@email_fe", MTer.email_fe);


                            connection.Open();
                            valor = cmd.ExecuteNonQuery();

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Error Interno Sia", MessageBoxButton.OK, MessageBoxImage.Stop);

                        }
                    }
                }
                return valor;
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Error Interno Sia", MessageBoxButton.OK, MessageBoxImage.Stop);
                return 0;
            }
        }

        int Modificar()
        {
            try
            {
                int valor = 0;
                using (SqlConnection connection = new SqlConnection(SiaWin.Func.DatosEmp(idemp)))
                {
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = "UPDATE Comae_ter SET dv=@dv, nom_ter=@nom_ter, clasific=@clasific, repres=@repres, dir1=@dir1, dir2=@dir2, tel1=@tel1, cel=@cel, email=@email, ciudad=@ciudad, depa=@depa, pais=@pais, conta=@conta, estado=@estado,fec_ing=@fec_ing, tip_prv=@tip_prv, por_des=@por_des, ind_ret=@ind_ret, ret_iva=@ret_iva, ret_ica=@ret_ica, rtiva=@rtiva, rtica=@rtica, aut_ret=@aut_ret, ind_rete=@ind_rete, ind_iva=@ind_iva, por_ica=@por_ica, ind_suc=@ind_suc, i_cupocc=@i_cupocc, cupo_cxc=@cupo_cxc, cod_zona=@cod_zona, cod_ven=@cod_ven, cod_ban=@cod_ban, cta=@cta, cod_can=@cod_can, i_cupocp=@i_cupocp, cupo_cxp=@cupo_cxp, bloqueo=@bloqueo, lista_prec=@lista_prec, ind_mayor=@ind_mayor, dia_plaz=@dia_plaz, tdoc=@tdoc, tip_pers=@tip_pers, cod_ciu=@cod_ciu, cod_depa=@cod_depa,cod_pais=@cod_pais, apl1=@apl1, apl2=@apl2, nom1=@nom1, nom2=@nom2, razon_soc=@razon_soc, dir_comer=@dir_comer, observ=@observ, cont_cxc=@cont_cxc, fec_cump=@fec_cump, uni_fra=@uni_fra,esp_gab=@esp_gab,email_fe=@email_fe,fec_act=@fec_act where idrow=" + MTer.idrow.ToString();
                        cmd.Parameters.AddWithValue("@dv", MTer.dv);
                        cmd.Parameters.AddWithValue("@nom_ter", MTer.nom_ter);
                        cmd.Parameters.AddWithValue("@clasific", MTer.clasific);
                        cmd.Parameters.AddWithValue("@repres", MTer.repres);
                        cmd.Parameters.AddWithValue("@dir1", MTer.dir1);
                        cmd.Parameters.AddWithValue("@dir2", MTer.dir2);
                        cmd.Parameters.AddWithValue("@tel1", MTer.tel1);
                        cmd.Parameters.AddWithValue("@cel", MTer.cel);
                        cmd.Parameters.AddWithValue("@email", MTer.email);
                        cmd.Parameters.AddWithValue("@ciudad", MTer.ciudad);
                        cmd.Parameters.AddWithValue("@depa", MTer.depa);
                        cmd.Parameters.AddWithValue("@pais", MTer.pais);
                        cmd.Parameters.AddWithValue("@conta", MTer.conta);
                        cmd.Parameters.AddWithValue("@estado", MTer.estado);

                        cmd.Parameters.AddWithValue("@fec_ing", MTer.fec_ing);
                        cmd.Parameters.AddWithValue("@fec_cump", MTer.fec_cump);
                        cmd.Parameters.AddWithValue("@fec_act", DateTime.Now.ToString("dd/MM/yyyy"));
                        cmd.Parameters.AddWithValue("@tip_prv", MTer.tip_prv);
                        cmd.Parameters.AddWithValue("@ind_ret", MTer.ind_ret);
                        cmd.Parameters.AddWithValue("@ret_iva", MTer.ret_iva);
                        cmd.Parameters.AddWithValue("@ret_ica", MTer.ret_ica);

                        cmd.Parameters.AddWithValue("@rtiva", MTer.rtiva);
                        cmd.Parameters.AddWithValue("@rtica", MTer.rtica);

                        cmd.Parameters.AddWithValue("@aut_ret", MTer.aut_ret);
                        cmd.Parameters.AddWithValue("@ind_rete", MTer.ind_rete);
                        cmd.Parameters.AddWithValue("@ind_iva", MTer.ind_iva);
                        cmd.Parameters.AddWithValue("@por_ica", MTer.por_ica);
                        cmd.Parameters.AddWithValue("@cod_ban", MTer.cod_ban);
                        cmd.Parameters.AddWithValue("@cta", MTer.cta);
                        //cmd.Parameters.AddWithValue("@cta_ban", MTer.cta_ban);// Cuenta
                        cmd.Parameters.AddWithValue("@ind_suc", MTer.ind_suc);
                        cmd.Parameters.AddWithValue("@i_cupocc", MTer.i_cupocc);
                        cmd.Parameters.AddWithValue("@cupo_cxc", MTer.cupo_cxc);
                        cmd.Parameters.AddWithValue("@i_cupocp", MTer.i_cupocp);
                        cmd.Parameters.AddWithValue("@cupo_cxp", MTer.cupo_cxp);
                        cmd.Parameters.AddWithValue("@bloqueo", MTer.bloqueo);
                        cmd.Parameters.AddWithValue("@lista_prec", MTer.lista_prec);
                        cmd.Parameters.AddWithValue("@ind_mayor", MTer.ind_mayor);
                        cmd.Parameters.AddWithValue("@cod_zona", MTer.cod_zona);
                        cmd.Parameters.AddWithValue("@cod_ven", MTer.cod_ven);
                        cmd.Parameters.AddWithValue("@dia_plaz", MTer.dia_plaz);
                        cmd.Parameters.AddWithValue("@por_des", MTer.por_des);
                        cmd.Parameters.AddWithValue("@cod_can", MTer.cod_can);
                        cmd.Parameters.AddWithValue("@tdoc", MTer.tdoc);
                        cmd.Parameters.AddWithValue("@tip_pers", MTer.tip_pers);
                        cmd.Parameters.AddWithValue("@cod_ciu", MTer.cod_ciu);
                        cmd.Parameters.AddWithValue("@cod_depa", MTer.cod_depa);
                        cmd.Parameters.AddWithValue("@cod_pais", MTer.cod_pais);
                        cmd.Parameters.AddWithValue("@apl1", MTer.apl1);
                        cmd.Parameters.AddWithValue("@apl2", MTer.apl2);
                        cmd.Parameters.AddWithValue("@nom1", MTer.nom1);
                        cmd.Parameters.AddWithValue("@nom2", MTer.nom2);
                        cmd.Parameters.AddWithValue("@razon_soc", MTer.razon_soc);
                        cmd.Parameters.AddWithValue("@dir_comer", MTer.dir_comer);
                        cmd.Parameters.AddWithValue("@observ", MTer.observ);
                        cmd.Parameters.AddWithValue("@cont_cxc", MTer.cont_cxc);

                        cmd.Parameters.AddWithValue("@uni_fra", MTer.uni_fra);
                        cmd.Parameters.AddWithValue("@esp_gab", MTer.esp_gab);
                        cmd.Parameters.AddWithValue("@email_fe", MTer.email_fe);
                        connection.Open();
                        valor = cmd.ExecuteNonQuery();
                    }
                }
                return valor;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Interno Sia", MessageBoxButton.OK, MessageBoxImage.Stop);
                return 0;
            }

        }


        bool ComparaDatos()
        {
            StringBuilder sbRed = new StringBuilder();
            StringBuilder sbLocal = new StringBuilder();
            Tercero __MTer = new Tercero();
            try
            {
                SqlDataReader dr;
                dr = SiaWin.Func.SqlDR("SELECT * FROM Comae_ter  where idrow=" + MTer.idrow.ToString(), idemp);
                while (dr.Read())
                {

                    __MTer.idrow = Convert.ToInt32(dr["Idrow"]);
                    __MTer.cod_ter = dr["cod_ter"].ToString().Trim();
                    __MTer.dv = dr["dv"].ToString().Trim();
                    __MTer.nom_ter = dr["nom_ter"].ToString().Trim();
                    __MTer.clasific = Convert.ToInt16(dr["clasific"] is DBNull ? -1 : dr["clasific"]);
                    __MTer.repres = dr["repres"].ToString().Trim();
                    __MTer.dir1 = dr["dir1"].ToString().Trim();
                    __MTer.dir2 = dr["dir2"].ToString().Trim();
                    __MTer.tel1 = dr["tel1"].ToString().Trim();
                    __MTer.cel = dr["cel"].ToString().Trim();
                    __MTer.email = dr["email"].ToString().Trim();
                    __MTer.ciudad = dr["ciudad"].ToString().Trim();
                    __MTer.depa = dr["depa"].ToString().Trim();
                    __MTer.pais = dr["pais"].ToString().Trim();
                    __MTer.conta = dr["conta"].ToString().Trim();
                    __MTer.estado = Convert.ToInt16(dr["estado"] is DBNull ? -1 : dr["estado"]);
                    __MTer.fec_ing = dr["fec_ing"].ToString().Trim();
                    __MTer.tip_prv = Convert.ToInt16(dr["tip_prv"] is DBNull ? -1 : dr["tip_prv"]);
                    __MTer.ind_ret = Convert.ToInt16(dr["ind_ret"] is DBNull ? -1 : dr["ind_ret"]);
                    __MTer.ret_iva = Convert.ToInt16(dr["ret_iva"] is DBNull ? -1 : dr["ret_iva"]);
                    __MTer.ret_ica = Convert.ToInt16(dr["ret_ica"] is DBNull ? -1 : dr["ret_ica"]);

                    __MTer.rtiva = Convert.ToInt16(dr["rtiva"] is DBNull ? -1 : dr["rtiva"]);
                    __MTer.rtica = Convert.ToInt16(dr["rtica"] is DBNull ? -1 : dr["rtica"]);

                    __MTer.aut_ret = Convert.ToInt16(dr["aut_ret"] is DBNull ? -1 : dr["aut_ret"]);
                    __MTer.ind_rete = Convert.ToInt16(dr["ind_rete"] is DBNull ? -1 : dr["ind_rete"]);
                    __MTer.ind_iva = Convert.ToInt16(dr["ind_iva"] is DBNull ? 1 : dr["ind_iva"]);
                    __MTer.por_ica = Convert.ToDecimal(dr["por_ica"] is DBNull ? 0 : dr["por_ica"]);
                    __MTer.cod_ban = dr["cod_ban"].ToString().Trim();
                    __MTer.cta = dr["cta"].ToString().Trim();
                    //__MTer.cta_ban = dr["cta_ban"].ToString().Trim();// Cuenta
                    __MTer.ind_suc = Convert.ToBoolean(dr["ind_suc"] is DBNull ? 0 : dr["ind_suc"]);
                    __MTer.i_cupocc = Convert.ToBoolean(dr["i_cupocc"] is DBNull ? 0 : dr["i_cupocc"]);
                    __MTer.cupo_cxc = Convert.ToInt32(dr["cupo_cxc"] is DBNull ? 0 : dr["cupo_cxc"]);
                    __MTer.i_cupocp = Convert.ToBoolean(dr["i_cupocp"] is DBNull ? 0 : dr["i_cupocp"]);
                    __MTer.cupo_cxp = Convert.ToInt32(dr["cupo_cxp"] is DBNull ? 0 : dr["cupo_cxp"]);
                    __MTer.bloqueo = Convert.ToInt16(dr["bloqueo"] is DBNull ? -1 : dr["bloqueo"]);
                    __MTer.lista_prec = Convert.ToInt16(dr["lista_prec"] is DBNull ? -1 : dr["lista_prec"]);
                    __MTer.ind_mayor = Convert.ToInt16(dr["ind_mayor"] is DBNull ? -1 : dr["ind_mayor"]);
                    __MTer.cod_zona = dr["cod_zona"].ToString().Trim();
                    __MTer.cod_ven = dr["cod_ven"].ToString().Trim();
                    __MTer.dia_plaz = Convert.ToInt16(dr["dia_plaz"] is DBNull ? 0 : dr["dia_plaz"]);
                    __MTer.por_des = Convert.ToInt32(dr["por_des"] is DBNull ? 0 : dr["por_des"]);
                    __MTer.cod_can = dr["cod_can"].ToString().Trim();
                    __MTer.tdoc = dr["tdoc"].ToString();
                    __MTer.tip_pers = Convert.ToInt16(dr["tip_pers"] is DBNull ? -1 : dr["tip_pers"]);
                    __MTer.cod_ciu = dr["cod_ciu"].ToString().Trim();
                    __MTer.cod_pais = dr["cod_pais"].ToString().Trim();
                    __MTer.apl1 = dr["apl1"].ToString().Trim();
                    __MTer.apl2 = dr["apl2"].ToString().Trim();
                    __MTer.nom1 = dr["nom1"].ToString().Trim();
                    __MTer.nom2 = dr["nom2"].ToString().Trim();
                    __MTer.razon_soc = dr["razon_soc"].ToString().Trim();
                    __MTer.dir_comer = dr["dir_comer"].ToString().Trim();//Direccion razon social
                    __MTer.observ = dr["observ"].ToString().Trim();
                    __MTer.cont_cxc = dr["cont_cxc"].ToString().Trim();//Contacto cobro
                    __MTer.fec_cump = dr["fec_cump"].ToString().Trim();//FEC_CUMP fecha de cumpleaños
                    __MTer.uni_fra = Convert.ToInt16(dr["uni_fra"] is DBNull ? -1 : dr["uni_fra"]);
                    __MTer.esp_gab = Convert.ToInt16(dr["esp_gab"] is DBNull ? 0 : dr["esp_gab"]);
                    __MTer.email_fe = dr["email_fe"].ToString().Trim();
                    __MTer.fec_act = dr["fec_act"].ToString().Trim();//FEC_CUMP fecha de cumpleaños

                }
                dr.Close();
                //// recorre campos de la clase
                MTer.GetType().GetProperties().ToList().ForEach(f =>
                {
                    try
                    {
                        //compara si cambiaron los campos del registro en le servidor
                        var propertyInfo = typeof(Tercero).GetProperties().Where(p => p.Name == f.Name).Single();
                        var valueA = propertyInfo.GetValue(_MTer, null); //ORIGINAL EN MEMORIA
                        var valueB = propertyInfo.GetValue(MTer, null);  //ACTUAL ///
                        var valueC = propertyInfo.GetValue(__MTer, null); //REAL SQL DATA
                        if (!valueA.Equals(valueC)) sbRed.Append("Tabla:" + tabla + Environment.NewLine + "Cambio Campo " + f.Name + Environment.NewLine + "Anterior :" + valueA + Environment.NewLine + "Nuevo: " + valueC);
                        if (!valueA.Equals(valueB)) sbLocal.Append("Tabla:" + tabla + " : Cambio Campo " + f.Name + Environment.NewLine + " Anterior :" + valueA + " - Nuevo: " + valueB + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });
                if (sbRed.ToString() != string.Empty)
                {
                    //enviar a auditoria el msg y la respuesta y la cadena sbRed
                    MessageBoxResult result = MessageBox.Show("Otro usuario ha cambiado ya este registro, Usted desea guardar sus cambios?", "Confirmacion", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result != MessageBoxResult.Yes)
                    {
                        //_usercontrol._EstadoSave = true;
                        //ActualizaCampos(_usercontrol.tabitem.CmpReturn, string.Empty);
                        //_usercontrol.ActivaDesactivaMaestra(2);
                        //_usercontrol._EstadoAdEdMae = 0;
                        return false;
                    }
                }
                // registra en auditoria los cambio hechos por el usuario                
                if (sbLocal.ToString() != string.Empty)
                {
                    SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idemp, 0, -1, 0, sbLocal.ToString(), "");
                    //MessageBox.Show("Cambio local:"+sbLocal.ToString());
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Error al Actualizar datos", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }
            catch (System.Exception _error)
            {
                MessageBox.Show(_error.Message);
                return false;
            }
            return true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            activecontrol(true, "Guardar");
            editdel(false);
            bloquear(false);
            ClearClas();
            ClearClasOld();
        }
        private void BtnSucursal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(MTer.cod_ter))
                {
                    MessageBox.Show("el codigo del tercero esta vacio para poder grabar una sucursal", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                if (MTer.ind_suc == false)
                {
                    MessageBox.Show("debe de habilitar el indicador de maneja sucursales", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                string query = "select * from comae_ter where cod_ter = '" + MTer.cod_ter + "'";
                DataTable dt = SiaWin.Func.SqlDT(query, "tercero", idemp);
                if (dt.Rows.Count > 0)
                {

                    dynamic ww = SiaWin.WindowExt(9469, "Sucursal");
                    ww.idemp = idemp;
                    ww.ShowInTaskbar = false;
                    ww.codigo_tercero = MTer.cod_ter;
                    ww.nombre_tercero = MTer.nom_ter;
                    ww.ind_suc = MTer.ind_suc;
                    ww.Owner = Application.Current.MainWindow;
                    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    ww.ShowDialog();
                }
                else
                {
                    MessageBox.Show("el tercero debe de estar primero guardado en la base de datos para poderle agregar una sucursal", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    ;
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar sucursales:" + w);
            }
        }

        private void BtnDesct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(MTer.cod_ter))
                {
                    MessageBox.Show("el codigo del tercero esta vacio para poder grabar una sucursal", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                dynamic ww = SiaWin.WindowExt(9479, "DescuentoPorLinea");
                ww.idemp = idemp;
                ww.ShowInTaskbar = false;
                ww.codigo_tercero = MTer.cod_ter;
                ww.nombre_tercero = MTer.nom_ter;
                ww.Owner = Application.Current.MainWindow;
                ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                ww.ShowDialog();
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar sucursales:" + w);
            }
        }

        private void txter_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty((sender as TextBox).Text)) return;

                string query = "select * from comae_ter where cod_ter = '" + (sender as TextBox).Text + "'";
                DataTable dt = SiaWin.Func.SqlDT(query, "tercero", idemp);
                if (dt.Rows.Count > 0)
                {
                    if (SiaWin.Func.Acceso(SiaWin._UserGroup, SiaWin._ProyectId, 6, idemp, 1, "lEdit") == true)
                    {
                        int id = (int)dt.Rows[0]["idrow"];
                        ActualizaCampos(id, string.Empty);
                        activecontrol(false, "Modificar");
                        Clone();
                    }
                    else
                    {
                        MessageBox.Show("este usaurio no tiene permisos para editar por favor digite un cliente nuevo", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        (sender as TextBox).Text = "";
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error el el foco:" + w);
            }
        }

        private void CBtipoPerso_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int selectedIndex = MTer.tip_pers;
                if (selectedIndex == 0)
                {
                    string nombre = MTer.nom_ter;
                    string[] split = nombre.Split(new Char[] { ' ', ',' });

                    if (split.Length <= 3)
                    {
                        MTer.nom1 = split[0];
                        MTer.apl1 = split[1];
                        MTer.apl2 = split[2];
                        MTer.razon_soc = "";
                    }
                    else
                    {
                        MTer.nom1 = split[0];
                        MTer.nom2 = split[1];
                        MTer.apl1 = split[2];
                        MTer.apl2 = split[3];
                        MTer.razon_soc = "";
                    }

                }
                if (selectedIndex == 1)
                {
                    MTer.razon_soc = MTer.nom_ter;
                    MTer.apl1 = "";
                    MTer.nom1 = "";
                    MTer.apl2 = "";
                    MTer.nom2 = "";
                }
            }
            catch (Exception)
            {

            }
        }

        private void BtnBuscarElement_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tbl = (sender as Button).Tag.ToString();
                string cod = "", nom = "", id = "", tit = "";

                switch (tbl)
                {
                    case "MmMae_muni":
                        cod = "cod_muni"; nom = "nom_muni"; id = "idrow"; tit = "Maestra de Municipios";
                        break;
                    case "MmMae_pais":
                        cod = "cod_pais"; nom = "nom_pais"; id = "cod_pais"; tit = "Maestra de Pais";
                        break;
                    case "MmMae_depa":
                        cod = "cod_dep"; nom = "nom_dep"; id = "cod_dep"; tit = "Maestra de Departamento";
                        break;
                }

                dynamic winb = SiaWin.WindowBuscar(tbl, cod, nom, cod, id, tit, cnEmp, false, "", idEmp: idemp);
                winb.ShowInTaskbar = false;
                winb.Owner = Application.Current.MainWindow;
                winb.Height = 400;
                winb.ShowDialog();
                int idrow = winb.IdRowReturn;
                string codigo = winb.Codigo.Trim();
                string nombre = winb.Nombre.Trim().ToUpper();
                //winb = null;
                if (idrow > 0)
                {
                    switch (tbl)
                    {
                        case "MmMae_muni":
                            MTer.cod_ciu = codigo; MTer.ciudad = nombre;
                            break;
                        case "MmMae_depa":
                            MTer.cod_depa = codigo; MTer.depa = nombre;
                            break;
                        case "MmMae_pais":
                            MTer.cod_pais = codigo; MTer.pais = nombre;
                            break;
                    }

                }
                if (string.IsNullOrEmpty(codigo)) e.Handled = false;
                e.Handled = true;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al buscar:" + 2);
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty((sender as TextBox).Text)) return;

                string tbl = (sender as TextBox).Tag.ToString();

                string cod = "", nom = "", id = "", tit = "";
                switch (tbl)
                {
                    case "MmMae_muni":
                        cod = "cod_muni"; nom = "nom_muni"; id = "idrow"; tit = "Maestra de Municipios";
                        break;
                    case "MmMae_pais":
                        cod = "cod_pais"; nom = "nom_pais"; id = "cod_pais"; tit = "Maestra de Pais";
                        break;
                    case "MmMae_depa":
                        cod = "cod_dep"; nom = "nom_dep"; id = "cod_dep"; tit = "Maestra de Departamento";
                        break;
                }

                string query = "select * from " + tbl + " where  " + cod + "='" + (sender as TextBox).Text + "' ";
                DataTable dt = SiaWin.Func.SqlDT(query, "tabla", idemp);
                if (dt.Rows.Count > 0)
                {
                    string code = dt.Rows[0][cod].ToString();
                    string name = dt.Rows[0][nom].ToString();
                    switch (tbl)
                    {
                        case "MmMae_muni":
                            MTer.cod_ciu = code; MTer.ciudad = name;
                            break;
                        case "MmMae_depa":
                            MTer.cod_depa = code; MTer.depa = name;
                            break;
                        case "MmMae_pais":
                            MTer.cod_pais = code; MTer.pais = name;
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("el codigo que ingreso no existe en la " + tit, "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    (sender as TextBox).Text = "";
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al buscar:" + w);
            }
        }
        private void BtnDigVer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(MTer.cod_ter))
                {
                    MessageBox.Show("el campo de NIT/CC debe de estar lleno para agregar el digito de verificacion", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                string procedure = "EXEC	[dbo].[dig_verif] @nit ='" + MTer.cod_ter + "' ";
                DataTable dt = SiaWin.Func.SqlDT(procedure, "tabla", 0);
                if (dt.Rows.Count > 0)
                {
                    MTer.dv = dt.Rows[0]["digito"].ToString();
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("errro en el digito de verificacion" + w);
            }
        }

        private void BtnNotas_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dynamic ww = SiaWin.WindowExt(9681, "NotasEmpleados");
                ww.ShowInTaskbar = false;
                ww.Owner = Application.Current.MainWindow;
                ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                ww.cod_empleado = MTer.cod_ter;
                ww.ShowDialog();
            }
            catch (Exception w)
            {
                MessageBox.Show("error al agregar una nota:" + w);
            }
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(MTer.cod_ter))
                {
                    MessageBox.Show("el campo del tercero esta vacio", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                DataTable dt = SiaWin.Func.SqlDT("select * from comae_ter where cod_ter='" + MTer.cod_ter + "'", "tabla", idemp);
                if (dt.Rows.Count > 0)
                {

                    using (ExcelEngine excelEngine = new ExcelEngine())
                    {
                        IApplication application = excelEngine.Excel;
                        IWorkbook workbook = application.Workbooks.Create(1);
                        IWorksheet sheet = workbook.Worksheets[0];
                        DataTable dataTable = dt;
                        sheet.ImportDataTable(dataTable, true, 1, 1, true);
                        sheet.UsedRange.AutofitColumns();


                        SaveFileDialog sfd = new SaveFileDialog
                        {
                            FilterIndex = 2,
                            Filter = "Excel 97 to 2003 Files(*.xls)|*.xls|Excel 2007 to 2010 Files(*.xlsx)|*.xlsx|Excel 2013 File(*.xlsx)|*.xlsx"
                        };
                        if (sfd.ShowDialog() == true)
                        {
                            using (Stream stream = sfd.OpenFile())
                            {
                                if (sfd.FilterIndex == 1)
                                    workbook.Version = ExcelVersion.Excel97to2003;
                                else if (sfd.FilterIndex == 2)
                                    workbook.Version = ExcelVersion.Excel2010;
                                else
                                    workbook.Version = ExcelVersion.Excel2013;

                                workbook.SaveAs(stream);
                            }
                            if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archivo", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                            {
                                System.Diagnostics.Process.Start(sfd.FileName);
                            }
                        }
                        else
                        {
                            MessageBox.Show("el tercero no existe", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            return;
                        }
                    }




                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al exportar:" + w);
            }
        }

        private void TextBoxNom_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace((sender as TextBox).Text))
            {
                MTer.repres = ((sender as TextBox).Text);
            }
        }


    }
}

