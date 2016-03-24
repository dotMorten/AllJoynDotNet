using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PInvokeCodeGenerator
{
    public static class SharedProjectGenerator
    {
        public static void Generate(string projectFolder, string projectName, IEnumerable<string> files)
        {
            var projectGuid = Guid.NewGuid().ToString();
            File.WriteAllText(projectFolder + projectName + ".shproj", shprojTemplate.Replace("{ProjectGuid}", projectGuid).Replace("{FileName}", projectName));
            StringBuilder sb = new StringBuilder();
            foreach(var file in files)
            {
                sb.AppendFormat("    <Compile Include=\"$(MSBuildThisFileDirectory){0}\" />\n", file);
            }
            File.WriteAllText(projectFolder + projectName + ".projitems", projitemsTemplate.Replace("{ProjectGuid}", projectGuid).Replace("{ProjectFiles}", sb.ToString()));
        }

        private const string projitemsTemplate = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <PropertyGroup>
    <MSBuildAllProjects>$(MSBuildAllProjects);$(MSBuildThisFileFullPath)</MSBuildAllProjects>
    <HasSharedItems>true</HasSharedItems>
    <SharedGUID>{ProjectGuid}</SharedGUID>
  </PropertyGroup>
  <PropertyGroup Label=""Configuration"">
    <Import_RootNamespace>AllJoynDotNet</Import_RootNamespace>
  </PropertyGroup>
  <ItemGroup>
{ProjectFiles}
  </ItemGroup>
</Project>
";

        private const string shprojTemplate = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""14.0"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <PropertyGroup Label=""Globals"">
    <ProjectGuid>{ProjectGuid}</ProjectGuid>
    <MinimumVisualStudioVersion>14.0</MinimumVisualStudioVersion>
  </PropertyGroup>
  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />
  <Import Project=""$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)\CodeSharing\Microsoft.CodeSharing.Common.Default.props"" />
  <Import Project=""$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)\CodeSharing\Microsoft.CodeSharing.Common.props"" />
  <PropertyGroup />
  <Import Project=""{FileName}.projitems"" Label=""Shared"" />
  <Import Project=""$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)\CodeSharing\Microsoft.CodeSharing.CSharp.targets"" />
</Project>
";
    }
}
