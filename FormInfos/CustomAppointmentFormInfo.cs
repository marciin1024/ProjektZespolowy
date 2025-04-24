using DevExpress.Blazor;

namespace ProjektZespolowy.FormInfos
{
    public class CustomAppointmentFormInfo : SchedulerAppointmentFormInfo
    {
        public CustomAppointmentFormInfo(DxSchedulerAppointmentItem AppointmentItem, DxSchedulerDataStorage DataStorage, DxScheduler scheduler)
            : base(AppointmentItem, DataStorage, scheduler) { }

        public override string Subject
        {
            get => base.Subject;
            set { base.Subject = value; }
        }

        public int AssignedToId
        {
            get => (int)CustomFields["AssignedToId"];
            set { CustomFields["AssignedToId"] = value; }
        }

        public string Name
        {
            get => (string)CustomFields["Name"];
            set { CustomFields["Name"] = value; }
        }
    }
}
