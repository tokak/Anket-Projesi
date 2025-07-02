namespace SurveyProject.Entities.Entity
{
    public class TSurvey
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Url { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }

        // Bir anketin birden fazla sorusu olabilir
        public ICollection<Question> Questions { get; set; }
    }
}
