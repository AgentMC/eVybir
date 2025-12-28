namespace eVybir.Infra
{
    public record BulletinEntry(int ParticipantId, ParticipantCore Location, Candidate Metadata, int CampaignId);

    public record BulletinHierarchy(BulletinEntry Entry, BulletinEntry[] Children);

    public record Bulletin(Campaign Campaign, BulletinHierarchy[] PrimaryEntries);
    
}
