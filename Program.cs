using System;
using System.Collections.Generic;
using AdminNamespace;
using UserNamespace;
using DoctorNamespace;
using UserManagerNamespace;
using DoctorAccountManagerNamespace;
using CSharp_Hospital;
using DepartmentNamespace;

namespace CSharpHospitalAppointment
{
    class Program
    {

        static void Main(string[] args)
        {
            var userManager = new UserManager();
            var doctorManager = new DoctorAccountManager();

            while (true)
            {
                Console.WriteLine("\n=== Select Account Type ===");
                Console.WriteLine("1. Admin");
                Console.WriteLine("2. User");
                Console.WriteLine("3. Doctor");
                Console.WriteLine("4. Exit");
                Console.Write("Choice: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AdminMenu(userManager);
                        break;
                    case "2":
                        UserMenu(userManager);
                        break;
                    case "3":
                        DoctorMenu(doctorManager);
                        break;
                    case "4":
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid choice, please enter 1–4.");
                        break;
                }
            }
        }

        static void AdminMenu(UserManager userManager)
        {
            while (true)
            {
                Console.WriteLine("\n--- Admin ---");
                Console.WriteLine("1. Sign In");
                Console.WriteLine("2. Back");
                Console.Write("Choice: ");
                var input = Console.ReadLine();

                if (input == "1")
                {
                    Console.Write("Username: ");
                    var username = Console.ReadLine();
                    Console.Write("Password: ");
                    var password = Console.ReadLine();

                    var result = userManager.SignIn(username, password);
                    if (result is Admin admin)
                    {
                        Console.WriteLine("Admin signed in successfully.");

                        while (true)
                        {
                            Console.WriteLine("\n--- Admin Panel ---");
                            Console.WriteLine("1. View Doctor Applications");
                            Console.WriteLine("2. Approve/Reject Application");
                            Console.WriteLine("3. Logout");
                            Console.Write("Choice: ");
                            var adminChoice = Console.ReadLine();

                            if (adminChoice == "1")
                            {
                                admin.ViewAllDoctorApplications();
                            }
                            else if (adminChoice == "2")
                            {
                                Console.Write("Enter doctor email to update status: ");
                                var email = Console.ReadLine();
                                Console.Write("Enter new status (Approved / Rejected): ");
                                var newStatus = Console.ReadLine();
                                //3
                                //admin.UpdateApplicationStatus(email, newStatus);
                            }
                            else if (adminChoice == "3")
                            {
                                Console.WriteLine("Logged out.");
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Invalid choice.");
                            }
                        }
                    }
                }
                else if (input == "2")
                {
                    return;
                }
                else
                {
                    Console.WriteLine("Invalid choice, please enter 1 or 2.");
                }
            }
        }

