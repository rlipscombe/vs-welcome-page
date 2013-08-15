namespace WelcomePage.Core
{
    public interface IDocumentFolder
    {
        bool Exists(string name);
        IDocumentFile Open(string name);
    }

    public interface IDocumentFile
    {
        string ReadAllText();
    }
}