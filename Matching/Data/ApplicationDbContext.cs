using Matching.Model;
using Microsoft.EntityFrameworkCore;

namespace Matching.Data
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            
            builder.Entity<PersonalPreferences>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete

            builder.Entity<PotentialPartnerPreferences>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete


            builder.Entity<Ticket>()
                .HasOne(x => x.Room)
                .WithMany()
                .HasForeignKey(x => x.RoomId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Ticket>()
                .HasOne(x => x.Creator)
                .WithMany()
                .HasForeignKey(x => x.CreatorId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<UserTicket>()
                .HasOne(x=>x.Sender)
                .WithMany()
                .HasForeignKey(x=>x.SenderId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<UserTicket>()
                .HasOne(x => x.Receiver)
                .WithMany()
                .HasForeignKey(x => x.ReceiverId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<UserTicket>()
                .HasOne(x => x.Ticket)
                .WithMany()
                .HasForeignKey(x => x.TicketId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Request>()
                .HasOne(x => x.Sender)
                .WithMany()
                .HasForeignKey(x => x.SenderId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Request>()
                .HasOne(x => x.Receiver)
                .WithMany()
                .HasForeignKey(x => x.ReceiverId)
                .OnDelete(DeleteBehavior.NoAction);
            //builder.Entity<Room>()
            //    .HasOne(x=>x.Creator)
            //    .WithMany()
            //    .HasForeignKey(x=>x.CreatorId)
            //    .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<RoomImages>()
                .HasOne(x => x.Room)
                .WithMany()
                .HasForeignKey(x => x.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

        }
        public DbSet<User> Users { get; set; }
        public DbSet<PersonalPreferences> PersonalPreferences { get; set; }
        public DbSet<PotentialPartnerPreferences> PotentialPartnerPreferences { get; set;}
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<UserTicket> UserTickets {  get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<RoomImages> RoomImages { get; set; }
        public DbSet<Blog> Blogs {  get; set; }
        
    }
}
