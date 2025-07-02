//using DoctorNamespace;
//using UserManagerNamespace;
//using UserNamespace;

//namespace CSharp_Hospital
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            Console.OutputEncoding = System.Text.Encoding.UTF8;

//            var userManager = new UserManager();
//            User currentUser = null;

//            string[] loginOptions = { "Qeydiyyatdan keçin", "Daxil olun" };
//            int loginChoiceIndex = 0;

//            while (currentUser == null)
//            {
//                Console.Clear();
//                Console.WriteLine("Mərkəzi Xəstəxana Qəbul Sisteminə Xoş Gəlmisiniz");

//                ConsoleKey key;
//                do
//                {
//                    Console.SetCursorPosition(0, 3);
//                    for (int i = 0; i < loginOptions.Length; i++)
//                    {
//                        if (i == loginChoiceIndex)
//                        {
//                            Console.BackgroundColor = ConsoleColor.Gray;
//                            Console.ForegroundColor = ConsoleColor.Black;
//                        }
//                        Console.WriteLine($"{i + 1}. {loginOptions[i]}     ");
//                        Console.ResetColor();
//                    }

//                    key = Console.ReadKey(true).Key;

//                    if (key == ConsoleKey.UpArrow && loginChoiceIndex > 0)
//                        loginChoiceIndex--;
//                    else if (key == ConsoleKey.DownArrow && loginChoiceIndex < loginOptions.Length - 1)
//                        loginChoiceIndex++;

//                    Console.SetCursorPosition(0, 3);
//                } while (key != ConsoleKey.Enter);

//                Console.Clear();
//                Console.Write("İstifadəçi adınızı daxil edin: ");
//                string username = Console.ReadLine();

//                Console.Write("Email daxil edin: ");
//                string email = Console.ReadLine();

//                if (loginChoiceIndex == 0) // Sign Up
//                {
//                    userManager.SignUp(username, email);
//                    Console.WriteLine("Qeydiyyat tamamlandı. Daxil olursunuz...");
//                    Thread.Sleep(1000);

//                    currentUser = userManager.SignIn(username, email);
//                    if (currentUser == null)
//                    {
//                        Console.WriteLine("Avtomatik giriş alınmadı. Yenidən yoxlayın.");
//                        Thread.Sleep(1000);
//                    }
//                }
//                else // Sign In
//                {
//                    currentUser = userManager.SignIn(username, email);
//                    if (currentUser == null)
//                    {
//                        Console.WriteLine("Giriş alınmadı. Məlumatları düzgün daxil etdiyinizə əmin olun.");
//                        Thread.Sleep(1000);
//                    }
//                }
//            }

//            // həkimlər və şöbələr
//            var pediatryDepartmentDoctors = new List<Doctor>
//            {
//                new Doctor("Lalə", "Məmmədova", 23),
//                new Doctor("Şahin", "Xəlilov", 20),
//                new Doctor("Anar", "Əmrah", 19)
//            };

//            var travmatologiyaDepartmentDoctors = new List<Doctor>
//            {
//                new Doctor("Ceyhun", "İsmayılov", 12),
//                new Doctor("Altay", "Cəfərov", 30),
//            };

//            var stomatologiyaDepartmentDoctors = new List<Doctor>
//            {
//                new Doctor("Xəyalə", "Həsənova", 5),
//                new Doctor("Rauf", "Səmədov", 10),
//                new Doctor("Püstə", "Xəlilova", 12),
//                new Doctor("Tural", "Rəhimli", 9)
//            };

//            var departments = new Dictionary<string, List<Doctor>>
//            {
//                { "1", pediatryDepartmentDoctors },
//                { "2", travmatologiyaDepartmentDoctors },
//                { "3", stomatologiyaDepartmentDoctors }
//            };

//            while (true)
//            {
//                Console.Clear();
//                Console.WriteLine("=== Qəbul üçün məlumatlar ===");
//                Console.Write("Adınızı daxil edin: ");
//                string firstname = Console.ReadLine();

//                Console.Write("Soyadınızı daxil edin: ");
//                string lastname = Console.ReadLine();

//                Console.Write("Telefon nömrənizi daxil edin: ");
//                string phone = Console.ReadLine();

//                var user = new User
//                {
//                    FirstName = firstname,
//                    LastName = lastname,
//                    Email = currentUser.Email,
//                    UserName = currentUser.UserName,
//                    PhoneNumber = phone
//                };

//                string[] departmentNames = { "Pediatriya", "Travmatologiya", "Stomatologiya" };
//                int departmentIndex = 0;

