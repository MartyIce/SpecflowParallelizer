using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BuildFileGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            string testAssemblyLocation = ConfigurationManager.AppSettings["testAssemblyLocation"];
            string buildFileLocation = ConfigurationManager.AppSettings["buildFileLocation"];

            GenerateBuildFile(testAssemblyLocation, buildFileLocation);
        }

        private static void GenerateBuildFile(string testAssemblyLocation, string outputLocation)
        {
            var assembly = Assembly.LoadFrom(testAssemblyLocation);
            parallel p = new parallel();
            p.TestOutputDirectory = ConfigurationManager.AppSettings["testOutputDirectory"];
            foreach (var type in assembly.GetTypes())
            {
                if (type.CustomAttributes.Any(x => x.AttributeType.Name.Contains("TestFixtureAttribute")))
                {
                    if (!p.Namespaces.Contains(type.Namespace))
                    {
                        p.Namespaces.Add(type.Namespace);
                    }
                }
            }

            WriteFile(outputLocation, p.TransformText());
        }

        private static void WriteFile(string outputLocation, string classText, bool appendText = false)
        {
            using (StreamWriter sw = new StreamWriter(outputLocation, appendText))
            {
                sw.Write(classText);
            }
        }
    }
}
