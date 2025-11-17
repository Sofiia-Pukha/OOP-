using System.Collections.Generic;

namespace Laba2 
{
    public class Student
    {
        public string FullName { get; set; }
        public string Faculty { get; set; }
        public string Course { get; set; }
        public string RoomNumber { get; set; }
        public string MonthlyFee { get; set; }

        public override string ToString()
        {
            return $"ПІП: {FullName}\nФакультет: {Faculty}\nКурс: {Course}\nКімната: {RoomNumber}\nПлата: {MonthlyFee} грн\n";
        }
    }

    public class SearchCriteria
    {
        public string Field { get; set; } 
        public string Value { get; set; } 
    }

    public interface ISearchStrategy
    {
        List<Student> Search(SearchCriteria criteria, string xmlPath);
    }
}