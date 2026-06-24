namespace Invento.Persistence.Auditing
{
    public class AuditEntry
    {
        public string EntityName { get; set; } = string.Empty;

        public string ActionType { get; set; } = string.Empty;

        public string RecordId { get; set; } = string.Empty;

        public Dictionary<string, object?> OldValues { get; set; }
            = new();

        public Dictionary<string, object?> NewValues { get; set; }
            = new();
    }
}