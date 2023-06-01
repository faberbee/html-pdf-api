namespace HtmlPdfApi.Helpers.Config
{
    public class LoadOptions
    {
        public static readonly LoadOptions DEFAULT = new LoadOptions();

        public bool SetEnvVars { get; }
        public bool ClobberExistingVars { get; }
        public bool OnlyExactPath { get; }

        public LoadOptions(
            bool setEnvVars = true,
            bool clobberExistingVars = true,
            bool onlyExactPath = true
        )
        {
            SetEnvVars = setEnvVars;
            ClobberExistingVars = clobberExistingVars;
            OnlyExactPath = onlyExactPath;
        }

        public LoadOptions(
            LoadOptions old,
            bool? setEnvVars = null,
            bool? clobberExistingVars = null,
            bool? onlyExactPath = null
        )
        {
            SetEnvVars = setEnvVars ?? old.SetEnvVars;
            ClobberExistingVars = clobberExistingVars ?? old.ClobberExistingVars;
            OnlyExactPath = onlyExactPath ?? old.OnlyExactPath;
        }
    }
}