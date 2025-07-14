using C_Hospital_Appointment.Models;

namespace AdminNamespace
{
    public interface IAdminService
    {
        List<DoctorApplication> ViewAllDoctorApplications();
        void AcceptApplication(Guid id);
        void RejectApplication(Guid applicationId);
    }
}