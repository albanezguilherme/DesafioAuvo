namespace DesafioAuvo.Core.Entities
{
    public  class FileData
    {
        public int Codigo { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal ValorHora { get; set; }
        public DateOnly Data { get; set; }
        public TimeOnly Entrada { get; set; }
        public TimeOnly Saida { get; set; }
        public TimeSpan Almoco { get; set; }
    }
}
