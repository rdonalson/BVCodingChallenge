using Newtonsoft.Json;
using System.Net;
using System.Collections.Generic;
using System;
using System.Linq;

namespace BVCodingChallenge
{
    class Program
    {
        /// <summary>
        /// * The program must be command-line executable.
        /// * The program retrieves an array of User objects, each containing a user id and integer age, by making a `GET` request to the following endpoint & path:
        /// ```shell
        /// ENDPOINT = https://tupleschallenge.blob.core.windows.net
        /// PATH= /interview/age_data.json
        ///  ```
        /// * The program accounts for any bad data encountered while processing.
        /// * The program outputs a list of tuples, where each tuple contains a distinct age and the count of users with that age, delimited by a comma. (A touple in Node is an array of known length, whose values are of a known type.)
        /// * The program should run as fast as possible on a modern multi-core processor.
        /// * The program should be tested for all edges.
        /// * Test coverage should be >80%.
        /// </summary>
        /// <param name="args">string[]</param>
        static void Main(string[] args)
        {
            // Variables
            string nonalphanumeric = @"[^A-Za-z0-9]";
            string numeric = @"[^\d*]";
            UserContainer result;
            List<User> invalidUsers = new List<User>();
            List<ValidatedUser> validatedUsers = new List<ValidatedUser>();
            Console.WriteLine("B & V Coding Challenge!");

            // Get json data from website
            using (WebClient client = new WebClient())
            {
                string url = @"https://tupleschallenge.blob.core.windows.net/interview/age_data.json";
                result = JsonConvert.DeserializeObject<UserContainer>(client.DownloadString(url));
            }

            // Validate the input data file
            foreach (User user in result.Users)
            {
                if (!IsValidRegex(user.Id, nonalphanumeric))
                {
                    invalidUsers.Add(user);
                }
                else if (user.Id.Length != 24)
                {
                    invalidUsers.Add(user);
                }
                else if (string.IsNullOrEmpty(user.Age))
                {
                    invalidUsers.Add(user);
                }
                else if (!IsValidRegex(user.Age, numeric))
                {
                    invalidUsers.Add(user);
                }
                else if (Convert.ToInt16(user.Age) <= 0 || Convert.ToInt16(user.Age) > 120)
                {
                    invalidUsers.Add(user);
                }
                else {
                    validatedUsers.Add(new ValidatedUser { Id = user.Id, Age = Convert.ToInt16(user.Age) });
                }
            }

            // Summarize the Validated Users -> Get the Count of each age in the list
            List<Result> results = validatedUsers
                .GroupBy(p => p.Age,
                             (k, c) => new Result()
                             {
                                 Age = k,
                                 Count = c.Count()
                             }
                        ).ToList();

            // Essentially stringify the results for converstion to Json
            var obj = new Wrapper() { Summary = new List<JsonResult>() };
            foreach (var item in results)
            {
                var a = new JsonResult() { Age = item.Age.ToString(),  Count = item.Count.ToString() };
                obj.Summary.Add(a);
            }
            // Convert the Summarized results to a Json File that can be output
            var json = JsonConvert.SerializeObject(obj);
            Console.WriteLine(json);
        }

        /// <summary>
        ///     Validate String value versus Regex Patterns
        /// </summary>
        /// <param name="inputValue">string: value to evaluated</param>
        /// <param name="pattern">string: Pattern to be used</param>
        /// <returns></returns>
        private static bool IsValidRegex(string inputValue, string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern)) return false;
            try
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(inputValue, pattern))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }


    /// <summary>
    ///     Helper Classes
    /// </summary>
    public class Wrapper
    {
        [JsonProperty("Summaries")]
        public List<JsonResult> Summary { get; set; }
    }
    public class Result
    {
        public int Age;
        public int Count;
    }
    public class JsonResult
    {
        public string Age;
        public string Count;
    }
    public class UserContainer
    {
        public List<User> Users { set; get; }
    }
    public class User
    {
        public string Id { get; set; }
        public string Age { get; set; }
    }
    public class ValidatedUser
    {
        public string Id { get; set; }
        public int Age { get; set; }
    }

}
