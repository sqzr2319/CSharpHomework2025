using System;
using System.Collections.Generic;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Text;
using System.Reflection.Metadata.Ecma335;
namespace StudentManagementSystem
{
    public enum Grade
    {

        A = 90,
        B = 80,
        C = 70,
        D = 60,
        F = 0
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
        public string Name;
        public int Age;
        public string StudentId;

        public Student(string studentId, string name, int age)
        {
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(studentId);
            ArgumentNullException.ThrowIfNull(age);
            Name = name;
            Age = age;
            StudentId = studentId;
        }

        public override string ToString()
        {
            return $"{Name}+{StudentId}+{Age}";
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
        public string Subject;
        public double Points;
        public Score(string subject, double points)
        {
            ArgumentNullException.ThrowIfNull(subject);
            ArgumentNullException.ThrowIfNull(points);
            Subject = subject;
            Points = points;
        }

        public override string ToString()
        {
            return $"{Subject}+{Points}";
        }
    }

    public class StudentManager : IRepository<Student>
    {
        private List<Student> students = new List<Student>();

        public void Add(Student item)
        {
            ArgumentNullException.ThrowIfNull(item);
            students.Add(item);
        }

        public bool Remove(Student item)
        {
            return students.Remove(item);
        }

        public List<Student> GetAll()
        {
            return new List<Student>(students);
        }

        public List<Student> Find(Func<Student, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            List<Student> pstudents = new List<Student>();
            foreach (Student student in students)
            {
                if (predicate(student)){
                    pstudents.Add(student);
                }
            }
            return pstudents;
        }

        public List<Student> GetStudentsByAge(int minAge , int maxAge)
        {
            ArgumentNullException.ThrowIfNull(minAge);
            ArgumentNullException.ThrowIfNull(maxAge);

            List<Student> pstudents = new List<Student>();
            foreach (Student student in students)
            {
                if(student.Age >= minAge && student.Age <= maxAge)
                {
                    pstudents.Add(student);
                }
            }
            return pstudents;
        }

    }

    public class ScoreManager
    {
        private Dictionary<string, List<Score>>? scoredic = new Dictionary<string, List<Score>>();

        public void AddScore(string studentId, Score score)
        {
            ArgumentNullException.ThrowIfNull(studentId);
            ArgumentNullException.ThrowIfNull(score);
            if (scoredic == null)
            {
                scoredic = new Dictionary<string, List<Score>>();
            }
            if (!scoredic.ContainsKey(studentId))
            {
                scoredic.Add(studentId, new List<Score>());
            }
            scoredic[studentId].Add(score);
        }

        public List<Score> GetStudentScores(string studentId)
        {
            ArgumentNullException.ThrowIfNull(studentId);
            if (scoredic != null && scoredic.ContainsKey(studentId))
            {
                return new List<Score>(scoredic[studentId]);
            }
            return new List<Score>();
        }

        public double CalculateAverage(string studentId)
        {
            ArgumentNullException.ThrowIfNull(studentId);
            if (scoredic != null && scoredic.ContainsKey(studentId))
            {
                List<Score> scores = scoredic[studentId];
                if (scores.Count == 0) return 0;
                double total = 0;
                foreach (var score in scores)
                {
                    total += score.Points;
                }
                return total / scores.Count;
            }
            return 0;
        }

        public Grade GetGrade(double score)
        {
            if (score >= (double)Grade.A)
                return Grade.A;
            else if (score >= (double)Grade.B)
                return Grade.B;
            else if (score >= (double)Grade.C)
                return Grade.C;
            else if (score >= (double)Grade.D)
                return Grade.D;
            else
                return Grade.F;
        }

        public List<(string StudentId,double Average)> GetTopStudents(int count)
        {
            List<(string,double)> stuave= new List<(string, double)>();
            if (scoredic == null || scoredic.Count == 0) return new List<(string, double)>();
            foreach (var studentScores in scoredic)
            {
                if (studentScores.Value.Count == 0) continue;
                double average = CalculateAverage(studentScores.Key);
                stuave.Add((studentScores.Key, average));

            }
            stuave.Sort((x, y) => y.Item2.CompareTo(x.Item2)); // Sort by average score descending
            if (count > stuave.Count) count = stuave.Count;
            return stuave.GetRange(0, count);
        }

