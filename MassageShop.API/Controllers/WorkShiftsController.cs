using MassageShop.API.Data;
using MassageShop.API.Models.DTOs.WorkShift;
using MassageShop.API.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MassageShop.API.Controllers;

[ApiController]
[Route("api/work-shifts")]
[Authorize(Roles = "ADMIN")]
public class WorkShiftsController : ControllerBase
{
    private readonly AppDbContext _db;
    public WorkShiftsController(AppDbContext db) => _db = db;

    private IQueryable<WorkShift> Query() => _db.WorkShifts
        .Include(x => x.Employee).ThenInclude(x => x.User)
        .Include(x => x.Room);

    private static WorkShiftResponseDto Map(WorkShift x) => new()
    {
        Id = x.Id, EmployeeId = x.EmployeeId, EmployeeName = x.Employee.User.FullName,
        Position = x.Employee.Position, WorkDate = x.WorkDate, ShiftType = x.ShiftType,
        StartTime = x.StartTime, EndTime = x.EndTime, RoomId = x.RoomId,
        RoomNumber = x.Room?.RoomNumber, Status = x.Status, Note = x.Note
    };

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var rows = await Query()
            .Where(x => (!from.HasValue || x.WorkDate >= from.Value.Date) &&
                        (!to.HasValue || x.WorkDate <= to.Value.Date))
            .OrderBy(x => x.WorkDate).ThenBy(x => x.StartTime).ToListAsync();
        return Ok(rows.Select(Map));
    }

    [HttpPost]
    public async Task<IActionResult> Create(WorkShiftDto dto)
    {
        var validation = await ValidateConflicts(new[] { dto.EmployeeId }, dto.WorkDate, dto.StartTime, dto.EndTime, Array.Empty<int>());
        if (validation is not null) return BadRequest(new { message = validation });
        var shift = CreateEntity(dto.EmployeeId, dto);
        _db.WorkShifts.Add(shift);
        await _db.SaveChangesAsync();
        return Ok(Map(await Query().FirstAsync(x => x.Id == shift.Id)));
    }

    // A single action used by the weekly calendar to assign many employees to one shift.
    [HttpPost("bulk")]
    public async Task<IActionResult> CreateGroup(WorkShiftGroupDto dto)
    {
        var employees = dto.EmployeeIds.Distinct().ToArray();
        if (employees.Length == 0) return BadRequest(new { message = "Vui lòng chọn ít nhất một nhân viên." });
        var validation = await ValidateConflicts(employees, dto.WorkDate, dto.StartTime, dto.EndTime, Array.Empty<int>());
        if (validation is not null) return BadRequest(new { message = validation });

        var template = ToSingleDto(dto);
        var rows = employees.Select(id => CreateEntity(id, template)).ToList();
        _db.WorkShifts.AddRange(rows);
        await _db.SaveChangesAsync();
        var ids = rows.Select(x => x.Id).ToArray();
        return Ok((await Query().Where(x => ids.Contains(x.Id)).ToListAsync()).Select(Map));
    }

    // Copies assignments from the immediately previous Monday-Sunday week to the selected week.
    // Existing target rows are preserved; no duplicate or overlapping assignment is created.
    [HttpPost("copy-previous-week")]
    public async Task<IActionResult> CopyPreviousWeek([FromQuery] DateTime weekStart)
    {
        var targetStart = weekStart.Date.AddDays(-(((int)weekStart.DayOfWeek + 6) % 7));
        var sourceStart = targetStart.AddDays(-7);
        var sourceEnd = sourceStart.AddDays(6);
        var sourceRows = await _db.WorkShifts
            .Where(x => x.WorkDate >= sourceStart && x.WorkDate <= sourceEnd)
            .ToListAsync();
        if (sourceRows.Count == 0)
            return BadRequest(new { message = "Tuần trước chưa có lịch phân ca để sao chép." });

        var targetEnd = targetStart.AddDays(6);
        var targetRows = await _db.WorkShifts
            .Where(x => x.WorkDate >= targetStart && x.WorkDate <= targetEnd)
            .ToListAsync();
        var created = new List<WorkShift>();
        var skipped = 0;
        foreach (var source in sourceRows)
        {
            var targetDate = targetStart.AddDays((source.WorkDate.Date - sourceStart).Days);
            var duplicate = targetRows.Concat(created).Any(x => x.EmployeeId == source.EmployeeId &&
                x.WorkDate.Date == targetDate && x.ShiftType == source.ShiftType &&
                x.StartTime == source.StartTime && x.EndTime == source.EndTime);
            var overlaps = targetRows.Concat(created).Any(x => x.EmployeeId == source.EmployeeId &&
                x.WorkDate.Date == targetDate && x.StartTime < source.EndTime && x.EndTime > source.StartTime);
            if (duplicate || overlaps) { skipped++; continue; }
            created.Add(new WorkShift
            {
                EmployeeId = source.EmployeeId, WorkDate = targetDate, ShiftType = source.ShiftType,
                StartTime = source.StartTime, EndTime = source.EndTime, RoomId = source.RoomId,
                Status = source.Status, Note = source.Note
            });
        }
        if (created.Count > 0)
        {
            _db.WorkShifts.AddRange(created);
            await _db.SaveChangesAsync();
        }
        return Ok(new { created = created.Count, skipped, message = $"Đã sao chép {created.Count} phân công; bỏ qua {skipped} phân công trùng hoặc chồng giờ." });
    }

    // Updates a calendar card and reconciles its employee list without introducing a Shift entity.
    [HttpPut("group")]
    public async Task<IActionResult> UpdateGroup(WorkShiftGroupDto dto)
    {
        var ids = dto.Ids.Distinct().ToArray();
        var rows = await _db.WorkShifts.Where(x => ids.Contains(x.Id)).ToListAsync();
        if (rows.Count != ids.Length) return NotFound(new { message = "Không tìm thấy đầy đủ các ca cần cập nhật." });
        var employees = dto.EmployeeIds.Distinct().ToArray();
        // An empty selection means the administrator unassigned this default calendar slot.
        // The slot itself remains on the UI; only its employee-specific WorkShift rows are removed.
        if (employees.Length == 0)
        {
            if (await _db.Attendances.AnyAsync(x => ids.Contains(x.WorkShiftId)))
                return BadRequest(new { message = "Không thể bỏ toàn bộ nhân viên khỏi ca đã có dữ liệu chấm công." });
            _db.WorkShifts.RemoveRange(rows);
            await _db.SaveChangesAsync();
            return Ok();
        }
        var validation = await ValidateConflicts(employees, dto.WorkDate, dto.StartTime, dto.EndTime, ids);
        if (validation is not null) return BadRequest(new { message = validation });

        var toRemove = rows.Where(x => !employees.Contains(x.EmployeeId)).ToList();
        if (toRemove.Count > 0 && await _db.Attendances.AnyAsync(x => toRemove.Select(s => s.Id).Contains(x.WorkShiftId)))
            return BadRequest(new { message = "Không thể bỏ nhân viên khỏi ca đã có dữ liệu chấm công." });

        foreach (var row in rows.Where(x => employees.Contains(x.EmployeeId))) Apply(row, dto);
        _db.WorkShifts.RemoveRange(toRemove);
        var retainedEmployees = rows.Select(x => x.EmployeeId).Except(toRemove.Select(x => x.EmployeeId));
        var template = ToSingleDto(dto);
        _db.WorkShifts.AddRange(employees.Except(retainedEmployees).Select(id => CreateEntity(id, template)));
        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, WorkShiftDto dto)
    {
        var shift = await _db.WorkShifts.FindAsync(id);
        if (shift is null) return NotFound();
        var validation = await ValidateConflicts(new[] { dto.EmployeeId }, dto.WorkDate, dto.StartTime, dto.EndTime, new[] { id });
        if (validation is not null) return BadRequest(new { message = validation });
        Apply(shift, dto);
        await _db.SaveChangesAsync();
        return Ok(Map(await Query().FirstAsync(x => x.Id == id)));
    }

    [HttpDelete("group")]
    public async Task<IActionResult> DeleteGroup([FromBody] WorkShiftGroupDto dto)
    {
        var ids = dto.Ids.Distinct().ToArray();
        var rows = await _db.WorkShifts.Where(x => ids.Contains(x.Id)).ToListAsync();
        if (rows.Count == 0) return NotFound();
        if (await _db.Attendances.AnyAsync(x => ids.Contains(x.WorkShiftId)))
            return BadRequest(new { message = "Không thể xóa ca đã có dữ liệu chấm công." });
        _db.WorkShifts.RemoveRange(rows);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var shift = await _db.WorkShifts.FindAsync(id);
        if (shift is null) return NotFound();
        if (await _db.Attendances.AnyAsync(x => x.WorkShiftId == id))
            return BadRequest(new { message = "Không thể xóa ca đã có dữ liệu chấm công." });
        _db.WorkShifts.Remove(shift);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private async Task<string?> ValidateConflicts(IEnumerable<int> employeeIds, DateTime date, TimeSpan start, TimeSpan end, IEnumerable<int> ignoredIds)
    {
        if (start == end) return "Giờ kết thúc phải khác giờ bắt đầu.";
        var employees = employeeIds.Distinct().ToArray();
        var ignored = ignoredIds.ToArray();
        var dayRows = await Query().Where(x => employees.Contains(x.EmployeeId) &&
            x.WorkDate.Date == date.Date && !ignored.Contains(x.Id)).ToListAsync();
        // End <= start represents an overnight shift (for example 22:00 - 06:00).
        var proposedStart = date.Date.Add(start);
        var proposedEnd = date.Date.Add(end);
        if (proposedEnd <= proposedStart) proposedEnd = proposedEnd.AddDays(1);
        var conflict = dayRows.FirstOrDefault(x =>
        {
            var existingStart = x.WorkDate.Date.Add(x.StartTime);
            var existingEnd = x.WorkDate.Date.Add(x.EndTime);
            if (existingEnd <= existingStart) existingEnd = existingEnd.AddDays(1);
            return existingStart < proposedEnd && proposedStart < existingEnd;
        });
        return conflict is null ? null : $"Nhân viên {conflict.Employee.User.FullName} đã có ca trùng thời gian.";
    }

    private static WorkShift CreateEntity(int employeeId, WorkShiftDto dto) => new()
    {
        EmployeeId = employeeId, WorkDate = dto.WorkDate.Date, ShiftType = dto.ShiftType,
        StartTime = dto.StartTime, EndTime = dto.EndTime, RoomId = dto.RoomId,
        Status = dto.Status, Note = dto.Note
    };

    private static WorkShiftDto ToSingleDto(WorkShiftGroupDto dto) => new()
    {
        WorkDate = dto.WorkDate, ShiftType = dto.ShiftType, StartTime = dto.StartTime,
        EndTime = dto.EndTime, RoomId = dto.RoomId, Status = dto.Status, Note = dto.Note
    };

    private static void Apply(WorkShift entity, WorkShiftDto dto)
    {
        entity.EmployeeId = dto.EmployeeId; entity.WorkDate = dto.WorkDate.Date;
        entity.ShiftType = dto.ShiftType; entity.StartTime = dto.StartTime; entity.EndTime = dto.EndTime;
        entity.RoomId = dto.RoomId; entity.Status = dto.Status; entity.Note = dto.Note;
    }

    private static void Apply(WorkShift entity, WorkShiftGroupDto dto)
    {
        entity.WorkDate = dto.WorkDate.Date; entity.ShiftType = dto.ShiftType;
        entity.StartTime = dto.StartTime; entity.EndTime = dto.EndTime; entity.RoomId = dto.RoomId;
        entity.Status = dto.Status; entity.Note = dto.Note;
    }
}
