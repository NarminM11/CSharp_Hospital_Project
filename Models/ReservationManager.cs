using EmailhelperNamespace;
using DoctorNamespace;
using UserNamespace;

namespace CSharp_Hospital
{
    public class ReservationManager : IReservationService
    {
        public List<User> _users { get; set; }  // bütün userler listi

        private readonly string _dataFolderPath;  
        private readonly string _filePath;        // reservations.json-un pathi

        public ReservationManager()
        {
            try
            {
                string projectRoot = Path.GetFullPath(
                    Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
                _dataFolderPath = Path.Combine(projectRoot, "Data");
                if (!Directory.Exists(_dataFolderPath))
                    Directory.CreateDirectory(_dataFolderPath); 

                _filePath = Path.Combine(_dataFolderPath, "reservations.json");
                _users = new List<User>(); 
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to setup ReservationManager.", ex);
            }
        }

        public bool ReserveAppointment(User user, Doctor doctor, string selectedTime)
        {
            if (doctor?.WorkingHours == null || user == null)
            {
                Console.WriteLine("Sistemdə həkim və ya istifadəçi məlumatı mövcud deyil.");
                return false;
            }

            var slot = doctor.WorkingHours
                .FirstOrDefault(s => s.TimeRange == selectedTime && !s.IsReserved);

            if (slot == null)
            {
                Console.WriteLine("Vaxt aralığı tapılmadı");
                return false;
            }

            // rezervasiya edirik
            slot.IsReserved = true;
            slot.ReservedBy = user;

            // email body
            var subject = "Yeni Rezervasiya Yaradıldı";
            var body =
                $"Hörmətli {user.FirstName} {user.LastName},\n\n" +
                $"Siz uğurla rezervasiya etdiniz:\n" +
                $"- Saat: {selectedTime}\n" +
                $"- Department: {doctor.Department}\n\n" +
                $"- Həkim: Dr. {doctor.Firstname} {doctor.Lastname}\n\n" +
                $"Zəhmət olmasa göstərilən vaxtda yaxınlaşın.\n\n" +
                $"Hörmətlə,\nMərkəzi Sağlamlıq Klinikası";

            try
            {
                EmailHelper.SendEmailToAdmin(subject, body, user.Email);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to send reservation email.", ex);
            }

            try
            {
                string record = $"{doctor.Firstname},{doctor.Lastname},{selectedTime},{user.FirstName} {user.LastName}";
                File.AppendAllText(_filePath, record + Environment.NewLine);
            }
            catch (IOException ex)
            {
                throw new ApplicationException("Error writing reservation record.", ex);
            }

            return true;  
        }

        public void LoadReservationsFromFile(List<Doctor> allDoctors)
        {
            if (!File.Exists(_filePath))
                return;  

            try
            {
                foreach (var line in File.ReadLines(_filePath))
                {
                    var parts = line.Split(',');
                    if (parts.Length < 3)
                        continue; 

                    var doctor = allDoctors
                        .FirstOrDefault(d => d.Firstname == parts[0] && d.Lastname == parts[1]);

                    var slot = doctor?.WorkingHours
                        .FirstOrDefault(s => s.TimeRange == parts[2]);

                    if (slot != null)
                        slot.IsReserved = true; 
                }
            }
            catch (IOException ex)
            {
                // fayl oxunmasi zamani xəta
                throw new ApplicationException("Error reading reservation file.", ex);
            }
        }
    }
}
