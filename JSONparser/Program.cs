using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace JSONparser
{
    class Program
    {
        private const string BaseUrl = "https://jsonplaceholder.typicode.com";
        private const string WorkResource = "todos";
        private const string PostsResource = "posts";
        private const string Filename = "Список действий.txt";
       
        private const int NumOfLastPosts = 5;

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

                var UserInfo = new UserInfo(BaseUrl, userId, NumOfLastPosts);
                var username = UserInfo.GetUsername();

                if (string.IsNullOrEmpty(username))
                    continue;

                var CompletedWorkTask = Task.Run(() => UserInfo.GetCompletedWork(WorkResource));
                var LastPostsTask = Task.Run(() => UserInfo.GetLastPosts(PostsResource));

                if (!Task.WaitAll(new Task[] { CompletedWorkTask, LastPostsTask }, 5000))
                {
                    Console.WriteLine("Не удалось получить данные.");
                    continue;
                }

                var output = $"Уважаемый {username}, ниже представлен список ваших действий за последнее время." +
                             $"\nВыполнено задач:\n{CompletedWorkTask.Result}" +
                             $"\nНаписано постов:\n{LastPostsTask.Result}";

                Console.WriteLine(output);

                Console.WriteLine("Сохранить на диск? (y/n)");
                var answer = Console.ReadLine();
                if (answer == "y")
                    SaveToFile(output);

                break;
            }
        }

        private static void SaveToFile(string output)
        {
            var path = ConfigurationManager.AppSettings["SaveFileLocation"];
            var fullpath = Path.Combine(path, Filename);

            try
            {
                File.WriteAllText(fullpath, output);
                Console.WriteLine($"Успешно сохранено по пути: \"{fullpath}\"");
            }
            catch(UnauthorizedAccessException)
            {
                Console.WriteLine($"Невозможно сохранить файл по выбранному пути: \"{path}\". Доступ запрещен.");
            }
            catch (Exception)
            {
                throw;
            }
        }
    } 
}
