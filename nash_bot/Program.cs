using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RedditSharp;
using RedditSharp.Things;

namespace nash_bot
{
	class Program
	{
		static async Task Main(string[] args)
		{
			dynamic d;

			var directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			using (StreamReader r = new StreamReader($"{directory}/credentials.json"))
			{
				var json = r.ReadToEnd();
				d = JObject.Parse(json);
			}

			var webAgent = new BotWebAgent(d.username.ToString(), d.password.ToString(), d.clientID.ToString(), d.clientSecret.ToString(), d.redirectURI.ToString());

			var reddit = new Reddit(webAgent, false);
			var subreddit = await reddit.GetSubredditAsync("/r/nashville");

			var commentsStream = subreddit.GetComments().Stream();

			var observer = new CommentObserver();

			commentsStream.Subscribe(observer);

			var cancellationToken = new CancellationToken();

			await commentsStream.Enumerate(cancellationToken);
		}

		public class CommentObserver : IObserver<Comment>
		{
			public void OnCompleted()
			{
			}

			public void OnError(Exception error)
			{
			}

			public void OnNext(Comment value)
			{
				Console.WriteLine($"{DateTime.Now} new comment by {value.AuthorName} in /r/nashville:{Environment.NewLine}{value.Body}");
			}
		}
	}
}