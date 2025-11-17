using Laba2;
using System;
using System.Collections.Generic;
using System.Linq; 
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Laba2
{
    public class LinqStrategy : ISearchStrategy
    {
        public List<Student> Search(SearchCriteria criteria, string xmlPath)
        {
            var results = new List<Student>();
            var doc = XDocument.Load(xmlPath);

          
            var query = from student in doc.Descendants("Student")
                        where (string.IsNullOrEmpty(criteria.Field) || string.IsNullOrEmpty(criteria.Value)) 
                               || 
                              (student.Element(criteria.Field) != null && student.Element(criteria.Field).Value == criteria.Value) 
                        
            select new Student
                        {
                            FullName = student.Element("FullName")?.Value,
                            Faculty = student.Element("Faculty")?.Value,
                            Course = student.Element("Course")?.Value,
                            RoomNumber = student.Element("RoomNumber")?.Value,
                            MonthlyFee = student.Element("MonthlyFee")?.Value
                        };

            results = query.ToList();
            return results;
        }
    }
}