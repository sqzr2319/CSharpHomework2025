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
        F = 0, D = 60, C = 70, B = 80, A = 90
    }

    // 泛型仓储接口
    public interface IRepository<T>
    {
        // TODO: 定义接口方法
        // Add(T item)
        // Remove(T item) 返回bool
        // GetAll() 返回List<T>
        // Find(Func<T, bool> predicate) 返回List<T>
        public void Add(T item);
        public bool Remove(T item);
        public List<T> GetAll();
        public List<T> Find(Func<T, bool> predicate);
    }

    // 学生类
    public class Student : IComparable<Student>
    {
        // TODO: 定义字段 StudentId, Name, Age
        public string? StudentId;
        public string? Name;
        public int Age;
        
        public Student(string studentId, string name, int age)
        {
            // TODO: 实现构造方法，包含参数验证（空值检查）
            if(studentId is not null)
            {
                StudentId = studentId;
            }
            else
            {
                StudentId = null;
            }
            if (name is not null)
            {
                Name = name;
            }
            else
            {
                Name = null;
            }
            Age = age;
        }

        public override string ToString()
        {
            // TODO: 返回格式化的学生信息字符串
            return $"学号:{StudentId},姓名:{Name},年龄:{Age};";
        }

        // TODO: 实现IComparable接口，按学号排序
        // 提示：使用string.Compare方法
        public int CompareTo(Student? other)
        {
            if (other is null)
            {
                return 1;
            }
            else
            {
                return string.Compare(this.StudentId, other.StudentId);
            }
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
        public string? Subject;
        public double Points;
        
        public Score(string subject, double points)
        {
            // TODO: 实现构造方法，包含参数验证
            if(subject is not null)
            {
                Subject = subject;
            }
            else
            {
                Subject = null;
            }
            Points = points;
        }

        public override string ToString()
        {
            // TODO: 返回格式化的成绩信息
            return $"科目:{Subject},分数:{Points};";
        }
    }

    // 学生管理类
    public class StudentManager : IRepository<Student>
    {
        // TODO: 定义私有字段存储学生列表
        // 提示：使用List<Student>存储
        private List<Student> studentList = new List<Student>();

        public void Add(Student item)
        {
            // TODO: 实现添加学生的逻辑
            // 1. 参数验证
            // 2. 添加到列表
            if (item is not null)
            {
                studentList.Add(item);
            }
        }

        public bool Remove(Student item)
        {
            // TODO: 实现Remove方法
            if(studentList.Remove(item)) return true;
            return false;
        }

        public List<Student> GetAll()
        {
            // TODO: 返回学生列表的副本
            return new List<Student>(studentList);
        }

        public List<Student> Find(Func<Student, bool> predicate)
        {
            // TODO: 使用foreach循环查找符合条件的学生
            List<Student> result = new List<Student>();
            foreach (Student student in studentList)
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
            foreach (Student student in studentList)
            {
                if(student.Age <= maxAge && student.Age >= minAge)
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
        private Dictionary<string, List<Score>> idScoreList = new Dictionary<string, List<Score>>();

        public void AddScore(string studentId, Score score)
        {
            // TODO: 实现添加成绩的逻辑
            // 1. 参数验证
            // 2. 初始化学生成绩列表（如不存在）
            // 3. 添加成绩
            if (studentId is not null)
            {
                if (!idScoreList.ContainsKey(studentId))
                {
                    idScoreList[studentId] = new List<Score>();
                }
                idScoreList[studentId].Add(score);
            }
        }

        public List<Score> GetStudentScores(string studentId)
        {
            // TODO: 获取指定学生的所有成绩
            if (idScoreList.TryGetValue(studentId, out var scoreList))
            {
                return scoreList;
            }
            return new List<Score>();
        }

        public double CalculateAverage(string studentId)
        {
            // TODO: 计算指定学生的平均分
            // 提示：使用foreach循环计算总分，然后除以科目数
            double average = 0.0;
            if (studentId is not null)
            {
                double sum = 0;
                int num = 0;
                foreach (Score score in idScoreList[studentId])
                {
                    sum += score.Points;
                    num++;
                }
                if (num != 0) average = sum / num;
            }
            return average;
        }

        // TODO: 使用模式匹配实现成绩等级转换
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

        public List<(string StudentId, double Average)> GetTopStudents(int count)
        {
            // TODO: 使用简单循环获取平均分最高的学生
            // 提示：可以先计算所有学生的平均分，然后排序取前count个
            List<(string StudentId, double Average)> averageList = new List<(string StudentId, double Average)> ();
            foreach (var idScore in idScoreList)
            {
                string id = idScore.Key;
                averageList.Add((id,CalculateAverage(id)));
            }
            for(int i = 0; i < averageList.Count; i++)
            {
                for(int j = i + 1; j < averageList.Count; j++)
                {
                    if (averageList[j].Average > averageList[i].Average)
                    {
                        var temp = averageList[j];
                        averageList[j] = averageList[i];
                        averageList[i] = temp;
                    }
                }
            }
            List<(string StudentId, double Average)> topStudents = new List<(string, double)>();
            int takeCount = Math.Min(count, averageList.Count);
            for (int i = 0; i < takeCount; i++)
            {
                topStudents.Add(averageList[i]);
            }
            return topStudents;
        }

        public Dictionary<string, List<Score>> GetAllScores()
        {
            return new Dictionary<string, List<Score>>(idScoreList);
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
                    foreach (Student student in students)
                    {
                        string id = student.StudentId ?? " ";
                        string name = student.Name ?? " ";
                        writer.WriteLine($"{id},{name},{student.Age}");
                    }
                    Console.WriteLine("文件保存成功");
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
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("文件不存在");
                    return students;
                }

                using (StreamReader reader = new StreamReader(filePath))
                {
                    string? header = reader.ReadLine();
                    string? line;
                    while((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split(',');
                        if (parts.Length == 3 && int.TryParse(parts[2], out int age))
                        {
                            students.Add(new Student(parts[0], parts[1], age));
                        }
                    }
                    Console.WriteLine($"文件读取成功");
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
                foreach (Student student in studentManager.GetStudentsByAge(19, 20))
                {
                    Console.WriteLine($"学号:{student.StudentId},姓名:{student.Name},年龄:{student.Age};");
                }

                // 4. 显示学生成绩统计
                Console.WriteLine("\n4. 学生成绩统计:");
                // TODO: 遍历所有学生，显示其成绩、平均分和等级
                foreach(Student student in studentManager.GetAll())
                {
                    Console.WriteLine($"学号:{student.StudentId},姓名:{student.Name},年龄:{student.Age};");
                    foreach (Score score in scoreManager.GetStudentScores(student.StudentId))
                    {
                        Console.WriteLine($"科目:{score.Subject},成绩:{score.Points};");
                    }
                    double average = scoreManager.CalculateAverage(student.StudentId);
                    Grade grade = scoreManager.GetGrade(average);
                    Console.WriteLine($"平均分:{average},等级:{grade}.");
                }

                // 5. 显示排名（简化版）
                Console.WriteLine("\n5. 平均分最高的学生:");
                // TODO: 调用GetTopStudents(1)方法显示第一名
                var top = scoreManager.GetTopStudents(1).FirstOrDefault();
                Console.WriteLine($"学号:{top.StudentId}, 平均分:{top.Average:F2}");

                // 6. 文件操作
                Console.WriteLine("\n6. 数据持久化演示:");
                // TODO: 保存和读取学生文件
                dataManager.SaveStudentsToFile(studentManager.GetAll(), "StudentData.txt");
                dataManager.LoadStudentsFromFile("StudentData.txt");

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