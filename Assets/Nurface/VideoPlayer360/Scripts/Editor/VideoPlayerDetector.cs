using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

using System.Reflection;

namespace VideoPlayer360
{
    public static class VideoPlayerDetector
    {
        static Dictionary<string, string> plugins = new Dictionary<string, string>()
        {
            {"MediaPlayerCtrl", "EMT_PRESENT"},            
        };

        static List<Assembly> assemblies = new List<Assembly>();


        [DidReloadScripts(1)]
        public static void ProcessInstalledPlugins()
        {
            ManageSymbolDefinitions();
        }

        static void ManageSymbolDefinitions()
        {            
            LoadAssemblies();

            List<string> platformsUpdated = new List<string>();

            foreach (BuildTargetGroup platform in Enum.GetValues(typeof(BuildTargetGroup)))
            {
                if (IsObsolete(platform) || platform == BuildTargetGroup.Unknown) continue;

                // Temporary code, we're getting an error message trying to set defines for the Switch platform
                // (I suspect a Unity 5.6 bug)
#if UNITY_5_6_OR_NEWER
                if (platform == BuildTargetGroup.Switch) continue;
#endif      
                // /Temporary code

                var existingSymbolsString = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform);
                var symbols = existingSymbolsString.Replace(" ", "").Split(';').ToList();

                foreach (var plugin in plugins)
                {
                    symbols = ManageSymbolDefinition(symbols, plugin.Key, plugin.Value);
                }

                var newSymbolsString = String.Join(";", symbols.Distinct().ToArray());

                if (existingSymbolsString != newSymbolsString)
                {
                    platformsUpdated.Add(platform.ToString());                    
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, newSymbolsString);                    
                }
            }

            if (platformsUpdated.Any())
            {
                Debug.LogFormat("[VideoPlayer360] Updating available Video Players {0}...", String.Join(", ", platformsUpdated.ToArray()));
            }
        }

        static List<string> ManageSymbolDefinition(List<string> symbols, string className, string symbol)
        {
            var type = assemblies.Select(a => a.GetTypes().Where(t => t.FullName.Equals(className)).FirstOrDefault())
                                 .FirstOrDefault(t => t != null);
            
            if (type != null)
            {
                if (!symbols.Contains(symbol)) symbols.Add(symbol);
            }
            else
            {
                if (symbols.Contains(symbol)) symbols.Remove(symbol);
            }

            return symbols;
        }

        // http://stackoverflow.com/questions/29832536/check-if-enum-is-obsolete
        static bool IsObsolete(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            var attributes = (ObsoleteAttribute[])
                fi.GetCustomAttributes(typeof(ObsoleteAttribute), false);
            return (attributes != null && attributes.Length > 0);
        }

        #region Assemblies
        private static List<string> m_AssemblyNames = new List<string>();

        static void LoadAssemblies()
        {
            assemblies = GetAssemblyNames()
                         .Select(an => Assembly.Load(an))
                         .ToList();
        }

        private static List<string> GetAssemblyNames()
        {
            if (!m_AssemblyNames.Any())
            {
                // Get all referenced assemblies
                m_AssemblyNames = Assembly.GetExecutingAssembly()
                                          .GetReferencedAssemblies()
                                          .Select(a => a.FullName)
                                          .ToList();

                // Get the current assembly as well
                m_AssemblyNames.Add(Assembly.GetExecutingAssembly().FullName);

                // Just in case
                m_AssemblyNames.Add(Assembly.GetAssembly(typeof(VideoPlayer360.MediaPlayerController)).FullName);

                // filter out any duplicates
                m_AssemblyNames = m_AssemblyNames.Distinct().ToList();
            }

            return m_AssemblyNames;
        }
        #endregion
    }
}