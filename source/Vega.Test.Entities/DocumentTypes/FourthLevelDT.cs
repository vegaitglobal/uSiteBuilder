namespace Vega.Test.Entities.DocumentTypes
{
    using Vega.USiteBuilder;

    [DocumentType(IconUrl = "doc4.gif",
                  Thumbnail = "folder.png",
                  AllowedTemplates = new string[] { "FourthLevel" },
                  Description = "Decription of ThirdLevelDT")]
    public class FourthLevelDT : FirstLevelDT
    {
    }
}
