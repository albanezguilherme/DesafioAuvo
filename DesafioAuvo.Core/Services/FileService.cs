using DesafioAuvo.Core.Entities;
using DesafioAuvo.Core.Interfaces.Repositories;
using DesafioAuvo.Core.Interfaces.Services;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace DesafioAuvo.Core.Services
{
    public class FileService : IFileService
    {
        IFileRepository _fileRepository;

        public FileService(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public async Task<string> ProcessDepartmentFiles(string path)
        {
            var fileNames = _fileRepository.GetAllCsvFilenames(path);
            LinkedList<Department> departmentList = new LinkedList<Department>();

            await Parallel.ForEachAsync(fileNames, async (file, ct) =>
            {
                var fileContent = await _fileRepository.GetFileContents(file.FullName);
                departmentList.AddLast(ProcessEmployeeDataFromLine(fileContent, file.Name).Result);
            });

            return JsonSerializer.Serialize(departmentList, new JsonSerializerOptions { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)});
        }

        private async Task<Department> ProcessEmployeeDataFromLine(IEnumerable<string> lineData, string fileName)
        {
            return await Task.Run(() =>
            {
                var department = new Department();

                var departmentInfo = fileName.Split('-');
                department.Departamento = departmentInfo[0];
                department.MesVigencia = departmentInfo[1];

                department.AnoVigencia = Convert.ToInt32(departmentInfo[2][..^4]);

                var fileDataList = new List<FileData>();

                foreach (var line in lineData)
                {
                    var info = line.Split(';');
                    var fileData = new FileData();
                    fileData.Codigo = Convert.ToInt32(info[0]);
                    fileData.Nome = info[1];
                    fileData.ValorHora = Decimal.Parse(info[2][2..]);
                    fileData.Data = DateOnly.Parse(info[3]);
                    fileData.Entrada = TimeOnly.Parse(info[4]);
                    fileData.Saida = TimeOnly.Parse(info[5]);
                    fileData.Almoco = TimeOnly.Parse(info[6].Split('-')[1]) - TimeOnly.Parse(info[6].Split('-')[0]);
                    fileDataList.Add(fileData);
                }

                var culture = new CultureInfo("pt-BR");

                var fileDataGroup = fileDataList.GroupBy(f => f.Nome);
                int fileMonth = DateTime.ParseExact(department.MesVigencia.Trim(), "MMMM", new CultureInfo("pt-BR")).Month;
                var businessDays = Enumerable.Range(1, DateTime.DaysInMonth(department.AnoVigencia, fileMonth))
                        .Select(day => new DateTime(department.AnoVigencia, fileMonth, day))
                        .Where(dt => dt.DayOfWeek != DayOfWeek.Sunday &&
                                     dt.DayOfWeek != DayOfWeek.Saturday).Count();
                int workedDays = 0;
                decimal totalDepartmentDiscounts = 0;
                decimal totalDepartmentExtras = 0;

                foreach (var dataList in fileDataGroup)
                {

                    var employee = new Employee();
                    employee.Nome = dataList.Key;
                    decimal totalAmount = 0;
                    decimal totalEmployeeDiscounts = 0;
                    decimal totalEmployeeExtra = 0;

                    foreach (var data in dataList)
                    {
                        employee.Codigo = data.Codigo;
                        workedDays++;

                        if (IsWeekend(data.Data))
                        {
                            employee.DiasExtras++;
                            workedDays--;

                            decimal weekendExtra = CalculateWorkedHours(data).Result;
                            employee.HorasExtras += weekendExtra;
                            totalEmployeeExtra += weekendExtra * data.ValorHora;
                        }
                        else
                        {
                            decimal debtAndExtraHours = CalculateDebtAndExtraHours(data).Result;
                            if (debtAndExtraHours < 0)
                            {
                                employee.HorasDebito += debtAndExtraHours;
                                totalEmployeeDiscounts += debtAndExtraHours * data.ValorHora;
                            }
                            else
                            {
                                employee.HorasExtras += debtAndExtraHours;
                                totalEmployeeExtra += debtAndExtraHours * data.ValorHora;
                            }
                            totalAmount += CalculateHoursValue(data).Result;
                        }
                    }

                    totalDepartmentDiscounts += totalEmployeeDiscounts;
                    totalDepartmentExtras += totalEmployeeExtra;

                    employee.DiasFalta = businessDays - workedDays;
                    employee.DiasTrabalhados = workedDays;
                    employee.TotalReceber = totalAmount + totalEmployeeExtra;

                    department.Employees.Add(employee);

                    workedDays = 0;
                }

                department.TotalDescontos = totalDepartmentDiscounts;
                department.TotalExtras = totalDepartmentExtras;
                department.TotalPagar = department.Employees.Sum(e => e.TotalReceber);

                return department;

            });
        }

        private async Task<decimal> CalculateHoursValue(FileData data)
        {
            return await Task.Run(() => (data.Saida.Hour - data.Entrada.Hour - data.Almoco.Hours) * data.ValorHora);
        }

        private async Task<decimal> CalculateDebtAndExtraHours(FileData data)
        {            
            return await Task.Run(() => data.Saida.Hour - data.Entrada.Hour - data.Almoco.Hours - 9);
        }

        private async Task<decimal> CalculateWorkedHours(FileData data)
        {
            return await Task.Run(() => data.Saida.Hour - data.Entrada.Hour - data.Almoco.Hours);
        }

        private bool IsWeekend(DateOnly data)
        {
            return data.DayOfWeek == DayOfWeek.Saturday || data.DayOfWeek == DayOfWeek.Sunday;            
        }
    }
}
