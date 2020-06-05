using Syncfusion.Windows.Controls.Grid.Converter;

using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
//using Syncfusion.UI.Xaml.Grid.Converter;
//using RecibosDeCaja;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.ScrollAxis;

namespace SiasoftAppExt
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    /// Sia.PublicarPnt(9381,"TrnDocumentoCruce");

    public partial class TrnDocumentoCruce : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        public string codter = "";
        public string nomter = "";
        string codbod = "";
        string codpvta = "";
        string nompvta = "";
        string codcco = "";
        string nitemp = "";
        string cnEmp = "";
        string codemp = "";
        int idLogo = 0;
        DataSet ds = new DataSet();
        DataTable dtCue = new DataTable();
        double valorCxC = 0;
        double valorCxCAnt = 0;
        double abonoCxC = 0;
        double abonoCxCAnt = 0;
        public string codcliente = "";
        public string codcta = "";
        public DateTime fechacorte = DateTime.Now.Date;
        public string doc_cruc = "";
        public string cod_trn = "";
        public string doc_ref = "";
        public decimal valabono = 0;
        public System.Data.DataRow[] FilasRegistros;
        int regcab = 0;
        public TrnDocumentoCruce()
        {
            try
            {
                InitializeComponent();
                SiaWin = Application.Current.MainWindow;
                //idemp = SiaWin._BusinessId;
                codpvta = SiaWin._UserTag;
                //LoadInfo();
                this.DataContext = this;
                //                BtbGrabar.Focus();
                //string ValorString = SiaWin.ValorString.ToString();
                //if (SiaWin.ValReturn != null) // inicia con una cedula
                //{
                  //  MessageBox.Show("entra diferentexxx"+ SiaWin.ValReturn.ToString());
                    //string codter = SiaWin.ValReturn.ToString();
                    //                SiaWin.ValReturn.ToString();
                    //InitRC(codter);
               // }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void InitRC(string cod_ter)
        {
            //BtbGrabar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            //TextCodeCliente.Text = cod_ter;
            //CmbBan.Focus();
            //TextCodeCliente.Focus();
            //BtbGrabar.Focus();
            //CmbBan.Focus();
            //ActualizaCampos(cod_ter);
            ConsultaSaldoCartera1(cod_ter);
            //TextCodeCliente.Focus();
            //dirter = SiaWin.Func.cmpCodigo("comae_ter", "cod_ter", "dir1", TextCodeCliente.Text, idemp);
            //telter = SiaWin.Func.cmpCodigo("comae_ter", "cod_ter", "tel1", TextCodeCliente.Text, idemp);

            //var uiElement = this as UIElement;
            //uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            //uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            //MessageBox.Show("ini");
            //TextCodeCliente.Focus();

        }
        private void ConsultaSaldoCartera1(string codter)
        {
            try
            {
                //MessageBox.Show(codter+"-"+ codcta+"-"+ fechacorte.ToShortDateString() + "-" + codemp);
                dtCue = SiaWin.Func.CarteraCliente(codter,codcta,fechacorte, codemp);
                if (dtCue.Rows.Count == 0)
                {
                    MessageBox.Show("Sin informacion de cartera");
                    dataGrid.ItemsSource = null;
                    //TextCodeCliente.Text = "";
                    //TextNomCliente.Text = "";
                    return;
                }
                dtCue.PrimaryKey = new System.Data.DataColumn[] { dtCue.Columns["num_trn"] };
                if (FilasRegistros.Length > 0)
                {
                    for (int i = 0; i < FilasRegistros.Length; i++)
                    {
                        string doccruc = FilasRegistros[i]["doc_cruc"].ToString();
                        if (!string.IsNullOrEmpty(doccruc))
                        {
                            System.Data.DataRow rowdele = dtCue.Rows.Find(doccruc);
                            rowdele.BeginEdit();
                            rowdele.Delete();
                            rowdele.EndEdit();
                            dtCue.AcceptChanges();
                            ///MessageBox.Show(FilasRegistros[i]["des_mov"].ToString() + "-" + FilasRegistros[i]["doc_cruc"].ToString());
                        }
                    }

                }
                sumaTotal();
                dataGrid.ItemsSource = dtCue.DefaultView;
                dataGrid.Focus();
                if (dtCue.Rows.Count > 0)
                {
                    dataGrid.SelectedIndex = 0;

                    this.dataGrid.MoveCurrentCell(new RowColumnIndex(1, 9), false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void ConsultaSaldoCartera(string codter)
        {
            try
            {
                string fechacortesp = fechacorte.ToShortDateString();
                SqlConnection con = new SqlConnection(cnEmp);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds1 = new DataSet();
                //cmd = new SqlCommand("ConsultaCxcCxpDeta", con);
                cmd = new SqlCommand("ConsultaCxcCxp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Ter", codter.Trim());//if you have parameters.
                cmd.Parameters.AddWithValue("@CTA", codcta.Trim());//if you have parameters.
                cmd.Parameters.AddWithValue("@Resumen", 1);//if you have parameters.
                cmd.Parameters.AddWithValue("@fechacorte", fechacortesp);//if you have parameters.
                //cmd.Parameters.AddWithValue("@Where", where);//if you have parameters.
                dtCue.Clear();
                da = new SqlDataAdapter(cmd);
                da.Fill(dtCue);
                con.Close();
                if (dtCue.Rows.Count == 0)
                {
                    MessageBox.Show("Sin informacion de cartera");
                    dataGrid.ItemsSource = null;
                    //TextCodeCliente.Text = "";
                    //TextNomCliente.Text = "";
                    return;
                }
                dtCue.PrimaryKey = new System.Data.DataColumn[] { dtCue.Columns["num_trn"] };
                if (FilasRegistros.Length > 0)
                {
                    for (int i = 0; i < FilasRegistros.Length; i++)
                    {
                        string doccruc = FilasRegistros[i]["doc_cruc"].ToString();
                        if (!string.IsNullOrEmpty(doccruc))
                        {
                            System.Data.DataRow rowdele = dtCue.Rows.Find(doccruc);
                            rowdele.BeginEdit();
                            rowdele.Delete();
                            rowdele.EndEdit();
                            dtCue.AcceptChanges();
                            ///MessageBox.Show(FilasRegistros[i]["des_mov"].ToString() + "-" + FilasRegistros[i]["doc_cruc"].ToString());
                        }
                    }

                }


                sumaTotal();
                dataGrid.ItemsSource = dtCue.DefaultView;
                dataGrid.Focus();
                dataGrid.SelectedIndex = 0;

                this.dataGrid.MoveCurrentCell(new RowColumnIndex(1, 9), false);
            }
            catch(SqlException exsql)
            {
                MessageBox.Show(exsql.Message);
                return;

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void sumaTotal()
        {
            if (dtCue.Rows.Count <= 0) return;
            double.TryParse(dtCue.Compute("Sum(saldo)", "").ToString(), out valorCxC);
            //double.TryParse(dtCue.Compute("Sum(valor)", "tip_apli=4").ToString(), out valorCxCAnt);
            //double valorA = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(valor)", "tip_apli=1 or tip_apli=4").ToString());
            //double saldo = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(saldo)", "tip_apli=2 or tip_apli=3").ToString());
            //            TextCxC.Text = saldoCxC.ToString("C");
            //          TextCxCAnt.Text = saldoCxCAnt.ToString("C");
            TotalRecaudo.Text = (valorCxC).ToString("C");
        }
        private void sumaAbonos()
        {
            if (dtCue.Rows.Count <= 0) return;
            //double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=3").ToString(), out abonoCxC);
            //double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=4").ToString(), out abonoCxCAnt);
            //TextCxCAbono.Text = abonoCxC.ToString("C");
            //TextCxCAntAbono.Text = abonoCxCAnt.ToString("C");
            //TotalRecaudo.Text = (abonoCxC - abonoCxCAnt - abonoCxP + abonoCxPAnt).ToString("C");
        }
        public void LoadInfo()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                codemp = foundRow["BusinessCode"].ToString().Trim();
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                nitemp = foundRow["BusinessNit"].ToString().Trim();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
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
                    System.Data.DataRow dr = dtCue.Rows[dataGrid.SelectedIndex];
                    dr.BeginEdit();
                    decimal _cnt = Convert.ToDecimal(dr["saldo"].ToString());
                    dr["abono"] = _cnt;
                    dr.EndEdit();
                    e.Handled = true;
                }
                dataGrid.UpdateLayout();
                //sumaAbonos();
            }
            if(e.Key==Key.F5)
            {

                DataRowView row = (DataRowView)dataGrid.SelectedItems[0];
                if (row == null)
                {
                    MessageBox.Show("Registro sin datos");
                    return;
                }
                decimal _abono = Convert.ToDecimal(row["abono"].ToString());
                if (_abono > 0)
                {
                    doc_cruc = row["num_trn"].ToString();
                    doc_ref = row["factura"].ToString();
                    cod_trn = row["cod_trn"].ToString();
                    valabono = _abono;
                    this.Close();
                }


            }

        }

        private void dataGrid_CurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs e)
        {
            GridNumericColumn Colum = ((SfDataGrid)sender).CurrentColumn as GridNumericColumn;
            if (Colum.MappingName == "abono")
            {
                System.Data.DataRow dr = dtCue.Rows[dataGrid.SelectedIndex];
                decimal _saldo = Convert.ToDecimal(dr["saldo"].ToString());
                decimal _abono = Convert.ToDecimal(dr["abono"].ToString());
                if (_abono > _saldo)
                {
                    MessageBox.Show("El valor abonado es mayor al saldo...");
                    dr.BeginEdit();
                    dr["abono"] = 0;
                    dr.EndEdit();
                    
                }
                dataGrid.UpdateLayout();
                //sumaAbonos();
            }

        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //MessageBox.Show("sss");
            //if (dtCue.Rows.Count > 0) e.Cancel = true;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
           


            if (e.Key == Key.Escape)
            {
                this.Close();
                //if (BtbGrabar.Content.ToString().Trim() == "Grabar")
                //{
                //  BtbCancelar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                //e.Handled = false;
                //return;
                //}
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TxtNit.Text = codcliente;
            TxtNomTer.Text = nomter;
            TxtFechaCorte.Text = fechacorte.ToShortDateString();
            TxtCuenta.Text = codcta;
            LoadInfo();
//            MessageBox.Show(FilasRegistros.Length.ToString());
            //if(dtCuerpo!=null) MessageBox.Show(dtCuerpo.Rows.Count.ToString());
            if (!string.IsNullOrEmpty(codcliente))
            {
                InitRC(codcliente);
            }
            if (dataGrid.SelectedIndex < 0) return;
            this.dataGrid.MoveCurrentCell(new RowColumnIndex(1, 9), false);
            if (!string.IsNullOrEmpty(codter))
            {
                //MessageBox.Show("focus");
//                CmbBan.Focus();
            }
  //          CmbBan.Focus();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            this.Close();
        }
    }

}
