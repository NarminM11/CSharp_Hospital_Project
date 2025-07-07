using System.Text.Json;
using AppointmentTimeNamespace;
using C_Hospital_Appointment.Models;
using DoctorNamespace;

namespace AdminNamespace
{
    public class Admin
    {
        public Guid Id { get; set; }  // adminin ID-si
        public string username { get; set; }  // adminin username-i (admin)
        public string password { get; set; }  // admin password-u (admin1234)

        private readonly string _dataFolderPath;  

        public Admin()
        {
            try
            {
                // project root-a getmək üçün (..\..\..) base directory tapılır
                string projectRoot = Path.GetFullPath(
                    Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
                _dataFolderPath = Path.Combine(projectRoot, "Data");  // data folder-i set edirik ki json-u orda saxlayaq

                // eger data folderimiz yoxdursa yaradiriq
                if (!Directory.Exists(_dataFolderPath))
                    Directory.CreateDirectory(_dataFolderPath);
            }
            catch (Exception ex)
            {
                // eger folder zamani xeta olsa
                throw new InvalidOperationException("Failed to initialize Admin data folder.", ex);
            }
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
            string appPath  = Path.Combine(_dataFolderPath, "applications.json");
            string accPath  = Path.Combine(_dataFolderPath, "acceptedDoctors.json");

            try
            {
                // bütün application-lari fayldan yükleyirik
                var applications = JsonSerializer.Deserialize<List<DoctorApplication>>(
                                       File.ReadAllText(appPath))
                                   ?? new List<DoctorApplication>();

                // userin daxil etdiyi id-e gore secmek
                var selected = applications.FirstOrDefault(a => a.Id == id)
                               ?? throw new KeyNotFoundException($"No application with Id {id}");

                selected.Status = "Accepted"; // qebul olunmus applicantin statusu accept olaraq yenilenir avtomatik


                var doctor = new Doctor
                {
                    Id = Guid.NewGuid(),
                    Firstname = selected.doctorFirstame,
                    Lastname = selected.doctorSurname,
                    Email = selected.doctorEmail,
                    WorkExperience = selected.doctorExperience,
                    Department = selected.jobDepartment,
                    Password = "doctorAccepted123", //standart password
                    WorkingHours = new List<AppointmentTime>
                    {
                        new AppointmentTime { TimeRange = "09:00-11:00" },
                        new AppointmentTime { TimeRange = "12:00-14:00" },
                        new AppointmentTime { TimeRange = "15:00-17:00" }
                    }
                };

                // yeni hekim qebul olundugu ucun acceptedDoctors.json faylını yenileyirik
                var acceptedDoctors = File.Exists(accPath)
                    ? JsonSerializer.Deserialize<List<Doctor>>(File.ReadAllText(accPath)) ?? new List<Doctor>()
                    : new List<Doctor>();

                acceptedDoctors.Add(doctor);
                File.WriteAllText(accPath,
                    JsonSerializer.Serialize(acceptedDoctors, new JsonSerializerOptions { WriteIndented = true }));
                File.WriteAllText(appPath,
                    JsonSerializer.Serialize(applications, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch (JsonException ex)
            {
                //eger jsonu deserialize ya da serialize edende xeta cixsa
                throw new ApplicationException("Error serializing or deserializing JSON data.", ex);
            }
        }

        public void RejectApplication(Guid applicationId)
        {
            string appPath = Path.Combine(_dataFolderPath, "applications.json");
            string rejPath = Path.Combine(_dataFolderPath, "rejectedDoctors.json");

            try
            {
                var allApps = JsonSerializer.Deserialize<List<DoctorApplication>>(
                                  File.ReadAllText(appPath))
                              ?? new List<DoctorApplication>();

                var application = allApps.FirstOrDefault(a => a.Id == applicationId)
                                  ?? throw new KeyNotFoundException($"No application with Id {applicationId}");

                application.Status = "Rejected";  

                var rejectedList = File.Exists(rejPath)
                    ? JsonSerializer.Deserialize<List<DoctorApplication>>(File.ReadAllText(rejPath)) 
                      ?? new List<DoctorApplication>()
                    : new List<DoctorApplication>();

                rejectedList.Add(application);
                File.WriteAllText(appPath, JsonSerializer.Serialize(allApps, new JsonSerializerOptions { WriteIndented = true }));
                File.WriteAllText(rejPath, JsonSerializer.Serialize(rejectedList, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch (JsonException ex)
            {
                throw new ApplicationException("JSON error while processing rejection.", ex);
            }
        }
    }
}