        public Dictionary<string,List<Score>> GetAllScores()
        {
            if (scoredic == null) return new Dictionary<string, List<Score>>();
            return new Dictionary<string, List<Score>>(scoredic);
        }
    }

    public class DataManager
    {
        public void SaveStudentsToFile(List<Student> students, string filepath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filepath, 
                    false, 
                    Encoding.UTF8))
                {
                    writer.WriteLine("Name,StudentId,Age");
                    foreach (var student in students)
                    {
                        writer.WriteLine($"{student.Name},{student.StudentId},{student.Age}");
                    }
                    Console.WriteLine($"CSV文件已成功生成：{Path.GetFullPath(filepath)}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存文件时发生错误：{ex.Message}");
            }
        }
        public List<Student> LoadStudentsFromFile(string filepath)
        {
            List<Student> students = new List<Student>();
            try
            {
                using (StreamReader reader = new StreamReader(filepath, Encoding.UTF8))
                {
                    string header = reader.ReadLine() ?? ""; // 读取表头
                    if (header != "Name,StudentId,Age")
                    {
                        throw new InvalidDataException("CSV文件格式不正确");
                    }
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine() ?? "";
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        string[] parts = line.Split(',');
                        if (parts.Length != 3) continue; // 确保每行有三个部分
                        string name = parts[0].Trim();
                        string studentId = parts[1].Trim();
                        if (!int.TryParse(parts[2].Trim(), out int age))
                        {
                            throw new InvalidDataException("年龄格式不正确");
                        }
                        students.Add(new Student(studentId, name, age));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取文件时发生错误：{ex.Message}");
            }
            return students;
        }
        
    }
    class Program
    {
        static void Main(string[] args)
        {
            // 示例代码可以在这里添加
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
                // 2. 成绩数据（每个学生各2门课程）
                Console.WriteLine("\n2. 添加成绩信息:");
                scoreManager.AddScore("2021001", new Score("数学", 95.5));
                scoreManager.AddScore("2021001", new Score("英语", 87.0));
                scoreManager.AddScore("2021002", new Score("数学", 78.5));
                scoreManager.AddScore("2021002", new Score("英语", 85.5));
                scoreManager.AddScore("2021003", new Score("数学", 88.0));
                scoreManager.AddScore("2021003", new Score("英语", 92.0));
                Console.WriteLine("成绩信息添加完成");
                Console.WriteLine("\n3. 查找年龄在19-20岁的学生:");
                var fdlst=studentManager.GetStudentsByAge(19, 20);
                foreach (var student in fdlst)
                {
                    Console.WriteLine($"\n{student}");
                }
                Console.WriteLine("\n4. 学生成绩统计:");
                foreach (var student in studentManager.GetAll())
                {
                    var scores = scoreManager.GetStudentScores(student.StudentId);
                    Console.WriteLine($"\n学生: {student.Name} ({student.StudentId})");
                    if (scores.Count > 0)
                    {
                        Console.WriteLine("成绩:");
                        foreach (var score in scores)
                        {
                            Console.WriteLine($"科目: {score.Subject}, 分数: {score.Points}");
                        }
                        double average = scoreManager.CalculateAverage(student.StudentId);
                        Grade grade = scoreManager.GetGrade(average);
                        Console.WriteLine($"平均分: {average}, 等级: {grade}");
                    }
                    else
                    {
                        Console.WriteLine("无成绩记录");
                    }
                }
                Console.WriteLine("\n5. 平均分最高的学生:");
                var bst=scoreManager.GetTopStudents(1);
                Console.WriteLine($"学生ID: {bst[0].StudentId}, 平均分: {bst[0].Average}");

                Console.WriteLine("\n6. 数据持久化演示:");
                string filepath = "students.csv";
                dataManager.SaveStudentsToFile(studentManager.GetAll(), filepath);
                dataManager.LoadStudentsFromFile(filepath);
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
