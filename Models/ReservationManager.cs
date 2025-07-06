namespace CSharp_Hospital
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using EmailhelperNamespace;
    using DoctorNamespace;
    using UserNamespace;

    public class ReservationManager
    {
        public List<User> _users { get; set; }

        private readonly string _dataFolderPath;
        private readonly string _filePath;

        public ReservationManager()
        {
            string projectRoot = Path.GetFullPath(
                Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));

            _dataFolderPath = Path.Combine(projectRoot, "Data");
            if (!Directory.Exists(_dataFolderPath))
                Directory.CreateDirectory(_dataFolderPath);

            _filePath = Path.Combine(_dataFolderPath, "reservations.json");

            _users = new List<User>();
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

            slot.IsReserved = true;
            slot.ReservedBy = user;

            var subject = "Yeni Rezervasiya Yaradıldı";
            var body =
                $"Hörmətli {user.FirstName} {user.LastName},\n\n" +
                $"Siz uğurla rezervasiya etdiniz:\n" +
                $"- Saat: {selectedTime}\n" +
                $"- Department {doctor.Department}\n\n" +
                $"- Həkim: Dr. {doctor.Firstname} {doctor.Lastname}\n\n" +
                $"Zəhmət olmasa göstərilən vaxtda yaxınlaşın.\n\n" +
                $"Hörmətlə,\nMərkəzi Sağlamlıq Klinikası";

            EmailHelper.SendEmailToAdmin(subject, body, user.Email);

            string record = $"{doctor.Firstname},{doctor.Lastname},{selectedTime},{user.FirstName} {user.LastName}";
            File.AppendAllText(_filePath, record + Environment.NewLine);

            return true;
        }

        public void LoadReservationsFromFile(List<Doctor> allDoctors)
        {
            if (!File.Exists(_filePath))
                return;

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
    }
}
