namespace Domain
{
    public class Review
    {
        public string Description { get; set; }
        public int Rating { get; set; }

        public Review()
        {
            Description = "";
            Rating = 0;
        }

        public bool IsFieldsFilled()
        {
            return !string.IsNullOrEmpty(Description) && Rating >= 1 && Rating <= 100;
        }
    }
}
