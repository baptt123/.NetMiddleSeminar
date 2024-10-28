using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
namespace WebApplication1.SoapBuilder
{
    public class SoapEnvelopeBuilder
    {
        public string WrapInSoapEnvelope(string responseName, XmlDocument bodyContent)
        {
            XmlDocument soapEnvelope = new XmlDocument();

            // Tạo SOAP Envelope
            XmlElement envelope = soapEnvelope.CreateElement("soap", "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
            soapEnvelope.AppendChild(envelope);

            // Tạo SOAP Body
            XmlElement body = soapEnvelope.CreateElement("soap", "Body", "http://schemas.xmlsoap.org/soap/envelope/");
            envelope.AppendChild(body);

            // Tạo phần phản hồi
            XmlElement response = soapEnvelope.CreateElement(responseName, "http://tempuri.org/");
            body.AppendChild(response);

            // Thêm nội dung vào phản hồi
            response.AppendChild(soapEnvelope.ImportNode(bodyContent.DocumentElement, true));

            return soapEnvelope.OuterXml;
        }
    }
}