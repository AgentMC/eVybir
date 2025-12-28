namespace eVybir.Infra
{
    public record TicketStats(int Offline, int Online, int OnlineUsed)
    {
        public int VotesCast { get; set; }
    }

    public record GroupMembersStats(string VotedMemberName, DateTime? VotedMemberDate, string GroupName, int Count);
}
