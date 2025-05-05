using Microsoft.EntityFrameworkCore;
using OneBeyondApi.Model;

namespace OneBeyondApi.DataAccess
{
    public class LibraryContext: DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "AuthorDb");
        }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookStock> Catalogue { get; set; }
        public DbSet<Borrower> Borrowers { get; set; }
        public DbSet<Fine> Fines { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fine>()
                .HasOne(f => f.Borrower)
                .WithMany()
                .HasForeignKey(f => f.BorrowerId);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Borrower)
                .WithMany()
                .HasForeignKey(r => r.BorrowerId);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Book)
                .WithMany()
                .HasForeignKey(r => r.BookId);
        }
    }
}
