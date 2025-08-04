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
        A = 90,
    }

    // 泛型仓储接口
    public interface IRepository<T>
    {
        // TODO: 定义接口方法
        public void Add(T item);
        public bool Remove(T item);
        public List<T> GetAll();
        List<T> Find(Func<T, bool> predicate);
        
    }

    // 学生类
    public class Student : IComparable<Student>
    {
        // TODO: 定义字段 StudentId, Name, Age
        public readonly string StudentId;
        public readonly string Name;
        public readonly int Age;

        public Student(string studentId, string name, int age)
        {
            if ((studentId is null) || (name is null))
            {
                throw new ArgumentException("Null Parameter");
            }
            StudentId = studentId;
            Name = name;
            Age = age;
        }

        public override string ToString()
        {
            return $$"""Student { StudentId : {{StudentId}}, Name : {{Name}}, Age : {{Age}} }""";
        }

        public int CompareTo(Student? other)
        {
            return String.Compare(StudentId, other?.StudentId);
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
        public readonly string Subject;
        public readonly double Points;
        
        public Score(string subject, double points)
        {
            if (subject is null)
            {
                throw new ArgumentException("Null Parameter");
            }
            Subject = subject;
            Points = points;
        }

        public override string ToString()
        {
            return $$"""Score { Subject : {{Subject}}, Points : {{Points}} }""";
        }
    }

    // 学生管理类
    public class StudentManager : IRepository<Student>
    {
        // TODO: 定义私有字段存储学生列表
        // 提示：使用List<Student>存储
        private List<Student> Students = new List<Student>();

        public void Add(Student item)
        {
            if (item is not null)
            {
                Students.Add(item);
            }
        }

        public bool Remove(Student item)
        {
            return Students.Remove(item);
        }

        public List<Student> GetAll()
        {
            return new List<Student>(Students);
        }

        public List<Student> Find(Func<Student, bool> predicate)
        {
            var res = new List<Student>();
            foreach (var item in Students)
            {
                if (predicate(item))
                {
                    res.Add(item);
                }
            }
            return res;
        }

        // 查找年龄在指定范围内的学生
        public List<Student> GetStudentsByAge(int minAge, int maxAge)
        {
            return this.Find(student => student.Age <= maxAge && student.Age >= minAge);
        }
    }

    // 成绩管理类
    public class ScoreManager
    {
        // TODO: 定义私有字段存储成绩字典
        // 提示：使用Dictionary<string, List<Score>>存储
        private Dictionary<string, List<Score>> ScoreDict = new Dictionary<string, List<Score>>();

        public void AddScore(string studentId, Score score)
        {
            if (studentId is not null)
            {
                if (! ScoreDict.ContainsKey(studentId))
                {
                    ScoreDict.Add(studentId, new List<Score>());
                }
                ScoreDict[studentId].Add(score);
            }
        }

        public List<Score> GetStudentScores(string studentId)
        {
            return ScoreDict[studentId];
        }

        public double CalculateAverage(string studentId)
        {
            double res = 0;
            var l = ScoreDict[studentId];
            foreach (var score in l)
            {
                res += score.Points;
            }
            return res / l.Count();
        }

        public Grade GetGrade(double score)
        {
            return score switch
            {
                < 60 => Grade.F,
                >= 60 and < 70 => Grade.D,
                >= 70 and < 80 => Grade.C,
                >= 80 and < 90 => Grade.B,
                >= 90 => Grade.A,
                _ => throw new ArgumentOutOfRangeException("Invalid score value")
            };
        }

        public List<(string StudentId, double Average)> GetTopStudents(int count)
        {
            var res = new List<(string StudentId, double Average)>();
            foreach (var item in ScoreDict)
            {
                res.Add((item.Key, CalculateAverage(item.Key)));
            }
            res.Sort((x, y) => -x.Item2.CompareTo(y.Item2));
            return res.GetRange(0, count);
        }

        public Dictionary<string, List<Score>> GetAllScores()
        {
            return new Dictionary<string, List<Score>>(ScoreDict);
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
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    // 跳过标题行
                    reader.ReadLine();
                    
                    while ((line = reader.ReadLine()) != null)
                    {
                        var parts = line.Split(',');
                        if (parts.Length == 3)
                        {
                            string studentId = parts[0].Trim();
                            string name = parts[1].Trim();
                            int age = int.Parse(parts[2].Trim());
                            students.Add(new Student(studentId, name, age));
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
                var students_temp = studentManager.GetStudentsByAge(19, 20);
                foreach (var student in students_temp)
                {
                    Console.WriteLine(student);
                }

                // 4. 显示学生成绩统计
                Console.WriteLine("\n4. 学生成绩统计:");
                foreach (var student in studentManager.GetAll())
                {
                    var scores = scoreManager.GetStudentScores(student.StudentId);
                    Console.WriteLine($"{student.Name} 的成绩:");
                    foreach (var score in scores)
                    {
                        Console.WriteLine($"科目: {score.Subject}, 分数: {score.Points}, 等级: {scoreManager.GetGrade(score.Points)}");
                    }
                    double average = scoreManager.CalculateAverage(student.StudentId);
                    Console.WriteLine($"平均分: {average}, 等级: {scoreManager.GetGrade(average)}\n");
                }
                

                // 5. 显示排名（简化版）
                Console.WriteLine("\n5. 平均分最高的学生:");
                Console.WriteLine(scoreManager.GetTopStudents(1)[0].StudentId);
                

                // 6. 文件操作
                Console.WriteLine("\n6. 数据持久化演示:");
                string filePath = "students.csv";
                // 保存学生数据到文件
                dataManager.SaveStudentsToFile(studentManager.GetAll(), filePath);
                Console.WriteLine($"学生数据已保存到 {filePath}");

                // 从文件加载学生数据
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
