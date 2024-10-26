using System.Text.Json;
using Domain;

namespace Infrastructure;

public static class IcdLoader
{
    private static string PATH_TO_ICD_JSON = @"C:\Users\bydeflt\Documents\1.2.643.5.1.13.13.11.1005_2.27.json";

    public static void loadIcd(ApplicationDbContext context)
    {
        if (context.Icds.Any())
        {
            return;
        }
        
        var json = File.ReadAllText(PATH_TO_ICD_JSON);
        var icdJsonRecords = JsonSerializer.Deserialize<IcdRecordsJson>(json);
        
        if (icdJsonRecords == null)
        {
            return;
        }

        var icdJsonList = icdJsonRecords.records;
        var icdDictionary = new Dictionary<int, Icd>();

        foreach (var jsonElement in icdJsonList)
        {
            Guid id = Guid.NewGuid();
            var code = jsonElement.MKB_CODE;
            var message = jsonElement.MKB_NAME;

            var icdFieldId = jsonElement.ID;
            
            var icd = new Icd
            {
                id = id,
                сode = code,
                name = message,
                createTime = DateTime.UtcNow
            };

            icdDictionary[icdFieldId] = icd;
        }

        foreach (var jsonElement in icdJsonList)
        {
            var icd = icdDictionary[jsonElement.ID];
            Icd? icdParent = null;
            int icdParentId = -1;
            if (jsonElement.ID_PARENT != null)
            {
                icdParentId = int.Parse(jsonElement.ID_PARENT);
            }

            if (icdParentId >= 0)
            {
                icdParent = icdDictionary[icdParentId];
            }

            icd.parent = icdParent;
        }

        var icdList = icdDictionary.Values.ToList();
        
        context.Icds.AddRange(icdList);
        context.SaveChanges();
    }
    
    public class IcdJson
    {
        public int ID { get; set; }
        public string MKB_CODE { get; set; }
        public string MKB_NAME { get; set; }
        public string? ID_PARENT { get; set; } 
    }
    public class IcdRecordsJson
    {
        public List<IcdJson> records { get; set; }
    }
}