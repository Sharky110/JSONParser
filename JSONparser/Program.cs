using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JSONparser.Models;

namespace JSONparser
{
    class Program
    {
        private const string BaseUrl = "https://jsonplaceholder.typicode.com";
        private const string WorkResource = "todos";
        private const string PostsResource = "posts";
        private const string Filename = "Список действий.txt";
        private const string AskSendMail = "Отправить письмо? (y/n)";
        private const string AskSaveFile = "Сохранить на диск? (y/n)";
       
        private const int NumOfLastPosts = 5;

        private static RestApiProvider RestApiProvider = new RestApiProvider(BaseUrl);

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

                var username = GetUsername(userId);

                if (string.IsNullOrEmpty(username))
                    continue;

                var CompletedWorkTask = Task.Run(() => GetCompletedWork(userId, WorkResource));
                var LastPostsTask = Task.Run(() => GetLastPosts(userId, PostsResource));

                if (!Task.WaitAll(new Task[] { CompletedWorkTask, LastPostsTask }, 5000))
                {
                    Console.WriteLine("Не удалось получить данные.");
                    continue;
                }

                var output = $"Уважаемый {username}, ниже представлен список ваших действий за последнее время." +
                             $"\nВыполнено задач:\n{CompletedWorkTask.Result}" +
                             $"\nНаписано постов:\n{LastPostsTask.Result}";

                Console.WriteLine(output);

                AskQuestion(SendMail, AskSendMail, output);
                AskQuestion(SaveToFile, AskSaveFile, output);

                break;
            }
        }

        private static void AskQuestion(Action<string> action, string question, string output)
        {
            Console.WriteLine(question);
            var answer = Console.ReadLine();
            if (answer == "y")
                action(output);
        }

        private static void SaveToFile(string output)
        {
            File.WriteAllText(Filename, output);
        }

        private static string GetLastPosts(int userId, string resource)
        {
            var postsResponse = RestApiProvider.GetData<List<Post>>(resource);
            var lastPosts = postsResponse.Data.Where(p => p.userId == userId).TakeLast(NumOfLastPosts);
            var posts = string.Join('\n', lastPosts.Select(p => p.title));
            return posts;
        }

        private static string GetCompletedWork(int userId, string resource)
        {
            var tasksResponse = RestApiProvider.GetData<List<Work>>(resource);
            var completedTasks = tasksResponse.Data.Where(t => t.userId == userId && t.completed);
            var tasks = string.Join('\n', completedTasks.Select(t => t.title));
            return tasks;
        }

        private static string GetUsername(int userId)
        {
            var usersResponse = RestApiProvider.GetData<User>($"users//{userId}");

            if (usersResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine("Пользователь с таким id не найден. Попробуйте еще раз.");
                return null;
            }

            return usersResponse.Data.username;
        }

        static void SendMail(string messageBody)
        {
            Console.WriteLine("Письмо как будто отправлено...");
        }
    } 
}
