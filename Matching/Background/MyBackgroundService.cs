using Matching.Data;
using Matching.Model;
using System.Globalization;

namespace Matching.Background
{
    public class MyBackgroundService : BackgroundService
    {
        private readonly ApplicationDbContext _context;

        public MyBackgroundService(IServiceProvider services)
        {
            _context = services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                try
                {

                    var tickets = _context.Tickets.ToList();

                    foreach (var tick in tickets)
                    {

                        if (DateTime.TryParseExact(tick.BookingDate, "yyyy-MM-dd hhtt", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime storedDate))
                        {

                            DateTime currentDate = DateTime.Now;
                            if (storedDate < currentDate)
                            {
                                var userTicket = _context.UserTickets.Where(x => x.TicketId == tick.Id).ToList();
                                if (userTicket.Count > 0)
                                {
                                    foreach (var usti in userTicket)
                                    {
                                        if (usti.TicketStatus == "Accepted")
                                        {
                                            usti.TicketStatus = "done";
                                        }
                                        else
                                        {
                                            usti.TicketStatus = "cancelled";
                                        }

                                    }

                                    _context.UpdateRange(userTicket);

                                }
                                else
                                {
                                    UserTicket userTicket1 = new()
                                    {
                                        SenderId = tick.CreatorId,
                                        ReceiverId = null,
                                        TicketId = tick.Id,
                                        TicketStatus = "cancelled"
                                    };
                                    _context.UserTickets.Add(userTicket1);
                                }
                                _context.SaveChanges();
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid date format.");
                        }


                    }
                }
                catch
                {

                }
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
