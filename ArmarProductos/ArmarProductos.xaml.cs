using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.UI.Xaml.ScrollAxis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
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
using System.Drawing.Imaging;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;

namespace SiasoftAppExt
{

    //Sia.PublicarPnt(9554,"ArmarProductos");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9554,"ArmarProductos");    
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();


    public partial class ArmarProductos : Window
    {
        public dynamic SiaWin;
        int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public string cod_bod = "";
        public string codpvta = "";

        DataTable dtCue = new DataTable();

        public ArmarProductos()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            LoadConfig();
            loadEgreso();
            Tx_Doc.Text = consecutivo();
        }
        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessIcon"].ToString().Trim());
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                loadImage(idLogo);
                this.Title = "Armar y Desarmar Producto " + cod_empresa + "-" + nomempresa;

                GridConfig.SelectionController = new GridSelectionControllerExt(GridConfig); // enter avance a la siguiente columna
                //GridConfig.CurrentCellMoved += new GridCurrentCellMovedEventHandler(DataGrid_CurrentCellMoved);
            }
            catch (Exception e)
            {
                SiaWin.Func.SiaExeptionGobal(e);
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        public void loadImage(int id)
        {
            try
            {
                string select = "select * from Images where ImageId='" + id + "'";
                DataTable dt = SiaWin.Func.SqlDT(select, "Imagen", 0);
                if (dt.Rows.Count > 0)
                {
                    byte[] blob = (byte[])dt.Rows[0]["Image"];
                    MemoryStream stream = new MemoryStream();
                    stream.Write(blob, 0, blob.Length);
                    stream.Position = 0;
                    System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
                    System.Windows.Media.Imaging.BitmapImage bi = new System.Windows.Media.Imaging.BitmapImage();
                    bi.BeginInit();
                    MemoryStream ms = new MemoryStream();
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    ms.Seek(0, SeekOrigin.Begin);
                    bi.StreamSource = ms;
                    bi.EndInit();
                    this.Icon = bi;
                }

            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error en el loadImage:" + w);
            }
        }

        public void loadEgreso()
        {
            dtCue.Columns.Add("cod_ref");
            dtCue.Columns.Add("nom_ref");
            dtCue.Columns.Add("cantidad", typeof(double));
            dtCue.Columns.Add("saldo", typeof(double));
            dtCue.Columns.Add("faltantes", typeof(double));
            dtCue.Columns.Add("val_ref", typeof(double));
            dtCue.Columns.Add("subtotal", typeof(double));
            dtCue.Rows.Add("", "", 1, 0, 0, 0, 0);
            GridConfig.ItemsSource = dtCue.DefaultView;
            updateTot();
        }

        public string consecutivo()
        {
            string con = "---";
            try
            {

                string sqlConsecutivo = @"declare @fecdoc as datetime;set @fecdoc = getdate(); ";
                sqlConsecutivo += "declare @fecdocsecond as datetime;set @fecdocsecond = DATEADD(second,1,GETDATE()); ";
                sqlConsecutivo += "declare @ini as char(4);declare @num as varchar(12);  ";
                sqlConsecutivo += "declare @iConsecutivo char(12) = '' ;declare @iFolioHost int = 0; ";
                sqlConsecutivo += "SELECT @iFolioHost= isnull(num_act,0)+1,@ini=rtrim(inicial) FROM inmae_trn WHERE cod_trn='147'; ";
                sqlConsecutivo += "set @num=@iFolioHost ";
                sqlConsecutivo += "select @iConsecutivo=rtrim(@ini)+REPLICATE ('0',12-len(rtrim(@ini))-len(rtrim(convert(varchar,@num))))+rtrim(convert(varchar,@num)); ";
                sqlConsecutivo += "select @iConsecutivo as consecutivo;  ";

                DataTable dt = SiaWin.DB.SqlDT(sqlConsecutivo, "cons", idemp);

                if (dt.Rows.Count > 0)
                {
                    con = dt.Rows[0]["consecutivo"].ToString();
                }


            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error en el consecutivo:" + w);
                con = "***";
            }

            return con;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tx_bodega.Text = cod_bod;
            TB_ref.Focus();
        }

        private void TB_ref_LostFocus(object sender, RoutedEventArgs e)
        {
            string text = (sender as TextBox).Text.Trim();
            if (string.IsNullOrEmpty(text)) return;

            string select = "select inmae_ref.cod_ref,inmae_ref.nom_ref,inmae_ref.cod_tip,inmae_tip.nom_tip,inmae_ref.cod_prv,inmae_prv.nom_prv,inmae_ref.cod_tiva,inmae_tiva.por_iva, ";
            select += "inmae_ref.val_ref,inmae_ref.vrunc ";
            select += "from inmae_ref  ";
            select += "inner join inmae_tip on inmae_ref.cod_tip = inmae_tip.cod_tip  ";
            select += "inner join inmae_prv on inmae_ref.cod_prv = inmae_prv.cod_prv ";
            select += "inner join InMae_tiva on inmae_ref.cod_tiva = InMae_tiva.cod_tiva ";
            select += "where inmae_ref.cod_ref='" + text + "' ";


            DataTable dt = SiaWin.Func.SqlDT(select, "referencias", idemp);
            if (dt.Rows.Count > 0)
            {
                TB_ref.Text = dt.Rows[0]["cod_ref"].ToString().Trim();
                TB_NamRef.Text = dt.Rows[0]["nom_ref"].ToString().Trim();
                TB_Codtip.Text = dt.Rows[0]["cod_tip"].ToString().Trim();
                TB_Nomtip.Text = dt.Rows[0]["nom_tip"].ToString().Trim();
                TB_CodPrv.Text = dt.Rows[0]["cod_prv"].ToString().Trim();
                TB_NomPrv.Text = dt.Rows[0]["nom_prv"].ToString().Trim();
                TB_Tiva.Text = dt.Rows[0]["cod_tiva"].ToString().Trim();
                TB_Por.Text = dt.Rows[0]["por_iva"].ToString().Trim();
                tx_val.Text = dt.Rows[0]["val_ref"].ToString().Trim();
                tx_cos.Text = dt.Rows[0]["vrunc"].ToString().Trim();
                double saldoin = Convert.ToDouble(SiaWin.Func.SaldoInv(dt.Rows[0]["cod_ref"].ToString().Trim(), tx_bodega.Text, cod_empresa));
                tx_sal.Text = saldoin.ToString();
            }
            else
            {
                MessageBox.Show("la referencia:" + TB_ref.Text.Trim() + " no existe");
                TB_ref.Text = "";
                TB_NamRef.Text = "";
                TB_Codtip.Text = "";
                TB_Nomtip.Text = "";
                TB_CodPrv.Text = "";
                TB_NomPrv.Text = "";
                TB_Tiva.Text = "";
                TB_Por.Text = "";
                tx_val.Text = "0";
                tx_cos.Text = "0";
            }
        }

        private void TB_ref_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter && !string.IsNullOrEmpty(TB_ref.Text))
            {
                MoveToNextUIElement(e);
                return;
            } 
            


            if (e.Key == Key.F8 || e.Key == Key.Enter)
            {
                int idr = 0; string code = ""; string nombre = "";
                dynamic xx = SiaWin.WindowBuscar("inmae_ref", "cod_ref", "nom_ref", "nom_ref", "idrow", "Maestra de Referencias", cnEmp, false, "", idEmp: idemp);
                xx.ShowInTaskbar = false;
                xx.Owner = Application.Current.MainWindow;
                xx.Height = 400;
                xx.ShowDialog();
                idr = xx.IdRowReturn;
                code = xx.Codigo;
                nombre = xx.Nombre;
                xx = null;

                if (idr > 0)
                {
                    string select = "select inmae_ref.cod_ref,inmae_ref.nom_ref,inmae_ref.cod_tip,inmae_tip.nom_tip,inmae_ref.cod_prv,inmae_prv.nom_prv,inmae_ref.cod_tiva,inmae_tiva.por_iva, ";
                    select += "inmae_ref.val_ref,inmae_ref.vrunc ";
                    select += "from inmae_ref  ";
                    select += "inner join inmae_tip on inmae_ref.cod_tip = inmae_tip.cod_tip  ";
                    select += "inner join inmae_prv on inmae_ref.cod_prv = inmae_prv.cod_prv ";
                    select += "inner join InMae_tiva on inmae_ref.cod_tiva = InMae_tiva.cod_tiva ";
                    select += "where inmae_ref.cod_ref='" + code + "' ";


                    DataTable dt = SiaWin.Func.SqlDT(select, "referencias", idemp);
                    if (dt.Rows.Count > 0)
                    {
                        TB_ref.Text = dt.Rows[0]["cod_ref"].ToString().Trim();
                        TB_NamRef.Text = dt.Rows[0]["nom_ref"].ToString().Trim();
                        TB_Codtip.Text = dt.Rows[0]["cod_tip"].ToString().Trim();
                        TB_Nomtip.Text = dt.Rows[0]["nom_tip"].ToString().Trim();
                        TB_CodPrv.Text = dt.Rows[0]["cod_prv"].ToString().Trim();
                        TB_NomPrv.Text = dt.Rows[0]["nom_prv"].ToString().Trim();
                        TB_Tiva.Text = dt.Rows[0]["cod_tiva"].ToString().Trim();
                        TB_Por.Text = dt.Rows[0]["por_iva"].ToString().Trim();
                        tx_val.Text = dt.Rows[0]["val_ref"].ToString().Trim();
                        tx_cos.Text = dt.Rows[0]["vrunc"].ToString().Trim();
                        double saldoin = Convert.ToDouble(SiaWin.Func.SaldoInv(dt.Rows[0]["cod_ref"].ToString().Trim(), tx_bodega.Text, cod_empresa));
                        tx_sal.Text = saldoin.ToString();

                        MoveToNextUIElement(e);
                    }
                }

            }
        }


        void MoveToNextUIElement(KeyEventArgs e)
        {
            FocusNavigationDirection focusDirection = FocusNavigationDirection.Next;
            TraversalRequest request = new TraversalRequest(focusDirection);
            UIElement elementWithFocus = Keyboard.FocusedElement as UIElement;
            if (elementWithFocus != null)
                if (elementWithFocus.MoveFocus(request)) e.Handled = true;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(TB_ref.Text))
            {
                MessageBox.Show("llene el campo de Referencia que desea armar");
                return;
            }
            if (tx_canti.Value <= 0)
            {
                MessageBox.Show("llene la cantida de la Referencia que desea armar");
                return;
            }

            foreach (System.Data.DataRow dr in dtCue.Rows)
            {
                if (dr == null)
                {
                    MessageBox.Show("llene todos los campos de la grilla");
                    return;
                }
                if (string.IsNullOrEmpty(dr["cod_ref"].ToString()))
                {
                    MessageBox.Show("llene todos los campos en blanco");
                    return;
                }
                double cnt = Convert.ToDouble(dr["cantidad"]);
                if (cnt < 0)
                {
                    MessageBox.Show("la cantidad de una referencia no puede ser 0");
                    return;
                }

                double exis = Convert.ToDouble(dr["faltantes"]);
                if (exis < 0)
                {
                    MessageBox.Show("la column de faltante debe de estar en 0");
                    showInterEmp();
                    return;
                }

            }

            if (generaDocument() == true)
            {
                MessageBox.Show("Documentos creados");
                dtCue.Clear();
                clean();
            }


        }

        private DataTable GetTableSaldosInv()
        {
            DataTable table = new DataTable();
            try
            {

                table.Columns.Add("cod_ref", typeof(string));
                table.Columns.Add("cod_ant", typeof(string));
                table.Columns.Add("nom_ref", typeof(string));
                table.Columns.Add("cod_bod", typeof(string));
                table.Columns.Add("cantidad", typeof(decimal));
                table.Columns.Add("saldo", typeof(decimal));
                table.Columns.Add("faltante", typeof(decimal));
                table.Columns.Add("saldoEmp1", typeof(decimal));
                table.Columns.Add("traslEmp1", typeof(decimal));
                table.Columns.Add("saldoEmp2", typeof(decimal));
                table.Columns.Add("traslEmp2", typeof(decimal));
                table.Columns.Add("saldoEmp3", typeof(decimal));
                table.Columns.Add("traslEmp3", typeof(decimal));
                table.Columns.Add("saldoEmp4", typeof(decimal));
                table.Columns.Add("traslEmp4", typeof(decimal));
                table.Columns.Add("traslTotal", typeof(decimal));
                table.Columns.Add("saldoB001", typeof(decimal));
                table.Columns.Add("saldoB005", typeof(decimal));
                table.Columns.Add("saldoB008", typeof(decimal));
                table.Columns.Add("saldoB010", typeof(decimal));
                //default value
                table.Columns["cantidad"].DefaultValue = 0;
                table.Columns["saldo"].DefaultValue = 0;
                table.Columns["faltante"].DefaultValue = 0;
                table.Columns["saldoEmp1"].DefaultValue = 0;
                table.Columns["traslEmp1"].DefaultValue = 0;
                table.Columns["saldoEmp2"].DefaultValue = 0;
                table.Columns["traslEmp2"].DefaultValue = 0;
                table.Columns["saldoEmp3"].DefaultValue = 0;
                table.Columns["traslEmp3"].DefaultValue = 0;
                table.Columns["saldoEmp4"].DefaultValue = 0;
                table.Columns["traslEmp4"].DefaultValue = 0;

                return table;
            }
            catch (Exception ex)
            {
                SiaWin.Func.SiaExeptionGobal(ex);
                MessageBox.Show(ex.Message, "GetTableSaldosInv");
            }
            return table;
        }

        public DataTable retTableInte()
        {
            //DataTable dtSaldosInvEmpresa = GetTableSaldosInv();
            DataTable dt = GetTableSaldosInv();

            foreach (System.Data.DataRow dr in dtCue.Rows)
            {
                System.Data.DataRow row;
                row = dt.NewRow();
                double diferencia = Convert.ToDouble(dr["cantidad"]) - Convert.ToDouble(dr["saldo"]);
                row["cod_ref"] = dr["cod_ref"].ToString().Trim();
                row["nom_ref"] = dr["nom_ref"].ToString().Trim();
                row["cod_ant"] = dr["nom_ref"].ToString().Trim();
                row["cantidad"] = Convert.ToDouble(dr["cantidad"]);
                row["saldo"] = Convert.ToDouble(dr["cantidad"]);
                row["faltante"] = diferencia;
                dt.Rows.Add(row);
            }
            return dt;
        }

        public void showInterEmp()
        {
            dynamic Pnt9467 = SiaWin.WindowExt(9467, "PvTrasladosAutomaticosEntreEmpresas");  //valida traslados
            Pnt9467.idEmp = idemp;
            Pnt9467.codbod = tx_bodega.Text;
            Pnt9467.DtCue = retTableInte();
            Pnt9467.codpvta = codpvta;
            Pnt9467.ShowInTaskbar = false;
            Pnt9467.Owner = Application.Current.MainWindow;
            Pnt9467.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Pnt9467.ShowDialog();

            //DataTable dtcop = dtCue.Copy();
            //dtCue.Clear();
            foreach (System.Data.DataRow row in dtCue.Rows)
            {
                double saldoin = Convert.ToDouble(SiaWin.Func.SaldoInv(row["cod_ref"].ToString().Trim(), tx_bodega.Text, cod_empresa));
                row["saldo"] = saldoin;
                double cnt = Convert.ToDouble(row["cantidad"]);
                double falt = saldoin - cnt;
                //MessageBox.Show("falt:"+ falt);
                row["faltantes"] = falt >= 0 ? 0 : falt;
                double valref = Convert.ToDouble(row["val_ref"]);
                row["subtotal"] = valref * cnt;
            }
        }

        public void clean()
        {
            TB_ref.Text = "";
            TB_NamRef.Text = "";
            TB_NamRef.Text = "";
            TB_Codtip.Text = "";
            TB_Nomtip.Text = "";
            TB_CodPrv.Text = "";
            TB_NomPrv.Text = "";
            TB_Tiva.Text = "";
            TB_Por.Text = "";
            tx_val.Text = "0";
            tx_cos.Text = "0";
            tx_sal.Text = "0";
            tx_canti.Value = 0;
            Tx_Doc.Text = consecutivo();
            TB_ref.Focus();
        }

        public bool generaDocument()
        {
            bool bandera = false;

            if (MessageBox.Show("Usted desea guardar el documento..?", "Guardar Traslado", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {

                using (SqlConnection connection = new SqlConnection(cnEmp))
                {
                    connection.Open();
                    StringBuilder errorMessages = new StringBuilder();
                    SqlCommand command = connection.CreateCommand();
                    SqlTransaction transaction;
                    transaction = connection.BeginTransaction("Transaction");
                    command.Connection = connection;
                    command.Transaction = transaction;

                    try
                    {
                        string CodSal = "147";
                        string CodEnt = "057";

                        string sqlcabSal = "";
                        string sqlcabEntra = "";

                        string sqlConsecutivo = @"declare @fecdoc as datetime;set @fecdoc = getdate();";
                        sqlConsecutivo = sqlConsecutivo + "declare @fecdocsecond as datetime;set @fecdocsecond = DATEADD(second,1,GETDATE()); ";
                        sqlConsecutivo = sqlConsecutivo + "declare @iniSal as char(4);declare @numSal as varchar(12); ";
                        sqlConsecutivo = sqlConsecutivo + "declare @iniEnt as char(4);declare @numEnt as varchar(12); ";

                        sqlConsecutivo = sqlConsecutivo + "declare @ConsecutivoSalida char(12) = '' ;declare @TempSal int = 0; ";
                        sqlConsecutivo = sqlConsecutivo + "declare @ConsecutivoEntrada char(12) = '' ;declare @TempEnt int = 0; ";

                        sqlConsecutivo = sqlConsecutivo + "SELECT @TempSal= isnull(num_act,0)+1,@iniSal=rtrim(inicial) FROM inmae_trn WHERE cod_trn='147';  ";
                        sqlConsecutivo = sqlConsecutivo + "SELECT @TempEnt= isnull(num_act,0)+1,@iniEnt=rtrim(inicial) FROM inmae_trn WHERE cod_trn='057';  ";

                        sqlConsecutivo = sqlConsecutivo + "set @numSal=@TempSal; set @numEnt=@TempEnt ";
                        sqlConsecutivo = sqlConsecutivo + "select @ConsecutivoSalida=rtrim(@iniSal)+REPLICATE ('0',12-len(rtrim(@iniSal))-len(rtrim(convert(varchar,@numSal))))+rtrim(convert(varchar,@numSal)); ";
                        //sqlConsecutivo = sqlConsecutivo + "select @ConsecutivoEntrada=rtrim(@iniEnt)+REPLICATE ('0',12-len(rtrim(@iniEnt))-len(rtrim(convert(varchar,@numEnt))))+rtrim(convert(varchar,@numEnt));  ";

                        sqlConsecutivo = sqlConsecutivo + "update inmae_trn set  num_act=isnull(num_act,0)+1 WHERE cod_trn='147'; ";
                        //sqlConsecutivo = sqlConsecutivo + "update inmae_trn set  num_act=isnull(num_act,0)+1 WHERE cod_trn='057'; ";


                        sqlcabSal = sqlConsecutivo + @"INSERT INTO incab_doc (cod_trn,fec_trn,num_trn,doc_ref,des_mov) values ('" + CodSal + "',@fecdoc, @ConsecutivoSalida, @ConsecutivoSalida,'Salida de Producto');DECLARE @NewIDSal INT;SELECT @NewIDSal = SCOPE_IDENTITY();";
                        sqlcabEntra = @"INSERT INTO incab_doc (cod_trn,fec_trn,num_trn,doc_ref,des_mov) values ('" + CodEnt + "',@fecdocsecond,@ConsecutivoSalida,@ConsecutivoEntrada,'Entrada de productos');DECLARE @NewIDEnt INT;SELECT @NewIDEnt = SCOPE_IDENTITY();";


                        string sqlCueSal = "";
                        string sqlCueEnt = "";

                        if (CodEnt == "057")
                        {
                            double cnt = Convert.ToDouble(tx_canti.Value);
                            double val_ref = Convert.ToDouble(tx_val.Value);
                            double total = val_ref * cnt;
                            sqlCueEnt += @"INSERT INTO incue_doc (idregcab,cod_trn,num_trn,fecha_aded,cod_ref,cod_bod,cantidad,val_uni,subtotal,tot_tot) values (@NewIDEnt,'" + CodEnt + "',@ConsecutivoSalida,@fecdoc,'" + TB_ref.Text + "','" + tx_bodega.Text + "'," + cnt.ToString("F", CultureInfo.InvariantCulture) + "," + val_ref.ToString("F", CultureInfo.InvariantCulture) + "," + total + "," + total + ");";
                        }

                        if (CodSal == "147")
                        {
                            foreach (System.Data.DataRow item in dtCue.Rows)
                            {
                                string cod_ref = item["cod_ref"].ToString();
                                decimal cantidad = Convert.ToDecimal(item["cantidad"]);
                                decimal val_ref = Convert.ToDecimal(item["val_ref"]);
                                decimal subtotal = Convert.ToDecimal(item["subtotal"]);

                                sqlCueSal += @"INSERT INTO incue_doc (idregcab,cod_trn,num_trn,fecha_aded,cod_ref,cod_bod,cantidad,val_uni,subtotal,tot_tot) values (@NewIDSal,'" + CodSal + "',@ConsecutivoSalida,@fecdoc,'" + cod_ref + "','" + tx_bodega.Text + "'," + cantidad.ToString("F", CultureInfo.InvariantCulture) + "," + val_ref.ToString("F", CultureInfo.InvariantCulture) + "," + subtotal.ToString("F", CultureInfo.InvariantCulture) + "," + subtotal.ToString("F", CultureInfo.InvariantCulture) + ");";
                            }
                        }


                        command.CommandText = sqlcabSal + sqlCueSal + sqlcabEntra + sqlCueEnt + @"select CAST(@NewIDSal AS int);";
                        //MessageBox.Show("command.CommandText:"+ command.CommandText);
                        var r = new object();
                        r = command.ExecuteScalar();
                        transaction.Commit();
                        connection.Close();
                        bandera = true;
                    }
                    catch (Exception ex)
                    {
                        SiaWin.Func.SiaExeptionGobal(ex);
                        errorMessages.Append("error al generar el documento:" + ex);
                        transaction.Rollback();
                        MessageBox.Show(errorMessages.ToString());
                        bandera = false;
                    }
                }
            }
            else
            {
                bandera = false; MessageBox.Show("mierda1");
            }

            return bandera;
        }

        private void BtnCancl_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void GridConfig_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
        {
            try
            {
                var reflector = this.GridConfig.View.GetPropertyAccessProvider();

                //if (GridConfig.View.IsCurrentBeforeFirst == true)return;

                if (Keyboard.IsKeyDown(Key.Up))                
                    return;
                
                

                if (Keyboard.IsKeyDown(Key.Right) || Keyboard.IsKeyDown(Key.Tab) || Keyboard.IsKeyDown(Key.Return) || Keyboard.IsKeyDown(Key.Enter))
                {
                    if (GridConfig.View.IsCurrentBeforeFirst == true) return;
                }

                

                var rowData = GridConfig.GetRecordAtRowIndex(e.RowColumnIndex.RowIndex);
                if (DBNull.Value.Equals(reflector.GetValue(rowData, "cod_ref"))) return;

                string refer = reflector.GetValue(rowData, "cod_ref").ToString();


                var func = refe(refer);
                if (func.Item1)
                {
                    reflector.SetValue(rowData, "nom_ref", func.Item2.Rows[0]["nom_ref"].ToString().Trim()).ToString();
                    double getCant = 1;

                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "cantidad")))
                        reflector.SetValue(rowData, "cantidad", 1);

                    getCant = Convert.ToDouble(reflector.GetValue(rowData, "cantidad"));
                    //if (getCant ==0) reflector.SetValue(rowData, "cantidad", 1);                   
                    double saldoin = Convert.ToDouble(SiaWin.Func.SaldoInv(func.Item2.Rows[0]["cod_ref"].ToString().Trim(), tx_bodega.Text, cod_empresa));
                    reflector.SetValue(rowData, "saldo", saldoin);
                    double fal = saldoin - getCant;
                    reflector.SetValue(rowData, "faltantes", fal >= 0 ? 0 : fal);
                    reflector.SetValue(rowData, "val_ref", Convert.ToDouble(func.Item2.Rows[0]["val_ref"].ToString()));
                    double sub = Convert.ToDouble(func.Item2.Rows[0]["val_ref"]) * getCant;
                    reflector.SetValue(rowData, "subtotal", sub);
                    GridConfig.UpdateDataRow(e.RowColumnIndex.RowIndex);
                    GridConfig.UpdateLayout();
                    GridConfig.Columns["nom_ref"].AllowEditing = false;
                    GridConfig.Columns["cantidad"].AllowEditing = true;
                    GridConfig.Columns["saldo"].AllowEditing = false;
                    GridConfig.Columns["faltantes"].AllowEditing = false;
                    GridConfig.Columns["val_ref"].AllowEditing = false;
                    GridConfig.Columns["subtotal"].AllowEditing = false;
                    updateTot();
                    return;
                }
                else
                {
                    reflector.SetValue(rowData, "cod_ref", "");
                    reflector.SetValue(rowData, "nom_ref", "");
                    reflector.SetValue(rowData, "cantidad", 0);
                    reflector.SetValue(rowData, "saldo", 0);
                    reflector.SetValue(rowData, "faltantes", 0);
                    reflector.SetValue(rowData, "subtotal", 0);

                    GridConfig.UpdateDataRow(e.RowColumnIndex.RowIndex);
                    GridConfig.UpdateLayout();
                    GridConfig.Columns["nom_ref"].AllowEditing = false;
                    GridConfig.Columns["cantidad"].AllowEditing = true;
                    GridConfig.Columns["saldo"].AllowEditing = false;
                    GridConfig.Columns["faltantes"].AllowEditing = false;
                    GridConfig.Columns["val_ref"].AllowEditing = false;
                    GridConfig.Columns["subtotal"].AllowEditing = false;
                    updateTot();
                    return;
                }
            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error al editar:" + w);
            }
        }

        public Tuple<bool, DataTable> refe(string refe)
        {
            string select = "select * from inmae_ref where cod_ref='" + refe + "'";
            DataTable dt = new DataTable();
            dt = SiaWin.Func.SqlDT(select, "referencias", idemp);
            var tuple = new Tuple<bool, DataTable>(dt.Rows.Count > 0 ? true : false, dt);
            return tuple;
        }
        void updateTot()
        {
            Tx_tot.Text = dtCue.Rows.Count.ToString();
        }

        private void GridConfig_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                GridColumn Colum = ((SfDataGrid)sender).CurrentColumn as GridColumn;

                if (Colum.MappingName == "cod_ref")
                {
                    if ((e.Key == Key.Enter || e.Key == Key.F8))
                    {
                        if (GridConfig.SelectedIndex == -1)
                            this.GridConfig.SelectionController.CurrentCellManager.BeginEdit();

                        dynamic ww = SiaWin.WindowExt(9326, "InBuscarReferencia");  //carga desde sql
                        ww.Conexion = SiaWin.Func.DatosEmp(idemp);
                        ww.idEmp = idemp;
                        ww.idBod = tx_bodega.Text;
                        ww.UltBusqueda = "";
                        ww.ShowInTaskbar = false;
                        ww.Owner = Application.Current.MainWindow;
                        ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        ww.Height = 400;
                        ww.ShowDialog();

                        string code = ww.Codigo;

                        if (!string.IsNullOrEmpty(code))
                        {
                            var func = refe(code);
                            if (func.Item1)
                            {
                                var reflector = this.GridConfig.View.GetPropertyAccessProvider();
                                int columnIndex = (sender as SfDataGrid).SelectionController.CurrentCellManager.CurrentRowColumnIndex.RowIndex;
                                var rowData = GridConfig.GetRecordAtRowIndex(columnIndex);


                                reflector.SetValue(rowData, "cod_ref", func.Item2.Rows[0]["cod_ref"].ToString().Trim());
                                reflector.SetValue(rowData, "nom_ref", func.Item2.Rows[0]["nom_ref"].ToString().Trim());
                                double getCant = 1;
                                if (DBNull.Value.Equals(reflector.GetValue(rowData, "cantidad")))
                                    reflector.SetValue(rowData, "cantidad", 1);
                                getCant = Convert.ToDouble(reflector.GetValue(rowData, "cantidad"));
                                double saldoin = Convert.ToDouble(SiaWin.Func.SaldoInv(func.Item2.Rows[0]["cod_ref"].ToString().Trim(), tx_bodega.Text, cod_empresa));
                                reflector.SetValue(rowData, "saldo", saldoin);
                                double fal = saldoin - getCant;
                                reflector.SetValue(rowData, "faltantes", fal >= 0 ? 0 : fal);
                                reflector.SetValue(rowData, "val_ref", Convert.ToDouble(func.Item2.Rows[0]["val_ref"].ToString()));
                                double sub = Convert.ToDouble(func.Item2.Rows[0]["val_ref"]) * getCant;
                                reflector.SetValue(rowData, "subtotal", sub);

                                GridConfig.UpdateDataRow(columnIndex);
                                GridConfig.UpdateLayout();
                                GridConfig.Columns["cod_ref"].AllowEditing = true;

                                updateTot();
                            }
                        }

                    }
                }

            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error en el producto f8:" + w);
            }
        }

        private void GridConfig_CurrentCellActivating(object sender, CurrentCellActivatingEventArgs e)
        {
            try
            {


                if (e.CurrentRowColumnIndex.ColumnIndex == 1 || e.CurrentRowColumnIndex.ColumnIndex == 7)
                    GridConfig.AddNewRowPosition = AddNewRowPosition.Bottom;
                else
                    GridConfig.AddNewRowPosition = AddNewRowPosition.None;
                //GridConfig.UpdateLayout();
            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error:" + w);
            }
        }

        public class GridSelectionControllerExt :  GridSelectionController
        {
            private SfDataGrid grid;
            public GridSelectionControllerExt(SfDataGrid datagrid) : base(datagrid)
            {
                grid = datagrid;
            }
            protected override void ProcessKeyDown(KeyEventArgs args)
            {
                try
                {
                    var currentKey = args.Key;
                    var arguments = new KeyEventArgs(args.KeyboardDevice, args.InputSource, args.Timestamp, Key.Tab)
                    {
                        RoutedEvent = args.RoutedEvent
                    };
                    if (currentKey == Key.Enter)
                    {
                        if (grid.IsReadOnly == false && grid.CurrentColumn is GridTextColumn) { }
                        base.ProcessKeyDown(arguments);
                        args.Handled = arguments.Handled;
                        return;
                    }

                    if (currentKey == Key.Up)
                    {
                        if (grid.View.IsAddingNew == true && grid.View.IsCurrentBeforeFirst == true)
                        {
                            grid.View.CancelEdit();
                            grid.View.CancelNew();
                        }
                        grid.UpdateLayout();
                    }


                    base.ProcessKeyDown(args);
                }
                catch (Exception w)
                {
                    MessageBox.Show("errro:::" + w);
                }
            }
        }

        private void GridConfig_CurrentCellActivated(object sender, CurrentCellActivatedEventArgs e)
        {
            try
            {
                bool t = this.GridConfig.View.IsAddingNew;

                if (!t)
                {
                    if ((e.CurrentRowColumnIndex.RowIndex) > GridConfig.View.Records.Count)
                    {
                        if (e.CurrentRowColumnIndex.ColumnIndex > 0)
                            this.GridConfig.SelectionController.CurrentCellManager.BeginEdit();
                    }
                    else
                    {
                        GridConfig.UpdateLayout();
                        var reflector = this.GridConfig.View.GetPropertyAccessProvider();
                        int columnIndex = (sender as SfDataGrid).SelectionController.CurrentCellManager.CurrentRowColumnIndex.RowIndex;
                        var rowData = GridConfig.GetRecordAtRowIndex(columnIndex);
                        string refe = reflector.GetValue(rowData, "cod_ref").ToString().Trim();

                        if (string.IsNullOrEmpty(refe))
                            this.GridConfig.SelectionController.CurrentCellManager.BeginEdit();
                    }
                }

                if (Keyboard.IsKeyDown(Key.Tab) || Keyboard.IsKeyDown(Key.Right) || Keyboard.IsKeyDown(Key.Return))
                {
                    var reflector = this.GridConfig.View.GetPropertyAccessProvider();
                    int columnIndex = (sender as SfDataGrid).SelectionController.CurrentCellManager.CurrentRowColumnIndex.RowIndex;
                    var rowData = GridConfig.GetRecordAtRowIndex(columnIndex);
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "cod_ref")))
                    {
                        dynamic ww = SiaWin.WindowExt(9326, "InBuscarReferencia");  //carga desde sql
                        ww.Conexion = SiaWin.Func.DatosEmp(idemp);
                        ww.idEmp = idemp;
                        ww.idBod = tx_bodega.Text;
                        ww.UltBusqueda = "";
                        ww.ShowInTaskbar = false;
                        ww.Owner = Application.Current.MainWindow;
                        ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        ww.Height = 400;
                        ww.ShowDialog();
                        string code = ww.Codigo;

                        if (string.IsNullOrEmpty(code))return;                        

                        var func = refe(code);
                        reflector.SetValue(rowData, "cod_ref", func.Item2.Rows[0]["cod_ref"].ToString().Trim());
                        reflector.SetValue(rowData, "nom_ref", func.Item2.Rows[0]["nom_ref"].ToString().Trim());
                        double getCant = 1;
                        if (DBNull.Value.Equals(reflector.GetValue(rowData, "cantidad")))
                            reflector.SetValue(rowData, "cantidad", 1);

                        double saldoin = Convert.ToDouble(SiaWin.Func.SaldoInv(func.Item2.Rows[0]["cod_ref"].ToString().Trim(), tx_bodega.Text, cod_empresa));
                        reflector.SetValue(rowData, "saldo", saldoin);
                        double fal = saldoin - getCant;
                        reflector.SetValue(rowData, "faltantes", fal >= 0 ? 0 : fal);
                        reflector.SetValue(rowData, "val_ref", Convert.ToDouble(func.Item2.Rows[0]["val_ref"].ToString()));
                        double sub = Convert.ToDouble(func.Item2.Rows[0]["val_ref"]) * getCant;
                        reflector.SetValue(rowData, "subtotal", sub);


                        GridConfig.UpdateDataRow(columnIndex);
                        GridConfig.UpdateLayout();
                        GridConfig.Columns["cod_ref"].AllowEditing = true;
                    }

                    else
                    {
                        string referen = reflector.GetValue(rowData, "cod_ref").ToString().Trim();

                        if (string.IsNullOrEmpty(referen))
                        {
                            dynamic ww = SiaWin.WindowExt(9326, "InBuscarReferencia");  //carga desde sql
                            ww.Conexion = SiaWin.Func.DatosEmp(idemp);
                            ww.idEmp = idemp;
                            ww.idBod = tx_bodega.Text;
                            ww.UltBusqueda = "";
                            ww.ShowInTaskbar = false;
                            ww.Owner = Application.Current.MainWindow;
                            ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                            ww.Height = 400;
                            ww.ShowDialog();
                            string code = ww.Codigo;

                            if (string.IsNullOrEmpty(code)) return;

                            var func = refe(code);

                            reflector.SetValue(rowData, "cod_ref", func.Item2.Rows[0]["cod_ref"].ToString().Trim());
                            reflector.SetValue(rowData, "nom_ref", func.Item2.Rows[0]["nom_ref"].ToString().Trim());
                            double getCant = 1;
                            if (DBNull.Value.Equals(reflector.GetValue(rowData, "cantidad")))
                                reflector.SetValue(rowData, "cantidad", 1);

                            double saldoin = Convert.ToDouble(SiaWin.Func.SaldoInv(func.Item2.Rows[0]["cod_ref"].ToString().Trim(), tx_bodega.Text, cod_empresa));
                            reflector.SetValue(rowData, "saldo", saldoin);
                            double fal = saldoin - getCant;
                            reflector.SetValue(rowData, "faltantes", fal >= 0 ? 0 : fal);
                            reflector.SetValue(rowData, "val_ref", Convert.ToDouble(func.Item2.Rows[0]["val_ref"].ToString()));
                            double sub = Convert.ToDouble(func.Item2.Rows[0]["val_ref"]) * getCant;
                            reflector.SetValue(rowData, "subtotal", sub);

                            GridConfig.UpdateDataRow(columnIndex);
                            GridConfig.UpdateLayout();
                            GridConfig.Columns["cod_ref"].AllowEditing = true;
                            
                        }
                    }
                }

            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("QQQ:" + w);
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.F5)
                    BtnSave.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error en el key down" + w);
            }
        }


        private void Tx_canti_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab || e.Key == Key.Enter)
            {
                GridConfig.MoveCurrentCell(new RowColumnIndex(1, 1), false);
                GridConfig.ScrollInView(new RowColumnIndex(1, 1));
                MoveToNextUIElement(e);
                MoveToNextUIElement(e);
                MoveToNextUIElement(e);
            }
        }

        private void GridConfig_CurrentCellValidating(object sender, CurrentCellValidatingEventArgs e)
        {
            try
            {
                var reflector = this.GridConfig.View.GetPropertyAccessProvider();
                int columnIndex = (sender as SfDataGrid).SelectionController.CurrentCellManager.CurrentRowColumnIndex.RowIndex;
                var rowData = GridConfig.GetRecordAtRowIndex(columnIndex);
                //{
                //        dynamic ww = SiaWin.WindowExt(9326, "InBuscarReferencia");  //carga desde sql
                //        ww.Conexion = SiaWin.Func.DatosEmp(idemp);
                //        ww.idEmp = idemp;
                //        ww.idBod = tx_bodega.Text;
                //        ww.UltBusqueda = "";
                //        ww.ShowInTaskbar = false;
                //        ww.Owner = Application.Current.MainWindow;
                //        ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                //        ww.Height = 400;
                //        ww.ShowDialog();
                //        string code = ww.Codigo;
                //        reflector.SetValue(rowData, "cod_ref", code);
                //        GridConfig.UpdateDataRow(columnIndex);
                //        GridConfig.UpdateLayout();
                //        GridConfig.Columns["cod_ref"].AllowEditing = true;
                //    }
                //    else
                //    {
                //        string refe = reflector.GetValue(rowData, "cod_ref").ToString().Trim();

                //        if (string.IsNullOrEmpty(refe))
                //        {
                //            dynamic ww = SiaWin.WindowExt(9326, "InBuscarReferencia");  //carga desde sql
                //            ww.Conexion = SiaWin.Func.DatosEmp(idemp);
                //            ww.idEmp = idemp;
                //            ww.idBod = tx_bodega.Text;
                //            ww.UltBusqueda = "";
                //            ww.ShowInTaskbar = false;
                //            ww.Owner = Application.Current.MainWindow;
                //            ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                //            ww.Height = 400;
                //            ww.ShowDialog();
                //            string code = ww.Codigo;
                //            reflector.SetValue(rowData, "cod_ref", code);
                //            GridConfig.UpdateDataRow(columnIndex);
                //            GridConfig.UpdateLayout();
                //            GridConfig.Columns["cod_ref"].AllowEditing = true;
                //        }
                //    }

            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error Activiting:" + w);
            }
        }

        private void BtnCancl_LostFocus(object sender, RoutedEventArgs e)
        {
            
        }



    }
}

