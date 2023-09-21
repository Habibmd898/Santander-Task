
using Moq;
using NUnit.Framework;
using Santander_Task.APIManager;
using Santander_Task.Controllers;
using Santander_Task.DataModel;

namespace TestProject1
{
    [TestClass]
    public class UnitTest
    {
        [Test]
        public void Stories_Conroller_Should_Be_Successful_When_there_Are_Best_Stories()
        {
            var mock = new Mock<IBestStoriesAPI>();
            IEnumerable<Story> stories = new List<Story> { new Story() { By = "me", Title = "title" } };
            mock.Setup(s => s.GetBestStories(10)).Returns((Task<IEnumerable<Story>>)stories);
            var request = new StoriesController(mock.Object).Best(10);
            NUnit.Framework.Assert.That(request.IsCompletedSuccessfully, Is.EqualTo(true));
        }
    }
}
