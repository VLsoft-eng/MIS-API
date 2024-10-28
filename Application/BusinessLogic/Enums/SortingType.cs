using System.Text.Json.Serialization;

namespace Application.BusinessLogic.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SortingType
{
    NameAsc,
    NameDesc,
    CreateAsc,
    CreateDesc,
    InspectionAsc,
    InspectionDesc
}