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
        F=0,
        D=60,
        C=70,
        B=80,
        A=90
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
        List<T> Find(Func<T,bool> predicate);
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
            if(string.IsNullOrWhiteSpace(studentId))
                throw new ArgumentNullException(nameof(studentId),"学号不能为空");
            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name),"姓名不能为空");
            if(age<=0)
                throw new ArgumentNullException(nameof(age),"年龄不能非正"); 
            StudentId=studentId;
            Name=name;
            Age=age;
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
            if(other==null)
                return -1;
            return string.Compare(StudentId,other.StudentId);
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
                throw new ArgumentNullException(nameof(subject),"学科不能为空");
            if(points<0||points>100)
                throw new ArgumentNullException(nameof(points),"分数应在0~100"); 
            Subject=subject;
            Points=points;
        }

        public override string ToString()
        {
            // TODO: 返回格式化的成绩信息
            return $"学科：{Subject}，分数：{Points:F1}";
        }
    }

    // 学生管理类
    public class StudentManager : IRepository<Student>
    {
        // TODO: 定义私有字段存储学生列表
        // 提示：使用List<Student>存储
        private List<Student> students=new List<Student>();

        public void Add(Student item)
        {
            // TODO: 实现添加学生的逻辑
            // 1. 参数验证
            // 2. 添加到列表
            if(item==null)
                throw new ArgumentNullException(nameof(item),"添加的学生不能为空");
            if(students.Contains(item))
                throw new ArgumentNullException($"学号为{item.StudentId}的学生已经存在");
            students.Add(item);
        }

        public bool Remove(Student item)
        {
            // TODO: 实现Remove方法
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
            List<Student> find=new List<Student>();
            foreach(var student in students)
            {
                if(predicate(student))
                    find.Add(student);
            }
            return find;
        }

        // 查找年龄在指定范围内的学生
        public List<Student> GetStudentsByAge(int minAge, int maxAge)
        {
            // TODO: 使用foreach循环和if判断实现年龄范围查询
            List<Student> get=new List<Student>();
            foreach(var student in students)
            {
                if(student.Age>=minAge&&student.Age<=maxAge)
                    get.Add(student);
            }
            return get;
        }
    }

    // 成绩管理类
    public class ScoreManager
    {
        // TODO: 定义私有字段存储成绩字典
        // 提示：使用Dictionary<string, List<Score>>存储
        private Dictionary<string,List<Score>> scores=new Dictionary<string,List<Score>>();

        public void AddScore(string studentId, Score score)
        {
            // TODO: 实现添加成绩的逻辑
            // 1. 参数验证
            // 2. 初始化学生成绩列表（如不存在）
            // 3. 添加成绩
            if(string.IsNullOrWhiteSpace(studentId))
                throw new ArgumentNullException(nameof(studentId),"学号不能为空");
            if(score==null)
                throw new ArgumentNullException(nameof(score),"成绩不能为空");
            if(!scores.ContainsKey(studentId))
                scores[studentId]=new List<Score>();
            scores[studentId].Add(score);
        }

        public List<Score> GetStudentScores(string studentId)
        {
            // TODO: 获取指定学生的所有成绩
            if(scores.TryGetValue(studentId,out var studentScores))
                return new List<Score>(studentScores);
            return new List<Score>();
        }

        public double CalculateAverage(string studentId)
        {
            // TODO: 计算指定学生的平均分
            // 提示：使用foreach循环计算总分，然后除以科目数
            List<Score> studentScores=GetStudentScores(studentId);
            if(studentScores.Count==0)
                return 0;
            double sum=0;
            foreach(var score in studentScores)
            {
                sum+=score.Points;
            }
            return sum/studentScores.Count;
        }

        // TODO: 使用模式匹配实现成绩等级转换
        public Grade GetGrade(double score)
        {
            return score switch
            {
                >=90=>Grade.A,
                >=80=>Grade.B,
                >=70=>Grade.C,
                >=60=>Grade.D,
                _=>Grade.F
            };
        }

        public List<(string StudentId, double Average)> GetTopStudents(int count)
        {
            // TODO: 使用简单循环获取平均分最高的学生
            // 提示：可以先计算所有学生的平均分，然后排序取前count个
            List<(string studentId,double average)> averages=new List<(string,double)>();
            foreach(var id in scores)
            {
                double idAverage=CalculateAverage(id.Key);
                averages.Add((id.Key,idAverage));
            }
            for(int i=0;i<averages.Count-1;i++)
            {
                for(int j=0;j<averages.Count-i-1;j++)
                {
                    if(averages[j].average<averages[j+1].average)
                    {
                        var temp=averages[j];
                        averages[j]=averages[j+1];
                        averages[j+1]=temp;
                    }
                }
            }
            var getTop=new List<(string,double)>();
            for(int i=0;i<count;i++)
            {
                getTop.Add(averages[i]);
            }
            return getTop;
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
                using(var write=new StreamWriter(filePath))
                {
                    foreach(var student in students)
                    {
                         write.WriteLine($"{student.StudentId},{student.Name},{student.Age}");
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
                using(var read=new StreamReader(filePath))
                {
                    string? line;
                    while((line=read.ReadLine())!=null)
                    {
                        var data=line.Split(',');
                        if(data.Length==3)
                        {
                            if(int.TryParse(data[2],out int age))
                            {
                                students.Add(new Student(data[0],data[1],age));
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
                var studnetsInAge=studentManager.GetStudentsByAge(19,20);
                Console.WriteLine($"年龄在19-20岁的学生有{studnetsInAge.Count}个：");
                foreach(var students in studnetsInAge)
                {
                    Console.WriteLine(students.ToString());
                }

                // 4. 显示学生成绩统计
                Console.WriteLine("\n4. 学生成绩统计:");
                // TODO: 遍历所有学生，显示其成绩、平均分和等级
                var allStudents=studentManager.GetAll();
                foreach(var students in allStudents)
                {
                    Console.WriteLine($"\n{students.ToString()}");
                    var studentScores=scoreManager.GetStudentScores(students.StudentId);
                    Console.WriteLine("成绩列表:");
                    foreach(var score in studentScores)
                    {
                        Console.WriteLine($"{score.ToString()}");
                    }
                    double studentAverage=scoreManager.CalculateAverage(students.StudentId);
                    Grade studentGrade=scoreManager.GetGrade(studentAverage);
                    Console.WriteLine($"平均分：{studentAverage:F2},等级：{studentGrade}");
                }

                // 5. 显示排名（简化版）
                Console.WriteLine("\n5. 平均分最高的学生:");
                // TODO: 调用GetTopStudents(1)方法显示第一名
                var topOne=scoreManager.GetTopStudents(1);
                var topStudent=topOne[0];
                var student=studentManager.Find(s=>s.StudentId==topStudent.StudentId)[0];
                Console.WriteLine($"学号：{student.StudentId},姓名：{student.Name},平均分：{topStudent.Average:F1}");

                // 6. 文件操作
                Console.WriteLine("\n6. 数据持久化演示:");
                // TODO: 保存和读取学生文件
                string filePath="students.csv";
                dataManager.SaveStudentsToFile(studentManager.GetAll(),filePath);
                var load=dataManager.LoadStudentsFromFile(filePath);
                Console.WriteLine("\n读取到的学生数据为：");
                foreach(var students in load)
                {
                    Console.WriteLine(students.ToString());
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