using DesafioAuvo.Core.Interfaces.Repositories;

namespace DesafioAuvo.Infrastructure.Repositories
{
    public class FileRepository : IFileRepository
    {
        public async Task<IEnumerable<string>> GetFileContents(string path)
        {
            return await Task.Run(() => {
                var fileContents = new List<string>();

                var lines = File.ReadAllLines(path).Skip(1);

                if (lines != null)
                {
                    fileContents.AddRange(lines);
                }

                return fileContents;
            });
            
        }

        public FileInfo[] GetAllCsvFilenames(string path)
        {
            DirectoryInfo directoryFiles = new DirectoryInfo(path);
            FileInfo[] files = directoryFiles.GetFiles("*.csv");

            return files;
        }
    }
}
