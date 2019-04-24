﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CSharp;

namespace RiskOfShame.Loader.Core
{
    public class Compiler
    {
        public static String UnityDllPath = "";
        public static Boolean UpdateSources()
        {
            if (Directory.Exists(@"..\..\..\RiskOfShame"))
                return false;
            Console.WriteLine("Getting latest version info from Github");
            System.Net.WebClient wc = new System.Net.WebClient();
            String hash = wc.DownloadString("https://github.com/shalzuth/RiskOfShame/tree/master/RiskOfShame");
            String hashSearch = "\"commit-tease-sha\" href=\"/shalzuth/RiskOfShame/commit/";
            hash = hash.Substring(hash.IndexOf(hashSearch) + hashSearch.Length, 7);
            String hashFile = @".\RiskOfShame-master\hash.txt";
            if (File.Exists(hashFile))
            {
                if (hash != File.ReadAllText(hashFile))
                {
                    Console.WriteLine("Later version exists, removing existing version");
                    Directory.Delete(@".\RiskOfShame-master", true);
                }
            }
            if (!File.Exists(hashFile))
            {
                Console.WriteLine("Downloading latest version");
                wc.DownloadFile("https://github.com/shalzuth/RiskOfShame/archive/master.zip", "RiskOfShame.zip");
                using (var archive = ZipFile.OpenRead("RiskOfShame.zip"))
                {
                    archive.ExtractToDirectory(@".\");
                }
                File.WriteAllText(hashFile, hash);
                return true;
            }
            return false;
        }
        public static Byte[] CompileDll(String randString)
        {
            var options = new Dictionary<string, string>();// { { "CompilerVersion", "v3.5" } };// { { "CompilerVersion", "v4.0" } };
            CSharpCodeProvider codeProvider = new CSharpCodeProvider(options);
            CodeDomProvider codeProvider2 = new Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider();
            CompilerParameters compilerParameters = new CompilerParameters
            {
                GenerateExecutable = false,
#if DEBUG
                IncludeDebugInformation = true,
#endif
                GenerateInMemory = false,
                OutputAssembly = randString + ".dll",
            };
            compilerParameters.ReferencedAssemblies.Clear();
            //compilerParameters.ReferencedAssemblies.Add("mscorlib.dll");
            var dlls = Directory.GetFiles(UnityDllPath, "*.dll", SearchOption.AllDirectories);
            foreach(var dll in dlls)
                if (!dll.Contains("mscorlib"))
                    compilerParameters.ReferencedAssemblies.Add(dll);
            /*compilerParameters.ReferencedAssemblies.Add(UnityDllPath + "System.dll");
            compilerParameters.ReferencedAssemblies.Add(UnityDllPath + "System.Core.dll"); // not sure which to use... prefer Linq...
            compilerParameters.ReferencedAssemblies.Add(UnityDllPath + "netstandard.dll");
            //compilerParameters.ReferencedAssemblies.Add(gamePath + "System.Drawing.dll");
            //compilerParameters.ReferencedAssemblies.Add(gamePath + "System.Windows.Forms.dll");
            compilerParameters.ReferencedAssemblies.Add(UnityDllPath + "Assembly-CSharp.dll");
            compilerParameters.ReferencedAssemblies.Add(UnityDllPath + "UnityEngine.dll");
            compilerParameters.ReferencedAssemblies.Add(UnityDllPath + "UnityEngine.UI.dll");
            compilerParameters.ReferencedAssemblies.Add(UnityDllPath + "UnityEngine.UIElementsModule.dll");
            compilerParameters.ReferencedAssemblies.Add(UnityDllPath + "UnityEngine.UIModule.dll");
            compilerParameters.ReferencedAssemblies.Add(UnityDllPath + "UnityEngine.Networking.dll");
            compilerParameters.ReferencedAssemblies.Add(UnityDllPath + "UnityEngine.CoreModule.dll");
            compilerParameters.ReferencedAssemblies.Add(UnityDllPath + "UnityEngine.IMGUIModule.dll");
            //compilerParameters.ReferencedAssemblies.Add(gamePath + "TextMeshPro-1.0.55.2017.1.0b11.dll");*/

            string[] sourceFiles;
            if (Directory.Exists(@"..\..\..\RiskOfShame"))
                sourceFiles = Directory.GetFiles(@"..\..\..\RiskOfShame", "*.cs", SearchOption.AllDirectories);
            else
                sourceFiles = Directory.GetFiles(@"RiskOfShame-master\RiskOfShame\", "*.cs", SearchOption.AllDirectories);
            var sources = new List<String>();
            foreach (var sourceFile in sourceFiles)
            {
                if (sourceFile.Contains(@"obj\\"))
                    continue;
                var source = File.ReadAllText(sourceFile);
                source = source.Replace("RiskOfShame", randString);
                sources.Add(source);
            }
            var result = codeProvider2.CompileAssemblyFromSource(compilerParameters, sources.ToArray());
            if (result.Errors.Count > 0)
            {
                var sb = new StringBuilder();
                foreach(CompilerError error in result.Errors)
                    sb.AppendLine(error.ToString());
                throw new Exception(sb.ToString());
            }
            return File.ReadAllBytes(randString + ".dll");
            /*using (MemoryStream stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, result.CompiledAssembly);
                return stream.ToArray();
            }*/
        }
    }
}
