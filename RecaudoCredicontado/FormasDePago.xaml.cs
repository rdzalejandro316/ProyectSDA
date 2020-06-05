using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.UI.Xaml.ScrollAxis;
using System;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace RecaudoCredicontado
{
    public partial class FormasDePago : Window
    {
        dynamic SiaWin;
        DataTable dtCue = new DataTable();
        DataTable dtBan = new DataTable();
        int idemp = 0;
        public string NomCliente = string.Empty;
        decimal totalPagar = 0;

        string cnEmp = string.Empty;

        public string recibo_prov = "";
        public string vendedor = "";

        public FormasDePago()
        {
            try
            {
                InitializeComponent();
                SiaWin = Application.Current.MainWindow;
                idemp = SiaWin._BusinessId;
                if (SiaWin.ValReturn != null) totalPagar = Convert.ToDecimal(SiaWin.ValReturn.ToString());
                TxtTotalRecaudo.Text = totalPagar.ToString("C2");
                cnEmp = SiaWin.Func.DatosEmp(idemp);

                loadInfo();
            }
            catch (Exception w)
            {
                MessageBox.Show("erro FormasDePago():" + w);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loadColumns();
            isReciboProv(recibo_prov, vendedor);
        }

        private void loadInfo()
        {
            try
            {
                dtBan = SiaWin.Func.SqlDT("select RTRIM(cod_ban) as cod_ban,RTRIM(nom_ban) as nom_ban from comae_ban  order by cod_ban", "comae_ban", idemp);
                CBpagos.ItemsSource = dtBan.DefaultView;
                CBpagos.DisplayMemberPath = "nom_ban";
                CBpagos.SelectedValuePath = "cod_ban";
            }
            catch (Exception w)
            {
                MessageBox.Show("error en loadInfo:" + w);
            }

        }


        public void isReciboProv(string recibo, string vendedor)
        {
            if (!string.IsNullOrEmpty(recibo))
            {
                string query = "select * from cofpagrpv where cod_ven='" + vendedor + "' and rcprov='" + recibo + "' ";
                DataTable dt = SiaWin.Func.SqlDT(query, "conceptos", idemp);
                if (dt.Rows.Count > 0)
                {
                    foreach (System.Data.DataRow item in dt.Rows)
                    {
                        insertGridReciboProvi(item["cod_ban"].ToString(), item["nom_ban"].ToString(), item["cod_cta"].ToString(), Convert.ToInt32(item["pagado"]), item["fec_ven"].ToString(), item["fec_con"].ToString(), item["doc_ref"].ToString(), item["cod_banc"].ToString());
                    }
                    sumaAbonos();
                }
            }
        }

        public void loadColumns()
        {
            try
            {
                dtCue.Columns.Add("cod_ban");
                dtCue.Columns.Add("nom_ban");
                dtCue.Columns.Add("cod_cta");
                dtCue.Columns.Add("valor", typeof(Int32));
                dtCue.Columns.Add("fec_venc");
                dtCue.Columns.Add("fec_con");
                dtCue.Columns.Add("documento");
                dtCue.Columns.Add("cod_banco");
                dataGrid.ItemsSource = dtCue.DefaultView;
            }
            catch (Exception w)
            {
                MessageBox.Show("error en loadColums:" + w);
            }

        }
        private void dataGrid_CurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs e)
        {
            try
            {
                GridColumn colum = ((SfDataGrid)sender).CurrentColumn as GridColumn;
                if (colum.MappingName == "valor")
                {
                    decimal totalabono = 0;

                    decimal.TryParse(dtCue.Compute("Sum(valor)", "").ToString(), out totalabono);
                    System.Data.DataRow dr = dtCue.Rows[dataGrid.SelectedIndex];
                    if (totalabono > totalPagar)
                    {
                        MessageBox.Show("El valor pagado es mayor al saldo...");
                        dr.BeginEdit();
                        dr["valor"] = 0;
                        dr.EndEdit();
                    }
                    dataGrid.UpdateLayout();
                    sumaAbonos();
                    SiaWin.Browse(dtCue);
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error en dataGrid_CurrentCellEndEdit:" + w);
            }
        }
        private void dataGrid_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            try
            {
                if (dataGrid.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)dataGrid.SelectedItems[0];
                    GridColumn colum = ((SfDataGrid)sender).CurrentColumn as GridColumn;

                    if (colum.MappingName == "documento")
                    {
                        if (row["cod_ban"].ToString().Trim() == "45" || row["cod_ban"].ToString().Trim() == "50")
                            e.Handled = false;
                        else
                            e.Handled = true;
                    }

                    if (e.Key == Key.F6)
                    {
                        if (colum.MappingName == "cod_banco")
                        {
                            if (row["cod_ban"].ToString().Trim() == "45" || row["cod_ban"].ToString().Trim() == "50")
                            {
                                int idr = 0; string code = ""; string nombre = "";
                                dynamic xx = SiaWin.WindowBuscar("Cobancos", "banco", "nombre", "banco", "banco", "Bancos", cnEmp, true, "", idEmp: idemp);
                                xx.ShowInTaskbar = false;
                                xx.Owner = Application.Current.MainWindow;
                                xx.Height = 400;
                                xx.Width = 500;
                                xx.ShowDialog();
                                idr = xx.IdRowReturn;
                                code = xx.Codigo;
                                nombre = xx.Nombre;
                                xx = null;
                                if (!string.IsNullOrEmpty(code))
                                {
                                    System.Data.DataRow dr = dtCue.Rows[dataGrid.SelectedIndex];
                                    dr.BeginEdit();
                                    dr["cod_banco"] = code;
                                    dr.EndEdit();
                                }
                            }
                        }
                    }

                    if (e.Key == Key.F8)
                    {
                        GridColumn Colum = ((SfDataGrid)sender).CurrentColumn as GridColumn;
                        if (Colum.MappingName == "valor")
                        {
                            decimal totalabono = 0;
                            decimal.TryParse(dtCue.Compute("Sum(valor)", "").ToString(), out totalabono);
                            System.Data.DataRow dr = dtCue.Rows[dataGrid.SelectedIndex];
                            dr.BeginEdit();
                            dr["valor"] = (totalPagar - totalabono);
                            dr.EndEdit();
                            e.Handled = true;
                        }
                        dataGrid.UpdateLayout();
                        sumaAbonos();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("error en dataGrid_PreviewKeyDown_1" + ex.Message.ToString());
            }
        }
        private void sumaAbonos()
        {
            try
            {
                decimal totalabono = 0;
                decimal.TryParse(dtCue.Compute("Sum(valor)", "").ToString(), out totalabono);
                TxtTotalPagado.Text = totalabono.ToString("C2");
                TxtTotalRecaudo.Text = Convert.ToDecimal(totalPagar - totalabono).ToString("C2"); ;
            }
            catch (Exception w)
            {
                MessageBox.Show("error en sumaAbonos():" + w);
            }


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (validarCampos() == false)
                {
                    MessageBox.Show("LLenen los campos del banco y el numero");
                    return;
                }


                var valor = TxtTotalRecaudo.Text;
                decimal value = decimal.Parse(valor, NumberStyles.Currency);

                if (value == 0)
                {
                    decimal abono = 0;
                    decimal.TryParse(dtCue.Compute("Sum(valor)", "").ToString(), out abono);
                    if (abono <= 0 || abono != totalPagar)
                    {
                        MessageBox.Show("Digita Valor a pagar o valor a abono es diferente al valor a pagar");
                        dataGrid.SelectedIndex = 0;
                        dataGrid.Focus();
                        return;
                    }
                    SiaWin.ValReturn = dtCue;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Tiene un saldo por pagar de:" + TxtTotalRecaudo.Text);
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error en Button_Click:" + w);
            }
        }

        bool validarCampos()
        {
            bool bandera = true;
            foreach (System.Data.DataRow item in dtCue.Rows)
            {
                if (item["cod_ban"].ToString().Trim() == "45" || item["cod_ban"].ToString().Trim() == "50")
                    if (item["documento"].ToString() == "" || string.IsNullOrEmpty(item["documento"].ToString()) || item["cod_banco"].ToString() == "" || string.IsNullOrEmpty(item["cod_banco"].ToString()))
                        bandera = false;
            }
            return bandera;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SiaWin.ValReturn = null;
            this.Close();
        }

        private void dataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //this.dataGrid.MoveCurrentCell(new RowColumnIndex(1, 1), true);
            }
            catch (Exception w)
            {
                MessageBox.Show("error en dataGrid_Loaded:" + w);
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.F5)
                {
                    if (e.Key == System.Windows.Input.Key.F5)
                    {
                        BtnGrabar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        return;
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error Window_PreviewKeyDown:" + w);
            }

        }

        private void Btnadd_Click(object sender, RoutedEventArgs e)
        {
            if (CBpagos.SelectedIndex >= 0)
            {
                System.Data.DataRow selectedDataRow = ((DataRowView)CBpagos.SelectedItem).Row;
                string name = selectedDataRow["nom_ban"].ToString();
                string codigo = selectedDataRow["cod_ban"].ToString();
                insertGrid(codigo, name);
            }
            else
            {
                MessageBox.Show("Selecione Banco");
            }
        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGrid.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)dataGrid.SelectedItems[0];
                    row.Delete();
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("errro en la eliminacio:" + w);
            }

        }

        void insertGridReciboProvi(string cod_ban, string nom_ban, string cod_cta, int valor, string fec_venc, string fec_con, string documento, string cod_banco)
        {
            dtCue.Rows.Add(cod_ban, nom_ban, cod_cta, valor, fec_venc, fec_con, documento, cod_banco);
        }

        void insertGrid(string cod_ban, string nom_ban)
        {
            DataTable dt = SiaWin.Func.SqlDT("select * from comae_ban where cod_ban='" + cod_ban.Trim() + "'", "conceptos", idemp);
            dtCue.Rows.Add(cod_ban, nom_ban, dt.Rows[0]["cod_cta"].ToString().Trim(), 0, DateTime.Now.ToString(), DateTime.Now.ToString(), "", "");
        }


    }
}
