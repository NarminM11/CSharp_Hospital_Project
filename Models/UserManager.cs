using System.Text.Json;
using System.Text.RegularExpressions;
using AdminNamespace;
using UserNamespace;

namespace UserManagerNamespace;

public class UserManager
{
    public List<User> _users { get; set; }
    private readonly string _filePath;


    public UserManager()
    {
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        _filePath = Path.Combine(desktopPath, "users.json");

        if (File.Exists(_filePath))
        {
            var jsonData = File.ReadAllText(_filePath);
            _users = JsonSerializer.Deserialize<List<User>>(jsonData);
        }
        else
        {
            _users = new List<User>();
        }
    }

    public int SearchUser(string username)
    {
        for (int i = 0; i < _users.Count; i++)
        {
            if (_users[i].UserName == username)
            {
                return i;
            }

        }
        return -1;
    }

    public void SignUp(string firstname, string lastname, string email, string username, string password)
    {
        if (SearchUser(username) != -1)
        {
            Console.WriteLine("This user already exists.");
            return;
        }


        if (username.Length <= 10)
        {
            Console.WriteLine("Username should be longer than 10 characters.");
            return;
        }
        if (password.Length <= 8)
        {
            Console.WriteLine("Password should be more than 8 characters.");
            return;
        }

        User newUser = new User()
        {
            FirstName = firstname,
            Email = email,
            LastName = lastname,
            UserName = username,
            Password = password,
        };

        _users.Add(newUser);
        Console.WriteLine("User successfully signed up.");
        var options = new JsonSerializerOptions { WriteIndented = true };
        var jsonData = JsonSerializer.Serialize(_users, options);
        File.WriteAllText(_filePath, jsonData);

    }


    public object SignIn(string username, string password)
    {
        if (username == "admin" && password == "admin1234")
        {
            return new Admin { username = "admin", Password = "admin1234" };
        }

        int index = SearchUser(username);
        if (index == -1)
        {
            Console.WriteLine("User not found.");
            return null;
        }

        if (_users[index].Password == password)
        {
            return _users[index];
        }
        else
        {
            Console.WriteLine("Wrong password.");
            return null;
        }
    }
}