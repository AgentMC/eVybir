namespace eVybir.Infra
{
    public record Participant(int CandidateId, int? GroupId, int DisplayOrder)
    {
        public bool IsChildEntry => GroupId.HasValue;
        public List<Participant> Children { get; set; } = [];
    }
}
