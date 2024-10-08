namespace SchoolSystem.Models
{
    public class ScheduleModulePreferences
    {
        public int Id { get; set; }
        public int ModuleNumber { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsBreakTime { get; set; }
    }
}
