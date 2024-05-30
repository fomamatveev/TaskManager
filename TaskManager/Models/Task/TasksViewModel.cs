namespace TaskManager.Models.Task
{
    public class TasksViewModel
    {
        public List<Domain.Task> Tasks { get; }

        public TasksViewModel(List<Domain.Task> tasks)
        {
            Tasks = tasks;
        }
    }
}
