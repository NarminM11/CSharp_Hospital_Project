using System.Text.Json;
using System.Text.RegularExpressions;
using DoctorNamespace;

namespace DoctorAccountManagerNamespace
{
    public class DoctorAccountManager
    {
        public List<Doctor> _doctors { get; set; }  // bütün doctorlar burada olacaq
        private readonly string _dataFolderPath;    
        private readonly string _filePath;         

        public DoctorAccountManager()
        {
            try
            {
                string projectRoot = Path.GetFullPath(
                    Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
                _dataFolderPath = Path.Combine(projectRoot, "Data");
                if (!Directory.Exists(_dataFolderPath))
                    Directory.CreateDirectory(_dataFolderPath);  

                _filePath = Path.Combine(_dataFolderPath, "doctors.json");

                if (File.Exists(_filePath))
                {
                    var jsonData = File.ReadAllText(_filePath);
                    _doctors = JsonSerializer.Deserialize<List<Doctor>>(jsonData)
                               ?? new List<Doctor>();
                }
                else
                {
                    _doctors = new List<Doctor>(); 
                }
            }
            catch (JsonException ex)
            {
                // json parse xetası
                throw new ApplicationException("Error parsing doctors.json.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to setup DoctorAccountManager.", ex);
            }
        }

        public int SearchUser(string email)
        {
            // email ilə doctor index-ini tapiriq
            for (int i = 0; i < _doctors.Count; i++)
            {
                if (_doctors[i].Email == email)
                    return i;
            }
            return -1;  // tapilmasa
        }

        public void SignUp(string firstname, string lastname, int workExperience, string email, string password)
        {
            if (SearchUser(email) != -1)
            {
                Console.WriteLine("This doctor already exists.");  // artıq  bu email ile qeydiyyatlı hekim var
                return;
            }

            // email format regex validasiya
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                Console.WriteLine("Invalid email format. Please enter a valid email address.");
                return;
            }

            // password length
            if (password.Length <= 8)
            {
                Console.WriteLine("Password should be more than 8 characters.");
                return;
            }

            var newDoctor = new Doctor
            {
                Firstname      = firstname,   
                Lastname       = lastname,
                WorkExperience = workExperience,
                Email          = email,
                Password       = password
            };

            _doctors.Add(newDoctor);
            Console.WriteLine("Doctor successfully signed up."); //succes mesage

            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var jsonData = JsonSerializer.Serialize(_doctors, options);
                File.WriteAllText(_filePath, jsonData);
            }
            catch (IOException ex)
            {
                throw new ApplicationException("Error writing to doctors.json.", ex);
            }
            catch (JsonException ex)
            {
                throw new ApplicationException("Error serializing doctor list.", ex);
            }
        }

        public Doctor? SignIn(string email, string password)
        {
            int index = SearchUser(email);
            if (index == -1)
            {
                Console.WriteLine("Doctor not found.");  
                return null;
            }

            if (_doctors[index].Password == password)
                return _doctors[index];  // login success
            else
            {
                Console.WriteLine("Wrong password.");  
                return null;
            }
        }
    }
}
