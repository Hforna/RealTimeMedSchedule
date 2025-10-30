using MedSchedule.Domain.Enums;

namespace MedSchedule.Domain.DTOs;

public class FilterAppointmentsDto
{
    public FilterAppointmentsDto(DateTime? queueDay, string? specialtyName, Guid? staffId, EAppointmentStatus? status, EPriorityLevel? priorityLevel, int page, int perPage, Guid? patientId)
    {
        this.queueDay = queueDay;
        this.specialtyName = specialtyName;
        this.staffId = staffId;
        this.status = status;
        this.priorityLevel = priorityLevel;
        this.page = page;
        this.perPage = perPage;
        this.patientId = patientId;
    }

    public DateTime? queueDay { get; set; } 
    public string? specialtyName { get; set; } 
    public Guid? staffId { get; set; } 
    public Guid? patientId { get; set; }
    public EAppointmentStatus? status { get; set; }
    public EPriorityLevel? priorityLevel { get; set; }
    public int page { get; set; }
    public int perPage { get; set; }
}