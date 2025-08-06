using System;
using System.Collections.Generic;
using System.IO;

namespace StudentManagementSystem
{
    // 成绩等级枚举
    public enum Grade
    {
        F = 0,   // 成绩低于60分
        D = 60,  // 60分及以上
        C = 70,  // 70分及以上
        B = 80,  // 80分及以上
        A = 90   // 90分及以上
    }

    // 泛型仓储接口
    public interface IRepository<T>
    {
        void Add(T item);
        bool Remove(T item);
        List<T> GetAll();
        List<T> Find(Func<T, bool> predicate);
    }

    // 学生类
    public class Student : IComparable<Student>
    {
        public string StudentId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        public Student(string studentId, string name, int age)
        {
            if (string.IsNullOrWhiteSpace(studentId))
                throw new ArgumentException("学号不能为空");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("姓名不能为空");
            if (age < 0 || age > 150)
                throw new ArgumentException("年龄必须在0到150之间");

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

    // 成绩类
    public class Score
    {
        public string Subject { get; set; }
        public double Points { get; set; }

        public Score(string subject, double points)
        {
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("科目不能为空");
            if (points < 0 || points > 100)
                throw new ArgumentException("分数必须在0到100之间");

            Subject = subject;
            Points = points;
        }

        public override string ToString()
        {
            return $"科目: {Subject}, 分数: {Points:F1}";
        }
    }

    // 学生管理类
    public class StudentManager : IRepository<Student>
    {
        private List<Student> students = new List<Student>();

        public void Add(Student item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (students.Contains(item))
                throw new ArgumentException($"学号 {item.StudentId} 已存在");

            students.Add(item);
        }

        public bool Remove(Student item)
        {
            if (item == null)
                return false;
            return students.Remove(item);
        }

        public List<Student> GetAll()
        {
            return new List<Student>(students);
        }

        public List<Student> Find(Func<Student, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            List<Student> result = new List<Student>();
            foreach (var student in students)
            {
                if (predicate(student))
                    result.Add(student);
            }
            return result;
        }

        public List<Student> GetStudentsByAge(int minAge, int maxAge)
        {
            if (minAge < 0 || maxAge < minAge)
                throw new ArgumentException("年龄范围无效");

            List<Student> result = new List<Student>();
            foreach (var student in students)
            {
                if (student.Age >= minAge && student.Age <= maxAge)
                    result.Add(student);
            }
            return result;
        }
    }

    // 成绩管理类
    public class ScoreManager
    {
        private Dictionary<string, List<Score>> scores = new Dictionary<string, List<Score>>();

        public void AddScore(string studentId, Score score)
        {
            if (string.IsNullOrWhiteSpace(studentId))
                throw new ArgumentException("学号不能为空");
            if (score == null)
                throw new ArgumentNullException(nameof(score));

            if (!scores.ContainsKey(studentId))
                scores[studentId] = new List<Score>();

            scores[studentId].Add(score);
        }

        public List<Score> GetStudentScores(string studentId)
        {
            if (string.IsNullOrWhiteSpace(studentId))
                throw new ArgumentException("学号不能为空");

            return scores.ContainsKey(studentId) ? new List<Score>(scores[studentId]) : new List<Score>();
        }

        public double CalculateAverage(string studentId)
        {
            if (string.IsNullOrWhiteSpace(studentId))
                throw new ArgumentException("学号不能为空");

            if (!scores.ContainsKey(studentId) || scores[studentId].Count == 0)
                return 0.0;

            double total = 0.0;
            foreach (var score in scores[studentId])
            {
                total += score.Points;
            }
            return total / scores[studentId].Count;
        }

        public Grade GetGrade(double score)
        {
            return score switch
            {
                >= 90 => Grade.A,
                >= 80 => Grade.B,
                >= 70 => Grade.C,
                >= 60 => Grade.D,
                _ => Grade.F
            };
        }

        public List<(string StudentId, double Average)> GetTopStudents(int count)
        {
            if (count < 1)
                throw new ArgumentException("数量必须大于0");

            var averages = new List<(string StudentId, double Average)>();
            foreach (var studentId in scores.Keys)
            {
                double avg = CalculateAverage(studentId);
                averages.Add((studentId, avg));
            }

            averages.Sort((x, y) => y.Average.CompareTo(x.Average));
            return averages.Take(count).ToList();
        }

        public Dictionary<string, List<Score>> GetAllScores()
        {
            return new Dictionary<string, List<Score>>(scores);
        }
    }

    // 数据管理类
    public class DataManager
    {
        public void SaveStudentsToFile(List<Student> students, string filePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("StudentId,Name,Age");
                    foreach (var student in students)
                    {
                        writer.WriteLine($"{student.StudentId},{student.Name},{student.Age}");
                    }
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
                if (!File.Exists(filePath))
                    return students;

                using (StreamReader reader = new StreamReader(filePath))
                {
                    string header = reader.ReadLine(); // 跳过标题行
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        string[] parts = line.Split(',');
                        if (parts.Length == 3 &&
                            int.TryParse(parts[2], out int age))
                        {
                            students.Add(new Student(parts[0], parts[1], age));
                        }
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

    // 主程序
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== 学生成绩管理系统 ===\n");

            // 创建管理器实例
            var studentManager = new StudentManager();
            var scoreManager = new ScoreManager();
            var dataManager = new DataManager();

            try
            {
                // 1. 学生数据（共3个学生）
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

                // 3. 测试年龄范围查询
                Console.WriteLine("\n3. 查找年龄在19-20岁的学生:");
                var studentsByAge = studentManager.GetStudentsByAge(19, 20);
                foreach (var student in studentsByAge)
                {
                    Console.WriteLine(student.ToString());
                }

                // 4. 显示学生成绩统计
                Console.WriteLine("\n4. 学生成绩统计:");
                foreach (var student in studentManager.GetAll())
                {
                    var scores = scoreManager.GetStudentScores(student.StudentId);
                    double average = scoreManager.CalculateAverage(student.StudentId);
                    Grade grade = scoreManager.GetGrade(average);
                    Console.WriteLine($"{student.ToString()}, 平均分: {average:F1}, 等级: {grade}");
                    foreach (var score in scores)
                    {
                        Console.WriteLine($"  {score.ToString()}");
                    }
                }

                // 5. 显示排名（简化版）
                Console.WriteLine("\n5. 平均分最高的学生:");
                var topStudents = scoreManager.GetTopStudents(1);
                foreach (var (studentId, average) in topStudents)
                {
                    var student = studentManager.Find(s => s.StudentId == studentId).FirstOrDefault();
                    Console.WriteLine($"学号: {studentId}, 姓名: {student?.Name}, 平均分: {average:F1}");
                }

                // 6. 文件操作
                Console.WriteLine("\n6. 数据持久化演示:");
                string filePath = "students.csv";
                dataManager.SaveStudentsToFile(studentManager.GetAll(), filePath);
                Console.WriteLine($"学生数据已保存到 {filePath}");
                var loadedStudents = dataManager.LoadStudentsFromFile(filePath);
                Console.WriteLine("从文件中加载的学生数据:");
                foreach (var student in loadedStudents)
                {
                    Console.WriteLine(student.ToString());
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
