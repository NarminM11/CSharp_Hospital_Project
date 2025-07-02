using AdminNamespace;
using System.Text.Json;
using System.Text.RegularExpressions;
using UserNamespace;
using DoctorNamespace;
namespace DoctorAccountManagerNamespace
{
    public class DoctorAccountManager
    {
        public List<Doctor> _doctors{ get; set; }


        public DoctorAccountManager()
        {
            string path = @"C:\Users\Ferid\Desktop\C#\C#Lesson13\C#Lesson13\Models\users.json";

            if (File.Exists(path))
            {
                var jsonData = File.ReadAllText(path);
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
            File.WriteAllText(@"C:\Users\Ferid\Desktop\C#Hospital_Project\C#Hospital_Appointment\Models\doctors.json", jsonData);

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
