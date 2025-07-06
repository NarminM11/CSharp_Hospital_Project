using UserNamespace;
using AppointmentTimeNamespace;
using DoctorNamespace;

namespace AppointmentNamespace;

public class Appointment
{
    public User? User { get; set; }
    public Doctor? Doctor { get; set; }
    public AppointmentTime? Slot { get; set; }
    public DateTime Date { get; set; }
}