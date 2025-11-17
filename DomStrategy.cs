using Laba2;
using System.Collections.Generic;
using System.Xml;

namespace Laba2
{
    public class DomStrategy : ISearchStrategy
    {
        public List<Student> Search(SearchCriteria criteria, string xmlPath)
        {
            var results = new List<Student>();
            var doc = new XmlDocument();
            doc.Load(xmlPath);

            string xPath = $"//Student[{criteria.Field}='{criteria.Value}']";

            if (string.IsNullOrEmpty(criteria.Field) || string.IsNullOrEmpty(criteria.Value))
            {
                xPath = "//Student";
            }

            XmlNodeList nodes = doc.SelectNodes(xPath);

            foreach (XmlNode node in nodes)
            {
                results.Add(new Student
                {
                    FullName = node["FullName"]?.InnerText,
                    Faculty = node["Faculty"]?.InnerText,
                    Course = node["Course"]?.InnerText,
                    RoomNumber = node["RoomNumber"]?.InnerText,
                    MonthlyFee = node["MonthlyFee"]?.InnerText
                });
            }
            return results;
        }
    }
}