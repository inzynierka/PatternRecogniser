using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PatternRecogniser.Models;
using PatternRecogniser.Services;
using PatternRecogniser.ThreadsComunication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternRecogniserUnitTests.ThreadsComunication
{
    [TestClass()]
    public class BackgroundQueueTest
    {
        private IFormFile _trainingSet;
        private ITrainingUpdate _trainingUpdate;
        private ItrainingInfoService trainingInfoService;
        public string TestedFiles { get; }
        BackgroundQueueLurchTable queue;

        public BackgroundQueueTest()
        {
            
            string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            TestedFiles = projectDirectory + "\\TestedFiles";
            string fileName = "cyfry.zip";
            string fileLocation = $"{TestedFiles}\\{fileName}";
            var file = File.OpenRead(fileLocation);
            trainingInfoService = Helper.CreateTrainingInfoMongoCollection();
            trainingInfoService.ClearTrainingInfoTestDB();
            queue = new BackgroundQueueLurchTable(trainingInfoService);
            _trainingSet = new FormFile(file, 0, file.Length, fileName, fileName);
        }

        [TestMethod()]
        public void NumberInQueueTest()
        {

            for (int i = 0; i < 5; i++)
            {
                var info = new TrainingInfo(i.ToString(), _trainingSet, i.ToString(), PatternRecogniser.Models.DistributionType.TrainTest, 60, 5);
                queue.Enqueue(info).Wait();
            }
            
            for (int i = 0; i < 5; i++)
            {
                var item = queue.Dequeue(new System.Threading.CancellationToken()).Result;
                int nextLogin = int.Parse(item.login) + 1;
                int placeInQueue = 0;
                for (int j = nextLogin; j < 5; j++)
                {
                    Assert.AreEqual(placeInQueue, queue.NumberInQueue(nextLogin.ToString()));
                    placeInQueue++;
                    nextLogin++;
                }
            }

        }


        [TestMethod()]
        public void RemoveTest()
        {
            for (int i = 0; i < 10; i++)
            {
                var info = new TrainingInfo(i.ToString(), _trainingSet, i.ToString(), PatternRecogniser.Models.DistributionType.TrainTest, 60, 5);
                queue.Enqueue(info).Wait();
            }

            queue.Remove("0").Wait();
            queue.Remove("5").Wait();
            queue.Remove("9").Wait();
            string[] loginsInQueue = new string[] {"1", "2","3","4", "6", "7", "8"};
            string[] loginsOutOfQueue = new string[] { "0", "5", "9" };

            for (int i = 0; i < 7; i++)
            {
                int placeInQueue = 0;
                for (int j = i; j < 7; j++)
                {
                    Assert.AreEqual(placeInQueue, queue.NumberInQueue(loginsInQueue[j]));
                    placeInQueue++;
                }
                var item =  queue.Dequeue(new System.Threading.CancellationToken()).Result;
                Assert.AreEqual(item.login, loginsInQueue[i]);
                
            }

            foreach(var login in loginsOutOfQueue)
            {
                Assert.IsFalse( queue.IsUsersModelInQueue(login));
            }

        }

        [TestMethod()]
        public void DequeueTest()
        {
            for (int i = 0; i < 10; i++)
            {
                var info = new TrainingInfo(i.ToString(), _trainingSet, i.ToString(), PatternRecogniser.Models.DistributionType.TrainTest, 60, 5);
                queue.Enqueue(info).Wait();
            }

            

            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(i.ToString(), queue.Dequeue(new System.Threading.CancellationToken()).Result.login);
                
            }
        }
    }
}
