using src.Log;
using UnityEditor;

namespace src.Editor
{
    public static class ScriptingSymbols
    {
        public static void ToggleScriptingSymbol(string symbol)
        {
            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
 
            // remove if already present
            if (defines.Contains(symbol))
            {
                defines = defines.Replace(symbol,"");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, ($"{defines}"));
                Logs.Log($"Remove {symbol} to scripting define symbols");            
                return;
            }
 
            // Append.
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, ($"{defines};{symbol}"));
            Logs.Log($"<b>{symbol}</b> added to <i>Scripting Define Symbols</i>");
        }
        
        public static void AddScriptingSymbol(string symbol)
        {
            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
 
            if (defines.Contains(symbol))
            {
                Logs.Log($"Already present {symbol} to scripting define symbols");            
                return;
            }
 
            // Append.
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, ($"{defines};{symbol}"));
            Logs.Log($"<b>{symbol}</b> added to <i>Scripting Define Symbols</i>");
        }
        
        public static void RemoveScriptingSymbol(string symbol)
        {
            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
 
            if (defines.Contains(symbol))
            {
                defines = defines.Replace(symbol,"");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, ($"{defines}"));
                Logs.Log($"Remove {symbol} to scripting define symbols");            
                return;
            }
        }
        
    }
}