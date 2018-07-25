namespace ZQNB.Common.Serialize
{
    public enum NbJsonFormatting
    {
        // 摘要: 
        //     No special formatting is applied. This is the default.
        None = 0,
        //
        // 摘要: 
        //     Causes child objects to be indented according to the Newtonsoft.Json.JsonTextWriter.Indentation
        //     and Newtonsoft.Json.JsonTextWriter.IndentChar settings.
        Indented = 1,
    }
}