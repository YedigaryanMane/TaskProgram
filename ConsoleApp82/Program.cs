using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp82
{
    public abstract class Task
    {      

        public int TaskId { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }
    class HighPriorityTask : Task
    {
       
    }
    class LowPriorityTask : Task
    {
       
        public LowPriorityTask()
        {
            TaskManager taskManager = new TaskManager();
        }
    }
    public delegate void TaskUpdatedEventHandler(Task task);
    class TaskManager : INotifiable
    {
        public event TaskUpdatedEventHandler taskUpdated;
        public void Subscribe(TaskUpdatedEventHandler taskUpdatedEventHandler)
        {
            if (taskUpdatedEventHandler != null)
            {
                taskUpdated += taskUpdatedEventHandler;
                SendNotification("Task is subscribed successfully");
                return;
            }
            SendNotification("Task is subscribed unsuccessfully");
        }
        public void UnSubscribe(TaskUpdatedEventHandler taskUpdatedEventHandler)
        {
            if (taskUpdatedEventHandler != null)
            {
                taskUpdated -= taskUpdatedEventHandler;
                SendNotification("Task is unsubscribed successfully");
                return;
            }
            SendNotification("Task is subscribed successfully");
        }
        public void InvokeDelegate(Task task)
        {
            taskUpdated.Invoke(task);
        }
        public void SendNotification(string message)
        {
            Console.WriteLine($"TaskMeneger: {message}");
        }
    }
    interface INotifiable
    {
        void SendNotification(string message);
    }
    interface ITaskRepository
    {
        void Add(Task task);
        void Remove(int id);
        void Update(Task task);
        List<Task> GetAll();
        Task GetById(int id);
    }
    public class TaskRepository : ITaskRepository
    {
        public const string CONNECTION_STRING = "Data Source=.;Initial Catalog=TMSDb;Integrated Security=True;Encrypt=False";
        public void Add(Task task)
        {
            using (SqlConnection conn = new SqlConnection(CONNECTION_STRING))
            {
                conn.Open();
                using (SqlCommand com = new SqlCommand())
                {
                    com.Connection = conn;
                    com.CommandText = "Insert into Tasks values(@TaskID,@Description, @Status)";
                    com.Parameters.Add(new SqlParameter("@TaskID", task.TaskId));
                    com.Parameters.Add(new SqlParameter("@Description", task.Description));
                    com.Parameters.Add(new SqlParameter("@Status", task.Status));
                    com.ExecuteNonQuery();
                }
            }
        }
        public List<Task> GetAll()
        {
            using (SqlConnection conn = new SqlConnection(CONNECTION_STRING))
            {
                conn.Open();
                List<Task> list = new List<Task>();
                using (SqlCommand com = new SqlCommand())
                {
                    com.Connection = conn;
                    com.CommandText = "Select * from Tasks";
                    using (SqlDataReader reader = com.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Task task = null;
                            var status = reader["@Status"].ToString();
                            var taskId = int.Parse(reader["@TaskID"].ToString());
                            var descrition = reader["@Description"].ToString();
                            if (status.ToLower() == "lowprioritytask")
                            {
                                task = new LowPriorityTask();
                            }
                            else
                            {
                                task = new HighPriorityTask();
                            }
                            task.TaskId = taskId;
                            task.Description = descrition;
                            task.Status = status;
                            list.Add(task);
                        }
                    }
                }
                return list;
            }
        }
        public Task GetById(int id)
        {
            using (SqlConnection conn = new SqlConnection(CONNECTION_STRING))
            {
                conn.Open();
                Task task = null;
                using (SqlCommand com = new SqlCommand())
                {
                    com.Connection = conn;
                    com.CommandText = "Select * from Tasks where @TaskID = id";
                    com.Parameters.Add(new SqlParameter("@TaskID", id));
                    using (SqlDataReader reader = com.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var status = reader["@Status"].ToString();
                            var taskId = int.Parse(reader["@TaskID"].ToString());
                            var descrition = reader["@Description"].ToString();
                            if (status.ToLower() == "lowprioritytask")
                            {
                                task = new LowPriorityTask();
                            }
                            else
                            {
                                task = new HighPriorityTask();
                            }
                            task.TaskId = taskId;
                            task.Description = descrition;
                            task.Status = status;
                        }
                    }
                }
                return task;
            }
        }
        public void Remove(int id)
        {
            using (SqlConnection con = new SqlConnection(CONNECTION_STRING))
            {
                con.Open();
                using (SqlCommand com = new SqlCommand())
                {
                    com.Connection = con;
                    com.CommandText = "Delete from Tasks where id = @TaskID";
                    com.Parameters.Add(new SqlParameter("@TaskID", id));
                    com.ExecuteNonQuery();
                }
            }
        }
        public void Update(Task task)
        {
            using (SqlConnection con = new SqlConnection(CONNECTION_STRING))
            {
                con.Open();

                using (SqlCommand com = new SqlCommand())
                {
                    com.Connection = con;
                    com.CommandText = "Update Tasks set TaskId = @TaskID,Status = @Status,Description = @Description";

                    com.CommandText = "Insert into Tasks values(@TaskID,@Description, @Status)";
                    com.Parameters.Add(new SqlParameter("@TaskID", task.TaskId));
                    com.Parameters.Add(new SqlParameter("@Description", task.Description));
                    com.Parameters.Add(new SqlParameter("@Status", task.Status));

                    com.ExecuteNonQuery();
                }
            }           
        }
        public void SendNotification(string message)
        {
            Console.WriteLine(message);
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
          
            


        }
    }
}


