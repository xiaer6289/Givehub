namespace Givehub.Helpers;

public class NotLoggedInException : Exception
{
    public NotLoggedInException() : base("Donor not logged in.") { }
}