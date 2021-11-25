using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Models;

namespace TodoApi.Factory
{
    public class TodoFactory : ITodoFactory
    {
        public TodoModel Create(int id, string name, bool isComplete)
        {
            if(id < 0 || name is null)
                return null;

            var model = new TodoModel();
            model.Id = id;
            model.TodoName = name;
            model.IsComplete = isComplete;

            return model;
        }

    }
}
