namespace DesafioAuvo.Core.Entities
{
    public class Employee
    {
        public string Nome { get; set; } = "";
        public int Codigo { get; set; }
        public decimal TotalReceber { get; set; }
        public decimal HorasExtras { get; set; }
        public decimal HorasDebito { get; set; }
        public int DiasFalta { get; set; }
        public int DiasExtras { get; set; }
        public int DiasTrabalhados { get; set; }
    }
}
