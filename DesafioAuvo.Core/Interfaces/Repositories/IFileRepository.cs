namespace DesafioAuvo.Core.Interfaces.Repositories
{
    public interface IFileRepository
    {
        Task<IEnumerable<string>> GetFileContents(string path);
        FileInfo[] GetAllCsvFilenames(string path);
    }
}