//                Console.WriteLine("\n↑ ↓ düymələri ilə şöbə seçin, Enter ilə təsdiqləyin");
//                ConsoleKey deptKey;
//                do
//                {
//                    Console.SetCursorPosition(0, 8);
//                    for (int i = 0; i < departmentNames.Length; i++)
//                    {
//                        if (i == departmentIndex)
//                        {
//                            Console.BackgroundColor = ConsoleColor.Gray;
//                            Console.ForegroundColor = ConsoleColor.Black;
//                        }
//                        Console.WriteLine($"{i + 1}. {departmentNames[i]}     ");
//                        Console.ResetColor();
//                    }

//                    deptKey = Console.ReadKey(true).Key;

//                    if (deptKey == ConsoleKey.UpArrow && departmentIndex > 0)
//                        departmentIndex--;
//                    else if (deptKey == ConsoleKey.DownArrow && departmentIndex < departmentNames.Length - 1)
//                        departmentIndex++;

//                } while (deptKey != ConsoleKey.Enter);

//                string selectedDepartment = (departmentIndex + 1).ToString();
//                var selectedDoctors = departments[selectedDepartment];

//                int doctorIndex = 0;
//                Console.Clear();
//                Console.WriteLine("↑ ↓ düymələri ilə həkim seçin, Enter ilə təsdiqləyin");

//                ConsoleKey docKey;
//                do
//                {
//                    Console.SetCursorPosition(0, 2);
//                    for (int i = 0; i < selectedDoctors.Count; i++)
//                    {
//                        var d = selectedDoctors[i];
//                        if (i == doctorIndex)
//                        {
//                            Console.BackgroundColor = ConsoleColor.Gray;
//                            Console.ForegroundColor = ConsoleColor.Black;
//                        }
//                        Console.WriteLine($"{i + 1}. {d.Firstname} {d.Lastname} - {d.WorkExperience} il təcrübə     ");
//                        Console.ResetColor();
//                    }

//                    docKey = Console.ReadKey(true).Key;
//                    if (docKey == ConsoleKey.UpArrow && doctorIndex > 0)
//                        doctorIndex--;
//                    else if (docKey == ConsoleKey.DownArrow && doctorIndex < selectedDoctors.Count - 1)
//                        doctorIndex++;

//                } while (docKey != ConsoleKey.Enter);

//                var selectedDoctor = selectedDoctors[doctorIndex];
//                var reservation = new ReservationManager();

//                var allDoctors = new List<Doctor>();
//                allDoctors.AddRange(pediatryDepartmentDoctors);
//                allDoctors.AddRange(travmatologiyaDepartmentDoctors);
//                allDoctors.AddRange(stomatologiyaDepartmentDoctors);

//                reservation.LoadReservationsFromFile(allDoctors);

//                bool reservedHour = false;
//                while (!reservedHour)
//                {
//                    int slotIndex = 0;
//                    Console.Clear();
//                    Console.WriteLine("\n↑ ↓ düymələri ilə saat seçin, Enter ilə təsdiqləyin");

//                    ConsoleKey slotKey;
//                    do
//                    {
//                        Console.SetCursorPosition(0, 2);
//                        for (int i = 0; i < selectedDoctor.WorkingHours.Count; i++)
//                        {
//                            var slot = selectedDoctor.WorkingHours[i];
//                            string status = slot.IsReserved ? "rezerv olunub" : "boşdur";

//                            if (i == slotIndex)
//                            {
//                                Console.BackgroundColor = ConsoleColor.Gray;
//                                Console.ForegroundColor = ConsoleColor.Black;
//                            }
//                            Console.WriteLine($"{i + 1}. {slot.TimeRange} - {status}     ");
//                            Console.ResetColor();
//                        }

//                        slotKey = Console.ReadKey(true).Key;
//                        if (slotKey == ConsoleKey.UpArrow && slotIndex > 0)
//                            slotIndex--;
//                        else if (slotKey == ConsoleKey.DownArrow && slotIndex < selectedDoctor.WorkingHours.Count - 1)
//                            slotIndex++;

//                    } while (slotKey != ConsoleKey.Enter);

//                    var selectedSlot = selectedDoctor.WorkingHours[slotIndex];
//                    if (!selectedSlot.IsReserved)
//                    {
//                        reservation.ReserveAppointment(user, selectedDoctor, selectedSlot.TimeRange);
//                        Console.WriteLine($"\nTəşəkkürlər {user.FirstName}, {selectedSlot.TimeRange} saatında {selectedDoctor.Firstname} həkimin qəbuluna yazıldınız.");
//                        reservedHour = true;
//                    }
//                    else
//                    {
//                        Console.WriteLine("Bu saat artıq rezerv olunub. Başqa birini seçin.");
//                        Thread.Sleep(1000);
//                    }
//                }

//                Console.WriteLine("\nYeni istifadəçi üçün davam etmək üçün Enter, çıxmaq üçün 'q' basın.");
//                string choice = Console.ReadLine();
//                if (choice.ToLower() == "q")
//                    break;
//            }
//        }
//    }
//}
