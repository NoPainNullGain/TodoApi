using TodoApi.Models;

namespace TodoApi.Factory
{
    public interface ITodoFactory
    {
        TodoModel Create(int id, string Name, bool isComplete);
    }
}