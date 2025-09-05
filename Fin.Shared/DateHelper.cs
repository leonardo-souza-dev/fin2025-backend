namespace Fin.Shared;

public class DateHelper
{
    public static bool IsObj1BeforeObj2(int year1, int month1, int year2, int month2)
    {
        var date1 = new DateTime(year1, month1, 1);
        var date2 = new DateTime(year2, month2, 1);

        var obj1IsBeforeObj2 = DateTime.Compare(date2, date1);

        return obj1IsBeforeObj2 > 0;
    }
}
