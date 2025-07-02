namespace Survey.DTOs.SurveyDto
{
    public class ListSurveyDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreateDate { get; set; }
        public string Url { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
