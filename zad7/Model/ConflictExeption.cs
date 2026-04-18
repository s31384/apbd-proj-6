namespace zad7.Model;

public class ConflictExeption : Exception
{
    public ConflictExeption(string message) :  base(message) 
    {
    }
}