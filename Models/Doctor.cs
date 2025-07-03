using AppointmentTimeNamespace;
using System.Text.Json;
using C_Hospital_Appointment.Models;

namespace DoctorNamespace;

public class Doctor
{
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public int? WorkExperience { get; set; }

    // həkimin qəbul saatlarını saxlamaq üçün
    public List<AppointmentTime> WorkingHours { get; set; }
    public List<AppointmentTime> WorkingDays { get; set; }
    public List<DoctorApplication> _applications { get; set; }

    private readonly string _filePath;

    public Doctor()
    {

    }
    public Doctor(string firstname, string lastname, int workExperience)
    {
        Firstname = firstname;
        Lastname = lastname;
        WorkExperience = workExperience;

        // hekimin qebul saatlari

        WorkingDays = new List<AppointmentTime>
        {

            new AppointmentTime { Date=DateTime.Now },
            new AppointmentTime { Date = DateTime.Now.AddDays(1)},
            new AppointmentTime { Date = DateTime.Now.AddDays(2)}
        };

        WorkingHours = new List<AppointmentTime>
        {

            new AppointmentTime { TimeRange = "09:00-11:00" },
            new AppointmentTime { TimeRange = "12:00-14:00" },
            new AppointmentTime { TimeRange = "15:00-17:00" }
        };


    }

    public void ApplyJob(string jobDepartment, string motivationLetter)
    {
  
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string applicationFilePath = Path.Combine(desktopPath, "applications.json");

        if (File.Exists(applicationFilePath))
        {
            var jsonData = File.ReadAllText(applicationFilePath);
            _applications = JsonSerializer.Deserialize<List<DoctorApplication>>(jsonData) ?? new List<DoctorApplication>();
        }
        else
        {
            _applications = new List<DoctorApplication>();
        }
        
        var newApplication = new DoctorApplication
        {
            doctorFirstame = this.Firstname,
            doctorSurname = this.Lastname,
            doctorEmail = this.Email,
            doctorExperience = this.WorkExperience,
            jobDepartment = jobDepartment,
            motivationLetter = motivationLetter
        }; 

        _applications.Add(newApplication);
        
        var options = new JsonSerializerOptions { WriteIndented = true };
        var updatedJson = JsonSerializer.Serialize(_applications, options);
        File.WriteAllText(applicationFilePath, updatedJson);

        Console.WriteLine("Application submitted successfully.");

    }
   
}