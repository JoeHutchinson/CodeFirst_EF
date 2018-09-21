namespace CodeFirst_EF.Security
{
    public interface IHashProvider
    {
        HashResult CreateHash(string password, string salt);
    }
}
