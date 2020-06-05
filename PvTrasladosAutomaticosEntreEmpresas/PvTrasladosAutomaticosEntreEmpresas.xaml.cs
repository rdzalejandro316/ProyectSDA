using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.ScrollAxis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
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
    /// <summary>
    /// Lógica de interacción para UserControl1.xaml
    /// </summary>
    public partial class PvTrasladosAutomaticosEntreEmpresas : Window
    {
        //Sia.PublicarPnt(9467, "PvTrasladosAutomaticosEntreEmpresas");
        dynamic SiaWin;
        public int idEmp = 0;
        public string codpvta = string.Empty;
        public string codbod = string.Empty;
        string nompvta = string.Empty;
        string codcco = string.Empty;
        string nitemp = string.Empty;
        public DataTable DtCue = new DataTable();
        double TotFaltante = 0;
        double TotTraslado1 = 0;
        double TotTraslado2 = 0;
        double TotTraslado3 = 0;
        double TotTraslado4 = 0;
        string cnEmp = "";
        int idLogo = 0;
        string[] ListEmpresas = new string[5];
        string[] ListBodegas = new string[5];
        string[] TitulosEmpresas = new string[5];
        int idEmpresa = 0;
        string cod_empresa = "";
        int idemp = 0;
        //DataTable dtCue = new DataTable();
        public PvTrasladosAutomaticosEntreEmpresas()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            this.dataGrid.SelectionController = new GridSelectionControllerExt(dataGrid);
            //            this.dataGrid.RowValidating += dataGrid_RowValidating;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //double height = ((Window)SiaWin).ActualHeight;
            //double width = ((Window)SiaWin).ActualWidth;
            //this.Width = width-100;
            LoadInfo();
        }

        public void LoadInfo()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idEmp);
                idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                nitemp = foundRow["BusinessNit"].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                idemp = SiaWin._BusinessId;
                //        System.Windows.Threading.Dispatcher.BeginInvoke(new A
                //Img.Source= AppDomain.CurrentDomain.BaseDirectory + "Imagenes\\" + idLogo.ToString() + "..png";
                //        ConfigCSource.PathImgBusiness=AppDomain.CurrentDomain.BaseDirectory + "Imagenes\\"+idLogo.ToString()+"..png";
                //        ConfigCSource.nomBuss = ((Inicio)Application.Current.MainWindow).Func.cmp("business","BusinessId","BusinessName",idEmp,0);
                TxtEmpresa.Text = SiaWin._BusinessName.ToString().Trim();
                TxtPVenta.Text = codpvta;

                //        _usercontrol.Seg.Auditor(0,_usercontrol.ProjectId,idUser,_usercontrol.GroupId,idEmp,_usercontrol.ModuleId,_usercontrol.AccesoId,0,"Ingreso a: Punto de venta"+" - " +_titulo,"");
                if (codpvta == string.Empty)
                {
                    //_usercontrol.Opacity = 0.5;
                    MessageBox.Show("El usuario no tiene asignado un punto de venta, Pantalla Bloqueada");
                    this.IsEnabled = false;
                    //_usercontrol.IsEnabled=false;
                }
                else
                {
                    nompvta = SiaWin.Func.cmpCodigo("copventas", "cod_pvt", "nom_pvt", codpvta, idEmp);
                    TxtPVenta.Text = codpvta + "-" + nompvta;
                    codbod = SiaWin.Func.cmpCodigo("copventas", "cod_pvt", "cod_bod", codpvta, idEmp);
                    codcco = SiaWin.Func.cmpCodigo("copventas", "cod_pvt", "cod_cco", codpvta, idEmp);
                    if (string.IsNullOrEmpty(codbod))
                    {
                        //_usercontrol.Opacity = 0.5;
                        MessageBox.Show("El punto de venta Asignado no tiene bodega , Pantalla Bloqueada");
                        this.IsEnabled = false;
                    }
                    dataGrid.ItemsSource = DtCue;
                    ListEmpresas[0] = "010"; ListEmpresas[1] = "020"; ListEmpresas[2] = "030"; ListEmpresas[3] = "040"; ListEmpresas[4] = "050";
                    ListBodegas[0] = "003"; ListBodegas[1] = "012"; ListBodegas[2] = "007"; ListBodegas[3] = "017"; ListBodegas[4] = "050";
                    TitulosEmpresas[0] = "Tres"; TitulosEmpresas[1] = "Saat"; TitulosEmpresas[2] = "Colm"; TitulosEmpresas[3] = "Rodam"; TitulosEmpresas[4] = "Invers";
                    idEmpresa = Array.IndexOf(ListBodegas, codbod);
                    ActualizaColumnas(codbod);
                    dataGrid.Focus();
                    if (DtCue.Rows.Count > 0) dataGrid.SelectedIndex = 0;
                    //if (dataGrid.SelectedIndex < 0) return;
                    if (DtCue.Rows.Count > 0) sumaAbonos();
                    if (DtCue.Rows.Count > 0) this.dataGrid.MoveCurrentCell(new RowColumnIndex(1, 6), false);

                    loadBod();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void loadBod()
        {
            string query = " select * from InMae_bod where cod_bod='001' or cod_bod='004' or cod_bod='010' or cod_bod='013' or cod_bod='005' or cod_bod='009' or cod_bod='019' or cod_bod='008' or cod_bod='052' order by cod_bod ";
            DataTable tabla = SiaWin.Func.SqlDT(query, "Buscar", idemp);

            TX_Bod001.Text = string.IsNullOrEmpty(tabla.Rows[0]["ini_bod"].ToString()) ? "Bod001" : tabla.Rows[0]["ini_bod"].ToString();
            TX_Bod004.Text = string.IsNullOrEmpty(tabla.Rows[1]["ini_bod"].ToString()) ? "Bod004" : tabla.Rows[1]["ini_bod"].ToString();
            TX_Bod005.Text = string.IsNullOrEmpty(tabla.Rows[2]["ini_bod"].ToString()) ? "Bod005" : tabla.Rows[2]["ini_bod"].ToString();
            TX_Bod008.Text = string.IsNullOrEmpty(tabla.Rows[3]["ini_bod"].ToString()) ? "Bod008" : tabla.Rows[3]["ini_bod"].ToString();
            TX_Bod009.Text = string.IsNullOrEmpty(tabla.Rows[4]["ini_bod"].ToString()) ? "Bod009" : tabla.Rows[4]["ini_bod"].ToString();
            TX_Bod010.Text = string.IsNullOrEmpty(tabla.Rows[5]["ini_bod"].ToString()) ? "Bod010" : tabla.Rows[5]["ini_bod"].ToString();
            TX_Bod013.Text = string.IsNullOrEmpty(tabla.Rows[6]["ini_bod"].ToString()) ? "Bod013" : tabla.Rows[6]["ini_bod"].ToString();
            TX_Bod019.Text = string.IsNullOrEmpty(tabla.Rows[7]["ini_bod"].ToString()) ? "Bod019" : tabla.Rows[7]["ini_bod"].ToString();
            TX_Bod052.Text = string.IsNullOrEmpty(tabla.Rows[8]["ini_bod"].ToString()) ? "Bod052" : tabla.Rows[8]["ini_bod"].ToString();

            //MessageBox.Show("TX_Bod019:"+ TX_Bod019.Text);
            
        }


        private void ButtonTerminar_Click(object sender, RoutedEventArgs e)
        {
            //validar que los codigos y las empresas existan
            //for (int i = 1; i <= 4; i++)
            //{
            // string Pventa = ListBodegas[i];
            // string codempresa = ListEmpresas[i];
            // int idempresa = Convert.ToInt32(SiaWin.Func.cmpCodigo("Business", "BusinessCode", "BusinessId", codempresa, 0));
            // codbod = SiaWin.Func.cmpCodigo("copventas", "cod_pvt", "cod_bod", codpvta, idempresa);
            //if(string.IsNullOrEmpty(codbod) || codbod =="")
            //{
            //  MessageBox.Show("Proceso detenido, No existe codigo de punto de venta:" + Pventa + "  en empresa:" + codempresa);
            //return;
            //}

            /// validar codigos de bodega en copve
            //}

            //codbod = SiaWin.Func.cmpCodigo("copventas", "cod_pvt", "cod_bod", codpvta, idEmp);

            /// valida saldos nuevamente y que cantidad sea total
            /// 

            if (MessageBox.Show("Usted desea Generar Traslados...?", "Traslados Automaticos ", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }
            try
            {
                ActualizaColumnas(codbod);
                if (!ValidaCantidadTraslado()) return;
                string Script = GeneraScript();
                if (string.IsNullOrEmpty(Script)) return;
                if (!GuardaTraslado(Script)) return;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void ButtonCancelar_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Usted desea Cancelar...?", "Traslados Automaticos ", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }
        private string GeneraScript()
        {
            //ListEmpresas[0]
            //            ListEmpresas[0] = "020"; ListEmpresas[1] = "030"; ListEmpresas[2] = "040"; ListEmpresas[3] = "050";
            //          ListBodegas[0] = "012"; ListBodegas[1] = "007"; ListBodegas[2] = "017"; ListBodegas[3] = "050";
            StringBuilder _SbSql = new StringBuilder();
            try
            {

                // empresa 1
                for (int i = 1; i <= 4; i++)
                {
                    string Pventa = ListBodegas[i];
                    /// validar codigos de bodega en copventas, que exista

                    string Consecutivo = @"declare @fecdoc__x as datetime;set @fecdoc__x = getdate();declare @ini__x as char(4);declare @num__x as varchar(12);declare @iConsecutivo__x char(12) = '' ;declare @iFolioHost__x int = 0;UPDATE COpventas SET trn_160 = ISNULL(trn_160, 0) + 1  WHERE cod_pvt='" + Pventa + "';SELECT @iFolioHost__x = trn_160,@ini__x=rtrim(cod_pvt) FROM Copventas  WHERE cod_pvt='" + Pventa + "';set @num__x=@iFolioHost__x;select @iConsecutivo__x=rtrim(@ini__x)+REPLICATE ('0',12-len(rtrim(@ini__x))-len(rtrim(convert(varchar,@num__x))))+rtrim(convert(varchar,@num__x));DECLARE @NewID__x INT;";
                    string sqlcab = @"INSERT INTO incab_doc (cod_trn,fec_trn,cod_cli,num_trn,doc_ref,des_mov,bod_tra,estado) values ('_trn160',@fecdoc__x,'800000000',@iConsecutivo__x,@iConsecutivo__x,'Traslado Interempresa','" + codbod + "',9);SELECT @NewID__x = SCOPE_IDENTITY();";
                    string sqlcab060 = @"INSERT INTO incab_doc (cod_trn,fec_trn,cod_cli,num_trn,doc_ref,des_mov,bod_tra,estado) values ('_trn160',@fecdoc__x,'800000000',@iConsecutivo__x,@iConsecutivo__x,'Traslado Interempresa','" + Pventa + "',9);SELECT @NewID__x = SCOPE_IDENTITY();";
                    string sqlInsert160 = string.Empty;
                    string sqlInsert060 = string.Empty;
                    string _sqlInsert060 = string.Empty;
                    foreach (System.Data.DataRow dr in DtCue.Rows)
                    {
                        decimal cantTrasladar = Convert.ToDecimal(dr["traslEmp" + i.ToString()].ToString());
                        if (cantTrasladar > 0)
                        {
                            sqlInsert160 = sqlInsert160 + @"INSERT INTO incue_doc (idregcab,cod_trn,num_trn,cod_ref,cod_bod,cantidad) values (@NewID__x,'_trn160',@iConsecutivo__x,'" + dr["cod_ref"].ToString() + "','" + ListBodegas[i] + "'," + cantTrasladar.ToString("F", CultureInfo.InvariantCulture) + ");";
                            sqlInsert060 = sqlInsert060 + @"INSERT INTO incue_doc (idregcab,cod_trn,num_trn,cod_ref,cod_bod,cantidad) values (@NewID__x,'_trn160',@iConsecutivo__x,'" + dr["cod_ref"].ToString() + "','" + codbod + "'," + cantTrasladar.ToString("F", CultureInfo.InvariantCulture) + ");";
                        }
                    }
                    if (!string.IsNullOrEmpty(sqlInsert160))
                    {
                        _SbSql.Append("use gruposaavedra_emp" + ListEmpresas[i] + ";");
                        _SbSql.Append(Consecutivo);
                        _SbSql.Append(sqlcab);
                        _SbSql.Append(sqlInsert160);
                        _SbSql.Replace("__x", "__" + ListEmpresas[i]);
                        _SbSql.Replace("_trn160", "160");
                        //MessageBox.Show("1  "+_SbSql.ToString());
                        _sqlInsert060 = "use gruposaavedra_emp" + ListEmpresas[0] + ";";
                        _sqlInsert060 = _sqlInsert060 + sqlcab060.Replace("_trn160", "060");
                        _sqlInsert060 = _sqlInsert060 + sqlInsert060.Replace("_trn160", "060");
                        _sqlInsert060 = _sqlInsert060.Replace("__x", "__" + ListEmpresas[i]);
                        _SbSql.Append(_sqlInsert060);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "TrasladoAutomaticoEmpresa-GeneraSript");
                return string.Empty;
            }

            return _SbSql.ToString(); ;
        }

        private bool GuardaTraslado(string _sqlFin)
        {
            //Clipboard.SetText(_sqlFin);
            //MessageBox.Show("entra a guarda" + _sqlFin);
            bool returnEstado = true;
            using (SqlConnection connection = new SqlConnection(SiaWin._cn))
            {
                connection.Open();
                StringBuilder errorMessages = new StringBuilder();
                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;
                // Start a local transaction.
                transaction = connection.BeginTransaction("Transaction");
                command.Connection = connection;
                command.Transaction = transaction;
                try
                {
                    command.CommandText = _sqlFin;
                    command.ExecuteScalar();
                    transaction.Commit();
                    connection.Close();
                }
                catch (SqlException ex)
                {
                    returnEstado = false;
                    transaction.Rollback();
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        errorMessages.Append(" SQL-Index #" + i + "\n" + "Message: " + ex.Errors[i].Message + "\n" + "LineNumber: " + ex.Errors[i].LineNumber + "\n" + "Source: " + ex.Errors[i].Source + "\n" + "Procedure: " + ex.Errors[i].Procedure + "\n");
                    }
                    MessageBox.Show(errorMessages.ToString());
                }
                catch (Exception ex)
                {
                    returnEstado = false;

                    transaction.Rollback();
                    errorMessages.Append("c Error:#" + ex.Message.ToString());
                    MessageBox.Show(errorMessages.ToString());
                }
            }
            ///set genera un 160 salida en la empresa y un 060 en la empresa destino
            return returnEstado;
        }

        private bool ValidaCantidadTraslado()
        {
            //verifica si hay cantidades para trasladar
            if (sumaAbonos() <= 0)
            {
                MessageBox.Show("No hay cantidades para trasladar .....");
                return false;
            }

            StringBuilder stringbuilder = new StringBuilder();
            foreach (System.Data.DataRow dr in DtCue.Rows) // search whole table
            {
                //emp1
                if (Convert.ToDecimal(dr["saldoEmp1"].ToString()) < Convert.ToDecimal(dr["traslEmp1"].ToString()))
                {
                    stringbuilder.Append("Producto " + dr["cod_ref"].ToString() + " Sin existencias - Empresa 1");
                }
                if (Convert.ToDecimal(dr["saldoEmp2"].ToString()) < Convert.ToDecimal(dr["traslEmp2"].ToString()))
                {
                    stringbuilder.Append("Producto " + dr["cod_ref"].ToString() + " Sin existencias - Empresa 2");
                }
                if (Convert.ToDecimal(dr["saldoEmp3"].ToString()) < Convert.ToDecimal(dr["traslEmp3"].ToString()))
                {
                    stringbuilder.Append("Producto " + dr["cod_ref"].ToString() + " Sin existencias - Empresa 3");
                }
                if (Convert.ToDecimal(dr["saldoEmp4"].ToString()) < Convert.ToDecimal(dr["traslEmp4"].ToString()))
                {
                    stringbuilder.Append("Producto " + dr["cod_ref"].ToString() + " Sin existencias - Empresa 4");
                }
                if (!string.IsNullOrEmpty(stringbuilder.ToString()))
                {
                    MessageBox.Show("Existen errores en traslados interempresa...." + Environment.NewLine + stringbuilder.ToString());
                    return false;
                }

            }
            return true;
        }
        private void ActualizaColumnas(string codigoBod)
        {
            //            	case _codbod = "003" && tresmfuelles
            //      thisform.grid1.column4.header1.caption = "Saldo_Tres"
            //    thisform.grid1.column6.header1.caption = "Saldo_Colm"
            //  thisform.grid1.column8.header1.caption = "Saldo_Saat"
            //thisform.grid1.column10.header1.caption = "Saldo_Rod"
            //thisform.grid1.column12.header1.caption = "Saldo_Inv"
            //= SdoInGeneralEMP(_cAnoTra, _cPerTra, "007", "_cursor1", "saldo1", "030")
            if (codigoBod == "003")
            {
                ListEmpresas[0] = "010"; ListEmpresas[1] = "020"; ListEmpresas[2] = "030"; ListEmpresas[3] = "040"; ListEmpresas[4] = "050";
                ListBodegas[0] = "003"; ListBodegas[1] = "012"; ListBodegas[2] = "007"; ListBodegas[3] = "017"; ListBodegas[4] = "050";
                //TitulosEmpresas[0] = "Tres"; TitulosEmpresas[1] = "Saat"; TitulosEmpresas[2] = "Colm"; TitulosEmpresas[3] = "Rodam"; TitulosEmpresas[4] = "Invers";
                //int dogIndex = Array.IndexOf(ListBodegas,codigoBod);
                dataGrid.Columns[3].HeaderText = "Saldo Tres";
                dataGrid.Columns[5].HeaderText = "Saldo Saat";
                dataGrid.Columns[6].HeaderText = "Trasl Saat";
                dataGrid.Columns[7].HeaderText = "Saldo Colm";
                dataGrid.Columns[8].HeaderText = "Trasl Colm";
                dataGrid.Columns[9].HeaderText = "Saldo Rodam";
                dataGrid.Columns[10].HeaderText = "Trasl Rodam";
                dataGrid.Columns[11].HeaderText = "Saldo Invers";
                dataGrid.Columns[12].HeaderText = "Trasl Invers";
                // trae inventarios
                if (DtCue.Rows.Count > 0)
                {
                    foreach (System.Data.DataRow dr in DtCue.Rows) // search whole table
                    {
                        string codReferencia = dr["cod_ref"].ToString();
                        decimal saldoin = SiaWin.Func.SaldoInv(codReferencia, "012", "020"); //saato
                        dr["saldoEmp1"] = saldoin;
                        saldoin = SiaWin.Func.SaldoInv(codReferencia, "007", "030"); //colmu
                        dr["saldoEmp2"] = saldoin;
                        saldoin = SiaWin.Func.SaldoInv(codReferencia, "017", "040"); //rodamiento
                        dr["saldoEmp3"] = saldoin;
                        saldoin = SiaWin.Func.SaldoInv(codReferencia, "050", "050"); //INVERSIONES
                        dr["saldoEmp4"] = saldoin;
                    }
                }
            }
            if (codigoBod == "007")
            {
                ListEmpresas[0] = "030"; ListEmpresas[1] = "010"; ListEmpresas[2] = "020"; ListEmpresas[3] = "040"; ListEmpresas[4] = "050";
                ListBodegas[0] = "007"; ListBodegas[1] = "003"; ListBodegas[2] = "012"; ListBodegas[3] = "017"; ListBodegas[4] = "050";

                dataGrid.Columns[3].HeaderText = "Saldo Colm";
                dataGrid.Columns[5].HeaderText = "Saldo Tres";
                dataGrid.Columns[6].HeaderText = "Trasl Tres";
                dataGrid.Columns[7].HeaderText = "Saldo Saat";
                dataGrid.Columns[8].HeaderText = "Trasl Saat";
                dataGrid.Columns[9].HeaderText = "Saldo Rodam";
                dataGrid.Columns[10].HeaderText = "Trasl Rodam";
                dataGrid.Columns[11].HeaderText = "Saldo Invers";
                dataGrid.Columns[12].HeaderText = "Trasl Invers";
                // trae inventarios
                if (DtCue.Rows.Count > 0)
                {
                    foreach (System.Data.DataRow dr in DtCue.Rows) // search whole table
                    {
                        string codReferencia = dr["cod_ref"].ToString();
                        decimal saldoin = SiaWin.Func.SaldoInv(codReferencia, "003", "010"); //colome
                        dr["saldoEmp1"] = saldoin;
                        saldoin = SiaWin.Func.SaldoInv(codReferencia, "012", "020"); //saatorcol
                        dr["saldoEmp2"] = saldoin;
                        saldoin = SiaWin.Func.SaldoInv(codReferencia, "017", "040"); //rodamiento
                        dr["saldoEmp3"] = saldoin;
                        saldoin = SiaWin.Func.SaldoInv(codReferencia, "050", "050"); //INVERSIONES
                        dr["saldoEmp4"] = saldoin;
                    }
                }
            }
            if (codigoBod == "012")
            {
                ListEmpresas[0] = "020"; ListEmpresas[1] = "010"; ListEmpresas[2] = "030"; ListEmpresas[3] = "040"; ListEmpresas[4] = "050";
                ListBodegas[0] = "012"; ListBodegas[1] = "003"; ListBodegas[2] = "007"; ListBodegas[3] = "017"; ListBodegas[4] = "050";
                dataGrid.Columns[3].HeaderText = "Saldo Saat";
                dataGrid.Columns[5].HeaderText = "Saldo Tres";
                dataGrid.Columns[6].HeaderText = "Trasl Tres";
                dataGrid.Columns[7].HeaderText = "Saldo Colm";
                dataGrid.Columns[8].HeaderText = "Trasl Colm";
                dataGrid.Columns[9].HeaderText = "Saldo Rodam";
                dataGrid.Columns[10].HeaderText = "Trasl Rodam";
                dataGrid.Columns[11].HeaderText = "Saldo Invers";
                dataGrid.Columns[12].HeaderText = "Trasl Inver";
                // trae inventarios
                if (DtCue.Rows.Count > 0)
                {
                    foreach (System.Data.DataRow dr in DtCue.Rows) // search whole table
                    {
                        string codReferencia = dr["cod_ref"].ToString();
                        decimal saldoin = SiaWin.Func.SaldoInv(codReferencia, "003", "010"); //tres
                        dr["saldoEmp1"] = saldoin;
                        saldoin = SiaWin.Func.SaldoInv(codReferencia, "007", "030"); //tres
                        dr["saldoEmp2"] = saldoin;
                        saldoin = SiaWin.Func.SaldoInv(codReferencia, "017", "040"); //rodamiento
                        dr["saldoEmp3"] = saldoin;
                        saldoin = SiaWin.Func.SaldoInv(codReferencia, "050", "050"); //INVERSIONES
                        dr["saldoEmp4"] = saldoin;
                    }
                }
            }
            if (codigoBod == "017")
            {
                ListEmpresas[0] = "040"; ListEmpresas[1] = "010"; ListEmpresas[2] = "020"; ListEmpresas[3] = "030"; ListEmpresas[4] = "050";
                ListBodegas[0] = "017"; ListBodegas[1] = "003"; ListBodegas[2] = "012"; ListBodegas[3] = "007"; ListBodegas[4] = "050";

                dataGrid.Columns[3].HeaderText = "Saldo Rodam";
                dataGrid.Columns[5].HeaderText = "Saldo Tres";
                dataGrid.Columns[6].HeaderText = "Trasl Tres";
                dataGrid.Columns[7].HeaderText = "Saldo Saat";
                dataGrid.Columns[8].HeaderText = "Trasl Saat";
                dataGrid.Columns[9].HeaderText = "Saldo Colm";
                dataGrid.Columns[10].HeaderText = "Trasl Colm";
                dataGrid.Columns[11].HeaderText = "Saldo Invers";
                dataGrid.Columns[12].HeaderText = "Trasl Invers";
                // trae inventarios
                if (DtCue.Rows.Count > 0)
                {
                    foreach (System.Data.DataRow dr in DtCue.Rows) // search whole table
                    {
                        string codReferencia = dr["cod_ref"].ToString();
                        decimal saldoin = SiaWin.Func.SaldoInv(codReferencia, "003", "010"); //tres
                        dr["saldoEmp1"] = saldoin;
                        saldoin = SiaWin.Func.SaldoInv(codReferencia, "012", "020"); //colm
                        dr["saldoEmp2"] = saldoin;
                        saldoin = SiaWin.Func.SaldoInv(codReferencia, "007", "030"); //saat
                        dr["saldoEmp3"] = saldoin;
                        saldoin = SiaWin.Func.SaldoInv(codReferencia, "050", "050"); //INVERSIONES
                        dr["saldoEmp4"] = saldoin;
                    }
                }
            }
            if (codigoBod == "050")
            {
                ListEmpresas[0] = "050"; ListEmpresas[1] = "010"; ListEmpresas[2] = "020"; ListEmpresas[3] = "030"; ListEmpresas[4] = "040";
                ListBodegas[0] = "050"; ListBodegas[1] = "003"; ListBodegas[2] = "012"; ListBodegas[3] = "007"; ListBodegas[4] = "017";

                dataGrid.Columns[3].HeaderText = "Saldo Invers";
                dataGrid.Columns[5].HeaderText = "Saldo Tres";
                dataGrid.Columns[6].HeaderText = "Trasl Tres";
                dataGrid.Columns[7].HeaderText = "Saldo Saat";
                dataGrid.Columns[8].HeaderText = "Trasl Saat";
                dataGrid.Columns[9].HeaderText = "Saldo Colm";
                dataGrid.Columns[10].HeaderText = "Trasl Colm";
                dataGrid.Columns[11].HeaderText = "Saldo Rodam";
                dataGrid.Columns[12].HeaderText = "Trasl Rodam";
                // trae inventarios
                if (DtCue.Rows.Count > 0)
                {
                    foreach (System.Data.DataRow dr in DtCue.Rows) // search whole table
                    {
                        string codReferencia = dr["cod_ref"].ToString();
                        decimal saldoin = SiaWin.Func.SaldoInv(codReferencia, "003", "010"); //tres
                        dr["saldoEmp1"] = saldoin;
                        saldoin = SiaWin.Func.SaldoInv(codReferencia, "012", "020"); //colm
                        dr["saldoEmp2"] = saldoin;
                        saldoin = SiaWin.Func.SaldoInv(codReferencia, "007", "030"); //saat
                        dr["saldoEmp3"] = saldoin;
                        saldoin = SiaWin.Func.SaldoInv(codReferencia, "017", "040"); //INVERSIONES
                        dr["saldoEmp4"] = saldoin;
                    }
                }
            }
        }
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                BtnTerminar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                return;

            }
            if (e.Key == Key.Escape)
            {
                BtnCancelar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                return;

            }
        }
        private void dataGrid_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            //MessageBox.Show("key enter"+e.Key.ToString());
            if (e.Key == Key.F11)
            {
                MessageBox.Show("key enter");
                var uiElement = e.OriginalSource as UIElement;
                uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                e.Handled = true;
            }
            if (e.Key == Key.F8)
            {
                GridNumericColumn Colum = ((SfDataGrid)sender).CurrentColumn as GridNumericColumn;
                if (Colum.MappingName == "abono")
                {
                    System.Data.DataRow dr = DtCue.Rows[dataGrid.SelectedIndex];
                    dr.BeginEdit();
                    decimal _cnt = Convert.ToDecimal(dr["saldo"].ToString());
                    dr["abono"] = _cnt;
                    dr.EndEdit();
                    e.Handled = true;
                }
                dataGrid.UpdateLayout();
                //sumaAbonos();
            }
        }
        void dataGrid_RowValidating(object sender, RowValidatingEventArgs args)
        {
            GridNumericColumn Colum = ((SfDataGrid)sender).CurrentColumn as GridNumericColumn;
            MessageBox.Show("colum:" + Colum.MappingName.ToString() + "-" + Colum.MappingName.ToString().Trim().Contains("traslEmp").ToString().Trim());
            if (Colum.MappingName.Contains("traslEmp"))
            {
                System.Data.DataRow dr = DtCue.Rows[args.RowIndex - 1];
                decimal _faltante = Convert.ToDecimal(dr["faltante"].ToString());

                var data = args.RowData.GetType().GetProperty(Colum.MappingName.ToString()).GetValue(args.RowData);
                MessageBox.Show(data.ToString());
                decimal totalTrasl = Convert.ToDecimal(sumaAbonos());
                if (totalTrasl > _faltante)
                {
                    args.IsValid = false;
                    args.ErrorMessages.Add("CustomerID", "Customer AROUT cannot be passed");

                }
            }
        }
        private void dataGrid_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs args)
        {

            try
            {
                GridNumericColumn Colum = ((SfDataGrid)sender).CurrentColumn as GridNumericColumn;
                //MessageBox.Show("colum:" + Colum.MappingName.ToString()+"-"+ Colum.MappingName.ToString().Trim().Contains("traslEmp").ToString().Trim());
                string nameColumn = Colum.MappingName.ToString().Trim();
                if (nameColumn.Contains("traslEmp"))
                {

                    System.Data.DataRow dr = DtCue.Rows[args.RowColumnIndex.RowIndex - 1];
                    decimal _faltante = Convert.ToDecimal(dr["faltante"].ToString());
                    //decimal _trasl1 = Convert.ToDecimal(dr["traslEmp1"].ToString());
                    //decimal _trasl2 = Convert.ToDecimal(dr["traslEmp2"].ToString());
                    //decimal _trasl3 = Convert.ToDecimal(dr["traslEmp3"].ToString());
                    //decimal _trasl4 = Convert.ToDecimal(dr["traslEmp4"].ToString());
                    //if ((_trasl1 + _trasl2 + _trasl3 + _trasl4) > _faltante)
                    decimal totalTrasl = Convert.ToDecimal(sumaAbonos(dr["cod_ref"].ToString()));
                    if (totalTrasl > _faltante)
                    {
                        MessageBox.Show("La cantidad trasladada es mayor a la faltante...");
                        dr.BeginEdit();
                        dr[Colum.MappingName] = 0;
                        dr.EndEdit();

                    }
                    else
                    {
                        string nomSaldo = nameColumn.Replace("traslEmp", "saldoEmp");
                        //MessageBox.Show(nomSaldo + "-" + nameColumn);
                        if (Convert.ToDecimal(dr[nomSaldo].ToString()) < Convert.ToDecimal(dr[nameColumn].ToString()))
                        {
                            MessageBox.Show("La cantidad trasladada es mayor al saldo de bodega...");
                            dr.BeginEdit();
                            dr[Colum.MappingName] = 0;
                            dr.EndEdit();
                        }


                        dr.BeginEdit();
                        dr["traslTotal"] = totalTrasl;
                        dr.EndEdit();

                    }
                    // valida la cantidad trasladada vs saldo de la bodega
                    //if (Convert.ToDecimal(dr["saldoEmp1"].ToString()) < Convert.ToDecimal(dr["traslEmp1"].ToString()))
                    sumaAbonos();
                    dataGrid.UpdateLayout();
                    //sumaAbonos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private double sumaAbonos(string codref = "")
        {
            if (!string.IsNullOrEmpty(codref)) codref = "cod_ref='" + codref.Trim() + "'";
            double.TryParse(DtCue.Compute("Sum(faltante)", codref).ToString(), out TotFaltante);
            double.TryParse(DtCue.Compute("Sum(traslEmp1)", codref).ToString(), out TotTraslado1);
            double.TryParse(DtCue.Compute("Sum(traslEmp2)", codref).ToString(), out TotTraslado2);
            double.TryParse(DtCue.Compute("Sum(traslEmp3)", codref).ToString(), out TotTraslado3);
            double.TryParse(DtCue.Compute("Sum(traslEmp4)", codref).ToString(), out TotTraslado4);

            TxtFaltante.Text = TotFaltante.ToString("N2");
            TxtTraslado.Text = (TotTraslado1 + TotTraslado2 + TotTraslado3 + TotTraslado4).ToString("N2");
            TxtPendiente.Text = (TotFaltante - (TotTraslado1 + TotTraslado2 + TotTraslado3 + TotTraslado4)).ToString("N2");
            return TotTraslado1 + TotTraslado2 + TotTraslado3 + TotTraslado4;
        }

        private void DataGrid_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            DataRowView row = (DataRowView)dataGrid.SelectedItems[0];
            string referencia = row["cod_ref"].ToString();


            decimal saldoBod001 = SiaWin.Func.SaldoInv(referencia, "001", cod_empresa);
            Bod001.Text = saldoBod001.ToString();

            decimal saldoBod004 = SiaWin.Func.SaldoInv(referencia, "004", cod_empresa);
            Bod004.Text = saldoBod004.ToString();

            decimal saldoBod010 = SiaWin.Func.SaldoInv(referencia, "010", cod_empresa);
            Bod010.Text = saldoBod010.ToString();

            decimal saldoBod013 = SiaWin.Func.SaldoInv(referencia, "013", cod_empresa);
            Bod013.Text = saldoBod013.ToString();

            decimal saldoBod005 = SiaWin.Func.SaldoInv(referencia, "005", cod_empresa);
            Bod005.Text = saldoBod005.ToString();

            decimal saldoBod009 = SiaWin.Func.SaldoInv(referencia, "009", cod_empresa);
            Bod009.Text = saldoBod009.ToString();

            decimal saldoBod019 = SiaWin.Func.SaldoInv(referencia, "019", cod_empresa);
            Bod019.Text = saldoBod019.ToString();

            decimal saldoBod008 = SiaWin.Func.SaldoInv(referencia, "008", cod_empresa);
            Bod008.Text = saldoBod008.ToString();

            decimal saldoBod052 = SiaWin.Func.SaldoInv(referencia, "052", cod_empresa);
            Bod052.Text = saldoBod052.ToString();

            //saldoBodegas(row["cod_ref"].ToString());
        }


    }


    public class GridSelectionControllerExt : GridSelectionController
    {
        //Inherits the GridSelectionController Class
        private SfDataGrid grid;

        public GridSelectionControllerExt(SfDataGrid datagrid)
            : base(datagrid)
        {
            grid = datagrid;
        }

        //overriding the ProcessKeyDown Event from GridSelectionController base class
        protected override void ProcessKeyDown(KeyEventArgs args)
        {
            var currentKey = args.Key;
            var arguments = new KeyEventArgs(args.KeyboardDevice, args.InputSource, args.Timestamp, Key.Tab)
            {
                RoutedEvent = args.RoutedEvent
            };
            if (currentKey == Key.Enter)
            {
                base.ProcessKeyDown(arguments);
                //assinging the state of Tab key Event handling to Enter key
                args.Handled = arguments.Handled;

                return;
            }
            base.ProcessKeyDown(args);
        }


    }

}
