namespace C_Hospital_Appointment.Models;

public class DoctorApplication
{
    public string doctorFirstame { get; set; }
    public string doctorSurname { get; set; }
    public string doctorEmail { get; set; }
    public int? doctorExperience { get; set; }
    public string motivationLetter { get; set; }
    public string jobDepartment  { get; set; }
    public DateTime ApplicationDate { get; set; } = DateTime.Now;
    public string Status { get; set; } = "Pending"; 
}