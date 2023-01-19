using Microsoft.VisualStudio.TestTools.UnitTesting;
using PatternRecogniser.Services;
using PatternRecogniser.ThreadsComunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternRecogniserUnitTests.Controllers.Services
{
    [TestClass]
    public class BackgroundQueueLurchTableTest
    {
        IBackgroundTaskQueue backgroundTaskQueue;
        private readonly trainingInfoService infoService;

        public BackgroundQueueLurchTableTest()
        {
            infoService = Helper.CreateTrainingInfoMongoCollection();
            infoService.ClearTrainingInfoTestDB();
            backgroundTaskQueue = new BackgroundQueueLurchTable(infoService);
        }

    [TestMethod]
        public void EnqueueTest()
        {
            var info = Helper.CreateSimpleTrainingInfo("EnqueueTest", "");
            infoService.RemoveAsync(info.login).Wait();
            backgroundTaskQueue.Enqueue(info);
            Assert.IsNotNull( infoService.GetAsync(info.login).Result);
            infoService.ClearTrainingInfoTestDB();
        }

        [TestMethod]
        public void DequeueTest()
        {
            var info = Helper.CreateSimpleTrainingInfo("DequeueTest", "");
            infoService.RemoveAsync(info.login).Wait();
            backgroundTaskQueue.Enqueue(info);
            var itemFromQueue = backgroundTaskQueue.Dequeue(new System.Threading.CancellationToken());
            Assert.IsNotNull(itemFromQueue);
            Assert.AreEqual(itemFromQueue.login, info.login);
            Assert.IsNull(infoService.GetAsync(itemFromQueue.login).Result);

        }
    }
}
