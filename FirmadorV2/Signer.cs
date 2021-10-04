using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using FirmaXadesNet;
using FirmaXadesNet.Crypto;
using FirmaXadesNet.Signature.Parameters;

namespace FirmadorV2
{
    public class Signer
    {
        public string FirmaXML_Xades(string pathXML, string thumbprintCertificado, int certType)
        {

            try
            { 
                if(certType == 0)
                {
                    X509Certificate2 cert = new X509Certificate2("C:\\Certificados\\" + thumbprintCertificado + ".p12", "2020");
                    //X509Certificate2 cert = GetCertificateByThumbprint(thumbprintCertificado);
                    XadesService xadesService = new XadesService();
                    SignatureParameters parametros = new SignatureParameters();

                    parametros.SignaturePolicyInfo = new SignaturePolicyInfo();
                    parametros.SignaturePolicyInfo.PolicyIdentifier = "https://tribunet.hacienda.go.cr/docs/esquemas/2016/v4.1/Resolucion_Comprobantes_Electronicos_DGT-R-48-2016.pdf";
                    //La propiedad PolicyHash es la misma para todos, es un cálculo en base al archivo pdf indicado en PolicyIdentifier
                    parametros.SignaturePolicyInfo.PolicyHash = "Ohixl6upD6av8N7pEvDABhEL6hM=";
                    parametros.SignaturePackaging = SignaturePackaging.ENVELOPED;
                    parametros.DataFormat = new DataFormat();
                    parametros.Signer = new FirmaXadesNet.Crypto.Signer(cert);
                    FileStream fs = new FileStream((pathXML + ".xml"), FileMode.Open);
                    FirmaXadesNet.Signature.SignatureDocument docFirmado = xadesService.Sign(fs, parametros);
                    docFirmado.Save((pathXML + "_Signed.xml"));

                    // El documento se firma con el dll FirmaXadesNet
                    // Esta libreria fue creada por Departamento de Nuevas Tecnologias - Direccion General de Urbanismo Ayuntamiento de Cartagena
                    // 'Fuente original se puede descargar en administracionelectronica.gob.es/ctt/firmaxadesnet
                    // 'La libreria se modifico levemente para que pueda funcionar para Costa Rica.
                    // 'Cambios por Roy Rojas - royrojas@dotnetcr.com - 06/Febrero/2018

                    fs.Close();
                    docFirmado = null;
                    return "Firma Finalizada.";
                }
                else
                {
                    X509Certificate2 cert = GetCertificateByThumbprint(thumbprintCertificado);
                    XadesService xadesService = new XadesService();
                    SignatureParameters parametros = new SignatureParameters();

                    parametros.SignaturePolicyInfo = new SignaturePolicyInfo();
                    parametros.SignaturePolicyInfo.PolicyIdentifier = "https://tribunet.hacienda.go.cr/docs/esquemas/2016/v4.1/Resolucion_Comprobantes_Electronicos_DGT-R-48-2016.pdf";
                    //La propiedad PolicyHash es la misma para todos, es un cálculo en base al archivo pdf indicado en PolicyIdentifier
                    parametros.SignaturePolicyInfo.PolicyHash = "Ohixl6upD6av8N7pEvDABhEL6hM=";
                    parametros.SignaturePackaging = SignaturePackaging.ENVELOPED;
                    parametros.DataFormat = new DataFormat();
                    parametros.Signer = new FirmaXadesNet.Crypto.Signer(cert);
                    FileStream fs = new FileStream((pathXML + ".xml"), FileMode.Open);
                    FirmaXadesNet.Signature.SignatureDocument docFirmado = xadesService.Sign(fs, parametros);
                    docFirmado.Save((pathXML + "_Signed.xml"));

                    // El documento se firma con el dll FirmaXadesNet
                    // Esta libreria fue creada por Departamento de Nuevas Tecnologias - Direccion General de Urbanismo Ayuntamiento de Cartagena
                    // 'Fuente original se puede descargar en administracionelectronica.gob.es/ctt/firmaxadesnet
                    // 'La libreria se modifico levemente para que pueda funcionar para Costa Rica.
                    // 'Cambios por Roy Rojas - royrojas@dotnetcr.com - 06/Febrero/2018

                    fs.Close();
                    docFirmado = null;
                    return "Firma Finalizada.";

                }

                return "No entro a ninguno";
                    
                

                // 'Ejemplo de un valor Thumbprint: C2E8D9DA714C98ED14B88ECBC4C3E5F3BD64F125
                // 'Si no se quiere leer el certificado del repositorio, se puede cargar el certificado directamente
                // 'Dim cert As X509Certificate2 = New X509Certificate2("rutaArchivoCertificado", "clave")

               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public X509Certificate2 GetCertificateByThumbprint(string thumbprintCertificado)
        {
            X509Certificate2 cert = null;
            X509Store store = new X509Store("My", StoreLocation.CurrentUser);
            try
            {
                store.Open((OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly));
                X509Certificate2Collection CertCol = store.Certificates;
                foreach (X509Certificate2 c in CertCol)
                {
                    if ((c.Thumbprint == thumbprintCertificado))
                    {
                        cert = c;
                        break;
                    }
                }

                if ((cert == null))
                {
                    store = new X509Store("My", StoreLocation.LocalMachine);
                    CertCol = store.Certificates;
                    foreach (X509Certificate2 c in CertCol)
                    {
                        if ((c.Thumbprint == thumbprintCertificado))
                        {
                            cert = c;
                            break;
                        }
                    }
                }

                if ((cert == null))
                {
                    throw new CryptographicException("El certificado no se encuentra registrado");
                }
            }
            finally
            {
                store.Close();
            }
            return cert;
        }

        public string Procesa(string xmlFactura, string txtFolderSalida, string certId, int certType)
        {
            XmlDocument xmlDocSF = new XmlDocument();
            xmlDocSF.LoadXml(xmlFactura);
            xmlDocSF.Save((txtFolderSalida + ".xml"));
            XmlTextWriter xmlTextWriter = new XmlTextWriter((txtFolderSalida + ".xml"), new System.Text.UTF8Encoding(false));
            xmlDocSF.WriteTo(xmlTextWriter);
            xmlTextWriter.Close();
            FirmaXML_Xades(txtFolderSalida, certId, certType);
            XmlDocument xmlElectronica = new XmlDocument();
            xmlElectronica.Load((txtFolderSalida + "_Signed.xml"));
            Console.WriteLine(xmlElectronica.OuterXml);
            return xmlElectronica.OuterXml;
        }
    }
}
