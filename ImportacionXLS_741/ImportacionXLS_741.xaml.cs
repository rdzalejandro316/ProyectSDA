using Microsoft.Win32;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
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
namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9640,"ImportacionXLS_741");
    //Sia.TabU(9640);
    public partial class ImportacionXLS_741 : UserControl
    {
        string ruta;
        bool ez;
        public System.Data.DataTable tablaXLS = new System.Data.DataTable();

        public System.Data.DataSet ds_errores = new System.Data.DataSet();
        public System.Data.DataSet ds_docs = new System.Data.DataSet();

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        dynamic tabitem;
        public long credito = 0, debito = 0;
        public int val1 = 0, val2 = 0;

        public ImportacionXLS_741(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            tabitem = tabitem1;
        }
        public void LoadConfig()
        {
            try
            {
                DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }
        public DataTable reemplazar(DataTable grande, DataTable pequeña)
        {
            double debito = 0, credito = 0, zero = 0;
            string cta = "";
            #region Validacion de datos a pasar a datatable pequeño
            foreach (DataRow item in grande.Rows)
            {
                double deb1 = 0, deb2 = 6, deb3 = 0, deb4 = 0, deb5 = 0, deb6 = 0;
                double cre1 = 0, cre2 = 0, cre3 = 0, cre4 = 0, cre5 = 0, cre6 = 0, cre7 = 0, cre8 = 0, cre9 = 0, cre10 = 0, cre11 = 0;
                if (item["DEB_1"] == DBNull.Value || double.TryParse(item["DEB_1"].ToString(), out deb1) == false)
                {
                    item["DEB_1"] = 0;
                }
                if (item["DEB_2"] == DBNull.Value || double.TryParse(item["DEB_2"].ToString(), out deb1) == false)
                {
                    item["DEB_2"] = 0;
                }
                if (item["DEB_3"] == DBNull.Value || double.TryParse(item["DEB_3"].ToString(), out deb1) == false)
                {
                    item["DEB_3"] = 0;
                }
                if (item["DEB_4"] == DBNull.Value || double.TryParse(item["DEB_4"].ToString(), out deb1) == false)
                {
                    item["DEB_4"] = 0;
                }
                if (item["DEB_5"] == DBNull.Value || double.TryParse(item["DEB_5"].ToString(), out deb1) == false)
                {
                    item["DEB_5"] = 0;
                }
                if (item["DEB_6"] == DBNull.Value || double.TryParse(item["DEB_6"].ToString(), out deb1) == false)
                {
                    item["DEB_6"] = 0;
                }
                if (item["CRE_1"] == DBNull.Value || double.TryParse(item["CRE_1"].ToString(), out deb1) == false)
                {
                    item["CRE_1"] = 0;
                }
                if (item["CRE_2"] == DBNull.Value || double.TryParse(item["CRE_2"].ToString(), out deb1) == false)
                {
                    item["CRE_2"] = 0;
                }
                if (item["CRE_3"] == DBNull.Value || double.TryParse(item["CRE_3"].ToString(), out deb1) == false)
                {
                    item["CRE_3"] = 0;
                }
                if (item["CRE_4"] == DBNull.Value || double.TryParse(item["CRE_4"].ToString(), out deb1) == false)
                {
                    item["CRE_4"] = 0;
                }
                if (item["CRE_5"] == DBNull.Value || double.TryParse(item["CRE_5"].ToString(), out deb1) == false)
                {
                    item["CRE_5"] = 0;
                }
                if (item["CRE_6"] == DBNull.Value || double.TryParse(item["CRE_6"].ToString(), out deb1) == false)
                {
                    item["CRE_6"] = 0;
                }
                if (item["CRE_7"] == DBNull.Value || double.TryParse(item["CRE_7"].ToString(), out deb1) == false)
                {
                    item["CRE_7"] = 0;
                }
                if (item["CRE_8"] == DBNull.Value || double.TryParse(item["CRE_8"].ToString(), out deb1) == false)
                {
                    item["CRE_8"] = 0;
                }
                if (item["CRE_9"] == DBNull.Value || double.TryParse(item["CRE_9"].ToString(), out deb1) == false)
                {
                    item["CRE_9"] = 0;
                }
                if (item["CRE_10"] == DBNull.Value || double.TryParse(item["CRE_10"].ToString(), out deb1) == false)
                {
                    item["CRE_10"] = 0;
                }
                if (item["CRE_11"] == DBNull.Value || double.TryParse(item["CRE_11"].ToString(), out deb1) == false)
                {
                    item["CRE_11"] = 0;
                }
                if (Convert.ToDouble(item["DEB_1"]) > 0)
                {
                    debito = Convert.ToDouble(item["DEB_1"]);
                    cta = item["CTADEB_1"].ToString();
                }
                if (Convert.ToDouble(item["DEB_2"]) > 0)
                {
                    debito = Convert.ToDouble(item["DEB_2"]);
                    cta = item["CTADEB_2"].ToString();
                }
                if (Convert.ToDouble(item["DEB_3"]) > 0)
                {
                    debito = Convert.ToDouble(item["DEB_3"]);
                    cta = item["CTADEB_3"].ToString();
                }
                if (Convert.ToDouble(item["DEB_4"]) > 0)
                {
                    debito = Convert.ToDouble(item["DEB_4"]);
                    cta = item["CTADEB_4"].ToString();
                }
                if (Convert.ToDouble(item["DEB_5"]) > 0)
                {
                    debito = Convert.ToDouble(item["DEB_5"]);
                    cta = item["CTADEB_5"].ToString();
                }
                if (Convert.ToDouble(item["DEB_6"]) > 0)
                {
                    debito = Convert.ToDouble(item["DEB_6"]);
                    cta = item["CTADEB_6"].ToString();
                }
                if (Convert.ToDouble(item["CRE_1"]) > 0)
                {
                    credito = Convert.ToDouble(item["CRE_1"]);
                    cta = item["CTACRE_1"].ToString();
                }
                if (Convert.ToDouble(item["CRE_2"]) > 0)
                {
                    credito = Convert.ToDouble(item["CRE_2"]);
                    cta = item["CTACRE_2"].ToString();
                }
                if (Convert.ToDouble(item["CRE_3"]) > 0)
                {
                    credito = Convert.ToDouble(item["CRE_3"]);
                    cta = item["CTACRE_3"].ToString();
                }
                if (Convert.ToDouble(item["CRE_4"]) > 0)
                {
                    credito = Convert.ToDouble(item["CRE_4"]);
                    cta = item["CTACRE_4"].ToString();
                }
                if (Convert.ToDouble(item["CRE_5"]) > 0)
                {
                    credito = Convert.ToDouble(item["CRE_5"]);
                    cta = item["CTACRE_5"].ToString();
                }
                if (Convert.ToDouble(item["CRE_6"]) > 0)
                {
                    credito = Convert.ToDouble(item["CRE_6"]);
                    cta = item["CTACRE_6"].ToString();
                }
                if (Convert.ToDouble(item["CRE_7"]) > 0)
                {
                    credito = Convert.ToDouble(item["CRE_7"]);
                    cta = item["CTACRE_7"].ToString();
                }
                if (Convert.ToDouble(item["CRE_8"]) > 0)
                {
                    credito = Convert.ToDouble(item["CRE_8"]);
                    cta = item["CTACRE_8"].ToString();
                }
                if (Convert.ToDouble(item["CRE_9"]) > 0)
                {
                    credito = Convert.ToDouble(item["CRE_9"]);
                    cta = item["CTACRE_9"].ToString();
                }
                if (Convert.ToDouble(item["CRE_10"]) > 0)
                {
                    credito = Convert.ToDouble(item["CRE_10"]);
                    cta = item["CTACRE_10"].ToString();
                }
                if (Convert.ToDouble(item["CRE_11"]) > 0)
                {
                    credito = Convert.ToDouble(item["CRE_11"]);
                    cta = item["CTACRE_11"].ToString();
                }
                MessageBox.Show("Credito:" + credito + "\nDebito:" + debito);
                string fec_trn = item["DIA"].ToString() + "/" + item["PER"].ToString() + "/" + item["ANO"].ToString();

                pequeña.Rows.Add(item["cod_trn"].ToString(), item["num_trn"].ToString(), fec_trn, item["factura"].ToString(), ""/*fec_ven*/, ""/*cod_ven*/, cta,
                                    ""/*cod_ciu*/, ""/*cod_suc*/, item["cod_cco"].ToString(), item["cod_ter"].ToString(), item["des_mov"].ToString(), ""/*num_chq*/, item["factura"].ToString(),
                                    ""/*doc_cruc*/, item["bas_mov"].ToString(), debito, credito, ""/*cod_banc*/, ""/*DOC_REF*/, item["ORD_PAG"].ToString(),
                                    item["NOM_TER"].ToString(), ""/*fec_venc*/, ""/*reg*/, item["fec_susc"].ToString());
            }
            //string nombre = "";
            //foreach (DataColumn item in pequeña.Columns)
            //{
            //    //MessageBox.Show(item.ColumnName);
            //    //nombre =nombre+ "|" + item.ColumnName;
            //}
            return pequeña;
            #endregion
        }
        public void Validaciones(DataTable dt)
        {
            try
            {
                ds_docs.Tables.Clear();
                ds_errores.Tables.Clear();//Limpieza de la tabla ERRORES

                //SiaWin.Browse(dt);

                DataView dv = dt.DefaultView;
                dv.Sort = "COD_TRN,NUM_TRN asc";
                DataTable sortedDT = dv.ToTable();
                DataTable dd = new DataTable();
                DataTable dd1 = new DataTable();
                DataTable dc = new DataTable();
                //SiaWin.Browse(sortedDT);
                string val_ant_doc_trn = "", val_ant_cod_trn = "";
                #region Estructuras

                dd.Columns.Add("COD_TRN");
                dd.Columns.Add("NUM_TRN");
                dd.Columns.Add("FEC_TRN");
                dd.Columns.Add("COD_CTA");
                dd.Columns.Add("COD_TER");
                dd.Columns.Add("DES_MOV");
                dd.Columns.Add("DOC_MOV");
                dd.Columns.Add("BAS_MOV");
                dd.Columns.Add("DEB_MOV", typeof(double));
                dd.Columns.Add("CRE_MOV", typeof(double));
                dd.Columns.Add("DOC_CRUC");
                dd.Columns.Add("ORD_PAG");
                dd.Columns.Add("COD_BANC");
                dd.Columns.Add("FEC_VENC");
                dd.Columns.Add("REG");
                dd.Columns.Add("NUM_CHQ");
                dd.Columns.Add("FACTURA");
                dd.Columns.Add("FEC_VEN");
                dd.Columns.Add("COD_VEN");
                dd.Columns.Add("COD_CIU");
                dd.Columns.Add("COD_SUC");
                dd.Columns.Add("COD_CCO");
                dd.Columns.Add("DOC_REF");
                dd.Columns.Add("FEC_SUSC");

                dc.Columns.Add("cod_trn");
                dc.Columns.Add("num_trn");
                dc.Columns.Add("fec_trn");
                dc.Columns.Add("factura");
                dc.Columns.Add("fec_ven");
                dc.Columns.Add("cod_ven");
                dc.Columns.Add("cod_cta");
                dc.Columns.Add("cod_ciu");
                dc.Columns.Add("cod_suc");
                dc.Columns.Add("cod_cco");
                dc.Columns.Add("cod_ter");
                dc.Columns.Add("des_mov");
                dc.Columns.Add("num_chq");
                dc.Columns.Add("doc_mov");
                dc.Columns.Add("doc_cruc");
                dc.Columns.Add("bas_mov");
                dc.Columns.Add("DEB_MOV", typeof(double));
                dc.Columns.Add("CRE_MOV", typeof(double));
                dc.Columns.Add("cod_banc");
                dc.Columns.Add("DOC_REF");
                dc.Columns.Add("ORD_PAG");
                dc.Columns.Add("NOM_TER");
                dc.Columns.Add("fec_venc");
                dc.Columns.Add("reg");
                dc.Columns.Add("fec_susc");


                dd1.Columns.Add("TRN");
                dd1.Columns.Add("DOCUM");
                dd1.Columns.Add("ANO");
                dd1.Columns.Add("PER");
                dd1.Columns.Add("DIA");
                dd1.Columns.Add("ORDEN");
                dd1.Columns.Add("COD_TER");
                dd1.Columns.Add("NOM_TER");
                dd1.Columns.Add("CONTRATO");
                dd1.Columns.Add("descri");
                dd1.Columns.Add("CCO");
                dd1.Columns.Add("DEB_1", typeof(double));
                dd1.Columns.Add("DEB_2", typeof(double));
                dd1.Columns.Add("DEB_3", typeof(double));
                dd1.Columns.Add("DEB_4", typeof(double));
                dd1.Columns.Add("DEB_5", typeof(double));
                dd1.Columns.Add("DEB_6", typeof(double));
                dd1.Columns.Add("BASE");
                dd1.Columns.Add("BASE1");
                dd1.Columns.Add("CRE_1", typeof(double));
                dd1.Columns.Add("BASERIVA");
                dd1.Columns.Add("CRE_2", typeof(double));
                dd1.Columns.Add("BASERICA");
                dd1.Columns.Add("CRE_3", typeof(double));
                dd1.Columns.Add("CRE_4", typeof(double));
                dd1.Columns.Add("CRE_5", typeof(double));
                dd1.Columns.Add("CRE_6", typeof(double));
                dd1.Columns.Add("CRE_7", typeof(double));
                dd1.Columns.Add("CRE_8", typeof(double));
                dd1.Columns.Add("CRE_9", typeof(double));
                dd1.Columns.Add("CRE_10", typeof(double));
                dd1.Columns.Add("CRE_11", typeof(double));
                dd1.Columns.Add("CTADEB_1");
                dd1.Columns.Add("CTADEB_2 ");
                dd1.Columns.Add("CTADEB_3");
                dd1.Columns.Add("CTADEB_4");
                dd1.Columns.Add("CTADEB_5");
                dd1.Columns.Add("CTADEB_6");
                dd1.Columns.Add("CTACRE_1");
                dd1.Columns.Add("CTACRE_2");
                dd1.Columns.Add("CTACRE_3");
                dd1.Columns.Add("CTACRE_4");
                dd1.Columns.Add("CTACRE_5");
                dd1.Columns.Add("CTACRE_6");
                dd1.Columns.Add("CTACRE_7");
                dd1.Columns.Add("CTACRE_8");
                dd1.Columns.Add("CTACRE_9");
                dd1.Columns.Add("CTACRE_10");
                dd1.Columns.Add("CTACRE_11");
                dd1.Columns.Add("fec_susc");
                dd1 = dv.ToTable();
                #endregion
                int a = 1;
                int b = 0;
                sortedDT = reemplazar(dd1, dc);
                Datos.ItemsSource = null;
                Datos.Items.Clear();
                Datos.ItemsSource = sortedDT.DefaultView;
                string nombre = "";
                //foreach (DataColumn item in sortedDT.Columns)
                //{
                //    //MessageBox.Show(item.ColumnName);
                //    nombre = nombre + "|" + item.ColumnName + "\n";
                //}
                MessageBox.Show(nombre);
                foreach (DataRow dr in sortedDT.Rows)
                {
                    b++;
                    if (string.IsNullOrEmpty(val_ant_doc_trn) && string.IsNullOrEmpty(val_ant_cod_trn))
                    {
                        val_ant_cod_trn = dr["COD_TRN"].ToString();
                        val_ant_doc_trn = dr["NUM_TRN"].ToString();
                    }
                    if (val_ant_cod_trn == dr["COD_TRN"].ToString() && val_ant_doc_trn == dr["NUM_TRN"].ToString())
                    {

                        double deb = dr["DEB_MOV"] == DBNull.Value ? 0 : Convert.ToDouble(dr["DEB_MOV"]);
                        double cre = dr["CRE_MOV"] == DBNull.Value ? 0 : Convert.ToDouble(dr["CRE_MOV"]);


                        dd.Rows.Add(dr["COD_TRN"].ToString(),
                            dr["NUM_TRN"].ToString(),
                            dr["FEC_TRN"].ToString(),
                            dr["COD_CTA"].ToString(),
                            dr["COD_TER"].ToString(),
                            dr["DES_MOV"].ToString(),
                            dr["DOC_MOV"].ToString(),
                            dr["BAS_MOV"].ToString(),
                            deb,
                            cre,
                            dr["DOC_CRUC"].ToString(),
                            dr["ORD_PAG"].ToString(),
                            dr["COD_BANC"].ToString(),
                            dr["FEC_VENC"].ToString(),
                            dr["REG"].ToString(),
                            dr["NUM_CHQ"].ToString(),
                            dr["FACTURA"].ToString(),
                            dr["FEC_VEN"].ToString(),
                            dr["COD_VEN"].ToString(),
                            dr["COD_CIU"].ToString(),
                            dr["COD_SUC"].ToString(),
                            dr["COD_CCO"].ToString(),
                            dr["DOC_REF"].ToString(),
                            dr["FEC_SUSC"].ToString());

                        DataRow lastRow = sortedDT.Rows[sortedDT.Rows.Count - 1];
                        if (b == sortedDT.Rows.Count)
                        {
                            a++;
                            DataTable daa = dd.Copy();
                            daa.TableName = a.ToString();
                            ds_docs.Tables.Add(daa);
                            dd.Clear();
                        }
                    }
                    else
                    {
                        a++;
                        DataTable daa = dd.Copy();
                        daa.TableName = a.ToString();
                        ds_docs.Tables.Add(daa);
                        dd.Clear();

                        double deb = dr["DEB_MOV"] == DBNull.Value ? 0 : Convert.ToDouble(dr["DEB_MOV"]);
                        double cre = dr["CRE_MOV"] == DBNull.Value ? 0 : Convert.ToDouble(dr["CRE_MOV"]);
                        //dd.Rows.Add(dr["COD_TRN"].ToString(), dr["NUM_TRN"].ToString(),deb,cre);
                        dd.Rows.Add(dr["COD_TRN"].ToString(), dr["NUM_TRN"].ToString(), dr["FEC_TRN"].ToString(), dr["COD_CTA"].ToString(),
                            dr["COD_TER"].ToString(), dr["DES_MOV"].ToString(), dr["DOC_MOV"].ToString(), dr["BAS_MOV"].ToString(), deb, cre,
                            dr["DOC_CRUC"].ToString(), dr["ORD_PAG"].ToString(), dr["COD_BANC"].ToString(), dr["FEC_VENC"].ToString(), dr["REG"].ToString(), dr["NUM_CHQ"].ToString(),
                            dr["FACTURA"].ToString(), dr["FEC_VEN"].ToString(), dr["COD_VEN"].ToString(), dr["COD_CIU"].ToString(), dr["COD_SUC"].ToString(), dr["COD_CCO"].ToString(),
                            dr["DOC_REF"].ToString(), dr["FEC_SUSC"].ToString());
                    }

                    val_ant_cod_trn = dr["COD_TRN"].ToString();
                    val_ant_doc_trn = dr["NUM_TRN"].ToString();
                }

                bool validacion = true;
                //int a = 0;
                foreach (DataTable dt_t in ds_docs.Tables)
                {
                    double deb = Convert.ToDouble(dt_t.Compute("Sum(DEB_MOV)", string.Empty));
                    double cre = Convert.ToDouble(dt_t.Compute("Sum(CRE_MOV)", string.Empty));
                    //segunda valicacion encontrar los codigos en la base de datos
                    validacion = valid(dt_t);
                    //segun las validaciones correspondientes se ira a un datase dependiendo si es erronea o no                                                                                                          
                }
                if (validacion == false)
                {
                    MessageBox.Show("Errores en el almacenamiento de los datos");
                    ez = false;
                }
                else
                {
                    ez = true;
                }
                DataGrid dt_grid = new DataGrid();
                DataTable Ferrores = new DataTable();
                Ferrores.Columns.Add("Error");
                dt_grid.CanUserAddRows = false;
                foreach (DataTable de in ds_errores.Tables)
                {
                    foreach (DataRow dr in de.Rows)
                    {
                        Ferrores.Rows.Add(dr[0].ToString());
                    }
                    //SiaWin.Browse(Ferrores);
                }
                dt_grid.ItemsSource = Ferrores.DefaultView;
                gridErrores.Children.Add(dt_grid);
            }
            catch (Exception w)
            {
                MessageBox.Show("Error:\n" + w);
            }
        }
        public bool valid(DataTable dt_temp)
        {
            DataTable dt_maybe = new DataTable();
            dt_maybe.Columns.Add("error", typeof(string));

            bool flag = true;
            bool flag_numeros = true;
            int index = 0;

            double debito = 0;
            double credito = 0;

            string num_trn = dt_temp.Rows[0]["num_trn"].ToString();
            string cod_trn = dt_temp.Rows[0]["cod_trn"].ToString();

            #region validacion


            foreach (DataRow item in dt_temp.Rows)
            {
                if (!string.IsNullOrEmpty(item["cod_ter"].ToString()))//Valida que el tercero exista
                {
                    string select = "select * from  comae_ter where cod_ter='" + item["cod_ter"].ToString().Trim() + "'  ";
                    DataTable dt = SiaWin.Func.SqlDT(select, "table", idemp);
                    if (dt.Rows.Count <= 0)
                    {
                        flag = false;
                        dt_maybe.Rows.Add("No existe tercero " + item["cod_ter"].ToString() + " en la fila:" + index);
                    }
                }
                //else flag = false;

                if (!string.IsNullOrEmpty(item["cod_cta"].ToString()))//Valida que la cuenta exista
                {
                    string select = "select * from  comae_cta where cod_cta='" + item["cod_cta"].ToString() + "'  and tip_cta='A' ";
                    DataTable dt = SiaWin.Func.SqlDT(select, "table", idemp);
                    if (dt.Rows.Count <= 0)
                    {
                        flag = false;
                        dt_maybe.Rows.Add("La cuenta del documento: " + item["cod_cta"].ToString().Trim() + " no se encuentra");
                    }
                }

                if (!string.IsNullOrEmpty(item["cod_trn"].ToString()))//Valida que la cuenta exista
                {
                    string select = "select * from  comae_trn where cod_trn='" + item["cod_trn"].ToString() + "'  ";
                    DataTable dt = SiaWin.Func.SqlDT(select, "table", idemp);
                    if (dt.Rows.Count <= 0)
                    {
                        flag = false;
                        dt_maybe.Rows.Add("La transaccion: " + item["cod_trn"].ToString().Trim() + " no se encuentra");
                    }
                }


                if (!string.IsNullOrEmpty(item["des_mov"].ToString()))
                {
                    string des_mov = item["des_mov"].ToString();
                    if (des_mov.Length > 300)
                    {
                        flag = false;
                        dt_maybe.Rows.Add("la descripcion no puede ser mayor a 300: " + item["cod_trn"].ToString().Trim() + "");
                    }
                }

                double d; double c; DateTime f;

                if (!string.IsNullOrEmpty(item["deb_mov"].ToString()))
                {
                    if (double.TryParse(item["deb_mov"].ToString(), out d) == false)
                    {
                        flag_numeros = false;
                        flag = false;
                        dt_maybe.Rows.Add("la columna debito tiene que ser numerica " + item["deb_mov"].ToString().Trim() + "");
                    }
                    else debito += d;
                }

                if (!string.IsNullOrEmpty(item["cre_mov"].ToString()))
                {
                    if (double.TryParse(item["cre_mov"].ToString(), out c) == false)
                    {
                        flag_numeros = false;
                        flag = false;
                        dt_maybe.Rows.Add("la columna debito tiene que ser numerica " + item["cre_mov"].ToString().Trim() + "");
                    }
                    else credito += c;
                }

                if (!string.IsNullOrEmpty(item["fec_trn"].ToString()))
                {
                    if (DateTime.TryParse(item["fec_trn"].ToString(), out f) == false)
                    {
                        flag = false;
                        dt_maybe.Rows.Add("la columna fecha de transaccion debe de contener una fecha" + item["fec_trn"].ToString().Trim() + "");
                    }
                }
                index++;
            }

            #endregion

            if (flag_numeros == true)
            {
                if ((debito - credito) != 0)//valida documento si esta descuadrado
                {
                    dt_maybe.Rows.Add("El documento se encuentra descuadrado:" + num_trn);
                    flag = false;
                }
            }



            if (!string.IsNullOrEmpty(num_trn))//Valida que la TRN no exista
            {

                string select = "select * from cocab_doc where num_trn='" + num_trn + "' and cod_trn='" + cod_trn + "';";
                DataTable dt = SiaWin.Func.SqlDT(select, "table", idemp);
                if (dt.Rows.Count > 0)
                {
                    flag = false;
                    dt_maybe.Rows.Add("El documento: " + num_trn + " con la transaccion " + cod_trn + " ya existe");
                }
            }


            DataTable daa = dt_maybe.Copy();
            daa.TableName = dt_temp.TableName;
            ds_errores.Tables.Add(daa);
            ez = flag;
            return flag;
        }
        private void SubirDatos()
        {
            insertTodos(ds_docs);
        }
        public void insertTodos(DataSet ds_exist)
        {
            try
            {
                if (ez == true)
                {
                    try
                    {
                        string sql_cab = "";
                        string sql_cue = "";

                        foreach (DataTable dt_cue in ds_exist.Tables)
                        {

                            string cod_trn_cab = dt_cue.Rows[0]["cod_trn"].ToString();
                            string num_trn_cab = dt_cue.Rows[0]["num_trn"].ToString();

                            sql_cab += @"INSERT INTO cocab_doc (cod_trn,fec_trn,num_trn,detalle) values ('" + cod_trn_cab + "',getdate(),'" + num_trn_cab + "','IMPORTACION EXCEL PROCESOS 740');DECLARE @NewID INT;SELECT @NewID = SCOPE_IDENTITY();";

                            foreach (DataRow data in dt_cue.Rows)
                            {
                                double dev_con = Convert.ToDouble(data["DEB_MOV"]);
                                double cre_con = Convert.ToDouble(data["CRE_MOV"]);
                                string cod_trn = data["cod_trn"].ToString();
                                string num_trn = data["num_trn"].ToString();
                                string cod_cta = data["cod_cta"].ToString();
                                string cod_cco = data["cod_cco"].ToString();
                                string cod_ter = data["cod_ter"].ToString();
                                string des_mov = data["des_mov"].ToString();
                                string doc_cruc = data["doc_cruc"].ToString();
                                string deb_mov = data["deb_mov"].ToString();
                                string cre_mov = data["cre_mov"].ToString();
                                string ord_pag = data["ord_pag"].ToString();
                                sql_cue += @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,doc_cruc,deb_mov,cre_mov,ord_pag) values (@NewID,'" + data["cod_trn"] + "','" + data["num_trn"] + "','" + data["cod_cta"].ToString() + "','" + data["cod_cco"].ToString() + "','" + data["cod_ter"].ToString() + "','" + data["DES_MOV"].ToString() + "','" + data["DOC_CRUC"].ToString() + "'," + dev_con.ToString("F", CultureInfo.InvariantCulture) + "," + cre_con.ToString("F", CultureInfo.InvariantCulture) + ",'" + data["ord_pag"].ToString() + "');";
                            }
                            if (SiaWin.Func.SqlCRUD(sql_cab + sql_cue, idemp) == true)
                            {
                                sql_cab = ""; sql_cue = "";
                                MessageBox.Show("Documentos ingresados de manera correcta");
                            }
                        }
                    }

                    catch (SqlException ex)
                    {
                        MessageBox.Show("error:::" + ex);
                    }

                    catch (Exception ex)
                    {
                        MessageBox.Show("err11111or:::" + ex);
                    }
                }
                else
                {
                    MessageBox.Show("Existen errores en el documento a subir.\nPara continuar realice la correccion de los mismos.");
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("ERROR INSER:" + w);
            }

        }
        public static System.Data.DataTable ConvertExcelToDataTable(string FileName)
        {
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Excel2013;
                IWorkbook workbook = application.Workbooks.Open(FileName);
                IWorksheet worksheet = workbook.Worksheets[0];

                //Read data from the worksheet and Export to the DataTable
                System.Data.DataTable customersTable = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ColumnNames);

                //Binding exported DataTable to data grid, likewise it can binded to any 
                //user interface control which supports binding
                return customersTable;
            }
        }
        public DataTable Limpiar(DataTable dt)
        {
            DataTable dt1 = dt.Clone(); //copy the structure 
            for (int i = 0; i <= dt.Rows.Count - 1; i++) //iterate through the rows of the source
            {
                DataRow currentRow = dt.Rows[i];  //copy the current row 
                foreach (var colValue in currentRow.ItemArray)//move along the columns 
                {
                    if (!string.IsNullOrEmpty(colValue.ToString())) // if there is a value in a column, copy the row and finish
                    {
                        dt1.ImportRow(currentRow);
                        break; //break and get a new row                        
                    }
                }
            }
            return dt1;
        }
        //public long SumarCol(DataTable d1, string col_name)
        //{
        //    long total = 0;
        //    string cadena = "Sum(" + col_name + ")";
        //    object resul;
        //    resul = d1.Compute(cadena, string.Empty);
        //    total = Convert.ToInt64(resul);
        //    return total;
        //}
        public string guardar()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".xlsx";
            saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
            saveFileDialog.Title = "Guardar Plantilla como...";
            saveFileDialog.ShowDialog();
            return saveFileDialog.FileName;
        }
        public string Buscar()
        {
            OpenFileDialog openfile = new OpenFileDialog();
            openfile.DefaultExt = ".xlsx";
            openfile.CheckFileExists = true;
            openfile.Filter = "Archivos XLSX (.xlsx)|*.xlsx|Archivos XLS (.xls)|*.xls";
            openfile.FilterIndex = 1;
            var browsefile = openfile.ShowDialog();
            return openfile.FileName;
        }

        public void Cargar(string ruta)
        {
            try
            {
                tablaXLS.Clear();
                DataTable tablaXLS_temp = ConvertExcelToDataTable(ruta);
                tablaXLS_temp = NameChange(tablaXLS_temp);
                tablaXLS = Limpiar(tablaXLS_temp);
                Validaciones(tablaXLS);
                tablaXLS = Limpiar(tablaXLS);

                //tablaXLS = ConverInt(tablaXLS);
                //debito = SumarCol(tablaXLS, "DEB_MOV");
                //credito = SumarCol(tablaXLS, "CRE_MOV");
                SiaWin.Browse(tablaXLS);
                //Datos.ItemsSource = null;
                //Datos.Items.Clear();
                //Datos.ItemsSource = tablaXLS.DefaultView;
                credi.Text = credito.ToString();
                debi.Text = debito.ToString();
                dife.Text = Convert.ToString(debito - credito);
            }
            catch (Exception m)
            {
                //MessageBox.Show(m.Message);
                MessageBox.Show("Error\n" + m);
            }
        }

        private DataTable NameChange(DataTable temp)
        {
            temp.Columns[0].ColumnName = "COD_TRN";
            temp.Columns[1].ColumnName = "NUM_TRN";
            temp.Columns[2].ColumnName = "ANO";
            temp.Columns[3].ColumnName = "PER";
            temp.Columns[4].ColumnName = "DIA";
            temp.Columns[5].ColumnName = "ORD_PAG";
            temp.Columns[6].ColumnName = "COD_TER";
            temp.Columns[7].ColumnName = "NOM_TER";
            temp.Columns[8].ColumnName = "FACTURA";
            temp.Columns[9].ColumnName = "DES_MOV";
            temp.Columns[10].ColumnName = "COD_CCO";
            temp.Columns[11].ColumnName = "DEB_1";
            temp.Columns[12].ColumnName = "DEB_2";
            temp.Columns[13].ColumnName = "DEB_3";
            temp.Columns[14].ColumnName = "DEB_4";
            temp.Columns[15].ColumnName = "DEB_5";
            temp.Columns[16].ColumnName = "DEB_6";
            temp.Columns[17].ColumnName = "BAS_MOV";
            temp.Columns[18].ColumnName = "BASE1";
            temp.Columns[19].ColumnName = "CRE_1";
            temp.Columns[20].ColumnName = "BASERIVA";
            temp.Columns[21].ColumnName = "CRE_2";
            temp.Columns[22].ColumnName = "BASERICA";
            temp.Columns[23].ColumnName = "CRE_3";
            temp.Columns[24].ColumnName = "CRE_4";
            temp.Columns[25].ColumnName = "CRE_5";
            temp.Columns[26].ColumnName = "CRE_6";
            temp.Columns[27].ColumnName = "CRE_7";
            temp.Columns[28].ColumnName = "CRE_8";
            temp.Columns[29].ColumnName = "CRE_9";
            temp.Columns[30].ColumnName = "CRE_10";
            temp.Columns[31].ColumnName = "CRE_11";
            temp.Columns[32].ColumnName = "CTADEB_1";
            temp.Columns[33].ColumnName = "CTADEB_2";
            temp.Columns[34].ColumnName = "CTADEB_3";
            temp.Columns[35].ColumnName = "CTADEB_4";
            temp.Columns[36].ColumnName = "CTADEB_5";
            temp.Columns[37].ColumnName = "CTADEB_6";
            temp.Columns[38].ColumnName = "CTACRE_1";
            temp.Columns[39].ColumnName = "CTACRE_2";
            temp.Columns[40].ColumnName = "CTACRE_3";
            temp.Columns[41].ColumnName = "CTACRE_4";
            temp.Columns[42].ColumnName = "CTACRE_5";
            temp.Columns[43].ColumnName = "CTACRE_6";
            temp.Columns[44].ColumnName = "CTACRE_7";
            temp.Columns[45].ColumnName = "CTACRE_8";
            temp.Columns[46].ColumnName = "CTACRE_9";
            temp.Columns[47].ColumnName = "CTACRE_10";
            temp.Columns[48].ColumnName = "CTACRE_11";
            temp.Columns[49].ColumnName = "fec_susc";

            return temp;
        }

        private void Button_Cargar(object sender, RoutedEventArgs e)
        {
            string url = Buscar();
            Cargar(url);
        }

        private void Button_Impo(object sender, RoutedEventArgs e)
        {
            SubirDatos();
        }

        private void Datos_CleanUpVirtualizedItem(object sender, CleanUpVirtualizedItemEventArgs e)
        {

        }

        private void Button_Salir(object sender, RoutedEventArgs e)
        {
            //this.Close();
        }

        private void Button_Crear(object sender, RoutedEventArgs e)
        {

            ruta = guardar();
            //Create an instance of ExcelEngine
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;

                application.DefaultVersion = ExcelVersion.Excel2010;

                //Create a workbook
                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet worksheet = workbook.Worksheets[0];

                //Disable gridlines in the worksheet
                worksheet.IsGridLinesVisible = true;

                //Enter values to the cells from A3 to A5
                worksheet.Range["A1"].Text = "COD_TRN";
                worksheet.Range["B1"].Text = "NUM_TRN";
                worksheet.Range["C1"].Text = "FEC_TRN";
                worksheet.Range["D1"].Text = "COD_CTA";
                worksheet.Range["E1"].Text = "COD_TER";
                worksheet.Range["F1"].Text = "DES_MOV";
                worksheet.Range["G1"].Text = "DOC_MOV";
                worksheet.Range["H1"].Text = "BAS_MOV";
                worksheet.Range["I1"].Text = "DEB_MOV";
                worksheet.Range["J1"].Text = "CRE_MOV";
                worksheet.Range["K1"].Text = "DOC_CRUC";
                worksheet.Range["L1"].Text = "ORD_PAG";
                worksheet.Range["M1"].Text = "COD_BANC";
                worksheet.Range["N1"].Text = "FEC_VENC";
                worksheet.Range["O1"].Text = "REG";
                worksheet.Range["P1"].Text = "NUM_CHQ";
                worksheet.Range["Q1"].Text = "FACTURA";
                worksheet.Range["R1"].Text = "FEC_VEN";
                worksheet.Range["S1"].Text = "COD_VEN";
                worksheet.Range["T1"].Text = "COD_CIU";
                worksheet.Range["U1"].Text = "COD_SUC";
                worksheet.Range["V1"].Text = "COD_CCO";
                worksheet.Range["W1"].Text = "DOC_REF";
                worksheet.Range["X1"].Text = "FEC_SUSC";


                //cod_trn
                //num_trn
                //fec_trn
                //factura
                //fec_ven
                //cod_ven
                //cod_cta
                //cod_ciu
                //cod_suc
                //cod_cco
                //cod_ter
                //des_mov
                //num_chq
                //doc_mov
                //doc_cruc
                //bas_mov
                //deb_mov
                //cre_mov
                //cod_banc
                //DOC_REF
                //ORD_PAG
                //NOM_TER
                //fec_venc
                //reg
                //fec_susc

                //Make the text bUld
                worksheet.Range["A1:X1"].CellStyle.Font.Bold = true;

                //Save the Excel document
                if (string.IsNullOrEmpty(ruta))
                {
                    MessageBox.Show("Por favor, seleccione una ruta para guardar la plantilla");
                }
                else
                {
                    workbook.SaveAs(ruta);
                }
            }
        }
    }
}
