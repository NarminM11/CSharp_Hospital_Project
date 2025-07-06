using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using AppointmentTimeNamespace;
using C_Hospital_Appointment.Models;

namespace DoctorNamespace
{
    public class Doctor
    {
        public Guid Id { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int? WorkExperience { get; set; }
        public string Department { get; set; }

        // hekimin qəbulu üçün saat və gün listələri
        public List<AppointmentTime> WorkingHours { get; set; }
        public List<AppointmentTime> WorkingDays  { get; set; }

        private List<DoctorApplication> _applications;

        private readonly string _dataFolderPath;
        private readonly string _filePath;

        public Doctor()
        {
            WorkingHours = new List<AppointmentTime>
            {
                new AppointmentTime { TimeRange = "09:00-11:00" },
                new AppointmentTime { TimeRange = "12:00-14:00" },
                new AppointmentTime { TimeRange = "15:00-17:00" }
            };
            WorkingDays = new List<AppointmentTime>
            {
                new AppointmentTime { Date = DateTime.Now },
                new AppointmentTime { Date = DateTime.Now.AddDays(1) },
                new AppointmentTime { Date = DateTime.Now.AddDays(2) }
            };
            _applications = new List<DoctorApplication>();

            string projectRoot = Path.GetFullPath(
                Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
            _dataFolderPath = Path.Combine(projectRoot, "Data");

            if (!Directory.Exists(_dataFolderPath))
                Directory.CreateDirectory(_dataFolderPath);

            _filePath = Path.Combine(_dataFolderPath, "applications.json");
        }

        public Doctor(string firstname, string lastname, int workExperience)
            : this() 
        {
            Firstname     = firstname;
            Lastname      = lastname;
            WorkExperience = workExperience;
        }

        public void ApplyJob()
        {
            // 1) Let user pick the department with arrows
            var allowed = new[] { "Pediatriya", "Travmatologiya", "Stomatologiya" };
            int depIndex = CSharpHospitalAppointment.Program.ShowMenu("Select Department", allowed);
            string jobDepartment = allowed[depIndex];

            // 2) Ask for the motivation letter
            Console.Write("Motivation Letter: ");
            var motivationLetter = Console.ReadLine()!;
    
            // 3) Load existing applications (if any)
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                _applications = JsonSerializer.Deserialize<List<DoctorApplication>>(json)
                                ?? new List<DoctorApplication>();
            }
            else
            {
                _applications = new List<DoctorApplication>();
            }

            // 4) Create and save
            var newApp = new DoctorApplication
            {
                Id               = Guid.NewGuid(),
                doctorFirstame   = Firstname,
                doctorSurname    = Lastname,
                doctorEmail      = Email,
                doctorExperience = WorkExperience,
                jobDepartment    = jobDepartment,
                motivationLetter = motivationLetter,
                ApplicationDate  = DateTime.Now,
                Status           = "Pending"
            };
            _applications.Add(newApp);

            var options    = new JsonSerializerOptions { WriteIndented = true };
            var updatedJson = JsonSerializer.Serialize(_applications, options);
            File.WriteAllText(_filePath, updatedJson);

            Console.WriteLine("Application submitted successfully.");
        }

    }
}
