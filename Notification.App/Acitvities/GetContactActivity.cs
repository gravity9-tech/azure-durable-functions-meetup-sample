using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Functions.Worker;
using Notification.App.Models;

namespace Notification.App.Acitvities;

public class GetContactActivity
{
    [Function(nameof(GetContactActivity))]
    public IEnumerable<Contact> Run([ActivityTrigger] string team)
    {
        var list = new[]
        {
            new Contact() { Name = "John Doe", Order = 1, PhoneNumber = "1234567890", Team = "Support" },
            new Contact() { Name = "Jane Doe", Order = 2, PhoneNumber = "1234567891", Team = "Support" },
            new Contact() { Name = "John Smith", Order = 3, PhoneNumber = "1234567892", Team = "Support" }
        };
        return list.Where(x => x.Team.Equals(team, StringComparison.OrdinalIgnoreCase));
    }
}
