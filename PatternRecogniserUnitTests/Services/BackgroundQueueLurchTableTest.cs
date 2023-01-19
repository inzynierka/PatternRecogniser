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
        private readonly ItrainingInfoService infoService;

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
            backgroundTaskQueue.Enqueue(info).Wait();
            Assert.IsNotNull( infoService.GetAsync(info.id).Result);
        }

        [TestMethod]
        public void DequeueTest()
        {
            var info = Helper.CreateSimpleTrainingInfo("DequeueTest", "");
            backgroundTaskQueue.Enqueue(info).Wait();
            var itemFromQueue = backgroundTaskQueue.Dequeue(new System.Threading.CancellationToken()).Result;
            Assert.IsNotNull(itemFromQueue);
            Assert.AreEqual(itemFromQueue.login, info.login);
            Assert.IsNull(infoService.GetAsync(itemFromQueue.id).Result);

        }
    }
}
