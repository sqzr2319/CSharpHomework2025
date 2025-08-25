using System;
using System.Collections.Generic;
using System.IO;

namespace StudentManagementSystem
{
    public enum Grade
    {
        F = 0,
        D = 60,
        C = 70,
        B = 80,
        A = 90
    }

    public interface IRepository<T>
    {
        void Add(T item);
        bool Remove(T item);
        List<T> GetAll();
        List<T> Find(Func<T, bool> predicate);
    }

    public class Student : IComparable<Student>
    {
        public string StudentId { get; }
        public string Name { get; }
        public int Age { get; }

        public Student(string studentId, string name, int age)
        {
            if (string.IsNullOrEmpty(studentId))
                throw new ArgumentNullException(nameof(studentId));
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            StudentId = studentId;
            Name = name;
            Age = age;
        }

        public override string ToString()
        {
            return $"学号: {StudentId}, 姓名: {Name}, 年龄: {Age}";
        }

        public int CompareTo(Student? other)
        {
            if (other == null) return 1;
            return string.Compare(StudentId, other.StudentId, StringComparison.Ordinal);
        }

        public override bool Equals(object? obj)
        {
            return obj is Student student && StudentId == student.StudentId;
        }

        public override int GetHashCode()
        {
            return StudentId?.GetHashCode() ?? 0;
        }
    }

    public class Score
    {
        public string Subject { get; }
        public double Points { get; }

        public Score(string subject, double points)
        {
            if (string.IsNullOrEmpty(subject))
                throw new ArgumentNullException(nameof(subject));

            Subject = subject;
            Points = points;
        }

        public override string ToString()
        {
            return $"科目: {Subject}, 分数: {Points}";
        }
    }

    public class StudentManager : IRepository<Student>
    {
        private List<Student> _students = new List<Student>();

        public void Add(Student item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            _students.Add(item);
        }

        public bool Remove(Student item)
        {
            return _students.Remove(item);
        }

        public List<Student> GetAll()
        {
            return new List<Student>(_students);
        }

        public List<Student> Find(Func<Student, bool> predicate)
        {
            var result = new List<Student>();
            foreach (var student in _students)
            {
                if (predicate(student))
                    result.Add(student);
            }
            return result;
        }

        public List<Student> GetStudentsByAge(int minAge, int maxAge)
        {
            var result = new List<Student>();
            foreach (var student in _students)
            {
                if (student.Age >= minAge && student.Age <= maxAge)
                    result.Add(student);
            }
            return result;
        }
    }

    public class ScoreManager
    {
        private Dictionary<string, List<Score>> _scores = new Dictionary<string, List<Score>>();

        public void AddScore(string studentId, Score score)
        {
            if (string.IsNullOrEmpty(studentId))
                throw new ArgumentNullException(nameof(studentId));
            if (score == null)
                throw new ArgumentNullException(nameof(score));

            if (!_scores.ContainsKey(studentId))
                _scores[studentId] = new List<Score>();

            _scores[studentId].Add(score);
        }

        public List<Score> GetStudentScores(string studentId)
        {
            if (_scores.TryGetValue(studentId, out var scores))
                return new List<Score>(scores);
            return new List<Score>();
        }

        public double CalculateAverage(string studentId)
        {
            var scores = GetStudentScores(studentId);
            if (scores.Count == 0)
                return 0;

            double total = 0;
            foreach (var score in scores)
            {
                total += score.Points;
            }
            return total / scores.Count;
        }

        public Grade GetGrade(double score)
        {
            if (score >= 90) return Grade.A;
            if (score >= 80) return Grade.B;
            if (score >= 70) return Grade.C;
            if (score >= 60) return Grade.D;
            return Grade.F;
        }

        public List<(string StudentId, double Average)> GetTopStudents(int count)
        {
            var averages = new List<(string StudentId, double Average)>();
            foreach (var kvp in _scores)
            {
                double avg = CalculateAverage(kvp.Key);
                averages.Add((kvp.Key, avg));
            }

            averages.Sort((a, b) => b.Average.CompareTo(a.Average));
            return averages.GetRange(0, Math.Min(count, averages.Count));
        }

