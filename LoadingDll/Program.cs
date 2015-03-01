﻿using System;
using System.Linq;
using MonoDevelop.Core;
using MonoDevelop.Core.ProgressMonitoring;
using MonoDevelop.Projects;
using SmartQuant;
using System.IO;
using System.Xml;

namespace LoadingDll
{
	class MainClass
	{
		public static void Main (string[] args)
        {
            LoadSmartQuant();
            Runtime.Initialize (true);
			foreach (var binding in LanguageBindingService.LanguageBindings)
				LoggingService.LogInfo ("Loaded Language Binding: {0}", binding.Language);
            Console.WriteLine("Hello, World");
            var service = Services.ProjectService;
            var file = "/home/alex/SampleProjects/SampleProjects.sln";
            var solu = service.ReadWorkspaceItem(new NullProgressMonitor(), file);
            var slnFile = "/home/alex/SampleProjects/SampleProjects.sln";
            var solution = service.ReadWorkspaceItem(new NullProgressMonitor(), slnFile).GetAllSolutions().First();
            Console.WriteLine("{0} {1}", solution.Name, solution.GetType());
           // Console.WriteLine(solution.Name);
            Console.WriteLine("dump");
            DumpSolution(solution);
            var wsItem = service.ReadWorkspaceItem(new NullProgressMonitor(), "/home/alex/Test/Test.mdw");
            Console.WriteLine("{0} {1}", wsItem.Name, wsItem.GetType());
            DumpWorkspace(wsItem as Workspace);

            var sol  = service.ReadWorkspaceItem(new NullProgressMonitor(), "/home/alex/Test/Sub2.sln").GetAllSolutions().First();
            DumpSolution(sol);

            var prj = Project.LoadProject("/home/alex/Test/TestProj.csproj", new NullProgressMonitor());
            DumpProject(prj);
        }

        private static void LoadSmartQuant()
        {
            var f = Framework.Current;
            foreach (var instrument in f.InstrumentManager.Instruments)
                Console.WriteLine(instrument.Symbol);
            f.InstrumentManager.Dump();
            f.DataManager.Dump();
        }

        public static void DumpWorkspace(Workspace ws)
        {
            foreach (var item in ws.Items)
                Console.WriteLine("{0} {1}", item.Name, item.GetType());
        }

        public static void DumpSolution(Solution solution)
        {
            foreach (var item in solution.RootFolder.Items)
                Console.WriteLine(item.Name);
            foreach (var filePath in solution.RootFolder.Files)
                Console.WriteLine(filePath.FileName);
        }

        public static void DumpProject(Project project)
        {
            Console.WriteLine("{0} {1}", project.Name,project.GetType());
            foreach (var item in project.Items)
            {
                if (item is ProjectReference)
                    Console.WriteLine("{0} {1}", (item as ProjectReference).Reference, item.GetType());
                if (item is ProjectFile)
                    Console.WriteLine("{0} {1}", (item as ProjectFile).FilePath, item.GetType());
            }
            foreach (var f in project.Files)
                Console.WriteLine("{0} {1} {2}", f.FilePath, f.Name, f.HasChildren);
        }

        public static Solution createSolution(string fileName)
        {
            var monitor = new NullProgressMonitor();
            var formats = Services.ProjectService.FileFormats.GetFileFormats(fileName, typeof(SolutionEntityItem));
            if (formats.Length == 0)
                formats = new [] { Services.ProjectService.DefaultFileFormat };

            var solution = new Solution();
            solution.SetLocation(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName));
            var sitem = Services.ProjectService.ReadSolutionItem(monitor, fileName);
            solution.RootFolder.AddItem(sitem);
            solution.CreateDefaultConfigurations();
            solution.Save(monitor);
            return solution;
        }

        public static void AddProjectToSolution(Solution solution, Project project)
        {
            solution.RootFolder.AddItem(project, true);
        }

        public static Project createProject(string fileName)
        {
            var doc = new XmlDocument();
            var projectOptions =doc.CreateElement("Project");
            projectOptions.SetAttribute("language", "c#");
            var pci = new ProjectCreateInformation();
            pci.ProjectName = Path.GetFileNameWithoutExtension(fileName);
            pci.ProjectBasePath=  Path.GetDirectoryName(fileName);
            var project = Services.ProjectService.CreateProject("DotNet", pci, projectOptions); 
            return project;
        }

        public static void deleteProject(Solution solution, Project project)
        {
           // solution.
        }

        static void RenameSolution(SolutionEntityItem item, string newName)
        {
            var oldFile = item.FileName;
            var oldName = item.Name;
            var newFile = oldFile.ParentDirectory.Combine(newName + oldFile.Extension);
            FileService.RenameFile(oldFile, newFile);
            item.FileName = newFile;
            item.Save(new NullProgressMonitor());
        }
	}
}
