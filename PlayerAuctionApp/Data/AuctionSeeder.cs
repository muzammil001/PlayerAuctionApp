using Microsoft.EntityFrameworkCore;
using PlayerAuctionApp.Domain;

namespace PlayerAuctionApp.Data;

public static class AuctionSeeder
{
    public static async Task SeedAsync(AuctionDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.Teams.AnyAsync(team => team.AuctionId == AuctionDbContext.DefaultAuctionId, cancellationToken))
        {
            return;
        }

        (string Name, string City, string Owner, string Phone, string Email, string Logo)[] demoTeams =
        [
            ("Lahore Lions", "Lahore", "Ahsan Malik", "+92 300 1110001", "ahsan@lions.example", "lahore-lions.svg"),
            ("Karachi Kings", "Karachi", "Sana Qureshi", "+92 300 1110002", "sana@kings.example", "karachi-kings.svg"),
            ("Islamabad Eagles", "Islamabad", "Hamza Farooq", "+92 300 1110003", "hamza@eagles.example", "islamabad-eagles.svg"),
            ("Peshawar Panthers", "Peshawar", "Zara Khan", "+92 300 1110004", "zara@panthers.example", "peshawar-panthers.svg"),
            ("Quetta Gladiators", "Quetta", "Bilal Ahmed", "+92 300 1110005", "bilal@gladiators.example", "quetta-gladiators.svg"),
            ("Multan Sultans", "Multan", "Hira Shah", "+92 300 1110006", "hira@sultans.example", "multan-sultans.svg"),
            ("Faisalabad Falcons", "Faisalabad", "Usman Raza", "+92 300 1110007", "usman@falcons.example", "faisalabad-falcons.svg"),
            ("Rawalpindi Riders", "Rawalpindi", "Maham Iqbal", "+92 300 1110008", "maham@riders.example", "rawalpindi-riders.svg"),
            ("Hyderabad Hawks", "Hyderabad", "Daniyal Asif", "+92 300 1110009", "daniyal@hawks.example", "hyderabad-hawks.svg"),
            ("Sialkot Strikers", "Sialkot", "Maryam Javed", "+92 300 1110010", "maryam@strikers.example", "sialkot-strikers.svg"),
        ];

        foreach (var (name, city, ownerName, phone, email, logo) in demoTeams)
        {
            db.Teams.Add(new Team
            {
                Id = Guid.NewGuid(),
                AuctionId = AuctionDbContext.DefaultAuctionId,
                Name = name,
                City = city,
                LogoImage = $"/images/team-logos/{logo}",
                Owner = new TeamOwner
                {
                    Id = Guid.NewGuid(),
                    FullName = ownerName,
                    Phone = phone,
                    Email = email,
                },
            });
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
