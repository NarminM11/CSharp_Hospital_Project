using UserNamespace;
namespace AppointmentTimeNamespace;

public class AppointmentTime
{
    public DateTime Date { get; set; }
    public string TimeRange { get; set; }        //hekimin saat araliqlari  
    public bool IsReserved { get; set; } = false; //hemin saatlarda appointment var ya yox
    public User ReservedBy { get; set; } = null; //hansi user reserv edib o
}