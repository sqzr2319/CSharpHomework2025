namespace StudentManagementSystem
{
    public enum Grade
    {
        F = 0,
        D = 60,
        C = 70,
        B = 80,
        A = 90
    }

    public interface IRepository<T>
    {
        public void Add(T item);
        public bool Remove(T item);
        public List<T> GetAll();
        public List<T> Find(Func<T, bool> predicate);
    }

    public class Student : IComparable<Student>
    {
        public string StudentId { get; private set; }
        public string Name { get; private set; }
        public int Age { get; private set; }

        public Student(string studentId, string name, int age)
        {
            this.StudentId = studentId ?? throw new ArgumentNullException(nameof(studentId), "学号不能为空");
            this.Name = name ?? throw new ArgumentNullException(nameof(name), "姓名不能为空");
            this.Age = age > 0 ? age : throw new ArgumentOutOfRangeException(nameof(age), "年龄必须大于0");
        }

        public override string ToString()
        {
            return $"学号: {StudentId}, 姓名: {Name}, 年龄: {Age}";
        }

        public int CompareTo(Student? other)
        {
            if (other == null) return 1;
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

    public class Score
    {
        public string Subject { get; private set; }
        public double Points { get; private set; }
        
        public Score(string subject, double points)
        {
            this.Subject = subject ?? throw new ArgumentNullException(nameof(subject), "科目不能为空");
            this.Points = points >= 0 ? points : throw new ArgumentOutOfRangeException(nameof(points), "分数不能为负");
        }

        public override string ToString()
        {
            return $"科目: {Subject}, 分数: {Points}";
        }
    }

    public class StudentManager : IRepository<Student>
    {
        private List<Student> students = new List<Student>();

        public void Add(Student item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item), "学生信息不能为空");
            if (students.Contains(item)) throw new ArgumentException("该学生已存在", nameof(item));

            students.Add(item);
        }

        public bool Remove(Student item)
        {
            if (item == null) return false;
            return students.Remove(item);
        }

        public List<Student> GetAll()
        {
            return new List<Student>(students);
        }

        public List<Student> Find(Func<Student, bool> predicate)
        {
            var result = new List<Student>();
            foreach (var student in students)
            {
                if (predicate(student))
                {
                    result.Add(student);
                }
            }
            return result;
        }

        public List<Student> GetStudentsByAge(int minAge, int maxAge)
        {
            var result = new List<Student>();
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

    public class ScoreManager
    {
        private Dictionary<string, List<Score>> scores = new Dictionary<string, List<Score>>();

        public void AddScore(string studentId, Score score)
        {
            if (string.IsNullOrEmpty(studentId)) throw new ArgumentNullException(nameof(studentId), "学号不能为空");
            if (score == null) throw new ArgumentNullException(nameof(score), "成绩信息不能为空");

            if (!scores.TryGetValue(studentId, out var scoreList))
            {
                scoreList = new List<Score>();
                scores[studentId] = scoreList;
            }

            scoreList.Add(score);
        }

        public List<Score> GetStudentScores(string studentId)
        {
            if (scores.TryGetValue(studentId, out var scoreList))
            {
                return scoreList;
            }

            return new List<Score>();
        }

        public double CalculateAverage(string studentId)
        {
            if (scores.TryGetValue(studentId, out var scoreList) && scoreList.Count > 0)
            {
                double total = 0;
                foreach (var score in scoreList)
                {
                    total += score.Points;
                }
                return total / scoreList.Count;
            }
            return 0;
        }

        public Grade GetGrade(double score)
        {
            switch (score)
            {
                case var s when s >= 90:
                    return Grade.A;
                case var s when s >= 80:
                    return Grade.B;
                case var s when s >= 70:
                    return Grade.C;
                case var s when s >= 60:
                    return Grade.D;
                default:
                    return Grade.F;
            }
        }

        public List<(string StudentId, double Average)> GetTopStudents(int count)
        {
            var topStudents = new List<(string StudentId, double Average)>();

            foreach (var studentId in scores.Keys)
            {
                var average = CalculateAverage(studentId);
                topStudents.Add((studentId, average));
            }

            return topStudents.OrderByDescending(s => s.Average).Take(count).ToList();
        }

        public Dictionary<string, List<Score>> GetAllScores()
        {
            return new Dictionary<string, List<Score>>(scores);
        }
    }

    public class DataManager
    {
        public void SaveStudentsToFile(List<Student> students, string filePath)
        {
            try
            {
                using (var writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("学号,姓名,年龄");
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
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    string? line;
                    bool isFirstLine = true;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (isFirstLine)
                        {
                            isFirstLine = false;
                            continue;
                        }
                        var parts = line.Split(',');
                        if (parts.Length == 3 &&
                        !string.IsNullOrWhiteSpace(parts[0]) &&
                        !string.IsNullOrWhiteSpace(parts[1]) &&
                        int.TryParse(parts[2], out int age))
                        {
                            var student = new Student(parts[0], parts[1], age);
                            students.Add(student);
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

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== 学生成绩管理系统 ===\n");

            var studentManager = new StudentManager();
            var scoreManager = new ScoreManager();
            var dataManager = new DataManager();

            try
            {
                Console.WriteLine("1. 添加学生信息:");
                studentManager.Add(new Student("2021001", "张三", 20));
                studentManager.Add(new Student("2021002", "李四", 19));
                studentManager.Add(new Student("2021003", "王五", 21));
                Console.WriteLine("学生信息添加完成");

                Console.WriteLine("\n2. 添加成绩信息:");
                scoreManager.AddScore("2021001", new Score("数学", 95.5));
                scoreManager.AddScore("2021001", new Score("英语", 87.0));
                scoreManager.AddScore("2021002", new Score("数学", 78.5));
                scoreManager.AddScore("2021002", new Score("英语", 85.5));
                scoreManager.AddScore("2021003", new Score("数学", 88.0));
                scoreManager.AddScore("2021003", new Score("英语", 92.0));
                Console.WriteLine("成绩信息添加完成");

                Console.WriteLine("\n3. 查找年龄在19-20岁的学生:");
                var ageFilteredStudents = studentManager.GetStudentsByAge(19, 20);
                foreach (var student in ageFilteredStudents)
                {
                    Console.WriteLine(student);
                }

                Console.WriteLine("\n4. 学生成绩统计:");
                foreach (var student in studentManager.GetAll())
                {
                    var scores = scoreManager.GetStudentScores(student.StudentId);
                    Console.WriteLine(student);
                    foreach (var score in scores)
                    {
                        Console.WriteLine(score);
                    }
                    double average = scoreManager.CalculateAverage(student.StudentId);
                    Grade grade = scoreManager.GetGrade(average);
                    Console.WriteLine($"平均分: {average}, 等级: {grade}");
                }

                Console.WriteLine("\n5. 平均分最高的学生:");
                var topStudents = scoreManager.GetTopStudents(1);
                if (topStudents.Count > 0)
                {
                    var student = topStudents[0];
                    Console.WriteLine($"学号: {student.StudentId}, 平均分: {student.Average}");
                }

                Console.WriteLine("\n6. 数据持久化演示:");
                string studentFilePath = "students.csv";
                dataManager.SaveStudentsToFile(studentManager.GetAll(), studentFilePath);
                Console.WriteLine($"学生信息已保存到 {studentFilePath}");
                var loadedStudents = dataManager.LoadStudentsFromFile(studentFilePath);
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