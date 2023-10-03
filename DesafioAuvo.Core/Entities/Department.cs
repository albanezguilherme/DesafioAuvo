namespace DesafioAuvo.Core.Entities
{
    public class Department
    {
        public string Departamento { get; set; } = "";
        public string MesVigencia { get; set; } = "";
        public int AnoVigencia { get; set; }
        public decimal TotalPagar { get; set; }
        public decimal TotalDescontos { get; set; }
        public decimal TotalExtras { get; set; }
        public List<Employee> Employees { get; set; } = new List<Employee>();

    }
}
