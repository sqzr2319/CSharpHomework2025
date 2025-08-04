// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.IO;

namespace StudentManagementSystem
{
    /// <summary>
    /// 成绩等级枚举
    /// </summary>
    public enum Grade
    {
        // TODO: 定义成绩等级 F(0), D(60), C(70), B(80), A(90)
        // done
        F = 0,
        D = 60,
        C = 70,
        B = 80,
        A = 90

    }

    /// <summary>
    /// 泛型仓储接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T>
    {
        // TODO: 定义接口方法
        // done
        // Add(T item)
        void Add(T item);
        // Remove(T item) 返回bool
        bool Remove(T item);
        // GetAll() 返回List<T>
        List<T> GetAll();
        // Find(Func<T, bool> predicate) 返回List<T>
        List<T> Find(Func<T, bool> predicate);

    }

    /// <summary>
    /// 学生类
    /// </summary>
    public class Student : IComparable<Student>
    {
        // TODO: 定义字段 studentId, name, age
        // done
        public string studentId;
        public string name;
        public int age;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="name"></param>
        /// <param name="age"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Student(string studentId, string name, int age)
        {
            // TODO: 实现构造方法，包含参数验证（空值检查）
            // done

            // 参数验证（字符串空值检查和年龄范围检查）
            if (string.IsNullOrWhiteSpace(studentId))
                throw new ArgumentException("学号不能为空", nameof(studentId));   
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("姓名不能为空", nameof(name));
            if (age < 0)    
                throw new ArgumentOutOfRangeException(nameof(age), "年龄不能为负数");
            
            // 初始化字段
            this.studentId = studentId;
            this.name = name;
            this.age = age;

        }

        /// <summary>
        /// 返回格式化的学生信息字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // TODO: 返回格式化的学生信息字符串
            // done
            return $"学号：{studentId}, 姓名：{name}, 年龄：{age}";

        }

