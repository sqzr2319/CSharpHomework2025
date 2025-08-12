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

    // 泛型仓储接口（主要给Student类使用）
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
        public string StudentId;
        public string Name;
        public int Age;
        
        public Student(string studentId, string name, int age)
        {
            // TODO: 实现构造方法，包含参数验证（空值检查）
            try
            {
                if (string.IsNullOrWhiteSpace(studentId))
                    throw new ArgumentException("StudentId 不能为空");
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("Name 不能为空");
                if (age < 0)
                    throw new ArgumentException("Age 不能为负数");

                this.StudentId = studentId;
                this.Name = name;
                this.Age = age;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"参数错误: {ex.Message}");
                throw;
            }
        }

        public override string ToString()
        {
            // TODO: 返回格式化的学生信息字符串
            return $"ID: {StudentId}, Name: {Name}, Age: {Age}";
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
        public string Subject;
        public double Points;
        
        public Score(string subject, double points)
        {
            // TODO: 实现构造方法，包含参数验证
            try
            {
                if (string.IsNullOrWhiteSpace(subject))
                    throw new ArgumentException("Subject 不能为空");
                if (points < 0)
                    throw new ArgumentException("Points 不能为负数");

                this.Subject = subject;
                this.Points = points;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"参数错误: {ex.Message}");
                throw;
            }
        }

        public override string ToString()
        {
            // TODO: 返回格式化的成绩信息
            return $"Subject: {Subject}, Points: {Points}";
        }
    }

    // 学生管理类
    public class StudentManager : IRepository<Student>
    {
        // TODO: 定义私有字段存储学生列表
        // 提示：使用List<Student>存储
        private List<Student> studentsList = new List<Student>();

        public void Add(Student item)
        {
            // TODO: 实现添加学生的逻辑
            // 1. 参数验证
            // 2. 添加到列表
            try
            {
                if (item == null)
                    throw new ArgumentException("Student 对象不能为空");
                if (string.IsNullOrWhiteSpace(item.StudentId))
                    throw new ArgumentException("StudentId 不能为空");
                if (string.IsNullOrWhiteSpace(item.Name))
                    throw new ArgumentException("Name 不能为空");
                if (item.Age < 0)
                    throw new ArgumentException("Age 不能为负数");

                studentsList.Add(item);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"参数错误: {ex.Message}");
                throw;
            }
        }

        public bool Remove(Student item)
        {
            // TODO: 实现Remove方法
            return studentsList.Remove(item);
        }

        public List<Student> GetAll()
        {
            // TODO: 返回学生列表的副本
            return studentsList;
        }

        public List<Student> Find(Func<Student, bool> predicate)  // 查找所有符合条件（待定义）的学生
        {
            // TODO: 使用foreach循环查找符合条件的学生
            List<Student> result = new List<Student>();

            foreach (var student in studentsList) {
                // 调用 predicate 委托（调用时才会执行具体条件，比如 Age >= 18）
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

            foreach (Student student in studentsList) {
                if (student.Age >= minAge && student.Age <= maxAge) {
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
        private Dictionary<string, List<Score>> scoresDict = new Dictionary<string, List<Score>>();  // 一个同学有多个成绩

        public void AddScore(string studentId, Score score)
        {
            // TODO: 实现添加成绩的逻辑
            // 1. 参数验证
            // 2. 初始化学生成绩列表（如不存在）
            // 3. 添加成绩
            try
            {
                if (string.IsNullOrWhiteSpace(studentId))
                    throw new ArgumentException("StudentId 不能为空");
                if (score == null)
                    throw new ArgumentException("Score 对象不能为空");
                if (string.IsNullOrWhiteSpace(score.Subject))
                    throw new ArgumentException("Subject 不能为空");
                if (score.Points < 0)
                    throw new ArgumentException("Points 不能为负数");

                if (!scoresDict.ContainsKey(studentId))
                {
                    scoresDict[studentId] = new List<Score>();
                }

                scoresDict[studentId].Add(score);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"参数错误: {ex.Message}");
                throw;
            }
        }

        public List<Score> GetStudentScores(string studentId)
        {
            // TODO: 获取指定学生的所有成绩
            if (scoresDict.ContainsKey(studentId))
            {
                return new List<Score>(scoresDict[studentId]); // 返回副本，防止外部修改
            }
            return new List<Score>(); // 如果没找到，返回空列表
        }

        public double CalculateAverage(string studentId)
        {
            // TODO: 计算指定学生的平均分
            // 提示：使用foreach循环计算总分，然后除以科目数
            if (!scoresDict.ContainsKey(studentId) || scoresDict[studentId].Count == 0) return 0;

            double sum = 0;
            foreach (var score in scoresDict[studentId])
            {
                sum += score.Points;
            }

            return sum / scoresDict[studentId].Count;
        }

        // TODO: 使用模式匹配实现成绩等级转换
        public Grade GetGrade(double score)
        {
            switch ((int)score) // 转成 int 避免浮点判断误差
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
            var averages = new List<(string StudentId, double Average)>();

            foreach (var kvp in scoresDict)
            {
                double avg = CalculateAverage(kvp.Key);
                averages.Add((kvp.Key, avg));
            }

            averages.Sort((a, b) => b.Average.CompareTo(a.Average)); // 降序
            return averages.Take(count).ToList();
        }

        public Dictionary<string, List<Score>> GetAllScores()
        {
            return new Dictionary<string, List<Score>>(scoresDict);
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
                    // 写表头
                    writer.WriteLine("StudentId,Name,Age");

                    // 写每个学生数据
                    foreach (var student in students)
                    {
                        writer.WriteLine($"{student.StudentId},{student.Name},{student.Age}");
                    }
                }
                Console.WriteLine("学生数据已保存到文件。");
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
                    bool isFirstLine = true;

                    while ((line = reader.ReadLine()) != null)
                    {
                        // 跳过表头
                        if (isFirstLine)
                        {
                            isFirstLine = false;
                            continue;
                        }

                        var parts = line.Split(',');
                        if (parts.Length == 3)
                        {
                            string studentId = parts[0];
                            string name = parts[1];
                            if (int.TryParse(parts[2], out int age))
                            {
                                students.Add(new Student(studentId, name, age));
                            }
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
                var ageFiltered = studentManager.GetStudentsByAge(19, 20);
                foreach (var student in ageFiltered)
                {
                    Console.WriteLine(student);
                }

                // 4. 显示学生成绩统计
                Console.WriteLine("\n4. 学生成绩统计:");
                // TODO: 遍历所有学生，显示其成绩、平均分和等级
                foreach (var student in studentManager.GetAll()) {
                    var scoreList = scoreManager.GetStudentScores(student.StudentId);
                    var average = scoreManager.CalculateAverage(student.StudentId);
                    var grade = scoreManager.GetGrade(average);

                    Console.WriteLine($"{student}");  // 隐式调用 student 对象的 ToString() 方法
                    Console.WriteLine($"成绩: {string.Join(", ", scoreList.Select(s => s.ToString()))}");
                    Console.WriteLine($"平均分: {average:F1}, 等级: {grade}");
                    Console.WriteLine();
                }

                // 5. 显示排名（简化版）
                Console.WriteLine("\n5. 平均分最高的学生:");
                // TODO: 调用GetTopStudents(1)方法显示第一名
                var topStudent = scoreManager.GetTopStudents(1).FirstOrDefault();
                if (topStudent != default)
                {
                    var student = studentManager.Find(s => s.StudentId == topStudent.StudentId).First();
                    Console.WriteLine($"{student} - 平均分: {topStudent.Average:F1}");
                }

                // 6. 文件操作
                Console.WriteLine("\n6. 数据持久化演示:");
                // TODO: 保存和读取学生文件
                string filePath = "students.csv";
                dataManager.SaveStudentsToFile(studentManager.GetAll(), filePath);
                Console.WriteLine($"学生数据已保存到 {filePath}");

                var loadedStudents = dataManager.LoadStudentsFromFile(filePath);
                Console.WriteLine($"从文件加载的学生数据:");
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