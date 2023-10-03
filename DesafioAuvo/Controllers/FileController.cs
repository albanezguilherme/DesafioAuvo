using DesafioAuvo.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace DesafioAuvo.Controllers
{
    public class FileController : Controller
    {
        private IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task<string> ProcessFiles(string filePath)
        {           
            return await _fileService.ProcessDepartmentFiles(filePath);
        }
    }
}
