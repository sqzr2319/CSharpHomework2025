using System;
using System.Collections.Generic;
using System.IO;

namespace StudentManagementSystem
{
    // 成绩等级枚举
    public enum Grade
    {
        F = 0,
        D = 60,
        C = 70,
        B = 80,
        A = 90
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
        public string StudentId { get; private set; }
        public string Name { get; private set; }
        public int Age { get; private set; }
        public Student(string studentId, string name, int age)
        {
            if (string.IsNullOrWhiteSpace(studentId)) throw new ArgumentException("学号不能为空");
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("姓名不能为空");
            if (age < 0) throw new ArgumentOutOfRangeException("年龄不能为负数");

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
            if (other == null) return 1; // 如果other为null，当前对象排在前面
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
        public string Subject { get; private set; }
        public double Points { get; private set; }

        public Score(string subject, double points)
        {
            if (string.IsNullOrWhiteSpace(subject)) throw new ArgumentException("科目不能为空");
            if (points < 0 || points > 100) throw new ArgumentOutOfRangeException("成绩必须在0到100之间");
            Subject = subject;
            Points = points;
        }



        public override string ToString()
        {
            return $"科目: {Subject}, 成绩: {Points:F2}"; // 格式化成绩到小数点后两位
        }
    }
    
    // 学生管理类
    public class StudentManager : IRepository<Student>
    {
        private List<Student> students = new List<Student>();


        public void Add(Student item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item), "学生信息不能为空");
            if (students.Contains(item)) throw new InvalidOperationException("学生已存在");
            students.Add(item);
            students.Sort(); // 保持学生列表按学号排序
        }
        public bool Remove(Student item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item), "学生信息不能为空");
            if (!students.Contains(item)) throw new InvalidOperationException("学生不存在");
            return students.Remove(item);
        }
        public List<Student> GetAll()
        {
            return new List<Student>(students);
        }
        public List<Student> Find(Func<Student, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate), "查询条件不能为空");
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
            if (minAge < 0 || maxAge < 0) throw new ArgumentOutOfRangeException("年龄不能为负数");
            if (minAge > maxAge) throw new ArgumentException("最小年龄不能大于最大年龄");
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
        private Dictionary<string, List<Score>> scores = new Dictionary<string, List<Score>>();
        public void AddScore(string studentId, Score score)
        {
            if (string.IsNullOrWhiteSpace(studentId)) throw new ArgumentException("学号不能为空");
            if (score == null) throw new ArgumentNullException(nameof(score), "成绩不能为空");
            if (!scores.ContainsKey(studentId))
            {
                scores[studentId] = new List<Score>();
            }
            scores[studentId].Add(score);
        }

        public List<Score> GetStudentScores(string studentId)
        {

            if (string.IsNullOrWhiteSpace(studentId)) throw new ArgumentException("学号不能为空");
            if (scores.ContainsKey(studentId))
            {
                return new List<Score>(scores[studentId]);
            }
            return new List<Score>();
        }

        public double CalculateAverage(string studentId)
        {
            if (string.IsNullOrWhiteSpace(studentId)) throw new ArgumentException("学号不能为空");
            if (!scores.ContainsKey(studentId) || scores[studentId].Count == 0) return 0.0;
            double total = 0.0;
            int count = 0;
            foreach (var score in scores[studentId])
            {
                total += score.Points;
                count++;
            }
            return total / count;
        }

        public Grade GetGrade(double score)
        {
            if (score < 0 || score > 100) throw new ArgumentOutOfRangeException("成绩必须在0到100之间");
            return score switch
            {
                >= 90 => Grade.A,
                >= 80 => Grade.B,
                >= 70 => Grade.C,
                >= 60 => Grade.D,
                _ => Grade.F,
            };
                
        }

        public List<(string StudentId, double Average)> GetTopStudents(int count)
        {
            // TODO: 使用简单循环获取平均分最高的学生
            // 提示：可以先计算所有学生的平均分，然后排序取前count个
            var averages = new List<(string StudentId, double Average)>();
            foreach (var kvp in scores)
            {
                double avg = CalculateAverage(kvp.Key);
                averages.Add((kvp.Key, avg));
            }
            averages.Sort((a, b) => b.Average.CompareTo(a.Average)); // 按平均分降序排序
            return averages.GetRange(0, Math.Min(count, averages.Count));
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
            {// 在这里实现文件写入逻辑
                using (var writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("StudentId,Name,Age");
                    foreach (var student in students)
                    {
                        writer.WriteLine($"{student.StudentId},{student.Name},{student.Age}");
                    }
                }
                Console.WriteLine("学生数据已保存到文件");
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
                using (var reader = new StreamReader(filePath))
                {
                    string headerLine = reader.ReadLine(); // 读取并忽略标题行
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
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
                Console.WriteLine("学生数据已从文件加载");
                return students;
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
                var ageFilteredStudents = studentManager.GetStudentsByAge(19, 20);
                foreach (var student in ageFilteredStudents)
                {
                    Console.WriteLine(student);
                }


                // 4. 显示学生成绩统计
                Console.WriteLine("\n4. 学生成绩统计:");
                // TODO: 遍历所有学生，显示其成绩、平均分和等级
                var allStudents = studentManager.GetAll();
                foreach (var student in allStudents)
                {
                    var scores = scoreManager.GetStudentScores(student.StudentId);
                    double average = scoreManager.CalculateAverage(student.StudentId);
                    Grade grade = scoreManager.GetGrade(average);
                    Console.WriteLine($"\n学生: {student}");
                    Console.WriteLine("成绩:");
                    foreach (var score in scores)
                    {
                        Console.WriteLine($"  {score}");
                    }
                    Console.WriteLine($"平均分: {average:F2}, 等级: {grade}");
                }

                // 5. 显示排名（简化版）
                Console.WriteLine("\n5. 平均分最高的学生:");
                // TODO: 调用GetTopStudents(1)方法显示第一名
                var topStudents = scoreManager.GetTopStudents(1);
                foreach (var (StudentId, Average) in topStudents)
                {
                    var student = allStudents.Find(s => s.StudentId == StudentId);
                    if (student != null)
                    {
                        Console.WriteLine($"第一名: {student}, 平均分: {Average:F2}");
                    }
                }

                // 6. 文件操作
                Console.WriteLine("\n6. 数据持久化演示:");
                // TODO: 保存和读取学生文件
                string filePath = "students.csv";
                dataManager.SaveStudentsToFile(allStudents, filePath);
                var loadedStudents = dataManager.LoadStudentsFromFile(filePath);
                Console.WriteLine("从文件加载的学生信息:");
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