        // TODO: 实现IComparable接口，按学号排序
        // 提示：使用string.Compare方法
        // done
        /// <summary>
        /// 实现学生比较方法
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Student? other)
        {
            if (other is null) 
                return 1; // 如果other为null，当前对象排在前面

            // 返回学号的比较结果
            return string.Compare(this.studentId, other.studentId, StringComparison.Ordinal);   

        }

        public override bool Equals(object? obj)
        {
            return obj is Student student && studentId == student.studentId;
        }

        public override int GetHashCode()
        {
            return studentId?.GetHashCode() ?? 0;
        }
    }

    /// <summary>
    /// 成绩类
    /// </summary>
    public class Score
    {
        // TODO: 定义字段 Subject, Points
        // done
        public string subject;
        public double points;


        public Score(string subject, double points)
        {
            // TODO: 实现构造方法，包含参数验证
            // done

            // 空值检查和分数范围检查
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("科目不能为空", nameof(subject));
            if (points < 0 || points > 100)
                throw new ArgumentOutOfRangeException(nameof(points), "分数必须在0到100之间");

            // 初始化字段
            this.subject = subject;
            this.points = points;

        }

        /// <summary>
        /// 返回格式化的成绩信息
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // TODO: 返回格式化的成绩信息
            // done 分数保留一位小数
            return $"科目：{subject}, 成绩：{points:F1}";
        }
    }

    /// <summary>
    /// 学生管理类
    /// </summary>
    public class StudentManager : IRepository<Student>
    {
        // TODO: 定义私有字段存储学生列表
        // 提示：使用List<Student>存储
        // done
        private List<Student> students = new List<Student>();

        /// <summary>
        /// 添加学生
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Add(Student item)
        {
            // TODO: 实现添加学生的逻辑
            // 1. 参数验证
            // 2. 添加到列表
            // done
            if (item is null)
                throw new ArgumentNullException(nameof(item), "学生信息不能为空");
            
            students.Add(item);

        }

        /// <summary>
        /// 删除学生
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(Student item)
        {
            // TODO: 实现Remove方法
            // done
            return students.Remove(item);
            
        }

        /// <summary>
        /// 返回所有学生列表的副本
        /// </summary>
        /// <returns></returns>
        public List<Student> GetAll()
        {
            // TODO: 返回学生列表的副本
            // done
            return new List<Student>(students);

        }

        /// <summary>
        /// 查找符合条件的学生
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<Student> Find(Func<Student, bool> predicate)
        {
            // TODO: 使用foreach循环查找符合条件的学生
            // done
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

        /// <summary>
        /// 查找年龄在指定范围内的学生
        /// </summary>
        /// <param name="minAge"></param>
        /// <param name="maxAge"></param>
        /// <returns></returns>
        public List<Student> GetStudentsByAge(int minAge, int maxAge)
        {
            // TODO: 使用foreach循环和if判断实现年龄范围查询
            // done
            List<Student> result = new List<Student>();

            foreach (var student in students)
            {
                if (student.age >= minAge && student.age <= maxAge)
                {
                    result.Add(student);
                }
            }

            return result;
        }
    }

    /// <summary>
    /// 成绩管理类
    /// </summary>
    public class ScoreManager
    {
        // TODO: 定义私有字段存储成绩字典
        // 提示：使用Dictionary<string, List<Score>>存储
        // done

        Dictionary<string, List<Score>> scores = new Dictionary<string, List<Score>>();

        /// <summary>
        /// 添加成绩
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="score"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddScore(string studentId, Score score)
        {
            // TODO: 实现添加成绩的逻辑
            // 1. 参数验证
            // 2. 初始化学生成绩列表（如不存在）
            // 3. 添加成绩
            // done

            // 空值检查
            if (string.IsNullOrWhiteSpace(studentId))
                throw new ArgumentException("学号不能为空", nameof(studentId));
            if (score is null)
                throw new ArgumentNullException(nameof(score), "成绩信息不能为空");

            // 初始化学生成绩列表（如不存在）
            if (!scores.ContainsKey(studentId))
            {
                scores[studentId] = new List<Score>();
            }

            // 添加成绩
            scores[studentId].Add(score);

        }

        /// <summary>
        /// 获取指定学生的所有成绩
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public List<Score> GetStudentScores(string studentId)
        {
            // TODO: 获取指定学生的所有成绩
            // done

            if (scores.TryGetValue(studentId, out var studentScores))
            {
                return new List<Score>(studentScores);
            }
            else
            {
                // 如果学生没有成绩记录，返回空列表
                Console.WriteLine($"学号为 {studentId} 的学生不存在或没有成绩记录!");
                return new List<Score>();
            }
        }

        /// <summary>
        /// 计算指定学生的平均分
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public double CalculateAverage(string studentId)
        {
            // TODO: 计算指定学生的平均分
            // 提示：使用foreach循环计算总分，然后除以科目数
            // done
            List<Score> studentScores = GetStudentScores(studentId);
            
            // 空值检查
            if (studentScores.Count == 0)
            {
                throw new InvalidOperationException($"学号为 {studentId} 的学生没有成绩记录");
            }

            // 计算总分
            double totalPoints = 0;
            foreach (var score in studentScores)
            {
                totalPoints += score.points;
            }

            // 返回平均分
            return totalPoints / studentScores.Count;
        }

        // TODO: 使用模式匹配实现成绩等级转换
        // done
        /// <summary>
        /// 成绩 -> 等级转换
        /// </summary>
        /// <param name="score"></param>
        /// <returns></returns>
        public Grade GetGrade(double score)
        {
            switch (score)
            {
                case < 60:
                    return Grade.F;
                case < 70:
                    return Grade.D;
                case < 80:
                    return Grade.C;
                case < 90:
                    return Grade.B;
                default:
                    return Grade.A;
            }
        }

        /// <summary>
        /// 获取平均分最高的count个学生
        /// </summary>
        /// <param name="count">学生数</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public List<(string StudentId, double Average)> GetTopStudents(int count = 1)
        {
            // TODO: 使用简单循环获取平均分最高的学生
            // 提示：可以先计算所有学生的平均分，然后排序取前count个
            // done

            // 空值检查
            foreach (var studentId in scores.Keys)
            {
                if (scores[studentId].Count == 0)
                {
                    throw new InvalidOperationException($"学号为 {studentId} 的学生没有成绩记录");
                }
            }

            // 输入参数检验
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "学生数必须大于0");
            }

            // 建立学生平均分列表averages
            List<(string StudentId, double Average)> averages = new List<(string, double)>();
            foreach (var studentId in scores.Keys)
            {
                double average = CalculateAverage(studentId);
                averages.Add((studentId, average));
            }

            // averages列表按平均分降序排序
            averages.Sort((x, y) => y.Average.CompareTo(x.Average));

            // 返回前count个学生
            return averages.GetRange(0, Math.Min(count, averages.Count));
        }

        public Dictionary<string, List<Score>> GetAllScores()
        {
            return new Dictionary<string, List<Score>>(scores);
        }
    }

    /// <summary>
    /// 数据管理类
    /// </summary>
    public class DataManager
    {
        /// <summary>
        /// 保存学生数据到文件
        /// </summary>
        /// <param name="students"></param>
        /// <param name="filePath"></param>
        public void SaveStudentsToFile(List<Student> students, string filePath)
        {
            // TODO: 实现保存学生数据到文件
            // 提示：使用StreamWriter，格式为CSV
            // done
            try
            {
                // 在这里实现文件写入逻辑
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // 写入CSV头
                    writer.WriteLine("学号, 姓名, 成绩");
                    
                    // 写入每个学生的信息
                    foreach (var student in students)
                    {
                        writer.WriteLine($"{student.studentId}, {student.name}, {student.age}");
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存文件时发生错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 从文件读取学生数据
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public List<Student> LoadStudentsFromFile(string filePath)
        {
            List<Student> students = new List<Student>();
            
            // TODO: 实现从文件读取学生数据
            // 提示：使用StreamReader，解析CSV格式
            // done
            try
            {
                // 在这里实现文件读取逻辑

                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    
                    // 跳过CSV头（一行）
                    reader.ReadLine();

                    // 逐行读取并解析
                    while ((line = reader.ReadLine()) != null)
                    {
                        var parts = line.Split(',');
                        if (parts.Length == 3)
                        {
                            string studentId = parts[0].Trim();
                            string name = parts[1].Trim();
                            if (int.TryParse(parts[2].Trim(), out int age))
                            {
                                students.Add(new Student(studentId, name, age));
                            }
                            else
                            {
                                Console.WriteLine($"无效的年龄数据: {parts[2]}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"无效的行格式: {line}");
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
                studentManager.Add(new Student("2021004", "赵六", 20));
                studentManager.Add(new Student("2021005", "陈七", 22));
                studentManager.Add(new Student("2021006", "林八", 18));
                studentManager.Add(new Student("2021007", "吴九", 20));
                studentManager.Add(new Student("2021008", "郑十", 19));
                studentManager.Add(new Student("2021009", "周十一", 21));
                studentManager.Add(new Student("2021010", "孙十二", 20));

                Console.WriteLine("学生信息添加完成");

                // 2. 成绩数据（每个学生各2门课程）
                Console.WriteLine("\n2. 添加成绩信息:");
                scoreManager.AddScore("2021001", new Score("数学", 95.5));
                scoreManager.AddScore("2021001", new Score("英语", 87.0));
                
                scoreManager.AddScore("2021002", new Score("数学", 78.5));
                scoreManager.AddScore("2021002", new Score("英语", 85.5));
                
                scoreManager.AddScore("2021003", new Score("数学", 88.0));
                scoreManager.AddScore("2021003", new Score("英语", 92.0));

                scoreManager.AddScore("2021004", new Score("语文", 72.5));
                scoreManager.AddScore("2021005", new Score("数学", 68.0));
                scoreManager.AddScore("2021006", new Score("英语", 87.5));
                scoreManager.AddScore("2021007", new Score("语文", 79.0));
                scoreManager.AddScore("2021008", new Score("数学", 95.5));
                scoreManager.AddScore("2021009", new Score("英语", 64.0));
                scoreManager.AddScore("2021010", new Score("语文", 88.5));
                scoreManager.AddScore("2021003", new Score("语文", 89.0));
                scoreManager.AddScore("2021004", new Score("数学", 73.5));
                scoreManager.AddScore("2021005", new Score("英语", 82.0));
                scoreManager.AddScore("2021006", new Score("语文", 94.5));
                scoreManager.AddScore("2021007", new Score("数学", 61.5));
                scoreManager.AddScore("2021008", new Score("英语", 90.0));
                scoreManager.AddScore("2021009", new Score("语文", 75.5));
                scoreManager.AddScore("2021010", new Score("数学", 84.0));
                scoreManager.AddScore("2021002", new Score("语文", 66.5));
                scoreManager.AddScore("2021003", new Score("数学", 97.0));

                Console.WriteLine("成绩信息添加完成");

                // 3. 测试年龄范围查询
                Console.WriteLine("\n3. 查找年龄在19-20岁的学生:");
                // TODO: 调用GetStudentsByAge方法并显示结果
                // done
                studentManager.GetStudentsByAge(19, 20).ForEach(student =>
                {
                    Console.WriteLine($"学号: {student.studentId}, 姓名: {student.name}, 年龄: {student.age}");
                });


                // 4. 显示学生成绩统计
                Console.WriteLine("\n4. 学生成绩统计:");
                // TODO: 遍历所有学生，显示其成绩、平均分和等级
                // done
                
                foreach (var student in studentManager.GetAll())
                {
                    Console.WriteLine($"\n学号: {student.studentId}, 姓名: {student.name}, 年龄: {student.age}");
                    
                    // 获取学生成绩
                    var scores = scoreManager.GetStudentScores(student.studentId);
                    if (scores.Count > 0)
                    {
                        // 显示成绩
                        Console.WriteLine("此学生的所有科目成绩如下:");
                        foreach (var score in scores)
                        {
                            Console.WriteLine(score.ToString());
                        }
                        
                        // 计算平均分和等级并显示
                        double average = scoreManager.CalculateAverage(student.studentId);
                        Grade grade = scoreManager.GetGrade(average);
                        Console.WriteLine($"------\n平均分: {average:F1}, 等级: {grade}");
                    }
                    else
                    {
                        Console.WriteLine("该学生没有成绩记录");
                    }
                }

                // 5. 显示排名（简化版）
                Console.WriteLine("\n5. 平均分最高的学生信息如下:");
                // TODO: 调用GetTopStudents(1)方法显示第一名
                
                var topStudents = scoreManager.GetTopStudents(1);
                if (topStudents.Count > 0)
                {
                    var topStudent = topStudents[0];
                    Console.WriteLine($"学号: {topStudent.StudentId}, 平均分: {topStudent.Average:F1}");
                }
                else
                {
                    Console.WriteLine("没有学生记录");
                }

                // 6. 文件操作
                Console.WriteLine("\n6. 数据持久化演示:");
                // TODO: 保存和读取学生文件

                string filePath = "students.csv";
                // 保存学生数据到文件
                dataManager.SaveStudentsToFile(studentManager.GetAll(), filePath);
                Console.WriteLine($"学生数据已保存到文件: {filePath}");
                // 从文件读取学生数据
                var loadedStudents = dataManager.LoadStudentsFromFile(filePath);
                Console.WriteLine("从文件加载的学生数据:");
                
                foreach (var student in loadedStudents)
                {
                    Console.WriteLine(student.ToString());
                }
                Console.WriteLine($"------\n共加载 {loadedStudents.Count} 个学生记录");
                Console.WriteLine($"文件内容: {File.ReadAllText(filePath)}");

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