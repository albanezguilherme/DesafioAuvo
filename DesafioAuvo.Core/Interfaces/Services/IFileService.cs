namespace DesafioAuvo.Core.Interfaces.Services
{
    public interface IFileService
    {
        Task<string> ProcessDepartmentFiles(string path);
    }
}
