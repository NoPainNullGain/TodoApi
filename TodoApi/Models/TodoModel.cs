using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Models
{
    public class TodoModel
    {
        public int Id { get; set; }
        public string TodoName { get; set; }
        public bool IsComplete { get; set; }
    }
}
