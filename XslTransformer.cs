using System.Xml.Xsl;

namespace Laba2
{
    public class XslTransformer
    {
        public void Transform(string xmlPath, string xsltPath, string htmlPath)
        {
            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(xsltPath);
            xslt.Transform(xmlPath, htmlPath);
        }
    }
}