        static void UserMenu(UserManager userManager)
        {
            while (true)
            {
                Console.WriteLine("\n--- User ---");
                Console.WriteLine("1. Sign In");
                Console.WriteLine("2. Sign Up");
                Console.WriteLine("3. Back");
                Console.Write("Choice: ");
                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        Console.Write("Username: ");
                        var username = Console.ReadLine();
                        Console.Write("Password: ");
                        var password = Console.ReadLine();

                        var user = userManager.SignIn(username, password) as User;
                        if (user != null)
                        {
                            Console.WriteLine($"Welcome, {user.FirstName}!");
                            Console.WriteLine("\nŞöbə seçin:");
                            Console.WriteLine("1. Pediatriya");
                            Console.WriteLine("2. Travmatologiya");
                            Console.WriteLine("3. Stomatologiya");
                            Console.Write("Choice: ");
                            var selected = Console.ReadLine();
                            var pediatryDepartmentDoctors = new List<Doctor>
                            {
                                new Doctor("Lalə", "Məmmədova", 23),
                                new Doctor("Şahin", "Xəlilov", 20),
                                new Doctor("Anar", "Əmrah", 19)
                            };

                            var travmatologiyaDepartmentDoctors = new List<Doctor>
                            {
                                new Doctor("Ceyhun", "İsmayılov", 12),
                                new Doctor("Altay", "Cəfərov", 30),
                            };

                            var stomatologiyaDepartmentDoctors = new List<Doctor>
                            {
                                new Doctor("Xəyalə", "Həsənova", 5),
                                new Doctor("Rauf", "Səmədov", 10),
                                new Doctor("Püstə", "Xəlilova", 12),
                                new Doctor("Tural", "Rəhimli", 9)
                            };

                            var departments = new Dictionary<string, List<Doctor>>
                            {
                                { "1", pediatryDepartmentDoctors },
                                { "2", travmatologiyaDepartmentDoctors },
                                { "3", stomatologiyaDepartmentDoctors }
                            };


                            if (!departments.ContainsKey(selected))
                            {
                                Console.WriteLine("Yanlış seçim, yenidən cəhd edin.");
                                continue;
                            }

                            var selectedDoctors = departments[selected];
                            Console.WriteLine("\nHəkimləri seçin:");
                            for (int i = 0; i < selectedDoctors.Count; i++)
                            {
                                var d = selectedDoctors[i];
                                Console.WriteLine(
                                    $"{i + 1}. {d.Firstname} {d.Lastname} - {d.WorkExperience} il təcrübə");
                            }

                            Console.Write("Choice: ");
                            if (!int.TryParse(Console.ReadLine(), out int docIndex) ||
                                docIndex < 1 || docIndex > selectedDoctors.Count)
                            {
                                Console.WriteLine("Yanlış seçim, yenidən cəhd edin.");
                                continue;
                            }

                            var selectedDoctor = selectedDoctors[docIndex - 1];
                            var reservation = new ReservationManager();
                            var allDoctors = new List<Doctor>();
                            allDoctors.AddRange(pediatryDepartmentDoctors);
                            allDoctors.AddRange(travmatologiyaDepartmentDoctors);
                            allDoctors.AddRange(stomatologiyaDepartmentDoctors);

                            reservation.LoadReservationsFromFile(allDoctors);

                            while (true)
                            {
                                Console.WriteLine("\nMümkün saatlar:");
                                for (int i = 0; i < selectedDoctor.WorkingHours.Count; i++)
                                {
                                    var slot = selectedDoctor.WorkingHours[i];
                                    var status = slot.IsReserved ? "rezerv olunub" : "boşdur";
                                    Console.WriteLine($"{i + 1}. {slot.TimeRange} - {status}");
                                }

                                Console.Write("Choice: ");
                                if (!int.TryParse(Console.ReadLine(), out int slotIndex) ||
                                    slotIndex < 1 || slotIndex > selectedDoctor.WorkingHours.Count)
                                {
                                    Console.WriteLine("Yanlış seçim, yenidən cəhd edin.");
                                    continue;
                                }

                                var chosen = selectedDoctor.WorkingHours[slotIndex - 1];
                                if (chosen.IsReserved)
                                {
                                    Console.WriteLine("Bu saat artıq rezerv olunub.");
                                    continue;
                                }

                                reservation.ReserveAppointment(user, selectedDoctor, chosen.TimeRange);
                                Console.WriteLine(
                                    $"Təşəkkürlər {user.FirstName}, {chosen.TimeRange} saatında {selectedDoctor.Firstname} həkimin qəbuluna yazıldınız.");
                                break;
                            }
                        }

                        break;
                    case "2":
                        Console.Write("First Name: ");
                        var fn = Console.ReadLine();
                        Console.Write("Last Name: ");
                        var ln = Console.ReadLine();
                        Console.Write("Email: ");
                        var em = Console.ReadLine();
                        Console.Write("Username: ");
                        var un = Console.ReadLine();
                        Console.Write("Password: ");
                        var pw = Console.ReadLine();

                        userManager.SignUp(fn, ln, em, un, pw);
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Invalid choice, please enter 1–3.");
                        break;
                }
            }
        }

        static void DoctorMenu(DoctorAccountManager doctorManager)
        {
            while (true)
            {
                Console.WriteLine("\n--- Doctor ---");
                Console.WriteLine("1. Sign In");
                Console.WriteLine("2. Sign Up");
                Console.WriteLine("3. Back");
                Console.Write("Choice: ");
                var input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        Console.Write("Email: ");
                        var email = Console.ReadLine();
                        Console.Write("Password: ");
                        var pw = Console.ReadLine();
                        var doc = doctorManager.SignIn(email, pw) as Doctor;
                        if (doc != null)
                        {
                            Console.WriteLine($"Welcome Dr. {doc.Firstname}!");

                            while (true)
                            {
                                Console.WriteLine("\n--- Doctor Panel ---");
                                Console.WriteLine("1. Apply for Job");
                                Console.WriteLine("2. View Profile");
                                Console.WriteLine("3. Logout");
                                Console.Write("Choice: ");
                                var doctorChoice = Console.ReadLine();

                                if (doctorChoice == "1")
                                {
                                    Console.Write("Department you are applying for: ");
                                    var department = Console.ReadLine();
                                    Console.Write("Motivation Letter: ");
                                    var motivation = Console.ReadLine();

                                    doc.ApplyJob(department, motivation);
                                }
                                else if (doctorChoice == "2")
                                {
                                    Console.WriteLine($"Name: Dr. {doc.Firstname} {doc.Lastname}");
                                    Console.WriteLine($"Email: {doc.Email}");
                                    Console.WriteLine($"Experience: {doc.WorkExperience} years");
                                }
                                else if (doctorChoice == "3")
                                {
                                    Console.WriteLine("Logged out successfully.");
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("Invalid choice, try again.");
                                }
                            }
                        }
                        break;

                    case "2":
                        Console.Write("First Name: ");
                        var firstName = Console.ReadLine();
                        Console.Write("Last Name: ");
                        var lastName = Console.ReadLine();
                        Console.Write("workExperience: ");
                        var workExpInput = Console.ReadLine();

                        if (!int.TryParse(workExpInput, out int workExperience) || workExperience < 0)
                        {
                            Console.WriteLine(
                                "Invalid input. Please enter a non‐negative whole number for work experience.");
                            return;
                        }

                        Console.Write("Email: ");
                        var doctorEmail = Console.ReadLine();
                        Console.Write("Password: ");
                        var password = Console.ReadLine();

                        doctorManager.SignUp(firstName, lastName, workExperience, doctorEmail, password);
                        break;

                    case "3":
                        return;
                    default:
                        Console.WriteLine("Invalid choice, please enter 1–3.");
                        break;
                }
            }
        }
    }
}
