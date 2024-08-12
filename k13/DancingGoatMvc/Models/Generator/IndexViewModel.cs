namespace DancingGoat.Models.Generator
{
    public class IndexViewModel
    {
        public bool DisplaySuccessMessage
        {
            get;
            set;
        }


        public bool IsAuthorized
        {
            get;
            set;
        } = true;


        public string ABTestErrorMessage
        {
            get;
            internal set;
        }
    }
}