// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Linq;
using System.Globalization;

namespace StudentManagementSystem
{
    // 成绩等级枚举
    public enum Grade
    {
        // TODO: 定义成绩等级 F(0), D(60), C(70), B(80), A(90)
        A = 90,
        B = 80,
        C = 70,
        D = 60,
        F = 0
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
        bool Delete(T item);
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
            if (string.IsNullOrWhiteSpace(studentId))
                throw new ArgumentException("学号不能为空", nameof(studentId));
            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("姓名不能为空", nameof (name));
            if (age < 0)
                throw new ArgumentOutOfRangeException(nameof(age), "年龄不能为负数");

            StudentId = studentId;
            Name = name;
            Age = age;

        }

        public override string ToString()
        {
            // TODO: 返回格式化的学生信息字符串
            return $"学号：{StudentId}，姓名：{Name}, 年龄：{Age}";
        }

        // TODO: 实现IComparable接口，按学号排序
        // 提示：使用string.Compare方法
        public int CompareTo(Student? other)
        {
            if (other == null) return 1 ;
            if (string.IsNullOrEmpty(StudentId) && string.IsNullOrEmpty(other.StudentId)) return 0;
            if (string.IsNullOrEmpty(StudentId)) return  -1;
            if (string.IsNullOrEmpty(other.StudentId)) return 1;  
            return StudentId.CompareTo(other.StudentId) ;
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
            if(string.IsNullOrWhiteSpace(subject)) 
                throw new ArgumentException("科目不能为空",nameof(subject));
            if (points < 0 || points > 100)
                throw new ArgumentOutOfRangeException(nameof(points),"分数只能在0到100之间");
            
            Subject = subject;
            Points = points;
        }

        public override string ToString()
        {
            // TODO: 返回格式化的成绩信息
            return $"学科：{Subject}, 成绩：{Points}";
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
            if(item == null) 
                throw new ArgumentNullException(nameof(item));
            if (_students.Any(s => s.StudentId == item.StudentId))
                throw new InvalidOperationException("学号已存在");

            _students.Add(item);
        }

