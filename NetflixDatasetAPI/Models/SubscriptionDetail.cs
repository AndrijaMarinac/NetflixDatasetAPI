namespace NetflixDatasetAPI.Models
{
    public class SubscriptionDetail
    {
        public int NetflixUserId { get; set; }
        public string SubscriptionType { get; set; }
        public int MontlyRevenue { get; set; }
        public DateTime JoinDate { get; set; }
        public DateTime LastPayment { get; set; }
        public int PlanDurationInDays { get; set; }
    }
}
