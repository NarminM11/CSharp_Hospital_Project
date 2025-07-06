using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using DoctorNamespace;

namespace DoctorAccountManagerNamespace
{
    public class DoctorAccountManager
    {
        public List<Doctor> _doctors { get; set; }
        private readonly string _dataFolderPath;
        private readonly string _filePath;

        public DoctorAccountManager()
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

        public int SearchUser(string email)
        {
            for (int i = 0; i < _doctors.Count; i++)
            {
                if (_doctors[i].Email == email)
                    return i;
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

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                Console.WriteLine("Invalid email format. Please enter a valid email address.");
                return;
            }

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
                return _doctors[index];
            else
            {
                Console.WriteLine("Wrong password.");
                return null;
            }
        }
    }
}
