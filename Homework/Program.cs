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
        public void Add(T item);
        // Remove(T item) 返回bool
        public bool Remove(T item);
        // GetAll() 返回List<T>
        public List<T> GetAll();
        // Find(Func<T, bool> predicate) 返回List<T>
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
            try
            {
                if (studentId is null)
                {
                    throw new ArgumentException("学号不能为空！");
                }
                else this.StudentId = studentId;

                if (name is null)
                {
                    throw new ArgumentException("姓名不能为空！");
                }
                else this.Name = name;

                if (!(age >= 6 && age <= 80))
                {
                    throw new ArgumentException("年龄应符合现实（6~80）");
                }
                else this.Age = age;
            }
            catch(ArgumentException ex)
            {
                Console.WriteLine($"错误：{ex.Message}");
            }
        }

        public override string ToString()
        {
            // TODO: 返回格式化的学生信息字符串
            string stu_info = $"学号：{this.StudentId}\t姓名：{this.Name}\t年龄：{this.Age}";
            return stu_info;
        }

        // TODO: 实现IComparable接口，按学号排序
        // 提示：使用string.Compare方法
        public int CompareTo(Student? other)
        {
            if (other == null)
            {
                return -1; //排序在null前
            }
            return string.Compare(this.StudentId, other.StudentId);
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
        public string Subject = new("");
        public double Points;
        
        public Score(string subject, double points)
        {
            // TODO: 实现构造方法，包含参数验证
            try
            {
                Subject = subject;

                if (!(points >= 0.0 && points <= 100.0))
                {
                    throw new ArgumentException("成绩应该在0~100区间内！");
                }
                else Points = points;
            }
            catch (ArgumentException ex) {
                Console.WriteLine($"错误：{ex.Message}");
            }
        }

        public override string ToString()
        {
            // TODO: 返回格式化的成绩信息
            string sco_info = $"科目：{this.Subject}\t分数：{this.Points}";
            return sco_info;
        }
    }

    // 学生管理类
    public class StudentManager : IRepository<Student>
    {
        // TODO: 定义私有字段存储学生列表
        // 提示：使用List<Student>存储
        private List<Student> students = new();

        public void Add(Student item)
        {
            // TODO: 实现添加学生的逻辑
            // 1. 参数验证
            // 2. 添加到列表
            try
            {
                if(item == null)
                {
                    throw new ArgumentException("请先填写好学生信息！");
                }
                else
                {
                    students.Add(item);
                }
            }
            catch(ArgumentException ex)
            {
                Console.WriteLine($"错误：{ex.Message}");
            }
        }

        public bool Remove(Student item)
        {
            // TODO: 实现Remove方法
            foreach (Student stu in students)
            {
                if (stu == item)
                {
                    return students.Remove(stu);
                }
            }
            return false;
        }

        public List<Student> GetAll()
        {
            // TODO: 返回学生列表的副本
            List<Student> students_backup = students;
            return students_backup;
        }

        public List<Student> Find(Func<Student, bool> predicate)
        {
            // TODO: 使用foreach循环查找符合条件的学生
            List<Student> students_pre = new List<Student> { };
            foreach(Student student in students)
            {
                if(predicate(student))
                {
                    students_pre.Add(student);
                }
            }
            return students_pre;
        }

        // 查找年龄在指定范围内的学生
        public List<Student> GetStudentsByAge(int minAge, int maxAge)
        {
            // TODO: 使用foreach循环和if判断实现年龄范围查询
            List<Student> students_result = new List<Student>();
            foreach (Student student in students)
            {
                if(minAge <= student.Age && maxAge >= student.Age)
                {
                    students_result.Add(student);
                }
            }
            return students_result;
        }
    }

    // 成绩管理类
    public class ScoreManager
    {
        // TODO: 定义私有字段存储成绩字典
        // 提示：使用Dictionary<string, List<Score>>存储
        private Dictionary<string, List<Score>> scores = new();

        public void AddScore(string studentId, Score score)
        {
            // TODO: 实现添加成绩的逻辑
            // 1. 参数验证
            // 2. 初始化学生成绩列表（如不存在）
            // 3. 添加成绩
            if(! scores.ContainsKey(studentId))
            {
                scores[studentId] = new List<Score>();
            }
            scores[studentId].Add(score);
        }

        public List<Score> GetStudentScores(string studentId)
        {
            // TODO: 获取指定学生的所有成绩
            try
            {
                foreach (string stu_Id in scores.Keys)
                {
                    if (studentId == stu_Id)
                    {
                        return scores[studentId];
                    }
                }
                throw new Exception("未查找到该学号学生的成绩！");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误：{ex.Message}");
                return null;
            }
        }

        public double CalculateAverage(string studentId)
        {
            // TODO: 计算指定学生的平均分
            // 提示：使用foreach循环计算总分，然后除以科目数
            double average_score = 0;
            double total_score = 0;
            try
            {
                foreach (string stu_Id in scores.Keys)
                {
                    if (studentId == stu_Id)
                    {
                        int subject_num = 0;
                        foreach (Score score in  scores[studentId])
                        {
                            total_score += score.Points;
                            subject_num++;
                        }
                        average_score = total_score / subject_num;
                        return average_score;
                    }
                }
                throw new Exception("未查找到该学号学生的成绩！");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误：{ex.Message}");
                return 0;
            }

        }

        // TODO: 使用模式匹配实现成绩等级转换
        public Grade GetGrade(double score)
        {
            try
            {
                if (score >= 0 && score < 60) return Grade.F;
                else if (score >= 60 && score < 70) return Grade.D;
                else if (score >= 70 && score < 80) return Grade.C;
                else if (score >= 80 && score < 90) return Grade.B;
                else if (score >= 90 && score <= 100) return Grade.A;
                else throw new Exception("成绩异常！");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误：{ex.Message}");
                return Grade.F;  //成绩有误，默认为F
            }
        }

        public List<(string StudentId, double Average)> GetTopStudents(int count)
        {
            // TODO: 使用简单循环获取平均分最高的学生
            // 提示：可以先计算所有学生的平均分，然后排序取前count个
            List<(string StudentId, double Average)> StudentRank = new();
            List<(string StudentId, double Average)> StudentTop = new();
            int stu_num = 0;
            foreach (string studentId in scores.Keys)
            {
                StudentRank.Add((studentId,CalculateAverage(studentId)));
                stu_num++;
            }
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < stu_num - i - 1; j++)
                {
                    if (StudentRank[j].Average > StudentRank[j+1].Average)
                    {
                        (string, double) temp = StudentRank[j];
                        StudentRank[j] = StudentRank[j+1];
                        StudentRank[j+1] = temp;
                    }
                }
            }
            for (int i = 0; i < count; i++)
            {
                StudentTop.Add(StudentRank[stu_num - 1 - i]);
            }
            return StudentTop;
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
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    foreach (Student student in students)
                    {
                        sw.WriteLine($"{student.StudentId}\t{student.Name}\t{student.Age}");
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
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    string[] infomation;
                    while ((line = sr.ReadLine()) != null)
                    {
                        infomation = line.Split("\t");
                        string studentId = infomation[0];
                        string studentName = infomation[1];
                        int studentAge = int.TryParse(infomation[2], out int age) ? age : 0;
                        Student student = new(studentId, studentName, studentAge);
                        students.Add(student);
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
                List<Student> studentSpecificAge = studentManager.GetStudentsByAge(19, 20);
                for (int i = 0; i < studentSpecificAge.Count; i++)
                {
                    Console.WriteLine(studentSpecificAge[i].ToString());
                }


                // 4. 显示学生成绩统计
                Console.WriteLine("\n4. 学生成绩统计:");
                // TODO: 遍历所有学生，显示其成绩、平均分和等级
                Dictionary<string, List<Score>> studentAllScore = new();
                Dictionary<string, double> studentAverageScore = new();
                Dictionary<string, Grade> studentGrade = new();
                // 获取所有学生的各科成绩
                studentAllScore = scoreManager.GetAllScores();  
                // 计算所有学生的平均分
                foreach (string studentId in studentAllScore.Keys)  
                {
                    studentAverageScore[studentId] = scoreManager.CalculateAverage(studentId);
                }
                // 获得所有学生的等级
                foreach (string studentId in studentAllScore.Keys)
                {
                    studentGrade[studentId] = scoreManager.GetGrade(studentAverageScore[studentId]);
                }
                // 打印
                foreach(Student student in studentManager.GetAll())
                {
                    Console.WriteLine(student.ToString());
                    for (int i = 0; i < studentAllScore[student.StudentId].Count; i++)
                    {
                        Console.WriteLine($"{studentAllScore[student.StudentId][i].Subject}：{studentAllScore[student.StudentId][i].Points}");
                    }
                    Console.WriteLine($"平均分：{studentAverageScore[student.StudentId]}\t等级：{studentGrade[student.StudentId]}\n");
                }


                // 5. 显示排名（简化版）
                Console.WriteLine("\n5. 平均分最高的学生:");
                // TODO: 调用GetTopStudents(1)方法显示第一名
                List<(string studentId, double average)> studentTop = new();
                studentTop = scoreManager.GetTopStudents(1);
                string top_studentId = studentTop[0].studentId;
                double top_average = studentTop[0].average;
                foreach (Student student in studentManager.GetAll())
                {
                    if (top_studentId == student.StudentId)
                    {
                        Console.WriteLine(student.Name);
                    }
                }
                Console.WriteLine($"学号：{top_studentId}\t平均分：{top_average}");


                // 6. 文件操作
                Console.WriteLine("\n6. 数据持久化演示:");
                // TODO: 保存和读取学生文件
                string studentDataPath = "./studentData.csv";
                // 保存
                dataManager.SaveStudentsToFile(studentManager.GetAll(), studentDataPath);
                // 读取
                List<Student> students_load = new();
                students_load = dataManager.LoadStudentsFromFile(studentDataPath);
                foreach (Student student in students_load)
                {
                    Console.WriteLine(student.ToString());
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