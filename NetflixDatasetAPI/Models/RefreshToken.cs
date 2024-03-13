namespace NetflixDatasetAPI.Models
{
    public class RefreshToken
    {
        public required string Token { get; set; }
        public required DateTime CreationDate { get; set; }
        public required int LifetimeInHours { get; set; }
        public required DateTime ExpirationDate { get; set; }                         
    }
}
