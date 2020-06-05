using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows;
using TrnFacturaElectronica.wsdl_dfacture;
namespace SiasoftAppExt
{
    /// <summary>
    /// Lógica de interacción para UserControl1.xaml
    /// </summary>
    public partial class TrnFacturaElectronica : Window
    {
        dynamic SiaWin;
        //Sia.PublicarPnt(9388,"TrnFacturaElectronica");
        //public string tokenEmpresa = "e9fbeffc8f1a8ccc6f7ddd980b92e9ce5b8d6135"; // los accesos deben ser solicitados al iniciar el proceso de integración
        //public string tokenAuthorizacion = "3453101e3c2bebcbcdafc755ca5f5b7df1eeedec"; // los accesos deben ser solicitados al iniciar el proceso de integración
        public string tokenEmpresa = string.Empty;
        public string tokenAuthorizacion = string.Empty;
        public string Url = "";
        ServiceClient service;
        FacturaGeneral factura;
        public int idrowcab = 0;
        public string codpvt = string.Empty;
        public String cnEmp = string.Empty;
        DataSet dsImprimir = new DataSet();
        DataSet dsAnulaFactura = new DataSet();
        public string NumDocElect = string.Empty;
        public string Codigo = string.Empty;
        public string Msg = string.Empty;
        public string FechaResp = string.Empty;
        public string Cufe = string.Empty;
        public int _ModuloId = 0;
        public int _EmpresaId = 0;
        public int _AccesoId = 0;


        public TrnFacturaElectronica()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            service = new ServiceClient();

