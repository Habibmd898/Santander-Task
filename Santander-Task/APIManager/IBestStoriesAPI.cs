using Santander_Task.DataModel;

namespace Santander_Task.APIManager
{
    public interface IBestStoriesAPI
    {
        Task<IEnumerable<Story>> GetBestStories(int number);
    }

}
