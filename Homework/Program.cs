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

    // 学生类
    public class Student : IComparable<Student>
    {
        // TODO: 定义字段 StudentId, Name, Age
        public string StudentId { get; }
        public string Name { get; }
        public int Age { get; }

        public Student(string studentId, string name, int age)
        {
            // TODO: 实现构造方法，包含参数验证（空值检查）
            if (string.IsNullOrWhiteSpace(studentId))
                throw new ArgumentException("学号不能为空", nameof(studentId));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("姓名不能为空", nameof(name));
            if (int.IsNegative(age))
                throw new ArgumentOutOfRangeException(nameof(age), "年龄不能为负数");
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
            if (other == null)
                return 1; // 当前对象大于null
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
        // TODO: 定义字段 Subject, Points
        public string Subject { get; }
        public double Points { get; }

        public Score(string subject, double points)
        {
            // TODO: 实现构造方法，包含参数验证
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentNullException(nameof(subject), "科目不能为空");
            if (points < 0 || points > 100)
                throw new ArgumentOutOfRangeException(nameof(points), "分数必须在0到100之间");
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
        private List<Student> studentlist = new List<Student>();

        public void Add(Student item)
        {
            // TODO: 实现添加学生的逻辑
            // 1. 参数验证
            if (item == null)
                throw new ArgumentNullException(nameof(item), "学生信息不能为空");
            if (studentlist.Contains(item))
                throw new ArgumentException("学生已存在", nameof(item));
            // 2. 添加到列表
            studentlist.Add(item);
        }
        public bool Remove(Student item)
        {
            // TODO: 实现Remove方法
            if (item == null)
                throw new ArgumentNullException(nameof(item), "学生信息不能为空");
            return studentlist.Remove(item);
        }

        public List<Student> GetAll()
        {
            // TODO: 返回学生列表的副本
            return new List<Student>(studentlist);
        }

        public List<Student> Find(Func<Student, bool> predicate)
        {
            // TODO: 使用foreach循环查找符合条件的学生
            List<Student> results = new List<Student>();
            foreach (var student in studentlist)
            {
                if (predicate(student))
                {
                    results.Add(student);
                }
            }
            return results;
        }

        // 查找年龄在指定范围内的学生
        public List<Student> GetStudentsByAge(int minAge, int maxAge)
        {
            // TODO: 使用foreach循环和if判断实现年龄范围查询
            List<Student> results = new List<Student>();
            foreach (var student in studentlist)
            {
                if (student.Age >= minAge && student.Age <= maxAge)
                {
                    results.Add(student);
                }
            }
            return results;
        }
    }

    // 成绩管理类
    public class ScoreManager
    {
        // TODO: 定义私有字段存储成绩字典
        // 提示：使用Dictionary<string, List<Score>>存储
        private Dictionary<string, List<Score>> scorelist = new Dictionary<string, List<Score>>();

        public void AddScore(string studentId, Score score)
        {
            // TODO: 实现添加成绩的逻辑
            // 1. 参数验证
            if (string.IsNullOrWhiteSpace(studentId))
                throw new ArgumentException("学生ID不能为空", nameof(studentId));
            if (score == null)
                throw new ArgumentNullException(nameof(score), "成绩信息不能为空");
            // 2. 初始化学生成绩列表（如不存在）
            if (!scorelist.ContainsKey(studentId))
            {
                scorelist[studentId] = new List<Score>();
            }
            // 3. 添加成绩
            scorelist[studentId].Add(score);
        }
        public List<Score> GetStudentScores(string studentId)
        {
            // TODO: 获取指定学生的所有成绩
            if (string.IsNullOrWhiteSpace(studentId))
                throw new ArgumentException("学生ID不能为空", nameof(studentId));
            if (!scorelist.ContainsKey(studentId))
                return new List<Score>();
            return new List<Score>(scorelist[studentId]);
        }
        public double CalculateAverage(string studentId)
        {
            // TODO: 计算指定学生的平均分
            // 提示：使用foreach循环计算总分，然后除以科目数
            if (string.IsNullOrWhiteSpace(studentId))
                throw new ArgumentException("学生ID不能为空", nameof(studentId));
            if (!scorelist.ContainsKey(studentId))
                return 0;
            double total = 0;
            int count = 0;
            foreach (var score in scorelist[studentId])
            {
                total += score.Points;
                count++;
            }
            return count > 0 ? total / count : 0;
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
            List<(string StudentId, double Average)> averages = new List<(string, double)>();
            foreach (var studentId in scorelist.Keys)
            {
                double average = CalculateAverage(studentId);
                averages.Add((studentId, average));
            }
            averages.Sort((x, y) => y.Average.CompareTo(x.Average)); // 降序排序
            return averages.GetRange(0, Math.Min(count, averages.Count)); // 取两者中的较小值，确保不会尝试提取超出列表长度的元素（避免 ArgumentOutOfRangeException）
        }

        public Dictionary<string, List<Score>> GetAllScores()
        {
            return new Dictionary<string, List<Score>>(scorelist);
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
                    bool isFirstLine = true; // 用于跳过表头
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (isFirstLine)
                        {
                            isFirstLine = false; // 跳过表头
                            continue;
                        }
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        var parts = line.Split(',');
                        if (parts.Length == 3 &&
                            !string.IsNullOrWhiteSpace(parts[0]) &&
                            !string.IsNullOrWhiteSpace(parts[1]) &&
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
                    double average = scoreManager.CalculateAverage(student.StudentId);
                    var grade = scoreManager.GetGrade(average);
                    Console.WriteLine($"- {student.Name} (ID: {student.StudentId}, 平均分: {average}, 等级: {grade})");
                }

                // 5. 显示排名（简化版）
                Console.WriteLine("\n5. 平均分最高的学生:");
                // TODO: 调用GetTopStudents(1)方法显示第一名
                var topStudents = scoreManager.GetTopStudents(1);
                if (topStudents.Count > 0)
                {
                    var topStudent = topStudents[0];
                    Console.WriteLine($"- 学号: {topStudent.StudentId}, 平均分: {topStudent.Average}");
                }
                else
                {
                    Console.WriteLine("没有学生数据");
                }

                // 6. 文件操作
                Console.WriteLine("\n6. 数据持久化演示:");
                // TODO: 保存和读取学生文件
                string filePath = "students.csv";
                dataManager.SaveStudentsToFile(studentManager.GetAll(), filePath);
                var loadedStudents = dataManager.LoadStudentsFromFile(filePath);
                Console.WriteLine("从文件加载的学生信息:");
                if (loadedStudents.Count == 0)
                {
                    Console.WriteLine("没有加载到任何学生信息");
                    return;
                }
                Console.WriteLine("- 姓名 (ID, Age):");
                foreach (var student in loadedStudents)
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