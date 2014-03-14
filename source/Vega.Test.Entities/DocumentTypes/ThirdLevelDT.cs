namespace Vega.Test.Entities.DocumentTypes
{
    using System;
    using Vega.USiteBuilder;

    [DocumentType(IconUrl = "doc4.gif",
                  Thumbnail = "folder.png",
                  Description = "Decription of ThirdLevelDT",
                  AllowedTemplates = new string[] { "ThirdLevel" },
                  AllowedChildNodeTypes=new Type[] {typeof(FourthLevelDT)},
                  AllowedChildNodeTypeOf=new Type[] {typeof(SecondLevelDT1)})]
    public class ThirdLevelDT : SecondLevelDT1
    {
    }
}
