namespace CnpValitation
{
    public class CnpValidator
    {

        /// <summary>
        /// Check to see if the cnp aka the romanian security code is valid.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsCnpValid(string? cnpNumber)
        {
            List<int> algorithmKeys = new() { 2, 7, 9, 1, 4, 6, 3, 5, 8, 2, 7, 9 };
            List<int> cnpsDigits = new();
            int controlNumber;

            if (string.IsNullOrEmpty(cnpNumber))
                return false;

            cnpNumber = cnpNumber.Trim();

            // the cnp must have exactly 13 digits
            if (cnpNumber.Length != 13 || !cnpNumber.All(char.IsDigit))
                return false;

            for (int i = 0; i < cnpNumber.Length; i++)
                cnpsDigits.Add(int.Parse(cnpNumber[i].ToString()));

            // it determines the birth century and the sex
            int sex = cnpsDigits[0];
            controlNumber = cnpsDigits[12];

            if (IsSexValid(sex) && IsCnpDateValid(cnpsDigits, sex) && IsCountryAreaValid(cnpsDigits))
            {
                // check if the first 12 digits are according to the control number (the last digit)
                int calculatedControlNr = GetCnpAlgorithmSum(cnpsDigits, algorithmKeys) % 11;

                if (calculatedControlNr == 10)
                    calculatedControlNr = 1;

                if (calculatedControlNr == controlNumber)
                    return true;

            }


            return false;

        }

        private static bool IsCountryAreaValid(List<int> cnpsDigits) => 10 * cnpsDigits[7] + cnpsDigits[8] <= 52;

        private static bool IsSexValid(int sex) => Enumerable.Range(1, 9).Contains(sex);

        /// <summary>
        /// The according 6 digits from the cnp are checked
        /// </summary>
        /// <param name="cnpsDigits"></param>
        /// <param name="sex"></param>
        /// <returns></returns>
        private static bool IsCnpDateValid(List<int> cnpsDigits, int sex)
        {

            var century = GetCentury();

            int year = century[sex][0] + cnpsDigits[1] * 10 + cnpsDigits[2];
            int month = cnpsDigits[3] * 10 + cnpsDigits[4];
            int day = cnpsDigits[5] * 10 + cnpsDigits[6];

            //if the cnp contains a month 13 or day 40 the DateOnly will crash with ArgumentOutOfRangeException
            try
            {
                DateOnly birthDate = new DateOnly(year, month, day);
                return birthDate.Year == year && birthDate.Month == month && birthDate.Day == day;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }


        }


        private static int GetCnpAlgorithmSum(List<int> cnpsDigits, List<int> algorithmKeys)
        {
            int cnpAlgorithmSum = 0;

            for (int i = 0; i <= 11; i++)
                cnpAlgorithmSum += cnpsDigits[i] * algorithmKeys[i];

            return cnpAlgorithmSum;
        }

        private static Dictionary<int, int[]> GetCentury()
        {
            Dictionary<int, int[]> century = new Dictionary<int, int[]>
            {
                { 1, new int[] { 1900, 1999 } },
                { 2, new int[] { 1900, 1999 } },
                { 3, new int[] { 1800, 1899 } },
                { 4, new int[] { 1800, 1899 } },
                { 5, new int[] { 2000, 2099 } },
                { 6, new int[] { 2000, 2099 } },
                { 7, new int[] { 1800, 2099 } },
                { 8, new int[] { 1800, 2099 } },
                { 9, new int[] { 1800, 2099 } }
            };

            return century;
        }
    }
}