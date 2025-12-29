using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleverWeb.Migrations
{
    /// <inheritdoc />
    public partial class InsertMembers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Membro",
                columns: new[] { "Nome", "Email", "DataNascimento", "Telefone", "DataCadastro" },
                values: new object[,]
                {
            {  "Albert Galileu Ferreira", "albert.ferreira@email.com", new DateTime(1985, 3, 12), "(85) 98801-0001", DateTime.Now },
            {  "Allan Ricardo", "allan.ricardo@email.com", new DateTime(1990, 7, 22), "(85) 98801-0002", DateTime.Now },
            {  "Anderson Batista Dourado", "anderson.dourado@email.com", new DateTime(1988, 1, 18), "(85) 98801-0003", DateTime.Now },
            {  "Antonia Garcia", "antonia.garcia@email.com", new DateTime(1975, 9, 5), "(85) 98801-0004", DateTime.Now },
            {  "Bernarda Costa", "bernarda.costa@email.com", new DateTime(1972, 11, 30), "(85) 98801-0005", DateTime.Now },
            {  "Carla Maressa Dias", "carla.dias@email.com", new DateTime(1994, 4, 14), "(85) 98801-0006", DateTime.Now },
            {  "Dalvina Farias do Nascimento", "dalvina.nascimento@email.com", new DateTime(1968, 6, 21), "(85) 98801-0007", DateTime.Now },
            {  "Daniele Moreira Freire", "daniele.freire@email.com", new DateTime(1992, 10, 9), "(85) 98801-0008", DateTime.Now },
            {  "Elen Cristina Vieira Silva", "elen.silva@email.com", new DateTime(1989, 2, 26), "(85) 98801-0009", DateTime.Now },
            {  "Eloah Vieira de Abreu", "eloah.abreu@email.com", new DateTime(2001, 8, 17), "(85) 98801-0010", DateTime.Now },
            {  "Eunice Maria B Monteiro", "eunice.monteiro@email.com", new DateTime(1965, 12, 3), "(85) 98801-0011", DateTime.Now },
            {  "Francisca Monteiro da Silva", "francisca.silva@email.com", new DateTime(1970, 5, 28), "(85) 98801-0012", DateTime.Now },
            {  "Geneci da Silva Bezerra", "geneci.bezerra@email.com", new DateTime(1969, 1, 11), "(85) 98801-0013", DateTime.Now },
            {  "Isabella Vitoria", "isabella.vitoria@email.com", new DateTime(2003, 3, 19), "(85) 98801-0014", DateTime.Now },
            {  "José Gomes Feitosa", "jose.feitosa@email.com", new DateTime(1980, 7, 7), "(85) 98801-0015", DateTime.Now },
            {  "Jovita Batista", "jovita.batista@email.com", new DateTime(1978, 9, 25), "(85) 98801-0016", DateTime.Now },
            {  "Kaua Romão", "kaua.romao@email.com", new DateTime(2004, 11, 2), "(85) 98801-0017", DateTime.Now },
            {  "Lusineide Sena Feitosa", "lusineide.feitosa@email.com", new DateTime(1983, 6, 13), "(85) 98801-0018", DateTime.Now },
            {  "Marcelo Sousa da Silva", "marcelo.silva@email.com", new DateTime(1986, 4, 1), "(85) 98801-0019", DateTime.Now },
            {  "Maria Irene Alves", "maria.alves@email.com", new DateTime(1974, 8, 18), "(85) 98801-0020", DateTime.Now },
            {  "Maria das Graças Almeida", "gracas.almeida@email.com", new DateTime(1967, 2, 9), "(85) 98801-0021", DateTime.Now },
            {  "Maxwel Correia Reis", "maxwel.reis@email.com", new DateTime(1991, 12, 15), "(85) 98801-0022", DateTime.Now },
            {  "Messias Soares de Oliveira", "messias.oliveira@email.com", new DateTime(1982, 10, 4), "(85) 98801-0023", DateTime.Now },
            {  "Mirian da S. Ferreira", "mirian.ferreira@email.com", new DateTime(1987, 3, 27), "(85) 98801-0024", DateTime.Now },
            {  "Neiridnalva Sena Feitosa", "neiridnalva.feitosa@email.com", new DateTime(1976, 5, 6), "(85) 98801-0025", DateTime.Now },
            {  "Nilza Martins", "nilza.martins@email.com", new DateTime(1971, 9, 14), "(85) 98801-0026", DateTime.Now },
            {  "Pr. Fernando Abreu", "fernando.abreu@email.com", new DateTime(1984, 1, 20), "(85) 98801-0027", DateTime.Now },
            {  "Rayane Rafaela Dias Romão", "rayane.romao@email.com", new DateTime(1998, 7, 29), "(85) 98801-0028", DateTime.Now },
            {  "Samantha Castro", "samantha.castro@email.com", new DateTime(1995, 11, 8), "(85) 98801-0029", DateTime.Now },
            {  "Sirley Sousa de Castro", "sirley.castro@email.com", new DateTime(1979, 6, 24), "(85) 98801-0030", DateTime.Now },
            {  "Stefani Madusa", "stefani.madusa@email.com", new DateTime(1996, 2, 16), "(85) 98801-0031", DateTime.Now },
            {  "Thayla dos Santos Oliveira", "thayla.oliveira@email.com", new DateTime(2000, 9, 3), "(85) 98801-0032", DateTime.Now },
            {  "Valdemar e Família", "valdemar.familia@email.com", new DateTime(1970, 1, 1), "(85) 98801-0033", DateTime.Now }
                });
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
