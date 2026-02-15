namespace CitiesManager.Core.DTO
{
    public class UserTokenRequest
    {
        public Guid UserId { get; set; }
        public string? Email { get; set; } 
        public string? PersonName { get; set; }
    }
}
