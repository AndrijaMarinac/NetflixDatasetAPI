namespace NetflixDatasetAPI.Models
{
    public class NetflixUserData
    {
        public int? NetflixUserId { get; set; }
        public string? Country { get; set; }
        public int? Age { get; set; }
        public string? Gender { get; set; }
        public string? Device { get; set; }
        public string? SubscriptionType { get; set; }
        public int? MontlyRevenue { get; set; }
        public DateTime? JoinDate { get; set; }
        public DateTime? LastPayment { get; set; }
        public int? PlanDurationInDays { get; set; }
    }
}
