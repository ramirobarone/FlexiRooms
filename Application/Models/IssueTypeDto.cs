using Infrastructure.Models;

namespace Application.Models
{
    public class IssueTypeDto(int Id, string Issue)
    {
        public int Id { get; } = Id;
        public string Issue { get; } = Issue;

        public static implicit operator IssueTypeDto(IssueType issueType)
        {
            return new IssueTypeDto(issueType.Id, issueType.Issue);
        }
    }
}
