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
            try{
                if(studentId is null) throw new ArgumentNullException("学号不能为空");
                if(name is null) throw new ArgumentNullException("姓名不能为空");
                if(age==0) throw new ArgumentNullException("年龄不能为0");
            }
            catch(Exception ex){
            Console.WriteLine($"Caught an exception: {ex.Message}");
            throw;  // 重新抛出异常
            }
            StudentId=studentId;
            Name=name;
            Age=age;
        }

        public override string ToString()
        {
            // TODO: 返回格式化的学生信息字符串
            return $"Id:{StudentId},Name:{Name},Age:{Age} ";
        }

        // TODO: 实现IComparable接口，按学号排序
        // 提示：使用string.Compare方法
        public int CompareTo(Student? other)
        {
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
            try{
                if(subject is null) throw new ArgumentNullException("科目不能为空");
            }
            catch(Exception ex){
            Console.WriteLine($"Caught an exception: {ex.Message}");
            throw;  // 重新抛出异常
            }
            Subject=subject;
            Points=points;
        }

        public override string ToString()
        {
            // TODO: 返回格式化的成绩信息
            return $"{Subject}:{Points} ";
        }
    }

    // 学生管理类
    public class StudentManager : IRepository<Student>
    {
        // TODO: 定义私有字段存储学生列表
        // 提示：使用List<Student>存储
        private List<Student> L=new List<Student>;

        public void Add(Student item)
        {
            // TODO: 实现添加学生的逻辑
            // 1. 参数验证
            // 2. 添加到列表
            try{
                if(item is null) throw new ArgumentNullException("学生不能为空");
            }
            catch(Exception ex){
            Console.WriteLine($"Caught an exception: {ex.Message}");
            throw;  // 重新抛出异常
            }
            L.Add(item);
        }

        public bool Remove(Student item)
        {
            // TODO: 实现Remove方法
            return L.Remove(item);
        }

        public List<Student> GetAll()
        {
            // TODO: 返回学生列表的副本
            return new List<Student> _L(L);
        }

        public List<Student> Find(Func<Student, bool> predicate)
        {
            // TODO: 使用foreach循环查找符合条件的学生
            List<Student> CL=new List<Student>;
            foreach(var item in L){
                if(predicate(item)) CL.Add(item);
            }
            return CL;
        }

        // 查找年龄在指定范围内的学生
        public List<Student> GetStudentsByAge(int minAge, int maxAge)
        {
            // TODO: 使用foreach循环和if判断实现年龄范围查询
            List<Student> CL=new List<Student>;
            foreach(var item in L){
                if(item.Age>=minAge and item.Age<=maxAge) CL.Add(item);
            }
            return CL;
        }
    }

    // 成绩管理类
    public class ScoreManager
    {
        // TODO: 定义私有字段存储成绩字典
        // 提示：使用Dictionary<string, List<Score>>存储
        private Dictionary<string,List<Score>> D=new Dictionary<string, List<Score>>;

        public void AddScore(string studentId, Score score)
        {
            // TODO: 实现添加成绩的逻辑
            // 1. 参数验证
            // 2. 初始化学生成绩列表（如不存在）
            // 3. 添加成绩
            if(studentId is null) throw new ArgumentNullException("学号不能为空");
            D[studengId]=new List<Score>();
            d[studentId].Add(score);
        }

        public List<Score> GetStudentScores(string studentId)
        {
            // TODO: 获取指定学生的所有成绩
            return D[studentId];
        }

        public double CalculateAverage(string studentId)
        {
            // TODO: 计算指定学生的平均分
            // 提示：使用foreach循环计算总分，然后除以科目数
            double total=0;
            int num=0;//科目数
            foreach(var item in D[studentId]){
                total+=item.Points;
                num++;
            }
            return total/num;
        }

        // TODO: 使用模式匹配实现成绩等级转换
        public Grade GetGrade(double score)
        {
            Grade degree;
            switch(score.Points){
                case >=90 :degree=Grade.A;break;
                case <90 and >=80 :degree=Grade.B;break;
                case <80 and >=70 :degree=Grade.C;break;
                case <70 and >=60 :degree=Grade.D;break;
                case <60 :degree=Grade.F;
            }
            return degree;
        }

        public List<(string StudentId, double Average)> GetTopStudents(int count)
        {
            // TODO: 使用简单循环获取平均分最高的学生
            // 提示：可以先计算所有学生的平均分，然后排序取前count个
            var averages=new List<(string,double)>;
            foreach(var item in D){
                averages.Add(item.Key,CalculateAverage(item.Key));
            }//计算每个学生的平均分
            var topStudents = averages.OrderByDescending(x => x.Average).Take(count).ToList();
            return topStudents;
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
                StreamWriter writer=new StreamWriter(filePath);
                writer.WriteLine("StudentId,Name,Age,CourseName,ScoreValue");//写入CSV表头
                foreach(var item in students){
                    string escapedName = student.Name.Contains(',') ? $"\"{student.Name}\"" : student.Name;
                    string escapedCourse = score.CourseName.Contains(',') ? $"\"{score.CourseName}\"" : score.CourseName;
                    //处理特殊转义字符
                    writer.WriteLine($"{student.StudentId},{escapedName},{student.Age},{escapedCourse},{score.Value.ToString(CultureInfo.InvariantCulture)}");
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
                List<Student> students = new List<Student>();
                StreamReader reader=new StreamReader(filePath);
                string headerLine = reader.ReadLine();//跳过标题
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var fields = ParseCsvLine(line);// 处理CSV行（考虑字段中包含逗号和引号的情况）
                    if (fields.Count >= 3) // 至少需要学号、姓名、年龄三个字段
                    {
                        string studentId = UnescapeCsvField(fields[0]);
                        string name = UnescapeCsvField(fields[1]);
                        int age;
                        Student student = new Student(studentId, name, age);
                        students.Add(student);
                    }
                    else
                    {
                        Console.WriteLine($"警告：跳过格式不正确的行: {line}");
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
                int min=19;int max=20;
                var studentsBA=studentManager.GetStudentsByAge(min,max);
                foreach(var item in studentsBA){
                    Console.Write($"{item.Name},");
                }
                // 4. 显示学生成绩统计
                Console.WriteLine("\n4. 学生成绩统计:");
                // TODO: 遍历所有学生，显示其成绩、平均分和等级
                foreach(var item in studentManager.GetAll()){
                    Console.Write($"{item.Name}:");
                    foreach(var item1 in scoreManager.GetStudentScores(item.StudentId)){
                        Consile.Write(item1.ToString());
                    }//显示格式化成绩信息
                    Console.Write($"Average:{CalculateAverage(item.StudentId)} ");//显示平均成绩
                    Console.Write($"Degree:{scoreManager.GetGrade(CalculateAverage(item.StudentId))}");
                    //显示等级
                }

                // 5. 显示排名（简化版）
                Console.WriteLine("\n5. 平均分最高的学生:");
                // TODO: 调用GetTopStudents(1)方法显示第一名
                foreach(var item2 in scoreManager.GetTopStudents(1)){
                    Console.Write(item2.StudentId);
                }

                // 6. 文件操作
                Console.WriteLine("\n6. 数据持久化演示:");
                // TODO: 保存和读取学生文件
                dataManager.SaveStudentsToFile(studentManager.GetAll(), string filePath);
                studentManager=new studentManager(dataManager.LoadStudentsFromFile(string filePath));

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
