using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public string TestedFiles { get; }
        BackgroundQueueLurchTable queue;

        public BackgroundQueueTest()
        {
            
            string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            TestedFiles = projectDirectory + "\\TestedFiles";
            string fileName = "cyfry.zip";
            string fileLocation = $"{TestedFiles}\\{fileName}";
            var file = File.OpenRead(fileLocation);
            queue = new BackgroundQueueLurchTable();
            _trainingSet = new FormFile(file, 0, file.Length, fileName, fileName);
        }

        [TestMethod()]
        public void NumberInQueueTest()
        {
            for (int i = 0; i < 10; i++)
            {
                var info = new TrainingInfo(i.ToString(), _trainingSet, i.ToString(), PatternRecogniser.Models.DistributionType.TrainTest, 60, 5);
                queue.Enqueue(info);
            }
            
            for (int i = 0; i < 10; i++)
            {
                var item = queue.Dequeue(new System.Threading.CancellationToken());
                int nextLogin = int.Parse(item.login) + 1;
                int placeInQueue = 0;
                for (int j = nextLogin; j < 10; j++)
                {
                    Assert.AreEqual(placeInQueue, queue.NumberInQueue(nextLogin.ToString()));
                    placeInQueue++;
                    nextLogin++;
                }
            }

        }

        [TestMethod()]
        public void DequeTest()
        {
            for (int i = 0; i < 10; i++)
            {
                var info = new TrainingInfo(i.ToString(), _trainingSet, i.ToString(), PatternRecogniser.Models.DistributionType.TrainTest, 60, 5);
                queue.Enqueue(info);
            }

            

            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(i.ToString(), queue.Dequeue(new System.Threading.CancellationToken()).login);
                
            }
        }
    }
}
