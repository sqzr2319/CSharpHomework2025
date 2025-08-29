// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.IO;

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

    public interface IComparable<T>
    {
        int CompareTo(T? other);
    }

    // 学生类
    public class Student : IComparable<Student>
    {
        // 改为公有只读属性
        public string StudentId { get; private set; }
        public string Name { get; private set; }
        public int Age { get; private set; }

        public Student(string studentId, string name, int age)
        {
            if (string.IsNullOrWhiteSpace(studentId))
                throw new ArgumentException("StudentId不能为空");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name不能为空");
            if (age <= 0)
                throw new ArgumentException("Age必须大于0");

            this.StudentId = studentId;
            this.Name = name;
            this.Age = age;
        }

        public override string ToString()
        {
            return $"学号: {StudentId}, 姓名: {Name}, 年龄: {Age}";
        }

        // TODO: 实现IComparable接口，按学号排序
        // 提示：使用string.Compare方法
        public int CompareTo(Student? other)
        {
            if (other == null) return 0;
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
        // 改为公有只读属性
        public string Subject { get; private set; }
        public double Points { get; private set; }

        public Score(string subject, double points)
        {
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Subject不能为空");
            if (points < 0 || points > 100) 
                throw new ArgumentException("Points必须在0到100之间");

            this.Subject = subject;
            this.Points = points;
        }

        public override string ToString()
        {
            return $"科目: {Subject}, 分数: {Points}";
        }
    }

    // 学生管理类
    public class StudentManager : IRepository<Student>
    {
        // TODO: 定义私有字段存储学生列表
        // 提示：使用List<Student>存储
        private List<Student> students = new List<Student>();

        public void Add(Student item)
        {
            // TODO: 实现添加学生的逻辑
            // 1. 参数验证
            // 2. 添加到列表
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Student不能为空");
            students.Add(item);
        }

        public bool Remove(Student item)
        {
            // TODO: 实现Remove方法
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Student不能为空");
            return students.Remove(item);
        }

        public List<Student> GetAll()
        {
            // TODO: 返回学生列表的副本
            return new List<Student>(students);
        }

        public List<Student> Find(Func<Student, bool> predicate)
        {
            // TODO: 使用foreach循环查找符合条件的学生
            var result = new List<Student>();
            foreach (var student in students)
            {
                if (predicate(student))
                    result.Add(student);
            }
            return result;
        }

        // 查找年龄在指定范围内的学生
        public List<Student> GetStudentsByAge(int minAge, int maxAge)
        {
            // TODO: 使用foreach循环和if判断实现年龄范围查询
            var result = new List<Student>();
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
        // TODO: 定义私有字段存储成绩字典
        // 提示：使用Dictionary<string, List<Score>>存储
        private Dictionary<string, List<Score>> scores = new Dictionary<string, List<Score>>();

        public void AddScore(string studentId, Score score)
        {
            // TODO: 实现添加成绩的逻辑
            // 1. 参数验证
            // 2. 初始化学生成绩列表（如不存在）
            // 3. 添加成绩
            if (string.IsNullOrWhiteSpace(studentId))
                throw new ArgumentException("StudentId不能为空");
            if (score == null)  
                throw new ArgumentNullException(nameof(score), "Score不能为空");
            if (!scores.ContainsKey(studentId))
                scores[studentId] = new List<Score>();
            scores[studentId].Add(score);
        }

        public List<Score> GetStudentScores(string studentId)
        {
            // TODO: 获取指定学生的所有成绩
            if (string.IsNullOrWhiteSpace(studentId))
                throw new ArgumentException("StudentId不能为空");
            if (scores.TryGetValue(studentId, out var studentScores))
                return studentScores;
            return new List<Score>();
        }

        public double CalculateAverage(string studentId)
        {
            // TODO: 计算指定学生的平均分
            // 提示：使用foreach循环计算总分，然后除以科目数
            if (string.IsNullOrWhiteSpace(studentId))
                throw new ArgumentException("StudentId不能为空");
            if (scores.TryGetValue(studentId, out var studentScores) && studentScores.Count > 0)
                {
                double total = 0;
                foreach (var score in studentScores)
                    total += score.Points;
                return total / studentScores.Count;
            }
            return 0.0;
        }

        // TODO: 使用模式匹配实现成绩等级转换
        public Grade GetGrade(double score)
        {
            switch(score)
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
            // TODO: 使用简单循环获取平均分最高的学生
            // 提示：可以先计算所有学生的平均分，然后排序取前count个
            var topStudents = new List<(string StudentId, double Average)>();
            foreach (var studentId in scores.Keys)
            {
                var average = CalculateAverage(studentId);
                topStudents.Add((studentId, average));
            }
            for (int i = 0; i < topStudents.Count - 1; i++)
            {
                for (int j = i + 1; j < topStudents.Count; j++)
                {
                    if (topStudents[j].Average > topStudents[i].Average)
                    {
                        var temp = topStudents[i];
                        topStudents[i] = topStudents[j];
                        topStudents[j] = temp;
                    }
                }
            }
            var result = new List<(string StudentId, double Average)>();
            for (int i = 0; i < count && i < topStudents.Count; i++)
                result.Add(topStudents[i]);
            return result;
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
            // TODO: 实现保存学生数据到文件
            // 提示：使用StreamWriter，格式为CSV
            try
            {
                // 在这里实现文件写入逻辑
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
            List<Student> students = new List<Student>();
            
            // TODO: 实现从文件读取学生数据
            // 提示：使用StreamReader，解析CSV格式
            try
            {
                // 在这里实现文件读取逻辑
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var parts = line.Split(',');
                        if (parts.Length == 3 &&
                            !string.IsNullOrWhiteSpace(parts[0]) &&
                            !string.IsNullOrWhiteSpace(parts[1]) &&
                            int.TryParse(parts[2], out int age) && age > 0)
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
                // TODO: 调用GetStudentsByAge方法并显示结果
                var studentsInRange = studentManager.GetStudentsByAge(19, 20);
                foreach (var student in studentsInRange)
                {
                    Console.WriteLine($"- {student.Name} (ID: {student.StudentId}, Age: {student.Age})");
                }

                // 4. 显示学生成绩统计
                Console.WriteLine("\n4. 学生成绩统计:");
                // TODO: 遍历所有学生，显示其成绩、平均分和等级
                foreach (var student in studentManager.GetAll())
                {
                    var scores = scoreManager.GetStudentScores(student.StudentId);
                    var average = scoreManager.CalculateAverage(student.StudentId);
                    var grade = scoreManager.GetGrade(average);
                    Console.WriteLine($"学生: {student.Name} (ID: {student.StudentId})");
                    foreach (var score in scores)
                    {
                        Console.WriteLine($"  - {score.Subject}: {score.Points}");
                    }
                    Console.WriteLine($"  平均分: {average:F2}, 等级: {grade}");
                }

                // 5. 显示排名（简化版）
                Console.WriteLine("\n5. 平均分最高的学生:");
                // TODO: 调用GetTopStudents(1)方法显示第一名
                var topStudent = scoreManager.GetTopStudents(1);
                foreach (var (StudentId, Average) in topStudent)
                {
                    var student = studentManager.Find(s => s.StudentId == StudentId)[0];
                    Console.WriteLine($"第一名: {student.Name} (ID: {student.StudentId}), 平均分: {Average:F2}");
                }

                // 6. 文件操作
                Console.WriteLine("\n6. 数据持久化演示:");
                // TODO: 保存和读取学生文件
                dataManager.SaveStudentsToFile(studentManager.GetAll(), "students.csv");
                var studentsFromFile = dataManager.LoadStudentsFromFile("students.csv");
                foreach (var student in studentsFromFile)
                {
                    Console.WriteLine($"- {student.Name} (ID: {student.StudentId}, Age: {student.Age})");
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