using System.Data;

var builder = WebApplication.CreateBuilder();
var app = builder.Build();

List<OrderHotel> repo =
[
    new(1,"Саша", "Матвеев", "Евгеньевич", 89630348648, "Нету", "Вайнера 3А", "302", 1,1,2024, "Проживает"),
    new(2,"Кирилл", "Логинов", "Денисович", 895214448248, "Нету", "Вайнера 3А", "302", 2, 2,2024, "Выселен" ),
    new(3,"Кирилл", "Логинов", "Денисович", 895214448248, "Нету", "Вайнера 3А", "302", 2, 2,2024, "Проживает" ),
    new(4,"Кирилл", "Логинов", "Денисович", 895214448248, "Нету", "Вайнера 3А", "304", 2, 2,2024, "Проживает" ),
    new(5,"Кирилл", "Логинов", "Денисович", 895214448248, "Нету", "Вайнера 3А", "306", 2, 2,2024, "В ожидании" ),
    new(6,"Кирилл", "Логинов", "Денисович", 895214448248, "Нету", "Вайнера 3А", "300", 2, 2,2024, "Выселен" ),
    new(7,"Кирилл", "Логинов", "Денисович", 895214448248, "Нету", "Вайнера 3А", "308", 2, 2,2024, "Выселен" ),
    new(8,"Кирилл", "Логинов", "Денисович", 895214448248, "Нету", "Вайнера 3А", "308", 2, 2,2024, "Выселен" ),
];
bool isUpdateStatus = false;
string message = "";

app.MapGet("/", () =>
{
    if (isUpdateStatus)
    {
        string buffer = message;
        isUpdateStatus = false;
        message = "";
        return Results.Json(new OrderUpdatedStatusDTO(repo, buffer));
    }
    else
        return Results.Json(repo);
});

app.MapPost("/add", (OrderHotel o) => repo.Add(o));

app.MapPut("/{num}", (int num, OrderHotelUpdateDTO dto) =>
{
    OrderHotel buffer = repo.Find(o => o.Number == num);
    if (buffer.Master != dto.Master)
        buffer.Master = dto.Master;
    if (buffer.StartDate != dto.StartDate)
        buffer.StartDate = dto.StartDate;
    if (buffer.Wishes != dto.Wishes)
        buffer.Wishes = dto.Wishes;
    if (buffer.EndDate != dto.EndDate)
        buffer.EndDate = dto.EndDate;
    if (buffer.EndDate != dto.EndDate)
    {
        buffer.EndDate = dto.EndDate;
        isUpdateStatus = true;
        message += "Дата выезда" + buffer.Number + " обновлена\n";
        if (buffer.Status == "Выселен")
            buffer.EndDate = DateTime.Now;
    }
    if (!string.IsNullOrEmpty(dto.Comments))
        buffer.Comments.Add(dto.Comments);
    return Results.Json(buffer);

});
app.MapGet("/{num}", (int num) => repo.Find(o => o.Number == num));

app.MapGet("/stat/readyOrder", () => repo.FindAll(o => o.Status == "Выселен").Count);

//app.MapGet("/stat/Avg", () =>
//{
//    double timesum = 0;
////   int oCount = 0;//    foreach (var o in repo)
//        if (o.Status == "Выселен")
//        {
//            oCount++;
//            timesum += o.TimeInDay();
//        }
//    return oCount > 0 ? timesum / oCount : 0;
//}); Не успел реализовать ошибка была


app.MapGet("/stat/countnum", () =>
{
    Dictionary<string, int> result = new();
    foreach (var o in repo)
        if (result.ContainsKey(o.AppartamentNum))
            result[o.AppartamentNum]++;
        else
            result[o.AppartamentNum] = 1;
    return result;
});

app.Run();
record class OrderHotelUpdateDTO(DateTime StartDate, string Wishes, DateTime EndDate, string Comments, string Master);
record class OrderUpdatedStatusDTO(List<OrderHotel> repo, string message);
class OrderHotel
{
    int number;
    string name;
    string surName;
    string lastName;
    long phoneNum;
    string wishes;
    string hotelAdress;
    string appartamentNum;
    string status;
    string master;

    public OrderHotel(int number, string name, string surName, string lastName, long phoneNum, string wishes, string hotelAdress, string appartamentNum, int day, int month, int year, string status)
    {
        Number = number;
        Name = name;
        SurName = surName;
        LastName = lastName;
        PhoneNum = phoneNum;
        Wishes = wishes;
        HotelAdress = hotelAdress;
        AppartamentNum = appartamentNum;
        StartDate = new DateTime(year, month, day);
        EndDate = null;
        Status = status;
        master = "Не назначен";

    }

    public int Number { get => number; set => number = value; }
    public string Name { get => name; set => name = value; }
    public string SurName { get => surName; set => surName = value; }
    public string LastName { get => lastName; set => lastName = value; }
    public long PhoneNum { get => phoneNum; set => phoneNum = value; }
    public string Wishes { get => wishes; set => wishes = value; }
    public string HotelAdress { get => hotelAdress; set => hotelAdress = value; }
    public string AppartamentNum { get => appartamentNum; set => appartamentNum = value; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get => status; set => status = value; }

    public string Master { get => master; set => master = value; }

    public List<string> Comments { get; set; } = new();
    public double TimeInDay() => (EndDate - StartDate).Value.TotalDays;
}