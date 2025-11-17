using Laba2;
using System.Collections.Generic;
using System.Xml;

namespace Laba2
{
    public class SaxStrategy : ISearchStrategy
    {
        public List<Student> Search(SearchCriteria criteria, string xmlPath)
        {
            var results = new List<Student>();
            Student currentStudent = null;
            string currentElement = "";
            bool foundMatch = false; 

            bool searchAll = string.IsNullOrEmpty(criteria.Field) || string.IsNullOrEmpty(criteria.Value);

            using (XmlReader reader = XmlReader.Create(xmlPath))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        currentElement = reader.Name;
                        if (currentElement == "Student")
                        {
                            currentStudent = new Student();
                            foundMatch = searchAll; 
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.Text && currentStudent != null)
                    {
                        string value = reader.Value;
                        switch (currentElement)
                        {
                            case "FullName": currentStudent.FullName = value; break;
                            case "Faculty": currentStudent.Faculty = value; break;
                            case "Course": currentStudent.Course = value; break;
                            case "RoomNumber": currentStudent.RoomNumber = value; break;
                            case "MonthlyFee": currentStudent.MonthlyFee = value; break;
                        }

                        if (!searchAll && currentElement == criteria.Field && value == criteria.Value)
                        {
                            foundMatch = true;
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Student")
                    {
                        if (currentStudent != null && foundMatch)
                        {
                            results.Add(currentStudent);
                        }
                        currentStudent = null;
                        foundMatch = false; 
                    }
                }
            }
            return results;
        }
    }
}