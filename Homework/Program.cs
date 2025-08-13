// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace StudentManagementSystem
{
    // 成绩
    public enum Grade
    {
        A,
        B,
        C,
        D,
        F
    };

    // 接口
    public interface IRepository<T>
    {
        // 定义接口方法
        public void Add(T item);
        public bool Remove(T item);
        public List<T> GetAll();
        public List<T> Find(Func<T, bool> predicate);
    }

    // Student类
    public class Student : IComparable<Student>
    {
        // 属性
        public string? StudentId;
        public string? Name;
        public int Age;

        // 构造函数
        public Student(string studentId, string name, int age)
        {
            try
            {
                if (string.IsNullOrEmpty(studentId)||string.IsNullOrEmpty(name))
                {
                    throw new ArgumentNullException("studentId or name can not be null or empty");
                }
                StudentId = studentId;
                Name = name;
                Age = age;
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public override string ToString()
        {
            return $"StudentId: {StudentId}, Name: {Name}, Age: {Age}";
        }

        // 实现IComparable接口
        public int CompareTo(Student? other)
        {
            if (other == null || other.StudentId == null) return 1;
            if (StudentId == null) return -1;
            return StudentId.CompareTo(other.StudentId);
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

    // 成绩
    public class Score
    {
        public string? Subject;
        public double Points;

        public Score(string subject, double points)
        {
            try
            {
                if (string.IsNullOrEmpty(subject))
                {
                    throw new ArgumentNullException(nameof(subject), "Subject cannot be null or empty");
                }
                Subject = subject;
                Points = points;
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public override string ToString()
        {
            return $"Subject: {Subject}, Points: {Points}";
        }
    }

    // 学生管理类
    public class StudentManager : IRepository<Student>
    {
        private List<Student> students = [];

        public void Add(Student item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item), "Student cannot be null");
            students.Add(item);
        }

        public bool Remove(Student item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item), "Student cannot be null");
            return students.Remove(item);
        }

        public List<Student> GetAll()
        {
            return new List<Student>(students);
        }

        public List<Student> Find(Func<Student, bool> predicate)
        {
            List<Student> newList = [];
            foreach (var student in students)
            {
                if (predicate(student))
                {
                    newList.Add(student);
                }
            }
            return newList;
        }

        // 根据年龄范围查找学生
        public List<Student> GetStudentsByAge(int minAge, int maxAge)
        {
            try
            {
                if (minAge < 0 || maxAge < 0 || minAge > maxAge)
                {
                    throw new ArgumentException("Invalid age range");
                }
                return Find(s => s.Age >= minAge && s.Age <= maxAge);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return [];
            }
        }
    }

    // 成绩管理类
    public class ScoreManager
    {
        private Dictionary<string, List<Score>> studentScores = [];

        public void AddScore(string studentId, Score score)
        {
            if (string.IsNullOrEmpty(studentId)) throw new ArgumentNullException(nameof(studentId), "StudentId cannot be null or empty");
            if (score == null) throw new ArgumentNullException(nameof(score), "Score cannot be null");
            if (!studentScores.ContainsKey(studentId))
            {
                studentScores[studentId] = [];
            }
            studentScores[studentId].Add(score);
        }

        public List<Score> GetStudentScores(string studentId)
        {
            if (string.IsNullOrEmpty(studentId)) throw new ArgumentNullException(nameof(studentId), "StudentId cannot be null or empty");
            List<Score> scores = [];
            if (studentScores.ContainsKey(studentId))
            {
                scores = studentScores[studentId];
            }
            return scores;
        }

        public double CalculateAverage(string studentId)
        {
            var scores = GetStudentScores(studentId);
            if (scores.Count == 0) return 0.0;
            double totalPoints = 0.0;
            foreach (var score in scores)
            {
                totalPoints += score.Points;
            }
            return totalPoints / scores.Count;
        }

        // 成绩转换
        public Grade GetGrade(double score)
        {
            switch (score)
            {
                case >= 90:
                    return Grade.A;
                case >= 80:
                    return Grade.B;
                case >= 70:
                    return Grade.C;
                case >= 60:
                    return Grade.D;
                default:
                    return Grade.F;
            }
        }

        public List<(string StudentId, double Average)> GetTopStudents(int count)
        {
            var averages = new List<(string StudentId, double Average)>();
            foreach (var studentId in studentScores.Keys)
            {
                double average = CalculateAverage(studentId);
                averages.Add((studentId, average));
            }
            averages.Sort((x, y) => y.Average.CompareTo(x.Average)); // 降序排序
            return averages.GetRange(0, Math.Min(count, averages.Count)); // 返回前count个学生
        }

        public Dictionary<string, List<Score>> GetAllScores()
        {
            return new Dictionary<string, List<Score>>(studentScores);
        }
    }

    // 数据管理类
    public class DataManager
    {
        public void SaveStudentsToFile(List<Student> students, string filePath)
        {
            try
            {
                if (students == null || string.IsNullOrEmpty(filePath))
                {
                    throw new ArgumentNullException("Students list or file path cannot be null or empty");
                }
                // csv文件格式
                using (StreamWriter writer = new StreamWriter(filePath))
                {
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
            var students = new List<Student>();
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    throw new ArgumentNullException(nameof(filePath), "File path cannot be null or empty");
                }
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var parts = line.Split(',');
                        if (parts.Length == 3 && int.TryParse(parts[2], out int age))
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
                    // TODO: 调用GetStudentsByAge方法并显示结果
                    var studentsInAgeRange = studentManager.GetStudentsByAge(19, 20);
                    foreach (var s in studentsInAgeRange)
                    {
                        Console.WriteLine(s);
                    }

                    // 4. 显示学生成绩统计
                    Console.WriteLine("\n4. 学生成绩统计:");
                    // TODO: 遍历所有学生，显示其成绩、平均分和等级
                    var allStudents = studentManager.GetAll();
                    foreach (var s in allStudents)
                    {
                        var scores = scoreManager.GetStudentScores(s.StudentId ?? "");
                        Console.WriteLine($"\n学生: {s.Name} (ID: {s.StudentId})");
                        foreach (var score in scores)
                        {
                            Console.WriteLine($"  课程: {score.Subject}, 分数: {score.Points}");
                        }
                        double avg = scoreManager.CalculateAverage(s.StudentId ?? "");
                        Grade grade = scoreManager.GetGrade(avg);
                        Console.WriteLine($"  平均分: {avg:F2}, 等级: {grade}");
                    }

                    // 5. 显示排名（简化版）
                    Console.WriteLine("\n5. 平均分最高的学生:");
                    // TODO: 调用GetTopStudents(1)方法显示第一名
                    var topStudent = scoreManager.GetTopStudents(1);
                        if (topStudent.Count > 0)
                        {
                            var student = allStudents.Find(s => s.StudentId == topStudent[0].StudentId);
                            if (student != null)
                            {
                                Console.WriteLine($"第一名: {student.Name} (ID: {student.StudentId}), 平均分: {topStudent[0].Average:F2}");
                            }
                            else
                            {
                                Console.WriteLine("未找到第一名学生信息");
                            }
                        }
                        else
                        {
                            Console.WriteLine("没有学生成绩信息");
                        }

                    // 6. 文件操作
                    Console.WriteLine("\n6. 数据持久化演示:");
                    // TODO: 保存和读取学生文件
                    string filePath = "students.csv";
                    dataManager.SaveStudentsToFile(allStudents, filePath);
                    dataManager.LoadStudentsFromFile(filePath);
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
}