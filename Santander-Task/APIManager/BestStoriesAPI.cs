using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Santander_Task.DataModel;

namespace Santander_Task.APIManager
{
    public class BestStoriesAPI : IBestStoriesAPI
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string StoriesUri = "https://hacker-news.firebaseio.com/v0/beststories.json";
        private const string StoryDetailsUriFormat = "https://hacker-news.firebaseio.com/v0/item/{0}.json";
        private readonly IMemoryCache _storyCache;

        public BestStoriesAPI(IHttpClientFactory httpClientFactory, IMemoryCache storyCache)
        {
            _httpClientFactory = httpClientFactory;
            _storyCache = storyCache;
        }

        public async Task<IEnumerable<Story>> GetBestStories(int number)
        {
            var httpClient = _httpClientFactory.CreateClient();
            List<Story> stories = new List<Story>();
            var batchSize = 10;
            try
            {
                // This is going to return a list of story ids
                var response = await httpClient.GetAsync(StoriesUri);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var storyIds = JsonConvert.DeserializeObject<List<int>>(data);
                    storyIds = storyIds?.Count < number ? storyIds : storyIds?.Take(number).ToList();

                    int numberOfBatches = (int)Math.Ceiling((double)storyIds.Count / batchSize);

                    for (int i = 0; i < numberOfBatches; i++)
                    {
                        var currentIds = storyIds.Skip(i * batchSize).Take(batchSize);
                        var task = currentIds.Select(x => GetStoryDetails(x, httpClient));
                        stories.AddRange(await Task.WhenAll(task));
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return stories;
        }

        public async Task<Story> GetStoryDetails(int storyId, HttpClient client)
        {
            if (_storyCache.TryGetValue(storyId, out Story? cachedStory) && cachedStory != null)
            {
                return cachedStory;
            }

            Story story = new Story();
            try
            {
                var formattedUrl = string.Format(StoryDetailsUriFormat, storyId);
                HttpResponseMessage response = await client.GetAsync(formattedUrl);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    story = JsonConvert.DeserializeObject<Story>(data) ?? throw new Exception();

                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(60))
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                    .SetPriority(CacheItemPriority.Normal)
                    .SetSize(1024);
                    _storyCache.Set(storyId, story, cacheEntryOptions);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return story;
        }

    }
}


