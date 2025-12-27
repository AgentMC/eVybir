namespace eVybir.Infra
{
    public record Ticket(Guid TicketId, 
                         DateTimeOffset TicketCreatedDate, 
                         DateTimeOffset? TicketCommittedDate, 
                         bool IsTicketOffline);
    public record CampaignState(int CampaignId,
                                string CampaignName,
                                bool IsCampaignFinished,
                                Ticket? Ticket)
    {
        const string L_NOT_OBTAINED_CURRENT = "(Ще не отримано)",
                     L_NOT_OBTAINED_PAST = "(Не було отримано)",
                     L_NOT_USED_CURRENT = "(Ще не використано)",
                     L_NOT_USED_PAST = "(Не було використано)",
                     L_NOT_APPLICABLE = "(Недоступно)",
                     L_ONLINE = "Онлайн",
                     L_OFFLINE = "У виборчій комісії";

        public string UfTicketReceived => Ticket?.TicketCreatedDate.ToString() ?? (IsCampaignFinished ? L_NOT_OBTAINED_PAST : L_NOT_OBTAINED_CURRENT);

        public string UfTicketCommitted
        {
            get
            {
                if (Ticket == null)
                {
                    return IsCampaignFinished ? L_NOT_OBTAINED_PAST : L_NOT_OBTAINED_CURRENT;
                }
                else
                {
                    if (Ticket.IsTicketOffline)
                    {
                        return L_NOT_APPLICABLE;
                    }
                    else if (Ticket.TicketCommittedDate != null)
                    {
                        return Ticket.TicketCommittedDate.ToString()!;
                    }
                    else if (!IsCampaignFinished)
                    {
                        return L_NOT_USED_CURRENT;
                    }
                    else
                    {
                        return L_NOT_USED_PAST;
                    }
                }
            }
        }

        public string UfTicketSource
        {
            get
            {
                if (Ticket == null)
                {
                    return IsCampaignFinished ? L_NOT_OBTAINED_PAST : L_NOT_OBTAINED_CURRENT;
                }
                else
                {
                    return Ticket.IsTicketOffline ? L_OFFLINE : L_ONLINE;
                }
            }
        }
    }
}
