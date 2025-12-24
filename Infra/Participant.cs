namespace eVybir.Infra
{
    public record Participant(int CandidateId, int? GroupId, int DisplayOrder)
    {
        public List<Participant> Children { get; set; } = [];
    }
}
