namespace NetflixDatasetAPI.Dal
{
    public static class ParseCSVUtils
    {
        public static int PlanDurationToDays(string planDurationValue)
        {
            if (planDurationValue.ToLower().Contains("day") || planDurationValue.ToLower().Contains("days"))
            {
                return int.Parse(planDurationValue.Split(' ')[0]);
            }
            else if (planDurationValue.ToLower().Contains("month") || planDurationValue.ToLower().Contains("months"))
            {
                return int.Parse(planDurationValue.Split(' ')[0]) * 30;
            }
            else if (planDurationValue.ToLower().Contains("year") || planDurationValue.ToLower().Contains("years"))
            {
                return int.Parse(planDurationValue.Split(' ')[0]) * 365;
            }
            else
            {
                return int.Parse(planDurationValue);
            }
        }
    }
}
