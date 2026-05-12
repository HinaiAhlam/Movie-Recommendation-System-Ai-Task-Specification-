namespace project.Models
{
    public abstract class Person
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        protected Person(int id, string username, string password)
        {
            Id = id;
            Username = username;
            Password = password;
        }
    }
}