            this.tbxFechaEmision.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        private void buildFactura()
        {
            try
            {
                factura = new FacturaGeneral();
                // tipo de documento 005 factura , 007-008 nota credito
                string _codtrn = (string)dsImprimir.Tables[0].Rows[0]["cod_trn"]; //1=juridica 2=natural
                if (_codtrn == "005") factura.tipoDocumento = "01";
                if (_codtrn != "005") factura.tipoDocumento = "04";
                if (_codtrn != "005")  //es nota credito
                {
                    if (_codtrn == "007") factura.motivoNota = "2"; //anulacion factura electronica
                    if (_codtrn == "008") factura.motivoNota = "1"; // devolucion de item
                    string codtrnanu = dsImprimir.Tables[0].Rows[0]["trn_anu"].ToString().Trim(); // notas credito
                    string numtrnanu = dsImprimir.Tables[0].Rows[0]["num_anu"].ToString().Trim(); // notas credito
                    if (_codtrn == "007" || _codtrn == "008") // trae fecha de emi.factura
                    {
                        /// trae datos del cufe ,fecha de emision factura
                        //PvFacturaElectronicaAnulacion
                        SqlConnection con = new SqlConnection(cnEmp);
                        SqlCommand cmd = new SqlCommand();
                        SqlDataAdapter da = new SqlDataAdapter();

                        cmd = new SqlCommand("PvFacturaElectronicaAnulacion", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@codtrn", codtrnanu);//if you have parameters.
                        cmd.Parameters.AddWithValue("@numtrn", numtrnanu);//if you have parameters.
                        da = new SqlDataAdapter(cmd);
                        dsAnulaFactura.Clear();
                        da.Fill(dsAnulaFactura);
                        factura.uuidDocumentoModificado = dsAnulaFactura.Tables[0].Rows[0]["fa_cufe"].ToString().Trim(); // notas credito
                        //factura.consecutivoDocumentoModificado = dsAnulaFactura.Tables[0].Rows[0]["num_anu"].ToString().Trim(); // notas credito
                        factura.consecutivoDocumentoModificado = "EXFV178";

                        DateTime fechadocAnula = Convert.ToDateTime(dsAnulaFactura.Tables[0].Rows[0]["fec_trn"].ToString().Trim());
                        factura.fechaEmisionDocumentoModificado = fechadocAnula.ToString("yyyy-MM-dd HH:mm:ss");

                    }
                }
                //MessageBox.Show(factura.tipoDocumento);
                /// validar campos del tercero antes de armar ////


                Cliente cliente = new Cliente();
                //            cliente.apellido = "Barrios";    //this.tbxApellido.Text;
                int tipPers = (int)dsImprimir.Tables[0].Rows[0]["tip_pers"]; //1=juridica 2=natural
                if (tipPers == 0) tipPers = 2;
                cliente.tipoPersona = tipPers.ToString();//this.tbxTipoPersona.Text;

                cliente.numeroDocumento = dsImprimir.Tables[0].Rows[0]["cod_cli"].ToString().Trim();
                if (tipPers == 1)
                {
                    cliente.nombreRazonSocial = dsImprimir.Tables[0].Rows[0]["razon_soc"].ToString().Trim();

                }
                else
                {
                    cliente.nombreRazonSocial = dsImprimir.Tables[0].Rows[0]["nom1"].ToString().Trim();
                    cliente.segundoNombre = dsImprimir.Tables[0].Rows[0]["nom2"].ToString().Trim();
                    cliente.apellido = dsImprimir.Tables[0].Rows[0]["apl1"].ToString().Trim() + " " + dsImprimir.Tables[0].Rows[0]["apl2"].ToString().Trim();

                }
                cliente.departamento = dsImprimir.Tables[0].Rows[0]["depa"].ToString().Trim().ToUpper();
                cliente.ciudad = dsImprimir.Tables[0].Rows[0]["ciudad"].ToString().Trim().ToUpper(); ;
                cliente.direccion = dsImprimir.Tables[0].Rows[0]["dir1"].ToString().Trim().ToUpper();
                cliente.email = dsImprimir.Tables[0].Rows[0]["email"].ToString().Trim().ToUpper(); ;
                cliente.notificar = "SI";

                cliente.pais = "CO";
                int tipRegim = Convert.ToInt32(dsImprimir.Tables[0].Rows[0]["tip_prv"].ToString());

                if (tipRegim == 0) tipRegim = 2;
                if (tipRegim == 1) tipRegim = 0;
                cliente.regimen = tipRegim.ToString();//this.tbxTipoPersona.Text;
                                                      //cliente.regimen = "0";//this.tbxRegimen.Text;
                cliente.segundoNombre = "";
                cliente.subDivision = "";
                cliente.telefono = dsImprimir.Tables[0].Rows[0]["tel1"].ToString().Trim().ToUpper();
                cliente.tipoIdentificacion = dsImprimir.Tables[0].Rows[0]["tdoc"].ToString().Trim();

                factura.cliente = cliente;

                if (_codtrn == "005")
                {
                    factura.rangoNumeracion = "EXFV-000001"; // rango desde, debe solicitar este campo 
                    factura.consecutivoDocumento = txtNumFactura.Text.Trim(); // depende del tipo de configuración Manual o automático
                }
                if(_codtrn!="005")

                {
                    //yyyy-MM-dd HH:mm:ss
                    factura.rangoNumeracion = "EXNC-000001"; // rango desde, debe solicitar este campo 
                    //factura.consecutivoDocumento = "2"; // depende del tipo de configuración Manual o automático
                    factura.consecutivoDocumento =  txtNumFactura.Text.Trim(); // depende del tipo de configuración Manual o automático

                }
                int ItemsCue = dsImprimir.Tables[1].Rows.Count;
                factura.detalleDeFactura = new FacturaDetalle[ItemsCue];
                int item = 0;
                // detalle de la factura 
                foreach (DataRow row in dsImprimir.Tables[1].Rows)
                {
                    FacturaDetalle detalle1 = new FacturaDetalle();
                    detalle1.cantidadUnidades =  row["cantidad"].ToString().Trim();
                    detalle1.codigoProducto = row["cod_ref"].ToString().Trim();
                    detalle1.descripcion = row["nom_ref"].ToString().Trim();
                    detalle1.descuento = row["val_des"].ToString().Trim(); ;

                    detalle1.impuestosDetalles = new FacturaImpuestos[1];
                    // impuestos por producto 
                    FacturaImpuestos impuestodetalles1 = new FacturaImpuestos();
                    impuestodetalles1.baseImponibleTOTALImp = Convert.ToDecimal(row["subtotal"]).ToString();
                    impuestodetalles1.codigoTOTALImp = "01";//this.tbxTipoImpuesto.Text;
                    impuestodetalles1.controlInterno = "";
                    impuestodetalles1.porcentajeTOTALImp = Convert.ToDecimal(row["por_iva"]).ToString();
                    impuestodetalles1.valorTOTALImp = Convert.ToDecimal(row["val_iva"]).ToString();
                    //fin impuestoprod 

                    detalle1.impuestosDetalles[0] = impuestodetalles1;
                    detalle1.precioTotal = Convert.ToDecimal(row["tot_tot"]).ToString();
                    detalle1.precioTotalSinImpuestos = (Convert.ToDecimal(row["subtotal"])- Convert.ToDecimal(row["val_des"])).ToString();
                    
                    detalle1.precioVentaUnitario = row["val_uni"].ToString();
                    detalle1.unidadMedida = "UNIDAD";//this.tbxtipoUnidad.Text;
                    // fin detalle
                    factura.detalleDeFactura[item] = detalle1;
                    item++;
                }

                factura.estadoPago = "1"; //pagada totalmente // 2=Pagado parcialmente 3=Sin pagar 
                DateTime fechadoc = Convert.ToDateTime(dsImprimir.Tables[0].Rows[0]["fec_trn"].ToString().Trim());
                factura.fechaEmision = fechadoc.ToString("yyyy-MM-dd HH:mm:ss");
                factura.fechaVencimiento = fechadoc.ToString("yyyy-MM-dd HH:mm:ss");
                factura.icoterms = "";
                factura.importeTotal = dsImprimir.Tables[2].Rows[0]["tot_tot"].ToString();// this.tbxImporteTotal.Text;
                factura.impuestosGenerales = new FacturaImpuestos[1];
                // factura impuestos generales 
                FacturaImpuestos impuestosg1 = new FacturaImpuestos();
                impuestosg1.baseImponibleTOTALImp = dsImprimir.Tables[2].Rows[0]["subtotal"].ToString();// this.tbxImporteTotal.Text;;
                impuestosg1.codigoTOTALImp = "01";
                impuestosg1.porcentajeTOTALImp = "19.00";
                impuestosg1.valorTOTALImp = dsImprimir.Tables[2].Rows[0]["val_iva"].ToString();// this.tbxImporteTotal.Text;
                FacturaImpuestos impuestosg2 = new FacturaImpuestos();
                impuestosg2.baseImponibleTOTALImp = "840";
                impuestosg2.codigoTOTALImp = "06";
                impuestosg2.porcentajeTOTALImp = "2.50";
                impuestosg2.valorTOTALImp = "21";
                factura.impuestosGenerales[0] = impuestosg1;
                //factura.impuestosGenerales[1] = impuestosg2;
                factura.informacionAdicional = "";
                factura.medioPago = "10";//this.tbxTipoPago.Text;
                factura.moneda = "COP";//this.tbxMoneda.Text;
                factura.propina = "0.00";

                factura.totalDescuentos = dsImprimir.Tables[2].Rows[0]["val_des"].ToString();// this.tbxImporteTotal.Text;
                factura.totalSinImpuestos = dsImprimir.Tables[2].Rows[0]["subtotal"].ToString();// this.tbxImporteTotal.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "buildFactura");
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Enviar();
            return;
            /// validaciones
            // retorna tablas 0 = cabeza factura y datos del cliente
            // 1 = cuerpo de factura y tarifas de iva
            // 2 = totales de factura factura y tarifas de iva
            // 3 = formas de pago
            // 4 = informacion del punto de venta
            // 5 = informacion config

            ////// TRAE INFORMACION DE FACTURA //////

            int nItems = dsImprimir.Tables[1].Rows.Count;
            int nItemsFpago = dsImprimir.Tables[3].Rows.Count;
            //string _Url = dsImprimir.Tables[5].Rows[0]["surl"].ToString().Trim();

            



            // armo el objeto factura 
            buildFactura();

            // envio factura 
            DocumentResponse response = service.Enviar(tokenEmpresa, tokenAuthorizacion, factura);

            if (response.codigo == 200 || response.codigo == 501)
                MessageBox.Show("La factura  " + response.consecutivoDocumento + " fue procesada. " + "\n" + response.mensaje);
            else
                MessageBox.Show(response.codigo.ToString() + " " + response.mensaje);
            

        }
        public void ActualizaDocFacturaElectronica(DocumentResponse resp)
        {
            string numdocele = resp.consecutivoDocumento;
            string cufe = resp.cufe.Trim();
            string fecharesp = resp.fechaRespuesta.ToString();
            string msg = resp.mensaje;
            string code = resp.codigo.ToString();
            DateTime dtime = DateTime.Now;
            if (!string.IsNullOrEmpty(fecharesp))
            {
                 dtime = Convert.ToDateTime(fecharesp);
            }

            
            /// envia a base de datos en cabeza de documento
                using (SqlConnection connection = new SqlConnection(cnEmp))
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
                        string sqlcab = string.Empty;
                        if (!string.IsNullOrEmpty(fecharesp))
                        {

                            sqlcab = @"update incab_doc set fa_docelect='"+numdocele.Trim()+"',fa_cufe='" + cufe + "',fa_msg='" + msg + "',fa_fecharesp='" + dtime.ToString() + "',fa_codigo='" + code + "' where idreg=" + idrowcab.ToString();
                        }
                        else
                        {
                            sqlcab = @"update incab_doc set fa_docelect='" + numdocele.Trim()+"',fa_cufe ='" + cufe + "',fa_msg='" + msg + "',fa_codigo='" + code + "' where idreg=" + idrowcab.ToString();
                        }
                        command.CommandText = sqlcab;
                        command.ExecuteScalar();
                        transaction.Commit();
                        connection.Close();
                    }
                    catch (SqlException ex)
                    {
                        for (int i = 0; i < ex.Errors.Count; i++)
                        {
                            errorMessages.Append(" SQL-Index #" + i + "\n" + "Message: " + ex.Errors[i].Message + "\n" + "LineNumber: " + ex.Errors[i].LineNumber + "\n" + "Source: " + ex.Errors[i].Source + "\n" + "Procedure: " + ex.Errors[i].Procedure + "\n");
                        }
                        transaction.Rollback();
                        MessageBox.Show(errorMessages.ToString());

                    }
                    catch (Exception ex)
                    {
                        errorMessages.Append("c Error:#" + ex.Message.ToString());
                        transaction.Rollback();
                        MessageBox.Show(errorMessages.ToString());
                    }

                }


        }
        public void Enviar()
        {
            try
            {

                if ((string)dsImprimir.Tables[0].Rows[0]["cod_trn"] == "005")
                {
                    FoliosRemainingResponse responseFolio = service.FoliosRestantes(tokenEmpresa, tokenAuthorizacion);

                    int folrest = Convert.ToInt32(responseFolio.foliosRestantes.ToString());
                    // MessageBox.Show(((3000 - folrest)+100).ToString());
                    txtNumFactura.Text = ((3000 - folrest) + 100).ToString();
                    this.UpdateLayout();
                }

                if((string)dsImprimir.Tables[0].Rows[0]["cod_trn"] != "005")
                {
                    if (string.IsNullOrEmpty(txtNumFactura.Text.Trim()))
                    {
                        MessageBox.Show("Falta numero de documento nota credito credito");
                        return;
                    }
                }
                //factura.consecutivoDocumento = ((3000 - folrest)+100).ToString();
                

                buildFactura();
                //MessageBox.Show(factura.rangoNumeracion + "-" + factura.tipoDocumento + "-" + factura.consecutivoDocumento);

                // envio factura 
                //MessageBox.Show("envia");
                DocumentResponse response = service.Enviar(tokenEmpresa, tokenAuthorizacion, factura);
                //MessageBox.Show("fin envia");
                //if (response.codigo == 200 || response.codigo == 501)
                if (response.codigo == 200 || response.codigo == 201)  //201 documento recibodo se enviara mas tarde a ala dian
                {
                    // almacena cufe en factura
                    ActualizaDocFacturaElectronica(response);
                    this.Codigo = response.codigo.ToString();
                    this.FechaResp = response.fechaRespuesta;
                    this.Msg = response.mensaje;
                    this.Cufe = response.cufe;
                    this.NumDocElect = response.consecutivoDocumento;
                    SiaWin.Auditor(idrowcab, "Factura Electronica:"+response.codigo.ToString() + " " + response.mensaje, _ModuloId, _AccesoId);
                    MessageBox.Show("Documento : " + response.consecutivoDocumento + " fue procesada. " + "\n" + response.mensaje);
                    this.Close();

                }
                else
                {
                    ActualizaDocFacturaElectronica(response);
                    this.Codigo = response.codigo.ToString();
                    this.FechaResp = response.fechaRespuesta;
                    this.Msg = response.mensaje;
                    this.Cufe = response.cufe;


                    //public void Auditor(int iddoc, string msg, int idmodu)
                    SiaWin.Auditor(idrowcab, "Factura Electronica:" + response.codigo.ToString() + " " + response.mensaje, _ModuloId,_AccesoId);

                    MessageBox.Show(response.codigo.ToString() + " " + response.mensaje);

                    //_usercontrol.Seg.Auditor(idregcab, _usercontrol.ProjectId, idUser, _usercontrol.GroupId, idEmp, _usercontrol.ModuleId, _usercontrol.AccesoId, 0, "Salio de: Punto de venta" + " - " + _titulo, "");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Enviar");
            }
        }

        public bool LoadData(int idregdoc,string codpvta,string cn)
        {
            try
            {
                // retorna tablas 0 = cabeza factura y datos del cliente
                // 1 = cuerpo de factura y tarifas de iva
                // 2 = totales de factura factura y tarifas de iva
                // 3 = formas de pago
                // 4 = informacion del punto de venta
                // 5 = informacion config

                SqlConnection con = new SqlConnection(cnEmp);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                //DataSet dsImprimir = new DataSet();
                //PvFacturaElectronicaAnulacion
                cmd = new SqlCommand("PvFacturaElectronica", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumRegCab", idrowcab);//if you have parameters.
                cmd.Parameters.AddWithValue("@CodPvt", codpvt);//if you have parameters.
                da = new SqlDataAdapter(cmd);
                dsImprimir.Clear();
                da.Fill(dsImprimir);
                tokenEmpresa = dsImprimir.Tables[5].Rows[0]["stockenemp_"].ToString().Trim();
                tokenAuthorizacion = dsImprimir.Tables[5].Rows[0]["stockenpas_"].ToString().Trim();
                
                if (string.IsNullOrEmpty(tokenEmpresa))
                {
                    MessageBox.Show("Token de empresa null o vacio");
                    return false;
                }
                if (string.IsNullOrEmpty(tokenAuthorizacion))
                {
                    MessageBox.Show("Token autorizacion  de empresa null o vacio");
                    return false;
                }


                int nItems = dsImprimir.Tables[0].Rows.Count;
                if (nItems <= 0)
                {
                    MessageBox.Show("No hay registro en cabeza de documento..");
                    return false;
                }
                nItems = dsImprimir.Tables[1].Rows.Count;
                if (nItems <= 0)
                {
                    MessageBox.Show("No hay registro en cuerpo de documento..");
                    return false;
                }
                nItems = dsImprimir.Tables[3].Rows.Count;
                if (nItems <= 0)
                {
                    MessageBox.Show("No hay registro en formas de pago en documento..");
                    return false;
                }
                nItems = dsImprimir.Tables[4].Rows.Count;
                if (nItems <= 0)
                {
                    MessageBox.Show("No hay registro informacion punto de venta...");
                    return false;
                }
                if (nItems <= 0)
                {
                    MessageBox.Show("No hay registro informacion Config...");
                    return false;
                }


                 tbxnit.Text = dsImprimir.Tables[0].Rows[0]["cod_cli"].ToString().Trim();
                tbxnombre.Text = dsImprimir.Tables[0].Rows[0]["nom_ter"].ToString().Trim();
                tbxEmail.Text = dsImprimir.Tables[0].Rows[0]["email"].ToString().Trim().ToUpper(); ;
                tbxFechaEmision.Text = dsImprimir.Tables[0].Rows[0]["fec_trn"].ToString().Trim();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "LoadData");
            }
            return false;

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(!LoadData(idrowcab,codpvt,cnEmp))
            {
                MessageBox.Show("Error al cargar los datos del documento....");
                this.Close();
                return;
            }
//            Enviar();
        }
    }
}
