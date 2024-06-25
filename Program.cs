using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace Guessing_Game
{
    internal class Program
    {
        static readonly HttpClient client = new HttpClient();
        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Think of a number...");
                Console.WriteLine("What is the Lower Bound?");
                int lower = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("What is the Upper Bound?");
                int upper = Convert.ToInt32(Console.ReadLine());

                while (lower >= upper)
                {
                    Console.WriteLine("Lower bound must be less than upper!");
                    Console.WriteLine("Lower Bound: ");
                    lower = Convert.ToInt32(Console.ReadLine());

                    Console.WriteLine("Upper Bound: ");
                    upper = Convert.ToInt32(Console.ReadLine());
                }

                Console.WriteLine($"Your range is from {lower} to {upper}");
                string previousResponse = null;
                int previousGuess = 0;
                bool isGuessed = false;
                while (!isGuessed)
                {
                    var request = new GuessRequest
                    {
                        LowerBound = lower,
                        UpperBound = upper,
                        PreviousResponse = previousResponse,
                        PreviousGuess = previousGuess
                    };

                    var jsonRequest = JsonConvert.SerializeObject(request);
                    var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("https://localhost:7129/Guess", content);
                    response.EnsureSuccessStatusCode();

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var guessResponse = JsonConvert.DeserializeObject<GuessResponse>(jsonResponse);

                    Console.WriteLine(guessResponse.Message);

                    if (guessResponse.IsGuessed)
                    {
                        isGuessed = true;
                        Console.WriteLine("You Won!");
                    }
                    else
                    {
                        previousGuess = guessResponse.Guess;
                        previousResponse = Console.ReadLine().Trim();
                        lower = guessResponse.NextLowerBound;
                        upper = guessResponse.NextUpperBound;
                    }
                }

            }
            catch(Exception e) 
            { Console.WriteLine($"Error: {e.Message}"); }
        }

        public class GuessRequest
        {
            public int LowerBound { get; set; }
            public int UpperBound { get; set; }
            public string PreviousResponse { get; set; }
            public int PreviousGuess { get; set; }
        }
        public class GuessResponse
        {
            public int Guess { get; set; }
            public string Message { get; set; }
            public bool IsGuessed { get; set; }
            public int NextLowerBound { get; set; }
            public int NextUpperBound { get; set; }
        }
    }
}
