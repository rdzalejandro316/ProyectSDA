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
        public System.Data.DataRow[] FilasRegistrosAbonos = null;

        int regcab = 0;
        public TrnDocumentoCruce()
        {
            try
            {
                InitializeComponent();
                SiaWin = Application.Current.MainWindow;
                codpvta = SiaWin._UserTag;
                this.DataContext = this;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) this.Close();
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SiaWin = Application.Current.MainWindow;
            TxtNit.Text = codcliente.Trim();
            TxtNomTer.Text = nomter;
            TxtFechaCorte.Text = fechacorte.ToShortDateString();
            TxtCuenta.Text = codcta;
            LoadInfo();

            if (!string.IsNullOrEmpty(codcliente))
                ConsultaSaldoCartera(codcliente);

            if (dataGrid.SelectedIndex < 0) return;
            this.dataGrid.MoveCurrentCell(new RowColumnIndex(1, 9), false);
        }



        //private void ConsultaSaldoCartera1(string codter)
        //{
        //    try
        //    {
        //        //MessageBox.Show(codter+"-"+ codcta+"-"+ fechacorte.ToShortDateString() + "-" + codemp);
        //        dtCue = SiaWin.Func.CarteraCliente(codter, codcta, fechacorte, codemp);
        //        //SiaWin.Browse(dtCue);

        //        if (dtCue.Rows.Count == 0)
        //        {
        //            MessageBox.Show("Sin informacion de cartera");
        //            dataGrid.ItemsSource = null;
        //            //TextCodeCliente.Text = "";
        //            //TextNomCliente.Text = "";
        //            return;
        //        }

        //        dtCue.PrimaryKey = new System.Data.DataColumn[] { dtCue.Columns["num_trn"] };

        //        if (FilasRegistros != null)
        //        {
        //            if (FilasRegistros.Length > 0)
        //            {

        //                for (int i = 0; i < FilasRegistros.Length; i++)
        //                {
        //                    string doccruc = FilasRegistros[i]["doc_cruc"].ToString().Trim();
        //                    if (!string.IsNullOrEmpty(doccruc))
        //                    {
        //                        System.Data.DataRow rowdele = dtCue.Rows.Find(doccruc);
        //                        if (rowdele != null)
        //                        {
        //                            rowdele.BeginEdit();
        //                            rowdele.Delete();
        //                            rowdele.EndEdit();
        //                            dtCue.AcceptChanges();
        //                        }
        //                        ///MessageBox.Show(FilasRegistros[i]["des_mov"].ToString() + "-" + FilasRegistros[i]["doc_cruc"].ToString());
        //                    }
        //                }

        //            }
        //        }

        //        sumaTotal();
        //        dataGrid.ItemsSource = dtCue.DefaultView;
        //        dataGrid.Focus();
        //        if (dtCue.Rows.Count > 0)
        //        {
        //            dataGrid.SelectedIndex = 0;

        //            this.dataGrid.MoveCurrentCell(new RowColumnIndex(1, 9), false);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }

        //}
        private void ConsultaSaldoCartera(string codter)
        {
            try
            {
                string fechacortesp = fechacorte.ToShortDateString();
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds1 = new DataSet();                
                //cmd = new SqlCommand("ConsultaCxcCxp", con);
                cmd = new SqlCommand("_EmpConsultaCxcCxp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TER", codter.Trim());
                cmd.Parameters.AddWithValue("@CTA", codcta.Trim());
                cmd.Parameters.AddWithValue("@Resumen", 1);
                cmd.Parameters.AddWithValue("@fechacorte", fechacortesp);                
                cmd.Parameters.AddWithValue("@codEmp", "010");                
                dtCue.Clear();
                da = new SqlDataAdapter(cmd);
                da.Fill(dtCue);
                con.Close();
                if (dtCue.Rows.Count == 0)
                {
                    MessageBox.Show("Sin informacion de cartera");
                    dataGrid.ItemsSource = null;                    
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
                        }
                    }
                }
                sumaTotal();
                dataGrid.ItemsSource = dtCue.DefaultView;
                dataGrid.Focus();
                dataGrid.SelectedIndex = 0;
                this.dataGrid.MoveCurrentCell(new RowColumnIndex(1, 9), false);
            }
            catch (SqlException exsql)
            {
                MessageBox.Show(exsql.Message);
                return;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void sumaTotal()
        {
            if (dtCue.Rows.Count <= 0) return;
            double.TryParse(dtCue.Compute("Sum(saldo)", "").ToString(), out valorCxC);            
            TotalRecaudo.Text = (valorCxC).ToString("C");
        }

        private void sumaAbonos()
        {
            if (dtCue.Rows.Count <= 0) return;
            double.TryParse(dtCue.Compute("Sum(abono)", "").ToString(), out abonoCxC);
            TotalAbonos.Text = (abonoCxC.ToString("C"));        
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
                sumaAbonos();
            }
            if (e.Key == Key.F5)
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
                FilasRegistrosAbonos = dtCue.Select("abono>0 and num_trn<>'" + doc_cruc + "'", "num_trn");
                if (FilasRegistrosAbonos.Length > 0)
                {
                    for (int i = 0; i < FilasRegistrosAbonos.Length; i++)
                    {
                        //string doccruc = FilasRegistrosAbonos[i]["num_trn"].ToString()+"- Abono:"+ FilasRegistrosAbonos[i]["abono"].ToString();
                        //MessageBox.Show(doccruc);
                    }
                }
                else
                {
                    FilasRegistrosAbonos = null;
                }
                this.Close();
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
                sumaAbonos();
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSeleccionar_Click(object sender, RoutedEventArgs e)
        {
            try
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
                this.Close();
            }
            catch (Exception w)
            {
                MessageBox.Show("error al seleccionar:" + w);
            }
        }



    }

}
