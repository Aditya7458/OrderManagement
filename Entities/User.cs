public class User
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; } 

    public User(int userId, string username, string password, string role)
    {
        UserId = userId;
        Username = username;
        Password = password;
        Role = role;
    }
}
