namespace Branch.API.Models.Response
{
    public class LobbyModel
    {
        public List<WorkingTimeModel> Monday { get; set; }=new List<WorkingTimeModel>();
        public List<WorkingTimeModel> Tuesday { get; set; } = new List<WorkingTimeModel>();
        public List<WorkingTimeModel> Wednesday { get; set; } = new List<WorkingTimeModel>();
        public List<WorkingTimeModel> Thursday { get; set; } = new List<WorkingTimeModel>();
        public List<WorkingTimeModel> Friday { get; set; } = new List<WorkingTimeModel>();
        public List<WorkingTimeModel> Saturday { get; set; } = new List<WorkingTimeModel>();
        public List<WorkingTimeModel> Sunday { get; set; } = new List<WorkingTimeModel>();
    }
}
