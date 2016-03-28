namespace Vega.USiteBuilder.MacroBuilder
{
    internal class MacroDefinition
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public bool UseInEditor { get; set; }
        public bool RenderContentInEditor { get; set; }
        public int CachePeriod { get; set; }
        public bool CacheByPage { get; set; }
        public bool CachePersonalized { get; set; }

        public MacroParameter[] Parameters { get; set; }
    }
}

/*
{
    "Name": "My macro 1",
    "Alias: "mymacro1",
    "UseInEditor": true,
 	"RenderContentInEditor": true,	
    "CachePeriod": 90,
	"CacheByPage": true,
	"CachePersonalized": true,
	"Parameters":
	[
		{
			"Alias": "parameter1", 
			"Name": "parameter 1", 
			"Show": true, 
			"Type": 0
		},
		{
			"Alias": "parameter2",
			"Name": "parameter 2",
			"Show": true, 
			"Type": 13
		},
		{
			"Alias": "parameter2",
			"Name": "parameter 2",
			"Show": false,
			"Type": 11
		}
	]
}
*/
