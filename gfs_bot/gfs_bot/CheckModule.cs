using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Discord.Commands;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Google;

namespace gfs_bot
{
	public class CheckModule : ModuleBase<SocketCommandContext>
	{
		[Command("check")]
		[Summary("Checks whether Gamefromscratch has ever covered something.")]
		public async Task CheckAsync([Remainder][Summary("The thing to check")] string query)
		{
			string message = "";

			try
			{
				var result = GetVideo(query);

				YoutubeVideo video = result.GetAwaiter().GetResult();
				// Check if the video is actually relevant to the result
				string[] words = query.ToLower().Split(' ');
				bool relevant = true;
				foreach (string word in words)
				{
					if (!video.Title.ToLower().Contains(word))
					{
						relevant = false;
					}
				}

				if (relevant)
				{
					message = "Yes! Here's a video: https://www.youtube.com/watch?v=" + video.Id + "\nIt was posted on " + video.Date.Day + "-" + video.Date.Month + "-" + video.Date.Year;
				}
				else
				{
					message = "I could not find a video about that.";
				}
			}
			catch (GoogleApiException ex)
			{
				Console.WriteLine("An error has occured:" + ex.Message);
				message = "An error has occured: ```\n" + ex.Message + "```";
			}
			catch (NoVidFoundException)
			{
				message = "I could not find a video about that.";
			}


			await ReplyAsync(message);
		}

		async public Task<YoutubeVideo> GetVideo(string query)
		{
			List<YoutubeVideo> videos = new List<YoutubeVideo>();

			using (var youtubeService = new YouTubeService(new BaseClientService.Initializer() { ApiKey = Program.config.Api }))
			{
				var searchListRequest = youtubeService.Search.List("snippet");
				searchListRequest.Q = query;
				searchListRequest.MaxResults = 5;
				// Gamefromscratch's channel ID. Might not be a good idea to hardcode it, but whatever.
				searchListRequest.ChannelId = "UCr-5TdGkKszdbboXXsFZJTQ";
				searchListRequest.Type = "video";
				searchListRequest.Order = SearchResource.ListRequest.OrderEnum.Relevance;

				var searchListResponse = await searchListRequest.ExecuteAsync();
				
				foreach (var responseVideo in searchListResponse.Items)
				{
					videos.Add(new YoutubeVideo()
					{
						Id = responseVideo.Id.VideoId,
						Title = responseVideo.Snippet.Title,
						Date = DateTime.Parse(responseVideo.Snippet.PublishedAt, null, System.Globalization.DateTimeStyles.RoundtripKind)
					});
				}
			}

			if (videos.Count == 0)
			{
				throw(new NoVidFoundException());
			}

			return videos[0];
		}
	}

	public class YoutubeVideo
	{
		public string Id { get; set; }
		public string Title { get; set; }
		public DateTime Date { get; set; }
	}

	public class NoVidFoundException : Exception
	{

	}
}
