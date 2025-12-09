  using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
namespace Givehub.Models
{
    public class DB : DbContext
    {
        public DB(DbContextOptions<DB> options) : base(options) { }

        public DbSet<Donor> Donors { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<Donee> Donees { get; set; }

        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // donor to donation: no cascade
            modelBuilder.Entity<Donation>()
                .HasOne(d => d.Donors)
                .WithMany(p => p.Donations)
                .HasForeignKey(d => d.DonorId)
                .OnDelete(DeleteBehavior.NoAction);

            // Donee to Donation: no cascade
            modelBuilder.Entity<Donation>()
                .HasOne(d => d.Donees)
                .WithMany(p => p.Donations)
                .HasForeignKey(d => d.DoneeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PasswordResetToken>()
                .HasOne(p => p.Donor)
                .WithMany()
                .HasForeignKey(p => p.DonorId)
                .OnDelete(DeleteBehavior.Cascade);
        }


    }



    public class Donor
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(11)]
        [RegularExpression(@"^01[0-9]{8,13}$", ErrorMessage = "Phone number must start with '01' and be 10 to 15 digits long.")]
        public string PhoneNo { get; set; }

        [MaxLength(200)]
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{8,20}$",
        ErrorMessage = "Password must be 8 to 20 characters long, with at least one uppercase letter, one lowercase letter, one digit, and one special character (!@#$%^&*).")]
        public string Password { get; set; }

        public int AdminId { get; set; }
        public Admin Admins { get; set; }

        public ICollection<Donation> Donations { get; set; } = new List<Donation>();
    }

    public class Admin
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(200)]
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{8,20}$",
        ErrorMessage = "Password must be 8 to 20 characters long, with at least one uppercase letter, one lowercase letter, one digit, and one special character (!@#$%^&*).")]
        public string Password { get; set; }

        public ICollection<Donor> Donors { get; set; } = new List<Donor>();

        public ICollection<Donee> Donees { get; set; } = new List<Donee>();
    }

    public class Donation
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(20)]
        [Required]
        public string? Method { get; set; }

        [Precision(10, 2)]
        [Required]
        public decimal? Amount { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        public DateTime Date { get; set; }

        public string? StripeTransactionId { get; set; }

        [Column(TypeName = "nvarchar(max)")] // type for json purpose
        public string? ItemsJson { get; set; }

        [NotMapped]
        public Dictionary<string,int>? Items 
        { 
            get => string.IsNullOrEmpty(ItemsJson) ? null : JsonSerializer.Deserialize<Dictionary<string, int>>(ItemsJson);  //convert json string to object
            set => ItemsJson = value == null ? null : JsonSerializer.Serialize(value);  //convert object to json string
        }

        public int DoneeId { get; set; }
        public Donee Donees { get; set; }

        public int DonorId { get; set; }
        public Donor Donors { get; set; }
    }

    public class Donee
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        [Required]
        public DateTime Date { get; set; }

        public string? Description { get; set; }

        public string? Category { get; set; }  //for identify refugees, nursing home, orphanage

        public string Address { get; set; }

        public string? Image { get; set; }

        public string? Requirements { get; set; }

        public int AdminId { get; set; }
        public Admin Admins { get; set; }

        public ICollection<Donation> Donations { get; set; } = new List<Donation>();
    }

    public class PasswordResetToken
    {
        [Key]
        public int Id { get; set; }
        public int DonorId { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }

        [ForeignKey("DonorId")]
        public Donor Donor { get; set; }
    }
}
