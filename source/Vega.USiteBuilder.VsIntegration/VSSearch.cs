namespace Vega.USiteBuilder.VsIntegration
{
    using System.Collections.Generic;

    using EnvDTE;
    using EnvDTE80;

    class VSSearch
    {
        private delegate void CodeElementEnumHandler(CodeElement2 codeElement);
        private delegate bool CodeClassEnumHandler(CodeClass2 codeClass);

        public VSSearch()
        {
        }

        public List<CodeClass2> SearchForClassInCurrentProject(string baseClassFullName)
        {
            List<CodeClass2> retVal = new List<CodeClass2>();

            this.EnumProjects(delegate(CodeElement2 codeElement)
            {
                if (codeElement.Kind == vsCMElement.vsCMElementClass)
                {
                    // we got the class, now check if some of base classes of this class is subclass of a baseClassFullName
                    CodeClass2 codeClass = codeElement as CodeClass2;

                    this.EnumCodeClass(codeClass.Bases, delegate(CodeClass2 baseClassCodeElement)
                    {
                        bool stop = false;

                        try
                        {
                            if (baseClassCodeElement.FullName == baseClassFullName)
                            {
                                retVal.Add(codeClass);
                                stop = true;
                            }
                        }
                        catch { } // Getting FullName if class has errors in it (not compilable is giving Exception.

                        return stop;
                    });
                }
            });

            return retVal;
        }

        private void EnumProjects(CodeElementEnumHandler searchHandler)
        {
            Solution2 solution = Util.GetCurrentSolution();

            if (solution.Projects != null)
            {
                foreach (Project project in solution.Projects)
                {
                    this.EnumProjectItems(project.ProjectItems, searchHandler);
                }
            }
        }

        private void EnumProjectItems(ProjectItems projectItems, CodeElementEnumHandler searchHandler)
        {
            if (projectItems != null)
            {
                foreach (ProjectItem projectItem in projectItems)
                {
                    FileCodeModel2 codeModel = projectItem.FileCodeModel as FileCodeModel2;
                    if (codeModel != null)
                    {
                        this.EnumCodeElements(codeModel.CodeElements, searchHandler);
                    }

                    // enumerate child project items
                    this.EnumProjectItems(projectItem.ProjectItems, searchHandler);
                }
            }
        }

        private void EnumCodeElements(CodeElements codeElements, CodeElementEnumHandler searchHandler)
        {
            if (codeElements != null)
            {
                foreach (CodeElement2 codeElement in codeElements)
                {
                    searchHandler(codeElement);

                    this.EnumCodeElements(codeElement.Children, searchHandler);
                }
            }
        }

        private void EnumCodeClass(CodeElements codeElements, CodeClassEnumHandler searchHandler)
        {
            if (codeElements != null)
            {
                foreach (CodeElement2 codeElement in codeElements)
                {
                    if (codeElement is CodeClass2)
                    {
                        CodeClass2 codeClass = codeElement as CodeClass2;
                        bool stop = searchHandler(codeClass);

                        if (stop)
                        {
                            break;
                        }
                        else
                        {
                            this.EnumCodeClass(codeClass.Bases, searchHandler);
                        }
                    }
                }
            }
        }
    }
}
