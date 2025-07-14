using System.Text.Json;
using AdminNamespace;
using AppointmentTimeNamespace;
using UserNamespace;
using DoctorNamespace;
using UserManagerNamespace;
using DoctorAccountManagerNamespace;
using CSharp_Hospital;
using Serilog;

namespace CSharpHospitalAppointment
{
    class Program
    {
        public static int ShowMenu(string title, string[] options)
        {
            int selected = 0;
            ConsoleKey key;
            do
            {
                Console.Clear();
                Console.WriteLine(title);
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selected)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    Console.WriteLine(options[i]);
                    Console.ResetColor();
                }
                key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.UpArrow)
                    selected = (selected - 1 + options.Length) % options.Length;
                else if (key == ConsoleKey.DownArrow)
                    selected = (selected + 1) % options.Length;
            } while (key != ConsoleKey.Enter);
            return selected;
        }

        static List<Doctor> LoadAcceptedDoctors()
        {
            Log.Debug("Loading accepted doctors from Data folder");
            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
            string dataFolder = Path.Combine(projectRoot, "Data");
            string filePath = Path.Combine(dataFolder, "acceptedDoctors.json");

            if (!File.Exists(filePath))
            {
                Log.Warning("Accepted doctors file not found at {FilePath}", filePath);
                return new List<Doctor>();
            }

            var jsonData = File.ReadAllText(filePath);
            var doctors = JsonSerializer.Deserialize<List<Doctor>>(jsonData);
            return doctors ?? new List<Doctor>();
        }


        
        static void Main(string[] args)
        {
            // serilog configuration
            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
            string dataFolder = Path.Combine(projectRoot, "Data");
            Directory.CreateDirectory(dataFolder);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(
                    path: Path.Combine(dataFolder, "log-.txt"),
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            Log.Information("Application started");

            var userManager = new UserManager();
            var doctorManager = new DoctorAccountManager();

            while (true)
            {
                var mainOpts = new[] { "Admin", "User", "Doctor", "Exit" };
                int choice = ShowMenu("=== Select Account Type ===", mainOpts);
                Log.Debug("Main menu choice: {Choice}", mainOpts[choice]);

                switch (choice)
                {
                    case 0:
                        AdminMenu(userManager);
                        break;
                    case 1:
                        UserMenu(userManager);
                        break;
                    case 2:
                        DoctorMenu(doctorManager);
                        break;
                    case 3:
                        Log.Information("Exiting application");
                        Console.WriteLine("Goodbye!");
                        Log.CloseAndFlush();
                        return;
                }
            }
        }

        static void AdminMenu(UserManager userManager)
        {
            Log.Information("Entering Admin menu");
            while (true)
            {
                int sel = ShowMenu("--- Admin ---", new[] { "Sign In", "Back" });
                Log.Debug("Admin menu choice: {Choice}", sel);
                if (sel == 1) return;

                Console.Write("Username: ");
                var username = Console.ReadLine()!;
                Console.Write("Password: ");
                var password = Console.ReadLine()!;
                Log.Debug("Admin sign-in attempt: {Username}", username);

                var result = userManager.SignIn(username, password);
                if (result is Admin admin)
                {
                    Log.Information("Admin signed in: {Username}", username);
                    while (true)
                    {
                        int opt = ShowMenu("--- Admin Panel ---", new[] { "View Applications", "Approve/Reject", "Logout" });
                        Log.Debug("Admin panel choice: {Choice}", opt);
                        if (opt == 2)
                        {
                            Log.Information("Admin logged out: {Username}", username);
                            break;
                        }

                        if (opt == 0)
                        {
                            admin.ViewAllDoctorApplications();
                            Console.WriteLine("\nPress any key to return...");
                            Console.ReadKey(true);
                        }
                        else if (opt == 1)
                        {
                            var apps = admin.ViewAllDoctorApplications();
                            if (!apps.Any())
                            {
                                Console.WriteLine("No applications.");
                                Console.ReadKey(true);
                                continue;
                            }

                            var appOpts = apps
                                .Select(a => $"{a.doctorFirstame} {a.doctorSurname} - {a.jobDepartment} ({a.Status})")
                                .ToArray();
                            int appSel = ShowMenu("Select Application", appOpts);
                            var selectedApp = apps[appSel];

                            int actionSel = ShowMenu("Choose Action", new[] { "Accept", "Reject", "Cancel" });
                            if (actionSel == 0)
                            {
                                admin.AcceptApplication(selectedApp.Id);
                                Log.Information("Accepted: {AppId}", selectedApp.Id);
                            }
                            else if (actionSel == 1)
                            {
                                admin.RejectApplication(selectedApp.Id);
                                Log.Information("Rejected: {AppId}", selectedApp.Id);
                            }

                            Console.WriteLine("\nPress any key to continue...");
                            Console.ReadKey(true);
                        }
                    }
                }
                else
                {
                    Log.Warning("Admin sign-in failed: {Username}", username);
                    Console.WriteLine("Sign-in failed.");
                    Console.ReadKey(true);
                }
            }
        }

        static void UserMenu(UserManager userManager)
        {
            Log.Information("Entering User menu");
            while (true)
            {
                var opts = new[] { "Sign In", "Sign Up", "Back" };
                int sel = ShowMenu("--- User ---", opts);
                Log.Debug("User menu choice: {Choice}", opts[sel]);
                if (sel == 2) return;

                if (sel == 0)
                {
                    Console.Write("Username: ");
                    var userUsername= Console.ReadLine()!;
                    Console.Write("Password: ");
                    var userPassword = Console.ReadLine()!;
                    Log.Debug("User sign-in attempt: {Username}", userUsername);
                    var user = userManager.SignIn(userUsername, userPassword) as User;
                    if (user != null)
                    {
                        Log.Information("User signed in: {Username}", userUsername);
                        var accepted = LoadAcceptedDoctors();
                        if (!accepted.Any()) { Console.WriteLine("No doctors."); continue; }
                        var departments = accepted.Select(d=>d.Department).Distinct().ToArray();
                        int selectedDepartment = ShowMenu("Select Department", departments);
                        var docs = accepted.Where(d=>d.Department==departments[selectedDepartment]).ToArray();
                        int selectedDoctor = ShowMenu("Select Doctor", docs.Select(d=>$"Dr. {d.Firstname} {d.Lastname}").ToArray());
                        var doc = docs[selectedDoctor];
                        var reservation = new ReservationManager();
                        reservation.LoadReservationsFromFile(accepted);
                        var slots = doc.WorkingHours.Select(s=>$"{s.TimeRange} {(s.IsReserved?"(X)":"")}").ToArray();
                        int slotSel = ShowMenu("Select Time Slot", slots);
                        if (!doc.WorkingHours[slotSel].IsReserved)
                            reservation.ReserveAppointment(user, doc, doc.WorkingHours[slotSel].TimeRange);
                    }
                }
                else if (sel == 1)
                {
                    Console.Write("First Name: "); var userFirstname = Console.ReadLine()!;
                    Console.Write("Last Name: "); var userLastname = Console.ReadLine()!;
                    Console.Write("Email: "); var userEmail = Console.ReadLine()!;
                    Console.Write("Username: "); var userUsername2 = Console.ReadLine()!;
                    Console.Write("Password: "); var userPassword2 = Console.ReadLine()!;
                    Log.Debug("User sign-up: {Username}", userUsername2);
                    userManager.SignUp(userFirstname, userLastname, userEmail, userUsername2, userPassword2);
                }
            }
        }

        static void DoctorMenu(DoctorAccountManager doctorManager)
        {
            Log.Information("Entering Doctor menu");
            while (true)
            {
                var opts = new[] { "Sign In", "Sign Up", "Back" };
                int sel = ShowMenu("--- Doctor ---", opts);
                Log.Debug("Doctor menu choice: {Choice}", opts[sel]);
                if (sel == 2) return;

                if (sel == 0)
                {
                    Console.Write("Email: "); var e = Console.ReadLine()!;
                    Console.Write("Password: "); var pw = Console.ReadLine()!;
                    var doc = doctorManager.SignIn(e, pw) as Doctor;
                    if (doc!=null)
                    {
                        Log.Information("Doctor signed in: {Email}", e);
                        while(true)
                        {
                            var panel = new[] { "Apply for Job", "View Profile", "Logout" };
                            int choice = ShowMenu("--- Doctor Panel ---", panel);
                            if(choice==2) break;
                            if (choice == 0)
                            {
                                doc.ApplyJob(); 
                            }

                            else if(choice==1)
                            {
                                Console.WriteLine($"Dr. {doc.Firstname} {doc.Lastname}");
                                Console.WriteLine(doc.Email);
                                Console.WriteLine($"Experience: {doc.WorkExperience}");
                            }
                        }
                    }
                }
                else if(sel==1)
                {
                    Console.Write("First Name: "); var doctorFirstname=Console.ReadLine()!;
                    Console.Write("Last Name: "); var doctorLastname=Console.ReadLine()!;
                    Console.Write("Experience: "); var doctorWorkExperience=Console.ReadLine()!;
                    int exp = int.TryParse(doctorWorkExperience, out var x)?x:0;
                    Console.Write("Email: "); var doctorEmail=Console.ReadLine()!;
                    Console.Write("Password: "); var doctorPassword=Console.ReadLine()!;
                    doctorManager.SignUp(doctorFirstname, doctorLastname, exp, doctorEmail, doctorPassword);
                }
            }
        }
    }
}
