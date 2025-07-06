using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using AppointmentTimeNamespace;
using C_Hospital_Appointment.Models;
using DoctorNamespace;

namespace AdminNamespace
{
    public class Admin
    {
        public Guid Id { get; set; }
        public string username { get; set; }
        public string Password { get; set; }

        private readonly string _dataFolderPath;

        public Admin()
        {
            string projectRoot = Path.GetFullPath(
                Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
            
            _dataFolderPath = Path.Combine(projectRoot, "Data");
            if (!Directory.Exists(_dataFolderPath))
                Directory.CreateDirectory(_dataFolderPath);
        }

        public List<DoctorApplication> ViewAllDoctorApplications()
        {
            string applicationFilePath = Path.Combine(_dataFolderPath, "applications.json");

            if (!File.Exists(applicationFilePath))
            {
                Console.WriteLine("No applications found.");
                return new List<DoctorApplication>();
            }

            string jsonData = File.ReadAllText(applicationFilePath);
            var applications = JsonSerializer.Deserialize<List<DoctorApplication>>(jsonData) 
                               ?? new List<DoctorApplication>();

            if (applications.Count == 0)
            {
                Console.WriteLine("Application list is empty.");
                return applications;
            }

            for (int i = 0; i < applications.Count; i++)
            {
                var app = applications[i];
                Console.WriteLine($"\nApplication #{i + 1}");
                Console.WriteLine($"Name: {app.doctorFirstame} {app.doctorSurname}");
                Console.WriteLine($"Email: {app.doctorEmail}");
                Console.WriteLine($"Department: {app.jobDepartment}");
                Console.WriteLine($"Experience: {app.doctorExperience} years");
                Console.WriteLine($"Motivation: {app.motivationLetter}");
                Console.WriteLine($"Status: {app.Status}");
                Console.WriteLine($"Applied on: {app.ApplicationDate}");
                Console.WriteLine("-----------------------------");
            }

            return applications;
        }

        public void AcceptApplication(Guid id)
        {
            string applicationFilePath = Path.Combine(_dataFolderPath, "applications.json");
            string acceptedPath        = Path.Combine(_dataFolderPath, "acceptedDoctors.json");

            var applications = JsonSerializer.Deserialize<List<DoctorApplication>>(
                                   File.ReadAllText(applicationFilePath))
                               ?? new List<DoctorApplication>();

            var selectedApp = applications.FirstOrDefault(a => a.Id == id);
            if (selectedApp == null)
            {
                Console.WriteLine("Application not found.");
                return;
            }

            selectedApp.Status = "Accepted";

            var doctor = new Doctor
            {
                Id             = Guid.NewGuid(),
                Firstname      = selectedApp.doctorFirstame,
                Lastname       = selectedApp.doctorSurname,
                Email          = selectedApp.doctorEmail,
                WorkExperience = selectedApp.doctorExperience,
                Department     = selectedApp.jobDepartment,
                Password       = "doctorAccepted123",
                WorkingHours   = new List<AppointmentTime>
                {
                    new AppointmentTime { TimeRange = "09:00-11:00" },
                    new AppointmentTime { TimeRange = "12:00-14:00" },
                    new AppointmentTime { TimeRange = "15:00-17:00" }
                }
            };

            var acceptedDoctors = File.Exists(acceptedPath)
                ? JsonSerializer.Deserialize<List<Doctor>>(File.ReadAllText(acceptedPath)) 
                  ?? new List<Doctor>()
                : new List<Doctor>();

            acceptedDoctors.Add(doctor);
            File.WriteAllText(acceptedPath,
                JsonSerializer.Serialize(acceptedDoctors, new JsonSerializerOptions { WriteIndented = true }));

            File.WriteAllText(applicationFilePath,
                JsonSerializer.Serialize(applications, new JsonSerializerOptions { WriteIndented = true }));

            Console.WriteLine("Application accepted and doctor added.");
        }

        public void RejectApplication(Guid applicationId)
        {
            string applicationFilePath   = Path.Combine(_dataFolderPath, "applications.json");
            string rejectedDoctorsPath   = Path.Combine(_dataFolderPath, "rejectedDoctors.json");

            var allApplications = JsonSerializer.Deserialize<List<DoctorApplication>>(
                                      File.ReadAllText(applicationFilePath))
                                  ?? new List<DoctorApplication>();

            var application = allApplications.FirstOrDefault(app => app.Id == applicationId);
            if (application == null)
            {
                Console.WriteLine("Application not found.");
                return;
            }

            application.Status = "Rejected";

            var rejectedList = File.Exists(rejectedDoctorsPath)
                ? JsonSerializer.Deserialize<List<DoctorApplication>>(File.ReadAllText(rejectedDoctorsPath))
                  ?? new List<DoctorApplication>()
                : new List<DoctorApplication>();

            rejectedList.Add(application);

            File.WriteAllText(applicationFilePath,
                JsonSerializer.Serialize(allApplications, new JsonSerializerOptions { WriteIndented = true }));
            File.WriteAllText(rejectedDoctorsPath,
                JsonSerializer.Serialize(rejectedList, new JsonSerializerOptions { WriteIndented = true }));

            Console.WriteLine($"Application from {application.doctorFirstame} {application.doctorSurname} rejected.");
        }
    }
}
