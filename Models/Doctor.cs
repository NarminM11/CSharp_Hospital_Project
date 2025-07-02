using AppointmentTimeNamespace;
using System.Security.Cryptography.X509Certificates;

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






    }
   
}