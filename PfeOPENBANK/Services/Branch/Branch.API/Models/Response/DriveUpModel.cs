namespace Branch.API.Models.Response
{
    public class DriveUpModel
    {
        public WorkingTimeModel Monday { get; set; }
        public WorkingTimeModel Tuesday { get; set; }
        public WorkingTimeModel Wednesday { get; set; }
        public WorkingTimeModel Thursday { get; set; }
        public WorkingTimeModel Friday { get; set; }
        public WorkingTimeModel Saturday { get; set; }
        public WorkingTimeModel Sunday { get; set; }
    }
}
