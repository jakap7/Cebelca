using Cebelica.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
public class CebelicaDatabase : DbContext
{
    public DbSet<ProductsModel> Products { get; set; }

    public CebelicaDatabase(DbContextOptions<CebelicaDatabase> options)
    : base(options)
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Data Source=DESKTOP-DA40B13;Initial Catalog=Cebelica;Integrated Security=True;Encrypt=False;Trust Server Certificate=True");
        //base.OnConfiguring(optionsBuilder);
    }
}
