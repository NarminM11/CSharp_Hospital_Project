using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using AdminNamespace;
using UserNamespace;

namespace UserManagerNamespace
{
    public class UserManager
    {
        public List<User> _users { get; set; }  // bütün users burada
        private readonly string _filePath;      // users.json file path-i
        

        public UserManager()
        {
            try
            {
                string projectRoot = Path.GetFullPath(
                    Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
                string dataFolderPath = Path.Combine(projectRoot, "Data");
                if (!Directory.Exists(dataFolderPath))
                    Directory.CreateDirectory(dataFolderPath);  

                _filePath = Path.Combine(dataFolderPath, "users.json");

                if (File.Exists(_filePath))
                {
                    var jsonData = File.ReadAllText(_filePath);
                    _users = JsonSerializer.Deserialize<List<User>>(jsonData)
                             ?? new List<User>();
                }
                else
                {
                    _users = new List<User>();  
                }
            }
            catch (JsonException ex)
            {
                throw new ApplicationException("Error parsing users.json.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to setup UserManager.", ex);
            }
        }

        public int SearchUser(string username)
        {
            for (int i = 0; i < _users.Count; i++)
            {
                if (_users[i].UserName == username)
                    return i;
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

            var newUser = new User
            {
                FirstName = firstname,  
                LastName  = lastname,
                Email     = email,
                UserName  = username,
                Password  = password
            };

            _users.Add(newUser);
            Console.WriteLine("User successfully signed up.");  

            try
            {
                var options  = new JsonSerializerOptions { WriteIndented = true };
                var jsonData = JsonSerializer.Serialize(_users, options);
                File.WriteAllText(_filePath, jsonData);
            }
            catch (IOException ex)
            {
                throw new ApplicationException("Error writing to users.json.", ex);
            }
            catch (JsonException ex)
            {
                throw new ApplicationException("Error serializing user list.", ex);
            }
        }

        public object SignIn(string username, string password)
        {
            if (username == "admin" && password == "admin1234")
                return new Admin { username = "admin", password = "admin1234" };  // admin kimi login 

            int index = SearchUser(username);
            if (index == -1)
            {
                Console.WriteLine("User not found.");  // username-e gore user tapilmasa
                return null;
            }

            if (_users[index].Password == password)
                return _users[index];  // login success
            else
            {
                Console.WriteLine("Wrong password.");  
                return null;
            }
        }
    }
}
