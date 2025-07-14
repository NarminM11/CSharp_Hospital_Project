using System.Text.Json;
using AppointmentTimeNamespace;
using C_Hospital_Appointment.Models;

namespace DoctorNamespace
{
    public class Doctor : IDoctorService
    {
        public Guid Id { get; set; }  
        public string? Firstname { get; set; }  // doctorun adı
        public string? Lastname { get; set; }   // doctorun soyadı
        public string Password { get; set; }   
        public string Email { get; set; }       // doctor email
        public int? WorkExperience { get; set; } // doctorun iş təcrübəsi il olaraq
        public string Department { get; set; }   // doctorun departament 

        // hekimin iş saatları və günləri üçün listlər
        public List<AppointmentTime> WorkingHours { get; set; }
        public List<AppointmentTime> WorkingDays  { get; set; }

        private List<DoctorApplication> _applications;  // umumi applications olan list

        private readonly string _dataFolderPath;  
        private readonly string _filePath;       

        public Doctor()
        {
            //hekimin is gunleri ve saatlarini initalize edirik
            WorkingHours = new List<AppointmentTime>
            {
                new AppointmentTime { TimeRange = "09:00-11:00" },
                new AppointmentTime { TimeRange = "12:00-14:00" },
                new AppointmentTime { TimeRange = "15:00-17:00" }
            };
            WorkingDays = new List<AppointmentTime>
            {
                new AppointmentTime { Date = DateTime.Now },
                new AppointmentTime { Date = DateTime.Now.AddDays(1) },
                new AppointmentTime { Date = DateTime.Now.AddDays(2) }
            };
            _applications = new List<DoctorApplication>();

            try
            {
                string projectRoot = Path.GetFullPath(
                    Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
                _dataFolderPath = Path.Combine(projectRoot, "Data");
                if (!Directory.Exists(_dataFolderPath))
                    Directory.CreateDirectory(_dataFolderPath);  // eger data folderimiz yoxdursa yaradir
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to init data folder for Doctor.", ex);
            }

            _filePath = Path.Combine(_dataFolderPath, "applications.json");
        }

        public Doctor(string firstname, string lastname, int workExperience)
            : this()
        {
            Firstname      = firstname;
            Lastname       = lastname;
            WorkExperience = workExperience;
        }

        public void ApplyJob()
        {
            //apply edende department secmek hissesi
            var allowed = new[] { "Pediatriya", "Travmatologiya", "Stomatologiya" };
            int depIndex = CSharpHospitalAppointment.Program.ShowMenu("Select Department", allowed);
            string jobDepartment = allowed[depIndex];  // doctorun secdiyi department
            
            // apply zamani motivation letter sorusulur, niye bu ise qebul olmaq isteyir
            Console.Write("Motivation Letter: ");
            var motivationLetter = Console.ReadLine()!;

            try
            {
                if (File.Exists(_filePath))
                {
                    var json = File.ReadAllText(_filePath);
                    _applications = JsonSerializer.Deserialize<List<DoctorApplication>>(json)
                                    ?? new List<DoctorApplication>();
                }
                else
                {
                    _applications = new List<DoctorApplication>();  // ele file yoxdursa hele
                }
            }
            catch (IOException ex)
            {
                throw new ApplicationException("Error reading applications file.", ex);
            }
            catch (JsonException ex)
            {
                throw new ApplicationException("Invalid JSON in applications file.", ex);
            }

            // yeni application obyekti
            var newApp = new DoctorApplication
            {
                Id               = Guid.NewGuid(),
                doctorFirstame   = Firstname,
                doctorSurname    = Lastname,
                doctorEmail      = Email,
                doctorExperience = WorkExperience,
                jobDepartment    = jobDepartment,
                motivationLetter = motivationLetter,
                ApplicationDate  = DateTime.Now,
                Status           = "Pending" //hele cavab gelmiyib(yeni yaradilib), gozlemededir
            };
            _applications.Add(newApp);

            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var updatedJson = JsonSerializer.Serialize(_applications, options);
                File.WriteAllText(_filePath, updatedJson);
            }
            catch (IOException ex)
            {
                throw new ApplicationException("Error writing applications file.", ex);
            }
            catch (JsonException ex)
            {
                throw new ApplicationException("Error serializing applications to JSON.", ex);
            }

            Console.WriteLine("Application submitted successfully.");  
        }
    }
}
