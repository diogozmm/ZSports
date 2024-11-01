namespace ZSports.Domain.User
{
    public class User : BaseEntity<Guid>
    {
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public Guid? KeycloakId { get; set; }
    }
}
