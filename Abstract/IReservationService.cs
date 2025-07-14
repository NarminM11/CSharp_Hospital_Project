using DoctorNamespace;
using UserNamespace;

namespace CSharp_Hospital
{
    public interface IReservationService
    {
        bool ReserveAppointment(User user, Doctor doctor, string selectedTime);
        void LoadReservationsFromFile(List<Doctor> allDoctors);
    }
}