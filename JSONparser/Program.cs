using System;
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
        private const string AskSendMail = "Отправить письмо? (y/n)";
        private const string AskSaveFile = "Сохранить на диск? (y/n)";
       
        private const int NumOfLastPosts = 5;

        private static UserInfo UserInfo = new UserInfo(BaseUrl, NumOfLastPosts);

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

                var username = UserInfo.GetUsername(userId);

                if (string.IsNullOrEmpty(username))
                    continue;

                var CompletedWorkTask = Task.Run(() => UserInfo.GetCompletedWork(userId, WorkResource));
                var LastPostsTask = Task.Run(() => UserInfo.GetLastPosts(userId, PostsResource));

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

        static void SendMail(string messageBody)
        {
            Console.WriteLine("Письмо как будто отправлено...");
        }
    } 
}
