namespace zad7.Model;

public class BadRequestExeption :  Exception
{
    public BadRequestExeption(string message) :  base(message) 
    {
    }
}