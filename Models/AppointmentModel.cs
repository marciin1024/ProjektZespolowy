using ProjektZespolowy.Entities;

namespace ProjektZespolowy.Models
{
    public class AppointmentModel
    {
        public int Id { get; set; }
        public int AppointmentType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Subject { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Label { get; set; }
        public int Status { get; set; }
        public bool AllDay { get; set; }
        public string Recurrence { get; set; }
        public int? ResourceId { get; set; }
        public string Resources { get; set; }
        public bool Accepted { get; set; }
        public User AssignedTo { get; set; }
        public int AssignedToId { get; set; }
    }
}
