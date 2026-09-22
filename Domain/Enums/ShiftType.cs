namespace Domain.Enums;

public enum ShiftType
{
      AM,   // morning
      PM,   // night
      ANN,  // annual leave
      BW,   // comes between - custom/split hours
      FULL, // AM + PM, works all day
      SL,    // seek leave
      OFF
}