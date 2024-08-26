namespace TechnicalTestApi.Models
{
    // Data Transfer Object (DTO) for updating an existing note
    public class UpdateNoteDto
    {
        // The updated title of the note
        public string Title { get; set; }

        // The updated body/content of the note
        public string Body { get; set; }
    }
}
