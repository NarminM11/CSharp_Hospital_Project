using System.Text.Json;
using System.Text.RegularExpressions;
using DoctorNamespace;
namespace DoctorAccountManagerNamespace
{
    public class DoctorAccountManager
    {
        public List<Doctor> _doctors{ get; set; }
        private readonly string _filePath;


        public DoctorAccountManager()
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            _filePath = Path.Combine(desktopPath, "doctors.json");
            if (File.Exists(_filePath))
            {
                var jsonData = File.ReadAllText(_filePath);
                _doctors = JsonSerializer.Deserialize<List<Doctor>>(jsonData);
            }
            else
            {
                _doctors = new List<Doctor>();
            }
        }

        public int SearchUser(string email)
        {
            for (int i = 0; i < _doctors.Count; i++)
            {
                if (_doctors[i].Email == email)
                {
                    return i;
                }

            }
            return -1;
        }

        public void SignUp(string firstname, string lastname, int workExperience, string email, string password)
        {
            if (SearchUser(email) != -1)
            {
                Console.WriteLine("This doctor already exists.");
                return;
            }
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            if (!Regex.IsMatch(email, emailPattern))
            {
                Console.WriteLine("Invalid email format. Please enter a valid email address.");
                return;
            }

            if (password.Length <= 8)
            {
                Console.WriteLine("Password should be more than 8 characters.");
                return;
            }

            Doctor newDoctor = new Doctor()
            {
                Firstname = firstname,
                Lastname = lastname,
                WorkExperience = workExperience,
                Email = email,
                Password = password,
            };

            _doctors.Add(newDoctor);
            Console.WriteLine("Doctor successfully signed up.");
            var options = new JsonSerializerOptions { WriteIndented = true };
            var jsonData = JsonSerializer.Serialize(_doctors, options);
            File.WriteAllText(_filePath, jsonData);

        }


        public object SignIn(string email, string password)
        {

            int index = SearchUser(email);
            if (index == -1)
            {
                Console.WriteLine("Doctor not found.");
                return null;
            }

            if (_doctors[index].Password == password)
            {
                return _doctors[index];
            }
            else
            {
                Console.WriteLine("Wrong password.");
                return null;
            }
        }
    }
}
