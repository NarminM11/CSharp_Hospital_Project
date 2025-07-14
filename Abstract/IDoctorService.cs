using AppointmentTimeNamespace;

namespace DoctorNamespace
{
    public interface IDoctorService
    {
        void ApplyJob();
        List<AppointmentTime> WorkingHours { get; set; }
        List<AppointmentTime> WorkingDays { get; set; }
    }
}