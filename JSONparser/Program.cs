using System;
using JSONparser.Models;
using RestSharp;
using System.Collections.Generic;
using System.Linq;

namespace JSONparser
{
    class Program
    {
        static RestClient client = new RestClient("https://jsonplaceholder.typicode.com");
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Введите id пользователя:");
                var input = Console.ReadLine();

                if (!int.TryParse(input, out int userId))
                {
                    Console.WriteLine("Некорректный id пользователя. Попробуйте еще раз.");
                    continue;
                }

                var usersResponse = GetData<User>($"users//{userId}");

                if (usersResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine("Пользователь с таким id не найден. Попробуйте еще раз.");
                    continue;
                }

                var username = usersResponse.Data.username;

                var tasksResponse = GetData<List<Task>>("todos");
                var completedTasks = tasksResponse.Data.Where(t => t.userId == userId && t.completed);

                var postsResponse = GetData<List<Post>>("posts"); 
                var lastPosts = postsResponse.Data.Where(p => p.userId == userId).TakeLast(5);

                var tasks = string.Join('\n', completedTasks.Select(t => t.title));
                var posts = string.Join('\n', lastPosts.Select(p=> p.title));

                var output = $"Уважаемый {username}, ниже представлен список ваших действий за последнее время.\n" +
                    $"Выполнено задач:\n{tasks}\n" +
                    $"Написано постов:\n{posts}";

                Console.WriteLine(output);
                Console.ReadLine();

                break;
            }

            Console.WriteLine("Hello World!");
        }

        static IRestResponse<T> GetData<T> (string resource)
        {
            var request = new RestRequest(resource);
            return client.Execute<T>(request);
        }
    }
}
