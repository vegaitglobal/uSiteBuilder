namespace Vega.USiteBuilder.VsIntegration
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using System.Windows.Forms;

    using EnvDTE;
    using EnvDTE80;

    class Util
    {
        private static void EnumProjectItems(ProjectItem projectItem)
        {
            FileCodeModel2 codeModel = projectItem.FileCodeModel as FileCodeModel2;
            if (codeModel != null)
            {
                foreach (CodeElement2 codeElement in codeModel.CodeElements)
                {
                    EnumCodeElement(codeElement);
                }
            }

            foreach (ProjectItem pi in projectItem.ProjectItems)
            {
                EnumProjectItems(pi);
            }
        }

        private static void EnumCodeElement(CodeElement2 codeElement)
        {
            if (codeElement.Kind == vsCMElement.vsCMElementClass)
            {
                MessageBox.Show(codeElement.Name);
            }

            foreach (CodeElement2 ce2 in codeElement.Children)
            {
                EnumCodeElement(ce2);
            }
        }

        #region [Imports]
        [DllImport("ole32.dll")]
        static extern int GetRunningObjectTable(uint reserved, out IRunningObjectTable pprot);

        [DllImport("ole32.dll")]
        static extern int CreateBindCtx(uint reserved, out IBindCtx pctx);
        #endregion

        /// <summary>
        /// Gets the current solution. VS2008 and VS2010 solutions are currently supported.
        /// </summary>
        /// <returns></returns>
        public static Solution2 GetCurrentSolution()
        {
            return Util.GetDTE2().Solution as Solution2;
        }

        public static DTE2 GetDTE2()
        {
            DTE2 retVal = null;

            // try 2008
            retVal = GetByMoniker("!VisualStudio.DTE.9.0:" + System.Diagnostics.Process.GetCurrentProcess().Id);

            if (retVal == null) // try 2010 if 2008 is not found
            {
                retVal = GetByMoniker("!VisualStudio.DTE.10.0:" + System.Diagnostics.Process.GetCurrentProcess().Id);
            }

            if (retVal == null)
            {
                throw new Exception("This version of visual studio is not supported");
            }

            return retVal;
        }

        private static DTE2 GetByMoniker(string moniker)
        {
            DTE2 retVal = null;

            try
            {
                retVal = Util.SeekDTE2InstanceFromROT(moniker);
            }
            catch { }

            return retVal;
        }

        private static DTE2 SeekDTE2InstanceFromROT(String moniker)
        {
            IRunningObjectTable prot = null;
            IEnumMoniker pmonkenum = null;
            IntPtr pfeteched = IntPtr.Zero;
            DTE2 ret = null;

            try
            {
                if ((GetRunningObjectTable(0, out prot) != 0) || (prot == null)) return ret;

                prot.EnumRunning(out pmonkenum);

                pmonkenum.Reset();

                IMoniker[] monikers = new IMoniker[1];
                while (pmonkenum.Next(1, monikers, pfeteched) == 0)
                {
                    String insname;
                    IBindCtx pctx;

                    CreateBindCtx(0, out pctx);

                    monikers[0].GetDisplayName(pctx, null, out insname);

                    Marshal.ReleaseComObject(pctx);

                    if (string.Compare(insname, moniker) == 0) //lookup by item moniker
                    {
                        Object obj;
                        prot.GetObject(monikers[0], out obj);
                        ret = (DTE2)obj;
                        break;
                    }
                }
            }

            finally
            {
                if (prot != null) Marshal.ReleaseComObject(prot);
                if (pmonkenum != null) Marshal.ReleaseComObject(pmonkenum);
            }

            return ret;
        }
    }
}
