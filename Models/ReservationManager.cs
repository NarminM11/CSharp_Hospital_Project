namespace CSharp_Hospital;
using EmailhelperNamespace;
using System.IO;
using DoctorNamespace;
using UserNamespace;

public class ReservationManager
{
    public List<User> _users { get; set; }

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
            Console.WriteLine("Vaxt aralığı tapılmadı və ya artıq rezerv olunub.");
            return false;
        }

        slot.IsReserved = true;
        slot.ReservedBy = user;
        var toEmail = user.Email;
        var toName = $"{user.FirstName} {user.LastName}";
        var subject = "Yeni Rezervasiya Yaradıldı";
        var body =
            $"Hörmətli {user.FirstName} {user.LastName},\n\n" +
            $"Siz uğurla rezervasiya etdiniz:\n" +
            $"- Saat: {selectedTime}\n" +
            $"- Həkim: Dr. {doctor.Firstname} {doctor.Lastname}\n\n" +
            $"Zəhmət olmasa göstərilən vaxtda yaxınlaşın.\n\n" +
            $"Hörmətlə,\nMərkəzi Sağlamlıq Klinikası";

        EmailHelper.SendEmailToAdmin(subject, body, toEmail);


        // Fayla yaz
        string record = $"{doctor.Firstname},{doctor.Lastname},{selectedTime},{user.FirstName} {user.LastName}";
        File.AppendAllText("reservations.txt", record + Environment.NewLine);

        return true;
    }

    public void LoadReservationsFromFile(List<Doctor> allDoctors)
    {
        if (!File.Exists("reservations.txt")) return;

        foreach (var line in File.ReadLines("reservations.txt"))
        {
            var parts = line.Split(',');
            if (parts.Length < 3) continue;

            var doctor = allDoctors
                .FirstOrDefault(d => d.Firstname == parts[0] && d.Lastname == parts[1]);

            var slot = doctor?.WorkingHours.FirstOrDefault(s => s.TimeRange == parts[2]);
            if (slot != null)
                slot.IsReserved = true;

        }
    }
}