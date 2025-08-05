// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StudentManagementSystem
{
    // 成绩等级枚举
    public enum Grade
    {
        // TODO: 定义成绩等级 F(0), D(60), C(70), B(80), A(90)
        F = 0,
        D = 60,
        C = 70,
        B = 80,
        A = 90
    }

    // 泛型仓储接口
    public interface IRepository<T>
    {
        // TODO: 定义接口方法
        // Add(T item)
        // Remove(T item) 返回bool
        // GetAll() 返回List<T>
        // Find(Func<T, bool> predicate) 返回List<T>
        void Add(T item);
        bool Remove(T item);
        List<T> GetAll();
        List<T> Find(Func<T, bool> predicate);
    }

    // 学生类
    public class Student : IComparable<Student>
    {
        // TODO: 定义字段 StudentId, Name, Age
        public string StudentId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        public Student(string studentId, string name, int age)
        {
            // TODO: 实现构造方法，包含参数验证（空值检查）
            if (string.IsNullOrWhiteSpace(studentId) || string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("学生ID和姓名不能为空。");
            }
            if (age <= 0)
            {
                throw new ArgumentException("年龄必须为正数。");
            }
            StudentId = studentId;
            Name = name;
            Age = age;
        }

        public override string ToString()
        {
            // TODO: 返回格式化的学生信息字符串
            return $"学号: {StudentId}, 姓名: {Name}, 年龄: {Age}";
        }

        // TODO: 实现IComparable接口，按学号排序
        // 提示：使用string.Compare方法
        public int CompareTo(Student? other)
        {
            if (other == null) return 1;
            return string.Compare(this.StudentId, other.StudentId, StringComparison.Ordinal);
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
        // TODO: 定义字段 Subject, Points
        public string Subject { get; set; }
        public double Points { get; set; }

        public Score(string subject, double points)
        {
            // TODO: 实现构造方法，包含参数验证
            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentException("科目不能为空。");
            }
            if (points < 0 || points > 100)
            {
                throw new ArgumentException("分数必须在0到100之间。");
            }
            Subject = subject;
            Points = points;
        }

        public override string ToString()
        {
            // TODO: 返回格式化的成绩信息
            return $"科目: {Subject}, 分数: {Points}";
        }
    }

    // 学生管理类
    public class StudentManager : IRepository<Student>
    {
        // TODO: 定义私有字段存储学生列表
        // 提示：使用List<Student>存储
        private readonly List<Student> _students = new List<Student>();

        public void Add(Student item)
        {
            // TODO: 实现添加学生的逻辑
            // 1. 参数验证
            // 2. 添加到列表
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "学生信息不能为空。");
            }
            if (_students.Contains(item))
            {
                throw new InvalidOperationException("已存在相同学号的学生。");
            }
            _students.Add(item);
        }

        public bool Remove(Student item)
        {
            // TODO: 实现Remove方法
            if (item == null) return false;
            return _students.Remove(item);
        }

        public List<Student> GetAll()
        {
            // TODO: 返回学生列表的副本
            return new List<Student>(_students);
        }

        public List<Student> Find(Func<Student, bool> predicate)
        {
            // TODO: 使用foreach循环查找符合条件的学生
            var result = new List<Student>();
            foreach (var student in _students)
            {
                if (predicate(student))
                {
                    result.Add(student);
                }
            }
            return result;
        }

        // 查找年龄在指定范围内的学生
        public List<Student> GetStudentsByAge(int minAge, int maxAge)
        {
            // TODO: 使用foreach循环和if判断实现年龄范围查询
            var result = new List<Student>();
            foreach (var student in _students)
            {
                if (student.Age >= minAge && student.Age <= maxAge)
                {
                    result.Add(student);
                }
            }
            return result;
        }
    }

    // 成绩管理类
    public class ScoreManager
    {
        // TODO: 定义私有字段存储成绩字典
        // 提示：使用Dictionary<string, List<Score>>存储
        private readonly Dictionary<string, List<Score>> _scores = new Dictionary<string, List<Score>>();

        public void AddScore(string studentId, Score score)
        {
            // TODO: 实现添加成绩的逻辑
            // 1. 参数验证
            // 2. 初始化学生成绩列表（如不存在）
            // 3. 添加成绩
            if (string.IsNullOrWhiteSpace(studentId) || score == null)
            {
                throw new ArgumentException("学生ID和成绩信息不能为空。");
            }

            if (!_scores.ContainsKey(studentId))
            {
                _scores[studentId] = new List<Score>();
            }
            _scores[studentId].Add(score);
        }

        public List<Score> GetStudentScores(string studentId)
        {
            // TODO: 获取指定学生的所有成绩
            if (string.IsNullOrWhiteSpace(studentId)) return new List<Score>();
            return _scores.TryGetValue(studentId, out var scores) ? new List<Score>(scores) : new List<Score>();
        }

        public double CalculateAverage(string studentId)
        {
            // TODO: 计算指定学生的平均分
            // 提示：使用foreach循环计算总分，然后除以科目数
            if (!_scores.ContainsKey(studentId) || !_scores[studentId].Any())
            {
                return 0.0;
            }

            double total = 0;
            foreach (var score in _scores[studentId])
            {
                total += score.Points;
            }
            return total / _scores[studentId].Count;
        }

        // TODO: 使用模式匹配实现成绩等级转换
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
            // TODO: 使用简单循环获取平均分最高的学生
            // 提示：可以先计算所有学生的平均分，然后排序取前count个
            var averages = new List<(string StudentId, double Average)>();
            foreach (var studentId in _scores.Keys)
            {
                averages.Add((studentId, CalculateAverage(studentId)));
            }
            
            // 手动实现排序
            for (int i = 0; i < averages.Count - 1; i++)
            {
                for (int j = i + 1; j < averages.Count; j++)
                {
                    if (averages[i].Average < averages[j].Average)
                    {
                        var temp = averages[i];
                        averages[i] = averages[j];
                        averages[j] = temp;
                    }
                }
            }

            var result = new List<(string, double)>();
            for(int i = 0; i < count && i < averages.Count; i++)
            {
                result.Add(averages[i]);
            }
            return result;
        }

        public Dictionary<string, List<Score>> GetAllScores()
        {
            return new Dictionary<string, List<Score>>(_scores);
        }
    }

    // 数据管理类
    public class DataManager
    {
        public void SaveStudentsToFile(List<Student> students, string filePath)
        {
            // TODO: 实现保存学生数据到文件
            // 提示：使用StreamWriter，格式为CSV
            try
            {
                using (var writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("StudentId,Name,Age"); // CSV header
                    foreach (var student in students)
                    {
                        writer.WriteLine($"{student.StudentId},{student.Name},{student.Age}");
                    }
                }
                Console.WriteLine($"学生数据已成功保存到 {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存文件时发生错误: {ex.Message}");
            }
        }

        public List<Student> LoadStudentsFromFile(string filePath)
        {
            List<Student> students = new List<Student>();
            
            // TODO: 实现从文件读取学生数据
            // 提示：使用StreamReader，解析CSV格式
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("文件不存在。");
                    return students;
                }

                using (var reader = new StreamReader(filePath))
                {
                    reader.ReadLine(); // Skip header
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var values = line.Split(',');
                        if (values.Length == 3)
                        {
                            var student = new Student(values[0], values[1], int.Parse(values[2]));
                            students.Add(student);
                        }
                    }
                }
                Console.WriteLine($"从 {filePath} 成功加载了 {students.Count} 名学生。");
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
            const string filePath = "students.csv";

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
                foreach (var student in studentsInAgeRange)
                {
                    Console.WriteLine(student);
                }

                // 4. 显示学生成绩统计
                Console.WriteLine("\n4. 学生成绩统计:");
                // TODO: 遍历所有学生，显示其成绩、平均分和等级
                var allStudents = studentManager.GetAll();
                allStudents.Sort(); // 按学号排序
                foreach (var student in allStudents)
                {
                    Console.WriteLine($"\n--- 学生: {student.Name} ({student.StudentId}) ---");
                    var scores = scoreManager.GetStudentScores(student.StudentId);
                    foreach (var score in scores)
                    {
                        Console.WriteLine($"  {score}");
                    }
                    var average = scoreManager.CalculateAverage(student.StudentId);
                    var grade = scoreManager.GetGrade(average);
                    Console.WriteLine($"  平均分: {average:F2}, 等级: {grade}");
                }

                // 5. 显示排名（简化版）
                Console.WriteLine("\n5. 平均分最高的学生:");
                // TODO: 调用GetTopStudents(1)方法显示第一名
                var topStudents = scoreManager.GetTopStudents(1);
                if (topStudents.Any())
                {
                    var topStudentInfo = studentManager.Find(s => s.StudentId == topStudents[0].StudentId).First();
                    Console.WriteLine($"第一名是 {topStudentInfo.Name}，平均分为: {topStudents[0].Average:F2}");
                }

                // 6. 文件操作
                Console.WriteLine("\n6. 数据持久化演示:");
                // TODO: 保存和读取学生文件
                dataManager.SaveStudentsToFile(studentManager.GetAll(), filePath);
                var loadedStudents = dataManager.LoadStudentsFromFile(filePath);
                Console.WriteLine("从文件加载的学生信息：");
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