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
        void Add(T item);
        // Remove(T item) 返回bool
        bool Remove(T item);
        // GetAll() 返回List<T>
        List<T> GetAll();
        // Find(Func<T, bool> predicate) 返回List<T>
        List<T> Find(Func<T, bool> predicate);
    }

    // 学生类
    public class Student : IComparable<Student>
    {
        // TODO: 定义字段 StudentId, Name, Age
        public string StudentId { get; private set; }
        public string Name { get; private set; }
        public int Age { get; private set; }

        public Student(string studentId, string name, int age)
        {
            // TODO: 实现构造方法，包含参数验证（空值检查）
            try
            {
                if (studentId is null || studentId == "")
                {
                    throw new ArgumentException("学号不能为空", nameof(studentId));
                }
                else
                {
                    StudentId = studentId;
                }
                if (name is null || name == "")
                {
                    throw new ArgumentException("姓名不能为空", nameof(name));
                }
                else
                {
                    Name = name;
                }
                if (age <= 0 || age > 150)
                {
                    throw new ArgumentOutOfRangeException(nameof(age), "年龄必须为有效值");
                }
                else
                {
                    Age = age;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Caught an exception: {ex.Message}");
            }
        }

        public override string ToString()
        {
            // TODO: 返回格式化的学生信息字符串
            return $"学号：{StudentId}，姓名：{Name}，年龄：{Age}";
        }

        // TODO: 实现IComparable接口，按学号排序
        // 提示：使用string.Compare方法
        public int CompareTo(Student? other)
        {
            return string.Compare(StudentId, other.StudentId);
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
            try
            {
                if (subject is null || subject == "")
                {
                    throw new ArgumentException("科目名称不能为空");
                }
                else
                {
                    Subject = subject;
                }
                if (points < 0 || points > 100)
                {
                    throw new ArgumentOutOfRangeException(nameof(points), "分数必须在0-100之间");
                }
                else
                {
                    Points = points;
                }   
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Caught an exception: {ex.Message}");
            }
        }

        public override string ToString()
        {
            // TODO: 返回格式化的成绩信息
            return $"科目：{Subject}，分数：{Points:F1}";
        }
    }

    // 学生管理类
    public class StudentManager : IRepository<Student>
    {
        // TODO: 定义私有字段存储学生列表
        // 提示：使用List<Student>存储
        private readonly List<Student> students = new List<Student>();

        public void Add(Student item)
        {
            // TODO: 实现添加学生的逻辑
            // 1. 参数验证
            // 2. 添加到列表
            try
            {
                if (item is null)
                {
                    throw new ArgumentNullException(nameof(item));
                }
                else
                {
                    students.Add(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Caught an exception: {ex.Message}");
            }
        }

        public bool Remove(Student item)
        {
            // TODO: 实现Remove方法
            try
            {
                if (item is null)
                {
                    throw new ArgumentNullException(nameof(item));
                }
                else
                {
                    students.Remove(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Caught an exception: {ex.Message}");
            }
        }

        public List<Student> GetAll()
        {
            // TODO: 返回学生列表的副本
            return new List<Student>(students);
        }

        public List<Student> Find(Func<Student, bool> predicate)
        {
            // TODO: 使用foreach循环查找符合条件的学生
            List<Student> result = new List<Student>();
            foreach (var student in students)
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
            List<Student> result = new List<Student>();
            foreach (var student in students)
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
        private readonly Dictionary<string, List<Score>> scores = new Dictionary<string, List<Score>>();

        public void AddScore(string studentId, Score score)
        {
            // TODO: 实现添加成绩的逻辑
            // 1. 参数验证
            // 2. 初始化学生成绩列表（如不存在）
            // 3. 添加成绩
            if (studentId is null || studentId == "")
                throw new ArgumentException("学号不能为空");
            if (score is null)
                throw new ArgumentNullException("成绩对象不能为空");

            if (!scores.ContainsKey(studentId))
            {
                scores[studentId] = new List<Score>();
            }
            scores[studentId].Add(score);
        }

        public List<Score> GetStudentScores(string studentId)
        {
            // TODO: 获取指定学生的所有成绩
            if (scores.TryGetValue(studentId, out var studentScores))
            {
                return new List<Score>(studentScores); // 返回副本
            }
            return new List<Score>();
        }

        public double CalculateAverage(string studentId)
        {
            // TODO: 计算指定学生的平均分
            // 提示：使用foreach循环计算总分，然后除以科目数
            var studentScores = GetStudentScores(studentId);
            if (studentScores.Count == 0)
                return 0;

            double total = 0;
            foreach (var score in studentScores)
            {
                total += score.Points;
            }
            return total / studentScores.Count;
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
            List<(string StudentId, double Average)> studentAverages = new List<(string, double)>();
            foreach (var studentId in scores.Keys)
            {
                studentAverages.Add((studentId, CalculateAverage(studentId)));
            }

            for (int i = 0; i < studentAverages.Count; i++)
            {
                for (int j = i + 1; j < studentAverages.Count; j++)
                {
                    if (studentAverages[j].Average > studentAverages[i].Average)
                    {
                        var temp = studentAverages[i];
                        studentAverages[i] = studentAverages[j];
                        studentAverages[j] = temp;
                    }
                }
            }

            List<(string, double)> result = new List<(string, double)>();
            for (int i = 0; i < Math.Min(count, studentAverages.Count); i++)
            {
                result.Add(studentAverages[i]);
            }
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
                    writer.WriteLine("StudentId,Name,Age");
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
                // 在这里实现文件读取逻辑
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split(',');
                        if (parts.Length == 3)
                        {
                            try
                            {
                                string studentId = parts[0];
                                string name = parts[1];
                                int age = int.Parse(parts[2]);

                                students.Add(new Student(studentId, name, age));
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"解析数据失败: {ex.Message}");
                            }
                        }
                    }
                }
                Console.WriteLine($"从 {filePath} 成功读取 {students.Count} 名学生数据");
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
                var ageRangeStudents = studentManager.GetStudentsByAge(19, 20);
                foreach (var student in ageRangeStudents)
                {
                    Console.WriteLine(student);
                }

                // 4. 显示学生成绩统计
                Console.WriteLine("\n4. 学生成绩统计:");
                // TODO: 遍历所有学生，显示其成绩、平均分和等级
                var allStudents = studentManager.GetAll();
                foreach (var student in allStudents)
                {
                    Console.WriteLine($"\n{student}");
                    var scores = scoreManager.GetStudentScores(student.StudentId);
                    foreach (var score in scores)
                    {
                        Console.WriteLine(score);
                    }
                    double average = scoreManager.CalculateAverage(student.StudentId);
                    Grade grade = scoreManager.GetGrade(average);
                    Console.WriteLine($"平均分: {average}, 等级: {grade}");
                }

                // 5. 显示排名（简化版）
                Console.WriteLine("\n5. 平均分最高的学生:");
                // TODO: 调用GetTopStudents(1)方法显示第一名
                var topStudents = scoreManager.GetTopStudents(1);
                foreach (var topStudent in topStudents)
                {
                    var student = studentManager.Find(s => s.StudentId == topStudent.StudentId).FirstOrDefault();
                    if (student != null)
                    {
                        Console.WriteLine($"第一名: {student.Name}, 平均分: {topStudent.Average}");
                    }
                }

                // 6. 文件操作
                Console.WriteLine("\n6. 数据持久化演示:");
                // TODO: 保存和读取学生文件
                string filePath = "students.csv";
                dataManager.SaveStudentsToFile(studentManager.GetAll(), filePath);
                var loadedStudents = dataManager.LoadStudentsFromFile(filePath);
                Console.WriteLine("读取的学生列表:");
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
