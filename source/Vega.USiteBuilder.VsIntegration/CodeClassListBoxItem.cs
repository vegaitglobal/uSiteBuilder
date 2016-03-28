namespace Vega.USiteBuilder.VsIntegration
{
    using EnvDTE80;

    class CodeClassListBoxItem
    {
        public CodeClass2 CodeClass { get; set; }

        public CodeClassListBoxItem(CodeClass2 codeClass)
        {
            this.CodeClass = codeClass;
        }

        public override string ToString()
        {
            return this.CodeClass.Name;
        }
    }
}
