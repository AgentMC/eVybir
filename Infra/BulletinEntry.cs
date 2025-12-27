namespace eVybir.Infra
{
    public record BulletinEntry(int ParticipantId, ParticipantCore Location, Candidate Metadata);

    public record BulletinHierarchy(BulletinEntry Entry) 
    {
        public BulletinEntry[]? Children { get; set; }
    }
}
