// src/TRSB.Domain/Entities/Setting.cs
namespace TRSB.Domain.Entities;

public class Setting
{
    public int Id { get; set; }
    public string KeyName { get; set; } = null!;
    public int IntValue { get; set; }
}
