using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using System.Linq;

namespace AzureFunctionApps
{
    public static class TodoApi
    {
        [FunctionName("CreateTodo")]
        public static async Task<IActionResult> CreateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "ToDo")] HttpRequest req,
            [Table("todos", Connection = "AzureWebJobsStorage")] IAsyncCollector<TodoTableEntity> todoTable,
            ILogger logger)
        {
            logger.LogInformation("Creating a new ToDo list");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<TodoCreateModel>(requestBody);

            var toDo = new Todo() { TaskDescription = input.TaskDescription };
            await todoTable.AddAsync(toDo.ToTableEntity());

            return new OkObjectResult(toDo);
        }

        [FunctionName("GetTodos")]
        public static async Task<IActionResult> GetTodos(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo")] HttpRequest req,
            [Table("todos", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            ILogger log)
        {
            log.LogInformation("Getting todo list items");
            var query = new TableQuery<TodoTableEntity>();
            var segment = await todoTable.ExecuteQuerySegmentedAsync(query, null);
            return new OkObjectResult(segment.Select(Mappings.ToTodo));
        }

        [FunctionName("GetToDoById")]
        public static IActionResult GetTodoById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ToDo/{id}")] HttpRequest req, string id, ILogger logger)
        {
            logger.LogInformation("getting the ToDo list");

            //var todo = items.FirstOrDefault(t => t.Id == id);
            //if (todo == null)
            //{
            //    return new NotFoundResult();
            //}
            return new OkResult();
        }

        [FunctionName("UpdateToDo")]
        public static async Task<IActionResult> UpdateTodo([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "ToDo/{id}")] HttpRequest req, string id, ILogger logger)
        {
            object todo = null;
            //var todo = items.FirstOrDefault(x => x.Id == id);
            if (todo == null)
            {
                return new NotFoundResult();
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updated = JsonConvert.DeserializeObject<TodoUpdateModel>(requestBody);

            //todo.IsCompleted = updated.IsCompleted;
            //if (!string.IsNullOrEmpty(updated.TaskDescription))
            //{
            //    todo.TaskDescription = updated.TaskDescription;
            //}

            return new OkObjectResult(todo);
        }

        [FunctionName("DeleteToDo")]
        public static IActionResult DeleteTodo([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "ToDo/{id}")] HttpRequest req, string id, ILogger logger)
        {
            //var todo = items.FirstOrDefault(x => x.Id == id);

            //if (todo == null)
            //{
            //    return new NotFoundResult();
            //}
            //items.Remove(todo);
            return new OkResult();
        }
    }
}
