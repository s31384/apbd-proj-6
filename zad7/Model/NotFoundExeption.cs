namespace zad7.Model;

public class NotFoundExeption : Exception
{
    public NotFoundExeption(string message) :  base(message) 
    {
    }
}