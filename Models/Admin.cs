using System.Text.Json;
using C_Hospital_Appointment.Models;
using DoctorAccountManagerNamespace;

namespace AdminNamespace
{
    public class Admin
    {
        public Guid Id { get; set; }
        public string username { get; set; }

        public string Password { get; set; }
        
        public List<DoctorApplication> ViewAllDoctorApplications()
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string applicationFilePath = Path.Combine(desktopPath, "applications.json");

            if (!File.Exists(applicationFilePath))
            {
                Console.WriteLine("No applications found.");
                return new List<DoctorApplication>();
            }

            string jsonData = File.ReadAllText(applicationFilePath);
            var applications = JsonSerializer.Deserialize<List<DoctorApplication>>(jsonData);

            if (applications == null || applications.Count == 0)
            {
                Console.WriteLine("Application list is empty.");
                return new List<DoctorApplication>();
            }

            foreach (var application in applications)
            {
                Console.WriteLine($"Name: {application.doctorFirstame} {application.doctorSurname}");
                Console.WriteLine($"Email: {application.doctorEmail}");
                Console.WriteLine($"Department: {application.jobDepartment}");
                Console.WriteLine($"Experience: {application.doctorExperience} years");
                Console.WriteLine($"Motivation: {application.motivationLetter}");
                Console.WriteLine($"Status: {application.Status}");
                Console.WriteLine($"Applied on: {application.ApplicationDate}");
                Console.WriteLine("-----------------------------");
            }

            return applications;
        }
        public void AcceptApplicatin()
        {
            
        
            
        }
    }
    
   
}
