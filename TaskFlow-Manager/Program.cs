using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp14
{

    public enum Status { ToDo, InProgress, Done}
    public enum Priority
    {
        Low,
        Medium,
        High
    }

    public class Task
    {
        private static int _indexCounter = 1;
        public int TaskID { get; }
        public string Title { get; set; }
        public Employee Asignee { get; set; }
        public Status Status { get; set; }
        public Priority Priority { get; set;}
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }

        public Task(string title, Priority priority, DateTime dueDate, string description)
        {
            TaskID = _indexCounter++;
            Title = title;
            Status = Status.ToDo;
            Priority = priority;
            StartDate = DateTime.Now;
            DueDate = dueDate;
            EndDate = null;
        }

        public void MarkAsCompleted()
        {
            if (Status != Status.Done)
            {
                Status = Status.Done;
                EndDate = DateTime.Now;
                Console.WriteLine("Task marked as completed.");
            }
            else
            {
                Console.WriteLine("Task is already marked as completed.");
            }
        }
        public void ChangePriority(Priority priority)
        {
            if (Priority != priority)
            {
                Priority = priority;
                Console.WriteLine("Priority changed successfuly.");
            }
            else
            {
                Console.WriteLine("Priority of this task is same.");
            }
        }
        public void ExtendDueDate(DateTime newDueDate)
        {
            if(newDueDate > DateTime.Now)
            {
                DueDate = newDueDate;
                Console.WriteLine($"Task due date extended to: {newDueDate}");
            }
            else
            {
                Console.WriteLine("New due date must be in the future.");
            }
        }

        public bool isOverdue()
        {
            return DueDate < DateTime.Now && Status != Status.Done;
        }
        public string GetTaskDetails()
        {
            return $"TaskID: {TaskID}, Title: {Title}, Status: {Status}, Priority: {Priority}, Created On: {StartDate}, Due Date: {DueDate.ToString() ?? "No Due Date"}";
        }

    }

    public class Employee
    {
        
        private static int _nextId = 1;
        public int EmployeeID { get; private set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public List<Task> AssignedTasks { get; private set; }

        
        public Employee(string name, string position)
        {
            EmployeeID = _nextId++;
            Name = name;
            Position = position;
            AssignedTasks = new List<Task>();
        }

        public void AssignTask(Task task)
        {
            if (AssignedTasks.Count < 5 && task != null)
            {
                AssignedTasks.Add(task);
                task.Status = Status.InProgress;
                task.Asignee = this;
                Console.WriteLine("Task assigned successfuly");
            }
            else
            {
                Console.WriteLine("Invalid task or maximum number of assigned tasks.");
            }
        }
        public void ListAssignedTasks()
        {
            foreach(Task task in AssignedTasks)
            {
                Console.WriteLine(task.GetTaskDetails());
            }
        }
        public void ListPendingTasks()
        {
            Console.WriteLine($"Pending tasks for {Name}:");
            foreach (Task task in AssignedTasks)
            {
                if (task.Status != Status.Done)
                {
                    Console.WriteLine(task.GetTaskDetails());
                }
            }
        }
        public void CompleteTask(int taskID)
        {
            Task taskToComplete = AssignedTasks.Find(t => t.TaskID == taskID);
            if(taskToComplete != null)
            {
                taskToComplete.MarkAsCompleted();
            }
            else
            {
                Console.WriteLine("Cant find task with that id");
            }

        }
    }
    public class Project
    {
        public int ProjectID { get; private set; }
        public string Name { get; set; }
        public Employee AssignedEmployee { get; set; }
        public List<Task> Tasks { get; private set; }
        public DateTime Deadline { get; private set; }

        
        public Project(string name, Employee assignedEmployee, DateTime deadline)
        {
            ProjectID = new Random().Next(1000, 9999); 
            Name = name;
            AssignedEmployee = assignedEmployee;
            Deadline = deadline;
            Tasks = new List<Task>();
        }

        public void AddTask(Task task)
        {
            if (task != null && Tasks.Count < 10)
            {
                Tasks.Add(task);
                Console.WriteLine("Task added successfuly.");
            }
            else
            {
                Console.WriteLine("Task Limit Reached or invalid task");
            }
        }
        public void removeTask(int taskId)
        {
            Task taskToRemove = Tasks.Find(t => t.TaskID == taskId);
            if(taskToRemove != null)
            {
                Tasks.Remove(taskToRemove);
                Console.WriteLine("Task removed successfuly");
            }
            else
            {
                Console.WriteLine("Invalid Id");
            }
        }
        public void ListAllTasks()
        {
            foreach(Task task in Tasks)
            {
                Console.Write(task.GetTaskDetails());
            }
        }

        public void ExtendDeadline(DateTime newDeadline)
        {
            if(newDeadline > Deadline)
            {
                Deadline = newDeadline;
                Console.WriteLine("Deadline Extended successfuly");
            }
            else
            {
                Console.WriteLine("New deadline must be later than actual deadline");
            }
        }
        public bool IsOverdue()
        {
            return Deadline < DateTime.Now;
        }
    }

    public class Company
    {
        public List<Project> Projects { get; private set; }
        public List<Employee> Employees { get; private set; }

        public Company()
        {
            Projects = new List<Project>();
            Employees = new List<Employee>();
        }
        public void addEmployee(Employee employee)
        {
            Employees.Add(employee);
            Console.WriteLine("Employee added successfuly");
        }
        public void addProject(Project project)
        {
            Projects.Add(project);
            Console.WriteLine("Project added successfuly");
        }
        public void ListAllTasksByPriority(Priority priority)
        {
            foreach (Project project in Projects)
            {
                foreach (Task task in project.Tasks)
                {
                    if (task.Priority == priority)
                    {
                        Console.WriteLine(task.GetTaskDetails());
                    }
                }
            }
        }

        public void ListAllEmployeesTasks()
        {
            foreach (Employee employee in Employees)
            {
                foreach (Task task in employee.AssignedTasks)
                {
                    Console.WriteLine(task.GetTaskDetails());
                }
            }
        }
    }
    class Program
    {
            static void Main(string[] args)
            {
                Company myCompany = new Company();

          
                Employee employee1 = new Employee("Alice", "Developer");
                Employee employee2 = new Employee("Bob", "Manager");
                myCompany.addEmployee(employee1);
                myCompany.addEmployee(employee2);


                Task task1 = new Task("Develop Login Page", Priority.High, DateTime.Now.AddDays(7), "Build a secure login page.");
                Task task2 = new Task("Design Database Schema", Priority.Medium, DateTime.Now.AddDays(10), "Design the DB schema for the app.");
                Task task3 = new Task("Prepare Presentation", Priority.Low, DateTime.Now.AddDays(5), "Prepare project presentation for clients.");

       
                employee1.AssignTask(task1);
                employee2.AssignTask(task2);
                employee2.AssignTask(task3);

                
                Project project1 = new Project("Website Development", employee2, DateTime.Now.AddDays(30));
                project1.AddTask(task1);
                project1.AddTask(task2);
                project1.AddTask(task3);
                myCompany.addProject(project1);

               
                Console.WriteLine("All tasks assigned to employees:");
                myCompany.ListAllEmployeesTasks();

             
                Console.WriteLine("\nTasks with High priority:");
                myCompany.ListAllTasksByPriority(Priority.High);

                
                Console.WriteLine("\nCompleting a task:");
                employee1.CompleteTask(task1.TaskID);

       
                project1.ExtendDeadline(DateTime.Now.AddDays(40));

                Console.WriteLine($"\nPending tasks for {employee2.Name}:");
                employee2.ListPendingTasks();

                Console.WriteLine($"\nIs the project overdue? {project1.IsOverdue()}");

                Console.ReadLine();


            }
    }
            
}
