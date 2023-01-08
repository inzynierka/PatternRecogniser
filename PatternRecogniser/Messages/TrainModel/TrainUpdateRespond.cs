namespace PatternRecogniser.Messages.TrainModel
{
    public class TrainUpdateRespond
    {
        public string log { get; set; }
        public string status { get; set; }
        public TrainUpdateRespond(string log, string status)
        {
            this.log = log;
            this.status = status;
        }
    }
}