        public bool Delete(Student item)
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
            foreach(var item in _students)
            {
                if(predicate(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        // 查找年龄在指定范围内的学生
        public List<Student> GetStudentsByAge(int minAge, int maxAge)
        {
            // TODO: 使用foreach循环和if判断实现年龄范围查询
            var result = new List<Student>();
            foreach(var item in _students)
            {
                if(minAge <= item.Age && maxAge >= item.Age)
                {
                    result.Add(item);
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

            // 1. 参数验证
            if (string.IsNullOrWhiteSpace(studentId))
                throw new ArgumentException("学生ID不能为空", nameof(studentId));
            if(score == null) 
                throw new ArgumentNullException(nameof(score));
            if(score.Points < 0 ||  score.Points > 100)
                throw new ArgumentOutOfRangeException(nameof(score.Points), "科目分数需在0到100之间");

            // 2. 初始化学生成绩列表（如不存在）
            if(!_scores.TryGetValue(studentId, out var scores))
            {
                scores = new List<Score>();
                _scores[studentId] = scores;
            }

            // 3. 添加成绩
            scores.Add(score);
        }

        public List<Score> GetStudentScores(string studentId)
        {
            // TODO: 获取指定学生的所有成绩
            List<Score> result = new List<Score>();
            foreach(var item in _scores)
            {
                if(item.Key == studentId)
                {
                    result = item.Value;
                }
            }
            return result;
        }

        public double CalculateAverage(string studentId)
        {
            // TODO: 计算指定学生的平均分
            // 提示：使用foreach循环计算总分，然后除以科目数

            if (!_scores.TryGetValue(studentId, out var scores))
                throw new KeyNotFoundException($"学生ID{studentId}不存在");

            int count = scores.Count;
            double sum = 0;
            foreach (var item in scores)
            {
                sum += item.Points;
            }

            if (count == 0)
                return 0;

            return sum / count;

        }
        // TODO: 使用模式匹配实现成绩等级转换
        public Grade GetGrade(double score)
        {
            switch (score) 
            {
                case >= 0.0 and < 60.0:
                    return Grade.F;   
                case >= 60.0 and < 70.0:
                    return Grade.D;
                case >= 70.0 and < 80.0:
                    return Grade.C;  
                case >= 80.0 and < 90.0:
                    return Grade.B;                
                case >= 90.0 and <= 100.0:
                    return Grade.A;                  
                default:
                    throw new ArgumentOutOfRangeException(nameof(score));
            }
        }

        public List<(string StudentId, double Average)> GetTopStudents(int count)
        {
            // TODO: 使用简单循环获取平均分最高的学生
            // 提示：可以先计算所有学生的平均分，然后排序取前count个
            var result = new List<(string StudentId, double Average)>();

            //计算所有学生的平均分
            foreach (var item in _scores)
            {
                result.Add((
                    StudentId:item.Key,
                    Average:CalculateAverage(item.Key)
                    ));
            }

            //排序
            return result
                .OrderByDescending(x => x.Average)  // 按平均分降序排序
                .Take(count)    // 取前 count 个
                .ToList();      // 转换为列表

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
                // 在这里实现文件写入逻辑
                using (var writer = new StreamWriter(filePath, append: false, encoding: Encoding.UTF8))
                {
                    writer.WriteLine("学号，姓名，年龄");

                    foreach(var student in students)
                    {
                        writer.WriteLine($"{student.StudentId},{student.Name},{student.Age}");
                    }
                }
                Console.WriteLine($"成功保存{students.Count}条学生数据到{filePath}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"无文件写入权限: {ex.Message}");
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine($"目录不存在: {ex.Message}");
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
                using (var reader = new StreamReader(filePath, Encoding.UTF8))
                {
                    reader.ReadLine();      //去除表头

                    while(!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        var fields = line.Split(',');
                        if (fields.Length != 3) 
                        {
                            Console.WriteLine($"警告: 文件中的行格式不正确，已跳过。内容: \"{line}\"");
                            continue;
                        }

                        try
                        {
                            var student = new Student(
                                studentId: fields[0].Trim(),
                                name: fields[1].Trim(),
                                age: int.Parse(fields[2].Trim(), CultureInfo.InvariantCulture)
                                );

                            students.Add(student);
                        }
                        catch(ArgumentException ex)
                        {
                            Console.WriteLine($"忽略无效学生数据：{ex.Message}");
                        }
                    }
                }
                Console.WriteLine($"成功从{filePath}读取{students.Count}条学生数据");
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
                var result_1 = new List<Student>();
                result_1 = studentManager.GetStudentsByAge(19, 20);
                foreach(var student in result_1)
                {
                    Console.WriteLine(student.ToString());
                }

                // 4. 显示学生成绩统计
                Console.WriteLine("\n4. 学生成绩统计:");
                // TODO: 遍历所有学生，显示其成绩、平均分和等级
                var Allstudents = new List<Student>();
                Allstudents = studentManager.GetAll();
                foreach(var student in Allstudents)
                {
                    Console.WriteLine($"Student {student.Name}");

                    //显示成绩
                    var studentscore = new List<Score>();
                    studentscore = scoreManager.GetStudentScores(student.StudentId);
                    foreach(var score in studentscore)
                    {
                        Console.WriteLine(score.ToString());
                    }
                    Console.Write("平均分：");
                    Console.WriteLine(scoreManager.CalculateAverage(student.StudentId));
                    Console.Write("等级：");
                    Console.WriteLine(scoreManager.GetGrade(scoreManager.CalculateAverage(student.StudentId)));
                }

                // 5. 显示排名（简化版）
                Console.WriteLine("\n5. 平均分最高的学生:");
                // TODO: 调用GetTopStudents(1)方法显示第一名
                var result_2 = new List<(string StudentId, double Average)>();
                result_2 = scoreManager.GetTopStudents(1);
                var target_students = new List<Student>();
                foreach( var item in result_2)
                {
                    target_students = studentManager.Find(s => s.StudentId == item.StudentId);
                }
                foreach( var student in target_students)
                {
                    Console.WriteLine(student.ToString());
                }

                // 6. 文件操作
                Console.WriteLine("\n6. 数据持久化演示:");
                // TODO: 保存和读取学生文件
                dataManager.SaveStudentsToFile(Allstudents, "Students.txt");
                var students_loadfromfile = new List<Student>();
                students_loadfromfile = dataManager.LoadStudentsFromFile("Students.txt");

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