        public Dictionary<string, List<Score>> GetAllScores()
        {
            return new Dictionary<string, List<Score>>(_scores);
        }
    }

    public class DataManager
    {
        public void SaveStudentsToFile(List<Student> students, string filePath)
        {
            try
            {
                using StreamWriter writer = new StreamWriter(filePath);
                foreach (var student in students)
                {
                    writer.WriteLine($"{student.StudentId},{student.Name},{student.Age}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存文件时发生错误: {ex.Message}");
            }
        }

        public List<Student> LoadStudentsFromFile(string filePath)
        {
            List<Student> students = new List<Student>();
            try
            {
                using StreamReader reader = new StreamReader(filePath);
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split(',');
                    if (parts.Length != 3)
                        continue;

                    string studentId = parts[0];
                    string name = parts[1];
                    if (int.TryParse(parts[2], out int age))
                    {
                        students.Add(new Student(studentId, name, age));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取文件时发生错误: {ex.Message}");
            }
            return students;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== 学生成绩管理系统 ===\n");
            var studentManager = new StudentManager();
            var scoreManager = new ScoreManager();
            var dataManager = new DataManager();

            try
            {
                Console.WriteLine("1. 添加学生信息:");
                studentManager.Add(new Student("2021001", "张三", 20));
                studentManager.Add(new Student("2021002", "李四", 19));
                studentManager.Add(new Student("2021003", "王五", 21));
                Console.WriteLine("学生信息添加完成");

                Console.WriteLine("\n2. 添加成绩信息:");
                scoreManager.AddScore("2021001", new Score("数学", 95.5));
                scoreManager.AddScore("2021001", new Score("英语", 87.0));

                scoreManager.AddScore("2021002", new Score("数学", 78.5));
                scoreManager.AddScore("2021002", new Score("英语", 85.5));

                scoreManager.AddScore("2021003", new Score("数学", 88.0));
                scoreManager.AddScore("2021003", new Score("英语", 92.0));
                Console.WriteLine("成绩信息添加完成");

                Console.WriteLine("\n3. 查找年龄在19-20岁的学生:");
                var studentsInAgeRange = studentManager.GetStudentsByAge(19, 20);
                foreach (var student in studentsInAgeRange)
                {
                    Console.WriteLine(student);
                }

                Console.WriteLine("\n4. 学生成绩统计:");
                var allStudents = studentManager.GetAll();
                foreach (var student in allStudents)
                {
                    var scores = scoreManager.GetStudentScores(student.StudentId);
                    double average = scoreManager.CalculateAverage(student.StudentId);
                    Grade grade = scoreManager.GetGrade(average);
                    Console.WriteLine($"{student} - 平均分: {average:F2}, 等级: {grade}");
                    foreach (var score in scores)
                    {
                        Console.WriteLine($"  {score}");
                    }
                }

                Console.WriteLine("\n5. 平均分最高的学生:");
                var topStudents = scoreManager.GetTopStudents(1);
                if (topStudents.Count > 0)
                {
                    var top = topStudents[0];
                    Console.WriteLine($"学号: {top.StudentId}, 平均分: {top.Average:F2}");
                }

                Console.WriteLine("\n6. 数据持久化演示:");
                string filePath = "students.csv";
                dataManager.SaveStudentsToFile(allStudents, filePath);
                Console.WriteLine("学生数据已保存到文件");

                var loadedStudents = dataManager.LoadStudentsFromFile(filePath);
                Console.WriteLine("从文件加载的学生数据:");
                foreach (var student in loadedStudents)
                {
                    Console.WriteLine(student);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"程序执行过程中发生错误: {ex.Message}");
            }

            Console.WriteLine("\n程序执行完毕，按任意键退出...");
            Console.ReadKey();
        }
    }
}