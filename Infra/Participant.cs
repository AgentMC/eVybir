namespace eVybir.Infra
{
    public record ParticipantCore(int CandidateId, int? GroupId, int DisplayOrder);
    public record Participant(int CandidateId, int? GroupId, int DisplayOrder) 
                : ParticipantCore(CandidateId, GroupId, DisplayOrder)
    {
        public List<Participant> Children { get; init; } = [];
    }
}
