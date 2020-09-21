using System;
using System.Collections.Generic;
using System.Linq;
using JSONparser.Models;

namespace JSONparser
{
    internal class UserInfo
    {
        private RestApiProvider RestApiProvider;
        private int NumOfLastPosts;
        private int UserId;

        public UserInfo(string BaseUrl, int userId, int LastPosts)
        {
            RestApiProvider = new RestApiProvider(BaseUrl);
            NumOfLastPosts = LastPosts;
            UserId = userId;
        }

        public string GetLastPosts(string resource)
        {
            var postsResponse = RestApiProvider.GetData<List<Post>>(resource);
            var lastPosts = postsResponse.Data.Where(p => p.userId == UserId).TakeLast(NumOfLastPosts);
            var posts = string.Join('\n', lastPosts.Select(p => p.title));
            return posts;
        }

        public string GetCompletedWork(string resource)
        {
            var tasksResponse = RestApiProvider.GetData<List<Work>>(resource);
            var completedTasks = tasksResponse.Data.Where(t => t.userId == UserId && t.completed);
            var tasks = string.Join('\n', completedTasks.Select(t => t.title));
            return tasks;
        }

        public string GetUsername()
        {
            var usersResponse = RestApiProvider.GetData<User>($"users//{UserId}");

            if (usersResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine("Пользователь с таким id не найден. Попробуйте еще раз.");
                return null;
            }

            return usersResponse.Data.username;
        }
    }
}
