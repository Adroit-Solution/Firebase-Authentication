namespace Firebase.Authentication.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string? Uid { get; set; }
        public string?  Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public int Provider { get; set; }
    }
}
